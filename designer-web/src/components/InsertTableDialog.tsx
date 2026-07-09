import { useEffect, useRef, useState } from "react";
import type { InsertTableOptions } from "@/lib/paletteCommands";

interface Props {
  onCancel: () => void;
  onInsert: (options: InsertTableOptions) => void;
}

/** Legacy `InsertTableView` / `InsertTableDialog` — width in inches, rows, columns. */
const WIDTH_MIN = 1;
const WIDTH_MAX = 7.5;
const WIDTH_STEP = 0.5;
const WIDTH_DEFAULT = 3;
const ROWS_DEFAULT = 4;
const COLUMNS_DEFAULT = 2;

export function InsertTableDialog({ onCancel, onInsert }: Props) {
  const [widthInches, setWidthInches] = useState(WIDTH_DEFAULT);
  const [rows, setRows] = useState(ROWS_DEFAULT);
  const [columns, setColumns] = useState(COLUMNS_DEFAULT);
  const widthRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    const el = widthRef.current;
    if (!el) return;
    el.focus();
    el.select();
  }, []);

  const clampWidth = (value: number) =>
    Math.min(WIDTH_MAX, Math.max(WIDTH_MIN, Math.round(value / WIDTH_STEP) * WIDTH_STEP));

  const submit = (e?: React.FormEvent) => {
    e?.preventDefault();
    onInsert({
      widthInches: clampWidth(widthInches),
      rows: Math.max(1, Math.min(200, Math.floor(rows))),
      columns: Math.max(1, Math.min(20, Math.floor(columns))),
    });
  };

  return (
    <div
      className="modal-backdrop insert-table-backdrop"
      role="presentation"
      onMouseDown={(e) => {
        if (e.target === e.currentTarget) onCancel();
      }}
    >
      <div
        className="modal insert-table-dialog"
        role="dialog"
        aria-labelledby="insert-table-title"
        aria-modal="true"
        onMouseDown={(e) => e.stopPropagation()}
      >
        <h2 id="insert-table-title">Insert Table</h2>
        <form onSubmit={submit}>
          <div className="insert-table-fields">
            <label className="insert-table-field">
              <span>Table Width (inches):</span>
              <input
                ref={widthRef}
                type="number"
                min={WIDTH_MIN}
                max={WIDTH_MAX}
                step={WIDTH_STEP}
                value={widthInches}
                onChange={(e) => setWidthInches(Number(e.target.value))}
              />
            </label>
            <label className="insert-table-field">
              <span>Rows:</span>
              <input
                type="number"
                min={1}
                max={200}
                step={1}
                value={rows}
                onChange={(e) => setRows(Number(e.target.value))}
              />
            </label>
            <label className="insert-table-field">
              <span>Columns:</span>
              <input
                type="number"
                min={1}
                max={20}
                step={1}
                value={columns}
                onChange={(e) => setColumns(Number(e.target.value))}
              />
            </label>
          </div>
          <div className="modal-actions">
            <button type="button" onClick={onCancel}>
              Cancel
            </button>
            <button type="submit">OK</button>
          </div>
        </form>
      </div>
    </div>
  );
}
