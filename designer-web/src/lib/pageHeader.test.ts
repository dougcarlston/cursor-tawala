import { describe, expect, it } from "vitest";
import {
  PAGE_HEADER_IMAGE_ID,
  applyPageHeaderToProject,
  pageHeaderHasContent,
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
  });

  it("clears pageHeader when both empty", () => {
    const next = applyPageHeaderToProject(emptyProject(), { text: "", image: null });
    expect(next.pageHeader).toBeUndefined();
  });
});
