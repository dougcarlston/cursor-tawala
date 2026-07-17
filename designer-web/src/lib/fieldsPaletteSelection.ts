/**
 * Fields palette leaf selection — enables Document Insert → Field (legacy).
 * Separate from drag/double-click insert; tracks the highlighted leaf.
 */

let selectedInsertName: string | null = null;
const listeners = new Set<() => void>();

function emit() {
  listeners.forEach((cb) => cb());
}

/** Insert name for the highlighted leaf (`Form:Field`, variable, or `Record:…`). */
export function getFieldsPaletteSelection(): string | null {
  return selectedInsertName;
}

export function subscribeFieldsPaletteSelection(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function setFieldsPaletteSelection(insertName: string | null): void {
  const next = insertName?.trim() ? insertName.trim() : null;
  if (next === selectedInsertName) return;
  selectedInsertName = next;
  emit();
}

/** Test helper. */
export function resetFieldsPaletteSelectionForTests(): void {
  selectedInsertName = null;
  emit();
}
