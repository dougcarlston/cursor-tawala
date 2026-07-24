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

  it("does not nest font when a styled wrapper span surrounds a placed MQL token", () => {
    const config = JSON.stringify({
      numberOfColumns: 3,
      column: [
        { header: "Name", contents: "<<Form 1:Name>>" },
        { header: "All Possibilities", contents: "<<Form 1:MCQ4>>" },
        { header: "My Faves", contents: "<<Form 1:MCQ1>>" },
      ],
      "show-print-control": "false",
      "show-export-control": "false",
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    const html =
      `<p class="doc-placed-text" style="position: absolute; left: 36pt; top: 40pt">` +
      `<span style="font-size: 12pt; font-family: Arial">` +
      `<span class="function-token" data-function-id="itemization-table" ` +
      `data-function-config="${config}">fx</span></span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain("itemization-table");
    expect(xml).toContain('name="Record:Form 1:MCQ4"');
    expect(xml).not.toMatch(/<font>\s*<font>/);
  });

  it("exports two identical placed MQL tokens with the same column XML", () => {
    const config = JSON.stringify({
      numberOfColumns: 3,
      column: [
        { header: "Name", contents: "<<Form 1:Name>>" },
        { header: "All Possibilities", contents: "<<Form 1:MCQ4>>" },
        { header: "My Faves", contents: "<<Form 1:MCQ1>>" },
      ],
      "show-print-control": "false",
      "show-export-control": "false",
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    const token =
      `<span class="function-token" data-function-id="itemization-table" ` +
      `data-function-config="${config}">fx</span>`;
    const html =
      `<p class="doc-placed-text" style="position: absolute; left: 36pt; top: 40pt">${token}</p>` +
      `<p class="doc-placed-text" style="position: absolute; left: 36pt; top: 120pt">${token}</p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    const matches = xml.match(/<itemization-table[\s\S]*?<\/itemization-table>/g) ?? [];
    expect(matches).toHaveLength(2);
    expect(matches[0]).toBe(matches[1]);
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

  it("emits simple-list version 2 (Java rejects version 1)", () => {
    const xml = documentHtmlToXml(
      tokenHtml("simple-list", { "simple-list-field": "<<Form 1:Name>>" }),
      escAttr,
      escText,
    );
    expect(xml).toContain('<simple-list version="2">');
    expect(xml).not.toContain('<simple-list version="1">');
  });

  it("emits Record:Form:Field for popular-choice-correlation-table", () => {
    const xml = documentHtmlToXml(
      tokenHtml("popular-choice-correlation-table", {
        rank: "1",
        "choice-available-field-name": "Form 1:MCQ1",
        "choice-preferred-field-name": "Form 1:MCQ2",
        "popular-choice-display-field-name": "Form 1:FIB1:a",
      }),
      escAttr,
      escText,
    );
    expect(xml).toContain("<popular-choice-correlation-table");
    expect(xml).toContain(
      "<choice-available-field-name>Record:Form 1:MCQ1</choice-available-field-name>",
    );
    expect(xml).toContain(
      "<choice-preferred-field-name>Record:Form 1:MCQ2</choice-preferred-field-name>",
    );
    expect(xml).toContain(
      "<popular-choice-display-field-name>Record:Form 1:FIB1:a</popular-choice-display-field-name>",
    );
  });

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

describe("documentHtmlToXml record-count Where conditions", () => {
  it("emits Record:Form:Field and isNotBlank for Deploy", () => {
    const xml = documentHtmlToXml(
      tokenHtml("record-count", {
        "form-name": "Registration",
        conditionsRows: [{ field: "<<Registration:WaiverReceived>>", op: "isNotBlank", value: "" }],
        conditionsCombinator: "and",
      }),
      escAttr,
      escText,
    );
    expect(xml).toContain("<record-count");
    expect(xml).toContain('field="Record:Registration:WaiverReceived"');
    expect(xml).toContain("<isNotBlank ");
    expect(xml).toMatch(
      /<conditions><form name="Registration"\/><conditions><isNotBlank field="Record:Registration:WaiverReceived"\/>/,
    );
  });

  it("qualifies FIB Item:blank (FIB1:a) with form-name for Deploy", () => {
    // Fields palette inserts FIB1:a (already has ':') without Form 1: prefix.
    const xml = documentHtmlToXml(
      tokenHtml("record-count", {
        "form-name": "Form 1",
        conditionsRows: [{ field: "FIB1:a", op: "equals", value: "Bogus" }],
        conditionsCombinator: "and",
      }),
      escAttr,
      escText,
    );
    expect(xml).toContain('field="Record:Form 1:FIB1:a"');
    expect(xml).not.toContain('field="Record:FIB1:a"');
    expect(xml).toContain("<string value=\"Bogus\"/>");
  });

  it("nests multi-row Where with binary <and> (DirtBowl shape)", () => {
    const xml = documentHtmlToXml(
      tokenHtml("record-count", {
        "form-name": "Registration",
        conditionsRows: [
          { field: "Registration:PaymentReceived", op: "equals", value: "Yes" },
          { field: "Registration:WaiverReceived", op: "equals", value: "Yes" },
          { field: "Registration:ShirtSize", op: "isBlank", value: "" },
        ],
        conditionsCombinator: "and",
      }),
      escAttr,
      escText,
    );
    expect(xml).toContain("<and>");
    expect(xml).toContain('field="Record:Registration:PaymentReceived"');
    expect(xml).toContain('field="Record:Registration:WaiverReceived"');
    expect(xml).toContain("<isBlank field=\"Record:Registration:ShirtSize\"/>");
    // Binary nest: outer and wraps first op + inner and of the rest
    expect(xml).toMatch(/<and><equals field="Record:Registration:PaymentReceived">[\s\S]*<and>/);
  });

  it("emits mcContains / mcEquals for MCQ Where (choice letter, not label)", () => {
    const multi = documentHtmlToXml(
      tokenHtml("choice-tally-table", {
        field: "<<Form 1:MCQ4>>",
        conditionsRows: [
          { field: "Form 1:MCQ4", op: "mcContains", value: "d" },
        ],
        conditionsCombinator: "and",
      }),
      escAttr,
      escText,
    );
    expect(multi).toContain("<mcContains field=\"Record:Form 1:MCQ4\">");
    expect(multi).toContain('<string value="d"/>');
    expect(multi).not.toContain("<equals ");

    const single = documentHtmlToXml(
      tokenHtml("response-totals-table", {
        field: "<<Form 1:MCQ1>>",
        "layout-type": "vertical",
        conditionsRows: [
          { field: "Form 1:MCQ1", op: "mcEquals", value: "d" },
        ],
        conditionsCombinator: "and",
      }),
      escAttr,
      escText,
    );
    expect(single).toContain("<mcEquals field=\"Record:Form 1:MCQ1\">");
    expect(single).toContain('<string value="d"/>');
  });
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
    // Display components skip the outer styled <font> — nested font would drop the MCQ block.
    expect(xml).not.toContain("rgb(");
    expect(xml).toContain("<display-mcq-label");
    expect(xml).toMatch(/<font><display-mcq-label/);
    expect(xml).not.toMatch(/<font[^>]*>\s*<font/);
  });

  it("converts rgb() on plain text spans to hex for Java Deploy", () => {
    const html =
      `<p><span style="font-size: 12pt; font-family: Arial; color: rgb(0, 0, 0)">Hello</span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('color="000000"');
    expect(xml).not.toContain("rgb(");
    expect(xml).toContain("Hello");
  });

  it("maps span font-weight:bold to <b> (styleWithCSS palette path)", () => {
    const html = `<p><span style="font-weight: bold;">Age:</span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain("<b>Age:</b>");
  });
});

describe("documentHtmlToXml invitation / hyperlink", () => {
  it("emits <invitation> for invitation-token", () => {
    const config = JSON.stringify({
      form: "Form 2",
      project: "",
      displayText: "Continue",
      isPrivate: false,
      authToken: "",
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    const html = `<p><span class="invitation-token" data-invitation-config="${config}">Continue</span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('<invitation form="Form 2" project="">Continue</invitation>');
  });

  it("emits private invitation with authenticationTokenValue", () => {
    const config = JSON.stringify({
      form: "PlayerDash",
      project: "",
      displayText: "Dashboard",
      isPrivate: true,
      authToken: "<<Registration:RegID>>",
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    const html = `<p><span class="invitation-token" data-invitation-config="${config}">Dashboard</span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('private="true"');
    expect(xml).toContain('<field name="Registration:RegID"/>');
    expect(xml).toContain("Dashboard</invitation>");
  });

  it("emits <link> for hyperlink-token", () => {
    const config = JSON.stringify({
      url: "http://www.dirtbowl.com",
      displayText: "Home",
      openNewWindow: true,
      conditional: false,
      conditions: [],
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    const html = `<p><span class="hyperlink-token" data-hyperlink-config="${config}">Home</span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain("<link>");
    expect(xml).toContain("<new-window/>");
    expect(xml).toContain('<string value="Home"/>');
    expect(xml).toContain('<string value="http://www.dirtbowl.com"/>');
  });

  it("recovers double-encoded invitation/hyperlink configs (setAttribute + entity encode bug)", () => {
    // Browser setAttribute(pre-encoded) then innerHTML → &amp;quot; in the attribute.
    const inv = JSON.stringify({
      form: "Form 1",
      project: "",
      displayText: "Go",
      isPrivate: false,
      authToken: "",
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/&/g, "&amp;"); // second pass like literal & in attr → &amp;
    const link = JSON.stringify({
      url: "http://example.com",
      displayText: "Example",
      openNewWindow: true,
      conditional: false,
      conditions: [],
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/&/g, "&amp;");
    const html =
      `<p><span class="invitation-token" data-invitation-config="${inv}">Go</span> ` +
      `<span class="hyperlink-token" data-hyperlink-config="${link}">Example</span></p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('<invitation form="Form 1" project="">Go</invitation>');
    expect(xml).toContain('<string value="http://example.com"/>');
    expect(xml).not.toContain('form=""');
    expect(xml).not.toContain('<string value=""/>');
  });

  it("keeps mailto / https URLs as strings, not field refs", () => {
    const config = JSON.stringify({
      url: "mailto:a@b.com",
      displayText: "mail",
      openNewWindow: false,
      conditional: false,
      conditions: [],
    })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;");
    const xml = documentHtmlToXml(
      `<p><span class="hyperlink-token" data-hyperlink-config="${config}">mail</span></p>`,
      escAttr,
      escText,
    );
    expect(xml).toContain('<string value="mailto:a@b.com"/>');
    expect(xml).not.toContain("<field name=");
  });
});

describe("documentHtmlToXml Form Text confirmation table (Potluck T6)", () => {
  it("keeps table inside data-doc-blank div and turns <<fields>> into <field/>", () => {
    const html =
      `You entered the following information:` +
      `<div data-doc-blank="1" style="min-height: 13.7pt;">` +
      `<table class="user user-border-2"><tbody>` +
      `<tr><td style="width: 108pt;">Name&nbsp;</td>` +
      `<td style="width: 108pt;">&nbsp;&lt;&lt;attendeeName&gt;&gt;</td></tr>` +
      `<tr><td>Dish&nbsp;</td><td>&lt;&lt;contribution&gt;&gt;</td></tr>` +
      `</tbody></table></div>`;
    const xml = documentHtmlToXml(html, escAttr, escText, {
      formName: "Potluck Organizer",
    });
    expect(xml).toContain("<table indent=");
    expect(xml).toContain('<field name="Potluck Organizer:attendeeName"/>');
    expect(xml).toContain('<field name="Potluck Organizer:contribution"/>');
    expect(xml).toContain("You entered the following information:");
    expect(xml).not.toMatch(/Name\s*&lt;&lt;attendeeName/);
  });

  it("leaves already-qualified Form:Field tokens alone", () => {
    const html = `<p>&lt;&lt;Potluck Organizer:attendeeName&gt;&gt;</p>`;
    const xml = documentHtmlToXml(html, escAttr, escText, {
      formName: "Potluck Organizer",
    });
    expect(xml).toContain('<field name="Potluck Organizer:attendeeName"/>');
    expect(xml).not.toContain("Potluck Organizer:Potluck Organizer:");
  });
});

describe("documentHtmlToXml Potluck Details SUM in table", () => {
  it("emits single-font <sum> with Record:Form:Field (no nested font drop)", () => {
    const config = (field) =>
      JSON.stringify({
        field,
        conditionsRows: [{ field: "", op: "equals", value: "" }],
        conditionsCombinator: "and",
      })
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
    const sumToken = (field, inst) =>
      `<span class="function-token function-table-token" contenteditable="false" ` +
      `data-function-id="sum" data-function-instance="${inst}" ` +
      `data-function-config="${config(field)}" title="SUM">` +
      `&lt;&lt;SUM(${field})&gt;&gt;</span>`;
    const html =
      `<p>Totals:</p>` +
      `<table class="user user-border-1"><tbody>` +
      `<tr><td style="width: 108pt;">Adults</td><td style="width: 108pt;">Kids</td></tr>` +
      `<tr><td style="width: 108pt;">&nbsp;${sumToken("Potluck Organizer:numAdults", 2)}</td>` +
      `<td style="width: 108pt;">&nbsp;${sumToken("Potluck Organizer:numKids", 3)}</td></tr>` +
      `</tbody></table>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml.match(/<sum\b/g)?.length).toBe(2);
    expect(xml).toContain("<field>Record:Potluck Organizer:numAdults</field>");
    expect(xml).toContain("<field>Record:Potluck Organizer:numKids</field>");
    // Nested font around sum is dropped by Java Font FACTORY on Deploy.
    expect(xml).not.toMatch(/<font>\s*<font>\s*<sum\b/);
    expect(xml).toMatch(/<font><sum version="1">/);
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

  it("does not nest font when styled span wraps an embedded image (Potluck T1)", () => {
    const html =
      `<p>Deploy (` +
      `<span style="font-family:Arial;font-size:10pt;color:#000000">` +
      `<img class="tawala-embedded-image" data-tawala-image-id="image1" ` +
      `data-image-width="22" data-image-height="23" width="22" height="23" ` +
      `src="data:image/png;base64,AAAA" alt=""/></span>)</p>`;
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('<image id="image1" width="22" height="23"></image>');
    // One font around the image — not <font><font><image/></font></font>
    expect(xml).not.toMatch(/<font[^>]*>\s*<font\b/);
    const opens = (xml.match(/<font\b/gi) || []).length;
    const closes = (xml.match(/<\/font>/gi) || []).length;
    expect(opens).toBe(closes);
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

describe("documentHtmlToXml placed vertical gaps (DISPLAY MCQ spacing)", () => {
  const mcqCfg = (field, display) =>
    JSON.stringify({ "field-name": field, display })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");

  function placedMcq(topPt, field, display) {
    return (
      `<p class="doc-placed-text" style="position: absolute; left: 36pt; top: ${topPt}pt">` +
      `<span class="function-token" data-function-id="display-mcq-label" ` +
      `data-function-config="${mcqCfg(field, display)}">fx</span></p>`
    );
  }

  it("injects blank paragraph(s) when two placed MCQ chips have a large top gap", () => {
    const html = placedMcq(40, "Form 1:MCQ1", "label_only") + placedMcq(200, "Form 1:MCQ2", "all_choices");
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain("<display-mcq-label");
    expect(xml).toMatch(/MCQ1[\s\S]*MCQ2/);
    // At least one blank spacer between the two content paragraphs.
    expect(xml).toMatch(
      /<\/display-mcq-label>[\s\S]*?<\/paragraph>\s*<paragraph indent="0" align="left"><tabPositions>/,
    );
  });

  it("keeps data-doc-blank placed lines as Deploy spacer paragraphs", () => {
    const html =
      placedMcq(40, "Form 1:MCQ1", "label_only") +
      `<p class="doc-placed-text" data-doc-blank="1" style="position: absolute; left: 36pt; top: 70pt"><br></p>` +
      placedMcq(100, "Form 1:MCQ2", "all_choices");
    const xml = documentHtmlToXml(html, escAttr, escText);
    expect(xml).toContain('<paragraph indent="0" align="left"><tabPositions>');
    expect(xml).toContain("MCQ1");
    expect(xml).toContain("MCQ2");
  });

  it("still drops unmarked empty husks (no content above+below blank mark)", () => {
    const html =
      `<p class="doc-placed-text" style="position: absolute; left: 36pt; top: 0pt"><br></p>` +
      placedMcq(40, "Form 1:MCQ1", "label_only");
    const xml = documentHtmlToXml(html, escAttr, escText);
    // Leading unmarked husk omitted; only the MCQ paragraph remains as content.
    expect(xml.match(/<paragraph\b/g)?.length).toBe(1);
    expect(xml).toContain("display-mcq-label");
  });

  it("sorts placed lines by absolute top (not DOM order) for Deploy", () => {
    // Simulate drag: Form Count DOM-first but visually below Second One.
    const html =
      `<p class="doc-placed-text" style="position: absolute; left: 36pt; top: 200pt">Form Count</p>` +
      placedMcq(40, "Form 1:MCQ1", "label_only") +
      `<p class="doc-placed-text" style="position: absolute; left: 36pt; top: 100pt">Second One</p>` +
      placedMcq(120, "Form 1:MCQ2", "all_choices");
    const xml = documentHtmlToXml(html, escAttr, escText);
    const iMcq1 = xml.indexOf("MCQ1");
    const iSecond = xml.indexOf("Second One");
    const iMcq2 = xml.indexOf("MCQ2");
    const iCount = xml.indexOf("Form Count");
    expect(iMcq1).toBeGreaterThanOrEqual(0);
    expect(iSecond).toBeGreaterThan(iMcq1);
    expect(iMcq2).toBeGreaterThan(iSecond);
    expect(iCount).toBeGreaterThan(iMcq2);
    expect(xml).toContain('label_only');
    expect(xml).toContain('all_choices');
  });
});
