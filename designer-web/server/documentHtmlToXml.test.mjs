import { describe, expect, it } from "vitest";
import { documentHtmlToXml } from "./documentHtmlToXml.mjs";

const escAttr = (s) =>
  String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
const escText = escAttr;

function tokenHtml(id, config) {
  const encoded = JSON.stringify(config)
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
  return (
    `<p><span class="function-token" data-function-id="${id}" ` +
    `data-function-config="${encoded}">fx</span></p>`
  );
}

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
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
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

/** Locks Document HTML→XML emit vs deferred stubs (cleanup Jul 16). */
describe("documentHtmlToXml function emit matrix", () => {
  const cases = [
    ["record-count", { "form-name": "Survey" }, "record-count"],
    ["sum", { field: "<<Survey:Count>>" }, "sum"],
    ["project-email-count", {}, "project-email-count"],
    ["display-image", { source: "https://example.com/a.png", width: "200" }, "display-image"],
    ["display-mcq-label", { "field-name": "<<Survey:Q1>>", display: "label_only" }, "display-mcq-label"],
    ["choice-tally-table", { field: "<<Survey:Q1>>" }, "choice-tally-table"],
    ["response-totals-table", { field: "<<Survey:Q1>>", "layout-type": "vertical" }, "response-totals-table"],
    [
      "question-correlation-table",
      {
        "question-field-name": "<<Survey:Dates>>",
        "display-field-name": "<<Survey:Name>>",
        "preferred-choice-field-name": "<<Survey:Preferred>>",
      },
      "question-correlation-table",
    ],
    ["popular-choice-display", { rank: "1", "popular-choice-field-name": "<<Survey:Q1>>" }, "popular-choice-display"],
    ["popular-choice-count", { rank: "1", "popular-choice-field-name": "<<Survey:Q1>>" }, "popular-choice-count"],
    [
      "popular-choice-correlation-table",
      {
        rank: "1",
        "choice-available-field-name": "<<Survey:A>>",
        "choice-preferred-field-name": "<<Survey:B>>",
        "popular-choice-display-field-name": "<<Survey:Name>>",
      },
      "popular-choice-correlation-table",
    ],
    ["simple-list", { "simple-list-field": "<<Survey:Name>>" }, "simple-list"],
    [
      "itemization-table",
      { numberOfColumns: 1, column: [{ header: "N", contents: "<<Survey:Name>>" }] },
      "itemization-table",
    ],
  ];

  for (const [id, config, tag] of cases) {
    it(`emits real <${tag}> for ${id}`, () => {
      const xml = documentHtmlToXml(tokenHtml(id, config), escAttr, escText);
      expect(xml).toContain(`<${tag}`);
      expect(xml).not.toContain(`<!-- function ${id} -->`);
    });
  }

  for (const id of [
    "categorizer",
    "export-team-roster",
    "link-to-project-details",
    "paypal-single-item-button",
  ]) {
    it(`defers ${id} as XML comment stub`, () => {
      const xml = documentHtmlToXml(tokenHtml(id, {}), escAttr, escText);
      expect(xml).toContain(`<!-- function ${id} -->`);
    });
  }
});
