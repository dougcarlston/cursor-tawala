import { useCallback, useEffect, useRef, useState } from "react";
import {
  EMBEDDED_IMAGE_HANDLES_CLASS,
  applyEmbeddedImageDisplaySize,
  findActiveEmbeddedImage,
  imageBoxInContainer,
  readEmbeddedImageSize,
  selectEmbeddedImage,
  sizeFromSeDrag,
} from "@/lib/embeddedImageResize";

interface Props {
  editorRef: React.RefObject<HTMLElement | null>;
  onCommit: () => void;
}

function attachPointerDrag(
  e: React.PointerEvent,
  onMove: (dxPx: number, dyPx: number) => void,
  onEnd: () => void,
) {
  e.preventDefault();
  e.stopPropagation();
  const startX = e.clientX;
  const startY = e.clientY;
  const move = (ev: PointerEvent) => onMove(ev.clientX - startX, ev.clientY - startY);
  const up = () => {
    window.removeEventListener("pointermove", move);
    window.removeEventListener("pointerup", up);
    document.body.style.cursor = "";
    document.body.style.userSelect = "";
    onEnd();
  };
  document.body.style.userSelect = "none";
  window.addEventListener("pointermove", move);
  window.addEventListener("pointerup", up);
}

/**
 * SE-corner resize chrome for From-your-PC embedded images (aspect locked).
 */
export function EmbeddedImageHandlesOverlay({ editorRef, onCommit }: Props) {
  const [img, setImg] = useState<HTMLImageElement | null>(null);
  const [box, setBox] = useState<{ top: number; left: number; width: number; height: number } | null>(
    null,
  );
  const imgRef = useRef<HTMLImageElement | null>(null);
  const draggingRef = useRef(false);

  const sync = useCallback(() => {
    const editor = editorRef.current;
    const container = editor?.parentElement;
    if (!editor || !container) {
      setImg(null);
      setBox(null);
      return;
    }
    const active = imgRef.current ?? findActiveEmbeddedImage(editor);
    if (!active || !editor.contains(active)) {
      if (!draggingRef.current) {
        setImg(null);
        setBox(null);
      }
      return;
    }
    selectEmbeddedImage(editor, active);
    setImg(active);
    setBox(imageBoxInContainer(active, container));
  }, [editorRef]);

  useEffect(() => {
    sync();
    const editor = editorRef.current;
    const container = editor?.parentElement;
    document.addEventListener("selectionchange", sync);
    window.addEventListener("resize", sync);
    container?.addEventListener("scroll", sync, { passive: true });
    editor?.addEventListener("input", sync);
    return () => {
      document.removeEventListener("selectionchange", sync);
      window.removeEventListener("resize", sync);
      container?.removeEventListener("scroll", sync);
      editor?.removeEventListener("input", sync);
    };
  }, [editorRef, sync]);

  if (!img || !box) return null;

  const startSe = (e: React.PointerEvent) => {
    const editor = editorRef.current;
    if (!editor) return;
    imgRef.current = img;
    draggingRef.current = true;
    const start = readEmbeddedImageSize(img);
    const maxWidth = Math.max(48, editor.clientWidth - 8);
    document.body.style.cursor = "nwse-resize";
    attachPointerDrag(
      e,
      (dx, dy) => {
        const target = imgRef.current;
        if (!target) return;
        const next = sizeFromSeDrag(start.width, start.height, dx, dy, maxWidth);
        applyEmbeddedImageDisplaySize(target, next.width, next.height);
        const container = editorRef.current?.parentElement;
        if (container) setBox(imageBoxInContainer(target, container));
      },
      () => {
        draggingRef.current = false;
        onCommit();
        sync();
      },
    );
  };

  return (
    <div
      className={EMBEDDED_IMAGE_HANDLES_CLASS}
      style={{
        top: box.top,
        left: box.left,
        width: box.width,
        height: box.height,
      }}
      onMouseDown={(e) => {
        // Keep focus in the editor; only the SE handle starts a drag.
        if (!(e.target as HTMLElement).classList.contains("embedded-image-handle-se")) {
          e.preventDefault();
        }
      }}
    >
      <div className="embedded-image-handles-frame" aria-hidden />
      <button
        type="button"
        className="embedded-image-handle embedded-image-handle-se"
        title="Drag to resize image"
        aria-label="Resize image"
        onPointerDown={startSe}
      />
    </div>
  );
}
