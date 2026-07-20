/** Click-to-place text on the Document canvas (free-positioned blocks). */

import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
} from "./paletteDefaults";
import {
  getTypingFormat,
  setTypingFormat,
  typingFormatForInsert,
  type TypingFormat,
} from "./paletteTypingFormat";
import { FIELD_TOKEN_CLASS } from "./fieldTokens";
import { FUNCTION_TOKEN_CLASS } from "./functionTokens";
import { listUserTableCells } from "./tableCellSelection";
import {
  formatPt,
  getAbsolutePositionPt,
  getLayoutHomeTopPt,
  parseCssPt,
  pxToPt,
  setAbsolutePositionPt,
  setLayoutHomeTopPt,
} from "./tableLayout";
import { wordBoundsInText } from "./wordSelect";
import { CARET_ZWSP, ensureTokenCaretLanding, placeCaretAfterToken, placeCaretBeforeToken } from "./tokenCaretLanding";

const PLACED_TEXT_CLASS = "doc-placed-text";
/** Marks an intentional blank placed line from Double-Return (keep until user deletes it). */
export const DOC_BLANK_ATTR = "data-doc-blank";
const LINE_TOLERANCE_PT = 2;
/**
 * Minimum left inset for invent / indent-0 (pt). Absolute children originate at the
 * padding edge of `.rich-surface`, so left:0 sits under the MDI/chrome border and
 * clips the caret — always keep at least this inset on invent.
 */
export const DOC_LINE_MARGIN_PT = pxToPt(10);
/** Half-inch indent step (legacy paragraph indent feel). */
export const DOC_INDENT_STEP_PT = 36;
const MAX_DOC_INDENT_LEVEL = 16;

export { PLACED_TEXT_CLASS };

