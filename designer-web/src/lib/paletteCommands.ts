/**
 * Formatting Palette command execution (row 2 → active rich-text editor).
 *
 * The palette lives in the app shell; the focused editor registers a `PaletteEditorHandle`
 * (see `formattingPaletteContext`). These helpers apply `document.execCommand` formatting to
 * that editor's selection, then commit the result back to the store.
 *
 * Selection handling: plain buttons keep the editor's selection alive by calling
 * `preventDefault` on mousedown, so the caret never leaves the editor. Dropdowns / the color
 * picker DO take focus, so they save the selection first and we restore it here before applying.
 */

import {
  getActivePaletteEditor,
  getFormattingFocusState,
  selectionCursorInTable,
  setFormattingFocus,
  type PaletteEditorHandle,
} from "./formattingPaletteContext";
import { parseCssPt, pxToPt } from "./tableLayout";

export interface PaletteActiveState {
  bold: boolean;
  italic: boolean;
  underline: boolean;
  align: "left" | "center" | "right" | "justify";
  fontFace: string;
  fontSize: string;
}

const WEB_SAFE_FACES = [
  "Arial",
  "Arial Black",
  "Comic Sans MS",
  "Courier New",
  "Georgia",
  "Impact",
  "Tahoma",
  "Times New Roman",
  "Trebuchet MS",
  "Verdana",
] as const;

const FONT_SIZE_PT = [8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72] as const;

const LEGACY_SIZE_TO_PT: Record<string, number> = {
  "1": 8,
  "2": 10,
  "3": 12,
  "4": 14,
  "5": 18,
  "6": 24,
  "7": 36,
};

const paletteActiveListeners = new Set<() => void>();
let cachedPaletteActiveState: PaletteActiveState | null = null;

function paletteStatesEqual(a: PaletteActiveState, b: PaletteActiveState): boolean {
  return (
    a.bold === b.bold &&
    a.italic === b.italic &&
    a.underline === b.underline &&
    a.align === b.align &&
    a.fontFace === b.fontFace &&
    a.fontSize === b.fontSize
  );
}

/** Stable snapshot for `useSyncExternalStore` — must reuse the same object reference when values are unchanged. */
export function getPaletteActiveStateSnapshot(enabled: boolean): PaletteActiveState | null {
  if (!enabled) {
    cachedPaletteActiveState = null;
    return null;
  }
  const next = readPaletteActiveState();
  if (cachedPaletteActiveState && paletteStatesEqual(cachedPaletteActiveState, next)) {
    return cachedPaletteActiveState;
  }
  cachedPaletteActiveState = next;
  return next;
}

export function subscribePaletteActiveState(listener: () => void): () => void {
  paletteActiveListeners.add(listener);
  if (paletteActiveListeners.size === 1) {
    document.addEventListener("selectionchange", emitPaletteActiveState);
  }
  return () => {
    paletteActiveListeners.delete(listener);
    if (paletteActiveListeners.size === 0) {
      document.removeEventListener("selectionchange", emitPaletteActiveState);
    }
  };
}

export function emitPaletteActiveState(): void {
  cachedPaletteActiveState = null;
  paletteActiveListeners.forEach((cb) => cb());
}

