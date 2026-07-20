/**
 * Function chip relocate must move only the token — not sibling paragraph text.
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  FUNCTION_TOKEN_ATTR,
  FUNCTION_TOKEN_CLASS,
  moveFunctionTokenToSelection,
} from "./functionTokens";
import { isDocumentInlineTokenTarget, PLACED_TEXT_CLASS } from "./documentCanvas";
import { FIELD_TOKEN_CLASS } from "./fieldTokens";
import { formatPt } from "./tableLayout";

describe("isDocumentInlineTokenTarget", () => {
  it("detects field and function chips", () => {
    const field = document.createElement("span");
    field.className = FIELD_TOKEN_CLASS;
    const func = document.createElement("span");
    func.className = FUNCTION_TOKEN_CLASS;
    const plain = document.createElement("span");
    plain.textContent = "like ";
    expect(isDocumentInlineTokenTarget(field)).toBe(true);
    expect(isDocumentInlineTokenTarget(func)).toBe(true);
    expect(isDocumentInlineTokenTarget(plain)).toBe(false);
  });
});

describe("moveFunctionTokenToSelection", () => {
  it("relocates the chip and leaves sibling text on the source line", () => {
    const editor = document.createElement("div");
    editor.className = "rich-surface";
    editor.contentEditable = "true";
    document.body.appendChild(editor);

    const source = document.createElement("p");
    source.className = PLACED_TEXT_CLASS;
    source.style.position = "absolute";
    source.style.left = formatPt(36);
    source.style.top = formatPt(40);
    source.append("like ");
    const chip = document.createElement("span");
    chip.className = FUNCTION_TOKEN_CLASS;
    chip.setAttribute(FUNCTION_TOKEN_ATTR, "display-mcq");
    chip.textContent = "<<DISPLAY MCQ>>";
    source.append(chip);
    source.append(" except");
    editor.appendChild(source);

    const dest = document.createElement("p");
    dest.className = PLACED_TEXT_CLASS;
    dest.style.position = "absolute";
    dest.style.left = formatPt(36);
    dest.style.top = formatPt(80);
    dest.textContent = "these types are: ";
    editor.appendChild(dest);

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(dest.firstChild as Text, dest.textContent!.length);
    range.collapse(true);
    sel?.removeAllRanges();
    sel?.addRange(range);

    moveFunctionTokenToSelection(chip);

    expect(source.textContent).toBe("like  except");
    expect(source.querySelector(`.${FUNCTION_TOKEN_CLASS}`)).toBeNull();
    expect(dest.querySelector(`.${FUNCTION_TOKEN_CLASS}`)).toBe(chip);
    expect(dest.textContent).toContain("these types are:");
    expect(dest.textContent).toContain("<<DISPLAY MCQ>>");

    editor.remove();
  });
});
