import { useMemo, useState } from "react";
import { FieldTextInput } from "./FieldDropInputs";
import { FunctionConditionsEditor } from "./FunctionConditionsEditor";
import { setConfigureFunctionFieldLock } from "@/lib/fieldInsertion";
import {
  DEFAULT_FUNCTION_CONDITIONS,
  functionConditionsToConfig,
  parseFunctionConditions,
  type FunctionConditionsState,
} from "@/lib/functionConditions";
import type { HyperlinkDraft } from "@/lib/linkInsert";

interface Props {
  initial?: Partial<HyperlinkDraft>;
  onCancel: () => void;
  onSave: (draft: HyperlinkDraft) => void;
}

/** Legacy `InsertHyperlinkDialog` — Url, Display text, new window, optional conditions. */
export function InsertHyperlinkDialog({ initial, onCancel, onSave }: Props) {
  const [url, setUrl] = useState(initial?.url ?? "");
  const [displayText, setDisplayText] = useState(initial?.displayText ?? "");
  const [openNewWindow, setOpenNewWindow] = useState(initial?.openNewWindow ?? false);
  const [conditional, setConditional] = useState(initial?.conditional ?? false);
  const [conditions, setConditions] = useState<FunctionConditionsState>(() => {
    if (initial?.conditions?.length) {
      return parseFunctionConditions({
        conditionsRows: initial.conditions,
      });
    }
    return { ...DEFAULT_FUNCTION_CONDITIONS };
  });

  const canSave = useMemo(() => url.trim().length > 0, [url]);

  const commit = () => {
    const cfg = functionConditionsToConfig(conditions);
    const rows = (cfg.conditionsRows as HyperlinkDraft["conditions"]) ?? [
      { field: "", op: "equals", value: "" },
    ];
    onSave({
      url: url.trim(),
      displayText,
      openNewWindow,
      conditional,
      conditions: rows,
    });
  };

  return (
    <div className="modal-overlay configure-function-overlay" role="presentation">
      <div
        className="modal configure-function-dialog insert-link-dialog insert-hyperlink-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="insert-hyperlink-title"
      >
        <h2 id="insert-hyperlink-title">Hyperlink</h2>
        <div className="configure-function-body">
          <div className="insert-hyperlink-row">
            <label htmlFor="insert-hyperlink-url" className="insert-hyperlink-label">
              Url:
            </label>
            <FieldTextInput
              id="insert-hyperlink-url"
              configureDialog
              value={url}
              onFocus={() => setConfigureFunctionFieldLock(true)}
              onBlur={() => setConfigureFunctionFieldLock(false)}
              onValueChange={setUrl}
            />
          </div>

          <div className="insert-hyperlink-row">
            <label htmlFor="insert-hyperlink-display" className="insert-hyperlink-label">
              Display text:
            </label>
            <FieldTextInput
              id="insert-hyperlink-display"
              configureDialog
              value={displayText}
              onFocus={() => setConfigureFunctionFieldLock(true)}
              onBlur={() => setConfigureFunctionFieldLock(false)}
              onValueChange={setDisplayText}
            />
          </div>
          <p className="insert-hyperlink-optional">
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
        <div className="modal-actions configure-function-footer insert-link-footer">
          <button type="button" disabled={!canSave} onClick={commit}>
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
