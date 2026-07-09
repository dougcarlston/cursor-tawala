import { Fragment } from "react";
import { useProjectStore } from "@/store/projectStore";
import {
  PROCESS_PANEL_LABELS,
  PROCESS_STATEMENT_PALETTE,
  processPanelKeyForLabel,
} from "@/processStatements";

/**
 * Docked "Processes" palette — the legacy **Statements palette** shown in the same
 * left column as the Items palette when a **Process** window is active (owner Issue 1,
 * July 2026). **If** and **Set** open property panels in the process window (legacy:
 * palette selects statement type); other buttons insert a template at the insertion arrow.
 */
export function ProcessStatementsPalette() {
  const insertProcessCommand = useProjectStore((s) => s.insertProcessCommand);
  const toggleProcessStatementPanel = useProjectStore((s) => s.toggleProcessStatementPanel);
  const processStatementPanel = useProjectStore((s) => s.processStatementPanel);
  const selection = useProjectStore((s) => s.selection);
  const disabled = selection.kind !== "process" || !selection.name;

  return (
    <>
      <div className="panel-title">Processes</div>
      <div className="items-palette-body">
        <div className="form-insert-buttons">
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
                  disabled={disabled}
                  title={
                    disabled
                      ? "Select a process first"
                      : PROCESS_PANEL_LABELS.has(def.label)
                        ? `Configure ${def.label} statement`
                        : `Insert ${def.label} statement`
                  }
                  onClick={() => {
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
