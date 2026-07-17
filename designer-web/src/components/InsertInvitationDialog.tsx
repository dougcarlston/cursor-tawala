import { useMemo, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FieldTextInput } from "./FieldDropInputs";
import { setConfigureFunctionFieldLock } from "@/lib/fieldInsertion";
import type { InvitationDraft } from "@/lib/linkInsert";

interface Props {
  initial?: Partial<InvitationDraft>;
  onCancel: () => void;
  onSave: (draft: InvitationDraft) => void;
}

const INVITEE_HELP =
  'Enter text or a field to be placed in the special variable "_InviteeID" when someone responds to this invitation. That text, or the value of the field, will be available in the "_InviteeID" variable when your invitee responds by clicking the Invitation link.';

/** Legacy `InsertInvitationDialog` — Form / in / Project / Display Text / private invite. */
export function InsertInvitationDialog({ initial, onCancel, onSave }: Props) {
  const forms = useProjectStore((s) => s.project.forms);
  const [form, setForm] = useState(initial?.form ?? forms[0]?.name ?? "");
  const [project, setProject] = useState(initial?.project ?? "");
  const [displayText, setDisplayText] = useState(initial?.displayText ?? "");
  const [isPrivate, setIsPrivate] = useState(initial?.isPrivate ?? false);
  const [authToken, setAuthToken] = useState(initial?.authToken ?? "");

  const canSave = useMemo(() => {
    if (!form.trim()) return false;
    if (isPrivate && !authToken.trim()) return false;
    return true;
  }, [form, isPrivate, authToken]);

  return (
    <div className="modal-overlay configure-function-overlay" role="presentation">
      <div
        className="modal configure-function-dialog insert-link-dialog insert-invitation-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="insert-invitation-title"
      >
        <h2 id="insert-invitation-title">Insert Invitation</h2>
        <div className="configure-function-body">
          <div className="insert-invitation-form-row">
            <div className="insert-invitation-field">
              <label htmlFor="insert-invitation-form">Form:</label>
              <select
                id="insert-invitation-form"
                value={form}
                onChange={(e) => setForm(e.target.value)}
                aria-label="Invitation form"
              >
                {forms.map((f) => (
                  <option key={f.name} value={f.name}>
                    {f.name}
                  </option>
                ))}
              </select>
            </div>
            <span className="insert-invitation-in">in</span>
            <div className="insert-invitation-field">
              <label htmlFor="insert-invitation-project">Project:</label>
              <select
                id="insert-invitation-project"
                value={project}
                onChange={(e) => setProject(e.target.value)}
                aria-label="Invitation project"
              >
                <option value="">(Current Project)</option>
              </select>
            </div>
          </div>

          <div className="insert-invitation-field insert-invitation-display">
            <label htmlFor="insert-invitation-display">Display Text:</label>
            <FieldTextInput
              id="insert-invitation-display"
              configureDialog
              value={displayText}
              onFocus={() => setConfigureFunctionFieldLock(true)}
              onBlur={() => setConfigureFunctionFieldLock(false)}
              onValueChange={setDisplayText}
            />
          </div>

          <label className="insert-link-checkbox">
            <input
              type="checkbox"
              checked={isPrivate}
              onChange={(e) => setIsPrivate(e.target.checked)}
            />
            <span>Make this a private invitation</span>
          </label>

          <p className="insert-invitation-help">{INVITEE_HELP}</p>

          <div className={!isPrivate ? "insert-link-dimmed" : undefined}>
            <FieldTextInput
              configureDialog
              disabled={!isPrivate}
              value={authToken}
              onFocus={() => setConfigureFunctionFieldLock(true)}
              onBlur={() => setConfigureFunctionFieldLock(false)}
              onValueChange={setAuthToken}
              aria-label="InviteeID value"
            />
          </div>
        </div>
        <div className="modal-actions configure-function-footer insert-link-footer">
          <button
            type="button"
            disabled={!canSave}
            onClick={() => onSave({ form, project, displayText, isPrivate, authToken })}
          >
            OK
          </button>
          <button type="button" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
