import { TawalaForm } from "@/types/tawala";

interface Props {
  form: TawalaForm;
  processNames: string[];
  onChange: (patch: Partial<TawalaForm>) => void;
}

/** Form-level settings — start point, pre/post process (legacy Designer). */
export function FormProperties({ form, processNames, onChange }: Props) {
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
      <label>
        Post-process
        <select
          value={form.process ?? ""}
          onChange={(e) => onChange({ process: e.target.value || undefined })}
        >
          <option value="">(none)</option>
          {processNames.map((name) => (
            <option key={name} value={name}>
              {name}
            </option>
          ))}
        </select>
      </label>
      <p className="hint">Runs after the user submits this form.</p>
      <label>
        Pre-process
        <select
          value={form.preProcess ?? ""}
          onChange={(e) => onChange({ preProcess: e.target.value || undefined })}
        >
          <option value="">(none)</option>
          {processNames.map((name) => (
            <option key={name} value={name}>
              {name}
            </option>
          ))}
        </select>
      </label>
      <p className="hint">Runs when the form loads, before display.</p>
      {processNames.length === 0 ? (
        <p className="hint">Add a process in Project Explorer (P) to attach it here.</p>
      ) : null}
    </div>
  );
}
