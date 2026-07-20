/**
 * Mid-edit draft for Edit Skip Instructions — survives accidental remounts
 * (form canvas re-key) until Close saves and clears.
 */
import type { SkipCommand } from "@/types/tawala";
import type { IfBuilderState, SetBuilderState } from "@/lib/statementBuilders";

type PanelMode = "none" | "if" | "skipTo" | "set" | "comment";

export interface SkipDialogSession {
  commands: SkipCommand[];
  insertPath: string;
  insertIndex: number;
  selectedCommandPath: string | null;
  panel: PanelMode;
  ifBuilder: IfBuilderState;
  setBuilder: SetBuilderState;
  skipToDest: string;
  commentText: string;
}

const sessions = new Map<string, SkipDialogSession>();

export function skipDialogSessionKey(formName: string, itemIndex: number): string {
  return `${formName}::${itemIndex}`;
}

export function readSkipDialogSession(key: string): SkipDialogSession | undefined {
  return sessions.get(key);
}

export function writeSkipDialogSession(key: string, session: SkipDialogSession): void {
  sessions.set(key, session);
}

export function clearSkipDialogSession(key: string): void {
  sessions.delete(key);
}
