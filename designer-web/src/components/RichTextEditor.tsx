import { useEffect, useRef, useState } from "react";
import {
  beginFieldTokenMove,
  clearMovingFieldToken,
  hasFieldDrag,
  isFieldTokenMoveDrag,
  readFieldDragName,
  setActiveFieldTarget,
  takeMovingFieldToken,
} from "@/lib/fieldInsertion";
import {
  ensureFieldTokensDraggable,
  insertFieldTokenAtSelection,
  moveFieldTokenToSelection,
  normalizeFieldTokenSpans,
  readFieldNameFromToken,
  FIELD_TOKEN_CLASS,
} from "@/lib/fieldTokens";
import {
  clearActivePaletteEditor,
  clearFormattingFocus,
  selectionCursorInTable,
  selectionHasResettableFormatting,
  setActivePaletteEditor,
  setFormattingFocus,
  type FormattingFocusKind,
} from "@/lib/formattingPaletteContext";
import { TableHandlesOverlay } from "./TableHandlesOverlay";
import { PlacedTextHandlesOverlay } from "./PlacedTextHandlesOverlay";
import {
  ensurePlacedBlockWrapWidth,
  extendDocumentSelectionToPoint,
  findPlacedTextBlockAtCaret,
  focusDocumentDropTarget,
  focusPlacedBlock,
  documentEnterInPlacedText,
  handlePlacedTextArrowKey,
  placeDocumentTextAtPoint,
  PLACED_TEXT_CLASS,
  reflowAllPlacedLines,
  reflowPlacedLinesBelow,
  resolveDocumentFieldDropTarget,
  pruneEmptyPlacedTextBlocks,
} from "@/lib/documentCanvas";
import { openFunctionTokenForEdit } from "@/lib/functionPicker";
import { FUNCTION_TOKEN_CLASS } from "@/lib/functionTokens";
import {
  ITEMIZATION_TOKEN_DATA_ATTR,
  openStructuredFunctionTokenForEdit,
  STRUCTURED_NODE_DATA_ATTR,
} from "@/lib/structuredItemizationEdit";

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
  /** Document canvas: distinguish click (place/focus block) from drag (text selection). */
  const documentPointerRef = useRef<{ x: number; y: number; dragged: boolean } | null>(null);
  /** Anchor range for cross-block Document drag-select. */
  const selectAnchorRef = useRef<Range | null>(null);
  const commitFromSurfaceRef = useRef<(target: HTMLDivElement) => void>(() => {});

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
      hasResettableFormatting: selectionHasResettableFormatting(el),
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
    if (!el || !html) return;
    normalizeFieldTokenSpans(el);
    ensureFieldTokensDraggable(el);
  }, [html]);

  useEffect(() => {
    const el = surfaceRef.current;
    if (el && !el.innerHTML && html) {
      el.innerHTML = html;
      normalizeFieldTokenSpans(el);
      ensureFieldTokensDraggable(el);
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

  /** Insert a field token span at the current (or restored) caret. */
  const insertFieldToken = (name: string) => {
    const el = surfaceRef.current;
    if (!el) return;
    el.focus();
    restoreSelection();
    insertFieldTokenAtSelection(name);
    stripFontSizeFormatting(el);
    const nextHtml = el.innerHTML;
    lastHtml.current = nextHtml;
    onChange(nextHtml);
    syncPaletteFocus();
  };

  const commitFromSurface = (target: HTMLDivElement) => {
    stripFontSizeFormatting(target);
    normalizeFieldTokenSpans(target);
    ensureFieldTokensDraggable(target);
    const next = target.innerHTML;
    lastHtml.current = next;
    onChange(next);
    syncPaletteFocus();
  };
  commitFromSurfaceRef.current = commitFromSurface;

  useEffect(() => {
    if (formattingKind !== "document") return;
    const el = surfaceRef.current;
    if (!el || typeof ResizeObserver === "undefined") return;

    let lastWidth = el.clientWidth;
    let settleTimer: number | undefined;
    const observer = new ResizeObserver(() => {
      const width = el.clientWidth;
      if (Math.abs(width - lastWidth) < 1) return;
      lastWidth = width;
      reflowAllPlacedLines(el);
      window.clearTimeout(settleTimer);
      settleTimer = window.setTimeout(() => {
        commitFromSurfaceRef.current(el);
      }, 120);
    });
    observer.observe(el);
    return () => {
      observer.disconnect();
      window.clearTimeout(settleTimer);
    };
  }, [formattingKind]);

  useEffect(() => {
    if (formattingKind !== "document") return;

    const onMove = (e: MouseEvent) => {
      const pointer = documentPointerRef.current;
      const el = surfaceRef.current;
      if (!pointer || !el) return;
      if (!pointer.dragged) {
        if (Math.abs(e.clientX - pointer.x) <= 3 && Math.abs(e.clientY - pointer.y) <= 3) {
          return;
        }
        pointer.dragged = true;
      }
      if (!selectAnchorRef.current) {
        const sel = window.getSelection();
        if (sel?.rangeCount) selectAnchorRef.current = sel.getRangeAt(0).cloneRange();
      }
      const anchor = selectAnchorRef.current;
      if (!anchor) return;
      if (extendDocumentSelectionToPoint(el, anchor, e.clientX, e.clientY)) {
        e.preventDefault();
        const selection = document.getSelection();
        if (selection?.rangeCount) {
          savedRangeRef.current = selection.getRangeAt(0).cloneRange();
        }
      }
    };

    const onUp = () => {
      selectAnchorRef.current = null;
      // pointer cleared by surface onMouseUp; keep as safety if mouseup is outside
      if (documentPointerRef.current?.dragged) {
        documentPointerRef.current = null;
      }
    };

    window.addEventListener("mousemove", onMove);
    window.addEventListener("mouseup", onUp);
    return () => {
      window.removeEventListener("mousemove", onMove);
      window.removeEventListener("mouseup", onUp);
    };
  }, [formattingKind]);

  const handleDocumentCanvasClick = (e: React.MouseEvent<HTMLDivElement>) => {
    const el = surfaceRef.current;
    if (!el) return;
    const target = e.target as HTMLElement;
    if (target.closest(".table-handles-overlay")) return;
    if (target.closest(`.${FIELD_TOKEN_CLASS}`)) return;
    if (target.closest(`.${FUNCTION_TOKEN_CLASS}`)) return;
    if (target.closest(`[${STRUCTURED_NODE_DATA_ATTR}]`)) return;
    if (target.closest(`[${ITEMIZATION_TOKEN_DATA_ATTR}]`)) return;

    const placed = placeDocumentTextAtPoint(el, e.clientX, e.clientY);
    if (placed) {
      savedRangeRef.current = null;
      registerAsPaletteEditor();
      syncPaletteFocus();
      commitFromSurface(el);
    }
  };

  const tryOpenFunctionToken = (target: EventTarget | null): boolean => {
    const el = surfaceRef.current;
    if (!el || !(target instanceof Element)) return false;
    if (formattingKind !== "text" && formattingKind !== "document") return false;

    const structured = target.closest(`[${STRUCTURED_NODE_DATA_ATTR}]`);
    if (structured instanceof HTMLElement) {
      registerAsPaletteEditor();
      return openStructuredFunctionTokenForEdit(structured, () => {
        rememberSelection();
        commitFromSurface(el);
      });
    }

    const token = target.closest(`.${FUNCTION_TOKEN_CLASS}`);
    if (!(token instanceof HTMLElement)) return false;
    registerAsPaletteEditor();
    return openFunctionTokenForEdit(token, el, () => {
      rememberSelection();
    });
  };

  const handleFieldDrop = (e: React.DragEvent<HTMLDivElement>) => {
    setFieldDragOver(false);
    const moving = takeMovingFieldToken();
    const name = moving
      ? readFieldNameFromToken(moving) ?? readFieldDragName(e.dataTransfer)
      : readFieldDragName(e.dataTransfer);
    if (!name && !moving) return;
    e.preventDefault();
    e.stopPropagation();
    const el = surfaceRef.current;
    if (!el) return;

    if (formattingKind === "document") {
      el.focus();
      const target = resolveDocumentFieldDropTarget(el, e.clientX, e.clientY);
      const range = focusDocumentDropTarget(target, e.clientX, e.clientY);
      if (range) savedRangeRef.current = range.cloneRange();
      if (moving) {
        moveFieldTokenToSelection(moving);
      } else if (name) {
        insertFieldTokenAtSelection(name);
      }
      if (target.classList.contains(PLACED_TEXT_CLASS)) {
        ensurePlacedBlockWrapWidth(el, target);
        reflowPlacedLinesBelow(el, target);
      }
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
    if (moving) {
      moveFieldTokenToSelection(moving);
      commitFromSurface(el);
    } else if (name) {
      insertFieldToken(name);
    }
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
          onDragStart={(e) => {
            const token = (e.target as HTMLElement).closest(`.${FIELD_TOKEN_CLASS}`);
            if (!(token instanceof HTMLElement) || !surfaceRef.current?.contains(token)) return;
            const name = readFieldNameFromToken(token);
            if (!name) {
              e.preventDefault();
              return;
            }
            beginFieldTokenMove(token, e.dataTransfer, name);
            if (documentPointerRef.current) documentPointerRef.current.dragged = true;
          }}
          onDragEnd={() => {
            clearMovingFieldToken();
          }}
          onDragOver={(e) => {
            if (!hasFieldDrag(e.dataTransfer)) return;
            e.preventDefault();
            e.dataTransfer.dropEffect = isFieldTokenMoveDrag(e.dataTransfer) ? "move" : "copy";
            if (!fieldDragOver) setFieldDragOver(true);
          }}
          onDragLeave={() => {
            if (fieldDragOver) setFieldDragOver(false);
          }}
          onDrop={handleFieldDrop}
          onInput={(e) => {
            const target = e.target as HTMLDivElement;
            if (formattingKind === "document") {
              const ie = e.nativeEvent as InputEvent;
              const deleted = typeof ie.inputType === "string" && ie.inputType.startsWith("delete");
              if (deleted) {
                pruneEmptyPlacedTextBlocks(target);
              }
              const placed = findPlacedTextBlockAtCaret(target);
              if (placed) {
                ensurePlacedBlockWrapWidth(target, placed);
                reflowPlacedLinesBelow(target, placed);
              }
            }
            commitFromSurface(target);
          }}
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
          onKeyDown={(e) => {
            if (formattingKind !== "document") return;
            const el = surfaceRef.current;
            if (!el) return;

            if (
              e.key === "ArrowLeft" ||
              e.key === "ArrowRight" ||
              e.key === "ArrowUp" ||
              e.key === "ArrowDown" ||
              e.key === "Home" ||
              e.key === "End"
            ) {
              if (handlePlacedTextArrowKey(el, e.key)) {
                e.preventDefault();
                rememberSelection();
                syncPaletteFocus();
              }
              return;
            }

            if (e.key !== "Enter" || e.shiftKey || e.ctrlKey || e.metaKey || e.altKey) return;
            if (documentEnterInPlacedText(el)) {
              e.preventDefault();
              commitFromSurface(el);
            }
          }}
          onMouseDown={(e) => {
            if (formattingKind === "document") {
              if (e.button !== 0) return;
              const target = e.target as HTMLElement;
              if (target.closest(".table-handles-overlay")) return;
              documentPointerRef.current = {
                x: e.clientX,
                y: e.clientY,
                dragged: false,
              };
              const el = surfaceRef.current;
              el?.focus();
              const anchor = caretRangeAtPoint(e.clientX, e.clientY);
              if (anchor && el && (anchor.commonAncestorContainer === el || el.contains(anchor.commonAncestorContainer))) {
                selectAnchorRef.current = anchor.cloneRange();
              } else {
                selectAnchorRef.current = null;
              }
              return;
            }
            savedRangeRef.current = null;
          }}
          onMouseMove={(e) => {
            const pointer = documentPointerRef.current;
            if (!pointer) return;
            if (!pointer.dragged) {
              if (
                Math.abs(e.clientX - pointer.x) <= 3 &&
                Math.abs(e.clientY - pointer.y) <= 3
              ) {
                return;
              }
              pointer.dragged = true;
            }
            if (formattingKind !== "document") return;
            const el = surfaceRef.current;
            if (!el) return;
            if (!selectAnchorRef.current) {
              const sel = window.getSelection();
              if (sel?.rangeCount) {
                selectAnchorRef.current = sel.getRangeAt(0).cloneRange();
              }
            }
            const anchor = selectAnchorRef.current;
            if (!anchor) return;
            if (extendDocumentSelectionToPoint(el, anchor, e.clientX, e.clientY)) {
              e.preventDefault();
              rememberSelection();
            }
          }}
          onMouseUp={(e) => {
            rememberSelection();
            syncPaletteFocus();
            selectAnchorRef.current = null;
            const pointer = documentPointerRef.current;
            documentPointerRef.current = null;
            if (pointer?.dragged || e.button !== 0) return;
            if (tryOpenFunctionToken(e.target)) {
              e.preventDefault();
              e.stopPropagation();
              return;
            }
            if (formattingKind !== "document") return;
            handleDocumentCanvasClick(e);
          }}
          onDoubleClick={(e) => {
            // Function tokens first — they live inside `.doc-placed-text` on Documents.
            if (tryOpenFunctionToken(e.target)) {
              e.preventDefault();
              e.stopPropagation();
              return;
            }
            if (formattingKind === "document") {
              const placed = (e.target as HTMLElement).closest(".doc-placed-text");
              if (placed instanceof HTMLElement) {
                e.preventDefault();
                focusPlacedBlock(placed);
                registerAsPaletteEditor();
                syncPaletteFocus();
              }
            }
          }}
        />
        {(formattingKind === "document" || formattingKind === "text") && (
          <TableHandlesOverlay editorRef={surfaceRef} onCommit={commitPalette} />
        )}
        {formattingKind === "document" && (
          <PlacedTextHandlesOverlay editorRef={surfaceRef} onCommit={commitPalette} />
        )}
      </div>
    </div>
  );
}
