import { useMemo, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FieldTextInput } from "./FieldDropInputs";
import { FunctionConditionsEditor } from "./FunctionConditionsEditor";
import { DesignerDialog } from "./DesignerDialog";
import { setConfigureFunctionFieldLock } from "@/lib/fieldInsertion";
import {
  DEFAULT_FUNCTION_CONDITIONS,
  functionConditionsToConfig,
  parseFunctionConditions,
  type FunctionConditionsState,
} from "@/lib/functionConditions";
import type { HyperlinkDraft, InvitationDraft } from "@/lib/linkInsert";

export type LinkDialogMode = "form" | "url";

export type LinkDialogSave =
  | { kind: "invitation"; draft: InvitationDraft }
  | { kind: "hyperlink"; draft: HyperlinkDraft };

interface Props {
  /** Opening mode — Form-in-project is the product default. */
  initialMode: LinkDialogMode;
  /** When editing an existing chip, do not allow switching token kind. */
  lockMode?: boolean;
  invitationInitial?: Partial<InvitationDraft>;
  hyperlinkInitial?: Partial<HyperlinkDraft>;
  onCancel: () => void;
  onSave: (result: LinkDialogSave) => void;
}

const INVITEE_HELP =
  'Enter text or a field to be placed in the special variable "_InviteeID" when someone responds to this invitation. That text, or the value of the field, will be available in the "_InviteeID" variable when your invitee responds by clicking the Invitation link.';

/**
 * Unified Insert → Link… (Jul 31 framing).
 * Primary: Form in project (`<invitation>`). Secondary: Web address (`<link>`).
 * Tertiary: private InviteeID under Form mode.
 */
