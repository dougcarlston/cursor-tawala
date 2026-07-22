import { QualifiedFieldInput, FieldTextInput } from "@/components/FieldDropInputs";
import type { ConditionCombinator, ConditionRow, IfBuilderState } from "@/lib/statementBuilders";
import { rowsAreValid } from "@/lib/statementBuilders";
import { conditionOpLabel, isUnaryConditionOp } from "@/lib/mcConditionOperators";
import {
  SKIP_OPERATORS,
  SKIP_OPERATOR_LABELS,
} from "@/lib/skipSummary";

export interface IfStatementBuilderProps {
  knownVariables: ReadonlySet<string>;
  state: IfBuilderState;
  onStateChange: (next: IfBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  /** Process MDI window — flatter chrome, no extra panel border (double-line divider below). */
  embedded?: boolean;
}

/**
 * Shared If statement property panel — used by Skip Instructions and Process editor.
 * Legacy copy and layout from `SkipInstructionsDialog` / `DESIGNER_PROCESS_STATEMENTS_IF.md`.
 */
export function IfStatementBuilder({
  knownVariables,
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  embedded = false,
}: IfStatementBuilderProps) {
  const { combinator, rows, hasElse } = state;
  const canSubmit = rowsAreValid(rows, knownVariables);

  const setCombinator = (value: ConditionCombinator) => {
    onStateChange({ ...state, combinator: value });
  };

  const updateRow = (index: number, patch: Partial<ConditionRow>) => {
    onStateChange({
      ...state,
      rows: rows.map((r, i) => (i === index ? { ...r, ...patch } : r)),
    });
  };

  const addRowAfter = (index: number) => {
    const next = [...rows];
    next.splice(index + 1, 0, { field: "", op: "isBlank", value: "" });
    onStateChange({ ...state, rows: next });
  };

  const removeRow = (index: number) => {
    if (rows.length <= 1) return;
    onStateChange({ ...state, rows: rows.filter((_, j) => j !== index) });
  };

  return (
    <div
      className={`skip-statement-panel skip-if-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="skip-statement-panel-tab">If</div>
      <div className="skip-statement-panel-body">
        <p className="skip-if-intro">
          If{" "}
          {rows.length > 1 ? (
            <>
              <select
                value={combinator}
                onChange={(e) => setCombinator(e.target.value as ConditionCombinator)}
                aria-label="Condition combinator"
                className="skip-combinator"
              >
                <option value="and">ALL</option>
                <option value="or">ANY</option>
              </select>{" "}
              of the following conditions are true, execute the first set of commands:
            </>
          ) : (
            <>the following condition is true, execute the first set of commands:</>
          )}
        </p>
        {rows.map((row, i) => (
          <div key={i} className="skip-if-row">
            <QualifiedFieldInput
              className="skip-if-field"
              placeholder="Form:Field"
              knownVariables={knownVariables}
              value={row.field}
              onValueChange={(v) => updateRow(i, { field: v })}
            />
            <select
              value={row.op}
              onChange={(e) => updateRow(i, { op: e.target.value })}
              aria-label="Operator"
              className="skip-if-operator"
            >
              {!SKIP_OPERATORS.includes(row.op) ? (
                <option value={row.op}>{conditionOpLabel(row.op)}</option>
              ) : null}
              {SKIP_OPERATORS.map((op) => (
                <option key={op} value={op}>
                  {SKIP_OPERATOR_LABELS[op]}
                </option>
              ))}
            </select>
            {!isUnaryConditionOp(row.op) ? (
              <FieldTextInput
                className="skip-if-value"
                placeholder="Value"
                value={row.value}
                onValueChange={(v) => updateRow(i, { value: v })}
              />
            ) : (
              <span className="skip-if-value-placeholder" aria-hidden />
            )}
            <button
              type="button"
              className="skip-if-row-btn"
              title="Add condition row"
              onClick={() => addRowAfter(i)}
            >
              +
            </button>
            <button
              type="button"
              className="skip-if-row-btn"
              title="Remove condition row"
              disabled={rows.length <= 1}
              onClick={() => removeRow(i)}
            >
              −
            </button>
          </div>
        ))}
        <label className="skip-if-else">
          <input
            type="checkbox"
            checked={hasElse}
            onChange={(e) => onStateChange({ ...state, hasElse: e.target.checked })}
          />
          Otherwise execute second set of commands
        </label>
        <div className="skip-if-add-row">
          <button type="button" className="skip-add-btn" disabled={!canSubmit} onClick={onSubmit}>
            {submitLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
