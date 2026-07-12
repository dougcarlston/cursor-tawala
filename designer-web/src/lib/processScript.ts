import type { TawalaProcessCommand } from "@/types/tawala";
import {
  formatCommentDisplayText,
  formatSetLineText,
  type ScriptLine,
} from "@/lib/skipScript";
import { SKIP_OPERATOR_LABELS, UNARY_SKIP_OPERATORS } from "@/lib/skipSummary";
import { getCommandsAtInsertPath, parentInsertPath } from "@/lib/skipInsertPath";

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

function formatSendRecipient(to: unknown): string {
  if (to == null) return "?";
  if (typeof to === "string") return to;
  if (typeof to === "object" && !Array.isArray(to)) {
    const addr = to as { fieldRef?: string; literal?: string };
    if (addr.fieldRef) return addr.fieldRef;
    if (addr.literal != null) return String(addr.literal);
  }
  return "?";
}

function formatProcessCommandText(cmd: TawalaProcessCommand): string {
  switch (cmd.cmd) {
    case "comment":
      return formatCommentDisplayText(cmd.text as string | undefined);
    case "set":
      return formatSetLineText(String(cmd.field ?? "?"), cmd.value);
    case "show":
    case "showDocument":
      if (cmd.url != null && String(cmd.url).length > 0) return `Show URL ${cmd.url}`;
      if (cmd.form) return `Show Form ${cmd.form}`;
      if (cmd.document) {
        const reset = cmd.reset === true ? " and reset" : "";
        return `Show${reset} Document ${cmd.document}`;
      }
      return "Show";
    case "edit": {
      const form = cmd.form ?? "?";
      const cond = cmd.condition as ConditionShape | undefined;
      const where = cond ? ` where ${formatConditionClause(cond)}` : "";
      return `Show stored record from ${form}${where}`;
    }
    case "send": {
      const body = cmd.body as { document?: string; reset?: boolean } | undefined;
      const doc = body?.document ?? String(cmd.document ?? "?");
      const to = formatSendRecipient(cmd.to);
      const reset =
        body?.reset === true || cmd.reset === true ? " and reset Document" : "";
      return `Send Document ${doc} to ${to}${reset}`;
    }
    case "append":
      return `Append ${cmd.appendage ?? "?"} to ${cmd.document ?? "?"}`;
    case "get": {
      const forms = (cmd.sourceForms as string[] | undefined) ?? [];
      const cond = cmd.where as ConditionShape | undefined;
      const where = cond ? ` where ${formatConditionClause(cond)}` : "";
      return `Get ${cmd.recordList ?? "?"} from ${forms.length ? forms.join(", ") : "?"}${where}`;
    }
    case "foreach":
      return `ForEach ${cmd.recordName ?? "?"} in ${cmd.recordList ?? "?"}`;
    case "delete": {
      const form = cmd.form ?? "?";
      const cond = cmd.where as ConditionShape | undefined;
      const where = cond ? ` where ${formatConditionClause(cond)}` : "";
      return `Delete records from ${form}${where}`;
    }
    default:
      return String(cmd.cmd ?? "?");
  }
}

