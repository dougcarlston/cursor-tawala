/** Layout helpers for legacy `table.user` elements in rich-text editors. */

const MIN_COL_PT = 18;
const MIN_ROW_PT = 12;

export function pxToPt(px: number): number {
  return (px * 72) / 96;
}

export function parseCssPt(raw: string | null | undefined): number {
  const value = (raw ?? "").trim();
  if (!value) return 0;
  const pt = value.match(/^([\d.]+)\s*pt$/i);
  if (pt) return Number(pt[1]);
  const px = value.match(/^([\d.]+)\s*px$/i);
  if (px) return pxToPt(Number(px[1]));
  const n = Number.parseFloat(value);
  return Number.isFinite(n) ? n : 0;
}

export function formatPt(value: number): string {
  return `${Math.max(0, Math.round(value * 10) / 10)}pt`;
}

/** Active `table.user` containing the selection, if any. */
export function findActiveUserTable(editor: HTMLElement | null): HTMLTableElement | null {
  if (!editor) return null;
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== editor) {
    if (node instanceof HTMLTableElement && node.classList.contains("user")) return node;
    node = node.parentNode;
  }
  return null;
}

export function tableColumnWidthsPt(table: HTMLTableElement): number[] {
  const row = table.querySelector("tr");
  if (!row) return [];
  return Array.from(row.cells).map((cell) => parseCssPt(cell.style.width));
}

export function tableRowHeightsPt(table: HTMLTableElement): number[] {
  const tbody = table.tBodies[0] ?? table;
  return Array.from(tbody.rows).map((row) => {
    const h = parseCssPt(row.style.height);
    if (h > 0) return h;
    return Math.max(MIN_ROW_PT, row.getBoundingClientRect().height * 0.75);
  });
}

export function setTableColumnWidths(table: HTMLTableElement, widths: number[]): void {
  const rows = table.tBodies[0]?.rows ?? table.rows;
  Array.from(rows).forEach((row) => {
    Array.from(row.cells).forEach((cell, i) => {
      if (widths[i] != null) cell.style.width = formatPt(widths[i]);
    });
  });
  table.style.width = formatPt(widths.reduce((a, b) => a + b, 0));
}

export function setTableRowHeights(table: HTMLTableElement, heights: number[]): void {
  const rows = table.tBodies[0]?.rows ?? table.rows;
  Array.from(rows).forEach((row, i) => {
    if (heights[i] != null) row.style.height = formatPt(heights[i]);
  });
}

/** Read table offset used for drag positioning (relative `left`/`top`, pt). */
export function getTablePositionPt(table: HTMLTableElement): { left: number; top: number } {
  if (table.style.position === "relative" || table.style.left || table.style.top) {
    return {
      left: parseCssPt(table.style.left),
      top: parseCssPt(table.style.top),
    };
  }
  return {
    left: parseCssPt(table.style.marginLeft),
    top: parseCssPt(table.style.marginTop),
  };
}

/** Move table by setting `position: relative` offsets (works with float + side text). */
export function setTablePositionPt(table: HTMLTableElement, left: number, top: number): void {
  table.style.position = "relative";
  table.style.left = formatPt(left);
  table.style.top = formatPt(top);
  table.style.marginLeft = "";
  table.style.marginTop = "";
}

/** Move table by adjusting top/left offsets (pt). */
export function moveTableByPt(table: HTMLTableElement, deltaLeftPt: number, deltaTopPt: number): void {
  const pos = getTablePositionPt(table);
  setTablePositionPt(table, pos.left + deltaLeftPt, pos.top + deltaTopPt);
}

/** Resize overall table width; columns scale proportionally (legacy `setTableColumnWidths`). */
export function resizeTableWidth(table: HTMLTableElement, newTotalPt: number): void {
  const widths = tableColumnWidthsPt(table);
  if (!widths.length) return;
  const oldTotal = widths.reduce((a, b) => a + b, 0) || parseCssPt(table.style.width) || 1;
  const scale = Math.max(widths.length * MIN_COL_PT, newTotalPt) / oldTotal;
  setTableColumnWidths(
    table,
    widths.map((w) => Math.max(MIN_COL_PT, w * scale)),
  );
}

/** Resize overall table height; rows scale proportionally. */
export function resizeTableHeight(table: HTMLTableElement, newTotalPt: number): void {
  const heights = tableRowHeightsPt(table);
  if (!heights.length) return;
  const oldTotal = heights.reduce((a, b) => a + b, 0) || 1;
  const scale = Math.max(heights.length * MIN_ROW_PT, newTotalPt) / oldTotal;
  setTableRowHeights(
    table,
    heights.map((h) => Math.max(MIN_ROW_PT, h * scale)),
  );
}

/** Drag a column divider between `leftIndex` and `leftIndex + 1`. */
export function resizeColumnDivider(
  table: HTMLTableElement,
  leftIndex: number,
  deltaPt: number,
): void {
  const widths = tableColumnWidthsPt(table);
  if (leftIndex < 0 || leftIndex >= widths.length - 1) return;
  const left = widths[leftIndex];
  const right = widths[leftIndex + 1];
  const nextLeft = Math.min(left + right - MIN_COL_PT, Math.max(MIN_COL_PT, left + deltaPt));
  const nextRight = left + right - nextLeft;
  widths[leftIndex] = nextLeft;
  widths[leftIndex + 1] = nextRight;
  setTableColumnWidths(table, widths);
}

/** Drag a row divider between `topIndex` and `topIndex + 1`. */
export function resizeRowDivider(table: HTMLTableElement, topIndex: number, deltaPt: number): void {
  const heights = tableRowHeightsPt(table);
  if (topIndex < 0 || topIndex >= heights.length - 1) return;
  const top = heights[topIndex];
  const bottom = heights[topIndex + 1];
  const nextTop = Math.min(top + bottom - MIN_ROW_PT, Math.max(MIN_ROW_PT, top + deltaPt));
  const nextBottom = top + bottom - nextTop;
  heights[topIndex] = nextTop;
  heights[topIndex + 1] = nextBottom;
  setTableRowHeights(table, heights);
}

export type TableFloatMode = "left" | "none" | "right";

export function getTableFloatMode(table: HTMLTableElement): TableFloatMode {
  const f = (table.style.float || "").toLowerCase();
  if (f === "right") return "right";
  if (f === "left") return "left";
  return "none";
}

export function setTableFloatMode(table: HTMLTableElement, mode: TableFloatMode): void {
  if (mode === "none") {
    table.style.float = "";
    table.style.marginRight = "";
  } else {
    table.style.float = mode;
    table.style.marginRight = mode === "left" ? "12pt" : "";
    if (mode === "right" && !table.style.left && !table.style.top) {
      const pos = getTablePositionPt(table);
      if (pos.left <= 0) setTablePositionPt(table, 12, pos.top);
    }
  }
}

/** Box of the table relative to a scroll container (the overlay parent). */
export function tableBoxInContainer(
  table: HTMLTableElement,
  container: HTMLElement,
): { top: number; left: number; width: number; height: number } {
  const tableR = table.getBoundingClientRect();
  const containerR = container.getBoundingClientRect();
  return {
    top: tableR.top - containerR.top + container.scrollTop,
    left: tableR.left - containerR.left + container.scrollLeft,
    width: tableR.width,
    height: tableR.height,
  };
}

/** @deprecated use tableBoxInContainer with the editor's scroll wrapper */
export function tableBoxInEditor(
  table: HTMLTableElement,
  editor: HTMLElement,
): { top: number; left: number; width: number; height: number } {
  const container = editor.parentElement ?? editor;
  return tableBoxInContainer(table, container);
}
