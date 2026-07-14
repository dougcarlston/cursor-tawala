/**
 * Delete Table must confirm (same spirit as Form/Process/Document delete).
 * Cancel keeps the table; OK removes it.
 */
import { afterEach, describe, expect, it, vi } from "vitest";
import { setActivePaletteEditor, clearActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { paletteDeleteTable } from "@/lib/paletteCommands";

describe("paletteDeleteTable confirm", () => {
  afterEach(() => {
    clearActivePaletteEditor();
    vi.restoreAllMocks();
  });

  function setupTableEditor(): { editor: HTMLElement; table: HTMLTableElement } {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    document.body.appendChild(editor);
    const table = document.createElement("table");
    table.className = "user";
    table.innerHTML = "<tr><td>cell</td></tr>";
    editor.appendChild(table);
    const cell = table.querySelector("td")!;
    const range = document.createRange();
    range.selectNodeContents(cell);
    range.collapse(true);
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);
    setActivePaletteEditor({
      el: editor,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });
    return { editor, table };
  }

  it("Cancel leaves the table in place", () => {
    const { editor, table } = setupTableEditor();
    vi.spyOn(window, "confirm").mockReturnValue(false);

    paletteDeleteTable();

    expect(window.confirm).toHaveBeenCalledWith("Are you sure you want to delete this table?");
    expect(editor.contains(table)).toBe(true);
    editor.remove();
  });

  it("OK removes the table", () => {
    const { editor, table } = setupTableEditor();
    vi.spyOn(window, "confirm").mockReturnValue(true);

    paletteDeleteTable();

    expect(window.confirm).toHaveBeenCalledWith("Are you sure you want to delete this table?");
    expect(editor.contains(table)).toBe(false);
    editor.remove();
  });
});
