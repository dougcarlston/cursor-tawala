import { useRef, type ReactNode } from "react";
import { useProjectStore, type DesignerWindow, type WindowKind } from "@/store/projectStore";
import { FormEditor } from "../FormEditor";
import { ProcessEditor } from "../ProcessEditor";
import { DocumentEditor } from "../DocumentEditor";

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
 * minimize / close controls. The body embeds the real single-pane editor for the
 * window's entity (form / process / document).
 */
export function CanvasWindow({ win, active }: Props) {
  const frameRef = useRef<HTMLDivElement>(null);
  const focusWindow = useProjectStore((s) => s.focusWindow);
  const closeWindow = useProjectStore((s) => s.closeWindow);
  const minimizeWindow = useProjectStore((s) => s.minimizeWindow);
  const setWindowBounds = useProjectStore((s) => s.setWindowBounds);

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
      className={`mdi-window${active ? " active" : ""}`}
      style={{ left: win.x, top: win.y, width: win.w, height: win.h, zIndex: win.z }}
      onPointerDown={() => {
        if (!active) focusWindow(win.id);
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
