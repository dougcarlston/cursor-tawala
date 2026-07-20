/**
 * @vitest-environment happy-dom
 *
 * Document commit must not strip font-size from function/field chips — that
 * made 12pt chips inherit a larger parent after MDI ResizeObserver commits.
 */
import { describe, expect, it } from "vitest";
import { isRedundantDefaultFontSize } from "./fontSizeStrip";
import { DEFAULT_PALETTE_FONT_SIZE_PT } from "./paletteDefaults";

/** Mirrors RichTextEditor.stripFontSizeFormatting chip guard. */
function stripFontSizeFormatting(root: ParentNode) {
  root.querySelectorAll<HTMLElement>("[style]").forEach((node) => {
    if (node.classList.contains("function-token") || node.classList.contains("field-token")) {
      return;
    }
    if (node.closest(".function-token, .field-token")) return;
    if (!node.style.fontSize) return;
    if (!isRedundantDefaultFontSize(node.style.fontSize)) return;
    node.style.removeProperty("font-size");
  });
}

describe("stripFontSizeFormatting chip guard", () => {
  it("keeps 12pt on a function chip inside a larger parent", () => {
    const root = document.createElement("div");
    root.innerHTML =
      `<p style="font-size: 20pt">` +
      `<span class="function-token" style="font-size: ${DEFAULT_PALETTE_FONT_SIZE_PT}pt">` +
      `<<MQL>>` +
      `</span></p>`;
    stripFontSizeFormatting(root);
    const chip = root.querySelector(".function-token") as HTMLElement;
    expect(chip.style.fontSize).toBe(`${DEFAULT_PALETTE_FONT_SIZE_PT}pt`);
  });

  it("still strips redundant 12pt from ordinary spans", () => {
    const root = document.createElement("div");
    root.innerHTML = `<span style="font-size: ${DEFAULT_PALETTE_FONT_SIZE_PT}pt">Hello</span>`;
    stripFontSizeFormatting(root);
    const span = root.querySelector("span") as HTMLElement;
    expect(span.style.fontSize).toBe("");
  });
});
