import { useCallback, useEffect, useRef, useState } from "react";
import {
  listPlacedBlocksInSelection,
  PLACED_TEXT_CLASS,
  reflowAllPlacedLines,
} from "@/lib/documentCanvas";
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

/** Union of one or more placed-line boxes (multi-paragraph selection). */
function unionBoxes(
  blocks: HTMLElement[],
  container: HTMLElement,
): { top: number; left: number; width: number; height: number } | null {
  if (!blocks.length) return null;
  let top = Infinity;
  let left = Infinity;
  let right = -Infinity;
  let bottom = -Infinity;
  for (const block of blocks) {
    const box = blockBoxInContainer(block, container);
    top = Math.min(top, box.top);
    left = Math.min(left, box.left);
    right = Math.max(right, box.left + box.width);
    bottom = Math.max(bottom, box.top + box.height);
  }
  return { top, left, width: Math.max(0, right - left), height: Math.max(0, bottom - top) };
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
 * Move handle (✥) for absolutely positioned `.doc-placed-text` blocks.
 * When the selection spans multiple placed lines, drag moves **all** of them together.
 */
export function PlacedTextHandlesOverlay({ editorRef, onCommit }: Props) {
  const [blocks, setBlocks] = useState<HTMLElement[]>([]);
  const [box, setBox] = useState<{ top: number; left: number; width: number; height: number } | null>(
    null,
  );
  const dragRef = useRef(false);
  const blocksRef = useRef<HTMLElement[]>([]);
  const originsRef = useRef<Array<{ left: number; top: number }>>([]);

  const sync = useCallback(() => {
    const editor = editorRef.current;
    const container = editor?.parentElement;
    if (!editor || !container) {
      setBlocks([]);
      setBox(null);
      return;
    }
    const active = dragRef.current
      ? blocksRef.current.filter((b) => editor.contains(b))
      : listPlacedBlocksInSelection(editor).filter(
          (b) => b instanceof HTMLElement && b.classList.contains(PLACED_TEXT_CLASS),
        );
    if (!active.length) {
      if (!dragRef.current) {
        setBlocks([]);
        setBox(null);
      }
      return;
    }
    setBlocks(active);
    setBox(unionBoxes(active, container));
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

  if (!blocks.length || !box) return null;

  const primary = blocks[0];

  const startMove = (e: React.PointerEvent) => {
    blocksRef.current = blocks.slice();
    originsRef.current = blocks.map((b) => getAbsolutePositionPt(b));
    dragRef.current = true;
    document.body.style.cursor = "move";
    attachPointerDrag(
      e,
      (dxPx, dyPx) => {
        const moving = blocksRef.current;
        const origins = originsRef.current;
        const dx = pxToPt(dxPx);
        const dy = pxToPt(dyPx);
        moving.forEach((b, i) => {
          const origin = origins[i];
          if (!origin) return;
          setAbsolutePositionPt(b, origin.left + dx, origin.top + dy);
        });
        const container = editorRef.current?.parentElement;
        if (container) setBox(unionBoxes(moving, container));
      },
      () => {
        const editor = editorRef.current;
        dragRef.current = false;
        blocksRef.current = [];
        originsRef.current = [];
        if (editor) {
          reflowAllPlacedLines(editor);
        }
        onCommit();
        sync();
      },
    );
  };

  const label =
    blocks.length > 1
      ? `${blocks.length} lines`
      : `${formatPt(parseCssPt(primary.style.left))} × ${formatPt(parseCssPt(primary.style.top))}`;

  return (
    <div
      className="placed-text-handles-overlay table-handles-overlay"
      style={{ top: box.top, left: box.left, width: box.width, height: box.height }}
      aria-hidden
    >
      <button
        type="button"
        className="table-handle table-handle-move"
        title={blocks.length > 1 ? "Move selected lines" : "Move"}
        style={{ top: -10, left: -10 }}
        onPointerDown={startMove}
      >
        ✥
      </button>
      <span className="placed-text-handle-label" style={{ top: -10, left: 14 }}>
        {label}
      </span>
    </div>
  );
}
