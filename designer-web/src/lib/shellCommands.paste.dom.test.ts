/**
 * Toolbar/menu Paste cannot use execCommand("paste") — browsers block it.
 * Shell paste restores the palette editor caret and inserts via Clipboard API.
 */
import { afterEach, beforeEach, describe, expect, it, vi } from "vitest";
import { setActivePaletteEditor, clearActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { pasteIntoActiveEditor, runShellEditCommand } from "@/lib/shellCommands";
import { useProjectStore } from "@/store/projectStore";

describe("shell paste into Form Text (palette editor)", () => {
  let editor: HTMLDivElement;
  let committed: string[];

  beforeEach(() => {
    useProjectStore.getState().newProject({ empty: true });
    useProjectStore.getState().addForm();
    const formName = useProjectStore.getState().project.forms[0]!.name;
    useProjectStore.getState().openWindow("form", formName);
    useProjectStore.setState({ editorTab: "design" });

    committed = [];
    editor = document.createElement("div");
    editor.contentEditable = "true";
    editor.innerHTML = "Hello";
    document.body.appendChild(editor);
    editor.focus();
    const range = document.createRange();
    range.selectNodeContents(editor);
    range.collapse(false);
    const sel = window.getSelection();
    sel?.removeAllRanges();
    sel?.addRange(range);

    const saved = range.cloneRange();
    setActivePaletteEditor({
      el: editor,
      commit: () => {
        committed.push(editor.innerHTML);
      },
      saveSelection: () => {},
      restoreSelection: () => {
        const s = window.getSelection();
        if (!s) return;
        s.removeAllRanges();
        s.addRange(saved.cloneRange());
      },
    });

    Object.defineProperty(navigator, "clipboard", {
      configurable: true,
      value: {
        readText: async () => " world",
        read: async () => [],
      },
    });
  });

  afterEach(() => {
    clearActivePaletteEditor(editor);
    editor.remove();
  });

  it("pasteIntoActiveEditor inserts clipboard text at the caret and commits", async () => {
    const ok = await pasteIntoActiveEditor();
    expect(ok).toBe(true);
    expect(editor.textContent).toContain("world");
    expect(committed.length).toBeGreaterThan(0);
  });

  it("runShellEditCommand('paste') kicks off clipboard paste when edit context is active", async () => {
    Object.defineProperty(navigator, "clipboard", {
      configurable: true,
      value: {
        readText: async () => "XYZ",
        read: async () => [],
      },
    });

    expect(runShellEditCommand("paste")).toBe(true);
    await vi.waitFor(() => {
      expect(editor.textContent).toContain("XYZ");
    });
  });
});
