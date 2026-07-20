/**
 * Caret landings beside contenteditable=false tokens (field chips, function boxes).
 * Without an editable text node after the token — and a caret *inside* that node —
 * Chromium often escapes the parent paragraph on the next keystroke and starts a
 * new block (Document: new placed line / reset face-size-margin).
 *
 * Face/Size execCommand often wraps a ZWSP landing into a font/span next to the
 * chip. A naive "insert if previousSibling is not a text node" then stacked a
 * *second* ZWSP outside the wrapper. Each Size pass grew that stack by ~2 per
 * chip and the selection bookmark end crept inward by the same amount.
 */

/** Invisible caret landing so clicks/typing work next to non-editable tokens. */
export const CARET_ZWSP = "\u200B";

function isZwspOnlyText(node: Node | null): node is Text {
  return node?.nodeType === 3 /* TEXT_NODE */ && (node as Text).data === CARET_ZWSP;
}

function isCaretLandingNode(node: Node | null): boolean {
  if (!node) return false;
  if (node.nodeType === 3 /* TEXT_NODE */) return true;
  return node.nodeName === "BR";
}

/**
 * Offset inside the text node immediately after a non-editable token.
 * Mid-sentence content → 0 (do not skip characters). Pure ZWSP landing → 1 (type after it).
 */
export function caretOffsetAfterTokenLanding(text: string): number {
  return text === CARET_ZWSP ? 1 : 0;
}

/**
 * If a Face/Size wrapper absorbed the caret pad, pull a trailing/leading ZWSP
 * out so it is again a direct sibling of the chip (no second pad inserted).
 *
 * Chrome's fontName often folds `\u200B` into the adjacent face text node
 * (`"…insert \u200B"`), not as a pure-ZWSP lastChild — split that off too.
 */
function extractAbsorbedZwsp(side: "before" | "after", span: HTMLElement): void {
  const parent = span.parentNode;
  if (!parent) return;
  const neighbor = side === "before" ? span.previousSibling : span.nextSibling;
  if (!neighbor || neighbor.nodeType !== 1 /* ELEMENT_NODE */) return;
  const asEl = neighbor as Element & { classList?: DOMTokenList };
  // Never pull text out of another token.
  if (
    asEl.classList?.contains("field-token") ||
    asEl.classList?.contains("function-token")
  ) {
    return;
  }

  if (side === "before") {
    let last = neighbor.lastChild;
    if (!last || last.nodeType !== 3 /* TEXT_NODE */) return;
    const text = last as Text;
    if (text.data === CARET_ZWSP) {
      parent.insertBefore(text, span);
      return;
    }
    if (text.data.endsWith(CARET_ZWSP) && text.data.length > 1) {
      const zw = text.splitText(text.data.length - 1);
      parent.insertBefore(zw, span);
    }
  } else {
    let first = neighbor.firstChild;
    if (!first || first.nodeType !== 3 /* TEXT_NODE */) return;
    const text = first as Text;
    if (text.data === CARET_ZWSP) {
      parent.insertBefore(text, span.nextSibling);
      return;
    }
    if (text.data.startsWith(CARET_ZWSP) && text.data.length > 1) {
      text.splitText(1); // leaves leading ZWSP as `text`, rest as next sibling
      parent.insertBefore(text, span.nextSibling);
    }
  }
}

/** Collapse duplicate adjacent ZWSP pads so landings stay one per side. */
function coalesceAdjacentZwspPads(span: HTMLElement): void {
  const parent = span.parentNode;
  if (!parent) return;

  while (isZwspOnlyText(span.previousSibling) && isZwspOnlyText(span.previousSibling.previousSibling)) {
    parent.removeChild(span.previousSibling.previousSibling!);
  }
  while (isZwspOnlyText(span.nextSibling) && isZwspOnlyText(span.nextSibling.nextSibling)) {
    parent.removeChild(span.nextSibling.nextSibling!);
  }
}

/**
 * Ensure editable text (or `<br>`) immediately before/after a non-editable token so the
 * caret can land when the line would otherwise end on the chip alone.
 * Idempotent across Face/Size wraps: never stacks a second ZWSP when one already
 * exists inside an adjacent formatting wrapper.
 */
export function ensureTokenCaretLanding(span: HTMLElement): void {
  const parent = span.parentNode;
  if (!parent) return;

  extractAbsorbedZwsp("before", span);
  extractAbsorbedZwsp("after", span);

  if (!isCaretLandingNode(span.previousSibling)) {
    parent.insertBefore(document.createTextNode(CARET_ZWSP), span);
  }
  if (!isCaretLandingNode(span.nextSibling)) {
    parent.insertBefore(document.createTextNode(CARET_ZWSP), span.nextSibling);
  }

  coalesceAdjacentZwspPads(span);
}

/** Place the selection inside the editable landing before `span` (same parent paragraph). */
export function placeCaretBeforeToken(span: HTMLElement): void {
  ensureTokenCaretLanding(span);
  const sel = window.getSelection();
  if (!sel) return;
  const before = document.createRange();
  const landing = span.previousSibling;
  if (landing?.nodeType === 3 /* TEXT_NODE */) {
    const text = landing as Text;
    // Sit at the end of the pre-token landing (after real glyphs, on ZWSP if pad-only).
    before.setStart(text, text.data.length);
  } else {
    before.setStartBefore(span);
  }
  before.collapse(true);
  sel.removeAllRanges();
  sel.addRange(before);
}

/** Place the selection inside the editable landing after `span` (same parent paragraph). */
export function placeCaretAfterToken(span: HTMLElement): void {
  ensureTokenCaretLanding(span);
  const sel = window.getSelection();
  if (!sel) return;
  const after = document.createRange();
  const landing = span.nextSibling;
  if (landing?.nodeType === 3 /* TEXT_NODE */) {
    const text = (landing as Text).data ?? "";
    after.setStart(landing, Math.min(caretOffsetAfterTokenLanding(text), text.length));
  } else {
    after.setStartAfter(span);
  }
  after.collapse(true);
  sel.removeAllRanges();
  sel.addRange(after);
}
