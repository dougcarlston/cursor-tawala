/**
 * Inline field reference tokens in rich-text editors — styled, non-editable spans.
 * Legacy parity: `BrowserControl.InsertField` (`<t:field>`); display as `<<name>>`.
 */

import { fieldToken } from "./fieldInsertion";
import { applyTypingFormatToToken } from "./documentCanvas";
import { getActivePaletteEditor } from "./formattingPaletteContext";
import { typingFormatForInsert } from "./paletteTypingFormat";
import { ensureTokenCaretLanding, placeCaretAfterToken } from "./tokenCaretLanding";
import { FUNCTION_TOKEN_CLASS } from "./functionTokens";

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

/** Field chip under a viewport point (Fields drop onto an existing placeholder). */
export function fieldTokenAtPoint(
  clientX: number,
  clientY: number,
  root?: ParentNode | null,
): HTMLElement | null {
  const hit = document.elementFromPoint(clientX, clientY);
  if (!(hit instanceof Element)) return null;
  const token = hit.closest(`.${FIELD_TOKEN_CLASS}`);
  if (!(token instanceof HTMLElement)) return null;
  if (root && !root.contains(token)) return null;
  return token;
}

/**
 * Prefer selecting the field chip under the pointer so insert/replace swaps the
 * whole placeholder — never nests mid-label (`<<attende<<Other>>eName>>`).
 * Falls back to caret-at-point when the hit is ordinary text (incl. plain `<<Field>>`).
 */
export function selectFieldDropTarget(
  editor: HTMLElement,
  clientX: number,
  clientY: number,
  caretAtPoint: (x: number, y: number) => Range | null,
): Range | null {
  const sel = window.getSelection();
  if (!sel) return null;
  const token = fieldTokenAtPoint(clientX, clientY, editor);
  if (token) {
    const range = document.createRange();
    range.selectNode(token);
    sel.removeAllRanges();
    sel.addRange(range);
    return range;
  }
  const range = caretAtPoint(clientX, clientY);
  if (range && editor.contains(range.commonAncestorContainer)) {
    sel.removeAllRanges();
    sel.addRange(range);
    return range;
  }
  return null;
}

/** Live field-token under / intersecting the selection (incl. selectNode on the chip). */
export function fieldTokenFromSelection(sel: Selection | null): HTMLElement | null {
  if (!sel?.rangeCount) return null;
  const range = sel.getRangeAt(0);
  for (const container of [range.startContainer, range.endContainer]) {
    const el = container instanceof Element ? container : container.parentElement;
    const hit = el?.closest?.(`.${FIELD_TOKEN_CLASS}`);
    if (hit instanceof HTMLElement) return hit;
  }
  const root =
    range.commonAncestorContainer instanceof Element
      ? range.commonAncestorContainer
      : range.commonAncestorContainer.parentElement;
  if (!root) return null;
  const scope: ParentNode = root;
  for (const token of scope.querySelectorAll(`.${FIELD_TOKEN_CLASS}`)) {
    if (!(token instanceof HTMLElement)) continue;
    try {
      if (range.intersectsNode(token)) return token;
    } catch {
      /* detached */
    }
  }
  return null;
}

/** Function chip under / intersecting the selection — insert must not nest inside it. */
function functionTokenFromSelection(sel: Selection | null): HTMLElement | null {
  if (!sel?.rangeCount) return null;
  const range = sel.getRangeAt(0);
  for (const container of [range.startContainer, range.endContainer]) {
    const el = container instanceof Element ? container : container.parentElement;
    const hit = el?.closest?.(`.${FUNCTION_TOKEN_CLASS}`);
    if (hit instanceof HTMLElement) return hit;
  }
  const root =
    range.commonAncestorContainer instanceof Element
      ? range.commonAncestorContainer
      : range.commonAncestorContainer.parentElement;
  if (!root?.querySelectorAll) return null;
  for (const token of root.querySelectorAll(`.${FUNCTION_TOKEN_CLASS}`)) {
    if (!(token instanceof HTMLElement)) continue;
    try {
      if (range.intersectsNode(token)) return token;
    } catch {
      /* detached */
    }
  }
  return null;
}

/**
 * Plain `<<Field>>` text run containing the caret (not yet a field-token span).
 * Used so replace does not nest a chip mid-token (`<<Na<<Other>>me>>`).
 */
