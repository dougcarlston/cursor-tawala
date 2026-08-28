import { describe, expect, it } from "vitest";
import {
  createAddressFibPreset,
  createDateFibPreset,
} from "@/lib/fibInsertPresets";
import { parseUnderscoreRuns, syncBlanksFromPrompt } from "@/lib/fibBlanks";

describe("fibInsertPresets", () => {
  it("Date preset is freeform with three blanks and DOB trailing hint", () => {
    const item = createDateFibPreset("FIB1");
    expect(item.type).toBe("fib");
    expect(item.style).toBe("freeform");
    expect(item.prompt).toMatch(/\(mm\/dd\/yyyy\)/);
    const runs = parseUnderscoreRuns(item.prompt!);
    expect(runs).toHaveLength(3);
    expect(item.blanks).toHaveLength(3);
    expect(item.blanks?.map((b) => b.alternateLabel)).toEqual(["Month", "Day", "Year"]);
    const synced = syncBlanksFromPrompt(item.prompt!, item.blanks!, "FIB1");
    expect(synced.map((b) => b.alternateLabel)).toEqual(["Month", "Day", "Year"]);
  });

  it("Address preset is two lines with Street on line 1 and City/State/Zip on line 2", () => {
    const item = createAddressFibPreset("FIB2");
    expect(item.prompt).toMatch(/^Street:/);
    expect(item.prompt).toMatch(/<br>City, State, Zip:/);
    const runs = parseUnderscoreRuns(item.prompt!);
    expect(runs).toHaveLength(4);
    expect(item.blanks?.map((b) => b.alternateLabel)).toEqual(["Street", "City", "State", "Zip"]);
    const synced = syncBlanksFromPrompt(item.prompt!, item.blanks!, "FIB2");
    expect(synced.map((b) => b.caption)).toEqual(["Street Address", "City", "State", "Zip"]);
  });
});
