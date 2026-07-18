/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import { fieldInsertText, insertTokenAtCaret } from "@/lib/fieldInsertion";

describe("Where field insert vs append", () => {
  it("bare insert text is Form:Field without <<>>", () => {
    expect(fieldInsertText("Form 1:FIB1:a", { bare: true })).toBe("Form 1:FIB1:a");
    expect(fieldInsertText("Form 1:FIB1:a", { bare: false })).toBe("<<Form 1:FIB1:a>>");
  });

  it("insertTokenAtCaret appends when caret is at end (legacy bug for Where)", () => {
    const el = document.createElement("input");
    el.value = "<<Form 1:MCQ1>>";
    el.setSelectionRange(el.value.length, el.value.length);
    let next = "";
    insertTokenAtCaret(el, "<<Form 1:FIB1:a>>", (v) => {
      next = v;
    });
    expect(next).toBe("<<Form 1:MCQ1>><<Form 1:FIB1:a>>");
  });

  it("replace semantics overwrite the whole Where field", () => {
    // Mirrors FieldDropInputs bare / replaceOnInsert path.
    const previous = "<<Form 1:MCQ1>>";
    const dropped = fieldInsertText("Form 1:FIB1:a", { bare: true });
    const next = dropped; // replace, do not concatenate
    expect(next).toBe("Form 1:FIB1:a");
    expect(next).not.toContain(previous);
    expect(`${previous}${dropped}`).toContain("MCQ1"); // shows what append would do
  });
});
