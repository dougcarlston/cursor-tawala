/**
 * @vitest-environment happy-dom
 *
 * Form Text CSS often computes ~13px (≈10pt). Insert must not stamp 10pt on
 * field chips when the palette sticky size is the default — that caused random
 * chip heights next to inherit-sized siblings.
 */
import { afterEach, describe, expect, it } from "vitest";
import { insertFieldTokenAtSelection, FIELD_TOKEN_CLASS } from "./fieldTokens";
import {
  setActivePaletteEditor,
  clearActivePaletteEditor,
} from "./formattingPaletteContext";
import { setTypingFormat, typingFormatForInsert } from "./paletteTypingFormat";
import { DEFAULT_PALETTE_FONT_SIZE_PT } from "./paletteDefaults";

describe("field token insert size (Form Text surface)", () => {
  afterEach(() => {
    clearActivePaletteEditor();
    document.body.replaceChildren();
  });

  it("treats Form Text ~13px as surface default — chip inherits, no 10pt stamp", () => {
    const editor = document.createElement("div");
    editor.className = "text-rich-editor";
    editor.contentEditable = "true";
    editor.style.fontSize = "13px";
    document.body.append(editor);
    editor.append("Name: ");
    setTypingFormat(editor, { fontSize: String(DEFAULT_PALETTE_FONT_SIZE_PT) });
    setActivePaletteEditor({
      el: editor,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });

    const range = document.createRange();
    range.selectNodeContents(editor);
    range.collapse(false);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    const typing = typingFormatForInsert(editor);
    expect(typing.fontSize).toBe(String(DEFAULT_PALETTE_FONT_SIZE_PT));

    insertFieldTokenAtSelection("FirstName");
    const chip = editor.querySelector(`.${FIELD_TOKEN_CLASS}`) as HTMLElement;
    expect(chip).toBeTruthy();
    expect(chip.style.fontSize).toBe("");
  });
});
