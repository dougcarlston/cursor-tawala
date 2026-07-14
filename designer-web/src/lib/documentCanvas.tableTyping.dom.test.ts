/**
 * Absolute Document tables must not trap caret / block inventing prose.
 * caretRangeFromPoint often lands in the table on blank-canvas clicks.
 */
import { describe, expect, it } from "vitest";
import {
  ensureDocumentTableLayout,
  placeDocumentTextAtPoint,
  PLACED_TEXT_CLASS,
  rangeIntersectsExistingDocumentText,
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
  table.style.left = formatPt(12);
  table.style.top = formatPt(40);
  table.style.width = formatPt(200);
  table.innerHTML = "<tr><td>a</td><td>b</td></tr><tr><td>c</td><td>d</td></tr>";
  editor.appendChild(table);
  return table;
}

describe("Document table + prose typing", () => {
  it("range inside a table is not treated as existing prose for place invent", () => {
    const editor = makeDocEditor();
    const table = makeAbsoluteTable(editor);
    const cell = table.querySelector("td")!;
    const range = document.createRange();
    range.selectNodeContents(cell);
    range.collapse(true);
    expect(rangeIntersectsExistingDocumentText(editor, range)).toBe(false);
    editor.remove();
  });

  it("blank-canvas place invents prose even when a table is already present", () => {
    const editor = makeDocEditor();
    makeAbsoluteTable(editor);
    ensureDocumentTableLayout(editor);
    editor.getBoundingClientRect = () =>
      ({
        x: 0,
        y: 0,
        top: 0,
        left: 0,
        bottom: 400,
        right: 600,
        width: 600,
        height: 400,
        toJSON() {
          return {};
        },
      }) as DOMRect;

    // Point far below the table — elementFromPoint may be null in happy-dom;
    // stub so the click is treated as hitting the editor (blank canvas).
    const prev = document.elementFromPoint;
    document.elementFromPoint = () => editor;

    const before = editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length;
    const invented = placeDocumentTextAtPoint(editor, 80, 300);
    document.elementFromPoint = prev;

    expect(invented).toBe(true);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(before + 1);
    editor.remove();
  });

  it("click on a table cell does not invent a placed line", () => {
    const editor = makeDocEditor();
    const table = makeAbsoluteTable(editor);
    const cell = table.querySelector("td")!;
    const prev = document.elementFromPoint;
    document.elementFromPoint = () => cell;

    const before = editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length;
    expect(placeDocumentTextAtPoint(editor, 40, 50)).toBe(false);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(before);

    document.elementFromPoint = prev;
    editor.remove();
  });

  it("click on a table cell focuses a caret inside that cell", () => {
    const editor = makeDocEditor();
    const table = makeAbsoluteTable(editor);
    const cell = table.querySelector("td")!;
    cell.getBoundingClientRect = () =>
      ({
        x: 20,
        y: 40,
        top: 40,
        left: 20,
        bottom: 60,
        right: 80,
        width: 60,
        height: 20,
        toJSON() {
          return {};
        },
      }) as DOMRect;
    table.getBoundingClientRect = () =>
      ({
        x: 12,
        y: 40,
        top: 40,
        left: 12,
        bottom: 80,
        right: 212,
        width: 200,
        height: 40,
        toJSON() {
          return {};
        },
      }) as DOMRect;

    document.elementFromPoint = () => cell;
    (document as Document & { caretRangeFromPoint?: (x: number, y: number) => Range | null }).caretRangeFromPoint =
      () => {
        const r = document.createRange();
        r.selectNodeContents(cell);
        r.collapse(true);
        return r;
      };

    expect(placeDocumentTextAtPoint(editor, 40, 50)).toBe(false);
    const sel = window.getSelection();
    expect(sel?.rangeCount).toBe(1);
    expect(cell.contains(sel!.anchorNode!) || sel!.anchorNode === cell).toBe(true);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(0);

    editor.remove();
  });

  it("click inside existing placed text edits that line and does not invent", () => {
    const editor = makeDocEditor();
    const line = document.createElement("p");
    line.className = PLACED_TEXT_CLASS;
    line.style.position = "absolute";
    line.style.left = "36pt";
    line.style.top = "40pt";
    line.textContent = "hello world";
    editor.appendChild(line);

    document.elementFromPoint = () => line;
    const text = line.firstChild as Text;
    (document as Document & { caretRangeFromPoint?: (x: number, y: number) => Range | null }).caretRangeFromPoint =
      () => {
        const r = document.createRange();
        r.setStart(text, 5);
        r.collapse(true);
        return r;
      };

    expect(placeDocumentTextAtPoint(editor, 80, 55)).toBe(false);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(1);
    const sel = window.getSelection();
    expect(sel?.anchorNode).toBe(text);
    expect(sel?.anchorOffset).toBe(5);

    editor.remove();
  });
});
