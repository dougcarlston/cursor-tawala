import { NameTextInput } from "@/components/FieldDropInputs";
import type { ForEachBuilderState } from "@/lib/statementBuilders";
import { foreachBuilderIsValid } from "@/lib/statementBuilders";

export interface ForEachStatementBuilderProps {
  state: ForEachBuilderState;
  onStateChange: (next: ForEachBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  recordNames: readonly string[];
  recordLists: readonly string[];
  embedded?: boolean;
}

/**
 * ForEach statement property panel per `DESIGNER_PROCESS_STATEMENTS_APPEND_GET_ETC.md`.
 * Layout: `[record variable ▼] in [record list ▼]` + centered Add.
 */
export function ForEachStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  recordNames,
  recordLists,
  embedded = false,
}: ForEachStatementBuilderProps) {
  const canSubmit = foreachBuilderIsValid(state, recordLists);
  const datalistId = "foreach-record-names";

  return (
    <div
      className={`skip-statement-panel skip-foreach-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="show-statement-tabs" role="tablist" aria-label="ForEach statement type">
        <button type="button" role="tab" aria-selected className="active">
          Record
        </button>
      </div>
      <div className="skip-statement-panel-body foreach-statement-panel-body">
        <div className="foreach-record-row">
          <NameTextInput
            id="foreach-record-name"
            className="foreach-record-input"
            list={datalistId}
            placeholder="Record"
            value={state.recordName}
            onChange={(e) => onStateChange({ ...state, recordName: e.target.value })}
            aria-label="Record variable"
          />
          <datalist id={datalistId}>
            {recordNames.map((name) => (
              <option key={name} value={name} />
            ))}
          </datalist>
          <span className="foreach-in-label">in</span>
          <select
            id="foreach-record-list"
            className="foreach-record-list-select"
            value={state.recordList}
            onChange={(e) => onStateChange({ ...state, recordList: e.target.value })}
            aria-label="Record list"
          >
            <option value="">— select —</option>
            {recordLists.map((name) => (
              <option key={name} value={name}>
                {name}
              </option>
            ))}
          </select>
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
