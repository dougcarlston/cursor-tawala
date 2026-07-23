import { describe, expect, it } from "vitest";
import {
  applyTextItemStyleToXml,
  collectProjectImages,
  imagesToXml,
  projectToXml,
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
