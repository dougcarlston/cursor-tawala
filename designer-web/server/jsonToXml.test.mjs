import { describe, expect, it } from "vitest";
import {
  applyTextItemStyleToXml,
  collectProjectImages,
  imagesToXml,
  projectToXml,
  xmlCommentText,
} from "./jsonToXml.mjs";

describe("imagesToXml", () => {
  it("omits empty images", () => {
    expect(imagesToXml(undefined)).toBe("");
    expect(imagesToXml([])).toBe("");
  });

  it("emits imagedef with JPEG not JPG", () => {
    const xml = imagesToXml([
      { id: "image1", imageFormat: "JPG", data: "QUFB" },
      { id: "image2", imageFormat: "PNG", data: "QkJC" },
    ]);
    expect(xml).toBe(
      `<images>` +
        `<imagedef id="image1"><imagedata imageFormat="JPEG">QUFB</imagedata></imagedef>` +
        `<imagedef id="image2"><imagedata imageFormat="PNG">QkJC</imagedata></imagedef>` +
        `</images>`,
    );
  });
});

describe("projectToXml images", () => {
  it("appends <images> after documents when present", () => {
    const xml = projectToXml({
      name: "Pic",
      themePath: "default",
      forms: [{ name: "Form 1", startPoint: true, items: [] }],
      processes: [],
      documents: [],
      images: [{ id: "image1", imageFormat: "PNG", data: "AAAA" }],
    });
    expect(xml).toContain("</documents><images>");
    expect(xml).toContain('<imagedef id="image1">');
    expect(xml).toContain('imageFormat="PNG"');
  });

  it("exports Form Text embedded img + project images together", () => {
    const xml = projectToXml({
      name: "Pic",
      forms: [
        {
          name: "Form 1",
          startPoint: true,
          items: [
            {
              type: "text",
              label: "T1",
              content:
                `<p><img class="tawala-embedded-image" data-tawala-image-id="image1" ` +
                `data-image-width="1" data-image-height="1" src="data:image/png;base64,AAAA"/></p>`,
            },
          ],
        },
      ],
      images: [{ id: "image1", imageFormat: "PNG", data: "AAAA" }],
    });
    expect(xml).toContain('<image id="image1" width="1" height="1"></image>');
    expect(xml).toContain("<images><imagedef id=\"image1\">");
  });

  it("harvests imagedef from HTML data-URL when project.images is empty", () => {
    const xml = projectToXml({
      name: "Pic",
      forms: [
        {
          name: "Form 1",
          startPoint: true,
          items: [
            {
              type: "text",
              label: "T1",
              content:
                `<p><img class="tawala-embedded-image" data-tawala-image-id="image1" ` +
                `data-image-width="10" data-image-height="8" ` +
                `src="data:image/png;base64,QUFB"/></p>`,
            },
          ],
        },
      ],
      images: [],
    });
    expect(xml).toContain('<image id="image1" width="10" height="8"></image>');
    expect(xml).toContain(
      `<imagedef id="image1"><imagedata imageFormat="PNG">QUFB</imagedata></imagedef>`,
    );
  });
});

describe("collectProjectImages", () => {
  it("prefers project.images over duplicate HTML src", () => {
    const list = collectProjectImages({
      forms: [
        {
          items: [
            {
              content:
                `<img data-tawala-image-id="image1" src="data:image/png;base64,FROMHTML"/>`,
            },
          ],
        },
      ],
      images: [{ id: "image1", imageFormat: "PNG", data: "FROMPROJECT" }],
    });
    expect(list).toEqual([
      { id: "image1", imageFormat: "PNG", data: "FROMPROJECT", fileName: undefined },
    ]);
  });
});

