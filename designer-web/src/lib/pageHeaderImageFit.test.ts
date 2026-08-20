import { describe, expect, it } from "vitest";
import {
  PAGE_HEADER_FRAME_H,
  PAGE_HEADER_FRAME_W,
  clampViewport,
  coverViewport,
  sourceRectForFrame,
} from "./pageHeaderImageFit";

describe("pageHeaderImageFit", () => {
  it("coverViewport fills the frame (wide banner vs square photo)", () => {
    const vp = coverViewport(400, 400);
    expect(vp.scaleX).toBeCloseTo(PAGE_HEADER_FRAME_W / 400, 5);
    expect(vp.scaleY).toBe(vp.scaleX);
    // Wider than tall frame → crop top/bottom
    expect(vp.x).toBe(0);
    expect(vp.y).toBeLessThan(0);
  });

  it("clampViewport keeps a zoomed image covering the frame", () => {
    const base = coverViewport(800, 400);
    const zoomed = clampViewport(
      { ...base, scaleX: base.scaleX * 2, scaleY: base.scaleY * 2, x: -500, y: -200 },
      800,
      400,
    );
    const dw = 800 * zoomed.scaleX;
    const dh = 400 * zoomed.scaleY;
    expect(zoomed.x).toBeLessThanOrEqual(0);
    expect(zoomed.x).toBeGreaterThanOrEqual(PAGE_HEADER_FRAME_W - dw);
    expect(zoomed.y).toBeLessThanOrEqual(0);
    expect(zoomed.y).toBeGreaterThanOrEqual(PAGE_HEADER_FRAME_H - dh);
  });

  it("sourceRectForFrame maps frame to source for cover", () => {
    const vp = coverViewport(1000, 500);
    const rect = sourceRectForFrame(vp, 1000, 500);
    expect(rect.sw).toBeCloseTo(PAGE_HEADER_FRAME_W / vp.scaleX, 5);
    expect(rect.sh).toBeCloseTo(PAGE_HEADER_FRAME_H / vp.scaleY, 5);
    expect(rect.sx).toBeGreaterThanOrEqual(0);
    expect(rect.sy).toBeGreaterThanOrEqual(-1);
  });

  it("wide cover-fit locks vertical pan until zoomed (dh == frame height)", () => {
    // Image wider than banner aspect → cover scales by height → no Y slack.
    const vp = coverViewport(2000, 400);
    expect(400 * vp.scaleY).toBeCloseTo(PAGE_HEADER_FRAME_H, 5);
    const nudged = clampViewport({ ...vp, y: vp.y - 40 }, 2000, 400);
    expect(nudged.y).toBe(0);
  });

  it("after zoom-in, clampViewport allows moving image down (show top of photo)", () => {
    const base = coverViewport(2000, 400);
    const zoomed = clampViewport(
      { ...base, scaleX: base.scaleX * 1.5, scaleY: base.scaleY * 1.5, x: 0, y: 0 },
      2000,
      400,
    );
    expect(zoomed.y).toBe(0); // top-aligned
    const showBottom = clampViewport(
      { ...zoomed, y: -9999 },
      2000,
      400,
    );
    const dh = 400 * showBottom.scaleY;
    expect(showBottom.y).toBeCloseTo(PAGE_HEADER_FRAME_H - dh, 5);
  });
});
