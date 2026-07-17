/**
 * Global Insert / Edit Function picker — opened from palette **fx**, Insert → Function…,
 * or by clicking an existing function token.
 */

import {
  clearActivePaletteEditor,
  getActivePaletteEditor,
  setActivePaletteEditor,
  type PaletteEditorHandle,
} from "./formattingPaletteContext";
import { getFunctionDef, type FunctionConfig, type FunctionDef } from "./functionCatalog";
import {
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

let pendingRequest: FunctionPickerRequest | null = null;
/** React `useSyncExternalStore` subscribers — must not be invoked synchronously in subscribe. */
const listeners = new Set<() => void>();

function emit() {
  listeners.forEach((cb) => cb());
}

export function subscribeFunctionPicker(listener: () => void): () => void {
  listeners.add(listener);
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

/**
 * True when the caret/selection is *inside* a function token (legacy: active element
 * id starts with `func_`). Caret merely after a token must still open the Insert list.
 */
function functionTokenContainingSelection(root: HTMLElement): FunctionTokenRef | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== root) {
    if (
      node instanceof HTMLSpanElement &&
      node.classList.contains(FUNCTION_TOKEN_CLASS) &&
      node.hasAttribute("data-function-id")
    ) {
      return tokenRefFromElement(node);
    }
    node = node.parentNode;
  }
  return null;
}

/** Open the picker from palette **fx** or Insert → Function…. */
export function openFunctionPickerFromEditor(): void {
  let handle = getActivePaletteEditor();
  if (handle && !handle.el.isConnected) {
    clearActivePaletteEditor(handle.el);
    handle = null;
  }
  if (!handle) {
    // Recover after image insert / remount left focus kind live but cleared the handle.
    const live =
      document.querySelector<HTMLElement>(
        ".text-canvas-row.selected.editing .text-rich-editor, .text-canvas-row.editing .text-rich-editor",
      ) ??
      document.querySelector<HTMLElement>(".rich-surface[contenteditable='true']");
    if (live?.isConnected) {
      handle = {
        el: live,
        commit: () => {
          live.dispatchEvent(new InputEvent("input", { bubbles: true }));
        },
        saveSelection: () => {},
        restoreSelection: () => {
          live.focus();
        },
      };
      // Re-seed so later palette clicks hit the live editor.
      setActivePaletteEditor(handle);
    }
  }
  if (!handle) {
    useProjectStore
      .getState()
      .setStatus("Click inside a Form Text or Document first, then Insert → Function…");
    return;
  }
  try {
    handle.saveSelection();
  } catch {
    /* selection may be outside a remounted editor */
  }
  // Legacy FormView.functionToolStripMenuItem_Click: Edit only when the function
  // chip itself is active; otherwise always show the Insert Function list.
  const existing = functionTokenContainingSelection(handle.el);
  requestFunctionPicker({
    mode: existing ? "edit" : "insert",
    existing,
    editor: handle,
  });
}

/**
 * Insert → Image… → From the Web… — same Configure as Function → DISPLAY IMAGE.
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
