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

const DEFAULT_STATE: FormattingFocusState = { kind: "none", cursorInTable: false };

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

  if (control === "fx") {
    return kind === "text" || kind === "document";
  }

  return kind === "text" || kind === "document";
}
