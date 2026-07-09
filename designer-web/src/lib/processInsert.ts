import { insertionPathAfterBlockInsert } from "@/lib/processScript";
import { getCommandsAtInsertPath, ROOT_INSERT_PATH } from "@/lib/skipInsertPath";
import type { TawalaProcessCommand } from "@/types/tawala";

/** Insert a command at a specific index within the target branch (not always append). */
export function insertCommandAtPoint(
  commands: TawalaProcessCommand[],
  insertPath: string,
  insertIndex: number,
  command: TawalaProcessCommand,
): {
  commands: TawalaProcessCommand[];
  insertPath: string;
  insertIndex: number;
} {
  const next = structuredClone(commands);
  const target = getCommandsAtInsertPath(next, insertPath);
  const idx = Math.max(0, Math.min(insertIndex, target.length));
  target.splice(idx, 0, structuredClone(command));
  const nextPath = insertionPathAfterBlockInsert(insertPath, idx, command);
  const nextIndex = nextPath !== insertPath ? 0 : idx + 1;
  return { commands: next, insertPath: nextPath, insertIndex: nextIndex };
}

export const DEFAULT_PROCESS_INSERT = {
  path: ROOT_INSERT_PATH,
  index: 0,
} as const;
