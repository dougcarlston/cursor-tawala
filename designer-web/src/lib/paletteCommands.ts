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
  applyAlignToSelectedTableCells,
  applyTableBorderStyle,
  forEachFormatTargetCell,
  selectionInsideUserTable,
  type TableBorderStyle,
} from "./tableCellSelection";
import { useProjectStore } from "@/store/projectStore";
import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
  MIXED_PALETTE_VALUE,
  matchFontFace,
} from "./paletteDefaults";
import {
  applyTypingFormatToPlacedBlock,
  applyTypingFormatToToken,
  adjustPlacedTextIndent,
  adjustUserTableIndent,
  alignPlacedTextBlock,
  DOC_INDENT_STEP_PT,
  findPlacedTextBlockAtCaret,
  insertDocumentUserTable,
  listPlacedBlocksInSelection,
  listUserTablesInSelection,
  PLACED_TEXT_CLASS,
  readPlacedTextAlign,
  reflowAllPlacedLines,
  reflowPlacedLinesBelow,
  stripLeadingWhitespaceForLeftAlign,
} from "./documentCanvas";
import {
  blockContainer,
  defaultTypingFormat,
  getTypingFormat,
  isBlankTypingContext,
  readInlineMarksAtCaret,
  setTypingFormat,
} from "./paletteTypingFormat";
import {
  bookmarkTextOffsets,
  expandRangeToTouchedTokens,
  rangeFromTextOffsets,
  restoreSelectionFromBookmark,
  type TextOffsetBookmark,
} from "./selectionBookmark";
import { ensureFieldTokenCaretGaps } from "./fieldTokens";
import { ensureFunctionTokenCaretGaps } from "./functionTokens";
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

/**
 * Chrome's `execCommand("fontSize", …, "7")` marker — HTML size 7 maps to ~36pt.
 * Must be rewritten to an explicit `Npt` or the run stays huge (classic 8→36 bug).
 */
const CHROME_SIZE_SEVEN_CSS = new Set(["xxx-large", "-webkit-xxx-large"]);

/** Exported for unit tests — true when a node is an unre-written size-7 / xxx-large marker. */
export function isChromeSizeSevenMarker(el: HTMLElement): boolean {
  if (el.tagName === "FONT" && el.getAttribute("size") === "7") return true;
  if (el.tagName !== "SPAN" && el.tagName !== "FONT") return false;
  const fs = el.style.fontSize.trim().toLowerCase();
  return CHROME_SIZE_SEVEN_CSS.has(fs);
}

/** Parse inline font-size including legacy CSS keywords from fontSize execCommand. */
function parseInlineFontSizePt(raw: string | null | undefined): number {
  const value = (raw ?? "").trim();
  if (!value) return 0;
  const fromCss = parseCssPt(value);
  if (fromCss > 0) return fromCss;
  const key = value.toLowerCase();
  if (CHROME_SIZE_SEVEN_CSS.has(key)) return LEGACY_SIZE_TO_PT["7"];
  if (key === "xx-large") return LEGACY_SIZE_TO_PT["6"];
  if (key === "x-large") return LEGACY_SIZE_TO_PT["5"];
  if (key === "large") return LEGACY_SIZE_TO_PT["4"];
  if (key === "medium") return LEGACY_SIZE_TO_PT["3"];
  if (key === "small") return LEGACY_SIZE_TO_PT["2"];
  if (key === "x-small" || key === "xx-small") return LEGACY_SIZE_TO_PT["1"];
  return 0;
}

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
  forEachInlineNodeInRange(root, range, visit);
}

/**
 * True when `node` is fully inside `range` (not merely overlapping). Used by Size/Face
 * flatten so we never clear wrappers whose glyphs extend outside the highlight — that
 * remounted neighbors (owner SS4: left→TNR, right→Arial after Size).
 */
function rangeFullyContainsNode(range: Range, node: Node): boolean {
  try {
    const nr = document.createRange();
    if (node.nodeType === Node.TEXT_NODE) {
      const len = (node as Text).data.length;
      nr.setStart(node, 0);
      nr.setEnd(node, len);
    } else {
      nr.selectNode(node);
    }
    return (
      range.compareBoundaryPoints(Range.START_TO_START, nr) <= 0 &&
      range.compareBoundaryPoints(Range.END_TO_END, nr) >= 0
    );
  } catch {
    return false;
  }
}

/**
 * True when `range` covers every meaningful glyph (and chip) inside `el`.
 * Word-select of "Left" inside `<span style="font-size:20pt">Left</span>` does not
 * `selectNode`-contain the span, and `selectNodeContents` + compareBoundaryPoints
 * is brittle across engines when the live range is text-anchored (happy-dom: startCmp
 * 1 / endCmp -1 even for a full-wrap select). Glyph ownership matches Chrome UX and
 * still refuses wrappers with glyphs outside the highlight (SS4 neighbors).
 */
function rangeOwnsEntireElementContents(range: Range, el: HTMLElement): boolean {
  if (rangeFullyContainsNode(range, el)) return true;

  let sawGlyph = false;
  const walker = document.createTreeWalker(el, NodeFilter.SHOW_TEXT);
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (text.parentElement?.closest(".field-token, .function-token")) continue;
    if (isIgnorableWhitespaceText(text.data)) continue;
    sawGlyph = true;
    if (!rangeFullyContainsNode(range, text)) return false;
  }
  for (const token of el.querySelectorAll(".field-token, .function-token")) {
    if (!(token instanceof HTMLElement)) continue;
    sawGlyph = true;
    if (!rangeFullyContainsNode(range, token)) return false;
  }
  return sawGlyph;
}

/** Clear eligibility for Face/Size flatten: chips + owned anonymous wrappers. */
function rangeMayClearFormatNode(range: Range, node: HTMLElement): boolean {
  if (isProtectedInlineToken(node)) return true;
  return rangeOwnsEntireElementContents(range, node);
}

/**
 * Clone face/size style attrs from `src` onto a fresh anonymous SPAN (or FONT).
 * Used when splitting wrappers at selection boundaries so outside glyphs keep Face.
 */
function cloneAnonymousFormatWrapper(src: HTMLElement): HTMLElement {
  const clone =
    src.tagName === "FONT"
      ? document.createElement("font")
      : document.createElement("span");
  if (src.style.fontFamily) clone.style.fontFamily = src.style.fontFamily;
  if (src.style.fontSize) clone.style.fontSize = src.style.fontSize;
  if (src.style.color) clone.style.color = src.style.color;
  if (src.style.fontWeight) clone.style.fontWeight = src.style.fontWeight;
  if (src.style.fontStyle) clone.style.fontStyle = src.style.fontStyle;
  if (src.style.textDecoration) clone.style.textDecoration = src.style.textDecoration;
  if (src.tagName === "FONT") {
    const face = src.getAttribute("face");
    if (face) clone.setAttribute("face", face);
    const color = src.getAttribute("color");
    if (color) clone.setAttribute("color", color);
    const size = src.getAttribute("size");
    if (size) clone.setAttribute("size", size);
  }
  return clone;
}

/**
 * Split anonymous Face/Size wrappers so the selection boundaries land on element
 * borders. Outside remnants keep their face (+ prior size); Size then paints only
 * the mid segment. Prevents mid-word orphan faces and neighbor bleed (SS4).
 */
function splitFormatWrappersAtRangeBoundaries(root: HTMLElement, range: Range): void {
  if (range.collapsed) return;

  const candidates: HTMLElement[] = [];
  root.querySelectorAll("span[style], font").forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    if (isProtectedInlineToken(node)) return;
    if (node.className) return;
    if (node === blockContainer(node, root)) return;
    try {
      if (!range.intersectsNode(node)) return;
    } catch {
      return;
    }
    if (rangeFullyContainsNode(range, node)) return;
    const hasFace =
      !!node.style.fontFamily ||
      (node.tagName === "FONT" && !!node.getAttribute("face"));
    const hasSize =
      !!node.style.fontSize ||
      (node.tagName === "FONT" && !!node.getAttribute("size"));
    if (!hasFace && !hasSize) return;
    candidates.push(node);
  });

  // Deepest first so nested wrappers split cleanly.
  candidates.sort((a, b) => {
    if (a.contains(b)) return 1;
    if (b.contains(a)) return -1;
    return 0;
  });

  for (const wrap of candidates) {
    if (!wrap.isConnected || !root.contains(wrap)) continue;
    try {
      if (!range.intersectsNode(wrap)) continue;
    } catch {
      continue;
    }
    if (rangeFullyContainsNode(range, wrap)) continue;

    // Leading remnant — glyphs before the selection start, same Face/Size as wrap.
    if (wrap.contains(range.startContainer)) {
      try {
        const cut = document.createRange();
        cut.selectNodeContents(wrap);
        cut.setEnd(range.startContainer, range.startOffset);
        if (!cut.collapsed) {
          const frag = cut.extractContents();
          if (frag.textContent?.replace(/\u00a0|\u200b/g, "").length) {
            const left = cloneAnonymousFormatWrapper(wrap);
            left.appendChild(frag);
            wrap.parentNode?.insertBefore(left, wrap);
          }
        }
      } catch {
        /* ignore split failures — Size still may apply */
      }
    }

    // Trailing remnant — glyphs after the selection end.
    if (wrap.isConnected && wrap.contains(range.endContainer)) {
      try {
        const cut = document.createRange();
        cut.selectNodeContents(wrap);
        cut.setStart(range.endContainer, range.endOffset);
        if (!cut.collapsed) {
          const frag = cut.extractContents();
          if (frag.textContent?.replace(/\u00a0|\u200b/g, "").length) {
            const right = cloneAnonymousFormatWrapper(wrap);
            right.appendChild(frag);
            wrap.parentNode?.insertBefore(right, wrap.nextSibling);
          }
        }
      } catch {
        /* ignore */
      }
    }
  }
}

