/**
 * Form item reorder: badge-only handles; Text-node drag targets resolve to Elements.
 */
import { describe, expect, it } from "vitest";
import {
  FORM_ITEM_REORDER_HANDLE_SELECTOR,
  dragEventElement,
  isFormItemReorderHandle,
} from "./formItemReorder";

describe("formItemReorder handles", () => {
  it("resolves Text-node drag targets to their parent Element", () => {
    const span = document.createElement("span");
    span.className = "fib-badge";
    const text = document.createTextNode("FIB1");
    span.appendChild(text);
    document.body.appendChild(span);

    expect(dragEventElement(text)).toBe(span);
    expect(isFormItemReorderHandle(text)).toBe(true);

    span.remove();
  });

  it("rejects body/editor surfaces even when selected", () => {
    const editor = document.createElement("div");
    editor.className = "text-rich-editor";
    editor.contentEditable = "true";
    editor.textContent = "Hello";
    document.body.appendChild(editor);

    expect(isFormItemReorderHandle(editor.firstChild)).toBe(false);
    expect(isFormItemReorderHandle(editor)).toBe(false);

    editor.remove();
  });

  it("accepts known badge chrome classes", () => {
    for (const sel of FORM_ITEM_REORDER_HANDLE_SELECTOR.split(", ")) {
      const el = document.createElement("div");
      el.className = sel.slice(1);
      document.body.appendChild(el);
      expect(isFormItemReorderHandle(el)).toBe(true);
      el.remove();
    }
  });
});
