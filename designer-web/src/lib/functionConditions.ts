import { SKIP_OPERATOR_LABELS, UNARY_SKIP_OPERATORS } from "./skipSummary";

export type FunctionConditionsCombinator = "and" | "or";

export interface FunctionConditionRow {
  field: string;
  op: string;
  value: string;
}

export interface FunctionConditionsState {
  combinator: FunctionConditionsCombinator;
  rows: FunctionConditionRow[];
}

export const EMPTY_FUNCTION_CONDITION_ROW: FunctionConditionRow = {
  field: "",
  op: "equals",
  value: "",
};

export const DEFAULT_FUNCTION_CONDITIONS: FunctionConditionsState = {
  combinator: "and",
  rows: [{ ...EMPTY_FUNCTION_CONDITION_ROW }],
};

export function parseFunctionConditions(config: Record<string, unknown>): FunctionConditionsState {
  const raw = config.conditionsRows;
  if (Array.isArray(raw) && raw.length > 0) {
    return {
      combinator: config.conditionsCombinator === "or" ? "or" : "and",
      rows: raw.map((row) => ({
        field: String((row as FunctionConditionRow).field ?? ""),
        op: String((row as FunctionConditionRow).op ?? "equals"),
        value: String((row as FunctionConditionRow).value ?? ""),
      })),
    };
  }
  return { ...DEFAULT_FUNCTION_CONDITIONS, rows: [{ ...EMPTY_FUNCTION_CONDITION_ROW }] };
}

export function functionConditionsToConfig(state: FunctionConditionsState): Record<string, unknown> {
  return {
    conditionsRows: state.rows,
    conditionsCombinator: state.combinator,
  };
}

export function functionConditionsRowIsEmpty(row: FunctionConditionRow): boolean {
  return !row.field.trim();
}

export function functionConditionsRowIsComplete(row: FunctionConditionRow): boolean {
  if (!row.field.trim()) return false;
  if (UNARY_SKIP_OPERATORS.has(row.op)) return true;
  return row.value.trim().length > 0;
}

/** Legacy OK rules — empty single row = all records; multiple rows must all be complete. */
export function functionConditionsAreValid(rows: FunctionConditionRow[]): boolean {
  if (rows.length === 0) return false;
  if (rows.length === 1) {
    const row = rows[0];
    return functionConditionsRowIsEmpty(row) || functionConditionsRowIsComplete(row);
  }
  return rows.every(functionConditionsRowIsComplete);
}

/** Legacy `ToDisplayString` fragment, e.g. `Record:Form 2:Q1:a equals "foo"`. */
export function formatFunctionConditionsDisplay(state: FunctionConditionsState): string | null {
  const filled = state.rows.filter((r) => r.field.trim());
  if (!filled.length) return null;
  const joiner = state.combinator === "or" ? " OR " : " AND ";
  return filled
    .map((row) => {
      const opLabel = SKIP_OPERATOR_LABELS[row.op] ?? row.op;
      if (UNARY_SKIP_OPERATORS.has(row.op)) {
        return `${row.field.trim()} ${opLabel}`;
      }
      const val = row.value.trim();
      const quoted = val.includes('"') ? val : `"${val}"`;
      return `${row.field.trim()} ${opLabel} ${quoted}`;
    })
    .join(joiner);
}
