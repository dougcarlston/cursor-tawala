import { describe, expect, it } from "vitest";
import {
  ALL_PROJECT_THEMES,
  CURATED_PROJECT_THEME_PATHS,
  PROJECT_THEMES,
  projectThemesForMenu,
  themeLabelForPath,
} from "./projectThemes";

describe("projectThemes catalog", () => {
  it("lists the curated shortlist in alphabetical order", () => {
    const labels = PROJECT_THEMES.map((t) => t.label);
    expect(labels).toEqual([...labels].sort((a, b) => a.localeCompare(b)));
    expect(PROJECT_THEMES).toHaveLength(CURATED_PROJECT_THEME_PATHS.length);
    expect(labels).toContain("Big Q");
    expect(labels).toContain("Plain");
    expect(labels).not.toContain("Green Tea");
  });

  it("keeps the full legacy catalog for label lookup", () => {
    expect(ALL_PROJECT_THEMES).toHaveLength(28);
    expect(ALL_PROJECT_THEMES.map((t) => t.label)).toContain("Green Tea");
  });

  it("marks curated themes as hasLocalCss when docker CSS exists", () => {
    expect(PROJECT_THEMES.every((t) => t.hasLocalCss)).toBe(true);
    const byPath = Object.fromEntries(PROJECT_THEMES.map((t) => [t.path, t.hasLocalCss]));
    expect(byPath.style2).toBe(true);
    expect(byPath.plain).toBe(true);
  });

  it("resolves theme labels from paths including retired themes", () => {
    expect(themeLabelForPath("greentea")).toBe("Green Tea");
    expect(themeLabelForPath("style2")).toBe("Big Q");
    expect(themeLabelForPath(undefined)).toBe("Default");
  });

  it("prepends a legacy row when the project theme is not curated", () => {
    const menu = projectThemesForMenu("greentea");
    expect(menu[0].path).toBe("greentea");
    expect(menu[0].label).toBe("Green Tea (legacy)");
    expect(menu).toHaveLength(PROJECT_THEMES.length + 1);
  });

  it("does not duplicate when the project theme is curated", () => {
    const menu = projectThemesForMenu("style2");
    expect(menu).toHaveLength(PROJECT_THEMES.length);
    expect(menu.some((t) => t.path === "style2")).toBe(true);
  });
});
