/**
 * Insert → Image… → From your PC… — pick a local GIF/JPG/PNG, store in project.images,
 * and insert an `<img data-tawala-image-id>` into the active Form Text / Document editor.
 * Separate from DISPLAY IMAGE (web URL / function token).
 */

import {
  getActivePaletteEditor,
  type PaletteEditorHandle,
} from "./formattingPaletteContext";
import {
  LOCAL_IMAGE_ACCEPT,
  createEmbeddedImageElement,
  dataUrlForImage,
  imageFormatFromMimeOrName,
  insertEmbeddedImageAtSelection,
  loadImageNaturalSize,
  parseDataUrl,
  readFileAsDataUrl,
} from "./projectImages";
import {
  DEFAULT_MAX_INSERT_WIDTH_PX,
  applyEmbeddedImageDisplaySize,
  fitEmbeddedImageSize,
  selectEmbeddedImage,
} from "./embeddedImageResize";
import { useProjectStore } from "@/store/projectStore";

export function openLocalImageInsertFromEditor(): void {
  const handle = getActivePaletteEditor();
  if (!handle) {
    useProjectStore
      .getState()
      .setStatus("Click inside a Form Text or Document first, then Insert → Image…");
    return;
  }
  handle.saveSelection();

  const input = document.createElement("input");
  input.type = "file";
  input.accept = LOCAL_IMAGE_ACCEPT;
  input.style.display = "none";
  input.addEventListener("change", () => {
    const file = input.files?.[0] ?? null;
    input.remove();
    if (!file) {
      useProjectStore.getState().setStatus("Image insert cancelled");
      return;
    }
    void insertLocalImageFile(handle, file);
  });
  // Cancel with no change event — clean up the ephemeral input.
  input.addEventListener("cancel", () => {
    input.remove();
  });
  document.body.appendChild(input);
  input.click();
}

export async function insertLocalImageFile(
  handle: PaletteEditorHandle,
  file: File,
): Promise<void> {
  const store = useProjectStore.getState();
  try {
    if (!/^image\/(gif|jpeg|jpg|png)$/i.test(file.type) && !/\.(gif|jpe?g|png)$/i.test(file.name)) {
      store.setStatus("Choose a GIF, JPG, or PNG image");
      return;
    }

    const dataUrl = await readFileAsDataUrl(file);
    const parsed = parseDataUrl(dataUrl);
    if (!parsed) {
      store.setStatus("Could not read that image file");
      return;
    }
    const imageFormat =
      parsed.imageFormat || imageFormatFromMimeOrName(file.type, file.name);
    const natural = await loadImageNaturalSize(dataUrl);
    const maxWidth = Math.max(
      48,
      Math.min(DEFAULT_MAX_INSERT_WIDTH_PX, (handle.el.clientWidth || DEFAULT_MAX_INSERT_WIDTH_PX) - 16),
    );
    const fitted = fitEmbeddedImageSize(natural.width, natural.height, maxWidth);
    const id = store.registerProjectImage({
      data: parsed.data,
      imageFormat,
      fileName: file.name,
    });
    const def = (useProjectStore.getState().project.images ?? []).find((i) => i.id === id);
    const src = def ? dataUrlForImage(def) : dataUrl;

    handle.restoreSelection();
    handle.el.focus();
    const img = createEmbeddedImageElement({
      id,
      dataUrl: src,
      width: fitted.width,
      height: fitted.height,
      alt: file.name,
    });
    applyEmbeddedImageDisplaySize(img, fitted.width, fitted.height);
    insertEmbeddedImageAtSelection(handle.el, img);
    selectEmbeddedImage(handle.el, img);
    handle.commit();
    const scaled =
      fitted.width < natural.width
        ? ` (scaled to ${fitted.width}×${fitted.height}; drag SE corner to resize)`
        : " (drag SE corner to resize)";
    store.setStatus(`Inserted ${file.name} (${id})${scaled}`);
  } catch (err) {
    const message = err instanceof Error ? err.message : "Image insert failed";
    useProjectStore.getState().setStatus(message);
  }
}
