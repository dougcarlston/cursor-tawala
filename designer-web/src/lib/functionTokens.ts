/**
 * Inline function tokens in rich-text editors — display strings, DOM spans, caret helpers.
 * Legacy parity: `BrowserControl.InsertFunction`, `FunctionReference.ToDisplayString`.
 */

import {
  FUNCTION_CATALOG,
  getFunctionDef,
  type ColumnConfig,
  type FunctionConfig,
  type FunctionDef,
} from "./functionCatalog";
import { formatFunctionConditionsDisplay, parseFunctionConditions } from "./functionConditions";
import {
  applyTypingFormatToPlacedBlock,
  applyTypingFormatToToken,
  findPlacedTextBlockAtCaret,
} from "./documentCanvas";
import { isBlankTypingContext, typingFormatForInsert } from "./paletteTypingFormat";
import { ensureTokenCaretLanding, placeCaretAfterToken } from "./tokenCaretLanding";

export { ensureTokenCaretLanding, placeCaretAfterToken } from "./tokenCaretLanding";

export const FUNCTION_TOKEN_CLASS = "function-token";
export const FUNCTION_TOKEN_ATTR = "data-function-id";
export const FUNCTION_CONFIG_ATTR = "data-function-config";

let nextFunctionInstanceId = 1;

const FUNCTION_CATALOG_BY_NAME = new Map(
  FUNCTION_CATALOG.map((f) => [f.name.toUpperCase(), f.id]),
);

export function allocateFunctionInstanceId(): number {
  return nextFunctionInstanceId++;
}

/** Legacy `<<NAME(params)>>` display string shown in the editor. */
export function buildFunctionDisplayString(def: FunctionDef, config: FunctionConfig): string {
  const parts: string[] = [];

  for (const param of def.parameters) {
    if (param.type === "column-collection") {
      const n = Number(config.numberOfColumns ?? 0);
      if (n > 0) parts.push(String(n));
      const cols = (config.column as ColumnConfig[] | undefined) ?? [];
      for (let i = 0; i < n; i++) {
        const col = cols[i];
        if (col?.contents?.trim()) parts.push(col.contents.trim());
        else if (n > 0) parts.push("...");
      }
      if (n > 2) {
        // Legacy truncates long column lists in display (ItemizationTableFunctionTest2020).
        while (parts.length > 2) parts.pop();
        if (parts.length === 2) parts.push("...");
      }
      continue;
    }

    if (param.type === "tawala-conditions") {
      const display = formatFunctionConditionsDisplay(parseFunctionConditions(config));
      if (display) parts.push(display);
      continue;
    }

    if (param.hidden) continue;

    const raw = config[param.id];
    if (raw === undefined || String(raw).trim() === "") continue;
    parts.push(truncateDisplayParam(String(raw).trim()));
  }

  if (parts.length === 0) return `<<${def.name}>>`;
  return `<<${def.name}(${parts.join(", ")})>>`;
}

/** Keep long URLs / expressions from blowing the token chrome out of the editor box. */
function truncateDisplayParam(value: string, max = 36): string {
  if (value.length <= max) return value;
  // Prefer keeping a filename tail for URLs.
  try {
    if (/^https?:\/\//i.test(value)) {
      const u = new URL(value);
      const base = u.pathname.split("/").filter(Boolean).pop() ?? value;
      const label = decodeURIComponent(base);
      if (label.length <= max) return label;
      return `${label.slice(0, max - 1)}…`;
    }
  } catch {
    /* not a URL */
  }
  return `${value.slice(0, max - 1)}…`;
}

export function serializeFunctionConfig(config: FunctionConfig): string {
  return JSON.stringify(config);
}

export function parseFunctionConfig(raw: string | null): FunctionConfig {
  if (!raw) return {};
  try {
    const parsed = JSON.parse(raw) as FunctionConfig;
    return parsed && typeof parsed === "object" ? parsed : {};
  } catch {
    return {};
  }
}

export function createFunctionTokenElement(
  def: FunctionDef,
  config: FunctionConfig,
  instanceId?: number,
): HTMLSpanElement {
  const id = instanceId ?? allocateFunctionInstanceId();
  const display = buildFunctionDisplayString(def, config);
  const span = document.createElement("span");
  span.className = `${FUNCTION_TOKEN_CLASS} function-table-token`;
  span.setAttribute("contenteditable", "false");
  span.setAttribute(FUNCTION_TOKEN_ATTR, def.id);
  span.setAttribute("data-function-instance", String(id));
  span.setAttribute(FUNCTION_CONFIG_ATTR, serializeFunctionConfig(config));
  span.setAttribute("title", def.name);
  span.textContent = display;
  return span;
}

