/**
 * Delete Table must confirm (same spirit as Form/Process/Document delete).
 * Cancel keeps the table; OK removes it.
 */
import { afterEach, describe, expect, it } from "vitest";
import { setActivePaletteEditor, clearActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { paletteDeleteTable } from "@/lib/paletteCommands";
import { getActiveConfirm } from "@/lib/confirmDialog";

describe("paletteDeleteTable confirm", () => {
  afterEach(() => {
    clearActivePaletteEditor();
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

  it("Cancel leaves the table in place", async () => {
    const { editor, table } = setupTableEditor();
    paletteDeleteTable();

    const active = getActiveConfirm();
    expect(active).not.toBeNull();
    expect(active?.options.message).toBe("Are you sure you want to delete this table?");
    active?.resolve(false);
    await Promise.resolve();

    expect(editor.contains(table)).toBe(true);
    editor.remove();
  });

  it("OK removes the table", async () => {
    const { editor, table } = setupTableEditor();
    paletteDeleteTable();

    const active = getActiveConfirm();
    expect(active).not.toBeNull();
    expect(active?.options.message).toBe("Are you sure you want to delete this table?");
    active?.resolve(true);
    await Promise.resolve();

    expect(editor.contains(table)).toBe(false);
    editor.remove();
  });
});

