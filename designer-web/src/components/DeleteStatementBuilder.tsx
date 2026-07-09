import { useEffect } from "react";
import { QualifiedFieldInput, FieldTextInput } from "@/components/FieldDropInputs";
import { setFieldsPaletteConditionsForm } from "@/lib/fieldsPaletteContext";
import type { ConditionCombinator, ConditionRow, DeleteBuilderState } from "@/lib/statementBuilders";
import {
  EMPTY_CONDITION_ROW,
  deleteBuilderIsValid,
  getWhereIsValid,
} from "@/lib/statementBuilders";
import {
  SKIP_OPERATORS,
  SKIP_OPERATOR_LABELS,
  UNARY_SKIP_OPERATORS,
} from "@/lib/skipSummary";

export interface DeleteStatementBuilderProps {
  state: DeleteBuilderState;
  onStateChange: (next: DeleteBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  formNames: readonly string[];
  knownVariables: ReadonlySet<string>;
  embedded?: boolean;
}

/**
 * Delete statement property panel per `DESIGNER_PROCESS_STATEMENTS_APPEND_GET_ETC.md`.
 * records from form + optional Where rows (same controls as Get / If).
 */
export function DeleteStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  formNames,
  knownVariables,
  embedded = false,
}: DeleteStatementBuilderProps) {
  const whereValid = getWhereIsValid(state.whereRows, knownVariables);
  const canSubmit = deleteBuilderIsValid(state, formNames, knownVariables) && whereValid;

  useEffect(() => {
    if (!embedded) return;
    if (state.sourceForm.trim()) {
      setFieldsPaletteConditionsForm(state.sourceForm);
      return () => setFieldsPaletteConditionsForm(null);
    }
    setFieldsPaletteConditionsForm(null);
    return () => setFieldsPaletteConditionsForm(null);
  }, [embedded, state.sourceForm]);

  const updateWhereRow = (index: number, patch: Partial<ConditionRow>) => {
    onStateChange({
      ...state,
      whereRows: state.whereRows.map((r, i) => (i === index ? { ...r, ...patch } : r)),
    });
  };

  const addWhereRowAfter = (index: number) => {
    const next = [...state.whereRows];
    next.splice(index + 1, 0, { ...EMPTY_CONDITION_ROW });
    onStateChange({ ...state, whereRows: next });
  };

  const removeWhereRow = (index: number) => {
    if (state.whereRows.length <= 1) return;
    onStateChange({ ...state, whereRows: state.whereRows.filter((_, j) => j !== index) });
  };

  const setWhereCombinator = (value: ConditionCombinator) => {
    onStateChange({ ...state, whereCombinator: value });
  };

  return (
    <div
      className={`skip-statement-panel skip-delete-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="show-statement-tabs" role="tablist" aria-label="Delete statement type">
        <button type="button" role="tab" aria-selected className="active">
          Delete
        </button>
      </div>
      <div className="skip-statement-panel-body delete-statement-panel-body">
        <div className="delete-header-row">
          <span className="delete-from-label">records from form:</span>
          <select
            id="delete-source-form"
            className="delete-form-select"
            value={state.sourceForm}
            onChange={(e) => onStateChange({ ...state, sourceForm: e.target.value })}
            aria-label="Source form"
          >
            <option value="">— select —</option>
            {formNames.map((name) => (
              <option key={name} value={name}>
                {name}
              </option>
            ))}
          </select>
        </div>

        <div className="delete-where-group">
          <div className="delete-where-label">Where</div>
          {state.whereRows.length > 1 && (
            <p className="skip-if-intro delete-where-intro">
              Match records where{" "}
              <select
                value={state.whereCombinator}
                onChange={(e) => setWhereCombinator(e.target.value as ConditionCombinator)}
                aria-label="Where combinator"
                className="skip-combinator"
              >
                <option value="and">ALL</option>
                <option value="or">ANY</option>
              </select>{" "}
              of the following are true:
            </p>
          )}
          {state.whereRows.map((row, i) => (
            <div key={i} className="skip-if-row">
              <QualifiedFieldInput
                className="skip-if-field"
                placeholder="Record:Form:Field"
                knownVariables={knownVariables}
                value={row.field}
                onValueChange={(v) => updateWhereRow(i, { field: v })}
              />
              <select
                value={row.op}
                onChange={(e) => updateWhereRow(i, { op: e.target.value })}
                aria-label="Operator"
                className="skip-if-operator"
              >
                {SKIP_OPERATORS.map((op) => (
                  <option key={op} value={op}>
                    {SKIP_OPERATOR_LABELS[op]}
                  </option>
                ))}
              </select>
              {!UNARY_SKIP_OPERATORS.has(row.op) ? (
                <FieldTextInput
                  className="skip-if-value"
                  placeholder="Value"
                  value={row.value}
                  onValueChange={(v) => updateWhereRow(i, { value: v })}
                />
              ) : (
                <span className="skip-if-value-placeholder" aria-hidden />
              )}
              <button
                type="button"
                className="skip-if-row-btn"
                title="Add condition row"
                onClick={() => addWhereRowAfter(i)}
              >
                +
              </button>
              <button
                type="button"
                className="skip-if-row-btn"
                title="Remove condition row"
                disabled={state.whereRows.length <= 1}
                onClick={() => removeWhereRow(i)}
              >
                −
              </button>
            </div>
          ))}
        </div>

        <div className="send-add-row">
          <button type="button" className="skip-add-btn" disabled={!canSubmit} onClick={onSubmit}>
            {submitLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
