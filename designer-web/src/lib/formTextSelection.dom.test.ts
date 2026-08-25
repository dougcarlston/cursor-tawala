import { afterEach, describe, expect, it } from "vitest";
import { EMBEDDED_IMAGE_CLASS, EMBEDDED_IMAGE_ID_ATTR } from "./projectImages";
import { extendFormTextSelectionToPoint } from "./formTextSelection";

describe("extendFormTextSelectionToPoint", () => {
  afterEach(() => {
    document.body.replaceChildren();
    window.getSelection()?.removeAllRanges();
  });

  it("extends a Form Text range across an embedded image to later text", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    editor.className = "text-rich-editor";
    const p1 = document.createElement("p");
    p1.append("Before ");
    const img = document.createElement("img");
    img.className = EMBEDDED_IMAGE_CLASS;
    img.setAttribute(EMBEDDED_IMAGE_ID_ATTR, "image1");
    img.width = 20;
    img.height = 20;
    p1.append(img);
    p1.append(" mid");
    const p2 = document.createElement("p");
    p2.append("After image line");
    editor.append(p1, p2);
    document.body.append(editor);

    const anchor = document.createRange();
    anchor.setStart(p1.firstChild as Text, 0);
    anchor.collapse(true);

    // Fake caret at end of second paragraph via a stub caretRangeFromPoint.
    const doc = document as Document & {
      caretRangeFromPoint?: (x: number, y: number) => Range | null;
    };
    const prev = doc.caretRangeFromPoint;
    doc.caretRangeFromPoint = () => {
      const r = document.createRange();
      r.setStart(p2.firstChild as Text, (p2.firstChild as Text).length);
      r.collapse(true);
      return r;
    };

    const ok = extendFormTextSelectionToPoint(editor, anchor, 10, 10);
    doc.caretRangeFromPoint = prev;

    expect(ok).toBe(true);
    const sel = window.getSelection();
    expect(sel?.rangeCount).toBe(1);
    const text = sel?.toString() ?? "";
    expect(text).toContain("Before");
    expect(text).toContain("After image line");
  });
});
