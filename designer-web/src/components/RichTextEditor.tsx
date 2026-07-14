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
  documentEnterInPlacedText,
  handlePlacedTextArrowKey,
  handleDocumentDeleteBoundary,
  clampDocumentSelectionToLayoutIsland,
  adoptOrphanDocumentContent,
  placeDocumentTextAtPoint,
  PLACED_TEXT_CLASS,
  reflowAllPlacedLines,
  reflowDocumentLayout,
  ensureDocumentTableLayout,
  syncTypingFormatFromCaret,
  resolveDocumentFieldDropTarget,
  pruneEmptyPlacedTextBlocks,
  preserveBlankPlacedLines,
  clearBlankPlacedBlockIfContent,
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
import { openFunctionTokenForEdit } from "@/lib/functionPicker";
import { FUNCTION_TOKEN_CLASS } from "@/lib/functionTokens";
import { isMultiClickSelectionGesture } from "@/lib/wordSelect";
import { isRedundantDefaultFontSize } from "@/lib/fontSizeStrip";
import {
  bookmarkTextOffsets,
  expandRangeToTouchedTokens,
  restoreSelectionFromBookmark,
  type TextOffsetBookmark,
} from "@/lib/selectionBookmark";
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

/**
 * Drop redundant default (12pt / legacy size 3) markers from saved HTML.
 * Must keep explicit palette stops — especially 10pt and 11pt — which the old
 * legacy 1–7 pixel buckets incorrectly treated as default and stripped.
 */
