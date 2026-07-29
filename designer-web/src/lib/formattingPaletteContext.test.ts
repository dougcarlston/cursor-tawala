/**
 * Formatting Palette enable rules — Document vs Form Text vs FIB.
 */
import { describe, expect, it } from "vitest";
import {
  isPaletteControlEnabled,
  type FormattingFocusState,
  type PaletteControlId,
} from "./formattingPaletteContext";

const doc: FormattingFocusState = {
  kind: "document",
  cursorInTable: false,
  hasResettableFormatting: false,
};

const docInTable: FormattingFocusState = {
  ...doc,
  cursorInTable: true,
};

const text: FormattingFocusState = {
  kind: "text",
  cursorInTable: false,
  hasResettableFormatting: false,
};

function enabled(
  control: PaletteControlId,
  state: FormattingFocusState,
  forms = 0,
  design = true,
): boolean {
  return isPaletteControlEnabled(control, state, design, forms);
}

describe("isPaletteControlEnabled", () => {
  it("greys the whole palette off Design / with no editor focus", () => {
    expect(enabled("bold", doc, 1, false)).toBe(false);
    expect(enabled("bold", { kind: "none", cursorInTable: false, hasResettableFormatting: false })).toBe(
      false,
    );
    expect(
      enabled("bold", { kind: "heading", cursorInTable: false, hasResettableFormatting: false }),
    ).toBe(false);
  });

  it("limits FIB/MCQ to bold/italic/underline", () => {
    const fib: FormattingFocusState = {
      kind: "fib",
      cursorInTable: false,
      hasResettableFormatting: false,
    };
    expect(enabled("bold", fib)).toBe(true);
    expect(enabled("insertTable", fib)).toBe(false);
    expect(enabled("fx", fib)).toBe(false);
  });

  it("disables Insert Table while the caret is inside a table (no nesting)", () => {
    expect(enabled("insertTable", doc)).toBe(true);
    expect(enabled("insertTable", docInTable)).toBe(false);
    expect(enabled("insertTable", text)).toBe(true);
    expect(enabled("insertTable", { ...text, cursorInTable: true })).toBe(false);
  });

  it("requires cursorInTable for deleteTable / row-col tools", () => {
    expect(enabled("deleteTable", doc)).toBe(false);
    expect(enabled("tableRowCol", doc)).toBe(false);
    expect(enabled("deleteTable", docInTable)).toBe(true);
    expect(enabled("tableRowCol", docInTable)).toBe(true);
  });

  it("enables Document fx only when the project has at least one form", () => {
    expect(enabled("fx", doc, 0)).toBe(false);
    expect(enabled("fx", doc, 1)).toBe(true);
    // Form Text always allows fx (even with zero forms in project).
    expect(enabled("fx", text, 0)).toBe(true);
  });

  it("enables Face/Size/Bold for document and text surfaces", () => {
    for (const control of ["fontFace", "fontSize", "bold", "alignment"] as const) {
      expect(enabled(control, doc, 1)).toBe(true);
      expect(enabled(control, text, 0)).toBe(true);
    }
  });
});
