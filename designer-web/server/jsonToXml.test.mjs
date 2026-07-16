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
