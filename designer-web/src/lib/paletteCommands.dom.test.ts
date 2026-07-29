/**
 * @vitest-environment happy-dom
 *
 * Face/Size helpers used by Document + Form Text palette commands.
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  isChromeSizeSevenMarker,
  selectionCoversFullPlacedBlockForTest,
  snapFontSizePtForTest,
} from "./paletteCommands";

afterEach(() => {
  document.body.innerHTML = "";
});

describe("isChromeSizeSevenMarker", () => {
  it("detects unre-written Chrome size-7 / xxx-large markers", () => {
    const font = document.createElement("font");
    font.setAttribute("size", "7");
    expect(isChromeSizeSevenMarker(font)).toBe(true);

    const span = document.createElement("span");
    span.style.fontSize = "xxx-large";
    expect(isChromeSizeSevenMarker(span)).toBe(true);

    // happy-dom clears unsupported -webkit-xxx-large from CSSOM; stub like Chrome.
    const webkit = {
      tagName: "SPAN",
      style: { fontSize: "-webkit-xxx-large" },
      getAttribute: () => null,
    } as unknown as HTMLElement;
    expect(isChromeSizeSevenMarker(webkit)).toBe(true);

    const normal = document.createElement("span");
    normal.style.fontSize = "20pt";
    expect(isChromeSizeSevenMarker(normal)).toBe(false);

    const size3 = document.createElement("font");
    size3.setAttribute("size", "3");
    expect(isChromeSizeSevenMarker(size3)).toBe(false);
  });
});

describe("selectionCoversFullPlacedBlockForTest", () => {
  it("is true for selectNodeContents and false for a partial word", () => {
    const block = document.createElement("p");
    block.className = "doc-placed-text";
    block.textContent = "one two three";
    document.body.appendChild(block);

    const full = document.createRange();
    full.selectNodeContents(block);
    expect(selectionCoversFullPlacedBlockForTest(full, block)).toBe(true);

    const text = block.firstChild as Text;
    const partial = document.createRange();
    partial.setStart(text, 0);
    partial.setEnd(text, 3); // "one"
    expect(selectionCoversFullPlacedBlockForTest(partial, block)).toBe(false);
  });
});

describe("snapFontSizePtForTest", () => {
  it("snaps to nearest palette pt stop", () => {
    expect(snapFontSizePtForTest(12)).toBe("12");
    expect(snapFontSizePtForTest(10)).toBe("10");
    expect(snapFontSizePtForTest(10.4)).toMatch(/10|11/);
  });
});
