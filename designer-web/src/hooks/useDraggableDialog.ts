import { useCallback, useRef, useState, type PointerEvent, type RefObject } from "react";

interface Offset {
  x: number;
  y: number;
}

interface Options {
  /** When set, keep the dialog rectangle inside the viewport while dragging. */
  dialogRef?: RefObject<HTMLElement | null>;
  /** Minimum visible px of the dialog that must remain on-screen (default 48). */
  minVisible?: number;
}

/**
 * Drag a modal by its title bar (legacy WinForms behavior).
 * Returns pixel offset from the overlay's centered origin.
 */
export function useDraggableDialog(initial: Offset = { x: 0, y: 0 }, options: Options = {}) {
  const [offset, setOffset] = useState(initial);
  const dragRef = useRef<{
    startX: number;
    startY: number;
    origX: number;
    origY: number;
    baseLeft: number;
    baseTop: number;
    width: number;
    height: number;
  } | null>(null);
  const optionsRef = useRef(options);
  optionsRef.current = options;

  const onPointerDown = useCallback(
    (e: PointerEvent<HTMLElement>) => {
      if (e.button !== 0) return;
      e.preventDefault();
      const el = optionsRef.current.dialogRef?.current;
      const rect = el?.getBoundingClientRect();
      dragRef.current = {
        startX: e.clientX,
        startY: e.clientY,
        origX: offset.x,
        origY: offset.y,
        baseLeft: rect ? rect.left - offset.x : 0,
        baseTop: rect ? rect.top - offset.y : 0,
        width: rect?.width ?? 0,
        height: rect?.height ?? 0,
      };
      e.currentTarget.setPointerCapture(e.pointerId);
    },
    [offset.x, offset.y],
  );

  const onPointerMove = useCallback((e: PointerEvent<HTMLElement>) => {
    if (!dragRef.current) return;
    const drag = dragRef.current;
    let x = drag.origX + (e.clientX - drag.startX);
    let y = drag.origY + (e.clientY - drag.startY);
    const { dialogRef, minVisible = 48 } = optionsRef.current;
    if (dialogRef?.current && drag.width > 0) {
      const maxX = window.innerWidth - minVisible - drag.baseLeft;
      const minX = minVisible - drag.width - drag.baseLeft;
      const maxY = window.innerHeight - minVisible - drag.baseTop;
      const minY = minVisible - drag.height - drag.baseTop;
      x = Math.min(maxX, Math.max(minX, x));
      y = Math.min(maxY, Math.max(minY, y));
    }
    setOffset({ x, y });
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
