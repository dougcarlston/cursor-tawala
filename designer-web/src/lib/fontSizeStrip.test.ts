/**
 * Guards Document commit strip: 10pt/11pt must not be treated as default 12pt.
 * Legacy bug: pt→px→legacy-bucket mapped both into size "3" and stripped them.
 */
import { describe, expect, it } from "vitest";
import { isRedundantDefaultFontSize } from "@/lib/fontSizeStrip";

describe("isRedundantDefaultFontSize", () => {
  it("treats legacy HTML size 3 and exact 12pt as default", () => {
    expect(isRedundantDefaultFontSize("3")).toBe(true);
    expect(isRedundantDefaultFontSize("12pt")).toBe(true);
    expect(isRedundantDefaultFontSize("12.0pt")).toBe(true);
    expect(isRedundantDefaultFontSize("16px")).toBe(true); // 12pt at 96dpi
    expect(isRedundantDefaultFontSize("medium")).toBe(true);
  });

  it("keeps 10pt and 11pt (must not collapse into default via 1–7 buckets)", () => {
    expect(isRedundantDefaultFontSize("10pt")).toBe(false);
    expect(isRedundantDefaultFontSize("11pt")).toBe(false);
    // px equivalents at 96dpi: 10pt≈13.333px, 11pt≈14.667px
    expect(isRedundantDefaultFontSize(`${10 * (4 / 3)}px`)).toBe(false);
    expect(isRedundantDefaultFontSize(`${11 * (4 / 3)}px`)).toBe(false);
  });

  it("keeps other non-default palette stops and legacy sizes", () => {
    expect(isRedundantDefaultFontSize("8pt")).toBe(false);
    expect(isRedundantDefaultFontSize("9pt")).toBe(false);
    expect(isRedundantDefaultFontSize("14pt")).toBe(false);
    expect(isRedundantDefaultFontSize("20pt")).toBe(false);
    expect(isRedundantDefaultFontSize("1")).toBe(false);
    expect(isRedundantDefaultFontSize("2")).toBe(false);
    expect(isRedundantDefaultFontSize("7")).toBe(false);
  });
});
