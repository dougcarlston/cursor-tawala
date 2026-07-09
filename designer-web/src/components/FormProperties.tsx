import { TawalaForm } from "@/types/tawala";

interface Props {
  form: TawalaForm;
  onChange: (patch: Partial<TawalaForm>) => void;
}

/** Form-level settings — start point (legacy Designer). Pre/Post-process linking lives on the Process window banner. */
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
    </div>
  );
}
