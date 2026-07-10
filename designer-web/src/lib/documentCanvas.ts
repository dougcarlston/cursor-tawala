/** Click-to-place text on the Document canvas (free-positioned blocks). */

import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
} from "./paletteDefaults";
import {
  getTypingFormat,
  type TypingFormat,
} from "./paletteTypingFormat";
import { formatPt, getAbsolutePositionPt, pxToPt } from "./tableLayout";

const PLACED_TEXT_CLASS = "doc-placed-text";
const LINE_TOLERANCE_PT = 2;
/** Extra inset inside the editor content box (~10px). Editor padding already provides chrome inset. */
const DOC_LINE_MARGIN_PT = pxToPt(10);

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

/** Viewport click → pt offset inside the editor content box (for absolute placement). */
export function clientPointToEditorPt(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): { left: number; top: number } {
  const rect = editor.getBoundingClientRect();
  const style = getComputedStyle(editor);
  const padL = Number.parseFloat(style.paddingLeft) || 0;
  const padT = Number.parseFloat(style.paddingTop) || 0;
  return {
    left: pxToPt(clientX - rect.left - padL),
    top: pxToPt(clientY - rect.top - padT),
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

/** True when the click should create/focus a free-positioned text block. */
export function shouldPlaceDocumentText(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  range: Range | null,
): boolean {
  const hit = document.elementFromPoint(clientX, clientY);
  if (hit instanceof HTMLElement && hit.closest(".table-handles-overlay")) return false;

  if (hit === editor) return true;

  if (isPlacedTextBlock(hit)) return false;

  if (hit instanceof HTMLElement && editor.contains(hit)) {
    if (hit.closest("table.user")) return false;
    if (isEditableTextTarget(hit, editor) && range) {
      const container = range.commonAncestorContainer;
      if (container !== editor && editor.contains(container)) return false;
    }
  }

  if (!range || !editor.contains(range.commonAncestorContainer)) return true;

  if (range.commonAncestorContainer === editor) return true;

  return false;
}

function meaningfulText(text: string | null | undefined): boolean {
  return (text ?? "").replace(/\u00a0|\u200b/g, "").trim().length > 0;
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
  if (typing.fontSize === String(DEFAULT_PALETTE_FONT_SIZE_PT)) {
    block.style.removeProperty("font-size");
  } else {
    block.style.fontSize = `${typing.fontSize}pt`;
  }
  if (typing.fontFace === DEFAULT_PALETTE_FONT_FACE) {
    block.style.removeProperty("font-family");
  } else {
    block.style.fontFamily = typing.fontFace;
  }
  if (typing.color) {
    block.style.color = typing.color;
  } else {
    block.style.removeProperty("color");
  }
}

/** Apply face/size/color to a field or function token (overrides badge CSS). */
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
    el.style.color = typing.color;
  } else {
    el.style.removeProperty("color");
  }
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

/**
 * Single-line Document alignment relative to left/right margins (not shrink-wrapped text-align alone).
 * Multi-line / full reflow deferred — see DESIGNER_OPEN_TODOS.md.
 */
export function alignPlacedTextBlock(
  editor: HTMLElement,
  block: HTMLElement,
  align: DocumentAlign,
): void {
  const { top } = getAbsolutePositionPt(block);
  const { marginPt, contentWidthPt } = getDocumentContentMetrics(editor);
  const lineWidth = Math.max(1, contentWidthPt - marginPt * 2);

  block.style.position = "absolute";
  block.style.top = formatPt(top);
  block.style.left = formatPt(marginPt);
  block.style.width = formatPt(lineWidth);
  block.style.right = "auto";
  block.style.textAlign = align;
  block.dataset.docAlign = align;

  if (align === "justify") {
    // Fill the margin width; wrap within the block, then push lines below down.
    // Do not set text-align-last: justify — that stretches the short final line unnaturally.
    block.style.whiteSpace = "normal";
    block.style.overflowWrap = "break-word";
    block.style.removeProperty("text-align-last");
  } else {
    block.style.whiteSpace = "nowrap";
    block.style.removeProperty("overflow-wrap");
    block.style.removeProperty("text-align-last");
  }

  // Absolute lines don't flow in normal layout — clear overlap after height changes.
  reflowPlacedLinesBelow(editor, block);
}

/**
 * Pack following placed lines under `block`: push down to clear overlap, or pull up
 * to close the gap when `block`'s box shrinks.
 *
 * Height is the rendered line box (tallest glyph/token on the line), not the last
 * palette action:
 * - Enlarging a single word/character can grow the box and push lines below down.
 * - Lines below move back up only when the box actually shrinks — i.e. every
 *   enlarged run on that line has been shrunk (or never enlarged). Shrinking one
 *   word while another stays large must not pull up.
 */
export function reflowPlacedLinesBelow(editor: HTMLElement, block: HTMLElement): void {
  const blocks = listPlacedBlocksSorted(editor);
  const idx = blocks.indexOf(block);
  if (idx < 0) return;

  const gapPt = 2;
  const pos = getAbsolutePositionPt(block);
  let nextTop = pos.top + placedBlockLayoutHeightPt(block) + gapPt;

  for (let i = idx + 1; i < blocks.length; i++) {
    const below = blocks[i];
    const belowPos = getAbsolutePositionPt(below);
    if (Math.abs(belowPos.top - nextTop) > 0.5) {
      below.style.top = formatPt(nextTop);
    }
    nextTop = nextTop + placedBlockLayoutHeightPt(below) + gapPt;
  }
}

/** Rendered height of a placed line for packing (tallest content on the line). */
function placedBlockLayoutHeightPt(block: HTMLElement): number {
  const measured = pxToPt(block.getBoundingClientRect().height);
  // Prefer the painted box so mixed inline sizes pack correctly; fall back for empty lines.
  if (measured >= 1) return measured;
  return placedBlockLineHeightPt(block);
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

function copyTypographicStyles(from: HTMLElement, to: HTMLElement): void {
  const style = getComputedStyle(from);
  to.style.fontFamily = style.fontFamily;
  to.style.fontSize = style.fontSize;
  to.style.fontWeight = style.fontWeight;
  to.style.fontStyle = style.fontStyle;
  to.style.color = style.color;
  to.style.textDecoration = style.textDecoration;
}

function createPlacedTextBlockAt(left: number, top: number): HTMLParagraphElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(Math.max(0, left));
  p.style.top = formatPt(Math.max(0, top));
  p.style.margin = "0";
  p.style.minWidth = "2em";
  return p;
}

/**
 * Shift every placed line at or below `fromTopPt` down by `deltaPt`.
 * Used so Return / growth pushes existing text instead of stacking over it.
 */
export function pushPlacedLinesFrom(
  editor: HTMLElement,
  fromTopPt: number,
  deltaPt: number,
  except?: HTMLElement | HTMLElement[],
): void {
  if (deltaPt <= 0) return;
  const skip = new Set(
    (Array.isArray(except) ? except : except ? [except] : []).filter(Boolean),
  );
  listPlacedBlocksSorted(editor).forEach((node) => {
    if (skip.has(node)) return;
    const pos = getAbsolutePositionPt(node);
    if (pos.top >= fromTopPt - LINE_TOLERANCE_PT) {
      node.style.top = formatPt(pos.top + deltaPt);
    }
  });
}

/**
 * Typewriter Return inside a `.doc-placed-text` block: new line at the same `left`,
 * one line height lower; existing lines at/below that spot are pushed down (not deleted).
 */
export function documentEnterInPlacedText(editor: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) return false;

  const block = findPlacedTextBlock(range.startContainer, editor);
  if (!block) return false;

  if (!range.collapsed) {
    range.deleteContents();
    range.collapse(true);
  }

  const { left, top } = getAbsolutePositionPt(block);
  const lineHeight = placedBlockLineHeightPt(block);
  const nextTop = top + lineHeight;

  const tailRange = document.createRange();
  tailRange.selectNodeContents(block);
  tailRange.setStart(range.startContainer, range.startOffset);
  const tail = tailRange.collapsed ? null : tailRange.extractContents();

  if (!meaningfulText(block.textContent)) {
    block.innerHTML = "<br>";
  }

  const newBlock = createPlacedTextBlockAt(left, nextTop);
  copyTypographicStyles(block, newBlock);
  if (tail && meaningfulText(tail.textContent)) {
    newBlock.appendChild(tail);
  } else {
    newBlock.innerHTML = "<br>";
  }

  // Make room first so the new line does not land on top of existing text.
  pushPlacedLinesFrom(editor, nextTop, lineHeight, block);
  editor.appendChild(newBlock);
  reflowPlacedLinesBelow(editor, newBlock);
  focusPlacedBlock(newBlock);
  return true;
}

