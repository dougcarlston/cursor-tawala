import { QualifiedFieldInput, FieldTextInput } from "@/components/FieldDropInputs";
import type { SetBuilderState } from "@/lib/statementBuilders";
import { expressionHasArithmetic, setBuilderIsValid } from "@/lib/statementBuilders";

export interface SetStatementBuilderProps {
  state: SetBuilderState;
  onStateChange: (next: SetBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  knownVariables?: ReadonlySet<string>;
  embedded?: boolean;
}

/**
 * Shared Set statement property panel — used by Skip Instructions and Process editor.
 */
export function SetStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  knownVariables,
  embedded = false,
}: SetStatementBuilderProps) {
  const { field, value, arithmeticAsText } = state;
  const arithmeticEnabled = expressionHasArithmetic(value);
  const canSubmit = setBuilderIsValid(state);

  const setValue = (nextValue: string) => {
    onStateChange({
      ...state,
      value: nextValue,
      arithmeticAsText: expressionHasArithmetic(nextValue) ? state.arithmeticAsText : false,
    });
  };

  return (
    <div
      className={`skip-statement-panel skip-set-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="skip-statement-panel-tab">Set</div>
      <div className="skip-statement-panel-body skip-set-body">
        <div className="skip-set-row">
          <QualifiedFieldInput
            className="skip-set-field"
            placeholder="Form:Field or Variable Name"
            knownVariables={knownVariables}
            value={field}
            onValueChange={(v) => onStateChange({ ...state, field: v })}
          />
          <span className="skip-set-to">to</span>
          <FieldTextInput
            className="skip-set-expression"
            placeholder="Value or expression"
            value={value}
            onValueChange={setValue}
          />
        </div>
        <label className={`skip-set-arithmetic${arithmeticEnabled ? "" : " disabled"}`}>
          <input
            type="checkbox"
            checked={arithmeticAsText}
            disabled={!arithmeticEnabled}
            onChange={(e) => onStateChange({ ...state, arithmeticAsText: e.target.checked })}
          />
          Treat arithmetic expression as text (do not interpret +, -, * or / as math)
        </label>
        <div className="skip-set-add-row">
          <button type="button" className="skip-add-btn" disabled={!canSubmit} onClick={onSubmit}>
            {submitLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