/** Strip explicit point sizes from the selection and unwrap empty FONT/SPAN wrappers. */
function clearExplicitFontSizeInSelection(root: HTMLElement, rangeOverride?: Range | null): void {
  let range = rangeOverride ?? currentRangeInEditor(root);
  if (!range) return;

  // Split straddling Face/Size wrappers first so clear only hits the mid segment.
  // Bookmark + rebuild: extractContents invalidates the live Range endpoints.
  const bm = !range.collapsed ? bookmarkTextOffsets(root, range) : null;
  if (!range.collapsed) {
    splitFormatWrappersAtRangeBoundaries(root, range);
    if (bm) {
      const rebuilt = rangeFromTextOffsets(root, bm.start, bm.end);
      if (rebuilt) range = rebuilt;
    }
  }

  const clearNode = (node: HTMLElement) => {
    if (isProtectedInlineToken(node)) {
      // Chips are restyled from the Face/Size token snapshot after flatten; clear
      // sticky insert-time sizes so they do not stay stuck mid-apply.
      if (node.style.fontSize) node.style.removeProperty("font-size");
      return;
    }
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

  forEachInlineNodeInRange(root, range, (node) => {
    // Never flatten a wrapper that still extends past the highlight.
    // Own-contents (not selectNode) so word-select of a fully-wrapped run clears Size.
    if (!rangeMayClearFormatNode(range, node)) {
      return;
    }
    clearNode(node);
  });

  // Catch size wrappers that aren't ancestors of the range endpoints.
  if (!range.collapsed) {
    root.querySelectorAll("font, span[style], [style*='font-size']").forEach((node) => {
      if (!(node instanceof HTMLElement)) return;
      if (isProtectedInlineToken(node)) {
        if (!tokenTouchesRange(node, range)) return;
        clearNode(node);
        return;
      }
      try {
        if (!range.intersectsNode(node)) return;
      } catch {
        return;
      }
      // Skip placed/block containers — those use glyph-cover clear on default Size.
      if (node === blockContainer(node, root)) return;
      if (!rangeMayClearFormatNode(range, node)) return;
      clearNode(node);
    });
    // Flatten sticky chip sizes (contenteditable=false — often missed by span walkers).
    for (const token of collectTokensForFontCommand(root, range)) {
      clearNode(token);
    }
  }
}

/** Strip explicit faces from the selection so a new Face apply does not nest sticky families. */
function clearExplicitFontFaceInSelection(root: HTMLElement, rangeOverride?: Range | null): void {
  let range = rangeOverride ?? currentRangeInEditor(root);
  if (!range) return;

  const bm = !range.collapsed ? bookmarkTextOffsets(root, range) : null;
  if (!range.collapsed) {
    splitFormatWrappersAtRangeBoundaries(root, range);
    if (bm) {
      const rebuilt = rangeFromTextOffsets(root, bm.start, bm.end);
      if (rebuilt) range = rebuilt;
    }
  }

  const clearNode = (node: HTMLElement) => {
    if (isProtectedInlineToken(node)) {
      if (node.style.fontFamily) node.style.removeProperty("font-family");
      return;
    }
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
  };

  forEachInlineNodeInRange(root, range, (node) => {
    if (!rangeMayClearFormatNode(range, node)) return;
    clearNode(node);
  });

  if (!range.collapsed) {
    root.querySelectorAll("font, span[style], [style*='font-family']").forEach((node) => {
      if (!(node instanceof HTMLElement)) return;
      if (isProtectedInlineToken(node)) {
        if (!tokenTouchesRange(node, range)) return;
        clearNode(node);
        return;
      }
      try {
        if (!range.intersectsNode(node)) return;
      } catch {
        return;
      }
      if (node === blockContainer(node, root)) return;
      if (!rangeMayClearFormatNode(range, node)) return;
      clearNode(node);
    });
    for (const token of collectTokensForFontCommand(root, range)) {
      clearNode(token);
    }
  }
}

/** Visit inline formatting nodes intersecting `range` (or at a collapsed caret). */
function forEachInlineNodeInRange(
  root: HTMLElement,
  range: Range,
  visit: (el: HTMLElement) => void,
): void {
  const seen = new Set<HTMLElement>();
  const addAncestors = (node: Node | null) => {
    // Stop at the block container — do not clear/restyle a whole `.doc-placed-text`
    // (or Form `<p>`/`<div>`) when the owner only highlighted one word inside it.
    const block = node ? blockContainer(node, root) : root;
    let el: Node | null = node;
    if (el?.nodeType === Node.TEXT_NODE) el = el.parentNode;
    while (el && el instanceof HTMLElement && el !== root) {
      if (el === block) break;
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
    // Never treat the placed/block container as an "inline" face/size wrapper.
    if (node === blockContainer(node, root)) return;
    try {
      if (!range.intersectsNode(node)) return;
    } catch {
      return;
    }
    seen.add(node);
    visit(node);
  });
}

function rewriteLegacyFontSizeMarkers(
  root: HTMLElement,
  sizePt: string,
  preexisting: Set<Element>,
): HTMLElement[] {
  const rewritten: HTMLElement[] = [];

  const rewrite = (node: HTMLElement) => {
    if (!isChromeSizeSevenMarker(node)) return;
    node.removeAttribute("size");
    node.style.fontSize = `${sizePt}pt`;
    rewritten.push(node);
  };

  // Prefer newly created markers — do not depend on the post-command selection.
  // Chrome often collapses the highlight after fontSize, which made the old
  // intersectsNode / ancestor-only path miss `<font size="7">` and leave ~36pt.
  root.querySelectorAll("font, span").forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    if (preexisting.has(node)) return;
    if (!isChromeSizeSevenMarker(node)) return;
    rewrite(node);
  });

  if (rewritten.length > 0) return rewritten;

  // Re-apply onto leftover unre-written markers under the caret/selection
  // (e.g. a previous apply failed and left size="7").
  const range = currentRangeInEditor(root);
  if (!range) return rewritten;

  if (range.collapsed) {
    forEachInlineNodeInSelection(root, rewrite);
    return rewritten;
  }

  root.querySelectorAll("font, span").forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    if (!isChromeSizeSevenMarker(node)) return;
    try {
      if (!range.intersectsNode(node)) return;
    } catch {
      return;
    }
    rewrite(node);
  });
  return rewritten;
}

/**
 * Offsets of `text` that lie inside `range` (exclusive end).
 * Used by the controlled Size path — avoids execCommand("fontSize") boundary drift.
 *
 * Prefer intersectsNode + endpoint identity over compareBoundaryPoints: the latter
 * is inconsistent in happy-dom and is unnecessary when bookmark restore already
 * lands on text nodes.
 */
function selectedOffsetsInText(range: Range, text: Text): { start: number; end: number } | null {
  const len = text.data.length;
  if (len === 0) return null;
  try {
    if (!range.intersectsNode(text)) return null;
  } catch {
    return null;
  }

  let start = 0;
  let end = len;

  if (range.startContainer === text) {
    start = Math.min(Math.max(0, range.startOffset), len);
  } else if (range.startContainer.nodeType === Node.ELEMENT_NODE) {
    const parent = range.startContainer as Element;
    if (text.parentNode === parent) {
      const textIndex = Array.prototype.indexOf.call(parent.childNodes, text);
      // startOffset is the child index the range starts at (before that child).
      if (textIndex >= 0 && range.startOffset > textIndex) return null;
    }
  }

  if (range.endContainer === text) {
    end = Math.min(Math.max(0, range.endOffset), len);
  } else if (range.endContainer.nodeType === Node.ELEMENT_NODE) {
    const parent = range.endContainer as Element;
    if (text.parentNode === parent) {
      const textIndex = Array.prototype.indexOf.call(parent.childNodes, text);
      // endOffset is exclusive — range ends before that child index.
      if (textIndex >= 0 && range.endOffset <= textIndex) return null;
    }
  }

  if (start >= end) return null;
  return { start, end };
}

/**
 * Put `font-size: Npt` on an existing face/size wrapper owning only this text, or wrap
 * the text leaf in a fresh span. Preserves `face` / `font-family` on the parent.
 */
function ensureFontSizeOnTextRun(
  text: Text,
  sizePt: string,
  root: HTMLElement,
): HTMLElement | null {
  const parent = text.parentElement;
  if (!parent || !root.contains(parent)) return null;
  if (isProtectedInlineToken(parent)) return null;

  const block = blockContainer(text, root);
  const canPaintParent =
    (parent.tagName === "SPAN" || parent.tagName === "FONT") &&
    parent !== block &&
    parent !== root &&
    !isProtectedInlineToken(parent) &&
    // Anonymous format wrappers only — never paint `.doc-placed-text` / chip classes.
    !parent.className &&
    parent.childNodes.length === 1 &&
    parent.firstChild === text;

  if (canPaintParent) {
    parent.style.fontSize = `${sizePt}pt`;
    if (parent.tagName === "FONT") parent.removeAttribute("size");
    return parent;
  }

  const span = document.createElement("span");
  span.style.fontSize = `${sizePt}pt`;
  // Carry an explicit face onto the size wrapper so mid-highlight glyphs never
  // fall through to `.rich-surface` Arial if a parent Face span is remounted
  // around chips (owner SS4 right-of-selection → Arial).
  const inheritedFace = readExplicitFontFaceFromAncestry(text, root);
  if (inheritedFace && inheritedFace !== DEFAULT_PALETTE_FONT_FACE) {
    span.style.fontFamily = inheritedFace;
  } else if (parent.style.fontFamily) {
    span.style.fontFamily = parent.style.fontFamily;
  } else if (parent.tagName === "FONT" && parent.getAttribute("face")) {
    span.style.fontFamily = parent.getAttribute("face")!;
  }
  parent.insertBefore(span, text);
  span.appendChild(text);
  return span;
}

