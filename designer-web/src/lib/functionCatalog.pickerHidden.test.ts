/**
 * Insert → Function picker visibility (deferred stubs hidden).
 */
import { describe, expect, it } from "vitest";
import {
  FUNCTION_CATEGORIES,
  getFunctionDef,
  pickerVisibleFunctions,
} from "./functionCatalog";

const HIDDEN_STUBS = [
  "categorizer",
  "export-team-roster",
  "link-to-project-details",
  "paypal-single-item-button",
] as const;

describe("functionCatalog pickerHidden stubs", () => {
  it("marks the four Document HTML→XML stubs as pickerHidden", () => {
    for (const id of HIDDEN_STUBS) {
      expect(getFunctionDef(id)?.pickerHidden).toBe(true);
    }
  });

  it("omits stubs from pickerVisibleFunctions and every category list", () => {
    const visibleIds = new Set(pickerVisibleFunctions().map((f) => f.id));
    for (const id of HIDDEN_STUBS) {
      expect(visibleIds.has(id)).toBe(false);
    }
    for (const cat of FUNCTION_CATEGORIES) {
      for (const id of HIDDEN_STUBS) {
        expect(cat.functionIds).not.toContain(id);
      }
    }
    expect(FUNCTION_CATEGORIES.some((c) => c.id === "payments")).toBe(false);
  });
});
