import { useEffect } from "react";
import { QualifiedFieldInput, FieldTextInput } from "@/components/FieldDropInputs";
import { setFieldsPaletteConditionsForm } from "@/lib/fieldsPaletteContext";
import type {
  ConditionCombinator,
  ConditionRow,
  RemoveDuplicatesBuilderState,
  RemoveDuplicatesKeep,
} from "@/lib/statementBuilders";
import {
  EMPTY_CONDITION_ROW,
  getWhereIsValid,
  removeDuplicatesBuilderIsValid,
} from "@/lib/statementBuilders";
import {
  SKIP_OPERATORS,
  SKIP_OPERATOR_LABELS,
  UNARY_SKIP_OPERATORS,
} from "@/lib/skipSummary";

export interface RemoveDuplicatesStatementBuilderProps {
  state: RemoveDuplicatesBuilderState;
  onStateChange: (next: RemoveDuplicatesBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  onCancel?: () => void;
  formNames: readonly string[];
  knownVariables: ReadonlySet<string>;
  embedded?: boolean;
}

/**
 * Remove Duplicates — delete submissions sharing a key field, keep first or latest.
 * Same form + optional Where pattern as Delete; plus key field and keep mode.
 */
export function RemoveDuplicatesStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  onCancel,
  formNames,
  knownVariables,
  embedded = false,
}: RemoveDuplicatesStatementBuilderProps) {
  const whereValid = getWhereIsValid(state.whereRows, knownVariables);
  const canSubmit =
    removeDuplicatesBuilderIsValid(state, formNames, knownVariables) && whereValid;

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
      <div className="show-statement-tabs" role="tablist" aria-label="Remove Duplicates statement">
        <button type="button" role="tab" aria-selected className="active">
          Remove Duplicates
        </button>
      </div>
      <div className="skip-statement-panel-body delete-statement-panel-body">
        <div className="delete-header-row">
          <span className="delete-from-label">records from form:</span>
          <select
            id="remove-duplicates-source-form"
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

        <div className="delete-header-row">
          <span className="delete-from-label">key field:</span>
          <QualifiedFieldInput
            className="skip-if-field"
            placeholder="Record:Form:PlayerID"
            knownVariables={knownVariables}
            value={state.field}
            onValueChange={(v) => onStateChange({ ...state, field: v })}
          />
        </div>

        <div className="delete-header-row">
          <span className="delete-from-label">keep:</span>
          <select
            className="delete-form-select"
            value={state.keep}
            onChange={(e) =>
              onStateChange({
                ...state,
                keep: e.target.value as RemoveDuplicatesKeep,
              })
            }
            aria-label="Keep duplicate"
          >
            <option value="latest">most recently added</option>
            <option value="first">first added</option>
          </select>
        </div>

        <div className="delete-where-group">
          <div className="delete-where-label">Where (optional)</div>
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
          {onCancel ? (
            <button type="button" className="skip-cancel-btn" onClick={onCancel}>
              Cancel
            </button>
          ) : null}
        </div>
      </div>
    </div>
  );
}
