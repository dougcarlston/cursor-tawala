/** Legacy document / rich-text defaults — Arial 12 pt (`jsonToXml` fontXml, owner feedback July 2026). */
export const DEFAULT_PALETTE_FONT_FACE = "Arial";
export const DEFAULT_PALETTE_FONT_SIZE_PT = 12;

/**
 * Select value when the highlight mixes faces/sizes. Must not equal a real face/size —
 * otherwise choosing "default" is a no-op (controlled <select> skips onChange).
 */
export const MIXED_PALETTE_VALUE = "";

export function defaultFontFaceLabel(face: string): string {
  return face === DEFAULT_PALETTE_FONT_FACE ? `${face} (default)` : face;
}

export function defaultFontSizeLabel(sizePt: string): string {
  return sizePt === String(DEFAULT_PALETTE_FONT_SIZE_PT)
    ? `${sizePt} pt (default)`
    : `${sizePt} pt`;
}
