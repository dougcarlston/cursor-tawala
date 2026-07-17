import { describe, expect, it } from "vitest";
import {
  addTabStop,
  inchesToTwips,
  tabPositionsXmlFromInches,
} from "./tabStops";
import {
  applyFibStyle,
  applyMcStyle,
  applyTextStyle,
  fibStyleToken,
  parseFibStyle,
} from "./formItemStyles";

describe("formItemStyles", () => {
  it("maps FIB placement + align-right to legacy style tokens", () => {
    expect(fibStyleToken({ placement: "above", alignRightSide: false })).toBe("topLabels");
    expect(fibStyleToken({ placement: "freeform", alignRightSide: true })).toBe("freeform");
    expect(fibStyleToken({ placement: "left", alignRightSide: false })).toBe("leftAlignLabels");
    expect(fibStyleToken({ placement: "left", alignRightSide: true })).toBe(
      "leftAlignLabelsJustified",
    );
    expect(fibStyleToken({ placement: "right", alignRightSide: true })).toBe(
      "rightAlignLabelsJustified",
    );
  });

  it("round-trips FIB parse/apply", () => {
    const draft = parseFibStyle("rightAlignLabelsJustified");
    expect(draft).toEqual({ placement: "right", alignRightSide: true });
    expect(applyFibStyle({ type: "fib", label: "FIB1" }, draft).style).toBe(
      "rightAlignLabelsJustified",
    );
  });

  it("applies MCQ layout, columnCount, and paddingBottom", () => {
    const item = applyMcStyle(
      { type: "mc", label: "MCQ1" },
      { layout: "multicolumn", columnCount: 3, noPaddingBottom: true },
    );
    expect(item.style).toBe("multicolumn");
    expect(item.columnCount).toBe(3);
    expect(item.paddingBottom).toBe(false);
  });

  it("applies Text instructional style and clears padding when unchecked", () => {
    const withPad = applyTextStyle(
      { type: "text", label: "T1", paddingBottom: false },
      { style: "instructional", noPaddingBottom: false },
    );
    expect(withPad.style).toBe("instructional");
    expect(withPad.paddingBottom).toBeUndefined();
  });
});

describe("tabStops", () => {
  it("converts inches to twips (0.5 → 720, 2 → 2880)", () => {
    expect(inchesToTwips(0.5)).toBe(720);
    expect(inchesToTwips(2)).toBe(2880);
  });

  it("adds sorted unique stops within 0–6.5", () => {
    expect(addTabStop([2], 0.5)).toEqual([0.5, 2]);
    expect(addTabStop([0.5], 0.5)).toBeNull();
    expect(addTabStop([], 0)).toBeNull();
    expect(addTabStop([], 7)).toBeNull();
  });

  it("emits tabPositions XML or falls back", () => {
    const fallback = '<tabPositions><tabStop position="2880"/></tabPositions>';
    expect(tabPositionsXmlFromInches(undefined, fallback)).toBe(fallback);
    expect(tabPositionsXmlFromInches([0.5, 2], fallback)).toBe(
      '<tabPositions><tabStop position="720"/><tabStop position="2880"/></tabPositions>',
    );
  });
});
