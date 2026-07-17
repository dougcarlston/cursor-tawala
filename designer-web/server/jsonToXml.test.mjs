import { describe, expect, it } from "vitest";
import { collectProjectImages, imagesToXml, projectToXml } from "./jsonToXml.mjs";

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