/**
 * Put `font-family` on an existing face/size wrapper owning only this text, or wrap
 * the text leaf. Avoids execCommand("fontName"), which splits mid-word around
 * contenteditable=false chips the same way fontSize does.
 */
function ensureFontFaceOnTextRun(
  text: Text,
  face: string,
  root: HTMLElement,
): HTMLElement | null {
  const parent = text.parentElement;
  if (!parent || !root.contains(parent)) return null;
  if (isProtectedInlineToken(parent)) return null;

  const block = blockContainer(text, root);
  const canPaintParent =
    (parent.tagName === "SPAN" || parent.tagName === "FONT") &&
    parent !== block &&
    parent !== root &&
    !isProtectedInlineToken(parent) &&
    !parent.className &&
    parent.childNodes.length === 1 &&
    parent.firstChild === text;

  if (canPaintParent) {
    parent.style.fontFamily = face;
    if (parent.tagName === "FONT") parent.removeAttribute("face");
    return parent;
  }

  const span = document.createElement("span");
  span.style.fontFamily = face;
  parent.insertBefore(span, text);
  span.appendChild(text);
  return span;
}

/**
 * Apply an explicit point size across `range` without `execCommand("fontSize")`.
 *
 * Why Size ≠ Face: Chrome's `fontSize` (legacy 1–7) splits existing `fontName`/
 * face wrappers mid-word when the highlight spans `contenteditable=false` field
 * chips and their ZWSP pads. Face (`fontName`) does the same — both use controlled
 * leaf painting now. Edge glyphs then fall outside the new size markers and revert
 * to default Arial/12 while the highlight appears mid-word (owner SS4 after Face→Size).
 *
 * Setting `style.fontSize` on fully-owned face wrappers (or wrapping only the
 * selected text leaves) keeps Face and sizes exactly the bookmarked span.
 */
export function applyFontSizePtAcrossRange(
  root: HTMLElement,
  range: Range,
  sizePt: string,
): HTMLElement[] {
  if (range.collapsed) return [];

  const texts: Text[] = [];
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (!text.data) continue;
    if (text.parentElement?.closest(".field-token, .function-token")) continue;
    try {
      if (!range.intersectsNode(text)) continue;
    } catch {
      continue;
    }
    texts.push(text);
  }

  const touched: HTMLElement[] = [];
  // Back-to-front so splitText does not invalidate later offsets in the list.
  // Include ZWSP/space pads so they carry the selection Size (re-drag sampling
  // skips them, but painted pads stay consistent if a walker regresses).
  for (let i = texts.length - 1; i >= 0; i--) {
    let text = texts[i]!;
    if (!text.isConnected) continue;
    if (!text.data) continue;

    const slice = selectedOffsetsInText(range, text);
    if (!slice) continue;

    if (slice.end < text.data.length) {
      text.splitText(slice.end);
    }
    if (slice.start > 0) {
      text = text.splitText(slice.start);
    }
    if (!text.data) continue;

    const sized = ensureFontSizeOnTextRun(text, sizePt, root);
    if (sized) touched.push(sized);
  }

  // Document order for restore — callers push back-to-front above.
  touched.reverse();
  return touched;
}

/**
 * Apply a font face across `range` without `execCommand("fontName")`.
 * Same leaf-split + wrap strategy as Size so chips mid-highlight do not chop
 * face wrappers mid-word.
 */
export function applyFontFaceAcrossRange(
  root: HTMLElement,
  range: Range,
  face: string,
): HTMLElement[] {
  if (range.collapsed) return [];

  const texts: Text[] = [];
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (!text.data) continue;
    if (text.parentElement?.closest(".field-token, .function-token")) continue;
    try {
      if (!range.intersectsNode(text)) continue;
    } catch {
      continue;
    }
    texts.push(text);
  }

  const touched: HTMLElement[] = [];
  // Paint ZWSP/space pads too — same rationale as Size (owner false Mixed on re-drag).
  for (let i = texts.length - 1; i >= 0; i--) {
    let text = texts[i]!;
    if (!text.isConnected) continue;
    if (!text.data) continue;

    const slice = selectedOffsetsInText(range, text);
    if (!slice) continue;

    if (slice.end < text.data.length) {
      text.splitText(slice.end);
    }
    if (slice.start > 0) {
      text = text.splitText(slice.start);
    }
    if (!text.data) continue;

    const faced = ensureFontFaceOnTextRun(text, face, root);
    if (faced) touched.push(faced);
  }

  touched.reverse();
  return touched;
}

/**
 * Re-select the runs we just styled so the highlight does not vanish after dropdown apply.
 * Always sort into document order (apply paths push then reverse, but callers may pass
 * mixed snapshots including chips). Expand across chips between first and last.
 */
function restoreSelectionOverNodes(root: HTMLElement, nodes: HTMLElement[]): void {
  const connected = nodes.filter((n) => n.isConnected && root.contains(n));
  if (connected.length === 0) return;
  connected.sort((a, b) => {
    const pos = a.compareDocumentPosition(b);
    if (pos & Node.DOCUMENT_POSITION_FOLLOWING) return -1;
    if (pos & Node.DOCUMENT_POSITION_PRECEDING) return 1;
    return 0;
  });
  const sel = window.getSelection();
  if (!sel) return;
  const first = connected[0]!;
  const last = connected[connected.length - 1]!;
  try {
    const range = document.createRange();
    range.setStartBefore(first);
    range.setEndAfter(last);
    expandRangeToTouchedTokens(root, range);
    sel.removeAllRanges();
    sel.addRange(range);
  } catch {
    /* DOM shape may not allow a single range — leave caret as-is */
  }
}

/**
 * Snapshot a Face/Size command range: expand over wholly touched chips + ZWSP pads,
 * then bookmark meaningful offsets. Live Range endpoints often skip CE=false chips.
 */
function snapshotFontCommandSelection(
  root: HTMLElement,
  live: Range,
): {
  applyRange: Range;
  bookmark: TextOffsetBookmark;
  tokens: HTMLElement[];
  touchBlocks: HTMLElement[];
  fullyCoveredBlocks: HTMLElement[];
} {
  const applyRange = live.cloneRange();
  expandRangeToTouchedTokens(root, applyRange);
  const bookmark = bookmarkTextOffsets(root, applyRange);
  const tokens = collectTokensForFontCommand(root, applyRange);
  const touchBlocks = placedBlocksTouchingRange(root, applyRange);
  const fullyCoveredBlocks = touchBlocks.filter((b) => rangeFullyCoversBlock(applyRange, b));
  return { applyRange, bookmark, tokens, touchBlocks, fullyCoveredBlocks };
}

export function emitPaletteActiveState(): void {
  const handle = getActivePaletteEditor();
  if (handle && !isBlankTypingContext(handle.el)) {
    // Keep typing attrs aligned with caret marks so the next keystroke / field insert
    // does not inherit a stale Comic/B/I choice from an earlier palette click.
    const marks = readInlineMarksAtCaret(handle.el);
    const sel = window.getSelection();
    const range =
      sel && sel.rangeCount > 0 && handle.el.contains(sel.getRangeAt(0).commonAncestorContainer)
        ? sel.getRangeAt(0)
        : null;

    if (range && !range.collapsed) {
      // Highlight: sync sticky typing only when the selection is uniform. Do not
      // overwrite Comic/20 with default Arial/12 just because startContainer is the
      // placed block (common after triple-click) while nested runs are styled.
      const face = selectionUniformFontFace(handle.el, range);
      const size = selectionUniformFontSize(handle.el, range);
      const color = selectionUniformFontColor(handle.el, range);
      const patch: Partial<ReturnType<typeof getTypingFormat>> = {
        bold: marks.bold,
        italic: marks.italic,
        underline: marks.underline,
      };
      if (face !== MIXED_PALETTE_VALUE) patch.fontFace = face;
      if (size !== MIXED_PALETTE_VALUE) patch.fontSize = size;
      if (color !== MIXED_PALETTE_VALUE) patch.color = color;
      setTypingFormat(handle.el, patch);
    } else {
      const face = readExplicitFontFace(handle.el) ?? DEFAULT_PALETTE_FONT_FACE;
      const pt = readExplicitFontSizePt(handle.el);
      const color = readExplicitFontColor(handle.el);
      setTypingFormat(handle.el, {
        bold: marks.bold,
        italic: marks.italic,
        underline: marks.underline,
        fontFace: face,
        fontSize: pt > 0 ? snapFontSizePt(pt) : String(DEFAULT_PALETTE_FONT_SIZE_PT),
        ...(color ? { color } : {}),
      });
    }
  }
  cachedPaletteActiveState = null;
  paletteActiveListeners.forEach((cb) => cb());
}

function snapFontSizePt(pt: number): string {
  // Do not collapse nearby palette stops into the default (the old ≤1.5pt band
  // around 12 swallowed 11 — and nearly 10 — so they looked and displayed as 12).
  if (pt <= 0) return String(DEFAULT_PALETTE_FONT_SIZE_PT);
  let best: number = FONT_SIZE_PT[0];
  let bestDiff = Infinity;
  for (const size of FONT_SIZE_PT) {
    const diff = Math.abs(size - pt);
    if (diff < bestDiff) {
      bestDiff = diff;
      best = size;
    }
  }
  // Prefer the nearest palette stop even when slightly off (computed px→pt noise).
  return String(bestDiff < Infinity ? best : DEFAULT_PALETTE_FONT_SIZE_PT);
}

