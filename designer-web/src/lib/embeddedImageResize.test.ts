/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  DEFAULT_MAX_INSERT_WIDTH_PX,
  MIN_EMBEDDED_IMAGE_PX,
  applyEmbeddedImageDisplaySize,
  fitEmbeddedImageSize,
  sizeFromSeDrag,
} from "@/lib/embeddedImageResize";
import { EMBEDDED_IMAGE_HEIGHT_ATTR, EMBEDDED_IMAGE_WIDTH_ATTR } from "@/lib/projectImages";

describe("fitEmbeddedImageSize", () => {
  it("leaves mid-size photos alone when under max width", () => {
    // Taller than the icon heuristic band (h0 > 160) so we don't auto-shrink chrome.
    expect(fitEmbeddedImageSize(200, 200, DEFAULT_MAX_INSERT_WIDTH_PX)).toEqual({
      width: 200,
      height: 200,
    });
  });

  it("scales large images to max width", () => {
    expect(fitEmbeddedImageSize(2000, 1000, 480)).toEqual({
      width: 480,
      height: 240,
    });
  });

  it("shrinks small UI screenshots to text-ish height", () => {
    expect(fitEmbeddedImageSize(88, 72, DEFAULT_MAX_INSERT_WIDTH_PX)).toEqual({
      width: 22,
      height: 18,
    });
  });
});

describe("sizeFromSeDrag", () => {
  it("locks aspect when dragging SE primarily by width", () => {
    expect(sizeFromSeDrag(200, 100, -50, -10)).toEqual({ width: 150, height: 75 });
  });

  it("respects minimum size", () => {
    const sized = sizeFromSeDrag(40, 40, -100, -100);
    expect(sized.width).toBeGreaterThanOrEqual(MIN_EMBEDDED_IMAGE_PX);
    expect(sized.height).toBeGreaterThanOrEqual(MIN_EMBEDDED_IMAGE_PX);
  });

  it("clamps to maxWidth", () => {
    expect(sizeFromSeDrag(200, 100, 400, 0, 300)).toEqual({ width: 300, height: 150 });
  });
});

describe("applyEmbeddedImageDisplaySize", () => {
  it("writes width/height attrs used by Deploy", () => {
    const img = document.createElement("img");
    applyEmbeddedImageDisplaySize(img, 120.6, 80.2);
    expect(img.getAttribute(EMBEDDED_IMAGE_WIDTH_ATTR)).toBe("121");
    expect(img.getAttribute(EMBEDDED_IMAGE_HEIGHT_ATTR)).toBe("80");
    expect(img.style.width).toBe("121px");
    expect(img.style.height).toBe("80px");
  });
});
