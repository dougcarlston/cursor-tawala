/**
 * Map DYNAMIC MCQ Configure Function ↔ McItem dynamic choice JSON.
 */

import {
  defaultFunctionConfig,
  getFunctionDef,
  type FunctionConfig,
} from "./functionCatalog";
import type { FunctionConditionRow } from "./functionConditions";

export const DYNAMIC_MCQ_FUNCTION_ID = "dynamic-mcq";

export type DynamicMcChoice = {
  type: "dynamic";
  sourceForm?: string;
  displayExpr?: string;
  valueExpr?: string;
  sortExpr?: string;
  /** Imported nested filter tree when it cannot flatten to conditionsRows. */
  where?: unknown;
  conditionsRows?: FunctionConditionRow[];
  conditionsCombinator?: "and" | "or";
};

export function getDynamicMcqDef() {
  const def = getFunctionDef(DYNAMIC_MCQ_FUNCTION_ID);
  if (!def) throw new Error("dynamic-mcq missing from function catalog");
  return def;
}

export function findDynamicChoice(
  choices: { type?: string }[] | undefined,
): DynamicMcChoice | undefined {
  return (choices ?? []).find((c) => c.type === "dynamic") as DynamicMcChoice | undefined;
}

/**
 * Dynamic MCQ display/value/sort run inside a record iteration.
 * Java skips choices when the expression evaluates blank — so `Form:Field`
 * (Fields palette) must become `Record:Form:Field` (DirtBowl / SignupSheets).
 */
export function normalizeDynamicMcqFieldRef(
  expr: string,
  sourceForm?: string,
): string {
  const raw = String(expr ?? "").trim();
  if (!raw) return "";
  const wrapped = raw.startsWith("<<") && raw.endsWith(">>");
  const inner = (wrapped ? raw.slice(2, -2) : raw).trim();
  if (!inner) return "";

  let field = inner;
  if (!/^Record:/i.test(field)) {
    if (field.includes(":")) {
      field = `Record:${field}`;
    } else if (sourceForm?.trim()) {
      field = `Record:${sourceForm.trim()}:${field}`;
    } else {
      field = `Record:${field}`;
    }
  } else if (!field.startsWith("Record:")) {
    // Preserve path after a case-variant Record: prefix
    field = `Record:${field.slice(field.indexOf(":") + 1)}`;
  }

  return `<<${field}>>`;
}

/** Normalize a condition field name (no <<>> wrappers). */
export function normalizeDynamicMcqConditionField(
  field: string,
  sourceForm?: string,
): string {
  return normalizeDynamicMcqFieldRef(field, sourceForm).replace(/^<<|>>$/g, "");
}

/**
 * Flatten a simple imported `where` tree into Configure Function rows.
 * Complex nested trees return null (keep `where` on the choice for Deploy later).
 */
export function conditionRowsFromWhere(where: unknown): {
  rows: FunctionConditionRow[];
  combinator: "and" | "or";
} | null {
  if (!where || typeof where !== "object") return null;
  const w = where as Record<string, unknown>;
  if (typeof w.field === "string" && typeof w.op === "string") {
    return {
      rows: [{ field: w.field, op: w.op, value: String(w.value ?? "") }],
      combinator: "and",
    };
  }
  for (const combinator of ["and", "or"] as const) {
    const list = w[combinator];
    if (!Array.isArray(list) || !list.length) continue;
    const rows: FunctionConditionRow[] = [];
    for (const item of list) {
      if (!item || typeof item !== "object") return null;
      const c = item as Record<string, unknown>;
      if (typeof c.field !== "string" || typeof c.op !== "string") return null;
      if ("and" in c || "or" in c) return null;
      rows.push({ field: c.field, op: c.op, value: String(c.value ?? "") });
    }
    return { rows, combinator };
  }
  return null;
}

/** Build Configure Function initial config from an existing dynamic choice (or empty). */
export function configFromDynamicChoice(choice?: DynamicMcChoice | null): FunctionConfig {
  const def = getDynamicMcqDef();
  const base = defaultFunctionConfig(def);
  if (!choice) return base;
  let rows = choice.conditionsRows;
  let combinator = choice.conditionsCombinator === "or" ? ("or" as const) : ("and" as const);
  if (!rows?.length && choice.where) {
    const flat = conditionRowsFromWhere(choice.where);
    if (flat) {
      rows = flat.rows;
      combinator = flat.combinator;
    }
  }
  const sourceForm = choice.sourceForm ?? "";
  return {
    ...base,
    "form-name": sourceForm,
    "display-expression": choice.displayExpr
      ? normalizeDynamicMcqFieldRef(choice.displayExpr, sourceForm)
      : "",
    "value-expression": choice.valueExpr
      ? normalizeDynamicMcqFieldRef(choice.valueExpr, sourceForm)
      : "",
    "sort-expression": choice.sortExpr
      ? normalizeDynamicMcqFieldRef(choice.sortExpr, sourceForm)
      : "",
    conditionsRows: rows?.length ? rows : (base.conditionsRows as FunctionConditionRow[]),
    conditionsCombinator: combinator,
  };
}

/** Persist Configure Function OK → dynamic choice object for McItem.choices. */
export function dynamicChoiceFromConfig(config: FunctionConfig): DynamicMcChoice {
  const sourceForm = String(config["form-name"] ?? "").trim();
  const rows = (config.conditionsRows as FunctionConditionRow[] | undefined) ?? [];
  const filled = rows.filter((r) => String(r?.field ?? "").trim());
  const choice: DynamicMcChoice = {
    type: "dynamic",
    sourceForm,
    displayExpr: normalizeDynamicMcqFieldRef(
      String(config["display-expression"] ?? "").trim(),
      sourceForm,
    ),
    valueExpr: normalizeDynamicMcqFieldRef(
      String(config["value-expression"] ?? "").trim(),
      sourceForm,
    ),
  };
  const sort = String(config["sort-expression"] ?? "").trim();
  if (sort) choice.sortExpr = normalizeDynamicMcqFieldRef(sort, sourceForm);
  if (filled.length) {
    choice.conditionsRows = rows.map((r) => ({
      field: normalizeDynamicMcqConditionField(String(r.field ?? ""), sourceForm),
      op: String(r.op ?? "equals"),
      value: String(r.value ?? ""),
    }));
    choice.conditionsCombinator = config.conditionsCombinator === "or" ? "or" : "and";
  }
  return choice;
}

/** Empty dynamic stub when switching Choice source to stored. */
export function emptyDynamicChoice(): DynamicMcChoice {
  return { type: "dynamic", sourceForm: "", displayExpr: "", valueExpr: "" };
}
