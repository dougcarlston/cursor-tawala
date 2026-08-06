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
import {
  buildFunctionDisplayString,
  serializeFunctionConfig,
} from "./functionTokens";

export const STRUCTURED_NODE_DATA_ATTR = "data-tawala-structured-node";
export const ITEMIZATION_TOKEN_DATA_ATTR = "data-itemization-token";

export interface ItemizationNode {
  type: "itemizationTable";
  form?: string;
  version?: number;
  columns?: { header: string; field: string; displayCondition?: unknown }[];
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

function whereTreeToConditionRows(where: unknown): FunctionConfig["conditionsRows"] | null {
  if (!where || typeof where !== "object") return null;
  const w = where as Record<string, unknown>;
  if (typeof w.field === "string" && typeof w.op === "string") {
    return [{ field: w.field, op: w.op, value: String(w.value ?? "") }];
  }
  for (const combinator of ["and", "or"] as const) {
    const list = w[combinator];
    if (!Array.isArray(list) || list.length === 0) continue;
    const rows: NonNullable<FunctionConfig["conditionsRows"]> = [];
    for (const item of list) {
      const nested = whereTreeToConditionRows(item);
      if (!nested) return null;
      rows.push(...nested);
    }
    return rows;
  }
  return null;
}

/** Structured / imported MQL node → Configure Function config (incl. Where rows). */
export function itemizationToConfig(node: ItemizationNode): FunctionConfig {
  const def = getFunctionDef("itemization-table");
  const cols = node.columns ?? [];
  const base = def ? defaultFunctionConfig(def) : {};
  // Prefer `conditionsRows` (modern function-token) then `conditions` (structured Configure).
  const fromConditions = Array.isArray(node.conditionsRows)
    ? (node.conditionsRows as FunctionConfig["conditionsRows"])
    : Array.isArray(node.conditions)
      ? (node.conditions as FunctionConfig["conditionsRows"])
      : null;
  const filledFromConditions = (fromConditions ?? []).filter((r) => String(r?.field ?? "").trim());
  const fromWhere = filledFromConditions.length
    ? null
    : whereTreeToConditionRows(node.where);
  return {
    ...base,
    "show-print-control": flagToEnum(node.showPrint ?? node["show-print-control"]),
    "show-export-control": flagToEnum(node.showExport ?? node["show-export-control"]),
    numberOfColumns: Math.max(1, cols.length),
    column: cols.map((c) => {
      const col: ColumnConfig = { header: c.header ?? "", contents: c.field ?? "" };
      if (c.displayCondition != null) col.displayCondition = c.displayCondition;
      return col;
    }),
    "form-name": node.form ?? "",
    conditionsRows: (filledFromConditions.length
      ? fromConditions
      : fromWhere ?? [{ field: "", op: "equals", value: "" }]) as FunctionConfig["conditionsRows"],
    conditionsCombinator:
      node.conditionsCombinator === "or" || node.combinator === "or" ? "or" : "and",
  };
}

/** Configure / function-token config → structured itemizationTable node for Deploy. */
export function functionConfigToItemizationNode(config: FunctionConfig): ItemizationNode {
  const cols = (config.column as ColumnConfig[] | undefined) ?? [];
  const n = Number(config.numberOfColumns ?? cols.length) || cols.length;
  let form = String(config["form-name"] ?? "").trim();
  if (!form) {
    for (const col of cols) {
      const bare = String(col?.contents ?? "")
        .replace(/^<<|>>$/g, "")
        .replace(/^Record:/i, "")
        .trim();
      if (bare.includes(":")) {
        form = bare.split(":")[0] ?? "";
        if (form) break;
      }
    }
  }
  if (!form) {
    for (const row of config.conditionsRows ?? []) {
      const bare = String(row?.field ?? "")
        .replace(/^<<|>>$/g, "")
        .replace(/^Record:/i, "")
        .trim();
      if (bare.includes(":")) {
        form = bare.split(":")[0] ?? "";
        if (form) break;
      }
    }
  }
  return {
    type: "itemizationTable",
    form: form || undefined,
    columns: cols.slice(0, n).map((c) => {
      const col: { header: string; field: string; displayCondition?: unknown } = {
        header: c.header ?? "",
        field: c.contents ?? "",
      };
      if (c.displayCondition != null) col.displayCondition = c.displayCondition;
      return col;
    }),
    showPrint: flagToEnum(config["show-print-control"]) === "true",
    showExport: flagToEnum(config["show-export-control"]) === "true",
    conditions: config.conditionsRows ?? [{ field: "", op: "equals", value: "" }],
    conditionsRows: config.conditionsRows ?? [{ field: "", op: "equals", value: "" }],
    combinator: config.conditionsCombinator === "or" ? "or" : "and",
    conditionsCombinator: config.conditionsCombinator === "or" ? "or" : "and",
  };
}

/**
 * Present-day Design chip: `<<MULTIPLE QUESTION LIST(false, false, ...)>>` with
 * `data-function-config` (Where lives in conditionsRows). Legacy `{ MULTIPLE QUESTION LIST }`
 * spans with only `data-tawala-structured-node` remain parseable for old projects.
 */
export function itemizationFunctionTokenHtml(node: ItemizationNode): string {
  const def = getFunctionDef("itemization-table");
  const config = itemizationToConfig(node);
  const display = def
    ? buildFunctionDisplayString(def, config)
    : STRUCTURED_TOKEN_LABELS.itemizationTable;
  const form = String(node.form ?? config["form-name"] ?? "").trim();
  const hasColDc = (node.columns ?? []).some((c) => c.displayCondition != null);
  const warnClass = hasColDc ? " preserved-import-warning" : "";
  const title = hasColDc
    ? "MULTIPLE QUESTION LIST — Visibility condition preserved; Designer cannot edit yet"
    : "MULTIPLE QUESTION LIST";
  return (
    `<span contenteditable="false" class="function-token function-table-token${warnClass}" ` +
    `data-function-id="itemization-table" ` +
    `data-function-config="${escAttr(serializeFunctionConfig(config))}" ` +
    (form ? `data-itemization-form="${escAttr(form)}" ` : "") +
    `data-itemization-token="true" ` +
    `${STRUCTURED_NODE_DATA_ATTR}="${escAttr(encodeStructuredNode(functionConfigToItemizationNode(config)))}" ` +
    `title="${escAttr(title)}" draggable="true">` +
    `${escText(display)}</span>`
  );
}

function escAttr(text: string): string {
  return String(text ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&#39;");
}

function escText(text: string): string {
  return String(text ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
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
    const rows = nextConfig.conditionsRows ?? node.conditions;
    const combinator = nextConfig.conditionsCombinator === "or" ? "or" : "and";
    const next: ItemizationNode = {
      ...node,
      form: form || node.form,
      columns: nextCols.slice(0, n).map((c) => ({
        header: c.header ?? "",
        field: c.contents ?? "",
      })),
      showPrint: flagToEnum(nextConfig["show-print-control"]) === "true",
      showExport: flagToEnum(nextConfig["show-export-control"]) === "true",
      conditions: rows,
      conditionsRows: rows,
      combinator,
      conditionsCombinator: combinator,
    };
    // Configure edits `conditions` rows; drop imported `where` so Deploy uses rows.
    delete next.where;
    return next;
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
      // Prefer present-day function-token chrome (`<<NAME(...)>>` + data-function-config)
      // so Deploy/Preview read Where from the same path as Insert → Function.
      if (patched.type === "itemizationTable") {
        const html = itemizationFunctionTokenHtml(patched);
        const wrap = document.createElement("div");
        wrap.innerHTML = html;
        const nextEl = wrap.firstElementChild;
        if (nextEl instanceof HTMLElement) {
          tokenEl.replaceWith(nextEl);
          onPatched();
          return;
        }
      }
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
