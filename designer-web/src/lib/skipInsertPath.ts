type CommandTree = { cmd?: string; [key: string]: unknown };

/** Insertion target: `root` or a branch path like `root/0/then`. */
export type InsertPath = string;

export const ROOT_INSERT_PATH = "root";

const BRANCH_KEYS = new Set(["then", "else", "do"]);

/**
 * Resolve the command array for an insert path.
 * Returns `null` when the path does not resolve (stale index, wrong branch key, etc.).
 * Prefer this for drag-reorder so a bad target cannot silently fall back to root and “lose” the statement.
 */
export function resolveCommandsAtInsertPath(
  root: CommandTree[],
  insertPath: InsertPath,
): CommandTree[] | null {
  if (insertPath === ROOT_INSERT_PATH) return root;
  const parts = insertPath.split("/").filter((p) => p !== "root");
  let current: CommandTree[] = root;
  let cmd: CommandTree | null = null;
  for (const p of parts) {
    if (BRANCH_KEYS.has(p)) {
      if (!cmd) return null;
      if (p === "do" && cmd.cmd !== "foreach") return null;
      if ((p === "then" || p === "else") && cmd.cmd !== "if") return null;
      if (!cmd[p]) cmd[p] = [];
      current = cmd[p] as CommandTree[];
      cmd = null;
      continue;
    }
    const idx = Number(p);
    if (Number.isNaN(idx)) return null;
    cmd = current[idx] ?? null;
    if (!cmd) return null;
  }
  return current;
}

/** Array that receives the next appended statement for this insertion path. */
export function getCommandsAtInsertPath(
  root: CommandTree[],
  insertPath: InsertPath,
): CommandTree[] {
  return resolveCommandsAtInsertPath(root, insertPath) ?? root;
}

/**
 * After removing `removedCommandPath`, rewrite `path` so sibling indices that followed
 * the removed command still point at the same nodes.
 * Example: remove `root/0`, dest `root/1/do` → `root/0/do`.
 * Returns `null` if `path` is the removed command or nested under it.
 */
export function adjustPathAfterCommandRemoval(
  removedCommandPath: string,
  path: string,
): string | null {
  if (path === removedCommandPath || path.startsWith(`${removedCommandPath}/`)) {
    return null;
  }
  const rem = removedCommandPath.split("/");
  const out = path.split("/");
  const remParent = rem.slice(0, -1);
  const remIdx = Number(rem[rem.length - 1]);
  if (Number.isNaN(remIdx)) return path;
  if (out.length <= remParent.length) return path;
  const sameParent = remParent.every((seg, j) => out[j] === seg);
  if (!sameParent) return path;
  const outIdx = Number(out[remParent.length]);
  if (!Number.isNaN(outIdx) && outIdx > remIdx) {
    out[remParent.length] = String(outIdx - 1);
  }
  return out.join("/");
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

/** Parent path and child index for a direct command path (`root/0/then/1` → then, 1). */
export function parentPathAndChildIndex(commandPath: string): {
  parentPath: InsertPath;
  childIndex: number;
} {
  const parts = commandPath.split("/").filter((p) => p !== "root");
  const childIndex = Number(parts[parts.length - 1]);
  parts.pop();
  const parentPath = parts.length ? (`root/${parts.join("/")}` as InsertPath) : ROOT_INSERT_PATH;
  return { parentPath, childIndex };
}

/** True when `commandPath` is a direct child of `insertPath` (not nested deeper). */
export function isDirectChildCommandPath(commandPath: string, insertPath: InsertPath): boolean {
  if (insertPath === ROOT_INSERT_PATH) {
    return /^root\/\d+$/.test(commandPath);
  }
  const prefix = `${insertPath}/`;
  if (!commandPath.startsWith(prefix)) return false;
  const rest = commandPath.slice(prefix.length);
  return /^\d+$/.test(rest);
}
