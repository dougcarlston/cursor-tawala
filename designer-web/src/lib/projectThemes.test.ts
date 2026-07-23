import { describe, expect, it } from "vitest";
import { PROJECT_THEMES, themeLabelForPath } from "./projectThemes";

describe("projectThemes catalog", () => {
  it("lists the legacy screenshot themes in alphabetical order", () => {
    const labels = PROJECT_THEMES.map((t) => t.label);
    expect(labels).toEqual([...labels].sort((a, b) => a.localeCompare(b)));
    expect(labels).toContain("Default");
    expect(labels).toContain("Green Tea");
    expect(labels).toContain("Dirtbowl - Variable Width");
    expect(labels).toHaveLength(28);
  });

  it("marks local Tomcat CSS themes as hasLocalCss", () => {
    const byPath = Object.fromEntries(PROJECT_THEMES.map((t) => [t.path, t.hasLocalCss]));
    expect(byPath.default).toBe(true);
    expect(byPath.baseball).toBe(true);
    expect(byPath.greentea).toBe(true);
    expect(byPath.dirtbowl2).toBe(true);
    expect(byPath.style2).toBe(true); // Big Q
    expect(byPath.mvsc).toBe(true);
    expect(byPath.redrays).toBe(true);
    expect(byPath.basicblue).toBe(false);
    expect(byPath.dirtbowl).toBe(false);
  });

  it("resolves theme labels from paths", () => {
    expect(themeLabelForPath("greentea")).toBe("Green Tea");
    expect(themeLabelForPath("style2")).toBe("Big Q");
    expect(themeLabelForPath(undefined)).toBe("Default");
  });
});
