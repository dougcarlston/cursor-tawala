/**
 * Which font-size values count as the palette default (12pt / legacy HTML size 3)
 * and may be stripped from saved HTML.
 *
 * IMPORTANT: Do not route 10pt/11pt through the legacy 1–7 pixel buckets.
 * `10pt` → ~13.33px and `11pt` → ~14.67px both land in the same bucket as
 * `12pt` (~16px) when using coarse `<font size>` thresholds, which made
 * Document commit strip them back to inherited 12pt.
 */

import { DEFAULT_PALETTE_FONT_SIZE_PT } from "./paletteDefaults";
import { parseCssPt, pxToPt } from "./tableLayout";

const LEGACY_DEFAULT_SIZE = "3";

/** True when this font-size value is redundant with the surface default (12pt). */
export function isRedundantDefaultFontSize(value: unknown): boolean {
  const raw = String(value ?? "").trim();
  if (!raw) return true;

  if (/^[1-7]$/.test(raw)) {
    return raw === LEGACY_DEFAULT_SIZE;
  }

  const lower = raw.toLowerCase();
  if (lower === "medium") return true;

  const ptMatch = raw.match(/^([\d.]+)\s*pt$/i);
  if (ptMatch) {
    const pt = Number(ptMatch[1]);
    return Number.isFinite(pt) && Math.abs(pt - DEFAULT_PALETTE_FONT_SIZE_PT) < 0.05;
  }

  const pxMatch = raw.match(/^([\d.]+)\s*px$/i);
  if (pxMatch) {
    const px = Number(pxMatch[1]);
    if (!Number.isFinite(px)) return false;
    // Only ~16px (12pt at 96dpi). 10pt≈13.33px and 11pt≈14.67px must be kept.
    const pt = pxToPt(px);
    return Math.abs(pt - DEFAULT_PALETTE_FONT_SIZE_PT) < 0.35;
  }

  const bare = Number.parseFloat(raw);
  if (Number.isFinite(bare) && /^-?[\d.]+$/.test(raw)) {
    // Unitless number — treat as pt if in palette range.
    return Math.abs(bare - DEFAULT_PALETTE_FONT_SIZE_PT) < 0.05;
  }

  // Keywords / unknown units from execCommand must not be casually stripped.
  const fromCss = parseCssPt(raw);
  if (fromCss > 0) {
    return Math.abs(fromCss - DEFAULT_PALETTE_FONT_SIZE_PT) < 0.05;
  }
  return false;
}
