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
import { EmbeddedImageHandlesOverlay } from "./EmbeddedImageHandlesOverlay";
import {
  EMBEDDED_IMAGE_HANDLES_CLASS,
  clearEmbeddedImageSelection,
  embeddedImageFromEventTarget,
  selectEmbeddedImage,
} from "@/lib/embeddedImageResize";
import {
  clearActivePaletteEditor,
  clearFormattingFocus,
  selectionCursorInTable,
  setActivePaletteEditor,
  setFormattingFocus,
  selectionHasResettableFormatting,
} from "@/lib/formattingPaletteContext";
import { openFunctionTokenForEdit } from "@/lib/functionPicker";
import { ensureFunctionTokenCaretGaps, FUNCTION_TOKEN_CLASS } from "@/lib/functionTokens";
import {
  seedBlankBlockTypingFormat,
  selectParagraphAtPoint,
  selectWordOrTokenAtPoint,
} from "@/lib/documentCanvas";
import {
  clearTableCellSelection,
  handleTableCellPointerDown,
  handleTableCellPointerMove,
  handleTableCellPointerUp,
  navigateTableCellOnTab,
} from "@/lib/tableCellSelection";
import { FIELD_TOKEN_CLASS } from "@/lib/fieldTokens";
import {
  setTypingFormat,
  typingFormatForInsert,
} from "@/lib/paletteTypingFormat";

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
  return (tmp.textContent ?? "").replace(/\u200B/g, "");
}