export function plainFieldRunAtRange(
  range: Range,
): { node: Text; start: number; end: number; name: string } | null {
  if (!range.collapsed && range.startContainer !== range.endContainer) return null;
  const node = range.startContainer;
  if (node.nodeType !== Node.TEXT_NODE) return null;
  const parent = node.parentElement;
  if (!parent) return null;
  if (parent.closest(`.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`)) return null;
  const text = node.textContent ?? "";
  const caret = range.startOffset;
  const re = /<<([^<>]+)>>/g;
  let match: RegExpExecArray | null;
  while ((match = re.exec(text))) {
    const start = match.index;
    const end = start + match[0].length;
    if (caret >= start && caret <= end) {
      return { node: node as Text, start, end, name: match[1].trim() };
    }
  }
  return null;
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
  // Without a text landing after contenteditable=false, browsers often open a new
  // block (split the .doc-placed-text <p>) on the next keystroke.
  placeCaretAfterToken(span);
}

/** Insert a field token span at the current selection, replacing any selected content. */
export function insertFieldTokenAtSelection(name: string): void {
  const sel = window.getSelection();
  const existing = fieldTokenFromSelection(sel);
  if (existing) {
    // Selected / caret-in chip: replace the whole token (never nest mid-label).
    const span = createFieldTokenElement(name);
    const handle = getActivePaletteEditor();
    if (handle) {
      const typing = typingFormatForInsert(handle.el);
      applyTypingFormatToToken(span, typing);
    } else {
      // Keep face/size from the chip being replaced when palette context is gone.
      for (const key of ["fontSize", "fontFamily", "fontWeight", "fontStyle", "textDecoration", "color"] as const) {
        const v = existing.style[key];
        if (v) span.style[key] = v;
      }
    }
    existing.replaceWith(span);
    placeCaretAfterToken(span);
    return;
  }

  if (sel?.rangeCount) {
    const range = sel.getRangeAt(0);
    // Caret inside a function chip (browser quirk): do not nest — place after the chip.
    const fn = functionTokenFromSelection(sel);
    if (fn) {
      const after = document.createRange();
      after.setStartAfter(fn);
      after.collapse(true);
      sel.removeAllRanges();
      sel.addRange(after);
    } else {
      // Caret mid plain `<<Field>>` text → replace that whole run.
      const plain = plainFieldRunAtRange(range);
      if (plain) {
        const span = createFieldTokenElement(name);
        const handle = getActivePaletteEditor();
        if (handle) {
          const typing = typingFormatForInsert(handle.el);
          applyTypingFormatToToken(span, typing);
        }
        const replace = document.createRange();
        replace.setStart(plain.node, plain.start);
        replace.setEnd(plain.node, plain.end);
        replace.deleteContents();
        replace.insertNode(span);
        placeCaretAfterToken(span);
        return;
      }
    }
  }

  const span = createFieldTokenElement(name);

  const handle = getActivePaletteEditor();
  if (handle) {
    // Same contract as function chips: inherit line format; do not resize the
    // parent paragraph when inserting (avoids label text jumping size).
    const typing = typingFormatForInsert(handle.el);
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
    if (!(node instanceof HTMLElement)) return;
    node.draggable = true;
    // Drop stale insert-time line-height:1 that forced stepped line boxes.
    node.style.removeProperty("line-height");
    node.style.verticalAlign = "baseline";
    ensureTokenCaretLanding(node);
  });
}

const PLAIN_FIELD_RE = /<<([^<>]+)>>/g;

/** Walk an editor and add caret landings around every field token. */
export function ensureFieldTokenCaretGaps(root: ParentNode): void {
  root.querySelectorAll(`.${FIELD_TOKEN_CLASS}`).forEach((node) => {
    if (node instanceof HTMLElement) ensureTokenCaretLanding(node);
  });
}

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

  ensureFieldTokenCaretGaps(root);
}

/**
 * Safe HTML for FIB idle/edit `innerHTML`: turn plain `<<Field>>` into chips first so the
 * browser does not treat `<ContactType1>` as an unknown tag (Signup “Your <>:” idle bug).
 * Existing field-token spans are left intact.
 */
export function embedPlainFieldTokensAsHtml(source: string): string {
  const chips: string[] = [];
  let s = String(source ?? "").replace(
    /<span\b[^>]*\bfield-token\b[^>]*>[\s\S]*?<\/span>/gi,
    (chip) => {
      const i = chips.length;
      chips.push(chip);
      return `\u0000CHIP${i}\u0000`;
    },
  );
  s = s.replace(/<<([^<>]+)>>/g, (_, name) => {
    const n = String(name).trim();
    if (!n) return "";
    return createFieldTokenElement(n).outerHTML;
  });
  return s.replace(/\u0000CHIP(\d+)\u0000/g, (_, i) => chips[Number(i)] ?? "");
}

export function readFieldNameFromToken(el: HTMLElement): string | null {
  return el.getAttribute(FIELD_NAME_ATTR) || null;
}
