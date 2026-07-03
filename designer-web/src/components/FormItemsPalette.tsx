import { FormInsertButtons } from "./FormInsertMenu";

/** Left strip — insert form item types (legacy Items palette). */
export function FormItemsPalette() {
  return (
    <div className="form-items-palette-wrap">
      <div className="form-items-palette-title">Insert item</div>
      <p className="hint form-items-palette-hint">Adds to the open form at the end.</p>
      <FormInsertButtons />
    </div>
  );
}
