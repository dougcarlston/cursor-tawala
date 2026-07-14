/**
 * Guards the Chrome fontSize 1–7 marker rewrite contract.
 * Leaving unre-written size="7" / xxx-large shows ~36pt when the owner asked for 8pt.
 */
import { describe, expect, it } from "vitest";
import { isChromeSizeSevenMarker } from "@/lib/paletteCommands";

function fakeEl(tag: string, attrs: { size?: string; fontSize?: string }): HTMLElement {
  const style = { fontSize: attrs.fontSize ?? "" };
  return {
    tagName: tag.toUpperCase(),
    style,
    getAttribute: (name: string) => (name === "size" ? (attrs.size ?? null) : null),
  } as unknown as HTMLElement;
}

describe("isChromeSizeSevenMarker", () => {
  it("detects HTML font size=7 (legacy execCommand marker → ~36pt)", () => {
    expect(isChromeSizeSevenMarker(fakeEl("font", { size: "7" }))).toBe(true);
    expect(isChromeSizeSevenMarker(fakeEl("font", { size: "3" }))).toBe(false);
    expect(isChromeSizeSevenMarker(fakeEl("font", { size: "1" }))).toBe(false);
  });

  it("detects Chrome CSS keyword markers for size 7", () => {
    expect(isChromeSizeSevenMarker(fakeEl("span", { fontSize: "xxx-large" }))).toBe(true);
    expect(isChromeSizeSevenMarker(fakeEl("span", { fontSize: "-webkit-xxx-large" }))).toBe(true);
    expect(isChromeSizeSevenMarker(fakeEl("span", { fontSize: "xx-large" }))).toBe(false);
    expect(isChromeSizeSevenMarker(fakeEl("span", { fontSize: "8pt" }))).toBe(false);
  });

  it("ignores already-rewritten point sizes", () => {
    expect(isChromeSizeSevenMarker(fakeEl("font", { fontSize: "8pt" }))).toBe(false);
    expect(isChromeSizeSevenMarker(fakeEl("span", { fontSize: "36pt" }))).toBe(false);
  });
});
