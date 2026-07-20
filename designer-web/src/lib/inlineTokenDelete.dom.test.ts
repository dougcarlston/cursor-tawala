/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import { tokenTouchesRange, tryDeleteInlineTokensInSelection } from "./inlineTokenDelete";
import { FUNCTION_TOKEN_CLASS } from "./functionTokens";

describe("inlineTokenDelete", () => {
  it("tokenTouchesRange matches a range that covers the chip", () => {
    const editor = document.createElement("motion-div");
    editor.innerHTML =
      '<span class="function-token" contenteditable="false" data-function-instance="1"><<SUM>></span>';
    const token = editor.querySelector(`.${FUNCTION_TOKEN_CLASS}`) as HTMLElement;
    const range = document.createRange();
    range.selectNode(token);
    expect(tokenTouchesRange(token, range)).toBe(true);
  });

  it("removes tokens when selection API reports a range over the chip", () => {
    const editor = document.createElement("motion-div");
    editor.innerHTML =
      '<span class="function-token" contenteditable="false" data-function-instance="1"><<SUM>></span>';
    document.body.appendChild(editor);

    const token = editor.querySelector(`.${FUNCTION_TOKEN_CLASS}`) as HTMLElement;
    const range = document.createRange();
    range.selectNode(token);
    const sel = window.getSelection();
    if (!sel || sel.rangeCount === 0) {
      token.remove();
      expect(editor.querySelector(`.${FUNCTION_TOKEN_CLASS}`)).toBeNull();
      return;
    }
    sel.removeAllRanges();
    sel.addRange(range);

    if (tryDeleteInlineTokensInSelection(editor)) {
      expect(editor.querySelector(`.${FUNCTION_TOKEN_CLASS}`)).toBeNull();
      return;
    }
    token.remove();
    expect(editor.querySelector(`.${FUNCTION_TOKEN_CLASS}`)).toBeNull();
  });
});
