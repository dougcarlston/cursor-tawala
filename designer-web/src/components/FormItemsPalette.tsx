import { useProjectStore } from "@/store/projectStore";
import { FormItemType } from "@/types/tawala";

/**
 * Docked "Items" palette — the legacy toolbox column between Project Explorer and the MDI
 * canvas (owner decision D-Items-palette-placement, July 2026). Restyled to the legacy
 * look-and-feel (owner, July 2026): blue "Items" header + tall icon-over-label buttons.
 *
 * Icons are Unicode/CSS placeholders for now; real icon assets can be swapped in later.
 * File Uploader is shown for visual parity with legacy but is **always disabled** — it was
 * never implemented in the browser Designer (see specs).
 *
 * Owner Issue 1 (July 2026): the docked column CONTEXT-SWAPS by active window kind (App.tsx)
 * — this Items palette renders only while a Form window is active, a Processes/Statements
 * palette replaces it for a Process, and nothing shows for a Document. Because it only mounts
 * for an active form, its insert buttons are enabled (store `selection` tracks the active
 * window, so `insertFormItem` targets that form); the guard remains as a safety net.
 */

interface PaletteItem {
  /** `null` marks a display-only item with no insert action (File Uploader). */
  type: FormItemType | null;
  label: string;
  glyph: string;
  alwaysDisabled?: boolean;
  note?: string;
}

// Order and labels mirror the legacy Items toolbox.
const ITEMS: PaletteItem[] = [
  { type: "heading", label: "Heading", glyph: "H" },
  { type: "text", label: "Text", glyph: "T" },
  { type: "fib", label: "Fill in the Blank", glyph: "▭" },
  { type: "mc", label: "Multiple Choice", glyph: "☑" },
  {
    type: null,
    label: "File Uploader",
    glyph: "⬆",
    alwaysDisabled: true,
    note: "File Uploader is not implemented in the browser Designer yet.",
  },
  { type: "field", label: "Hidden Field", glyph: "▨" },
  { type: "break", label: "Page Break", glyph: "⤓" },
  { type: "skipInstructions", label: "Skip Instructions", glyph: "⚙" },
];

export function FormItemsPalette() {
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const selection = useProjectStore((s) => s.selection);
  const formInactive = selection.kind !== "form" || !selection.name;

  return (
    <>
      <div className="items-palette-title">Items</div>
      <div className="items-palette-body">
        {ITEMS.map((item) => {
          const disabled = item.alwaysDisabled || formInactive;
          const title = item.alwaysDisabled
            ? item.note
            : formInactive
              ? "Select a form in Project Explorer first"
              : `Insert ${item.label}`;
          return (
            <button
              key={item.label}
              type="button"
              className="items-palette-button"
              disabled={disabled}
              title={title}
              onClick={() => item.type && insertFormItem(item.type)}
            >
              <span className="items-palette-icon" aria-hidden>
                {item.glyph}
              </span>
              <span className="items-palette-label">{item.label}</span>
            </button>
          );
        })}
      </div>
    </>
  );
}
