import { useEffect, useMemo, useRef, useState, type MouseEvent } from "react";
import {
  FUNCTION_CATEGORIES,
  getFunctionDef,
  type FunctionDef,
} from "@/lib/functionCatalog";

interface Props {
  onCancel: () => void;
  onSelect: (def: FunctionDef) => void;
  /** When editing, pre-select this function in the list. */
  initialFunctionId?: string;
}

/** Legacy `InsertFunctionDialog` — category dropdown + function list + description pane. */
export function InsertFunctionDialog({ onCancel, onSelect, initialFunctionId }: Props) {
  const [categoryId, setCategoryId] = useState(FUNCTION_CATEGORIES[0]?.id ?? "all");
  const category = FUNCTION_CATEGORIES.find((c) => c.id === categoryId) ?? FUNCTION_CATEGORIES[0];
  const functions = useMemo(
    () =>
      (category?.functionIds ?? [])
        .map((id) => getFunctionDef(id))
        .filter((f): f is FunctionDef => !!f && !f.formOnly && !f.pickerHidden),
    [category],
  );
  const [selectedId, setSelectedId] = useState(
    initialFunctionId && functions.some((f) => f.id === initialFunctionId)
      ? initialFunctionId
      : (functions[0]?.id ?? ""),
  );
  const listRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    if (functions.some((f) => f.id === selectedId)) return;
    setSelectedId(functions[0]?.id ?? "");
  }, [functions, selectedId]);

  useEffect(() => {
    listRef.current?.querySelector<HTMLElement>(".insert-function-item.selected")?.scrollIntoView({
      block: "nearest",
    });
  }, [selectedId]);

  const selected = getFunctionDef(selectedId);

  const confirm = (e?: MouseEvent) => {
    e?.preventDefault();
    e?.stopPropagation();
    if (!selected) return;
    onSelect(selected);
  };

  return (
    <div
      className="modal-backdrop insert-function-backdrop"
      role="presentation"
      onClick={(e) => {
        if (e.target === e.currentTarget) onCancel();
      }}
    >
      <div
        className="modal insert-function-dialog"
        role="dialog"
        aria-labelledby="insert-function-title"
        aria-modal="true"
        onMouseDown={(e) => e.stopPropagation()}
      >
        <h2 id="insert-function-title">Insert Function</h2>
        <div className="insert-function-body">
          <label className="insert-function-category">
            <span>Select a category:</span>
            <select
              value={categoryId}
              onChange={(e) => setCategoryId(e.target.value)}
              autoFocus
            >
              {FUNCTION_CATEGORIES.map((cat) => (
                <option key={cat.id} value={cat.id}>
                  {cat.label}
                </option>
              ))}
            </select>
          </label>
          <div className="insert-function-list-wrap">
            <span className="insert-function-list-label">Select a function:</span>
            <div className="insert-function-list" ref={listRef} role="listbox" aria-label="Functions">
              {functions.map((fn) => (
                <button
                  key={fn.id}
                  type="button"
                  role="option"
                  aria-selected={fn.id === selectedId}
                  className={`insert-function-item${fn.id === selectedId ? " selected" : ""}`}
                  onClick={() => setSelectedId(fn.id)}
                  onDoubleClick={(e) => confirm(e)}
                >
                  {fn.name}
                </button>
              ))}
            </div>
          </div>
          <div className="insert-function-description" aria-live="polite">
            <strong>{selected?.name ?? ""}</strong>
            <p>{selected?.description ?? ""}</p>
          </div>
        </div>
        <div className="modal-actions">
          <button type="button" onClick={onCancel}>
            Cancel
          </button>
          <button
            type="button"
            disabled={!selected}
            // preventDefault on mousedown so the OK click is not retargeted onto
            // **fx** / chrome when Insert unmounts and Configure mounts underneath.
            onMouseDown={(e) => {
              e.preventDefault();
              e.stopPropagation();
            }}
            onClick={(e) => confirm(e)}
          >
            OK
          </button>
        </div>
      </div>
    </div>
  );
}