describe("Response Totals question titles", () => {
  function totalsTokenHtml(field) {
    const encoded = JSON.stringify({ field, "layout-type": "vertical" })
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;");
    return (
      `<p><span class="function-token" data-function-id="response-totals-table" ` +
      `data-function-config="${encoded}">RESPONSE TOTALS</span></p>`
    );
  }

  const project = {
    name: "MCQ",
    forms: [
      {
        name: "Form 1",
        startPoint: true,
        items: [
          {
            type: "mc",
            label: "Q1",
            question: "Where is Greenland?",
            choices: [{ name: "a", text: "Europe" }],
          },
          {
            type: "mc",
            label: "Q2",
            question: "Favorite color?",
            choices: [{ name: "a", text: "Blue" }],
          },
          {
            type: "text",
            label: "T1",
            content: totalsTokenHtml("<<Form 1:Q1>>") + totalsTokenHtml("<<Form 1:Q2>>"),
          },
        ],
      },
    ],
    processes: [],
    documents: [],
  };

  it("lookupMcqQuestionPlain resolves Form:Label and Record:Form:Label", async () => {
    const { lookupMcqQuestionPlain } = await import("./jsonToXml.mjs");
    expect(lookupMcqQuestionPlain(project, "Form 1:Q1")).toBe("Where is Greenland?");
    expect(lookupMcqQuestionPlain(project, "<<Form 1:Q1>>")).toBe("Where is Greenland?");
    expect(lookupMcqQuestionPlain(project, "Record:Form 1:Q2")).toBe("Favorite color?");
  });

  it("Deploy XML puts MCQ question text above each totals table + spacer after", () => {
    const xml = projectToXml(project);
    expect(xml).toContain("<response-totals-table");
    expect(xml).toMatch(
      /<b>Where is Greenland\?<\/b><\/font><\/paragraph><paragraph[^>]*>[\s\S]*?<response-totals-table/,
    );
    expect(xml).toMatch(
      /<b>Favorite color\?<\/b><\/font><\/paragraph><paragraph[^>]*>[\s\S]*?<response-totals-table/,
    );
    // Spacer paragraph after each table (Deploy vertical rhythm).
    expect(xml).toMatch(
      /<\/response-totals-table><\/font><\/paragraph><paragraph[^>]*>/,
    );
  });

  it("titles every totals table even when chips share one paragraph", () => {
    const encoded = (field) =>
      JSON.stringify({ field, "layout-type": "vertical" })
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
    const sharedPara =
      `<p>` +
      `<span class="function-token" data-function-id="response-totals-table" data-function-config="${encoded("<<Form 1:Q1>>")}">A</span>` +
      `<span class="function-token" data-function-id="response-totals-table" data-function-config="${encoded("<<Form 1:Q2>>")}">B</span>` +
      `</p>`;
    const xml = projectToXml({
      ...project,
      forms: [
        {
          ...project.forms[0],
          items: [
            ...project.forms[0].items.filter((i) => i.type === "mc"),
            { type: "text", label: "T1", content: sharedPara },
          ],
        },
      ],
    });
    expect(xml.match(/<b>Where is Greenland\?<\/b>/g)?.length).toBeGreaterThanOrEqual(1);
    expect(xml.match(/<b>Favorite color\?<\/b>/g)?.length).toBeGreaterThanOrEqual(1);
    expect((xml.match(/<response-totals-table/g) || []).length).toBe(2);
  });
});

describe("projectToXml form flags", () => {
  it("emits dataEntryOnly and blockBackButton when set", () => {
    const xml = projectToXml({
      name: "Flags",
      themePath: "default",
      forms: [
        {
          name: "Form 1",
          startPoint: true,
          dataEntryOnly: true,
          blockBackButton: true,
          items: [],
        },
      ],
      processes: [],
      documents: [],
    });
    expect(xml).toContain('startPoint="true"');
    expect(xml).toContain('dataEntryOnly="true"');
    expect(xml).toContain('blockBackButton="true"');
  });
});

