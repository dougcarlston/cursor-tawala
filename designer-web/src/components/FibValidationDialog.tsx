import { useEffect, useRef, useState } from "react";
import type { BlankValidation } from "@/types/tawala";
import { validatorMeta } from "@/lib/fibBlanks";

interface Props {
  validation: BlankValidation;
  onCancel: () => void;
  onSave: (validation: BlankValidation) => void;
}

type FieldKey = "errorMessage" | "lowerLimit" | "upperLimit";

const FIELD_HELP: Record<FieldKey, { title: string; body: string }> = {
  errorMessage: {
    title: "Error message",
    body: "Message shown to the respondent when the entry fails validation.",
  },
  lowerLimit: {
    title: "Lower limit",
    body: "Optional lower limit.",
  },
  upperLimit: {
    title: "Upper limit",
    body: "Optional upper limit.",
  },
};

/**
 * Configure Function dialog for a FIB blank validator (legacy `ConfigureFunctionDialog`).
 * Every editable validator has an Error message; the Integer validator adds optional
 * Lower / Upper limits. The right-hand panel shows context help for the focused field.
 */
export function FibValidationDialog({ validation, onCancel, onSave }: Props) {
  const meta = validatorMeta(validation.type);
  const [errorMessage, setErrorMessage] = useState(validation.errorMessage ?? "");
  const [lowerLimit, setLowerLimit] = useState(validation.lowerLimit ?? "");
  const [upperLimit, setUpperLimit] = useState(validation.upperLimit ?? "");
  const [focused, setFocused] = useState<FieldKey>("errorMessage");
  const errorRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    const el = errorRef.current;
    if (!el) return;
    el.focus();
    el.select();
  }, []);

  const save = () => {
    onSave({
      type: validation.type,
      errorMessage: errorMessage.trim() ? errorMessage : undefined,
      lowerLimit: meta?.hasLimits && lowerLimit.trim() ? lowerLimit : undefined,
      upperLimit: meta?.hasLimits && upperLimit.trim() ? upperLimit : undefined,
    });
  };

  const help = FIELD_HELP[focused];

  return (
    <div className="modal-overlay" role="presentation">
      <div
        className="modal-dialog fib-validation-dialog"
        role="dialog"
        aria-labelledby="fib-validation-title"
        aria-modal="true"
      >
        <div className="modal-header">
          <h2 id="fib-validation-title">
            <span className="fib-validation-fx">fx</span> Configure Function
          </h2>
          <button type="button" className="modal-close" onClick={onCancel} aria-label="Close">
            ×
          </button>
        </div>
        <div className="fib-validation-body">
          <div className="fib-validation-fields">
            <label>
              <span>Error message:</span>
              <input
                ref={errorRef}
                type="text"
                value={errorMessage}
                onFocus={() => setFocused("errorMessage")}
                onChange={(e) => setErrorMessage(e.target.value)}
              />
            </label>
            {meta?.hasLimits && (
              <>
                <label>
                  <span>Lower limit:</span>
                  <input
                    type="text"
                    value={lowerLimit}
                    onFocus={() => setFocused("lowerLimit")}
                    onChange={(e) => setLowerLimit(e.target.value)}
                  />
                </label>
                <label>
                  <span>Upper limit:</span>
                  <input
                    type="text"
                    value={upperLimit}
                    onFocus={() => setFocused("upperLimit")}
                    onChange={(e) => setUpperLimit(e.target.value)}
                  />
                </label>
              </>
            )}
          </div>
          <aside className="fib-validation-help">
            <h3>{meta?.label ?? "Validation"}</h3>
            {meta?.hasLimits && (
              <p>Checks for valid integer, optionally checking for the integer to be within limits.</p>
            )}
            <h4>{help.title}</h4>
            <p>{help.body}</p>
            {(focused === "lowerLimit" || focused === "upperLimit") && (
              <p className="fib-validation-help-note">A compound expression</p>
            )}
          </aside>
        </div>
        <div className="modal-footer fib-validation-footer">
          <button type="button" onClick={save}>
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