function matchFontFace(raw: string): string {
  const first = raw.replace(/['"]/g, "").split(",")[0]?.trim().toLowerCase() ?? "";
  if (!first) return "Default Font";
  for (const face of WEB_SAFE_FACES) {
    if (face.toLowerCase() === first) return face;
  }
  return "Default Font";
}

function readSelectionFontSizePt(): number {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return 0;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  const handle = getActivePaletteEditor();
  if (!(node instanceof HTMLElement) || !handle?.el.contains(node)) return 0;

  let el: HTMLElement | null = node;
  while (el && el !== handle.el) {
    if (el.style.fontSize) return parseCssPt(el.style.fontSize);
    el = el.parentElement;
  }

  try {
    const legacy = document.queryCommandValue("fontSize");
    if (legacy && legacy !== "3" && LEGACY_SIZE_TO_PT[legacy]) {
      return LEGACY_SIZE_TO_PT[legacy];
    }
  } catch {
    /* ignore */
  }

  const px = Number.parseFloat(getComputedStyle(node).fontSize);
  return Number.isFinite(px) ? pxToPt(px) : 0;
}

function readFontSizeLabel(): string {
  const pt = readSelectionFontSizePt();
  if (pt <= 0) return "Default Size";
  let best: number = FONT_SIZE_PT[0];
  let bestDiff = Infinity;
  for (const size of FONT_SIZE_PT) {
    const diff = Math.abs(size - pt);
    if (diff < bestDiff) {
      bestDiff = diff;
      best = size;
    }
  }
  return bestDiff <= 1.5 ? String(best) : "Default Size";
}

function readFontFaceLabel(): string {
  try {
    const raw = document.queryCommandValue("fontName");
    if (raw) return matchFontFace(String(raw));
  } catch {
    /* ignore */
  }
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return "Default Font";
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  if (node instanceof HTMLElement) {
    const family = getComputedStyle(node).fontFamily;
    if (family) return matchFontFace(family);
  }
  return "Default Font";
}

const ALIGN_COMMAND: Record<PaletteActiveState["align"], string> = {
  left: "justifyLeft",
  center: "justifyCenter",
  right: "justifyRight",
  justify: "justifyFull",
};

function selectionInside(el: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  return el.contains(sel.getRangeAt(0).commonAncestorContainer);
}

function refreshCursorInTable(handle: PaletteEditorHandle): void {
  const state = getFormattingFocusState();
  if (state.kind === "none" || state.kind === "heading") return;
  setFormattingFocus({
    kind: state.kind,
    cursorInTable: selectionCursorInTable(handle.el),
  });
}

/** Focus the active editor, make sure a selection lives inside it, then run `fn` and commit. */
function withEditor(fn: (handle: PaletteEditorHandle) => void, styleWithCss = true): void {
  const handle = getActivePaletteEditor();
  if (!handle) return;
  handle.el.focus();
  if (!selectionInside(handle.el)) handle.restoreSelection();
  try {
    document.execCommand("styleWithCSS", false, String(styleWithCss));
  } catch {
    /* not supported — ignore */
  }
  fn(handle);
  handle.commit();
  refreshCursorInTable(handle);
  emitPaletteActiveState();
}

function parentElement(node: Node | null): HTMLElement | null {
  const parent = node?.parentNode;
  return parent instanceof HTMLElement ? parent : null;
}

function getSelectedRow(root: HTMLElement): HTMLTableRowElement | null {
  const cell = getSelectedCell(root);
  const row = cell?.parentElement;
  return row instanceof HTMLTableRowElement ? row : null;
}

/** Table cell containing the current selection, if any. */
function getSelectedCell(root: HTMLElement): HTMLTableCellElement | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== root) {
    if (node instanceof HTMLTableCellElement) return node;
    node = node.parentNode;
  }
  return null;
}

function editorIsEmpty(root: HTMLElement): boolean {
  const text = root.textContent?.replace(/\u00a0|\u200b/g, "").trim() ?? "";
  return text.length === 0 && !root.querySelector("table");
}

/** True when the caret sits at the very start of the editor (no text before it). */
function caretAtEditorStart(root: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return true;
  const range = sel.getRangeAt(0);
  const probe = document.createRange();
  probe.selectNodeContents(root);
  probe.setEnd(range.startContainer, range.startOffset);
  return probe.toString().length === 0;
}

function insertBlankParagraph(): HTMLParagraphElement {
  const p = document.createElement("p");
  p.innerHTML = "<br>";
  return p;
}

