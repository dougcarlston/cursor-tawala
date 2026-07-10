import { FieldTextInput } from "./FieldDropInputs";
import {
  DEFAULT_FUNCTION_CONDITIONS,
  functionConditionsRowIsComplete,
  type FunctionConditionRow,
  type FunctionConditionsState,
} from "@/lib/functionConditions";
import { SKIP_OPERATORS, SKIP_OPERATOR_LABELS, UNARY_SKIP_OPERATORS } from "@/lib/skipSummary";

interface Props {
  /** Parameter label from catalog (e.g. "Count only the records"). */
  paramName: string;
  state: FunctionConditionsState;
  onChange: (next: FunctionConditionsState) => void;
  onFocus?: () => void;
}

/**
 * Legacy `ConditionListControl` — "Limit output to records where" + field / operator / value rows.
 */
export function FunctionConditionsEditor({ paramName, state, onChange, onFocus }: Props) {
  const { combinator, rows } = state;
  const multi = rows.length > 1;

  const patchRow = (index: number, patch: Partial<FunctionConditionRow>) => {
    onChange({
      ...state,
      rows: rows.map((r, i) => (i === index ? { ...r, ...patch } : r)),
    });
  };

  const addRowAfter = (index: number) => {
    const next = [...rows];
    next.splice(index + 1, 0, { field: "", op: "equals", value: "" });
    onChange({ ...state, rows: next });
  };

  const removeRow = (index: number) => {
    if (rows.length <= 1) {
      onChange({ ...DEFAULT_FUNCTION_CONDITIONS, rows: [{ field: "", op: "equals", value: "" }] });
      return;
    }
    onChange({ ...state, rows: rows.filter((_, i) => i !== index) });
  };

  return (
    <div className="function-conditions-editor" onFocus={onFocus}>
      <div className="function-conditions-header">
        <span className="function-conditions-where">Limit output to records where</span>
        {multi && (
          <>
            <select
              className="function-conditions-combinator"
              value={combinator}
              aria-label="Condition combinator"
              onChange={(e) =>
                onChange({
                  ...state,
                  combinator: e.target.value as FunctionConditionsState["combinator"],
                })
              }
            >
              <option value="and">ALL</option>
              <option value="or">ANY</option>
            </select>
            <span className="function-conditions-where-tail">of the following are true:</span>
          </>
        )}
      </div>
      <p className="function-conditions-param-name">{paramName}</p>
      {rows.map((row, i) => (
        <div key={i} className="skip-if-row function-conditions-row">
          <FieldTextInput
            className="skip-if-field"
            placeholder="Record:Form:Field"
            value={row.field}
            onFocus={onFocus}
            onValueChange={(v) => patchRow(i, { field: v })}
          />
          <select
            className="skip-if-operator"
            value={row.op}
            aria-label="Operator"
            onFocus={onFocus}
            onChange={(e) => patchRow(i, { op: e.target.value })}
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
              onFocus={onFocus}
              onValueChange={(v) => patchRow(i, { value: v })}
            />
          ) : (
            <span className="skip-if-value-placeholder" aria-hidden />
          )}
          <button
            type="button"
            className="skip-if-row-btn"
            title="Add condition row"
            disabled={!functionConditionsRowIsComplete(row)}
            onClick={() => addRowAfter(i)}
          >
            +
          </button>
          <button
            type="button"
            className="skip-if-row-btn"
            title="Remove condition row"
            onClick={() => removeRow(i)}
          >
            −
          </button>
        </div>
      ))}
    </div>
  );
}
