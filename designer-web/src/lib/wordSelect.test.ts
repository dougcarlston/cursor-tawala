import { describe, expect, it } from "vitest";
import { isMultiClickSelectionGesture, isWordChar, wordBoundsInText } from "@/lib/wordSelect";
import { matchFontFace } from "@/lib/paletteDefaults";

describe("wordBoundsInText (cross-span double-click)", () => {
  it("expands to full word inside a plain string", () => {
    expect(wordBoundsInText("pretty much", 3)).toEqual({ start: 0, end: 6 });
    expect(wordBoundsInText("pretty much", 7)).toEqual({ start: 7, end: 11 });
  });

  it("treats punctuation as a boundary", () => {
    expect(wordBoundsInText("hello,world", 2)).toEqual({ start: 0, end: 5 });
    expect(wordBoundsInText("hello,world", 6)).toEqual({ start: 6, end: 11 });
  });

  it("handles caret at end-of-word edge", () => {
    expect(wordBoundsInText("everywhere", 10)).toEqual({ start: 0, end: 10 });
  });

  it("isWordChar matches alphanumerics", () => {
    expect(isWordChar("a")).toBe(true);
    expect(isWordChar("9")).toBe(true);
    expect(isWordChar(" ")).toBe(false);
    expect(isWordChar(",")).toBe(false);
  });
});

describe("isMultiClickSelectionGesture (triple-click guard)", () => {
  it("treats detail ≥ 2 as a selection gesture that must not collapse to a caret", () => {
    expect(isMultiClickSelectionGesture(1)).toBe(false);
    expect(isMultiClickSelectionGesture(2)).toBe(true);
    expect(isMultiClickSelectionGesture(3)).toBe(true);
  });
});

describe("matchFontFace", () => {
  it("maps CSS stacks to palette faces", () => {
    expect(matchFontFace("'Comic Sans MS', cursive")).toBe("Comic Sans MS");
    expect(matchFontFace("Arial, Helvetica, sans-serif")).toBe("Arial");
    expect(matchFontFace("Segoe UI")).toBe("Arial");
  });

  it("maps abbreviated stems (Comic → Comic Sans MS) without stealing Arial Black", () => {
    expect(matchFontFace("Comic")).toBe("Comic Sans MS");
    expect(matchFontFace("Times")).toBe("Times New Roman");
    expect(matchFontFace("Arial")).toBe("Arial");
    expect(matchFontFace("Arial Black")).toBe("Arial Black");
  });
});
