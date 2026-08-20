/**
 * Project → Page Header… — banner text + optional image (legacy PageHeader.cs).
 * Deploy: `<pageHeader>` + imagedef; runtime: `<h1 class="pageHeading">`.
 *
 * OK bakes a crop into `__HEADER__` for Push, and keeps the full original in
 * `__HEADER_SOURCE__` plus `pageHeader.imageViewport` so reopen stays editable.
 */

import type { TawalaImageDef, TawalaPageHeader, TawalaProject } from "@/types/tawala";
import type { PageHeaderImageViewport } from "@/lib/pageHeaderImageFit";

/** Stable imagedef id for the project banner crop (what Deploy / Preview show). */
export const PAGE_HEADER_IMAGE_ID = "__HEADER__";

/** Designer-only full-resolution original — not exported to Java Deploy XML. */
export const PAGE_HEADER_SOURCE_IMAGE_ID = "__HEADER_SOURCE__";

export function pageHeaderHasContent(
  header: TawalaPageHeader | null | undefined,
): boolean {
  if (!header) return false;
  if (String(header.text ?? "").trim()) return true;
  return !!String(header.imageId ?? "").trim();
}

/** Resolve the baked banner image from `project.images`, if any. */
export function pageHeaderImage(
  project: Pick<TawalaProject, "images" | "pageHeader">,
): TawalaImageDef | null {
  const id = String(project.pageHeader?.imageId ?? "").trim();
  if (!id) return null;
  return (project.images ?? []).find((img) => img.id === id) ?? null;
}

/** Full original photo for re-editing (falls back to null when only a bake exists). */
export function pageHeaderSourceImage(
  project: Pick<TawalaProject, "images">,
): TawalaImageDef | null {
  return (project.images ?? []).find((img) => img.id === PAGE_HEADER_SOURCE_IMAGE_ID) ?? null;
}

/**
 * Apply dialog OK: set text / baked image / optional source + viewport.
 * Keeps other project images; replaces prior `__HEADER__` / `__HEADER_SOURCE__`.
 */
export function applyPageHeaderToProject(
  project: TawalaProject,
  next: {
    text: string;
    /** Baked crop for Deploy (imageId `__HEADER__`). */
    image: { data: string; imageFormat: TawalaImageDef["imageFormat"]; fileName?: string } | null;
    /** Full original for later pan/zoom (omitted when removing image). */
    sourceImage?: {
      data: string;
      imageFormat: TawalaImageDef["imageFormat"];
      fileName?: string;
    } | null;
    viewport?: PageHeaderImageViewport;
    width?: number;
    height?: number;
  },
): TawalaProject {
  const text = String(next.text ?? "");
  const images = [...(project.images ?? [])].filter(
    (img) => img.id !== PAGE_HEADER_IMAGE_ID && img.id !== PAGE_HEADER_SOURCE_IMAGE_ID,
  );

  let pageHeader: TawalaPageHeader | undefined;
  if (next.image) {
    images.push({
      id: PAGE_HEADER_IMAGE_ID,
      imageFormat: next.image.imageFormat,
      data: next.image.data.replace(/\s+/g, ""),
      fileName: next.image.fileName,
    });
    if (next.sourceImage?.data) {
      images.push({
        id: PAGE_HEADER_SOURCE_IMAGE_ID,
        imageFormat: next.sourceImage.imageFormat,
        data: next.sourceImage.data.replace(/\s+/g, ""),
        fileName: next.sourceImage.fileName ?? next.image.fileName,
      });
    }
    pageHeader = {
      text,
      imageId: PAGE_HEADER_IMAGE_ID,
      width: next.width && next.width > 0 ? next.width : undefined,
      height: next.height && next.height > 0 ? next.height : undefined,
    };
    if (next.viewport) {
      pageHeader.imageViewport = { ...next.viewport };
    }
  } else if (text.trim()) {
    pageHeader = { text };
  } else {
    pageHeader = undefined;
  }

  return {
    ...project,
    images,
    pageHeader,
  };
}
