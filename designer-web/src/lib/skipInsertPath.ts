import type { SkipCommand } from "@/types/tawala";

/** Insertion target: `root` or a branch path like `root/0/then`. */
export type InsertPath = string;

export const ROOT_INSERT_PATH = "root";

/** Array that receives the next appended statement for this insertion path. */
export function getCommandsAtInsertPath(
  root: SkipCommand[],
  insertPath: InsertPath,
): SkipCommand[] {
  if (insertPath === ROOT_INSERT_PATH) return root;
  const parts = insertPath.split("/").filter((p) => p !== "root");
  let current: SkipCommand[] = root;
  let cmd: SkipCommand | null = null;
  for (const p of parts) {
    if (p === "then" || p === "else") {
      if (!cmd || cmd.cmd !== "if") return root;
      if (!cmd[p]) cmd[p] = [];
      current = cmd[p] as SkipCommand[];
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
  if (last === "then" || last === "else") {
    return `root/${parts.slice(0, -1).join("/")}/${last}`;
  }
  if (parts.length >= 2 && (parts[parts.length - 2] === "then" || parts[parts.length - 2] === "else")) {
    return `root/${parts.slice(0, -1).join("/")}`;
  }
  return ROOT_INSERT_PATH;
}