/** Legacy table markup — `BrowserControl.InsertTable` + `class="user"`. */
function buildLegacyTableElement(widthPt: number, rows: number, columns: number): HTMLTableElement {
  const columnWidthPt = Math.max(1, Math.floor(widthPt / columns));
  const table = document.createElement("table");
  table.className = "user";
  table.style.position = "relative";
  table.style.width = `${widthPt}pt`;
  table.style.borderCollapse = "collapse";
  table.style.float = "left";
  table.style.marginRight = "12pt";
  table.style.left = "12pt";
  table.style.top = "12pt";
  table.style.marginBottom = "12pt";

  const tbody = document.createElement("tbody");
  for (let r = 0; r < rows; r++) {
    const tr = document.createElement("tr");
    tr.style.height = "12pt";
    for (let c = 0; c < columns; c++) {
      const td = document.createElement("td");
      td.style.width = `${columnWidthPt}pt`;
      td.innerHTML = "&nbsp;";
      tr.appendChild(td);
    }
    tbody.appendChild(tr);
  }
  table.appendChild(tbody);
  return table;
}

export interface InsertTableOptions {
  widthInches: number;
  rows: number;
  columns: number;
}

/** Insert a sized table at the caret via the legacy Insert Table dialog. */
export function paletteInsertTable(options: InsertTableOptions): void {
  const widthPt = Math.round(options.widthInches * 72);
  const rows = Math.max(1, Math.floor(options.rows));
  const columns = Math.max(1, Math.floor(options.columns));

  withEditor((handle) => {
    const el = handle.el;
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) return;
    const range = sel.getRangeAt(0);
    if (!range.collapsed) range.deleteContents();

    const frag = document.createDocumentFragment();
    if (editorIsEmpty(el) || caretAtEditorStart(el)) {
      frag.appendChild(insertBlankParagraph());
    }

    const table = buildLegacyTableElement(widthPt, rows, columns);
    frag.appendChild(table);
    frag.appendChild(insertBlankParagraph());

    range.insertNode(frag);

    const firstCell = table.querySelector("td, th");
    if (firstCell) {
      const nextRange = document.createRange();
      nextRange.selectNodeContents(firstCell);
      nextRange.collapse(true);
      sel.removeAllRanges();
      sel.addRange(nextRange);
    }
  });
}

function exec(command: string, value?: string, styleWithCss = true): void {
  withEditor(() => {
    document.execCommand(command, false, value);
  }, styleWithCss);
}

export function paletteBold(): void {
  exec("bold");
}

export function paletteItalic(): void {
  exec("italic");
}

export function paletteUnderline(): void {
  exec("underline");
}

export function paletteReset(): void {
  exec("removeFormat");
}

export function paletteIndent(): void {
  exec("indent");
}

export function paletteOutdent(): void {
  exec("outdent");
}

export function paletteAlign(dir: PaletteActiveState["align"]): void {
  exec(ALIGN_COMMAND[dir]);
}

export function paletteFontColor(color: string): void {
  exec("foreColor", color);
}

export function paletteFontFace(face: string): void {
  // "Default Font" clearing is deferred (would need to strip inherited font runs); applying a
  // named face is the common case. Choosing Default is a no-op for now.
  if (face === "Default Font") return;
  exec("fontName", face);
}

/**
 * Apply a point size. `execCommand("fontSize")` only takes the legacy 1–7 scale, so we emit a
 * `<font size="7">` marker (styleWithCSS off) and rewrite it to an explicit `font-size` in pt.
 */
export function paletteFontSize(size: string): void {
  if (size === "Default Size") return;
  withEditor((handle) => {
    document.execCommand("fontSize", false, "7");
    handle.el.querySelectorAll('font[size="7"]').forEach((node) => {
      node.removeAttribute("size");
      (node as HTMLElement).style.fontSize = `${size}pt`;
    });
  }, false);
}

/** Delete the entire table containing the caret. */
export function paletteDeleteTable(): void {
  withEditor((handle) => {
    const cell = getSelectedCell(handle.el);
    const table = cell?.closest("table");
    if (table && handle.el.contains(table)) {
      table.remove();
      return;
    }
    document.execCommand("deleteTable");
  });
}

