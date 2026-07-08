/**
 * Fields panel → editor drag-and-drop (backlog §1 Phase 2, legacy `FieldsPalette.cs`).
 *
 * A dragged field/variable leaf carries two payloads:
 *  - a custom MIME (`FIELD_DRAG_MIME`) holding the bare field name, so drop targets can
 *    distinguish a Tawala-field drag during `dragover` (when `getData` is unreadable, only
 *    `dataTransfer.types` is available) and insert a controlled `<<name>>` token; and
 *  - `text/plain` holding the ready-made `<<name>>` token, so any native editable control
 *    (uncontrolled textareas, external targets) inserts the correct reference automatically.
 *
 * Double-click insert (legacy: insert at current editor focus) reuses the same token via a
 * module-level "active target" the focused editor registers; see `insertFieldIntoActiveTarget`.
 */

export const FIELD_DRAG_MIME = "application/x-tawala-field";

/** Legacy `<<FieldReference>>` token wrapping a field/variable name. */
export function fieldToken(name: string): string {
  return `<<${name}>>`;
}

/** Populate a drag DataTransfer for a field/variable leaf. */
export function setFieldDragData(dataTransfer: DataTransfer, name: string): void {
  dataTransfer.setData(FIELD_DRAG_MIME, name);
  dataTransfer.setData("text/plain", fieldToken(name));
  dataTransfer.effectAllowed = "copy";
}

/** True when an in-flight drag carries a Tawala field (checked during `dragover`). */
export function hasFieldDrag(dataTransfer: DataTransfer | null): boolean {
  if (!dataTransfer) return false;
  return Array.from(dataTransfer.types).includes(FIELD_DRAG_MIME);
}

/** Field name from a drop event, or null when the drop is not a Tawala field. */
export function readFieldDragName(dataTransfer: DataTransfer | null): string | null {
  if (!dataTransfer) return null;
  const custom = dataTransfer.getData(FIELD_DRAG_MIME);
  if (custom) return custom;
  const text = dataTransfer.getData("text/plain");
  const match = text.match(/^<<(.+)>>$/);
  return match ? match[1] : null;
}

/** Insert `<<token>>` into an input/textarea at the caret, replacing any selection. */
export function insertTokenAtCaret(
  el: HTMLInputElement | HTMLTextAreaElement,
  token: string,
  onValueChange: (next: string) => void,
): void {
  const value = el.value;
  const start = el.selectionStart ?? value.length;
  const end = el.selectionEnd ?? value.length;
  const next = value.slice(0, start) + token + value.slice(end);
  onValueChange(next);
  const caret = start + token.length;
  // Restore caret after React re-renders the controlled value.
  requestAnimationFrame(() => {
    try {
      el.focus();
      el.setSelectionRange(caret, caret);
    } catch {
      /* element may have unmounted */
    }
  });
}

/** Insert a field token into the editor that currently owns insertion focus. */
type ActiveInserter = (name: string) => void;

/** Options for the focused drop/double-click target (If vs Set, bare vs token, etc.). */
export type FieldTargetContext = {
  /** Insert bare `Form:Field` (process If conditions) instead of `<<Form:Field>>`. */
  bare?: boolean;
  /** Reject Variables-folder leaves — only colon-qualified form fields (If condition field). */
  formFieldsOnly?: boolean;
  /** If set: accept form fields or these variables only — reject unknown plain names (If). */
  knownVariables?: ReadonlySet<string>;
};

let activeInserter: ActiveInserter | null = null;
let activeTargetContext: FieldTargetContext = {};
const activeTargetListeners = new Set<() => void>();

function notifyActiveTargetListeners(): void {
  for (const listener of activeTargetListeners) listener();
}

/** True while a Fields-panel leaf drag is in flight (suppresses accidental modal dismiss). */
let fieldDragActive = false;

export function setFieldDragActive(active: boolean): void {
  fieldDragActive = active;
}

export function isFieldDragActive(): boolean {
  return fieldDragActive;
}

/**
 * Canvas editors should stay mounted when focus moves to the Fields panel for a drag or
 * double-click insert — otherwise blur collapses the contenteditable before drop fires.
 */
export function retainEditorFocusOnBlur(relatedTarget: EventTarget | null): boolean {
  if (isFieldDragActive()) return true;
  const el = relatedTarget as HTMLElement | null;
  return Boolean(el?.closest?.(".fields-tree, .fields-palette"));
}

/**
 * Register (or clear) the editor target that a double-clicked field leaf should insert into.
 * The most recently focused drop-enabled editor wins, matching legacy "insert at current
 * editor focus". Not cleared on blur so clicking a tree leaf (which blurs the editor) still
 * targets the last-focused editor.
 */
export function setActiveFieldTarget(
  inserter: ActiveInserter | null,
  context: FieldTargetContext = {},
): void {
  activeInserter = inserter;
  activeTargetContext = inserter ? context : {};
  notifyActiveTargetListeners();
}

/** Subscribe to active target context changes (Fields palette If-only filtering). */
export function subscribeActiveFieldTargetContext(listener: () => void): () => void {
  activeTargetListeners.add(listener);
  return () => {
    activeTargetListeners.delete(listener);
  };
}

/** Snapshot for `useSyncExternalStore` — current active target context. */
export function getActiveFieldTargetContextSnapshot(): Readonly<FieldTargetContext> {
  return activeTargetContext;
}

/** Context for the editor input that last registered as the active field target. */
export function getActiveFieldTargetContext(): Readonly<FieldTargetContext> {
  return activeTargetContext;
}

/** True when the name is a form-field reference (`Form:Field`), not a plain variable. */
export function isFormFieldReference(name: string): boolean {
  return name.trim().includes(":");
}

/** If condition field: form field or existing project variable — not a new variable name. */
export function isValidIfConditionField(
  name: string,
  knownVariables: ReadonlySet<string>,
): boolean {
  const trimmed = name.trim();
  if (!trimmed) return false;
  if (isFormFieldReference(trimmed)) return true;
  return knownVariables.has(trimmed);
}

/** Whether a Fields-panel leaf may insert into the active target. */
export function fieldLeafAcceptedByActiveTarget(name: string): boolean {
  if (!activeInserter) return false;
  if (activeTargetContext.formFieldsOnly && !isFormFieldReference(name)) return false;
  if (
    activeTargetContext.knownVariables &&
    !isValidIfConditionField(name, activeTargetContext.knownVariables)
  ) {
    return false;
  }
  return true;
}

/** Fire the active editor's inserter (double-click). Returns false when nothing is focused. */
export function insertFieldIntoActiveTarget(name: string): boolean {
  if (!activeInserter) return false;
  if (!fieldLeafAcceptedByActiveTarget(name)) return false;
  activeInserter(name);
  return true;
}
