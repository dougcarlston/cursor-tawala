/**
 * Document delete boundaries:
 * - Backspace/Delete beside a table must not hop into a cell.
 * - Same-column placed lines merge across Returns (caret epic A).
 */
import { describe, expect, it } from "vitest";
import {
  clampDocumentSelectionToLayoutIsland,
  handleDocumentDeleteBoundary,
  handlePlacedTextArrowKey,
  PLACED_TEXT_CLASS,
} from "./documentCanvas";
import { formatPt } from "./tableLayout";

function makeDocEditor(): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
  // Realistic width so wrap/reflow does not collapse lines to 1pt (jsdom default).
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: 600 });
  document.body.appendChild(editor);
  return editor;
}

function makeAbsoluteTable(editor: HTMLElement): HTMLTableElement {
  const table = document.createElement("table");
  table.className = "user";
  table.style.position = "absolute";
  table.style.left = formatPt(100);
  table.style.top = formatPt(40);
  table.innerHTML =
    "<tr><th>N</th><th>E</th><th>P</th></tr>" +
    "<tr><td>&nbsp;</td><td>&nbsp;</td><td id='br'>&nbsp;</td></tr>";
  editor.appendChild(table);
  return table;
}

function makePlaced(
  editor: HTMLElement,
  text: string,
  opts?: { left?: number; top?: number; face?: string },
): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(opts?.left ?? 340);
  p.style.top = formatPt(opts?.top ?? 160);
  p.style.fontFamily = opts?.face ?? "Times New Roman";
  // Narrow box so L/R columns do not falsely "overlap" for merge tests.
  p.style.width = formatPt(80);
  if (text) p.textContent = text;
  else p.innerHTML = "<br>";
  editor.appendChild(p);
  return p;
}

function caretAtStart(block: HTMLElement): void {
  const sel = window.getSelection();
  const range = document.createRange();
  if (block.firstChild) {
    range.setStart(block.firstChild, 0);
  } else {
    range.selectNodeContents(block);
  }
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

describe("Document delete boundary (placed ↔ table)", () => {
  it("blocks Backspace at start of right-of-table invent so text does not hop into the last cell", () => {
    const editor = makeDocEditor();
    makeAbsoluteTable(editor);
    // Sole placed line right of the table (no same-column neighbor to merge into).
    const test = makePlaced(editor, "Test", { left: 340, top: 160 });

    caretAtStart(test);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);

    expect(editor.contains(test)).toBe(true);
    expect(test.textContent).toBe("Test");
    expect(test.style.fontFamily).toMatch(/Times/i);
    const br = editor.querySelector("#br") as HTMLElement;
    expect(br.textContent?.replace(/\u00a0/g, "").trim()).toBe("");

    editor.remove();
  });

  it("does not merge side-by-side columns on Backspace (left stays left)", () => {
    const editor = makeDocEditor();
    const left = makePlaced(editor, "Left", { left: 20, top: 40 });
    const right = makePlaced(editor, "Right", { left: 340, top: 40 });

    caretAtStart(right);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);
    expect(editor.contains(left)).toBe(true);
    expect(editor.contains(right)).toBe(true);
    expect(left.textContent).toBe("Left");
    expect(right.textContent).toBe("Right");

    editor.remove();
  });

  it("removes an empty invent husk on Backspace without merging into the table", () => {
    const editor = makeDocEditor();
    makeAbsoluteTable(editor);
    const husk = makePlaced(editor, "", { left: 340, top: 160 });
    caretAtStart(husk);

    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);
    expect(editor.contains(husk)).toBe(false);
    const br = editor.querySelector("#br") as HTMLElement;
    expect(br.textContent?.replace(/\u00a0/g, "").trim()).toBe("");

    editor.remove();
  });

  it("still allows Backspace to delete characters inside a placed line", () => {
    const editor = makeDocEditor();
    makeAbsoluteTable(editor);
    const test = makePlaced(editor, "Test");
    caretAtEnd(test);

    // Not at the layout edge → do not preventDefault; native delete keeps working.
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(false);
    expect(test.textContent).toBe("Test");
    expect(editor.querySelector("#br")?.textContent?.replace(/\u00a0/g, "").trim()).toBe("");

    editor.remove();
  });

  it("clamps a selection that spans a placed line and a table cell", () => {
    const editor = makeDocEditor();
    const table = makeAbsoluteTable(editor);
    const test = makePlaced(editor, "Test");
    const br = table.querySelector("#br") as HTMLElement;

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(br, 0);
    range.setEnd(test.firstChild as Text, 4);
    sel?.removeAllRanges();
    sel?.addRange(range);

    expect(clampDocumentSelectionToLayoutIsland(editor)).toBe(true);
    const after = sel!.getRangeAt(0);
    const startInTable = !!(
      after.startContainer instanceof Node &&
      (after.startContainer.nodeType === Node.TEXT_NODE
        ? after.startContainer.parentElement
        : (after.startContainer as Element)
      )?.closest?.("td, th")
    );
    const endInTable = !!(
      after.endContainer instanceof Node &&
      (after.endContainer.nodeType === Node.TEXT_NODE
        ? after.endContainer.parentElement
        : (after.endContainer as Element)
      )?.closest?.("td, th")
    );
    const startInPlaced = !!(
      after.startContainer instanceof Node &&
      (after.startContainer.nodeType === Node.TEXT_NODE
        ? after.startContainer.parentElement
        : (after.startContainer as Element)
      )?.closest?.(`.${PLACED_TEXT_CLASS}`)
    );
    const endInPlaced = !!(
      after.endContainer instanceof Node &&
      (after.endContainer.nodeType === Node.TEXT_NODE
        ? after.endContainer.parentElement
        : (after.endContainer as Element)
      )?.closest?.(`.${PLACED_TEXT_CLASS}`)
    );
    // Both ends must share one island (not table+placed).
    expect(startInTable === endInTable || startInPlaced === endInPlaced).toBe(true);
    expect(startInTable && endInPlaced).toBe(false);
    expect(startInPlaced && endInTable).toBe(false);

    editor.remove();
  });

  it("does not block Backspace inside a table cell with content", () => {
    const editor = makeDocEditor();
    const table = makeAbsoluteTable(editor);
    const cell = table.querySelector("#br") as HTMLElement;
    cell.textContent = "Cell";
    caretAtEnd(cell);

    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(false);
    editor.remove();
  });
});

