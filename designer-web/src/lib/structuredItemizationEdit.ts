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

export interface ChoiceTallyNode {
  type: "choiceTallyTable";
  form?: string;
  version?: number;
  /** Often `Record:Form:Q1` or `Form:Q1`. */
  field?: string;
  [key: string]: unknown;
}

export type StructuredFunctionNode =
  | ItemizationNode
  | QuestionCorrelationNode
  | ChoiceTallyNode;

const STRUCTURED_FUNCTION_IDS: Record<StructuredFunctionNode["type"], string> = {
  itemizationTable: "itemization-table",
  questionCorrelationTable: "question-correlation-table",
  choiceTallyTable: "choice-tally-table",
};

const STRUCTURED_TOKEN_LABELS: Record<StructuredFunctionNode["type"], string> = {
  itemizationTable: "{ MULTIPLE QUESTION LIST }",
  questionCorrelationTable: "{ QUESTION CORRELATION TABLE }",
  choiceTallyTable: "{ RESPONSE BAR GRAPH }",
};

export function encodeStructuredNode(node: StructuredFunctionNode): string {
  return encodeURIComponent(JSON.stringify(node));
}

export function decodeStructuredNode(value: string | null): StructuredFunctionNode | null {
  if (!value) return null;
  try {
    const parsed = JSON.parse(decodeURIComponent(value)) as StructuredFunctionNode;
    if (
      parsed?.type === "itemizationTable" ||
      parsed?.type === "questionCorrelationTable" ||
      parsed?.type === "choiceTallyTable"
    ) {
      return parsed;
    }
    return null;
  } catch {
    return null;
  }
}

function flagToEnum(v: unknown, fallback = "false"): "true" | "false" {
  if (v === true || v === "true" || v === "yes" || v === 1 || v === "1") return "true";
  if (v === false || v === "false" || v === "no" || v === 0 || v === "0") return "false";
  return fallback === "true" ? "true" : "false";
}

function itemizationToConfig(node: ItemizationNode): FunctionConfig {
  const def = getFunctionDef("itemization-table");
  const cols = node.columns ?? [];
  const base = def ? defaultFunctionConfig(def) : {};
  return {
    ...base,
    "show-print-control": flagToEnum(node.showPrint ?? node["show-print-control"]),
    "show-export-control": flagToEnum(node.showExport ?? node["show-export-control"]),
    numberOfColumns: Math.max(1, cols.length),
    column: cols.map((c) => ({ header: c.header ?? "", contents: c.field ?? "" })),
    "form-name": node.form ?? "",
    conditionsRows: (node.conditions as FunctionConfig["conditionsRows"]) ?? [
      { field: "", op: "equals", value: "" },
    ],
    conditionsCombinator: node.combinator === "or" ? "or" : "and",
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

function choiceTallyToConfig(node: ChoiceTallyNode): FunctionConfig {
  const def = getFunctionDef("choice-tally-table");
  const base = def ? defaultFunctionConfig(def) : {};
  const field = String(node.field ?? "").trim();
  return {
    ...base,
    field: field.includes("<<") ? field : field ? `<<${field}>>` : "",
    conditionsRows: [{ field: "", op: "equals", value: "" }],
    conditionsCombinator: "and",
  };
}

function nodeToConfig(node: StructuredFunctionNode): FunctionConfig {
  if (node.type === "itemizationTable") return itemizationToConfig(node);
  if (node.type === "choiceTallyTable") return choiceTallyToConfig(node);
  return correlationToConfig(node);
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
      showPrint: flagToEnum(nextConfig["show-print-control"]) === "true",
      showExport: flagToEnum(nextConfig["show-export-control"]) === "true",
      conditions: nextConfig.conditionsRows ?? node.conditions,
      combinator: nextConfig.conditionsCombinator === "or" ? "or" : "and",
    };
  }

  if (node.type === "choiceTallyTable") {
    let field = String(nextConfig.field ?? "").trim();
    if (field.startsWith("<<") && field.endsWith(">>")) field = field.slice(2, -2).trim();
    const formPart = field.includes(":") ? field.split(":")[0] : String(node.form ?? "").trim();
    return {
      ...node,
      field,
      form: formPart || node.form,
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

/** Open Configure for a structured function token (itemization / correlation / choice tally). */
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
      tokenEl.textContent = STRUCTURED_TOKEN_LABELS[patched.type];
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
