/**
 * Toolbar/menu Paste cannot use execCommand("paste") — browsers block it.
 * Shell paste restores the palette editor caret and inserts via Clipboard API or in-app clipboard.
 */
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { setActivePaletteEditor, clearActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { pasteIntoActiveEditor, runShellEditCommand } from "@/lib/shellCommands";
import { setTextClipboard, getTextClipboard } from "@/lib/textClipboard";
import { useProjectStore } from "@/store/projectStore";

describe("shell cut/copy/paste in Form Text (palette editor)", () => {
  let editor: HTMLDivElement;
  let committed: string[];
  let savedRange: Range | null = null;

  beforeEach(() => {
    useProjectStore.getState().newProject({ empty: true });
    useProjectStore.getState().addForm();
    const formName = useProjectStore.getState().project.forms[0]!.name;
    useProjectStore.getState().openWindow("form", formName);
    useProjectStore.setState({ editorTab: "design" });

    committed = [];
    editor = document.createElement("div");
    editor.contentEditable = "true";
    editor.innerHTML = "Hello world";
    document.body.appendChild(editor);
    editor.focus();

    const textNode = editor.firstChild as Text;
    const range = document.createRange();
    range.setStart(textNode, 0);
    range.setEnd(textNode, 5); // "Hello"
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    savedRange = range.cloneRange();
    setActivePaletteEditor({
      el: editor,
      commit: () => {
        committed.push(editor.innerHTML);
      },
      saveSelection: () => {
        const s = window.getSelection();
        if (s && s.rangeCount > 0) savedRange = s.getRangeAt(0).cloneRange();
      },
      restoreSelection: () => {
        const s = window.getSelection();
        if (!s || !savedRange) return;
        s.removeAllRanges();
        s.addRange(savedRange.cloneRange());
      },
    });

    Object.defineProperty(navigator, "clipboard", {
      configurable: true,
      value: {
        readText: async () => " world",
        writeText: async () => {},
        read: async () => [],
      },
    });
  });

  afterEach(() => {
    clearActivePaletteEditor(editor);
    editor.remove();
    setTextClipboard(null);
  });

  it("pasteIntoActiveEditor inserts clipboard text at the caret and commits", async () => {
    const ok = await pasteIntoActiveEditor();
    expect(ok).toBe(true);
    expect(editor.textContent).toContain("world");
    expect(committed.length).toBeGreaterThan(0);
  });

  it("runShellEditCommand('copy') captures highlighted text to in-app text clipboard", () => {
    expect(runShellEditCommand("copy")).toBe(true);
    const clip = getTextClipboard();
    expect(clip?.text).toBe("Hello");
    expect(useProjectStore.getState().statusMessage).toBe("Copied text");
  });

  it("runShellEditCommand('cut') cuts highlighted text and commits", () => {
    expect(runShellEditCommand("cut")).toBe(true);
    const clip = getTextClipboard();
    expect(clip?.text).toBe("Hello");
    expect(editor.textContent).toBe(" world");
    expect(committed.length).toBeGreaterThan(0);
    expect(useProjectStore.getState().statusMessage).toBe("Cut text");
  });

  it("runShellEditCommand('paste') after cut prefers in-app text over stale system clipboard", async () => {
    Object.defineProperty(navigator, "clipboard", {
      configurable: true,
      value: {
        readText: async () => "STALE",
        writeText: async () => {},
        read: async () => [],
        write: async () => {},
      },
    });

    expect(runShellEditCommand("cut")).toBe(true);
    expect(editor.textContent).toBe(" world");

    const textNode = editor.firstChild as Text;
    const range = document.createRange();
    range.setStart(textNode, 0);
    range.collapse(true);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);
    savedRange = range.cloneRange();

    expect(runShellEditCommand("paste")).toBe(true);
    await vi.waitFor(() => {
      expect(editor.textContent).toBe("Hello world");
      expect(editor.textContent).not.toContain("STALE");
    });
  });

  it("runShellEditCommand('paste') falls back to in-app text clipboard when clipboard readText throws", async () => {
    Object.defineProperty(navigator, "clipboard", {
      configurable: true,
      value: {
        readText: async () => {
          throw new Error("Permission denied");
        },
        read: async () => {
          throw new Error("Permission denied");
        },
      },
    });

    setTextClipboard({ text: "Super" });
    expect(runShellEditCommand("paste")).toBe(true);
    await vi.waitFor(() => {
      expect(editor.textContent).toContain("Super");
      expect(useProjectStore.getState().statusMessage).toBe("Pasted text");
    });
  });
});
