import { useEffect, useRef, useState } from "react";
import { BlankValidation, FibItem, FIB_PLACEHOLDER } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import {
  FIB_VALIDATION_OPTIONS,
  activeBlankIndex,
  defaultValidation,
  htmlToPlainText,
  isAlternateLabelUnique,
  selectionIsSingleBlank,
  selectionPlainOffset,
  syncBlanksFromPrompt,
  validatorMeta,
} from "@/lib/fibBlanks";
import {
  fieldToken,
  hasFieldDrag,
  readFieldDragName,
  setActiveFieldTarget,
} from "@/lib/fieldInsertion";
import {
  clearActivePaletteEditor,
  clearFormattingFocus,
  setActivePaletteEditor,
  setFormattingFocus,
} from "@/lib/formattingPaletteContext";
import { FibValidationDialog } from "./FibValidationDialog";

/** Caret Range at a viewport point, across Chromium (`caretRangeFromPoint`) and Firefox. */
function caretRangeAtPoint(x: number, y: number): Range | null {
  const doc = document as Document & {
    caretRangeFromPoint?: (x: number, y: number) => Range | null;
    caretPositionFromPoint?: (
      x: number,
      y: number,
    ) => { offsetNode: Node; offset: number } | null;
  };
  if (typeof doc.caretRangeFromPoint === "function") return doc.caretRangeFromPoint(x, y);
  if (typeof doc.caretPositionFromPoint === "function") {
    const pos = doc.caretPositionFromPoint(x, y);
    if (!pos) return null;
    const range = document.createRange();
    range.setStart(pos.offsetNode, pos.offset);
    range.collapse(true);
    return range;
  }
  return null;
}

interface Props {
  item: FibItem;
  index: number;
  formName: string;
  selected: boolean;
}

/**
 * FIB item — canvas-inline WYSIWYG (spec: `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` FIB section;
 * legacy `FibItemView`). Q-badge, rich prompt (B/I/U via palette), underscore runs become
 * inline blank inputs when idle, property strip for the active blank.
 */
