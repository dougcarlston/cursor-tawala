/**
 * @vitest-environment happy-dom
 *
 * Owner SS1→SS4 / Jul 14 retest: paragraph with field chips → Face → Size must keep
 * the full highlight (including chips), never leave mid-word orphan spans, and paint
 * chip face/size. Harness mirrors `.doc-placed-text` + CE=false + ZWSP pads.
 *
 * Both Face and Size use controlled leaf painting — never execCommand fontName/fontSize
 * on highlights spanning chips.
 */
import { beforeEach, describe, expect, it } from "vitest";
import {
  applyFontFaceAcrossRange,
  applyFontSizePtAcrossRange,
  paletteFontFace,
  paletteFontSize,
} from "@/lib/paletteCommands";
import { setActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { CARET_ZWSP, ensureTokenCaretLanding } from "@/lib/tokenCaretLanding";
import {
  bookmarkTextOffsets,
  expandRangeToTouchedTokens,
  rangeFromTextOffsets,
  textBetweenOffsets,
} from "@/lib/selectionBookmark";
import { PLACED_TEXT_CLASS } from "@/lib/documentCanvas";

function chip(name: string): HTMLSpanElement {
  const span = document.createElement("span");
  span.className = "field-token function-table-token";
  span.setAttribute("contenteditable", "false");
  span.setAttribute("data-field-name", name);
  span.textContent = `<<${name}>>`;
  return span;
}

/** Build the owner field paragraph with ZWSP pads around each chip (3 chips). */
function buildFieldParagraph(): { editor: HTMLElement; block: HTMLElement } {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
  document.body.appendChild(editor);

  const block = document.createElement("p");
  block.className = PLACED_TEXT_CLASS;
  block.style.fontSize = "18pt";
  block.style.fontFamily = "Arial";
  block.style.width = "360px";
  block.style.whiteSpace = "normal";
  block.style.overflowWrap = "break-word";

  const appendText = (t: string) => block.appendChild(document.createTextNode(t));

  appendText("Here is one with some fields in it. ");
  block.appendChild(document.createTextNode(CARET_ZWSP));
  block.appendChild(chip("FIB1:a"));
  block.appendChild(document.createTextNode(CARET_ZWSP));
  appendText(" should be name, while ");
  block.appendChild(document.createTextNode(CARET_ZWSP));
  block.appendChild(chip("FIB1:c"));
  block.appendChild(document.createTextNode(CARET_ZWSP));
  appendText(" and ");
  block.appendChild(document.createTextNode(CARET_ZWSP));
  block.appendChild(chip("FIB1:b"));
  block.appendChild(document.createTextNode(CARET_ZWSP));
  appendText(" are contact fields.");

  editor.appendChild(block);
  return { editor, block };
}

/** Select from start of "some" through end of "while" (partial span, one chip). */
function selectOwnerPartialSpan(block: HTMLElement): Range {
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let startNode: Text | null = null;
  let startOff = 0;
  let endNode: Text | null = null;
  let endOff = 0;
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (text.parentElement?.closest(".field-token")) continue;
    const idxSome = text.data.indexOf("some");
    if (idxSome >= 0 && !startNode) {
      startNode = text;
      startOff = idxSome;
    }
    const idxWhile = text.data.indexOf("while");
    if (idxWhile >= 0) {
      endNode = text;
      endOff = idxWhile + "while".length;
    }
  }
  if (!startNode || !endNode) {
    throw new Error("failed to locate owner selection anchors");
  }
  const range = document.createRange();
  range.setStart(startNode, startOff);
  range.setEnd(endNode, endOff);
  const sel = window.getSelection();
  sel?.removeAllRanges();
  sel?.addRange(range);
  return range;
}

/**
 * happy-dom has no execCommand. Highlight Face/Size must not call fontName/fontSize —
 * fail loudly if they do.
 */
function installExecCommandPolyfill(): void {
  document.execCommand = ((commandId: string) => {
    if (commandId === "styleWithCSS") return true;
    if (commandId === "fontName" || commandId === "fontSize") {
      throw new Error(`execCommand(${commandId}) must not run on highlight Face/Size path`);
    }
    return false;
  }) as typeof document.execCommand;
}