export function paletteInsertColumnBefore(): void {
  withEditor((handle) => {
    const cell = getSelectedCell(handle.el);
    if (!cell) return;
    const row = parentElement(cell);
    const section = row?.parentElement;
    if (!row || !section) return;
    const cellIndex = cell.cellIndex;
    const rows =
      section instanceof HTMLTableSectionElement
        ? Array.from(section.rows)
        : Array.from(section.querySelectorAll("tr"));
    rows.forEach((r) => {
      const inserted = r.insertCell(cellIndex);
      inserted.innerHTML = "&nbsp;";
    });
  });
}

export function paletteInsertColumnAfter(): void {
  withEditor((handle) => {
    const cell = getSelectedCell(handle.el);
    if (!cell) return;
    const row = parentElement(cell);
    const section = row?.parentElement;
    if (!row || !section) return;
    const cellIndex = cell.cellIndex + 1;
    const rows =
      section instanceof HTMLTableSectionElement
        ? Array.from(section.rows)
        : Array.from(section.querySelectorAll("tr"));
    rows.forEach((r) => {
      const inserted = r.insertCell(cellIndex);
      inserted.innerHTML = "&nbsp;";
    });
  });
}

export function paletteDeleteColumn(): void {
  withEditor((handle) => {
    const cell = getSelectedCell(handle.el);
    if (!cell) return;
    const row = parentElement(cell);
    const section = row?.parentElement;
    if (!row || !section) return;
    const cellIndex = cell.cellIndex;
    const rows =
      section instanceof HTMLTableSectionElement
        ? Array.from(section.rows)
        : Array.from(section.querySelectorAll("tr"));
    if (rows.every((r) => r.cells.length <= 1)) return;
    rows.forEach((r) => {
      if (r.cells.length > cellIndex) r.deleteCell(cellIndex);
    });
  });
}

export function paletteInsertRowBefore(): void {
  withEditor((handle) => {
    const row = getSelectedRow(handle.el);
    const section = row?.parentElement;
    if (!row || !section || !(section instanceof HTMLTableSectionElement)) return;
    const newRow = section.insertRow(row.rowIndex);
    for (let i = 0; i < row.cells.length; i++) {
      const inserted = newRow.insertCell();
      inserted.innerHTML = "&nbsp;";
    }
  });
}

export function paletteInsertRowAfter(): void {
  withEditor((handle) => {
    const row = getSelectedRow(handle.el);
    const section = row?.parentElement;
    if (!row || !section || !(section instanceof HTMLTableSectionElement)) return;
    const newRow = section.insertRow(row.rowIndex + 1);
    for (let i = 0; i < row.cells.length; i++) {
      const inserted = newRow.insertCell();
      inserted.innerHTML = "&nbsp;";
    }
  });
}

export function paletteDeleteRow(): void {
  withEditor((handle) => {
    const row = getSelectedRow(handle.el);
    const section = row?.parentElement;
    if (!row || !section || !(section instanceof HTMLTableSectionElement)) return;
    if (section.rows.length <= 1) return;
    section.deleteRow(row.rowIndex);
  });
}

/** Current B/I/U + alignment of the selection, for the palette's pressed/active styling. */
export function readPaletteActiveState(): PaletteActiveState {
  const query = (command: string): boolean => {
    try {
      return document.queryCommandState(command);
    } catch {
      return false;
    }
  };
  let align: PaletteActiveState["align"] = "left";
  if (query("justifyCenter")) align = "center";
  else if (query("justifyRight")) align = "right";
  else if (query("justifyFull")) align = "justify";
  return {
    bold: query("bold"),
    italic: query("italic"),
    underline: query("underline"),
    align,
    fontFace: readFontFaceLabel(),
    fontSize: readFontSizeLabel(),
  };
}
