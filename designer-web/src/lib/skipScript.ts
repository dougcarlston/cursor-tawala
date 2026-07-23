import type { SkipCommand } from "@/types/tawala";
import { conditionOpLabel, isUnaryConditionOp } from "@/lib/mcConditionOperators";
import { skipDestinationLabel } from "@/lib/skipSummary";

export type ScriptLineType =
  | "if-header"
  | "foreach-header"
  | "block-open"
  | "block-close"
  | "otherwise"
  | "set"
  | "skip"
  | "comment"
  | "command";

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
  const opLabel = conditionOpLabel(op);
  if (isUnaryConditionOp(op)) {
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
  const m = /^root\/(.+)\/(then|else|do)$/.exec(insertPath);
  if (!m) return insertPath;
  const branchLabel =
    m[2] === "then" ? "then branch" : m[2] === "else" ? "else branch" : "ForEach body";
  const topIndex = Number(m[1].split("/")[0]);
  if (Number.isNaN(topIndex)) return branchLabel;
  const stmtLabel = m[2] === "do" ? "ForEach" : "If";
  return `${branchLabel} of ${stmtLabel} #${topIndex + 1}`;
}

/** Line index after which the active insertion arrow should appear. */
export function findInsertionLineIndex(
  lines: ScriptLine[],
  insertPath: string,
  insertIndex = 0,
): number {
  if (lines.length === 0) return -1;

  if (insertIndex <= 0) {
    if (insertPath === "root") return -1;
    for (let i = 0; i < lines.length; i++) {
      if (lines[i].lineType === "block-open" && lines[i].insertZone === insertPath) {
        return i;
      }
    }
    return -1;
  }

  const childLineIndices: number[] = [];
  for (let i = 0; i < lines.length; i++) {
    const line = lines[i];
    if (!line.path) continue;
    if (insertPath === "root") {
      if (line.indent === 0 && /^root\/\d+$/.test(line.path)) {
        childLineIndices.push(i);
      }
    } else {
      const prefix = `${insertPath}/`;
      if (line.path.startsWith(prefix)) {
        const rest = line.path.slice(prefix.length);
        if (/^\d+$/.test(rest)) childLineIndices.push(i);
      }
    }
  }

  if (insertIndex >= childLineIndices.length) {
    if (childLineIndices.length === 0) {
      for (let i = 0; i < lines.length; i++) {
        if (lines[i].lineType === "block-open" && lines[i].insertZone === insertPath) {
          return i;
        }
      }
      return -1;
    }
    return childLineIndices[childLineIndices.length - 1];
  }

  return childLineIndices[insertIndex - 1];
}

/** Line span for an If/ForEach header's full block (header through closing `)`). */
export function blockSpanForHeader(
  lines: ScriptLine[],
  headerPath: string,
): { start: number; end: number } | null {
  const start = lines.findIndex(
    (l) =>
      l.path === headerPath &&
      (l.lineType === "if-header" || l.lineType === "foreach-header"),
  );
  if (start < 0) return null;
  const baseIndent = lines[start].indent;
  let end = start;
  for (let i = start + 1; i < lines.length; i++) {
    const line = lines[i];
    if (line.indent < baseIndent) break;
    // Next sibling command at this indent ends the block (Set/Skip/Comment/If/…).
    if (line.indent === baseIndent && line.path != null && line.path !== headerPath) {
      break;
    }
    if (line.indent === baseIndent && line.lineType === "block-close") {
      end = i;
    }
  }
  return { start, end };
}

/** Role of line `index` when a block header is the sole selection (legacy full-block highlight). */
export function selectedBlockHighlightRole(
  lines: ScriptLine[],
  lineIndex: number,
  selectedCommandPath: string | null,
): "header" | "body" | null {
  if (!selectedCommandPath) return null;
  const headerLine = lines.find((l) => l.path === selectedCommandPath);
  if (
    !headerLine ||
    (headerLine.lineType !== "if-header" && headerLine.lineType !== "foreach-header")
  ) {
    return null;
  }
  const span = blockSpanForHeader(lines, selectedCommandPath);
  if (!span || lineIndex < span.start || lineIndex > span.end) return null;
  return lineIndex === span.start ? "header" : "body";
}

/** True when `closeIndex` closes a non-empty block (insertion arrow already sits above `)`). */
export function isNonemptyBlockClose(lines: ScriptLine[], closeIndex: number): boolean {
  const closeLine = lines[closeIndex];
  if (closeLine.lineType !== "block-close" || closeLine.closeZone == null) return false;
  for (let j = closeIndex - 1; j >= 0; j--) {
    const line = lines[j];
    if (line.indent < closeLine.indent) return false;
    if (
      line.indent === closeLine.indent &&
      line.lineType === "block-open" &&
      line.insertZone === closeLine.closeZone
    ) {
      return !isEmptyBlock(lines, j);
    }
  }
  return false;
}

export function isEmptyBlock(lines: ScriptLine[], openIndex: number): boolean {
  const open = lines[openIndex];
  const next = lines[openIndex + 1];
  return (
    open.lineType === "block-open" &&
    next?.lineType === "block-close" &&
    next.indent === open.indent &&
    next.closeZone === open.insertZone
  );
}
