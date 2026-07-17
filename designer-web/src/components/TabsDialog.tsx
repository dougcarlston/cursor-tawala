import { useMemo, useState } from "react";
import {
  addTabStop,
  formatInches,
  parseInchesInput,
  TAB_MAX_INCHES,
} from "@/lib/tabStops";

interface Props {
  /** Current stops in inches (from selected form item). */
  initialStops: number[];
  /** False when no suitable form item is selected. */
  canApply: boolean;
  onCancel: () => void;
  onOk: (stops: number[]) => void;
}

/** Legacy TextEditor TabDialog — tab stops in inches. */
export function TabsDialog({ initialStops, canApply, onCancel, onOk }: Props) {
  const [stops, setStops] = useState<number[]>(() => [...initialStops].sort((a, b) => a - b));
  const [input, setInput] = useState("0.00");
  const [selected, setSelected] = useState<number | null>(null);

  const selectedIndex = useMemo(
    () => (selected == null ? -1 : stops.findIndex((s) => Math.abs(s - selected) < 0.001)),
    [stops, selected],
  );

  const trySet = () => {
    const inches = parseInchesInput(input);
    if (inches == null) return;
    const next = addTabStop(stops, inches);
    if (!next) return;
    setStops(next);
    setSelected(Math.round(inches * 100) / 100);
  };

  const clearSelected = () => {
    if (selectedIndex < 0) return;
    const next = stops.filter((_, i) => i !== selectedIndex);
    setStops(next);
    setSelected(next[0] ?? null);
  };

  return (
    <div className="modal-overlay configure-function-overlay" role="presentation">
      <div
        className="modal configure-function-dialog tabs-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="tabs-dialog-title"
      >
        <h2 id="tabs-dialog-title">Tabs</h2>
        <div className="configure-function-body tabs-dialog-body">
          <label className="tabs-dialog-input-row">
            Tab stop position
            <input
              type="text"
              value={input}
              onChange={(e) => setInput(e.target.value)}
              onKeyDown={(e) => {
                if (e.key === "Enter") {
                  e.preventDefault();
                  trySet();
                }
              }}
              aria-label="Tab stop position in inches"
            />
            <span className="tabs-dialog-units">inches (max {TAB_MAX_INCHES})</span>
          </label>
          <div className="tabs-dialog-list-wrap">
            <select
              className="tabs-dialog-list"
              size={8}
              value={selectedIndex >= 0 ? String(selectedIndex) : ""}
              onChange={(e) => {
                const i = Number(e.target.value);
                setSelected(Number.isFinite(i) ? stops[i] ?? null : null);
              }}
              aria-label="Tab stops"
            >
              {stops.map((s, i) => (
                <option key={`${s}-${i}`} value={String(i)}>
                  {formatInches(s)}
                </option>
              ))}
            </select>
            <div className="tabs-dialog-side-actions">
              <button type="button" onClick={trySet}>
                Set
              </button>
              <button type="button" disabled={selectedIndex < 0} onClick={clearSelected}>
                Clear
              </button>
              <button type="button" disabled={stops.length === 0} onClick={() => setStops([])}>
                Clear All
              </button>
            </div>
          </div>
          {!canApply && (
            <p className="form-item-styles-note">
              Select a Heading, Text, Fill in the Blank, or Multiple Choice item on a Form, then
              open Tabs again.
            </p>
          )}
        </div>
        <div className="modal-actions form-item-styles-actions">
          <button type="button" disabled={!canApply} onClick={() => onOk(stops)}>
            OK
          </button>
          <button type="button" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
