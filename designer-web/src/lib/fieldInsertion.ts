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
/** Form branch name when a field leaf is dragged from under a form folder (not Variables). */
export const FIELD_DRAG_FORM_MIME = "application/x-tawala-field-form";
/** In-editor drag of an existing field token (move, not copy from Fields palette). */
export const FIELD_TOKEN_MOVE_MIME = "application/x-tawala-field-token-move";

/** Token currently being relocated via HTML5 drag (cleared on drop / dragend). */
let movingFieldToken: HTMLElement | null = null;

/** Legacy `<<FieldReference>>` token wrapping a field/variable name. */
export function fieldToken(name: string): string {
  return `<<${name}>>`;
}

/** Populate a drag DataTransfer for a field/variable leaf. */
export function setFieldDragData(
  dataTransfer: DataTransfer,
  name: string,
  formName?: string,
): void {
  dataTransfer.setData(FIELD_DRAG_MIME, name);
  if (formName) {
    dataTransfer.setData(FIELD_DRAG_FORM_MIME, formName);
  }
  dataTransfer.setData("text/plain", fieldToken(name));
  dataTransfer.effectAllowed = "copy";
}

/** Start relocating an existing field token already in the editor. */
export function beginFieldTokenMove(token: HTMLElement, dataTransfer: DataTransfer, name: string): void {
  movingFieldToken = token;
  dataTransfer.setData(FIELD_DRAG_MIME, name);
  dataTransfer.setData(FIELD_TOKEN_MOVE_MIME, "1");
  dataTransfer.setData("text/plain", fieldToken(name));
  dataTransfer.effectAllowed = "move";
  try {
    dataTransfer.setDragImage(token, Math.min(12, token.offsetWidth / 2), Math.min(8, token.offsetHeight / 2));
  } catch {
    /* some browsers reject setDragImage on detached/odd nodes */
  }
}

/** True when the in-flight drag is relocating an editor token (not a Fields-palette copy). */
export function isFieldTokenMoveDrag(dataTransfer: DataTransfer | null): boolean {
  if (movingFieldToken) return true;
  if (!dataTransfer) return false;
  return Array.from(dataTransfer.types).includes(FIELD_TOKEN_MOVE_MIME);
}

/** Consume and clear the token being moved; null if this was a palette copy drop. */
export function takeMovingFieldToken(): HTMLElement | null {
  const token = movingFieldToken;
  movingFieldToken = null;
  return token;
}

