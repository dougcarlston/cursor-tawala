import { useEffect, useState } from "react";
import { DesignerDialog } from "./DesignerDialog";
import { FunctionConditionsEditor } from "./FunctionConditionsEditor";
import {
  conditionalDisplayOkEnabled,
  displayConditionToEditorState,
  editorStateToDisplayCondition,
} from "@/lib/conditionalDisplay";
import type { FunctionConditionsState } from "@/lib/functionConditions";
import type { FormItem } from "@/types/tawala";

export interface ConditionalDisplayDialogProps {
  item: FormItem;
  onCancel: () => void;
  onSave: (displayCondition: unknown | undefined) => void;
}

/**
 * Legacy "Conditional Display of Form Item" — checkbox + Where rows.
 * Spec: DESIGNER_FORM_ITEMS_CONDITIONAL_DISPLAY.md
 */
export function ConditionalDisplayDialog({
  item,
  onCancel,
  onSave,
}: ConditionalDisplayDialogProps) {
  const initial = displayConditionToEditorState(
    (item as { displayCondition?: unknown }).displayCondition,
  );
  const [enabled, setEnabled] = useState(initial.enabled);
  const [conditions, setConditions] = useState<FunctionConditionsState>(initial.conditions);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") {
        e.preventDefault();
        onCancel();
      }
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [onCancel]);

  const canOk = conditionalDisplayOkEnabled(enabled, conditions);

  return (
    <DesignerDialog
      title="Conditional Display of Form Item"
      titleId="conditional-display-title"
      onClose={onCancel}
      /* Punch-through to Fields (like Insert Link). Do NOT set body.configure-function-open
       * — that raises Fields above the dialog and steals clicks from the value box. */
      overlayClassName="configure-function-overlay conditional-display-overlay"
      className="conditional-display-dialog"
      bodyClassName="conditional-display-body"
      footer={
        <>
          <button
            type="button"
            className="designer-dialog-btn"
            disabled={!canOk}
            onClick={() => onSave(editorStateToDisplayCondition(enabled, conditions))}
          >
            OK
          </button>
          <button type="button" className="designer-dialog-btn" onClick={onCancel}>
            Cancel
          </button>
        </>
      }
    >
      <label className="conditional-display-enable">
        <input
          type="checkbox"
          checked={enabled}
          onChange={(e) => setEnabled(e.target.checked)}
        />
        <span>Display this item conditionally</span>
      </label>

      <div className={enabled ? undefined : "insert-link-dimmed"}>
        <FunctionConditionsEditor
          variant="displayWhen"
          paramName="Display only when"
          state={conditions}
          onChange={setConditions}
          disabled={!enabled}
        />
      </div>
    </DesignerDialog>
  );
}
