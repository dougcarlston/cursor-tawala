import { describe, expect, it } from "vitest";
import {
  addTabStop,
  inchesToTwips,
  tabPositionsXmlFromInches,
} from "./tabStops";
import {
  applyFibStyle,
  applyMcStyle,
  applyStyleToAllFormItems,
  applyTextStyle,
  countFormItemsOfKind,
  fibStyleToken,
  parseFibStyle,
  stylesKindForFormItem,
} from "./formItemStyles";
import type { FormItem } from "@/types/tawala";

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

  it("applyStyleToAllFormItems patches only matching kinds on one form", () => {
    const items: FormItem[] = [
      { type: "fib", label: "F1", style: "topLabels" },
      { type: "text", label: "T1", style: "normal" },
      { type: "fib", label: "F2", style: "freeform" },
      { type: "mc", label: "M1", style: "vertical" },
    ];
    expect(countFormItemsOfKind(items, "fib")).toBe(2);
    expect(countFormItemsOfKind(items, "text")).toBe(1);

    const { items: next, changed } = applyStyleToAllFormItems(items, "fib", {
      placement: "left",
      alignRightSide: true,
    });
    expect(changed).toBe(2);
    expect(next[0]).toMatchObject({ type: "fib", label: "F1", style: "leftAlignLabelsJustified" });
    expect(next[1]).toEqual(items[1]);
    expect(next[2]).toMatchObject({ type: "fib", label: "F2", style: "leftAlignLabelsJustified" });
    expect(next[3]).toEqual(items[3]);
  });

  it("applyStyleToAllFormItems leaves other forms' items untouched when called per form", () => {
    const formA: FormItem[] = [
      { type: "mc", label: "A1", style: "vertical" },
      { type: "mc", label: "A2", style: "horizontal" },
    ];
    const formB: FormItem[] = [{ type: "mc", label: "B1", style: "vertical" }];
    const { items: nextA, changed } = applyStyleToAllFormItems(formA, "mc", {
      layout: "multicolumn",
      columnCount: 2,
      noPaddingBottom: true,
    });
    expect(changed).toBe(2);
    expect(nextA.every((i) => i.type === "mc" && i.style === "multicolumn")).toBe(true);
    expect(formB[0].style).toBe("vertical");
  });

  it("stylesKindForFormItem maps only FIB / MCQ / Text", () => {
    expect(stylesKindForFormItem({ type: "fib", label: "F1" })).toBe("fib");
    expect(stylesKindForFormItem({ type: "mc", label: "M1" })).toBe("mc");
    expect(stylesKindForFormItem({ type: "text", label: "T1" })).toBe("text");
    expect(stylesKindForFormItem({ type: "heading", label: "H1", content: "" })).toBeNull();
    expect(stylesKindForFormItem({ type: "field", label: "HF1", name: "x" })).toBeNull();
    expect(stylesKindForFormItem({ type: "break", label: "PB1" })).toBeNull();
    expect(stylesKindForFormItem({ type: "skipInstructions", label: "SI1" })).toBeNull();
    expect(stylesKindForFormItem(null)).toBeNull();
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
