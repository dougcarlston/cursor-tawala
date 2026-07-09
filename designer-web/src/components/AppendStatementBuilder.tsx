import type { AppendBuilderState } from "@/lib/statementBuilders";
import { appendBuilderIsValid } from "@/lib/statementBuilders";

export interface AppendStatementBuilderProps {
  state: AppendBuilderState;
  onStateChange: (next: AppendBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  documentNames: readonly string[];
  embedded?: boolean;
}

/**
 * Append statement property panel — Documents tab per `DESIGNER_PROCESS_STATEMENTS_APPEND_GET_ETC.md`.
 * Layout: `[appendage ▼] to [target ▼]` + centered Add.
 */
export function AppendStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  documentNames,
  embedded = false,
}: AppendStatementBuilderProps) {
  const canSubmit = appendBuilderIsValid(state, documentNames);

  return (
    <div
      className={`skip-statement-panel skip-append-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="show-statement-tabs" role="tablist" aria-label="Append statement type">
        <button type="button" role="tab" aria-selected className="active">
          Documents
        </button>
      </div>
      <div className="skip-statement-panel-body append-statement-panel-body">
        <div className="append-doc-row">
          <select
            id="append-appendage"
            className="append-doc-select"
            value={state.appendage}
            onChange={(e) => onStateChange({ ...state, appendage: e.target.value })}
            aria-label="Document to append"
          >
            <option value="">— select —</option>
            {documentNames.map((name) => (
              <option key={name} value={name}>
                {name}
              </option>
            ))}
          </select>
          <span className="append-to-label">to</span>
          <select
            id="append-document"
            className="append-doc-select"
            value={state.document}
            onChange={(e) => onStateChange({ ...state, document: e.target.value })}
            aria-label="Target document"
          >
            <option value="">— select —</option>
            {documentNames.map((name) => (
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
