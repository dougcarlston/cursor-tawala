import { describe, expect, it } from "vitest";
import { buildScriptLines } from "@/lib/skipScript";
import type { SkipCommand } from "@/types/tawala";

describe("Skip script condition display", () => {
  it("shows friendly equals for imported mcEquals (not raw mcEquals)", () => {
    const commands: SkipCommand[] = [
      {
        cmd: "if",
        condition: {
          field: "Potluck Organizer:Q5",
          op: "mcEquals",
          value: "b",
        },
        then: [{ cmd: "skip", to: "T2" }],
      },
    ];
    const lines = buildScriptLines(commands);
    const header = lines.find((l) => l.lineType === "if-header");
    expect(header?.text).toBe('If Potluck Organizer:Q5 equals "b"');
    expect(header?.text).not.toContain("mcEquals");
  });
});
