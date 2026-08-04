/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  embedPlainFieldTokensAsHtml,
  FIELD_NAME_ATTR,
  FIELD_TOKEN_CLASS,
  readFieldNameFromToken,
} from "./fieldTokens";
import { htmlToPlainText as fibHtmlToPlain } from "./fibBlanks";

function naiveHtmlToPlain(html: string): string {
  const tmp = document.createElement("div");
  tmp.innerHTML = html;
  return tmp.textContent ?? "";
}

describe("MCQ question field token HTML parse", () => {
  it("shows why naive innerHTML destroys <<Question:Question>>", () => {
    const raw = "<<Question:Question>>";
    const naive = naiveHtmlToPlain(raw);
    // happy-dom / browsers tag-parse the second <…> segment
    expect(naive).not.toBe(raw);
    expect(fibHtmlToPlain(raw)).toBe(raw);
  });

  it("embedPlainFieldTokensAsHtml keeps full Form:Field on real DOM", () => {
    const raw = "<<Question:Question>>";
    const html = embedPlainFieldTokensAsHtml(raw);
    const div = document.createElement("div");
    div.innerHTML = html;
    expect(div.textContent).toBe(raw);
    const chip = div.querySelector(`.${FIELD_TOKEN_CLASS}`) as HTMLElement;
    expect(readFieldNameFromToken(chip)).toBe("Question:Question");
    expect(chip.getAttribute(FIELD_NAME_ATTR)).toBe("Question:Question");
  });
});
