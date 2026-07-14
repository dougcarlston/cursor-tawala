/**
 * @vitest-environment happy-dom
 *
 * Owner: left-of-table `.doc-placed-text` (e.g. "Left") can take any Face/Size
 * EXCEPT the defaults — Arial / 12 pt are ignored because invent/sticky paints
 * face+size on the block and default apply only clears inline wrappers.
 */
import { beforeEach, describe, expect, it } from "vitest";
import { paletteFontFace, paletteFontSize } from "@/lib/paletteCommands";
import { setActivePaletteEditor } from "@/lib/formattingPaletteContext";
import {
  DEFAULT_PALETTE_FONT_FACE,
  DEFAULT_PALETTE_FONT_SIZE_PT,
} from "@/lib/paletteDefaults";
import { PLACED_TEXT_CLASS } from "@/lib/documentCanvas";

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

/** Absolute left-of-table line with face/size on the block (invent + sticky typing). */
function buildLeftOfTableLine(
  text: string,
  face: string,
  sizePt: string,
): { editor: HTMLElement; block: HTMLElement; textNode: Text } {
  const editor = document.createElement("div");
  editor.className = "rich-surface";
  editor.contentEditable = "true";
  editor.style.fontFamily = "Arial";
  editor.style.fontSize = "12pt";
  document.body.appendChild(editor);

  const table = document.createElement("table");
  table.className = "user";
  table.style.position = "absolute";
  table.style.left = "120pt";
  table.style.top = "40pt";
  const td = document.createElement("td");
  td.textContent = "cell";
  table.appendChild(document.createElement("tr")).appendChild(td);
  editor.appendChild(table);

  const block = document.createElement("p");
  block.className = PLACED_TEXT_CLASS;
  block.style.position = "absolute";
  block.style.left = "8pt";
  block.style.top = "40pt";
  block.style.fontFamily = face;
  block.style.fontSize = `${sizePt}pt`;
  const textNode = document.createTextNode(text);
  block.appendChild(textNode);
  editor.appendChild(block);

  return { editor, block, textNode };
}

function selectWholeText(textNode: Text): void {
  const range = document.createRange();
  range.setStart(textNode, 0);
  range.setEnd(textNode, textNode.data.length);
  const sel = window.getSelection();
  sel?.removeAllRanges();
  sel?.addRange(range);
}

function explicitFaceOf(el: HTMLElement): string {
  return (el.style.fontFamily || "").toLowerCase();
}

function explicitSizeOf(el: HTMLElement): string {
  return (el.style.fontSize || "").toLowerCase();
}

/** Explicit face between the text leaf and its placed block (not `.rich-surface`). */
function leafFace(text: Text, block: HTMLElement): string {
  let el: HTMLElement | null = text.parentElement;
  while (el && el !== block) {
    if (el.style.fontFamily) return el.style.fontFamily.toLowerCase();
    if (el.tagName === "FONT" && el.getAttribute("face")) {
      return el.getAttribute("face")!.toLowerCase();
    }
    el = el.parentElement;
  }
  return "";
}

/** Explicit size between the text leaf and its placed block (not `.rich-surface`). */
function leafSize(text: Text, block: HTMLElement): string {
  let el: HTMLElement | null = text.parentElement;
  while (el && el !== block) {
    if (el.style.fontSize) return el.style.fontSize.toLowerCase();
    el = el.parentElement;
  }
  return "";
}

function findTextLeaf(block: HTMLElement, needle: string): Text | null {
  const walker = document.createTreeWalker(block, NodeFilter.SHOW_TEXT);
  let node: Node | null;
  while ((node = walker.nextNode())) {
    const text = node as Text;
    if (text.data.includes(needle)) return text;
  }
  return null;
}

describe("left-of-table placed line → default Face/Size", () => {
  beforeEach(() => {
    document.body.innerHTML = "";
    installExecCommandPolyfill();
  });

  it("Arial (default) + 12 pt clear block-level Comic/20 on a full-word select", () => {
    const { editor, block, textNode } = buildLeftOfTableLine("Left", "Comic Sans MS", "20");
    registerEditor(editor);
    selectWholeText(textNode);

    paletteFontFace(DEFAULT_PALETTE_FONT_FACE);
    paletteFontSize(String(DEFAULT_PALETTE_FONT_SIZE_PT));

    const leaf = findTextLeaf(block, "Left");
    expect(leaf).toBeTruthy();
    expect(explicitFaceOf(block)).toBe("");
    expect(explicitSizeOf(block)).toBe("");
    expect(leafFace(leaf!, block)).toBe("");
    expect(leafSize(leaf!, block)).toBe("");

    editor.remove();
  });

  it("non-default Face/Size still override block sticky (regression for Comic 20)", () => {
    const { editor, block, textNode } = buildLeftOfTableLine("Left", "Times New Roman", "18");
    registerEditor(editor);
    selectWholeText(textNode);

    paletteFontFace("Comic Sans MS");
    paletteFontSize("20");

    const leaf = findTextLeaf(block, "Left");
    expect(leaf).toBeTruthy();
    expect(leafFace(leaf!, block)).toContain("comic");
    expect(leafSize(leaf!, block)).toBe("20pt");

    editor.remove();
  });

  it("owner smoke: Comic 20 then Arial/default + 12 clears to defaults", () => {
    // Bare invent line (no sticky on block) → Face/Size paints spans → defaults must clear.
    const { editor, block, textNode } = buildLeftOfTableLine("Left", "", "");
    block.style.removeProperty("font-family");
    block.style.removeProperty("font-size");
    registerEditor(editor);
    selectWholeText(textNode);

    paletteFontFace("Comic Sans MS");
    paletteFontSize("20");
    // Re-select after Face/Size — restore may leave a usable highlight, but be explicit.
    const mid = findTextLeaf(block, "Left");
    expect(mid).toBeTruthy();
    expect(leafSize(mid!, block)).toBe("20pt");
    selectWholeText(mid!);
    paletteFontFace(DEFAULT_PALETTE_FONT_FACE);
    // Face default must not drop the highlight for the following Size default.
    const afterFace = findTextLeaf(block, "Left");
    expect(afterFace).toBeTruthy();
    selectWholeText(afterFace!);
    paletteFontSize(String(DEFAULT_PALETTE_FONT_SIZE_PT));

    const leaf = findTextLeaf(block, "Left");
    expect(leaf).toBeTruthy();
    expect(explicitFaceOf(block)).toBe("");
    expect(explicitSizeOf(block)).toBe("");
    expect(leafFace(leaf!, block)).toBe("");
    expect(leafSize(leaf!, block)).toBe("");

    editor.remove();
  });

  it("partial one-word highlight inside a longer line does not strip the whole block", () => {
    const { editor, block } = buildLeftOfTableLine("Hello Left World", "Comic Sans MS", "20");
    registerEditor(editor);

    const textNode = block.firstChild as Text;
    const start = textNode.data.indexOf("Left");
    const range = document.createRange();
    range.setStart(textNode, start);
    range.setEnd(textNode, start + "Left".length);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    paletteFontFace(DEFAULT_PALETTE_FONT_FACE);
    paletteFontSize(String(DEFAULT_PALETTE_FONT_SIZE_PT));

    // Block sticky must remain — only the mid word was defaulted (inline clear).
    expect(explicitFaceOf(block).toLowerCase()).toContain("comic");
    expect(explicitSizeOf(block)).toBe("20pt");

    editor.remove();
  });
});
