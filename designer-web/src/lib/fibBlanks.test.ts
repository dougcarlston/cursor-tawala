import { describe, expect, it } from "vitest";
import { parseUnderscoreRuns, syncBlanksFromPrompt } from "@/lib/fibBlanks";

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
