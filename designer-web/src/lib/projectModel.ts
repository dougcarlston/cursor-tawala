import type { BlankValidation, FormItem, TawalaForm, TawalaProcess, TawalaProject } from "@/types/tawala";

export interface FieldLeaf {
  /** Field reference name (the token used in <<…>> references). */
  name: string;
  /** Value placed on the drag DataTransfer (Phase 2 wires editor drop targets). */
  dragValue: string;
}

export interface LinkedProcess {
  /** "Pre" (preProcess) or "Post" (process). */
  role: "Pre" | "Post";
  /** Name of the linked process (matches an entry in project.processes). */
  name: string;
}

const DEFAULT_BLANK_LETTER = /^[a-z]$/;

/** Field name a FIB blank contributes, mirroring server fibToXml (alternateLabel ?? name). */
export function blankFieldName(item: FormItem, blankIndex: number): string | null {
  if (item.type !== "fib" || !item.blanks) return null;
  const blank = item.blanks[blankIndex];
  if (!blank) return null;
  const explicit = blank.alternateLabel?.trim() || blank.name?.trim();
  // Single-blank FIBs with a default letter name inherit the item label, matching legacy GetFormItemFields.
  if (
    item.blanks.length === 1 &&
    (!blank.alternateLabel || !blank.alternateLabel.trim()) &&
    blank.name &&
    DEFAULT_BLANK_LETTER.test(blank.name)
  ) {
    return item.label;
  }
  return explicit || item.label;
}

/**
 * Response field names a form contributes to the Fields tree — parity with legacy
 * `form.GetFormItemFields()`. Only FIB blanks, MCQ, and Hidden Field items produce
 * fields; headings/text/breaks/skip instructions do not.
 */
export function formFieldNames(form: TawalaForm): FieldLeaf[] {
  const leaves: FieldLeaf[] = [];
  const seen = new Set<string>();
  const push = (name: string | null | undefined) => {
    const trimmed = name?.trim();
    if (!trimmed || seen.has(trimmed)) return;
    seen.add(trimmed);
    leaves.push({ name: trimmed, dragValue: trimmed });
  };

  for (const item of form.items) {
    switch (item.type) {
      case "fib":
        (item.blanks ?? []).forEach((_, i) => push(blankFieldName(item, i)));
        break;
      case "mc":
        push(item.name ?? item.alternateLabel ?? item.label);
        break;
      case "field":
        push(item.fieldName ?? item.name ?? item.label);
        break;
      default:
        break;
    }
  }
  return leaves;
}

/** Resolve a `Form:Field` reference to its FIB blank validation, if any. */
export function lookupFormFieldBlank(
  project: TawalaProject,
  qualifiedRef: string,
): { validation?: BlankValidation } | null {
  const colon = qualifiedRef.indexOf(":");
  if (colon < 0) return null;
  const formName = qualifiedRef.slice(0, colon);
  const fieldName = qualifiedRef.slice(colon + 1).trim();
  if (!fieldName) return null;
  const form = project.forms.find((f) => f.name === formName);
  if (!form) return null;

  for (const item of form.items) {
    if (item.type !== "fib" || !item.blanks) continue;
    for (let i = 0; i < item.blanks.length; i++) {
      const name = blankFieldName(item, i);
      if (name && name.localeCompare(fieldName, undefined, { sensitivity: "accent" }) === 0) {
        return { validation: item.blanks[i].validation };
      }
    }
  }
  return null;
}

/** Strip `<<…>>` and optional `Record:` prefix; return Form + field leaf. */
export function parseFormFieldRef(raw: string): { form: string; field: string } | null {
  let s = String(raw ?? "").trim();
  if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
  if (/^Record:/i.test(s)) s = s.slice("Record:".length).trim();
  const colon = s.indexOf(":");
  if (colon < 0) return null;
  const form = s.slice(0, colon).trim();
  const field = s.slice(colon + 1).trim();
  if (!form || !field) return null;
  return { form, field };
}

/**
 * Resolve a Where / Fields ref to an MCQ item (`type: "mc"`).
 * Accepts `Form:MCQ4`, `Record:Form:MCQ4`, or `<<Form:MCQ4>>`.
 */
export function lookupFormFieldMcItem(
  project: TawalaProject,
  qualifiedRef: string,
): Extract<FormItem, { type: "mc" }> | null {
  const parsed = parseFormFieldRef(qualifiedRef);
  if (!parsed) return null;
  const form = project.forms.find((f) => f.name === parsed.form);
  if (!form) return null;
  for (const item of form.items) {
    if (item.type !== "mc") continue;
    const keys = [item.name, item.alternateLabel, item.label]
      .map((k) => String(k ?? "").trim())
      .filter(Boolean);
    if (
      keys.some(
        (k) => k.localeCompare(parsed.field, undefined, { sensitivity: "accent" }) === 0,
      )
    ) {
      return item;
    }
  }
  return null;
}

function isPlainVariableName(value: unknown): value is string {
  return (
    typeof value === "string" &&
    value.trim().length > 0 &&
    !value.includes(":") &&
    !value.includes("<<") &&
    !/\s/.test(value)
  );
}

