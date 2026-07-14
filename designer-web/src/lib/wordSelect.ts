/**
 * Word selection across mixed inline formatting spans.
 * Native double-click often stops at style-element boundaries mid-word.
 */

/**
 * MouseEvent.detail ≥ 2 is a multi-click selection gesture (word / paragraph).
 * Callers must not collapse the selection to a caret on those mouseups
 * (e.g. Document place-at-point) or triple-click paragraph select dies.
 */
export function isMultiClickSelectionGesture(detail: number): boolean {
  return detail >= 2;
}

/** True when `ch` is part of a double-click word (letters, digits, underscore). */
export function isWordChar(ch: string): boolean {
  if (!ch) return false;
  return /[A-Za-z0-9_]/.test(ch) || /\p{L}/u.test(ch) || /\p{N}/u.test(ch);
}

/**
 * Expand character offsets within a flat string to word bounds around `index`.
 * Pure helper for unit tests and for DOM expansion.
 */
export function wordBoundsInText(text: string, index: number): { start: number; end: number } {
  const i = Math.max(0, Math.min(index, text.length));
  if (!text.length) return { start: 0, end: 0 };

  let start = i;
  let end = i;
  // If caret is between chars, prefer the character to the left for word pick.
  if (start > 0 && (start === text.length || !isWordChar(text[start]!))) {
    start -= 1;
    end = start + 1;
  }
  while (start > 0 && isWordChar(text[start - 1]!)) start -= 1;
  while (end < text.length && isWordChar(text[end]!)) end += 1;
  if (start === end && end < text.length) end += 1;
  return { start, end };
}
