import type { TawalaProcessCommand, SkipCommand } from "@/types/tawala";

let clipboardCommand: TawalaProcessCommand | SkipCommand | null = null;

export function getProcessClipboard(): TawalaProcessCommand | SkipCommand | null {
  return clipboardCommand ? JSON.parse(JSON.stringify(clipboardCommand)) : null;
}

export function setProcessClipboard(cmd: TawalaProcessCommand | SkipCommand | null): void {
  clipboardCommand = cmd ? JSON.parse(JSON.stringify(cmd)) : null;
}

export function hasProcessClipboard(): boolean {
  return clipboardCommand !== null;
}
