/**
 * Table cell rectangle selection + border class persistence.
 */
import { describe, expect, it } from "vitest";
import {
  applyAlignToSelectedTableCells,
  applyTableBorderStyle,
  cellsInRectangle,
  clearTableCellSelection,
  focusTableCell,
  getSelectedTableCells,
  handleTableCellPointerDown,
  handleTableCellPointerMove,
  navigateTableCellOnTab,
  readTableBorderStyle,
  TABLE_BORDER_CLASS,
  TABLE_CELL_SELECTED_CLASS,
} from "./tableCellSelection";

function makeTable(rows: number, cols: number): {
  editor: HTMLElement;
  table: HTMLTableElement;
  cell: (r: number, c: number) => HTMLTableCellElement;
} {
  const editor = document.createElement("div");
  editor.contentEditable = "true";
  const table = document.createElement("table");
  table.className = "user user-border-1";
  const tbody = document.createElement("tbody");
  for (let r = 0; r < rows; r++) {
    const tr = document.createElement("tr");
    for (let c = 0; c < cols; c++) {
      const td = document.createElement("td");
      td.textContent = `${r},${c}`;
      tr.appendChild(td);
    }
    tbody.appendChild(tr);
  }
  table.appendChild(tbody);
  editor.appendChild(table);
  document.body.appendChild(editor);
  return {
    editor,
    table,
    cell: (r, c) => table.rows[r].cells[c],
  };
}

describe("tableCellSelection", () => {
  it("builds a rectangle of cells between two corners", () => {
    const { editor, cell } = makeTable(3, 3);
    const cells = cellsInRectangle(cell(0, 0), cell(1, 2));
    expect(cells).toHaveLength(6);
    expect(cells.map((c) => c.textContent)).toEqual([
      "0,0",
      "0,1",
      "0,2",
      "1,0",
      "1,1",
      "1,2",
    ]);
    editor.remove();
  });

  it("highlights a cell on pointer down inside a table", () => {
    const { editor, cell } = makeTable(2, 2);
    const ok = handleTableCellPointerDown(editor, cell(1, 1), 0);
    expect(ok).toBe(true);
    expect(cell(1, 1).classList.contains(TABLE_CELL_SELECTED_CLASS)).toBe(true);
    expect(getSelectedTableCells(editor)).toHaveLength(1);
    clearTableCellSelection(editor);
    expect(getSelectedTableCells(editor)).toHaveLength(0);
    editor.remove();
  });

  it("applies Border 1 / Border 2 / No Border classes on the table", () => {
    const { editor, table, cell } = makeTable(2, 2);
    handleTableCellPointerDown(editor, cell(0, 0), 0);

    applyTableBorderStyle(editor, "border2");
    expect(table.classList.contains(TABLE_BORDER_CLASS.border2)).toBe(true);
    expect(readTableBorderStyle(table)).toBe("border2");

    applyTableBorderStyle(editor, "none");
    expect(table.classList.contains(TABLE_BORDER_CLASS.none)).toBe(true);
    expect(table.classList.contains(TABLE_BORDER_CLASS.border2)).toBe(false);
    expect(readTableBorderStyle(table)).toBe("none");

    applyTableBorderStyle(editor, "border1");
    expect(readTableBorderStyle(table)).toBe("border1");
    editor.remove();
  });

  it("Tab / Shift+Tab moves caret between cells and stays at edges", () => {
    const { editor, cell } = makeTable(2, 2);
    focusTableCell(editor, cell(0, 0));

    expect(navigateTableCellOnTab(editor, false)).toBe(true);
    expect(getSelectedTableCells(editor)).toEqual([cell(0, 1)]);

    expect(navigateTableCellOnTab(editor, false)).toBe(true);
    expect(getSelectedTableCells(editor)).toEqual([cell(1, 0)]);

    expect(navigateTableCellOnTab(editor, false)).toBe(true);
    expect(getSelectedTableCells(editor)).toEqual([cell(1, 1)]);

    // Last cell: stay (do not leave table)
    expect(navigateTableCellOnTab(editor, false)).toBe(true);
    expect(getSelectedTableCells(editor)).toEqual([cell(1, 1)]);

    expect(navigateTableCellOnTab(editor, true)).toBe(true);
    expect(getSelectedTableCells(editor)).toEqual([cell(1, 0)]);

    focusTableCell(editor, cell(0, 0));
    expect(navigateTableCellOnTab(editor, true)).toBe(true);
    expect(getSelectedTableCells(editor)).toEqual([cell(0, 0)]);

    editor.remove();
  });

  it("align applies only to highlighted cells, not other cells", () => {
    const { editor, cell } = makeTable(2, 2);
    handleTableCellPointerDown(editor, cell(0, 0), 0);
    const original = document.elementFromPoint;
    document.elementFromPoint = () => cell(0, 1);
    handleTableCellPointerMove(editor, 10, 10, 1);
    document.elementFromPoint = original;

    expect(getSelectedTableCells(editor)).toHaveLength(2);
    expect(applyAlignToSelectedTableCells(editor, "center")).toBe(true);
    expect(cell(0, 0).style.textAlign).toBe("center");
    expect(cell(0, 1).style.textAlign).toBe("center");
    expect(cell(1, 0).style.textAlign).toBe("");
    expect(cell(1, 1).style.textAlign).toBe("");
    editor.remove();
  });
});
