import { useRef, useState, type ReactNode } from "react";
import { useProjectStore, type DesignerWindow, type WindowKind } from "@/store/projectStore";
import { FormEditor } from "../FormEditor";
import { ProcessEditor } from "../ProcessEditor";
import { DocumentEditor } from "../DocumentEditor";
import {
  hasExplorerEntityDrag,
  hasFormItemDrag,
  hasProcessStatementDrag,
  readExplorerEntityDrag,
  readFormItemDrag,
  readProcessStatementDrag,
} from "@/lib/designerDrag";
import {
  PROCESS_PANEL_LABELS,
  PROCESS_STATEMENT_PALETTE,
} from "@/processStatements";

/** Minimum interactive size so a window never collapses past its chrome. */
const MIN_W = 320;
const MIN_H = 180;

const KIND_LABEL: Record<WindowKind, string> = {
  form: "Form",
  process: "Process",
  document: "Document",
};

/** Resize handle directions (edges + corners). */
type ResizeDir = "n" | "s" | "e" | "w" | "ne" | "nw" | "se" | "sw";
const RESIZE_DIRS: ResizeDir[] = ["n", "s", "e", "w", "ne", "nw", "se", "sw"];

interface Props {
  win: DesignerWindow;
  active: boolean;
}

/**
 * A single MDI child window on the designer canvas (backlog §2, Pass 1). Draggable
 * by its title bar, resizable from all edges/corners, click-to-front, with
 * minimize / close controls. Accepts Items/Statements drops when kind matches.
 */
