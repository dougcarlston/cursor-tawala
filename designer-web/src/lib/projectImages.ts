/**
 * Project-level image assets for Insert → Image → From your PC….
 * Legacy: ImageDefinitionCollection / `<images><imagedef><imagedata>`.
 */

import type { TawalaImageDef, TawalaImageFormat } from "@/types/tawala";

export const EMBEDDED_IMAGE_CLASS = "tawala-embedded-image";
export const EMBEDDED_IMAGE_ID_ATTR = "data-tawala-image-id";
export const EMBEDDED_IMAGE_WIDTH_ATTR = "data-image-width";
export const EMBEDDED_IMAGE_HEIGHT_ATTR = "data-image-height";

export const LOCAL_IMAGE_ACCEPT =
  "image/gif,image/jpeg,image/png,.gif,.jpg,.jpeg,.png";

/** Next id in the legacy `image1`, `image2`, … sequence. */
export function nextImageId(images: readonly TawalaImageDef[]): string {
  let max = 0;
  for (const img of images) {
    const m = /^image(\d+)$/i.exec(img.id.trim());
    if (m) max = Math.max(max, Number(m[1]));
  }
  return `image${max + 1}`;
}

/** Map MIME / file name to Java `Image.Data.Format` names (JPEG not JPG). */
export function imageFormatFromMimeOrName(
  mime: string,
  fileName = "",
): TawalaImageFormat {
  const m = (mime || "").toLowerCase();
  if (m.includes("png")) return "PNG";
  if (m.includes("gif")) return "GIF";
  if (m.includes("jpeg") || m.includes("jpg")) return "JPEG";
  const ext = fileName.split(".").pop()?.toLowerCase() ?? "";
  if (ext === "png") return "PNG";
  if (ext === "gif") return "GIF";
  return "JPEG";
}

export function parseDataUrl(
  dataUrl: string,
): { imageFormat: TawalaImageFormat; data: string } | null {
  const m = /^data:image\/(png|gif|jpeg|jpg);base64,(.+)$/i.exec(dataUrl.trim());
  if (!m) return null;
  const fmt = m[1].toLowerCase();
  const imageFormat: TawalaImageFormat =
    fmt === "png" ? "PNG" : fmt === "gif" ? "GIF" : "JPEG";
  return { imageFormat, data: m[2].replace(/\s+/g, "") };
}

export function dataUrlForImage(img: TawalaImageDef): string {
  const mime =
    img.imageFormat === "PNG"
      ? "image/png"
      : img.imageFormat === "GIF"
        ? "image/gif"
        : "image/jpeg";
  return `data:${mime};base64,${img.data}`;
}

/**
 * Add a new imagedef or reuse an existing one with the same bytes + format.
 * Returns the (possibly unchanged) images array and the id to reference in HTML.
 */
export function addOrReuseImage(
  images: readonly TawalaImageDef[],
  input: { data: string; imageFormat: TawalaImageFormat; fileName?: string },
): { images: TawalaImageDef[]; id: string } {
  const data = input.data.replace(/\s+/g, "");
  const existing = images.find(
    (i) => i.data === data && i.imageFormat === input.imageFormat,
  );
  if (existing) return { images: [...images], id: existing.id };
  const id = nextImageId(images);
  return {
    images: [
      ...images,
      {
        id,
        imageFormat: input.imageFormat,
        data,
        fileName: input.fileName,
      },
    ],
    id,
  };
}

export function createEmbeddedImageElement(opts: {
  id: string;
  dataUrl: string;
  width: number;
  height: number;
  alt?: string;
}): HTMLImageElement {
  const img = document.createElement("img");
  img.className = EMBEDDED_IMAGE_CLASS;
  img.setAttribute(EMBEDDED_IMAGE_ID_ATTR, opts.id);
  img.setAttribute(EMBEDDED_IMAGE_WIDTH_ATTR, String(opts.width));
  img.setAttribute(EMBEDDED_IMAGE_HEIGHT_ATTR, String(opts.height));
  img.width = opts.width;
  img.height = opts.height;
  img.style.width = `${opts.width}px`;
  img.style.height = `${opts.height}px`;
  img.src = opts.dataUrl;
  img.alt = opts.alt ?? "";
  return img;
}

export function insertEmbeddedImageAtSelection(
  root: HTMLElement,
  img: HTMLImageElement,
): void {
  const sel = window.getSelection();
  let range: Range | null = sel && sel.rangeCount > 0 ? sel.getRangeAt(0) : null;
  if (!range || !root.contains(range.commonAncestorContainer)) {
    range = document.createRange();
    range.selectNodeContents(root);
    range.collapse(false);
  }
  if (!range.collapsed) range.deleteContents();
  range.insertNode(img);
  const after = document.createRange();
  after.setStartAfter(img);
  after.collapse(true);
  sel?.removeAllRanges();
  sel?.addRange(after);
}

export function readFileAsDataUrl(file: File): Promise<string> {
  return new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.onload = () => {
      if (typeof reader.result === "string") resolve(reader.result);
      else reject(new Error("Could not read image file"));
    };
    reader.onerror = () => reject(reader.error ?? new Error("Could not read image file"));
    reader.readAsDataURL(file);
  });
}

export function loadImageNaturalSize(
  dataUrl: string,
): Promise<{ width: number; height: number }> {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () =>
      resolve({
        width: Math.max(1, img.naturalWidth || 1),
        height: Math.max(1, img.naturalHeight || 1),
      });
    img.onerror = () => reject(new Error("Could not decode image"));
    img.src = dataUrl;
  });
}
