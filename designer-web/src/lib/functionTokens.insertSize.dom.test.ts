/**
 * @vitest-environment happy-dom
 *
 * Function chips must inherit the line size at default — not stamp a computed
 * 10pt from Form Text CSS, and not resize the parent paragraph.
 */
import { describe, expect, it } from "vitest";
import {
  FUNCTION_TOKEN_CLASS,
  insertFunctionTokenAtSelection,
} from "./functionTokens";
import { getFunctionDef } from "./functionCatalog";
import { setTypingFormat } from "./paletteTypingFormat";
import { DEFAULT_PALETTE_FONT_SIZE_PT } from "./paletteDefaults";

describe("insertFunctionTokenAtSelection size", () => {
  it("does not set inline font-size at palette default (inherits line)", () => {
    const editor = document.createElement("div");
    editor.className = "text-rich-editor";
    editor.contentEditable = "true";
    editor.style.fontSize = "13px";
    document.body.append(editor);
    editor.append("Count: ");
    setTypingFormat(editor, { fontSize: String(DEFAULT_PALETTE_FONT_SIZE_PT) });

    const range = document.createRange();
    range.selectNodeContents(editor);
    range.collapse(false);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    const def = getFunctionDef("record-count")!;
    insertFunctionTokenAtSelection(editor, def, { form: "Form 2" });

    const chip = editor.querySelector(`.${FUNCTION_TOKEN_CLASS}`) as HTMLElement;
    expect(chip).toBeTruthy();
    expect(chip.style.fontSize).toBe("");
    // Parent editor must not pick up a stamped size from insert.
    expect(editor.style.fontSize).toBe("13px");
  });

  it("does not rewrite an existing paragraph font-size when inserting", () => {
    const editor = document.createElement("div");
    editor.className = "text-rich-editor";
    editor.contentEditable = "true";
    document.body.append(editor);
    const line = document.createElement("div");
    line.style.fontSize = "18pt";
    line.textContent = "Label: ";
    editor.append(line);
    // Sticky size differs from the line — chip must follow the line, not restyle it.
    setTypingFormat(editor, { fontSize: "26" });

    const range = document.createRange();
    range.selectNodeContents(line);
    range.collapse(false);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    const def = getFunctionDef("record-count")!;
    insertFunctionTokenAtSelection(editor, def, { form: "Form 2" });

    expect(line.style.fontSize).toBe("18pt");
    const chip = line.querySelector(`.${FUNCTION_TOKEN_CLASS}`) as HTMLElement;
    expect(chip.style.fontSize).toBe("18pt");
  });
});
