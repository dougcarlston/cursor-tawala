import type { BlankValidation, TawalaBlank } from "@/types/tawala";

/**
 * FIB blank validators (legacy `blank-validator` repository; defaults from the Java
 * `component.properties`). `id` is our stored key; `xmlId` is the runtime element name;
 * `hasLimits` marks the Integer validator's optional Lower/Upper limits. Proper Name has no
 * parameters, so its `defaultMessage` is empty and the Edit dialog is not offered for it.
 */
export interface FibValidatorMeta {
  id: string;
  xmlId: string;
  label: string;
  defaultMessage: string;
  hasLimits: boolean;
  /** Whether the validator exposes editable parameters (drives the Edit… button + auto-open). */
  hasParams: boolean;
}

export const FIB_VALIDATORS: FibValidatorMeta[] = [
  {
    id: "email",
    xmlId: "email-validator",
    label: "Email",
    defaultMessage: "Please enter a valid email address.",
    hasLimits: false,
    hasParams: true,
  },
  {
    id: "phone",
    xmlId: "phone-number-validator",
    label: "Phone Number",
    defaultMessage: "Please enter a valid phone number including area code.",
    hasLimits: false,
    hasParams: true,
  },
  {
    id: "integer",
    xmlId: "integer-range-validator",
    label: "Integer",
    defaultMessage: "This number is not valid.",
    hasLimits: true,
    hasParams: true,
  },
  {
    id: "usState",
    xmlId: "us-state-validator",
    label: "US State",
    defaultMessage: "Please enter a valid two-character state abbreviation.",
    hasLimits: false,
    hasParams: true,
  },
  {
    id: "zip",
    xmlId: "zip-code-validator",
    label: "ZIP Code",
    defaultMessage: "Please enter a valid ZIP code.",
    hasLimits: false,
    hasParams: true,
  },
  {
    id: "properName",
    xmlId: "proper-validator",
    label: "Proper Name",
    defaultMessage: "",
    hasLimits: false,
    hasParams: false,
  },
  {
    id: "dollar",
    xmlId: "us-dollar-amount-validator",
    label: "Dollar Amount",
    defaultMessage: "Please enter a valid dollar amount.",
    hasLimits: false,
    hasParams: true,
  },
];

export function validatorMeta(id: string | undefined): FibValidatorMeta | null {
  if (!id) return null;
  return FIB_VALIDATORS.find((v) => v.id === id) ?? null;
}

/** Build a fresh validation object for a newly-selected type, seeded with the default message. */
export function defaultValidation(id: string): BlankValidation {
  const meta = validatorMeta(id);
  return { type: id, errorMessage: meta?.defaultMessage || undefined };
}

/** Validation types shown in the FIB property strip (legacy FibOptions dropdown). */
export const FIB_VALIDATION_OPTIONS = [
  { value: "", label: "-- No Validation --" },
  ...FIB_VALIDATORS.map((v) => ({ value: v.id, label: v.label })),
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
      validation: prev?.validation,
    };
  });
}

/**
 * Alternate-label uniqueness across a form (legacy `ItemList.ValidAlternateLabel`). Empty is
 * always allowed (falls back to the default letter name). `taken` is the set of every other
 * field name on the form; comparison is case-insensitive to match the runtime field map.
 */
export function isAlternateLabelUnique(label: string, taken: Set<string>): boolean {
  const trimmed = label.trim();
  if (!trimmed) return true;
  return !taken.has(trimmed.toLowerCase());
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

/**
 * Set `range` to cover plain-text offsets [start, end) inside `root`.
 * Returns false when the offsets cannot be resolved.
 */
export function setPlainTextRange(
  root: HTMLElement,
  range: Range,
  start: number,
  end: number,
): boolean {
  const points: { node: Text; offset: number }[] = [];
  const walker = document.createTreeWalker(root, NodeFilter.SHOW_TEXT);
  let node: Text | null;
  let offset = 0;
  while ((node = walker.nextNode() as Text | null)) {
    const len = node.data.length;
    points.push({ node, offset });
    offset += len;
  }
  if (start < 0 || end < start || end > offset) return false;

  const locate = (target: number): { node: Text; offset: number } | null => {
    for (let i = 0; i < points.length; i++) {
      const { node: n, offset: at } = points[i];
      const len = n.data.length;
      if (target <= at + len) return { node: n, offset: target - at };
    }
    const last = points[points.length - 1];
    return last ? { node: last.node, offset: last.node.data.length } : null;
  };

  const a = locate(start);
  const b = locate(end);
  if (!a || !b) return false;
  range.setStart(a.node, a.offset);
  range.setEnd(b.node, b.offset);
  return true;
}
