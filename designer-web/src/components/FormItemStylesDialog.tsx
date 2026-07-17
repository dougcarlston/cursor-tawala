import { useMemo, useState } from "react";
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
  /** Selected items of this kind on the active form (may be empty). */
  selectedItems: Array<FibItem | McItem | TextItem>;
  onCancel: () => void;
  onApply: (patch: FibItem | McItem | TextItem) => void;
}

/** Legacy FibStylesDialog / McqItemStylesDialog / TextItemStylesDialog. */
export function FormItemStylesDialog({ kind, selectedItems, onCancel, onApply }: Props) {
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
  const canApply = useMemo(() => {
    if (!hasSelection) return false;
    if (kind === "fib") return true;
    if (kind === "mc") return true;
    return true;
  }, [hasSelection, kind]);

  const title =
    kind === "fib" ? "Fill in the Blank Styles" : kind === "mc" ? "Styles" : "Styles";

  const footerKind =
    kind === "fib" ? "Fill in the Blank" : kind === "mc" ? "Multiple Choice" : "Text";

  const apply = () => {
    if (!seed || !canApply) return;
    if (kind === "fib" && seed.type === "fib") onApply(applyFibStyle(seed, fib));
    else if (kind === "mc" && seed.type === "mc") onApply(applyMcStyle(seed, mc));
    else if (kind === "text" && seed.type === "text") onApply(applyTextStyle(seed, text));
  };

  return (
    <div className="modal-overlay configure-function-overlay" role="presentation">
      <div
        className="modal configure-function-dialog form-item-styles-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="form-item-styles-title"
      >
        <h2 id="form-item-styles-title">{title}</h2>
        <div className="configure-function-body form-item-styles-body">
          <div className="form-item-styles-preview" aria-live="polite">
            {kind === "fib" && <FibPreview draft={fib} />}
            {kind === "mc" && <McPreview draft={mc} />}
            {kind === "text" && <TextPreview draft={text} />}
          </div>

          {kind === "fib" && (
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
                    onChange={() =>
                      setFib((d) => ({
                        ...d,
                        placement: value,
                        alignRightSide:
                          value === "left" || value === "right" ? d.alignRightSide : false,
                      }))
                    }
                  />
                  {label}
                </label>
              ))}
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
          )}

          {kind === "mc" && (
            <>
              <fieldset className="form-item-styles-fieldset">
                <legend>Layout</legend>
                {(
                  [
                    ["vertical", "Vertical"],
                    ["horizontal", "Horizontal"],
                    ["multicolumn", "Multi-column"],
                  ] as const
                ).map(([value, label]) => (
                  <label key={value} className="form-item-styles-radio">
                    <input
                      type="radio"
                      name="mc-layout"
                      checked={mc.layout === value}
                      onChange={() => setMc((d) => ({ ...d, layout: value }))}
                    />
                    {label}
                  </label>
                ))}
                <label
                  className={`form-item-styles-columns${
                    mc.layout === "multicolumn" ? "" : " disabled"
                  }`}
                >
                  Columns
                  <select
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
                </label>
              </fieldset>
              <label className="form-item-styles-check">
                <input
                  type="checkbox"
                  checked={mc.noPaddingBottom}
                  onChange={(e) => setMc((d) => ({ ...d, noPaddingBottom: e.target.checked }))}
                />
                Do not add blank space below question when displayed.
              </label>
            </>
          )}

          {kind === "text" && (
            <>
              <fieldset className="form-item-styles-fieldset">
                <legend>Style</legend>
                {(
                  [
                    ["normal", "Normal"],
                    ["instructional", "Instructional"],
                    ["error", "Error"],
                  ] as const
                ).map(([value, label]) => (
                  <label key={value} className="form-item-styles-radio">
                    <input
                      type="radio"
                      name="text-style"
                      checked={text.style === value}
                      onChange={() => setText((d) => ({ ...d, style: value }))}
                    />
                    {label}
                  </label>
                ))}
              </fieldset>
              <label className="form-item-styles-check">
                <input
                  type="checkbox"
                  checked={text.noPaddingBottom}
                  onChange={(e) => setText((d) => ({ ...d, noPaddingBottom: e.target.checked }))}
                />
                Do not add blank space below text when displayed.
              </label>
            </>
          )}

          <p className="form-item-styles-note">
            Style may be applied only to selected {footerKind} questions in the active form, if
            any. The &quot;Apply to All&quot; feature has been disabled.
          </p>
        </div>
        <div className="modal-actions form-item-styles-actions">
          <button type="button" disabled title="Disabled — inadvertent use broke large projects">
            Apply to All
          </button>
          <button type="button" disabled={!canApply} onClick={apply}>
            Apply to Selected
          </button>
          <button type="button" onClick={onCancel}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}

function FibPreview({ draft }: { draft: FibStyleDraft }) {
  const label = "[Name]";
  const blank = "________";
  if (draft.placement === "above") {
    return (
      <div className="styles-preview-fib styles-preview-fib-above">
        <div>{label}</div>
        <div>{blank}</div>
      </div>
    );
  }
  if (draft.placement === "freeform") {
    return (
      <div className="styles-preview-fib">
        Your name: {blank}
      </div>
    );
  }
  const justify = draft.alignRightSide ? "flex-end" : "flex-start";
  return (
    <div className="styles-preview-fib styles-preview-fib-side" style={{ justifyContent: justify }}>
      <span className="styles-preview-fib-label">{label}</span>
      <span>{blank}</span>
    </div>
  );
}

function McPreview({ draft }: { draft: McStyleDraft }) {
  const choices = ["Red", "Blue", "Green"];
  const cls =
    draft.layout === "horizontal"
      ? "styles-preview-mc-horizontal"
      : draft.layout === "multicolumn"
        ? "styles-preview-mc-multi"
        : "styles-preview-mc-vertical";
  return (
    <div className={`styles-preview-mc ${cls}`}>
      <div className="styles-preview-mc-q">What is your favorite color?</div>
      <div className="styles-preview-mc-choices">
        {choices.map((c) => (
          <label key={c}>
            <input type="checkbox" readOnly tabIndex={-1} /> {c}
          </label>
        ))}
      </div>
    </div>
  );
}

function TextPreview({ draft }: { draft: TextStyleDraft }) {
  const cls =
    draft.style === "instructional"
      ? "styles-preview-text-instructional"
      : draft.style === "error"
        ? "styles-preview-text-error"
        : "styles-preview-text-normal";
  const sample =
    draft.style === "instructional"
      ? "This is instructional text"
      : draft.style === "error"
        ? "This is error text"
        : "This is normal text";
  return <div className={`styles-preview-text ${cls}`}>{sample}</div>;
}
