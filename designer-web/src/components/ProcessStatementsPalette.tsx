import { Fragment } from "react";
import { useProjectStore } from "@/store/projectStore";
import { PROCESS_STATEMENT_PALETTE } from "@/processStatements";

/**
 * Docked "Processes" palette — the legacy **Statements palette** shown in the same
 * left column as the Items palette when a **Process** window is active (owner Issue 1,
 * July 2026). Buttons append a statement to the active process (`insertProcessCommand`
 * targets store `selection`, which tracks the active MDI window). Grouping/order mirror
 * DESIGNER_PROCESS_STATEMENTS_IF.md.
 *
 * The full per-statement property editors still live inside `ProcessEditor` (the MDI
 * window body); this dock provides the legacy "click a Statements button to add a
 * statement" entry point beside Project Explorer.
 */
export function ProcessStatementsPalette() {
  const insertProcessCommand = useProjectStore((s) => s.insertProcessCommand);
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
            return (
              <Fragment key={def.label}>
                {newGroup && <div className="palette-group-gap" aria-hidden />}
                <button
                  type="button"
                  disabled={disabled}
                  title={
                    disabled
                      ? "Select a process first"
                      : `Insert ${def.label} statement`
                  }
                  onClick={() => insertProcessCommand(def.template)}
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