/** Exported for unit tests — nearest palette point size for a raw pt measurement. */
export function snapFontSizePtForTest(pt: number): string {
  return snapFontSizePt(pt);
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
    if (node.style.fontSize) return parseInlineFontSizePt(node.style.fontSize);
    if (node.tagName === "FONT") {
      const legacy = node.getAttribute("size");
      if (legacy && legacy !== "3" && LEGACY_SIZE_TO_PT[legacy]) {
        return LEGACY_SIZE_TO_PT[legacy];
      }
    }
    node = node.parentNode;
  }
  if (block.classList.contains(PLACED_TEXT_CLASS) && block.style.fontSize) {
    return parseInlineFontSizePt(block.style.fontSize);
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

/**
 * Collapse walked face/size labels to a single palette value.
 * Empty → default; one distinct value → that value; otherwise Mixed.
 * Exported for unit tests (chip inheritance must not invent Mixed vs Arial/12).
 */
export function coalesceTypographicSamples(
  samples: string[],
  fallback: string,
): string | typeof MIXED_PALETTE_VALUE {
  if (samples.length === 0) return fallback;
  const unique = new Set(samples);
  if (unique.size === 1) return [...unique][0]!;
  return MIXED_PALETTE_VALUE;
}

/**
 * Face/size for a non-collapsed highlight.
 * Walks selected text runs **and** field/function chips (chips are often skipped
 * by text walkers when `contenteditable=false` / `user-select:none`).
 * Chips without their own face/size inherit adjacent run styles — they must not
 * report bare Arial/12 and force a false Mixed / "12 pt (default)" banner.
 * Important: do **not** fall back to `range.startContainer` alone — triple-click
 * often anchors on the `.doc-placed-text` block, which has no inline face/size
 * even when nested spans are Comic/20pt.
 */
function selectionUniformFontSize(editor: HTMLElement, range: Range): string | typeof MIXED_PALETTE_VALUE {
  const sizes: string[] = [];
  const glyphSizes: string[] = [];
  walkSelectionTypographicSamples(editor, range, (node) => {
    const size = readFontSizeLabelFromNode(node, editor);
    sizes.push(size);
    // Pads are already skipped; chips can still desync after remount / inherit races.
    if (!(node instanceof HTMLElement && isFieldOrFunctionToken(node))) {
      glyphSizes.push(size);
    }
  });
  const all = coalesceTypographicSamples(sizes, String(DEFAULT_PALETTE_FONT_SIZE_PT));
  if (all !== MIXED_PALETTE_VALUE) return all;
  // Owner re-drag (Comic+26 + field): every glyph run may agree while a chip sample
  // still disagrees — prefer the uniform glyph size over false Mixed.
  if (glyphSizes.length > 0) {
    const glyphs = coalesceTypographicSamples(glyphSizes, String(DEFAULT_PALETTE_FONT_SIZE_PT));
    if (glyphs !== MIXED_PALETTE_VALUE) return glyphs;
  }
  return all;
}

function selectionUniformFontFace(editor: HTMLElement, range: Range): string | typeof MIXED_PALETTE_VALUE {
  const faces: string[] = [];
  const glyphFaces: string[] = [];
  walkSelectionTypographicSamples(editor, range, (node) => {
    const face = readFontFaceLabelFromNode(node, editor);
    faces.push(face);
    if (!(node instanceof HTMLElement && isFieldOrFunctionToken(node))) {
      glyphFaces.push(face);
    }
  });
  const all = coalesceTypographicSamples(faces, DEFAULT_PALETTE_FONT_FACE);
  if (all !== MIXED_PALETTE_VALUE) return all;
  if (glyphFaces.length > 0) {
    const glyphs = coalesceTypographicSamples(glyphFaces, DEFAULT_PALETTE_FONT_FACE);
    if (glyphs !== MIXED_PALETTE_VALUE) return glyphs;
  }
  return all;
}

function readFontSizeLabel(): string {
  const handle = getActivePaletteEditor();
  if (!handle) return String(DEFAULT_PALETTE_FONT_SIZE_PT);

  const sel = window.getSelection();
  if (sel && sel.rangeCount > 0 && !sel.isCollapsed) {
    const range = sel.getRangeAt(0);
    if (handle.el.contains(range.commonAncestorContainer)) {
      return selectionUniformFontSize(handle.el, range);
    }
  }

  const pt = readExplicitFontSizePt(handle.el);
  if (pt > 0) {
    return snapFontSizePt(pt);
  }

  // Blank line only: show sticky typing size. On real glyphs, no explicit size ⇒ default.
  if (isBlankTypingContext(handle.el)) {
    return getTypingFormat(handle.el).fontSize;
  }
  return String(DEFAULT_PALETTE_FONT_SIZE_PT);
}

function readFontFaceLabel(): string {
  const handle = getActivePaletteEditor();
  if (!handle) return DEFAULT_PALETTE_FONT_FACE;

  const sel = window.getSelection();
  if (sel && sel.rangeCount > 0 && !sel.isCollapsed) {
    const range = sel.getRangeAt(0);
    if (handle.el.contains(range.commonAncestorContainer)) {
      return selectionUniformFontFace(handle.el, range);
    }
  }

  const face = readExplicitFontFace(handle.el);
  if (face) return face;

  // Blank line only: show sticky typing face. On real glyphs, no explicit face ⇒ default.
  if (isBlankTypingContext(handle.el)) {
    return getTypingFormat(handle.el).fontFace;
  }
  return DEFAULT_PALETTE_FONT_FACE;
}

function isFieldOrFunctionToken(el: HTMLElement): boolean {
  return el.classList.contains("field-token") || el.classList.contains("function-token");
}

/** Explicit face on ancestry between `node` and its block (no chip inheritance). */
function readExplicitFontFaceFromAncestry(node: Node, editor: HTMLElement): string | null {
  const block = blockContainer(node, editor);
  let el: Node | null = node.nodeType === Node.TEXT_NODE ? node.parentNode : node;
  while (el && el instanceof HTMLElement && el !== block) {
    // Chip wrapper styles are handled separately — skip nested walk into the badge.
    if (isFieldOrFunctionToken(el) && el !== node) break;
    if (el.style.fontFamily) return matchFontFace(el.style.fontFamily);
    if (el.tagName === "FONT" && el.getAttribute("face")) {
      return matchFontFace(el.getAttribute("face")!);
    }
    el = el.parentElement;
  }
  if (block.classList.contains(PLACED_TEXT_CLASS) && block.style.fontFamily) {
    return matchFontFace(block.style.fontFamily);
  }
  return null;
}

/** Explicit point size on ancestry between `node` and its block (no chip inheritance). */
function readExplicitFontSizeFromAncestry(node: Node, editor: HTMLElement): string | null {
  const block = blockContainer(node, editor);
  let el: Node | null = node.nodeType === Node.TEXT_NODE ? node.parentNode : node;
  while (el && el instanceof HTMLElement && el !== block) {
    if (isFieldOrFunctionToken(el) && el !== node) break;
    if (el.style.fontSize) {
      const pt = parseInlineFontSizePt(el.style.fontSize);
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
    return snapFontSizePt(parseInlineFontSizePt(block.style.fontSize));
  }
  return null;
}

/**
 * Inherit face/size from the nearest non-token sibling run when a chip has no
 * explicit typography of its own (avoids false Mixed vs Arial/12).
 * Skips ZWSP and other whitespace-only pads — those often sit directly under the
 * `.doc-placed-text` block and would report block TNR before the adjacent Comic
 * wrap is considered (owner SS3 false Mixed).
 */
function isIgnorableWhitespaceText(data: string): boolean {
  return !data.replace(/\u00a0|\u200b|\s/g, "").length;
}

function inheritTypographicFromAdjacentRuns(
  token: HTMLElement,
  editor: HTMLElement,
  kind: "face" | "size",
): string | null {
  const read = (n: Node) =>
    kind === "face"
      ? readExplicitFontFaceFromAncestry(n, editor)
      : readExplicitFontSizeFromAncestry(n, editor);

  for (const dir of ["prev", "next"] as const) {
    let sib: Node | null = dir === "prev" ? token.previousSibling : token.nextSibling;
    while (sib) {
      if (sib instanceof HTMLElement && isFieldOrFunctionToken(sib)) {
        sib = dir === "prev" ? sib.previousSibling : sib.nextSibling;
        continue;
      }
      if (sib.nodeType === Node.TEXT_NODE) {
        if (!isIgnorableWhitespaceText(sib.textContent ?? "")) {
          const hit = read(sib);
          if (hit) return hit;
        }
      } else if (sib instanceof HTMLElement) {
        // Prefer a meaningful text leaf inside a wrapper sibling (skip chip chrome).
        const walker = document.createTreeWalker(sib, NodeFilter.SHOW_TEXT);
        let text: Text | null;
        let foundInner = false;
        while ((text = walker.nextNode() as Text | null)) {
          if (isIgnorableWhitespaceText(text.textContent ?? "")) continue;
          if (text.parentElement?.closest(".field-token, .function-token")) continue;
          const inner = read(text);
          if (inner) {
            foundInner = true;
            return inner;
          }
        }
        if (!foundInner) {
          const hit = read(sib);
          if (hit) return hit;
        }
      }
      sib = dir === "prev" ? sib.previousSibling : sib.nextSibling;
    }
  }
  return null;
}

function placedBlockFace(editor: HTMLElement, node: Node): string | null {
  const block = blockContainer(node, editor);
  if (block.classList.contains(PLACED_TEXT_CLASS) && block.style.fontFamily) {
    return matchFontFace(block.style.fontFamily);
  }
  return null;
}

function readFontFaceLabelFromNode(node: Node, editor: HTMLElement): string {
  const token =
    node instanceof HTMLElement && isFieldOrFunctionToken(node)
      ? node
      : node instanceof HTMLElement
        ? node.closest(".field-token, .function-token")
        : node.parentElement?.closest(".field-token, .function-token");

  if (token instanceof HTMLElement) {
    const inherited = inheritTypographicFromAdjacentRuns(token, editor, "face");
    // Explicit chip style wins — unless it is a stale insert-time face (block or default)
    // that disagrees with a uniform adjacent Comic/etc. run (SS3 false Mixed).
    if (token.style.fontFamily) {
      const explicit = matchFontFace(token.style.fontFamily);
      const blockFace = placedBlockFace(editor, token);
      const staleInsert =
        inherited &&
        inherited !== explicit &&
        (explicit === DEFAULT_PALETTE_FONT_FACE ||
          (blockFace != null && explicit === blockFace));
      if (!staleInsert) return explicit;
      return inherited;
    }
    if (inherited) return inherited;
    return DEFAULT_PALETTE_FONT_FACE;
  }

  return readExplicitFontFaceFromAncestry(node, editor) ?? DEFAULT_PALETTE_FONT_FACE;
}

function readFontSizeLabelFromNode(node: Node, editor: HTMLElement): string {
  const token =
    node instanceof HTMLElement && isFieldOrFunctionToken(node)
      ? node
      : node instanceof HTMLElement
        ? node.closest(".field-token, .function-token")
        : node.parentElement?.closest(".field-token, .function-token");

  if (token instanceof HTMLElement) {
    const inherited = inheritTypographicFromAdjacentRuns(token, editor, "size");
    if (token.style.fontSize) {
      const pt = parseInlineFontSizePt(token.style.fontSize);
      if (pt > 0) {
        const explicit = snapFontSizePt(pt);
        const block = blockContainer(token, editor);
        const blockSize =
          block.classList.contains(PLACED_TEXT_CLASS) && block.style.fontSize
            ? snapFontSizePt(parseInlineFontSizePt(block.style.fontSize))
            : null;
        const staleInsert =
          inherited &&
          inherited !== explicit &&
          (explicit === String(DEFAULT_PALETTE_FONT_SIZE_PT) ||
            (blockSize != null && explicit === blockSize));
        if (!staleInsert) return explicit;
        return inherited;
      }
    }
    if (inherited) return inherited;
    return String(DEFAULT_PALETTE_FONT_SIZE_PT);
  }

  return readExplicitFontSizeFromAncestry(node, editor) ?? String(DEFAULT_PALETTE_FONT_SIZE_PT);
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
    // Must match chip-inherit: ZWSP-only was already skipped, but plain spaces /
    // `" \u200b"` pads under `.doc-placed-text` still report block TNR/18 and
    // force false Mixed next to a uniform Comic+26 run (owner re-drag after
    // Face→Size with a field). Glyph typography lives on non-whitespace leaves.
    if (isIgnorableWhitespaceText(node.textContent ?? "")) continue;
    visit(node);
  }
}

/** Text runs + field/function chips that participate in the highlight's typography. */
function walkSelectionTypographicSamples(
  editor: HTMLElement,
  range: Range,
  visit: (node: Node) => void,
): void {
  const seen = new Set<Node>();
  const add = (node: Node) => {
    if (seen.has(node)) return;
    seen.add(node);
    visit(node);
  };

  // Skip glyph text inside chips — the chip element is sampled once with inherit logic.
  // Whitespace pads are already excluded by walkRangeTextNodes.
  walkRangeTextNodes(range, (text) => {
    if (text.parentElement?.closest(".field-token, .function-token")) return;
    add(text);
  });

  editor.querySelectorAll(".field-token, .function-token").forEach((token) => {
    if (!(token instanceof HTMLElement)) return;
    if (!tokenTouchesRange(token, range)) return;
    add(token);
  });
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
  // Form Text: in-flow block. Document insert overrides to absolute via
  // `insertDocumentUserTable` (no float — float overlays absolute placed lines).
  table.style.position = "relative";
  table.style.width = `${widthPt}pt`;
  table.style.borderCollapse = "collapse";
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
    if (selectionInsideUserTable(handle.el)) {
      useProjectStore.getState().setStatus("Cannot insert a table inside another table");
      return;
    }

    const el = handle.el;
    const table = buildLegacyTableElement(widthPt, rows, columns);
    table.classList.add("user-border-1");

    // Document canvas: absolute stack with placed lines (never float — remount overlay).
    const isDocumentCanvas =
      !!el.closest(".document-editor") || !!el.querySelector(`.${PLACED_TEXT_CLASS}`);
    if (isDocumentCanvas) {
      insertDocumentUserTable(el, table);
    } else {
      const sel = window.getSelection();
      if (!sel || sel.rangeCount === 0) return;
      const range = sel.getRangeAt(0);
      if (!range.collapsed) range.deleteContents();

      const frag = document.createDocumentFragment();
      if (editorIsEmpty(el) || caretAtEditorStart(el)) {
        frag.appendChild(insertBlankParagraph());
      }
      frag.appendChild(table);
      frag.appendChild(insertBlankParagraph());
      range.insertNode(frag);
    }

    const firstCell = table.querySelector("td, th");
    if (firstCell) {
      const nextRange = document.createRange();
      nextRange.selectNodeContents(firstCell);
      nextRange.collapse(true);
      const sel = window.getSelection();
      if (sel) {
        sel.removeAllRanges();
        sel.addRange(nextRange);
      }
    }
  });
}

