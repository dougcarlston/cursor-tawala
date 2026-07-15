import { useCallback, useEffect, useRef, useState } from "react";
import { PLACED_TEXT_CLASS, reflowAllPlacedLines } from "@/lib/documentCanvas";
import {
  formatPt,
  getAbsolutePositionPt,
  parseCssPt,
  pxToPt,
  setAbsolutePositionPt,
} from "@/lib/tableLayout";

interface Props {
  editorRef: React.RefObject<HTMLElement | null>;
  onCommit: () => void;
}

function findActivePlacedBlock(editor: HTMLElement): HTMLElement | null {
  const sel = window.getSelection();
  if (!sel || sel.rangeCount === 0) return null;
  let node: Node | null = sel.getRangeAt(0).commonAncestorContainer;
  if (node.nodeType === Node.TEXT_NODE) node = node.parentNode;
  while (node && node !== editor) {
    if (node instanceof HTMLElement && node.classList.contains(PLACED_TEXT_CLASS)) {
      return node;
    }
    node = node.parentNode;
  }
  return null;
}

function blockBoxInContainer(
  block: HTMLElement,
  container: HTMLElement,
): { top: number; left: number; width: number; height: number } {
  const blockRect = block.getBoundingClientRect();
  const containerRect = container.getBoundingClientRect();
  return {
    top: blockRect.top - containerRect.top,
    left: blockRect.left - containerRect.left,
    width: blockRect.width,
    height: blockRect.height,
  };
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

/** Move handle (✥) for absolutely positioned `.doc-placed-text` blocks on the Document canvas. */
export function PlacedTextHandlesOverlay({ editorRef, onCommit }: Props) {
  const [block, setBlock] = useState<HTMLElement | null>(null);
  const [box, setBox] = useState<{ top: number; left: number; width: number; height: number } | null>(
    null,
  );
  const dragRef = useRef(false);
  const blockRef = useRef<HTMLElement | null>(null);

  const sync = useCallback(() => {
    const editor = editorRef.current;
    const container = editor?.parentElement;
    if (!editor || !container) {
      setBlock(null);
      setBox(null);
      return;
    }
    const active = blockRef.current ?? findActivePlacedBlock(editor);
    if (!active || !editor.contains(active)) {
      if (!dragRef.current) {
        setBlock(null);
        setBox(null);
      }
      return;
    }
    setBlock(active);
    setBox(blockBoxInContainer(active, container));
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

  if (!block || !box) return null;

  const startMove = (e: React.PointerEvent) => {
    blockRef.current = block;
    dragRef.current = true;
    document.body.style.cursor = "move";
    const { left, top } = getAbsolutePositionPt(block);
    attachPointerDrag(
      e,
      (dxPx, dyPx) => {
        const b = blockRef.current;
        if (!b) return;
        setAbsolutePositionPt(b, left + pxToPt(dxPx), top + pxToPt(dyPx));
        const container = editorRef.current?.parentElement;
        if (b && container) setBox(blockBoxInContainer(b, container));
      },
      () => {
        const b = blockRef.current;
        const editor = editorRef.current;
        dragRef.current = false;
        blockRef.current = null;
        // Same packing as table ✥ — restore neighbors toward home when space frees.
        if (b && editor) {
          reflowAllPlacedLines(editor);
        }
        onCommit();
        sync();
      },
    );
  };

  return (
    <div
      className="placed-text-handles-overlay table-handles-overlay"
      style={{ top: box.top, left: box.left, width: box.width, height: box.height }}
      aria-hidden
    >
      <button
        type="button"
        className="table-handle table-handle-move"
        title="Move"
        style={{ top: -10, left: -10 }}
        onPointerDown={startMove}
      >
        ✥
      </button>
      <span className="placed-text-handle-label" style={{ top: -10, left: 14 }}>
        {formatPt(parseCssPt(block.style.left))} × {formatPt(parseCssPt(block.style.top))}
      </span>
    </div>
  );
}
