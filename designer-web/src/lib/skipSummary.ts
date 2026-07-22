import type { SkipCommand } from "@/types/tawala";
import { conditionOpLabel, isUnaryConditionOp } from "@/lib/mcConditionOperators";

export const EMPTY_SKIP_SUMMARY =
  "(No instructions. Click edit link on left to add instructions.)";

const END_OF_FORM = "__EndOfForm__";

/** Human-readable operator labels (legacy ComparisonOperator) — Hybrid / FIB list for builders. */
export const SKIP_OPERATOR_LABELS: Record<string, string> = {
  equals: "equals",
  doesNotEqual: "does not equal",
  contains: "contains",
  doesNotContain: "does not contain",
  beginsWith: "begins with",
  endsWith: "ends with",
  isLessThan: "is less than",
  isLessThanOrEqualTo: "is less than or equal to",
  isGreaterThan: "is greater than",
  isGreaterThanOrEqualTo: "is greater than or equal to",
  isBlank: "is blank",
  isNotBlank: "is not blank",
};

export const SKIP_OPERATORS = Object.keys(SKIP_OPERATOR_LABELS);

export const UNARY_SKIP_OPERATORS = new Set(["isBlank", "isNotBlank"]);

/** Label for script / summary lines — Hybrid + mc* (imported MCQ conditions). */
export function skipOperatorDisplayLabel(op: string): string {
  return conditionOpLabel(op);
}

export interface SkipTargetInfo {
  targets: string[];
  hasConditional: boolean;
}

export function skipDestinationLabel(to: string): string {
  return to === END_OF_FORM ? "End of Form" : to;
}

/** Walk skip commands and collect unique skip destinations. */
export function analyzeSkipTargets(
  commands: SkipCommand[] | undefined,
  insideConditional = false,
): SkipTargetInfo {
  const targets: string[] = [];
  let hasConditional = false;

  const walk = (list: SkipCommand[] | undefined, conditional: boolean) => {
    for (const cmd of list ?? []) {
      if (cmd.cmd === "skip" && typeof cmd.to === "string") {
        if (!targets.includes(cmd.to)) targets.push(cmd.to);
        if (conditional) hasConditional = true;
      } else if (cmd.cmd === "if") {
        walk(cmd.then as SkipCommand[] | undefined, true);
        walk(cmd.else as SkipCommand[] | undefined, true);
      }
    }
  };

  walk(commands, insideConditional);
  return { targets, hasConditional };
}

/** Canvas summary next to the Edit link (legacy SkipItemView). */
export function skipCanvasSummary(commands: SkipCommand[] | undefined): string {
  if (!commands?.length) return EMPTY_SKIP_SUMMARY;

  const { targets, hasConditional } = analyzeSkipTargets(commands);
  if (!targets.length) return EMPTY_SKIP_SUMMARY;

  const labels = targets.map(skipDestinationLabel);

  if (!hasConditional && targets.length === 1 && targets[0] === END_OF_FORM) {
    return "(Skips to End of Form)";
  }

  return `(May skip to: ${labels.join(", ")})`;
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

function formatCommand(cmd: SkipCommand, indent: string): string[] {
  const lines: string[] = [];
  switch (cmd.cmd) {
    case "comment":
      lines.push(`${indent}-- ${cmd.text ?? ""}`);
      break;
    case "set":
      lines.push(
        `${indent}Set ${cmd.field ?? "?"} = ${JSON.stringify(cmd.value ?? "")}`,
      );
      break;
    case "skip":
      lines.push(`${indent}Skip to ${skipDestinationLabel(String(cmd.to ?? "?"))}`);
      break;
    case "if": {
      const cond = cmd.condition as ConditionShape | undefined;
      lines.push(`${indent}If ${cond ? formatConditionClause(cond) : "?"}`);
      lines.push(`${indent}(`);
      for (const child of (cmd.then as SkipCommand[] | undefined) ?? []) {
        lines.push(...formatCommand(child, indent + "    "));
      }
      lines.push(`${indent})`);
      const elseBranch = (cmd.else as SkipCommand[] | undefined) ?? [];
      if (elseBranch.length || cmd.else !== undefined) {
        lines.push(`${indent}Otherwise`);
        lines.push(`${indent}(`);
        for (const child of elseBranch) {
          lines.push(...formatCommand(child, indent + "    "));
        }
        lines.push(`${indent})`);
      }
      break;
    }
    default:
      lines.push(`${indent}${cmd.cmd ?? "?"}`);
  }
  return lines;
}

/** Text representation shown in the Edit Skip Instructions script area. */
export function formatSkipCommandsText(commands: SkipCommand[] | undefined): string {
  if (!commands?.length) return "";
  return commands.flatMap((cmd) => formatCommand(cmd, "")).join("\n");
}

export function buildConditionFromRows(
  combinator: "and" | "or",
  rows: { field: string; op: string; value: string }[],
): ConditionShape {
  const clauses = rows
    .filter((r) => r.field.trim())
    .map((r) => {
      const base: ConditionShape = { field: r.field.trim(), op: r.op };
      if (!UNARY_SKIP_OPERATORS.has(r.op)) {
        base.value = r.value;
      }
      return base;
    });
  if (clauses.length === 0) {
    return { field: "", op: "equals", value: "" };
  }
  if (clauses.length === 1) return clauses[0];
  return { op: combinator, conditions: clauses };
}
