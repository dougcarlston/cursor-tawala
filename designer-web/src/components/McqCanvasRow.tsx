import { useEffect, useRef, useState } from "react";
import { McItem, MCQ_PLACEHOLDER, TawalaChoice } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
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

interface Props {
  item: McItem;
  index: number;
  formName: string;
  selected: boolean;
}

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

function htmlToPlainText(html: string): string {
  const tmp = document.createElement("div");
  tmp.innerHTML = html;
  return tmp.textContent ?? "";
}

const choiceLetter = (i: number) => String.fromCharCode(97 + (i % 26));

/**
 * MCQ item — canvas-inline WYSIWYG (spec: `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` MCQ section;
 * legacy `McqItemView`). Q-badge, rich question (B/I/U via palette), inline lettered choices
 * where Enter adds the next choice, and a property strip (multi-select, required, choice source).
 */
export function McqCanvasRow({ item, index, formName, selected }: Props) {
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const [editing, setEditing] = useState(selected);
  const [editingLabel, setEditingLabel] = useState(false);
  const [dragOver, setDragOver] = useState(false);
  const editorRef = useRef<HTMLDivElement>(null);
  const labelInputRef = useRef<HTMLInputElement>(null);
  const choiceRefs = useRef<(HTMLInputElement | null)[]>([]);
  const savedRangeRef = useRef<Range | null>(null);
  const wasSelected = useRef(selected);
  const pendingFocusRef = useRef<number | null>(null);

  const question = item.question ?? "";
  const choices = item.choices ?? [];
  const choiceSource = item.choiceSource ?? "manual";
  const multiSelect = item.onlyone === false;

  const update = (patch: Partial<McItem>) =>
    updateFormItem(formName, index, { ...item, ...patch });

  useEffect(() => {
    if (selected && !wasSelected.current) setEditing(true);
    wasSelected.current = selected;
  }, [selected]);

  useEffect(() => {
    if (!editing) {
      clearFormattingFocus("mcq");
      return;
    }
    const el = editorRef.current;
    if (!el) return;
    el.innerHTML = question;
    el.focus();
    setActiveFieldTarget(insertFieldToken);
    registerAsPaletteEditor();
    setFormattingFocus({ kind: "mcq", cursorInTable: false });
    // Re-entering after all choices were pruned leaves nothing to type into.
    if (choiceSource === "manual" && choices.length === 0) {
      update({ choices: [{ name: "a", text: "" }] });
    }
    const sel = window.getSelection();
    if (!sel) return;
    const range = document.createRange();
    range.selectNodeContents(el);
    if (htmlToPlainText(question) !== MCQ_PLACEHOLDER) range.collapse(false);
    sel.removeAllRanges();
    sel.addRange(range);
    savedRangeRef.current = range.cloneRange();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [editing]);

  useEffect(
    () => () => {
      clearFormattingFocus("mcq");
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

  // Focus a freshly-added choice after the store re-renders (Enter → new choice below).
  useEffect(() => {
    if (pendingFocusRef.current === null) return;
    const idx = pendingFocusRef.current;
    pendingFocusRef.current = null;
    requestAnimationFrame(() => {
      const el = choiceRefs.current[idx];
      if (el) {
        el.focus();
        el.select();
      }
    });
  });

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

  const commitQuestion = () => {
    const el = editorRef.current;
    if (!el) return;
    update({ question: el.innerHTML });
  };

  const registerAsPaletteEditor = () => {
    const el = editorRef.current;
    if (!el) return;
    setActivePaletteEditor({
      el,
      commit: commitQuestion,
      saveSelection: rememberSelection,
      restoreSelection,
    });
  };

  const insertFieldToken = (name: string) => {
    const el = editorRef.current;
    if (!el) return;
    el.focus();
    restoreSelection();
    document.execCommand("insertText", false, fieldToken(name));
    commitQuestion();
  };

  const syncPaletteFocus = () => {
    const el = editorRef.current;
    if (!el || document.activeElement !== el) return;
    setFormattingFocus({ kind: "mcq", cursorInTable: false });
  };

  const commitLabel = (raw: string) => {
    const trimmed = raw.trim();
    if (trimmed && trimmed !== item.label) update({ label: trimmed });
    setEditingLabel(false);
  };

  const handleBlur = (e: React.FocusEvent<HTMLDivElement>) => {
    if (e.currentTarget.contains(e.relatedTarget as Node | null)) return;
    const next = e.relatedTarget as HTMLElement | null;
    if (next?.closest(".formatting-palette")) return;
    if (next?.closest(".fib-property-strip")) return;
    pruneEmptyChoices();
    clearFormattingFocus("mcq");
    setEditing(false);
  };

  /** Renumber choice names to letters by position (legacy auto-lettering a/b/c…). */
  const withLetters = (list: TawalaChoice[]): TawalaChoice[] =>
    list.map((c, i) => ({ ...c, name: choiceLetter(i) }));

  const updateChoiceText = (i: number, text: string) => {
    const next = [...choices];
    if (!next[i]) return;
    next[i] = { ...next[i], text };
    update({ choices: next });
  };

  const addChoiceAfter = (i: number) => {
    const next = [...choices];
    next.splice(i + 1, 0, { name: choiceLetter(i + 1), text: "" });
    pendingFocusRef.current = i + 1;
    update({ choices: withLetters(next) });
  };

  const removeChoice = (i: number) => {
    if (choices.length <= 1) return;
    const next = choices.filter((_, j) => j !== i);
    pendingFocusRef.current = Math.max(0, i - 1);
    update({ choices: withLetters(next) });
  };

  /** Drop empty choices when leaving edit mode (also clears the trailing blank Enter adds). */
  const pruneEmptyChoices = () => {
    const kept = choices.filter((c) => c.text.trim() !== "");
    if (kept.length !== choices.length) update({ choices: withLetters(kept) });
  };

  const isEmpty = htmlToPlainText(question).trim() === "";

  return (
    <div
      className={`mcq-canvas-row ${editing ? "editing" : "idle"}${selected ? " selected" : ""}`}
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
          className="mcq-badge-input"
          defaultValue={item.label}
          maxLength={20}
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
          className={`mcq-badge${editing ? " editing" : ""}`}
          title="Click to edit question label"
          onClick={(e) => {
            e.stopPropagation();
            setEditingLabel(true);
          }}
        >
          {item.label}
        </div>
      )}
      <div className="mcq-canvas-main">
        {editing ? (
          <>
            <div
              key="mcq-editor"
              ref={editorRef}
              className={`mcq-question-editor${dragOver ? " field-drop-active" : ""}`}
              contentEditable
              suppressContentEditableWarning
              data-placeholder={MCQ_PLACEHOLDER}
              onInput={() => {
                commitQuestion();
                syncPaletteFocus();
              }}
              onKeyUp={() => {
                rememberSelection();
                syncPaletteFocus();
              }}
              onMouseUp={() => {
                rememberSelection();
                syncPaletteFocus();
              }}
              onFocus={() => {
                setActiveFieldTarget(insertFieldToken);
                registerAsPaletteEditor();
                syncPaletteFocus();
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
            {choiceSource === "manual" ? (
              <div className="mcq-choices-edit">
                {choices.map((c, i) => (
                  <div key={i} className="mcq-choice-edit-row">
                    <span className="mcq-choice-letter">{choiceLetter(i)})</span>
                    <input
                      ref={(el) => {
                        choiceRefs.current[i] = el;
                      }}
                      type="text"
                      className="mcq-choice-input"
                      value={c.text}
                      placeholder={i === 0 ? "Type a choice; press Enter for the next" : ""}
                      onChange={(e) => updateChoiceText(i, e.target.value)}
                      onKeyDown={(e) => {
                        if (e.key === "Enter") {
                          e.preventDefault();
                          addChoiceAfter(i);
                        } else if (
                          e.key === "Backspace" &&
                          c.text === "" &&
                          choices.length > 1
                        ) {
                          e.preventDefault();
                          removeChoice(i);
                        }
                      }}
                    />
                  </div>
                ))}
              </div>
            ) : (
              <p className="mcq-stored-note">
                Choices come from stored data. Use <strong>Edit</strong> to configure the data
                source.
              </p>
            )}
            <div className="fib-property-strip mcq-property-strip">
              <label className="fib-prop-checkbox">
                <input
                  type="checkbox"
                  checked={multiSelect}
                  onChange={(e) => update({ onlyone: !e.target.checked })}
                />
                User may select more than one
              </label>
              <label className="fib-prop-checkbox">
                <input
                  type="checkbox"
                  checked={item.required ?? false}
                  onChange={(e) => update({ required: e.target.checked })}
                />
                Response required
              </label>
              <label>
                Choice source
                <select
                  value={choiceSource}
                  onChange={(e) =>
                    update({ choiceSource: e.target.value as "manual" | "stored" })
                  }
                >
                  <option value="manual">Choices are entered above</option>
                  <option value="stored">Choices are from stored data</option>
                </select>
              </label>
              <button
                type="button"
                className="fib-validation-edit"
                disabled={choiceSource !== "stored"}
                title="Configure the stored-data source"
                onClick={() => {
                  // Configure Function (dynamic MCQ) dialog — deferred (spec: SportsDashboards
                  // dynamic choices). Manual choices are the supported path today.
                }}
              >
                Edit
              </button>
            </div>
          </>
        ) : (
          <div key="mcq-rendered" className="mcq-rendered">
            <div
              className={`mcq-question${isEmpty ? " placeholder" : ""}`}
              dangerouslySetInnerHTML={{ __html: isEmpty ? MCQ_PLACEHOLDER : question }}
            />
            {choiceSource === "manual" ? (
              <ul className="mcq-choices-preview">
                {choices.map((c, i) => (
                  <li key={i}>
                    <span className={`mcq-choice-marker${multiSelect ? " checkbox" : ""}`} />
                    <span className="mcq-choice-letter">{choiceLetter(i)})</span>{" "}
                    {c.text || <span className="mcq-choice-empty">(empty choice)</span>}
                  </li>
                ))}
              </ul>
            ) : (
              <p className="mcq-stored-note">Choices from stored data</p>
            )}
          </div>
        )}
      </div>
    </div>
  );
}
