import { formFieldNames } from "@/lib/projectModel";
import { ROOT_INSERT_PATH, type InsertPath } from "@/lib/skipInsertPath";
import type { TawalaProcessCommand, TawalaProject } from "@/types/tawala";

/** Legacy `FieldUtil.DefaultRecordQualifierName` — Show Stored Record Where rows. */
export const DEFAULT_RECORD_QUALIFIER = "Record";

const BRANCH_KEYS = new Set(["then", "else", "do"]);

export interface RecordPaletteLeaf {
  /** Tree label — full qualified reference (`Admin:Form:Field`). */
  name: string;
  /** Bare token inserted into editors (`Admin:Form:Field` or `<<…>>` per target). */
  insertName: string;
}

export interface RecordPaletteBranch {
  /** ForEach record variable name (branch folder label). */
  recordName: string;
  leaves: RecordPaletteLeaf[];
}

function walkCommands(
  nodes: TawalaProcessCommand[] | undefined,
  visit: (cmd: TawalaProcessCommand) => void,
): void {
  if (!nodes) return;
  for (const cmd of nodes) {
    visit(cmd);
    walkCommands(cmd.then as TawalaProcessCommand[] | undefined, visit);
    walkCommands(cmd.else as TawalaProcessCommand[] | undefined, visit);
    walkCommands(cmd.do as TawalaProcessCommand[] | undefined, visit);
  }
}

/** Map Get `recordList` names to their source forms (`Process.RecordSets`). */
export function buildRecordListSourceForms(
  commands: TawalaProcessCommand[],
): Map<string, string[]> {
  const map = new Map<string, string[]>();
  walkCommands(commands, (cmd) => {
    if (cmd.cmd !== "get" || cmd.recordList == null) return;
    const listName = String(cmd.recordList).trim();
    if (!listName) return;
    const forms = (cmd.sourceForms as string[] | undefined) ?? [];
    map.set(listName, forms.filter((f) => f.trim().length > 0));
  });
  return map;
}

export function qualifiedRecordFieldName(
  recordName: string,
  formName: string,
  fieldDragValue: string,
): string {
  return `${recordName}:${formName}:${fieldDragValue}`;
}

function recordLeavesForForms(
  recordName: string,
  formNames: readonly string[],
  project: TawalaProject,
): RecordPaletteLeaf[] {
  const leaves: RecordPaletteLeaf[] = [];
  const seen = new Set<string>();
  for (const formName of formNames) {
    const form = project.forms.find((f) => f.name === formName);
    if (!form) continue;
    for (const field of formFieldNames(form)) {
      const insertName = qualifiedRecordFieldName(recordName, formName, field.dragValue);
      if (seen.has(insertName)) continue;
      seen.add(insertName);
      leaves.push({ name: insertName, insertName });
    }
  }
  leaves.sort((a, b) => a.name.localeCompare(b.name));
  return leaves;
}

interface ForEachFrame {
  recordName: string;
  recordList: string;
}

/**
 * ForEach statements enclosing an insertion path (`root/0/do/2`, nested `do` chains, …).
 * Order matches legacy `Process.GetForEachRecords` — outermost first.
 */
export function foreachChainAtInsertPath(
  commands: TawalaProcessCommand[],
  insertPath: InsertPath,
): ForEachFrame[] {
  if (insertPath === ROOT_INSERT_PATH) return [];

  const parts = insertPath.split("/").filter((p) => p !== ROOT_INSERT_PATH);
  const chain: ForEachFrame[] = [];
  let current = commands;
  let i = 0;

  while (i < parts.length) {
    const part = parts[i];
    if (BRANCH_KEYS.has(part)) {
      i++;
      continue;
    }
    const idx = Number(part);
    if (Number.isNaN(idx) || idx < 0 || idx >= current.length) break;
    const cmd = current[idx];
    const next = parts[i + 1];
    if (next && BRANCH_KEYS.has(next)) {
      if (cmd.cmd === "foreach" && next === "do") {
        chain.push({
          recordName: String(cmd.recordName ?? "").trim(),
          recordList: String(cmd.recordList ?? "").trim(),
        });
      }
      current = (cmd[next] as TawalaProcessCommand[] | undefined) ?? [];
      i += 2;
      continue;
    }
    i++;
  }

  return chain.filter((f) => f.recordName.length > 0);
}

/** Record branches for the Fields panel when the insertion point is inside ForEach block(s). */
export function recordBranchesAtInsertPath(
  commands: TawalaProcessCommand[],
  insertPath: InsertPath,
  project: TawalaProject,
): RecordPaletteBranch[] {
  const listSources = buildRecordListSourceForms(commands);
  const chain = foreachChainAtInsertPath(commands, insertPath);
  const branches: RecordPaletteBranch[] = [];

  for (const frame of chain) {
    const formNames = listSources.get(frame.recordList) ?? [];
    const leaves = recordLeavesForForms(frame.recordName, formNames, project);
    if (leaves.length === 0) continue;
    branches.push({ recordName: frame.recordName, leaves });
  }

  return branches;
}

/** Single `Record:` branch for Show → Stored Record Where (legacy `ConditionsForms`). */
export function recordBranchForConditionsForm(
  formName: string,
  project: TawalaProject,
  recordQualifier = DEFAULT_RECORD_QUALIFIER,
): RecordPaletteBranch | null {
  const trimmed = formName.trim();
  if (!trimmed) return null;
  const leaves = recordLeavesForForms(recordQualifier, [trimmed], project);
  if (leaves.length === 0) return null;
  return { recordName: recordQualifier, leaves };
}
