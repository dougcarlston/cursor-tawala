/**
 * In-memory text clipboard for rich text / plain text operations.
 * Allows Copy/Cut/Paste within Designer even when browser security
 * sandboxes or permission restrictions restrict `navigator.clipboard.readText()`.
 */

export interface TextClipboardEntry {
  text: string;
  html?: string;
}

let clipboardEntry: TextClipboardEntry | null = null;
/** Set when Copy/Cut ran via Edit menu or toolbar — Paste should prefer in-app buffer first. */
let shellTextClipboardPending = false;

export function getTextClipboard(): TextClipboardEntry | null {
  return clipboardEntry ? { ...clipboardEntry } : null;
}

export function setTextClipboard(entry: TextClipboardEntry | null): void {
  clipboardEntry = entry ? { ...entry } : null;
  shellTextClipboardPending = entry != null;
}

export function consumeShellTextClipboardPending(): boolean {
  if (!shellTextClipboardPending) return false;
  shellTextClipboardPending = false;
  return true;
}

export function hasTextClipboard(): boolean {
  return Boolean(clipboardEntry && (clipboardEntry.text || clipboardEntry.html));
}

/** True when the last Copy/Cut came from the shell (menu/toolbar), not an external app. */
export function shellTextClipboardPendingForPaste(): boolean {
  return shellTextClipboardPending && hasTextClipboard();
}
