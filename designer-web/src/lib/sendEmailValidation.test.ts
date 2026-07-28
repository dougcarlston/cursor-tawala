import { describe, expect, it } from "vitest";
import {
  getSendFieldErrors,
  isSendCommandVisuallyInvalid,
  validateSendFromName,
} from "./sendEmailValidation";
import { sendBuilderIsValid, type SendBuilderState } from "./statementBuilders";
import type { TawalaProject } from "@/types/tawala";

const project: TawalaProject = {
  name: "P",
  forms: [{ name: "Form 1", startPoint: true, items: [] }],
  processes: [],
  documents: [{ name: "NewSignup", items: [] }],
} as unknown as TawalaProject;

const documentNames = ["NewSignup"];

function baseSendState(overrides: Partial<SendBuilderState> = {}): SendBuilderState {
  return {
    to: "",
    cc: "",
    fromAddress: "",
    fromName: "",
    subject: "New Signup on Signup Sheet",
    document: "NewSignup",
    documentReset: false,
    showPageHeader: false,
    ...overrides,
  };
}

describe("validateSendFromName", () => {
  it("accepts empty, plain text, and a single qualified field token", () => {
    expect(validateSendFromName("")).toEqual({ valid: true });
    expect(validateSendFromName("Camp Registrar")).toEqual({ valid: true });
    expect(validateSendFromName("<<Form 1:AdminName>>")).toEqual({ valid: true });
    expect(validateSendFromName("<<AdminName>>")).toEqual({ valid: true });
  });

  it("rejects two field tokens combined — the owner's reported bug", () => {
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

describe("sendBuilderIsValid — placeholder To (legacy Modify)", () => {
  it("allows Modify when To is the insert-email placeholder", () => {
    const state = baseSendState({ to: "<Insert your email address here>" });
    expect(sendBuilderIsValid(state, documentNames, project, new Set())).toBe(true);
    expect(getSendFieldErrors(state, project, new Set()).to).toBeTruthy();
  });

  it("still requires To, Subject, and Document", () => {
    expect(
      sendBuilderIsValid(baseSendState({ to: "" }), documentNames, project, new Set()),
    ).toBe(false);
    expect(
      sendBuilderIsValid(
        baseSendState({ to: "a@b.com", subject: "" }),
        documentNames,
        project,
        new Set(),
      ),
    ).toBe(false);
  });

  it("still blocks Add/Modify when From (Name) mixes multiple field tokens", () => {
    const state = baseSendState({
      to: "<Insert your email address here>",
      fromName: "<<Form 1:FirstName>> <<Form 1:LastName>>",
    });
    expect(sendBuilderIsValid(state, documentNames, project, new Set())).toBe(false);
  });
});

describe("isSendCommandVisuallyInvalid", () => {
  it("marks placeholder To as visually invalid (red script line)", () => {
    expect(
      isSendCommandVisuallyInvalid(
        {
          cmd: "send",
          to: { literal: "<Insert your email address here>" },
          body: { document: "NewSignup" },
        },
        project,
        new Set(),
      ),
    ).toBe(true);
  });

  it("does not mark a valid email literal To as invalid", () => {
    expect(
      isSendCommandVisuallyInvalid(
        {
          cmd: "send",
          to: { literal: "owner@example.com" },
          body: { document: "NewSignup" },
        },
        project,
        new Set(),
      ),
    ).toBe(false);
  });
});
