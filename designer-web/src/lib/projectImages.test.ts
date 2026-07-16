/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  addOrReuseImage,
  dataUrlForImage,
  imageFormatFromMimeOrName,
  nextImageId,
  parseDataUrl,
} from "@/lib/projectImages";

describe("nextImageId", () => {
  it("starts at image1 and advances past highest imageN", () => {
    expect(nextImageId([])).toBe("image1");
    expect(nextImageId([{ id: "image1", imageFormat: "PNG", data: "aa" }])).toBe(
      "image2",
    );
    expect(
      nextImageId([
        { id: "image2", imageFormat: "PNG", data: "aa" },
        { id: "image9", imageFormat: "GIF", data: "bb" },
      ]),
    ).toBe("image10");
  });
});

describe("imageFormatFromMimeOrName", () => {
  it("maps MIME and extension; uses JPEG not JPG for Java", () => {
    expect(imageFormatFromMimeOrName("image/png", "x.png")).toBe("PNG");
    expect(imageFormatFromMimeOrName("image/gif")).toBe("GIF");
    expect(imageFormatFromMimeOrName("image/jpeg", "x.jpg")).toBe("JPEG");
    expect(imageFormatFromMimeOrName("", "photo.JPG")).toBe("JPEG");
  });
});

describe("parseDataUrl / dataUrlForImage", () => {
  it("round-trips PNG data URLs", () => {
    const raw =
      "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg==";
    const parsed = parseDataUrl(`data:image/png;base64,${raw}`);
    expect(parsed).toEqual({ imageFormat: "PNG", data: raw });
    expect(dataUrlForImage({ id: "image1", ...parsed! })).toBe(
      `data:image/png;base64,${raw}`,
    );
  });

  it("maps image/jpg to JPEG", () => {
    expect(parseDataUrl("data:image/jpg;base64,AAAA")?.imageFormat).toBe("JPEG");
  });
});

describe("addOrReuseImage", () => {
  it("dedupes identical bytes and assigns sequential ids", () => {
    const first = addOrReuseImage([], {
      data: "AAA",
      imageFormat: "PNG",
      fileName: "a.png",
    });
    expect(first.id).toBe("image1");
    expect(first.images).toHaveLength(1);

    const again = addOrReuseImage(first.images, {
      data: "AAA",
      imageFormat: "PNG",
    });
    expect(again.id).toBe("image1");
    expect(again.images).toHaveLength(1);

    const second = addOrReuseImage(first.images, {
      data: "BBB",
      imageFormat: "JPEG",
      fileName: "b.jpg",
    });
    expect(second.id).toBe("image2");
    expect(second.images).toHaveLength(2);
  });
});