/** Variable present in every project — the private-invitation token (legacy `_InviteeID`). */
export const INVITEE_ID_VARIABLE = "_InviteeID";

/** Statements whose target field defines a variable (Set / Append / arithmetic). */
const ASSIGNMENT_COMMANDS = new Set([
  "set",
  "append",
  "addTo",
  "subtractFrom",
  "multiplyBy",
  "divideBy",
]);

const VARIABLE_REFERENCE = /<<([^<>]+)>>/g;

/** Add every plain (`<<name>>`) variable reference found in a string value. */
function collectVariableReferences(value: string, out: Set<string>): void {
  VARIABLE_REFERENCE.lastIndex = 0;
  let match: RegExpExecArray | null;
  while ((match = VARIABLE_REFERENCE.exec(value)) !== null) {
    const token = match[1].trim();
    if (isPlainVariableName(token)) out.add(token);
  }
}

/**
 * Recursively scan a process / skip-instruction command tree for variables, mirroring legacy
 * `Process.Variables` → `Project.AllVariables`. Captures assignment targets (Set / Append /
 * arithmetic) plus plain `<<variable>>` references used anywhere — including Get, ForEach,
 * Delete, and If conditions / Where clauses (owner Q2: `ForEach Where [Record:Name] Equals
 * [variable]`). Record and record-set names (which are colon-qualified when referenced) are
 * excluded by `isPlainVariableName`, matching the Variables-node scope.
 */
function collectVariablesFromNode(node: unknown, out: Set<string>): void {
  if (Array.isArray(node)) {
    for (const child of node) collectVariablesFromNode(child, out);
    return;
  }
  if (!node || typeof node !== "object") return;
  const record = node as Record<string, unknown>;
  if (typeof record.cmd === "string" && ASSIGNMENT_COMMANDS.has(record.cmd)) {
    const target = record.field ?? record.variable;
    if (isPlainVariableName(target)) out.add(target.trim());
  }
  for (const value of Object.values(record)) {
    if (typeof value === "string") collectVariableReferences(value, out);
    else if (value && typeof value === "object") collectVariablesFromNode(value, out);
  }
}

/**
 * Project variables for the Fields tree Variables node. The JSON project has no explicit
 * variables list, so (as legacy `Project.AllVariables` effectively captures) we derive them
 * from assignment targets and `<<variable>>` references across all processes and form Skip
 * Instructions. `_InviteeID` is always present and pinned first; the remainder are alphabetical
 * (`localeCompare`), matching legacy `getSortedVariables()` + the always-present invitation token.
 */
export function collectProjectVariables(project: TawalaProject): string[] {
  const vars = new Set<string>();
  for (const process of project.processes ?? []) {
    collectVariablesFromNode(process.commands, vars);
  }
  for (const form of project.forms) {
    for (const item of form.items) {
      if (item.type === "skipInstructions") collectVariablesFromNode(item.commands, vars);
    }
  }
  vars.delete(INVITEE_ID_VARIABLE);
  const rest = [...vars].sort((a, b) => a.localeCompare(b));
  return [INVITEE_ID_VARIABLE, ...rest];
}

/** Merge project variables with any from an in-flight command tree (e.g. unsaved skip dialog). */
export function collectKnownVariables(
  project: TawalaProject,
  extraCommands?: unknown,
): Set<string> {
  const vars = new Set(collectProjectVariables(project));
  if (extraCommands) collectVariablesFromNode(extraCommands, vars);
  return vars;
}

/** Processes linked to a form (Pre = preProcess, Post = process), filtered to ones that exist. */
export function linkedProcessesForForm(
  form: TawalaForm,
  processes: TawalaProcess[] | undefined,
): LinkedProcess[] {
  const known = new Set((processes ?? []).map((p) => p.name));
  const links: LinkedProcess[] = [];
  if (form.preProcess && known.has(form.preProcess)) {
    links.push({ role: "Pre", name: form.preProcess });
  }
  if (form.process && known.has(form.process)) {
    links.push({ role: "Post", name: form.process });
  }
  return links;
}

export interface ProcessFormLink {
  formName: string;
  role: "Pre" | "Post";
}

/** Forms that reference a process as Pre- or Post-process. */
export function formLinksForProcess(
  project: TawalaProject,
  processName: string,
): ProcessFormLink[] {
  const links: ProcessFormLink[] = [];
  for (const form of project.forms) {
    if (form.preProcess === processName) {
      links.push({ formName: form.name, role: "Pre" });
    }
    if (form.process === processName) {
      links.push({ formName: form.name, role: "Post" });
    }
  }
  return links;
}

/** Legacy auto-name when attaching a new linked process (`Pre-Process1`, `Post-Process2`, …). */
export function nextLinkedProcessName(
  role: "Pre" | "Post",
  processes: TawalaProcess[] | undefined,
): string {
  const prefix = role === "Pre" ? "Pre-Process" : "Post-Process";
  const names = new Set((processes ?? []).map((p) => p.name));
  let n = 1;
  while (names.has(`${prefix}${n}`)) n++;
  return `${prefix}${n}`;
}
