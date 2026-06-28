import { useProjectStore } from "@/store/projectStore";
import { FORM_ITEM_PALETTE } from "@/types/tawala";

/** Mirrors legacy FormItemsPalette — insert form item types. */
export function FormItemsPalette() {
  const insertFormItem = useProjectStore((s) => s.insertFormItem);

  return (
    <div className="form-items-palette">
      {FORM_ITEM_PALETTE.map(({ label, type }) => (
        <button key={type} type="button" onClick={() => insertFormItem(type)} title={label}>
          {label}
        </button>
      ))}
    </div>
  );
}
