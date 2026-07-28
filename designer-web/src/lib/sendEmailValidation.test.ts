import { describe, expect, it } from "vitest";
import { getSendFieldErrors, validateSendFromName } from "./sendEmailValidation";
import type { TawalaProject } from "@/types/tawala";

const project: TawalaProject = {
  name: "P",
  forms: [{ name: "Form 1", startPoint: true, items: [] }],
  processes: [],
  documents: [],
} as unknown as TawalaProject;

describe("validateSendFromName", () => {
  it("accepts empty, plain text, and a single qualified field token", () => {
    expect(validateSendFromName("")).toEqual({ valid: true });
    expect(validateSendFromName("Camp Registrar")).toEqual({ valid: true });
    expect(validateSendFromName("<<Form 1:AdminName>>")).toEqual({ valid: true });
    expect(validateSendFromName("<<AdminName>>")).toEqual({ valid: true });
  });

  it("rejects two field tokens combined — the owner's reported bug", () => {
    // Matches the literal example from Email.java's buildSafeReplyTo comment: this used to
    // silently export as aliasLiteral containing raw, never-evaluated "<<…>>" text, which
    // then showed up unresolved in the process From alias at email-queue time.
    const result = validateSendFromName("<<Form 1:FirstName>> <<Form 1:LastName>>");
    expect(result.valid).toBe(false);
    expect(result.message).toMatch(/Set command/);
  });

  it("rejects a field token mixed with literal text", () => {
    const result = validateSendFromName("Hi <<Form 1:FirstName>>");
    expect(result.valid).toBe(false);
  });
});

describe("getSendFieldErrors fromName wiring", () => {
  it("surfaces fromName error alongside the other Send fields", () => {
    const errors = getSendFieldErrors(
      {
        to: "a@b.com",
        cc: "",
        fromAddress: "",
        fromName: "<<Form 1:FirstName>> <<Form 1:LastName>>",
      },
      project,
      new Set<string>(),
    );
    expect(errors.fromName).toBeTruthy();
    expect(errors.to).toBeUndefined();
  });

  it("leaves fromName unset when the value is a single field or plain text", () => {
    const errors = getSendFieldErrors(
      { to: "a@b.com", cc: "", fromAddress: "", fromName: "<<FromName>>" },
      project,
      new Set<string>(["FromName"]),
    );
    expect(errors.fromName).toBeUndefined();
  });
});