/** Walk an editor and add caret landings around every function token. */
export function ensureFunctionTokenCaretGaps(root: HTMLElement): void {
  root.querySelectorAll(`.${FUNCTION_TOKEN_CLASS}`).forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    // Drop stale insert-time line-height:1 (same contract as field chips).
    node.style.removeProperty("line-height");
    node.style.verticalAlign = "baseline";
    ensureTokenCaretLanding(node);
  });
}

export interface FunctionTokenRef {
  element: HTMLSpanElement;
  functionId: string;
  config: FunctionConfig;
  instanceId: number;
}

function isFunctionTokenElement(node: Node | null): node is HTMLSpanElement {
  return (
    node instanceof HTMLSpanElement &&
    node.classList.contains(FUNCTION_TOKEN_CLASS) &&
    node.hasAttribute(FUNCTION_TOKEN_ATTR)
  );
}

/** Nearest function token span containing or adjacent to the selection. */
export function findFunctionTokenAtSelection(root: HTMLElement): FunctionTokenRef | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== root) {
    if (isFunctionTokenElement(node)) {
      return tokenRefFromElement(node);
    }
    node = node.parentNode;
  }

  // Collapsed caret immediately after a function token.
  const range = sel.getRangeAt(0);
  if (!range.collapsed) return null;
  const { startContainer, startOffset } = range;
  if (startContainer.nodeType === Node.TEXT_NODE && startOffset === 0) {
    const prev = startContainer.previousSibling;
    if (isFunctionTokenElement(prev)) return tokenRefFromElement(prev);
  }
  if (startContainer.nodeType === Node.ELEMENT_NODE) {
    const el = startContainer as HTMLElement;
    const prev = startOffset > 0 ? el.childNodes[startOffset - 1] : el.previousSibling;
    if (isFunctionTokenElement(prev)) return tokenRefFromElement(prev);
  }
  return null;
}

export function tokenRefFromElement(el: HTMLSpanElement): FunctionTokenRef {
  const functionId = el.getAttribute(FUNCTION_TOKEN_ATTR) ?? "";
  const instanceRaw = el.getAttribute("data-function-instance");
  return {
    element: el,
    functionId,
    config: parseFunctionConfig(el.getAttribute(FUNCTION_CONFIG_ATTR)),
    instanceId: instanceRaw ? Number(instanceRaw) : 0,
  };
}

/** Insert or replace a function token at the saved editor selection. */
export function insertFunctionTokenAtSelection(
  root: HTMLElement,
  def: FunctionDef,
  config: FunctionConfig,
  replace?: FunctionTokenRef | null,
): void {
  const sel = window.getSelection();
  if (!sel) return;

  const span = createFunctionTokenElement(def, config, replace?.instanceId || undefined);

  if (replace) {
    // Keep existing chip face/size — applying caret "typing format" after Configure
    // randomly resized placeholders (owner Jul 16 Response Totals).
    copyFunctionTokenPresentation(replace.element, span);
    replace.element.replaceWith(span);
    placeCaretAfterToken(span);
    return;
  }

  const typing = typingFormatForInsert(root);
  const placed = findPlacedTextBlockAtCaret(root);
  if (placed && isBlankTypingContext(root)) {
    applyTypingFormatToPlacedBlock(placed, typing);
  }
  applyTypingFormatToToken(span, typing);

  let range: Range | null = sel.rangeCount > 0 ? sel.getRangeAt(0) : null;
  if (!range || !root.contains(range.commonAncestorContainer)) {
    range = document.createRange();
    range.selectNodeContents(root);
    range.collapse(false);
  }
  if (!range.collapsed) range.deleteContents();
  range.insertNode(span);
  placeCaretAfterToken(span);
}

/** Copy inline presentation styles from an existing function chip onto its replacement. */
function copyFunctionTokenPresentation(from: HTMLElement, to: HTMLElement): void {
  const keys = [
    "fontSize",
    "fontFamily",
    "fontWeight",
    "fontStyle",
    "textDecoration",
    "color",
    "verticalAlign",
    "lineHeight",
  ] as const;
  for (const key of keys) {
    const value = from.style[key];
    if (value) to.style[key] = value;
  }
}

export function lookupFunctionDefByName(name: string): FunctionDef | undefined {
  const id = FUNCTION_CATALOG_BY_NAME.get(name.trim().toUpperCase());
  return id ? getFunctionDef(id) : undefined;
}
