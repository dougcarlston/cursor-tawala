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
  selectionHasResettableFormatting,
  setFormattingFocus,
  type PaletteEditorHandle,
} from "./formattingPaletteContext";
import { parseCssPt } from "./tableLayout";
import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
} from "./paletteDefaults";
import {
  applyTypingFormatToPlacedBlock,
  findPlacedTextBlockAtCaret,
  PLACED_TEXT_CLASS,
} from "./documentCanvas";
import {
  blockContainer,
  defaultTypingFormat,
  getTypingFormat,
  isBlankTypingContext,
  resetTypingFormat,
  setTypingFormat,
} from "./paletteTypingFormat";

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

export const FONT_SIZE_PT = [8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 26, 28, 36, 48, 72] as const;

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

function currentRangeInEditor(root: HTMLElement): Range | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  const range = sel.getRangeAt(0);
  if (!root.contains(range.commonAncestorContainer)) return null;
  return range;
}

/** Visit inline formatting nodes at the caret or within the current selection only. */
function forEachInlineNodeInSelection(
  root: HTMLElement,
  visit: (el: HTMLElement) => void,
): void {
  const range = currentRangeInEditor(root);
  if (!range) return;

  const seen = new Set<HTMLElement>();
  const addAncestors = (node: Node | null) => {
    let el: Node | null = node;
    if (el?.nodeType === Node.TEXT_NODE) el = el.parentNode;
    while (el && el instanceof HTMLElement && el !== root) {
      if (!seen.has(el)) {
        seen.add(el);
        visit(el);
      }
      el = el.parentNode;
    }
  };

  if (range.collapsed) {
    addAncestors(range.startContainer);
    return;
  }

  addAncestors(range.startContainer);
  addAncestors(range.endContainer);
  root.querySelectorAll("font, span[style]").forEach((node) => {
    if (!(node instanceof HTMLElement) || seen.has(node)) return;
    if (!range.intersectsNode(node)) return;
    seen.add(node);
    visit(node);
  });
}

function rewriteLegacyFontSizeMarkers(root: HTMLElement, sizePt: string): void {
  const range = currentRangeInEditor(root);
  if (!range) return;

  const rewrite = (node: HTMLElement) => {
    if (node.tagName !== "FONT" || node.getAttribute("size") !== "7") return;
    node.removeAttribute("size");
    node.style.fontSize = `${sizePt}pt`;
  };

  if (range.collapsed) {
    forEachInlineNodeInSelection(root, rewrite);
    return;
  }

  root.querySelectorAll('font[size="7"]').forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    if (!range.intersectsNode(node)) return;
    rewrite(node);
  });
}

export function emitPaletteActiveState(): void {
  cachedPaletteActiveState = null;
  paletteActiveListeners.forEach((cb) => cb());
}

