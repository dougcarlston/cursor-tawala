import { isValidIfConditionField } from "@/lib/fieldInsertion";
import { UNARY_SKIP_OPERATORS } from "@/lib/skipSummary";

export type ConditionCombinator = "and" | "or";

export interface ConditionRow {
  field: string;
  op: string;
  value: string;
}

export interface IfBuilderState {
  combinator: ConditionCombinator;
  rows: ConditionRow[];
  hasElse: boolean;
}

export interface SetBuilderState {
  field: string;
  value: string;
  arithmeticAsText: boolean;
}

export const EMPTY_CONDITION_ROW: ConditionRow = { field: "", op: "isBlank", value: "" };

export const EMPTY_IF_BUILDER: IfBuilderState = {
  combinator: "and",
  rows: [{ ...EMPTY_CONDITION_ROW }],
  hasElse: false,
};

export const EMPTY_SET_BUILDER: SetBuilderState = {
  field: "",
  value: "",
  arithmeticAsText: false,
};

export function rowsAreValid(
  rows: ConditionRow[],
  knownVariables: ReadonlySet<string>,
): boolean {
  return rows.every((r) => {
    if (!isValidIfConditionField(r.field, knownVariables)) return false;
    if (UNARY_SKIP_OPERATORS.has(r.op)) return true;
    return r.value.trim().length > 0;
  });
}

export function expressionHasArithmetic(value: string): boolean {
  return /[+\-*/]/.test(value);
}

export function setBuilderIsValid(state: SetBuilderState): boolean {
  return state.field.trim().length > 0 && state.value.trim().length > 0;
}

interface ConditionShape {
  op?: string;
  field?: string;
  value?: unknown;
  conditions?: ConditionShape[];
  and?: ConditionShape[];
  or?: ConditionShape[];
}

function clauseToRow(cond: ConditionShape): ConditionRow {
  return {
    field: String(cond.field ?? ""),
    op: String(cond.op ?? "equals"),
    value: cond.value === undefined || cond.value === null ? "" : String(cond.value),
  };
}

/** Parse a stored If condition back into builder rows (for Modify mode). */
export function parseConditionToRows(condition: ConditionShape | undefined): {
  combinator: ConditionCombinator;
  rows: ConditionRow[];
} {
  if (!condition) {
    return { combinator: "and", rows: [{ ...EMPTY_CONDITION_ROW }] };
  }
  const andList = condition.op === "and" ? condition.conditions : condition.and;
  if (andList?.length) {
    return { combinator: "and", rows: andList.map(clauseToRow) };
  }
  const orList = condition.op === "or" ? condition.conditions : condition.or;
  if (orList?.length) {
    return { combinator: "or", rows: orList.map(clauseToRow) };
  }
  return { combinator: "and", rows: [clauseToRow(condition)] };
}

export function ifBuilderFromCommand(command: {
  condition?: unknown;
  else?: unknown;
  [key: string]: unknown;
}): IfBuilderState {
  const { combinator, rows } = parseConditionToRows(command.condition as ConditionShape | undefined);
  const hasElse = command.else !== undefined;
  return { combinator, rows, hasElse };
}

export function setBuilderFromCommand(command: {
  field?: unknown;
  value?: unknown;
  arithmeticAsText?: unknown;
  [key: string]: unknown;
}): SetBuilderState {
  return {
    field: String(command.field ?? ""),
    value: command.value === undefined || command.value === null ? "" : String(command.value),
    arithmeticAsText: command.arithmeticAsText === true,
  };
}
