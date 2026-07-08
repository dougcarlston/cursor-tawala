import type { TawalaBlank } from "@/types/tawala";

/** Validation types shown in the FIB property strip (legacy FibOptions dropdown). */
export const FIB_VALIDATION_OPTIONS = [
  { value: "", label: "-- No Validation --" },
  { value: "email", label: "Email" },
  { value: "phone", label: "Phone Number" },
  { value: "integer", label: "Integer" },
  { value: "usState", label: "US State" },
  { value: "zip", label: "ZIP Code" },
  { value: "properName", label: "Proper Name" },
  { value: "dollar", label: "Dollar Amount" },
] as const;

export interface UnderscoreRun {
  start: number;
  end: number;
  length: number;
}

/** Plain text of an HTML fragment (underscore positions ignore markup). */
export function htmlToPlainText(html: string): string {
  const tmp = document.createElement("div");
  tmp.innerHTML = html;
  return tmp.textContent ?? "";
}

/** Find every underscore run in plain prompt text (legacy `Regex.Matches(rawText, "_+")`). */
export function parseUnderscoreRuns(plainText: string): UnderscoreRun[] {
  const normalized = plainText.replace(/\r\n/g, "\n");
  const runs: UnderscoreRun[] = [];
  const re = /_+/g;
  let match: RegExpExecArray | null;
  while ((match = re.exec(normalized)) !== null) {
    runs.push({ start: match.index, end: match.index + match[0].length, length: match[0].length });
  }
  return runs;
}

function blankLetter(index: number): string {
  return String.fromCharCode(97 + (index % 26));
}

/**
 * Reconcile `blanks[]` with underscore runs in the prompt. When the count is unchanged,
 * attributes are preserved in order (legacy FibItemView `copyAttributesToBlankList`).
 */
export function syncBlanksFromPrompt(
  plainText: string,
  existing: TawalaBlank[],
  itemLabel: string,
): TawalaBlank[] {
  const runs = parseUnderscoreRuns(plainText);
  if (runs.length === 0) {
    if (existing.length > 0) return existing;
    return [{ name: "a", length: 20, height: 1 }];
  }

  return runs.map((run, i) => {
    const prev = existing[i];
    const letter = prev?.name && /^[a-z]$/.test(prev.name) ? prev.name : blankLetter(i);
    const defaultAlt = `${itemLabel}:${letter}`;
    return {
      name: letter,
      length: run.length,
      required: prev?.required ?? false,
      alternateLabel: prev?.alternateLabel ?? defaultAlt,
      displayLabel: prev?.displayLabel,
      height: prev?.height ?? 1,
      validationType: prev?.validationType,
    };
  });
}

/**
 * Index of the blank under/adjacent to the caret (legacy `cursorIsInOrAdjacentToBlank` /
 * `indexOfMatchingBlank`). Returns -1 when no single blank is active.
 */
export function activeBlankIndex(plainText: string, cursorOffset: number): number {
  const runs = parseUnderscoreRuns(plainText);
  for (let i = 0; i < runs.length; i++) {
    const { start, end } = runs[i];
    const inBlank = cursorOffset < plainText.length && plainText[cursorOffset] === "_";
    const justAfter = cursorOffset > 0 && plainText[cursorOffset - 1] === "_";
    if ((cursorOffset >= start && cursorOffset <= end) || inBlank || justAfter) {
      // Disambiguate between adjacent blanks: prefer the blank whose range contains offset.
      if (cursorOffset >= start && cursorOffset < end) return i;
      if (justAfter && cursorOffset === end) return i;
      if (inBlank && cursorOffset >= start && cursorOffset < end) return i;
    }
  }
  // Fallback: walk runs for inclusive end (cursor at end of run).
  for (let i = 0; i < runs.length; i++) {
    if (cursorOffset >= runs[i].start && cursorOffset <= runs[i].end) return i;
  }
  return -1;
}

/** Selection covers only underscores for a single run (legacy `selectionComprisesOnlyOneBlank`). */
export function selectionIsSingleBlank(
  plainText: string,
  selStart: number,
  selEnd: number,
): number {
  if (selEnd <= selStart) return activeBlankIndex(plainText, selStart);
  const selected = plainText.slice(selStart, selEnd);
  if (!/^[_]+$/.test(selected)) return -1;
  return activeBlankIndex(plainText, selStart);
}

/** Map a DOM selection inside a contenteditable root to a plain-text offset. */
export function selectionPlainOffset(root: HTMLElement): number {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return 0;
  const range = sel.getRangeAt(0);
  if (!root.contains(range.startContainer)) return 0;

  const pre = document.createRange();
  pre.selectNodeContents(root);
  pre.setEnd(range.startContainer, range.startOffset);
  return pre.toString().length;
}
