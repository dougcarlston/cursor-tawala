/**
 * #6 Centering — Document placed-line paragraph alignment (Center/Left/Right/Justify).
 * Survives wrap-width restore and Return; does not break table cell align (tested elsewhere).
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  alignPlacedTextBlock,
  DOC_INDENT_STEP_PT,
  documentEnterInPlacedText,
  ensurePlacedBlockWrapWidth,
  focusPlacedBlockEnd,
  getDocumentContentMetrics,
  normalizePlacedTextAlign,
  PLACED_TEXT_CLASS,
  readPlacedTextAlign,
  stripLeadingWhitespaceForLeftAlign,
  syncTypingFormatFromCaret,
} from "./documentCanvas";
import { formatPt, parseCssPt } from "./tableLayout";
import { getTypingFormat, setTypingFormat } from "./paletteTypingFormat";

function makeDocEditor(widthPx = 800): HTMLElement {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.style.width = `${widthPx}px`;
  editor.style.height = "600px";
  Object.defineProperty(editor, "clientWidth", { configurable: true, value: widthPx });
  Object.defineProperty(editor, "clientHeight", { configurable: true, value: 600 });
  document.body.appendChild(editor);
  return editor;
}

function makePlaced(editor: HTMLElement, left: number, top: number, html: string): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = formatPt(left);
  p.style.top = formatPt(top);
  p.innerHTML = html;
  editor.appendChild(p);
  return p;
}

afterEach(() => {
  document.body.replaceChildren();
});

describe("Document placed-text alignment (#6)", () => {
  it("Center expands the placed line across the margin box and sets text-align", () => {
    const editor = makeDocEditor();
    const block = makePlaced(editor, 12, 20, "Centered title");
    alignPlacedTextBlock(editor, block, "center");

    expect(readPlacedTextAlign(block)).toBe("center");
    expect(block.style.textAlign).toBe("center");
    expect(block.dataset.docAlign).toBe("center");
    const { marginPt, contentWidthPt } = getDocumentContentMetrics(editor);
    expect(parseCssPt(block.style.left)).toBeCloseTo(marginPt, 0);
    expect(parseCssPt(block.style.width)).toBeCloseTo(contentWidthPt - 2 * marginPt, 0);
  });

  it("style-only Center (remount without dataset) still gets full wrap width", () => {
    const editor = makeDocEditor();
    const block = makePlaced(editor, 12, 20, "Remounted");
    block.style.textAlign = "center";
    // No dataset.docAlign — hydrate from style alone.
    ensurePlacedBlockWrapWidth(editor, block);
    expect(normalizePlacedTextAlign(block)).toBe("center");
    expect(block.dataset.docAlign).toBe("center");
    const { marginPt, contentWidthPt } = getDocumentContentMetrics(editor);
    expect(parseCssPt(block.style.width)).toBeCloseTo(contentWidthPt - 2 * marginPt, 0);
  });

  it("Return on a centered line inherits Center on the new placed block", () => {
    const editor = makeDocEditor();
    const block = makePlaced(editor, 12, 20, "Line one");
    alignPlacedTextBlock(editor, block, "center");
    setTypingFormat(editor, {
      fontFace: "Trebuchet MS",
      fontSize: "20",
      bold: false,
      italic: false,
      underline: false,
    });

    const text = block.firstChild as Text;
    const range = document.createRange();
    range.setStart(text, text.data.length);
    range.collapse(true);
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);

    expect(documentEnterInPlacedText(editor)).toBe(true);
    const lines = Array.from(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`));
    expect(lines.length).toBe(2);
    expect(readPlacedTextAlign(lines[1]!)).toBe("center");
    expect(lines[1]!.style.fontFamily).toContain("Trebuchet");
  });

  it("Left / Right / Justify keep working alongside Center", () => {
    const editor = makeDocEditor();
    const block = makePlaced(editor, 12, 20, "Align me");
    for (const dir of ["right", "justify", "left", "center"] as const) {
      alignPlacedTextBlock(editor, block, dir);
      expect(readPlacedTextAlign(block)).toBe(dir);
      if (dir === "left") expect(block.dataset.docAlign).toBeUndefined();
      else expect(block.dataset.docAlign).toBe(dir);
    }
  });

  it("Align Left strips leading spaces/tabs/nbsp/ZWSP so text is flush left", () => {
    const editor = makeDocEditor();
    const block = makePlaced(editor, 12, 20, "\t  \u00a0\u200bFlush me");
    alignPlacedTextBlock(editor, block, "left");
    expect(block.textContent).toBe("Flush me");
    expect(readPlacedTextAlign(block)).toBe("left");
  });

  it("Align Left strips leading whitespace inside nested spans and after BR", () => {
    const editor = makeDocEditor();
    const block = makePlaced(
      editor,
      12,
      20,
      `<span><i>  Line one</i></span><br>\tLine two`,
    );
    alignPlacedTextBlock(editor, block, "left");
    expect(block.textContent).toBe("Line oneLine two");
    const afterBr = block.querySelector("br")?.nextSibling as Text | null;
    expect(afterBr?.data).toBe("Line two");
  });

  it("Align Left does not strip Indent margin / data-doc-indent", () => {
    const editor = makeDocEditor();
    const block = makePlaced(editor, 12, 20, "  Indented");
    block.dataset.docIndent = "2";
    alignPlacedTextBlock(editor, block, "left");
    expect(block.textContent).toBe("Indented");
    expect(block.dataset.docIndent).toBe("2");
    const { marginPt } = getDocumentContentMetrics(editor);
    expect(parseCssPt(block.style.left)).toBeCloseTo(marginPt + 2 * DOC_INDENT_STEP_PT, 0);
  });

  it("Center / Right leave leading whitespace characters alone", () => {
    const editor = makeDocEditor();
    const block = makePlaced(editor, 12, 20, "  Keep spaces");
    alignPlacedTextBlock(editor, block, "center");
    expect(block.textContent).toBe("  Keep spaces");
    alignPlacedTextBlock(editor, block, "right");
    expect(block.textContent).toBe("  Keep spaces");
  });

  it("stripLeadingWhitespaceForLeftAlign is usable for Form Text blocks", () => {
    const div = document.createElement("div");
    div.innerHTML = `\u00a0\tForm line`;
    document.body.appendChild(div);
    stripLeadingWhitespaceForLeftAlign(div);
    expect(div.textContent).toBe("Form line");
  });
});

describe("Continue after break (#8)", () => {
  it("focusPlacedBlockEnd lands inside the last styled text run (not after the span)", () => {
    const editor = makeDocEditor();
    const block = makePlaced(
      editor,
      12,
      20,
      `<span style="font-family: Trebuchet MS; font-size: 20pt;">Hello world</span>`,
    );
    const range = focusPlacedBlockEnd(block);
    expect(range).toBeTruthy();
    const node = range!.startContainer;
    expect(node.nodeType).toBe(Node.TEXT_NODE);
    expect(node.parentElement?.style.fontFamily).toContain("Trebuchet");
    syncTypingFormatFromCaret(editor);
    expect(getTypingFormat(editor).fontFace).toBe("Trebuchet MS");
    expect(getTypingFormat(editor).fontSize).toBe("20");
  });
});