describe("Text item Styles colors on export", () => {
  it("rewrites default black font color for instructional / error", () => {
    const base = `<paragraph><font face="Arial" size="200" color="000000">Hi</font></paragraph>`;
    expect(applyTextItemStyleToXml(base, "instructional")).toContain('color="000080"');
    expect(applyTextItemStyleToXml(base, "error")).toContain('color="C00000"');
    expect(applyTextItemStyleToXml(base, "normal")).toContain('color="000000"');
  });

  it("adds bold+italic when Style is instructional / error", () => {
    const base = `<paragraph><font face="Arial" size="200" color="000000">Hi</font></paragraph>`;
    const xml = applyTextItemStyleToXml(base, "instructional");
    expect(xml).toContain("<b>");
    expect(xml).toContain("<i>");
    expect(xml).toContain("Hi");
  });

  it("does not re-bold when Design HTML cleared weight (font-weight: normal)", () => {
    const base = `<paragraph>Get Together helps.</paragraph>`;
    const xml = applyTextItemStyleToXml(base, "instructional", {
      sourceHtml: `<p><span style="font-weight: normal;">Get Together helps.</span></p>`,
    });
    expect(xml).toContain('color="000080"');
    expect(xml).toContain("<i>");
    expect(xml).not.toContain("<b>");
  });

  it("does not wrap image-only fonts in bold/italic", () => {
    const nested =
      `<paragraph indent="0" align="left">Then (` +
      `<font face="Arial" size="200" color="000000">` +
      `<image id="image1" width="22" height="23"></image>` +
      `</font>)</paragraph>`;
    const xml = applyTextItemStyleToXml(nested, "instructional", {
      sourceHtml: `<p><span style="font-weight: normal;">Then ( <img data-tawala-image-id="image1"/> )</span></p>`,
    });
    expect(xml).toContain('<image id="image1"');
    expect(xml).toContain('color="000080"');
    expect(xml).not.toMatch(/<b>\s*<i>\s*<image/i);
    expect(xml).not.toMatch(/<i>\s*<image/i);
  });

  it("styles loose text around an image font (Potluck Deploy button line)", () => {
    const mixed =
      `<paragraph indent="0" align="left">Then click on the Deploy button ( ` +
      `<font face="Arial" size="200" color="000000">` +
      `<image id="image1" width="22" height="23"></image>` +
      `</font>) up above.</paragraph>`;
    const xml = applyTextItemStyleToXml(mixed, "instructional");
    expect(xml).toContain('<image id="image1"');
    expect(xml).toContain("<b><i>Then click on the Deploy button ( ");
    expect(xml).toContain("<b><i>) up above.</i></b>");
    expect(xml).toContain('color="000080"');
  });

  it("keeps non-default author colors", () => {
    const green = `<paragraph><font color="00FF00">Hi</font></paragraph>`;
    expect(applyTextItemStyleToXml(green, "instructional")).toContain('color="00FF00"');
  });

  it("does not orphan </font> when nested fonts wrap an image (Potluck Deploy)", () => {
    const nested =
      `<paragraph indent="0" align="left">Then (` +
      `<font face="Arial" size="200" color="000000">` +
      `<font face="Arial" size="200" color="000000">` +
      `<image id="image1" width="22" height="23"></image>` +
      `</font></font>)</paragraph>`;
    const xml = applyTextItemStyleToXml(nested, "instructional");
    const opens = (xml.match(/<font\b/gi) || []).length;
    const closes = (xml.match(/<\/font>/gi) || []).length;
    expect(opens).toBe(closes);
    expect(xml).toContain('<image id="image1"');
    // Image-only fonts keep color rewrite but are not wrapped in <b>/<i>.
    expect(xml).toContain('color="000080"');
    expect(xml).not.toMatch(/<b><i><image id="image1"/);
    // Old bug left an unclosed inner <font> before </i></b>.
    expect(xml).not.toMatch(/<font[^>]*>\s*<image[^>]*>\s*<\/image>\s*<\/i>/);
  });

  it("projectToXml HTML Text instructional emits blue font color", () => {
    const xml = projectToXml({
      name: "Styles",
      forms: [
        {
          name: "Form 1",
          startPoint: true,
          items: [
            {
              type: "text",
              label: "T1",
              style: "instructional",
              content: "<p>This is instructional</p>",
            },
            {
              type: "text",
              label: "T2",
              style: "error",
              content: "<p>This is a warning</p>",
            },
          ],
        },
      ],
    });
    expect(xml).toMatch(/style="instructional"[\s\S]*color="000080"/);
    expect(xml).toMatch(/style="error"[\s\S]*color="C00000"/);
  });
});

describe("projectToXml heading type", () => {
  it("emits type=Sub for a uniform Sub-size heading box", () => {
    const xml = projectToXml({
      name: "H",
      forms: [
        {
          name: "Form 1",
          startPoint: true,
          items: [
            {
              type: "heading",
              label: "H1",
              content: '<span class="heading-size-sub">Details</span>',
            },
          ],
        },
      ],
    });
    expect(xml).toContain('<heading label="H1" type="Sub">Details</heading>');
  });

  it("emits type=Main for mixed Main/Sub runs (legacy single-type limit)", () => {
    const xml = projectToXml({
      name: "H",
      forms: [
        {
          name: "Form 1",
          startPoint: true,
          items: [
            {
              type: "heading",
              label: "H1",
              content: 'Title <span class="heading-size-sub">sub</span>',
            },
          ],
        },
      ],
    });
    expect(xml).toContain('<heading label="H1" type="Main">Title sub</heading>');
  });
});

