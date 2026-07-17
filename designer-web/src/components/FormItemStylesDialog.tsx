import { useState } from "react";
import type { FibItem, McItem, TextItem } from "@/types/tawala";
import {
  applyFibStyle,
  applyMcStyle,
  applyTextStyle,
  parseFibStyle,
  parseMcStyle,
  parseTextStyle,
  type FibStyleDraft,
  type McStyleDraft,
  type StylesDialogKind,
  type TextStyleDraft,
} from "@/lib/formItemStyles";

interface Props {
  kind: StylesDialogKind;
  /** Selected items of this kind on the active form (may be empty — one item max in legacy). */
  selectedItems: Array<FibItem | McItem | TextItem>;
  /** Count of matching items on the active form (enables Apply to All when ≥ 1). */
  formItemCount: number;
  onCancel: () => void;
  onApplySelected: (patch: FibItem | McItem | TextItem) => void;
  onApplyAll: (draft: FibStyleDraft | McStyleDraft | TextStyleDraft) => void;
}

/** Legacy FibStylesDialog / McqItemStylesDialog / TextItemStylesDialog. */
export function FormItemStylesDialog({
  kind,
  selectedItems,
  formItemCount,
  onCancel,
  onApplySelected,
  onApplyAll,
}: Props) {
  const seed = selectedItems[0];

  const [fib, setFib] = useState<FibStyleDraft>(() =>
    parseFibStyle(seed?.type === "fib" ? seed.style : undefined),
  );
  const [mc, setMc] = useState<McStyleDraft>(() =>
    seed?.type === "mc" ? parseMcStyle(seed) : { layout: "vertical", columnCount: 0, noPaddingBottom: false },
  );
  const [text, setText] = useState<TextStyleDraft>(() =>
    seed?.type === "text"
      ? parseTextStyle(seed)
      : { style: "normal", noPaddingBottom: false },
  );

  const hasSelection = selectedItems.length > 0;
  const canApplySelected = hasSelection;
  const canApplyAll = formItemCount >= 1;

  const title = kind === "fib" ? "Fill in the Blank Styles" : "Styles";

  const note =
    kind === "fib"
      ? "Note: Apply to Selected updates the one selected Fill in the Blank on this form. Apply to All updates every Fill in the Blank on this form only (not the whole project)."
      : kind === "mc"
        ? "Note: Apply to Selected updates the one selected Multiple Choice on this form. Apply to All updates every Multiple Choice on this form only (not the whole project)."
        : "Note: Apply to Selected updates the one selected Text item on this form. Apply to All updates every Text item on this form only (not the whole project).";

  const currentDraft = (): FibStyleDraft | McStyleDraft | TextStyleDraft => {
    if (kind === "fib") return fib;
    if (kind === "mc") return mc;
    return text;
  };

  const applySelected = () => {
    if (!seed || !canApplySelected) return;
    if (kind === "fib" && seed.type === "fib") onApplySelected(applyFibStyle(seed, fib));
    else if (kind === "mc" && seed.type === "mc") onApplySelected(applyMcStyle(seed, mc));
    else if (kind === "text" && seed.type === "text") onApplySelected(applyTextStyle(seed, text));
  };

  const applyAll = () => {
    if (!canApplyAll) return;
    onApplyAll(currentDraft());
  };

  return (
    <div className="modal-overlay configure-function-overlay" role="presentation">
      <div
        className={`modal configure-function-dialog form-item-styles-dialog form-item-styles-${kind}`}
        role="dialog"
        aria-modal="true"
        aria-labelledby="form-item-styles-title"
      >
        <h2 id="form-item-styles-title">{title}</h2>
        <div className="configure-function-body form-item-styles-body">
          {kind === "fib" && (
            <div className="form-item-styles-fib-main">
              <div className="form-item-styles-fib-controls">
                <fieldset className="form-item-styles-fieldset">
                  <legend>Labels</legend>
                  {(
                    [
                      ["above", "Above"],
                      ["left", "Left justified"],
                      ["right", "Right justified"],
                      ["freeform", "Freeform"],
                    ] as const
                  ).map(([value, label]) => (
                    <label key={value} className="form-item-styles-radio">
                      <input
                        type="radio"
                        name="fib-placement"
                        checked={fib.placement === value}
                        onChange={() => setFib((d) => ({ ...d, placement: value }))}
                      />
                      {label}
                    </label>
                  ))}
                </fieldset>
                <fieldset className="form-item-styles-fieldset">
                  <legend>Blanks</legend>
                  <label
                    className={`form-item-styles-check${
                      fib.placement === "left" || fib.placement === "right" ? "" : " disabled"
                    }`}
                  >
                    <input
                      type="checkbox"
                      checked={fib.alignRightSide}
                      disabled={fib.placement !== "left" && fib.placement !== "right"}
                      onChange={(e) => setFib((d) => ({ ...d, alignRightSide: e.target.checked }))}
                    />
                    Align right side
                  </label>
                </fieldset>
              </div>
              <div className="form-item-styles-preview form-item-styles-preview-pane" aria-live="polite">
                <FibPreview draft={fib} />
              </div>
            </div>
          )}

          {kind === "mc" && (
            <>
              <div className="form-item-styles-stack" role="radiogroup" aria-label="Multiple Choice layout">
                {(
                  [
                    ["vertical", "Vertical"],
                    ["horizontal", "Horizontal"],
                    ["multicolumn", "Multi-column"],
                  ] as const
                ).map(([value, label]) => (
                  <div key={value} className="form-item-styles-stack-row">
                    <div className="form-item-styles-stack-radio">
                      <label className="form-item-styles-radio form-item-styles-radio-bold">
                        <input
                          type="radio"
                          name="mc-layout"
                          checked={mc.layout === value}
                          onChange={() => setMc((d) => ({ ...d, layout: value }))}
                        />
                        {label}
                      </label>
                      {value === "multicolumn" && (
                        <select
                          className="form-item-styles-columns-select"
                          aria-label="Column count"
                          value={mc.columnCount === 0 ? "Auto" : String(mc.columnCount)}
                          disabled={mc.layout !== "multicolumn"}
                          onChange={(e) => {
                            const v = e.target.value;
                            setMc((d) => ({
                              ...d,
                              columnCount: v === "Auto" ? 0 : Number(v),
                            }));
                          }}
                        >
                          {["Auto", "2", "3", "4", "5"].map((opt) => (
                            <option key={opt} value={opt}>
                              {opt}
                            </option>
                          ))}
                        </select>
                      )}
                    </div>
                    <div className="form-item-styles-preview-pane">
                      <McLayoutPreview layout={value} />
                    </div>
                  </div>
                ))}
              </div>
              <fieldset className="form-item-styles-fieldset form-item-styles-spacing">
                <legend>Spacing</legend>
                <label className="form-item-styles-check">
                  <input
                    type="checkbox"
                    checked={mc.noPaddingBottom}
                    onChange={(e) => setMc((d) => ({ ...d, noPaddingBottom: e.target.checked }))}
                  />
                  Do not add blank space below question when displayed.
                </label>
              </fieldset>
            </>
          )}

          {kind === "text" && (
            <>
              <div className="form-item-styles-stack" role="radiogroup" aria-label="Text style">
                {(
                  [
                    ["normal", "Normal"],
                    ["instructional", "Instructional"],
                    ["error", "Error"],
                  ] as const
                ).map(([value, label]) => (
                  <div key={value} className="form-item-styles-stack-row">
                    <div className="form-item-styles-stack-radio">
                      <label className="form-item-styles-radio form-item-styles-radio-bold">
                        <input
                          type="radio"
                          name="text-style"
                          checked={text.style === value}
                          onChange={() => setText((d) => ({ ...d, style: value }))}
                        />
                        {label}
                      </label>
                    </div>
                    <div className="form-item-styles-preview-pane">
                      <TextLayoutPreview style={value} />
                    </div>
                  </div>
                ))}
              </div>
              <fieldset className="form-item-styles-fieldset form-item-styles-spacing">
                <legend>Spacing</legend>
                <label className="form-item-styles-check">
                  <input
                    type="checkbox"
                    checked={text.noPaddingBottom}
                    onChange={(e) => setText((d) => ({ ...d, noPaddingBottom: e.target.checked }))}
                  />
                  Do not add blank space below text when displayed.
                </label>
              </fieldset>
            </>
          )}

          <p className="form-item-styles-note">{note}</p>
        </div>
        <div className="modal-actions form-item-styles-actions">
          {hasSelection ? (
            <button type="button" disabled={!canApplySelected} onClick={applySelected}>
              Apply to Selected
            </button>
          ) : null}
          <button
            type="button"
            disabled={!canApplyAll}
            title={
              canApplyAll
                ? `Apply this style to all ${formItemCount} matching item(s) on this form`
                : "No matching items on this form"
            }
            onClick={applyAll}
          >
            Apply to All
          </button>
          <button type="button" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

function FibBlank({ width }: { width: "sm" | "md" | "lg" }) {
  return <span className={`styles-preview-fib-blank styles-preview-fib-blank-${width}`} />;
}

function FibPreview({ draft }: { draft: FibStyleDraft }) {
  if (draft.placement === "above") {
    return (
      <div className="styles-preview-fib styles-preview-fib-above">
        <div className="styles-preview-fib-above-row">
          <span className="styles-preview-fib-label">Name:</span>
          <FibBlank width="sm" />
        </div>
        <div className="styles-preview-fib-above-row">
          <span className="styles-preview-fib-label">Address:</span>
          <FibBlank width="lg" />
        </div>
      </div>
    );
  }

  if (draft.placement === "freeform") {
    return (
      <div className="styles-preview-fib styles-preview-fib-freeform">
        Columbus had three ships called the <FibBlank width="sm" /> the{" "}
        <FibBlank width="sm" /> and the <FibBlank width="sm" />
      </div>
    );
  }

  const rows: Array<{ label: string; width: "sm" | "md" | "lg" }> = [
    { label: "Name:", width: "lg" },
    { label: "Address:", width: "lg" },
    { label: "Phone:", width: "md" },
  ];
  const sideClass =
    draft.placement === "right"
      ? draft.alignRightSide
        ? "styles-preview-fib-right-justified"
        : "styles-preview-fib-right"
      : draft.alignRightSide
        ? "styles-preview-fib-left-justified"
        : "styles-preview-fib-left";

  return (
    <div className={`styles-preview-fib ${sideClass}`}>
      {rows.map((row) => (
        <div key={row.label} className="styles-preview-fib-side-row">
          <span className="styles-preview-fib-label">{row.label}</span>
          <FibBlank width={draft.alignRightSide ? "lg" : row.width} />
        </div>
      ))}
    </div>
  );
}

function McLayoutPreview({
  layout,
}: {
  layout: McStyleDraft["layout"];
}) {
  const question = "What is your favorite color?";
  const three = ["Red", "Orange", "Yellow"] as const;
  const six = ["Red", "Orange", "Yellow", "Green", "Blue", "Violet"] as const;
  const choices = layout === "multicolumn" ? six : three;
  const cls =
    layout === "horizontal"
      ? "styles-preview-mc-horizontal"
      : layout === "multicolumn"
        ? "styles-preview-mc-multi"
        : "styles-preview-mc-vertical";

  return (
    <div className={`styles-preview-mc ${cls}`}>
      <div className="styles-preview-mc-q">{question}</div>
      <div className="styles-preview-mc-choices">
        {choices.map((c, i) => (
          <label key={c}>
            <input
              type="radio"
              tabIndex={-1}
              readOnly
              checked={layout === "vertical" && i === 0}
              onChange={() => {}}
            />{" "}
            {c}
          </label>
        ))}
      </div>
    </div>
  );
}

function TextLayoutPreview({ style }: { style: TextStyleDraft["style"] }) {
  const cls =
    style === "instructional"
      ? "styles-preview-text-instructional"
      : style === "error"
        ? "styles-preview-text-error"
        : "styles-preview-text-normal";
  const sample =
    style === "instructional"
      ? "This is instructional text"
      : style === "error"
        ? "This is error text"
        : "This is normal text";
  return <div className={`styles-preview-text ${cls}`}>{sample}</div>;
}
