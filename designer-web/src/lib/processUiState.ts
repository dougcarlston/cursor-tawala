import { DEFAULT_PROCESS_INSERT } from "@/lib/processInsert";
import { ROOT_INSERT_PATH } from "@/lib/skipInsertPath";
import type { ProcessStatementPanel } from "@/processStatements";
import type { Selection } from "@/types/tawala";

type WindowKind = "form" | "process" | "document";

export interface ProcessUiSnapshot {
  processInsertPath: string;
  processInsertIndex: number;
  selectedProcessCommandPath: string | null;
  processStatementPanel: ProcessStatementPanel;
}

export const DEFAULT_PROCESS_UI: ProcessUiSnapshot = {
  processInsertPath: ROOT_INSERT_PATH,
  processInsertIndex: DEFAULT_PROCESS_INSERT.index,
  selectedProcessCommandPath: null,
  processStatementPanel: "none",
};

export function snapshotProcessUi(fields: ProcessUiSnapshot): ProcessUiSnapshot {
  return { ...fields };
}

/**
 * When leaving a process window, stash its statement-panel state. When entering a
 * process window, restore the last snapshot for that process name.
 */
export function transitionProcessUi(
  from: Selection,
  toKind: WindowKind,
  toName: string,
  sameEntity: boolean,
  current: ProcessUiSnapshot,
  cache: Record<string, ProcessUiSnapshot>,
): { cache: Record<string, ProcessUiSnapshot>; ui: ProcessUiSnapshot } {
  if (sameEntity && toKind === "process") {
    return { cache, ui: current };
  }

  const nextCache = { ...cache };
  if (from.kind === "process" && from.name) {
    nextCache[from.name] = current;
  }

  const ui =
    toKind === "process" ? (nextCache[toName] ?? DEFAULT_PROCESS_UI) : DEFAULT_PROCESS_UI;

  return { cache: nextCache, ui };
}

export function remapProcessUiCache(
  cache: Record<string, ProcessUiSnapshot>,
  oldName: string,
  newName: string,
): Record<string, ProcessUiSnapshot> {
  if (!cache[oldName]) return cache;
  const next = { ...cache };
  next[newName] = next[oldName];
  delete next[oldName];
  return next;
}
