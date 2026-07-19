/**
 * @vitest-environment happy-dom
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  clearFunctionPickerRequest,
  getFunctionPickerRequest,
  openFunctionPickerFromEditor,
  requestFunctionPicker,
} from "./functionPicker";
import {
  clearActivePaletteEditor,
  setActivePaletteEditor,
} from "./formattingPaletteContext";
import { FUNCTION_TOKEN_CLASS } from "./functionTokens";

describe("openFunctionPickerFromEditor", () => {
  afterEach(() => {
    clearFunctionPickerRequest();
    clearActivePaletteEditor();
    document.body.innerHTML = "";
  });

  it("opens the Insert list when the caret is after a function token (not inside it)", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const token = document.createElement("span");
    token.className = FUNCTION_TOKEN_CLASS;
    token.setAttribute("data-function-id", "display-mcq-label");
    token.setAttribute("data-function-config", "{}");
    token.setAttribute("data-function-instance", "1");
    token.textContent = "<<DISPLAY MCQ…>>";
    const landing = document.createTextNode("\u200b");
    editor.append(token, landing);
    document.body.append(editor);
    editor.focus();

    const range = document.createRange();
    range.setStart(landing, 0);
    range.collapse(true);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    setActivePaletteEditor({
      el: editor,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });

    openFunctionPickerFromEditor();
    const req = getFunctionPickerRequest();
    expect(req?.mode).toBe("insert");
    expect(req?.existing).toBeFalsy();
  });

  it("opens Configure when the selection is inside a function token", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const token = document.createElement("span");
    token.className = FUNCTION_TOKEN_CLASS;
    token.setAttribute("data-function-id", "display-mcq-label");
    token.setAttribute("data-function-config", "{}");
    token.setAttribute("data-function-instance", "1");
    token.textContent = "<<DISPLAY MCQ…>>";
    editor.append(token);
    document.body.append(editor);

    const range = document.createRange();
    range.selectNodeContents(token);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    setActivePaletteEditor({
      el: editor,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });

    openFunctionPickerFromEditor();
    const req = getFunctionPickerRequest();
    expect(req?.mode).toBe("edit");
    expect(req?.existing?.functionId).toBe("display-mcq-label");
  });

  it("does not reopen Insert while a picker request is already active", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    editor.append(document.createTextNode("hello"));
    document.body.append(editor);
    editor.focus();
    const range = document.createRange();
    range.selectNodeContents(editor);
    range.collapse(true);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    setActivePaletteEditor({
      el: editor,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });

    openFunctionPickerFromEditor();
    const first = getFunctionPickerRequest();
    expect(first?.mode).toBe("insert");

    openFunctionPickerFromEditor();
    expect(getFunctionPickerRequest()).toBe(first);
  });

  it("advancing to Configure keeps the request active so fx cannot reopen Insert", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    editor.append(document.createTextNode("hello"));
    document.body.append(editor);
    editor.focus();
    const range = document.createRange();
    range.selectNodeContents(editor);
    range.collapse(true);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    setActivePaletteEditor({
      el: editor,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });

    openFunctionPickerFromEditor();
    expect(getFunctionPickerRequest()?.mode).toBe("insert");

    // Simulate Insert OK → request-phase advance (FunctionPickerHost).
    requestFunctionPicker({
      mode: "insert",
      editor: getFunctionPickerRequest()?.editor,
      configureFunctionId: "project-email-count",
    });
    expect(getFunctionPickerRequest()?.configureFunctionId).toBe("project-email-count");

    openFunctionPickerFromEditor();
    expect(getFunctionPickerRequest()?.configureFunctionId).toBe("project-email-count");
  });
});
