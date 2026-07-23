/**
 * Contract lock for Tomcat FIB layout CSS.
 * Source of truth: docker/tomcat/css/project/form-layout-core.css
 * (baked into the Tomcat image; CommonTheme loads it last).
 */
import { readFileSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";

const ROOT = resolve(dirname(fileURLToPath(import.meta.url)), "../..");
const CORE = resolve(ROOT, "docker/tomcat/css/project/form-layout-core.css");
const COMMON_THEME = resolve(
  ROOT,
  "TawalaWebapp-build1700/src/com/tawala/project/theme/CommonTheme.java",
);
const USER_THEME = resolve(
  ROOT,
  "TawalaWebapp-build1700/src/com/tawala/project/theme/UserDefinedTheme.java",
);
const DEFAULT_OVERLAY = resolve(
  ROOT,
  "docker/tomcat/css/project/default/project.css",
);

describe("form-layout-core.css (Deploy layout lock)", () => {
  const css = readFileSync(CORE, "utf8");
  const commonTheme = readFileSync(COMMON_THEME, "utf8");
  const userTheme = readFileSync(USER_THEME, "utf8");
  const overlay = readFileSync(DEFAULT_OVERLAY, "utf8");

  it("exists and documents the product contract", () => {
    expect(css).toMatch(/form-layout-core|Layout contracts/i);
    expect(css).toContain("Align right side");
    expect(css).toContain("--tawala-fib-label-width");
  });

  it("locks shared label column, even FIB margins, and justified fill", () => {
    expect(css).toMatch(/form table\.fib\s*\{[^}]*margin:\s*0 0 0\.75em 0\s*!important/s);
    expect(css).toMatch(/td\.label\s*\{[^}]*--tawala-fib-label-width/s);
    expect(css).toMatch(
      /form table\.fib\.justified[^\{]*td\.blank input\.text[\s\S]*?width:\s*100%\s*!important/,
    );
    expect(css).toMatch(/table\.remainder\s*\{[^}]*width:\s*100%\s*!important/s);
    expect(css).toMatch(/td\.blank[\s\S]*?min-width:\s*8ch\s*!important/);
    // Multi-row Align tables must not glue rows (Signup Sheet justified Deploy).
    expect(css).toMatch(
      /form table\.fib > (?:tbody > )?tr > td\s*\{[^}]*padding-top:\s*0\.45em\s*!important/s,
    );
    expect(css).toMatch(
      /form table\.fib table\.remainder td\s*\{[^}]*padding-top:\s*0\s*!important/s,
    );
  });

  it("locks freeform div.fib soft-row spacing (Signup Sheet Deploy)", () => {
    expect(css).toMatch(/form div\.fib:not\(\.vertical\)\s*\{[^}]*padding-bottom:\s*0\s*!important/s);
    expect(css).toMatch(
      /form div\.fib:not\(\.vertical\) > div\s*\{[^}]*display:\s*flex\s*!important/s,
    );
    expect(css).toMatch(
      /form div\.fib:not\(\.vertical\) > div\s*\{[^}]*margin:\s*0 0 0\.85em 0\s*!important/s,
    );
    expect(css).toMatch(/form div\.fib:not\(\.vertical\) > div > script/);
  });

  it("locks list/MQL tables to content-fit, nowrap, and 6in max", () => {
    expect(css).toContain("--tawala-list-table-max-width: 6in");
    expect(css).toMatch(/form table\.component[\s\S]*?width:\s*max-content\s*!important/);
    expect(css).toMatch(/form table\.component[\s\S]*?white-space:\s*nowrap\s*!important/);
    expect(css).toMatch(/max-width:\s*min\(var\(--tawala-list-table-max-width/);
  });

  it("is wired last in CommonTheme and re-appended after user themes", () => {
    expect(commonTheme).toContain('FORM_LAYOUT_CORE_CSS = "/css/project/form-layout-core.css"');
    expect(commonTheme).toContain("screenStylesheetURLs.add(FORM_LAYOUT_CORE_CSS)");
    expect(commonTheme).toMatch(
      /return defaultThemeCSS \+ "\\n" \+ themeSpecificCSS \+ "\\n" \+ layoutCoreCSS/,
    );
    expect(userTheme).toContain("result.remove(CommonTheme.FORM_LAYOUT_CORE_CSS)");
    expect(userTheme).toContain("result.add(CommonTheme.FORM_LAYOUT_CORE_CSS)");
  });

  it("keeps default theme overlay free of FIB geometry (core owns it)", () => {
    const withoutComments = overlay.replace(/\/\*[\s\S]*?\*\//g, "").trim();
    expect(withoutComments).not.toMatch(/form\s+table\.fib/);
    expect(withoutComments).not.toMatch(/table\.remainder/);
    expect(overlay).toMatch(/form-layout-core/);
  });
});
