/**
 * Deleted Document text must not resurrect as orphan→adopt ghosts or stacked duplicates.
 */
import { describe, expect, it } from "vitest";
import {
  adoptOrphanDocumentContent,
  discardOrphanDocumentContent,
  hasOrphanDocumentContent,
  PLACED_TEXT_CLASS,
  pruneDuplicateOverlappingPlacedBlocks,
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

describe("Document ghost text after delete", () => {
  it("does not resurrect deleted text when orphans are discarded instead of adopted", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    // Simulate delete ejecting glyphs to the editor root while the husk is empty.
    const husk = placed(40, "<br>");
    editor.append(husk);
    editor.append(document.createTextNode("Form Count"));

    expect(hasOrphanDocumentContent(editor)).toBe(true);

    pruneEmptyPlacedTextBlocks(editor);
    // Old path: adopt would wrap "Form Count" into a new placed line (ghost).
    discardOrphanDocumentContent(editor);

    expect(hasOrphanDocumentContent(editor)).toBe(false);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(0);
    expect(editor.textContent ?? "").not.toContain("Form Count");

    editor.remove();
  });

  it("shows why adopt-after-delete was wrong (orphan reappears as placed text)", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    editor.append(document.createTextNode("Form Count"));

    expect(adoptOrphanDocumentContent(editor)).toBe(true);
    expect(editor.textContent).toContain("Form Count");
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(1);

    editor.remove();
  });

  it("removes stacked duplicate placed lines at the same top", () => {
    const editor = document.createElement("div");
    // layoutItemBoxPt needs getBoundingClientRect — attach to body.
    document.body.appendChild(editor);
    editor.style.position = "relative";
    editor.style.width = "400px";
    editor.style.height = "200px";

    const a = placed(40, "Form Count");
    const b = placed(41, "Form Count");
    editor.append(a, b);

    expect(pruneDuplicateOverlappingPlacedBlocks(editor)).toBe(true);
    const left = Array.from(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`));
    expect(left.length).toBe(1);
    expect(left[0].textContent).toContain("Form Count");

    editor.remove();
  });

  it("keeps identical text on separate lines (different tops)", () => {
    const editor = document.createElement("div");
    document.body.appendChild(editor);
    editor.style.position = "relative";
    editor.style.width = "400px";
    editor.style.height = "200px";

    const a = placed(40, "Form Count");
    const b = placed(80, "Form Count");
    editor.append(a, b);

    expect(pruneDuplicateOverlappingPlacedBlocks(editor)).toBe(false);
    expect(editor.querySelectorAll(`.${PLACED_TEXT_CLASS}`).length).toBe(2);

    editor.remove();
  });
});