export function CanvasWindow({ win, active }: Props) {
  const frameRef = useRef<HTMLDivElement>(null);
  const focusWindow = useProjectStore((s) => s.focusWindow);
  const closeWindow = useProjectStore((s) => s.closeWindow);
  const minimizeWindow = useProjectStore((s) => s.minimizeWindow);
  const setWindowBounds = useProjectStore((s) => s.setWindowBounds);
  const openWindow = useProjectStore((s) => s.openWindow);
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const insertProcessCommand = useProjectStore((s) => s.insertProcessCommand);
  const toggleProcessStatementPanel = useProjectStore((s) => s.toggleProcessStatementPanel);
  const project = useProjectStore((s) => s.project);
  const [paletteDropOver, setPaletteDropOver] = useState(false);

  const parentEl = () => frameRef.current?.parentElement ?? null;

  const startDrag = (e: React.PointerEvent) => {
    if (e.button !== 0) return;
    focusWindow(win.id);
    const startX = e.clientX;
    const startY = e.clientY;
    const origin = { x: win.x, y: win.y };
    const parent = parentEl();
    const onMove = (ev: PointerEvent) => {
      let nx = origin.x + (ev.clientX - startX);
      let ny = origin.y + (ev.clientY - startY);
      if (parent) {
        const maxX = Math.max(0, parent.clientWidth - 80);
        const maxY = Math.max(0, parent.clientHeight - 28);
        nx = Math.max(0, Math.min(nx, maxX));
        ny = Math.max(0, Math.min(ny, maxY));
      }
      setWindowBounds(win.id, { x: nx, y: ny });
    };
    endableDrag(onMove, "move");
  };

  const startResize = (dir: ResizeDir) => (e: React.PointerEvent) => {
    if (e.button !== 0) return;
    e.stopPropagation();
    focusWindow(win.id);
    const startX = e.clientX;
    const startY = e.clientY;
    const b = { x: win.x, y: win.y, w: win.w, h: win.h };
    const onMove = (ev: PointerEvent) => {
      const dx = ev.clientX - startX;
      const dy = ev.clientY - startY;
      let { x, y, w, h } = b;
      if (dir.includes("e")) w = Math.max(MIN_W, b.w + dx);
      if (dir.includes("s")) h = Math.max(MIN_H, b.h + dy);
      if (dir.includes("w")) {
        w = Math.max(MIN_W, b.w - dx);
        x = b.x + (b.w - w);
      }
      if (dir.includes("n")) {
        h = Math.max(MIN_H, b.h - dy);
        y = b.y + (b.h - h);
      }
      setWindowBounds(win.id, { x, y, w, h });
    };
    endableDrag(onMove, resizeCursor(dir));
  };

  return (
    <div
      ref={frameRef}
      className={`mdi-window${active ? " active" : ""}${paletteDropOver ? " mdi-window-drop-active" : ""}`}
      style={{ left: win.x, top: win.y, width: win.w, height: win.h, zIndex: win.z }}
      onPointerDown={() => {
        if (!active) focusWindow(win.id);
      }}
      onDragOver={(e) => {
        if (hasExplorerEntityDrag(e.dataTransfer)) {
          e.preventDefault();
          e.dataTransfer.dropEffect = "copy";
          return;
        }
        if (win.kind === "form" && hasFormItemDrag(e.dataTransfer)) {
          e.preventDefault();
          e.stopPropagation();
          e.dataTransfer.dropEffect = "copy";
          if (!paletteDropOver) setPaletteDropOver(true);
          return;
        }
        if (win.kind === "process" && hasProcessStatementDrag(e.dataTransfer)) {
          e.preventDefault();
          e.stopPropagation();
          e.dataTransfer.dropEffect = "copy";
          if (!paletteDropOver) setPaletteDropOver(true);
        }
      }}
      onDragLeave={(e) => {
        if (!e.currentTarget.contains(e.relatedTarget as Node)) {
          setPaletteDropOver(false);
        }
      }}
      onDrop={(e) => {
        setPaletteDropOver(false);
        const entity = readExplorerEntityDrag(e.dataTransfer);
        if (entity) {
          e.preventDefault();
          e.stopPropagation();
          openWindow(entity.kind, entity.name);
          return;
        }
        if (win.kind === "form") {
          const itemType = readFormItemDrag(e.dataTransfer);
          if (itemType) {
            // Form canvas / insertion points own positioned drops. Only append when
            // the drop lands on window chrome (title bar, borders, etc.).
            if ((e.target as HTMLElement).closest(".form-canvas")) return;
            e.preventDefault();
            e.stopPropagation();
            openWindow("form", win.name);
            const form = project.forms.find((f) => f.name === win.name);
            insertFormItem(itemType, {
              formName: win.name,
              beforeIndex: form?.items.length ?? 0,
            });
            return;
          }
        }
        if (win.kind === "process") {
          const label = readProcessStatementDrag(e.dataTransfer);
          if (label) {
            // Script area owns position-aware drops (caret). Chrome-only drops insert
            // at the stored insertion point.
            if ((e.target as HTMLElement).closest(".process-script-scroll")) return;
            e.preventDefault();
            e.stopPropagation();
            openWindow("process", win.name);
            if (PROCESS_PANEL_LABELS.has(label)) {
              toggleProcessStatementPanel(label);
            } else {
              const def = PROCESS_STATEMENT_PALETTE.find((d) => d.label === label);
              if (def) insertProcessCommand(def.template);
            }
          }
        }
      }}
      role="dialog"
      aria-label={`${KIND_LABEL[win.kind]} - ${win.name}`}
    >
      <div className="mdi-titlebar" onPointerDown={startDrag}>
        <span className="mdi-title-icon" aria-hidden>
          {win.kind === "form" ? "▤" : win.kind === "process" ? "⚙" : "▦"}
        </span>
        <span className="mdi-title-text">
          {KIND_LABEL[win.kind]} - {win.name}
        </span>
        <span className="mdi-controls">
          <button
            type="button"
            className="mdi-control"
            title="Minimize"
            aria-label={`Minimize ${win.name}`}
            onPointerDown={(e) => e.stopPropagation()}
            onClick={() => minimizeWindow(win.id)}
          >
            _
          </button>
          <button
            type="button"
            className="mdi-control mdi-control-close"
            title="Close"
            aria-label={`Close ${win.name}`}
            onPointerDown={(e) => e.stopPropagation()}
            onClick={() => closeWindow(win.id)}
          >
            ×
          </button>
        </span>
      </div>
      <div className="mdi-body">
        <WindowBody kind={win.kind} name={win.name} />
      </div>
      {RESIZE_DIRS.map((dir) => (
        <div
          key={dir}
          className={`mdi-resize mdi-resize-${dir}`}
          onPointerDown={startResize(dir)}
        />
      ))}
    </div>
  );
}

function WindowBody({ kind, name }: { kind: WindowKind; name: string }): ReactNode {
  switch (kind) {
    case "form":
      return <FormEditor formName={name} />;
    case "process":
      return <ProcessEditor processName={name} />;
    case "document":
      return <DocumentEditor documentName={name} />;
    default:
      return null;
  }
}

/** Attach global pointer listeners for a drag/resize gesture and clean up on release. */
function endableDrag(onMove: (ev: PointerEvent) => void, cursor: string) {
  const prevCursor = document.body.style.cursor;
  const prevSelect = document.body.style.userSelect;
  document.body.style.cursor = cursor;
  document.body.style.userSelect = "none";
  const onUp = () => {
    window.removeEventListener("pointermove", onMove);
    window.removeEventListener("pointerup", onUp);
    document.body.style.cursor = prevCursor;
    document.body.style.userSelect = prevSelect;
  };
  window.addEventListener("pointermove", onMove);
  window.addEventListener("pointerup", onUp);
}

function resizeCursor(dir: ResizeDir): string {
  switch (dir) {
    case "n":
    case "s":
      return "ns-resize";
    case "e":
    case "w":
      return "ew-resize";
    case "ne":
    case "sw":
      return "nesw-resize";
    case "nw":
    case "se":
      return "nwse-resize";
  }
}
