/**
 * Shared Formatting Palette focus context (row 2 app shell).
 * Legacy parity: `MDIFormView.Application_Idle`, `MDIDocumentView` idle handlers.
 *
 * Editors register when their rich-text surface gains focus; the palette reads this
 * state to enable/disable its 14 controls per `DESIGNER_FORM_FORMAT_TOOLBAR.md`.
 */

export type FormattingFocusKind = "none" | "heading" | "text" | "document" | "fib" | "mcq";

export interface FormattingFocusState {
  kind: FormattingFocusKind;
  /** True when the caret sits inside a `<table>` in the active editor. */
  cursorInTable: boolean;
  /** True when the active editor has non-default formatting (reset button). */
  hasResettableFormatting: boolean;
}

export type PaletteControlId =
  | "fontFace"
  | "fontSize"
  | "fontColor"
  | "reset"
  | "bold"
  | "italic"
  | "underline"
  | "outdent"
  | "indent"
  | "alignment"
  | "insertTable"
  | "deleteTable"
  | "tableRowCol"
  | "fx";

const DEFAULT_STATE: FormattingFocusState = {
  kind: "none",
  cursorInTable: false,
  hasResettableFormatting: false,
};

function nodeHasCharacterFormatting(node: Node): boolean {
  if (!(node instanceof HTMLElement)) return false;
  if (node.classList.contains("field-token") || node.classList.contains("function-token")) {
    return false;
  }
  const tag = node.tagName;
  if (tag === "B" || tag === "STRONG" || tag === "I" || tag === "EM" || tag === "U" || tag === "FONT") {
    return true;
  }
  if (tag === "SPAN" && node.hasAttribute("style")) {
    const style = node.style;
    return !!(
      style.fontWeight ||
      style.fontStyle ||
      style.textDecoration ||
      style.color ||
      style.fontFamily ||
      style.fontSize ||
      style.backgroundColor
    );
  }
  return false;
}

function fragmentHasCharacterFormatting(root: ParentNode): boolean {
  if (root instanceof HTMLElement && nodeHasCharacterFormatting(root)) return true;
  for (const el of root.querySelectorAll("b, strong, i, em, u, font, span[style]")) {
    if (el instanceof HTMLElement && nodeHasCharacterFormatting(el)) return true;
  }
  return false;
}

/** True when the caret/selection carries character formatting reset can clear. */
export function selectionHasResettableFormatting(root: HTMLElement | null): boolean {
  if (!root) return false;
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  const range = sel.getRangeAt(0);
  if (!root.contains(range.commonAncestorContainer)) return false;

  if (!range.collapsed) {
    return fragmentHasCharacterFormatting(range.cloneContents());
  }

  let node: Node | null = range.startContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== root) {
    if (nodeHasCharacterFormatting(node)) return true;
    node = node.parentNode;
  }
  return false;
}

/** True when the active editor contains formatting reset can clear (legacy: greyed on fresh doc). */
export function editorHasResettableFormatting(root: HTMLElement | null): boolean {
  if (!root) return false;
  if (!root.innerHTML.replace(/\u00a0|\u200b/g, "").trim()) return false;
  return selectionHasResettableFormatting(root) || !!root.querySelector("table.user, .doc-placed-text");
}

let focusState: FormattingFocusState = { ...DEFAULT_STATE };
const listeners = new Set<() => void>();

/**
 * The rich-text editor the Formatting Palette currently drives. Editors register on focus;
 * the palette calls back into `el` (via `paletteCommands`) so its row-2 buttons format the
 * active surface. Like the field-insert target, this persists across the editor blurring to
 * the palette itself (dropdowns/color picker take focus), and is only replaced when another
 * editor focuses or the current one unmounts.
 */
export interface PaletteEditorHandle {
  el: HTMLElement;
  /** Persist the editor's current HTML back into the store. */
  commit: () => void;
  /** Remember the current selection (before palette focus steals it). */
  saveSelection: () => void;
  /** Restore the remembered selection into the editor. */
  restoreSelection: () => void;
}

let activeEditor: PaletteEditorHandle | null = null;

export function setActivePaletteEditor(handle: PaletteEditorHandle): void {
  activeEditor = handle;
}

/** Clear the active editor. Pass the element to only clear when it is still the active one. */
export function clearActivePaletteEditor(el?: HTMLElement): void {
  if (el && activeEditor?.el !== el) return;
  activeEditor = null;
}

export function getActivePaletteEditor(): PaletteEditorHandle | null {
  return activeEditor;
}

function emit() {
  listeners.forEach((cb) => cb());
}

export function getFormattingFocusState(): FormattingFocusState {
  return focusState;
}

export function subscribeFormattingFocus(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function setFormattingFocus(patch: Partial<FormattingFocusState> & { kind: FormattingFocusKind }) {
  focusState = { ...focusState, ...patch };
  emit();
}

/** Clear focus when an editor blurs; optional `expectedKind` avoids races on focus hand-off. */
export function clearFormattingFocus(expectedKind?: FormattingFocusKind) {
  if (expectedKind && focusState.kind !== expectedKind) return;
  focusState = { ...DEFAULT_STATE };
  emit();
}

/** Walk ancestors from the selection to see if the caret is inside a table. */
export function selectionCursorInTable(root: HTMLElement | null): boolean {
  if (!root) return false;
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  while (node && node !== root) {
    if (node instanceof HTMLTableCellElement || node instanceof HTMLTableRowElement) return true;
    if (node instanceof HTMLElement && node.closest("table") && root.contains(node.closest("table"))) {
      return true;
    }
    node = node.parentNode;
  }
  return false;
}

/**
 * Per-control enable rules for the Formatting Palette shell.
 * @param designTabActive — false on Form Preview tab (entire palette greyed).
 */
export function isPaletteControlEnabled(
  control: PaletteControlId,
  state: FormattingFocusState,
  designTabActive: boolean,
  projectFormCount = 0,
): boolean {
  if (!designTabActive) return false;

  const { kind, cursorInTable } = state;
  if (kind === "none" || kind === "heading") return false;

  if (kind === "fib" || kind === "mcq") {
    return control === "bold" || control === "italic" || control === "underline";
  }

  if (control === "deleteTable" || control === "tableRowCol") {
    return cursorInTable;
  }

  if (control === "insertTable") {
    return kind === "text" || kind === "document";
  }

  if (control === "reset") {
    // TODO: Reset formatting regressed with document typing-format work — re-enable when fixed.
    return false;
  }

  if (control === "fx") {
    if (kind === "text") return true;
    if (kind === "document") return projectFormCount >= 1;
    return false;
  }

  return kind === "text" || kind === "document";
}
