/**
 * Global Insert / Edit Function picker — opened from palette **fx**, Insert → Function…,
 * or by clicking an existing function token.
 */

import { getActivePaletteEditor, type PaletteEditorHandle } from "./formattingPaletteContext";
import { getFunctionDef, type FunctionConfig, type FunctionDef } from "./functionCatalog";
import {
  findFunctionTokenAtSelection,
  FUNCTION_TOKEN_CLASS,
  tokenRefFromElement,
  type FunctionTokenRef,
} from "./functionTokens";
import { useProjectStore } from "@/store/projectStore";

export type FunctionPickerMode = "insert" | "edit";

export interface FunctionPickerRequest {
  mode: FunctionPickerMode;
  existing?: FunctionTokenRef | null;
  /**
   * Snapshot of the Form Text / Document editor at open time. Required because opening
   * the menu can blur the canvas; OK must still insert into this handle.
   */
  editor?: PaletteEditorHandle | null;
  /**
   * When set (e.g. structured Form Text function tables), Configure saves through this
   * instead of inserting/replacing a DOM token in the active palette editor.
   */
  commitConfig?: (def: FunctionDef, config: FunctionConfig) => void;
  /**
   * Skip the function list and open Configure for this catalog id
   * (e.g. Insert → Image… → From the Web → DISPLAY IMAGE).
   */
  configureFunctionId?: string;
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
  if (!handle) {
    useProjectStore
      .getState()
      .setStatus("Click inside a Form Text or Document first, then Insert → Function…");
    return;
  }
  handle.saveSelection();
  const existing = findFunctionTokenAtSelection(handle.el);
  requestFunctionPicker({
    mode: existing ? "edit" : "insert",
    existing,
    editor: handle,
  });
}

/**
 * Insert → Image… → From the Web or Tawala Upload… — same Configure as Function → DISPLAY IMAGE.
 */
export function openDisplayImageConfigureFromEditor(): void {
  const handle = getActivePaletteEditor();
  if (!handle) {
    useProjectStore
      .getState()
      .setStatus("Click inside a Form Text or Document first, then Insert → Image…");
    return;
  }
  handle.saveSelection();
  requestFunctionPicker({
    mode: "insert",
    configureFunctionId: "display-image",
    editor: handle,
  });
}

/**
 * Select a function token and open Configure with its saved parameters.
 * Caller must register the palette editor (and saveSelection) so OK can rewrite the token.
 */
export function openFunctionTokenForEdit(
  token: HTMLElement,
  editor: HTMLElement,
  saveSelection?: () => void,
): boolean {
  if (!(token instanceof HTMLSpanElement) || !token.classList.contains(FUNCTION_TOKEN_CLASS)) {
    return false;
  }
  if (!editor.contains(token)) return false;
  const ref = tokenRefFromElement(token);
  if (!ref.functionId || !getFunctionDef(ref.functionId)) return false;

  editor.focus();
  const range = document.createRange();
  range.selectNode(token);
  const sel = window.getSelection();
  sel?.removeAllRanges();
  sel?.addRange(range);
  saveSelection?.();

  const active = getActivePaletteEditor();
  const editorHandle: PaletteEditorHandle =
    active && active.el === editor
      ? active
      : {
          el: editor,
          commit: () => {},
          saveSelection: saveSelection ?? (() => {}),
          restoreSelection: () => {
            const s = window.getSelection();
            if (!s) return;
            const r = document.createRange();
            r.selectNode(token);
            s.removeAllRanges();
            s.addRange(r);
          },
        };

  requestFunctionPicker({ mode: "edit", existing: ref, editor: editorHandle });
  return true;
}
