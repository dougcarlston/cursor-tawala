import { useEffect, useRef, useState } from "react";
import {
  fieldToken,
  hasFieldDrag,
  readFieldDragName,
  setActiveFieldTarget,
} from "@/lib/fieldInsertion";
import {
  clearActivePaletteEditor,
  clearFormattingFocus,
  selectionCursorInTable,
  setActivePaletteEditor,
  setFormattingFocus,
  type FormattingFocusKind,
} from "@/lib/formattingPaletteContext";
import { TableHandlesOverlay } from "./TableHandlesOverlay";
import {
  focusDocumentDropTarget,
  placeDocumentTextAtPoint,
  resolveDocumentFieldDropTarget,
} from "@/lib/documentCanvas";

interface Props {
  html: string;
  onChange: (html: string) => void;
  placeholder?: string;
  /** Registers this surface with the shared Formatting Palette (row 2). */
  formattingKind?: Extract<FormattingFocusKind, "text" | "document" | "fib" | "mcq">;
}

const DEFAULT_FONT_SIZE_VALUE = "default";
const LEGACY_DEFAULT_FONT_SIZE = "3";

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
  if (!raw) return DEFAULT_FONT_SIZE_VALUE;

  if (/^[1-7]$/.test(raw)) {
    return raw === LEGACY_DEFAULT_FONT_SIZE ? DEFAULT_FONT_SIZE_VALUE : raw;
  }

  const pxMatch = raw.match(/^(\d+(?:\.\d+)?)px$/i);
  if (pxMatch) return normalizeFontSizeValue(mapPixelsToFontSize(Number(pxMatch[1])));

  const ptMatch = raw.match(/^(\d+(?:\.\d+)?)pt$/i);
  if (ptMatch) return normalizeFontSizeValue(mapPixelsToFontSize(Number(ptMatch[1]) * (4 / 3)));

  return DEFAULT_FONT_SIZE_VALUE;
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

function stripFontSizeFormatting(root: ParentNode) {
  root.querySelectorAll("font[size]").forEach((node) => {
    const size = node.getAttribute("size");
    if (normalizeFontSizeValue(size) !== DEFAULT_FONT_SIZE_VALUE) return;

    node.removeAttribute("size");
    if (!node.getAttributeNames().length) {
      unwrapElement(node);
    }
  });

  root.querySelectorAll<HTMLElement>("[style]").forEach((node) => {
    if (!node.style.fontSize) return;
    if (normalizeFontSizeValue(node.style.fontSize) !== DEFAULT_FONT_SIZE_VALUE) return;

    node.style.removeProperty("font-size");
    if (!node.getAttribute("style")) {
      node.removeAttribute("style");
    }
    if (node.tagName === "SPAN" && node.attributes.length === 0) {
      unwrapElement(node);
    }
  });
}

/**
 * Rich-text surface for Document body and Properties-panel text. Formatting is driven by the
 * shared Formatting Palette (row 2) when `formattingKind` is set — no embedded mini-toolbar.
 */
