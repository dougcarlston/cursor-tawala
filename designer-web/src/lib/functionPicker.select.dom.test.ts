/**
 * @vitest-environment happy-dom
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  clearFunctionPickerRequest,
  getFunctionPickerRequest,
  openFunctionTokenForEdit,
  selectFunctionToken,
} from "./functionPicker";
import { FUNCTION_TOKEN_CLASS } from "./functionTokens";

describe("selectFunctionToken", () => {
  afterEach(() => {
    clearFunctionPickerRequest();
    document.body.innerHTML = "";
  });

  it("selects a chip without opening Configure", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const token = document.createElement("span");
    token.className = FUNCTION_TOKEN_CLASS;
    token.setAttribute("data-function-id", "sum");
    token.setAttribute("data-function-config", "{}");
    token.setAttribute("data-function-instance", "1");
    token.textContent = "<<SUM>>";
    editor.append(token);
    document.body.append(editor);

    expect(selectFunctionToken(token, editor)).toBe(true);
    expect(getFunctionPickerRequest()).toBeNull();

    const sel = window.getSelection();
    expect(sel?.rangeCount).toBe(1);
    const range = sel!.getRangeAt(0);
    expect(range.startContainer).toBe(token.parentNode);
    expect(range.endContainer).toBe(token.parentNode);
  });

  it("openFunctionTokenForEdit opens Configure after selecting", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const token = document.createElement("span");
    token.className = FUNCTION_TOKEN_CLASS;
    token.setAttribute("data-function-id", "sum");
    token.setAttribute("data-function-config", "{}");
    token.setAttribute("data-function-instance", "1");
    token.textContent = "<<SUM>>";
    editor.append(token);
    document.body.append(editor);

    expect(openFunctionTokenForEdit(token, editor)).toBe(true);
    expect(getFunctionPickerRequest()?.mode).toBe("edit");
    expect(getFunctionPickerRequest()?.existing?.functionId).toBe("sum");
  });
});
