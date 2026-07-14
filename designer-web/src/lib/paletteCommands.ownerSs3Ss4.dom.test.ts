/**
 * @vitest-environment happy-dom
 *
 * Owner Jul 14 SS1→SS4 (TNR 18 + `<<_InviteeID>>`):
 * SS3 — reselect after Face Comic → readout Comic (not Mixed), even with a
 *   stale insert-time TNR chip face and whitespace/ZWSP pads.
 * Re-drag — after Face Comic + Size 26, click away + reselect mid-span with
 *   field: Face Comic and Size 26 (not Mixed) even with bare block pads /
 *   cleared chip attrs.
 * SS4 — mid-span Size 26 must not alter outside-left / outside-right face+size.
 */
import { beforeEach, describe, expect, it } from "vitest";
import {
  paletteFontFace,
  paletteFontSize,
  readPaletteActiveState,
} from "@/lib/paletteCommands";
import { getActivePaletteEditor, setActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { CARET_ZWSP } from "@/lib/tokenCaretLanding";
import {
  bookmarkTextOffsets,
  expandRangeToTouchedTokens,
  rangeFromTextOffsets,
} from "@/lib/selectionBookmark";
import { PLACED_TEXT_CLASS } from "@/lib/documentCanvas";
import { MIXED_PALETTE_VALUE } from "@/lib/paletteDefaults";

function chip(name: string, face?: string): HTMLSpanElement {
  const span = document.createElement("span");
  span.className = "field-token function-table-token";
  span.setAttribute("contenteditable", "false");
  span.setAttribute("data-field-name", name);
  span.textContent = `<<${name}>>`;
  // Stale insert-time face (block TNR) — Face should restyle, but readout must
  // not report Mixed if a leftover TNR chip meets adjacent Comic text.
  if (face) span.style.fontFamily = face;
  return span;
}

function buildOwnerParagraph(opts?: { staleChipFace?: boolean }): {
  editor: HTMLElement;
  block: HTMLElement;
} {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
  // .rich-surface default Arial — right-of-selection must NOT fall through to this.
  editor.style.fontFamily = "Arial";
  document.body.appendChild(editor);

  const block = document.createElement("p");
  block.className = PLACED_TEXT_CLASS;
  block.style.fontSize = "18pt";
  block.style.fontFamily = "Times New Roman";

  const appendText = (t: string) => block.appendChild(document.createTextNode(t));
  appendText("This is an example of some test with a single field in it here: ");
  block.appendChild(document.createTextNode(CARET_ZWSP));
  // Interstitial space under the block (between ZWSP absorption scenarios) —
  // inherit must skip whitespace, not report block TNR as the chip face.
  block.appendChild(document.createTextNode(" "));
  block.appendChild(
    chip("_InviteeID", opts?.staleChipFace ? "Times New Roman" : undefined),
  );
  block.appendChild(document.createTextNode(CARET_ZWSP));
  appendText(" We are trying to figure out why font size changes irrationally.");

  editor.appendChild(block);
  return { editor, block };
}

function selectFromExampleThroughWhy(block: HTMLElement): Range {
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let startNode: Text | null = null;
  let startOff = 0;
  let endNode: Text | null = null;
  let endOff = 0;
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (text.parentElement?.closest(".field-token")) continue;
    const iEx = text.data.indexOf("example");
    if (iEx >= 0 && !startNode) {
      startNode = text;
      startOff = iEx;
    }
    const iWhy = text.data.indexOf("why");
    if (iWhy >= 0) {
      endNode = text;
      endOff = iWhy + "why".length;
    }
  }
  if (!startNode || !endNode) throw new Error("anchors missing");
  const range = document.createRange();
  range.setStart(startNode, startOff);
  range.setEnd(endNode, endOff);
  const sel = window.getSelection();
  sel?.removeAllRanges();
  sel?.addRange(range);
  return range;
}

/** Mid-word reselect inside the Face span (SS4): "mple" … "fig". */
function selectMidWordInsideComic(block: HTMLElement): Range {
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let startNode: Text | null = null;
  let startOff = 0;
  let endNode: Text | null = null;
  let endOff = 0;
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (text.parentElement?.closest(".field-token")) continue;
    const iEx = text.data.indexOf("example");
    if (iEx >= 0 && !startNode) {
      startNode = text;
      // Mid-word: skip "exa"
      startOff = iEx + 3;
    }
    const iFig = text.data.indexOf("figure");
    if (iFig >= 0) {
      endNode = text;
      endOff = iFig + 3; // "fig"
    }
  }
  if (!startNode || !endNode) throw new Error("mid-word anchors missing");
  const range = document.createRange();
  range.setStart(startNode, startOff);
  range.setEnd(endNode, endOff);
  const sel = window.getSelection();
  sel?.removeAllRanges();
  sel?.addRange(range);
  return range;
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
    if (face) return face;
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
    if (el.style.fontSize) return el.style.fontSize;
    el = el.parentElement;
  }
  return "";
}