function matchFontFace(raw: string): string {
  const first = raw.replace(/['"]/g, "").split(",")[0]?.trim().toLowerCase() ?? "";
  if (!first) return DEFAULT_PALETTE_FONT_FACE;
  if (first.includes("segoe")) return DEFAULT_PALETTE_FONT_FACE;
  for (const face of WEB_SAFE_FACES) {
    if (face.toLowerCase() === first) return face;
  }
  return DEFAULT_PALETTE_FONT_FACE;
}

function snapFontSizePt(pt: number): string {
  if (pt <= 0 || Math.abs(pt - DEFAULT_PALETTE_FONT_SIZE_PT) <= 1.5) {
    return String(DEFAULT_PALETTE_FONT_SIZE_PT);
  }
  let best: number = FONT_SIZE_PT[0];
  let bestDiff = Infinity;
  for (const size of FONT_SIZE_PT) {
    const diff = Math.abs(size - pt);
    if (diff < bestDiff) {
      bestDiff = diff;
      best = size;
    }
  }
  return bestDiff <= 1.5 ? String(best) : String(DEFAULT_PALETTE_FONT_SIZE_PT);
}

/** Inline font-size on ancestors between the caret and its block — not inherited block CSS. */
function readExplicitFontSizePt(editor: HTMLElement): number {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return 0;
  const range = sel.getRangeAt(0);
  const block = blockContainer(range.startContainer, editor);
  let node: Node | null = range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node instanceof HTMLElement && node !== block) {
    if (node.style.fontSize) return parseCssPt(node.style.fontSize);
    if (node.tagName === "FONT") {
      const legacy = node.getAttribute("size");
      if (legacy && legacy !== "3" && LEGACY_SIZE_TO_PT[legacy]) {
        return LEGACY_SIZE_TO_PT[legacy];
      }
    }
    node = node.parentNode;
  }
  if (block.classList.contains(PLACED_TEXT_CLASS) && block.style.fontSize) {
    return parseCssPt(block.style.fontSize);
  }
  return 0;
}

function readExplicitFontFace(editor: HTMLElement): string | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  const range = sel.getRangeAt(0);
  const block = blockContainer(range.startContainer, editor);
  let node: Node | null = range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node instanceof HTMLElement && node !== block) {
    if (node.style.fontFamily) return matchFontFace(node.style.fontFamily);
    if (node.tagName === "FONT" && node.getAttribute("face")) {
      return matchFontFace(node.getAttribute("face")!);
    }
    node = node.parentNode;
  }
  if (block.classList.contains(PLACED_TEXT_CLASS) && block.style.fontFamily) {
    return matchFontFace(block.style.fontFamily);
  }
  return null;
}

function readFontSizeLabel(): string {
  const handle = getActivePaletteEditor();
  if (!handle) return String(DEFAULT_PALETTE_FONT_SIZE_PT);

  if (isBlankTypingContext(handle.el)) {
    return getTypingFormat(handle.el).fontSize;
  }

  if (selectionHasMixedFontSize()) return String(DEFAULT_PALETTE_FONT_SIZE_PT);

  const pt = readExplicitFontSizePt(handle.el);
  if (pt > 0) {
    return snapFontSizePt(pt);
  }

  return String(DEFAULT_PALETTE_FONT_SIZE_PT);
}

function readFontFaceLabel(): string {
  const handle = getActivePaletteEditor();
  if (!handle) return DEFAULT_PALETTE_FONT_FACE;

  if (isBlankTypingContext(handle.el)) {
    return getTypingFormat(handle.el).fontFace;
  }

  if (selectionHasMixedFontFace()) return DEFAULT_PALETTE_FONT_FACE;

  const face = readExplicitFontFace(handle.el);
  if (face) return face;

  return DEFAULT_PALETTE_FONT_FACE;
}

function selectionHasMixedFontFace(): boolean {
  const handle = getActivePaletteEditor();
  const sel = window.getSelection();
  if (!handle || !sel || sel.rangeCount === 0 || sel.isCollapsed) return false;
  const range = sel.getRangeAt(0);
  if (!handle.el.contains(range.commonAncestorContainer)) return false;
  const faces = new Set<string>();
  walkRangeTextNodes(range, (node) => {
    if (!(node.parentElement && handle.el.contains(node.parentElement))) return;
    const face = matchFontFace(getComputedStyle(node.parentElement!).fontFamily);
    faces.add(face);
  });
  return faces.size > 1;
}

function selectionHasMixedFontSize(): boolean {
  const handle = getActivePaletteEditor();
  const sel = window.getSelection();
  if (!handle || !sel || sel.rangeCount === 0 || sel.isCollapsed) return false;
  const range = sel.getRangeAt(0);
  if (!handle.el.contains(range.commonAncestorContainer)) return false;
  const sizes = new Set<string>();
  walkRangeTextNodes(range, (node) => {
    if (!(node.parentElement && handle.el.contains(node.parentElement))) return;
    sizes.add(readFontSizeLabelFromNode(node, handle.el));
  });
  return sizes.size > 1;
}

