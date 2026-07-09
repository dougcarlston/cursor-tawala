import { isValidIfConditionField } from "@/lib/fieldInsertion";
import { getSendFieldErrors } from "@/lib/sendEmailValidation";
import { buildConditionFromRows, UNARY_SKIP_OPERATORS } from "@/lib/skipSummary";
import type { TawalaProcessCommand, TawalaProject } from "@/types/tawala";

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

export interface SendBuilderState {
  to: string;
  cc: string;
  fromAddress: string;
  fromName: string;
  subject: string;
  document: string;
  documentReset: boolean;
  showPageHeader: boolean;
}

export const EMPTY_SEND_BUILDER: SendBuilderState = {
  to: "",
  cc: "",
  fromAddress: "",
  fromName: "",
  subject: "",
  document: "",
  documentReset: false,
  showPageHeader: false,
};

interface SendAddressJson {
  fieldRef?: string;
  literal?: string;
  aliasField?: string;
  aliasLiteral?: string;
}

function addressFieldToText(addr: unknown): string {
  if (addr == null) return "";
  if (typeof addr === "string") return addr;
  if (typeof addr === "object" && !Array.isArray(addr)) {
    const a = addr as SendAddressJson;
    if (a.fieldRef) return a.fieldRef;
    if (a.literal != null) return String(a.literal);
  }
  return "";
}

function fromAddressToParts(from: unknown): { address: string; name: string } {
  if (from == null || typeof from !== "object" || Array.isArray(from)) {
    return { address: addressFieldToText(from), name: "" };
  }
  const f = from as SendAddressJson;
  const address = f.fieldRef ?? (f.literal != null ? String(f.literal) : "");
  const name = f.aliasField ?? (f.aliasLiteral != null ? String(f.aliasLiteral) : "");
  return { address, name };
}

function buildSendAddress(
  text: string,
  knownVariables: ReadonlySet<string>,
): { fieldRef: string } | { literal: string } | undefined {
  const trimmed = text.trim();
  if (!trimmed) return undefined;
  const tokenOnly = trimmed.match(/^<<([^>]+)>>$/);
  if (tokenOnly) {
    const inner = tokenOnly[1].trim();
    if (inner.includes(":") || knownVariables.has(inner)) {
      return { fieldRef: inner };
    }
    return { literal: trimmed };
  }
  if (trimmed.includes("<<")) return { literal: trimmed };
  if (trimmed.includes(":") || knownVariables.has(trimmed)) {
    return { fieldRef: trimmed };
  }
  return { literal: trimmed };
}

function buildFromAddress(
  addressText: string,
  nameText: string,
  knownVariables: ReadonlySet<string>,
): SendAddressJson | undefined {
  const addr = buildSendAddress(addressText, knownVariables);
  if (!addr) return undefined;
  const result: SendAddressJson = { ...addr };
  const name = nameText.trim();
  if (!name) return result;
  const tokenOnly = name.match(/^<<([^>]+)>>$/);
  if (tokenOnly) {
    const inner = tokenOnly[1].trim();
    if (inner.includes(":") || knownVariables.has(inner)) {
      result.aliasField = inner;
    } else {
      result.aliasLiteral = name;
    }
    return result;
  }
  if (!name.includes("<<") && (name.includes(":") || knownVariables.has(name))) {
    result.aliasField = name;
  } else {
    result.aliasLiteral = name;
  }
  return result;
}

export function sendBuilderIsValid(
  state: SendBuilderState,
  documentNames: readonly string[],
  project: TawalaProject,
  knownVariables: ReadonlySet<string>,
): boolean {
  if (
    !state.to.trim() ||
    !state.subject.trim() ||
    !state.document.trim() ||
    !documentNames.includes(state.document)
  ) {
    return false;
  }
  const errors = getSendFieldErrors(state, project, knownVariables);
  return !errors.to && !errors.cc && !errors.fromAddress;
}

export function sendBuilderFromCommand(command: {
  to?: unknown;
  cc?: unknown;
  from?: unknown;
  subject?: unknown;
  body?: unknown;
  document?: unknown;
  reset?: unknown;
  [key: string]: unknown;
}): SendBuilderState {
  const body = command.body as
    | { document?: string; reset?: boolean; showHeader?: boolean }
    | undefined;
  const document = body?.document ?? String(command.document ?? "");
  const { address: fromAddress, name: fromName } = fromAddressToParts(command.from);
  return {
    to: addressFieldToText(command.to),
    cc: addressFieldToText(command.cc),
    fromAddress,
    fromName,
    subject: command.subject == null ? "" : String(command.subject),
    document,
    documentReset: body?.reset === true || command.reset === true,
    showPageHeader: body?.showHeader !== false,
  };
}

/** Build a process command from the Send builder state (Email tab). */
export function buildSendCommand(
  state: SendBuilderState,
  knownVariables: ReadonlySet<string>,
  hasPageHeaderContent: boolean,
): TawalaProcessCommand {
  const cmd: TawalaProcessCommand = { cmd: "send" };
  const to = buildSendAddress(state.to, knownVariables);
  if (to) cmd.to = to;
  const cc = buildSendAddress(state.cc, knownVariables);
  if (cc) cmd.cc = cc;
  const from = buildFromAddress(state.fromAddress, state.fromName, knownVariables);
  if (from) cmd.from = from;
  const subject = state.subject.trim();
  if (subject) cmd.subject = subject;
  const body: { document: string; reset?: boolean; showHeader?: boolean } = {
    document: state.document,
  };
  if (state.documentReset) body.reset = true;
  if (hasPageHeaderContent && !state.showPageHeader) body.showHeader = false;
  cmd.body = body;
  return cmd;
}

