import { useProjectStore } from "@/store/projectStore";

/** Right-hand Fields Palette — field names for expressions (legacy Designer). */
export function FieldsPalette() {
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);

  const form =
    selection.kind === "form" && selection.name
      ? project.forms.find((f) => f.name === selection.name)
      : undefined;

  const fieldNames = new Set<string>();
  if (form) {
    for (const item of form.items) {
      if (item.type === "fib" && item.blanks) {
        for (const b of item.blanks) fieldNames.add(b.name);
      }
      if (item.type === "mc" && item.choices) {
        for (const c of item.choices) fieldNames.add(c.name);
      }
      if (item.type === "field" && item.fieldName) fieldNames.add(item.fieldName);
    }
  }

  return (
    <>
      <div className="panel-title">Fields Palette</div>
      <div className="fields-palette">
        <p className="hint">
          Drag fields into expressions (coming soon). Listed fields are inferred from the
          current form.
        </p>
        {form ? (
          <div className="field-group">
            <div className="field-group-title">{form.name}</div>
            {fieldNames.size === 0 ? (
              <span className="hint">No fields yet</span>
            ) : (
              [...fieldNames].map((name) => (
                <span key={name} className="field-chip" title={name}>
                  {name}
                </span>
              ))
            )}
          </div>
        ) : (
          <span className="hint">Open a form to see fields</span>
        )}
      </div>
    </>
  );
}
