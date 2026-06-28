import { getFieldValue } from "./runtimeEngine.mjs";

const KNOWN_TLD = /\.(com|io|net|org|edu|gov|us|uk|ca|info|biz|me|co)$/i;

function fieldValue(ctx, itemLabel, blankName) {
  return String(
    getFieldValue(ctx, `Registration:${itemLabel}:${blankName}`) ||
      getFieldValue(ctx, `${itemLabel}:${blankName}`) ||
      "",
  ).trim();
}

function registrationEmail(ctx, blankName) {
  return fieldValue(ctx, "Q4", blankName);
}

function isValidEmailFormat(email) {
  if (!email) return false;
  if ((email.match(/@/g) ?? []).length !== 1) return false;
  const domain = email.split("@")[1] ?? "";
  return domain.includes(".") && KNOWN_TLD.test(domain);
}

export function validateRegistrationDob(ctx) {
  const mo = fieldValue(ctx, "Q1", "RegAgeMo");
  const day = fieldValue(ctx, "Q1", "RegAgeDay");
  const yr = fieldValue(ctx, "Q1", "RegAgeYr");

  if (!mo || !day || !yr) {
    return "Date of birth is incomplete.";
  }
  if (!/^\d+$/.test(mo) || !/^\d+$/.test(day) || !/^\d+$/.test(yr)) {
    return "Date of birth must be numeric (mm/dd/yyyy).";
  }

  const month = Number(mo);
  const dayNum = Number(day);
  const year = Number(yr);
  const currentYear = new Date().getFullYear();

  if (month < 1 || month > 12) {
    return "Birth month must be between 1 and 12.";
  }
  if (dayNum < 1 || dayNum > 31) {
    return "Birth day must be between 1 and 31.";
  }
  if (year > currentYear) {
    return "Birth year cannot be in the future.";
  }
  if (year < 1900) {
    return "Birth year cannot be before 1900.";
  }

  return null;
}

export function validateRegistrationEmails(ctx) {
  const parentEmail = registrationEmail(ctx, "ParentEmail");
  const confirmEmail = registrationEmail(ctx, "g");

  if (!parentEmail || parentEmail !== confirmEmail) {
    return "Email address is blank or two copies of email fail to match.";
  }

  if (!isValidEmailFormat(parentEmail)) {
    return "Parent email address is not valid.";
  }

  const parentEmail2 = registrationEmail(ctx, "ParentEmail2");
  if (parentEmail2) {
    const confirm2 = registrationEmail(ctx, "ParentEmail2Confirm");
    if (parentEmail2 !== confirm2) {
      return "Two copies of additional email fail to match.";
    }
    if (!isValidEmailFormat(parentEmail2)) {
      return "Additional parent email address is not valid.";
    }
  }

  return null;
}

/** Page 1 submit checks — DOB, email match, and email format. */
export function validateRegistrationPage1(ctx) {
  return validateRegistrationDob(ctx) || validateRegistrationEmails(ctx);
}
