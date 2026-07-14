/**
 * Invent at the canvas left edge must keep a visible caret inset — absolute
 * left:0 clips under MDI/chrome; padding must not be subtracted from click→pt.
 */
import { describe, expect, it } from "vitest";
import {
  clientPointToEditorPt,
  DOC_LINE_MARGIN_PT,
  insertPlacedTextBlock,
  inventPlacedLeftPt,
  PLACED_TEXT_CLASS,
} from "./documentCanvas";
import { parseCssPt, pxToPt } from "./tableLayout";

function stubEditorRect(editor: HTMLElement): void {
  editor.getBoundingClientRect = () =>
    ({
      x: 100,
      y: 50,
      top: 50,
      left: 100,
      bottom: 450,
      right: 700,
      width: 600,
      height: 400,
      toJSON() {
        return {};
      },
    }) as DOMRect;
}

describe("Document invent left inset", () => {
  it("maps clientX to absolute left without subtracting editor padding", () => {
    const editor = document.createElement("div");
    editor.style.padding = "10px 12px";
    document.body.appendChild(editor);
    stubEditorRect(editor);

    // Click 12px into the editor (legacy “content” start). Must not become ~0.
    const { left } = clientPointToEditorPt(editor, 100 + 12, 50 + 40);
    expect(left).toBeCloseTo(pxToPt(12), 5);
    expect(left).toBeGreaterThan(DOC_LINE_MARGIN_PT - 0.01);

    editor.remove();
  });

  it("clamps left-edge invent to DOC_LINE_MARGIN_PT so caret is not at 0", () => {
    const editor = document.createElement("div");
    editor.className = "rich-surface";
    document.body.appendChild(editor);
    stubEditorRect(editor);

    // Click almost on the left border of the surface.
    const block = insertPlacedTextBlock(editor, 100 + 1, 50 + 80);
    expect(block.classList.contains(PLACED_TEXT_CLASS)).toBe(true);
    expect(parseCssPt(block.style.left)).toBeCloseTo(DOC_LINE_MARGIN_PT, 5);
    expect(parseCssPt(block.style.left)).toBeGreaterThan(0);

    editor.remove();
  });

  it("keeps mid-canvas invent X (does not force everything to the left margin)", () => {
    const editor = document.createElement("div");
    editor.className = "rich-surface";
    document.body.appendChild(editor);
    stubEditorRect(editor);

    const clickX = 100 + 180;
    const block = insertPlacedTextBlock(editor, clickX, 50 + 120);
    expect(parseCssPt(block.style.left)).toBeCloseTo(pxToPt(180), 5);

    editor.remove();
  });

  it("inventPlacedLeftPt only insets when flush left; otherwise keeps free-space X", () => {
    expect(inventPlacedLeftPt(0)).toBeCloseTo(DOC_LINE_MARGIN_PT, 5);
    expect(inventPlacedLeftPt(DOC_LINE_MARGIN_PT / 2)).toBeCloseTo(DOC_LINE_MARGIN_PT, 5);
    expect(inventPlacedLeftPt(120)).toBe(120);
  });
});