/** Border 1 / Border 2 / No Border — class on `table.user` (survives save). */
export function paletteTableBorder(style: TableBorderStyle): void {
  withEditor((handle) => {
    applyTableBorderStyle(handle.el, style);
  });
}

function tokenHasInlineMark(
  token: HTMLElement,
  mark: "bold" | "italic" | "underline",
): boolean {
  const style = token.style;
  if (mark === "bold") {
    const weight = style.fontWeight;
    if (weight === "bold" || weight === "bolder") return true;
    const n = Number.parseInt(weight, 10);
    return Number.isFinite(n) && n >= 600;
  }
  if (mark === "italic") {
    return style.fontStyle === "italic" || style.fontStyle === "oblique";
  }
  return `${style.textDecoration} ${style.textDecorationLine}`.toLowerCase().includes("underline");
}

/** True when the highlight has editable text outside field/function chips. */
function selectionHasNonTokenText(_root: HTMLElement, range: Range): boolean {
  if (range.collapsed) return false;
  try {
    const frag = range.cloneContents();
    const walker = document.createTreeWalker(frag, NodeFilter.SHOW_TEXT);
    let node = walker.nextNode();
    while (node) {
      const text = (node.textContent ?? "").replace(/\u00a0|\u200b/g, "");
      if (text.length > 0) {
        let el: Node | null = node.parentNode;
        let insideToken = false;
        while (el && el !== frag) {
          if (
            el instanceof HTMLElement &&
            (el.classList.contains("field-token") || el.classList.contains("function-token"))
          ) {
            insideToken = true;
            break;
          }
          el = el.parentNode;
        }
        if (!insideToken) return true;
      }
      node = walker.nextNode();
    }
  } catch {
    return true;
  }
  return false;
}

/**
 * Toggle B/I/U. Chips are contenteditable=false so execCommand skips them —
 * paint selected field/function tokens explicitly (owner: B/I/U apply to Fields).
 */
function paletteToggleInlineMark(command: "bold" | "italic" | "underline"): void {
  withEditor((handle) => {
    // Multi-cell drag highlight: toggle mark across every selected cell.
    if (
      forEachFormatTargetCell(handle.el, () => {
        document.execCommand(command);
      })
    ) {
      setTypingFormat(handle.el, { [command]: document.queryCommandState(command) });
      return;
    }

    const live = currentRangeInEditor(handle.el);
    const applyRange = live ? live.cloneRange() : null;
    const tokens = applyRange ? collectTokensForFontCommand(handle.el, applyRange) : [];
    const chipsOnly =
      !!applyRange &&
      !applyRange.collapsed &&
      tokens.length > 0 &&
      !selectionHasNonTokenText(handle.el, applyRange);

    let next: boolean;
    if (chipsOnly) {
      // Chip-only highlight (or chip alone): toggle — on if any chip lacks the mark.
      next = !tokens.every((t) => tokenHasInlineMark(t, command));
    } else {
      document.execCommand(command);
      next = readInlineMarksAtCaret(handle.el)[command];
    }

    setTypingFormat(handle.el, { [command]: next });
    if (tokens.length > 0) {
      const typing = getTypingFormat(handle.el);
      styleTokensList(tokens, { ...typing, [command]: next });
    }
  });
}

export function paletteBold(): void {
  paletteToggleInlineMark("bold");
}

export function paletteItalic(): void {
  paletteToggleInlineMark("italic");
}

export function paletteUnderline(): void {
  paletteToggleInlineMark("underline");
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
    indentSelection(handle.el, 1);
  });
}

export function paletteOutdent(): void {
  withEditor((handle) => {
    indentSelection(handle.el, -1);
  });
}

/**
 * Document placed lines / user tables, or Form paragraph margin.
 * Never uses execCommand("indent") — that wraps blockquotes and drops face/size on siblings.
 */
