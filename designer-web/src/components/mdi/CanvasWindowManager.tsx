import { useEffect, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { CanvasWindow } from "./CanvasWindow";
import {
  hasExplorerEntityDrag,
  hasFormItemDrag,
  hasProcessStatementDrag,
  readExplorerEntityDrag,
} from "@/lib/designerDrag";
import { syncDesignerTargetsToActiveMdiWindow } from "@/lib/fieldInsertion";
import { windowMenuLabel } from "@/lib/mdiWindowLayout";

/**
 * MDI canvas host (backlog §2 Multi-window / MDI). Renders every open window
 * absolutely positioned inside the center canvas, plus a bottom squib strip of
 * minimized windows (legacy: short bars with Restore / Maximize / Close).
 * Accepts Project Explorer entity drops to open/focus windows.
 */
export function CanvasWindowManager() {
  const openWindows = useProjectStore((s) => s.openWindows);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const restoreWindow = useProjectStore((s) => s.restoreWindow);
  const maximizeWindow = useProjectStore((s) => s.maximizeWindow);
  const closeWindow = useProjectStore((s) => s.closeWindow);
  const openWindow = useProjectStore((s) => s.openWindow);
  const [explorerDragOver, setExplorerDragOver] = useState(false);

  const visible = openWindows.filter((w) => !w.minimized);
  const minimized = openWindows.filter((w) => w.minimized);

  // Drop stale Document/Form field + palette targets when the front window changes
  // (Explorer select, title bar, Windows menu) so Insert/Fields never write to a background doc.
  useEffect(() => {
    syncDesignerTargetsToActiveMdiWindow();
  }, [activeWindowId]);

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
          {minimized.map((win) => {
            const label = windowMenuLabel(win.kind, win.name);
            return (
              <div key={win.id} className="mdi-squib" title={label}>
                <button
                  type="button"
                  className="mdi-squib-label"
                  aria-label={`Restore ${label}`}
                  onClick={() => restoreWindow(win.id)}
                >
                  <span className="mdi-squib-icon" aria-hidden>
                    {win.kind === "form" ? "▤" : win.kind === "process" ? "⚙" : "▦"}
                  </span>
                  <span className="mdi-squib-title">{label}</span>
                </button>
                <span className="mdi-squib-controls">
                  <button
                    type="button"
                    className="mdi-control"
                    title="Restore"
                    aria-label={`Restore ${label}`}
                    onClick={() => restoreWindow(win.id)}
                  >
                    ❐
                  </button>
                  <button
                    type="button"
                    className="mdi-control"
                    title="Maximize"
                    aria-label={`Maximize ${label}`}
                    onClick={() => maximizeWindow(win.id)}
                  >
                    □
                  </button>
                  <button
                    type="button"
                    className="mdi-control mdi-control-close"
                    title="Close"
                    aria-label={`Close ${label}`}
                    onClick={() => closeWindow(win.id)}
                  >
                    ×
                  </button>
                </span>
              </div>
            );
          })}
        </div>
      )}
    </div>
  );
}
