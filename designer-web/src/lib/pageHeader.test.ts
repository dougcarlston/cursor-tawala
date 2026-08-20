import { describe, expect, it } from "vitest";
import {
  PAGE_HEADER_IMAGE_ID,
  PAGE_HEADER_SOURCE_IMAGE_ID,
  applyPageHeaderToProject,
  pageHeaderHasContent,
  pageHeaderSourceImage,
} from "./pageHeader";
import { emptyProject } from "@/types/tawala";

describe("pageHeaderHasContent", () => {
  it("is true for text or image", () => {
    expect(pageHeaderHasContent(undefined)).toBe(false);
    expect(pageHeaderHasContent({ text: "  " })).toBe(false);
    expect(pageHeaderHasContent({ text: "Hi" })).toBe(true);
    expect(pageHeaderHasContent({ imageId: PAGE_HEADER_IMAGE_ID })).toBe(true);
  });
});

describe("applyPageHeaderToProject", () => {
  it("stores text-only header and clears image", () => {
    const withImg = applyPageHeaderToProject(emptyProject(), {
      text: "Camp",
      image: { data: "QUFB", imageFormat: "PNG" },
      width: 40,
      height: 20,
    });
    expect(withImg.pageHeader?.imageId).toBe(PAGE_HEADER_IMAGE_ID);
    expect(withImg.images?.some((i) => i.id === PAGE_HEADER_IMAGE_ID)).toBe(true);

    const textOnly = applyPageHeaderToProject(withImg, {
      text: "Camp",
      image: null,
    });
    expect(textOnly.pageHeader).toEqual({ text: "Camp" });
    expect(textOnly.images?.some((i) => i.id === PAGE_HEADER_IMAGE_ID)).toBe(false);
    expect(textOnly.images?.some((i) => i.id === PAGE_HEADER_SOURCE_IMAGE_ID)).toBe(false);
  });

  it("clears pageHeader when both empty", () => {
    const next = applyPageHeaderToProject(emptyProject(), { text: "", image: null });
    expect(next.pageHeader).toBeUndefined();
  });

  it("keeps full source + viewport so reopen can re-edit after OK", () => {
    const next = applyPageHeaderToProject(emptyProject(), {
      text: "Camp",
      image: { data: "QkFLRQ==", imageFormat: "PNG", fileName: "banner.png" },
      sourceImage: { data: "U09VUkNF", imageFormat: "JPEG", fileName: "photo.jpg" },
      viewport: { x: -10, y: -20, scaleX: 0.5, scaleY: 0.5 },
      width: 520,
      height: 160,
    });
    expect(next.pageHeader?.imageId).toBe(PAGE_HEADER_IMAGE_ID);
    expect(next.pageHeader?.imageViewport).toEqual({
      x: -10,
      y: -20,
      scaleX: 0.5,
      scaleY: 0.5,
    });
    expect(next.images?.find((i) => i.id === PAGE_HEADER_IMAGE_ID)?.data).toBe("QkFLRQ==");
    const src = pageHeaderSourceImage(next);
    expect(src?.data).toBe("U09VUkNF");
    expect(src?.imageFormat).toBe("JPEG");
  });
});
