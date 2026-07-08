import { useEffect, useRef, useState } from "react";
import { HeadingItem, HEADING_PLACEHOLDER } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import {
  fieldToken,
  hasFieldDrag,
  readFieldDragName,
  retainEditorFocusOnBlur,
  setActiveFieldTarget,
} from "@/lib/fieldInsertion";
import { FormItemDeleteButton } from "./FormItemDeleteButton";
import { clearFormattingFocus, setFormattingFocus } from "@/lib/formattingPaletteContext";

interface Props {
  item: HeadingItem;
  index: number;
  formName: string;
  selected: boolean;
}

type HeadingSize = "main" | "sub";

const SIZE_CLASS: Record<HeadingSize, string> = {
  main: "heading-size-main",
  sub: "heading-size-sub",
};

/** Zero-width marker used to "pend" a size for the next typed characters (see `applySize`). */
const CARET_MARKER = "\u200b";

function escapeHtml(text: string): string {
  return text
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}

/** True when heading content already carries per-run size markup (new format). */
function isRichHeadingContent(content: string): boolean {
  return /heading-size-(main|sub)/.test(content);
}

/**
 * Migrate any HeadingItem to editor HTML with per-run size spans.
 *  - New format: content already holds `heading-size-*` spans → used as-is.
 *  - Legacy format: plain text; the whole box adopts the old `level` (Sub → one Sub run,
 *    otherwise bare Main text). This keeps existing single-size headings intact.
 */
function headingContentToHtml(item: HeadingItem): string {
  const content = item.content ?? "";
  if (isRichHeadingContent(content)) return content;
  const esc = escapeHtml(content);
  if (!esc) return "";
  return item.level === "sub" ? `<span class="${SIZE_CLASS.sub}">${esc}</span>` : esc;
}

