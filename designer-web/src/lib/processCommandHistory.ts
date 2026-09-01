/**
 * Step-by-step Undo/Redo for Process script edits (parity with Skip Instructions).
 * Active while a Process MDI window is mounted; records snapshots on each command-tree change.
 */

import type { TawalaProcessCommand } from "@/types/tawala";

type Session = {
  processName: string;
  history: TawalaProcessCommand[][];
  historyIndex: number;
};

let session: Session | null = null;
let applyingHistory = false;
const listeners = new Set<() => void>();

function notify(): void {
  for (const l of listeners) l();
}

export function subscribeProcessCommandHistory(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

/** Snapshot token for useSyncExternalStore (menu Undo/Redo enable). */
export function getProcessCommandHistorySnapshot(): string {
  if (!session) return "";
  return `${session.processName}:${session.historyIndex}:${session.history.length}`;
}

function cloneCommands(commands: TawalaProcessCommand[]): TawalaProcessCommand[] {
  return structuredClone(commands);
}

function commandsDeepEqual(
  a: TawalaProcessCommand[] | undefined,
  b: TawalaProcessCommand[] | undefined,
): boolean {
  return JSON.stringify(a ?? []) === JSON.stringify(b ?? []);
}

/** Begin tracking undo for this process (call on ProcessEditor mount / process switch). */
export function beginProcessCommandHistory(
  processName: string,
  initialCommands: TawalaProcessCommand[],
): void {
  session = {
    processName,
    history: [cloneCommands(initialCommands)],
    historyIndex: 0,
  };
  notify();
}

/** Stop tracking (ProcessEditor unmount). */
export function endProcessCommandHistory(processName?: string): void {
  if (!session) return;
  if (processName != null && session.processName !== processName) return;
  session = null;
  notify();
}

export function isProcessCommandHistoryActive(processName: string): boolean {
  return session?.processName === processName;
}

export function canProcessCommandUndo(): boolean {
  return session != null && session.historyIndex > 0;
}

export function canProcessCommandRedo(): boolean {
  return session != null && session.historyIndex < session.history.length - 1;
}

/** Call from updateProcessCommands when the tree changes (not during undo/redo). */
export function recordProcessCommandSnapshot(
  processName: string,
  commands: TawalaProcessCommand[],
): void {
  recordProcessCommandChange(processName, session?.history[session.historyIndex] ?? [], commands);
}

/** Record a transition from store's current commands to the next tree. */
export function recordProcessCommandChange(
  processName: string,
  before: TawalaProcessCommand[],
  after: TawalaProcessCommand[],
): void {
  if (applyingHistory || !session || session.processName !== processName) return;
  if (commandsDeepEqual(before, after)) return;

  const tip = session.history[session.historyIndex] ?? [];
  let truncated = session.history.slice(0, session.historyIndex + 1);

  // If the store drifted ahead of the history tip, anchor the real pre-change state first.
  if (!commandsDeepEqual(tip, before)) {
    const last = truncated[truncated.length - 1];
    if (!commandsDeepEqual(last, before)) {
      truncated.push(cloneCommands(before));
    }
  }

  const last = truncated[truncated.length - 1];
  if (!commandsDeepEqual(last, after)) {
    truncated.push(cloneCommands(after));
  }

  session = {
    ...session,
    history: truncated,
    historyIndex: truncated.length - 1,
  };
  notify();
}

export function undoProcessCommands(): TawalaProcessCommand[] | null {
  if (!canProcessCommandUndo() || !session) return null;
  applyingHistory = true;
  try {
    const nextIdx = session.historyIndex - 1;
    session = { ...session, historyIndex: nextIdx };
    return cloneCommands(session.history[nextIdx] ?? []);
  } finally {
    applyingHistory = false;
    notify();
  }
}

export function redoProcessCommands(): TawalaProcessCommand[] | null {
  if (!canProcessCommandRedo() || !session) return null;
  applyingHistory = true;
  try {
    const nextIdx = session.historyIndex + 1;
    session = { ...session, historyIndex: nextIdx };
    return cloneCommands(session.history[nextIdx] ?? []);
  } finally {
    applyingHistory = false;
    notify();
  }
}

/** Test-only reset */
export function resetProcessCommandHistoryForTests(): void {
  session = null;
  applyingHistory = false;
}
