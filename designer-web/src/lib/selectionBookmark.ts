/**
 * Text-offset selection bookmarks that survive face/size DOM rewrites.
 * Live `Range` containers often detach or collapse after unwrap / `fontSize`
 * execCommand; character offsets in the editor root remain stable when only
 * wrapper markup changes.
 *
 * Offsets ignore caret-landing ZWSP (`\u200B`) so Face/Size passes that wrap
 * or re-insert landings around field chips do not shift the logical end inward
 * by ~2 chars per chip on every apply. A text snapshot repairs restores when
 * length still drifts. Restore expands to absorb adjacent ZWSP pads so the
 * next Size still covers the full original glyph span.
 */

import { CARET_ZWSP } from "./tokenCaretLanding";

export interface TextOffsetBookmark {
  start: number;
  end: number;
  /** Meaningful text between start/end (ZWSP stripped) for restore verification. */
  text: string;
}

const TEXT_NODE = 3;

function meaningfulText(data: string): string {
  return data.replaceAll(CARET_ZWSP, "");
}

function meaningfulLength(data: string): number {
  return meaningfulText(data).length;
}

/** Map a DOM text offset → meaningful (ZWSP-skipped) offset within the same node. */
function domOffsetToMeaningful(data: string, domOffset: number): number {
  const clamped = Math.min(Math.max(0, domOffset), data.length);
  return meaningfulText(data.slice(0, clamped)).length;
}

/**
 * Map a meaningful offset → DOM offset within `data` (prefer landing just after
 * the Nth meaningful character / at end).
 */
function meaningfulToDomOffset(data: string, meaningfulOffset: number): number {
  if (meaningfulOffset <= 0) return 0;
  let seen = 0;
  for (let i = 0; i < data.length; i++) {
    if (data[i] === CARET_ZWSP) continue;
    seen += 1;
    if (seen >= meaningfulOffset) return i + 1;
  }
  return data.length;
}

function nodeTextLength(node: Node): number {
  if (node.nodeType === TEXT_NODE) return meaningfulLength(node.textContent ?? "");
  let total = 0;
  for (let i = 0; i < node.childNodes.length; i++) {
    total += nodeTextLength(node.childNodes[i]!);
  }
  return total;
}

/** Character offset of (`node`, `offset`) within `root` (ZWSP-skipped walk). */
export function textOffsetAt(root: Node, node: Node, offset: number): number {
  if (node === root) {
    let total = 0;
    for (let i = 0; i < Math.min(offset, root.childNodes.length); i++) {
      total += nodeTextLength(root.childNodes[i]!);
    }
    return total;
  }

  let total = 0;
  const walk = (current: Node): boolean => {
    if (current === node) {
      if (current.nodeType === TEXT_NODE) {
        total += domOffsetToMeaningful(current.textContent ?? "", offset);
        return true;
      }
      for (let i = 0; i < Math.min(offset, current.childNodes.length); i++) {
        total += nodeTextLength(current.childNodes[i]!);
      }
      return true;
    }
    if (current.nodeType === TEXT_NODE) {
      total += meaningfulLength(current.textContent ?? "");
      return false;
    }
    for (let i = 0; i < current.childNodes.length; i++) {
      if (walk(current.childNodes[i]!)) return true;
    }
    return false;
  };
  walk(root);
  return total;
}

export function bookmarkTextOffsets(root: HTMLElement, range: Range): TextOffsetBookmark {
  const start = textOffsetAt(root, range.startContainer, range.startOffset);
  const end = textOffsetAt(root, range.endContainer, range.endOffset);
  return {
    start,
    end,
    text: textBetweenOffsets(root, start, end),
  };
}

/**
 * Locate meaningful offset `offset` in `root`.
 * `preferEnd`: when landing exactly on a text-node boundary, stay at the end of
 * the previous node (end-of-selection) instead of the start of the next — so a
 * trailing ZWSP sibling after a face/size wrap is not used as the end anchor in a
 * way that drops the last visible glyphs on the next apply.
 */
export function positionFromTextOffset(
  root: Node,
  offset: number,
  preferEnd = false,
): { node: Node; offset: number } | null {
  let remaining = Math.max(0, offset);
  let found: { node: Node; offset: number } | null = null;
  let last: Node | null = null;
  let lastDomLen = 0;

  const walk = (node: Node): boolean => {
    if (node.nodeType === TEXT_NODE) {
      const data = node.textContent ?? "";
      const len = meaningfulLength(data);
      last = node;
      lastDomLen = data.length;
      if (preferEnd && remaining === len && len > 0) {
        found = { node, offset: data.length };
        return true;
      }
      if (remaining < len) {
        found = { node, offset: meaningfulToDomOffset(data, remaining) };
        return true;
      }
      remaining -= len;
      return false;
    }
    for (let i = 0; i < node.childNodes.length; i++) {
      if (walk(node.childNodes[i]!)) return true;
    }
    return false;
  };
  walk(root);
  if (found) return found;
  if (last) return { node: last, offset: lastDomLen };
  return null;
}