/** Caret Range at a viewport point, across Chromium and Firefox. */
export function caretRangeAtPoint(x: number, y: number): Range | null {
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

/**
 * Viewport click → pt offset for absolute placement.
 * Origin matches CSS absolute containing block (padding edge of the editor) —
 * do not subtract padding, or content-area clicks map to left≈0 and hide the caret.
 */
export function clientPointToEditorPt(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): { left: number; top: number } {
  const rect = editor.getBoundingClientRect();
  return {
    left: pxToPt(clientX - rect.left),
    top: pxToPt(clientY - rect.top),
  };
}

function isPlacedTextBlock(node: Node | null): node is HTMLElement {
  return node instanceof HTMLElement && node.classList.contains(PLACED_TEXT_CLASS);
}

function isEditableTextTarget(node: Node | null, editor: HTMLElement): boolean {
  if (!node || node === editor) return false;
  if (isPlacedTextBlock(node)) return true;
  if (node instanceof HTMLTableCellElement) return true;
  if (node instanceof HTMLElement) {
    if (node.closest("table.user")) return true;
    if (node.closest(`.${PLACED_TEXT_CLASS}`)) return true;
    const tag = node.tagName;
    if (tag === "P" || tag === "DIV" || tag === "SPAN" || tag === "BR") return true;
  }
  if (node.nodeType === Node.TEXT_NODE && node.textContent?.replace(/\u00a0|\u200b/g, "").trim()) {
    return true;
  }
  return false;
}

/** True when a range / node sits inside a Document/Form `table.user`. */
function rangeInsideUserTable(editor: HTMLElement, range: Range | null): boolean {
  if (!range || !editor.contains(range.commonAncestorContainer)) return false;
  let node: Node | null = range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  if (!(node instanceof Element)) return false;
  const table = node.closest("table.user");
  return !!(table && editor.contains(table));
}

/**
 * True when a caret range already sits in typed glyphs (placed line or orphan text).
 * Orphan text is a direct text-node child of the contenteditable — `elementFromPoint`
 * then returns the editor itself, and `caretRangeFromPoint` may report the editor with
 * a child offset instead of the Text node. Neither case must invent a new placed line.
 *
 * Absolute `table.user` must NOT count as prose here: with only absolute children,
 * `caretRangeFromPoint` often lands inside the table when the user clicks blank
 * canvas, which previously blocked inventing a new `.doc-placed-text` forever.
 */
export function rangeIntersectsExistingDocumentText(
  editor: HTMLElement,
  range: Range | null,
): boolean {
  if (!range || !editor.contains(range.commonAncestorContainer)) return false;
  // Table cells are not placeable prose anchors — hit-testing decides table clicks.
  if (rangeInsideUserTable(editor, range)) return false;

  const placed = findPlacedTextBlock(range.startContainer, editor);
  if (placed) return true;

  if (range.startContainer.nodeType === Node.TEXT_NODE) {
    return meaningfulText(range.startContainer.textContent);
  }

  // Editor root + child offset (common for orphan Text under an absolutely positioned canvas).
  if (range.startContainer === editor) {
    const idx = Math.min(range.startOffset, editor.childNodes.length);
    const around = [
      editor.childNodes[idx],
      editor.childNodes[idx - 1],
      editor.childNodes[Math.max(0, idx - 1)],
    ];
    for (const child of around) {
      if (!child) continue;
      if (child.nodeType === Node.TEXT_NODE && meaningfulText(child.textContent)) return true;
      if (child instanceof HTMLElement) {
        if (child.matches("table.user")) continue;
        if (child.classList.contains(PLACED_TEXT_CLASS)) return true;
        if (meaningfulText(child.textContent)) return true;
      }
    }
  }

  if (range.commonAncestorContainer instanceof HTMLElement) {
    const el = range.commonAncestorContainer;
    if (el === editor) return false;
    if (el.closest("table.user")) return false;
    if (isEditableTextTarget(el, editor) && meaningfulText(el.textContent)) return true;
  }

  return false;
}

/** True when the editor has loose text/nodes outside `.doc-placed-text`. */
export function hasOrphanDocumentContent(editor: HTMLElement): boolean {
  return listOrphanDocumentNodes(editor).length > 0;
}

/** True when the click should create a new free-positioned text block. */
export function shouldPlaceDocumentText(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  range: Range | null,
): boolean {
  const hit = document.elementFromPoint(clientX, clientY);
  if (hit instanceof HTMLElement && hit.closest(".table-handles-overlay")) return false;

  // Click on / inside an existing placed line — never spawn another anchor.
  if (hit instanceof Element) {
    const placed = hit.closest(`.${PLACED_TEXT_CLASS}`);
    if (placed instanceof HTMLElement && editor.contains(placed)) return false;
  }

  // Existing glyphs under the caret (including orphan text under the editor root).
  if (rangeIntersectsExistingDocumentText(editor, range)) return false;

  // Orphan text is painted under this point — do not overlay a new empty anchor.
  if (hasOrphanDocumentContent(editor) && orphanTextUnderPoint(editor, clientX, clientY)) {
    return false;
  }

  // Geometric/DOM cell under the point — never invent over a table (even when
  // caretRangeFromPoint is blank-canvas noise elsewhere).
  if (findUserTableCellAtPoint(editor, clientX, clientY)) return false;

  if (hit instanceof HTMLElement && editor.contains(hit)) {
    if (hit.closest("table.user")) return false;
    if (isEditableTextTarget(hit, editor) && range) {
      const container = range.commonAncestorContainer;
      if (container !== editor && editor.contains(container)) return false;
    }
  }

  if (hit === editor) {
    // Bare text under click → hit is editor. Never treat as blank canvas when glyphs exist.
    if (rangeIntersectsExistingDocumentText(editor, range)) return false;
    if (orphanTextUnderPoint(editor, clientX, clientY)) return false;
    return true;
  }

  if (!range || !editor.contains(range.commonAncestorContainer)) return true;

  if (range.commonAncestorContainer === editor) {
    if (rangeIntersectsExistingDocumentText(editor, range)) return false;
    return true;
  }

  return false;
}

/** True when any orphan text node’s painted box covers (clientX, clientY). */
function orphanTextUnderPoint(editor: HTMLElement, clientX: number, clientY: number): boolean {
  for (const child of Array.from(editor.childNodes)) {
    if (child instanceof HTMLElement && child.classList.contains(PLACED_TEXT_CLASS)) continue;
    if (child instanceof HTMLElement && child.matches("table.user")) continue;
    if (child.nodeType !== Node.TEXT_NODE && !(child instanceof HTMLElement)) continue;
    if (child.nodeType === Node.TEXT_NODE && !meaningfulText(child.textContent)) continue;

    const probe = document.createRange();
    try {
      if (child.nodeType === Node.TEXT_NODE) probe.selectNodeContents(child);
      else probe.selectNode(child);
    } catch {
      continue;
    }
    for (const rect of Array.from(probe.getClientRects())) {
      if (
        clientX >= rect.left &&
        clientX <= rect.right &&
        clientY >= rect.top &&
        clientY <= rect.bottom
      ) {
        return true;
      }
    }
  }
  return false;
}

function meaningfulText(text: string | null | undefined): boolean {
  return (text ?? "").replace(/\u00a0|\u200b/g, "").trim().length > 0;
}

/** True when a placed line has no glyphs and no field/function tokens / embedded images. */
export function isPlacedTextBlockEmpty(block: HTMLElement): boolean {
  if (block.querySelector(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`)) return false;
  if (block.querySelector("img.tawala-embedded-image, img[data-tawala-image-id]")) return false;
  return !meaningfulText(block.textContent);
}

/** True when this empty placed line is an intentional Double-Return gap between content. */
export function isIntentionalBlankPlacedBlock(
  block: HTMLElement,
  all: HTMLElement[] = [],
  emptySet?: Set<HTMLElement>,
): boolean {
  if (block.getAttribute(DOC_BLANK_ATTR) === "1") return true;
  if (!all.length) return false;
  const empties = emptySet ?? new Set(all.filter(isPlacedTextBlockEmpty));
  if (!empties.has(block)) return false;
  const idx = all.indexOf(block);
  if (idx < 0) return false;
  const hasContentAbove = all.slice(0, idx).some((b) => !empties.has(b));
  const hasContentBelow = all.slice(idx + 1).some((b) => !empties.has(b));
  return hasContentAbove && hasContentBelow;
}

/**
 * Scaffold / mark an empty placed line so Double-Return gaps keep one line-height
 * through packing and sibling edits (Chromium often strips bare `<br>` husks).
 */
export function markBlankPlacedBlock(block: HTMLElement): void {
  block.setAttribute(DOC_BLANK_ATTR, "1");
  ensureBlankPlacedScaffold(block);
}

/** Clear blank mark once the line has real glyphs or tokens. */
export function clearBlankPlacedBlockIfContent(block: HTMLElement): void {
  if (!isPlacedTextBlockEmpty(block)) {
    block.removeAttribute(DOC_BLANK_ATTR);
    block.style.removeProperty("min-height");
  }
}

function ensureBlankPlacedScaffold(block: HTMLElement): void {
  if (!isPlacedTextBlockEmpty(block)) return;
  if (!block.querySelector("br")) {
    block.innerHTML = "<br>";
  }
  const linePt = placedBlockLineHeightPt(block);
  block.style.minHeight = formatPt(linePt);
}

/**
 * After typing in a neighboring line: restore blank scaffolds and clear marks on
 * lines that now have content. Call from Document `onInput`.
 */
export function preserveBlankPlacedLines(editor: HTMLElement): void {
  for (const block of listPlacedBlocksSorted(editor)) {
    if (isPlacedTextBlockEmpty(block)) {
      if (block.getAttribute(DOC_BLANK_ATTR) === "1") {
        ensureBlankPlacedScaffold(block);
      }
    } else {
      clearBlankPlacedBlockIfContent(block);
    }
  }
}

/**
 * After select-all + Delete (or backspace-to-empty), remove husk `.doc-placed-text`
 * lines so they do not remain as invisible snap/pack slots. Intentional blank lines
 * from Double-Return (between content paragraphs) are kept until the user deletes them.
 *
 * `except` — keep these blocks even when empty (active invent / destination of a click).
 * `keepCaretEmpty` — when true, do not remove the placed line that currently holds the caret
 * (used while the designer is still focused in an unused invent anchor).
 * `restoreFocus` — when false, never steal/clear the selection (table clicks / place finish).
 * Default true only repositions when the caret’s placed line was among the removals.
 * `onlyCaretBlock` — when true, remove at most the empty block that held the caret (one
 * Backspace should not clear every trailing blank invent in the Document).
 */
export function pruneEmptyPlacedTextBlocks(
  editor: HTMLElement,
  options?: {
    except?: HTMLElement | HTMLElement[] | null;
    keepCaretEmpty?: boolean;
    restoreFocus?: boolean;
    onlyCaretBlock?: boolean;
  },
): boolean {
  const all = listPlacedBlocksSorted(editor);
  const empties = all.filter(isPlacedTextBlockEmpty);
  if (!empties.length) return false;

  const exceptSet = new Set(
    (Array.isArray(options?.except)
      ? options.except
      : options?.except
        ? [options.except]
        : []
    ).filter((n): n is HTMLElement => n instanceof HTMLElement),
  );
  const sel = window.getSelection();
  const caretBlock =
    sel?.rangeCount && sel.anchorNode && editor.contains(sel.anchorNode)
      ? findPlacedTextBlock(sel.anchorNode, editor)
      : null;
  if (options?.keepCaretEmpty && caretBlock) exceptSet.add(caretBlock);

  const emptySet = new Set(empties);
  let toRemove = empties.filter(
    (empty) =>
      !exceptSet.has(empty) && !isIntentionalBlankPlacedBlock(empty, all, emptySet),
  );
  if (options?.onlyCaretBlock) {
    toRemove = caretBlock && toRemove.includes(caretBlock) ? [caretBlock] : [];
  }
  if (!toRemove.length) {
    // Still repair scaffolds on kept blanks (delete may have stripped `<br>`).
    for (const empty of empties) {
      if (isIntentionalBlankPlacedBlock(empty, all, emptySet)) {
        markBlankPlacedBlock(empty);
      }
    }
    return false;
  }

  const removeSet = new Set(toRemove);
  const caretWasRemoved = !!(caretBlock && removeSet.has(caretBlock));
  const restoreFocus = options?.restoreFocus !== false;

  let focusAfter: HTMLElement | null = null;
  if (restoreFocus && caretWasRemoved) {
    for (let i = 0; i < all.length && !focusAfter; i++) {
      if (!removeSet.has(all[i])) continue;
      for (let j = i - 1; j >= 0; j--) {
        if (!removeSet.has(all[j]) && !emptySet.has(all[j])) {
          focusAfter = all[j];
          break;
        }
      }
      if (focusAfter) break;
      for (let j = i + 1; j < all.length; j++) {
        if (!removeSet.has(all[j]) && !emptySet.has(all[j])) {
          focusAfter = all[j];
          break;
        }
      }
    }
  }

  for (const empty of toRemove) {
    empty.remove();
  }

  // Keep / re-scaffold intentional blanks that survived.
  preserveBlankPlacedLines(editor);

  if (listPlacedBlocksSorted(editor).length) {
    reflowAllPlacedLines(editor);
  }

  // Do not clear a table-cell (or other non-placed) selection after pruning husks.
  if (restoreFocus && caretWasRemoved) {
    if (focusAfter && editor.contains(focusAfter)) {
      focusPlacedBlockEnd(focusAfter);
    } else if (sel) {
      sel.removeAllRanges();
    }
  }
  return true;
}

function findPlacedTextBlock(node: Node | null, editor: HTMLElement): HTMLElement | null {
  let current: Node | null = node;
  if (current?.nodeType === Node.TEXT_NODE) current = current.parentNode;
  while (current && current !== editor) {
    if (current instanceof HTMLTableCellElement) return null;
    if (current instanceof HTMLElement && current.classList.contains(PLACED_TEXT_CLASS)) {
      return current;
    }
    current = current.parentNode;
  }
  return null;
}

/** Placed-text line containing the caret, if any. */
export function findPlacedTextBlockAtCaret(editor: HTMLElement): HTMLElement | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  return findPlacedTextBlock(sel.getRangeAt(0).startContainer, editor);
}

/** Apply palette typing attributes to a document line block (inherited by typed characters). */
export function applyTypingFormatToPlacedBlock(block: HTMLElement, typing: TypingFormat): void {
  applyTypingFormatStylePatch(block.style, typing);
}

/**
 * Face/size/color patch for a new paragraph after Return (Document placed line or Form Text
 * `<div>`/`<p>`). Callers must pass sticky/caret typing attrs — never copy bare
 * `getComputedStyle(parent)` (that ignored inline font spans and reset to Arial 12).
 */
export function applyTypingFormatStylePatch(
  style: CSSStyleDeclaration,
  typing: TypingFormat,
): void {
  if (typing.fontSize === String(DEFAULT_PALETTE_FONT_SIZE_PT)) {
    style.removeProperty("font-size");
  } else {
    style.fontSize = `${typing.fontSize}pt`;
  }
  if (typing.fontFace === DEFAULT_PALETTE_FONT_FACE) {
    style.removeProperty("font-family");
  } else {
    style.fontFamily = typing.fontFace;
  }
  if (typing.color) {
    style.color = typing.color;
  } else {
    style.removeProperty("color");
  }
}

/** Apply face/size/color/B/I/U to a field or function token (overrides badge CSS). */
export function applyTypingFormatToToken(el: HTMLElement, typing: TypingFormat): void {
  if (typing.fontSize === String(DEFAULT_PALETTE_FONT_SIZE_PT)) {
    el.style.removeProperty("font-size");
  } else {
    el.style.fontSize = `${typing.fontSize}pt`;
  }
  if (typing.fontFace === DEFAULT_PALETTE_FONT_FACE) {
    el.style.removeProperty("font-family");
  } else {
    el.style.fontFamily = typing.fontFace;
  }
  if (typing.color) {
    el.style.setProperty("color", typing.color);
  } else {
    el.style.removeProperty("color");
  }
  if (typing.bold) {
    el.style.fontWeight = "bold";
  } else {
    el.style.removeProperty("font-weight");
  }
  if (typing.italic) {
    el.style.fontStyle = "italic";
  } else {
    el.style.removeProperty("font-style");
  }
  if (typing.underline) {
    el.style.textDecoration = "underline";
  } else {
    el.style.removeProperty("text-decoration");
  }
  // Baseline + inherit line-height so chips match the text run (do not force
  // line-height:1 — that + inline-block border blew out Document line boxes).
  el.style.verticalAlign = "baseline";
  el.style.removeProperty("line-height");
}

export type DocumentAlign = "left" | "center" | "right" | "justify";

/** Content box metrics for Document line alignment (pt, content-relative like placed `left`). */
export function getDocumentContentMetrics(editor: HTMLElement): {
  marginPt: number;
  contentWidthPt: number;
} {
  const style = getComputedStyle(editor);
  const padL = Number.parseFloat(style.paddingLeft) || 0;
  const padR = Number.parseFloat(style.paddingRight) || 0;
  const innerPx = Math.max(1, editor.clientWidth - padL - padR);
  return {
    marginPt: DOC_LINE_MARGIN_PT,
    contentWidthPt: Math.max(DOC_LINE_MARGIN_PT * 2 + 1, pxToPt(innerPx)),
  };
}

/** True when a placed line uses Center / Right / Justify (dataset or style). */
export function placedBlockHasExplicitAlign(block: HTMLElement): boolean {
  return readPlacedTextAlign(block) !== "left";
}

/**
 * Keep `data-doc-align` in sync with `style.text-align` so remount / wrap-width
 * restore Center/Right/Justify to the full margin box (not a shrink-wrapped left strip).
 */
export function normalizePlacedTextAlign(block: HTMLElement): DocumentAlign {
  const align = readPlacedTextAlign(block);
  if (align === "left") {
    if (block.style.textAlign && block.style.textAlign !== "left") {
      block.style.textAlign = "left";
    }
    if (block.dataset.docAlign) delete block.dataset.docAlign;
  } else {
    block.style.textAlign = align;
    block.dataset.docAlign = align;
  }
  return align;
}

/**
 * Leading whitespace that makes Align Left look indented. Indent via palette uses
 * `margin-left` / `data-doc-indent` — never strip those; only text characters.
 */
const LEADING_ALIGN_WS = /^[ \t\u00a0\u200b]+/;

/**
 * Remove leading spaces/tabs/nbsp/ZWSP from the start of each soft line in a block
 * so Align Left sits flush at the left edge (intentional Indent margin stays).
 */
export function stripLeadingWhitespaceForLeftAlign(block: HTMLElement): void {
  stripLeadingAlignWhitespaceFrom(block.firstChild, block);
  for (const br of Array.from(block.querySelectorAll("br"))) {
    stripLeadingAlignWhitespaceFrom(br.nextSibling, block);
  }
}

/** Next node after `node` still under `boundary` (climbs out of empty wrappers). */
function nextSiblingWithin(node: Node, boundary: Node): Node | null {
  let cur: Node | null = node;
  while (cur && cur !== boundary) {
    if (cur.nextSibling) return cur.nextSibling;
    cur = cur.parentNode;
  }
  return null;
}

/** Walk from `start` (line head) until real content; strip LEADING_ALIGN_WS text nodes. */
function stripLeadingAlignWhitespaceFrom(start: Node | null, boundary: HTMLElement): void {
  let node: Node | null = start;
  while (node && boundary.contains(node)) {
    if (node instanceof HTMLElement) {
      if (node.tagName === "BR") return;
      if (
        node.classList.contains(FIELD_TOKEN_CLASS) ||
        node.classList.contains(FUNCTION_TOKEN_CLASS)
      ) {
        return;
      }
      if (node.firstChild) {
        node = node.firstChild;
        continue;
      }
      node = nextSiblingWithin(node, boundary);
      continue;
    }
    if (node.nodeType === Node.TEXT_NODE) {
      const text = node as Text;
      const cleaned = text.data.replace(LEADING_ALIGN_WS, "");
      if (cleaned.length === 0) {
        const next = nextSiblingWithin(text, boundary);
        text.remove();
        node = next;
        continue;
      }
      if (cleaned !== text.data) text.data = cleaned;
      return;
    }
    node = nextSiblingWithin(node, boundary);
  }
}

/**
 * Single-line Document alignment relative to left/right margins (not shrink-wrapped text-align alone).
 * Text wraps within the margin box; height growth packs lines below. Preserves indent level.
 * Align Left also strips leading whitespace characters so the visual edge matches Left.
 */
export function alignPlacedTextBlock(
  editor: HTMLElement,
  block: HTMLElement,
  align: DocumentAlign,
): void {
  if (align === "left") {
    stripLeadingWhitespaceForLeftAlign(block);
    block.style.textAlign = "left";
    delete block.dataset.docAlign;
  } else {
    block.style.textAlign = align;
    block.dataset.docAlign = align;
  }
  applyPlacedIndentLayout(editor, block);
  reflowPlacedLinesBelow(editor, block);
}

/** Indent level stored on a placed line (`0` = at left margin). */
export function readPlacedIndentLevel(block: HTMLElement): number {
  const raw = Number(block.dataset.docIndent ?? "");
  if (Number.isFinite(raw) && raw >= 0) return Math.min(MAX_DOC_INDENT_LEVEL, Math.floor(raw));
  return 0;
}

/**
 * Nudge a placed line’s left edge by one indent step (clamped to the left margin).
 * Width always runs to the right margin so wrap-on-type still applies.
 */
export function adjustPlacedTextIndent(
  editor: HTMLElement,
  block: HTMLElement,
  delta: 1 | -1,
): void {
  const { marginPt, contentWidthPt } = getDocumentContentMetrics(editor);
  const rightEdge = contentWidthPt - marginPt;
  const { left, top } = getAbsolutePositionPt(block);

  let level = readPlacedIndentLevel(block);
  if (block.dataset.docIndent == null || block.dataset.docIndent === "") {
    level = Math.max(0, Math.round((left - marginPt) / DOC_INDENT_STEP_PT));
  }
  level = Math.max(0, Math.min(MAX_DOC_INDENT_LEVEL, level + delta));
  block.dataset.docIndent = String(level);

  const nextLeft = Math.max(marginPt, Math.min(rightEdge - 24, marginPt + level * DOC_INDENT_STEP_PT));
  block.style.position = "absolute";
  block.style.top = formatPt(top);
  block.style.left = formatPt(nextLeft);
  ensurePlacedBlockWrapWidth(editor, block);
  // Force layout so wrap height is current before packing siblings (avoids overlap).
  void block.offsetHeight;
}

/** Apply left/width from indent level (+ optional align) between document margins. */
function applyPlacedIndentLayout(editor: HTMLElement, block: HTMLElement): void {
  const { top } = getAbsolutePositionPt(block);
  const { marginPt, contentWidthPt } = getDocumentContentMetrics(editor);
  const level = readPlacedIndentLevel(block);
  const left = marginPt + level * DOC_INDENT_STEP_PT;
  const rightEdge = contentWidthPt - marginPt;
  const width = Math.max(1, rightEdge - left);

  block.style.position = "absolute";
  block.style.top = formatPt(top);
  block.style.left = formatPt(left);
  block.style.width = formatPt(width);
  block.style.right = "auto";
  applyPlacedBlockWrapStyles(block);
}

/**
 * Stop a placed line’s wrap box at the left edge of any same-band user table to its right.
 * Prevents full-width (or oversized) hit boxes from covering free space beside the table —
 * click-invent to the right must see the editor, not a left/full-width line’s empty width.
 */
function clipPlacedBlockWrapBeforeTables(editor: HTMLElement, block: HTMLElement): void {
  const { left, top } = getAbsolutePositionPt(block);
  const { marginPt, contentWidthPt } = getDocumentContentMetrics(editor);
  const rightEdge = contentWidthPt - marginPt;
  const lineH = Math.max(placedBlockLayoutHeightPt(block), placedBlockLineHeightPt(block));
  let maxRight = rightEdge;

  for (const item of listDocumentUserTables(editor)) {
    if (item === block) continue;
    const tb = layoutItemBoxPt(item);
    // Same vertical band (or overlapping) — clip wrap before the table.
    if (!rangesOverlap1d(top, lineH, tb.top, tb.height, 4)) continue;
    if (tb.left > left + 1) {
      maxRight = Math.min(maxRight, tb.left - 2);
    }
  }

  const width = Math.max(1, maxRight - Math.min(left, maxRight - 1));
  block.style.width = formatPt(width);
  block.style.right = "auto";
}

/**
 * Constrain a placed line from its current `left` to the right margin and enable wrapping.
 * Typing past the right edge soft-wraps within the block; callers should reflow below.
 *
 * Document tables: wrap width stops at the left edge of any user table that sits to the
 * right on the same vertical band, so left-of-table prose stays beside the table instead
 * of claiming the table’s X range (which made packing treat them as one stack column).
 * Aligned / indented lines keep the margin+indent grid, then the same table clip so
 * Centering does not leave a full-width invisible hit box across the table and its right.
 */
export function ensurePlacedBlockWrapWidth(editor: HTMLElement, block: HTMLElement): void {
  // Center/Right/Justify need the margin box for text-align to read as page alignment;
  // style-only text-align (after remount without dataset) must take the same path.
  normalizePlacedTextAlign(block);
  const indented = block.dataset.docIndent != null && block.dataset.docIndent !== "";
  if (placedBlockHasExplicitAlign(block) || indented) {
    applyPlacedIndentLayout(editor, block);
    clipPlacedBlockWrapBeforeTables(editor, block);
    applyPlacedBlockWrapStyles(block);
    return;
  }
  clipPlacedBlockWrapBeforeTables(editor, block);
  applyPlacedBlockWrapStyles(block);
}

function applyPlacedBlockWrapStyles(block: HTMLElement): void {
  block.style.whiteSpace = "normal";
  // Do NOT use overflow-wrap:break-word — deleting a space glues words into one
  // long token that mid-breaks ("fashioned" / "names"), then re-spacing cannot
  // undo the visual wrap. Wrap only at real spaces; long chips may overflow.
  block.style.overflowWrap = "normal";
  block.style.wordBreak = "normal";
  block.dataset.docWrap = "1";
}

/** Top-level Document `table.user` elements (not nested in another table). */
function listDocumentUserTables(editor: HTMLElement): HTMLTableElement[] {
  return Array.from(editor.querySelectorAll("table.user")).filter(
    (n): n is HTMLTableElement =>
      n instanceof HTMLTableElement && editor.contains(n) && !n.parentElement?.closest("table.user"),
  );
}

/** Absolute layout box in pt for a placed line or user table. */
export function layoutItemBoxPt(item: HTMLElement): {
  left: number;
  top: number;
  width: number;
  height: number;
} {
  const { left, top } = getAbsolutePositionPt(item);
  const height = layoutItemHeightPt(item);
  let width = parseCssPt(item.style.width);
  if (width < 1) {
    const measured = pxToPt(item.getBoundingClientRect().width);
    width = measured >= 1 ? measured : isDocumentUserTable(item) ? 120 : 40;
  }
  return { left, top, width, height };
}

function rangesOverlap1d(
  aStart: number,
  aSize: number,
  bStart: number,
  bSize: number,
  slack = 0,
): boolean {
  return aStart < bStart + bSize + slack && bStart < aStart + aSize + slack;
}

/** True when two absolute layout boxes share horizontal space (side-by-side = false). */
export function layoutBoxesHorizontallyOverlap(
  a: { left: number; width: number },
  b: { left: number; width: number },
  slack = 1,
): boolean {
  return rangesOverlap1d(a.left, a.width, b.left, b.width, -slack);
}

/**
 * Resolve same-column overlaps for absolute placed lines / tables.
 *
 * Each item targets `max(homeTop, collisionFloor)`:
 * - Collision push moves `style.top` down without rewriting home — so table ✥
 *   drag and window widen can restore prior positions when space returns.
 * - Intentional gaps stay (home sits below free space; we do not pack-to-tight).
 * - Side-by-side (no horizontal overlap) items ignore each other’s floors.
 */
export function resolveDocumentLayoutCollisions(editor: HTMLElement): void {
  const gapPt = 2;
  // Order by intentional home (not displaced current top) so a restored item is
  // placed before later siblings and cannot land underneath them after pull-up.
  const items = listDocumentLayoutItemsSorted(editor)
    .slice()
    .sort((a, b) => {
      const ha = getLayoutHomeTopPt(a);
      const hb = getLayoutHomeTopPt(b);
      if (Math.abs(ha - hb) > 0.5) return ha - hb;
      return getAbsolutePositionPt(a).top - getAbsolutePositionPt(b).top;
    });
  for (let i = 0; i < items.length; i++) {
    const item = items[i];
    void item.offsetHeight;
    const homeTop = getLayoutHomeTopPt(item);
    // Measure width/height at current paint; X is unchanged by this pass.
    const box = layoutItemBoxPt(item);
    let floor = 0;
    for (let j = 0; j < i; j++) {
      const prior = items[j];
      const pb = layoutItemBoxPt(prior);
      if (!layoutBoxesHorizontallyOverlap(box, pb)) continue;
      floor = Math.max(floor, pb.top + pb.height + gapPt);
    }
    const targetTop = Math.max(homeTop, floor);
    const pos = getAbsolutePositionPt(item);
    if (Math.abs(pos.top - targetTop) > 0.5) {
      item.style.top = formatPt(targetTop);
    }
  }
}

/**
 * Pack following placed lines / tables under `block` only when they collide horizontally.
 * Side-by-side prose + tables keep independent tops; intentional gaps are not closed.
 *
 * Height is the rendered line box (tallest glyph/token on the line), not the last
 * palette action:
 * - Enlarging a single word/character can grow the box and push overlapping lines below.
 * - Lines / tables that were only collision-displaced restore toward `data-doc-home-top`
 *   when the overlapping item moves away or the window widens again.
 *
 * Document `table.user` items participate so remount cannot leave prose under a table
 * in the same column.
 */
export function reflowPlacedLinesBelow(editor: HTMLElement, block: HTMLElement): void {
  const blocks = listDocumentLayoutItemsSorted(editor);
  if (blocks.indexOf(block) < 0) return;
  // Force layout before measuring — width/indent changes must settle first.
  void block.offsetHeight;
  resolveDocumentLayoutCollisions(editor);
}

/**
 * Re-apply wrap widths for every placed line and resolve overlaps (window / MDI resize).
 * Clamps left edges that overflow a narrower content box so text/fields/tables do not
 * keep absolute anchors off-canvas. Tables keep owner home X/Y when they do not collide
 * with prose in the same column (no forced pack-under; intentional gaps / beside stay).
 * Widen after a narrow push restores toward each item’s `data-doc-home-top`.
 */
export function reflowAllPlacedLines(editor: HTMLElement): void {
  normalizeDocumentUserTables(editor);
  const { marginPt, contentWidthPt } = getDocumentContentMetrics(editor);
  const rightEdge = contentWidthPt - marginPt;
  const blocks = listDocumentLayoutItemsSorted(editor);
  if (!blocks.length) return;
  for (const block of blocks) {
    const pos = getAbsolutePositionPt(block);
    if (pos.left > rightEdge - 24) {
      block.style.left = formatPt(Math.max(marginPt, rightEdge - 24));
    } else if (
      pos.left < marginPt - 0.5 &&
      block.classList.contains(PLACED_TEXT_CLASS) &&
      !placedBlockHasExplicitAlign(block) &&
      (block.dataset.docIndent == null || block.dataset.docIndent === "")
    ) {
      // Free-placed lines that drifted left of the margin snap back on shrink.
      block.style.left = formatPt(marginPt);
    }
    if (block.classList.contains(PLACED_TEXT_CLASS)) {
      ensurePlacedBlockWrapWidth(editor, block);
    }
  }
  // Force layout once after width changes, then collision-pack (no pull-up).
  void editor.offsetHeight;
  resolveDocumentLayoutCollisions(editor);
}

/**
 * Document window / browser resize entry point — same packing as `reflowAllPlacedLines`.
 * Prefer this name from ResizeObserver / window `resize` hooks.
 */
export function reflowDocumentLayout(editor: HTMLElement): void {
  reflowAllPlacedLines(editor);
}

/**
 * Convert Document `table.user` elements from float/relative (legacy insert) to
 * absolute positions in the placed-text stack, and hoist tables nested inside
 * `.doc-placed-text`. Call after HTML remount / insert so tables reserve space.
 */
export function normalizeDocumentUserTables(editor: HTMLElement): boolean {
  let changed = false;
  const tables = Array.from(editor.querySelectorAll("table.user")).filter(
    (n): n is HTMLTableElement => n instanceof HTMLTableElement,
  );

  for (const table of tables) {
    if (table.parentElement?.closest("table.user")) continue;

    const host = table.closest(`.${PLACED_TEXT_CLASS}`);
    if (host instanceof HTMLElement && host !== table && editor.contains(host)) {
      // Nested in a placed line — hoist to editor so absolute tops are document-scoped.
      const hostPos = getAbsolutePositionPt(host);
      const hostHeight = placedBlockLayoutHeightPt(host);
      const left = parseCssPt(table.style.left) || hostPos.left || DOC_LINE_MARGIN_PT;
      const top =
        parseCssPt(table.style.top) > 20
          ? parseCssPt(table.style.top)
          : hostPos.top + Math.max(hostHeight, 0) + 2;
      host.after(table);
      table.style.position = "absolute";
      table.style.left = formatPt(left);
      table.style.top = formatPt(top);
      setLayoutHomeTopPt(table, top);
      table.style.float = "";
      table.style.marginRight = "";
      table.style.marginLeft = "";
      table.style.marginTop = "";
      changed = true;
      continue;
    }

    const float = (table.style.float || "").toLowerCase();
    const pos = (table.style.position || "").toLowerCase();
    if (float || pos === "relative" || pos !== "absolute") {
      const left = parseCssPt(table.style.left) || DOC_LINE_MARGIN_PT;
      const top = parseCssPt(table.style.top);
      table.style.float = "";
      table.style.marginRight = "";
      table.style.marginLeft = "";
      table.style.marginTop = "";
      table.style.position = "absolute";
      table.style.left = formatPt(left);
      // Keep existing top when present so reflow can order vs placed lines.
      table.style.top = formatPt(top);
      setLayoutHomeTopPt(table, top);
      changed = true;
    }
  }
  return changed;
}

/**
 * After Document HTML hydrate or leave/return: normalize tables into absolute
 * positions and resolve same-column overlaps so text is never layered under a table.
 * Side-by-side placement and intentional drag gaps are preserved.
 */
export function ensureDocumentTableLayout(editor: HTMLElement): void {
  normalizeDocumentUserTables(editor);
  if (listDocumentLayoutItemsSorted(editor).length) {
    reflowAllPlacedLines(editor);
  }
}

/**
 * Place a user table into the Document absolute stack at/below the caret line.
 * Returns the inserted table (caller focuses first cell).
 */
export function insertDocumentUserTable(
  editor: HTMLElement,
  table: HTMLTableElement,
): HTMLTableElement {
  normalizeDocumentUserTables(editor);

  const { marginPt } = getDocumentContentMetrics(editor);
  const placed = findPlacedTextBlockAtCaret(editor);
  let left = marginPt;
  let insertTop = marginPt;

  if (placed) {
    const pos = getAbsolutePositionPt(placed);
    left = pos.left || marginPt;
    insertTop = pos.top + placedBlockLayoutHeightPt(placed) + 2;
  } else {
    const items = listDocumentLayoutItemsSorted(editor);
    if (items.length) {
      const last = items[items.length - 1];
      const pos = getAbsolutePositionPt(last);
      left = marginPt;
      insertTop = pos.top + layoutItemHeightPt(last) + 2;
    }
  }

  table.style.float = "";
  table.style.marginRight = "";
  table.style.marginLeft = "";
  table.style.marginTop = "";
  table.style.marginBottom = "";
  table.style.position = "absolute";
  table.style.left = formatPt(left);
  table.style.top = formatPt(insertTop);
  setLayoutHomeTopPt(table, insertTop);

  if (placed && editor.contains(placed)) {
    placed.after(table);
  } else {
    editor.appendChild(table);
  }

  void table.offsetHeight;
  const height = layoutItemHeightPt(table);
  pushPlacedLinesFrom(editor, insertTop + 0.1, height + 2, table, { columnOf: table });
  reflowAllPlacedLines(editor);
  return table;
}

/** Placed lines that intersect the current selection (visual multi-line highlight). */
export function listPlacedBlocksInSelection(editor: HTMLElement): HTMLElement[] {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) {
    const one = findPlacedTextBlockAtCaret(editor);
    return one ? [one] : [];
  }
  const range = sel.getRangeAt(0);
  const root = range.commonAncestorContainer;
  if (root !== editor && !editor.contains(root)) {
    const one = findPlacedTextBlockAtCaret(editor);
    return one ? [one] : [];
  }
  const hit = listPlacedBlocksSorted(editor).filter((block) => {
    try {
      return range.intersectsNode(block);
    } catch {
      return false;
    }
  });
  if (hit.length) {
    // Absolute lines can be out of DOM order vs painted tops — fill same-column
    // gaps so a middle line is not left behind on highlight-move.
    return fillSameColumnPlacedBlocksInVisualSpan(editor, hit);
  }
  const one = findPlacedTextBlockAtCaret(editor);
  return one ? [one] : [];
}

/**
 * True when the current selection covers essentially all content of a placed line
 * (triple-click / select-all on that line), not a partial word or mid-line run.
 */
export function selectionFullyCoversPlacedBlock(block: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || !sel.rangeCount || sel.isCollapsed) return false;
  const range = sel.getRangeAt(0);
  const blockRange = document.createRange();
  try {
    blockRange.selectNodeContents(block);
  } catch {
    return false;
  }
  // Allow tiny ZWSP / caret-pad slack at the edges of chip landings.
  const startsAtOrBefore =
    range.compareBoundaryPoints(Range.START_TO_START, blockRange) <= 0;
  const endsAtOrAfter = range.compareBoundaryPoints(Range.END_TO_END, blockRange) >= 0;
  if (startsAtOrBefore && endsAtOrAfter) return true;

  // Double-click word / partial run: selection is strictly inside the block.
  const startsAfter = range.compareBoundaryPoints(Range.START_TO_START, blockRange) > 0;
  const endsBefore = range.compareBoundaryPoints(Range.END_TO_END, blockRange) < 0;
  if (startsAfter || endsBefore) return false;

  return startsAtOrBefore && endsAtOrAfter;
}

/**
 * Placed lines to move on highlight-drag. Partial selections (double-click word,
 * mid-line drag) return [] so native text editing / word drag is not stolen.
 * Multi-line spans and whole-line selects still move those `.doc-placed-text` blocks.
 */
export function listPlacedBlocksForHighlightMove(editor: HTMLElement): HTMLElement[] {
  const blocks = listPlacedBlocksInSelection(editor);
  if (blocks.length >= 2) return blocks;
  if (blocks.length === 1 && selectionFullyCoversPlacedBlock(blocks[0])) {
    return blocks;
  }
  return [];
}

/**
 * Given selected placed lines, also include same-column siblings whose `top` lies
 * between the uppermost and lowermost hit (DOM order ≠ visual order after invent/move).
 */
export function fillSameColumnPlacedBlocksInVisualSpan(
  editor: HTMLElement,
  hit: HTMLElement[],
): HTMLElement[] {
  if (hit.length < 2) return hit;
  const hitSet = new Set(hit);
  const hitBoxes = hit.map((b) => layoutItemBoxPt(b));
  const minTop = Math.min(...hitBoxes.map((b) => b.top));
  const maxTop = Math.max(...hitBoxes.map((b) => b.top));
  const all = listPlacedBlocksSorted(editor);
  const filled: HTMLElement[] = [];
  for (const block of all) {
    if (hitSet.has(block)) {
      filled.push(block);
      continue;
    }
    const box = layoutItemBoxPt(block);
    if (box.top < minTop - 0.5 || box.top > maxTop + 0.5) continue;
    const sameColumn = hitBoxes.some((h) => layoutBoxesHorizontallyOverlap(box, h));
    if (sameColumn) filled.push(block);
  }
  return filled.length ? filled : hit;
}

/**
 * Extend a Range so every placed block in `blocks` is covered (needed when a
 * visually middle line is later in the DOM than the drag endpoints).
 */
export function expandRangeToIncludePlacedBlocks(range: Range, blocks: HTMLElement[]): Range {
  const next = range.cloneRange();
  for (const block of blocks) {
    let already = false;
    try {
      already = next.intersectsNode(block);
    } catch {
      already = false;
    }
    if (already) continue;
    const blockRange = document.createRange();
    try {
      blockRange.selectNodeContents(block);
    } catch {
      continue;
    }
    if (next.compareBoundaryPoints(Range.START_TO_START, blockRange) > 0) {
      next.setStart(blockRange.startContainer, blockRange.startOffset);
    }
    if (next.compareBoundaryPoints(Range.END_TO_END, blockRange) < 0) {
      next.setEnd(blockRange.endContainer, blockRange.endOffset);
    }
  }
  return next;
}

/**
 * True when the event target is a field/function chip. Highlight-drag must not
 * move the whole `.doc-placed-text` when the user is grabbing a placeholder.
 */
export function isDocumentInlineTokenTarget(target: EventTarget | null): boolean {
  if (!(target instanceof Element)) return false;
  return !!target.closest(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`);
}

/** True when (clientX, clientY) lies inside the current non-collapsed selection’s painted rects. */
export function clientPointInDocumentSelection(clientX: number, clientY: number): boolean {
  const sel = window.getSelection();
  if (!sel || !sel.rangeCount || sel.isCollapsed) return false;
  const range = sel.getRangeAt(0);
  const rects = range.getClientRects();
  for (let i = 0; i < rects.length; i++) {
    const r = rects[i];
    if (r.width < 1 && r.height < 1) continue;
    if (clientX >= r.left && clientX <= r.right && clientY >= r.top && clientY <= r.bottom) {
      return true;
    }
  }
  return false;
}

/** Move placed lines by a shared pt delta (highlight drag — no ✥ anchors). */
export function movePlacedBlocksByDelta(
  blocks: HTMLElement[],
  origins: Array<{ left: number; top: number }>,
  deltaLeftPt: number,
  deltaTopPt: number,
): void {
  blocks.forEach((block, i) => {
    const origin = origins[i];
    if (!origin) return;
    setAbsolutePositionPt(block, origin.left + deltaLeftPt, origin.top + deltaTopPt);
  });
}

/**
 * After highlight-drag: drop empty same-column blank husks that the moved lines now
 * occupy (so dragging “c)” up into the gap above works — same as Backspace on the blank).
 * Does **not** run full collision pack (that snapped lines back under blanks).
 */
export function finalizePlacedBlocksMove(editor: HTMLElement, moved: HTMLElement[]): void {
  if (!moved.length) return;
  const movedSet = new Set(moved.filter((b) => editor.contains(b)));
  if (!movedSet.size) return;

  const empties = listPlacedBlocksSorted(editor).filter(
    (b) => !movedSet.has(b) && isPlacedTextBlockEmpty(b),
  );
  for (const blank of empties) {
    const blankBox = layoutItemBoxPt(blank);
    let overlapsMoved = false;
    for (const block of movedSet) {
      const box = layoutItemBoxPt(block);
      if (!layoutBoxesHorizontallyOverlap(box, blankBox)) continue;
      // Vertical overlap (or blank sandwiched in the band the move covered).
      if (rangesOverlap1d(box.top, box.height, blankBox.top, blankBox.height, 4)) {
        overlapsMoved = true;
        break;
      }
    }
    if (overlapsMoved) blank.remove();
  }

  // Refresh homes to the painted tops so a later resize reflow does not restore
  // the pre-drag slot under a (now removed) blank.
  for (const block of movedSet) {
    if (!editor.contains(block)) continue;
    const { left, top } = getAbsolutePositionPt(block);
    setAbsolutePositionPt(block, left, top);
  }
}

/**
 * Extend a Document selection from an anchor range to the caret under (clientX, clientY),
 * including across separate `.doc-placed-text` blocks.
 */
export function extendDocumentSelectionToPoint(
  editor: HTMLElement,
  anchor: Range,
  clientX: number,
  clientY: number,
): boolean {
  const anchorRoot = anchor.commonAncestorContainer;
  if (anchorRoot !== editor && !editor.contains(anchorRoot)) {
    return false;
  }

  let focusRange = caretRangeAtPoint(clientX, clientY);
  if (!focusRange || (focusRange.commonAncestorContainer !== editor && !editor.contains(focusRange.commonAncestorContainer))) {
    const near = findPlacedTextBlockNearPoint(editor, clientX, clientY, { bandScale: 2 });
    if (near) {
      focusRange = focusPlacedBlockAtClientPoint(near, clientX, clientY);
    }
  }
  if (
    !focusRange ||
    (focusRange.commonAncestorContainer !== editor &&
      !editor.contains(focusRange.commonAncestorContainer))
  ) {
    return false;
  }

  // Never drag-select across a table ↔ placed-line boundary (face/size bleed + delete merge).
  const anchorIsland = documentLayoutIsland(anchor.startContainer, editor);
  const focusIsland = documentLayoutIsland(focusRange.startContainer, editor);
  if (
    anchorIsland &&
    focusIsland &&
    anchorIsland !== focusIsland &&
    selectionWouldCrossTableBoundary(anchorIsland, focusIsland)
  ) {
    focusRange = edgeRangeOfIsland(anchorIsland, clientX, clientY);
    if (!focusRange) return false;
  }

  const sel = window.getSelection();
  if (!sel) return false;

  try {
    const next = document.createRange();
    const anchorIsBefore =
      anchor.compareBoundaryPoints(Range.START_TO_START, focusRange) <= 0;
    if (anchorIsBefore) {
      next.setStart(anchor.startContainer, anchor.startOffset);
      next.setEnd(focusRange.startContainer, focusRange.startOffset);
    } else {
      next.setStart(focusRange.startContainer, focusRange.startOffset);
      next.setEnd(anchor.startContainer, anchor.startOffset);
    }

    // Seed hits from endpoints + intersects, then fill visual-column gaps and
    // expand the Range so a DOM-out-of-order middle line paints as selected too.
    const endpointBlocks: HTMLElement[] = [];
    for (const node of [next.startContainer, next.endContainer]) {
      const el = node instanceof Element ? node : node.parentElement;
      const placed = el?.closest?.(`.${PLACED_TEXT_CLASS}`);
      if (placed instanceof HTMLElement && editor.contains(placed)) {
        endpointBlocks.push(placed);
      }
    }
    const intersectHits = listPlacedBlocksSorted(editor).filter((block) => {
      try {
        return next.intersectsNode(block);
      } catch {
        return false;
      }
    });
    const seed = [...new Set([...endpointBlocks, ...intersectHits])];
    const filled = fillSameColumnPlacedBlocksInVisualSpan(editor, seed);
    const expanded = expandRangeToIncludePlacedBlocks(next, filled);

    sel.removeAllRanges();
    sel.addRange(expanded);
    return true;
  } catch {
    return false;
  }
}

/** Rendered height of a placed line for packing (tallest content on the line). */
function placedBlockLayoutHeightPt(block: HTMLElement): number {
  const linePt = placedBlockLineHeightPt(block);
  // Intentional Double-Return blanks must always reserve a full line even when the
  // browser reports a collapsed empty box (or strips `<br>` mid-edit).
  if (block.getAttribute(DOC_BLANK_ATTR) === "1" || isPlacedTextBlockEmpty(block)) {
    const measuredEmpty = pxToPt(block.getBoundingClientRect().height);
    return Math.max(linePt, measuredEmpty >= 1 ? measuredEmpty : 0);
  }
  const measured = pxToPt(block.getBoundingClientRect().height);
  // Prefer the painted box so mixed inline sizes pack correctly; fall back for empty lines.
  if (measured >= 1) return measured;
  return linePt;
}

export function readPlacedTextAlign(block: HTMLElement): DocumentAlign {
  const raw = block.dataset.docAlign || block.style.textAlign || "left";
  if (raw === "center" || raw === "right" || raw === "justify") return raw;
  return "left";
}

/** One line step for typewriter Return — matches document `line-height: 1.4`. */
export function placedBlockLineHeightPt(block: HTMLElement): number {
  const style = getComputedStyle(block);
  const fontSizePx = Number.parseFloat(style.fontSize);
  if (!Number.isFinite(fontSizePx)) return 12 * 1.4;
  const lineHeight = style.lineHeight;
  if (lineHeight === "normal") return pxToPt(fontSizePx * 1.4);
  if (lineHeight.endsWith("px")) return pxToPt(Number.parseFloat(lineHeight));
  const factor = Number.parseFloat(lineHeight);
  return pxToPt(fontSizePx * (Number.isFinite(factor) ? factor : 1.4));
}

function createPlacedTextBlockAt(left: number, top: number): HTMLParagraphElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(Math.max(0, left));
  const topPt = Math.max(0, top);
  p.style.top = formatPt(topPt);
  p.style.margin = "0";
  p.style.minWidth = "2em";
  setLayoutHomeTopPt(p, topPt);
  return p;
}

/**
 * Shift placed lines / user tables at or below `fromTopPt` down by `deltaPt`.
 * When `columnOf` is set, only moves items that share a horizontal column with that
 * item — so Return / growth next to a table does not shove the table down.
 */
export function pushPlacedLinesFrom(
  editor: HTMLElement,
  fromTopPt: number,
  deltaPt: number,
  except?: HTMLElement | HTMLElement[],
  options?: { columnOf?: HTMLElement },
): void {
  if (deltaPt <= 0) return;
  const skip = new Set(
    (Array.isArray(except) ? except : except ? [except] : []).filter(Boolean),
  );
  const columnBox = options?.columnOf ? layoutItemBoxPt(options.columnOf) : null;
  listDocumentLayoutItemsSorted(editor).forEach((node) => {
    if (skip.has(node)) return;
    const pos = getAbsolutePositionPt(node);
    if (pos.top < fromTopPt - LINE_TOLERANCE_PT) return;
    if (columnBox) {
      const box = layoutItemBoxPt(node);
      if (!layoutBoxesHorizontallyOverlap(columnBox, box)) return;
    }
    const nextTop = pos.top + deltaPt;
    node.style.top = formatPt(nextTop);
    // Return / insert growth is intentional — move home with the item so the next
    // collision resolve does not pull siblings back onto the new line.
    setLayoutHomeTopPt(node, nextTop);
  });
}

/**
 * Typewriter Return inside a `.doc-placed-text` block: split at the caret into a new
 * placed block below; existing lines at/below that spot are pushed down (not deleted).
 *
 * New blank lines inherit the active typing format (palette sticky / caret run face+size),
 * NOT the previous block's computed style — that ignored inline `<font>`/`<span>` wrappers
 * and reset the banner + next glyphs to Arial 12 pt.
 */
export function documentEnterInPlacedText(editor: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) return false;

  const block = findPlacedTextBlock(range.startContainer, editor);
  if (!block) return false;

  // Capture before extractContents moves the caret / empties the tail.
  // Sticky Face/Size wins on Return (palette choice / syncTypingFormatFromCaret).
  // Caret probe alone can mis-report host defaults (e.g. Times) and wipe Trebuchet 20.
  const sticky = getTypingFormat(editor);
  const fromCaret = typingFormatForInsert(editor);
  const typing: TypingFormat = {
    fontFace:
      sticky.fontFace !== DEFAULT_PALETTE_FONT_FACE ? sticky.fontFace : fromCaret.fontFace,
    fontSize:
      sticky.fontSize !== String(DEFAULT_PALETTE_FONT_SIZE_PT)
        ? sticky.fontSize
        : fromCaret.fontSize,
    bold: fromCaret.bold,
    italic: fromCaret.italic,
    underline: fromCaret.underline,
    color: sticky.color ?? fromCaret.color,
  };
  setTypingFormat(editor, typing);

  if (!range.collapsed) {
    range.deleteContents();
    range.collapse(true);
  }

  const { left } = getAbsolutePositionPt(block);

  const tailRange = document.createRange();
  tailRange.selectNodeContents(block);
  tailRange.setStart(range.startContainer, range.startOffset);
  const tail = tailRange.collapsed ? null : tailRange.extractContents();

  if (!meaningfulText(block.textContent)) {
    block.innerHTML = "<br>";
    markBlankPlacedBlock(block);
  } else {
    clearBlankPlacedBlockIfContent(block);
  }

  ensurePlacedBlockWrapWidth(editor, block);
  const gapPt = 2;
  const nextTop =
    getAbsolutePositionPt(block).top + placedBlockLayoutHeightPt(block) + gapPt;
  const step = placedBlockLineHeightPt(block);

  const newBlock = createPlacedTextBlockAt(left, nextTop);
  applyTypingFormatToPlacedBlock(newBlock, typing);
  // Continue paragraph chrome: align + indent survive Return.
  const align = readPlacedTextAlign(block);
  if (align !== "left") {
    newBlock.dataset.docAlign = align;
    newBlock.style.textAlign = align;
  }
  if (block.dataset.docIndent != null && block.dataset.docIndent !== "") {
    newBlock.dataset.docIndent = block.dataset.docIndent;
  }
  if (tail && meaningfulText(tail.textContent)) {
    newBlock.appendChild(tail);
    clearBlankPlacedBlockIfContent(newBlock);
  } else {
    newBlock.innerHTML = "<br>";
    markBlankPlacedBlock(newBlock);
  }
  ensurePlacedBlockWrapWidth(editor, newBlock);

  // Make room first so the new line does not land on top of existing text.
  pushPlacedLinesFrom(editor, nextTop, step, block, { columnOf: block });
  editor.appendChild(newBlock);
  reflowPlacedLinesBelow(editor, block);
  focusPlacedBlock(newBlock);
  return true;
}

