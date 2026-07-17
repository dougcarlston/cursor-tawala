import { describe, expect, it } from "vitest";
import {
  cascadeBounds,
  tileHorizontalBounds,
  tileVerticalBounds,
  windowMenuLabel,
} from "./mdiWindowLayout";

const vp = { width: 800, height: 600 };

describe("mdiWindowLayout", () => {
  it("cascades with stepped offsets", () => {
    const b = cascadeBounds(3, vp);
    expect(b).toHaveLength(3);
    expect(b[0]!.x).toBeLessThan(b[1]!.x);
    expect(b[0]!.y).toBeLessThan(b[1]!.y);
  });

  it("tiles horizontally as stacked strips", () => {
    const b = tileHorizontalBounds(2, vp);
    expect(b[0]!.y).toBeLessThan(b[1]!.y);
    expect(b[0]!.x).toBe(b[1]!.x);
    expect(b[0]!.w).toBe(b[1]!.w);
  });

  it("tiles vertically as side-by-side strips", () => {
    const b = tileVerticalBounds(2, vp);
    expect(b[0]!.x).toBeLessThan(b[1]!.x);
    expect(b[0]!.y).toBe(b[1]!.y);
  });

  it("formats Windows menu labels", () => {
    expect(windowMenuLabel("form", "Start")).toBe("Form - Start");
    expect(windowMenuLabel("process", "Pre-1")).toBe("Process - Pre-1");
    expect(windowMenuLabel("document", "Thanks")).toBe("Document - Thanks");
  });
});