function registerEditor(el: HTMLElement): void {
  let saved: Range | null = null;
  setActivePaletteEditor({
    el,
    commit: () => undefined,
    saveSelection: () => {
      const sel = window.getSelection();
      if (sel && sel.rangeCount > 0) saved = sel.getRangeAt(0).cloneRange();
    },
    restoreSelection: () => {
      if (!saved) return;
      const sel = window.getSelection();
      if (!sel) return;
      sel.removeAllRanges();
      try {
        sel.addRange(saved);
      } catch {
        /* ignore */
      }
    },
  });
}

function fontFamilyOf(node: Node): string {
  let el: HTMLElement | null =
    node.nodeType === Node.TEXT_NODE
      ? (node as Text).parentElement
      : node instanceof HTMLElement
        ? node
        : null;
  while (el) {
    const face = el.style.fontFamily || el.getAttribute("face") || "";
    if (face) return face.toLowerCase();
    el = el.parentElement;
  }
  return "";
}

function fontSizeOf(node: Node): string {
  let el: HTMLElement | null =
    node.nodeType === Node.TEXT_NODE
      ? (node as Text).parentElement
      : node instanceof HTMLElement
        ? node
        : null;
  while (el) {
    if (el.style.fontSize) return el.style.fontSize.toLowerCase();
    el = el.parentElement;
  }
  return "";
}

function assertNoMidWordOrphans(block: HTMLElement, intended: string): void {
  // Edges of the intended span must not leave unstyled mid-word leftovers like
  // "wi"/"th" of "with" or "w"/"hile" of "while" (owner SS4).
  expect(intended.startsWith("some")).toBe(true);
  expect(intended.endsWith("while")).toBe(true);

  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const t = node as Text;
    if (t.parentElement?.closest(".field-token")) continue;
    if (!t.data || t.data === CARET_ZWSP) continue;
    // Classic SS4 chop residues
    expect(t.data === "th" || t.data === "wi" || t.data === "w" || t.data === "hile").toBe(
      false,
    );
  }
}