/**
 * After native Form Text Return, paint face/size onto an empty new `<div>`/`<p>`
 * so the next glyphs (and blank-context palette banner) keep the prior typing format.
 * Skips non-empty blocks (mid-paragraph split that carried styled tail).
 */
export function seedBlankBlockTypingFormat(editor: HTMLElement, typing: TypingFormat): void {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return;
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) return;

  let node: Node | null = range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  let block: HTMLElement | null = null;
  while (node && node !== editor) {
    if (node instanceof HTMLElement) {
      const tag = node.tagName;
      if (tag === "P" || tag === "DIV" || tag === "LI") {
        block = node;
        break;
      }
    }
    node = node.parentNode;
  }
  if (!block || block === editor) return;
  if (meaningfulText(block.textContent)) return;
  applyTypingFormatToPlacedBlock(block, typing);
  setTypingFormat(editor, typing);
  // Tag empty post-Return paragraphs so Form Text Double-Return gaps survive sibling edits.
  markBlankPlacedBlock(block);
}

/**
 * Invent X: keep the click’s free-space left, with only a tiny inset when flush
 * against the window/chrome edge so the caret stays visible. Never forces mid-
 * canvas or L/R-of-table invents onto the left margin.
 */
export function inventPlacedLeftPt(leftPt: number): number {
  return Math.max(DOC_LINE_MARGIN_PT, leftPt);
}

