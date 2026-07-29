import { describe, expect, it } from "vitest";
import {
  FIB_DEFAULT_PROMPT,
  FIB_DEFAULT_UNDERSCORES,
  FIB_PLACEHOLDER,
} from "@/types/tawala";
import {
  fibHintHighlightEnd,
  parseUnderscoreRuns,
  syncBlanksFromPrompt,
} from "@/lib/fibBlanks";

describe("FIB underscore → blanks (Design metadata)", () => {
  it("parses underscore runs for blank length", () => {
    const runs = parseUnderscoreRuns("Name ________");
    expect(runs).toHaveLength(1);
    expect(runs[0].length).toBe(8);
  });

  it("syncs blank count and length from underscore runs", () => {
    const blanks = syncBlanksFromPrompt("Name ________ lives in ______.", [], "FIB1");
    expect(blanks).toHaveLength(2);
    expect(blanks[0]).toMatchObject({ name: "a", length: 8, alternateLabel: "FIB1:a" });
    expect(blanks[1]).toMatchObject({ name: "b", length: 6, alternateLabel: "FIB1:b" });
  });

  it("preserves existing blank attributes when run count is unchanged", () => {
    const existing = [
      {
        name: "a",
        length: 3,
        alternateLabel: "Surveyee",
        required: true,
        height: 2,
      },
    ];
    const blanks = syncBlanksFromPrompt("Name ________", existing, "FIB1");
    expect(blanks).toHaveLength(1);
    expect(blanks[0]).toMatchObject({
      name: "a",
      length: 8,
      alternateLabel: "Surveyee",
      required: true,
      height: 2,
    });
  });
});

describe("fibHintHighlightEnd", () => {
  it("selects only the stock hint — not trailing underscores (space-separated)", () => {
    expect(fibHintHighlightEnd(FIB_DEFAULT_PROMPT)).toBe(FIB_PLACEHOLDER.length);
    expect(FIB_DEFAULT_PROMPT.slice(fibHintHighlightEnd(FIB_DEFAULT_PROMPT)!)).toBe(
      ` ${FIB_DEFAULT_UNDERSCORES}`,
    );
  });

  it("selects only the stock hint when blanks wrap onto the next line", () => {
    const plain = `${FIB_PLACEHOLDER}\n${FIB_DEFAULT_UNDERSCORES}`;
    expect(fibHintHighlightEnd(plain)).toBe(FIB_PLACEHOLDER.length);
  });

  it("returns null for customized question prose with blanks", () => {
    expect(fibHintHighlightEnd("Name ________")).toBeNull();
    expect(fibHintHighlightEnd(`${FIB_PLACEHOLDER} Extra prose ________`)).toBeNull();
  });
});
