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

let activeInserter: ActiveInserter | null = null;

/**
 * Register (or clear) the editor target that a double-clicked field leaf should insert into.
 * The most recently focused drop-enabled editor wins, matching legacy "insert at current
 * editor focus". Not cleared on blur so clicking a tree leaf (which blurs the editor) still
 * targets the last-focused editor.
 */
export function setActiveFieldTarget(inserter: ActiveInserter | null): void {
  activeInserter = inserter;
}

/** Fire the active editor's inserter (double-click). Returns false when nothing is focused. */
export function insertFieldIntoActiveTarget(name: string): boolean {
  if (!activeInserter) return false;
  activeInserter(name);
  return true;
}
