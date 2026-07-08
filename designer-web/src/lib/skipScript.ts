import type { SkipCommand } from "@/types/tawala";
import { SKIP_OPERATOR_LABELS, UNARY_SKIP_OPERATORS, skipDestinationLabel } from "@/lib/skipSummary";

export type ScriptLineType =
  | "if-header"
  | "block-open"
  | "block-close"
  | "otherwise"
  | "set"
  | "skip"
  | "comment";

export interface ScriptLine {
  /** Command path (`root/0/then/1`) or null for structural lines. */
  path: string | null;
  lineType: ScriptLineType;
  text: string;
  indent: number;
  command?: SkipCommand;
  /** Branch path for `( )` blocks — click sets insertion here (`root/0/then`). */
  insertZone?: string;
  /** Matching branch for a closing `)` — click also sets insertion inside the block. */
  closeZone?: string;
}

interface ConditionShape {
  op?: string;
  field?: string;
  value?: unknown;
  conditions?: ConditionShape[];
  and?: ConditionShape[];
  or?: ConditionShape[];
}

function formatConditionClause(cond: ConditionShape): string {
  const andList = cond.op === "and" ? cond.conditions : cond.and;
  if (andList?.length) {
    return andList.map(formatConditionClause).join(" AND ");
  }
  const orList = cond.op === "or" ? cond.conditions : cond.or;
  if (orList?.length) {
    return orList.map(formatConditionClause).join(" OR ");
  }
  const field = cond.field ?? "?";
  const op = cond.op ?? "equals";
  const opLabel = SKIP_OPERATOR_LABELS[op] ?? op;
  if (UNARY_SKIP_OPERATORS.has(op)) {
    return `${field} ${opLabel}`;
  }
  const value =
    cond.value === undefined || cond.value === null ? "" : String(cond.value);
  return `${field} ${opLabel} ${JSON.stringify(value)}`;
}

export function formatSetLineText(field: string, value: unknown): string {
  return `Set ${field} to ${String(value ?? "")}`;
}

export function formatCommentDisplayText(text: string | undefined): string {
  const raw = String(text ?? "");
  return raw.replace(/^\s*--\s*/, "");
}

function pushCommandLines(
  lines: ScriptLine[],
  commands: SkipCommand[],
  prefix: string,
  indent: number,
): void {
  commands.forEach((cmd, index) => {
    const path = `${prefix}/${index}`;
    const pad = "    ".repeat(indent);
    switch (cmd.cmd) {
      case "comment":
        lines.push({
          path,
          lineType: "comment",
          text: formatCommentDisplayText(cmd.text as string | undefined),
          indent,
          command: cmd,
        });
        break;
      case "set":
        lines.push({
          path,
          lineType: "set",
          text: formatSetLineText(String(cmd.field ?? "?"), cmd.value),
          indent,
          command: cmd,
        });
        break;
      case "skip":
        lines.push({
          path,
          lineType: "skip",
          text: `Skip to ${skipDestinationLabel(String(cmd.to ?? "?"))}`,
          indent,
          command: cmd,
        });
        break;
      case "if": {
        const cond = cmd.condition as ConditionShape | undefined;
        lines.push({
          path,
          lineType: "if-header",
          text: `If ${cond ? formatConditionClause(cond) : "?"}`,
          indent,
          command: cmd,
        });
        lines.push({
          path: null,
          lineType: "block-open",
          text: "(",
          indent,
          insertZone: `${path}/then`,
        });
        const thenZone = `${path}/then`;
        pushCommandLines(lines, (cmd.then as SkipCommand[] | undefined) ?? [], thenZone, indent + 1);
        lines.push({
          path: null,
          lineType: "block-close",
          text: ")",
          indent,
          closeZone: thenZone,
        });
        const elseBranch = (cmd.else as SkipCommand[] | undefined) ?? [];
        if (elseBranch.length || cmd.else !== undefined) {
          const elseZone = `${path}/else`;
          lines.push({
            path: null,
            lineType: "otherwise",
            text: "Otherwise",
            indent,
            insertZone: elseZone,
          });
          lines.push({
            path: null,
            lineType: "block-open",
            text: "(",
            indent,
            insertZone: elseZone,
          });
          pushCommandLines(lines, elseBranch, elseZone, indent + 1);
          lines.push({
            path: null,
            lineType: "block-close",
            text: ")",
            indent,
            closeZone: elseZone,
          });
        }
        break;
      }
      default:
        lines.push({
          path,
          lineType: "set",
          text: `${pad}${cmd.cmd ?? "?"}`,
          indent,
          command: cmd,
        });
    }
  });
}

