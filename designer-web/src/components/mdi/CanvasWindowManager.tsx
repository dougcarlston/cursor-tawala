import { useProjectStore } from "@/store/projectStore";
import { CanvasWindow } from "./CanvasWindow";

const KIND_LABEL = { form: "Form", process: "Process", document: "Document" } as const;

/**
 * MDI canvas host (backlog §2 Multi-window / MDI, Pass 1). Renders every open
 * window absolutely positioned inside the center canvas, plus a taskbar of
 * minimized windows. Windows open from the Project Explorer (see `openWindow`).
 */
export function CanvasWindowManager() {
  const openWindows = useProjectStore((s) => s.openWindows);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const restoreWindow = useProjectStore((s) => s.restoreWindow);

  const visible = openWindows.filter((w) => !w.minimized);
  const minimized = openWindows.filter((w) => w.minimized);

  return (
    <div className="mdi-canvas">
      <div className="mdi-surface">
        {visible.length === 0 && minimized.length === 0 ? (
          <div className="placeholder-editor">
            Select a form, process, or document in Project Explorer to open a window
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
