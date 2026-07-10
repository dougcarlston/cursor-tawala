/**
 * Inline field reference tokens in rich-text editors — styled, non-editable spans.
 * Legacy parity: `BrowserControl.InsertField` (`<t:field>`); display as `<<name>>`.
 */

import { fieldToken } from "./fieldInsertion";
import {
  applyTypingFormatToPlacedBlock,
  applyTypingFormatToToken,
  findPlacedTextBlockAtCaret,
} from "./documentCanvas";
import { getActivePaletteEditor } from "./formattingPaletteContext";
import { getTypingFormat, isBlankTypingContext } from "./paletteTypingFormat";

export const FIELD_TOKEN_CLASS = "field-token";
export const FIELD_NAME_ATTR = "data-field-name";

export function createFieldTokenElement(name: string): HTMLSpanElement {
  const span = document.createElement("span");
  span.className = `${FIELD_TOKEN_CLASS} function-table-token`;
  span.setAttribute("contenteditable", "false");
  span.setAttribute(FIELD_NAME_ATTR, name);
  span.setAttribute("title", name);
  span.draggable = true;
  span.textContent = fieldToken(name);
  return span;
}

function placeTokenAtSelection(span: HTMLElement): void {
  const sel = window.getSelection();
  if (!sel || !sel.rangeCount) return;
  const range = sel.getRangeAt(0);
  // Dropping onto the token itself (or a selection that includes it) is a no-op.
  if (span.contains(range.commonAncestorContainer)) return;
  if (!range.collapsed) {
    const probe = range.cloneRange();
    try {
      if (probe.intersectsNode(span)) return;
    } catch {
      /* intersectsNode can throw on detached nodes */
    }
    range.deleteContents();
  }
  range.insertNode(span);
  const after = document.createRange();
  after.setStartAfter(span);
  after.collapse(true);
  sel.removeAllRanges();
  sel.addRange(after);
}

/** Insert a field token span at the current selection, replacing any selected content. */
export function insertFieldTokenAtSelection(name: string): void {
  const span = createFieldTokenElement(name);

  const handle = getActivePaletteEditor();
  if (handle) {
    const typing = getTypingFormat(handle.el);
    const placed = findPlacedTextBlockAtCaret(handle.el);
    if (placed && isBlankTypingContext(handle.el)) {
      applyTypingFormatToPlacedBlock(placed, typing);
    }
    applyTypingFormatToToken(span, typing);
  }

  placeTokenAtSelection(span);
}

/** Relocate an existing field token to the current selection (preserves styles). */
export function moveFieldTokenToSelection(token: HTMLElement): void {
  placeTokenAtSelection(token);
}

/** Ensure loaded / upgraded tokens are draggable for in-editor move. */
export function ensureFieldTokensDraggable(root: ParentNode): void {
  root.querySelectorAll(`.${FIELD_TOKEN_CLASS}`).forEach((node) => {
    if (node instanceof HTMLElement) node.draggable = true;
  });
}

const PLAIN_FIELD_RE = /<<([^<>]+)>>/g;

/** Upgrade legacy plain `<<field>>` text nodes to styled spans (idempotent). */
export function normalizeFieldTokenSpans(root: ParentNode): void {
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
  const toUpgrade: { node: Text; parts: Array<{ type: "text" | "field"; value: string }> }[] = [];

  let node: Text | null;
  while ((node = walker.nextNode() as Text | null)) {
    const parent = node.parentElement;
    if (!parent) continue;
    if (parent.closest(`.${FIELD_TOKEN_CLASS}`)) continue;
    if (parent.closest(".function-token")) continue;
    const text = node.textContent ?? "";
    if (!text.includes("<<")) continue;

    const parts: Array<{ type: "text" | "field"; value: string }> = [];
    let last = 0;
    PLAIN_FIELD_RE.lastIndex = 0;
    let match: RegExpExecArray | null;
    let found = false;
    while ((match = PLAIN_FIELD_RE.exec(text))) {
      found = true;
      if (match.index > last) parts.push({ type: "text", value: text.slice(last, match.index) });
      parts.push({ type: "field", value: match[1].trim() });
      last = match.index + match[0].length;
    }
    if (!found) continue;
    if (last < text.length) parts.push({ type: "text", value: text.slice(last) });
    toUpgrade.push({ node, parts });
  }

  for (const { node, parts } of toUpgrade) {
    const frag = document.createDocumentFragment();
    for (const part of parts) {
      if (part.type === "field" && part.value) {
        frag.appendChild(createFieldTokenElement(part.value));
      } else if (part.value) {
        frag.appendChild(document.createTextNode(part.value));
      }
    }
    node.parentNode?.replaceChild(frag, node);
  }
}

export function readFieldNameFromToken(el: HTMLElement): string | null {
  return el.getAttribute(FIELD_NAME_ATTR) || null;
}