function stripFontSizeFormatting(root: ParentNode) {
  root.querySelectorAll("font[size]").forEach((node) => {
    const size = node.getAttribute("size");
    if (!isRedundantDefaultFontSize(size)) return;

    node.removeAttribute("size");
    if (!node.getAttributeNames().length) {
      unwrapElement(node);
    }
  });

  root.querySelectorAll<HTMLElement>("[style]").forEach((node) => {
    if (!node.style.fontSize) return;
    if (!isRedundantDefaultFontSize(node.style.fontSize)) return;

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
  /** Text-offset bookmark survives Face/Size DOM rewrites and focus-steal Range nudges. */
  const savedBookmarkRef = useRef<TextOffsetBookmark | null>(null);
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
    const range = selection.getRangeAt(0).cloneRange();
    if (!range.collapsed) {
      expandRangeToTouchedTokens(el, range);
    }
    savedRangeRef.current = range;
    savedBookmarkRef.current = !range.collapsed ? bookmarkTextOffsets(el, range) : null;
  };

  const restoreSelection = () => {
    const el = surfaceRef.current;
    const selection = document.getSelection();
    if (!el || !selection) return;
    const bookmark = savedBookmarkRef.current;
    if (bookmark && restoreSelectionFromBookmark(el, bookmark)) return;
    const savedRange = savedRangeRef.current;
    if (!savedRange) return;
    if (!el.contains(savedRange.commonAncestorContainer)) return;
    selection.removeAllRanges();
    try {
      selection.addRange(savedRange);
    } catch {
      /* ignore detached endpoints */
    }
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
      if (formattingKind === "document") {
        ensureDocumentTableLayout(el);
        lastHtml.current = el.innerHTML;
      }
    }
  }, [html, formattingKind]);

  useEffect(() => {
    const el = surfaceRef.current;
    if (!el || !html) return;
    normalizeFieldTokenSpans(el);
    ensureFieldTokensDraggable(el);
    if (formattingKind === "document") {
      ensureDocumentTableLayout(el);
    }
    // Landings mutate the DOM (ZWSP pads). Keep lastHtml in sync so a later
    // blur remount does not wipe pads / shift Face-Size selection bookmarks.
    lastHtml.current = el.innerHTML;
  }, [html, formattingKind]);

  useEffect(() => {
    const el = surfaceRef.current;
    if (el && !el.innerHTML && html) {
      el.innerHTML = html;
      normalizeFieldTokenSpans(el);
      ensureFieldTokensDraggable(el);
      if (formattingKind === "document") {
        ensureDocumentTableLayout(el);
      }
      lastHtml.current = el.innerHTML;
    }
  }, []);

  useEffect(() => {
    const handleSelectionChange = () => {
      const el = surfaceRef.current;
      if (!el) return;
      // Only refresh the saved bookmark while the editor keeps focus. Opening Face/Size
      // <select> steals focus and Chrome often nudges CE=false chip ranges mid-word —
      // overwriting the mousedown save would bake that corruption into the next apply.
      if (document.activeElement !== el) return;
      const selection = document.getSelection();
      if (selectionIsInside(selection, el) && selection?.rangeCount) {
        const range = selection.getRangeAt(0).cloneRange();
        if (!range.collapsed) {
          expandRangeToTouchedTokens(el, range);
        }
        savedRangeRef.current = range;
        savedBookmarkRef.current = !range.collapsed ? bookmarkTextOffsets(el, range) : null;
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
    rememberSelection();
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
    if (!el) return;

    let lastWidth = el.clientWidth;
    let settleTimer: number | undefined;
    const pack = () => {
      const width = el.clientWidth;
      if (Math.abs(width - lastWidth) < 1 && document.visibilityState === "visible") {
        // Still pack when height/MDI chrome changes without a surface width delta.
      } else {
        lastWidth = width;
      }
      reflowDocumentLayout(el);
      window.clearTimeout(settleTimer);
      settleTimer = window.setTimeout(() => {
        commitFromSurfaceRef.current(el);
      }, 120);
    };

    const observer =
      typeof ResizeObserver !== "undefined" ? new ResizeObserver(() => pack()) : null;
    observer?.observe(el);
    // MDI window chrome may resize the editor without always notifying the surface.
    const mdi = el.closest(".mdi-window, .document-editor");
    if (mdi instanceof HTMLElement && mdi !== el) observer?.observe(mdi);
    // MDI / browser window resize can change layout without a reliable surface
    // ResizeObserver tick in some Chromium cases — pack on window resize too.
    window.addEventListener("resize", pack);
    return () => {
      observer?.disconnect();
      window.removeEventListener("resize", pack);
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
        rememberSelection();
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
      savedBookmarkRef.current = null;
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
      if (range) {
        savedRangeRef.current = range.cloneRange();
        savedBookmarkRef.current = null;
      }
      if (moving) {
        moveFieldTokenToSelection(moving);
      } else if (name) {
        insertFieldTokenAtSelection(name);
      }
      rememberSelection();
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
      savedBookmarkRef.current = null;
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
          onBeforeInput={(e) => {
            if (formattingKind !== "document") return;
            const el = surfaceRef.current;
            if (!el) return;
            const ie = e.nativeEvent as InputEvent;
            const inputType = typeof ie.inputType === "string" ? ie.inputType : "";
            if (!inputType.startsWith("delete")) return;
            // Absolute placed lines must not merge into an adjacent table cell on
            // Backspace at the start of a short invent (Chromium joins islands).
            if (handleDocumentDeleteBoundary(el, inputType)) {
              e.preventDefault();
              commitFromSurface(el);
              rememberSelection();
              syncPaletteFocus();
            }
          }}
          onInput={(e) => {
            const target = e.target as HTMLDivElement;
            if (formattingKind === "document") {
              const ie = e.nativeEvent as InputEvent;
              const deleted = typeof ie.inputType === "string" && ie.inputType.startsWith("delete");
              if (deleted) {
                pruneEmptyPlacedTextBlocks(target);
              }
              // Chromium can escape absolute `.doc-placed-text` and leave orphan glyphs
              // as direct children — re-home them before wrap/reflow so the next click
              // does not invent a second paragraph anchor mid-text.
              adoptOrphanDocumentContent(target);
              // Keep Double-Return blank lines from collapsing when editing the next para.
              preserveBlankPlacedLines(target);
              const placed = findPlacedTextBlockAtCaret(target);
              if (placed) {
                clearBlankPlacedBlockIfContent(placed);
                ensurePlacedBlockWrapWidth(target, placed);
              }
              // Full stack pack: a restored blank above the caret must push following lines.
              reflowAllPlacedLines(target);
            }
            commitFromSurface(target);
          }}
          onFocus={() => {
            const el = surfaceRef.current;
            if (el && formattingKind === "document") {
              ensureDocumentTableLayout(el);
            }
            setActiveFieldTarget(insertFieldToken);
            registerAsPaletteEditor();
            syncPaletteFocus();
          }}
          onBlur={(e) => {
            const next = e.relatedTarget as HTMLElement | null;
            if (next?.closest(".formatting-palette")) return;
            if (next?.closest(".table-handles-overlay")) return;
            if (next?.closest(".menu-bar, .menu-drop, .main-icon-toolbar, .modal-overlay, .modal-backdrop, .configure-function-dialog, .insert-function-dialog")) {
              return;
            }
            const el = surfaceRef.current;
            if (el && formattingKind === "document") {
              // Unused invent anchors (click blank, type nothing) go away when focus leaves.
              if (pruneEmptyPlacedTextBlocks(el)) {
                commitFromSurface(el);
              }
            }
            if (formattingKind) clearFormattingFocus(formattingKind);
          }}
          onKeyUp={() => {
            const el = surfaceRef.current;
            if (el && formattingKind === "document") {
              syncTypingFormatFromCaret(el);
            }
            rememberSelection();
            syncPaletteFocus();
          }}
          onKeyDown={(e) => {
            const el = surfaceRef.current;
            if (el && e.key === "Tab" && !e.ctrlKey && !e.metaKey && !e.altKey) {
              if (navigateTableCellOnTab(el, e.shiftKey)) {
                e.preventDefault();
                rememberSelection();
                syncPaletteFocus();
                return;
              }
            }
            if (formattingKind !== "document") return;
            if (!el) return;

            // Fallback when beforeinput is missing: keep Backspace/Delete inside the
            // placed-line / cell island (same contract as onBeforeInput).
            if (
              (e.key === "Backspace" || e.key === "Delete") &&
              !e.ctrlKey &&
              !e.metaKey &&
              !e.altKey
            ) {
              const inputType =
                e.key === "Backspace" ? "deleteContentBackward" : "deleteContentForward";
              if (handleDocumentDeleteBoundary(el, inputType)) {
                e.preventDefault();
                commitFromSurface(el);
                rememberSelection();
                syncPaletteFocus();
                return;
              }
            }

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
              rememberSelection();
              syncPaletteFocus();
            }
          }}
          onMouseDown={(e) => {
            if (e.button === 0 && e.detail === 3) {
              const el = surfaceRef.current;
              if (el && selectParagraphAtPoint(el, e.clientX, e.clientY)) {
                // Custom dblclick word-select + preventDefault breaks native
                // triple-click chaining — select the paragraph ourselves.
                e.preventDefault();
                rememberSelection();
                registerAsPaletteEditor();
                syncPaletteFocus();
                return;
              }
            }
            const el = surfaceRef.current;
            if (el && handleTableCellPointerDown(el, e.target, e.button)) {
              registerAsPaletteEditor();
              // Multi-cell drag: do not start placed-line text selection from here.
              documentPointerRef.current = {
                x: e.clientX,
                y: e.clientY,
                dragged: false,
              };
              selectAnchorRef.current = null;
              rememberSelection();
              syncPaletteFocus();
              return;
            }
            if (el) clearTableCellSelection(el);
            if (formattingKind === "document") {
              if (e.button !== 0) return;
              const target = e.target as HTMLElement;
              if (target.closest(".table-handles-overlay")) return;
              documentPointerRef.current = {
                x: e.clientX,
                y: e.clientY,
                dragged: false,
              };
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
      savedBookmarkRef.current = null;
          }}
          onMouseMove={(e) => {
            const el = surfaceRef.current;
            if (el && handleTableCellPointerMove(el, e.clientX, e.clientY, e.buttons)) {
              e.preventDefault();
              rememberSelection();
              syncPaletteFocus();
              return;
            }
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
            const el = surfaceRef.current;
            if (el) handleTableCellPointerUp(el);
            if (el && formattingKind === "document") {
              // Native drag can bridge absolute placed text and a table cell — clamp
              // before Face/Size / delete see a cross-island range.
              clampDocumentSelectionToLayoutIsland(el);
            }
            rememberSelection();
            if (el && formattingKind === "document") {
              syncTypingFormatFromCaret(el);
            }
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
            // detail ≥ 2: word/paragraph selection — do not collapse via place-at-point.
            if (isMultiClickSelectionGesture(e.detail)) return;
            handleDocumentCanvasClick(e);
          }}
          onDoubleClick={(e) => {
            // Function tokens first — they live inside `.doc-placed-text` on Documents.
            if (tryOpenFunctionToken(e.target)) {
              e.preventDefault();
              e.stopPropagation();
              return;
            }

            const el = surfaceRef.current;
            if (!el) return;

            // Field token: select it — never let a stray dblclick invent another insert.
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
                rememberSelection();
              }
              registerAsPaletteEditor();
              syncPaletteFocus();
              return;
            }

            // Word select across mixed style spans (native often chops mid-word at boundaries).
            if (selectWordOrTokenAtPoint(el, e.clientX, e.clientY)) {
              e.preventDefault();
              e.stopPropagation();
              rememberSelection();
              registerAsPaletteEditor();
              syncPaletteFocus();
              return;
            }

            // Free space / empty invent: mouseup skips place for detail≥2 (word/para
            // protect). Without a fallthrough invent here, dblclick on blank canvas
            // leaves a dead Document — no caret, no typing.
            if (formattingKind === "document") {
              const target = e.target as HTMLElement;
              if (target.closest("table.user")) return;
              if (target.closest(".table-handles-overlay")) return;
              e.preventDefault();
              e.stopPropagation();
              handleDocumentCanvasClick(e);
              rememberSelection();
              registerAsPaletteEditor();
              syncPaletteFocus();
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
