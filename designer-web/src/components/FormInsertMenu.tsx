import { useProjectStore } from "@/store/projectStore";
import { FORM_ITEM_PALETTE, FormItemType } from "@/types/tawala";

/** Shared insert actions — left strip, toolbar, and Insert menu. */
export function FormInsertButtons({ compact = false }: { compact?: boolean }) {
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const selection = useProjectStore((s) => s.selection);
  const disabled = selection.kind !== "form" || !selection.name;

  return (
    <div className={`form-insert-buttons${compact ? " compact" : ""}`}>
      {FORM_ITEM_PALETTE.map(({ label, type }) => (
        <button
          key={type}
          type="button"
          disabled={disabled}
          title={disabled ? "Select a form in Project Explorer first" : `Insert ${label}`}
          onClick={() => insertFormItem(type as FormItemType)}
        >
          {compact ? label.replace(" Item", "").replace("Fill in the Blank", "FIB") : label}
        </button>
      ))}
    </div>
  );
}
