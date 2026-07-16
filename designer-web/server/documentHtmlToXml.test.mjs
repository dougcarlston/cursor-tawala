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

describe("documentHtmlToXml nested function tokens", () => {
  it("keeps display-mcq when the token sits inside a styled span (palette face/size)", () => {
    const cfg =
      `{&quot;field-name&quot;:&quot;Form 1:Q1&quot;,&quot;display&quot;:&quot;all_choices&quot;}`;
    const html =
      `<p><span style="font-size: 12pt; font-family: Arial; color: #000000">` +
      `<span class="function-token" data-function-id="display-mcq-label" ` +
      `data-function-config="${cfg}">x</span></span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain("<display-mcq-label");
    expect(xml).toContain("<field-name>Form 1:Q1</field-name>");
    expect(xml).toContain("<display>all_choices</display>");
    // Single font wrapper — nested <font> would be dropped by Java.
    expect(xml).not.toMatch(/<font[^>]*>\s*<font/);
  });

  it("converts rgb() span colors to hex so Java Deploy does not reject the upload", () => {
    const cfg =
      `{&quot;field-name&quot;:&quot;Form 1:Q1&quot;,&quot;display&quot;:&quot;all_choices&quot;}`;
    const html =
      `<p><span style="font-size: 12pt; font-family: Arial; color: rgb(0, 0, 0)">` +
      `<span class="function-token" data-function-id="display-mcq-label" ` +
      `data-function-config="${cfg}">x</span></span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('color="000000"');
    expect(xml).not.toContain("rgb(");
    expect(xml).toContain("<display-mcq-label");
  });
});

describe("documentHtmlToXml embedded local image", () => {
  it("exports data-tawala-image-id img as <image id width height/>", () => {
    const html =
      `<p><img class="tawala-embedded-image" data-tawala-image-id="image1" ` +
      `data-image-width="12" data-image-height="8" width="12" height="8" ` +
      `src="data:image/png;base64,AAAA" alt=""/></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('<image id="image1" width="12" height="8"></image>');
    expect(xml).toContain('<font face="Arial" size="200" color="000000">');
    expect(xml).not.toContain("data:image");
    expect(xml).not.toContain("display-image");
  });

  it("ignores img without data-tawala-image-id", () => {
    const xml = documentHtmlToXml(
      `<p><img src="https://example.com/x.png" width="10" height="10"/></p>`,
      escAttr,
      escText,
    );
    expect(xml).not.toContain("<image ");
  });
});