export function FibCanvasRow({ item, index, formName, selected }: Props) {
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const project = useProjectStore((s) => s.project);
  const [editing, setEditing] = useState(selected);
  const [editingLabel, setEditingLabel] = useState(false);
  const [activeBlank, setActiveBlank] = useState(-1);
  const [dragOver, setDragOver] = useState(false);
  const [validationDialogOpen, setValidationDialogOpen] = useState(false);
  const [altLabelError, setAltLabelError] = useState<string | null>(null);
  const editorRef = useRef<HTMLDivElement>(null);
  const labelInputRef = useRef<HTMLInputElement>(null);
  const altLabelInputRef = useRef<HTMLInputElement>(null);
  const savedRangeRef = useRef<Range | null>(null);
  const wasSelected = useRef(selected);
  const altLabelDraftRef = useRef<string | null>(null);
  const revertingAltLabelRef = useRef(false);

  const prompt = item.prompt ?? "";
  const blanks = item.blanks ?? [];

  const update = (patch: Partial<FibItem>) =>
    updateFormItem(formName, index, { ...item, ...patch });

  useEffect(() => {
    if (selected && !wasSelected.current) setEditing(true);
    wasSelected.current = selected;
  }, [selected]);

  useEffect(() => {
    if (!editing) {
      clearFormattingFocus("fib");
      setActiveBlank(-1);
      return;
    }
    const el = editorRef.current;
    if (!el) return;
    el.innerHTML = prompt;
    el.focus();
    // Register with the palette immediately so B/I/U work on the first click, without
    // waiting for a later focus event (fixes formatting being dead until the editor is
    // re-focused). Mirrors the onFocus handler.
    setActiveFieldTarget(insertFieldToken);
    registerAsPaletteEditor();
    setFormattingFocus({ kind: "fib", cursorInTable: false });
    const sel = window.getSelection();
    if (!sel) return;
    const range = document.createRange();
    range.selectNodeContents(el);
    if (htmlToPlainText(prompt) !== FIB_PLACEHOLDER) range.collapse(false);
    sel.removeAllRanges();
    sel.addRange(range);
    savedRangeRef.current = range.cloneRange();
    syncActiveBlank(el);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [editing]);

  useEffect(
    () => () => {
      clearFormattingFocus("fib");
      clearActivePaletteEditor(editorRef.current ?? undefined);
    },
    [],
  );

  useEffect(() => {
    if (!editingLabel) return;
    const el = labelInputRef.current;
    if (!el) return;
    el.focus();
    el.select();
  }, [editingLabel]);

  const enterEditing = () => {
    setSelectedItemIndex(index);
    setEditing(true);
  };

  const rememberSelection = () => {
    const el = editorRef.current;
    const sel = window.getSelection();
    if (!el || !sel || sel.rangeCount === 0) return;
    const range = sel.getRangeAt(0);
    if (el.contains(range.commonAncestorContainer)) savedRangeRef.current = range.cloneRange();
  };

  const restoreSelection = () => {
    const el = editorRef.current;
    const sel = window.getSelection();
    const saved = savedRangeRef.current;
    if (!el || !sel || !saved || !el.contains(saved.commonAncestorContainer)) return;
    sel.removeAllRanges();
    sel.addRange(saved);
  };

  const syncActiveBlank = (el: HTMLElement) => {
    const plain = htmlToPlainText(el.innerHTML);
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) {
      setActiveBlank(-1);
      return;
    }
    const range = sel.getRangeAt(0);
    const start = selectionPlainOffset(el);
    const end =
      range.collapsed
        ? start
        : start +
          range.toString().length;
    const idx =
      end > start
        ? selectionIsSingleBlank(plain, start, end)
        : activeBlankIndex(plain, start);
    setActiveBlank((prev) => {
      if (prev !== idx) setAltLabelError(null);
      return idx;
    });
  };

  const commit = () => {
    const el = editorRef.current;
    if (!el) return;
    const html = el.innerHTML;
    const plain = htmlToPlainText(html);
    const nextBlanks = syncBlanksFromPrompt(plain, blanks, item.label);
    update({ prompt: html, blanks: nextBlanks });
  };

  const registerAsPaletteEditor = () => {
    const el = editorRef.current;
    if (!el) return;
    setActivePaletteEditor({
      el,
      commit,
      saveSelection: rememberSelection,
      restoreSelection,
    });
  };

  const commitLabel = (raw: string) => {
    const trimmed = raw.trim();
    if (trimmed && trimmed !== item.label) update({ label: trimmed });
    setEditingLabel(false);
  };

  const handleBlur = (e: React.FocusEvent<HTMLDivElement>) => {
    if (validationDialogOpen) return;
    // A duplicate Alternate Label refocuses its input to keep the strip open; don't collapse.
    if (revertingAltLabelRef.current) return;
    if (e.currentTarget.contains(e.relatedTarget as Node | null)) return;
    const next = e.relatedTarget as HTMLElement | null;
    if (next?.closest(".formatting-palette")) return;
    if (next?.closest(".fib-property-strip")) return;
    if (next?.closest(".fib-validation-dialog")) return;
    clearFormattingFocus("fib");
    setEditing(false);
    setActiveBlank(-1);
  };

  const updateBlank = (blankIndex: number, patch: Partial<(typeof blanks)[0]>) => {
    const next = [...blanks];
    if (!next[blankIndex]) return;
    next[blankIndex] = { ...next[blankIndex], ...patch };
    update({ blanks: next });
  };

  const insertFieldToken = (name: string) => {
    const el = editorRef.current;
    if (!el) return;
    el.focus();
    restoreSelection();
    document.execCommand("insertText", false, fieldToken(name));
    commit();
  };

  /** Every other alternate-label / field name on this form (for uniqueness checks). */
  const takenFieldNames = (blankIndex: number): Set<string> => {
    const taken = new Set<string>();
    const form = project.forms.find((f) => f.name === formName);
    if (!form) return taken;
    for (const it of form.items) {
      if (it.type === "fib") {
        (it.blanks ?? []).forEach((b, bi) => {
          if (it === item && bi === blankIndex) return;
          const nm = b.alternateLabel?.trim();
          if (nm) taken.add(nm.toLowerCase());
        });
      } else if (it.type === "mc") {
        const nm = (it.name ?? it.alternateLabel)?.trim();
        if (nm) taken.add(nm.toLowerCase());
      } else if (it.type === "field") {
        const nm = (it.fieldName ?? it.name)?.trim();
        if (nm) taken.add(nm.toLowerCase());
      }
    }
    return taken;
  };

  /** Returns true when the label was accepted; false when it was a duplicate and reverted. */
  const commitAltLabel = (blankIndex: number, raw: string): boolean => {
    const blank = blanks[blankIndex];
    if (!blank) return true;
    const trimmed = raw.trim();
    const previous = altLabelDraftRef.current ?? blank.alternateLabel ?? "";
    if (!isAlternateLabelUnique(trimmed, takenFieldNames(blankIndex))) {
      // Duplicate — revert to the value at focus time (legacy rejects duplicates on validate).
      // The revert is done in the store so the controlled input visibly snaps back; the caller
      // keeps the property strip open (refocus) so the designer can see it happen.
      updateBlank(blankIndex, { alternateLabel: previous.trim() || undefined });
      setAltLabelError(`"${trimmed}" is already used by another field — reverted.`);
      return false;
    }
    altLabelDraftRef.current = null;
    setAltLabelError(null);
    updateBlank(blankIndex, { alternateLabel: trimmed || undefined });
    return true;
  };

  const changeValidationType = (blankIndex: number, typeId: string) => {
    if (!typeId) {
      updateBlank(blankIndex, { validation: undefined });
      return;
    }
    const validation = defaultValidation(typeId);
    updateBlank(blankIndex, { validation });
    // Legacy opens the Configure Function dialog when a validator with parameters is picked.
    if (validatorMeta(typeId)?.hasParams) setValidationDialogOpen(true);
  };

  const saveValidation = (blankIndex: number, validation: BlankValidation) => {
    updateBlank(blankIndex, { validation });
    setValidationDialogOpen(false);
  };

  const plainPrompt = htmlToPlainText(prompt);
  const isEmpty = plainPrompt.trim() === "";
  const currentBlank = activeBlank >= 0 ? blanks[activeBlank] : null;
  const stripEnabled = activeBlank >= 0 && !!currentBlank;

  return (
    <div
      className={`fib-canvas-row ${editing ? "editing" : "idle"}${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        if (editing) setSelectedItemIndex(index);
        else enterEditing();
      }}
      onBlur={handleBlur}
    >
      {editingLabel ? (
        <input
          ref={labelInputRef}
          className="fib-badge-input"
          defaultValue={item.label}
          maxLength={12}
          onClick={(e) => e.stopPropagation()}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              e.preventDefault();
              commitLabel(e.currentTarget.value);
            } else if (e.key === "Escape") {
              e.preventDefault();
              setEditingLabel(false);
            }
          }}
          onBlur={(e) => commitLabel(e.currentTarget.value)}
        />
      ) : (
        <div
          className={`fib-badge${editing ? " editing" : ""}`}
          title="Click to edit question label"
          onClick={(e) => {
            e.stopPropagation();
            setEditingLabel(true);
          }}
        >
          {item.label}
        </div>
      )}
      <div className="fib-canvas-main">
        {editing ? (
          <>
            <div
              key="fib-editor"
              ref={editorRef}
              className={`fib-rich-editor${dragOver ? " field-drop-active" : ""}`}
              contentEditable
              suppressContentEditableWarning
              data-placeholder={FIB_PLACEHOLDER}
              onInput={() => {
                commit();
                const el = editorRef.current;
                if (el) syncActiveBlank(el);
              }}
              onKeyUp={() => {
                rememberSelection();
                const el = editorRef.current;
                if (el) {
                  setFormattingFocus({ kind: "fib", cursorInTable: false });
                  syncActiveBlank(el);
                }
              }}
              onMouseUp={() => {
                rememberSelection();
                const el = editorRef.current;
                if (el) syncActiveBlank(el);
              }}
              onFocus={() => {
                setActiveFieldTarget(insertFieldToken);
                registerAsPaletteEditor();
                setFormattingFocus({ kind: "fib", cursorInTable: false });
                const el = editorRef.current;
                if (el) syncActiveBlank(el);
              }}
              onDragOver={(e) => {
                if (!hasFieldDrag(e.dataTransfer)) return;
                e.preventDefault();
                e.dataTransfer.dropEffect = "copy";
                if (!dragOver) setDragOver(true);
              }}
              onDragLeave={() => {
                if (dragOver) setDragOver(false);
              }}
              onDrop={(e) => {
                setDragOver(false);
                const name = readFieldDragName(e.dataTransfer);
                if (!name) return;
                e.preventDefault();
                const el = editorRef.current;
                const range = caretRangeAtPoint(e.clientX, e.clientY);
                const sel = window.getSelection();
                if (el && range && sel && el.contains(range.commonAncestorContainer)) {
                  sel.removeAllRanges();
                  sel.addRange(range);
                  savedRangeRef.current = range.cloneRange();
                }
                insertFieldToken(name);
              }}
            />
            <div className={`fib-property-strip${stripEnabled ? "" : " disabled"}`}>
              <label>
                Alternate Label
                <input
                  ref={altLabelInputRef}
                  type="text"
                  className={altLabelError ? "fib-alt-invalid" : undefined}
                  value={currentBlank?.alternateLabel ?? ""}
                  disabled={!stripEnabled}
                  onFocus={() => {
                    altLabelDraftRef.current = currentBlank?.alternateLabel ?? "";
                  }}
                  onChange={(e) => {
                    const v = e.target.value;
                    updateBlank(activeBlank, { alternateLabel: v });
                    // Live feedback so the designer sees the clash before leaving the field.
                    const dup = !isAlternateLabelUnique(v, takenFieldNames(activeBlank));
                    setAltLabelError(dup ? `"${v.trim()}" is already used by another field.` : null);
                  }}
                  onBlur={(e) => {
                    if (!commitAltLabel(activeBlank, e.target.value)) {
                      // Keep the strip open and refocus so the reversion is visible.
                      revertingAltLabelRef.current = true;
                      requestAnimationFrame(() => {
                        altLabelInputRef.current?.focus();
                        altLabelInputRef.current?.select();
                        revertingAltLabelRef.current = false;
                      });
                    }
                  }}
                />
              </label>
              <label>
                Height
                <input
                  type="number"
                  min={1}
                  max={20}
                  value={currentBlank?.height ?? 1}
                  disabled={!stripEnabled}
                  onChange={(e) =>
                    updateBlank(activeBlank, { height: Number(e.target.value) || 1 })
                  }
                />
              </label>
              <label className="fib-prop-checkbox">
                <input
                  type="checkbox"
                  checked={currentBlank?.required ?? false}
                  disabled={!stripEnabled}
                  onChange={(e) => updateBlank(activeBlank, { required: e.target.checked })}
                />
                Required
              </label>
              <label>
                Validation
                <select
                  value={currentBlank?.validation?.type ?? ""}
                  disabled={!stripEnabled}
                  onChange={(e) => changeValidationType(activeBlank, e.target.value)}
                >
                  {FIB_VALIDATION_OPTIONS.map((opt) => (
                    <option key={opt.value || "none"} value={opt.value}>
                      {opt.label}
                    </option>
                  ))}
                </select>
              </label>
              <button
                type="button"
                className="fib-validation-edit"
                disabled={
                  !stripEnabled ||
                  !currentBlank?.validation ||
                  !validatorMeta(currentBlank.validation.type)?.hasParams
                }
                title="Edit validation function"
                onClick={() => setValidationDialogOpen(true)}
              >
                Edit…
              </button>
              {altLabelError && (
                <p className="fib-alt-error" role="alert">
                  {altLabelError}
                </p>
              )}
            </div>
          </>
        ) : (
          <div
            key="fib-rendered"
            className={`fib-rendered${isEmpty ? " placeholder" : ""}`}
            dangerouslySetInnerHTML={{ __html: isEmpty ? FIB_PLACEHOLDER : prompt }}
          />
        )}
      </div>
      {validationDialogOpen && currentBlank?.validation && (
        <FibValidationDialog
          validation={currentBlank.validation}
          onCancel={() => setValidationDialogOpen(false)}
          onSave={(v) => saveValidation(activeBlank, v)}
        />
      )}
    </div>
  );
}