export interface AppendBuilderState {
  appendage: string;
  document: string;
}

export const EMPTY_APPEND_BUILDER: AppendBuilderState = {
  appendage: "",
  document: "",
};

export function appendBuilderIsValid(
  state: AppendBuilderState,
  documentNames: readonly string[],
): boolean {
  return (
    state.appendage.trim().length > 0 &&
    state.document.trim().length > 0 &&
    documentNames.includes(state.appendage) &&
    documentNames.includes(state.document)
  );
}

export function appendBuilderFromCommand(command: {
  appendage?: unknown;
  document?: unknown;
  [key: string]: unknown;
}): AppendBuilderState {
  return {
    appendage: String(command.appendage ?? ""),
    document: String(command.document ?? ""),
  };
}

export function buildAppendCommand(state: AppendBuilderState): TawalaProcessCommand {
  return {
    cmd: "append",
    appendage: state.appendage,
    document: state.document,
  };
}

export interface GetBuilderState {
  recordList: string;
  sourceForm: string;
  whereCombinator: ConditionCombinator;
  whereRows: ConditionRow[];
}

export const EMPTY_GET_BUILDER: GetBuilderState = {
  recordList: "",
  sourceForm: "",
  whereCombinator: "and",
  whereRows: [{ ...EMPTY_CONDITION_ROW }],
};

/** Legacy `RecordSetNamer.GetNextName()` — next unused `Record List N` in a process. */
export function nextRecordListName(commands: TawalaProcessCommand[]): string {
  const used = new Set<string>();
  const walk = (nodes: TawalaProcessCommand[] | undefined) => {
    if (!nodes) return;
    for (const cmd of nodes) {
      if (cmd.cmd === "get" && cmd.recordList != null) {
        used.add(String(cmd.recordList));
      }
      walk(cmd.then as TawalaProcessCommand[] | undefined);
      walk(cmd.else as TawalaProcessCommand[] | undefined);
      walk(cmd.do as TawalaProcessCommand[] | undefined);
    }
  };
  walk(commands);
  let n = 1;
  while (used.has(`Record List ${n}`)) n++;
  return `Record List ${n}`;
}

export function getWhereIsValid(
  rows: ConditionRow[],
  knownVariables: ReadonlySet<string>,
): boolean {
  if (!rows.some((r) => r.field.trim())) return true;
  return rowsAreValid(rows, knownVariables);
}

export function getBuilderIsValid(
  state: GetBuilderState,
  formNames: readonly string[],
  knownVariables: ReadonlySet<string>,
): boolean {
  if (!state.recordList.trim()) return false;
  if (!state.sourceForm.trim() || !formNames.includes(state.sourceForm)) return false;
  return getWhereIsValid(state.whereRows, knownVariables);
}

/** True when condition rows contain user input beyond the default blank row. */
export function conditionRowsHaveDraft(rows: ConditionRow[]): boolean {
  if (rows.length > 1) return true;
  return rows.some(
    (r) => r.field.trim() !== "" || r.value.trim() !== "" || r.op !== "isBlank",
  );
}

export function ifBuilderHasDraft(state: IfBuilderState): boolean {
  return state.hasElse || conditionRowsHaveDraft(state.rows);
}

export function setBuilderHasDraft(state: SetBuilderState): boolean {
  return (
    state.field.trim() !== "" ||
    state.value.trim() !== "" ||
    state.arithmeticAsText
  );
}

export function showBuilderHasDraft(state: ShowBuilderState): boolean {
  return (
    state.document.trim() !== "" ||
    state.documentReset ||
    state.form.trim() !== "" ||
    state.recordForm.trim() !== "" ||
    state.recordSubmit !== "modify" ||
    state.url.trim() !== "" ||
    state.tab !== "document" ||
    conditionRowsHaveDraft(state.recordRows)
  );
}

export function sendBuilderHasDraft(state: SendBuilderState): boolean {
  return (
    state.to.trim() !== "" ||
    state.cc.trim() !== "" ||
    state.fromAddress.trim() !== "" ||
    state.fromName.trim() !== "" ||
    state.subject.trim() !== "" ||
    state.document.trim() !== "" ||
    state.documentReset ||
    state.showPageHeader
  );
}

export function appendBuilderHasDraft(state: AppendBuilderState): boolean {
  return state.appendage.trim() !== "" || state.document.trim() !== "";
}

export function getBuilderHasDraft(state: GetBuilderState): boolean {
  return (
    state.recordList.trim() !== "" ||
    state.sourceForm.trim() !== "" ||
    conditionRowsHaveDraft(state.whereRows)
  );
}

export function getBuilderFromCommand(command: {
  recordList?: unknown;
  sourceForms?: unknown;
  where?: unknown;
  [key: string]: unknown;
}): GetBuilderState {
  const forms = command.sourceForms as string[] | undefined;
  const { combinator, rows } = parseConditionToRows(
    command.where as ConditionShape | undefined,
  );
  return {
    recordList: String(command.recordList ?? ""),
    sourceForm: forms?.[0] ?? "",
    whereCombinator: combinator,
    whereRows: rows,
  };
}

export function buildGetCommand(state: GetBuilderState): TawalaProcessCommand {
  const cmd: TawalaProcessCommand = {
    cmd: "get",
    recordList: state.recordList.trim(),
    sourceForms: [state.sourceForm],
  };
  if (state.whereRows.some((r) => r.field.trim())) {
    cmd.where = buildConditionFromRows(state.whereCombinator, state.whereRows);
  }
  return cmd;
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
