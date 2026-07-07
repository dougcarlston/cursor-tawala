import { useEffect, useRef, useState } from "react";
import {
  fieldToken,
  hasFieldDrag,
  readFieldDragName,
  setActiveFieldTarget,
} from "@/lib/fieldInsertion";

interface Props {
  html: string;
  onChange: (html: string) => void;
  placeholder?: string;
}

interface FormatState {
  bold: boolean;
  italic: boolean;
  underline: boolean;
  fontSize: string;
}

const DEFAULT_FONT_SIZE_VALUE = "default";
const LEGACY_DEFAULT_FONT_SIZE = "3";

const DEFAULT_FORMAT_STATE: FormatState = {
  bold: false,
  italic: false,
  underline: false,
  fontSize: DEFAULT_FONT_SIZE_VALUE,
};

const FONT_SIZE_OPTIONS = [
  { value: DEFAULT_FONT_SIZE_VALUE, label: "Default / Normal" },
  { value: "1", label: "1 - Tiny" },
  { value: "2", label: "2 - Small" },
  { value: "4", label: "4 - Medium" },
  { value: "5", label: "5 - Large" },
  { value: "6", label: "6 - X-Large" },
  { value: "7", label: "7 - XX-Large" },
];

function mapPixelsToFontSize(value: number) {
  if (value <= 10) return "1";
  if (value <= 13) return "2";
  if (value <= 16) return "3";
  if (value <= 18) return "4";
  if (value <= 24) return "5";
  if (value <= 32) return "6";
  return "7";
}

