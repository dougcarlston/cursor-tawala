/**
 * @vitest-environment happy-dom
 *
 * Document canvas invent guards, orphans, and left-margin clamp.
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  DOC_LINE_MARGIN_PT,
  discardOrphanDocumentContent,
  hasOrphanDocumentContent,
  inventPlacedLeftPt,
  rangeIntersectsExistingDocumentText,
} from "./documentCanvas";

afterEach(() => {
  document.body.innerHTML = "";
  window.getSelection()?.removeAllRanges();
});

describe("inventPlacedLeftPt", () => {
  it("clamps flush-left invents to DOC_LINE_MARGIN_PT", () => {
    expect(DOC_LINE_MARGIN_PT).toBeCloseTo((10 * 72) / 96, 5);
    expect(inventPlacedLeftPt(0)).toBe(DOC_LINE_MARGIN_PT);
    expect(inventPlacedLeftPt(1)).toBe(DOC_LINE_MARGIN_PT);
    expect(inventPlacedLeftPt(40)).toBe(40);
  });
});

describe("rangeIntersectsExistingDocumentText", () => {
  it("is true for caret inside a .doc-placed-text block", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const placed = document.createElement("p");
    placed.className = "doc-placed-text";
    placed.textContent = "Hello";
    editor.appendChild(placed);
    document.body.appendChild(editor);

    const range = document.createRange();
    range.setStart(placed.firstChild as Text, 2);
    range.collapse(true);
    expect(rangeIntersectsExistingDocumentText(editor, range)).toBe(true);
  });

  it("is false for an empty editor", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    document.body.appendChild(editor);
    expect(rangeIntersectsExistingDocumentText(editor, null)).toBe(false);
    const range = document.createRange();
    range.setStart(editor, 0);
    range.collapse(true);
    expect(rangeIntersectsExistingDocumentText(editor, range)).toBe(false);
  });

  it("does not treat caret inside table.user as prose (must not block invent)", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const table = document.createElement("table");
    table.className = "user";
    const tr = document.createElement("tr");
    const td = document.createElement("td");
    td.textContent = "cell";
    tr.appendChild(td);
    table.appendChild(tr);
    editor.appendChild(table);
    document.body.appendChild(editor);

    const range = document.createRange();
    range.setStart(td.firstChild as Text, 1);
    range.collapse(true);
    expect(rangeIntersectsExistingDocumentText(editor, range)).toBe(false);
  });

  it("is true for orphan text under the editor root", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    editor.appendChild(document.createTextNode("orphan glyphs"));
    document.body.appendChild(editor);

    const range = document.createRange();
    range.setStart(editor.firstChild as Text, 2);
    range.collapse(true);
    expect(rangeIntersectsExistingDocumentText(editor, range)).toBe(true);
  });
});

describe("orphan document content", () => {
  it("detects and discards loose editor-root nodes", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const placed = document.createElement("p");
    placed.className = "doc-placed-text";
    placed.textContent = "keep";
    editor.appendChild(placed);
    editor.appendChild(document.createTextNode("loose"));
    document.body.appendChild(editor);

    expect(hasOrphanDocumentContent(editor)).toBe(true);
    expect(discardOrphanDocumentContent(editor)).toBe(true);
    expect(hasOrphanDocumentContent(editor)).toBe(false);
    expect(editor.querySelector(".doc-placed-text")?.textContent).toBe("keep");
    expect(editor.textContent).toBe("keep");
  });

  it("ignores placed lines and user tables", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const placed = document.createElement("p");
    placed.className = "doc-placed-text";
    placed.textContent = "line";
    const table = document.createElement("table");
    table.className = "user";
    table.innerHTML = "<tr><td>x</td></tr>";
    editor.appendChild(placed);
    editor.appendChild(table);
    document.body.appendChild(editor);

    expect(hasOrphanDocumentContent(editor)).toBe(false);
    expect(discardOrphanDocumentContent(editor)).toBe(false);
  });
});
