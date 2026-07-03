import { useState } from "react";
import { TawalaForm } from "@/types/tawala";

interface FieldEntry {
  key: string;
  label: string;
  dragValue: string;
}

function fieldsForForm(form: TawalaForm, formName: string): Map<string, FieldEntry[]> {
  const groups = new Map<string, FieldEntry[]>();
  for (const item of form.items) {
    if (item.type === "fib" && item.blanks?.length) {
      const entries = item.blanks.map((b) => ({
        key: `${item.label}:${b.name}`,
        label: b.displayLabel?.trim() || b.alternateLabel || b.name,
        dragValue: `Record:${formName}:${item.label}:${b.name}`,
      }));
      groups.set(item.label, entries);
    }
    if (item.type === "mc" && item.choices?.length) {
      groups.set(
        item.label,
        item.choices.map((c) => ({
          key: `${item.label}:${c.name}`,
          label: c.text || c.name,
          dragValue: `Record:${formName}:${item.label}:${c.name}`,
        })),
      );
    }
    if (item.type === "field" && item.fieldName) {
      groups.set(item.label, [
        {
          key: item.fieldName,
          label: item.fieldName,
          dragValue: item.fieldName,
        },
      ]);
    }
  }
  return groups;
}

/** Right-hand Fields Palette — grouped, collapsible, draggable (drop targets coming). */
export function FieldsPalette({ form, formName }: { form?: TawalaForm; formName?: string }) {
  const [formOpen, setFormOpen] = useState(true);
  const [closedItems, setClosedItems] = useState<Set<string>>(new Set());

  if (!form || !formName) {
    return <p className="hint fields-palette-empty">Select a form to see fields.</p>;
  }

  const groups = fieldsForForm(form, formName);
  const itemLabels = [...groups.keys()];

  const toggleItem = (label: string) => {
    setClosedItems((prev) => {
      const next = new Set(prev);
      if (next.has(label)) next.delete(label);
      else next.add(label);
      return next;
    });
  };

  return (
    <div className="fields-palette-compact">
      <button
        type="button"
        className="fields-form-toggle"
        onClick={() => setFormOpen((v) => !v)}
        aria-expanded={formOpen}
      >
        <span className="fields-chevron">{formOpen ? "▾" : "▸"}</span>
        {formName}
      </button>
      {formOpen ? (
        <div className="fields-form-body">
          {itemLabels.length === 0 ? (
            <p className="hint">No fields yet — add FIB or MCQ items.</p>
          ) : (
            itemLabels.map((itemLabel) => {
              const entries = groups.get(itemLabel) ?? [];
              const open = !closedItems.has(itemLabel);
              return (
                <div key={itemLabel} className="fields-item-group">
                  <button
                    type="button"
                    className="fields-item-toggle"
                    onClick={() => toggleItem(itemLabel)}
                    aria-expanded={open}
                  >
                    <span className="fields-chevron">{open ? "▾" : "▸"}</span>
                    {itemLabel}
                    <span className="fields-item-count">{entries.length}</span>
                  </button>
                  {open ? (
                    <ul className="fields-list">
                      {entries.map((e) => (
                        <li key={e.key}>
                          <span
                            className="field-row"
                            draggable
                            title={e.dragValue}
                            onDragStart={(ev) => {
                              ev.dataTransfer.setData("text/plain", e.dragValue);
                              ev.dataTransfer.effectAllowed = "copy";
                            }}
                          >
                            {e.label}
                          </span>
                        </li>
                      ))}
                    </ul>
                  ) : null}
                </div>
              );
            })
          )}
        </div>
      ) : null}
    </div>
  );
}
