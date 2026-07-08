/**
 * Formatting Palette command execution (row 2 → active rich-text editor).
 *
 * The palette lives in the app shell; the focused editor registers a `PaletteEditorHandle`
 * (see `formattingPaletteContext`). These helpers apply `document.execCommand` formatting to
 * that editor's selection, then commit the result back to the store.
 *
 * Selection handling: plain buttons keep the editor's selection alive by calling
 * `preventDefault` on mousedown, so the caret never leaves the editor. Dropdowns / the color
 * picker DO take focus, so they save the selection first and we restore it here before applying.
 */

import { getActivePaletteEditor, type PaletteEditorHandle } from "./formattingPaletteContext";

export interface PaletteActiveState {
  bold: boolean;
  italic: boolean;
  underline: boolean;
  align: "left" | "center" | "right" | "justify";
}

const ALIGN_COMMAND: Record<PaletteActiveState["align"], string> = {
  left: "justifyLeft",
  center: "justifyCenter",
  right: "justifyRight",
  justify: "justifyFull",
};

function selectionInside(el: HTMLElement): boolean {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return false;
  return el.contains(sel.getRangeAt(0).commonAncestorContainer);
}

/** Focus the active editor, make sure a selection lives inside it, then run `fn` and commit. */
function withEditor(fn: (handle: PaletteEditorHandle) => void, styleWithCss = true): void {
  const handle = getActivePaletteEditor();
  if (!handle) return;
  handle.el.focus();
  if (!selectionInside(handle.el)) handle.restoreSelection();
  try {
    document.execCommand("styleWithCSS", false, String(styleWithCss));
  } catch {
    /* not supported — ignore */
  }
  fn(handle);
  handle.commit();
}

function exec(command: string, value?: string, styleWithCss = true): void {
  withEditor(() => {
    document.execCommand(command, false, value);
  }, styleWithCss);
}

export function paletteBold(): void {
  exec("bold");
}

export function paletteItalic(): void {
  exec("italic");
}

export function paletteUnderline(): void {
  exec("underline");
}

export function paletteReset(): void {
  exec("removeFormat");
}

export function paletteIndent(): void {
  exec("indent");
}

export function paletteOutdent(): void {
  exec("outdent");
}

export function paletteAlign(dir: PaletteActiveState["align"]): void {
  exec(ALIGN_COMMAND[dir]);
}

export function paletteFontColor(color: string): void {
  exec("foreColor", color);
}

export function paletteFontFace(face: string): void {
  // "Default Font" clearing is deferred (would need to strip inherited font runs); applying a
  // named face is the common case. Choosing Default is a no-op for now.
  if (face === "Default Font") return;
  exec("fontName", face);
}

/**
 * Apply a point size. `execCommand("fontSize")` only takes the legacy 1–7 scale, so we emit a
 * `<font size="7">` marker (styleWithCSS off) and rewrite it to an explicit `font-size` in pt.
 */
export function paletteFontSize(size: string): void {
  if (size === "Default Size") return;
  withEditor((handle) => {
    document.execCommand("fontSize", false, "7");
    handle.el.querySelectorAll('font[size="7"]').forEach((node) => {
      node.removeAttribute("size");
      (node as HTMLElement).style.fontSize = `${size}pt`;
    });
  }, false);
}

/** Current B/I/U + alignment of the selection, for the palette's pressed/active styling. */
export function readPaletteActiveState(): PaletteActiveState {
  const query = (command: string): boolean => {
    try {
      return document.queryCommandState(command);
    } catch {
      return false;
    }
  };
  let align: PaletteActiveState["align"] = "left";
  if (query("justifyCenter")) align = "center";
  else if (query("justifyRight")) align = "right";
  else if (query("justifyFull")) align = "justify";
  return {
    bold: query("bold"),
    italic: query("italic"),
    underline: query("underline"),
    align,
  };
}
