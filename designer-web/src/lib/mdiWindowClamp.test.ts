import { describe, expect, it } from "vitest";
import {
  MDI_TITLEBAR_CONTROLS_W,
  clampMdiWindowBounds,
  clampMdiWindowOrigin,
} from "./mdiWindowClamp";

describe("clampMdiWindowOrigin", () => {
  it("allows sliding left under Project Explorer / Items (negative x)", () => {
    const parentW = 800;
    const parentH = 600;
    const w = 400;
    const h = 300;
    const left = clampMdiWindowOrigin(-200, 10, w, h, parentW, parentH);
    expect(left.x).toBe(-200);
    // Right CONTROLS_W of the frame still intersects the surface.
    expect(left.x + w).toBeGreaterThanOrEqual(MDI_TITLEBAR_CONTROLS_W);
  });

  it("allows sliding right under Fields so close controls can clear occlusion", () => {
    const parentW = 800;
    const parentH = 600;
    const w = 400;
    const h = 300;
    // Old "fully on-canvas" clamp capped at parentW - w (=400). Owner needs further.
    const farRight = clampMdiWindowOrigin(720, 10, w, h, parentW, parentH);
    expect(farRight.x).toBe(720);
    expect(farRight.x).toBeGreaterThan(parentW - w);
    const capped = clampMdiWindowOrigin(900, 10, w, h, parentW, parentH);
    expect(capped.x).toBe(parentW - MDI_TITLEBAR_CONTROLS_W);
  });

  it("lets an oversized window slide so title-bar controls enter the surface", () => {
    // Palette restored after enlarge: frame wider than surface, previously pinned at x=0
    // with close buttons clipped past the right edge.
    const parentW = 500;
    const parentH = 400;
    const w = 700;
    const h = 300;
    const slid = clampMdiWindowOrigin(-250, 0, w, h, parentW, parentH);
    expect(slid.x).toBe(-250);
    // Controls live in [x+w-CONTROLS_W, x+w]; after slide they intersect the surface.
    const controlsLeft = slid.x + w - MDI_TITLEBAR_CONTROLS_W;
    expect(controlsLeft).toBeLessThan(parentW);
    expect(slid.x + w).toBeGreaterThan(0);
  });

  it("does not let the frame disappear entirely off either side", () => {
    const w = 400;
    const tooFarLeft = clampMdiWindowOrigin(-9999, 0, w, 200, 800, 600);
    expect(tooFarLeft.x).toBe(MDI_TITLEBAR_CONTROLS_W - w);
    const tooFarRight = clampMdiWindowOrigin(9999, 0, w, 200, 800, 600);
    expect(tooFarRight.x).toBe(800 - MDI_TITLEBAR_CONTROLS_W);
  });

  it("keeps title bar visible vertically", () => {
    const o = clampMdiWindowOrigin(0, 9999, 400, 300, 800, 600);
    expect(o.y).toBe(600 - 28);
  });

  it("rejects upward escape above the surface", () => {
    const o = clampMdiWindowOrigin(10, -40, 400, 300, 800, 600);
    expect(o.y).toBe(0);
  });
});

describe("clampMdiWindowBounds", () => {
  it("caps east resize width to the surface (maximize/tile stay sane)", () => {
    const next = clampMdiWindowBounds({ x: 100, y: 20, w: 900, h: 200 }, 800, 600, 320, 180);
    expect(next.w).toBeLessThanOrEqual(800);
    expect(next.w).toBe(800);
  });

  it("shrinks an oversized frame on resize but may leave origin under docks", () => {
    const next = clampMdiWindowBounds({ x: -100, y: 0, w: 900, h: 200 }, 500, 400, 320, 180);
    expect(next.w).toBe(500);
    // Origin clamp may keep a negative x so the frame can sit under left docks.
    expect(next.x + next.w).toBeGreaterThanOrEqual(MDI_TITLEBAR_CONTROLS_W);
  });

  it("respects minimum size", () => {
    const next = clampMdiWindowBounds({ x: 0, y: 0, w: 10, h: 10 }, 800, 600, 320, 180);
    expect(next.w).toBe(320);
    expect(next.h).toBe(180);
  });
});