/** Plain text of an HTML fragment (for placeholder / empty checks). */
function htmlToPlainText(html: string): string {
  const tmp = document.createElement("div");
  tmp.innerHTML = html;
  return (tmp.textContent ?? "").replace(new RegExp(CARET_MARKER, "g"), "");
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

function unwrap(el: Element) {
  const parent = el.parentNode;
  if (!parent) return;
  while (el.firstChild) parent.insertBefore(el.firstChild, el);
  parent.removeChild(el);
}

/**
 * Serialize the editor to storage markup keeping ONLY text, `<br>`, and the two size spans
 * (`heading-size-main` / `heading-size-sub`). Any other element (bold/italic/font/pasted
 * markup, browser-inserted block wrappers) is unwrapped to its text — enforcing "two sizes
 * only" and keeping the read-only collapsed render safe. Caret markers are dropped; empty
 * size spans are omitted.
 */
function serializeChildren(node: Node): string {
  let html = "";
  node.childNodes.forEach((child) => {
    if (child.nodeType === Node.TEXT_NODE) {
      html += escapeHtml((child.textContent ?? "").replace(new RegExp(CARET_MARKER, "g"), ""));
      return;
    }
    if (child.nodeType !== Node.ELEMENT_NODE) return;
    const el = child as HTMLElement;
    const tag = el.tagName;
    if (tag === "BR") {
      html += "<br>";
      return;
    }
    if (
      tag === "SPAN" &&
      (el.classList.contains("heading-size-sub") || el.classList.contains("heading-size-main"))
    ) {
      const cls = el.classList.contains("heading-size-sub")
        ? "heading-size-sub"
        : "heading-size-main";
      const inner = serializeChildren(el);
      if (inner) html += `<span class="${cls}">${inner}</span>`;
      return;
    }
    if (tag === "DIV" || tag === "P") {
      if (html && !html.endsWith("<br>")) html += "<br>";
      html += serializeChildren(el);
      return;
    }
    // Any other element (formatting/pasted): unwrap to its text content.
    html += serializeChildren(el);
  });
  return html;
}

function serializeHeading(root: HTMLElement): string {
  const html = serializeChildren(root);
  return html.replace(/<[^>]*>/g, "").trim() ? html : "";
}

/**
 * Canvas WYSIWYG row for a Heading form item (spec:
 * `Tawala_Key_Documents/DESIGNER_FORM_ITEMS_HEADING.md`; legacy `HeadingView.cs`).
 *
 * State machine (design mode):
 *  - **editing** — orange badge + inline rich editor (contenteditable) + on-canvas
 *    **Heading Type** dropdown. Selecting text + choosing Main/Sub sizes ONLY the selection
 *    (per-run size); with no selection it "pends" the size for the next typed characters.
 *  - **collapsed** — gray badge + rendered heading (read-only, mixed sizes preserved); no
 *    dropdown / editor chrome. Entered when focus leaves the row.
 *
 * The **label** (`H1`, `H2`, …) is edited directly in the badge (click → input), not in the
 * Properties panel. Label, text, and per-run size all live on the canvas (owner, July 2026).
 */
export function HeadingCanvasRow({ item, index, formName, selected }: Props) {
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const [editing, setEditing] = useState(selected);
  const [editingLabel, setEditingLabel] = useState(false);
  const [dragOver, setDragOver] = useState(false);
  const [currentSize, setCurrentSize] = useState<HeadingSize | "mixed">("main");
  const editorRef = useRef<HTMLDivElement>(null);
  const labelInputRef = useRef<HTMLInputElement>(null);
  const savedRangeRef = useRef<Range | null>(null);
  const wasSelected = useRef(selected);

  const update = (patch: Partial<HeadingItem>) =>
    updateFormItem(formName, index, { ...item, ...patch });

  // Enter editing when the row becomes selected (insert or re-select from elsewhere);
  // collapse when selection moves away (mirrors HeadingView GotFocus / OnValidated).
  useEffect(() => {
    if (selected && !wasSelected.current) setEditing(true);
    if (!selected) setEditing(false);
    wasSelected.current = selected;
  }, [selected]);

  // Seed the contenteditable when entering edit. Deps are [editing] only so our own typing
  // (which flows back into `content`) never re-writes the DOM and clobbers the caret.
  useEffect(() => {
    if (!editing) {
      clearFormattingFocus("heading");
      return;
    }
    const el = editorRef.current;
    if (!el) return;
    const html = headingContentToHtml(item);
    el.innerHTML = html;
    el.focus();
    setFormattingFocus({ kind: "heading", cursorInTable: false });
    const sel = window.getSelection();
    if (!sel) return;
    const range = document.createRange();
    if (htmlToPlainText(html) === HEADING_PLACEHOLDER) {
      // Legacy AfterAddedToFormByUser → SelectAll: first keystroke replaces the placeholder.
      range.selectNodeContents(el);
    } else {
      range.selectNodeContents(el);
      range.collapse(false);
    }
    sel.removeAllRanges();
    sel.addRange(range);
    savedRangeRef.current = range.cloneRange();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [editing]);

  useEffect(() => () => clearFormattingFocus("heading"), []);

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

  /**
   * Read the editor back to storage: keep only text + the two size spans (drop markers,
   * empty spans, and any stray formatting); "" when blank. Once edited, the heading is fully
   * per-run, so the legacy whole-box `level` is cleared.
   */
  const commit = () => {
    const el = editorRef.current;
    if (!el) return;
    update({ content: serializeHeading(el), level: undefined });
  };

  /** Size of the run containing a node (walks up to the editor root; default Main). */
  const sizeOfNode = (node: Node | null): HeadingSize => {
    const el = editorRef.current;
    let n: Node | null = node?.nodeType === Node.TEXT_NODE ? node.parentElement : node;
    while (n && n !== el) {
      const cls = (n as HTMLElement).classList;
      if (cls?.contains(SIZE_CLASS.sub)) return "sub";
      if (cls?.contains(SIZE_CLASS.main)) return "main";
      n = (n as HTMLElement).parentElement;
    }
    return "main";
  };

  const syncCurrentSize = () => {
    const el = editorRef.current;
    const sel = window.getSelection();
    if (!el || !sel || sel.rangeCount === 0) return;
    const range = sel.getRangeAt(0);
    if (!el.contains(range.commonAncestorContainer)) return;
    savedRangeRef.current = range.cloneRange();
    const startSize = sizeOfNode(range.startContainer);
    // A selection that straddles both sizes shows a neutral "—" so re-picking either size
    // always fires onChange (native selects skip onChange when the value is unchanged).
    const endSize = range.collapsed ? startSize : sizeOfNode(range.endContainer);
    setCurrentSize(startSize === endSize ? startSize : "mixed");
  };

  /** Apply a size to the current selection only, or pend it for the next typed characters. */
  const applySize = (size: HeadingSize) => {
    const el = editorRef.current;
    if (!el) return;
    el.focus();
    restoreSelection();
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return;
    const range = sel.getRangeAt(0);
    if (!el.contains(range.commonAncestorContainer)) return;
    const cls = SIZE_CLASS[size];

    if (range.collapsed) {
      // No selection → pend the size: type into a fresh span so new characters adopt it.
      const span = document.createElement("span");
      span.className = cls;
      span.appendChild(document.createTextNode(CARET_MARKER));
      range.insertNode(span);
      const next = document.createRange();
      next.setStart(span.firstChild as Node, 1);
      next.collapse(true);
      sel.removeAllRanges();
      sel.addRange(next);
      savedRangeRef.current = next.cloneRange();
    } else {
      const frag = range.extractContents();
      frag
        .querySelectorAll("span.heading-size-main, span.heading-size-sub")
        .forEach((s) => unwrap(s));
      const span = document.createElement("span");
      span.className = cls;
      span.appendChild(frag);
      range.insertNode(span);
      el.querySelectorAll("span.heading-size-main, span.heading-size-sub").forEach((s) => {
        if (!s.textContent) s.remove();
      });
      el.normalize();
      const next = document.createRange();
      next.selectNodeContents(span);
      sel.removeAllRanges();
      sel.addRange(next);
      savedRangeRef.current = next.cloneRange();
    }
    setCurrentSize(size);
    commit();
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
    // Keep editing while focus moves within the row (dropdown, label input, editor).
    if (e.currentTarget.contains(e.relatedTarget as Node | null)) return;
    if (retainEditorFocusOnBlur(e.relatedTarget)) return;
    clearFormattingFocus("heading");
    setEditing(false);
  };

  const renderedHtml = headingContentToHtml(item);
  const isEmpty = htmlToPlainText(renderedHtml).trim() === "";

  return (
    <div
      className={`heading-canvas-row ${editing ? "editing" : "collapsed"}${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
        const target = e.target as HTMLElement;
        if (target.closest(".heading-badge, .heading-badge-input, .canvas-item-delete")) return;
        if (target.closest(".heading-canvas-main")) {
          if (!editing) setEditing(true);
          return;
        }
        setEditing(false);
        editorRef.current?.blur();
      }}
      onBlur={handleBlur}
    >
      <FormItemDeleteButton formName={formName} index={index} visible={selected} />
      {editingLabel ? (
        <input
          ref={labelInputRef}
          className="heading-badge-input"
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
          className={`heading-badge${editing ? " editing" : ""}`}
          title="Click to edit heading label"
          onClick={(e) => {
            e.stopPropagation();
            setEditingLabel(true);
          }}
        >
          {item.label}
        </div>
      )}
      <div className="heading-canvas-main">
        {editing ? (
          <>
            <div
              ref={editorRef}
              className={`heading-rich-editor${dragOver ? " field-drop-active" : ""}`}
              contentEditable
              suppressContentEditableWarning
              data-placeholder={HEADING_PLACEHOLDER}
              onInput={() => {
                commit();
                syncCurrentSize();
              }}
              onKeyUp={syncCurrentSize}
              onMouseUp={syncCurrentSize}
              onFocus={() => {
                setActiveFieldTarget(insertFieldToken);
                setFormattingFocus({ kind: "heading", cursorInTable: false });
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
            <div className="heading-type-row">
              <label onMouseDown={rememberSelection}>
                Heading Type:
                <select
                  value={currentSize}
                  onMouseDown={rememberSelection}
                  onChange={(e) => applySize(e.target.value as HeadingSize)}
                >
                  <option value="mixed" disabled hidden>
                    —
                  </option>
                  <option value="main">Main</option>
                  <option value="sub">Sub</option>
                </select>
              </label>
              <span className="heading-type-hint">Applies to highlighted text</span>
            </div>
          </>
        ) : (
          <div
            className="heading-rendered"
            dangerouslySetInnerHTML={{
              __html: isEmpty ? escapeHtml(HEADING_PLACEHOLDER) : renderedHtml,
            }}
          />
        )}
      </div>
    </div>
  );
}
