/**
 * #7 Window resize — reflowDocumentLayout clamps overflow anchors and re-wraps
 * without forcing side-by-side tables under preceding text.
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  alignPlacedTextBlock,
  PLACED_TEXT_CLASS,
  reflowDocumentLayout,
} from "./documentCanvas";
import { formatPt, getAbsolutePositionPt, parseCssPt } from "./tableLayout";

function makeDocEditor(widthPx: number): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.style.width = `${widthPx}px`;
  editor.style.height = "600px";
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: widthPx });
  Object.defineProperty(editor, "clientHeight", { configurable: true, value: 600 });
  document.body.appendChild(editor);
  return editor;
}

function makePlaced(
  editor: HTMLElement,
  left: number,
  top: number,
  text: string,
  widthPt?: number,
): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(left);
  p.style.top = formatPt(top);
  if (widthPt != null) p.style.width = formatPt(widthPt);
  p.textContent = text;
  editor.appendChild(p);
  return p;
}

function makeAbsoluteTable(
  editor: HTMLElement,
  left: number,
  top: number,
  width = 200,
): HTMLTableElement {
  const table = document.createElement("table");
  table.className = "user";
  table.style.position = "absolute";
  table.style.left = formatPt(left);
  table.style.top = formatPt(top);
  table.style.width = formatPt(width);
  table.innerHTML = "<tr><td style='height:40px;width:80px'>a</td><td>b</td></tr>";
  editor.appendChild(table);
  return table;
}

afterEach(() => {
  document.body.replaceChildren();
});

describe("Document resize reflow (#7)", () => {
  it("clamps far-right placed lines when the content box narrows", () => {
    const editor = makeDocEditor(800);
    const block = makePlaced(editor, 520, 20, "Far right line", 80);
    // Narrow the editor (MDI / browser resize).
    Object.defineProperty(editor, "clientWidth", { configurable: true, value: 320 });
    editor.style.width = "320px";

    reflowDocumentLayout(editor);

    const left = getAbsolutePositionPt(block).left;
    // Narrow content ~240pt → clamp near rightEdge−24 (~208pt), not the original 520.
    expect(left).toBeLessThan(520);
    expect(left).toBeLessThanOrEqual(220);
    expect(parseCssPt(block.style.width)).toBeGreaterThan(1);
  });

  it("keeps table beside text (no forced pack-under) after resize reflow", () => {
    const editor = makeDocEditor(800);
    const leftText = makePlaced(editor, 12, 40, "Left", 80);
    const table = makeAbsoluteTable(editor, 220, 40, 200);

    Object.defineProperty(editor, "clientWidth", { configurable: true, value: 700 });
    reflowDocumentLayout(editor);

    expect(getAbsolutePositionPt(leftText).top).toBe(40);
    expect(getAbsolutePositionPt(table).top).toBe(40);
  });

  it("preserves intentional table gap after resize reflow", () => {
    const editor = makeDocEditor(800);
    makePlaced(editor, 12, 0, "Above", 400);
    const table = makeAbsoluteTable(editor, 12, 140, 200);

    Object.defineProperty(editor, "clientWidth", { configurable: true, value: 600 });
    reflowDocumentLayout(editor);

    expect(getAbsolutePositionPt(table).top).toBe(140);
  });

  it("re-applies Center wrap width after narrow resize", () => {
    const editor = makeDocEditor(800);
    const block = makePlaced(editor, 12, 20, "Centered", 400);
    alignPlacedTextBlock(editor, block, "center");

    Object.defineProperty(editor, "clientWidth", { configurable: true, value: 400 });
    reflowDocumentLayout(editor);

    expect(block.style.textAlign).toBe("center");
    expect(parseCssPt(block.style.width)).toBeGreaterThan(100);
    expect(parseCssPt(block.style.left) + parseCssPt(block.style.width)).toBeLessThan(320);
  });
});
