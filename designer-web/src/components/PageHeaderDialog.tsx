/**
 * Project → Page Header… — plain text + optional banner image (Browse / Remove).
 * Legacy: `PageHeaderDialog` / `PageHeaderPresenter`.
 */

import { useEffect, useRef, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import {
  LOCAL_IMAGE_ACCEPT,
  dataUrlForImage,
  imageFormatFromMimeOrName,
  loadImageNaturalSize,
  parseDataUrl,
  readFileAsDataUrl,
} from "@/lib/projectImages";
import {
  PAGE_HEADER_IMAGE_ID,
  applyPageHeaderToProject,
  pageHeaderImage,
} from "@/lib/pageHeader";
import type { TawalaImageFormat } from "@/types/tawala";

interface Props {
  open: boolean;
  onClose: () => void;
}

interface PendingImage {
  data: string;
  imageFormat: TawalaImageFormat;
  fileName?: string;
  width: number;
  height: number;
  previewUrl: string;
}

export function PageHeaderDialog({ open, onClose }: Props) {
  const project = useProjectStore((s) => s.project);
  const setPageHeader = useProjectStore((s) => s.setPageHeader);
  const setStatus = useProjectStore((s) => s.setStatus);
  const fileRef = useRef<HTMLInputElement>(null);

  const [text, setText] = useState("");
  const [pendingImage, setPendingImage] = useState<PendingImage | null>(null);
  /** True after Remove until Browse again — OK clears the stored image. */
  const [imageRemoved, setImageRemoved] = useState(false);

  useEffect(() => {
    if (!open) return;
    const ph = project.pageHeader;
    setText(ph?.text ?? "");
    setImageRemoved(false);
    const existing = pageHeaderImage(project);
    if (existing) {
      const previewUrl = dataUrlForImage(existing);
      setPendingImage({
        data: existing.data,
        imageFormat: existing.imageFormat,
        fileName: existing.fileName,
        width: ph?.width ?? 0,
        height: ph?.height ?? 0,
        previewUrl,
      });
      if (!ph?.width || !ph?.height) {
        void loadImageNaturalSize(previewUrl).then((sz) => {
          setPendingImage((cur) =>
            cur
              ? { ...cur, width: sz.width, height: sz.height }
              : cur,
          );
        });
      }
    } else {
      setPendingImage(null);
    }
    // Seed once when opening — don't reset while typing if project updates.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [open]);

  useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [open, onClose]);

  if (!open) return null;

  const showImage = !imageRemoved && pendingImage;
  const imageLabel = showImage
    ? `Image (${showImage.width || "?"}×${showImage.height || "?"} px)`
    : "Image";

  const onBrowse = () => {
    fileRef.current?.click();
  };

  const onFilePicked = async (file: File | null) => {
    if (!file) return;
    if (!/^image\/(gif|jpeg|jpg|png)$/i.test(file.type) && !/\.(gif|jpe?g|png)$/i.test(file.name)) {
      setStatus("Choose a GIF, JPG, or PNG image");
      return;
    }
    try {
      const dataUrl = await readFileAsDataUrl(file);
      const parsed = parseDataUrl(dataUrl);
      if (!parsed) {
        setStatus("Could not read that image file");
        return;
      }
      const imageFormat =
        parsed.imageFormat || imageFormatFromMimeOrName(file.type, file.name);
      const natural = await loadImageNaturalSize(dataUrl);
      setPendingImage({
        data: parsed.data,
        imageFormat,
        fileName: file.name,
        width: natural.width,
        height: natural.height,
        previewUrl: dataUrl,
      });
      setImageRemoved(false);
    } catch {
      setStatus("Could not open that image file");
    }
  };

  const onOk = () => {
    const image =
      !imageRemoved && pendingImage
        ? {
            data: pendingImage.data,
            imageFormat: pendingImage.imageFormat,
            fileName: pendingImage.fileName,
          }
        : null;
    const next = applyPageHeaderToProject(project, {
      text,
      image,
      width: pendingImage && !imageRemoved ? pendingImage.width : undefined,
      height: pendingImage && !imageRemoved ? pendingImage.height : undefined,
    });
    setPageHeader(next.pageHeader, next.images ?? []);
    setStatus(
      next.pageHeader
        ? "Page Header saved — Push shows it on form pages"
        : "Page Header cleared",
    );
    onClose();
  };

  return (
    <div
      className="modal-overlay page-header-overlay"
      role="presentation"
      onClick={(e) => {
        if (e.target === e.currentTarget) onClose();
      }}
    >
      <div
        className="modal-dialog page-header-dialog"
        role="dialog"
        aria-labelledby="page-header-dialog-title"
        aria-modal="true"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-header page-header-dialog-titlebar">
          <h2 id="page-header-dialog-title">Page Header</h2>
          <button type="button" className="designer-dialog-close" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>

        <div className="page-header-dialog-body">
          <label className="page-header-text-label">
            Text
            <input
              type="text"
              className="page-header-text-input"
              value={text}
              onChange={(e) => setText(e.target.value)}
              autoFocus
              placeholder=""
            />
          </label>

          <fieldset className="page-header-image-group">
            <legend>{imageLabel}</legend>
            <div className="page-header-image-row">
              <div className="page-header-image-preview">
                {showImage ? (
                  <img src={showImage.previewUrl} alt="" />
                ) : (
                  <span className="page-header-image-empty">No image</span>
                )}
              </div>
              <div className="page-header-image-actions">
                <button type="button" onClick={onBrowse} title="Browse for image">
                  Browse…
                </button>
                <button
                  type="button"
                  className="page-header-remove"
                  disabled={!showImage}
                  onClick={() => {
                    setImageRemoved(true);
                    setPendingImage(null);
                  }}
                  title="Remove image"
                >
                  Remove
                </button>
              </div>
            </div>
          </fieldset>
        </div>

        <input
          ref={fileRef}
          type="file"
          accept={LOCAL_IMAGE_ACCEPT}
          hidden
          onChange={(e) => {
            const file = e.target.files?.[0] ?? null;
            e.target.value = "";
            void onFilePicked(file);
          }}
        />

        <div className="modal-footer">
          <button type="button" onClick={onOk}>
            OK
          </button>
          <button type="button" onClick={onClose}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

/** @deprecated keep import stable if tests reference id */
export { PAGE_HEADER_IMAGE_ID };
