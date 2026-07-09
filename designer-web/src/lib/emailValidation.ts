import { validatorMeta } from "@/lib/fibBlanks";
import type { BlankValidation } from "@/types/tawala";

/** Mirrors `Tawala.validation.emailValidation` in legacy `default.js`. */
const EMAIL_LITERAL_RE = /^([A-Za-z0-9_\-\.])+\@([A-Za-z0-9_\-\.])+\.([A-Za-z]{2,4})$/;

export function defaultEmailErrorMessage(): string {
  return validatorMeta("email")?.defaultMessage ?? "Please enter a valid email address.";
}

/** FIB Email validator message — custom `errorMessage` when set, else default. */
export function emailValidationMessage(validation?: BlankValidation): string {
  if (validation?.type === "email" && validation.errorMessage?.trim()) {
    return validation.errorMessage.trim();
  }
  return defaultEmailErrorMessage();
}

export function isValidEmailLiteral(value: string): boolean {
  const trimmed = value.trim();
  if (!trimmed) return false;
  return EMAIL_LITERAL_RE.test(trimmed) && !trimmed.includes("..");
}
