import { describe, expect, it, beforeEach } from "vitest";
import {
  getFieldsPaletteSelection,
  resetFieldsPaletteSelectionForTests,
  setFieldsPaletteSelection,
} from "./fieldsPaletteSelection";

describe("fieldsPaletteSelection", () => {
  beforeEach(() => {
    resetFieldsPaletteSelectionForTests();
  });

  it("starts empty", () => {
    expect(getFieldsPaletteSelection()).toBeNull();
  });

  it("stores trimmed insert names", () => {
    setFieldsPaletteSelection("  Form 1:Name  ");
    expect(getFieldsPaletteSelection()).toBe("Form 1:Name");
  });

  it("clears on empty", () => {
    setFieldsPaletteSelection("x");
    setFieldsPaletteSelection("");
    expect(getFieldsPaletteSelection()).toBeNull();
  });
});
