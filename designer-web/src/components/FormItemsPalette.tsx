import { Fragment } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FormItemType } from "@/types/tawala";
import { setFormItemDrag } from "@/lib/designerDrag";

/**
 * Docked "Items" palette — legacy toolbox between Project Explorer and MDI
 * (D-Items-palette-placement). Icons are exact 24×24 bitmaps from legacy
 * `Form_Item*.png`. File Uploader omitted (owner Jul 17). Separator after
 * Multiple Choice matches legacy rule below File Uploader.
 */

interface PaletteItem {
  type: FormItemType;
  label: string;
  icon: string;
  /** Draw the thin legacy rule after this button (before Hidden Field group). */
  separatorAfter?: boolean;
}

const ITEMS: PaletteItem[] = [
  { type: "heading", label: "Heading", icon: "/icons/form-item-heading.png" },
  { type: "text", label: "Text", icon: "/icons/form-item-text.png" },
  { type: "fib", label: "Fill in the Blank", icon: "/icons/form-item-fib.png" },
  {
    type: "mc",
    label: "Multiple Choice",
    icon: "/icons/form-item-mcq.png",
    separatorAfter: true,
  },
  { type: "field", label: "Hidden Field", icon: "/icons/form-item-hidden.png" },
  { type: "break", label: "Page Break", icon: "/icons/form-item-break.png" },
  { type: "skipInstructions", label: "Skip Instructions", icon: "/icons/form-item-skip.png" },
];

export function FormItemsPalette() {
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const selection = useProjectStore((s) => s.selection);
  const formInactive = selection.kind !== "form" || !selection.name;

  return (
    <>
      <div className="items-palette-title">Items</div>
      <div className="items-palette-body legacy-scrollbar">
        {ITEMS.map((item) => {
          const title = formInactive
            ? "Open a Form window (or drag this item onto one)"
            : `Insert ${item.label} (click uses selection; or drag onto a Form window)`;
          return (
            <Fragment key={item.label}>
              <button
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
                <img
                  className="items-palette-icon-img"
                  src={item.icon}
                  width={24}
                  height={24}
                  alt=""
                  draggable={false}
                />
                <span className="items-palette-label">{item.label}</span>
              </button>
              {item.separatorAfter ? <div className="items-palette-separator" aria-hidden /> : null}
            </Fragment>
          );
        })}
      </div>
    </>
  );
}