function readFontSizeLabelFromNode(node: Node, editor: HTMLElement): string {
  const block = blockContainer(node, editor);
  let el: Node | null = node.nodeType === Node.TEXT_NODE ? node.parentNode : node;
  while (el && el instanceof HTMLElement && el !== block) {
    if (el.style.fontSize) {
      const pt = parseCssPt(el.style.fontSize);
      if (pt > 0) return snapFontSizePt(pt);
    }
    if (el.tagName === "FONT") {
      const legacy = el.getAttribute("size");
      if (legacy && legacy !== "3" && LEGACY_SIZE_TO_PT[legacy]) {
        return snapFontSizePt(LEGACY_SIZE_TO_PT[legacy]);
      }
    }
    el = el.parentElement;
  }
  if (block.classList.contains(PLACED_TEXT_CLASS) && block.style.fontSize) {
    return snapFontSizePt(parseCssPt(block.style.fontSize));
  }
  return String(DEFAULT_PALETTE_FONT_SIZE_PT);
}

function walkRangeTextNodes(range: Range, visit: (node: Text) => void): void {
  const root = range.commonAncestorContainer;
  const walker = document.createTreeWalker(
    root.nodeType === Node.ELEMENT_NODE ? root : root.parentNode ?? root,
    NodeFilter.SHOW_TEXT,
  );
  let node: Text | null;
  while ((node = walker.nextNode() as Text | null)) {
    if (!range.intersectsNode(node)) continue;
    if (!node.textContent?.replace(/\u00a0|\u200b/g, "").length) continue;
    visit(node);
  }
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

function refreshPaletteFocus(handle: PaletteEditorHandle): void {
  const state = getFormattingFocusState();
  if (state.kind === "none" || state.kind === "heading") return;
  setFormattingFocus({
    kind: state.kind,
    cursorInTable: selectionCursorInTable(handle.el),
    hasResettableFormatting: selectionHasResettableFormatting(handle.el),
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
  refreshPaletteFocus(handle);
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
  withEditor((handle) => {
    document.execCommand("bold");
    try {
      setTypingFormat(handle.el, { bold: document.queryCommandState("bold") });
    } catch {
      /* ignore */
    }
  });
}

export function paletteItalic(): void {
  withEditor((handle) => {
    document.execCommand("italic");
    try {
      setTypingFormat(handle.el, { italic: document.queryCommandState("italic") });
    } catch {
      /* ignore */
    }
  });
}

export function paletteUnderline(): void {
  withEditor((handle) => {
    document.execCommand("underline");
    try {
      setTypingFormat(handle.el, { underline: document.queryCommandState("underline") });
    } catch {
      /* ignore */
    }
  });
}

export function paletteReset(): void {
  withEditor((handle) => {
    stripFormattingInEditor(handle.el);
    resetTypingFormat(handle.el);
  });
}

function isProtectedInlineToken(el: HTMLElement): boolean {
  return el.classList.contains("field-token") || el.classList.contains("function-token");
}

function unwrapElement(el: Element): void {
  const parent = el.parentNode;
  if (!parent) return;
  while (el.firstChild) parent.insertBefore(el.firstChild, el);
  parent.removeChild(el);
}

function stripInlineStyle(span: HTMLElement): void {
  span.style.removeProperty("font-weight");
  span.style.removeProperty("font-style");
  span.style.removeProperty("text-decoration");
  span.style.removeProperty("color");
  span.style.removeProperty("font-family");
  span.style.removeProperty("font-size");
  span.style.removeProperty("background-color");
  if (!span.getAttribute("style")?.trim()) {
    unwrapElement(span);
  }
}

/** Remove character formatting without `removeFormat` (which can block later palette commands). */
function stripFormattingInEditor(root: HTMLElement): void {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return;
  const range = sel.getRangeAt(0);
  if (!root.contains(range.commonAncestorContainer)) return;

  if (!range.collapsed) {
    const extracted = range.extractContents();
    stripFormattingFragment(extracted);
    range.insertNode(extracted);
    sel.removeAllRanges();
    sel.addRange(range);
    return;
  }

  for (let pass = 0; pass < 32; pass++) {
    if (!stripOneFormatLayerAtCaret(root, range)) break;
    if (!sel.rangeCount) break;
  }
}

function stripOneFormatLayerAtCaret(root: HTMLElement, range: Range): boolean {
  let node: Node | null = range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== root) {
    if (!(node instanceof HTMLElement)) {
      node = node.parentNode;
      continue;
    }
    if (isProtectedInlineToken(node)) return false;
    const tag = node.tagName;
    if (tag === "B" || tag === "STRONG" || tag === "I" || tag === "EM" || tag === "U" || tag === "FONT") {
      unwrapElement(node);
      return true;
    }
    if (tag === "SPAN" && node.hasAttribute("style")) {
      stripInlineStyle(node);
      return true;
    }
    node = node.parentNode;
  }
  return false;
}

function stripFormattingFragment(root: ParentNode): void {
  for (let pass = 0; pass < 8; pass++) {
    let changed = false;
    root.querySelectorAll("b, strong, i, em, u, font").forEach((node) => {
      if (node instanceof HTMLElement && !isProtectedInlineToken(node)) {
        unwrapElement(node);
        changed = true;
      }
    });
    root.querySelectorAll("span[style]").forEach((node) => {
      if (node instanceof HTMLElement && !isProtectedInlineToken(node)) {
        const before = node.getAttribute("style");
        stripInlineStyle(node);
        if (before !== node.getAttribute("style")) changed = true;
      }
    });
    if (!changed) break;
  }
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
  withEditor((handle) => {
    setTypingFormat(handle.el, { fontFace: face });
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      applyTypingFormatToPlacedBlock(placed, getTypingFormat(handle.el));
      return;
    }
    if (face === DEFAULT_PALETTE_FONT_FACE) {
      forEachInlineNodeInSelection(handle.el, (node) => {
        if (isProtectedInlineToken(node)) return;
        if (node.style.fontFamily) {
          node.style.removeProperty("font-family");
          if (!node.getAttribute("style")?.trim() && node.tagName === "SPAN") {
            unwrapElement(node);
          }
        }
        if (node.tagName === "FONT" && node.hasAttribute("face")) {
          unwrapElement(node);
        }
      });
      return;
    }
    document.execCommand("fontName", false, face);
  });
}

/**
 * Apply a point size. `execCommand("fontSize")` only takes the legacy 1–7 scale, so we emit a
 * `<font size="7">` marker (styleWithCSS off) and rewrite it to an explicit `font-size` in pt.
 */
export function paletteFontSize(size: string): void {
  withEditor((handle) => {
    setTypingFormat(handle.el, { fontSize: size });
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      applyTypingFormatToPlacedBlock(placed, getTypingFormat(handle.el));
      return;
    }
    if (size === String(DEFAULT_PALETTE_FONT_SIZE_PT)) {
      forEachInlineNodeInSelection(handle.el, (node) => {
        if (isProtectedInlineToken(node)) return;
        if (node.style.fontSize) {
          node.style.removeProperty("font-size");
          if (!node.getAttribute("style")?.trim() && node.tagName === "SPAN") {
            unwrapElement(node);
          }
        }
        if (node.tagName === "FONT" && node.hasAttribute("size")) {
          unwrapElement(node);
        }
      });
      return;
    }
    document.execCommand("fontSize", false, "7");
    rewriteLegacyFontSizeMarkers(handle.el, size);
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
  const handle = getActivePaletteEditor();
  const blank = handle ? isBlankTypingContext(handle.el) : false;
  const typing = handle ? getTypingFormat(handle.el) : defaultTypingFormat();

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
    bold: blank ? typing.bold : query("bold"),
    italic: blank ? typing.italic : query("italic"),
    underline: blank ? typing.underline : query("underline"),
    align,
    fontFace: readFontFaceLabel(),
    fontSize: readFontSizeLabel(),
  };
}
