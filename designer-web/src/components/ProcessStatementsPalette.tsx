import { Fragment } from "react";
import { useProjectStore } from "@/store/projectStore";
import {
  PROCESS_PANEL_LABELS,
  PROCESS_STATEMENT_PALETTE,
  processPanelKeyForLabel,
} from "@/processStatements";
import { setProcessStatementDrag } from "@/lib/designerDrag";

/**
 * Docked "Statements" palette — legacy process toolbox in the same left column as
 * Items when a Process window is active. Click opens configure panels / inserts;
 * drag onto a Process window does the same.
 */
export function ProcessStatementsPalette() {
  const insertProcessCommand = useProjectStore((s) => s.insertProcessCommand);
  const toggleProcessStatementPanel = useProjectStore((s) => s.toggleProcessStatementPanel);
  const processStatementPanel = useProjectStore((s) => s.processStatementPanel);
  const selection = useProjectStore((s) => s.selection);
  const processInactive = selection.kind !== "process" || !selection.name;

  return (
    <>
      <div className="items-palette-title">Statements</div>
      <div className="items-palette-body statements-palette-body">
        <div className="form-insert-buttons statements-palette-buttons">
          {PROCESS_STATEMENT_PALETTE.map((def, i) => {
            const prev = PROCESS_STATEMENT_PALETTE[i - 1];
            const newGroup = prev != null && prev.group !== def.group;
            const panelKey = processPanelKeyForLabel(def.label);
            const active = panelKey != null && processStatementPanel === panelKey;
            return (
              <Fragment key={def.label}>
                {newGroup && <div className="palette-group-gap" aria-hidden />}
                <button
                  type="button"
                  className={active ? "active" : ""}
                  title={
                    processInactive
                      ? `Drag onto a Process window to configure ${def.label}`
                      : PROCESS_PANEL_LABELS.has(def.label)
                        ? `Configure ${def.label} statement (or drag onto a Process window)`
                        : `Insert ${def.label} statement`
                  }
                  draggable
                  onDragStart={(e) => {
                    setProcessStatementDrag(e.dataTransfer, def.label);
                  }}
                  onClick={() => {
                    if (processInactive) return;
                    if (PROCESS_PANEL_LABELS.has(def.label)) {
                      toggleProcessStatementPanel(def.label);
                    } else {
                      insertProcessCommand(def.template);
                    }
                  }}
                >
                  {def.label}
                </button>
              </Fragment>
            );
          })}
        </div>
      </div>
    </>
  );
}
