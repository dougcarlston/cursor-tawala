/**
 * Backspace at the start of a placed line beside a table must not merge that
 * line into the last table cell (Chromium contenteditable island join).
 */
import { describe, expect, it } from "vitest";
import {
  clampDocumentSelectionToLayoutIsland,
  handleDocumentDeleteBoundary,
  PLACED_TEXT_CLASS,
} from "./documentCanvas";
import { formatPt } from "./tableLayout";

function makeDocEditor(): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
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
  face = "Times New Roman",
): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(340);
  p.style.top = formatPt(160);
  p.style.fontFamily = face;
  p.textContent = text;
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
    const right = makePlaced(editor, "Right side");
    right.style.top = formatPt(40);
    // Ensure Test’s previousElementSibling is the table (append order).
    const test = makePlaced(editor, "Test");
    // Move right before the table so DOM order matches: right, table, test
    editor.insertBefore(right, editor.querySelector("table.user"));
    editor.appendChild(test);

    caretAtStart(test);
    expect(handleDocumentDeleteBoundary(editor, "deleteContentBackward")).toBe(true);

    expect(editor.contains(test)).toBe(true);
    expect(test.textContent).toBe("Test");
    expect(test.style.fontFamily).toMatch(/Times/i);
    expect(right.style.fontFamily).toMatch(/Times/i);
    const br = editor.querySelector("#br") as HTMLElement;
    expect(br.textContent?.replace(/\u00a0/g, "").trim()).toBe("");

    editor.remove();
  });

  it("removes an empty invent husk on Backspace without merging into the table", () => {
    const editor = makeDocEditor();
    makeAbsoluteTable(editor);
    const husk = makePlaced(editor, "");
    husk.innerHTML = "<br>";
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
