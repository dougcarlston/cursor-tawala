import { useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { CanvasWindow } from "./CanvasWindow";
import {
  hasExplorerEntityDrag,
  hasFormItemDrag,
  hasProcessStatementDrag,
  readExplorerEntityDrag,
} from "@/lib/designerDrag";

const KIND_LABEL = { form: "Form", process: "Process", document: "Document" } as const;

/**
 * MDI canvas host (backlog §2 Multi-window / MDI, Pass 1). Renders every open
 * window absolutely positioned inside the center canvas, plus a taskbar of
 * minimized windows. Accepts Project Explorer entity drops to open/focus windows.
 */
export function CanvasWindowManager() {
  const openWindows = useProjectStore((s) => s.openWindows);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const restoreWindow = useProjectStore((s) => s.restoreWindow);
  const openWindow = useProjectStore((s) => s.openWindow);
  const [explorerDragOver, setExplorerDragOver] = useState(false);

  const visible = openWindows.filter((w) => !w.minimized);
  const minimized = openWindows.filter((w) => w.minimized);

  return (
    <div className="mdi-canvas">
      <div
        className={`mdi-surface${explorerDragOver ? " mdi-surface-drop-active" : ""}`}
        onDragOver={(e) => {
          if (hasFormItemDrag(e.dataTransfer) || hasProcessStatementDrag(e.dataTransfer)) {
            // Form/process drops are handled on the matching window frames.
            return;
          }
          if (!hasExplorerEntityDrag(e.dataTransfer)) return;
          e.preventDefault();
          e.dataTransfer.dropEffect = "copy";
          if (!explorerDragOver) setExplorerDragOver(true);
        }}
        onDragLeave={(e) => {
          if (e.currentTarget === e.target) setExplorerDragOver(false);
        }}
        onDrop={(e) => {
          setExplorerDragOver(false);
          const entity = readExplorerEntityDrag(e.dataTransfer);
          if (!entity) return;
          e.preventDefault();
          e.stopPropagation();
          openWindow(entity.kind, entity.name);
        }}
      >
        {visible.length === 0 && minimized.length === 0 ? (
          <div className="placeholder-editor">
            Select or drag a form, process, or document from Project Explorer to open a window
          </div>
        ) : (
          visible.map((win) => (
            <CanvasWindow key={win.id} win={win} active={win.id === activeWindowId} />
          ))
        )}
      </div>
      {minimized.length > 0 && (
        <div className="mdi-taskbar" role="toolbar" aria-label="Minimized windows">
          {minimized.map((win) => (
            <button
              key={win.id}
              type="button"
              className="mdi-taskbar-item"
              title={`Restore ${KIND_LABEL[win.kind]} - ${win.name}`}
              onClick={() => restoreWindow(win.id)}
            >
              <span className="mdi-taskbar-icon" aria-hidden>
                {win.kind === "form" ? "▤" : win.kind === "process" ? "⚙" : "▦"}
              </span>
              {KIND_LABEL[win.kind]} - {win.name}
            </button>
          ))}
        </div>
      )}
    </div>
  );
}
