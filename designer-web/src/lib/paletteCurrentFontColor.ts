/**
 * Sticky "A and bar" Font Color for the Formatting Palette.
 * Updated by Choose Color / recent swatches / long-press sample — not by normal caret moves.
 */

import { normalizeFontColorHex } from "@/lib/recentFontColors";

const DEFAULT_COLOR = "#000000";

type Listener = (color: string) => void;
const listeners = new Set<Listener>();

let current = DEFAULT_COLOR;

function emit(): void {
  listeners.forEach((cb) => cb(current));
}

/** Current palette Font Color (A-bar underline). */
export function getPaletteCurrentFontColor(): string {
  return current;
}

export function subscribePaletteCurrentFontColor(listener: Listener): () => void {
  listeners.add(listener);
  listener(current);
  return () => {
    listeners.delete(listener);
  };
}

/**
 * Set the A-bar current color without applying to the selection.
 * Returns normalized hex, or null if invalid.
 */
export function setPaletteCurrentFontColor(raw: string): string | null {
  const hex = normalizeFontColorHex(raw);
  if (!hex) return null;
  if (hex === current) return hex;
  current = hex;
  emit();
  return hex;
}

/** Test helper. */
export function resetPaletteCurrentFontColorForTests(hex = DEFAULT_COLOR): void {
  current = normalizeFontColorHex(hex) ?? DEFAULT_COLOR;
  emit();
}
