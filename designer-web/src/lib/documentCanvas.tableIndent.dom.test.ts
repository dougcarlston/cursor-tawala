/**
 * Document Indent/Outdent must move absolute user tables with multi-selected
 * placed lines (Potluck Details Totals alignment).
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  adjustPlacedTextIndent,
  adjustUserTableIndent,
  DOC_INDENT_STEP_PT,
  getDocumentContentMetrics,
  listPlacedBlocksInSelection,
  listUserTablesInSelection,
  PLACED_TEXT_CLASS,
  reflowAllPlacedLines,
} from "./documentCanvas";
import { formatPt, getAbsolutePositionPt, parseCssPt } from "./tableLayout";

function makeDocEditor(widthPx = 800): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.style.width = `${widthPx}px`;
  editor.style.height = "600px";
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: widthPx });
  Object.defineProperty(editor, "clientHeight", { configurable: true, value: 600 });
  document.body.appendChild(editor);
  return editor;
}

function makePlaced(editor: HTMLElement, left: number, top: number, text: string): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(left);
  p.style.top = formatPt(top);
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
  table.innerHTML =
    "<tr style='height:20px'><td style='height:20px'>Adults</td><td style='height:20px'>Kids</td></tr>" +
    "<tr style='height:20px'><td style='height:20px'>1</td><td style='height:20px'>2</td></tr>";
  editor.appendChild(table);
  return table;
}

/** Select from start of `from` through end of `to` (both under editor). */
function selectAcross(from: Node, to: Node): void {
  const range = document.createRange();
  range.setStart(from, 0);
  range.setEnd(to, to.childNodes.length || (to.textContent?.length ?? 0));
  const sel = window.getSelection()!;
  sel.removeAllRanges();
  sel.addRange(range);
}

afterEach(() => {
  document.body.replaceChildren();
});

describe("Document table Indent/Outdent", () => {
  it("listUserTablesInSelection includes table when highlight spans lines around it", () => {
    const editor = makeDocEditor();
    const { marginPt } = getDocumentContentMetrics(editor);
    const above = makePlaced(editor, marginPt, 20, "Who is coming:");
    const table = makeAbsoluteTable(editor, marginPt, 50);
    const below = makePlaced(editor, marginPt, 120, "Who can't come:");

    selectAcross(above, below);

    expect(listPlacedBlocksInSelection(editor)).toEqual(
      expect.arrayContaining([above, below]),
    );
    expect(listUserTablesInSelection(editor)).toEqual([table]);
  });

  it("collapsed caret in a cell does not list the table for Indent", () => {
    const editor = makeDocEditor();
    const { marginPt } = getDocumentContentMetrics(editor);
    const table = makeAbsoluteTable(editor, marginPt, 40);
    const cell = table.querySelector("td")!;
    const range = document.createRange();
    range.selectNodeContents(cell);
    range.collapse(true);
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);

    expect(listUserTablesInSelection(editor)).toEqual([]);
  });

  it("Indent steps table left with surrounding placed lines", () => {
    const editor = makeDocEditor();
    const { marginPt } = getDocumentContentMetrics(editor);
    const above = makePlaced(editor, marginPt, 20, "Who is coming:");
    const table = makeAbsoluteTable(editor, marginPt, 50);
    const below = makePlaced(editor, marginPt, 120, "Who can't come:");

    selectAcross(above, below);
    const blocks = listPlacedBlocksInSelection(editor);
    const tables = listUserTablesInSelection(editor);
    expect(tables).toEqual([table]);

    for (const block of blocks) adjustPlacedTextIndent(editor, block, 1);
    for (const t of tables) adjustUserTableIndent(editor, t, 1);
    reflowAllPlacedLines(editor);

    const expected = marginPt + DOC_INDENT_STEP_PT;
    expect(parseCssPt(above.style.left)).toBeCloseTo(expected, 0);
    expect(getAbsolutePositionPt(table).left).toBeCloseTo(expected, 0);
    expect(parseCssPt(below.style.left)).toBeCloseTo(expected, 0);
    expect(table.dataset.docIndent).toBe("1");
  });

  it("Outdent clamps table back to the left margin", () => {
    const editor = makeDocEditor();
    const { marginPt } = getDocumentContentMetrics(editor);
    const table = makeAbsoluteTable(editor, marginPt + DOC_INDENT_STEP_PT, 40);
    table.dataset.docIndent = "1";

    adjustUserTableIndent(editor, table, -1);
    expect(getAbsolutePositionPt(table).left).toBeCloseTo(marginPt, 0);
    expect(table.dataset.docIndent).toBe("0");

    adjustUserTableIndent(editor, table, -1);
    expect(getAbsolutePositionPt(table).left).toBeCloseTo(marginPt, 0);
  });
});
