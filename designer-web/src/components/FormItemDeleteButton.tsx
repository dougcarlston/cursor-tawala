import { confirmAndDeleteFormItem } from "@/lib/shellCommands";

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
        confirmAndDeleteFormItem(formName, index);
      }}
    >
      ×
    </button>
  );
}
