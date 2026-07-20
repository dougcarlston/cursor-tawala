/**
 * Multi-paragraph highlight drag: shared delta + consume blank husks on drop.
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  DOC_BLANK_ATTR,
  expandRangeToIncludePlacedBlocks,
  finalizePlacedBlocksMove,
  listPlacedBlocksForHighlightMove,
  listPlacedBlocksInSelection,
  movePlacedBlocksByDelta,
  PLACED_TEXT_CLASS,
  selectionFullyCoversPlacedBlock,
} from "./documentCanvas";
import { DOC_HOME_TOP_ATTR, formatPt } from "./tableLayout";

function makeDocEditor(): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: 600 });
  document.body.appendChild(editor);
  return editor;
}

function makePlaced(editor: HTMLElement, text: string, top: number): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(36);
  p.style.top = formatPt(top);
  p.style.width = formatPt(200);
  p.textContent = text;
  editor.appendChild(p);
  return p;
}

describe("listPlacedBlocksInSelection (multi-paragraph drag)", () => {
  it("returns every placed line intersecting a cross-block selection", () => {
    const editor = makeDocEditor();
    const a = makePlaced(editor, "First", 40);
    const b = makePlaced(editor, "Second", 60);
    const c = makePlaced(editor, "Third", 80);

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(a.firstChild as Text, 0);
    range.setEnd(c.firstChild as Text, 5);
    sel?.removeAllRanges();
    sel?.addRange(range);

    const hit = listPlacedBlocksInSelection(editor);
    expect(hit).toEqual([a, b, c]);

    editor.remove();
  });

  it("returns a single line for a collapsed caret", () => {
    const editor = makeDocEditor();
    const a = makePlaced(editor, "Only", 40);
    makePlaced(editor, "Other", 60);

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(a.firstChild as Text, 2);
    range.collapse(true);
    sel?.removeAllRanges();
    sel?.addRange(range);

    expect(listPlacedBlocksInSelection(editor)).toEqual([a]);

    editor.remove();
  });

  it("includes a visually middle line that is later in the DOM", () => {
    const editor = makeDocEditor();
    // DOM order: Choices, a), b) — then append "Many" with a top between Choices and a).
    const choices = makePlaced(editor, "Choices for a Dog name:", 40);
    const aLine = makePlaced(editor, "a) Old fashioned names", 80);
    const bLine = makePlaced(editor, "b) Foreign Names", 100);
    const many = makePlaced(editor, "Many of these types are:", 60);

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(choices.firstChild as Text, 0);
    range.setEnd(bLine.firstChild as Text, 5);
    sel?.removeAllRanges();
    sel?.addRange(range);

    // Bare intersectsNode misses `many` (after b in DOM). Fill must still return it.
    expect(listPlacedBlocksInSelection(editor)).toEqual([choices, many, aLine, bLine]);

    editor.remove();
  });
});

describe("listPlacedBlocksForHighlightMove (word vs whole line)", () => {
  it("does not move a line when only a word is selected", () => {
    const editor = makeDocEditor();
    const line = makePlaced(editor, "Old fashioned names", 40);
    const text = line.firstChild as Text;

    const sel = window.getSelection();
    const range = document.createRange();
    // Select only "fashioned"
    range.setStart(text, 4);
    range.setEnd(text, 13);
    sel?.removeAllRanges();
    sel?.addRange(range);

    expect(selectionFullyCoversPlacedBlock(line)).toBe(false);
    expect(listPlacedBlocksForHighlightMove(editor)).toEqual([]);
    // Still reports the intersecting line for format tools, etc.
    expect(listPlacedBlocksInSelection(editor)).toEqual([line]);

    editor.remove();
  });

  it("moves a single line when the whole line is selected", () => {
    const editor = makeDocEditor();
    const line = makePlaced(editor, "Old fashioned names", 40);

    const sel = window.getSelection();
    const range = document.createRange();
    range.selectNodeContents(line);
    sel?.removeAllRanges();
    sel?.addRange(range);

    expect(selectionFullyCoversPlacedBlock(line)).toBe(true);
    expect(listPlacedBlocksForHighlightMove(editor)).toEqual([line]);

    editor.remove();
  });

  it("still moves multiple lines for a cross-block selection", () => {
    const editor = makeDocEditor();
    const a = makePlaced(editor, "First", 40);
    const b = makePlaced(editor, "Second", 60);

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(a.firstChild as Text, 0);
    range.setEnd(b.firstChild as Text, 6);
    sel?.removeAllRanges();
    sel?.addRange(range);

    expect(listPlacedBlocksForHighlightMove(editor)).toEqual([a, b]);

    editor.remove();
  });
});

describe("expandRangeToIncludePlacedBlocks", () => {
  it("extends the range end so a DOM-late middle line intersects", () => {
    const editor = makeDocEditor();
    const choices = makePlaced(editor, "Choices", 40);
    const aLine = makePlaced(editor, "a) names", 80);
    const many = makePlaced(editor, "Many types", 60);

    const range = document.createRange();
    range.setStart(choices.firstChild as Text, 0);
    range.setEnd(aLine.firstChild as Text, 3);
    expect(range.intersectsNode(many)).toBe(false);

    const expanded = expandRangeToIncludePlacedBlocks(range, [choices, many, aLine]);
    expect(expanded.intersectsNode(many)).toBe(true);

    editor.remove();
  });
});

describe("movePlacedBlocksByDelta", () => {
  it("applies the same delta to every origin", () => {
    const editor = makeDocEditor();
    const a = makePlaced(editor, "A", 40);
    const b = makePlaced(editor, "B", 80);
    const origins = [
      { left: 36, top: 40 },
      { left: 36, top: 80 },
    ];
    movePlacedBlocksByDelta([a, b], origins, 10, -5);
    expect(a.style.left).toBe("46pt");
    expect(a.style.top).toBe("35pt");
    expect(b.style.left).toBe("46pt");
    expect(b.style.top).toBe("75pt");
    editor.remove();
  });
});

describe("finalizePlacedBlocksMove (drag into blank)", () => {
  it("removes a same-column blank husk the moved line now occupies", () => {
    const editor = makeDocEditor();
    const blank = makePlaced(editor, "", 40);
    blank.setAttribute(DOC_BLANK_ATTR, "1");
    blank.innerHTML = "<br>";
    const line = makePlaced(editor, "c) Useless Names", 60);

    movePlacedBlocksByDelta([line], [{ left: 36, top: 60 }], 0, -20);
    finalizePlacedBlocksMove(editor, [line]);

    expect(editor.contains(blank)).toBe(false);
    expect(line.style.top).toBe("40pt");
    expect(line.getAttribute(DOC_HOME_TOP_ATTR)).toBe("40pt");
    editor.remove();
  });

  it("keeps a blank that does not overlap the moved line", () => {
    const editor = makeDocEditor();
    const blank = makePlaced(editor, "", 40);
    blank.setAttribute(DOC_BLANK_ATTR, "1");
    blank.innerHTML = "<br>";
    const line = makePlaced(editor, "Still below", 100);

    movePlacedBlocksByDelta([line], [{ left: 36, top: 100 }], 0, -10);
    finalizePlacedBlocksMove(editor, [line]);

    expect(editor.contains(blank)).toBe(true);
    expect(line.style.top).toBe("90pt");
    editor.remove();
  });

  it("does not remove a blank in a different horizontal column", () => {
    const editor = makeDocEditor();
    const blank = makePlaced(editor, "", 40);
    blank.setAttribute(DOC_BLANK_ATTR, "1");
    blank.innerHTML = "<br>";
    blank.style.left = formatPt(300);
    blank.style.width = formatPt(120);
    const line = makePlaced(editor, "Left column", 60);
    line.style.width = formatPt(120);

    movePlacedBlocksByDelta([line], [{ left: 36, top: 60 }], 0, -20);
    finalizePlacedBlocksMove(editor, [line]);

    expect(editor.contains(blank)).toBe(true);
    editor.remove();
  });
});
