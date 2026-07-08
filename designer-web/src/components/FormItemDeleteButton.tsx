import { useProjectStore } from "@/store/projectStore";

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
  const deleteFormItem = useProjectStore((s) => s.deleteFormItem);
  if (!visible) return null;
  return (
    <button
      type="button"
      className="canvas-item-delete"
      title="Delete item (Del)"
      onClick={(e) => {
        e.stopPropagation();
        deleteFormItem(formName, index);
      }}
    >
      ×
    </button>
  );
}
