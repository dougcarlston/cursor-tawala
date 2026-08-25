/**
 * Form Text drag-select across embedded images.
 * Native contenteditable selection often stops at replaced `<img>` nodes
 * (owner Jul 30 / Aug 11: highlight won't include the image paragraph or text beyond).
 */

import { EMBEDDED_IMAGE_CLASS } from "./projectImages";
import { isEmbeddedImageElement } from "./embeddedImageResize";

/** Caret Range at a viewport point (Chromium + Firefox). */
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

function rangeAfterNode(node: Node): Range {
  const r = document.createRange();
  r.setStartAfter(node);
  r.collapse(true);
  return r;
}

function rangeBeforeNode(node: Node): Range {
  const r = document.createRange();
  r.setStartBefore(node);
  r.collapse(true);
  return r;
}

/**
 * When the pointer is over an embedded image (caretRangeFromPoint often fails),
 * pick a collapsed caret before/after the img from drag direction.
 */
export function focusRangeNearEmbeddedImage(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  anchor: Range,
): Range | null {
  const hit = document.elementFromPoint(clientX, clientY);
  if (!(hit instanceof Element) || !editor.contains(hit)) return null;
  const img = hit.closest(`img.${EMBEDDED_IMAGE_CLASS}`);
  if (!(img instanceof HTMLImageElement) || !isEmbeddedImageElement(img)) return null;

  const ir = img.getBoundingClientRect();
  const midY = (ir.top + ir.bottom) / 2;
  const midX = (ir.left + ir.right) / 2;
  // Prefer vertical drag direction (Form Text is mostly top→bottom).
  const after =
    clientY > midY + 2 || (Math.abs(clientY - midY) <= 2 && clientX >= midX);

  try {
    const probe = after ? rangeAfterNode(img) : rangeBeforeNode(img);
    // If anchor is already past the image, flipping before/after keeps extend stable.
    const anchorAfterImg = anchor.compareBoundaryPoints(Range.START_TO_START, probe) > 0;
    if (after && anchorAfterImg) return rangeBeforeNode(img);
    if (!after && !anchorAfterImg) return rangeAfterNode(img);
    return probe;
  } catch {
    return null;
  }
}

/**
 * Extend Form Text selection from `anchor` to the caret under (clientX, clientY),
 * including across embedded images that block native drag-select.
 */
export function extendFormTextSelectionToPoint(
  editor: HTMLElement,
  anchor: Range,
  clientX: number,
  clientY: number,
): boolean {
  const anchorRoot = anchor.commonAncestorContainer;
  if (anchorRoot !== editor && !editor.contains(anchorRoot)) return false;

  let focus =
    caretRangeAtPoint(clientX, clientY) ??
    focusRangeNearEmbeddedImage(editor, clientX, clientY, anchor);

  if (
    !focus ||
    (focus.commonAncestorContainer !== editor &&
      !editor.contains(focus.commonAncestorContainer))
  ) {
    focus = focusRangeNearEmbeddedImage(editor, clientX, clientY, anchor);
  }
  if (
    !focus ||
    (focus.commonAncestorContainer !== editor &&
      !editor.contains(focus.commonAncestorContainer))
  ) {
    return false;
  }

  const sel = window.getSelection();
  if (!sel) return false;

  try {
    const next = document.createRange();
    const anchorIsBefore =
      anchor.compareBoundaryPoints(Range.START_TO_START, focus) <= 0;
    if (anchorIsBefore) {
      next.setStart(anchor.startContainer, anchor.startOffset);
      next.setEnd(focus.startContainer, focus.startOffset);
    } else {
      next.setStart(focus.startContainer, focus.startOffset);
      next.setEnd(anchor.startContainer, anchor.startOffset);
    }
    sel.removeAllRanges();
    sel.addRange(next);
    return true;
  } catch {
    return false;
  }
}
