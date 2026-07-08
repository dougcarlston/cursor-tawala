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
 */
export function HiddenFieldCanvasRow({ item, index, formName, selected }: Props) {
  const project = useProjectStore((s) => s.project);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const [nameError, setNameError] = useState<string | null>(null);
  const inputRef = useRef<HTMLInputElement>(null);
  const draftRef = useRef<string | null>(null);
  const revertingRef = useRef(false);

  const fieldName = item.fieldName ?? item.name ?? "";

  const update = (patch: Partial<FieldItem>) =>
    updateFormItem(formName, index, { ...item, ...patch });

  const takenNames = () =>
    collectProjectFieldNames(project, { formName, itemIndex: index });

  useEffect(() => {
    if (!selected) return;
    const el = inputRef.current;
    if (!el) return;
    el.focus();
    el.select();
  }, [selected]);

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

  return (
    <div
      className={`hidden-field-canvas-row${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
      }}
    >
      <div className="hidden-field-badge" title="Hidden field">
        FIELD
      </div>
      <div className="hidden-field-main">
        <label className="hidden-field-name-label">
          Name:
          <input
            ref={inputRef}
            type="text"
            className={`hidden-field-name-input${nameError ? " hidden-field-invalid" : ""}`}
            value={fieldName}
            maxLength={50}
            onFocus={(e) => {
              draftRef.current = fieldName;
              e.target.select();
            }}
            onChange={(e) => {
              const v = e.target.value;
              update({ fieldName: v, name: v });
              const err = isValidHiddenFieldName(v, takenNames());
              setNameError(err);
            }}
            onBlur={(e) => {
              if (revertingRef.current) return;
              const ok = commitName(e.target.value);
              if (!ok) {
                revertingRef.current = true;
                requestAnimationFrame(() => {
                  inputRef.current?.focus();
                  inputRef.current?.select();
                  revertingRef.current = false;
                });
              }
            }}
            onKeyDown={(e) => {
              if (e.key === "Enter") {
                e.preventDefault();
                e.currentTarget.blur();
              }
            }}
          />
        </label>
        {nameError ? <p className="hidden-field-error">{nameError}</p> : null}
      </div>
    </div>
  );
}
