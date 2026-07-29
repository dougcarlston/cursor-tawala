import { describe, expect, it } from "vitest";
import {
  cascadeBounds,
  clipArrangeViewport,
  maximizedBounds,
  sanitizeRestoreBounds,
  tileGridDimensions,
  tileHorizontalBounds,
  tileVerticalBounds,
  windowMenuLabel,
  MDI_DEFAULT_H,
  MDI_DEFAULT_W,
  MDI_RESTORE_MIN_H,
  MDI_RESTORE_MIN_W,
} from "./mdiWindowLayout";

const vp = { width: 800, height: 600 };

describe("mdiWindowLayout", () => {
  it("cascades with stepped offsets", () => {
    const b = cascadeBounds(3, vp);
    expect(b).toHaveLength(3);
    expect(b[0]!.x).toBeLessThan(b[1]!.x);
    expect(b[0]!.y).toBeLessThan(b[1]!.y);
  });

  it("cascade sizes leave room inside the viewport", () => {
    const b = cascadeBounds(1, { width: 500, height: 400 });
    expect(b[0]!.w).toBeLessThanOrEqual(500 - 16);
    expect(b[0]!.h).toBeLessThanOrEqual(400 - 16);
    expect(b[0]!.w).toBeGreaterThanOrEqual(MDI_RESTORE_MIN_W);
    expect(b[0]!.h).toBeGreaterThanOrEqual(MDI_RESTORE_MIN_H);
  });

  it("cascade wraps to a second diagonal instead of a flush-right column", () => {
    // Arrange viewport narrow enough that ~4–5 cascade steps hit the right margin
    // (Fields-dock clipped), matching the owner screenshot failure mode.
    const narrow = { width: 800, height: 700 };
    const b = cascadeBounds(8, narrow);
    expect(b).toHaveLength(8);

    const wrapAt = b.findIndex((win, i) => i > 0 && win.x < b[i - 1]!.x);
    expect(wrapAt).toBeGreaterThan(0);

    // Second stack restarts near the left padding, not clamped to maxX.
    expect(b[wrapAt]!.x).toBe(8);
    expect(b[wrapAt]!.y).toBeGreaterThan(b[0]!.y);
    // New stack clears prior title bars: stack2[0].y >= last of stack1.
    expect(b[wrapAt]!.y).toBeGreaterThanOrEqual(b[wrapAt - 1]!.y);

    // Continues as a diagonal (down-right), not a same-x vertical column.
    expect(b[wrapAt + 1]!.x).toBeGreaterThan(b[wrapAt]!.x);
    expect(b[wrapAt + 1]!.y).toBeGreaterThan(b[wrapAt]!.y);
    const postWrapXs = b.slice(wrapAt).map((win) => win.x);
    expect(new Set(postWrapXs).size).toBeGreaterThan(1);
  });

  it("cascade wraps after CASCADE_WRAP on a wide viewport", () => {
    const wide = { width: 2000, height: 1500 };
    const b = cascadeBounds(10, wide);
    expect(b[7]!.x).toBe(8 + 7 * 28);
    expect(b[7]!.y).toBe(8 + 7 * 28);
    // 9th window starts a new lower-left cascade below all prior title-bar offsets.
    expect(b[8]!.x).toBe(8);
    expect(b[8]!.y).toBe(8 + 8 * 28);
    expect(b[8]!.y).toBeGreaterThanOrEqual(b[7]!.y);
    expect(b[9]!.x).toBe(8 + 28);
    expect(b[9]!.y).toBe(8 + 8 * 28 + 28);
  });


  it("after wrap, stack2 origin clears stack1 title bars on a typical viewport", () => {
    const typical = { width: 900, height: 700 };
    const b = cascadeBounds(12, typical);
    expect(b).toHaveLength(12);

    const wrapAt = b.findIndex((win, i) => i > 0 && win.x < b[i - 1]!.x);
    expect(wrapAt).toBeGreaterThan(0);

    const stack1Last = b[wrapAt - 1]!;
    const stack2First = b[wrapAt]!;
    expect(stack2First.x).toBe(8);
    expect(stack2First.y).toBeGreaterThanOrEqual(stack1Last.y);
  });

  it("cascade wraps on short viewports instead of a flush-right column", () => {
    // height - cascadeH is tiny (~40; same class as 600×500 with default 640×460).
    // Old full-frame maxY refused wrap → tail clamped to maxX. Title-bar maxOriginY
    // still allows a second diagonal. Width must exceed cascadeW so maxX > PAD.
    const short = { width: 1000, height: 500 };
    const b = cascadeBounds(11, short);
    expect(b).toHaveLength(11);

    const maxX = Math.max(8, short.width - b[0]!.w - 8);
    expect(maxX).toBeGreaterThan(8);

    const wrapAt = b.findIndex((win, i) => i > 0 && win.x < b[i - 1]!.x);
    expect(wrapAt).toBeGreaterThan(0);

    // After hitting the right edge / CASCADE_WRAP, next window returns near PAD.
    expect(b[wrapAt]!.x).toBe(8);
    expect(b[wrapAt]!.y).toBeGreaterThan(b[0]!.y);

    // Tail is a second diagonal, not a vertical stack all clamped to maxX.
    const tail = b.slice(wrapAt);
    expect(tail.every((win) => win.x === maxX)).toBe(false);
    expect(new Set(tail.map((win) => win.x)).size).toBeGreaterThan(1);
    expect(tail[1]!.x).toBeGreaterThan(tail[0]!.x);
    expect(tail[1]!.y).toBeGreaterThan(tail[0]!.y);
  });

  it("tiles horizontally as stacked strips for a few windows", () => {
    const b = tileHorizontalBounds(2, vp);
    expect(b[0]!.y).toBeLessThan(b[1]!.y);
    expect(b[0]!.x).toBe(b[1]!.x);
    expect(b[0]!.w).toBe(b[1]!.w);
  });

  it("tiles vertically as side-by-side strips for a few windows", () => {
    const b = tileVerticalBounds(2, vp);
    expect(b[0]!.x).toBeLessThan(b[1]!.x);
    expect(b[0]!.y).toBe(b[1]!.y);
  });

  it("Tile Vertically packs three columns for a wide canvas", () => {
    const dim = tileGridDimensions(3, vp, "vertical");
    expect(dim).toEqual({ cols: 3, rows: 1 });
    const b = tileVerticalBounds(3, vp);
    expect(b).toHaveLength(3);
    expect(b[0]!.x).toBeLessThan(b[1]!.x);
    expect(b[1]!.x).toBeLessThan(b[2]!.x);
  });

  it("Tile Horizontally packs three rows for a tall canvas", () => {
    const dim = tileGridDimensions(3, vp, "horizontal");
    expect(dim).toEqual({ cols: 1, rows: 3 });
  });

  it("dense Tile Vertically fills a near-square grid for many windows", () => {
    // Legacy ~20-window shot: 5×4 column-primary packing.
    const dim = tileGridDimensions(20, { width: 1000, height: 700 }, "vertical");
    expect(dim.cols).toBe(5);
    expect(dim.rows).toBe(4);
    const b = tileVerticalBounds(20, { width: 1000, height: 700 });
    expect(b).toHaveLength(20);
    // Grid fills the padded surface — last cell reaches the right/bottom edge.
    const last = b[19]!;
    expect(last.x + last.w).toBe(1000 - 8);
    expect(last.y + last.h).toBe(700 - 8);
  });

  it("formats Windows menu labels", () => {
    expect(windowMenuLabel("form", "Start")).toBe("Form - Start");
    expect(windowMenuLabel("process", "Pre-1")).toBe("Process - Pre-1");
    expect(windowMenuLabel("document", "Thanks")).toBe("Document - Thanks");
  });

  it("maximizedBounds fills the surface", () => {
    expect(maximizedBounds(vp)).toEqual({ x: 0, y: 0, w: 800, h: 600 });
  });
});

