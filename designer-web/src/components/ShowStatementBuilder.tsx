import { QualifiedFieldInput, FieldTextInput } from "@/components/FieldDropInputs";
import type {
  ConditionCombinator,
  ConditionRow,
  ShowBuilderState,
  ShowTab,
} from "@/lib/statementBuilders";
import { rowsAreValid, showBuilderIsValid } from "@/lib/statementBuilders";
import {
  SKIP_OPERATORS,
  SKIP_OPERATOR_LABELS,
  UNARY_SKIP_OPERATORS,
} from "@/lib/skipSummary";

const SHOW_TABS: { key: ShowTab; label: string }[] = [
  { key: "document", label: "Document" },
  { key: "form", label: "Form" },
  { key: "storedRecord", label: "Stored Record" },
  { key: "url", label: "URL" },
];

export interface ShowStatementBuilderProps {
  state: ShowBuilderState;
  onStateChange: (next: ShowBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  documentNames: readonly string[];
  formNames: readonly string[];
  knownVariables: ReadonlySet<string>;
  embedded?: boolean;
}

/**
 * Show statement property panel — four tabs per `DESIGNER_PROCESS_STATEMENTS_SHOW.md`.
 * Used by the Process editor (not Skip Instructions).
 */
export function ShowStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  documentNames,
  formNames,
  knownVariables,
  embedded = false,
}: ShowStatementBuilderProps) {
  const { tab } = state;
  const canSubmit = showBuilderIsValid(state, knownVariables, documentNames, formNames);

  const setTab = (nextTab: ShowTab) => {
    onStateChange({ ...state, tab: nextTab });
  };

  const updateRecordRow = (index: number, patch: Partial<ConditionRow>) => {
    onStateChange({
      ...state,
      recordRows: state.recordRows.map((r, i) => (i === index ? { ...r, ...patch } : r)),
    });
  };

  const addRecordRowAfter = (index: number) => {
    const next = [...state.recordRows];
    next.splice(index + 1, 0, { field: "", op: "equals", value: "" });
    onStateChange({ ...state, recordRows: next });
  };

  const removeRecordRow = (index: number) => {
    if (state.recordRows.length <= 1) return;
    onStateChange({ ...state, recordRows: state.recordRows.filter((_, j) => j !== index) });
  };

  const setRecordCombinator = (value: ConditionCombinator) => {
    onStateChange({ ...state, recordCombinator: value });
  };

  const recordWhereValid =
    !state.recordRows.some((r) => r.field.trim()) ||
    rowsAreValid(state.recordRows, knownVariables);

  return (
    <div
      className={`skip-statement-panel skip-show-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="skip-statement-panel-tab">Show</div>
      <div className="show-statement-tabs" role="tablist" aria-label="Show statement type">
        {SHOW_TABS.map(({ key, label }) => (
          <button
            key={key}
            type="button"
            role="tab"
            aria-selected={tab === key}
            className={tab === key ? "active" : ""}
            onClick={() => setTab(key)}
          >
            {label}
          </button>
        ))}
      </div>
      <div className="skip-statement-panel-body show-statement-panel-body">
        {tab === "document" && (
          <div className="show-tab-panel">
            <label className="show-field-label">
              Document
              <select
                value={state.document}
                onChange={(e) => onStateChange({ ...state, document: e.target.value })}
                aria-label="Document to show"
              >
                <option value="">— select —</option>
                {documentNames.map((name) => (
                  <option key={name} value={name}>
                    {name}
                  </option>
                ))}
              </select>
            </label>
            <label className="show-reset-checkbox">
              <input
                type="checkbox"
                checked={state.documentReset}
                onChange={(e) => onStateChange({ ...state, documentReset: e.target.checked })}
              />
              Reset document to original state after Show
              <span className="show-reset-sublabel">(removes any appended documents)</span>
            </label>
          </div>
        )}

        {tab === "form" && (
          <div className="show-tab-panel">
            <label className="show-field-label">
              Form
              <select
                value={state.form}
                onChange={(e) => onStateChange({ ...state, form: e.target.value })}
                aria-label="Form to show"
              >
                <option value="">— select —</option>
                {formNames.map((name) => (
                  <option key={name} value={name}>
                    {name}
                  </option>
                ))}
              </select>
            </label>
          </div>
        )}

        {tab === "storedRecord" && (
          <div className="show-tab-panel show-stored-record-panel">
            <label className="show-field-label show-from-label">
              from
              <select
                value={state.recordForm}
                onChange={(e) => onStateChange({ ...state, recordForm: e.target.value })}
                aria-label="Source form for stored records"
              >
                <option value="">— select —</option>
                {formNames.map((name) => (
                  <option key={name} value={name}>
                    {name}
                  </option>
                ))}
              </select>
            </label>
            <fieldset className="show-submit-fieldset">
              <legend>Upon submit:</legend>
              <label>
                <input
                  type="radio"
                  name="show-record-submit"
                  checked={state.recordSubmit === "modify"}
                  onChange={() => onStateChange({ ...state, recordSubmit: "modify" })}
                />
                Modify existing record
              </label>
              <label>
                <input
                  type="radio"
                  name="show-record-submit"
                  checked={state.recordSubmit === "new"}
                  onChange={() => onStateChange({ ...state, recordSubmit: "new" })}
                />
                Create new record
              </label>
            </fieldset>
            <div className="show-where-group">
              <div className="show-where-label">Where (optional)</div>
              {state.recordRows.length > 1 && (
                <p className="skip-if-intro show-where-intro">
                  Match records where{" "}
                  <select
                    value={state.recordCombinator}
                    onChange={(e) => setRecordCombinator(e.target.value as ConditionCombinator)}
                    aria-label="Where combinator"
                    className="skip-combinator"
                  >
                    <option value="and">ALL</option>
                    <option value="or">ANY</option>
                  </select>{" "}
                  of the following are true:
                </p>
              )}
              {state.recordRows.map((row, i) => (
                <div key={i} className="skip-if-row">
                  <QualifiedFieldInput
                    className="skip-if-field"
                    placeholder="Record:Form:Field"
                    knownVariables={knownVariables}
                    value={row.field}
                    onValueChange={(v) => updateRecordRow(i, { field: v })}
                  />
                  <select
                    value={row.op}
                    onChange={(e) => updateRecordRow(i, { op: e.target.value })}
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
                      onValueChange={(v) => updateRecordRow(i, { value: v })}
                    />
                  ) : (
                    <span className="skip-if-value-placeholder" aria-hidden />
                  )}
                  <button
                    type="button"
                    className="skip-if-row-btn"
                    title="Add condition row"
                    onClick={() => addRecordRowAfter(i)}
                  >
                    +
                  </button>
                  <button
                    type="button"
                    className="skip-if-row-btn"
                    title="Remove condition row"
                    disabled={state.recordRows.length <= 1}
                    onClick={() => removeRecordRow(i)}
                  >
                    −
                  </button>
                </div>
              ))}
            </div>
          </div>
        )}

        {tab === "url" && (
          <div className="show-tab-panel">
            <label className="show-url-label">
              Type or paste URL here:
              <FieldTextInput
                className="show-url-input"
                placeholder="www.example.com"
                value={state.url}
                onValueChange={(v) => onStateChange({ ...state, url: v })}
              />
            </label>
          </div>
        )}

        <div className="skip-if-add-row">
          <button
            type="button"
            className="skip-add-btn"
            disabled={!canSubmit || (tab === "storedRecord" && !recordWhereValid)}
            onClick={onSubmit}
          >
            {submitLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
