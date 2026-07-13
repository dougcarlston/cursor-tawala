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
import { formatPt, parseCssPt } from "./tableLayout";
import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
  MIXED_PALETTE_VALUE,
} from "./paletteDefaults";
import {
  applyTypingFormatToPlacedBlock,
  applyTypingFormatToToken,
  adjustPlacedTextIndent,
  alignPlacedTextBlock,
  DOC_INDENT_STEP_PT,
  findPlacedTextBlockAtCaret,
  listPlacedBlocksInSelection,
  PLACED_TEXT_CLASS,
  readPlacedTextAlign,
  reflowAllPlacedLines,
  reflowPlacedLinesBelow,
} from "./documentCanvas";
import {
  blockContainer,
  defaultTypingFormat,
  getTypingFormat,
  isBlankTypingContext,
  setTypingFormat,
} from "./paletteTypingFormat";

export interface PaletteActiveState {
  bold: boolean;
  italic: boolean;
  underline: boolean;
  align: "left" | "center" | "right" | "justify";
  fontFace: string;
  fontSize: string;
  /** Current / typing font color as `#rrggbb`. */
  color: string;
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
    a.fontSize === b.fontSize &&
    a.color === b.color
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

/** Strip explicit point sizes from the selection and unwrap empty FONT/SPAN wrappers. */
function clearExplicitFontSizeInSelection(root: HTMLElement): void {
  const range = currentRangeInEditor(root);
  if (!range) return;

  const clearNode = (node: HTMLElement) => {
    if (isProtectedInlineToken(node)) return;
    if (node.style.fontSize) {
      node.style.removeProperty("font-size");
      if (!node.getAttribute("style")?.trim()) {
        node.removeAttribute("style");
      }
    }
    if (node.tagName === "FONT") {
      node.removeAttribute("size");
      const keep =
        node.getAttribute("face") ||
        node.getAttribute("color") ||
        node.getAttribute("style")?.trim();
      if (!keep) unwrapElement(node);
      return;
    }
    if (
      node.tagName === "SPAN" &&
      !node.className &&
      !node.getAttribute("style")?.trim() &&
      node.attributes.length === 0
    ) {
      unwrapElement(node);
    }
  };

  forEachInlineNodeInSelection(root, clearNode);

  // Catch size wrappers that aren't ancestors of the range endpoints.
  if (!range.collapsed) {
    root.querySelectorAll("font, span[style], [style*='font-size']").forEach((node) => {
      if (!(node instanceof HTMLElement)) return;
      if (isProtectedInlineToken(node)) return;
      try {
        if (!range.intersectsNode(node)) return;
      } catch {
        return;
      }
      clearNode(node);
    });
  }
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

  // Mixed highlight must not report as default — that blocks re-selecting default in the UI.
  if (selectionHasMixedFontSize()) return MIXED_PALETTE_VALUE;

  const pt = readExplicitFontSizePt(handle.el);
  if (pt > 0) {
    return snapFontSizePt(pt);
  }

  // Blank line / collapsed caret after a palette change: show typing attrs, not "default".
  return getTypingFormat(handle.el).fontSize;
}

function readFontFaceLabel(): string {
  const handle = getActivePaletteEditor();
  if (!handle) return DEFAULT_PALETTE_FONT_FACE;

  if (selectionHasMixedFontFace()) return MIXED_PALETTE_VALUE;

  const face = readExplicitFontFace(handle.el);
  if (face) return face;

  // No wrapping face on the caret (collapsed selection, empty line, or fontName did not
  // wrap existing glyphs yet). Prefer the typing format set by the palette so the
  // Font Face box shows the face the designer just chose.
  return getTypingFormat(handle.el).fontFace;
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

function refreshPaletteFocus(handle: PaletteEditorHandle): void {
  const state = getFormattingFocusState();
  if (state.kind === "none" || state.kind === "heading") return;
  setFormattingFocus({
    kind: state.kind,
    cursorInTable: selectionCursorInTable(handle.el),
    hasResettableFormatting: selectionHasResettableFormatting(handle.el),
  });
}

/** Focus the active editor, restore any palette-saved selection, then run `fn` and commit. */
function withEditor(fn: (handle: PaletteEditorHandle) => void, styleWithCss = true): void {
  const handle = getActivePaletteEditor();
  if (!handle) return;
  handle.el.focus();
  // Always restore after focus: dropdowns/color picker leave a collapsed caret on focus,
  // which would otherwise skip restore and drop a text highlight.
  handle.restoreSelection();
  try {
    document.execCommand("styleWithCSS", false, String(styleWithCss));
  } catch {
    /* not supported — ignore */
  }
  fn(handle);
  handle.commit();
  // Keep the post-command range for the next palette click (dropdowns steal focus).
  handle.saveSelection();
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

function isProtectedInlineToken(el: HTMLElement): boolean {
  return el.classList.contains("field-token") || el.classList.contains("function-token");
}

function unwrapElement(el: Element): void {
  const parent = el.parentNode;
  if (!parent) return;
  while (el.firstChild) parent.insertBefore(el.firstChild, el);
  parent.removeChild(el);
}

export function paletteIndent(): void {
  withEditor((handle) => {
    if (indentSelection(handle.el, 1)) return;
    document.execCommand("indent");
  });
}

export function paletteOutdent(): void {
  withEditor((handle) => {
    if (indentSelection(handle.el, -1)) return;
    document.execCommand("outdent");
  });
}

/** Document placed lines or Form paragraph margin — returns true when handled. */
function indentSelection(editor: HTMLElement, delta: 1 | -1): boolean {
  const blocks = listPlacedBlocksInSelection(editor);
  if (blocks.length > 0) {
    for (const block of blocks) {
      adjustPlacedTextIndent(editor, block, delta);
    }
    reflowAllPlacedLines(editor);
    return true;
  }

  const block = blockContainer(
    window.getSelection()?.getRangeAt(0)?.startContainer ?? editor,
    editor,
  );
  if (block === editor || block.classList.contains(PLACED_TEXT_CLASS)) return false;
  if (block instanceof HTMLTableCellElement) return false;

  const current = parseCssPt(block.style.marginLeft);
  const next = Math.max(0, current + delta * DOC_INDENT_STEP_PT);
  if (next <= 0) block.style.removeProperty("margin-left");
  else block.style.marginLeft = formatPt(next);
  return true;
}

export function paletteAlign(dir: PaletteActiveState["align"]): void {
  withEditor((handle) => {
    const blocks = listPlacedBlocksInSelection(handle.el);
    if (blocks.length > 0) {
      for (const block of blocks) {
        alignPlacedTextBlock(handle.el, block, dir);
      }
      reflowAllPlacedLines(handle.el);
      return;
    }
    document.execCommand(ALIGN_COMMAND[dir]);
  });
}

function styleTokensInSelection(root: HTMLElement, typing: ReturnType<typeof getTypingFormat>): void {
  const range = currentRangeInEditor(root);
  if (!range) return;

  const styleOne = (el: HTMLElement) => {
    if (el.classList.contains("field-token") || el.classList.contains("function-token")) {
      applyTypingFormatToToken(el, typing);
    }
  };

  if (range.collapsed) {
    let node: Node | null = range.startContainer;
    if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
    while (node && node !== root) {
      if (node instanceof HTMLElement) styleOne(node);
      node = node.parentNode;
    }
    return;
  }

  root.querySelectorAll(".field-token, .function-token").forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    try {
      if (!range.intersectsNode(node)) return;
    } catch {
      return;
    }
    styleOne(node);
  });
}

function rgbToHex(raw: string): string | null {
  const m = raw.trim().match(/^rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/i);
  if (!m) {
    if (/^#[0-9a-f]{6}$/i.test(raw.trim())) return raw.trim().toLowerCase();
    if (/^#[0-9a-f]{3}$/i.test(raw.trim())) {
      const h = raw.trim().slice(1);
      return `#${h[0]}${h[0]}${h[1]}${h[1]}${h[2]}${h[2]}`.toLowerCase();
    }
    return null;
  }
  const hex = (n: string) => Number(n).toString(16).padStart(2, "0");
  return `#${hex(m[1])}${hex(m[2])}${hex(m[3])}`;
}

function readFontColorLabel(): string {
  const handle = getActivePaletteEditor();
  if (!handle) return "#000000";
  const typing = getTypingFormat(handle.el);
  if (typing.color) {
    return rgbToHex(typing.color) ?? typing.color;
  }

  try {
    const raw = String(document.queryCommandValue("foreColor") || "");
    const hex = rgbToHex(raw);
    if (hex && hex !== "#000000") return hex;
  } catch {
    /* ignore */
  }

  const sel = window.getSelection();
  if (sel && sel.rangeCount > 0 && handle.el.contains(sel.getRangeAt(0).commonAncestorContainer)) {
    let node: Node | null = sel.getRangeAt(0).startContainer;
    if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
    if (node instanceof HTMLElement) {
      const hex = rgbToHex(getComputedStyle(node).color);
      if (hex) return hex;
    }
  }
  return "#000000";
}

export function paletteFontColor(color: string): void {
  withEditor((handle) => {
    const hex = rgbToHex(color) ?? color;
    setTypingFormat(handle.el, { color: hex });

    const sel = window.getSelection();
    const range =
      sel && sel.rangeCount > 0 && handle.el.contains(sel.getRangeAt(0).commonAncestorContainer)
        ? sel.getRangeAt(0)
        : null;
    const hasHighlight = !!(range && !range.collapsed);

    if (hasHighlight && range) {
      // Recolor only the highlight — do not set the whole Document line's color.
      document.execCommand("foreColor", false, hex);

      // Tokens are contenteditable=false; paint any that intersect the highlight.
      handle.el.querySelectorAll(".field-token, .function-token").forEach((node) => {
        if (!(node instanceof HTMLElement)) return;
        try {
          if (!range.intersectsNode(node)) return;
        } catch {
          return;
        }
        node.style.setProperty("color", hex);
      });
      return;
    }

    // Collapsed caret: set typing color for the next characters / inserts.
    // Only paint the Document line block when it is still blank.
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      placed.style.color = hex;
    }
    document.execCommand("foreColor", false, hex);
  });
}

export function paletteFontFace(face: string): void {
  withEditor((handle) => {
    setTypingFormat(handle.el, { fontFace: face });
    const typing = getTypingFormat(handle.el);
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      applyTypingFormatToPlacedBlock(placed, typing);
      styleTokensInSelection(handle.el, typing);
      return;
    }
    styleTokensInSelection(handle.el, typing);
    if (face === DEFAULT_PALETTE_FONT_FACE) {
      forEachInlineNodeInSelection(handle.el, (node) => {
        if (isProtectedInlineToken(node)) return;
        if (node.style.fontFamily) {
          node.style.removeProperty("font-family");
          if (!node.getAttribute("style")?.trim()) {
            node.removeAttribute("style");
          }
        }
        if (node.tagName === "FONT") {
          node.removeAttribute("face");
          const keep =
            node.getAttribute("size") ||
            node.getAttribute("color") ||
            node.getAttribute("style")?.trim();
          if (!keep) unwrapElement(node);
          return;
        }
        if (
          node.tagName === "SPAN" &&
          !node.className &&
          !node.getAttribute("style")?.trim() &&
          node.attributes.length === 0
        ) {
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
 * Non-blank Document lines: size applies only to the selection (or next typed chars), not the
 * whole `.doc-placed-text` block.
 */
export function paletteFontSize(size: string): void {
  withEditor((handle) => {
    setTypingFormat(handle.el, { fontSize: size });
    const typing = getTypingFormat(handle.el);
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      applyTypingFormatToPlacedBlock(placed, typing);
      styleTokensInSelection(handle.el, typing);
      reflowPlacedLinesBelow(handle.el, placed);
      return;
    }
    styleTokensInSelection(handle.el, typing);
    if (size === String(DEFAULT_PALETTE_FONT_SIZE_PT)) {
      clearExplicitFontSizeInSelection(handle.el);
      if (placed) reflowPlacedLinesBelow(handle.el, placed);
      return;
    }
    document.execCommand("fontSize", false, "7");
    rewriteLegacyFontSizeMarkers(handle.el, size);
    if (placed) reflowPlacedLinesBelow(handle.el, placed);
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
  const placed = handle ? findPlacedTextBlockAtCaret(handle.el) : null;
  if (placed) {
    align = readPlacedTextAlign(placed);
  } else if (query("justifyCenter")) align = "center";
  else if (query("justifyRight")) align = "right";
  else if (query("justifyFull")) align = "justify";
  return {
    bold: blank ? typing.bold : query("bold"),
    italic: blank ? typing.italic : query("italic"),
    underline: blank ? typing.underline : query("underline"),
    align,
    fontFace: readFontFaceLabel(),
    fontSize: readFontSizeLabel(),
    color: readFontColorLabel(),
  };
}
