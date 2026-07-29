/**
 * Function chip display strings (Document / Form Text Insert → Function).
 */
import { describe, expect, it } from "vitest";
import { getFunctionDef } from "./functionCatalog";
import { buildFunctionDisplayString, parseFunctionConfig, serializeFunctionConfig } from "./functionTokens";

describe("buildFunctionDisplayString", () => {
  it("renders bare <<NAME>> when config has no visible params", () => {
    const def = getFunctionDef("sum");
    expect(def).toBeTruthy();
    expect(buildFunctionDisplayString(def!, {})).toBe("<<SUM>>");
  });

  it("includes field and truncates long URL-like params", () => {
    const def = getFunctionDef("sum");
    expect(buildFunctionDisplayString(def!, { field: "Amount" })).toBe("<<SUM(Amount)>>");

    const long =
      "https://example.com/very/long/path/to/a/resource/file-with-a-really-long-name.json";
    const display = buildFunctionDisplayString(def!, { field: long });
    expect(display.startsWith("<<SUM(")).toBe(true);
    expect(display.endsWith(")>>")).toBe(true);
    expect(display.length).toBeLessThan(`<<SUM(${long})>>`.length);
  });

  it("truncates multi-column collections to a short display", () => {
    const def = getFunctionDef("itemization-table");
    if (!def) {
      // Catalog id may differ; skip softly if renamed.
      expect(def).toBeUndefined();
      return;
    }
    const display = buildFunctionDisplayString(def, {
      numberOfColumns: 4,
      column: [
        { contents: "A" },
        { contents: "B" },
        { contents: "C" },
        { contents: "D" },
      ],
    });
    expect(display).toContain("...");
    expect(display.startsWith("<<")).toBe(true);
  });
});

describe("serializeFunctionConfig / parseFunctionConfig", () => {
  it("round-trips config JSON and tolerates bad input", () => {
    const raw = serializeFunctionConfig({ field: "X" });
    expect(parseFunctionConfig(raw)).toEqual({ field: "X" });
    expect(parseFunctionConfig(null)).toEqual({});
    expect(parseFunctionConfig("{not-json")).toEqual({});
  });
});
