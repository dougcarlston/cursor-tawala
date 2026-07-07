import { FormInsertButtons } from "./FormInsertMenu";

/**
 * Docked "Items" palette — the legacy toolbox column that sits between Project
 * Explorer and the MDI canvas (owner decision D-Items-palette-placement, July
 * 2026). It is no longer nested inside each Form window.
 *
 * Owner Issue 1 (July 2026): the docked column CONTEXT-SWAPS by active window kind
 * (App.tsx) — this Items palette renders only while a **Form** window is active, a
 * Processes/Statements palette replaces it for a Process, and nothing shows for a
 * Document. Because it only mounts for an active form, its buttons are always
 * enabled (store `selection` tracks the active window, so `insertFormItem` targets
 * that form). The disabled guard in `FormInsertButtons` remains as a safety net.
 */
export function FormItemsPalette() {
  return (
    <>
      <div className="panel-title">Items</div>
      <div className="items-palette-body">
        <FormInsertButtons />
      </div>
    </>
  );
}
