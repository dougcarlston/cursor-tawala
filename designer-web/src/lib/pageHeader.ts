/**
 * Project → Page Header… — banner text + optional image (legacy PageHeader.cs).
 * Deploy: `<pageHeader>` + imagedef; runtime: `<h1 class="pageHeading">`.
 */

import type { TawalaImageDef, TawalaPageHeader, TawalaProject } from "@/types/tawala";

/** Stable imagedef id for the project banner (replaces on each Browse). */
export const PAGE_HEADER_IMAGE_ID = "__HEADER__";

export function pageHeaderHasContent(
  header: TawalaPageHeader | null | undefined,
): boolean {
  if (!header) return false;
  if (String(header.text ?? "").trim()) return true;
  return !!String(header.imageId ?? "").trim();
}

/** Resolve the banner image from `project.images`, if any. */
export function pageHeaderImage(
  project: Pick<TawalaProject, "images" | "pageHeader">,
): TawalaImageDef | null {
  const id = String(project.pageHeader?.imageId ?? "").trim();
  if (!id) return null;
  return (project.images ?? []).find((img) => img.id === id) ?? null;
}

/**
 * Apply dialog OK: set text / image, keep other project images, replace prior
 * `__HEADER__` imagedef when the banner image changes or is removed.
 */
export function applyPageHeaderToProject(
  project: TawalaProject,
  next: {
    text: string;
    image: { data: string; imageFormat: TawalaImageDef["imageFormat"]; fileName?: string } | null;
    width?: number;
    height?: number;
  },
): TawalaProject {
  const text = String(next.text ?? "");
  const images = [...(project.images ?? [])].filter((img) => img.id !== PAGE_HEADER_IMAGE_ID);

  let pageHeader: TawalaPageHeader | undefined;
  if (next.image) {
    images.push({
      id: PAGE_HEADER_IMAGE_ID,
      imageFormat: next.image.imageFormat,
      data: next.image.data.replace(/\s+/g, ""),
      fileName: next.image.fileName,
    });
    pageHeader = {
      text,
      imageId: PAGE_HEADER_IMAGE_ID,
      width: next.width && next.width > 0 ? next.width : undefined,
      height: next.height && next.height > 0 ? next.height : undefined,
    };
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