/**
 * `td`/`th` of a Document `table.user` under a viewport point.
 * Prefers `elementFromPoint`; falls back to painted cell boxes when the hit is
 * the editor (or a non-cell overlay) but the point still sits in a cell.
 */
export function findUserTableCellAtPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): HTMLTableCellElement | null {
  const hit = document.elementFromPoint(clientX, clientY);
  if (hit instanceof Element && editor.contains(hit) && !hit.closest(".table-handles-overlay")) {
    // Placed prose sits above tables in hit-testing — never steal those clicks for cells.
    if (hit.closest(`.${PLACED_TEXT_CLASS}`)) return null;

    const cell = hit.closest("td, th");
    if (cell instanceof HTMLTableCellElement) {
      const table = cell.closest("table.user");
      if (table && editor.contains(table)) return cell;
    }
  }

  for (const table of listDocumentUserTables(editor)) {
    const tableRect = table.getBoundingClientRect();
    if (
      clientX < tableRect.left ||
      clientX > tableRect.right ||
      clientY < tableRect.top ||
      clientY > tableRect.bottom
    ) {
      continue;
    }
    for (const cell of listUserTableCells(table)) {
      const rect = cell.getBoundingClientRect();
      if (
        clientX >= rect.left &&
        clientX <= rect.right &&
        clientY >= rect.top &&
        clientY <= rect.bottom
      ) {
        return cell;
      }
    }
  }
  return null;
}

