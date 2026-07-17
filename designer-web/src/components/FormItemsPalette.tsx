import { useProjectStore } from "@/store/projectStore";
import { FormItemType } from "@/types/tawala";
import { setFormItemDrag } from "@/lib/designerDrag";

/**
 * Docked "Items" palette — the legacy toolbox column between Project Explorer and the MDI
 * canvas (owner decision D-Items-palette-placement, July 2026). Restyled to the legacy
 * look-and-feel (owner, July 2026): blue "Items" header + tall icon-over-label buttons.
 *
 * Icons are Unicode/CSS placeholders for now; real icon assets can be swapped in later.
 * **File Uploader** is omitted (owner Jul 17) — never wired in the 2011 reference build or
 * browser Designer; see `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` (deferred / out of palette).
 *
 * Click inserts at the current selection (before the selected item, or at end);
 * drag onto a Form window shows a live caret at the drop point.
 */

interface PaletteItem {
  type: FormItemType;
  label: string;
  glyph: string;
}

// Order and labels mirror the legacy Items toolbox (without File Uploader).
const ITEMS: PaletteItem[] = [
  { type: "heading", label: "Heading", glyph: "H" },
  { type: "text", label: "Text", glyph: "T" },
  { type: "fib", label: "Fill in the Blank", glyph: "▭" },
  { type: "mc", label: "Multiple Choice", glyph: "☑" },
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
          const title = formInactive
            ? "Open a Form window (or drag this item onto one)"
            : `Insert ${item.label} (click uses selection; or drag onto a Form window)`;
          return (
            <button
              key={item.label}
              type="button"
              className="items-palette-button"
              title={title}
              draggable
              onDragStart={(e) => {
                setFormItemDrag(e.dataTransfer, item.type);
              }}
              onClick={() => {
                if (formInactive) return;
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