/** Rebuild a Range from text offsets; returns null when the root has no text. */
export function rangeFromTextOffsets(
  root: HTMLElement,
  start: number,
  end: number,
): Range | null {
  const a = Math.min(start, end);
  const b = Math.max(start, end);
  const startPos = positionFromTextOffset(root, a, false);
  const endPos = positionFromTextOffset(root, b, true);
  if (!startPos || !endPos) return null;
  try {
    const range = document.createRange();
    range.setStart(startPos.node, startPos.offset);
    range.setEnd(endPos.node, endPos.offset);
    expandRangeToTouchedTokens(root, range);
    return range;
  } catch {
    return null;
  }
}

/**
 * Pull pure ZWSP caret pads just outside the range endpoints into the selection
 * so the next Face/Size apply still covers glyphs beside field chips.
 */
export function expandRangeOverZwspPads(range: Range): void {
  const start = range.startContainer;
  if (start.nodeType === TEXT_NODE && range.startOffset === 0) {
    let prev: Node | null = start.previousSibling;
    if (!prev && start.parentNode) {
      // Walk up only through anonymous formatting wrappers.
      let p: Node | null = start.parentNode;
      while (p && p !== range.commonAncestorContainer) {
        if (p.previousSibling) {
          prev = p.previousSibling;
          break;
        }
        p = p.parentNode;
      }
    }
    if (prev?.nodeType === TEXT_NODE && (prev as Text).data === CARET_ZWSP) {
      try {
        range.setStart(prev, 0);
      } catch {
        /* ignore */
      }
    }
  }

  const end = range.endContainer;
  if (end.nodeType === TEXT_NODE && range.endOffset >= ((end as Text).data?.length ?? 0)) {
    let next: Node | null = end.nextSibling;
    if (!next && end.parentNode) {
      let p: Node | null = end.parentNode;
      while (p && p !== range.commonAncestorContainer) {
        if (p.nextSibling) {
          next = p.nextSibling;
          break;
        }
        p = p.parentNode;
      }
    }
    if (next?.nodeType === TEXT_NODE && (next as Text).data === CARET_ZWSP) {
      try {
        range.setEnd(next, (next as Text).data.length);
      } catch {
        /* ignore */
      }
    }
  }
}

function isFieldOrFunctionTokenEl(el: Element): boolean {
  return el.classList.contains("field-token") || el.classList.contains("function-token");
}

function tokenTouchesRangeLoose(token: HTMLElement, range: Range): boolean {
  try {
    if (range.intersectsNode(token)) return true;
  } catch {
    /* detached */
  }
  try {
    const tr = document.createRange();
    tr.selectNode(token);
    return (
      range.compareBoundaryPoints(Range.END_TO_START, tr) > 0 &&
      range.compareBoundaryPoints(Range.START_TO_END, tr) < 0
    );
  } catch {
    return false;
  }
}

/**
 * When a selection intersects a field/function chip (or lands on its ZWSP pad),
 * expand to cover the whole chip + adjacent ZWSP pads. contenteditable=false chips
 * are often excluded from visual highlight / live Range endpoints; Face/Size must
 * still treat them as in-selection so borders are not remounted mid-word around them.
 */
