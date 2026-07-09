import { useMemo } from "react";
import { QualifiedFieldInput, FieldTextInput } from "@/components/FieldDropInputs";
import { getSendFieldErrors } from "@/lib/sendEmailValidation";
import type { SendBuilderState } from "@/lib/statementBuilders";
import { sendBuilderIsValid } from "@/lib/statementBuilders";
import type { TawalaProject } from "@/types/tawala";

export interface SendStatementBuilderProps {
  state: SendBuilderState;
  onStateChange: (next: SendBuilderState) => void;
  submitLabel: string;
  onSubmit: () => void;
  project: TawalaProject;
  documentNames: readonly string[];
  knownVariables: ReadonlySet<string>;
  /** Legacy: Include Page Header is enabled only when the project has page header content. */
  hasPageHeaderContent?: boolean;
  embedded?: boolean;
}

function fieldClass(base: string, error?: string): string {
  return `${base}${error ? " send-field-invalid" : ""}`.trim();
}

/**
 * Send statement property panel — Email tab per `DESIGNER_PROCESS_STATEMENTS_SEND.md`.
 * To/Cc are email-validated (FIB Email field or literal). From literals are email-validated.
 * From (Name) and Subject accept fields, variables, literals, or combinations.
 */
export function SendStatementBuilder({
  state,
  onStateChange,
  submitLabel,
  onSubmit,
  project,
  documentNames,
  knownVariables,
  hasPageHeaderContent = false,
  embedded = false,
}: SendStatementBuilderProps) {
  const fieldErrors = useMemo(
    () => getSendFieldErrors(state, project, knownVariables),
    [state, project, knownVariables],
  );
  const canSubmit = sendBuilderIsValid(state, documentNames, project, knownVariables);

  return (
    <div
      className={`skip-statement-panel skip-send-builder${embedded ? " process-embedded" : ""}`}
    >
      <div className="show-statement-tabs" role="tablist" aria-label="Send statement type">
        <button type="button" role="tab" aria-selected className="active">
          Email
        </button>
      </div>
      <div className="skip-statement-panel-body send-statement-panel-body">
        <div className="send-email-panel">
          <div className="send-email-row-pairs">
            <div className="send-email-pair send-email-pair-wide-label">
              <label className="send-field-label" htmlFor="send-to">
                To:
              </label>
              <div className="send-field-cell">
                <QualifiedFieldInput
                  id="send-to"
                  className={fieldClass("send-field-input", fieldErrors.to)}
                  placeholder="Form:Field"
                  knownVariables={knownVariables}
                  value={state.to}
                  onValueChange={(v) => onStateChange({ ...state, to: v })}
                  aria-invalid={fieldErrors.to ? true : undefined}
                  aria-describedby={fieldErrors.to ? "send-to-error" : undefined}
                />
                {fieldErrors.to ? (
                  <p id="send-to-error" className="send-field-error">
                    {fieldErrors.to}
                  </p>
                ) : null}
              </div>
            </div>
            <div className="send-email-pair send-email-pair-narrow-label">
              <label className="send-field-label" htmlFor="send-cc">
                Cc:
              </label>
              <div className="send-field-cell">
                <QualifiedFieldInput
                  id="send-cc"
                  className={fieldClass("send-field-input", fieldErrors.cc)}
                  placeholder=""
                  knownVariables={knownVariables}
                  value={state.cc}
                  onValueChange={(v) => onStateChange({ ...state, cc: v })}
                  aria-invalid={fieldErrors.cc ? true : undefined}
                  aria-describedby={fieldErrors.cc ? "send-cc-error" : undefined}
                />
                {fieldErrors.cc ? (
                  <p id="send-cc-error" className="send-field-error">
                    {fieldErrors.cc}
                  </p>
                ) : null}
              </div>
            </div>
          </div>

          <div className="send-email-row-pairs">
            <div className="send-email-pair send-email-pair-wide-label">
              <label className="send-field-label" htmlFor="send-from-address">
                From (Address):
              </label>
              <div className="send-field-cell">
                <FieldTextInput
                  id="send-from-address"
                  className={fieldClass("send-field-input", fieldErrors.fromAddress)}
                  placeholder=""
                  value={state.fromAddress}
                  onValueChange={(v) => onStateChange({ ...state, fromAddress: v })}
                  aria-invalid={fieldErrors.fromAddress ? true : undefined}
                  aria-describedby={fieldErrors.fromAddress ? "send-from-error" : undefined}
                />
                {fieldErrors.fromAddress ? (
                  <p id="send-from-error" className="send-field-error">
                    {fieldErrors.fromAddress}
                  </p>
                ) : null}
              </div>
            </div>
            <div className="send-email-pair send-email-pair-narrow-label">
              <label className="send-field-label" htmlFor="send-from-name">
                (Name):
              </label>
              <FieldTextInput
                id="send-from-name"
                className="send-field-input"
                placeholder=""
                value={state.fromName}
                onValueChange={(v) => onStateChange({ ...state, fromName: v })}
              />
            </div>
          </div>

          <div className="send-email-pair send-subject-row">
            <label className="send-field-label" htmlFor="send-subject">
              Subject:
            </label>
            <FieldTextInput
              id="send-subject"
              className="send-field-input send-subject-input"
              placeholder=""
              value={state.subject}
              onValueChange={(v) => onStateChange({ ...state, subject: v })}
            />
          </div>

          <div className="send-email-pair send-doc-row">
            <label className="send-field-label send-doc-label" htmlFor="send-document">
              Document to be used as Body text:
            </label>
            <select
              id="send-document"
              className="send-doc-select"
              value={state.document}
              onChange={(e) => onStateChange({ ...state, document: e.target.value })}
              aria-label="Document to be used as body text"
            >
              <option value="">— select —</option>
              {documentNames.map((name) => (
                <option key={name} value={name}>
                  {name}
                </option>
              ))}
            </select>
          </div>
        </div>

        <div className="send-checkbox-row">
          <label
            className={`send-checkbox-label${hasPageHeaderContent ? "" : " disabled"}`}
          >
            <input
              type="checkbox"
              checked={hasPageHeaderContent && state.showPageHeader}
              disabled={!hasPageHeaderContent}
              onChange={(e) => onStateChange({ ...state, showPageHeader: e.target.checked })}
            />
            Include Page Header
          </label>
          <div className="send-checkbox-reset-col">
            <label className="send-checkbox-label">
              <input
                type="checkbox"
                checked={state.documentReset}
                onChange={(e) => onStateChange({ ...state, documentReset: e.target.checked })}
              />
              Reset document to original state after Send
            </label>
            <span className="send-reset-sublabel">(removes any appended documents)</span>
          </div>
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
