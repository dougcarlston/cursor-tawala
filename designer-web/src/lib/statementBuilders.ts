import { isValidIfConditionField } from "@/lib/fieldInsertion";
import { buildConditionFromRows, UNARY_SKIP_OPERATORS } from "@/lib/skipSummary";
import type { TawalaProcessCommand } from "@/types/tawala";

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

export type ShowTab = "document" | "form" | "storedRecord" | "url";

export interface ShowBuilderState {
  tab: ShowTab;
  document: string;
  documentReset: boolean;
  form: string;
  recordForm: string;
  recordSubmit: "modify" | "new";
  recordCombinator: ConditionCombinator;
  recordRows: ConditionRow[];
  url: string;
}

export const EMPTY_SHOW_BUILDER: ShowBuilderState = {
  tab: "document",
  document: "",
  documentReset: false,
  form: "",
  recordForm: "",
  recordSubmit: "modify",
  recordCombinator: "and",
  recordRows: [{ ...EMPTY_CONDITION_ROW }],
  url: "",
};

/** Stored Record Where is optional — empty rows are valid when `from` is set. */
export function showStoredRecordIsValid(
  recordForm: string,
  rows: ConditionRow[],
  knownVariables: ReadonlySet<string>,
): boolean {
  if (!recordForm.trim()) return false;
  if (!rows.some((r) => r.field.trim())) return true;
  return rowsAreValid(rows, knownVariables);
}

export function showBuilderIsValid(
  state: ShowBuilderState,
  knownVariables: ReadonlySet<string>,
  documentNames: readonly string[],
  formNames: readonly string[],
): boolean {
  switch (state.tab) {
    case "document":
      return state.document.trim().length > 0 && documentNames.includes(state.document);
    case "form":
      return state.form.trim().length > 0 && formNames.includes(state.form);
    case "storedRecord":
      return showStoredRecordIsValid(state.recordForm, state.recordRows, knownVariables);
    case "url":
      return state.url.trim().length > 0;
    default:
      return false;
  }
}

export function showBuilderFromCommand(command: {
  cmd?: unknown;
  document?: unknown;
  form?: unknown;
  url?: unknown;
  reset?: unknown;
  submit?: unknown;
  condition?: unknown;
  [key: string]: unknown;
}): ShowBuilderState {
  const base = { ...EMPTY_SHOW_BUILDER };
  if (command.cmd === "edit") {
    const { combinator, rows } = parseConditionToRows(
      command.condition as ConditionShape | undefined,
    );
    return {
      ...base,
      tab: "storedRecord",
      recordForm: String(command.form ?? ""),
      recordSubmit: command.submit === "new" ? "new" : "modify",
      recordCombinator: combinator,
      recordRows: rows,
    };
  }
  if (command.cmd === "show" || command.cmd === "showDocument") {
    if (command.url != null && String(command.url).length > 0) {
      return { ...base, tab: "url", url: String(command.url) };
    }
    if (command.form != null && String(command.form).length > 0) {
      return { ...base, tab: "form", form: String(command.form) };
    }
    if (command.document != null && String(command.document).length > 0) {
      return {
        ...base,
        tab: "document",
        document: String(command.document),
        documentReset: command.reset === true,
      };
    }
  }
  return base;
}

/** Build a process command from the Show builder state (active tab). */
export function buildShowCommand(state: ShowBuilderState): TawalaProcessCommand {
  switch (state.tab) {
    case "document": {
      const cmd: TawalaProcessCommand = { cmd: "show", document: state.document };
      if (state.documentReset) cmd.reset = true;
      return cmd;
    }
    case "form":
      return { cmd: "show", form: state.form };
    case "storedRecord": {
      const cmd: TawalaProcessCommand = {
        cmd: "edit",
        form: state.recordForm,
        submit: state.recordSubmit,
      };
      if (state.recordRows.some((r) => r.field.trim())) {
        cmd.condition = buildConditionFromRows(state.recordCombinator, state.recordRows);
      }
      return cmd;
    }
    case "url":
      return { cmd: "show", url: state.url };
    default:
      return { cmd: "show" };
  }
}