function indentSelection(editor: HTMLElement, delta: 1 | -1): boolean {
  const blocks = listPlacedBlocksInSelection(editor);
  const tables = listUserTablesInSelection(editor);
  if (blocks.length > 0 || tables.length > 0) {
    for (const block of blocks) {
      adjustPlacedTextIndent(editor, block, delta);
    }
    for (const table of tables) {
      adjustUserTableIndent(editor, table, delta);
    }
    // Full stack pack — indent narrows a line and can grow its height; packing only
    // from the indented block would leave an overlapping sibling above or below.
    // Table X moves also need a pack so same-column prose clears the new left edge.
    reflowAllPlacedLines(editor);
    return true;
  }

  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return true;
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) return true;

  let block: HTMLElement | null = blockContainer(range.startContainer, editor);
  if (block.classList.contains(PLACED_TEXT_CLASS)) return true;
  if (block instanceof HTMLTableCellElement) return true;

  if (block === editor) {
    block = ensureFormIndentBlock(editor, range.startContainer);
  }
  if (!block) return true;

  // Outdent: peel a leftover blockquote from execCommand("indent") without rewriting spans.
  if (delta < 0 && block.tagName === "BLOCKQUOTE") {
    const parent = block.parentNode;
    if (parent) {
      while (block.firstChild) parent.insertBefore(block.firstChild, block);
      parent.removeChild(block);
    }
    return true;
  }

  const current = parseCssPt(block.style.marginLeft);
  const next = Math.max(0, current + delta * DOC_INDENT_STEP_PT);
  if (next <= 0) block.style.removeProperty("margin-left");
  else block.style.marginLeft = formatPt(next);
  return true;
}

const FORM_INDENT_INLINE = new Set([
  "SPAN",
  "FONT",
  "B",
  "STRONG",
  "I",
  "EM",
  "U",
  "A",
  "LABEL",
]);

/**
 * When the caret sits in a flat contenteditable (no P/DIV yet), wrap the current
 * soft line in a DIV so margin-left indent does not need execCommand("indent").
 */
function ensureFormIndentBlock(editor: HTMLElement, start: Node): HTMLElement | null {
  let node: Node | null = start.nodeType === Node.TEXT_NODE ? start.parentNode : start;
  while (node && node.parentNode && node.parentNode !== editor) {
    node = node.parentNode;
  }
  if (!(node instanceof HTMLElement) || node === editor) return null;

  if (
    node.tagName === "P" ||
    node.tagName === "DIV" ||
    node.tagName === "LI" ||
    node.tagName === "BLOCKQUOTE"
  ) {
    return node;
  }

  if (!FORM_INDENT_INLINE.has(node.tagName) && node.tagName !== "BR") {
    return node;
  }

  // Collect this soft line (siblings until BR / block boundary) into a DIV.
  const parent = editor;
  const lineNodes: Node[] = [];
  let cur: Node | null = node;
  while (cur && cur.previousSibling) {
    const prev: Node = cur.previousSibling;
    if (prev instanceof HTMLElement) {
      if (prev.tagName === "BR") break;
      if (prev.tagName === "DIV" || prev.tagName === "P" || prev.tagName === "BLOCKQUOTE") break;
    }
    cur = prev;
  }
  while (cur) {
    if (cur instanceof HTMLElement) {
      if (cur.tagName === "BR") break;
      if (cur.tagName === "DIV" || cur.tagName === "P" || cur.tagName === "BLOCKQUOTE") break;
    }
    lineNodes.push(cur);
    cur = cur.nextSibling;
  }
  if (!lineNodes.length) return null;

  const wrap = document.createElement("div");
  parent.insertBefore(wrap, lineNodes[0]);
  for (const n of lineNodes) wrap.appendChild(n);
  return wrap;
}

/** Form rich-text paragraph/div blocks that intersect the selection (not Document placed lines). */
function listFormBlocksInSelection(editor: HTMLElement): HTMLElement[] {
  // Document canvas uses `.doc-placed-text` — never treat those as Form blocks.
  if (editor.querySelector(`.${PLACED_TEXT_CLASS}`)) return [];
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0 || !editor.contains(sel.getRangeAt(0).commonAncestorContainer)) {
    return [];
  }
  const range = sel.getRangeAt(0);
  const top = Array.from(editor.children).filter(
    (n): n is HTMLElement =>
      n instanceof HTMLElement &&
      (n.tagName === "P" || n.tagName === "DIV" || n.tagName === "LI") &&
      !n.closest("table.user"),
  );
  const hit = top.filter((block) => {
    try {
      return range.intersectsNode(block);
    } catch {
      return false;
    }
  });
  if (hit.length) return hit;
  // Flat caret with no block yet — wrap the soft line so align sticks.
  const wrapped = ensureFormIndentBlock(editor, range.startContainer);
  return wrapped ? [wrapped] : [];
}

export function paletteAlign(dir: PaletteActiveState["align"]): void {
  withEditor((handle) => {
    const cssAlign = dir === "justify" ? "justify" : dir;
    // Highlighted cells first (like Bold): never execCommand across the table/row.
    if (applyAlignToSelectedTableCells(handle.el, cssAlign)) {
      return;
    }
    // Caret in a cell with no multi-highlight — align that cell only.
    const cell = getSelectedCell(handle.el);
    if (cell) {
      cell.style.textAlign = cssAlign;
      for (const child of Array.from(cell.children)) {
        if (child instanceof HTMLElement) child.style.textAlign = cssAlign;
      }
      return;
    }
    const blocks = listPlacedBlocksInSelection(handle.el);
    if (blocks.length > 0) {
      for (const block of blocks) {
        alignPlacedTextBlock(handle.el, block, dir);
      }
      reflowAllPlacedLines(handle.el);
      return;
    }
    const formBlocks = listFormBlocksInSelection(handle.el);
    if (formBlocks.length > 0) {
      for (const block of formBlocks) {
        if (dir === "left") stripLeadingWhitespaceForLeftAlign(block);
        block.style.textAlign = cssAlign;
        if (dir === "left") delete block.dataset.docAlign;
        else block.dataset.docAlign = dir;
      }
      return;
    }
    document.execCommand(ALIGN_COMMAND[dir]);
  });
}

/**
 * True when a field/function token lies inside (or on the boundary of) the selection.
 * `intersectsNode` alone can miss `contenteditable=false` chips when the range is built
 * from surrounding text nodes only.
 */
function tokenTouchesRange(token: HTMLElement, range: Range): boolean {
  try {
    if (range.intersectsNode(token)) return true;
  } catch {
    /* detached / document mismatch */
  }
  try {
    const tr = document.createRange();
    tr.selectNode(token);
    // Selection starts before token ends AND ends after token starts.
    return (
      range.compareBoundaryPoints(Range.END_TO_START, tr) > 0 &&
      range.compareBoundaryPoints(Range.START_TO_END, tr) < 0
    );
  } catch {
    return false;
  }
}

/**
 * Collect field/function chips that belong to a font Face/Size command selection.
 * Snapshot element refs up front: `execCommand` often collapses the live range, and
 * `restoreSelectionOverNodes` only covers rewritten text spans (chips between runs
 * would otherwise miss restyle after repeated face/size changes).
 */
function collectTokensForFontCommand(root: HTMLElement, range: Range): HTMLElement[] {
  const seen = new Set<HTMLElement>();
  const add = (el: HTMLElement) => {
    if (el.classList.contains("field-token") || el.classList.contains("function-token")) {
      seen.add(el);
    }
  };

  root.querySelectorAll(".field-token, .function-token").forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    if (!tokenTouchesRange(node, range)) return;
    add(node);
  });

  root.querySelectorAll(`.${PLACED_TEXT_CLASS}`).forEach((block) => {
    if (!(block instanceof HTMLElement)) return;
    try {
      if (!range.intersectsNode(block)) return;
      const contents = document.createRange();
      contents.selectNodeContents(block);
      const covers =
        range.compareBoundaryPoints(Range.START_TO_START, contents) <= 0 &&
        range.compareBoundaryPoints(Range.END_TO_END, contents) >= 0;
      if (!covers) return;
    } catch {
      return;
    }
    block.querySelectorAll(".field-token, .function-token").forEach((node) => {
      if (node instanceof HTMLElement) add(node);
    });
  });

  return [...seen];
}

function styleTokensList(tokens: HTMLElement[], typing: ReturnType<typeof getTypingFormat>): void {
  for (const el of tokens) {
    if (!el.isConnected) continue;
    applyTypingFormatToToken(el, typing);
  }
}

function styleTokensInSelection(
  root: HTMLElement,
  typing: ReturnType<typeof getTypingFormat>,
  rangeOverride?: Range | null,
): void {
  const range = rangeOverride ?? currentRangeInEditor(root);
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
    // Collapsed caret next to a chip: sizing the “current run” should still hit the
    // adjacent field/function token in the same placed line / parent.
    const parent =
      range.startContainer.nodeType === Node.TEXT_NODE
        ? range.startContainer.parentElement
        : range.startContainer instanceof HTMLElement
          ? range.startContainer
          : null;
    const block = parent ? parent.closest(`.${PLACED_TEXT_CLASS}, td, th, p, div`) : null;
    const scope = block instanceof HTMLElement ? block : parent;
    scope?.querySelectorAll(".field-token, .function-token").forEach((node) => {
      if (!(node instanceof HTMLElement)) return;
      const prev = node.previousSibling;
      const next = node.nextSibling;
      if (prev === range.startContainer || next === range.startContainer) styleOne(node);
      if (
        prev?.nodeType === Node.TEXT_NODE &&
        range.startContainer === prev &&
        range.startOffset >= (prev.textContent?.length ?? 0)
      ) {
        styleOne(node);
      }
      if (next?.nodeType === Node.TEXT_NODE && range.startContainer === next && range.startOffset === 0) {
        styleOne(node);
      }
    });
    return;
  }

  styleTokensList(collectTokensForFontCommand(root, range), typing);
}

/** True when `range` covers every content node of a placed/block element. */
function rangeFullyCoversBlock(range: Range, block: HTMLElement): boolean {
  try {
    const contents = document.createRange();
    contents.selectNodeContents(block);
    return (
      range.compareBoundaryPoints(Range.START_TO_START, contents) <= 0 &&
      range.compareBoundaryPoints(Range.END_TO_END, contents) >= 0
    );
  } catch {
    return false;
  }
}

