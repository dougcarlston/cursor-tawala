import { useEffect, useRef, useState } from "react";
import { FibItem, FIB_PLACEHOLDER } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import {
  FIB_VALIDATION_OPTIONS,
  activeBlankIndex,
  htmlToPlainText,
  selectionIsSingleBlank,
  selectionPlainOffset,
  syncBlanksFromPrompt,
} from "@/lib/fibBlanks";
import {
  clearActivePaletteEditor,
  clearFormattingFocus,
  setActivePaletteEditor,
  setFormattingFocus,
} from "@/lib/formattingPaletteContext";

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
  const [editing, setEditing] = useState(selected);
  const [editingLabel, setEditingLabel] = useState(false);
  const [activeBlank, setActiveBlank] = useState(-1);
  const editorRef = useRef<HTMLDivElement>(null);
  const labelInputRef = useRef<HTMLInputElement>(null);
  const savedRangeRef = useRef<Range | null>(null);
  const wasSelected = useRef(selected);

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
    setActiveBlank(idx);
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
    if (e.currentTarget.contains(e.relatedTarget as Node | null)) return;
    const next = e.relatedTarget as HTMLElement | null;
    if (next?.closest(".formatting-palette")) return;
    if (next?.closest(".fib-property-strip")) return;
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

  const plainPrompt = htmlToPlainText(prompt);
  const isEmpty = plainPrompt.trim() === "";
  const currentBlank = activeBlank >= 0 ? blanks[activeBlank] : null;
  const stripEnabled = activeBlank >= 0 && !!currentBlank;

  const renderedPrompt = () => {
    if (isEmpty) {
      return <span className="fib-rendered-placeholder">{FIB_PLACEHOLDER}</span>;
    }
    const parts: React.ReactNode[] = [];
    const re = /_+/g;
    let lastIndex = 0;
    let blankIdx = 0;
    let match: RegExpExecArray | null;
    while ((match = re.exec(plainPrompt)) !== null) {
      if (match.index > lastIndex) {
        parts.push(
          <span key={`t-${lastIndex}`}>{plainPrompt.slice(lastIndex, match.index)}</span>,
        );
      }
      const blank = blanks[blankIdx];
      const size = Math.min(Math.max(blank?.length ?? match[0].length, 5), 120);
      parts.push(
        <input
          key={`b-${match.index}`}
          type="text"
          className="fib-canvas-blank"
          size={size}
          style={blank?.height && blank.height > 1 ? { height: `${blank.height * 1.4}em` } : undefined}
          readOnly
          tabIndex={-1}
          aria-label={blank?.alternateLabel ?? blank?.name ?? `blank ${blankIdx + 1}`}
        />,
      );
      blankIdx++;
      lastIndex = match.index + match[0].length;
    }
    if (lastIndex < plainPrompt.length) {
      parts.push(<span key={`t-${lastIndex}`}>{plainPrompt.slice(lastIndex)}</span>);
    }
    return parts;
  };

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
              ref={editorRef}
              className="fib-rich-editor"
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
                registerAsPaletteEditor();
                setFormattingFocus({ kind: "fib", cursorInTable: false });
                const el = editorRef.current;
                if (el) syncActiveBlank(el);
              }}
            />
            <div className={`fib-property-strip${stripEnabled ? "" : " disabled"}`}>
              <label>
                Alternate Label
                <input
                  type="text"
                  value={currentBlank?.alternateLabel ?? ""}
                  disabled={!stripEnabled}
                  onChange={(e) => updateBlank(activeBlank, { alternateLabel: e.target.value })}
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
                  value={currentBlank?.validationType ?? ""}
                  disabled={!stripEnabled}
                  onChange={(e) =>
                    updateBlank(activeBlank, { validationType: e.target.value || undefined })
                  }
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
                disabled={!stripEnabled || !currentBlank?.validationType}
                title="Edit validation message"
                onClick={() => {
                  /* Validation editor popup — deferred (spec: screenshot pending). */
                }}
              >
                Edit…
              </button>
            </div>
          </>
        ) : (
          <div className="fib-rendered">{renderedPrompt()}</div>
        )}
      </div>
    </div>
  );
}
