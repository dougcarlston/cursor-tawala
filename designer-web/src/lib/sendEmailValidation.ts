import {
  defaultEmailErrorMessage,
  emailValidationMessage,
  isValidEmailLiteral,
} from "@/lib/emailValidation";
import { lookupFormFieldBlank } from "@/lib/projectModel";
import type { TawalaProject } from "@/types/tawala";

export interface SendFieldValidationResult {
  valid: boolean;
  message?: string;
}

export interface SendFieldErrors {
  to?: string;
  cc?: string;
  fromAddress?: string;
}

type BareAddressKind = "empty" | "formField" | "variable" | "literal";

function classifyBareAddressValue(
  text: string,
  knownVariables: ReadonlySet<string>,
): BareAddressKind {
  const trimmed = text.trim();
  if (!trimmed) return "empty";
  if (trimmed.includes(":")) return "formField";
  if (knownVariables.has(trimmed)) return "variable";
  return "literal";
}

/**
 * To / Cc — must be empty (Cc only), a variable, a Form:Field with Email FIB validation,
 * or a typed address that passes email format validation.
 */
export function validateSendRecipientField(
  value: string,
  project: TawalaProject,
  knownVariables: ReadonlySet<string>,
  required: boolean,
): SendFieldValidationResult {
  const trimmed = value.trim();
  if (!trimmed) {
    return required ? { valid: false, message: defaultEmailErrorMessage() } : { valid: true };
  }

  const kind = classifyBareAddressValue(trimmed, knownVariables);
  if (kind === "variable") return { valid: true };

  if (kind === "formField") {
    const blank = lookupFormFieldBlank(project, trimmed);
    if (blank?.validation?.type === "email") {
      return { valid: true };
    }
    const message = blank?.validation
      ? emailValidationMessage(blank.validation)
      : defaultEmailErrorMessage();
    return { valid: false, message };
  }

  if (kind === "literal") {
    if (isValidEmailLiteral(trimmed)) return { valid: true };
    return { valid: false, message: defaultEmailErrorMessage() };
  }

  return { valid: false, message: defaultEmailErrorMessage() };
}

/**
 * From (Address) — optional. Field, variable, or expression combinations are accepted;
 * typed literals must pass email format validation (FIB Email default / custom message).
 */
export function validateSendFromAddress(
  value: string,
  _project: TawalaProject,
  knownVariables: ReadonlySet<string>,
): SendFieldValidationResult {
  const trimmed = value.trim();
  if (!trimmed) return { valid: true };
  if (trimmed.includes("<<")) return { valid: true };

  const kind = classifyBareAddressValue(trimmed, knownVariables);
  if (kind === "formField" || kind === "variable") return { valid: true };

  if (kind === "literal") {
    if (isValidEmailLiteral(trimmed)) return { valid: true };
    return { valid: false, message: defaultEmailErrorMessage() };
  }

  return { valid: true };
}

export function getSendFieldErrors(
  state: {
    to: string;
    cc: string;
    fromAddress: string;
  },
  project: TawalaProject,
  knownVariables: ReadonlySet<string>,
): SendFieldErrors {
  const errors: SendFieldErrors = {};
  const to = validateSendRecipientField(state.to, project, knownVariables, true);
  if (!to.valid && to.message) errors.to = to.message;
  const cc = validateSendRecipientField(state.cc, project, knownVariables, false);
  if (!cc.valid && cc.message) errors.cc = cc.message;
  const from = validateSendFromAddress(state.fromAddress, project, knownVariables);
  if (!from.valid && from.message) errors.fromAddress = from.message;
  return errors;
}
