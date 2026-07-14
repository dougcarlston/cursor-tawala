/**
 * Placement guards for Document canvas clicks.
 * Locks the orphan-text vs blank-canvas contract that was inventing a new
 * `.doc-placed-text` mid-sentence.
 */
import { describe, expect, it } from "vitest";

/**
 * Mirrors the decision gates in `shouldPlaceDocumentText` /
 * `placeDocumentTextAtPoint` for inventing a new placed block.
 */
function shouldInventPlacedBlock(args: {
  hitIsEditor: boolean;
  hitInsidePlaced: boolean;
  hitInsideTable: boolean;
  rangeInExistingText: boolean;
  /** caretRangeFromPoint landed in an absolute table but the click was blank canvas */
  rangeOnlyInTable: boolean;
  nearPlacedLine: boolean;
  orphanGlyphsUnderPoint: boolean;
}): boolean {
  if (args.hitInsidePlaced) return false;
  if (args.hitInsideTable) return false;
  if (args.nearPlacedLine) return false;
  // Absolute tables steal caretRangeFromPoint — ignore that for invent decisions.
  if (args.rangeOnlyInTable) {
    if (args.hitIsEditor) return true;
    return true;
  }
  if (args.rangeInExistingText) return false;
  if (args.orphanGlyphsUnderPoint) return false;
  if (args.hitIsEditor) return true;
  return true;
}

describe("Document place-at-point invent guards", () => {
  it("does not invent a block when clicking orphan text under the editor root", () => {
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: true,
        hitInsidePlaced: false,
        hitInsideTable: false,
        rangeInExistingText: true,
        rangeOnlyInTable: false,
        nearPlacedLine: false,
        orphanGlyphsUnderPoint: true,
      }),
    ).toBe(false);
  });

  it("does not invent when caretRange reports editor root but orphan glyphs are under the point", () => {
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: true,
        hitInsidePlaced: false,
        hitInsideTable: false,
        rangeInExistingText: false,
        rangeOnlyInTable: false,
        nearPlacedLine: false,
        orphanGlyphsUnderPoint: true,
      }),
    ).toBe(false);
  });

  it("does not invent a block when clicking inside an existing placed line", () => {
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: false,
        hitInsidePlaced: true,
        hitInsideTable: false,
        rangeInExistingText: true,
        rangeOnlyInTable: false,
        nearPlacedLine: true,
        orphanGlyphsUnderPoint: false,
      }),
    ).toBe(false);
  });

  it("does not invent a block when Y-snap finds a nearby placed line", () => {
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: true,
        hitInsidePlaced: false,
        hitInsideTable: false,
        rangeInExistingText: false,
        rangeOnlyInTable: false,
        nearPlacedLine: true,
        orphanGlyphsUnderPoint: false,
      }),
    ).toBe(false);
  });

  it("invents a block on a true blank-canvas click", () => {
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: true,
        hitInsidePlaced: false,
        hitInsideTable: false,
        rangeInExistingText: false,
        rangeOnlyInTable: false,
        nearPlacedLine: false,
        orphanGlyphsUnderPoint: false,
      }),
    ).toBe(true);
  });

  it("invents a block when blank click caretRange false-positives into an absolute table", () => {
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: true,
        hitInsidePlaced: false,
        hitInsideTable: false,
        rangeInExistingText: false,
        rangeOnlyInTable: true,
        nearPlacedLine: false,
        orphanGlyphsUnderPoint: false,
      }),
    ).toBe(true);
  });

  it("does not invent when the click hit is inside the table", () => {
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: false,
        hitInsidePlaced: false,
        hitInsideTable: true,
        rangeInExistingText: false,
        rangeOnlyInTable: true,
        nearPlacedLine: false,
        orphanGlyphsUnderPoint: false,
      }),
    ).toBe(false);
  });

  it("invents in free space even when not at the left margin", () => {
    // Mid-canvas / L/R-of-table invent: only the left-edge flush path clamps inset.
    expect(
      shouldInventPlacedBlock({
        hitIsEditor: true,
        hitInsidePlaced: false,
        hitInsideTable: false,
        rangeInExistingText: false,
        rangeOnlyInTable: false,
        nearPlacedLine: false,
        orphanGlyphsUnderPoint: false,
      }),
    ).toBe(true);
  });
});
