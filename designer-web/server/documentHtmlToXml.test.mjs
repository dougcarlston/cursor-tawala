import { describe, expect, it } from "vitest";
import { documentHtmlToXml } from "./documentHtmlToXml.mjs";

const escAttr = (s) =>
  String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
const escText = escAttr;

describe("documentHtmlToXml itemization", () => {
  it("exports MQL column fields as Record:Form:Field (legacy Java shape)", () => {
    const config = JSON.stringify({
      numberOfColumns: 2,
      column: [
        { header: "First Name", contents: "<<Form 1:First>>" },
        { header: "Last Name", contents: "<<Form 1:Last>>" },
      ],
      "show-print-control": "false",
      "show-export-control": "false",
    }).replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    const html =
      `<span class="function-token" data-function-id="itemization-table" ` +
      `data-function-config="${config}">fx</span>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('name="Record:Form 1:First"');
    expect(xml).toContain('name="Record:Form 1:Last"');
    expect(xml).not.toContain("&lt;&lt;Form 1:First");
    expect(xml).toContain('<form name="Form 1"/>');
  });

  it("exports placed Document text without <division> (Java Document ignores division)", () => {
    const config = JSON.stringify({
      numberOfColumns: 1,
      column: [{ header: "First", contents: "<<Form 1:First>>" }],
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    const html =
      `<p class="doc-placed-text" style="position: absolute; left: 52pt; top: 70pt">` +
      `<span class="function-token" data-function-id="itemization-table" ` +
      `data-function-config="${config}">fx</span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain("itemization-table");
    expect(xml).toContain('name="Record:Form 1:First"');
    expect(xml).not.toContain("<division");
    // Single font wrap only (nested font/font is dropped by Java Font FACTORY)
    expect(xml).toMatch(/<paragraph[^>]*><font><itemization-table/);
    expect(xml).not.toMatch(/<font>\s*<font>/);
  });
});
