/**
 * Sample the visible font color under a point inside a rich-text surface (A-bar long-press).
 */

import { normalizeFontColorHex } from "@/lib/recentFontColors";

/**
 * Walk from the element under (x,y) up through `root` and return an explicit color
 * (`style.color` / `<font color>`) when present; otherwise computed color.
 */
export function sampleFontColorAtPoint(
  root: HTMLElement,
  clientX: number,
  clientY: number,
): string | null {
  const hit = document.elementFromPoint(clientX, clientY);
  if (!(hit instanceof Element) || !root.contains(hit)) return null;

  let el: Element | null = hit;
  while (el && el !== root && root.contains(el)) {
    if (el instanceof HTMLElement) {
      if (el.style.color) {
        const hex = normalizeFontColorHex(el.style.color);
        if (hex) return hex;
      }
      if (el.tagName === "FONT") {
        const attr = el.getAttribute("color");
        if (attr) {
          const hex = normalizeFontColorHex(attr);
          if (hex) return hex;
        }
      }
    }
    el = el.parentElement;
  }

  const node = hit instanceof HTMLElement ? hit : hit.parentElement;
  if (!(node instanceof HTMLElement) || !root.contains(node)) return null;
  return normalizeFontColorHex(getComputedStyle(node).color);
}
