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
const COLUMN_TOLERANCE_PT = 1.5;
const LINE_TOLERANCE_PT = 2;

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

/** Remove any existing line in this column at `top` (typewriter overwrite). */
function removePlacedBlocksAtLine(
  editor: HTMLElement,
  left: number,
  top: number,
  except?: HTMLElement,
): void {
  editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).forEach((node) => {
    if (!(node instanceof HTMLElement) || node === except) return;
    const pos = getAbsolutePositionPt(node);
    if (Math.abs(pos.left - left) > COLUMN_TOLERANCE_PT) return;
    if (Math.abs(pos.top - top) <= LINE_TOLERANCE_PT) node.remove();
  });
}

/**
 * Legacy typewriter Return inside a `.doc-placed-text` block: new line at the same `left`,
 * one line height lower; overwrites any existing line already at that position.
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

  removePlacedBlocksAtLine(editor, left, nextTop, block);

  const newBlock = createPlacedTextBlockAt(left, nextTop);
  copyTypographicStyles(block, newBlock);
  if (tail && meaningfulText(tail.textContent)) {
    newBlock.appendChild(tail);
  } else {
    newBlock.innerHTML = "<br>";
  }

  editor.appendChild(newBlock);
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
 */
export function resolveDocumentFieldDropTarget(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): HTMLElement {
  const hit = document.elementFromPoint(clientX, clientY);
  if (hit instanceof HTMLElement) {
    if (hit.closest(".table-handles-overlay")) {
      return insertPlacedTextBlock(editor, clientX, clientY);
    }
    const cell = hit.closest("td, th");
    if (cell instanceof HTMLTableCellElement && editor.contains(cell)) {
      return cell;
    }
    const placed = hit.closest(`.${PLACED_TEXT_CLASS}`);
    if (placed instanceof HTMLElement && editor.contains(placed)) {
      return placed;
    }
  }
  return insertPlacedTextBlock(editor, clientX, clientY);
}

/** Put the caret inside a table cell at the drop point, or at the start of a placed block. */
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