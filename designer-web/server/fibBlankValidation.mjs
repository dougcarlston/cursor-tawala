/**
 * Node Preview/Deploy FIB blank validators (parity with Java blank-validator types).
 * Blanks with `blank.validation.type` are checked; Email/Tel-like names also get
 * default email/phone checks when no validator is configured (Signup Sheet).
 * Empty optional fields skip.
 */

function isValidEmail(email) {
  if (!email) return false;
  if ((email.match(/@/g) ?? []).length !== 1) return false;
  if (/\.\./.test(email) || email.includes(" ")) return false;
  const [local, domain] = email.split("@");
  if (!local || !domain || !domain.includes(".")) return false;
  return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email);
}

function isValidPhone(phone) {
  const digits = String(phone).replace(/\D/g, "");
  return digits.length >= 10 && digits.length <= 15;
}

function isValidInteger(value, validation) {
  if (!/^-?\d+$/.test(String(value).trim())) return false;
  const n = Number(value);
  const lower = validation.lowerLimit?.trim();
  const upper = validation.upperLimit?.trim();
  if (lower !== undefined && lower !== "" && n < Number(lower)) return false;
  if (upper !== undefined && upper !== "" && n > Number(upper)) return false;
  return true;
}

function isValidUsState(value) {
  return /^[A-Za-z]{2}$/.test(String(value).trim());
}

function isValidZip(value) {
  return /^\d{5}(-\d{4})?$/.test(String(value).trim());
}

function isValidDollar(value) {
  return /^-?\$?\d{1,3}(,\d{3})*(\.\d{2})?$|^-?\$?\d+(\.\d{2})?$/.test(String(value).trim());
}

function blankPostedValue(ctx, formName, item, blank) {
  const fname = `${item.label}:${blank.name}`;
  const alt = blank.alternateLabel || blank.name;
  return (
    ctx.fields?.[`${formName}:${alt}`] ??
    ctx.fields?.[fname] ??
    ctx.fields?.[alt] ??
    ctx.fields?.[blank.name] ??
    ctx.formFields?.[formName]?.[fname] ??
    ctx.formFields?.[formName]?.[alt] ??
    ctx.formFields?.[formName]?.[blank.name] ??
    ""
  );
}

/** Infer email/phone when FIB Properties left validation unset but labels say Email/Tel. */
export function inferredBlankValidation(blank) {
  if (blank?.validation?.type) return blank.validation;
  const labels = [blank?.name, blank?.alternateLabel, blank?.displayLabel]
    .map((s) => String(s ?? "").trim().toLowerCase())
    .filter(Boolean);
  if (labels.some((l) => l === "email" || /(^|[^a-z])email([^a-z]|$)/.test(l))) {
    return { type: "email", errorMessage: "Please enter a valid email address." };
  }
  if (labels.some((l) => /^(tel|phone|telephone)$/.test(l) || /(^|[^a-z])phone([^a-z]|$)/.test(l))) {
    return {
      type: "phone",
      errorMessage: "Please enter a valid phone number including area code.",
    };
  }
  return null;
}

function checkOne(value, validation) {
  const type = validation?.type;
  if (!type) return null;
  const msg = validation.errorMessage?.trim() || "Please correct this field.";
  switch (type) {
    case "email":
      return isValidEmail(value) ? null : msg || "Please enter a valid email address.";
    case "phone":
      return isValidPhone(value) ? null : msg || "Please enter a valid phone number including area code.";
    case "integer":
      return isValidInteger(value, validation) ? null : msg || "This number is not valid.";
    case "usState":
      return isValidUsState(value) ? null : msg || "Please enter a valid two-character state abbreviation.";
    case "zip":
      return isValidZip(value) ? null : msg || "Please enter a valid ZIP code.";
    case "dollar":
      return isValidDollar(value) ? null : msg || "Please enter a valid dollar amount.";
    case "properName":
      return null;
    default:
      return null;
  }
}

/** First FIB validation error message for the current form segment items, or null. */
export function validateFibBlanks(form, ctx, items) {
  const formName = ctx.formName || form?.name || "";
  for (const item of items ?? []) {
    if (item.type !== "fib") continue;
    for (const blank of item.blanks ?? []) {
      const validation = inferredBlankValidation(blank);
      if (!validation) continue;
      const raw = blankPostedValue(ctx, formName, item, blank);
      const value = String(raw ?? "").trim();
      if (!value) continue;
      const err = checkOne(value, validation);
      if (err) return err;
    }
  }
  return null;
}
