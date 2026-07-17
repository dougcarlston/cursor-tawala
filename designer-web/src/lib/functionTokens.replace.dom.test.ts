/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  FUNCTION_TOKEN_CLASS,
  insertFunctionTokenAtSelection,
  tokenRefFromElement,
} from "./functionTokens";
import { getFunctionDef } from "./functionCatalog";

describe("insertFunctionTokenAtSelection replace", () => {
  it("preserves font-size when replacing after Configure (no typing-format jitter)", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    document.body.append(editor);

    const def = getFunctionDef("response-totals-table")!;
    const existing = document.createElement("span");
    existing.className = FUNCTION_TOKEN_CLASS;
    existing.setAttribute("data-function-id", def.id);
    existing.setAttribute("data-function-instance", "1");
    existing.setAttribute("data-function-config", "{}");
    existing.style.fontSize = "11pt";
    existing.textContent = "<<RESPONSE TOTALS>>";
    editor.append(existing);

    const range = document.createRange();
    range.selectNode(existing);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    insertFunctionTokenAtSelection(
      editor,
      def,
      { "layout-type": "vertical", field: "Form 1:MCQ1" },
      tokenRefFromElement(existing),
    );

    const next = editor.querySelector(`.${FUNCTION_TOKEN_CLASS}`) as HTMLElement;
    expect(next).toBeTruthy();
    expect(next.style.fontSize).toBe("11pt");
    expect(next.textContent).toContain("RESPONSE TOTALS");
  });
});
