/**
 * @vitest-environment happy-dom
 *
 * Form Text hint text is a bare text node (no P/DIV). First Face/Size/B/I/U must keep
 * the select-all highlight; Indent must wrap a DIV and apply margin (Align already did).
 */
import { afterEach, describe, expect, it } from "vitest";
import { TEXT_PLACEHOLDER } from "@/types/tawala";
import {
  clearActivePaletteEditor,
  clearFormattingFocus,
  getActivePaletteEditor,
  getFormattingFocusState,
  setActivePaletteEditor,
  setFormattingFocus,
} from "@/lib/formattingPaletteContext";
import {
  paletteFontFace,
  paletteFontSize,
  paletteIndent,
  paletteAlign,
} from "@/lib/paletteCommands";

function mountEditor(html: string) {
  const el = document.createElement("div");
  el.contentEditable = "true";
  el.innerHTML = html;
  document.body.appendChild(el);
  el.focus();
  const range = document.createRange();
  range.selectNodeContents(el);
  const sel = window.getSelection()!;
  sel.removeAllRanges();
  sel.addRange(range);
  let saved = range.cloneRange();
  setActivePaletteEditor({
    el,
    commit: () => {},
    saveSelection: () => {
      const s = window.getSelection();
      if (!s || s.rangeCount === 0) return;
      const r = s.getRangeAt(0);
      if (el.contains(r.commonAncestorContainer) || r.commonAncestorContainer === el) {
        saved = r.cloneRange();
      }
    },
    restoreSelection: () => {
      const s = window.getSelection();
      if (!s) return;
      if (!(el.contains(saved.commonAncestorContainer) || saved.commonAncestorContainer === el)) {
        return;
      }
      s.removeAllRanges();
      s.addRange(saved);
    },
  });
  setFormattingFocus({ kind: "text", cursorInTable: false, hasResettableFormatting: false });
  return el;
}

function selectedText(): string {
  return window.getSelection()?.toString() ?? "";
}

afterEach(() => {
  clearActivePaletteEditor();
  clearFormattingFocus();
  document.body.innerHTML = "";
});

describe("palette format on Text hint text", () => {
  it("applies font face to select-all hint and keeps the highlight", () => {
    const el = mountEditor(TEXT_PLACEHOLDER);
    paletteFontFace("Georgia");
    expect(el.innerHTML.toLowerCase()).toMatch(/georgia/);
    expect(selectedText()).toBe(TEXT_PLACEHOLDER);
  });

  it("applies font size to select-all hint and keeps the highlight", () => {
    const el = mountEditor(TEXT_PLACEHOLDER);
    paletteFontSize("18");
    expect(el.innerHTML).toMatch(/18\s*pt|font-size/i);
    expect(selectedText()).toBe(TEXT_PLACEHOLDER);
  });

  it("still formats after collapsing then re-selecting hint", () => {
    const el = mountEditor(TEXT_PLACEHOLDER);
    const sel = window.getSelection()!;
    sel.collapse(el.firstChild!, 0);
    const range = document.createRange();
    range.selectNodeContents(el);
    sel.removeAllRanges();
    sel.addRange(range);
    getActivePaletteEditor()?.saveSelection();
    paletteFontFace("Courier New");
    expect(el.innerHTML.toLowerCase()).toMatch(/courier/);
    expect(selectedText().length).toBeGreaterThan(0);
  });

  it("indents flat hint text by wrapping a DIV with margin-left", () => {
    const el = mountEditor(TEXT_PLACEHOLDER);
    paletteIndent();
    const div = el.querySelector(":scope > div");
    expect(div).toBeTruthy();
    expect((div as HTMLElement).style.marginLeft).toMatch(/pt|px/);
    expect(el.textContent).toBe(TEXT_PLACEHOLDER);
  });

  it("align center wraps a block so a following face keeps working on the hint", () => {
    const el = mountEditor(TEXT_PLACEHOLDER);
    paletteAlign("center");
    const block = el.querySelector(":scope > div, :scope > p") as HTMLElement | null;
    expect(block).toBeTruthy();
    expect(block!.style.textAlign).toBe("center");
    getActivePaletteEditor()?.saveSelection();
    // Re-select all after align (execCommand may or may not keep highlight in happy-dom).
    const range = document.createRange();
    range.selectNodeContents(el);
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);
    getActivePaletteEditor()?.saveSelection();
    paletteFontFace("Georgia");
    expect(el.innerHTML.toLowerCase()).toMatch(/georgia/);
  });
});

describe("clearFormattingFocus ownership", () => {
  it("does not clear when another editor owns the active handle", () => {
    const a = document.createElement("div");
    const b = document.createElement("div");
    document.body.append(a, b);
    setActivePaletteEditor({
      el: b,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });
    setFormattingFocus({ kind: "text", cursorInTable: false, hasResettableFormatting: false });
    clearFormattingFocus("text", a);
    expect(getFormattingFocusState().kind).toBe("text");
    clearFormattingFocus("text", b);
    expect(getFormattingFocusState().kind).toBe("none");
  });
});
