/**
 * Structured Form Text function tokens in the rich editor
 * (`data-tawala-structured-node` — Multiple Question List, Question Correlation Table, …).
 */

import {
  defaultFunctionConfig,
  getFunctionDef,
  type ColumnConfig,
  type FunctionConfig,
} from "./functionCatalog";
import { requestFunctionPicker } from "./functionPicker";

export const STRUCTURED_NODE_DATA_ATTR = "data-tawala-structured-node";
export const ITEMIZATION_TOKEN_DATA_ATTR = "data-itemization-token";

export interface ItemizationNode {
  type: "itemizationTable";
  form?: string;
  version?: number;
  columns?: { header: string; field: string }[];
  [key: string]: unknown;
}

export interface QuestionCorrelationNode {
  type: "questionCorrelationTable";
  form?: string;
  version?: number;
  questionField?: string;
  displayField?: string;
  preferredField?: string;
  [key: string]: unknown;
}

export type StructuredFunctionNode = ItemizationNode | QuestionCorrelationNode;

const STRUCTURED_FUNCTION_IDS: Record<StructuredFunctionNode["type"], string> = {
  itemizationTable: "itemization-table",
  questionCorrelationTable: "question-correlation-table",
};

export function encodeStructuredNode(node: StructuredFunctionNode): string {
  return encodeURIComponent(JSON.stringify(node));
}

export function decodeStructuredNode(value: string | null): StructuredFunctionNode | null {
  if (!value) return null;
  try {
    const parsed = JSON.parse(decodeURIComponent(value)) as StructuredFunctionNode;
    if (parsed?.type === "itemizationTable" || parsed?.type === "questionCorrelationTable") {
      return parsed;
    }
    return null;
  } catch {
    return null;
  }
}

function itemizationToConfig(node: ItemizationNode): FunctionConfig {
  const def = getFunctionDef("itemization-table");
  const cols = node.columns ?? [];
  const base = def ? defaultFunctionConfig(def) : {};
  return {
    ...base,
    "show-print-control": "false",
    "show-export-control": "false",
    numberOfColumns: Math.max(1, cols.length),
    column: cols.map((c) => ({ header: c.header ?? "", contents: c.field ?? "" })),
    "form-name": node.form ?? "",
    conditionsRows: [{ field: "", op: "equals", value: "" }],
    conditionsCombinator: "and",
  };
}

function correlationToConfig(node: QuestionCorrelationNode): FunctionConfig {
  const def = getFunctionDef("question-correlation-table");
  const base = def ? defaultFunctionConfig(def) : {};
  return {
    ...base,
    "question-field-name": node.questionField ?? "",
    "display-field-name": node.displayField ?? "",
    "preferred-choice-field-name": node.preferredField ?? "",
    conditionsRows: [{ field: "", op: "equals", value: "" }],
    conditionsCombinator: "and",
  };
}

function nodeToConfig(node: StructuredFunctionNode): FunctionConfig {
  return node.type === "itemizationTable" ? itemizationToConfig(node) : correlationToConfig(node);
}

function patchNodeFromConfig(
  node: StructuredFunctionNode,
  nextConfig: FunctionConfig,
): StructuredFunctionNode {
  if (node.type === "itemizationTable") {
    const nextCols = (nextConfig.column as ColumnConfig[] | undefined) ?? [];
    const n = Number(nextConfig.numberOfColumns ?? nextCols.length) || nextCols.length;
    const form = String(nextConfig["form-name"] ?? node.form ?? "").trim();
    return {
      ...node,
      form: form || node.form,
      columns: nextCols.slice(0, n).map((c) => ({
        header: c.header ?? "",
        field: c.contents ?? "",
      })),
    };
  }

  const form = String(node.form ?? "").trim();
  return {
    ...node,
    form: form || node.form,
    questionField: String(nextConfig["question-field-name"] ?? "").trim(),
    displayField: String(nextConfig["display-field-name"] ?? "").trim(),
    preferredField: String(nextConfig["preferred-choice-field-name"] ?? "").trim() || undefined,
  };
}

/** Open Configure for a structured function token (itemization / correlation) in rich text. */
export function openStructuredFunctionTokenForEdit(
  tokenEl: HTMLElement,
  onPatched: () => void,
): boolean {
  const node = decodeStructuredNode(tokenEl.getAttribute(STRUCTURED_NODE_DATA_ATTR));
  if (!node) return false;
  const functionId = STRUCTURED_FUNCTION_IDS[node.type];
  const def = getFunctionDef(functionId);
  if (!def) return false;

  requestFunctionPicker({
    mode: "edit",
    existing: {
      element: tokenEl as HTMLSpanElement,
      functionId,
      config: nodeToConfig(node),
      instanceId: 0,
    },
    commitConfig: (_d, nextConfig) => {
      const patched = patchNodeFromConfig(node, nextConfig);
      tokenEl.setAttribute(STRUCTURED_NODE_DATA_ATTR, encodeStructuredNode(patched));
      if (patched.type === "itemizationTable" && patched.form) {
        tokenEl.setAttribute("data-itemization-form", patched.form);
      }
      tokenEl.setAttribute("title", def.name);
      tokenEl.textContent =
        patched.type === "itemizationTable"
          ? "{ MULTIPLE QUESTION LIST }"
          : "{ QUESTION CORRELATION TABLE }";
      onPatched();
    },
  });
  return true;
}

/** @deprecated Prefer openStructuredFunctionTokenForEdit */
export function openStructuredItemizationTokenForEdit(
  tokenEl: HTMLElement,
  onPatched: () => void,
): boolean {
  return openStructuredFunctionTokenForEdit(tokenEl, onPatched);
}