describe("clipArrangeViewport", () => {
  it("ends arrange width at Fields dock.left, not surface.right", () => {
    // Surface rect paints under Fields (overflow:visible); Fields left edge at 900.
    const surface = {
      left: 300,
      right: 1180,
      top: 80,
      bottom: 780,
      clientWidth: 880,
      clientHeight: 700,
    };
    const fieldsLeft = 900;
    const arranged = clipArrangeViewport(surface, { right: fieldsLeft });
    expect(arranged.width).toBe(fieldsLeft - surface.left); // 600 — not 880
    expect(arranged.width).toBeLessThan(surface.clientWidth);
    expect(arranged.height).toBe(700);

    const tiled = tileHorizontalBounds(2, arranged);
    for (const b of tiled) {
      expect(b.x + b.w).toBeLessThanOrEqual(arranged.width);
    }
  });

  it("reserves height above the squib taskbar when it overlaps the surface", () => {
    const surface = {
      left: 0,
      right: 800,
      top: 0,
      bottom: 600,
      clientWidth: 800,
      clientHeight: 600,
    };
    const arranged = clipArrangeViewport(surface, { bottom: 568 });
    expect(arranged.height).toBe(568);
    expect(arranged.width).toBe(800);
  });
});

describe("sanitizeRestoreBounds", () => {
  it("keeps comfortable restore sizes unchanged", () => {
    const kept = sanitizeRestoreBounds({ x: 40, y: 30, w: 500, h: 400 }, vp);
    expect(kept).toEqual({ x: 40, y: 30, w: 500, h: 400 });
  });

  it("replaces postage-stamp sizes after shrink-then-minimize (Windows6)", () => {
    const tiny = sanitizeRestoreBounds({ x: 10, y: 10, w: 120, h: 90 }, vp);
    expect(tiny.w).toBeGreaterThanOrEqual(MDI_RESTORE_MIN_W);
    expect(tiny.h).toBeGreaterThanOrEqual(MDI_RESTORE_MIN_H);
    expect(tiny.w).toBeLessThanOrEqual(MDI_DEFAULT_W);
    expect(tiny.h).toBeLessThanOrEqual(MDI_DEFAULT_H);
  });

  it("clamps default restore into a small viewport", () => {
    const smallVp = { width: 400, height: 300 };
    const tiny = sanitizeRestoreBounds({ x: 0, y: 0, w: 80, h: 60 }, smallVp);
    expect(tiny.w).toBeLessThanOrEqual(400 - 16);
    expect(tiny.h).toBeLessThanOrEqual(300 - 16);
    expect(tiny.w).toBeGreaterThanOrEqual(MDI_RESTORE_MIN_W);
    expect(tiny.h).toBeGreaterThanOrEqual(MDI_RESTORE_MIN_H);
  });
});
