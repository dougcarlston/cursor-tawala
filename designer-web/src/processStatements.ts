import { TawalaProcessCommand } from "@/types/tawala";

/**
 * Legacy Designer "Statements palette" (process context). Order and grouping match
 * `Tawala_Key_Documents/DESIGNER_PROCESS_STATEMENTS_IF.md` → "Statements palette
 * (process selected)", which is the same order as Insert → Process:
 *
 *   1. If
 *   2. Show, Send
 *   3. Append
 *   4. Get, ForEach, Delete
 *   5. Set
 *   6. Comment
 *
 * `group` drives the small visual gaps between groups. Each `template` is the JSON
 * command inserted into the active process; shapes follow the existing
 * `TawalaProcessCommand` conventions used by `ProcessEditor` and the runtime.
 */
export interface ProcessStatementDef {
  label: string;
  group: number;
  template: TawalaProcessCommand;
}

export const PROCESS_STATEMENT_PALETTE: ProcessStatementDef[] = [
  {
    label: "If",
    group: 1,
    template: {
      cmd: "if",
      condition: { field: "Form:Field", op: "equals", value: "Yes" },
      then: [],
    },
  },
  { label: "Show", group: 2, template: { cmd: "show", form: "Report" } },
  {
    label: "Send",
    group: 2,
    template: { cmd: "send", to: "Start:UserEmail", subject: "", document: "Document 1" },
  },
  {
    label: "Append",
    group: 3,
    template: { cmd: "append", document: "Header", appendage: "Document 2" },
  },
  {
    label: "Get",
    group: 4,
    template: { cmd: "get", recordList: "Record List 1", sourceForms: ["Start"] },
  },
  {
    label: "ForEach",
    group: 4,
    template: { cmd: "foreach", recordName: "Rec", recordList: "Record List 1", do: [] },
  },
  { label: "Delete", group: 4, template: { cmd: "delete", form: "Form 1", where: [] } },
  { label: "Set", group: 5, template: { cmd: "set", field: "Completed", value: "Yes" } },
  { label: "Comment", group: 6, template: { cmd: "comment", text: "-- note" } },
];

/** Statement types with a property panel in the process window (legacy: palette selects panel). */
export type ProcessStatementPanel = "none" | "if" | "set" | "show" | "send" | "append" | "get";

export const PROCESS_PANEL_LABELS = new Set(["If", "Set", "Show", "Send", "Append", "Get"]);

export function processPanelKeyForLabel(label: string): ProcessStatementPanel | null {
  const key = label.toLowerCase();
  if (
    key === "if" ||
    key === "set" ||
    key === "show" ||
    key === "send" ||
    key === "append" ||
    key === "get"
  ) {
    return key;
  }
  return null;
}

/** Map a process command to its statement property panel, if any. */
export function processPanelKeyForCommand(
  cmd: TawalaProcessCommand,
): ProcessStatementPanel | null {
  if (cmd.cmd === "if") return "if";
  if (cmd.cmd === "set") return "set";
  if (cmd.cmd === "show" || cmd.cmd === "showDocument" || cmd.cmd === "edit") return "show";
  if (cmd.cmd === "send") return "send";
  if (cmd.cmd === "append") return "append";
  if (cmd.cmd === "get") return "get";
  return null;
}
