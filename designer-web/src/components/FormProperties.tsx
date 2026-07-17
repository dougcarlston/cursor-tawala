import { TawalaForm } from "@/types/tawala";

interface Props {
  form: TawalaForm;
  onChange: (patch: Partial<TawalaForm>) => void;
}

/**
 * Form-level settings UI (kept for possible future Properties re-dock).
 * Live toggles today: Project Explorer toolbar — Starting Point, Pre-populate,
 * Block Back (Properties panel was removed July 2026; Fields-only right column).
 */
export function FormProperties({ form, onChange }: Props) {
  return (
    <div className="properties-panel form-properties-panel">
      <p className="form-properties-heading">
        <strong>{form.name}</strong>
      </p>
      <label className="property-checkbox">
        <input
          type="checkbox"
          checked={form.startPoint === true}
          onChange={(e) => onChange({ startPoint: e.target.checked })}
        />
        Starting Point
      </label>
      <label className="property-checkbox">
        <input
          type="checkbox"
          checked={form.dataEntryOnly === true}
          onChange={(e) => onChange({ dataEntryOnly: e.target.checked })}
        />
        Pre-populate With Last Entry
      </label>
    </div>
  );
}