export function clearMovingFieldToken(): void {
  movingFieldToken = null;
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

/** Form folder name carried on a Fields-tree drag, when present. */
export function readFieldDragFormName(dataTransfer: DataTransfer | null): string | null {
  if (!dataTransfer) return null;
  const form = dataTransfer.getData(FIELD_DRAG_FORM_MIME);
  return form || null;
}

/** True when drag carries form-branch context (types-only check for `dragover`). */
export function hasFieldDragFormContext(dataTransfer: DataTransfer | null): boolean {
  if (!dataTransfer) return false;
  return Array.from(dataTransfer.types).includes(FIELD_DRAG_FORM_MIME);
}

/**
 * Legacy `Form:Field` for process/skip If conditions — qualify bare leaves from a form branch.
 * Variables and already-qualified names pass through unchanged.
 */
export function qualifyPaletteFieldName(name: string, formName?: string | null): string {
  const trimmed = name.trim();
  if (!trimmed || trimmed.includes(":") || !formName) return trimmed;
  return `${formName}:${trimmed}`;
}

/** Resolve a palette leaf for the active editor target (double-click insert). */
export function paletteLeafInsertName(
  leafName: string,
  formName: string | undefined,
  _target: FieldTargetContext,
  insertOverride?: string,
): string {
  if (insertOverride?.trim()) return insertOverride.trim();
  return qualifyPaletteFieldName(leafName, formName);
}

/** Resolve a dragged field for a drop target (qualifies bare form leaves when needed). */
export function readFieldDragNameForTarget(
  dataTransfer: DataTransfer | null,
  _target: FieldTargetContext,
): string | null {
  const name = readFieldDragName(dataTransfer);
  if (!name) return null;
  return qualifyPaletteFieldName(name, readFieldDragFormName(dataTransfer));
}

/** Text to insert after a drop or double-click — bare `Form:Field` or `<<Form:Field>>`. */
export function fieldInsertText(
  qualifiedName: string,
  target: FieldTargetContext,
): string {
  return target.bare ? qualifiedName : fieldToken(qualifiedName);
}

/** True for `RecordVar:Form:Field` references from ForEach / stored-record contexts. */
export function isRecordQualifiedField(name: string): boolean {
  const segments = name.trim().split(":");
  return segments.length >= 3 && segments.every((s) => s.length > 0);
}

/** Whether `name` may insert into a target with the given options. */
export function fieldAcceptedByTarget(
  name: string,
  target: FieldTargetContext,
): boolean {
  if (target.formFieldsOnly && !isFormFieldReference(name)) return false;
  const trimmed = name.trim();
  if (target.knownVariables && !isValidIfConditionField(trimmed, target.knownVariables)) {
    if (isRecordQualifiedField(trimmed)) return true;
    return false;
  }
  return true;
}

/**
 * Whether an in-flight field drag may drop on a target. During `dragover`, bare field names
 * and form MIME may be present while `getData` is still unreadable — accept those optimistically.
 */
export function fieldDragAcceptedByTarget(
  dataTransfer: DataTransfer,
  target: FieldTargetContext,
): boolean {
  const name = readFieldDragName(dataTransfer);
  if (!name) return true;
  const resolved = qualifyPaletteFieldName(name, readFieldDragFormName(dataTransfer));
  if (fieldAcceptedByTarget(resolved, target)) return true;
  // During `dragover`, form MIME may be present while bare names are still unreadable.
  if (!name.includes(":") && hasFieldDragFormContext(dataTransfer)) return true;
  return false;
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
  /** Allow register while Configure Function lock is on (Configure dialog inputs only). */
  configureDialog?: boolean;
};

let activeInserter: ActiveInserter | null = null;
let activeTargetContext: FieldTargetContext = {};
const activeTargetListeners = new Set<() => void>();

/**
 * While Configure Function is open, canvas editors must not steal Fields double-click —
 * only inputs that pass `configureDialog: true` may register as the insert target.
 */
let configureFunctionFieldLock = false;

export function setConfigureFunctionFieldLock(locked: boolean): void {
  configureFunctionFieldLock = locked;
  if (locked) {
    activeInserter = null;
    activeTargetContext = {};
    notifyActiveTargetListeners();
  }
}

export function isConfigureFunctionFieldLock(): boolean {
  return configureFunctionFieldLock;
}

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
  if (!el?.closest) return false;
  // Fields panel, main menu (Insert → Function/Image), and Configure/Insert Function dialogs
  // must not collapse the Form Text / Document editor mid-insert.
  return Boolean(
    el.closest(
      [
        ".fields-tree",
        ".fields-palette",
        ".designer-left",
        ".designer-items",
        ".main-icon-toolbar",
        ".menu-bar",
        ".menu-drop",
        ".menu-submenu",
        ".modal-overlay",
        ".modal-backdrop",
        ".configure-function-dialog",
        ".insert-function-dialog",
        ".fib-validation-dialog",
      ].join(", "),
    ),
  );
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
  if (configureFunctionFieldLock && inserter && !context.configureDialog) {
    return;
  }
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
  return fieldAcceptedByTarget(name, activeTargetContext);
}

/** Fire the active editor's inserter (double-click). Returns false when nothing is focused. */
export function insertFieldIntoActiveTarget(name: string): boolean {
  if (!activeInserter) return false;
  if (!fieldLeafAcceptedByActiveTarget(name)) return false;
  activeInserter(name);
  return true;
}
