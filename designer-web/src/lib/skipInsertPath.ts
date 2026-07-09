type CommandTree = { cmd?: string; [key: string]: unknown };

/** Insertion target: `root` or a branch path like `root/0/then`. */
export type InsertPath = string;

export const ROOT_INSERT_PATH = "root";

const BRANCH_KEYS = new Set(["then", "else", "do"]);

/** Array that receives the next appended statement for this insertion path. */
export function getCommandsAtInsertPath(
  root: CommandTree[],
  insertPath: InsertPath,
): CommandTree[] {
  if (insertPath === ROOT_INSERT_PATH) return root;
  const parts = insertPath.split("/").filter((p) => p !== "root");
  let current: CommandTree[] = root;
  let cmd: CommandTree | null = null;
  for (const p of parts) {
    if (BRANCH_KEYS.has(p)) {
      if (!cmd) return root;
      if (p === "do" && cmd.cmd !== "foreach") return root;
      if ((p === "then" || p === "else") && cmd.cmd !== "if") return root;
      if (!cmd[p]) cmd[p] = [];
      current = cmd[p] as CommandTree[];
      cmd = null;
      continue;
    }
    const idx = Number(p);
    if (Number.isNaN(idx)) return root;
    cmd = current[idx] ?? null;
    if (!cmd) return root;
  }
  return current;
}

/** Parent insertion path for a command line (`root/0/then/1` → `root/0/then`). */
export function parentInsertPath(commandPath: string): InsertPath {
  const parts = commandPath.split("/").filter((p) => p !== "root");
  if (parts.length === 0) return ROOT_INSERT_PATH;
  const last = parts[parts.length - 1];
  if (BRANCH_KEYS.has(last)) {
    return `root/${parts.slice(0, -1).join("/")}/${last}`;
  }
  if (parts.length >= 2 && BRANCH_KEYS.has(parts[parts.length - 2])) {
    return `root/${parts.slice(0, -1).join("/")}`;
  }
  return ROOT_INSERT_PATH;
}