export function RichTextEditor({ html, onChange, placeholder, formattingKind }: Props) {
  const surfaceRef = useRef<HTMLDivElement>(null);
  const lastHtml = useRef(html);
  const savedRangeRef = useRef<Range | null>(null);
  const [fieldDragOver, setFieldDragOver] = useState(false);

  const selectionIsInside = (selection: Selection | null, el: HTMLDivElement) => {
    if (!selection) return false;
    const { anchorNode, focusNode } = selection;
    return !!anchorNode && !!focusNode && el.contains(anchorNode) && el.contains(focusNode);
  };

  const syncPaletteFocus = () => {
    if (!formattingKind) return;
    const el = surfaceRef.current;
    if (!el || document.activeElement !== el) return;
    setFormattingFocus({
      kind: formattingKind,
      cursorInTable: selectionCursorInTable(el),
    });
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
    return () => {
      if (formattingKind) clearFormattingFocus(formattingKind);
      clearActivePaletteEditor(surfaceRef.current ?? undefined);
    };
  }, [formattingKind]);

  const commitPalette = () => {
    const el = surfaceRef.current;
    if (!el) return;
    stripFontSizeFormatting(el);
    const next = el.innerHTML;
    lastHtml.current = next;
    onChange(next);
    syncPaletteFocus();
  };

  const registerAsPaletteEditor = () => {
    const el = surfaceRef.current;
    if (!el || !formattingKind) return;
    setActivePaletteEditor({
      el,
      commit: commitPalette,
      saveSelection: rememberSelection,
      restoreSelection,
    });
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
      const el = surfaceRef.current;
      if (!el) return;
      const selection = document.getSelection();
      if (selectionIsInside(selection, el) && selection?.rangeCount) {
        savedRangeRef.current = selection.getRangeAt(0).cloneRange();
        syncPaletteFocus();
      }
    };

    document.addEventListener("selectionchange", handleSelectionChange);
    return () => document.removeEventListener("selectionchange", handleSelectionChange);
  }, [formattingKind]);

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
    syncPaletteFocus();
  };

  const commitFromSurface = (target: HTMLDivElement) => {
    stripFontSizeFormatting(target);
    const next = target.innerHTML;
    lastHtml.current = next;
    onChange(next);
    syncPaletteFocus();
  };

  const handleDocumentCanvasMouseDown = (e: React.MouseEvent<HTMLDivElement>) => {
    if (formattingKind !== "document") return;
    if (e.button !== 0) return;
    const el = surfaceRef.current;
    if (!el) return;
    const target = e.target as HTMLElement;
    if (target.closest(".table-handles-overlay")) return;

    el.focus();
    const placed = placeDocumentTextAtPoint(el, e.clientX, e.clientY);
    if (placed) {
      e.preventDefault();
      savedRangeRef.current = null;
      registerAsPaletteEditor();
      syncPaletteFocus();
      commitFromSurface(el);
    }
  };

  const handleFieldDrop = (e: React.DragEvent<HTMLDivElement>) => {
    setFieldDragOver(false);
    const name = readFieldDragName(e.dataTransfer);
    if (!name) return;
    e.preventDefault();
    e.stopPropagation();
    const el = surfaceRef.current;
    if (!el) return;

    if (formattingKind === "document") {
      el.focus();
      const target = resolveDocumentFieldDropTarget(el, e.clientX, e.clientY);
      const range = focusDocumentDropTarget(target, e.clientX, e.clientY);
      if (range) savedRangeRef.current = range.cloneRange();
      document.execCommand("insertText", false, fieldToken(name));
      commitFromSurface(el);
      return;
    }

    const range = caretRangeAtPoint(e.clientX, e.clientY);
    const selection = window.getSelection();
    if (range && selection && el.contains(range.commonAncestorContainer)) {
      selection.removeAllRanges();
      selection.addRange(range);
      savedRangeRef.current = range.cloneRange();
    }
    insertFieldToken(name);
  };

  return (
    <div
      className="rich-editor"
      onMouseDown={(e) => e.stopPropagation()}
      onClick={(e) => e.stopPropagation()}
    >
      <div className="rich-surface-wrap">
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
          onInput={(e) => commitFromSurface(e.target as HTMLDivElement)}
          onFocus={() => {
            setActiveFieldTarget(insertFieldToken);
            registerAsPaletteEditor();
            syncPaletteFocus();
          }}
          onBlur={(e) => {
            const next = e.relatedTarget as HTMLElement | null;
            if (next?.closest(".formatting-palette")) return;
            if (next?.closest(".table-handles-overlay")) return;
            if (formattingKind) clearFormattingFocus(formattingKind);
          }}
          onKeyUp={() => {
            rememberSelection();
            syncPaletteFocus();
          }}
          onMouseDown={(e) => {
            savedRangeRef.current = null;
            handleDocumentCanvasMouseDown(e);
          }}
          onMouseUp={() => {
            rememberSelection();
            syncPaletteFocus();
          }}
        />
        {(formattingKind === "document" || formattingKind === "text") && (
          <TableHandlesOverlay editorRef={surfaceRef} onCommit={commitPalette} />
        )}
      </div>
    </div>
  );
}
