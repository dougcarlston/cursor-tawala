import type { CommentBuilderState } from "@/lib/statementBuilders";
import { commentBuilderIsValid } from "@/lib/statementBuilders";

export interface CommentStatementBuilderProps {
  state: CommentBuilderState;
  onStateChange: (next: CommentBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  embedded?: boolean;
}

/**
 * Comment statement property panel per `DESIGNER_PROCESS_STATEMENTS_APPEND_GET_ETC.md`.
 * Designer-only documentation line — not executed at runtime.
 */
export function CommentStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  embedded = false,
}: CommentStatementBuilderProps) {
  const canSubmit = commentBuilderIsValid(state);

  return (
    <div
      className={`skip-statement-panel skip-comment-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="show-statement-tabs" role="tablist" aria-label="Comment statement type">
        <button type="button" role="tab" aria-selected className="active">
          Comment
        </button>
      </div>
      <div className="skip-statement-panel-body skip-comment-body process-comment-body">
        <textarea
          id="process-comment-text"
          className="skip-comment-input process-comment-input"
          placeholder="Documentation note (visible in Designer only)"
          rows={4}
          value={state.text}
          onChange={(e) => onStateChange({ ...state, text: e.target.value })}
          aria-label="Comment text"
        />
        <div className="send-add-row">
          <button type="button" className="skip-add-btn" disabled={!canSubmit} onClick={onSubmit}>
            {submitLabel}
          </button>
        </div>
      </div>
    </div>
  );
}
