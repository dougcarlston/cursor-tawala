/**
 * Document double-click: empty invent / free space must leave a usable caret.
 * mouseup skips place-at-point for detail≥2; word-select must not leave a dead canvas.
 */
import { describe, expect, it } from "vitest";
import {
  PLACED_TEXT_CLASS,
  placeDocumentTextAtPoint,
  selectWordOrTokenAtPoint,
} from "./documentCanvas";

function makeDocEditor(): HTMLElement {
  const editor = document.createElement("div");
  editor.contentEditable = "true";
  editor.style.position = "relative";
  editor.style.width = "400px";
  editor.style.height = "300px";
  document.body.appendChild(editor);
  return editor;
}

function makePlaced(editor: HTMLElement, left: number, top: number, html: string): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = `${left}pt`;
  p.style.top = `${top}pt`;
  p.innerHTML = html;
  editor.appendChild(p);
  return p;
}

describe("Document dblclick caret / invent", () => {
  it("selectWordOrTokenAtPoint lands a caret in an empty invent (br-only)", () => {
    const editor = makeDocEditor();
    const invent = makePlaced(editor, 36, 40, "<br>");
    document.elementFromPoint = () => invent;
    (document as Document & { caretRangeFromPoint?: (x: number, y: number) => Range | null }).caretRangeFromPoint =
      () => null;

    const ok = selectWordOrTokenAtPoint(editor, 60, 55);
    expect(ok).toBe(true);
    const sel = window.getSelection();
    expect(sel?.rangeCount).toBe(1);
    expect(sel?.isCollapsed).toBe(true);
    expect(invent.contains(sel!.anchorNode!) || sel!.anchorNode === invent).toBe(true);

    editor.remove();
  });

  it("placeDocumentTextAtPoint invents on blank canvas after a failed word pick", () => {
    const editor = makeDocEditor();
    // No text nodes — selectWord would find nothing; place must invent.
    const invented = placeDocumentTextAtPoint(editor, 80, 90);
    expect(invented).toBe(true);
    const block = editor.querySelector(`.${PLACED_TEXT_CLASS}`);
    expect(block).toBeTruthy();
    const sel = window.getSelection();
    expect(sel?.rangeCount).toBe(1);
    expect(sel?.isCollapsed).toBe(true);
    expect(block!.contains(sel!.anchorNode!) || sel!.anchorNode === block).toBe(true);

    editor.remove();
  });

  it("selectWordOrTokenAtPoint still selects a whole word in a placed line", () => {
    const editor = makeDocEditor();
    const line = makePlaced(editor, 36, 40, "hello world");
    const text = line.firstChild as Text;
    // Point into "world"
    const range = document.createRange();
    range.setStart(text, 8);
    range.collapse(true);
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);

    // caretRangeFromPoint / elementFromPoint may be stubby in happy-dom —
    // seed via a real range inside the word and assert bounds helper path when possible.
    document.elementFromPoint = () => line;
    // happy-dom may lack caretRangeFromPoint; polyfill from the seeded caret.
    (document as Document & { caretRangeFromPoint?: (x: number, y: number) => Range | null }).caretRangeFromPoint =
      () => {
        const r = document.createRange();
        r.setStart(text, 8);
        r.collapse(true);
        return r;
      };

    const ok = selectWordOrTokenAtPoint(editor, 100, 55);
    expect(ok).toBe(true);
    expect(sel.toString()).toBe("world");

    editor.remove();
  });
});
