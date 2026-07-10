/**
 * Per-editor "typing attributes" — font/size/B/I/U for the next characters typed.
 * Legacy parity: blank paragraphs keep the last palette choice instead of reverting
 * to browser defaults (which mis-report as 8 pt via `queryCommandValue("fontSize")`).
 */

import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
} from "./paletteDefaults";

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