describe("Face then Size with 3 field chips (SS1→SS4)", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
    installExecCommandPolyfill();
  });

  it("expandRangeToTouchedTokens pulls whole chip + pads into a text-only endpoint range", () => {
    const { editor, block } = buildFieldParagraph();
    // Select text before chip ending at the pre-chip ZWSP (chip not in range endpoints).
    const before = [...block.childNodes].find(
      (n) => n.nodeType === 3 && (n as Text).data.includes("some"),
    ) as Text;
    const pad = before.nextSibling as Text;
    expect(pad.data).toBe(CARET_ZWSP);
    const range = document.createRange();
    range.setStart(before, before.data.indexOf("some"));
    range.setEnd(pad, 1);
    expandRangeToTouchedTokens(editor, range);
    const bm = bookmarkTextOffsets(editor, range);
    expect(bm.text).toContain("<<FIB1:a>>");
    expect(bm.text.startsWith("some")).toBe(true);
  });

  it("applyFontFace then applyFontSize keeps face on wrappers and sizes the full bookmark", () => {
    const { editor, block } = buildFieldParagraph();
    const range = selectOwnerPartialSpan(block);
    expandRangeToTouchedTokens(editor, range);
    const bookmark = bookmarkTextOffsets(editor, range);
    expect(bookmark.text.startsWith("some")).toBe(true);
    expect(bookmark.text).toContain("<<FIB1:a>>");
    expect(bookmark.text.endsWith("while")).toBe(true);

    const faced = applyFontFaceAcrossRange(editor, range, "Times New Roman");
    expect(faced.length).toBeGreaterThan(0);

    const afterFace =
      rangeFromTextOffsets(editor, bookmark.start, bookmark.end) ?? range;

    const sized = applyFontSizePtAcrossRange(editor, afterFace, "26");
    expect(sized.length).toBeGreaterThan(0);

    expect(textBetweenOffsets(editor, bookmark.start, bookmark.end)).toBe(bookmark.text);

    const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
    let someNode: Text | null = null;
    let whileNode: Text | null = null;
    let n: Node | null;
    while ((n = walker.nextNode())) {
      const t = n as Text;
      if (t.parentElement?.closest(".field-token")) continue;
      if (t.data.includes("some") && !someNode) someNode = t;
      if (t.data.includes("while")) whileNode = t;
    }
    expect(someNode).not.toBeNull();
    expect(whileNode).not.toBeNull();
    expect(fontSizeOf(someNode!).includes("26")).toBe(true);
    expect(fontSizeOf(whileNode!).includes("26")).toBe(true);
    expect(fontFamilyOf(someNode!)).toMatch(/times/i);
    expect(fontFamilyOf(whileNode!)).toMatch(/times/i);
  });

  it("paletteFontFace then paletteFontSize keeps selection text and sizes edges (3 chips)", () => {
    const { editor, block } = buildFieldParagraph();
    registerEditor(editor);
    editor.focus();
    const range = selectOwnerPartialSpan(block);
    expandRangeToTouchedTokens(editor, range);
    const bookmark = bookmarkTextOffsets(editor, range);
    const intended = bookmark.text;

    paletteFontFace("Times New Roman");

    const afterFaceSel = window.getSelection();
    expect(afterFaceSel && afterFaceSel.rangeCount > 0 && !afterFaceSel.getRangeAt(0).collapsed).toBe(
      true,
    );
    const afterFaceBm = bookmarkTextOffsets(editor, afterFaceSel!.getRangeAt(0));
    expect(afterFaceBm.text).toBe(intended);

    paletteFontSize("26");

    const afterSizeSel = window.getSelection();
    expect(afterSizeSel && afterSizeSel.rangeCount > 0 && !afterSizeSel.getRangeAt(0).collapsed).toBe(
      true,
    );
    const afterSizeBm = bookmarkTextOffsets(editor, afterSizeSel!.getRangeAt(0));
    // Must not shrink mid-word (SS4: "th" … "w" of while).
    expect(afterSizeBm.text).toBe(intended);
    expect(afterSizeBm.text.startsWith("some")).toBe(true);
    expect(afterSizeBm.text.endsWith("while")).toBe(true);
    assertNoMidWordOrphans(block, intended);

    const tokenA = block.querySelector('.field-token[data-field-name="FIB1:a"]');
    expect(tokenA).toBeInstanceOf(HTMLElement);
    expect((tokenA as HTMLElement).style.fontSize).toBe("26pt");
    expect((tokenA as HTMLElement).style.fontFamily).toMatch(/times/i);

    const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
    let someNode: Text | null = null;
    let whileNode: Text | null = null;
    let n: Node | null;
    while ((n = walker.nextNode())) {
      const t = n as Text;
      if (t.parentElement?.closest(".field-token")) continue;
      if (t.data.includes("some") && !someNode) someNode = t;
      if (t.data.includes("while")) whileNode = t;
    }
    expect(someNode).not.toBeNull();
    expect(whileNode).not.toBeNull();
    expect(fontSizeOf(someNode!).includes("26")).toBe(true);
    expect(fontSizeOf(whileNode!).includes("26")).toBe(true);
    expect(fontFamilyOf(someNode!)).toMatch(/times/i);
    expect(fontFamilyOf(whileNode!)).toMatch(/times/i);
  });

  it("Face→Size survives Chrome-absorbed ZWSP inside face wrappers around a chip", () => {
    const { editor, block } = buildFieldParagraph();
    registerEditor(editor);
    const range = selectOwnerPartialSpan(block);
    expandRangeToTouchedTokens(editor, range);
    const intended = bookmarkTextOffsets(editor, range).text;

    // Simulate Chrome folding pads into adjacent face runs, then re-landing.
    paletteFontFace("Comic Sans MS");
    const token = block.querySelector(".field-token") as HTMLElement;
    const beforeWrap = token.previousElementSibling as HTMLElement | null;
    const afterWrap = token.nextElementSibling as HTMLElement | null;
    if (beforeWrap && !beforeWrap.classList.contains("field-token")) {
      beforeWrap.appendChild(document.createTextNode(CARET_ZWSP));
    }
    if (afterWrap && !afterWrap.classList.contains("field-token")) {
      afterWrap.insertBefore(document.createTextNode(CARET_ZWSP), afterWrap.firstChild);
    }
    ensureTokenCaretLanding(token);

    paletteFontSize("26");
    const after = bookmarkTextOffsets(editor, window.getSelection()!.getRangeAt(0));
    expect(after.text).toBe(intended);
    expect(after.text.startsWith("some")).toBe(true);
    expect(after.text.endsWith("while")).toBe(true);
    assertNoMidWordOrphans(block, intended);
    expect((token.style.fontSize)).toBe("26pt");
  });
});
