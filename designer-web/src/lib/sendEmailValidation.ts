import {
  defaultEmailErrorMessage,
  emailValidationMessage,
  isValidEmailLiteral,
} from "@/lib/emailValidation";
import { lookupFormFieldBlank } from "@/lib/projectModel";
import type { TawalaProcessCommand, TawalaProject } from "@/types/tawala";

export interface SendFieldValidationResult {
  valid: boolean;
  message?: string;
}

export interface SendFieldErrors {
  to?: string;
  cc?: string;
  fromAddress?: string;
  fromName?: string;
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

/**
 * From (Name) — display name / alias shown to recipients. Unlike Subject (which the Java
 * runtime stores as a chunked sequence of literal text + `<field>` elements, see
 * `Send.SUBJECT_FACTORY` / `<subject>` XML), the `<from>` alias is a single XML attribute
 * (`aliasField="…"` or `aliasLiteral="…"`) — it can hold ONE field/variable reference, or
 * plain literal text, but never a mix of the two or multiple fields concatenated together.
 *
 * A value that still contains `<<…>>` after trimming but isn't a single whole-string token
 * (e.g. `<<Form 1:FirstName>> <<Form 1:LastName>>`, or `Hi <<Form 1:FirstName>>`) cannot be
 * exported as a working alias: it silently becomes a literal display name containing the raw,
 * never-evaluated `<<…>>` text. That is exactly the "unresolved `<<Form 1:FirstName>>
 * <<Form 1:LastName>>`" value seen in process From aliases at email-queue time — the runtime
 * sanitizer in `Email.buildSafeReplyTo` (Java) strips it back out at send-time (so delivery
 * doesn't bounce), but the intended personalization never worked. Catch it here instead, at
 * authoring time, with guidance toward the supported pattern (Set a variable first, then
 * reference that single variable).
 */
export function validateSendFromName(value: string): SendFieldValidationResult {
  const trimmed = value.trim();
  if (!trimmed) return { valid: true };
  if (!trimmed.includes("<<")) return { valid: true };
  if (/^<<[^<>]+>>$/.test(trimmed)) return { valid: true };
  return {
    valid: false,
    message:
      "From display name can be plain text or a single field/variable, not multiple fields " +
      "combined — combine them into one variable with a Set command first (e.g. Set FromName " +
      "to <<Form 1:FirstName>> <<Form 1:LastName>>), then use <<FromName>> here.",
  };
}

export function getSendFieldErrors(
  state: {
    to: string;
    cc: string;
    fromAddress: string;
    fromName: string;
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
  const fromName = validateSendFromName(state.fromName);
  if (!fromName.valid && fromName.message) errors.fromName = fromName.message;
  return errors;
}

function addressValueToText(addr: unknown): string {
  if (addr == null) return "";
  if (typeof addr === "string") return addr;
  if (typeof addr === "object" && !Array.isArray(addr)) {
    const a = addr as { fieldRef?: string; literal?: string };
    if (a.fieldRef) return a.fieldRef;
    if (a.literal != null) return String(a.literal);
  }
  return "";
}

/**
 * True when a Send command should render in red in the process script list:
 * incomplete/invalid To or Cc for runtime (e.g. placeholder literal), or missing body document.
 * Does not block Modify/save — designer allows incomplete Send config.
 */
export function isSendCommandVisuallyInvalid(
  command: TawalaProcessCommand,
  project: TawalaProject,
  knownVariables: ReadonlySet<string>,
): boolean {
  const to = validateSendRecipientField(
    addressValueToText(command.to),
    project,
    knownVariables,
    true,
  );
  if (!to.valid) return true;
  const cc = validateSendRecipientField(
    addressValueToText(command.cc),
    project,
    knownVariables,
    false,
  );
  if (!cc.valid) return true;
  const body = command.body as { document?: string } | undefined;
  const doc = body?.document ?? (command.document != null ? String(command.document) : "");
  return !doc.trim();
}
