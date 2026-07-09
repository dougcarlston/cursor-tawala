/** Click-to-place text on the Document canvas (free-positioned blocks). */

import { formatPt, pxToPt } from "./tableLayout";

const PLACED_TEXT_CLASS = "doc-placed-text";

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

/** Insert an absolutely positioned paragraph at the click point. */
export function insertPlacedTextBlock(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
): HTMLParagraphElement {
  const { left, top } = clientPointToEditorPt(editor, clientX, clientY);
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(Math.max(0, left));
  p.style.top = formatPt(Math.max(0, top));
  p.style.margin = "0";
  p.style.minWidth = "2em";
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