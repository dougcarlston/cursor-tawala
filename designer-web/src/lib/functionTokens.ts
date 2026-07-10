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
import { getTypingFormat, isBlankTypingContext } from "./paletteTypingFormat";

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

    const raw = config[param.id];
    if (raw === undefined || String(raw).trim() === "") continue;
    parts.push(String(raw).trim());
  }

  if (parts.length === 0) return `<<${def.name}>>`;
  return `<<${def.name}(${parts.join(", ")})>>`;
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
  const typing = getTypingFormat(root);
  const placed = findPlacedTextBlockAtCaret(root);
  if (placed && isBlankTypingContext(root)) {
    applyTypingFormatToPlacedBlock(placed, typing);
  }
  applyTypingFormatToToken(span, typing);

  if (replace) {
    replace.element.replaceWith(span);
    const range = document.createRange();
    range.setStartAfter(span);
    range.collapse(true);
    sel.removeAllRanges();
    sel.addRange(range);
    return;
  }

  if (!sel.rangeCount) return;
  const range = sel.getRangeAt(0);
  if (!range.collapsed) range.deleteContents();
  range.insertNode(span);

  const after = document.createRange();
  after.setStartAfter(span);
  after.collapse(true);
  sel.removeAllRanges();
  sel.addRange(after);
}

export function lookupFunctionDefByName(name: string): FunctionDef | undefined {
  const id = FUNCTION_CATALOG_BY_NAME.get(name.trim().toUpperCase());
  return id ? getFunctionDef(id) : undefined;
}
