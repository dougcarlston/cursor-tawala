/**
 * Guards Face/Size palette readout: Mixed only when values truly disagree;
 * bare Arial/12 must not win over a uniform Comic/20 highlight just because
 * field chips lack explicit styles.
 */
import { describe, expect, it } from "vitest";
import {
  coalesceTypographicSamples,
  snapFontSizePtForTest,
} from "@/lib/paletteCommands";
import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
  MIXED_PALETTE_VALUE,
} from "@/lib/paletteDefaults";
import { applyTypingFormatToToken } from "@/lib/documentCanvas";
import type { TypingFormat } from "@/lib/paletteTypingFormat";

describe("coalesceTypographicSamples", () => {
  it("returns fallback when there are no samples", () => {
    expect(coalesceTypographicSamples([], DEFAULT_PALETTE_FONT_FACE)).toBe(
      DEFAULT_PALETTE_FONT_FACE,
    );
    expect(coalesceTypographicSamples([], String(DEFAULT_PALETTE_FONT_SIZE_PT))).toBe(
      String(DEFAULT_PALETTE_FONT_SIZE_PT),
    );
  });

  it("returns the uniform value when all samples agree (Comic + inherited chips)", () => {
    // After chip-inherit fix: text Comic + chip-without-style → both report Comic.
    expect(
      coalesceTypographicSamples(
        ["Comic Sans MS", "Comic Sans MS", "Comic Sans MS"],
        DEFAULT_PALETTE_FONT_FACE,
      ),
    ).toBe("Comic Sans MS");
    expect(coalesceTypographicSamples(["20", "20", "20"], String(DEFAULT_PALETTE_FONT_SIZE_PT))).toBe(
      "20",
    );
  });

  it("returns Mixed only when faces or sizes actually disagree", () => {
    expect(
      coalesceTypographicSamples(
        ["Comic Sans MS", DEFAULT_PALETTE_FONT_FACE],
        DEFAULT_PALETTE_FONT_FACE,
      ),
    ).toBe(MIXED_PALETTE_VALUE);
    expect(
      coalesceTypographicSamples(["20", String(DEFAULT_PALETTE_FONT_SIZE_PT)], "12"),
    ).toBe(MIXED_PALETTE_VALUE);
  });

  it("must not report bare default 12 when the only samples are a larger uniform size", () => {
    const size = coalesceTypographicSamples(["20", "20"], String(DEFAULT_PALETTE_FONT_SIZE_PT));
    expect(size).toBe("20");
    expect(size).not.toBe(String(DEFAULT_PALETTE_FONT_SIZE_PT));
  });
});

describe("snapFontSizePtForTest", () => {
  it("snaps near-20pt measurements to the 20 palette stop", () => {
    expect(snapFontSizePtForTest(20)).toBe("20");
    expect(snapFontSizePtForTest(19.5)).toBe("20");
  });

  it("keeps 10 and 11 as distinct palette stops (must not collapse to default 12)", () => {
    expect(snapFontSizePtForTest(10)).toBe("10");
    expect(snapFontSizePtForTest(11)).toBe("11");
    expect(snapFontSizePtForTest(10.2)).toBe("10");
    expect(snapFontSizePtForTest(11.2)).toBe("11");
    // Measurement noise around the default still lands on 12.
    expect(snapFontSizePtForTest(12)).toBe("12");
    expect(snapFontSizePtForTest(11.6)).toBe("12");
    expect(snapFontSizePtForTest(12.4)).toBe("12");
  });
});