describe("projectToXml pageHeader", () => {
  it("emits pageHeader text + image and imagedef", () => {
    const xml = projectToXml({
      name: "Banner",
      forms: [{ name: "Form 1", startPoint: true, items: [] }],
      pageHeader: {
        text: "Hello Camp",
        imageId: "__HEADER__",
        width: 40,
        height: 60,
      },
      images: [{ id: "__HEADER__", imageFormat: "PNG", data: "QUFB" }],
    });
    expect(xml).toContain(
      '<pageHeader><text>Hello Camp</text><image id="__HEADER__" width="40" height="60"/></pageHeader>',
    );
    expect(xml).toContain('<imagedef id="__HEADER__">');
  });
});

describe("xmlCommentText", () => {
  it("spaces consecutive dashes so XML comments stay well-formed", () => {
    expect(xmlCommentText("-- note")).toBe("- - note");
    expect(xmlCommentText("--- section")).toBe("- - - section");
    expect(xmlCommentText("a----b")).toBe("a- - - -b");
    expect(xmlCommentText("trailing-")).toBe("trailing");
    expect(xmlCommentText("---")).toBe("- - ");
  });

  it("exports process comments with --- without illegal -- in XML", () => {
    const xml = projectToXml({
      name: "Pollish",
      forms: [{ name: "Form 1", startPoint: true, items: [] }],
      processes: [
        {
          name: "Post-SessionBySessionReport",
          commands: [
            {
              cmd: "comment",
              text: "--- This is the the case of the top part of the form submitted",
            },
          ],
        },
      ],
      documents: [],
    });
    expect(xml).toContain("<!-- ");
    expect(xml).toContain(
      "This is the the case of the top part of the form submitted -->",
    );
    const comments = [...xml.matchAll(/<!--([\s\S]*?)-->/g)].map((m) => m[1]);
    expect(comments.length).toBeGreaterThan(0);
    for (const body of comments) {
      expect(body.includes("--")).toBe(false);
      expect(body.endsWith("-")).toBe(false);
    }
  });
});

/**
 * Process Set statement typing (owner rules, `DESIGNER_OPEN_BUGS.md` "Variables treated
 * as text"): numeric SET expressions must compile to Java's `<add>/<sub>/<mul>/<div>`
 * operator XML, not a flat `<string>` concatenation that bakes the literal operator
 * character into the stored text.
 */
function setXml(value, extra = {}) {
  const xml = projectToXml({
    name: "SetTest",
    forms: [{ name: "Form 1", startPoint: true, items: [] }],
    processes: [
      {
        name: "P1",
        commands: [{ cmd: "set", field: "c", value, ...extra }],
      },
    ],
    documents: [],
  });
  const m = xml.match(/<set field="c"[^>]*>([\s\S]*?)<\/set>/);
  if (!m) throw new Error(`No <set> found in: ${xml}`);
  return m[0];
}