/** Insert an absolutely positioned paragraph at the click point. */
export function insertPlacedTextBlock(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): HTMLParagraphElement {
  const { left, top } = clientPointToEditorPt(editor, clientX, clientY);
  const p = createPlacedTextBlockAt(left, top);
  applyTypingFormatToPlacedBlock(p, getTypingFormat(editor));
  p.innerHTML = "<br>";
  editor.appendChild(p);
  return p;
}

export function focusPlacedBlock(block: HTMLElement): Range | null {
  const sel = window.getSelection();
  if (!sel) return null;
  const range = document.createRange();
  range.selectNodeContents(block);
  range.collapse(true);
  sel.removeAllRanges();
  sel.addRange(range);
  return range;
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
  if (hit instanceof HTMLElement) {
    if (hit.closest(".table-handles-overlay")) {
      // Fall through to snap / new line below.
    } else {
      const cell = hit.closest("td, th");
      if (cell instanceof HTMLTableCellElement && editor.contains(cell)) {
        return cell;
      }
      const placed = hit.closest(`.${PLACED_TEXT_CLASS}`);
      if (placed instanceof HTMLElement && editor.contains(placed)) {
        return placed;
      }
    }
  }

  // Snap into the nearest line on this row even if the drop missed the glyphs.
  const near = findPlacedTextBlockNearPoint(editor, clientX, clientY, { bandScale: 1.75 });
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

/**
 * Document canvas click — place caret in existing text or start a new positioned block.
 * Returns true when a new block was inserted.
 */
export function placeDocumentTextAtPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): boolean {
  // Prefer an existing line on this row (click past the end of text, empty margin, etc.).
  const near = findPlacedTextBlockNearPoint(editor, clientX, clientY);
  if (near) {
    focusPlacedBlockAtClientPoint(near, clientX, clientY);
    return false;
  }

  const range = caretRangeAtPoint(clientX, clientY);
  if (!shouldPlaceDocumentText(editor, clientX, clientY, range)) {
    if (range && editor.contains(range.commonAncestorContainer)) {
      const sel = window.getSelection();
      sel?.removeAllRanges();
      sel?.addRange(range);
    }
    return false;
  }

  const block = insertPlacedTextBlock(editor, clientX, clientY);
  focusPlacedBlock(block);
  return true;
}

