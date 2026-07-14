import { useEffect, useRef, useState } from "react";
import { FieldItem } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import {
  collectProjectFieldNames,
  isValidHiddenFieldName,
} from "@/lib/fieldNames";

interface Props {
  item: FieldItem;
  index: number;
  formName: string;
  selected: boolean;
}

/**
 * Hidden field — canvas-inline name editor (legacy `HiddenFieldView`).
 * Fixed FIELD badge (dark green, italic); body is Name: + text box. Not shown at runtime.
 * First click selects; second click on the name enters edit (same as other form item labels).
 */
export function HiddenFieldCanvasRow({ item, index, formName, selected }: Props) {
  const project = useProjectStore((s) => s.project);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const [nameError, setNameError] = useState<string | null>(null);
  const [editingName, setEditingName] = useState(false);
  const inputRef = useRef<HTMLInputElement>(null);
  const draftRef = useRef<string | null>(null);
  const revertingRef = useRef(false);

  const fieldName = item.fieldName ?? item.name ?? "";

  const update = (patch: Partial<FieldItem>) =>
    updateFormItem(formName, index, { ...item, ...patch });

  const takenNames = () =>
    collectProjectFieldNames(project, { formName, itemIndex: index });

  useEffect(() => {
    if (!selected) setEditingName(false);
  }, [selected]);

  useEffect(() => {
    if (!editingName) return;
    const el = inputRef.current;
    if (!el) return;
    el.focus();
    el.select();
  }, [editingName]);

  const commitName = (raw: string): boolean => {
    const trimmed = raw.trim();
    const previous = draftRef.current ?? fieldName;
    const err = isValidHiddenFieldName(trimmed, takenNames());
    if (err) {
      update({ fieldName: previous, name: previous });
      setNameError(`${err} — reverted.`);
      return false;
    }
    draftRef.current = null;
    setNameError(null);
    update({ fieldName: trimmed, name: trimmed });
    return true;
  };

  const endNameEdit = (raw: string) => {
    const ok = commitName(raw);
    if (!ok) {
      revertingRef.current = true;
      requestAnimationFrame(() => {
        inputRef.current?.focus();
        inputRef.current?.select();
        revertingRef.current = false;
      });
      return;
    }
    setEditingName(false);
  };

  return (
    <div
      className={`hidden-field-canvas-row${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
      }}
    >
      <div
        className="hidden-field-badge"
        title={selected ? "Drag to reorder" : "Hidden field"}
        draggable={selected}
        onDragStart={(e) => {
          if (!selected) {
            e.preventDefault();
            return;
          }
          e.dataTransfer.effectAllowed = "move";
        }}
      >
        FIELD
      </div>
      <div className="hidden-field-main">
        <label className="hidden-field-name-label">
          Name:
          {editingName ? (
            <input
              ref={inputRef}
              type="text"
              className={`hidden-field-name-input${nameError ? " hidden-field-invalid" : ""}`}
              defaultValue={fieldName}
              maxLength={50}
              onClick={(e) => e.stopPropagation()}
              onFocus={() => {
                draftRef.current = fieldName;
              }}
              onBlur={(e) => {
                if (revertingRef.current) return;
                endNameEdit(e.currentTarget.value);
              }}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  e.currentTarget.blur();
                } else if (e.key === "Escape") {
                  e.preventDefault();
                  setEditingName(false);
                  setNameError(null);
                }
              }}
            />
          ) : (
            <button
              type="button"
              className="hidden-field-name-display"
              title={selected ? "Click to edit name" : "Click to select"}
              onClick={(e) => {
                e.stopPropagation();
                setSelectedItemIndex(index);
                if (selected) {
                  draftRef.current = fieldName;
                  setEditingName(true);
                }
              }}
            >
              {fieldName || "…"}
            </button>
          )}
        </label>
        {nameError ? <p className="hidden-field-error">{nameError}</p> : null}
      </div>
    </div>
  );
}