describe("Process Set expression typing (numeric vs text)", () => {
  it("plain numeric literal (no operator) stays a simple string operand", () => {
    expect(setXml("1")).toBe('<set field="c"><string value="1"/></set>');
  });

  it("field + field compiles to <add>, not literal '+' text", () => {
    const xml = setXml("<<a>> + <<b>>");
    expect(xml).toContain("<add>");
    expect(xml).toContain('<operand field="a"/>');
    expect(xml).toContain('<operand field="b"/>');
    expect(xml).not.toContain("value=\"+\"");
  });

  it("field + number literal compiles to <add> (regression: legacy narrow-case parity)", () => {
    const xml = setXml("<<a>> + 5");
    expect(xml).toContain("<add><operand field=\"a\"/><operand value=\"5\"/></add>");
  });

  it("subtraction compiles to <sub>", () => {
    expect(setXml("<<a>> - <<b>>")).toContain(
      '<sub><operand field="a"/><operand field="b"/></sub>',
    );
  });

  it("multiplication compiles to <mul>", () => {
    expect(setXml("<<a>> * <<b>>")).toContain(
      '<mul><operand field="a"/><operand field="b"/></mul>',
    );
  });

  it("division compiles to <div>", () => {
    expect(setXml("<<a>> / <<b>>")).toContain(
      '<div><operand field="a"/><operand field="b"/></div>',
    );
  });

  it("parentheses control operator precedence/order", () => {
    const xml = setXml("(<<a>> + <<b>>) * 2");
    expect(xml).toContain(
      '<mul><add><operand field="a"/><operand field="b"/></add><operand value="2"/></mul>',
    );
  });

  it("respects * before + without parentheses", () => {
    const xml = setXml("<<a>> + <<b>> * 2");
    expect(xml).toContain(
      '<add><operand field="a"/><mul><operand field="b"/><operand value="2"/></mul></add>',
    );
  });

  it("^ with a literal non-negative integer exponent expands to nested <mul> (Java has no pow op)", () => {
    const xml = setXml("<<a>> ^ 3");
    expect(xml).toContain(
      '<mul><mul><operand field="a"/><operand field="a"/></mul><operand field="a"/></mul>',
    );
  });

  it("^ with a non-integer/field exponent has no Java equivalent — falls back to literal text", () => {
    const xml = setXml("<<a>> ^ <<b>>");
    expect(xml).not.toContain("<mul>");
    expect(xml).toContain("<string");
  });

  it('quoted text concatenation: "ice"+"cream" -> icecream (sibling <string> ops, not <add>)', () => {
    const xml = setXml('"ice"+"cream"');
    expect(xml).toContain('<string value="ice"/><string value="cream"/>');
    expect(xml).not.toContain("<add>");
  });

  it("field + quoted text concatenates (mixed field/text via +)", () => {
    const xml = setXml('<<a>>+"cream"');
    expect(xml).toContain('<string field="a"/><string value="cream"/>');
  });

  it('a quoted number is text, not numeric — "123"+"456" concatenates instead of adding', () => {
    const xml = setXml('"123"+"456"');
    expect(xml).toContain('<string value="123"/><string value="456"/>');
    expect(xml).not.toContain("<add>");
  });

  it("text operands with a non-+ operator are undefined — falls back to literal text as-is", () => {
    const xml = setXml('"ice" - "cream"');
    expect(xml).not.toContain("<sub>");
    expect(xml).toContain("<string value=");
  });

  it('arithmeticAsText=true forces literal text even when the value looks arithmetic', () => {
    const xml = setXml("<<a>> + <<b>>", { arithmeticAsText: true });
    expect(xml).not.toContain("<add>");
    expect(xml).toContain('arithmeticAsText="true"');
  });

  it("a single field reference (no operators) still emits a plain <string field> (FIB typing follows the field's own runtime value)", () => {
    expect(setXml("<<a>>")).toBe('<set field="c"><string field="a"/></set>');
  });

  it("mixed field + literal text with no operators keeps existing concatenation behavior", () => {
    expect(setXml("Hi <<a>>!")).toBe(
      '<set field="c" arithmeticAsText="false">' +
        '<string value="Hi "/><string field="a"/><string value="!"/></set>',
    );
  });

  // Regression: real "Horses and Penguins Test" project (owner Jul 27) — a self-referencing
  // `Set Score to <<Score>> + 1` (field on both LHS and RHS) and `Set Wrong to 17 - <<Score>>`
  // (a bare number as the *first* operand, field second — the reverse operand order from the
  // synthetic `<<a>> - <<b>>` test above). Both exercise the exact spacing/pattern from the
  // owner's screenshot, not just a normalized `field+field` shape.
  it('self-referencing "Set Score to <<Score>> + 1" compiles to <add>, not literal text', () => {
    const xml = setXml("<<Score>> + 1");
    expect(xml).toBe(
      '<set field="c" arithmeticAsText="false">' +
        '<add><operand field="Score"/><operand value="1"/></add></set>',
    );
  });

  it('"Set Wrong to 17 - <<Score>>" (number first, field second) compiles to <sub>', () => {
    const xml = setXml("17 - <<Score>>");
    expect(xml).toBe(
      '<set field="c" arithmeticAsText="false">' +
        '<sub><operand value="17"/><operand field="Score"/></sub></set>',
    );
  });

  it('"Set Score to <<Score>> * 100 / 17" (chained mul/div, real project percent calc)', () => {
    const xml = setXml("<<Score>> * 100 / 17");
    expect(xml).toBe(
      '<set field="c" arithmeticAsText="false">' +
        "<div><mul><operand field=\"Score\"/><operand value=\"100\"/></mul>" +
        '<operand value="17"/></div></set>',
    );
  });
});
