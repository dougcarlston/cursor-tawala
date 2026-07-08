import { useCallback, useRef, useState, type PointerEvent } from "react";

interface Offset {
  x: number;
  y: number;
}

/**
 * Drag a modal by its title bar (legacy WinForms behavior).
 * Returns pixel offset from the overlay's centered origin.
 */
export function useDraggableDialog(initial: Offset = { x: 0, y: 0 }) {
  const [offset, setOffset] = useState(initial);
  const dragRef = useRef<{
    startX: number;
    startY: number;
    origX: number;
    origY: number;
  } | null>(null);

  const onPointerDown = useCallback(
    (e: PointerEvent<HTMLElement>) => {
      if (e.button !== 0) return;
      e.preventDefault();
      dragRef.current = {
        startX: e.clientX,
        startY: e.clientY,
        origX: offset.x,
        origY: offset.y,
      };
      e.currentTarget.setPointerCapture(e.pointerId);
    },
    [offset.x, offset.y],
  );

  const onPointerMove = useCallback((e: PointerEvent<HTMLElement>) => {
    if (!dragRef.current) return;
    setOffset({
      x: dragRef.current.origX + (e.clientX - dragRef.current.startX),
      y: dragRef.current.origY + (e.clientY - dragRef.current.startY),
    });
  }, []);

  const endDrag = useCallback((e: PointerEvent<HTMLElement>) => {
    dragRef.current = null;
    try {
      e.currentTarget.releasePointerCapture(e.pointerId);
    } catch {
      /* already released */
    }
  }, []);

  return {
    offset,
    titleBarProps: {
      onPointerDown,
      onPointerMove,
      onPointerUp: endDrag,
      onPointerCancel: endDrag,
    },
  };
}
