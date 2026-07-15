/**
 * @vitest-environment happy-dom
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  MAX_RECENT_FONT_COLORS,
  getRecentFontColors,
  normalizeFontColorHex,
  pushRecentFontColor,
  resetRecentFontColorsForTests,
} from "@/lib/recentFontColors";
import {
  getPaletteCurrentFontColor,
  resetPaletteCurrentFontColorForTests,
  setPaletteCurrentFontColor,
} from "@/lib/paletteCurrentFontColor";
import { sampleFontColorAtPoint } from "@/lib/sampleFontColorAtPoint";

afterEach(() => {
  resetRecentFontColorsForTests();
  resetPaletteCurrentFontColorForTests();
});

describe("normalizeFontColorHex", () => {
  it("normalizes #rgb and #rrggbb", () => {
    expect(normalizeFontColorHex("#AbC")).toBe("#aabbcc");
    expect(normalizeFontColorHex("#AABBCC")).toBe("#aabbcc");
  });

  it("rejects non-colors", () => {
    expect(normalizeFontColorHex("red")).toBeNull();
    expect(normalizeFontColorHex("")).toBeNull();
  });
});

describe("pushRecentFontColor", () => {
  it("keeps newest first, dedupes, and caps at max", () => {
    expect(pushRecentFontColor("#ff0000")).toBe("#ff0000");
    pushRecentFontColor("#00ff00");
    pushRecentFontColor("#0000ff");
    pushRecentFontColor("#ff0000"); // move to front
    expect(getRecentFontColors()).toEqual(["#ff0000", "#0000ff", "#00ff00"]);

    for (let i = 0; i < MAX_RECENT_FONT_COLORS + 2; i++) {
      pushRecentFontColor(`#${i.toString(16).padStart(2, "0")}0000`);
    }
    const list = getRecentFontColors();
    expect(list).toHaveLength(MAX_RECENT_FONT_COLORS);
    expect(list[0]).toBe(`#${(MAX_RECENT_FONT_COLORS + 1).toString(16).padStart(2, "0")}0000`);
    expect(list).not.toContain("#ff0000");
  });

  it("persists to localStorage", () => {
    pushRecentFontColor("#abcdef");
    const stored = localStorage.getItem("tawala.designer.recentFontColors");
    expect(JSON.parse(stored!)).toEqual(["#abcdef"]);
  });
});

describe("paletteCurrentFontColor", () => {
  it("defaults to black and updates sticky A-bar color", () => {
    expect(getPaletteCurrentFontColor()).toBe("#000000");
    expect(setPaletteCurrentFontColor("#800080")).toBe("#800080");
    expect(getPaletteCurrentFontColor()).toBe("#800080");
    expect(setPaletteCurrentFontColor("bogus")).toBeNull();
    expect(getPaletteCurrentFontColor()).toBe("#800080");
  });
});

describe("sampleFontColorAtPoint", () => {
  it("reads explicit style.color under the point", () => {
    const root = document.createElement("div");
    root.innerHTML = `<span style="color: #ff00aa">purple</span>`;
    document.body.appendChild(root);
    const span = root.querySelector("span")!;
    const rect = span.getBoundingClientRect();
    // happy-dom often reports 0-size rects — stub elementFromPoint.
    const orig = document.elementFromPoint.bind(document);
    document.elementFromPoint = () => span;
    try {
      expect(sampleFontColorAtPoint(root, rect.left + 1, rect.top + 1)).toBe("#ff00aa");
    } finally {
      document.elementFromPoint = orig;
      root.remove();
    }
  });
});
