/**
 * Click-invent in free space to the right of a table must work.
 * Drag can already park prose there; caretRangeFromPoint / full-width hit boxes
 * must not steal the blank right-side click into left-column text or the table.
 */
import { describe, expect, it } from "vitest";
import {
  ensurePlacedBlockWrapWidth,
  placeDocumentTextAtPoint,
  PLACED_TEXT_CLASS,
} from "./documentCanvas";
import { formatPt, parseCssPt, pxToPt } from "./tableLayout";

function stubEditorRect(editor: HTMLElement): void {
  editor.getBoundingClientRect = () =>
    ({
      x: 0,
      y: 0,
      top: 0,
      left: 0,
      bottom: 500,
      right: 700,
      width: 700,
      height: 500,
      toJSON() {
        return {};
      },
    }) as DOMRect;
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: 700 });
  Object.defineProperty(editor, "clientHeight", { configurable: true, value: 500 });
}

function makeDocEditor(): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
  stubEditorRect(editor);
  document.body.appendChild(editor);
  return editor;
}

function makeAbsoluteTable(
  editor: HTMLElement,
  left: number,
  top: number,
  width = 200,
): HTMLTableElement {
  const table = document.createElement("table");
  table.className = "user";
  table.style.position = "absolute";
  table.style.left = formatPt(left);
  table.style.top = formatPt(top);
  table.style.width = formatPt(width);
  table.innerHTML =
    "<tr style='height:20px'><td style='height:20px'>a</td><td style='height:20px'>b</td></tr>" +
    "<tr style='height:20px'><td style='height:20px'>c</td><td style='height:20px'>d</td></tr>";
  table.getBoundingClientRect = () => {
    const l = (left * 96) / 72;
    const t = (top * 96) / 72;
    const w = (width * 96) / 72;
    return {
      x: l,
      y: t,
      top: t,
      left: l,
      bottom: t + 40,
      right: l + w,
      width: w,
      height: 40,
      toJSON() {
        return {};
      },
    } as DOMRect;
  };
  editor.appendChild(table);
  return table;
}

function makePlaced(
  editor: HTMLElement,
  left: number,
  top: number,
  text: string,
  widthPt: number,
): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(left);
  p.style.top = formatPt(top);
  p.style.width = formatPt(widthPt);
  p.textContent = text;
  const l = (left * 96) / 72;
  const t = (top * 96) / 72;
  const w = (widthPt * 96) / 72;
  p.getBoundingClientRect = () =>
    ({
      x: l,
      y: t,
      top: t,
      left: l,
      bottom: t + 18,
      right: l + w,
      width: w,
      height: 18,
      toJSON() {
        return {};
      },
    }) as DOMRect;
  editor.appendChild(p);
  return p;
}

describe("Document click invent right of table", () => {
  it("clips left-of-table wrap so the hit box does not cover free space to the right", () => {
    const editor = makeDocEditor();
    const tableLeft = 220;
    makeAbsoluteTable(editor, tableLeft, 40, 200);
    const leftText = makePlaced(editor, 12, 40, "Names here:", 400);

    ensurePlacedBlockWrapWidth(editor, leftText);

    const w = parseCssPt(leftText.style.width);
    expect(12 + w).toBeLessThanOrEqual(tableLeft);
    editor.remove();
  });

  it("clips centered/full-width wrap on the same band before the table", () => {
    const editor = makeDocEditor();
    const tableLeft = 220;
    makeAbsoluteTable(editor, tableLeft, 40, 200);
    const full = makePlaced(editor, 12, 40, "Wide line", 500);
    full.dataset.docAlign = "center";
    full.style.textAlign = "center";

    ensurePlacedBlockWrapWidth(editor, full);

    const w = parseCssPt(full.style.width);
    expect(parseCssPt(full.style.left) + w).toBeLessThanOrEqual(tableLeft);
    editor.remove();
  });

  it("invents when click is right of table even if caretRange snaps into left prose", () => {
    const editor = makeDocEditor();
    const tableLeft = 220;
    const tableWidth = 200;
    makeAbsoluteTable(editor, tableLeft, 40, tableWidth);
    const leftText = makePlaced(editor, 12, 40, "Names here:", 180);
    ensurePlacedBlockWrapWidth(editor, leftText);

    const clickX = ((tableLeft + tableWidth + 40) * 96) / 72;
    const clickY = (50 * 96) / 72;

    document.elementFromPoint = () => editor;
    const textNode = leftText.firstChild as Text;
    (document as Document & { caretRangeFromPoint?: (x: number, y: number) => Range | null }).caretRangeFromPoint =
      () => {
        // Browser false-positive: blank right-of-table click lands in left-column text.
        const r = document.createRange();
        r.setStart(textNode, 0);
        r.collapse(true);
        return r;
      };

    const before = editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length;
    expect(placeDocumentTextAtPoint(editor, clickX, clickY)).toBe(true);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(before + 1);

    const invented = Array.from(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`)).find(
      (n) => n !== leftText,
    ) as HTMLElement;
    expect(parseCssPt(invented.style.left)).toBeCloseTo(pxToPt(clickX), 0);

    editor.remove();
  });

  it("invents when click is right of table even if caretRange is editor-root + sibling offset", () => {
    const editor = makeDocEditor();
    const tableLeft = 220;
    const tableWidth = 200;
    makeAbsoluteTable(editor, tableLeft, 40, tableWidth);
    const leftText = makePlaced(editor, 12, 40, "Names here:", 180);
    ensurePlacedBlockWrapWidth(editor, leftText);

    const clickX = ((tableLeft + tableWidth + 60) * 96) / 72;
    const clickY = (55 * 96) / 72;

    document.elementFromPoint = () => editor;
    (document as Document & { caretRangeFromPoint?: (x: number, y: number) => Range | null }).caretRangeFromPoint =
      () => {
        const r = document.createRange();
        const idx = Array.from(editor.childNodes).indexOf(leftText);
        r.setStart(editor, Math.max(0, idx));
        r.collapse(true);
        return r;
      };

    expect(placeDocumentTextAtPoint(editor, clickX, clickY)).toBe(true);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(2);

    editor.remove();
  });

  it("still edits left-of-table text when the click is on that line", () => {
    const editor = makeDocEditor();
    makeAbsoluteTable(editor, 220, 40, 200);
    const leftText = makePlaced(editor, 12, 40, "Names here:", 180);
    ensurePlacedBlockWrapWidth(editor, leftText);

    document.elementFromPoint = () => leftText;
    const textNode = leftText.firstChild as Text;
    (document as Document & { caretRangeFromPoint?: (x: number, y: number) => Range | null }).caretRangeFromPoint =
      () => {
        const r = document.createRange();
        r.setStart(textNode, 2);
        r.collapse(true);
        return r;
      };

    const clickX = ((20 * 96) / 72);
    const clickY = ((48 * 96) / 72);
    expect(placeDocumentTextAtPoint(editor, clickX, clickY)).toBe(false);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(1);

    editor.remove();
  });
});
