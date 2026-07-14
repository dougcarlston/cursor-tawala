/**
 * Per-editor "typing attributes" — font/size/B/I/U for the next characters typed.
 * Legacy parity: blank paragraphs keep the last palette choice instead of reverting
 * to browser defaults (which mis-report as 8 pt via `queryCommandValue("fontSize")`).
 */

import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
  matchFontFace,
} from "./paletteDefaults";
import { parseCssPt, pxToPt } from "./tableLayout";

export interface TypingFormat {
  fontFace: string;
  fontSize: string;
  bold: boolean;
  italic: boolean;
  underline: boolean;
  /** Hex color e.g. `#000000`; omit/undefined = default (inherit). */
  color?: string;
}

export function defaultTypingFormat(): TypingFormat {
  return {
    fontFace: DEFAULT_PALETTE_FONT_FACE,
    fontSize: String(DEFAULT_PALETTE_FONT_SIZE_PT),
    bold: false,
    italic: false,
    underline: false,
    color: undefined,
  };
}

const typingByEditor = new WeakMap<HTMLElement, TypingFormat>();

export function getTypingFormat(editor: HTMLElement): TypingFormat {
  return typingByEditor.get(editor) ?? defaultTypingFormat();
}

export function setTypingFormat(editor: HTMLElement, patch: Partial<TypingFormat>): void {
  typingByEditor.set(editor, { ...getTypingFormat(editor), ...patch });
}

export function resetTypingFormat(editor: HTMLElement): void {
  typingByEditor.set(editor, defaultTypingFormat());
}

/** Block-level container for caret inspection (stop before editor root). */
export function blockContainer(node: Node, editor: HTMLElement): HTMLElement {
  let current: Node | null = node.nodeType === Node.TEXT_NODE ? node.parentNode : node;
  while (current && current !== editor) {
    if (current instanceof HTMLElement) {
      const tag = current.tagName;
      if (
        tag === "P" ||
        tag === "DIV" ||
        tag === "TD" ||
        tag === "TH" ||
        tag === "LI" ||
        current.classList.contains("doc-placed-text")
      ) {
        return current;
      }
    }
    current = current.parentNode;
  }
  return editor;
}

function meaningfulText(text: string | null | undefined): string {
  return (text ?? "").replace(/\u00a0|\u200b/g, "").trim();
}

/** True when the caret sits in an empty block — palette should show typing attrs, not DOM sniffing. */
export function isBlankTypingContext(editor: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  const range = sel.getRangeAt(0);
  if (!range.collapsed || !editor.contains(range.commonAncestorContainer)) return false;

  const block = blockContainer(range.startContainer, editor);
  // Gaps on the document canvas (between placed blocks) — not the text in other blocks.
  if (block === editor) return true;
  if (meaningfulText(block.textContent)) return false;

  return true;
}

export function caretTextNode(editor: HTMLElement): Node | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) return null;
  let node: Node | null = range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) return node;
  if (node instanceof HTMLElement && node.childNodes.length) {
    return node.childNodes[Math.min(range.startOffset, node.childNodes.length - 1)] ?? node;
  }
  return node;
}

function isProtectedToken(el: HTMLElement): boolean {
  return el.classList.contains("field-token") || el.classList.contains("function-token");
}

/**
 * B/I/U pressed state at the caret.
 * Prefer the caret probe's **computed** style (tracks plain vs bold/italic glyphs)
 * plus tag / inline-style ancestry. Do **not** use `queryCommandState` — it stays
 * lit across run boundaries after B/I.
 * Field/function chips carry marks on the chip element itself (contenteditable=false
 * cannot nest `<b>`/`<i>`/`<u>`); read those styles when the caret/selection is on a chip.
 */
export function readInlineMarksAtCaret(editor: HTMLElement): {
  bold: boolean;
  italic: boolean;
  underline: boolean;
} {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) {
    return { bold: false, italic: false, underline: false };
  }
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) {
    return { bold: false, italic: false, underline: false };
  }

  const marksFromStyle = (el: HTMLElement): {
    bold: boolean;
    italic: boolean;
    underline: boolean;
  } => {
    let bold = false;
    let italic = false;
    let underline = false;
    const tag = el.tagName;
    if (tag === "B" || tag === "STRONG") bold = true;
    if (tag === "I" || tag === "EM") italic = true;
    if (tag === "U") underline = true;
    const style = el.style;
    const weight = style.fontWeight;
    if (weight === "bold" || weight === "bolder") bold = true;
    const weightN = Number.parseInt(weight, 10);
    if (Number.isFinite(weightN) && weightN >= 600) bold = true;
    if (style.fontStyle === "italic" || style.fontStyle === "oblique") italic = true;
    const deco = `${style.textDecoration} ${style.textDecorationLine}`.toLowerCase();
    if (deco.includes("underline")) underline = true;
    return { bold, italic, underline };
  };

  // Chip alone / caret on chip: marks live on the token style, not ancestors.
  const tokenHit =
    range.startContainer instanceof HTMLElement && isProtectedToken(range.startContainer)
      ? range.startContainer
      : range.startContainer.parentElement?.closest(".field-token, .function-token");
  if (tokenHit instanceof HTMLElement && editor.contains(tokenHit)) {
    if (
      range.collapsed &&
      (tokenHit === range.startContainer || tokenHit.contains(range.startContainer))
    ) {
      return marksFromStyle(tokenHit);
    }
    if (!range.collapsed) {
      try {
        const tr = document.createRange();
        tr.selectNode(tokenHit);
        const within =
          range.compareBoundaryPoints(Range.START_TO_START, tr) >= 0 &&
          range.compareBoundaryPoints(Range.END_TO_END, tr) <= 0;
        if (within) return marksFromStyle(tokenHit);
      } catch {
        /* fall through to ancestry walk */
      }
    }
  }

  const block = blockContainer(range.startContainer, editor);
  const probe = caretStyleProbeElement(editor);

  let bold = false;
  let italic = false;
  let underline = false;

  let node: Node | null = probe ?? range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;

  while (node && node instanceof HTMLElement && node !== editor) {
    if (isProtectedToken(node)) {
      const chip = marksFromStyle(node);
      bold = bold || chip.bold;
      italic = italic || chip.italic;
      underline = underline || chip.underline;
      node = node.parentNode;
      continue;
    }
    const m = marksFromStyle(node);
    bold = bold || m.bold;
    italic = italic || m.italic;
    underline = underline || m.underline;

    if (node === block) break;
    node = node.parentNode;
  }

  if (probe && !isProtectedToken(probe)) {
    try {
      const cs = getComputedStyle(probe);
      const cw = (cs.fontWeight || "").trim();
      // Only trust CSSOM when the engine reports a real weight (skip empty jsdom stubs).
      if (cw) {
        const cwN = Number.parseInt(cw, 10);
        bold = cw === "bold" || cw === "bolder" || (Number.isFinite(cwN) && cwN >= 600);
        italic = cs.fontStyle === "italic" || cs.fontStyle === "oblique";
        underline = `${cs.textDecorationLine} ${cs.textDecoration}`
          .toLowerCase()
          .includes("underline");
      }
    } catch {
      /* detached — keep tag/inline result */
    }
  }

  return { bold, italic, underline };
}