/** Insert an absolutely positioned paragraph at the click point. */
export function insertPlacedTextBlock(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): HTMLParagraphElement {
  const { left, top } = clientPointToEditorPt(editor, clientX, clientY);
  // Visible left inset only when inventing flush against the chrome edge.
  const p = createPlacedTextBlockAt(inventPlacedLeftPt(left), top);
  applyTypingFormatToPlacedBlock(p, getTypingFormat(editor));
  p.innerHTML = "<br>";
  editor.appendChild(p);
  ensurePlacedBlockWrapWidth(editor, p);
  clearPlacedBlockOverlap(editor, p);
  return p;
}

/**
 * If `block` sits inside a previous layout item’s box in the same column, push it down.
 * Preserves intentional free-placement gaps and side-by-side (beside table) placement.
 */
function clearPlacedBlockOverlap(editor: HTMLElement, block: HTMLElement): void {
  resolveDocumentLayoutCollisions(editor);
  reflowPlacedLinesBelow(editor, block);
}

/**
 * Drop target for a field on the Document canvas — always uses viewport coordinates,
 * not `caretRangeFromPoint` (which snaps beside floats/tables to the wrong corner).
 * Prefers an existing placed line under/near the drop (snap-to-line).
 */
export function resolveDocumentFieldDropTarget(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): HTMLElement {
  const hit = document.elementFromPoint(clientX, clientY);
  // Prefer existing placed prose under the pointer (same as click — edit, do not invent).
  if (hit instanceof HTMLElement && !hit.closest(".table-handles-overlay")) {
    const placed = hit.closest(`.${PLACED_TEXT_CLASS}`);
    if (placed instanceof HTMLElement && editor.contains(placed)) {
      return placed;
    }
  }

  const cell = findUserTableCellAtPoint(editor, clientX, clientY);
  if (cell) return cell;

  // Snap to the nearest line first (generous band) so near-miss drops join the
  // line instead of spawning an orphan block one row above/beside the text.
  const near = findPlacedTextBlockNearPoint(editor, clientX, clientY, { bandScale: 2.5 });
  if (near) return near;

  return insertPlacedTextBlock(editor, clientX, clientY);
}

/** Put the caret inside a table cell at the drop point, or in a placed line at that X. */
export function focusDocumentDropTarget(
  target: HTMLElement,
  clientX: number,
  clientY: number,
): Range | null {
  if (target instanceof HTMLTableCellElement) {
    const range = caretRangeAtPoint(clientX, clientY);
    const sel = window.getSelection();
    if (range && sel && target.contains(range.commonAncestorContainer)) {
      sel.removeAllRanges();
      sel.addRange(range);
      return range;
    }
    if (sel) {
      const fallback = document.createRange();
      fallback.selectNodeContents(target);
      fallback.collapse(true);
      sel.removeAllRanges();
      sel.addRange(fallback);
      return fallback;
    }
    return null;
  }
  if (target.classList.contains(PLACED_TEXT_CLASS)) {
    return focusPlacedBlockAtClientPoint(target, clientX, clientY);
  }
  return focusPlacedBlock(target);
}

/** Loose editor children that are not placed prose / user tables / overlays. */
function listOrphanDocumentNodes(editor: HTMLElement): Node[] {
  const orphans: Node[] = [];
  for (const child of Array.from(editor.childNodes)) {
    if (child instanceof HTMLElement) {
      if (child.classList.contains(PLACED_TEXT_CLASS)) continue;
      if (child.closest("table.user") || child.matches("table.user")) continue;
      if (child.classList.contains("table-handles-overlay")) continue;
      orphans.push(child);
      continue;
    }
    if (child.nodeType === Node.TEXT_NODE) {
      if (!meaningfulText(child.textContent) && !(child.textContent ?? "").includes("\u200b")) {
        continue;
      }
      orphans.push(child);
    }
  }
  return orphans;
}

/**
 * Drop loose editor-root glyphs left by contenteditable delete/cut.
 * Prefer this after delete — `adoptOrphanDocumentContent` would re-wrap them into
 * new `.doc-placed-text` lines and resurrect text the designer just removed.
 * Returns true when the DOM changed.
 */
export function discardOrphanDocumentContent(editor: HTMLElement): boolean {
  const orphans = listOrphanDocumentNodes(editor);
  if (!orphans.length) return false;
  for (const node of orphans) {
    node.parentNode?.removeChild(node);
  }
  return true;
}

/**
 * Remove stacked duplicate placed lines (same glyphs, nearly same top, same column).
 * Classic ghost: invent/adopt leaves a second "Form Count" under the visible label.
 * Returns true when any block was removed.
 */
export function pruneDuplicateOverlappingPlacedBlocks(editor: HTMLElement): boolean {
  const blocks = listPlacedBlocksSorted(editor);
  if (blocks.length < 2) return false;

  const sel = window.getSelection();
  const caretBlock =
    sel?.rangeCount && sel.anchorNode && editor.contains(sel.anchorNode)
      ? findPlacedTextBlock(sel.anchorNode, editor)
      : null;

  const remove = new Set<HTMLElement>();
  for (let i = 0; i < blocks.length; i++) {
    const a = blocks[i];
    if (remove.has(a)) continue;
    const textA = (a.textContent ?? "").replace(/\u00a0|\u200b/g, "").trim();
    if (!textA) continue;
    const boxA = layoutItemBoxPt(a);
    for (let j = i + 1; j < blocks.length; j++) {
      const b = blocks[j];
      if (remove.has(b)) continue;
      const textB = (b.textContent ?? "").replace(/\u00a0|\u200b/g, "").trim();
      if (textA !== textB) continue;
      const boxB = layoutItemBoxPt(b);
      if (Math.abs(boxA.top - boxB.top) > LINE_TOLERANCE_PT * 2) continue;
      if (!layoutBoxesHorizontallyOverlap(boxA, boxB)) continue;
      // Keep the caret’s line when it is one of the duplicates; else keep the earlier.
      if (caretBlock === b) remove.add(a);
      else remove.add(b);
    }
  }
  if (!remove.size) return false;
  for (const block of remove) {
    block.remove();
  }
  if (listPlacedBlocksSorted(editor).length) {
    reflowAllPlacedLines(editor);
  }
  return true;
}

/**
 * Re-home loose document content (text / inline nodes outside `.doc-placed-text`)
 * into placed paragraphs so mid-text clicks do not invent a second anchor on top.
 * Returns true when the DOM changed.
 */
export function adoptOrphanDocumentContent(editor: HTMLElement): boolean {
  const toWrap = listOrphanDocumentNodes(editor);
  if (!toWrap.length) return false;

  // Group contiguous orphans into one placed line each run.
  const groups: Node[][] = [];
  let current: Node[] = [];
  const childList = Array.from(editor.childNodes);
  for (const node of childList) {
    if (toWrap.includes(node)) {
      current.push(node);
    } else if (current.length) {
      groups.push(current);
      current = [];
    }
  }
  if (current.length) groups.push(current);

  for (const group of groups) {
    const first = group[0];
    let left = 0;
    let top = 0;
    if (first instanceof HTMLElement) {
      const abs = getAbsolutePositionPt(first);
      left = abs.left;
      top = abs.top;
    } else if (first.nodeType === Node.TEXT_NODE) {
      const probe = document.createRange();
      probe.selectNodeContents(first);
      const rect = probe.getBoundingClientRect();
      if (rect.height > 0 || rect.width > 0) {
        const pt = clientPointToEditorPt(editor, rect.left, rect.top);
        left = pt.left;
        top = pt.top;
      }
    }

    const block = createPlacedTextBlockAt(left, top);
    applyTypingFormatToPlacedBlock(block, getTypingFormat(editor));
    for (const node of group) {
      block.appendChild(node);
    }
    if (!meaningfulText(block.textContent) && !block.querySelector("br")) {
      block.innerHTML = "<br>";
    }
    editor.appendChild(block);
    ensurePlacedBlockWrapWidth(editor, block);
    clearPlacedBlockOverlap(editor, block);
  }
  return true;
}

/**
 * Document canvas click / field-drop style placement:
 * 1. Inside `.doc-placed-text` → edit caret in that line (never invent on top).
 * 2. Inside `td`/`th` of `table.user` → caret/selection in that cell (never invent).
 * 3. Else free space → invent at the point (visible left inset only when flush to chrome).
 *
 * Returns true when a new block was inserted. Unused empty invent anchors are pruned
 * when focus moves to another line / invent / cell.
 */
export function placeDocumentTextAtPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): boolean {
  // Re-home escaped glyphs first so later snap/hit tests see real placed lines.
  if (hasOrphanDocumentContent(editor)) {
    adoptOrphanDocumentContent(editor);
  }

  const finish = (keep: HTMLElement | null, invented: boolean): boolean => {
    // Never steal table caret / already-placed focus when cleaning unused invents.
    pruneEmptyPlacedTextBlocks(editor, { except: keep, restoreFocus: false });
    return invented;
  };

  const hit = document.elementFromPoint(clientX, clientY);
  if (hit instanceof Element && hit.closest(".table-handles-overlay")) {
    return finish(null, false);
  }

  // 1) Existing placed prose under the pointer → edit (not invent).
  if (hit instanceof Element && editor.contains(hit)) {
    const hitPlaced = hit.closest(`.${PLACED_TEXT_CLASS}`);
    if (hitPlaced instanceof HTMLElement && editor.contains(hitPlaced)) {
      ensurePlacedBlockWrapWidth(editor, hitPlaced);
      focusPlacedBlockAtClientPoint(hitPlaced, clientX, clientY);
      syncTypingFormatFromCaret(editor);
      reflowPlacedLinesBelow(editor, hitPlaced);
      return finish(hitPlaced, false);
    }
  }

  // 2) Table cell under the pointer → edit / highlight that cell (not invent / Y-snap).
  const cell = findUserTableCellAtPoint(editor, clientX, clientY);
  if (cell) {
    focusDocumentDropTarget(cell, clientX, clientY);
    return finish(null, false);
  }

  // Prefer an existing line on this row (click past the end of text, empty margin, etc.).
  const near = findPlacedTextBlockNearPoint(editor, clientX, clientY);
  if (near) {
    ensurePlacedBlockWrapWidth(editor, near);
    focusPlacedBlockAtClientPoint(near, clientX, clientY);
    syncTypingFormatFromCaret(editor);
    reflowPlacedLinesBelow(editor, near);
    return finish(near, false);
  }

  const range = caretRangeAtPoint(clientX, clientY);
  // Absolute tables / left-column prose steal caretRangeFromPoint on blank-canvas clicks
  // (incl. free space to the right of a table) — keep only geometrically honest ranges.
  const usableRange = usableCaretRangeForDocumentPlace(editor, clientX, clientY, range);

  // Orphan / non-placed glyphs under the point: focus — do not overlay a new empty anchor.
  if (orphanTextUnderPoint(editor, clientX, clientY)) {
    const after = findPlacedTextBlockNearPoint(editor, clientX, clientY);
    if (after) {
      ensurePlacedBlockWrapWidth(editor, after);
      focusPlacedBlockAtClientPoint(after, clientX, clientY);
      syncTypingFormatFromCaret(editor);
      reflowPlacedLinesBelow(editor, after);
      return finish(after, false);
    }
    if (usableRange && editor.contains(usableRange.commonAncestorContainer)) {
      const sel = window.getSelection();
      sel?.removeAllRanges();
      sel?.addRange(usableRange);
      syncTypingFormatFromCaret(editor);
    }
    return finish(null, false);
  }

  // Caret landed in real prose that covers this point — edit that line (not invent).
  if (rangeIntersectsExistingDocumentText(editor, usableRange)) {
    const after =
      (usableRange && findPlacedTextBlock(usableRange.startContainer, editor)) ||
      findPlacedTextBlockNearPoint(editor, clientX, clientY);
    if (after && placedBlockAcceptsClientPoint(after, clientX, clientY)) {
      ensurePlacedBlockWrapWidth(editor, after);
      focusPlacedBlockAtClientPoint(after, clientX, clientY);
      syncTypingFormatFromCaret(editor);
      reflowPlacedLinesBelow(editor, after);
      return finish(after, false);
    }
    // Fall through: rangeIntersects saw absolute-sibling noise; invent free space.
  }

  if (!shouldPlaceDocumentText(editor, clientX, clientY, usableRange)) {
    if (usableRange && editor.contains(usableRange.commonAncestorContainer)) {
      const sel = window.getSelection();
      sel?.removeAllRanges();
      sel?.addRange(usableRange);
    }
    return finish(null, false);
  }

  // 3) Free space → invent at the click (left inset only when flush against chrome).
  const block = insertPlacedTextBlock(editor, clientX, clientY);
  focusPlacedBlock(block);
  return finish(block, true);
}