export function expandRangeToTouchedTokens(root: HTMLElement, range: Range): void {
  if (range.collapsed) return;

  root.querySelectorAll(".field-token, .function-token").forEach((node) => {
    if (!(node instanceof HTMLElement) || !isFieldOrFunctionTokenEl(node)) return;

    const prev = node.previousSibling;
    const next = node.nextSibling;
    const touchesPad =
      (prev?.nodeType === TEXT_NODE &&
        (prev as Text).data === CARET_ZWSP &&
        (range.startContainer === prev || range.endContainer === prev)) ||
      (next?.nodeType === TEXT_NODE &&
        (next as Text).data === CARET_ZWSP &&
        (range.startContainer === next || range.endContainer === next));

    if (!tokenTouchesRangeLoose(node, range) && !touchesPad) return;

    try {
      const tr = document.createRange();
      tr.selectNode(node);
      if (range.compareBoundaryPoints(Range.START_TO_START, tr) > 0) {
        range.setStartBefore(node);
      }
      if (range.compareBoundaryPoints(Range.END_TO_END, tr) < 0) {
        range.setEndAfter(node);
      }
    } catch {
      try {
        range.setStartBefore(node);
        range.setEndAfter(node);
      } catch {
        /* ignore */
      }
    }

    const padBefore = node.previousSibling;
    if (padBefore?.nodeType === TEXT_NODE && (padBefore as Text).data === CARET_ZWSP) {
      try {
        const probe = document.createRange();
        probe.setStart(padBefore, 0);
        probe.collapse(true);
        if (range.compareBoundaryPoints(Range.START_TO_START, probe) > 0) {
          range.setStart(padBefore, 0);
        }
      } catch {
        /* ignore */
      }
    }
    const padAfter = node.nextSibling;
    if (padAfter?.nodeType === TEXT_NODE && (padAfter as Text).data === CARET_ZWSP) {
      try {
        const probe = document.createRange();
        probe.setStart(padAfter, (padAfter as Text).data.length);
        probe.collapse(true);
        if (range.compareBoundaryPoints(Range.END_TO_END, probe) < 0) {
          range.setEnd(padAfter, (padAfter as Text).data.length);
        }
      } catch {
        /* ignore */
      }
    }
  });

  expandRangeOverZwspPads(range);
}

/**
 * Find `needle` (ZWSP-stripped) in `root` nearest to `hintStart`.
 * Returns meaningful offsets, or null when not found.
 */
export function findTextOffsetsNear(
  root: Node,
  needle: string,
  hintStart: number,
): { start: number; end: number } | null {
  if (!needle) return null;
  const haystack = textBetweenOffsets(root, 0, Number.MAX_SAFE_INTEGER);
  if (!haystack.includes(needle)) return null;

  let best = -1;
  let bestDist = Number.POSITIVE_INFINITY;
  for (let i = 0; i <= haystack.length - needle.length; i++) {
    if (haystack.slice(i, i + needle.length) !== needle) continue;
    const dist = Math.abs(i - hintStart);
    if (dist < bestDist) {
      bestDist = dist;
      best = i;
    }
  }
  if (best < 0) return null;
  return { start: best, end: best + needle.length };
}

export function restoreSelectionFromBookmark(
  root: HTMLElement,
  bookmark: TextOffsetBookmark,
): boolean {
  let start = bookmark.start;
  let end = bookmark.end;

  if (bookmark.text) {
    const atOffsets = textBetweenOffsets(root, start, end);
    if (atOffsets !== bookmark.text) {
      const found = findTextOffsetsNear(root, bookmark.text, start);
      if (found) {
        start = found.start;
        end = found.end;
      }
    }
  }

  if (start === end) return false;
  const range = rangeFromTextOffsets(root, start, end);
  if (!range || range.collapsed) return false;

  if (bookmark.text) {
    const got = textBetweenOffsets(root, start, end);
    if (got !== bookmark.text) {
      const found = findTextOffsetsNear(root, bookmark.text, start);
      if (!found) return false;
      const repaired = rangeFromTextOffsets(root, found.start, found.end);
      if (!repaired || repaired.collapsed) return false;
      return applyRange(repaired);
    }
  }

  return applyRange(range);
}

function applyRange(range: Range): boolean {
  const sel = window.getSelection();
  if (!sel) return false;
  try {
    sel.removeAllRanges();
    sel.addRange(range);
    return true;
  } catch {
    return false;
  }
}

/** Read the meaningful text spanning bookmark offsets (ZWSP stripped). */
export function textBetweenOffsets(root: Node, start: number, end: number): string {
  const a = Math.min(start, end);
  const b = Math.max(start, end);
  if (!Number.isFinite(b) || b >= Number.MAX_SAFE_INTEGER / 2) {
    let out = "";
    const walkAll = (node: Node) => {
      if (node.nodeType === TEXT_NODE) {
        out += meaningfulText(node.textContent ?? "");
        return;
      }
      for (let i = 0; i < node.childNodes.length; i++) walkAll(node.childNodes[i]!);
    };
    walkAll(root);
    return out;
  }

  let out = "";
  let index = 0;
  const walk = (node: Node) => {
    if (node.nodeType === TEXT_NODE) {
      const meaningful = meaningfulText(node.textContent ?? "");
      const from = Math.max(0, a - index);
      const to = Math.min(meaningful.length, b - index);
      if (from < to) out += meaningful.slice(from, to);
      index += meaningful.length;
      return;
    }
    for (let i = 0; i < node.childNodes.length; i++) walk(node.childNodes[i]!);
  };
  walk(root);
  return out;
}