describe("Document cross-Return Backspace / Delete (caret epic A)", () => {
  it("Backspace at start of next line merges into the previous same-column line", () => {
    const editor = makeDocEditor();
    const first = makePlaced(editor, "Hello", { left: 36, top: 40 });
    const second = makePlaced(editor, "World", { left: 36, top: 60 });

    caretAtStart(second);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);

    expect(editor.contains(second)).toBe(false);
    expect(editor.contains(first)).toBe(true);
    expect(first.textContent).toBe("HelloWorld");
    const sel = window.getSelection();
    expect(sel?.isCollapsed).toBe(true);
    expect(first.contains(sel!.anchorNode!)).toBe(true);

    editor.remove();
  });

  it("Backspace on an empty Return line removes it and lands on the previous line", () => {
    const editor = makeDocEditor();
    const first = makePlaced(editor, "Hello", { left: 36, top: 40 });
    const blank = makePlaced(editor, "", { left: 36, top: 60 });

    caretAtStart(blank);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);

    expect(editor.contains(blank)).toBe(false);
    expect(first.textContent).toBe("Hello");
    expect(first.contains(window.getSelection()!.anchorNode!)).toBe(true);

    editor.remove();
  });

  it("Delete at end of a line merges the next same-column line", () => {
    const editor = makeDocEditor();
    const first = makePlaced(editor, "Hello", { left: 36, top: 40 });
    const second = makePlaced(editor, "World", { left: 36, top: 60 });

    caretAtEnd(first);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentForward")).toBe(true);

    expect(editor.contains(second)).toBe(false);
    expect(first.textContent).toBe("HelloWorld");

    editor.remove();
  });

  it("repeated Backspace can clear the document one line at a time", () => {
    const editor = makeDocEditor();
    const a = makePlaced(editor, "A", { left: 36, top: 40 });
    const b = makePlaced(editor, "B", { left: 36, top: 60 });
    const c = makePlaced(editor, "C", { left: 36, top: 80 });

    caretAtStart(c);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(2);
    expect(b.textContent).toBe("BC");

    // Re-query after reflow — do not assume the same node is still the lower line.
    const lines = Array.from(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`)) as HTMLElement[];
    expect(lines.map((el) => el.textContent)).toEqual(["A", "BC"]);
    const lower = lines[1];
    caretAtStart(lower);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(1);
    expect(a.textContent).toBe("ABC");

    // Empty the merged line via native deletes (not boundary); then husk Backspace.
    a.textContent = "";
    a.innerHTML = "<br>";
    caretAtStart(a);
    handleDocumentDeleteBoundary(editor, "deleteContentBackward");
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(0);

    editor.remove();
  });
});

describe("Document Backspace on function chips (caret restore)", () => {
  it("Backspace after a trailing function chip removes it and leaves a live caret", () => {
    const editor = makeDocEditor();
    const line = makePlaced(editor, "", { left: 36, top: 40 });
    const token = document.createElement("span");
    token.className = "function-token";
    token.setAttribute("contenteditable", "false");
    token.textContent = "<<RANKED RESPONSE NAME(3, Form 1:MCQ1)>>";
    const pad = document.createTextNode("\u200b");
    line.append(token, pad);

    // Caret in the trailing ZWSP landing (after the chip).
    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(pad, 1);
    range.collapse(true);
    sel?.removeAllRanges();
    sel?.addRange(range);

    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);
    expect(line.querySelector(".function-token")).toBeNull();
    expect(sel?.isCollapsed).toBe(true);
    expect(sel?.rangeCount).toBe(1);
    expect(line.contains(sel!.anchorNode!)).toBe(true);
    // ArrowLeft must still resolve a placed line at the caret.
    expect(handlePlacedTextArrowKey(editor, "ArrowLeft")).toBe(true);

    editor.remove();
  });

  it("Backspace after deleting a chip-only line focuses the previous chip line with a caret", () => {
    const editor = makeDocEditor();
    const first = makePlaced(editor, "", { left: 36, top: 40 });
    const t1 = document.createElement("span");
    t1.className = "function-token";
    t1.setAttribute("contenteditable", "false");
    t1.textContent = "<<NAME(2)>>";
    first.append(t1);

    const second = makePlaced(editor, "", { left: 36, top: 60 });
    const t2 = document.createElement("span");
    t2.className = "function-token";
    t2.setAttribute("contenteditable", "false");
    t2.textContent = "<<NAME(3)>>";
    const pad = document.createTextNode("\u200b");
    second.append(t2, pad);

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(pad, 1);
    range.collapse(true);
    sel?.removeAllRanges();
    sel?.addRange(range);

    handleDocumentDeleteBoundary(editor, "deleteContentBackward");
    // Chip gone; blank husk may remain — either way caret must stay arrowable.
    expect(second.querySelector(".function-token")).toBeNull();
    expect(sel?.rangeCount).toBe(1);
    expect(sel?.isCollapsed).toBe(true);
    expect(handlePlacedTextArrowKey(editor, "ArrowLeft")).toBe(true);

    editor.remove();
  });
});
