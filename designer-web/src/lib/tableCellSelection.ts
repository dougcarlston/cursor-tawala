/**
 * Multi-cell drag selection for `table.user` in Document / Form Text editors.
 * Rectangular highlight drives Bold/Italic/Underline/color/face/size/align.
 */

export const TABLE_CELL_SELECTED_CLASS = "table-cell-selected";

export type TableBorderStyle = "border1" | "border2" | "none";

export const TABLE_BORDER_CLASS: Record<TableBorderStyle, string> = {
  border1: "user-border-1",
  border2: "user-border-2",
  none: "user-border-none",
};

const ALL_BORDER_CLASSES = Object.values(TABLE_BORDER_CLASS);

type CellDragState = {
  table: HTMLTableElement;
  anchor: HTMLTableCellElement;
};

const dragByEditor = new WeakMap<HTMLElement, CellDragState | null>();
const selectedByEditor = new WeakMap<HTMLElement, HTMLTableCellElement[]>();

function cellIndexInRow(cell: HTMLTableCellElement): number {
  return cell.cellIndex;
}

function rowIndexInTable(cell: HTMLTableCellElement): number {
  const row = cell.parentElement;
  if (!(row instanceof HTMLTableRowElement)) return -1;
  const table = cell.closest("table");
  if (!(table instanceof HTMLTableElement)) return -1;
  return row.rowIndex;
}

/** Cells in the inclusive rectangle between two cells of the same table. */
export function cellsInRectangle(
  a: HTMLTableCellElement,
  b: HTMLTableCellElement,
): HTMLTableCellElement[] {
  const tableA = a.closest("table");
  const tableB = b.closest("table");
  if (!(tableA instanceof HTMLTableElement) || tableA !== tableB) return [a];

  const r0 = Math.min(rowIndexInTable(a), rowIndexInTable(b));
  const r1 = Math.max(rowIndexInTable(a), rowIndexInTable(b));
  const c0 = Math.min(cellIndexInRow(a), cellIndexInRow(b));
  const c1 = Math.max(cellIndexInRow(a), cellIndexInRow(b));
  if (r0 < 0 || c0 < 0) return [a];

  const out: HTMLTableCellElement[] = [];
  for (let r = r0; r <= r1; r++) {
    const row = tableA.rows[r];
    if (!row) continue;
    for (let c = c0; c <= c1; c++) {
      const cell = row.cells[c];
      if (cell) out.push(cell);
    }
  }
  return out.length ? out : [a];
}

function paintSelectedCells(editor: HTMLElement, cells: HTMLTableCellElement[]): void {
  clearSelectedCellPaint(editor);
  for (const cell of cells) {
    cell.classList.add(TABLE_CELL_SELECTED_CLASS);
  }
  selectedByEditor.set(editor, cells);
}

function clearSelectedCellPaint(editor: HTMLElement): void {
  editor.querySelectorAll(`.${TABLE_CELL_SELECTED_CLASS}`).forEach((node) => {
    node.classList.remove(TABLE_CELL_SELECTED_CLASS);
  });
}

/** Clear multi-cell highlight for an editor. */
export function clearTableCellSelection(editor: HTMLElement): void {
  clearSelectedCellPaint(editor);
  selectedByEditor.set(editor, []);
  dragByEditor.set(editor, null);
}

/** Currently highlighted cells (empty when none / single native caret). */
export function getSelectedTableCells(editor: HTMLElement): HTMLTableCellElement[] {
  const cells = selectedByEditor.get(editor) ?? [];
  return cells.filter((c) => editor.contains(c));
}

/** True when caret or multi-cell selection sits inside a `table.user`. */
export function selectionInsideUserTable(root: HTMLElement | null): boolean {
  if (!root) return false;
  const multi = getSelectedTableCells(root);
  if (multi.length > 0) return true;
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== root) {
    if (node instanceof HTMLElement && node.closest("table.user")) return true;
    node = node.parentNode;
  }
  return false;
}

/**
 * Begin or update multi-cell drag from a pointer event target.
 * Returns true when the event was consumed as a table cell selection gesture.
 */
export function handleTableCellPointerDown(
  editor: HTMLElement,
  target: EventTarget | null,
  button: number,
): boolean {
  if (button !== 0) return false;
  if (!(target instanceof Element)) return false;
  const cell = target.closest("td, th");
  if (!(cell instanceof HTMLTableCellElement) || !editor.contains(cell)) {
    clearTableCellSelection(editor);
    return false;
  }
  const table = cell.closest("table.user");
  if (!(table instanceof HTMLTableElement)) {
    clearTableCellSelection(editor);
    return false;
  }
  dragByEditor.set(editor, { table, anchor: cell });
  paintSelectedCells(editor, [cell]);
  return true;
}

/** Extend rectangular cell selection while the primary button is held. */
export function handleTableCellPointerMove(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  buttons: number,
): boolean {
  if ((buttons & 1) === 0) {
    dragByEditor.set(editor, null);
    return false;
  }
  const drag = dragByEditor.get(editor);
  if (!drag || !editor.contains(drag.table)) return false;

  const hit = document.elementFromPoint(clientX, clientY);
  const cell = hit instanceof Element ? hit.closest("td, th") : null;
  if (!(cell instanceof HTMLTableCellElement) || !drag.table.contains(cell)) {
    return true;
  }
  paintSelectedCells(editor, cellsInRectangle(drag.anchor, cell));
  return true;
}

