/**
 * Ephemeral Fields-panel context not stored in the project file.
 * Legacy `FieldsPalette.ConditionsForms` for Show → Stored Record.
 */

let conditionsRecordForm: string | null = null;
const listeners = new Set<() => void>();

function notify(): void {
  for (const listener of listeners) listener();
}

export function setFieldsPaletteConditionsForm(formName: string | null): void {
  const next = formName?.trim() || null;
  if (conditionsRecordForm === next) return;
  conditionsRecordForm = next;
  notify();
}

export function getFieldsPaletteConditionsFormSnapshot(): string | null {
  return conditionsRecordForm;
}

export function subscribeFieldsPaletteConditionsForm(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}
