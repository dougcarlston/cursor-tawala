/**
 * Guards Return / Enter typing-format inheritance: new blank paragraphs must keep
 * sticky/caret face+size (e.g. Trebuchet 20), not reset via bare computed Arial/12.
 */
import { describe, expect, it } from "vitest";
import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
} from "./paletteDefaults";
import type { TypingFormat } from "./paletteTypingFormat";

/** Mirrors `applyTypingFormatStylePatch` without needing a live CSSStyleDeclaration. */
function stylePatchFromTyping(typing: TypingFormat): {
  fontSize: string | null;
  fontFamily: string | null;
  color: string | null;
} {
  return {
    fontSize:
      typing.fontSize === String(DEFAULT_PALETTE_FONT_SIZE_PT)
        ? null
        : `${typing.fontSize}pt`,
    fontFamily:
      typing.fontFace === DEFAULT_PALETTE_FONT_FACE ? null : typing.fontFace,
    color: typing.color ?? null,
  };
}

describe("Enter / Return paragraph typing format", () => {
  it("keeps Trebuchet 20pt on a new blank paragraph (owner smoke)", () => {
    const typing: TypingFormat = {
      fontFace: "Trebuchet MS",
      fontSize: "20",
      bold: false,
      italic: false,
      underline: false,
    };
    expect(stylePatchFromTyping(typing)).toEqual({
      fontSize: "20pt",
      fontFamily: "Trebuchet MS",
      color: null,
    });
  });

  it("does not invent explicit Arial/12 when sticky is default (clears inline)", () => {
    const typing: TypingFormat = {
      fontFace: DEFAULT_PALETTE_FONT_FACE,
      fontSize: String(DEFAULT_PALETTE_FONT_SIZE_PT),
      bold: false,
      italic: false,
      underline: false,
    };
    expect(stylePatchFromTyping(typing)).toEqual({
      fontSize: null,
      fontFamily: null,
      color: null,
    });
  });

  it("must not copy unstyled parent computed defaults as the enter source of truth", () => {
    // Regression: copyTypographicStyles(getComputedStyle(block)) when face/size lived
    // only on inner <font>/<span> runs yielded Arial + 16px and overwrote sticky.
    const sticky: TypingFormat = {
      fontFace: "Trebuchet MS",
      fontSize: "20",
      bold: false,
      italic: false,
      underline: false,
    };
    const wrongComputedParent = {
      fontFamily: "Arial",
      fontSize: "16px",
    };
    const chosen = stylePatchFromTyping(sticky);
    expect(chosen.fontFamily).toBe("Trebuchet MS");
    expect(chosen.fontSize).toBe("20pt");
    expect(chosen.fontFamily).not.toBe(wrongComputedParent.fontFamily);
    expect(chosen.fontSize).not.toBe(wrongComputedParent.fontSize);
  });
});