export function handleTableCellPointerUp(editor: HTMLElement): void {
  dragByEditor.set(editor, null);
}

/** Apply a border preset to the table containing the caret / cell selection. */
export function applyTableBorderStyle(
  editor: HTMLElement,
  style: TableBorderStyle,
): HTMLTableElement | null {
  const cells = getSelectedTableCells(editor);
  let table: HTMLTableElement | null = null;
  if (cells.length) {
    const t = cells[0].closest("table");
    if (t instanceof HTMLTableElement) table = t;
  }
  if (!table) {
    const sel = window.getSelection();
    if (sel?.rangeCount) {
      let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
      if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
      while (node && node !== editor) {
        if (node instanceof HTMLTableElement && node.classList.contains("user")) {
          table = node;
          break;
        }
        node = node.parentNode;
      }
    }
  }
  if (!table || !editor.contains(table)) return null;

  for (const cls of ALL_BORDER_CLASSES) {
    table.classList.remove(cls);
  }
  table.classList.add(TABLE_BORDER_CLASS[style]);
  // Default insert uses Border 1 look via CSS for bare `table.user`.
  return table;
}

export function readTableBorderStyle(table: HTMLTableElement): TableBorderStyle {
  if (table.classList.contains(TABLE_BORDER_CLASS.none)) return "none";
  if (table.classList.contains(TABLE_BORDER_CLASS.border2)) return "border2";
  return "border1";
}

/**
 * Run `fn` once per selected cell (or the single caret cell), selecting each cell's
 * contents so execCommand / inline formats apply to all highlighted cells.
 */
export function forEachFormatTargetCell(
  editor: HTMLElement,
  fn: (cell: HTMLTableCellElement) => void,
): boolean {
  let cells = getSelectedTableCells(editor);
  if (!cells.length) {
    const sel = window.getSelection();
    if (sel?.rangeCount) {
      let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
      if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
      while (node && node !== editor) {
        if (node instanceof HTMLTableCellElement) {
          cells = [node];
          break;
        }
        node = node.parentNode;
      }
    }
  }
  if (cells.length <= 1) return false;

  const sel = window.getSelection();
  for (const cell of cells) {
    if (sel) {
      const range = document.createRange();
      range.selectNodeContents(cell);
      sel.removeAllRanges();
      sel.addRange(range);
    }
    fn(cell);
  }
  return true;
}

/** Flat list of td/th in document order for a `table.user`. */
export function listUserTableCells(table: HTMLTableElement): HTMLTableCellElement[] {
  const out: HTMLTableCellElement[] = [];
  for (let r = 0; r < table.rows.length; r++) {
    const row = table.rows[r];
    for (let c = 0; c < row.cells.length; c++) {
      out.push(row.cells[c]);
    }
  }
  return out;
}

/** Cell containing the caret (or the first multi-cell highlight cell). */
export function getCaretTableCell(editor: HTMLElement): HTMLTableCellElement | null {
  const multi = getSelectedTableCells(editor);
  if (multi.length) return multi[0];
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== editor) {
    if (node instanceof HTMLTableCellElement) {
      const table = node.closest("table.user");
      if (table && editor.contains(table)) return node;
    }
    node = node.parentNode;
  }
  return null;
}

/**
 * Move caret to the start of `cell` and update the multi-cell highlight to that cell.
 */
export function focusTableCell(editor: HTMLElement, cell: HTMLTableCellElement): void {
  paintSelectedCells(editor, [cell]);
  const sel = window.getSelection();
  if (!sel) return;
  const range = document.createRange();
  range.selectNodeContents(cell);
  range.collapse(true);
  sel.removeAllRanges();
  sel.addRange(range);
}

/**
 * Tab / Shift+Tab cell navigation inside `table.user`.
 * Advances to the next/previous cell (row wrap). At the last/first cell, stays put
 * (does not leave the table or insert a row). Returns true when Tab was consumed.
 */
export function navigateTableCellOnTab(
  editor: HTMLElement,
  shiftKey: boolean,
): boolean {
  const cell = getCaretTableCell(editor);
  if (!cell || !editor.contains(cell)) return false;
  const table = cell.closest("table.user");
  if (!(table instanceof HTMLTableElement)) return false;

  const cells = listUserTableCells(table);
  if (!cells.length) return false;
  const idx = cells.indexOf(cell);
  if (idx < 0) return false;

  const nextIdx = shiftKey ? idx - 1 : idx + 1;
  if (nextIdx < 0 || nextIdx >= cells.length) {
    // Stay in the edge cell — keep focus inside the table.
    focusTableCell(editor, cell);
    return true;
  }
  focusTableCell(editor, cells[nextIdx]);
  return true;
}

/** Apply text-align to highlighted cells only (never the whole table / row). */
export function applyAlignToSelectedTableCells(
  editor: HTMLElement,
  align: "left" | "center" | "right" | "justify",
): boolean {
  const cells = getSelectedTableCells(editor);
  if (!cells.length) return false;
  for (const cell of cells) {
    cell.style.textAlign = align;
    for (const child of Array.from(cell.children)) {
      if (child instanceof HTMLElement) {
        child.style.textAlign = align;
      }
    }
  }
  return true;
}