/** Find a placed line whose vertical band contains the point (for snap / click-to-continue). */
export function findPlacedTextBlockNearPoint(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  options?: { bandScale?: number },
): HTMLElement | null {
  const { top } = clientPointToEditorPt(editor, clientX, clientY);
  const bandScale = options?.bandScale ?? 1;
  let best: HTMLElement | null = null;
  let bestDist = Infinity;

  editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    const pos = getAbsolutePositionPt(node);
    const lineH = placedBlockLineHeightPt(node);
    const mid = pos.top + lineH / 2;
    const dist = Math.abs(top - mid);
    const band = (Math.max(lineH, LINE_TOLERANCE_PT * 4) / 2 + LINE_TOLERANCE_PT) * bandScale;
    if (dist <= band && dist < bestDist) {
      bestDist = dist;
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
  // Click/drop was on the line’s empty width (past the glyphs) — put caret at end.
  return focusPlacedBlockEnd(block);
}

export function focusPlacedBlockEnd(block: HTMLElement): Range | null {
  const sel = window.getSelection();
  if (!sel) return null;
  const range = document.createRange();
  range.selectNodeContents(block);
  range.collapse(false);
  sel.removeAllRanges();
  sel.addRange(range);
  return range;
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

function listPlacedBlocksSorted(editor: HTMLElement): HTMLElement[] {
  return Array.from(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`)).filter(
    (n): n is HTMLElement => n instanceof HTMLElement,
  ).sort((a, b) => getAbsolutePositionPt(a).top - getAbsolutePositionPt(b).top);
}

/**
 * Keep arrow / Home / End navigation inside Document placed lines (and move Up/Down between lines).
 * Returns true when the event was handled (caller should preventDefault).
 */
export function handlePlacedTextArrowKey(editor: HTMLElement, key: string): boolean {
  const placed = findPlacedTextBlockAtCaret(editor);
  if (!placed) return false;

  if (key === "ArrowRight") {
    if (rangeAtEdgeOfBlock(placed, "end")) {
      focusPlacedBlockEnd(placed);
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
      focusPlacedBlock(placed);
      return true;
    }
    return false;
  }
  if (key === "Home") {
    focusPlacedBlock(placed);
    return true;
  }
  if (key === "ArrowUp" || key === "ArrowDown") {
    const blocks = listPlacedBlocksSorted(editor);
    const idx = blocks.indexOf(placed);
    if (idx < 0) return true;
    const next =
      key === "ArrowUp"
        ? blocks[Math.max(0, idx - 1)]
        : blocks[Math.min(blocks.length - 1, idx + 1)];
    if (next && next !== placed) {
      focusPlacedBlockEnd(next);
    }
    return true;
  }
  return false;
}