/**
 * Document caret epic C — arrow keys across same-column Returns and over chips.
 */
import { describe, expect, it } from "vitest";
import {
  handlePlacedTextArrowKey,
  PLACED_TEXT_CLASS,
} from "./documentCanvas";
import { FIELD_TOKEN_CLASS } from "./fieldTokens";
import { FUNCTION_TOKEN_CLASS } from "./functionTokens";
import { CARET_ZWSP } from "./tokenCaretLanding";
import { formatPt } from "./tableLayout";

function makeDocEditor(): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: 600 });
  document.body.appendChild(editor);
  return editor;
}

function makePlaced(
  editor: HTMLElement,
  text: string,
  opts?: { left?: number; top?: number },
): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(opts?.left ?? 36);
  p.style.top = formatPt(opts?.top ?? 40);
  p.style.width = formatPt(200);
  p.textContent = text;
  editor.appendChild(p);
  return p;
}

function caretAtStart(block: HTMLElement): void {
  const sel = window.getSelection();
  const range = document.createRange();
  if (block.firstChild) range.setStart(block.firstChild, 0);
  else range.selectNodeContents(block);
  range.collapse(true);
  sel?.removeAllRanges();
  sel?.addRange(range);
}

function caretAtEnd(block: HTMLElement): void {
  const sel = window.getSelection();
  const range = document.createRange();
  range.selectNodeContents(block);
  range.collapse(false);
  sel?.removeAllRanges();
  sel?.addRange(range);
}

function caretIn(node: Node, offset: number): void {
  const sel = window.getSelection();
  const range = document.createRange();
  range.setStart(node, offset);
  range.collapse(true);
  sel?.removeAllRanges();
  sel?.addRange(range);
}

describe("Document arrow keys across Returns (caret epic C)", () => {
  it("ArrowRight at end of a line moves to the start of the next same-column line", () => {
    const editor = makeDocEditor();
    const first = makePlaced(editor, "Hello", { top: 40 });
    const second = makePlaced(editor, "World", { top: 60 });

    caretAtEnd(first);
    expect(handlePlacedTextArrowKey(editor, "ArrowRight")).toBe(true);
    expect(second.contains(window.getSelection()!.anchorNode!)).toBe(true);

    editor.remove();
  });

  it("ArrowLeft at start of a line moves to the end of the previous same-column line", () => {
    const editor = makeDocEditor();
    const first = makePlaced(editor, "Hello", { top: 40 });
    const second = makePlaced(editor, "World", { top: 60 });

    caretAtStart(second);
    expect(handlePlacedTextArrowKey(editor, "ArrowLeft")).toBe(true);
    expect(first.contains(window.getSelection()!.anchorNode!)).toBe(true);

    editor.remove();
  });

  it("does not jump Left/Right into a side-by-side column", () => {
    const editor = makeDocEditor();
    const left = makePlaced(editor, "Left", { left: 20, top: 40 });
    left.style.width = formatPt(80);
    const right = makePlaced(editor, "Right", { left: 340, top: 40 });
    right.style.width = formatPt(80);

    caretAtStart(right);
    expect(handlePlacedTextArrowKey(editor, "ArrowLeft")).toBe(true);
    expect(right.contains(window.getSelection()!.anchorNode!)).toBe(true);
    expect(left.contains(window.getSelection()!.anchorNode!)).toBe(false);

    editor.remove();
  });
});

describe("Document arrow keys over chips (caret epic C)", () => {
  it("ArrowRight jumps over a field token to the landing after it", () => {
    const editor = makeDocEditor();
    const line = makePlaced(editor, "", { top: 40 });
    const before = document.createTextNode("Hi");
    const token = document.createElement("span");
    token.className = FIELD_TOKEN_CLASS;
    token.setAttribute("contenteditable", "false");
    token.textContent = "<<Name>>";
    const after = document.createTextNode(`${CARET_ZWSP}there`);
    line.append(before, token, after);

    caretIn(before, before.data.length);
    expect(handlePlacedTextArrowKey(editor, "ArrowRight")).toBe(true);
    const sel = window.getSelection()!;
    expect(sel.anchorNode).toBe(after);
    // Landing sits at start of post-token text (ZWSP offset handled by placeCaretAfterToken).
    expect(after.contains(sel.anchorNode!) || sel.anchorNode === after).toBe(true);

    editor.remove();
  });

  it("ArrowLeft jumps over a function token to the landing before it", () => {
    const editor = makeDocEditor();
    const line = makePlaced(editor, "", { top: 40 });
    const before = document.createTextNode(`Hi${CARET_ZWSP}`);
    const token = document.createElement("span");
    token.className = FUNCTION_TOKEN_CLASS;
    token.setAttribute("contenteditable", "false");
    token.textContent = "<<SUM>>";
    const after = document.createTextNode("there");
    line.append(before, token, after);

    caretIn(after, 0);
    expect(handlePlacedTextArrowKey(editor, "ArrowLeft")).toBe(true);
    const sel = window.getSelection()!;
    expect(before.contains(sel.anchorNode!) || sel.anchorNode === before).toBe(true);

    editor.remove();
  });
});
