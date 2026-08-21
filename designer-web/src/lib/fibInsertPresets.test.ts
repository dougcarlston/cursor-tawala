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

  it("Address preset is freeform with Street/City/Zip captions (no inline City:/Zip:)", () => {
    const item = createAddressFibPreset("FIB2");
    expect(item.style).toBe("freeform");
    expect(item.prompt).toMatch(/^Address:/);
    expect(item.prompt).not.toMatch(/City:/);
    expect(item.prompt).not.toMatch(/Zip:/);
    expect(item.prompt).not.toMatch(/<br>/);
    const runs = parseUnderscoreRuns(item.prompt!);
    expect(runs).toHaveLength(3);
    expect(item.blanks?.map((b) => b.caption)).toEqual(["Street", "City", "Zip"]);
    expect(item.blanks?.map((b) => b.alternateLabel)).toEqual(["Street", "City", "Zip"]);
    const synced = syncBlanksFromPrompt(item.prompt!, item.blanks!, "FIB2");
    expect(synced.map((b) => b.caption)).toEqual(["Street", "City", "Zip"]);
  });
});
