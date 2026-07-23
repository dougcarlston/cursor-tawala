import { describe, expect, it } from "vitest";
import { resolveTheme, themeBodyClass, getThemeCss, BASE_FORM_CSS } from "./index.mjs";

describe("resolveTheme", () => {
  it("falls back missing baseball to default name + matching body class", () => {
    const resolved = resolveTheme("baseball");
    expect(resolved.name).toBe("default");
    expect(themeBodyClass("baseball")).toBe("tawala-theme-default");
    expect(resolved.css).toContain("body.tawala-theme-default");
    expect(getThemeCss("baseball")).toBe(resolved.css);
  });

  it("keeps dirtbowl2 when CSS file exists", () => {
    const resolved = resolveTheme("dirtbowl2");
    expect(resolved.name).toBe("dirtbowl2");
    expect(themeBodyClass("dirtbowl2")).toBe("tawala-theme-dirtbowl2");
    expect(resolved.css).toContain("body.tawala-theme-dirtbowl2");
  });

  it("exports base form readability CSS", () => {
    expect(BASE_FORM_CSS).toContain("font-family");
    expect(BASE_FORM_CSS).toContain("fieldset.mc");
    expect(BASE_FORM_CSS).toContain("input[type=submit]");
  });

  it("matches Design canvas 13px body size (not browser-default 16px)", () => {
    expect(BASE_FORM_CSS).toMatch(/html\s*\{[^}]*font-size:\s*13px/);
    expect(BASE_FORM_CSS).toMatch(/body\s*\{[^}]*font-size:\s*13px/);
    expect(BASE_FORM_CSS).toContain("text-size-adjust: 100%");
    expect(BASE_FORM_CSS).toMatch(
      /input\[type=text\][^}]*font-size:\s*100%/,
    );
  });

  it("stacks justified FIB items and fills the shared field column (Deploy parity)", () => {
    expect(BASE_FORM_CSS).toContain("display: block");
    expect(BASE_FORM_CSS).toContain("fib-style-leftAlignLabelsJustified");
    expect(BASE_FORM_CSS).toContain("preview-itemization-table table");
    expect(BASE_FORM_CSS).toMatch(
      /\.preview-itemization-table table\s*\{[^}]*width:\s*max-content/,
    );
    expect(BASE_FORM_CSS).toContain("--tawala-list-table-max-width");
  });

  it("includes Text Instructional / Error styles for Preview + Documents", () => {
    expect(BASE_FORM_CSS).toContain("div.text.instructional");
    expect(BASE_FORM_CSS).toContain("div.text.text-item-error");
    expect(BASE_FORM_CSS).toContain("color: #000080");
    expect(BASE_FORM_CSS).toContain("color: #c00000");
  });
});