function findTextIncluding(block: HTMLElement, needle: string): Text | null {
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let n: Node | null;
  while ((n = walker.nextNode())) {
    const t = n as Text;
    if (t.parentElement?.closest(".field-token")) continue;
    if (t.data.includes(needle)) return t;
  }
  return null;
}

describe("Owner SS1→SS4 TNR + InviteeID (false Mixed + Size bleed)", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
    document.execCommand = ((commandId: string) => {
      if (commandId === "styleWithCSS") return true;
      if (commandId === "fontName" || commandId === "fontSize") {
        throw new Error(`execCommand(${commandId}) must not run on highlight Face/Size`);
      }
      return false;
    }) as typeof document.execCommand;
  });

  it("SS3: reselect after Face Comic → Comic not Mixed (stale chip + whitespace)", () => {
    const { editor, block } = buildOwnerParagraph({ staleChipFace: true });
    registerEditor(editor);
    editor.focus();

    const range = selectFromExampleThroughWhy(block);
    expandRangeToTouchedTokens(editor, range);
    const bookmark = bookmarkTextOffsets(editor, range);
    expect(bookmark.text).toContain("<<_InviteeID>>");

    paletteFontFace("Comic Sans MS");

    // Click away + reselect same offsets (owner SS3).
    window.getSelection()?.removeAllRanges();
    // Simulate chip still carrying insert-time TNR (Face paint missed / React race).
    const tok = block.querySelector(".field-token") as HTMLElement;
    tok.style.fontFamily = "Times New Roman";

    const reselect = rangeFromTextOffsets(editor, bookmark.start, bookmark.end);
    expect(reselect).not.toBeNull();
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(reselect!);

    const state = readPaletteActiveState();
    expect(state.fontFace).not.toBe(MIXED_PALETTE_VALUE);
    expect(state.fontFace).toMatch(/comic/i);
  });

  it("re-drag after Face Comic + Size 26 → Face Comic Size 26 (not Mixed)", () => {
    // Owner remaining bug: mid-span Comic+26 with field → click away → re-drag →
    // Mixed/Mixed. Bare space / `" \u200b"` pads under the block still sampled as
    // TNR 18 even when glyph runs + chip were Comic 26 (SS3 inherit alone was not enough).
    const { editor, block } = buildOwnerParagraph({ staleChipFace: true });
    registerEditor(editor);
    editor.focus();

    const mid = selectMidWordInsideComic(block);
    expandRangeToTouchedTokens(editor, mid);
    const midBm = bookmarkTextOffsets(editor, mid);
    expect(midBm.text).toContain("<<_InviteeID>>");
    getActivePaletteEditor()?.saveSelection();

    paletteFontFace("Comic Sans MS");
    getActivePaletteEditor()?.saveSelection();
    paletteFontSize("26");

    // Click away.
    window.getSelection()?.removeAllRanges();

    // Simulate post-apply pads Chrome/caret-landing can leave under the block:
    // a bare space (and ZWSP-adjacent) that would report block TNR/18 if sampled,
    // plus a chip remount that dropped face/size (inherits visually from paint race).
    const tok = block.querySelector(".field-token") as HTMLElement;
    tok.style.cssText = "";
    const bareSpace = document.createTextNode(" ");
    const zwspPad = document.createTextNode(` ${CARET_ZWSP}`);
    block.insertBefore(bareSpace, tok);
    block.insertBefore(zwspPad, tok);

    const reselect = rangeFromTextOffsets(editor, midBm.start, midBm.end);
    expect(reselect).not.toBeNull();
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(reselect!);

    const state = readPaletteActiveState();
    expect(state.fontFace).not.toBe(MIXED_PALETTE_VALUE);
    expect(state.fontFace).toMatch(/comic/i);
    expect(state.fontSize).not.toBe(MIXED_PALETTE_VALUE);
    expect(state.fontSize).toBe("26");
  });

  it("re-drag: chip with non-stale Georgia/14 does not force Mixed when glyphs are Comic/26", () => {
    // Stronger false-Mixed: chip reports an explicit face/size that is neither the
    // block default nor Arial/12 (staleInsert path would not override). Glyph runs
    // are uniform Comic 26 — palette must prefer glyphs over the disagreeing chip.
    const { editor, block } = buildOwnerParagraph();
    registerEditor(editor);
    editor.focus();

    const mid = selectMidWordInsideComic(block);
    expandRangeToTouchedTokens(editor, mid);
    const midBm = bookmarkTextOffsets(editor, mid);
    getActivePaletteEditor()?.saveSelection();

    paletteFontFace("Comic Sans MS");
    getActivePaletteEditor()?.saveSelection();
    paletteFontSize("26");
    window.getSelection()?.removeAllRanges();

    const tok = block.querySelector(".field-token") as HTMLElement;
    tok.style.fontFamily = "Georgia";
    tok.style.fontSize = "14pt";
    block.insertBefore(document.createTextNode(" "), tok);
    block.insertBefore(document.createTextNode(CARET_ZWSP), tok);

    const reselect = rangeFromTextOffsets(editor, midBm.start, midBm.end)!;
    window.getSelection()!.removeAllRanges();
    window.getSelection()!.addRange(reselect);

    const state = readPaletteActiveState();
    expect(state.fontFace).not.toBe(MIXED_PALETTE_VALUE);
    expect(state.fontFace).toMatch(/comic/i);
    expect(state.fontSize).not.toBe(MIXED_PALETTE_VALUE);
    expect(state.fontSize).toBe("26");
  });

  it("SS4: Size 26 mid-span leaves outside left TNR 18 and outside right TNR 18", () => {
    const { editor, block } = buildOwnerParagraph();
    registerEditor(editor);
    editor.focus();

    const faceRange = selectFromExampleThroughWhy(block);
    expandRangeToTouchedTokens(editor, faceRange);
    const faceBm = bookmarkTextOffsets(editor, faceRange);

    paletteFontFace("Comic Sans MS");

    // Reselect a MID-WORD subset inside the Comic span (owner SS4 edges).
    // Critical: palette withEditor restoreSelection() otherwise reapplies the Face
    // bookmark from the prior save — update the saved range like Size <select>
    // mousedown does in FormattingPalette.
    window.getSelection()?.removeAllRanges();
    const mid = selectMidWordInsideComic(block);
    expandRangeToTouchedTokens(editor, mid);
    const midBm = bookmarkTextOffsets(editor, mid);
    expect(midBm.text.startsWith("mple")).toBe(true);
    expect(midBm.text).toContain("<<_InviteeID>>");
    expect(midBm.text.includes("fig")).toBe(true);
    // Persist mid-word highlight as the palette-saved selection.
    getActivePaletteEditor()?.saveSelection();

    // Left remnant "exa" / "This is an " and right "ure..." / "font size..." must
    // keep prior face+size after Size.
    const leftBefore = findTextIncluding(block, "This is an");
    const rightBefore = findTextIncluding(block, "irrationally");
    expect(leftBefore).not.toBeNull();
    expect(rightBefore).not.toBeNull();

    paletteFontSize("26");

    const leftAfter = findTextIncluding(block, "This is an") ?? findTextIncluding(block, "exa");
    const rightAfter =
      findTextIncluding(block, "irrationally") ?? findTextIncluding(block, "ure");
    expect(leftAfter).not.toBeNull();
    expect(rightAfter).not.toBeNull();

    // Outside-left: TNR (block), not Comic 26, never Arial default.
    const leftFace = fontFamilyOf(leftAfter!).toLowerCase();
    expect(leftFace === "" || /times/.test(leftFace)).toBe(true);
    expect(leftFace).not.toMatch(/arial/);
    expect(fontSizeOf(leftAfter!) === "" || fontSizeOf(leftAfter!).includes("18")).toBe(true);
    expect(fontSizeOf(leftAfter!).includes("26")).toBe(false);

    // Outside-right: must keep Comic (was inside Face) OR TNR (if never faced) —
    // never bare Arial from .rich-surface.
    const rightAfterFace = fontFamilyOf(rightAfter!).toLowerCase();
    expect(rightAfterFace).not.toMatch(/^["']?arial["']?$/);
    expect(rightAfterFace === "" || /times|comic/.test(rightAfterFace)).toBe(true);
    expect(fontSizeOf(rightAfter!).includes("26")).toBe(false);

    // Mid selection: Comic 26.
    const midAfter = rangeFromTextOffsets(editor, midBm.start, midBm.end);
    expect(midAfter).not.toBeNull();
    window.getSelection()!.removeAllRanges();
    window.getSelection()!.addRange(midAfter!);
    const state = readPaletteActiveState();
    expect(state.fontFace).toMatch(/comic/i);
    expect(state.fontSize).toBe("26");

    // Left mid-word remnant "exa" (was Comic after Face) must stay Comic — not TNR.
    // After splitFormatWrappersAtRangeBoundaries, "exa" is its own Comic remnant.
    const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
    let exaNode: Text | null = null;
    let ureNode: Text | null = null;
    let n: Node | null;
    while ((n = walker.nextNode())) {
      const t = n as Text;
      if (t.parentElement?.closest(".field-token")) continue;
      if (t.data === "exa" || /^exa/.test(t.data)) exaNode = t;
      if (t.data === "ure" || t.data.startsWith("ure ")) ureNode = t;
    }
    expect(exaNode).not.toBeNull();
    expect(fontFamilyOf(exaNode!).toLowerCase()).toMatch(/comic/);
    expect(fontSizeOf(exaNode!).includes("26")).toBe(false);
    if (ureNode) {
      expect(fontFamilyOf(ureNode).toLowerCase()).toMatch(/comic/);
      expect(fontSizeOf(ureNode).includes("26")).toBe(false);
    }

    void faceBm;
  });
});
