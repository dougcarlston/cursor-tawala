/**
 * @vitest-environment happy-dom
 *
 * Owner Jul 24: Italic (unlike Bold/Underline) often collapses the live range after
 * execCommand. Palette B/I/U must restore the highlight so a second click can undo.
 *
 * happy-dom has no real execCommand — we stub wrap + optional collapse to mirror Blink.
 */
import { beforeEach, describe, expect, it } from "vitest";
import { paletteBold, paletteItalic, paletteUnderline } from "@/lib/paletteCommands";
import { setActivePaletteEditor } from "@/lib/formattingPaletteContext";

const TAG: Record<"bold" | "italic" | "underline", string> = {
  bold: "b",
  italic: "i",
  underline: "u",
};

function mountEditor(html: string): HTMLElement {
  const editor = document.createElement("div");
  editor.contentEditable = "true";
  editor.innerHTML = html;
  document.body.appendChild(editor);
  setActivePaletteEditor({
    el: editor,
    commit: () => {},
    saveSelection: () => {},
    restoreSelection: () => {},
  });
  return editor;
}

function selectWord(editor: HTMLElement, word: string): void {
  const text = editor.firstChild;
  expect(text?.nodeType).toBe(Node.TEXT_NODE);
  const data = text!.textContent ?? "";
  const start = data.indexOf(word);
  expect(start).toBeGreaterThanOrEqual(0);
  const range = document.createRange();
  range.setStart(text!, start);
  range.setEnd(text!, start + word.length);
  const sel = window.getSelection();
  sel?.removeAllRanges();
  sel?.addRange(range);
}

function selectedText(): string {
  return window.getSelection()?.toString() ?? "";
}

/**
 * Stub CE formatting: wrap the selection, then optionally collapse (italic quirk).
 * Toggle unwraps when the selection is already inside the mark tag.
 */
function stubExecCommand(collapseAfter: Set<string>): void {
  document.execCommand = ((commandId: string) => {
    if (commandId === "styleWithCSS") return true;
    if (commandId !== "bold" && commandId !== "italic" && commandId !== "underline") {
      return false;
    }
    const sel = window.getSelection();
    if (!sel?.rangeCount) return false;
    const range = sel.getRangeAt(0);
    if (range.collapsed) return false;

    const tag = TAG[commandId];
    const parent = range.commonAncestorContainer.parentElement;
    if (parent?.tagName.toLowerCase() === tag && parent.textContent === range.toString()) {
      const text = document.createTextNode(parent.textContent ?? "");
      parent.replaceWith(text);
      const next = document.createRange();
      next.selectNodeContents(text);
      sel.removeAllRanges();
      sel.addRange(next);
    } else {
      const wrap = document.createElement(tag);
      wrap.appendChild(range.extractContents());
      range.insertNode(wrap);
      const next = document.createRange();
      next.selectNodeContents(wrap);
      sel.removeAllRanges();
      sel.addRange(next);
    }

    if (collapseAfter.has(commandId) && sel.rangeCount) {
      sel.getRangeAt(0).collapse(true);
    }
    return true;
  }) as typeof document.execCommand;

  document.queryCommandState = ((commandId: string) => {
    const sel = window.getSelection();
    if (!sel?.rangeCount) return false;
    let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
    if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
    const tag = TAG[commandId as keyof typeof TAG];
    if (!tag) return false;
    while (node && node instanceof HTMLElement) {
      if (node.tagName.toLowerCase() === tag) return true;
      node = node.parentElement;
    }
    return false;
  }) as typeof document.queryCommandState;
}

beforeEach(() => {
  document.body.innerHTML = "";
  // Italic collapses like Chrome; bold/underline keep the range.
  stubExecCommand(new Set(["italic"]));
});

describe("paletteToggleInlineMark selection restore", () => {
  it.each([
    ["italic", paletteItalic],
    ["bold", paletteBold],
    ["underline", paletteUnderline],
  ] as const)("%s keeps the highlight so a second click can undo", (_name, apply) => {
    const editor = mountEditor("How many angels?");
    selectWord(editor, "angels");
    expect(selectedText()).toBe("angels");

    apply();
    expect(selectedText()).toBe("angels");
    expect(window.getSelection()?.getRangeAt(0).collapsed).toBe(false);

    apply();
    expect(selectedText()).toBe("angels");
  });
});
