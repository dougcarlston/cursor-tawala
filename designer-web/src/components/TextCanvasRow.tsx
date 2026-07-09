import { useEffect, useRef, useState } from "react";
import { TextItem, TEXT_PLACEHOLDER } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import {
  fieldToken,
  hasFieldDrag,
  readFieldDragName,
  retainEditorFocusOnBlur,
  setActiveFieldTarget,
} from "@/lib/fieldInsertion";
import { FormItemDeleteButton } from "./FormItemDeleteButton";
import { TableHandlesOverlay } from "./TableHandlesOverlay";
import {
  clearActivePaletteEditor,
  clearFormattingFocus,
  selectionCursorInTable,
  setActivePaletteEditor,
  setFormattingFocus,
} from "@/lib/formattingPaletteContext";

interface Props {
  item: TextItem;
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

/** Plain text of an HTML fragment (for the empty/placeholder check). */
function htmlToPlainText(html: string): string {
  const tmp = document.createElement("div");
  tmp.innerHTML = html;
  return tmp.textContent ?? "";
}

/**
 * Text item — canvas-inline WYSIWYG (spec: `DESIGNER_FORM_ITEMS_TEXT_FIB_MCQ.md` Text section;
 * legacy `TextItemView`). Like Heading it is edited on the canvas (no Properties popup), but:
 *  - **full rich text** (formatting via the shared Formatting Palette on row 2), not per-run size;
 *  - **no collapse on blur** — the badge + body stay visible whether or not the box is focused
 *    (almost no visual difference between edit and non-edit).
 *
 * "Click-to-activate": clicking the box when focus was elsewhere makes the inline editor live,
 * registering `text` focus so the Formatting Palette lights up (Heading greys it).
 *
 * The label (`T1`, `T2`, …) is edited directly in the badge (click → input), like Heading.
 * Array/structured `content` (function tables) does NOT reach this row — the generic canvas
 * block handles it — so `content` here is always the plain-string body.
 */
export function TextCanvasRow({ item, index, formName, selected }: Props) {
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const [editing, setEditing] = useState(selected);
  const [editingLabel, setEditingLabel] = useState(false);
  const [dragOver, setDragOver] = useState(false);
  const editorRef = useRef<HTMLDivElement>(null);
  const labelInputRef = useRef<HTMLInputElement>(null);
  const savedRangeRef = useRef<Range | null>(null);
  const wasSelected = useRef(selected);

  const content = typeof item.content === "string" ? item.content : "";

  const update = (patch: Partial<TextItem>) =>
    updateFormItem(formName, index, { ...item, ...patch });

  // Enter editing when the row becomes selected (insert or re-select from elsewhere). Unlike
  // Heading, losing selection does NOT collapse — blur (below) exits editing but the body stays.
  useEffect(() => {
    if (selected && !wasSelected.current) setEditing(true);
    wasSelected.current = selected;
  }, [selected]);

  // Seed the contenteditable when entering edit. Deps are [editing] only so our own typing
  // (which flows back into `content`) never re-writes the DOM and clobbers the caret.
  useEffect(() => {
    if (!editing) {
      clearFormattingFocus("text");
      return;
    }
    const el = editorRef.current;
    if (!el) return;
    el.innerHTML = content;
    el.focus();
    // Register with the palette immediately so B/I/U (and font/align) work on the first
    // click — same timing fix as FibCanvasRow / McqCanvasRow. setFormattingFocus alone
    // enables the buttons; without a PaletteEditorHandle they silently no-op.
    setActiveFieldTarget(insertFieldToken);
    registerAsPaletteEditor();
    setFormattingFocus({ kind: "text", cursorInTable: selectionCursorInTable(el) });
    const sel = window.getSelection();
    if (!sel) return;
    const range = document.createRange();
    // Legacy AfterAddedToFormByUser → SelectAll: first keystroke replaces the placeholder.
    range.selectNodeContents(el);
    if (htmlToPlainText(content) !== TEXT_PLACEHOLDER) range.collapse(false);
    sel.removeAllRanges();
    sel.addRange(range);
    savedRangeRef.current = range.cloneRange();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [editing]);

  useEffect(
    () => () => {
      clearFormattingFocus("text");
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

  const commit = () => {
    const el = editorRef.current;
    if (!el) return;
    update({ content: el.innerHTML });
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

  const syncPaletteFocus = () => {
    const el = editorRef.current;
    if (!el || document.activeElement !== el) return;
    setFormattingFocus({ kind: "text", cursorInTable: selectionCursorInTable(el) });
  };

  const insertFieldToken = (name: string) => {
    const el = editorRef.current;
    if (!el) return;
    el.focus();
    restoreSelection();
    document.execCommand("insertText", false, fieldToken(name));
    commit();
  };

  const commitLabel = (raw: string) => {
    const trimmed = raw.trim();
    if (trimmed && trimmed !== item.label) update({ label: trimmed });
    setEditingLabel(false);
  };

  const handleBlur = (e: React.FocusEvent<HTMLDivElement>) => {
    // Keep editing while focus moves within the row (label input, editor).
    if (e.currentTarget.contains(e.relatedTarget as Node | null)) return;
    // Keep the palette live while the user operates its dropdowns / color picker.
    const next = e.relatedTarget as HTMLElement | null;
    if (next?.closest(".formatting-palette")) return;
    if (next?.closest(".table-handles-overlay")) return;
    // Keep the editor mounted while dragging/double-clicking from the Fields panel.
    if (retainEditorFocusOnBlur(e.relatedTarget)) return;
    clearFormattingFocus("text");
    setEditing(false);
  };

  const isEmpty = htmlToPlainText(content).trim() === "";
  const renderedHtml = isEmpty ? "" : content;

  return (
    <div
      className={`text-canvas-row ${editing ? "editing" : "idle"}${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
        const target = e.target as HTMLElement;
        if (target.closest(".text-badge, .text-badge-input, .canvas-item-delete")) return;
        if (target.closest(".text-canvas-main")) {
          if (!editing) setEditing(true);
          return;
        }
        // Border/chrome click: select without focusing the editor so Del deletes the row.
        setEditing(false);
        editorRef.current?.blur();
      }}
      onBlur={handleBlur}
    >
      <FormItemDeleteButton formName={formName} index={index} visible={selected} />
      {editingLabel ? (
        <input
          ref={labelInputRef}
          className="text-badge-input"
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
          className={`text-badge${editing ? " editing" : ""}`}
          title="Click to edit text label"
          onClick={(e) => {
            e.stopPropagation();
            setEditingLabel(true);
          }}
        >
          {item.label}
        </div>
      )}
      <div className="text-canvas-main">
        {editing ? (
          <div className="text-rich-wrap">
            <div
              ref={editorRef}
              className={`text-rich-editor${dragOver ? " field-drop-active" : ""}`}
              contentEditable
              suppressContentEditableWarning
              data-placeholder={TEXT_PLACEHOLDER}
              onInput={() => {
                commit();
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
                e.stopPropagation();
                const el = editorRef.current;
                const range = caretRangeAtPoint(e.clientX, e.clientY);
                const sel = window.getSelection();
                if (el && range && sel && el.contains(range.commonAncestorContainer)) {
                  sel.removeAllRanges();
                  sel.addRange(range);
                  savedRangeRef.current = range.cloneRange();
                } else if (el) {
                  el.focus();
                  restoreSelection();
                }
                insertFieldToken(name);
              }}
            />
            <TableHandlesOverlay editorRef={editorRef} onCommit={commit} />
          </div>
        ) : (
          <div
            className={`text-rendered${isEmpty ? " placeholder" : ""}`}
            dangerouslySetInnerHTML={{ __html: isEmpty ? TEXT_PLACEHOLDER : renderedHtml }}
          />
        )}
      </div>
    </div>
  );
}