/**
 * True when a placed line’s painted box (plus snap slack) covers a viewport point.
 * Shared by snap-to-line and caret-range false-positive filters so right-of-table
 * free space is not stolen by a left-column line on the same Y band.
 */
export function placedBlockAcceptsClientPoint(
  block: HTMLElement,
  clientX: number,
  clientY: number,
  options?: { bandScale?: number },
): boolean {
  const bandScale = options?.bandScale ?? 1;
  const rect = block.getBoundingClientRect();
  if (rect.width <= 0 && rect.height <= 0) return false;

  // Must be on this line’s horizontal band (small past-end slack for click-to-continue).
  const slackXPx = 10;
  if (clientX < rect.left - slackXPx || clientX > rect.right + slackXPx) return false;

  let distPx: number;
  if (clientY >= rect.top && clientY <= rect.bottom) {
    distPx = 0;
  } else if (clientY < rect.top) {
    distPx = rect.top - clientY;
  } else {
    distPx = clientY - rect.bottom;
  }

  const lineH = Math.max(rect.height, 16);
  const slackPx = (Math.max(lineH * 0.55, 12) + 6) * bandScale;
  return distPx <= slackPx;
}

/**
 * Drop caretRangeFromPoint results that do not honestly cover the click.
 * Absolute `table.user` and left-of-table `.doc-placed-text` often steal blank-canvas
 * (and especially right-of-table) invent clicks — same class of bug as table trapping.
 */
function usableCaretRangeForDocumentPlace(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  range: Range | null,
): Range | null {
  if (!range || !editor.contains(range.commonAncestorContainer)) return null;
  if (rangeInsideUserTable(editor, range)) return null;

  const placed = findPlacedTextBlock(range.startContainer, editor);
  if (placed && !placedBlockAcceptsClientPoint(placed, clientX, clientY)) {
    return null;
  }

  // Editor-root + child offset among absolute siblings is blank-canvas noise unless
  // orphan glyphs are actually painted under the point.
  if (range.startContainer === editor && !orphanTextUnderPoint(editor, clientX, clientY)) {
    return null;
  }

  return range;
}

/**
 * Find a placed line under/near a viewport point (snap-to-line / click-to-continue).
 *
 * Uses painted `getBoundingClientRect` bands — not `style.top` midpoints — so soft-wrapped
 * tall blocks and near-miss drops above/below glyphs still join the correct line.
 * Horizontal: the click must fall within (or slightly past) the line’s width so free space
 * beside a table is not stolen by a left-column line on the same Y band.
 */
export function findPlacedTextBlockNearPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  options?: { bandScale?: number },
): HTMLElement | null {
  let best: HTMLElement | null = null;
  let bestDist = Infinity;

  editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    if (!placedBlockAcceptsClientPoint(node, clientX, clientY, options)) return;

    const rect = node.getBoundingClientRect();
    let distPx: number;
    if (clientY >= rect.top && clientY <= rect.bottom) {
      distPx = 0;
    } else if (clientY < rect.top) {
      distPx = rect.top - clientY;
    } else {
      distPx = clientY - rect.bottom;
    }

    if (distPx < bestDist) {
      bestDist = distPx;
      best = node;
    }
  });
  return best;
}

export function focusPlacedBlockAtClientPoint(
  block: HTMLElement,
  clientX: number,
  clientY: number,
): Range | null {
  const sel = window.getSelection();
  if (!sel) return null;
  const range = caretRangeAtPoint(clientX, clientY);
  if (range && block.contains(range.commonAncestorContainer)) {
    sel.removeAllRanges();
    sel.addRange(range);
    return range;
  }
  // Absolute placed lines sometimes report the editor as caret container with a child
  // offset that identifies this block — still land inside the paragraph, preferably
  // near the click X via a second probe on the block’s box.
  if (range?.startContainer === block.parentNode || range?.startContainer === block.parentElement) {
    const child = range.startContainer.childNodes[range.startOffset] ??
      range.startContainer.childNodes[range.startOffset - 1];
    if (child === block || (child instanceof Node && block.contains(child))) {
      const rect = block.getBoundingClientRect();
      const clampedX = Math.min(Math.max(clientX, rect.left + 1), rect.right - 1);
      const clampedY = Math.min(Math.max(clientY, rect.top + 1), rect.bottom - 1);
      const inner = caretRangeAtPoint(clampedX, clampedY);
      if (inner && block.contains(inner.commonAncestorContainer)) {
        sel.removeAllRanges();
        sel.addRange(inner);
        return inner;
      }
    }
  }
  // Click/drop was on the line’s empty width (past the glyphs) — put caret at end.
  return focusPlacedBlockEnd(block);
}

/**
 * Prefer the end of the last meaningful text node so click-away → continue typing
 * inherits that run’s face/size (caret after a `</span>` lands on the block and
 * picks up Arial/12 from the surface).
 */
function lastMeaningfulTextIn(block: HTMLElement): Text | null {
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let last: Text | null = null;
  let node = walker.nextNode();
  while (node) {
    const text = node as Text;
    if (text.parentElement?.closest(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`)) {
      node = walker.nextNode();
      continue;
    }
    if ((text.textContent ?? "").replace(/\u00a0|\u200b/g, "").length) {
      last = text;
    }
    node = walker.nextNode();
  }
  return last;
}

/**
 * Last editable text node in a placed line, including ZWSP caret pads beside chips.
 * Used for focus restore — ZWSP-only landings are valid caret homes.
 */
function lastEditableTextIn(block: HTMLElement): Text | null {
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let last: Text | null = null;
  let node = walker.nextNode();
  while (node) {
    const text = node as Text;
    if (text.parentElement?.closest(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`)) {
      node = walker.nextNode();
      continue;
    }
    if (text.data.length > 0) last = text;
    node = walker.nextNode();
  }
  return last;
}

/** Ensure every field/function chip in `block` has ZWSP landings for a visible caret. */
function ensurePlacedBlockTokenLandings(block: HTMLElement): void {
  block.querySelectorAll(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`).forEach((node) => {
    if (node instanceof HTMLElement) ensureTokenCaretLanding(node);
  });
}

export function focusPlacedBlock(block: HTMLElement): Range | null {
  ensurePlacedBlockTokenLandings(block);
  const sel = window.getSelection();
  if (!sel) return null;
  const range = document.createRange();
  // Prefer the first editable text (ZWSP before a leading chip) over collapsing
  // onto a contenteditable=false token (invisible caret).
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let first: Text | null = null;
  let node = walker.nextNode();
  while (node) {
    const text = node as Text;
    if (!text.parentElement?.closest(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`) && text.data.length > 0) {
      first = text;
      break;
    }
    node = walker.nextNode();
  }
  if (first) {
    range.setStart(first, 0);
    range.collapse(true);
  } else {
    range.selectNodeContents(block);
    range.collapse(true);
  }
  sel.removeAllRanges();
  sel.addRange(range);
  return range;
}

/**
 * Prefer the end of the last editable text (including ZWSP pads after chips).
 * Without a landing after a trailing contenteditable=false chip, collapse(false)
 * leaves an invisible caret and arrow keys appear dead.
 */
export function focusPlacedBlockEnd(block: HTMLElement): Range | null {
  ensurePlacedBlockTokenLandings(block);
  const sel = window.getSelection();
  if (!sel) return null;
  const range = document.createRange();
  const lastText = lastEditableTextIn(block) ?? lastMeaningfulTextIn(block);
  if (lastText) {
    range.setStart(lastText, lastText.data.length);
    range.collapse(true);
  } else {
    range.selectNodeContents(block);
    range.collapse(false);
  }
  sel.removeAllRanges();
  sel.addRange(range);
  return range;
}

/**
 * After click-away / Return / mid-paragraph re-entry: refresh sticky typing attrs from
 * the caret run so Face/Size banner + next insert match (and do not revert to Arial/12).
 */
export function syncTypingFormatFromCaret(editor: HTMLElement): void {
  setTypingFormat(editor, typingFormatForInsert(editor));
}

function rangeAtEdgeOfBlock(block: HTMLElement, edge: "start" | "end"): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0 || !sel.isCollapsed) return false;
  const caret = sel.getRangeAt(0);
  if (!block.contains(caret.commonAncestorContainer)) return false;
  const probe = document.createRange();
  probe.selectNodeContents(block);
  if (edge === "start") {
    probe.setEnd(caret.startContainer, caret.startOffset);
  } else {
    probe.setStart(caret.endContainer, caret.endOffset);
  }
  return !probe.toString().replace(/\u00a0|\u200b/g, "").length;
}

/** True when the caret sits on the first/last wrapped visual line of a placed block. */
function caretOnVisualLineEdge(block: HTMLElement, edge: "first" | "last"): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0 || !sel.isCollapsed) return true;
  const caretRange = sel.getRangeAt(0);
  if (!block.contains(caretRange.commonAncestorContainer)) return true;

  const contentRange = document.createRange();
  contentRange.selectNodeContents(block);
  const lineRects = Array.from(contentRange.getClientRects()).filter((r) => r.height > 0);
  if (lineRects.length <= 1) return true;

  const tops = [...new Set(lineRects.map((r) => Math.round(r.top)))].sort((a, b) => a - b);
  const firstTop = tops[0];
  const lastTop = tops[tops.length - 1];

  let caretTop: number | null = null;
  const caretRects = caretRange.getClientRects();
  if (caretRects.length > 0) {
    caretTop = caretRects[0].top;
  } else {
    const br = caretRange.getBoundingClientRect();
    if (br.height > 0 || br.width > 0) caretTop = br.top;
  }
  if (caretTop == null) return true;

  const tol = 4;
  if (edge === "first") return caretTop <= firstTop + tol;
  return caretTop >= lastTop - tol;
}

function listPlacedBlocksSorted(editor: HTMLElement): HTMLElement[] {
  return Array.from(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`)).filter(
    (n): n is HTMLElement => n instanceof HTMLElement,
  ).sort((a, b) => getAbsolutePositionPt(a).top - getAbsolutePositionPt(b).top);
}

/** True when `el` is a top-level Document `table.user` (not a nested cell table). */
export function isDocumentUserTable(el: HTMLElement): el is HTMLTableElement {
  return el instanceof HTMLTableElement && el.classList.contains("user");
}

/**
 * Placed lines + user tables sorted by absolute `top`.
 * Tables participate so same-column remount/reflow can clear overlays; X is free
 * (beside-table prose is not forced into a single vertical stack).
 */
export function listDocumentLayoutItemsSorted(editor: HTMLElement): HTMLElement[] {
  const placed = Array.from(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`)).filter(
    (n): n is HTMLElement => n instanceof HTMLElement,
  );
  const tables = Array.from(editor.querySelectorAll("table.user")).filter(
    (n): n is HTMLTableElement =>
      n instanceof HTMLTableElement && editor.contains(n) && !n.parentElement?.closest("table.user"),
  );
  return [...placed, ...tables].sort(
    (a, b) => getAbsolutePositionPt(a).top - getAbsolutePositionPt(b).top,
  );
}

/** Rendered height of a layout item (placed line or table) for packing. */
function layoutItemHeightPt(item: HTMLElement): number {
  if (item.classList.contains(PLACED_TEXT_CLASS)) {
    return placedBlockLayoutHeightPt(item);
  }
  const measured = pxToPt(item.getBoundingClientRect().height);
  if (measured >= 1) return measured;
  if (item instanceof HTMLTableElement) {
    const rows = item.rows.length || 1;
    return Math.max(12, rows * 12);
  }
  return 12;
}

function isInlineDocToken(node: Node | null): node is HTMLElement {
  return (
    node instanceof HTMLElement &&
    (node.classList.contains(FIELD_TOKEN_CLASS) ||
      node.classList.contains(FUNCTION_TOKEN_CLASS))
  );
}

function isZwspOnlyTextNode(node: Node | null): node is Text {
  return node?.nodeType === Node.TEXT_NODE && (node as Text).data === CARET_ZWSP;
}

