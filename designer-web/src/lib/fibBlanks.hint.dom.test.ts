/**
 * FIB default-hint selection must exclude trailing underscore blanks.
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import { FIB_DEFAULT_PROMPT, FIB_PLACEHOLDER } from "@/types/tawala";
import { fibHintHighlightEnd, setPlainTextRange } from "@/lib/fibBlanks";

describe("setPlainTextRange hint selection", () => {
  it("highlights hint prose without the underscore run", () => {
    const root = document.createElement("div");
    root.textContent = FIB_DEFAULT_PROMPT;
    const range = document.createRange();
    const end = fibHintHighlightEnd(root.textContent ?? "")!;
    expect(setPlainTextRange(root, range, 0, end)).toBe(true);
    expect(range.toString()).toBe(FIB_PLACEHOLDER);
    expect(range.toString()).not.toContain("_");
  });
});
