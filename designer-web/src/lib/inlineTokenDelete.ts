/**
 * Delete field/function inline chips from a rich-text selection without removing
 * the whole Form Text row (Del/× used to call confirmAndDeleteFormItem instead).
 */

import {
  editorHtmlToStructuredContent,
  findEditableStructuredFunctionNode,
} from "@/components/StructuredTextProperties";
import { FIELD_TOKEN_CLASS, normalizeFieldTokenSpans } from "@/lib/fieldTokens";
import { ensureFunctionTokenCaretGaps, FUNCTION_TOKEN_CLASS } from "@/lib/functionTokens";
import { useProjectStore } from "@/store/projectStore";
import type { FormItem } from "@/types/tawala";

const INLINE_TOKEN_SELECTOR = `.${FIELD_TOKEN_CLASS}, .${FUNCTION_TOKEN_CLASS}`;

/** True when a field/function chip lies inside (or on the boundary of) the range. */
export function tokenTouchesRange(token: HTMLElement, range: Range): boolean {
  try {
    if (range.intersectsNode(token)) return true;
  } catch {
    /* detached / document mismatch */
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

function selectionTouchesRoot(sel: Selection, root: Element): boolean {
  if (!sel.rangeCount) return false;
  const range = sel.getRangeAt(0);
  try {
    if (root.contains(range.commonAncestorContainer)) return true;
  } catch {
    /* ignore */
  }
  if (sel.anchorNode && root.contains(sel.anchorNode)) return true;
  if (sel.focusNode && root.contains(sel.focusNode)) return true;
  try {
    return root.contains(range.startContainer) || root.contains(range.endContainer);
  } catch {
    return false;
  }
}

/** Field/function chips intersecting the current window selection inside `root`. */
export function collectInlineTokensInSelection(root: Element): HTMLElement[] {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0 || !selectionTouchesRoot(sel, root)) return [];
  const range = sel.getRangeAt(0);
  const seen = new Set<HTMLElement>();

  const add = (token: HTMLElement | null) => {
    if (token && root.contains(token)) seen.add(token);
  };

  for (const node of [sel.anchorNode, sel.focusNode]) {
    if (!node || !root.contains(node)) continue;
    const el =
      node.nodeType === Node.ELEMENT_NODE ? (node as Element) : node.parentElement;
    add(el?.closest(INLINE_TOKEN_SELECTOR) as HTMLElement | null);
  }

  root.querySelectorAll(INLINE_TOKEN_SELECTOR).forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    if (!tokenTouchesRange(node, range)) return;
    seen.add(node);
  });
  return [...seen];
}

function normalizeAfterTokenRemoval(root: HTMLElement): void {
  ensureFunctionTokenCaretGaps(root);
  normalizeFieldTokenSpans(root);
}

/** Remove selected inline chips from a live editor surface; returns true when any were removed. */
export function tryDeleteInlineTokensInSelection(root: HTMLElement | null): boolean {
  if (!root) return false;
  const tokens = collectInlineTokensInSelection(root);
  if (!tokens.length) return false;
  for (const token of tokens) token.remove();
  normalizeAfterTokenRemoval(root);
  const sel = window.getSelection();
  sel?.removeAllRanges();
  return true;
}

function removeTokensFromHtmlString(html: string, tokensFromDom: HTMLElement[]): string {
  const instanceIds = new Set<string>();
  const fieldNames = new Set<string>();
  for (const token of tokensFromDom) {
    const inst = token.getAttribute("data-function-instance");
    if (inst) instanceIds.add(inst);
    const field = token.getAttribute("data-field-name");
    if (field) fieldNames.add(field);
  }
  const temp = document.createElement("motion-div");
  temp.innerHTML = html;
  if (instanceIds.size) {
    temp.querySelectorAll(`.${FUNCTION_TOKEN_CLASS}`).forEach((node) => {
      if (!(node instanceof HTMLElement)) return;
      const inst = node.getAttribute("data-function-instance");
      if (inst && instanceIds.has(inst)) node.remove();
    });
  }
  if (fieldNames.size) {
    temp.querySelectorAll(`.${FIELD_TOKEN_CLASS}`).forEach((node) => {
      if (!(node instanceof HTMLElement)) return;
      const name = node.getAttribute("data-field-name");
      if (name && fieldNames.has(name)) node.remove();
    });
  }
  normalizeAfterTokenRemoval(temp);
  return temp.innerHTML;
}

function commitPlainTextHtml(formName: string, index: number, item: FormItem, html: string): void {
  if (item.type !== "text" || Array.isArray(item.content)) return;
  useProjectStore.getState().updateFormItem(formName, index, { ...item, content: html });
}

function commitStructuredHtml(
  formName: string,
  index: number,
  item: FormItem,
  editorHtml: string,
): void {
  if (item.type !== "text" || !Array.isArray(item.content)) return;
  const table = findEditableStructuredFunctionNode(item.content);
  if (!table) return;
  const next = editorHtmlToStructuredContent(editorHtml, table);
  useProjectStore.getState().updateFormItem(formName, index, { ...item, content: next });
}

/**
 * When a Form Text row is selected and a field/function chip is highlighted,
 * remove only the chip(s) and persist. Returns true when handled.
 */
export function tryDeleteSelectedFormInlineTokens(): boolean {
  const canvas = document.querySelector(".form-canvas");
  if (!(canvas instanceof HTMLElement)) return false;

  const { selection, selectedItemIndex, project } = useProjectStore.getState();
  if (selection.kind !== "form" || !selection.name || selectedItemIndex === null) return false;

  const form = project.forms.find((f) => f.name === selection.name);
  const item = form?.items[selectedItemIndex];
  if (!item || item.type !== "text") return false;

  const row = canvas.querySelector(".text-canvas-row.selected");
  if (!(row instanceof HTMLElement)) return false;

  const editor = row.querySelector(
    ".text-rich-editor[contenteditable='true'], .rich-surface[contenteditable='true']",
  );
  const rendered = row.querySelector(".text-rendered");

  if (editor instanceof HTMLElement) {
    const active = document.activeElement;
    const focusInEditor = active === editor || editor.contains(active);
    if (focusInEditor || collectInlineTokensInSelection(editor).length > 0) {
      if (tryDeleteInlineTokensInSelection(editor)) {
        if (Array.isArray(item.content)) {
          commitStructuredHtml(selection.name, selectedItemIndex, item, editor.innerHTML);
        } else {
          commitPlainTextHtml(selection.name, selectedItemIndex, item, editor.innerHTML);
        }
        return true;
      }
    }
  }

  if (rendered instanceof HTMLElement && typeof item.content === "string") {
    const tokens = collectInlineTokensInSelection(rendered);
    if (!tokens.length) return false;
    const nextHtml = removeTokensFromHtmlString(item.content, tokens);
    commitPlainTextHtml(selection.name, selectedItemIndex, item, nextHtml);
    window.getSelection()?.removeAllRanges();
    return true;
  }

  return false;
}
