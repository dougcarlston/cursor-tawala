/**
 * Double-Return blank lines between Document placed paragraphs must survive
 * prune-on-delete and packing when the second paragraph is edited.
 * Form Text flow paragraphs clear blank marks once they gain content.
 */
import { describe, expect, it } from "vitest";
import {
  DOC_BLANK_ATTR,
  isIntentionalBlankPlacedBlock,
  isPlacedTextBlockEmpty,
  markBlankPlacedBlock,
  PLACED_TEXT_CLASS,
  preserveBlankPlacedLines,
  preserveFormTextFlowParagraphs,
  pruneEmptyPlacedTextBlocks,
  sanitizeFormTextFlowHtml,
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

  it("does not treat From-your-PC embedded images as empty (keep on prune)", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    const pic = placed(
      0,
      `<img class="tawala-embedded-image" data-tawala-image-id="image1" ` +
        `data-image-width="40" data-image-height="20" width="40" height="20" ` +
        `src="data:image/gif;base64,R0lGODlhAQABAIAAAAAAAP///yH5BAEAAAAALAAAAAABAAEAAAIBRAA7" alt="" />`,
    );
    editor.append(pic);

    expect(isPlacedTextBlockEmpty(pic)).toBe(false);
    expect(pruneEmptyPlacedTextBlocks(editor)).toBe(false);
    expect(editor.contains(pic)).toBe(true);

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

describe("Form Text flow blank paragraphs", () => {
  it("clears data-doc-blank and scaffold black when a spacer gains text", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    const a = document.createElement("p");
    a.innerHTML = `<span style="font-family: Tahoma;">First</span>`;
    const blank = document.createElement("p");
    blank.setAttribute(DOC_BLANK_ATTR, "1");
    blank.style.color = "rgb(0, 0, 0)";
    blank.style.minHeight = "13.7pt";
    blank.innerHTML = "<br>";
    const typed = document.createElement("p");
    typed.setAttribute(DOC_BLANK_ATTR, "1");
    typed.style.color = "rgb(0, 0, 0)";
    typed.style.minHeight = "13.7pt";
    typed.innerHTML =
      `<span style="background-color: transparent; font-family: Tahoma;">Once typed</span>`;
    editor.append(a, blank, typed);

    preserveFormTextFlowParagraphs(editor);

    expect(blank.getAttribute(DOC_BLANK_ATTR)).toBe("1");
    expect(blank.querySelector("br")).toBeTruthy();
    expect(typed.getAttribute(DOC_BLANK_ATTR)).toBeNull();
    expect(typed.style.color).toBe("");
    expect(typed.style.minHeight).toBe("");
    expect(typed.textContent).toContain("Once typed");

    editor.remove();
  });

  it("puts br into bare empty p so Double-Return gaps do not collapse", () => {
    const html = sanitizeFormTextFlowHtml(
      `<p><span style="font-family: Tahoma;">A</span></p><p></p><p><span style="font-family: Tahoma;">B</span></p>`,
    );
    expect(html).toContain("<p><br></p>");
    expect(html).toContain(">A</span>");
    expect(html).toContain(">B</span>");
  });
});
