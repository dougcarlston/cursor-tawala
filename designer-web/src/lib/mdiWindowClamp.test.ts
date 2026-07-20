import { describe, expect, it } from "vitest";
import {
  MDI_TITLEBAR_CONTROLS_W,
  clampMdiWindowBounds,
  clampMdiWindowOrigin,
} from "./mdiWindowClamp";

describe("clampMdiWindowOrigin", () => {
  it("keeps a fitting window fully on-canvas (controls cannot slide under Fields)", () => {
    const parentW = 800;
    const parentH = 600;
    const w = 400;
    const h = 300;
    // Old bug: maxX = parentW - 80 let left=720 with w=400 → controls at ~1120 (off-canvas).
    const farRight = clampMdiWindowOrigin(720, 10, w, h, parentW, parentH);
    expect(farRight.x).toBe(parentW - w);
    expect(farRight.x + w).toBe(parentW);
    // Controls sit in the last CONTROLS_W of the frame.
    expect(farRight.x + w - MDI_TITLEBAR_CONTROLS_W).toBeLessThanOrEqual(parentW);
  });

  it("does not allow sliding left of the canvas (Project Explorer side)", () => {
    const o = clampMdiWindowOrigin(-40, -10, 400, 300, 800, 600);
    expect(o.x).toBe(0);
    expect(o.y).toBe(0);
  });

  it("keeps title bar visible vertically", () => {
    const o = clampMdiWindowOrigin(0, 9999, 400, 300, 800, 600);
    expect(o.y).toBe(600 - 28);
  });

  it("for oversized windows, pins left at 0 (resize clamp shrinks width instead)", () => {
    const parentW = 500;
    const w = 700;
    const o = clampMdiWindowOrigin(0, 0, w, 200, parentW, 400);
    expect(o.x).toBe(0);
  });
});

describe("clampMdiWindowBounds", () => {
  it("caps east resize so the frame does not extend under Fields", () => {
    const next = clampMdiWindowBounds({ x: 100, y: 20, w: 900, h: 200 }, 800, 600, 320, 180);
    expect(next.x + next.w).toBeLessThanOrEqual(800);
    expect(next.w).toBeLessThanOrEqual(800);
  });

  it("shrinks an oversized frame so title-bar controls stay on-canvas", () => {
    const next = clampMdiWindowBounds({ x: 0, y: 0, w: 900, h: 200 }, 500, 400, 320, 180);
    expect(next.w).toBe(500);
    expect(next.x + next.w).toBe(500);
  });

  it("respects minimum size", () => {
    const next = clampMdiWindowBounds({ x: 0, y: 0, w: 10, h: 10 }, 800, 600, 320, 180);
    expect(next.w).toBe(320);
    expect(next.h).toBe(180);
  });
});