/**
 * Probe element for computed face/size at the caret — prefers a glyph next to the
 * caret so inserts match surrounding runs (not sticky typing-format fallbacks).
 */
export function caretStyleProbeElement(editor: HTMLElement): HTMLElement | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) return null;

  const block = blockContainer(range.startContainer, editor);
  let node: Node | null = range.startContainer;
  const offset = range.startOffset;

  if (node.nodeType === Node.TEXT_NODE) {
    const parent = node.parentElement;
    if (parent && !isProtectedToken(parent)) return parent;
    const prev = adjacentText(node, block, "prev");
    if (prev?.parentElement && !isProtectedToken(prev.parentElement)) return prev.parentElement;
    const next = adjacentText(node, block, "next");
    if (next?.parentElement && !isProtectedToken(next.parentElement)) return next.parentElement;
    return parent;
  }

  if (node instanceof HTMLElement) {
    if (isProtectedToken(node)) return node.parentElement;
    const idx = Math.max(0, Math.min(offset, node.childNodes.length) - 1);
    const child = node.childNodes[idx];
    if (child?.nodeType === Node.TEXT_NODE && child.parentElement) return child.parentElement;
    if (child instanceof HTMLElement && !isProtectedToken(child)) return child;
    return node;
  }
  return null;
}

function adjacentText(from: Node, stopAt: Node, dir: "prev" | "next"): Text | null {
  let node: Node | null = from;
  while (node && node !== stopAt) {
    const sibling: Node | null = dir === "prev" ? node.previousSibling : node.nextSibling;
    if (sibling) {
      let walk: Node = sibling;
      while (dir === "prev" ? walk.lastChild : walk.firstChild) {
        walk = (dir === "prev" ? walk.lastChild : walk.firstChild)!;
      }
      if (walk.nodeType === Node.TEXT_NODE) {
        const t = walk as Text;
        if ((t.textContent ?? "").replace(/\u00a0|\u200b/g, "").length) return t;
      }
      node = walk;
      continue;
    }
    node = node.parentNode;
  }
  return null;
}

/** Face/size/B/I/U for a field/function insert at the current caret. */
export function typingFormatForInsert(editor: HTMLElement): TypingFormat {
  const sticky = getTypingFormat(editor);
  if (isBlankTypingContext(editor)) return sticky;

  const probe = caretStyleProbeElement(editor);
  if (!probe) return sticky;

  const marks = readInlineMarksAtCaret(editor);
  const cs = getComputedStyle(probe);
  const sizePx = Number.parseFloat(cs.fontSize);
  const sizePt = Number.isFinite(sizePx) ? pxToPt(sizePx) : DEFAULT_PALETTE_FONT_SIZE_PT;
  // Prefer explicit inline size when present on the probe ancestry.
  let explicitPt = 0;
  let el: HTMLElement | null = probe;
  const block = blockContainer(probe, editor);
  while (el && el !== editor) {
    if (el.style.fontSize) {
      explicitPt = parseCssPt(el.style.fontSize);
      break;
    }
    if (el === block) break;
    el = el.parentElement;
  }
  if (block.classList.contains("doc-placed-text") && !explicitPt && block.style.fontSize) {
    explicitPt = parseCssPt(block.style.fontSize);
  }
  const pt = explicitPt > 0 ? explicitPt : sizePt;
  const rounded = String(Math.max(1, Math.round(pt)));

  let face = matchFontFace(cs.fontFamily);
  el = probe;
  while (el && el !== editor) {
    if (el.style.fontFamily) {
      face = matchFontFace(el.style.fontFamily);
      break;
    }
    if (el.tagName === "FONT" && el.getAttribute("face")) {
      face = matchFontFace(el.getAttribute("face")!);
      break;
    }
    if (el === block) break;
    el = el.parentElement;
  }
  if (block.classList.contains("doc-placed-text") && block.style.fontFamily && face === DEFAULT_PALETTE_FONT_FACE) {
    face = matchFontFace(block.style.fontFamily);
  }

  return {
    fontFace: face,
    fontSize: rounded,
    bold: marks.bold,
    italic: marks.italic,
    underline: marks.underline,
    color: sticky.color,
  };
}
