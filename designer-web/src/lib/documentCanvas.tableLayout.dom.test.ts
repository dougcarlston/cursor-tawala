/**
 * Document tables: absolute layout with collision-aware packing.
 * Same-column overlays clear; side-by-side and intentional drag gaps keep X/Y.
 */
import { describe, expect, it } from "vitest";
import {
  ensureDocumentTableLayout,
  ensurePlacedBlockWrapWidth,
  listDocumentLayoutItemsSorted,
  normalizeDocumentUserTables,
  PLACED_TEXT_CLASS,
  pruneEmptyPlacedTextBlocks,
  reflowAllPlacedLines,
  resolveDocumentLayoutCollisions,
} from "./documentCanvas";
import { formatPt, getAbsolutePositionPt, parseCssPt } from "./tableLayout";

function makeDocEditor(): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.style.width = "800px";
  editor.style.height = "600px";
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: 800 });
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

function makeFloatTable(editor: HTMLElement): HTMLTableElement {
  const table = document.createElement("table");
  table.className = "user user-border-2";
  table.style.position = "relative";
  table.style.float = "left";
  table.style.left = "12pt";
  table.style.top = "12pt";
  table.style.width = "200pt";
  const tbody = document.createElement("tbody");
  for (let r = 0; r < 3; r++) {
    const tr = document.createElement("tr");
    tr.style.height = "20px";
    for (let c = 0; c < 2; c++) {
      const td = document.createElement("td");
      td.textContent = `${r},${c}`;
      td.style.height = "20px";
      tr.appendChild(td);
    }
    tbody.appendChild(tr);
  }
  table.appendChild(tbody);
  editor.appendChild(table);
  return table;
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
    "<tr style='height:20px'><td style='height:20px'>a</td><td style='height:20px'>b</td></tr>" +
    "<tr style='height:20px'><td style='height:20px'>c</td><td style='height:20px'>d</td></tr>";
  editor.appendChild(table);
  return table;
}

describe("Document table layout (no overlay)", () => {
  it("normalize converts float/relative table to absolute", () => {
    const editor = makeDocEditor();
    makePlaced(editor, 12, 0, "Above");
    const table = makeFloatTable(editor);
    makePlaced(editor, 12, 80, "Below");

    expect(normalizeDocumentUserTables(editor)).toBe(true);
    expect(table.style.float).toBe("");
    expect(table.style.position).toBe("absolute");
    editor.remove();
  });

  it("same-column text below a table is pushed down so tops do not overlay", () => {
    const editor = makeDocEditor();
    const above = makePlaced(editor, 12, 0, "Can go anywhere", 400);
    const table = makeAbsoluteTable(editor, 12, 10, 200);
    const below = makePlaced(editor, 12, 12, "Must not overlay", 400);

    ensureDocumentTableLayout(editor);

    const items = listDocumentLayoutItemsSorted(editor);
    expect(items).toContain(above);
    expect(items).toContain(table);
    expect(items).toContain(below);

    // Same-column collision: below must clear the table box.
    expect(getAbsolutePositionPt(below).top).toBeGreaterThan(getAbsolutePositionPt(table).top);
    editor.remove();
  });

  it("keeps side-by-side text left of a table (does not snap under)", () => {
    const editor = makeDocEditor();
    const table = makeAbsoluteTable(editor, 220, 40, 200);
    const leftText = makePlaced(editor, 12, 40, "Left of table", 80);

    ensurePlacedBlockWrapWidth(editor, leftText);
    resolveDocumentLayoutCollisions(editor);

    expect(getAbsolutePositionPt(leftText).top).toBe(40);
    expect(getAbsolutePositionPt(table).top).toBe(40);
    // Wrap must stop before the table so L/R stay separate columns.
    const w = parseCssPt(leftText.style.width);
    expect(w).toBeGreaterThan(1);
    expect(12 + w).toBeLessThanOrEqual(220);
    editor.remove();
  });

  it("keeps side-by-side text right of a table (does not snap under)", () => {
    const editor = makeDocEditor();
    const table = makeAbsoluteTable(editor, 12, 40, 200);
    const rightText = makePlaced(editor, 230, 40, "Right of table", 120);

    resolveDocumentLayoutCollisions(editor);

    expect(getAbsolutePositionPt(rightText).top).toBe(40);
    expect(getAbsolutePositionPt(table).top).toBe(40);
    editor.remove();
  });

  it("preserves intentional table drag gap below text (no snap-under pack)", () => {
    const editor = makeDocEditor();
    makePlaced(editor, 12, 0, "Above with gap", 400);
    const table = makeAbsoluteTable(editor, 12, 120, 200);

    reflowAllPlacedLines(editor);

    expect(getAbsolutePositionPt(table).top).toBe(120);
    editor.remove();
  });

  it("hoists a table nested inside a placed line onto the editor", () => {
    const editor = makeDocEditor();
    const host = makePlaced(editor, 12, 100, "host");
    const table = document.createElement("table");
    table.className = "user";
    table.style.float = "left";
    table.style.position = "relative";
    table.innerHTML = "<tr><td>x</td></tr>";
    host.appendChild(table);

    ensureDocumentTableLayout(editor);

    expect(table.parentElement).toBe(editor);
    expect(table.style.position).toBe("absolute");
    expect(getAbsolutePositionPt(table).top).toBeGreaterThanOrEqual(100);
    editor.remove();
  });

  it("prunes unused empty invent without clearing a table-cell selection", () => {
    const editor = makeDocEditor();
    const husk = makePlaced(editor, 12, 0, "");
    husk.innerHTML = "<br>";
    const table = makeAbsoluteTable(editor, 12, 80, 200);
    const cell = table.querySelector("td")!;

    const range = document.createRange();
    range.selectNodeContents(cell);
    range.collapse(true);
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);

    const removed = pruneEmptyPlacedTextBlocks(editor, { restoreFocus: false });
    expect(removed).toBe(true);
    expect(editor.contains(husk)).toBe(false);
    expect(sel.rangeCount).toBeGreaterThan(0);
    expect(cell.contains(sel.anchorNode!) || sel.anchorNode === cell).toBe(true);

    editor.remove();
  });
});