export function buildScriptLines(commands: SkipCommand[] | undefined): ScriptLine[] {
  if (!commands?.length) return [];
  const lines: ScriptLine[] = [];
  pushCommandLines(lines, commands, "root", 0);
  return lines;
}

export function getCommandAtPath(commands: SkipCommand[], path: string): SkipCommand | null {
  const parts = path.split("/").filter((p) => p !== "root");
  let current: SkipCommand[] = commands;
  let cmd: SkipCommand | null = null;
  for (let i = 0; i < parts.length; i++) {
    const p = parts[i];
    if (p === "then" || p === "else") {
      if (!cmd) return null;
      current = (cmd[p] as SkipCommand[] | undefined) ?? [];
      continue;
    }
    const idx = Number(p);
    if (Number.isNaN(idx)) return null;
    cmd = current[idx] ?? null;
  }
  return cmd;
}

export function replaceCommandAtPath(
  commands: SkipCommand[],
  path: string,
  newCmd: SkipCommand,
): SkipCommand[] {
  const next = structuredClone(commands);
  const parts = path.split("/").filter((p) => p !== "root");
  let current: SkipCommand[] = next;
  let parent: SkipCommand[] = next;
  let lastIdx = -1;
  let cmd: SkipCommand | null = null;

  for (let i = 0; i < parts.length; i++) {
    const p = parts[i];
    if (p === "then" || p === "else") {
      if (!cmd) return next;
      if (!cmd[p]) cmd[p] = [];
      current = cmd[p] as SkipCommand[];
      parent = current;
      lastIdx = -1;
      cmd = null;
      continue;
    }
    const idx = Number(p);
    lastIdx = idx;
    cmd = current[idx] ?? null;
    if (i < parts.length - 1) {
      const nextPart = parts[i + 1];
      if (nextPart !== "then" && nextPart !== "else") {
        parent = current;
      }
    }
  }

  if (lastIdx >= 0) {
    parent[lastIdx] = structuredClone(newCmd);
  }
  return next;
}

/** Human-readable label for an insertion zone path (`root/0/else`). */
export function formatInsertPathLabel(insertPath: string): string {
  if (insertPath === "root") return "top level";
  const m = /^root\/(.+)\/(then|else)$/.exec(insertPath);
  if (!m) return insertPath;
  const branchLabel = m[2] === "then" ? "then branch" : "else branch";
  const topIfIndex = Number(m[1].split("/")[0]);
  if (Number.isNaN(topIfIndex)) return branchLabel;
  return `${branchLabel} of If #${topIfIndex + 1}`;
}

/** Line index after which the insertion arrow should appear. */
export function findInsertionLineIndex(lines: ScriptLine[], insertPath: string): number {
  if (lines.length === 0) return -1;

  if (insertPath === "root") {
    for (let i = lines.length - 1; i >= 0; i--) {
      if (lines[i].indent === 0) return i;
    }
    return -1;
  }

  let openIdx = -1;
  let openIndent = -1;
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    if (line.lineType === "block-open" && line.insertZone === insertPath) {
      openIdx = i;
      openIndent = line.indent;
      break;
    }
  }
  if (openIdx < 0) return -1;

  for (let i = openIdx + 1; i < lines.length; i++) {
    if (lines[i].lineType === "block-close" && lines[i].indent === openIndent) {
      return i - 1;
    }
  }
  return openIdx;
}