function pushProcessLines(
  lines: ScriptLine[],
  commands: TawalaProcessCommand[],
  prefix: string,
  indent: number,
): void {
  commands.forEach((cmd, index) => {
    const path = `${prefix}/${index}`;
    switch (cmd.cmd) {
      case "comment":
        lines.push({
          path,
          lineType: "comment",
          text: formatProcessCommandText(cmd),
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
        pushProcessLines(lines, (cmd.then as TawalaProcessCommand[] | undefined) ?? [], thenZone, indent + 1);
        lines.push({
          path: null,
          lineType: "block-close",
          text: ")",
          indent,
          closeZone: thenZone,
        });
        const elseBranch = (cmd.else as TawalaProcessCommand[] | undefined) ?? [];
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
          pushProcessLines(lines, elseBranch, elseZone, indent + 1);
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
      case "foreach": {
        lines.push({
          path,
          lineType: "foreach-header",
          text: formatProcessCommandText(cmd),
          indent,
          command: cmd,
        });
        lines.push({
          path: null,
          lineType: "block-open",
          text: "(",
          indent,
          insertZone: `${path}/do`,
        });
        const doZone = `${path}/do`;
        pushProcessLines(lines, (cmd.do as TawalaProcessCommand[] | undefined) ?? [], doZone, indent + 1);
        lines.push({
          path: null,
          lineType: "block-close",
          text: ")",
          indent,
          closeZone: doZone,
        });
        break;
      }
      default:
        lines.push({
          path,
          lineType: "command",
          text: formatProcessCommandText(cmd),
          indent,
          command: cmd,
        });
    }
  });
}

export function buildProcessScriptLines(
  commands: TawalaProcessCommand[] | undefined,
): ScriptLine[] {
  if (!commands?.length) return [];
  const lines: ScriptLine[] = [];
  pushProcessLines(lines, commands, "root", 0);
  return lines;
}

export function getProcessCommandAtPath(
  commands: TawalaProcessCommand[],
  path: string,
): TawalaProcessCommand | null {
  const parts = path.split("/").filter((p) => p !== "root");
  let current: TawalaProcessCommand[] = commands;
  let cmd: TawalaProcessCommand | null = null;
  for (let i = 0; i < parts.length; i++) {
    const p = parts[i];
    if (p === "then" || p === "else" || p === "do") {
      if (!cmd) return null;
      current = (cmd[p] as TawalaProcessCommand[] | undefined) ?? [];
      continue;
    }
    const idx = Number(p);
    if (Number.isNaN(idx)) return null;
    cmd = current[idx] ?? null;
  }
  return cmd;
}

export function replaceProcessCommandAtPath(
  commands: TawalaProcessCommand[],
  path: string,
  newCmd: TawalaProcessCommand,
): TawalaProcessCommand[] {
  const next = structuredClone(commands);
  const parts = path.split("/").filter((p) => p !== "root");
  let current: TawalaProcessCommand[] = next;
  let parent: TawalaProcessCommand[] = next;
  let lastIdx = -1;
  let cmd: TawalaProcessCommand | null = null;

  for (let i = 0; i < parts.length; i++) {
    const p = parts[i];
    if (p === "then" || p === "else" || p === "do") {
      if (!cmd) return next;
      if (!cmd[p]) cmd[p] = [];
      current = cmd[p] as TawalaProcessCommand[];
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
      if (nextPart !== "then" && nextPart !== "else" && nextPart !== "do") {
        parent = current;
      }
    }
  }

  if (lastIdx >= 0) {
    parent[lastIdx] = structuredClone(newCmd);
  }
  return next;
}

interface CommandLocation {
  parent: TawalaProcessCommand[];
  index: number;
}

function locateProcessCommand(
  commands: TawalaProcessCommand[],
  path: string,
): CommandLocation | null {
  const parts = path.split("/").filter((p) => p !== "root");
  if (parts.length === 0) return null;
  let current = commands;
  let cmd: TawalaProcessCommand | null = null;
  for (let i = 0; i < parts.length; i++) {
    const p = parts[i];
    if (p === "then" || p === "else" || p === "do") {
      if (!cmd) return null;
      current = (cmd[p] as TawalaProcessCommand[] | undefined) ?? [];
      cmd = null;
      continue;
    }
    const idx = Number(p);
    if (Number.isNaN(idx)) return null;
    if (i === parts.length - 1) {
      return { parent: current, index: idx };
    }
    cmd = current[idx] ?? null;
    if (!cmd) return null;
  }
  return null;
}

/** Whether a command can move within its sibling block. */
export function canMoveProcessCommandAtPath(
  commands: TawalaProcessCommand[],
  path: string,
  direction: "up" | "down",
): boolean {
  const loc = locateProcessCommand(commands, path);
  if (!loc) return false;
  const target = direction === "up" ? loc.index - 1 : loc.index + 1;
  return target >= 0 && target < loc.parent.length;
}

/** Swap a command with its sibling within the same block. Returns null when move is invalid. */
export function moveProcessCommandAtPath(
  commands: TawalaProcessCommand[],
  path: string,
  direction: "up" | "down",
): { commands: TawalaProcessCommand[]; newPath: string } | null {
  const loc = locateProcessCommand(commands, path);
  if (!loc) return null;
  const { parent, index } = loc;
  const target = direction === "up" ? index - 1 : index + 1;
  if (target < 0 || target >= parent.length) return null;
  const next = structuredClone(commands);
  const nextLoc = locateProcessCommand(next, path);
  if (!nextLoc) return null;
  const arr = nextLoc.parent;
  const [moved] = arr.splice(nextLoc.index, 1);
  arr.splice(target, 0, moved);
  const parts = path.split("/");
  parts[parts.length - 1] = String(target);
  return { commands: next, newPath: parts.join("/") };
}

/**
 * Move a statement to `destParentPath` at `destIndex` (0 = first in that branch).
 * Forbids dropping a block into its own then/else/do subtree.
 */
export function moveProcessCommandBefore(
  commands: TawalaProcessCommand[],
  fromPath: string,
  destParentPath: string,
  destIndex: number,
): { commands: TawalaProcessCommand[]; newPath: string } | null {
  if (
    destParentPath === fromPath ||
    destParentPath.startsWith(`${fromPath}/`)
  ) {
    return null;
  }
  const next = structuredClone(commands);
  const fromLoc = locateProcessCommand(next, fromPath);
  if (!fromLoc) return null;
  const fromParent = fromLoc.parent;
  const fromIndex = fromLoc.index;
  const [moved] = fromParent.splice(fromIndex, 1);

  const destArr = getCommandsAtInsertPath(next, destParentPath) as TawalaProcessCommand[];

  let idx = destIndex;
  // After removal, same-parent targets after fromIndex shift left.
  if (fromParent === destArr && fromIndex < idx) idx -= 1;
  idx = Math.max(0, Math.min(idx, destArr.length));
  destArr.splice(idx, 0, moved);

  const newPath =
    destParentPath === "root" ? `root/${idx}` : `${destParentPath}/${idx}`;
  return { commands: next, newPath };
}

export function deleteProcessCommandAtPath(
  commands: TawalaProcessCommand[],
  path: string,
): TawalaProcessCommand[] {
  const loc = locateProcessCommand(commands, path);
  if (!loc) return commands;
  const next = structuredClone(commands);
  const nextLoc = locateProcessCommand(next, path);
  if (!nextLoc) return commands;
  nextLoc.parent.splice(nextLoc.index, 1);
  return next;
}

/** Path for the sibling after a move (used to keep selection aligned). */
export function siblingProcessCommandPath(
  path: string,
  direction: "up" | "down",
): string | null {
  const parts = path.split("/");
  const last = parts[parts.length - 1];
  const idx = Number(last);
  if (Number.isNaN(idx)) return null;
  const newIdx = direction === "up" ? idx - 1 : idx + 1;
  if (newIdx < 0) return null;
  parts[parts.length - 1] = String(newIdx);
  return parts.join("/");
}

/** After inserting a block statement, move the insertion point inside its body. */
export function insertionPathAfterBlockInsert(
  insertPath: string,
  insertedIndex: number,
  cmd: TawalaProcessCommand,
): string {
  const base = insertPath === "root" ? `root/${insertedIndex}` : `${insertPath}/${insertedIndex}`;
  if (cmd.cmd === "if") return `${base}/then`;
  if (cmd.cmd === "foreach") return `${base}/do`;
  return insertPath;
}

export { parentInsertPath };
