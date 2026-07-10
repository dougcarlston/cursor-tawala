/**
 * Global Insert / Edit Function picker — opened from palette **fx** or Insert → Function….
 */

import { getActivePaletteEditor } from "./formattingPaletteContext";
import { findFunctionTokenAtSelection, type FunctionTokenRef } from "./functionTokens";

export type FunctionPickerMode = "insert" | "edit";

export interface FunctionPickerRequest {
  mode: FunctionPickerMode;
  existing?: FunctionTokenRef | null;
}

type FunctionPickerListener = (request: FunctionPickerRequest | null) => void;

let pendingRequest: FunctionPickerRequest | null = null;
const listeners = new Set<FunctionPickerListener>();

function emit() {
  listeners.forEach((cb) => cb(pendingRequest));
}

export function subscribeFunctionPicker(listener: FunctionPickerListener): () => void {
  listeners.add(listener);
  listener(pendingRequest);
  return () => listeners.delete(listener);
}

export function getFunctionPickerRequest(): FunctionPickerRequest | null {
  return pendingRequest;
}

export function requestFunctionPicker(request: FunctionPickerRequest): void {
  pendingRequest = request;
  emit();
}

export function clearFunctionPickerRequest(): void {
  pendingRequest = null;
  emit();
}

/** Open the picker from palette **fx** or Insert → Function…. */
export function openFunctionPickerFromEditor(): void {
  const handle = getActivePaletteEditor();
  if (!handle) return;
  handle.saveSelection();
  const existing = findFunctionTokenAtSelection(handle.el);
  requestFunctionPicker({
    mode: existing ? "edit" : "insert",
    existing,
  });
}
