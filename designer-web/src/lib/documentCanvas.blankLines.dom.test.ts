/**
 * Double-Return blank lines between Document placed paragraphs must survive
 * prune-on-delete and packing when the second paragraph is edited.
 */
import { describe, expect, it } from "vitest";
import {
  DOC_BLANK_ATTR,
  isIntentionalBlankPlacedBlock,
  isPlacedTextBlockEmpty,
  markBlankPlacedBlock,
  PLACED_TEXT_CLASS,
  preserveBlankPlacedLines,
  pruneEmptyPlacedTextBlocks,
} from "./documentCanvas";

function placed(top: number, html: string): HTMLElement {
  const p = document.createElement("p");
  p.className = PLACED_TEXT_CLASS;
  p.style.position = "absolute";
  p.style.left = "36pt";
  p.style.top = `${top}pt`;
  p.innerHTML = html;
  return p;
}

describe("Document Double-Return blank lines", () => {
  it("keeps an empty block between two content paragraphs on prune", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    const a = placed(0, "First paragraph");
    const blank = placed(20, "<br>");
    markBlankPlacedBlock(blank);
    const c = placed(40, "Second paragraph");
    editor.append(a, blank, c);

    expect(isPlacedTextBlockEmpty(blank)).toBe(true);
    expect(isIntentionalBlankPlacedBlock(blank, [a, blank, c])).toBe(true);

    const removed = pruneEmptyPlacedTextBlocks(editor);
    expect(removed).toBe(false);
    expect(editor.contains(blank)).toBe(true);
    expect(blank.getAttribute(DOC_BLANK_ATTR)).toBe("1");

    editor.remove();
  });

  it("still prunes leading/trailing empty husks after delete", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    const husk = placed(0, "<br>");
    const content = placed(20, "Keep me");
    const trail = placed(40, "<br>");
    editor.append(husk, content, trail);

    const removed = pruneEmptyPlacedTextBlocks(editor);
    expect(removed).toBe(true);
    expect(editor.contains(husk)).toBe(false);
    expect(editor.contains(trail)).toBe(false);
    expect(editor.contains(content)).toBe(true);

    editor.remove();
  });

  it("restore scaffold when Chromium strips br from a marked blank", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    const a = placed(0, "First");
    const blank = placed(20, "");
    blank.setAttribute(DOC_BLANK_ATTR, "1");
    const c = placed(40, "Second");
    editor.append(a, blank, c);

    preserveBlankPlacedLines(editor);
    expect(blank.querySelector("br")).toBeTruthy();
    expect(blank.style.minHeight).toMatch(/pt$/);

    editor.remove();
  });
});
