import { confirmAndDeleteFormItem } from "@/lib/shellCommands";
import { tryDeleteSelectedFormInlineTokens } from "@/lib/inlineTokenDelete";

/** Red × delete control for selected canvas WYSIWYG rows (legacy toolbar #8 parity). */
export function FormItemDeleteButton({
  formName,
  index,
  visible,
}: {
  formName: string;
  index: number;
  visible: boolean;
}) {
  if (!visible) return null;
  return (
    <button
      type="button"
      className="canvas-item-delete"
      title="Delete item (Del)"
      onClick={(e) => {
        e.stopPropagation();
        if (tryDeleteSelectedFormInlineTokens()) return;
        confirmAndDeleteFormItem(formName, index);
      }}
    >
      ×
    </button>
  );
}
