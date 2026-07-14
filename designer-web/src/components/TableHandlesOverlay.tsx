import { useCallback, useEffect, useRef, useState } from "react";
import { reflowAllPlacedLines } from "@/lib/documentCanvas";
import {
  findActiveUserTable,
  getTablePositionPt,
  parseCssPt,
  pxToPt,
  resizeTableHeight,
  resizeTableWidth,
  setTableColumnWidths,
  setTablePositionPt,
  setTableRowHeights,
  tableBoxInContainer,
  tableColumnWidthsPt,
  tableRowHeightsPt,
} from "@/lib/tableLayout";

interface Props {
  editorRef: React.RefObject<HTMLElement | null>;
  onCommit: () => void;
}

type DragKind =
  | { type: "move"; startLeftPt: number; startTopPt: number }
  | { type: "edge-e"; startWidthPt: number }
  | { type: "edge-s"; startHeightPt: number }
  | { type: "corner-se"; startWidthPt: number; startHeightPt: number }
  | { type: "col"; index: number; startWidths: number[] }
  | { type: "row"; index: number; startHeights: number[] };

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
 * Selection chrome for `table.user` — one top-left move handle (anchor corner),
 * edge/corner resize, and column/row dividers.
 * Float wrap toggles (left/block/right) stay omitted.
 */
