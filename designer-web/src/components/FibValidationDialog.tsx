import { useEffect, useRef, useState } from "react";
import type { BlankValidation } from "@/types/tawala";
import { validatorMeta } from "@/lib/fibBlanks";
import {
  ConfigureFunctionShell,
  type ConfigureFunctionHelp,
} from "./ConfigureFunctionShell";

interface Props {
  validation: BlankValidation;
  onCancel: () => void;
  onSave: (validation: BlankValidation) => void;
}

type FieldKey = "errorMessage" | "lowerLimit" | "upperLimit";

const FIELD_HELP: Record<FieldKey, ConfigureFunctionHelp> = {
  errorMessage: {
    title: "Error message",
    body: "Message shown to the respondent when the entry fails validation.",
    required: true,
  },
  lowerLimit: {
    title: "Lower limit",
    body: "Optional lower limit.",
    hint: "A compound expression",
  },
  upperLimit: {
    title: "Upper limit",
    body: "Optional upper limit.",
    hint: "A compound expression",
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
  const functionDescription = meta?.hasLimits
    ? "Checks for valid integer, optionally checking for the integer to be within limits."
    : "";

  return (
    <ConfigureFunctionShell
      titleId="fib-validation-title"
      onClose={onCancel}
      functionTitle={(meta?.label ?? "Validation").toUpperCase()}
      functionDescription={functionDescription}
      help={help}
      canOk={true}
      onOk={save}
      onCancel={onCancel}
    >
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
    </ConfigureFunctionShell>
  );
}