export function InsertLinkDialog({
  initialMode,
  lockMode = false,
  invitationInitial,
  hyperlinkInitial,
  onCancel,
  onSave,
}: Props) {
  const forms = useProjectStore((s) => s.project.forms);
  const [mode, setMode] = useState<LinkDialogMode>(initialMode);

  const [form, setForm] = useState(invitationInitial?.form ?? forms[0]?.name ?? "");
  const [project, setProject] = useState(invitationInitial?.project ?? "");
  const [formDisplayText, setFormDisplayText] = useState(invitationInitial?.displayText ?? "");
  const [isPrivate, setIsPrivate] = useState(invitationInitial?.isPrivate ?? false);
  const [authToken, setAuthToken] = useState(invitationInitial?.authToken ?? "");

  const [url, setUrl] = useState(hyperlinkInitial?.url ?? "");
  const [urlDisplayText, setUrlDisplayText] = useState(hyperlinkInitial?.displayText ?? "");
  const [openNewWindow, setOpenNewWindow] = useState(hyperlinkInitial?.openNewWindow ?? false);
  const [conditional, setConditional] = useState(hyperlinkInitial?.conditional ?? false);
  const [conditions, setConditions] = useState<FunctionConditionsState>(() => {
    if (hyperlinkInitial?.conditions?.length) {
      return parseFunctionConditions({
        conditionsRows: hyperlinkInitial.conditions,
      });
    }
    return { ...DEFAULT_FUNCTION_CONDITIONS };
  });

  const canSaveForm = useMemo(() => {
    if (!form.trim()) return false;
    if (isPrivate && !authToken.trim()) return false;
    return true;
  }, [form, isPrivate, authToken]);

  const canSaveUrl = useMemo(() => url.trim().length > 0, [url]);
  const canSave = mode === "form" ? canSaveForm : canSaveUrl;

  const commit = () => {
    if (mode === "form") {
      onSave({
        kind: "invitation",
        draft: {
          form,
          project,
          displayText: formDisplayText,
          isPrivate,
          authToken,
        },
      });
      return;
    }
    const cfg = functionConditionsToConfig(conditions);
    const rows = (cfg.conditionsRows as HyperlinkDraft["conditions"]) ?? [
      { field: "", op: "equals", value: "" },
    ];
    onSave({
      kind: "hyperlink",
      draft: {
        url: url.trim(),
        displayText: urlDisplayText,
        openNewWindow,
        conditional,
        conditions: rows,
      },
    });
  };

  return (
    <DesignerDialog
      title="Insert Link"
      titleId="insert-link-title"
      onClose={onCancel}
      overlayClassName="configure-function-overlay"
      className={`insert-link-dialog${mode === "url" ? " insert-link-dialog-wide designer-dialog-wide" : ""}`}
      footer={
        <>
          <button type="button" disabled={!canSave} onClick={commit}>
            OK
          </button>
          <button type="button" onClick={onCancel}>
            Cancel
          </button>
        </>
      }
    >
      <div className="designer-dialog-panel insert-link-panel">
        <div
          className="insert-link-mode"
          role="radiogroup"
          aria-label="Link type"
          aria-disabled={lockMode || undefined}
        >
          <label className="insert-link-mode-option">
            <input
              type="radio"
              name="insert-link-mode"
              checked={mode === "form"}
              disabled={lockMode && mode !== "form"}
              onChange={() => {
                if (!lockMode) setMode("form");
              }}
            />
            <span>Form in project</span>
          </label>
          <label className="insert-link-mode-option">
            <input
              type="radio"
              name="insert-link-mode"
              checked={mode === "url"}
              disabled={lockMode && mode !== "url"}
              onChange={() => {
                if (!lockMode) setMode("url");
              }}
            />
            <span>Web address (URL)</span>
          </label>
        </div>

        {mode === "form" ? (
          <div className="insert-invitation-panel">
            <div className="insert-invitation-form-row">
              <div className="insert-invitation-field">
                <label htmlFor="insert-link-form">Form:</label>
                <select
                  id="insert-link-form"
                  value={form}
                  onChange={(e) => setForm(e.target.value)}
                  aria-label="Link form"
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
                <label htmlFor="insert-link-project">Project:</label>
                <select
                  id="insert-link-project"
                  value={project}
                  onChange={(e) => setProject(e.target.value)}
                  aria-label="Link project"
                >
                  <option value="">(Current Project)</option>
                </select>
              </div>
            </div>

            <div className="insert-invitation-field insert-invitation-display">
              <label htmlFor="insert-link-form-display">Display Text:</label>
              <FieldTextInput
                id="insert-link-form-display"
                configureDialog
                value={formDisplayText}
                onFocus={() => setConfigureFunctionFieldLock(true)}
                onBlur={() => setConfigureFunctionFieldLock(false)}
                onValueChange={setFormDisplayText}
              />
            </div>

            <label className="insert-link-checkbox insert-invitation-private">
              <input
                type="checkbox"
                checked={isPrivate}
                onChange={(e) => setIsPrivate(e.target.checked)}
              />
              <span>Make this a private invitation.</span>
            </label>

            <p className="insert-invitation-help">{INVITEE_HELP}</p>

            <div
              className={
                !isPrivate
                  ? "insert-link-dimmed insert-invitation-invitee"
                  : "insert-invitation-invitee"
              }
            >
              <FieldTextInput
                configureDialog
                disabled={!isPrivate}
                value={authToken}
                onFocus={() => setConfigureFunctionFieldLock(true)}
                onBlur={() => setConfigureFunctionFieldLock(false)}
                onValueChange={setAuthToken}
                aria-label="InviteeID value"
                className="insert-invitation-invitee-input"
              />
            </div>
          </div>
        ) : (
          <div className="insert-hyperlink-panel">
            <div className="insert-hyperlink-row">
              <label htmlFor="insert-link-url" className="insert-hyperlink-label">
                Url:
              </label>
              <FieldTextInput
                id="insert-link-url"
                configureDialog
                value={url}
                onFocus={() => setConfigureFunctionFieldLock(true)}
                onBlur={() => setConfigureFunctionFieldLock(false)}
                onValueChange={setUrl}
              />
            </div>

            <div className="insert-hyperlink-row">
              <label htmlFor="insert-link-url-display" className="insert-hyperlink-label">
                Display text:
              </label>
              <FieldTextInput
                id="insert-link-url-display"
                configureDialog
                value={urlDisplayText}
                onFocus={() => setConfigureFunctionFieldLock(true)}
                onBlur={() => setConfigureFunctionFieldLock(false)}
                onValueChange={setUrlDisplayText}
              />
            </div>
            <p className="insert-hyperlink-optional hint">
              (optional; if you leave this blank the full URL or filename will be shown)
            </p>

            <label className="insert-link-checkbox">
              <input
                type="checkbox"
                checked={openNewWindow}
                onChange={(e) => setOpenNewWindow(e.target.checked)}
              />
              <span>Open in new browser window.</span>
            </label>

            <hr className="insert-hyperlink-sep" />

            <label className="insert-link-checkbox">
              <input
                type="checkbox"
                checked={conditional}
                onChange={(e) => setConditional(e.target.checked)}
              />
              <span>Display link conditionally</span>
            </label>

            <div className={conditional ? undefined : "insert-link-dimmed"}>
              <FunctionConditionsEditor
                variant="displayWhen"
                paramName="Display link only when"
                state={conditions}
                onChange={setConditions}
                disabled={!conditional}
              />
            </div>
          </div>
        )}
      </div>
    </DesignerDialog>
  );
}