export function TableHandlesOverlay({ editorRef, onCommit }: Props) {
  const [table, setTable] = useState<HTMLTableElement | null>(null);
  const [box, setBox] = useState<{ top: number; left: number; width: number; height: number } | null>(
    null,
  );
  const dragRef = useRef<DragKind | null>(null);
  const tableRef = useRef<HTMLTableElement | null>(null);

  const sync = useCallback(() => {
    const editor = editorRef.current;
    const container = editor?.parentElement;
    if (!editor || !container) {
      setTable(null);
      setBox(null);
      return;
    }
    const active = tableRef.current ?? findActiveUserTable(editor);
    if (!active || !editor.contains(active)) {
      if (!dragRef.current) {
        setTable(null);
        setBox(null);
      }
      return;
    }
    setTable(active);
    setBox(tableBoxInContainer(active, container));
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

  if (!table || !box) return null;

  const colCount = table.querySelector("tr")?.cells.length ?? 0;
  const rowCount = (table.tBodies[0]?.rows.length ?? table.rows.length) || 0;

  const startDrag = (kind: DragKind, cursor: string) => (e: React.PointerEvent) => {
    tableRef.current = table;
    dragRef.current = kind;
    document.body.style.cursor = cursor;
    attachPointerDrag(
      e,
      (dxPx, dyPx) => {
        const t = tableRef.current;
        if (!t) return;
        const dxPt = pxToPt(dxPx);
        const dyPt = pxToPt(dyPx);
        switch (kind.type) {
          case "move":
            setTablePositionPt(t, kind.startLeftPt + dxPt, kind.startTopPt + dyPt);
            break;
          case "edge-e":
            resizeTableWidth(t, kind.startWidthPt + dxPt);
            break;
          case "edge-s":
            resizeTableHeight(t, kind.startHeightPt + dyPt);
            break;
          case "corner-se":
            resizeTableWidth(t, kind.startWidthPt + dxPt);
            resizeTableHeight(t, kind.startHeightPt + dyPt);
            break;
          case "col": {
            const widths = [...kind.startWidths];
            const left = widths[kind.index];
            const right = widths[kind.index + 1];
            const nextLeft = Math.min(
              left + right - 18,
              Math.max(18, left + dxPt),
            );
            widths[kind.index] = nextLeft;
            widths[kind.index + 1] = left + right - nextLeft;
            setTableColumnWidths(t, widths);
            break;
          }
          case "row": {
            const heights = [...kind.startHeights];
            const top = heights[kind.index];
            const bottom = heights[kind.index + 1];
            const nextTop = Math.min(top + bottom - 12, Math.max(12, top + dyPt));
            heights[kind.index] = nextTop;
            heights[kind.index + 1] = top + bottom - nextTop;
            setTableRowHeights(t, heights);
            break;
          }
        }
        sync();
      },
      () => {
        const t = tableRef.current;
        const editor = editorRef.current;
        dragRef.current = null;
        tableRef.current = null;
        // Document absolute layout: resolve same-column overlaps after move; keep
        // intentional gaps / beside placement (do not pack into a single stack).
        if (t && editor && (t.style.position || "").toLowerCase() === "absolute") {
          reflowAllPlacedLines(editor);
        }
        onCommit();
        sync();
      },
    );
  };

  const totalWidthPt =
    tableColumnWidthsPt(table).reduce((a, b) => a + b, 0) || parseCssPt(table.style.width);
  const totalHeightPt =
    tableRowHeightsPt(table).reduce((a, b) => a + b, 0) || box.height * 0.75;
  const movePos = getTablePositionPt(table);

  const colDividerAt = (index: number) => {
    const row = table.querySelector("tr");
    if (!row || index >= row.cells.length - 1) return null;
    const leftCell = row.cells[index];
    const rightCell = row.cells[index + 1];
    const container = editorRef.current?.parentElement;
    if (!container) return null;
    const leftR = leftCell.getBoundingClientRect();
    const rightR = rightCell.getBoundingClientRect();
    const containerR = container.getBoundingClientRect();
    const x =
      (leftR.right + rightR.left) / 2 - containerR.left + container.scrollLeft;
    return x - box.left;
  };

  const rowDividerAt = (index: number) => {
    const tbody = table.tBodies[0] ?? table;
    if (index >= tbody.rows.length - 1) return null;
    const topRow = tbody.rows[index];
    const bottomRow = tbody.rows[index + 1];
    const container = editorRef.current?.parentElement;
    if (!container) return null;
    const topR = topRow.getBoundingClientRect();
    const bottomR = bottomRow.getBoundingClientRect();
    const containerR = container.getBoundingClientRect();
    const y =
      (topR.bottom + bottomR.top) / 2 - containerR.top + container.scrollTop;
    return y - box.top;
  };

  return (
    <div
      className="table-handles-overlay"
      style={{
        top: box.top,
        left: box.left,
        width: box.width,
        height: box.height,
      }}
      onMouseDown={(e) => e.preventDefault()}
    >
      <div className="table-handles-frame" aria-hidden />

      <button
        type="button"
        className="table-handle table-handle-move"
        title="Move table"
        aria-label="Move table"
        onPointerDown={startDrag(
          {
            type: "move",
            startLeftPt: movePos.left,
            startTopPt: movePos.top,
          },
          "grab",
        )}
      >
        ✥
      </button>

      <button
        type="button"
        className="table-handle table-handle-e"
        title="Resize table width"
        aria-label="Resize table width"
        onPointerDown={startDrag({ type: "edge-e", startWidthPt: totalWidthPt }, "ew-resize")}
      />
      <button
        type="button"
        className="table-handle table-handle-s"
        title="Resize table height"
        aria-label="Resize table height"
        onPointerDown={startDrag({ type: "edge-s", startHeightPt: totalHeightPt }, "ns-resize")}
      />
      <button
        type="button"
        className="table-handle table-handle-se"
        title="Resize table width and height"
        aria-label="Resize table width and height"
        onPointerDown={startDrag(
          { type: "corner-se", startWidthPt: totalWidthPt, startHeightPt: totalHeightPt },
          "nwse-resize",
        )}
      />

      {Array.from({ length: Math.max(0, colCount - 1) }, (_, i) => {
        const x = colDividerAt(i);
        if (x == null) return null;
        return (
          <button
            key={`col-${i}`}
            type="button"
            className="table-handle table-handle-col"
            style={{ left: x }}
            title="Resize column"
            aria-label={`Resize column ${i + 1}`}
            onPointerDown={startDrag({ type: "col", index: i, startWidths: tableColumnWidthsPt(table) }, "col-resize")}
          />
        );
      })}

      {Array.from({ length: Math.max(0, rowCount - 1) }, (_, i) => {
        const y = rowDividerAt(i);
        if (y == null) return null;
        return (
          <button
            key={`row-${i}`}
            type="button"
            className="table-handle table-handle-row"
            style={{ top: y }}
            title="Resize row"
            aria-label={`Resize row ${i + 1}`}
            onPointerDown={startDrag({ type: "row", index: i, startHeights: tableRowHeightsPt(table) }, "row-resize")}
          />
        );
      })}
    </div>
  );
}
