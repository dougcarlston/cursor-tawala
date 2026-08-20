/**
 * Project → Page Header… — plain text + optional banner image with pan/zoom/stretch.
 * Legacy: `PageHeaderDialog` / `PageHeaderPresenter`.
 * Bake keeps source crop resolution (not CSS 520×160) so Deploy stays sharp.
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
  pageHeaderSourceImage,
} from "@/lib/pageHeader";
import {
  PAGE_HEADER_FRAME_H,
  PAGE_HEADER_FRAME_W,
  type PageHeaderImageViewport,
  bakePageHeaderViewport,
  clampViewport,
  coverViewport,
} from "@/lib/pageHeaderImageFit";
import type { TawalaImageFormat } from "@/types/tawala";

interface Props {
  open: boolean;
  onClose: () => void;
}

interface PendingImage {
  /** Full-resolution source (before bake). */
  sourceData: string;
  sourceFormat: TawalaImageFormat;
  fileName?: string;
  naturalW: number;
  naturalH: number;
  previewUrl: string;
  viewport: PageHeaderImageViewport;
}

type DragMode = "pan" | "zoom" | "stretchX" | null;

export function PageHeaderDialog({ open, onClose }: Props) {
  const project = useProjectStore((s) => s.project);
  const setPageHeader = useProjectStore((s) => s.setPageHeader);
  const setStatus = useProjectStore((s) => s.setStatus);
  const fileRef = useRef<HTMLInputElement>(null);
  const frameRef = useRef<HTMLDivElement>(null);
  const dragRef = useRef<{
    mode: DragMode;
    startX: number;
    startY: number;
    origin: PageHeaderImageViewport;
  } | null>(null);
  /** Keep natural size / viewport for pointer handlers without rebinding every move. */
  const pendingRef = useRef<PendingImage | null>(null);

  const [text, setText] = useState("");
  const [pendingImage, setPendingImage] = useState<PendingImage | null>(null);
  /** True after Remove until Browse again — OK clears the stored image. */
  const [imageRemoved, setImageRemoved] = useState(false);
  const [baking, setBaking] = useState(false);

  pendingRef.current = pendingImage;

  useEffect(() => {
    if (!open) return;
    const ph = project.pageHeader;
    setText(ph?.text ?? "");
    setImageRemoved(false);
    // Prefer full original + saved viewport so OK → Push → reopen stays editable.
    const source = pageHeaderSourceImage(project);
    const baked = pageHeaderImage(project);
    const existing = source ?? baked;
    if (existing) {
      const previewUrl = dataUrlForImage(existing);
      void loadImageNaturalSize(previewUrl).then((sz) => {
        const saved = ph?.imageViewport;
        const viewport =
          source && saved
            ? clampViewport(saved, sz.width, sz.height)
            : coverViewport(sz.width, sz.height);
        setPendingImage({
          sourceData: existing.data,
          sourceFormat: existing.imageFormat,
          fileName: existing.fileName,
          naturalW: sz.width,
          naturalH: sz.height,
          previewUrl,
          viewport,
        });
      });
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

  useEffect(() => {
    if (!open) return;
    const onMove = (e: PointerEvent) => {
      const drag = dragRef.current;
      const img = pendingRef.current;
      if (!drag || !drag.mode || !img) return;
      const dx = e.clientX - drag.startX;
      const dy = e.clientY - drag.startY;
      let next = { ...drag.origin };
      if (drag.mode === "pan") {
        next.x = drag.origin.x + dx;
        next.y = drag.origin.y + dy;
      } else if (drag.mode === "zoom") {
        // Drag right/down = zoom in (uniform).
        const factor = Math.exp((dx + dy) * 0.004);
        next.scaleX = Math.max(0.05, drag.origin.scaleX * factor);
        next.scaleY = Math.max(0.05, drag.origin.scaleY * factor);
        // Keep frame center stable while zooming.
        const cx = PAGE_HEADER_FRAME_W / 2;
        const cy = PAGE_HEADER_FRAME_H / 2;
        const sx = (cx - drag.origin.x) / drag.origin.scaleX;
        const sy = (cy - drag.origin.y) / drag.origin.scaleY;
        next.x = cx - sx * next.scaleX;
        next.y = cy - sy * next.scaleY;
      } else if (drag.mode === "stretchX") {
        const factor = Math.exp(dx * 0.006);
        next.scaleX = Math.max(0.05, drag.origin.scaleX * factor);
        const cx = PAGE_HEADER_FRAME_W / 2;
        const sx = (cx - drag.origin.x) / drag.origin.scaleX;
        next.x = cx - sx * next.scaleX;
      }
      next = clampViewport(next, img.naturalW, img.naturalH);
      setPendingImage((cur) => (cur ? { ...cur, viewport: next } : cur));
    };
    const onUp = () => {
      dragRef.current = null;
    };
    // Stable listeners — do not depend on pendingImage (that rebinding killed pan).
    window.addEventListener("pointermove", onMove);
    window.addEventListener("pointerup", onUp);
    window.addEventListener("pointercancel", onUp);
    return () => {
      window.removeEventListener("pointermove", onMove);
      window.removeEventListener("pointerup", onUp);
      window.removeEventListener("pointercancel", onUp);
    };
  }, [open]);

  if (!open) return null;

  const showImage = !imageRemoved && pendingImage;
  const panRoomY =
    showImage != null
      ? Math.max(0, showImage.naturalH * showImage.viewport.scaleY - PAGE_HEADER_FRAME_H)
      : 0;
  const imageLabel = showImage
    ? panRoomY > 0.5
      ? `Image (${showImage.naturalW}×${showImage.naturalH} px) — drag to pan; corner = zoom; side = stretch`
      : `Image (${showImage.naturalW}×${showImage.naturalH} px) — zoom in (corner) first, then drag to pan; side = stretch`
    : "Image";

  const beginDrag = (mode: DragMode, e: React.PointerEvent) => {
    if (!pendingImage || !mode) return;
    e.preventDefault();
    e.stopPropagation();
    (e.currentTarget as HTMLElement).setPointerCapture?.(e.pointerId);
    dragRef.current = {
      mode,
      startX: e.clientX,
      startY: e.clientY,
      origin: { ...pendingImage.viewport },
    };
  };

  const nudgeVertical = (where: "top" | "center" | "bottom") => {
    setPendingImage((cur) => {
      if (!cur) return cur;
      const dh = cur.naturalH * cur.viewport.scaleY;
      let y = cur.viewport.y;
      if (dh <= PAGE_HEADER_FRAME_H) {
        y = (PAGE_HEADER_FRAME_H - dh) / 2;
      } else if (where === "top") {
        y = 0; // image top aligned with frame → show top of photo
      } else if (where === "bottom") {
        y = PAGE_HEADER_FRAME_H - dh;
      } else {
        y = (PAGE_HEADER_FRAME_H - dh) / 2;
      }
      return {
        ...cur,
        viewport: clampViewport({ ...cur.viewport, y }, cur.naturalW, cur.naturalH),
      };
    });
  };

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
        sourceData: parsed.data,
        sourceFormat: imageFormat,
        fileName: file.name,
        naturalW: natural.width,
        naturalH: natural.height,
        previewUrl: dataUrl,
        viewport: coverViewport(natural.width, natural.height),
      });
      setImageRemoved(false);
    } catch {
      setStatus("Could not open that image file");
    }
  };

  const onOk = async () => {
    if (imageRemoved || !pendingImage) {
      const next = applyPageHeaderToProject(project, {
        text,
        image: null,
      });
      setPageHeader(next.pageHeader, next.images ?? []);
      setStatus(next.pageHeader ? "Page Header saved" : "Page Header cleared");
      onClose();
      return;
    }
    setBaking(true);
    try {
      const baked = await bakePageHeaderViewport(
        pendingImage.previewUrl,
        pendingImage.naturalW,
        pendingImage.naturalH,
        pendingImage.viewport,
      );
      const parsed = parseDataUrl(baked.dataUrl);
      if (!parsed) {
        setStatus("Could not prepare Page Header image");
        return;
      }
      const next = applyPageHeaderToProject(project, {
        text,
        image: {
          data: parsed.data,
          imageFormat: "PNG",
          fileName: pendingImage.fileName,
        },
        sourceImage: {
          data: pendingImage.sourceData,
          imageFormat: pendingImage.sourceFormat,
          fileName: pendingImage.fileName,
        },
        viewport: pendingImage.viewport,
        width: baked.width,
        height: baked.height,
      });
      setPageHeader(next.pageHeader, next.images ?? []);
      setStatus("Page Header saved — Push to preview; reopen to keep adjusting");
      onClose();
    } catch {
      setStatus("Could not prepare Page Header image");
    } finally {
      setBaking(false);
    }
  };

  const vp = showImage?.viewport;
  const imgStyle =
    showImage && vp
      ? {
          position: "absolute" as const,
          left: vp.x,
          top: vp.y,
          width: showImage.naturalW * vp.scaleX,
          height: showImage.naturalH * vp.scaleY,
          maxWidth: "none",
          maxHeight: "none",
          userSelect: "none" as const,
          pointerEvents: "none" as const,
        }
      : undefined;

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
              <div
                ref={frameRef}
                className={`page-header-image-preview${showImage ? " page-header-image-preview-edit" : ""}`}
                style={{ width: PAGE_HEADER_FRAME_W, height: PAGE_HEADER_FRAME_H }}
                onPointerDown={(e) => beginDrag("pan", e)}
              >
                {showImage ? (
                  <>
                    <img src={showImage.previewUrl} alt="" draggable={false} style={imgStyle} />
                    <div className="page-header-crop-highlight" aria-hidden>
                      <button
                        type="button"
                        className="page-header-handle page-header-handle-zoom"
                        title="Drag to zoom"
                        onPointerDown={(e) => beginDrag("zoom", e)}
                      />
                      <button
                        type="button"
                        className="page-header-handle page-header-handle-stretch"
                        title="Drag to stretch sideways"
                        onPointerDown={(e) => beginDrag("stretchX", e)}
                      />
                    </div>
                  </>
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
                {showImage ? (
                  <>
                    <button
                      type="button"
                      title="Align photo so the top is visible in the banner"
                      onClick={() => nudgeVertical("top")}
                    >
                      Show top
                    </button>
                    <button
                      type="button"
                      title="Center the photo in the banner"
                      onClick={() => nudgeVertical("center")}
                    >
                      Show center
                    </button>
                    <button
                      type="button"
                      title="Align photo so the bottom is visible in the banner"
                      onClick={() => nudgeVertical("bottom")}
                    >
                      Show bottom
                    </button>
                    <button
                      type="button"
                      title="Reset pan / zoom / stretch to cover"
                      onClick={() => {
                        setPendingImage((cur) =>
                          cur
                            ? {
                                ...cur,
                                viewport: coverViewport(cur.naturalW, cur.naturalH),
                              }
                            : cur,
                        );
                      }}
                    >
                      Reset view
                    </button>
                  </>
                ) : null}
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
          <button type="button" onClick={() => void onOk()} disabled={baking}>
            {baking ? "Preparing…" : "OK"}
          </button>
          <button type="button" onClick={onClose} disabled={baking}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

/** @deprecated keep import stable if tests reference id */
export { PAGE_HEADER_IMAGE_ID };
