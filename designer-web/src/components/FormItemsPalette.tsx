import { useProjectStore } from "@/store/projectStore";
import { FormItemType } from "@/types/tawala";
import { setFormItemDrag } from "@/lib/designerDrag";

/**
 * Docked "Items" palette — the legacy toolbox column between Project Explorer and the MDI
 * canvas (owner decision D-Items-palette-placement, July 2026). Restyled to the legacy
 * look-and-feel (owner, July 2026): blue "Items" header + tall icon-over-label buttons.
 *
 * Icons are Unicode/CSS placeholders for now; real icon assets can be swapped in later.
 * File Uploader is shown for visual parity with legacy but is **always disabled** — it was
 * never implemented in the browser Designer (see specs).
 *
 * Click inserts at the blue arrow; drag onto a Form window inserts into that form.
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
          const title = item.alwaysDisabled
            ? item.note
            : formInactive
              ? "Open a Form window (or drag this item onto one)"
              : `Insert ${item.label} at the blue insertion arrow (or drag onto a Form window)`;
          return (
            <button
              key={item.label}
              type="button"
              className="items-palette-button"
              disabled={!!item.alwaysDisabled}
              title={title}
              draggable={!!item.type && !item.alwaysDisabled}
              onDragStart={(e) => {
                if (!item.type || item.alwaysDisabled) {
                  e.preventDefault();
                  return;
                }
                setFormItemDrag(e.dataTransfer, item.type);
              }}
              onClick={() => {
                if (!item.type || formInactive) return;
                insertFormItem(item.type);
              }}
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
