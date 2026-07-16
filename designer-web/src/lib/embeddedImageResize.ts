/**
 * Design-canvas sizing for Insert → Image → From your PC embeds.
 * Deploy still reads data-image-width / data-image-height → `<image width height/>`.
 */

import {
  EMBEDDED_IMAGE_CLASS,
  EMBEDDED_IMAGE_HEIGHT_ATTR,
  EMBEDDED_IMAGE_ID_ATTR,
  EMBEDDED_IMAGE_WIDTH_ATTR,
} from "./projectImages";

export const EMBEDDED_IMAGE_SELECTED_CLASS = "tawala-embedded-image-selected";
export const EMBEDDED_IMAGE_HANDLES_CLASS = "embedded-image-handles-overlay";
export const MIN_EMBEDDED_IMAGE_PX = 24;
/** Soft cap when inserting so huge photos don't blow the Text / Document canvas. */
export const DEFAULT_MAX_INSERT_WIDTH_PX = 480;

export function isEmbeddedImageElement(node: Node | null): node is HTMLImageElement {
  return (
    node instanceof HTMLImageElement &&
    node.classList.contains(EMBEDDED_IMAGE_CLASS) &&
    !!node.getAttribute(EMBEDDED_IMAGE_ID_ATTR)
  );
}

export function clearEmbeddedImageSelection(editor: HTMLElement): void {
  editor
    .querySelectorAll(`img.${EMBEDDED_IMAGE_CLASS}.${EMBEDDED_IMAGE_SELECTED_CLASS}`)
    .forEach((el) => el.classList.remove(EMBEDDED_IMAGE_SELECTED_CLASS));
}

export function selectEmbeddedImage(editor: HTMLElement, img: HTMLImageElement): void {
  clearEmbeddedImageSelection(editor);
  if (!editor.contains(img)) return;
  img.classList.add(EMBEDDED_IMAGE_SELECTED_CLASS);
}

/** Scale natural size to fit maxWidth (aspect preserved). */
export function fitEmbeddedImageSize(
  naturalW: number,
  naturalH: number,
  maxWidth: number,
): { width: number; height: number } {
  const w0 = Math.max(1, Math.round(naturalW));
  const h0 = Math.max(1, Math.round(naturalH));
  const maxW = Math.max(MIN_EMBEDDED_IMAGE_PX, Math.round(maxWidth));
  if (w0 <= maxW) return { width: w0, height: h0 };
  const scale = maxW / w0;
  return {
    width: Math.max(1, Math.round(w0 * scale)),
    height: Math.max(1, Math.round(h0 * scale)),
  };
}

export function readEmbeddedImageSize(img: HTMLImageElement): {
  width: number;
  height: number;
} {
  const wAttr = Number(img.getAttribute(EMBEDDED_IMAGE_WIDTH_ATTR) || img.width || 0);
  const hAttr = Number(img.getAttribute(EMBEDDED_IMAGE_HEIGHT_ATTR) || img.height || 0);
  const width = wAttr > 0 ? wAttr : Math.max(1, Math.round(img.getBoundingClientRect().width));
  const height =
    hAttr > 0 ? hAttr : Math.max(1, Math.round(img.getBoundingClientRect().height));
  return { width, height };
}

export function applyEmbeddedImageDisplaySize(
  img: HTMLImageElement,
  width: number,
  height: number,
): void {
  const w = Math.max(MIN_EMBEDDED_IMAGE_PX, Math.round(width));
  const h = Math.max(MIN_EMBEDDED_IMAGE_PX, Math.round(height));
  img.width = w;
  img.height = h;
  img.setAttribute(EMBEDDED_IMAGE_WIDTH_ATTR, String(w));
  img.setAttribute(EMBEDDED_IMAGE_HEIGHT_ATTR, String(h));
  img.style.width = `${w}px`;
  img.style.height = `${h}px`;
}

/**
 * SE-corner drag: lock aspect to start size; drive by the larger delta so
 * dragging feels natural toward the corner.
 */
export function sizeFromSeDrag(
  startWidth: number,
  startHeight: number,
  dxPx: number,
  dyPx: number,
  maxWidth?: number,
): { width: number; height: number } {
  const aspect = startWidth > 0 && startHeight > 0 ? startWidth / startHeight : 1;
  // Prefer width-driven when |dx| dominates; else height-driven.
  let width: number;
  let height: number;
  if (Math.abs(dxPx) >= Math.abs(dyPx)) {
    width = startWidth + dxPx;
    height = width / aspect;
  } else {
    height = startHeight + dyPx;
    width = height * aspect;
  }
  width = Math.max(MIN_EMBEDDED_IMAGE_PX, width);
  height = Math.max(MIN_EMBEDDED_IMAGE_PX, height);
  if (maxWidth != null && maxWidth > MIN_EMBEDDED_IMAGE_PX && width > maxWidth) {
    width = maxWidth;
    height = width / aspect;
  }
  return { width: Math.round(width), height: Math.round(height) };
}

export function findActiveEmbeddedImage(editor: HTMLElement): HTMLImageElement | null {
  const marked = editor.querySelector(
    `img.${EMBEDDED_IMAGE_CLASS}.${EMBEDDED_IMAGE_SELECTED_CLASS}`,
  );
  if (marked instanceof HTMLImageElement && editor.contains(marked)) return marked;

  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  const range = sel.getRangeAt(0);

  const fromNode = (node: Node | null): HTMLImageElement | null => {
    if (!node) return null;
    if (isEmbeddedImageElement(node)) return node;
    if (node instanceof Element) {
      const img = node.closest(`img.${EMBEDDED_IMAGE_CLASS}`);
      if (img instanceof HTMLImageElement && isEmbeddedImageElement(img)) return img;
    }
    return null;
  };

  const direct = fromNode(range.commonAncestorContainer);
  if (direct && editor.contains(direct)) return direct;

  // Collapsed caret / selection that covers an <img> child.
  if (range.startContainer instanceof Element) {
    const child = range.startContainer.childNodes[range.startOffset];
    if (isEmbeddedImageElement(child) && editor.contains(child)) return child;
  }
  return null;
}

export function imageBoxInContainer(
  img: HTMLImageElement,
  container: HTMLElement,
): { top: number; left: number; width: number; height: number } {
  const ir = img.getBoundingClientRect();
  const cr = container.getBoundingClientRect();
  return {
    top: ir.top - cr.top + container.scrollTop,
    left: ir.left - cr.left + container.scrollLeft,
    width: ir.width,
    height: ir.height,
  };
}

/** Click target is an embedded image (not a handle). */
export function embeddedImageFromEventTarget(target: EventTarget | null): HTMLImageElement | null {
  if (!(target instanceof Element)) return null;
  if (target.closest(`.${EMBEDDED_IMAGE_HANDLES_CLASS}`)) return null;
  const img = target.closest(`img.${EMBEDDED_IMAGE_CLASS}`);
  return img instanceof HTMLImageElement && isEmbeddedImageElement(img) ? img : null;
}