/** True when the body has no visible text and no media / tokens worth keeping. */
function textHtmlIsEmpty(html: string): boolean {
  const tmp = document.createElement("div");
  tmp.innerHTML = html;
  if (tmp.querySelector("img, table, video, iframe, .function-token, .field-token, .tawala-embedded-image")) {
    return false;
  }
  return (tmp.textContent ?? "").replace(/\u200B/g, "").trim() === "";
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
 * The label (`T1`, `T2`, …) is edited in the badge: first click selects, second click edits.
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
  /** Face/size captured before native insertParagraph so the new empty block can inherit them. */
  const enterTypingRef = useRef<ReturnType<typeof typingFormatForInsert> | null>(null);
  const pendingFunctionEditRef = useRef<{
    instanceId: string | null;
    functionId: string | null;
    config: string | null;
  } | null>(null);

  const content = typeof item.content === "string" ? item.content : "";

  const update = (patch: Partial<TextItem>) =>
    updateFormItem(formName, index, { ...item, ...patch });

  // Enter editing when the row becomes selected (insert or re-select from elsewhere). Unlike
  // Heading, losing selection does NOT collapse — blur (below) exits editing but the body stays.
  useEffect(() => {
    if (selected && !wasSelected.current) setEditing(true);
    if (!selected && wasSelected.current) setEditing(false);
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
    ensureFunctionTokenCaretGaps(el);
    el.focus();
    // Register with the palette immediately so B/I/U (and font/align) work on the first
    // click — same timing fix as FibCanvasRow / McqCanvasRow. setFormattingFocus alone
    // enables the buttons; without a PaletteEditorHandle they silently no-op.
    setActiveFieldTarget(insertFieldToken);
    registerAsPaletteEditor();
    setFormattingFocus({
      kind: "text",
      cursorInTable: selectionCursorInTable(el),
      hasResettableFormatting: selectionHasResettableFormatting(el),
    });
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

  // After the editor mounts HTML, open Configure for a function token clicked while idle.
  useEffect(() => {
    if (!editing) return;
    const pending = pendingFunctionEditRef.current;
    if (!pending) return;
    pendingFunctionEditRef.current = null;
    const el = editorRef.current;
    if (!el) return;
    const token = findPendingFunctionToken(el, pending);
    if (!token) return;
    registerAsPaletteEditor();
    openFunctionTokenForEdit(token, el, rememberSelection);
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
    setFormattingFocus({
      kind: "text",
      cursorInTable: selectionCursorInTable(el),
      hasResettableFormatting: selectionHasResettableFormatting(el),
    });
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
    if (next?.closest(`.${EMBEDDED_IMAGE_HANDLES_CLASS}`)) return;
    // Keep the editor mounted while dragging/double-clicking from the Fields panel.
    if (retainEditorFocusOnBlur(e.relatedTarget)) return;
    clearFormattingFocus("text");
    // Keep expanded while selected so form-item reorder drag does not collapse mid-drag.
    if (selected) return;
    setEditing(false);
  };

  const isEmpty = textHtmlIsEmpty(content);
  const renderedHtml = isEmpty ? "" : content;

  return (
    <div
      className={`text-canvas-row ${editing ? "editing" : "idle"}${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
        const target = e.target as HTMLElement;
        if (target.closest(".text-badge, .text-badge-input, .canvas-item-delete")) return;

        const token = target.closest(`.${FUNCTION_TOKEN_CLASS}`);
        if (token instanceof HTMLElement) {
          if (editing && editorRef.current) {
            registerAsPaletteEditor();
            openFunctionTokenForEdit(token, editorRef.current, rememberSelection);
            return;
          }
          pendingFunctionEditRef.current = {
            instanceId: token.getAttribute("data-function-instance"),
            functionId: token.getAttribute("data-function-id"),
            config: token.getAttribute("data-function-config"),
          };
          setEditing(true);
          return;
        }

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
          draggable={selected}
          title={selected ? "Drag to reorder, or click to edit text label" : "Click to select"}
          onDragStart={(e) => {
            if (!selected) {
              e.preventDefault();
              return;
            }
            e.dataTransfer.effectAllowed = "move";
          }}
          onClick={(e) => {
            e.stopPropagation();
            setSelectedItemIndex(index);
            if (selected) setEditingLabel(true);
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
              onInput={(e) => {
                const el = editorRef.current;
                const pending = enterTypingRef.current;
                if (el && pending) {
                  enterTypingRef.current = null;
                  const ie = e.nativeEvent as InputEvent;
                  if (
                    ie.inputType === "insertParagraph" ||
                    ie.inputType === "insertLineBreak"
                  ) {
                    seedBlankBlockTypingFormat(el, pending);
                  }
                }
                if (el) {
                  // Restore Double-Return empty paragraphs Chromium may strip while editing.
                  el.querySelectorAll("[data-doc-blank='1']").forEach((node) => {
                    if (!(node instanceof HTMLElement)) return;
                    const text = (node.textContent ?? "").replace(/\u00a0|\u200b/g, "").trim();
                    if (!text && !node.querySelector("br")) {
                      node.innerHTML = "<br>";
                    }
                  });
                }
                commit();
                syncPaletteFocus();
              }}
              onKeyDown={(e) => {
                const el = editorRef.current;
                if (el && e.key === "Tab" && !e.ctrlKey && !e.metaKey && !e.altKey) {
                  if (navigateTableCellOnTab(el, e.shiftKey)) {
                    e.preventDefault();
                    rememberSelection();
                    syncPaletteFocus();
                    return;
                  }
                }
                if (e.key !== "Enter" || e.shiftKey || e.ctrlKey || e.metaKey || e.altKey) return;
                if (!el) return;
                // Capture before native insertParagraph drops font wrappers on the new block.
                const typing = typingFormatForInsert(el);
                setTypingFormat(el, typing);
                enterTypingRef.current = typing;
              }}
              onKeyUp={() => {
                rememberSelection();
                syncPaletteFocus();
              }}
              onMouseUp={() => {
                const el = editorRef.current;
                if (el) {
                  handleTableCellPointerUp(el);
                  // Click-away → continue: sticky Face/Size follows the caret run.
                  setTypingFormat(el, typingFormatForInsert(el));
                }
                rememberSelection();
                syncPaletteFocus();
              }}
              onMouseMove={(e) => {
                const el = editorRef.current;
                if (el && handleTableCellPointerMove(el, e.clientX, e.clientY, e.buttons)) {
                  e.preventDefault();
                  rememberSelection();
                  syncPaletteFocus();
                }
              }}
              onMouseDown={(e) => {
                if (e.button === 0 && e.detail === 3) {
                  const el = editorRef.current;
                  if (el && selectParagraphAtPoint(el, e.clientX, e.clientY)) {
                    e.preventDefault();
                    rememberSelection();
                    registerAsPaletteEditor();
                    syncPaletteFocus();
                    return;
                  }
                }
                const el = editorRef.current;
                const hitImage = embeddedImageFromEventTarget(e.target);
                if (el && hitImage && e.button === 0) {
                  selectEmbeddedImage(el, hitImage);
                } else if (
                  el &&
                  e.button === 0 &&
                  !(e.target as HTMLElement).closest(`.${EMBEDDED_IMAGE_HANDLES_CLASS}`)
                ) {
                  clearEmbeddedImageSelection(el);
                }
                if (el && handleTableCellPointerDown(el, e.target, e.button)) {
                  registerAsPaletteEditor();
                  rememberSelection();
                  syncPaletteFocus();
                  return;
                }
                if (el) clearTableCellSelection(el);
                // Clicking empty padding below a lone function token: force caret after it.
                if (e.target !== e.currentTarget) return;
                if (!el) return;
                const tokens = el.querySelectorAll(`.${FUNCTION_TOKEN_CLASS}`);
                const last = tokens[tokens.length - 1];
                if (!(last instanceof HTMLElement)) return;
                ensureFunctionTokenCaretGaps(el);
                const landing = last.nextSibling;
                const range = document.createRange();
                if (landing?.nodeType === Node.TEXT_NODE) {
                  range.setStart(landing, landing.textContent?.length ?? 0);
                } else {
                  range.setStartAfter(last);
                }
                range.collapse(true);
                const sel = window.getSelection();
                if (!sel) return;
                sel.removeAllRanges();
                sel.addRange(range);
                savedRangeRef.current = range.cloneRange();
              }}
              onDoubleClick={(e) => {
                const el = editorRef.current;
                if (!el) return;
                const field = (e.target as HTMLElement).closest(`.${FIELD_TOKEN_CLASS}`);
                if (field instanceof HTMLElement && el.contains(field)) {
                  e.preventDefault();
                  e.stopPropagation();
                  const sel = window.getSelection();
                  if (sel) {
                    const range = document.createRange();
                    range.selectNode(field);
                    sel.removeAllRanges();
                    sel.addRange(range);
                    savedRangeRef.current = range.cloneRange();
                  }
                  registerAsPaletteEditor();
                  syncPaletteFocus();
                  return;
                }
                if (selectWordOrTokenAtPoint(el, e.clientX, e.clientY)) {
                  e.preventDefault();
                  e.stopPropagation();
                  rememberSelection();
                  registerAsPaletteEditor();
                  syncPaletteFocus();
                }
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
            <EmbeddedImageHandlesOverlay editorRef={editorRef} onCommit={commit} />
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

function findPendingFunctionToken(
  editor: HTMLElement,
  pending: { instanceId: string | null; functionId: string | null; config: string | null },
): HTMLElement | null {
  if (pending.instanceId) {
    const byInstance = editor.querySelector(
      `.${FUNCTION_TOKEN_CLASS}[data-function-instance="${CSS.escape(pending.instanceId)}"]`,
    );
    if (byInstance instanceof HTMLElement) return byInstance;
  }
  if (!pending.functionId) return null;
  const tokens = editor.querySelectorAll(`.${FUNCTION_TOKEN_CLASS}`);
  for (const node of tokens) {
    if (!(node instanceof HTMLElement)) continue;
    if (node.getAttribute("data-function-id") !== pending.functionId) continue;
    if (pending.config && node.getAttribute("data-function-config") !== pending.config) continue;
    return node;
  }
  return null;
}