function normalizeFontSizeValue(value: unknown) {
  const raw = String(value ?? "").trim();
  if (!raw) return DEFAULT_FORMAT_STATE.fontSize;

  if (/^[1-7]$/.test(raw)) {
    return raw === LEGACY_DEFAULT_FONT_SIZE ? DEFAULT_FORMAT_STATE.fontSize : raw;
  }

  const pxMatch = raw.match(/^(\d+(?:\.\d+)?)px$/i);
  if (pxMatch) return normalizeFontSizeValue(mapPixelsToFontSize(Number(pxMatch[1])));

  const ptMatch = raw.match(/^(\d+(?:\.\d+)?)pt$/i);
  if (ptMatch) return normalizeFontSizeValue(mapPixelsToFontSize(Number(ptMatch[1]) * (4 / 3)));

  return DEFAULT_FORMAT_STATE.fontSize;
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
  if (typeof doc.caretRangeFromPoint === "function") {
    return doc.caretRangeFromPoint(x, y);
  }
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

function unwrapElement(el: Element) {
  const parent = el.parentNode;
  if (!parent) return;

  while (el.firstChild) {
    parent.insertBefore(el.firstChild, el);
  }

  parent.removeChild(el);
}

function stripFontSizeFormatting(root: ParentNode, removeAll = false) {
  root.querySelectorAll("font[size]").forEach((node) => {
    const size = node.getAttribute("size");
    if (!removeAll && normalizeFontSizeValue(size) !== DEFAULT_FONT_SIZE_VALUE) return;

    node.removeAttribute("size");
    if (!node.getAttributeNames().length) {
      unwrapElement(node);
    }
  });

  root.querySelectorAll<HTMLElement>("[style]").forEach((node) => {
    if (!node.style.fontSize) return;
    if (!removeAll && normalizeFontSizeValue(node.style.fontSize) !== DEFAULT_FONT_SIZE_VALUE) {
      return;
    }

    node.style.removeProperty("font-size");
    if (!node.getAttribute("style")) {
      node.removeAttribute("style");
    }
    if (node.tagName === "SPAN" && node.attributes.length === 0) {
      unwrapElement(node);
    }
  });
}

/** Simplified rich-text surface (replaces legacy IE WYSIWYG for text items). */
export function RichTextEditor({ html, onChange, placeholder }: Props) {
  const surfaceRef = useRef<HTMLDivElement>(null);
  const lastHtml = useRef(html);
  const savedRangeRef = useRef<Range | null>(null);
  const [formatState, setFormatState] = useState(DEFAULT_FORMAT_STATE);
  const [fieldDragOver, setFieldDragOver] = useState(false);

  const selectionIsInside = (selection: Selection | null, el: HTMLDivElement) => {
    if (!selection) return false;
    const { anchorNode, focusNode } = selection;
    return !!anchorNode && !!focusNode && el.contains(anchorNode) && el.contains(focusNode);
  };

  const readFormatState = (): FormatState => {
    try {
      return {
        bold: document.queryCommandState("bold"),
        italic: document.queryCommandState("italic"),
        underline: document.queryCommandState("underline"),
        fontSize: normalizeFontSizeValue(document.queryCommandValue("fontSize")),
      };
    } catch {
      return DEFAULT_FORMAT_STATE;
    }
  };

  const syncToolbarState = () => {
    const el = surfaceRef.current;
    if (!el) return;

    const selection = document.getSelection();
    if (selectionIsInside(selection, el) && selection?.rangeCount) {
      savedRangeRef.current = selection.getRangeAt(0).cloneRange();
      setFormatState(readFormatState());
      return;
    }

    if (document.activeElement === el) {
      setFormatState(readFormatState());
    }
  };

  const rememberSelection = () => {
    const el = surfaceRef.current;
    const selection = document.getSelection();
    if (!el || !selection || !selectionIsInside(selection, el) || !selection.rangeCount) return;
    savedRangeRef.current = selection.getRangeAt(0).cloneRange();
  };

  const restoreSelection = () => {
    const el = surfaceRef.current;
    const selection = document.getSelection();
    const savedRange = savedRangeRef.current;
    if (!el || !selection || !savedRange) return;
    if (!el.contains(savedRange.commonAncestorContainer)) return;
    selection.removeAllRanges();
    selection.addRange(savedRange);
  };

  useEffect(() => {
    const el = surfaceRef.current;
    if (!el) return;
    if (document.activeElement === el) return;
    const next = html || "";
    if (next !== lastHtml.current) {
      el.innerHTML = next;
      lastHtml.current = next;
    }
  }, [html]);

  useEffect(() => {
    const el = surfaceRef.current;
    if (el && !el.innerHTML && html) {
      el.innerHTML = html;
      lastHtml.current = html;
    }
  }, []);

  useEffect(() => {
    const handleSelectionChange = () => {
      syncToolbarState();
    };

    document.addEventListener("selectionchange", handleSelectionChange);
    return () => document.removeEventListener("selectionchange", handleSelectionChange);
  }, []);

  const exec = (cmd: string, value?: string) => {
    const el = surfaceRef.current;
    if (!el) return;
    el.focus();
    restoreSelection();

    if (cmd === "fontSize" && value === DEFAULT_FONT_SIZE_VALUE) {
      const selection = document.getSelection();
      if (selectionIsInside(selection, el) && selection?.rangeCount) {
        let range = selection.getRangeAt(0);

        if (range.collapsed) {
          const marker = document.createElement("span");
          marker.dataset.richEditorCaretMarker = "true";
          marker.textContent = "\u200b";
          range.insertNode(marker);
          range = document.createRange();
          range.selectNodeContents(marker);
          selection.removeAllRanges();
          selection.addRange(range);
        }

        const fragment = range.extractContents();
        const lastInsertedNode = fragment.lastChild;
        stripFontSizeFormatting(fragment, true);
        range.insertNode(fragment);

        const nextRange = document.createRange();
        const liveMarker = el.querySelector('span[data-rich-editor-caret-marker="true"]');
        if (liveMarker) {
          nextRange.setStartAfter(liveMarker);
          nextRange.collapse(true);
          liveMarker.remove();
        } else if (lastInsertedNode) {
          nextRange.selectNodeContents(lastInsertedNode);
          nextRange.collapse(false);
        } else {
          nextRange.setStart(range.endContainer, range.endOffset);
          nextRange.collapse(true);
        }

        selection.removeAllRanges();
        selection.addRange(nextRange);
        savedRangeRef.current = nextRange.cloneRange();
      }
    } else {
      document.execCommand(cmd, false, value);
    }

    stripFontSizeFormatting(el);
    const next = el.innerHTML;
    lastHtml.current = next;
    onChange(next);
    syncToolbarState();
  };

  /** Insert a `<<field>>` token at the current (or restored) caret. */
  const insertFieldToken = (name: string) => {
    const el = surfaceRef.current;
    if (!el) return;
    el.focus();
    restoreSelection();
    document.execCommand("insertText", false, fieldToken(name));
    stripFontSizeFormatting(el);
    const nextHtml = el.innerHTML;
    lastHtml.current = nextHtml;
    onChange(nextHtml);
    syncToolbarState();
  };

  const handleFieldDrop = (e: React.DragEvent<HTMLDivElement>) => {
    setFieldDragOver(false);
    const name = readFieldDragName(e.dataTransfer);
    if (!name) return;
    e.preventDefault();
    const el = surfaceRef.current;
    if (el) {
      const range = caretRangeAtPoint(e.clientX, e.clientY);
      const selection = document.getSelection();
      if (range && selection && el.contains(range.commonAncestorContainer)) {
        selection.removeAllRanges();
        selection.addRange(range);
        savedRangeRef.current = range.cloneRange();
      }
    }
    insertFieldToken(name);
  };

  return (
    <div
      className="rich-editor"
      onMouseDown={(e) => e.stopPropagation()}
      onClick={(e) => e.stopPropagation()}
    >
      <div className="rich-toolbar">
        <button
          type="button"
          title="Bold"
          className={formatState.bold ? "active" : undefined}
          aria-pressed={formatState.bold}
          onMouseDown={(e) => {
            rememberSelection();
            e.preventDefault();
          }}
          onClick={() => exec("bold")}
        >
          B
        </button>
        <button
          type="button"
          title="Italic"
          className={formatState.italic ? "active" : undefined}
          aria-pressed={formatState.italic}
          onMouseDown={(e) => {
            rememberSelection();
            e.preventDefault();
          }}
          onClick={() => exec("italic")}
        >
          I
        </button>
        <button
          type="button"
          title="Underline"
          className={formatState.underline ? "active" : undefined}
          aria-pressed={formatState.underline}
          onMouseDown={(e) => {
            rememberSelection();
            e.preventDefault();
          }}
          onClick={() => exec("underline")}
        >
          U
        </button>
        <select
          value={formatState.fontSize}
          onMouseDown={() => rememberSelection()}
          onChange={(e) => {
            exec("fontSize", e.target.value);
            e.currentTarget.blur();
          }}
          title="Font size"
        >
          {FONT_SIZE_OPTIONS.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
      </div>
      <div
        ref={surfaceRef}
        className={`rich-surface${fieldDragOver ? " field-drop-active" : ""}`}
        contentEditable
        suppressContentEditableWarning
        data-placeholder={placeholder}
        onDragOver={(e) => {
          if (!hasFieldDrag(e.dataTransfer)) return;
          e.preventDefault();
          e.dataTransfer.dropEffect = "copy";
          if (!fieldDragOver) setFieldDragOver(true);
        }}
        onDragLeave={() => {
          if (fieldDragOver) setFieldDragOver(false);
        }}
        onDrop={handleFieldDrop}
        onInput={(e) => {
          const target = e.target as HTMLDivElement;
          stripFontSizeFormatting(target);
          const next = target.innerHTML;
          lastHtml.current = next;
          onChange(next);
          syncToolbarState();
        }}
        onFocus={() => {
          setActiveFieldTarget(insertFieldToken);
          syncToolbarState();
        }}
        onKeyUp={() => syncToolbarState()}
        onMouseDown={() => {
          savedRangeRef.current = null;
        }}
        onMouseUp={() => syncToolbarState()}
      />
    </div>
  );
}