/** Walk siblings (skipping ZWSP / empty text) looking for a field/function chip. */
function nearestTokenSibling(from: Node, direction: "before" | "after"): HTMLElement | null {
  let sib: Node | null = direction === "after" ? from.nextSibling : from.previousSibling;
  while (sib) {
    if (isInlineDocToken(sib)) return sib;
    if (
      isZwspOnlyTextNode(sib) ||
      (sib.nodeType === Node.TEXT_NODE && !meaningfulText(sib.textContent))
    ) {
      sib = direction === "after" ? sib.nextSibling : sib.previousSibling;
      continue;
    }
    // Face/Size wrapper that only holds a ZWSP pad — keep walking.
    if (sib instanceof HTMLElement && !isInlineDocToken(sib)) {
      const inner = direction === "after" ? sib.firstChild : sib.lastChild;
      if (
        isZwspOnlyTextNode(inner) ||
        (inner?.nodeType === Node.TEXT_NODE && !meaningfulText(inner.textContent))
      ) {
        sib = direction === "after" ? sib.nextSibling : sib.previousSibling;
        continue;
      }
    }
    break;
  }
  return null;
}

/**
 * Skip ZWSP pads / empty text to find a field/function chip beside the caret.
 */
function findTokenBesideCaret(direction: "before" | "after"): HTMLElement | null {
  const sel = window.getSelection();
  if (!sel || !sel.isCollapsed || sel.rangeCount === 0) return null;
  const range = sel.getRangeAt(0);
  const node = range.startContainer;
  const offset = range.startOffset;

  if (node.nodeType === Node.TEXT_NODE) {
    const text = node as Text;
    if (direction === "after") {
      const rest = text.data.slice(offset).replace(/\u200b/g, "");
      if (rest.length > 0) return null;
    } else {
      const lead = text.data.slice(0, offset).replace(/\u200b/g, "");
      if (lead.length > 0) return null;
    }
    return nearestTokenSibling(text, direction);
  }

  if (node instanceof Element) {
    if (direction === "after") {
      const child = node.childNodes[offset] ?? null;
      if (isInlineDocToken(child)) return child;
      if (child) return nearestTokenSibling(child, "after") ?? (isInlineDocToken(child) ? child : null);
      // Past last child — no token after.
      return null;
    }
    if (offset > 0) {
      const child = node.childNodes[offset - 1] ?? null;
      if (isInlineDocToken(child)) return child;
      if (child) return nearestTokenSibling(child, "before");
      return null;
    }
    // Caret before first child of this element.
    return nearestTokenSibling(node, "before");
  }

  return null;
}

/**
 * Keep arrow / Home / End navigation inside Document placed lines (and move across
 * same-column Returns). Jump Left/Right over field/function chips.
 * Within a soft-wrapped block, Up/Down move between visual lines until the first/last line.
 * Returns true when the event was handled (caller should preventDefault).
 */
export function handlePlacedTextArrowKey(editor: HTMLElement, key: string): boolean {
  const placed = findPlacedTextBlockAtCaret(editor);
  if (!placed) return false;

  if (key === "ArrowRight") {
    if (rangeAtEdgeOfBlock(placed, "end")) {
      const next = findAdjacentSameColumnPlaced(editor, placed, "next");
      if (next) {
        focusPlacedBlock(next);
        return true;
      }
      focusPlacedBlockEnd(placed);
      return true;
    }
    const token = findTokenBesideCaret("after");
    if (token) {
      placeCaretAfterToken(token);
      return true;
    }
    return false;
  }
  if (key === "End") {
    focusPlacedBlockEnd(placed);
    return true;
  }
  if (key === "ArrowLeft") {
    if (rangeAtEdgeOfBlock(placed, "start")) {
      const prev = findAdjacentSameColumnPlaced(editor, placed, "prev");
      if (prev) {
        focusPlacedBlockEnd(prev);
        return true;
      }
      focusPlacedBlock(placed);
      return true;
    }
    const token = findTokenBesideCaret("before");
    if (token) {
      placeCaretBeforeToken(token);
      return true;
    }
    return false;
  }
  if (key === "Home") {
    focusPlacedBlock(placed);
    return true;
  }
  if (key === "ArrowUp" || key === "ArrowDown") {
    // Soft-wrapped block: let the browser move within visual lines first.
    if (key === "ArrowUp" && !caretOnVisualLineEdge(placed, "first")) return false;
    if (key === "ArrowDown" && !caretOnVisualLineEdge(placed, "last")) return false;

    const next =
      key === "ArrowUp"
        ? findAdjacentSameColumnPlaced(editor, placed, "prev")
        : findAdjacentSameColumnPlaced(editor, placed, "next");
    if (next) {
      if (key === "ArrowUp") focusPlacedBlockEnd(next);
      else focusPlacedBlock(next);
    }
    return true;
  }
  return false;
}

/**
 * Resolve the block (placed line / P / DIV / editor) used for word and paragraph select.
 */
export function blockForSelectionAtPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): HTMLElement | null {
  const caret = caretRangeAtPoint(clientX, clientY);
  if (!caret || !editor.contains(caret.commonAncestorContainer)) return null;
  const block =
    findPlacedTextBlock(caret.startContainer, editor) ??
    blockContainerForWord(caret.startContainer, editor);
  if (!(block instanceof HTMLElement) || !editor.contains(block)) return null;
  return block;
}

/**
 * Triple-click: select the whole placed Document line / Form paragraph.
 * Needed once double-click preventDefault + custom word select replaces native chaining.
 */
export function selectParagraphAtPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): boolean {
  const block = blockForSelectionAtPoint(editor, clientX, clientY);
  if (!block) return false;
  const sel = window.getSelection();
  if (!sel) return false;
  const range = document.createRange();
  range.selectNodeContents(block);
  sel.removeAllRanges();
  sel.addRange(range);
  return true;
}

/**
 * Double-click: select the whole field/function token, or expand to word bounds
 * across mixed B/I/U/face spans (native selection often chops at element edges).
 */
export function selectWordOrTokenAtPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): boolean {
  const hit = document.elementFromPoint(clientX, clientY);
  if (hit instanceof Element && editor.contains(hit)) {
    const token = hit.closest(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`);
    if (token instanceof HTMLElement && editor.contains(token)) {
      const sel = window.getSelection();
      if (!sel) return false;
      const range = document.createRange();
      range.selectNode(token);
      sel.removeAllRanges();
      sel.addRange(range);
      return true;
    }
  }

  const hitPlaced =
    hit instanceof Element && editor.contains(hit)
      ? hit.closest(`.${PLACED_TEXT_CLASS}`)
      : null;

  const caret = caretRangeAtPoint(clientX, clientY);
  if (!caret || !editor.contains(caret.commonAncestorContainer)) {
    // Empty invent under the pointer when caretRangeFromPoint misses (absolute
    // `<br>`-only lines). Land a caret so dblclick free space stays typeable.
    if (
      hitPlaced instanceof HTMLElement &&
      editor.contains(hitPlaced) &&
      isPlacedTextBlockEmpty(hitPlaced)
    ) {
      focusPlacedBlock(hitPlaced);
      return true;
    }
    return false;
  }

  const block =
    findPlacedTextBlock(caret.startContainer, editor) ??
    blockContainerForWord(caret.startContainer, editor);
  if (!(block instanceof HTMLElement) || !editor.contains(block)) return false;

  const flat = flattenTextNodes(block);
  if (!flat.length) {
    // Empty invent / Double-Return blank (often only `<br>`): native dblclick +
    // mouseup skipping place-at-point left no caret. Land one so typing works.
    if (block.classList.contains(PLACED_TEXT_CLASS)) {
      focusPlacedBlock(block);
      return true;
    }
    return false;
  }

  let caretIndex = 0;
  let found = false;
  for (const part of flat) {
    if (part.node === caret.startContainer) {
      caretIndex += Math.min(caret.startOffset, part.node.length);
      found = true;
      break;
    }
    caretIndex += part.node.length;
  }
  if (!found) {
    // Caret may be on an element — map via client point to nearest text.
    const near = caretRangeAtPoint(clientX, clientY);
    if (near?.startContainer.nodeType === Node.TEXT_NODE) {
      caretIndex = 0;
      for (const part of flat) {
        if (part.node === near.startContainer) {
          caretIndex += Math.min(near.startOffset, part.node.length);
          found = true;
          break;
        }
        caretIndex += part.node.length;
      }
    }
  }
  if (!found) return false;

  const joined = flat.map((p) => p.node.textContent ?? "").join("");
  const { start, end } = wordBoundsInText(joined, caretIndex);
  if (start >= end) return false;

  const startPos = locateFlatOffset(flat, start);
  const endPos = locateFlatOffset(flat, end);
  if (!startPos || !endPos) return false;

  const sel = window.getSelection();
  if (!sel) return false;
  const range = document.createRange();
  range.setStart(startPos.node, startPos.offset);
  range.setEnd(endPos.node, endPos.offset);
  sel.removeAllRanges();
  sel.addRange(range);
  return true;
}

function blockContainerForWord(node: Node, editor: HTMLElement): HTMLElement {
  let current: Node | null = node.nodeType === Node.TEXT_NODE ? node.parentNode : node;
  while (current && current !== editor) {
    if (current instanceof HTMLElement) {
      const tag = current.tagName;
      if (
        tag === "P" ||
        tag === "DIV" ||
        tag === "TD" ||
        tag === "TH" ||
        tag === "LI" ||
        current.classList.contains(PLACED_TEXT_CLASS)
      ) {
        return current;
      }
    }
    current = current.parentNode;
  }
  return editor;
}

function flattenTextNodes(root: HTMLElement): Array<{ node: Text; start: number }> {
  const out: Array<{ node: Text; start: number }> = [];
  let offset = 0;
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
  let n: Text | null;
  while ((n = walker.nextNode() as Text | null)) {
    if (n.parentElement?.closest(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`)) {
      continue;
    }
    out.push({ node: n, start: offset });
    offset += n.length;
  }
  return out;
}

function locateFlatOffset(
  flat: Array<{ node: Text; start: number }>,
  index: number,
): { node: Text; offset: number } | null {
  for (let i = 0; i < flat.length; i++) {
    const part = flat[i];
    const len = part.node.length;
    const nextStart = part.start + len;
    if (index < nextStart || i === flat.length - 1) {
      return { node: part.node, offset: Math.max(0, Math.min(len, index - part.start)) };
    }
  }
  return null;
}

/**
 * A Document editing island: one `.doc-placed-text` block or one `td`/`th` of `table.user`.
 * Absolute placed lines and table cells must not merge across this boundary on delete.
 */
function documentLayoutIsland(node: Node | null, editor: HTMLElement): HTMLElement | null {
  let current: Node | null = node;
  if (current?.nodeType === Node.TEXT_NODE) current = current.parentNode;
  while (current && current !== editor) {
    if (current instanceof HTMLTableCellElement) {
      const table = current.closest("table.user");
      if (table && editor.contains(table)) return current;
    }
    if (current instanceof HTMLElement && current.classList.contains(PLACED_TEXT_CLASS)) {
      return current;
    }
    current = current.parentNode;
  }
  return null;
}

function islandIsTableCell(island: HTMLElement): boolean {
  return island instanceof HTMLTableCellElement;
}

/** True when extending a selection would bridge placed prose and a table cell. */
function selectionWouldCrossTableBoundary(a: HTMLElement, b: HTMLElement): boolean {
  return islandIsTableCell(a) !== islandIsTableCell(b);
}

function edgeRangeOfIsland(
  island: HTMLElement,
  clientX: number,
  clientY: number,
): Range | null {
  const rect = island.getBoundingClientRect();
  const midX = (rect.left + rect.right) / 2;
  const towardStart = clientX < midX;
  const selRange = document.createRange();
  selRange.selectNodeContents(island);
  selRange.collapse(towardStart);
  // Prefer a caret near the pointer when the island still owns that point.
  const atPoint = caretRangeAtPoint(
    Math.min(Math.max(clientX, rect.left + 1), Math.max(rect.left + 1, rect.right - 1)),
    Math.min(Math.max(clientY, rect.top + 1), Math.max(rect.top + 1, rect.bottom - 1)),
  );
  if (atPoint && island.contains(atPoint.commonAncestorContainer)) {
    return atPoint;
  }
  return selRange;
}

/**
 * Clamp a live selection that already spans placed text and a table cell back into
 * the island that held the selection anchor (stops face/size apply bleed).
 * Returns true when the selection was rewritten.
 */
export function clampDocumentSelectionToLayoutIsland(editor: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0 || sel.isCollapsed) return false;
  const range = sel.getRangeAt(0);
  if (
    range.commonAncestorContainer !== editor &&
    !editor.contains(range.commonAncestorContainer)
  ) {
    return false;
  }
  const startIsland = documentLayoutIsland(range.startContainer, editor);
  const endIsland = documentLayoutIsland(range.endContainer, editor);
  if (!startIsland || !endIsland || startIsland === endIsland) return false;
  if (!selectionWouldCrossTableBoundary(startIsland, endIsland)) return false;

  const anchorIsland = documentLayoutIsland(sel.anchorNode, editor) ?? startIsland;
  const next = document.createRange();
  try {
    next.selectNodeContents(anchorIsland);
    // Keep a partial highlight when the anchor side of the range still sits in the island.
    if (
      sel.anchorNode &&
      anchorIsland.contains(sel.anchorNode) &&
      anchorIsland.contains(range.startContainer) &&
      sel.anchorNode === range.startContainer &&
      sel.anchorOffset === range.startOffset
    ) {
      next.setStart(range.startContainer, range.startOffset);
    } else if (
      sel.anchorNode &&
      anchorIsland.contains(sel.anchorNode) &&
      anchorIsland.contains(range.endContainer) &&
      sel.anchorNode === range.endContainer &&
      sel.anchorOffset === range.endOffset
    ) {
      next.setEnd(range.endContainer, range.endOffset);
    }
    sel.removeAllRanges();
    sel.addRange(next);
    return true;
  } catch {
    return false;
  }
}