/**
 * True when every meaningful glyph (and chip) in a placed line lies inside `range`.
 * Word-select of "Left" on a one-word invent line covers the text leaf but not the
 * `.doc-placed-text` element itself (`rangeFullyContainsNode` / `selectNode` fails),
 * and a trailing `<br>` also breaks `rangeFullyCoversBlock`.
 */
function selectionCoversAllPlacedGlyphs(range: Range, block: HTMLElement): boolean {
  if (rangeFullyCoversBlock(range, block)) return true;

  let sawGlyph = false;
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (text.parentElement?.closest(".field-token, .function-token")) continue;
    if (isIgnorableWhitespaceText(text.data)) continue;
    sawGlyph = true;
    if (!rangeFullyContainsNode(range, text)) return false;
  }
  for (const token of block.querySelectorAll(".field-token, .function-token")) {
    if (!(token instanceof HTMLElement)) continue;
    sawGlyph = true;
    if (!rangeFullyContainsNode(range, token)) return false;
  }
  return sawGlyph;
}

/**
 * Default Face/Size clears inline wrappers only — invent + sticky typing paints
 * face/size on the `.doc-placed-text` block itself (common left-of-table). When the
 * highlight covers every glyph on that line, also strip the block so Arial/12 is
 * not ignored. Never strip a multi-word line for a one-word default.
 */
function clearDefaultTypographyOnGlyphCoveredPlacedBlocks(
  root: HTMLElement,
  range: Range | null,
  opts: { face?: boolean; size?: boolean },
  fullyCoveredBlocks: HTMLElement[] = [],
): void {
  if (!range || range.collapsed) return;
  if (!opts.face && !opts.size) return;

  const candidates = new Set<HTMLElement>();
  for (const block of fullyCoveredBlocks) {
    if (block.classList.contains(PLACED_TEXT_CLASS)) candidates.add(block);
  }
  for (const block of placedBlocksTouchingRange(root, range)) {
    if (selectionCoversAllPlacedGlyphs(range, block)) candidates.add(block);
  }

  for (const block of candidates) {
    if (opts.face && block.style.fontFamily) {
      block.style.removeProperty("font-family");
    }
    if (opts.size && block.style.fontSize) {
      block.style.removeProperty("font-size");
    }
  }
}

/**
 * Exported for unit tests — Face/Size must not expand a one-word highlight to a
 * whole `.doc-placed-text` line via "restore via placed blocks".
 */
export function selectionCoversFullPlacedBlockForTest(
  range: Range,
  block: HTMLElement,
): boolean {
  return rangeFullyCoversBlock(range, block);
}

/**
 * Re-select after Face/Size flattening mutates the DOM.
 * Prefer text-offset bookmarks (survive unwrap / fontSize rewrite). Never re-apply
 * a pre-mutation `Range` — its containers often stay "connected" but point at the
 * wrong fragment (lost highlight, last-word-only, or ghost range for the next Face).
 * Only fall back to `selectNodeContents` on placed blocks when the original highlight
 * fully covered those blocks. Never expand a partial one-word highlight to the
 * whole placed paragraph.
 *
 * Normalize field/function caret landings first: Face/Size wraps often absorb the
 * ZWSP pad into a font/span; without extracting it, the next ensure stacks another
 * pad and the highlighted end creeps inward by ~2 chars per chip each Size.
 */
function restoreHighlightAfterFontMutation(
  root: HTMLElement,
  blocks: HTMLElement[],
  bookmark: TextOffsetBookmark | null,
  options?: {
    rewritten?: HTMLElement[];
    fullyCoveredBlocks?: HTMLElement[];
  },
): void {
  ensureFieldTokenCaretGaps(root);
  ensureFunctionTokenCaretGaps(root);

  if (bookmark && restoreSelectionFromBookmark(root, bookmark)) return;

  if (options?.rewritten && options.rewritten.length > 0) {
    restoreSelectionOverNodes(root, options.rewritten);
    return;
  }

  const fullyCovered = (options?.fullyCoveredBlocks ?? []).filter(
    (b) => b.isConnected && root.contains(b),
  );
  if (fullyCovered.length === 0) {
    // Legacy path: only expand to blocks when we know they were fully covered
    // before the mutation (caller must pass fullyCoveredBlocks).
    void blocks;
    return;
  }
  const sel = window.getSelection();
  if (!sel) return;
  try {
    const range = document.createRange();
    if (fullyCovered.length === 1) {
      range.selectNodeContents(fullyCovered[0]!);
    } else {
      range.setStart(fullyCovered[0]!, 0);
      const last = fullyCovered[fullyCovered.length - 1]!;
      range.setEnd(last, last.childNodes.length);
    }
    expandRangeToTouchedTokens(root, range);
    sel.removeAllRanges();
    sel.addRange(range);
  } catch {
    /* leave selection as-is */
  }
}

function placedBlocksTouchingRange(root: HTMLElement, range: Range): HTMLElement[] {
  return Array.from(root.querySelectorAll(`.${PLACED_TEXT_CLASS}`)).filter(
    (block): block is HTMLElement => {
      if (!(block instanceof HTMLElement)) return false;
      try {
        return range.intersectsNode(block);
      } catch {
        return false;
      }
    },
  );
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

/** Explicit color on ancestry between `node` and its block (chips use their own style). */
function readExplicitFontColorFromAncestry(node: Node, editor: HTMLElement): string | null {
  const token =
    node instanceof HTMLElement && isFieldOrFunctionToken(node)
      ? node
      : node instanceof HTMLElement
        ? node.closest(".field-token, .function-token")
        : node.parentElement?.closest(".field-token, .function-token");
  if (token instanceof HTMLElement && token.style.color) {
    return rgbToHex(token.style.color) ?? token.style.color;
  }

  const block = blockContainer(node, editor);
  let el: Node | null = node.nodeType === Node.TEXT_NODE ? node.parentNode : node;
  while (el && el instanceof HTMLElement && el !== block) {
    if (isFieldOrFunctionToken(el) && el !== node) break;
    if (el.style.color) return rgbToHex(el.style.color) ?? el.style.color;
    if (el.tagName === "FONT" && el.getAttribute("color")) {
      const raw = el.getAttribute("color")!;
      return rgbToHex(raw) ?? raw;
    }
    el = el.parentElement;
  }
  if (block.classList.contains(PLACED_TEXT_CLASS) && block.style.color) {
    return rgbToHex(block.style.color) ?? block.style.color;
  }
  return null;
}

function readExplicitFontColor(editor: HTMLElement): string | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  const range = sel.getRangeAt(0);
  if (!editor.contains(range.commonAncestorContainer)) return null;
  return readExplicitFontColorFromAncestry(range.startContainer, editor);
}

function selectionUniformFontColor(
  editor: HTMLElement,
  range: Range,
): string | typeof MIXED_PALETTE_VALUE {
  const colors: string[] = [];
  const glyphColors: string[] = [];
  walkSelectionTypographicSamples(editor, range, (node) => {
    const color = readExplicitFontColorFromAncestry(node, editor) ?? "#000000";
    colors.push(color);
    if (!(node instanceof HTMLElement && isFieldOrFunctionToken(node))) {
      glyphColors.push(color);
    }
  });
  const all = coalesceTypographicSamples(colors, "#000000");
  if (all !== MIXED_PALETTE_VALUE) return all;
  if (glyphColors.length > 0) {
    const glyphs = coalesceTypographicSamples(glyphColors, "#000000");
    if (glyphs !== MIXED_PALETTE_VALUE) return glyphs;
  }
  return all;
}

