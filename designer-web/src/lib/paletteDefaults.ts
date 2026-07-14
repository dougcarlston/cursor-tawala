/** Legacy document / rich-text defaults — Arial 12 pt (`jsonToXml` fontXml, owner feedback July 2026). */
export const DEFAULT_PALETTE_FONT_FACE = "Arial";
export const DEFAULT_PALETTE_FONT_SIZE_PT = 12;

/**
 * Select value when the highlight mixes faces/sizes. Must not equal a real face/size —
 * otherwise choosing "default" is a no-op (controlled <select> skips onChange).
 */
export const MIXED_PALETTE_VALUE = "";

/** Web-safe faces offered by the Formatting Palette (Document + Form Text). */
export const WEB_SAFE_FONT_FACES = [
  "Arial",
  "Arial Black",
  "Comic Sans MS",
  "Courier New",
  "Georgia",
  "Impact",
  "Tahoma",
  "Times New Roman",
  "Trebuchet MS",
  "Verdana",
] as const;

export function defaultFontFaceLabel(face: string): string {
  return face === DEFAULT_PALETTE_FONT_FACE ? `${face} (default)` : face;
}

export function defaultFontSizeLabel(sizePt: string): string {
  return sizePt === String(DEFAULT_PALETTE_FONT_SIZE_PT)
    ? `${sizePt} pt (default)`
    : `${sizePt} pt`;
}

/** Map a CSS `font-family` value to a palette face (or default Arial). */
export function matchFontFace(raw: string): string {
  const first = raw.replace(/['"]/g, "").split(",")[0]?.trim().toLowerCase() ?? "";
  if (!first) return DEFAULT_PALETTE_FONT_FACE;
  if (first.includes("segoe")) return DEFAULT_PALETTE_FONT_FACE;
  // Exact match first.
  for (const face of WEB_SAFE_FONT_FACES) {
    if (face.toLowerCase() === first) return face;
  }
  // Abbreviated names ("Comic" → Comic Sans MS). Prefer longer faces first.
  // Only prefix/exact-stem matches — never "sans" → Comic Sans MS.
  const byLength = [...WEB_SAFE_FONT_FACES].sort((a, b) => b.length - a.length);
  for (const face of byLength) {
    const fl = face.toLowerCase();
    if (fl === first) return face;
    if (first.length >= 4 && (fl.startsWith(first) || first.startsWith(fl))) return face;
  }
  return DEFAULT_PALETTE_FONT_FACE;
}