function isDeleteBackwardInput(inputType: string): boolean {
  return (
    inputType === "deleteContentBackward" ||
    inputType === "deleteWordBackward" ||
    inputType === "deleteSoftLineBackward" ||
    inputType === "deleteHardLineBackward"
  );
}

function isDeleteForwardInput(inputType: string): boolean {
  return (
    inputType === "deleteContentForward" ||
    inputType === "deleteWordForward" ||
    inputType === "deleteSoftLineForward" ||
    inputType === "deleteHardLineForward"
  );
}

/**
 * Confine a non-collapsed selection delete to one layout island (placed line or cell).
 */
function deleteRangeWithinIsland(island: HTMLElement, fullRange: Range): void {
  const clipped = fullRange.cloneRange();
  const bounds = document.createRange();
  bounds.selectNodeContents(island);
  if (!island.contains(clipped.startContainer)) {
    clipped.setStart(bounds.startContainer, bounds.startOffset);
  }
  if (!island.contains(clipped.endContainer)) {
    clipped.setEnd(bounds.endContainer, bounds.endOffset);
  }
  if (
    clipped.compareBoundaryPoints(Range.START_TO_END, clipped) <= 0 &&
    clipped.startContainer === clipped.endContainer &&
    clipped.startOffset === clipped.endOffset
  ) {
    // Empty after clamp.
  } else {
    clipped.deleteContents();
  }
  if (island.classList.contains(PLACED_TEXT_CLASS) && isPlacedTextBlockEmpty(island)) {
    island.innerHTML = "<br>";
  }
  const sel = window.getSelection();
  if (!sel) return;
  const caret = document.createRange();
  try {
    caret.setStart(clipped.startContainer, clipped.startOffset);
  } catch {
    caret.selectNodeContents(island);
    caret.collapse(true);
  }
  caret.collapse(true);
  if (!island.contains(caret.startContainer)) {
    caret.selectNodeContents(island);
    caret.collapse(true);
  }
  sel.removeAllRanges();
  sel.addRange(caret);
}

/**
 * Same-column neighbor used for cross-Return Backspace / Delete merge.
 * Side-by-side columns (no horizontal overlap) stay independent; tables are never neighbors.
 */
function findAdjacentSameColumnPlaced(
  editor: HTMLElement,
  placed: HTMLElement,
  direction: "prev" | "next",
): HTMLElement | null {
  const blocks = listPlacedBlocksSorted(editor);
  const idx = blocks.indexOf(placed);
  if (idx < 0) return null;
  const box = layoutItemBoxPt(placed);
  if (direction === "prev") {
    for (let i = idx - 1; i >= 0; i--) {
      if (layoutBoxesHorizontallyOverlap(box, layoutItemBoxPt(blocks[i]))) {
        return blocks[i];
      }
    }
    return null;
  }
  for (let i = idx + 1; i < blocks.length; i++) {
    if (layoutBoxesHorizontallyOverlap(box, layoutItemBoxPt(blocks[i]))) {
      return blocks[i];
    }
  }
  return null;
}

/** Drop empty `<br>` / ZWSP scaffolds so a merge join sits after real glyphs. */
function stripTrailingPlacedScaffold(block: HTMLElement): void {
  while (block.lastChild) {
    const last = block.lastChild;
    if (last.nodeName === "BR") {
      block.removeChild(last);
      continue;
    }
    if (
      last.nodeType === Node.TEXT_NODE &&
      !meaningfulText(last.textContent)
    ) {
      block.removeChild(last);
      continue;
    }
    break;
  }
}

/**
 * Merge `from` into `into` (same-column Return undo). Caret lands at the join.
 * Never used across table cells — callers only pass `.doc-placed-text` blocks.
 */
function mergePlacedTextBlocks(into: HTMLElement, from: HTMLElement): void {
  stripTrailingPlacedScaffold(into);
  // Avoid "fashionednames" when joining two word runs across a Return.
  const intoText = (into.textContent ?? "").replace(/\u00a0|\u200b/g, "");
  const fromText = (from.textContent ?? "").replace(/\u00a0|\u200b/g, "");
  const intoEndsWord = /[A-Za-z0-9)]$/.test(intoText);
  const fromStartsWord = /^[A-Za-z0-9(]/.test(fromText);
  if (intoEndsWord && fromStartsWord) {
    into.appendChild(document.createTextNode(" "));
  }

  const join = document.createRange();
  join.selectNodeContents(into);
  join.collapse(false);
  const joinContainer = join.startContainer;
  const joinOffset = join.startOffset;

  while (from.firstChild) {
    const child = from.firstChild;
    // Empty destination: skip a lone `<br>` husk from the source blank line.
    if (
      !into.hasChildNodes() &&
      child.nodeName === "BR" &&
      from.childNodes.length === 1
    ) {
      from.removeChild(child);
      continue;
    }
    into.appendChild(child);
  }
  clearBlankPlacedBlockIfContent(into);
  from.remove();

  const sel = window.getSelection();
  if (!sel) return;
  const caret = document.createRange();
  try {
    const max =
      joinContainer.nodeType === Node.TEXT_NODE
        ? (joinContainer.textContent?.length ?? 0)
        : joinContainer.childNodes.length;
    caret.setStart(joinContainer, Math.min(joinOffset, max));
    caret.collapse(true);
    sel.removeAllRanges();
    sel.addRange(caret);
  } catch {
    focusPlacedBlockEnd(into);
  }
}

/**
 * Backspace / Delete at placed-line edges:
 * - Chip immediately before/after caret → remove chip and keep a live caret landing.
 * - Same-column previous/next `.doc-placed-text` → merge (Word-like across Returns).
 * - No same-column neighbor → swallow the key (never let Chromium hop into a table cell).
 *
 * Classic failure this still prevents: caret at the start of a short invent to the
 * right of a table + Backspace → word hops into the bottom-right cell.
 *
 * Returns true when the caller must `preventDefault` (DOM may already be updated).
 */
export function handleDocumentDeleteBoundary(editor: HTMLElement, inputType: string): boolean {
  if (!inputType.startsWith("delete")) return false;
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  const range = sel.getRangeAt(0);
  if (
    range.commonAncestorContainer !== editor &&
    !editor.contains(range.commonAncestorContainer)
  ) {
    return false;
  }

  // Selection that already bridges placed prose ↔ table: delete only inside the focus island.
  if (!range.collapsed) {
    const startIsland = documentLayoutIsland(range.startContainer, editor);
    const endIsland = documentLayoutIsland(range.endContainer, editor);
    if (
      startIsland &&
      endIsland &&
      startIsland !== endIsland &&
      selectionWouldCrossTableBoundary(startIsland, endIsland)
    ) {
      const focusIsland = documentLayoutIsland(sel.focusNode, editor) ?? startIsland;
      deleteRangeWithinIsland(focusIsland, range);
      return true;
    }
    // Selected field/function chip(s): remove and restore a live caret (native often
    // leaves selection empty → invisible caret + dead arrows).
    const island = documentLayoutIsland(range.startContainer, editor);
    if (island?.classList.contains(PLACED_TEXT_CLASS)) {
      const tokens = [
        ...island.querySelectorAll(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`),
      ].filter((t): t is HTMLElement => t instanceof HTMLElement && tokenTouchesSelection(t, range));
      if (tokens.length) {
        const host = tokens[0].parentElement ?? island;
        for (const token of tokens) token.remove();
        ensurePlacedBlockTokenLandings(island);
        if (isPlacedTextBlockEmpty(island)) {
          const prev = findAdjacentSameColumnPlaced(editor, island, "prev");
          island.remove();
          if (prev) focusPlacedBlockEnd(prev);
          else sel.removeAllRanges();
        } else {
          focusCaretIn(host, island);
        }
        if (listPlacedBlocksSorted(editor).length) reflowAllPlacedLines(editor);
        return true;
      }
    }
    return false;
  }

  const placed = findPlacedTextBlock(range.startContainer, editor);
  if (placed) {
    // Chip immediately beside caret — delete chip, keep caret in a ZWSP landing.
    if (isDeleteBackwardInput(inputType)) {
      const token = findTokenBesideCaret("before");
      if (token && placed.contains(token)) {
        deleteInlineTokenKeepCaret(token, placed, "before");
        return true;
      }
    }
    if (isDeleteForwardInput(inputType)) {
      const token = findTokenBesideCaret("after");
      if (token && placed.contains(token)) {
        deleteInlineTokenKeepCaret(token, placed, "after");
        return true;
      }
    }

    if (isDeleteBackwardInput(inputType) && rangeAtEdgeOfBlock(placed, "start")) {
      const prev = findAdjacentSameColumnPlaced(editor, placed, "prev");
      if (prev) {
        // Blank line above + content here: delete only the blank (one Backspace =
        // one Return). Do not merge content up into the husk.
        if (isPlacedTextBlockEmpty(prev) && !isPlacedTextBlockEmpty(placed)) {
          prev.remove();
          focusPlacedBlock(placed);
        } else if (isPlacedTextBlockEmpty(placed)) {
          placed.remove();
          focusPlacedBlockEnd(prev);
        } else {
          mergePlacedTextBlocks(prev, placed);
          ensurePlacedBlockTokenLandings(prev);
        }
        if (listPlacedBlocksSorted(editor).length) {
          reflowAllPlacedLines(editor);
        }
        return true;
      }
      if (isPlacedTextBlockEmpty(placed)) {
        // No same-column neighbor — drop husk; do not join a table / other column.
        placed.remove();
        sel.removeAllRanges();
        if (listPlacedBlocksSorted(editor).length) {
          reflowAllPlacedLines(editor);
        }
      }
      return true;
    }
    if (isDeleteForwardInput(inputType) && rangeAtEdgeOfBlock(placed, "end")) {
      const next = findAdjacentSameColumnPlaced(editor, placed, "next");
      if (next) {
        if (isPlacedTextBlockEmpty(next)) {
          // Drop only the blank below (one Delete = one Return).
          next.remove();
          focusPlacedBlockEnd(placed);
        } else {
          mergePlacedTextBlocks(placed, next);
          ensurePlacedBlockTokenLandings(placed);
        }
        if (listPlacedBlocksSorted(editor).length) {
          reflowAllPlacedLines(editor);
        }
      }
      // Always preventDefault at the edge so Chromium cannot pull a table cell in.
      return true;
    }
    return false;
  }

  // Inside a table: do not pull neighboring placed prose in at the table’s outer edges.
  const cell = documentLayoutIsland(range.startContainer, editor);
  if (cell instanceof HTMLTableCellElement) {
    const table = cell.closest("table.user");
    if (!table || !editor.contains(table)) return false;
    const cells = listUserTableCells(table);
    const idx = cells.indexOf(cell);
    if (idx < 0) return false;
    if (isDeleteBackwardInput(inputType) && idx === 0 && rangeAtEdgeOfBlock(cell, "start")) {
      return true;
    }
    if (
      isDeleteForwardInput(inputType) &&
      idx === cells.length - 1 &&
      rangeAtEdgeOfBlock(cell, "end")
    ) {
      return true;
    }
  }

  return false;
}

function tokenTouchesSelection(token: HTMLElement, range: Range): boolean {
  try {
    if (range.intersectsNode(token)) return true;
  } catch {
    /* ignore */
  }
  try {
    const tr = document.createRange();
    tr.selectNode(token);
    return (
      range.compareBoundaryPoints(Range.END_TO_START, tr) > 0 &&
      range.compareBoundaryPoints(Range.START_TO_END, tr) < 0
    );
  } catch {
    return false;
  }
}

/** Remove one chip and leave a collapsed caret in an editable landing. */
function deleteInlineTokenKeepCaret(
  token: HTMLElement,
  placed: HTMLElement,
  from: "before" | "after",
): void {
  const parent = token.parentNode;
  const anchor =
    from === "before" ? token.previousSibling : token.nextSibling;
  token.remove();
  ensurePlacedBlockTokenLandings(placed);
  if (isPlacedTextBlockEmpty(placed)) {
    // Leave an editable husk so the designer can keep typing / arrowing on this line
    // until a later Backspace at the start merges/removes it intentionally.
    placed.innerHTML = "<br>";
    markBlankPlacedBlock(placed);
    focusPlacedBlock(placed);
    return;
  }
  const sel = window.getSelection();
  if (!sel) return;
  const caret = document.createRange();
  if (anchor && placed.contains(anchor)) {
    if (anchor.nodeType === Node.TEXT_NODE) {
      const text = anchor as Text;
      const offset = from === "before" ? text.data.length : 0;
      caret.setStart(text, Math.min(offset, text.data.length));
    } else if (from === "before") {
      caret.setStartAfter(anchor);
    } else {
      caret.setStartBefore(anchor);
    }
    caret.collapse(true);
    sel.removeAllRanges();
    sel.addRange(caret);
    return;
  }
  if (parent instanceof HTMLElement && placed.contains(parent)) {
    focusCaretIn(parent, placed);
  } else {
    focusPlacedBlockEnd(placed);
  }
}

function focusCaretIn(host: Node, fallbackBlock: HTMLElement): void {
  const sel = window.getSelection();
  if (!sel) return;
  if (host.nodeType === Node.TEXT_NODE) {
    const text = host as Text;
    const caret = document.createRange();
    caret.setStart(text, text.data.length);
    caret.collapse(true);
    sel.removeAllRanges();
    sel.addRange(caret);
    return;
  }
  if (host instanceof HTMLElement) {
    ensurePlacedBlockTokenLandings(fallbackBlock);
    const text = lastEditableTextIn(host) ?? lastEditableTextIn(fallbackBlock);
    if (text) {
      const caret = document.createRange();
      caret.setStart(text, text.data.length);
      caret.collapse(true);
      sel.removeAllRanges();
      sel.addRange(caret);
      return;
    }
  }
  focusPlacedBlockEnd(fallbackBlock);
}