function readFontColorLabel(): string {
  const handle = getActivePaletteEditor();
  if (!handle) return "#000000";

  const sel = window.getSelection();
  if (sel && sel.rangeCount > 0 && !sel.isCollapsed) {
    const range = sel.getRangeAt(0);
    if (handle.el.contains(range.commonAncestorContainer)) {
      const uniform = selectionUniformFontColor(handle.el, range);
      if (uniform !== MIXED_PALETTE_VALUE) return uniform;
      // Mixed run colors: keep sticky typing so the swatch does not flicker to black.
      const typingMixed = getTypingFormat(handle.el).color;
      if (typingMixed) return rgbToHex(typingMixed) ?? typingMixed;
    }
  }

  const explicit = readExplicitFontColor(handle.el);
  if (explicit) return explicit;

  const typing = getTypingFormat(handle.el);
  if (typing.color) {
    return rgbToHex(typing.color) ?? typing.color;
  }

  try {
    const raw = String(document.queryCommandValue("foreColor") || "");
    const hex = rgbToHex(raw);
    if (hex) return hex;
  } catch {
    /* ignore */
  }

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

    if (
      forEachFormatTargetCell(handle.el, () => {
        document.execCommand("foreColor", false, hex);
      })
    ) {
      return;
    }

    const sel = window.getSelection();
    const range =
      sel && sel.rangeCount > 0 && handle.el.contains(sel.getRangeAt(0).commonAncestorContainer)
        ? sel.getRangeAt(0)
        : null;
    const hasHighlight = !!(range && !range.collapsed);

    if (hasHighlight && range) {
      // Recolor only the highlight — do not set the whole Document line's color.
      const applyRange = range.cloneRange();
      expandRangeToTouchedTokens(handle.el, applyRange);
      document.execCommand("foreColor", false, hex);

      // Tokens are contenteditable=false; paint any that intersect the highlight.
      handle.el.querySelectorAll(".field-token, .function-token").forEach((node) => {
        if (!(node instanceof HTMLElement)) return;
        if (!tokenTouchesRange(node, applyRange)) return;
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
    if (
      forEachFormatTargetCell(handle.el, () => {
        const sel = window.getSelection();
        if (!sel?.rangeCount) return;
        const range = sel.getRangeAt(0);
        applyFontFaceAcrossRange(handle.el, range, face);
      })
    ) {
      return;
    }
    const typing = getTypingFormat(handle.el);
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      applyTypingFormatToPlacedBlock(placed, typing);
      styleTokensInSelection(handle.el, typing);
      return;
    }

    const live = currentRangeInEditor(handle.el);
    const hadHighlight = !!(live && !live.collapsed);
    const snap = live && hadHighlight ? snapshotFontCommandSelection(handle.el, live) : null;
    const applyRange = snap?.applyRange ?? (live ? live.cloneRange() : null);
    const bookmark = snap?.bookmark ?? null;
    const tokens = snap?.tokens ?? (applyRange ? collectTokensForFontCommand(handle.el, applyRange) : []);
    const touchBlocks = snap?.touchBlocks ?? [];
    const fullyCoveredBlocks = snap?.fullyCoveredBlocks ?? [];

    // Flatten sticky faces before (re)applying so repeated Face changes do not nest.
    if (applyRange) clearExplicitFontFaceInSelection(handle.el, applyRange);
    if (hadHighlight) {
      restoreHighlightAfterFontMutation(handle.el, touchBlocks, bookmark, {
        fullyCoveredBlocks,
      });
    }

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
      // Block-level sticky Face (invent left-of-table) — clear when the whole line is covered.
      const faceCoverRange =
        (bookmark ? rangeFromTextOffsets(handle.el, bookmark.start, bookmark.end) : null) ??
        currentRangeInEditor(handle.el) ??
        applyRange;
      clearDefaultTypographyOnGlyphCoveredPlacedBlocks(
        handle.el,
        faceCoverRange,
        { face: true },
        fullyCoveredBlocks,
      );
      styleTokensList(tokens, typing);
      if (hadHighlight) {
        restoreHighlightAfterFontMutation(handle.el, touchBlocks, bookmark, {
          fullyCoveredBlocks,
        });
      }
      return;
    }

    if (hadHighlight) {
      // Controlled Face — never execCommand("fontName") on highlights spanning chips.
      const faceRange =
        (bookmark ? rangeFromTextOffsets(handle.el, bookmark.start, bookmark.end) : null) ??
        currentRangeInEditor(handle.el) ??
        applyRange;
      const rewritten = faceRange
        ? applyFontFaceAcrossRange(handle.el, faceRange, face)
        : [];
      styleTokensList(tokens, typing);
      restoreHighlightAfterFontMutation(handle.el, touchBlocks, bookmark, {
        rewritten: [...rewritten, ...tokens],
        fullyCoveredBlocks,
      });
      return;
    }

    // Collapsed caret: legacy fontName so the next typed characters pick up the face.
    document.execCommand("fontName", false, face);
    styleTokensList(tokens, typing);
  });
}

/**
 * Apply a point size.
 *
 * Highlight path (non-collapsed): controlled `applyFontSizePtAcrossRange` — never
 * `execCommand("fontSize")`. Highlight Face likewise uses `applyFontFaceAcrossRange`
 * (never `fontName`). Both split Face/Size wrappers mid-word around field chips when
 * driven by execCommand (SS4: Size after Face). Collapsed caret still uses legacy
 * markers so the next typed characters pick up the size.
 *
 * Non-blank Document lines: size applies only to the selection (or next typed chars),
 * not the whole `.doc-placed-text` block — but field/function chips inside the
 * selection (or a fully selected placed line) get the same point size so they do
 * not stay stuck at insert-time size. Ranges are expanded over wholly touched chips
 * + ZWSP pads before apply/restore.
 */
export function paletteFontSize(size: string): void {
  withEditor((handle) => {
    setTypingFormat(handle.el, { fontSize: size });
    if (
      forEachFormatTargetCell(handle.el, () => {
        const sel = window.getSelection();
        if (!sel?.rangeCount) return;
        const range = sel.getRangeAt(0);
        if (size === String(DEFAULT_PALETTE_FONT_SIZE_PT)) {
          clearExplicitFontSizeInSelection(handle.el, range);
        } else {
          applyFontSizePtAcrossRange(handle.el, range, size);
        }
      })
    ) {
      return;
    }
    const typing = getTypingFormat(handle.el);
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      applyTypingFormatToPlacedBlock(placed, typing);
      styleTokensInSelection(handle.el, typing);
      reflowPlacedLinesBelow(handle.el, placed);
      return;
    }

    const live = currentRangeInEditor(handle.el);
    const hadHighlight = !!(live && !live.collapsed);
    const snap = live && hadHighlight ? snapshotFontCommandSelection(handle.el, live) : null;
    const applyRange = snap?.applyRange ?? (live ? live.cloneRange() : null);
    const bookmark = snap?.bookmark ?? null;
    const tokens = snap?.tokens ?? (applyRange ? collectTokensForFontCommand(handle.el, applyRange) : []);
    const touchBlocks = snap?.touchBlocks ?? [];
    const fullyCoveredBlocks = snap?.fullyCoveredBlocks ?? [];

    if (size === String(DEFAULT_PALETTE_FONT_SIZE_PT)) {
      if (applyRange) clearExplicitFontSizeInSelection(handle.el, applyRange);
      // Block-level sticky Size (invent left-of-table) — clear when the whole line is covered.
      // Commit also strips redundant 12pt markers; block 20pt must go here or defaults no-op.
      const sizeCoverRange =
        (bookmark ? rangeFromTextOffsets(handle.el, bookmark.start, bookmark.end) : null) ??
        currentRangeInEditor(handle.el) ??
        applyRange;
      clearDefaultTypographyOnGlyphCoveredPlacedBlocks(
        handle.el,
        sizeCoverRange,
        { size: true },
        fullyCoveredBlocks,
      );
      // Default size: clear sticky chip sizes so they inherit the surrounding run again.
      styleTokensList(tokens, typing);
      if (hadHighlight) {
        restoreHighlightAfterFontMutation(handle.el, touchBlocks, bookmark, {
          fullyCoveredBlocks,
        });
      }
      if (placed) reflowPlacedLinesBelow(handle.el, placed);
      return;
    }

    // Flatten prior point sizes so repeated Size changes do not nest sticky wrappers.
    if (applyRange) clearExplicitFontSizeInSelection(handle.el, applyRange);
    if (hadHighlight) {
      restoreHighlightAfterFontMutation(handle.el, touchBlocks, bookmark, {
        fullyCoveredBlocks,
      });
    }

    if (hadHighlight) {
      // Prefer a bookmark-rebuilt range so Size hits the same Face span even when
      // Chrome collapsed or nudged the live selection after flatten.
      const sizeRange =
        (bookmark ? rangeFromTextOffsets(handle.el, bookmark.start, bookmark.end) : null) ??
        currentRangeInEditor(handle.el) ??
        applyRange;
      const rewritten = sizeRange
        ? applyFontSizePtAcrossRange(handle.el, sizeRange, size)
        : [];
      // Chips are contenteditable=false — paint size (and sticky face) explicitly.
      styleTokensList(tokens, typing);
      restoreHighlightAfterFontMutation(handle.el, touchBlocks, bookmark, {
        rewritten: [...rewritten, ...tokens],
        fullyCoveredBlocks,
      });
    } else {
      // Collapsed caret: legacy marker so the browser's next-insert size matches.
      const preexisting = new Set<Element>();
      handle.el.querySelectorAll("font, span").forEach((node) => {
        if (node instanceof HTMLElement && isChromeSizeSevenMarker(node)) {
          preexisting.add(node);
        }
      });
      document.execCommand("fontSize", false, "7");
      rewriteLegacyFontSizeMarkers(handle.el, size, preexisting);
    }
    if (placed) reflowPlacedLinesBelow(handle.el, placed);
  }, false);
}

/** Delete the entire table containing the caret (with confirm). */
export function paletteDeleteTable(): void {
  withEditor((handle) => {
    const cell = getSelectedCell(handle.el);
    const table = cell?.closest("table");
    if (table && handle.el.contains(table)) {
      const ok = window.confirm("Are you sure you want to delete this table?");
      if (!ok) return;
      table.remove();
      if (handle.el.querySelector(`.${PLACED_TEXT_CLASS}`)) {
        reflowAllPlacedLines(handle.el);
      }
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
  const marks = handle && !blank ? readInlineMarksAtCaret(handle.el) : null;

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
  } else if (handle) {
    const formBlocks = listFormBlocksInSelection(handle.el);
    if (formBlocks.length === 1) {
      align = readPlacedTextAlign(formBlocks[0]!);
    } else if (formBlocks.length > 1) {
      const first = readPlacedTextAlign(formBlocks[0]!);
      align = formBlocks.every((b) => readPlacedTextAlign(b) === first) ? first : "left";
    } else if (query("justifyCenter")) align = "center";
    else if (query("justifyRight")) align = "right";
    else if (query("justifyFull")) align = "justify";
  } else if (query("justifyCenter")) align = "center";
  else if (query("justifyRight")) align = "right";
  else if (query("justifyFull")) align = "justify";
  return {
    // Prefer ancestor marks over queryCommandState — the latter stays true across
    // plain runs after B/I boundaries (caret in "everywhere" with B+I still lit).
    bold: blank ? typing.bold : (marks?.bold ?? false),
    italic: blank ? typing.italic : (marks?.italic ?? false),
    underline: blank ? typing.underline : (marks?.underline ?? false),
    align,
    fontFace: readFontFaceLabel(),
    fontSize: readFontSizeLabel(),
    color: readFontColorLabel(),
  };
}