describe("applyTypingFormatToToken (Face/Size/B-I-U on chips)", () => {
  it("paints chip face/size and clears them back to inherit on default", () => {
    const el = {
      style: {
        fontSize: "",
        fontFamily: "",
        fontWeight: "",
        fontStyle: "",
        textDecoration: "",
        setProperty: () => undefined,
        removeProperty: (prop: string) => {
          if (prop === "font-size") el.style.fontSize = "";
          if (prop === "font-family") el.style.fontFamily = "";
          if (prop === "font-weight") el.style.fontWeight = "";
          if (prop === "font-style") el.style.fontStyle = "";
          if (prop === "text-decoration") el.style.textDecoration = "";
          if (prop === "line-height") el.style.lineHeight = "";
          if (prop === "color") el.style.color = "";
        },
        color: "",
        verticalAlign: "",
        lineHeight: "",
      },
    } as unknown as HTMLElement;

    const comic20: TypingFormat = {
      fontFace: "Comic Sans MS",
      fontSize: "20",
      bold: false,
      italic: false,
      underline: false,
    };
    applyTypingFormatToToken(el, comic20);
    expect(el.style.fontFamily).toBe("Comic Sans MS");
    expect(el.style.fontSize).toBe("20pt");

    const defaults: TypingFormat = {
      fontFace: DEFAULT_PALETTE_FONT_FACE,
      fontSize: String(DEFAULT_PALETTE_FONT_SIZE_PT),
      bold: false,
      italic: false,
      underline: false,
    };
    applyTypingFormatToToken(el, defaults);
    expect(el.style.fontFamily).toBe("");
    expect(el.style.fontSize).toBe("");
  });

  it("keeps face when size changes (Size must not clear Face on chips)", () => {
    const el = {
      style: {
        fontSize: "",
        fontFamily: "",
        fontWeight: "",
        fontStyle: "",
        textDecoration: "",
        setProperty: () => undefined,
        removeProperty: (prop: string) => {
          if (prop === "font-size") el.style.fontSize = "";
          if (prop === "font-family") el.style.fontFamily = "";
          if (prop === "font-weight") el.style.fontWeight = "";
          if (prop === "font-style") el.style.fontStyle = "";
          if (prop === "text-decoration") el.style.textDecoration = "";
          if (prop === "line-height") el.style.lineHeight = "";
          if (prop === "color") el.style.color = "";
        },
        color: "",
        verticalAlign: "",
        lineHeight: "",
      },
    } as unknown as HTMLElement;

    applyTypingFormatToToken(el, {
      fontFace: "Comic Sans MS",
      fontSize: "20",
      bold: false,
      italic: false,
      underline: false,
    });
    applyTypingFormatToToken(el, {
      fontFace: "Comic Sans MS",
      fontSize: "26",
      bold: false,
      italic: false,
      underline: false,
    });
    expect(el.style.fontFamily).toBe("Comic Sans MS");
    expect(el.style.fontSize).toBe("26pt");
  });

  it("applies Bold/Italic/Underline on chips (Fields)", () => {
    const el = {
      style: {
        fontSize: "",
        fontFamily: "",
        fontWeight: "",
        fontStyle: "",
        textDecoration: "",
        setProperty: () => undefined,
        removeProperty: (prop: string) => {
          if (prop === "font-weight") el.style.fontWeight = "";
          if (prop === "font-style") el.style.fontStyle = "";
          if (prop === "text-decoration") el.style.textDecoration = "";
          if (prop === "font-size") el.style.fontSize = "";
          if (prop === "font-family") el.style.fontFamily = "";
          if (prop === "line-height") el.style.lineHeight = "";
          if (prop === "color") el.style.color = "";
        },
        color: "",
        verticalAlign: "",
        lineHeight: "",
      },
    } as unknown as HTMLElement;

    applyTypingFormatToToken(el, {
      fontFace: DEFAULT_PALETTE_FONT_FACE,
      fontSize: String(DEFAULT_PALETTE_FONT_SIZE_PT),
      bold: true,
      italic: true,
      underline: true,
    });
    expect(el.style.fontWeight).toBe("bold");
    expect(el.style.fontStyle).toBe("italic");
    expect(el.style.textDecoration).toBe("underline");

    applyTypingFormatToToken(el, {
      fontFace: DEFAULT_PALETTE_FONT_FACE,
      fontSize: String(DEFAULT_PALETTE_FONT_SIZE_PT),
      bold: false,
      italic: false,
      underline: false,
    });
    expect(el.style.fontWeight).toBe("");
    expect(el.style.fontStyle).toBe("");
    expect(el.style.textDecoration).toBe("");
  });
});
