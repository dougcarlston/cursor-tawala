import { describe, expect, it } from "vitest";
import { validateFibBlanks } from "./fibBlankValidation.mjs";
import { handleFormSubmit } from "./runtime.mjs";
import { createSession } from "./sessionStore.mjs";

describe("validateFibBlanks", () => {
  const form = {
    name: "Form 1",
    items: [
      {
        type: "fib",
        label: "Q2",
        style: "topLabels",
        blanks: [
          {
            name: "Email",
            alternateLabel: "Email",
            validation: { type: "email", errorMessage: "Bad email." },
          },
          {
            name: "Tel",
            alternateLabel: "Tel",
            validation: { type: "phone", errorMessage: "Bad phone." },
          },
        ],
      },
    ],
  };

  it("rejects invalid email", () => {
    const err = validateFibBlanks(
      form,
      {
        formName: "Form 1",
        fields: { "Q2:Email": "not-an-email", Email: "not-an-email" },
        formFields: { "Form 1": { "Q2:Email": "not-an-email" } },
      },
      form.items,
    );
    expect(err).toBe("Bad email.");
  });

  it("rejects short phone", () => {
    const err = validateFibBlanks(
      form,
      {
        formName: "Form 1",
        fields: { "Q2:Email": "a@b.com", Email: "a@b.com", "Q2:Tel": "123", Tel: "123" },
        formFields: { "Form 1": { "Q2:Email": "a@b.com", "Q2:Tel": "123" } },
      },
      form.items,
    );
    expect(err).toBe("Bad phone.");
  });

  it("infers Email/Tel validators from alternateLabel when unset", () => {
    const signupForm = {
      name: "Form 1",
      items: [
        {
          type: "fib",
          label: "FIB1",
          blanks: [
            { name: "c", alternateLabel: "Email" },
            { name: "d", alternateLabel: "Tel" },
          ],
        },
      ],
    };
    const err = validateFibBlanks(
      signupForm,
      {
        formName: "Form 1",
        fields: {},
        formFields: { "Form 1": { "FIB1:c": "asdlfkj", "FIB1:d": ";kjf" } },
      },
      signupForm.items,
    );
    expect(err).toBe("Please enter a valid email address.");
  });

  it("rejects double-dot email", () => {
    const err = validateFibBlanks(
      form,
      {
        formName: "Form 1",
        fields: { "Q2:Email": "non@comcast..net", Email: "non@comcast..net" },
        formFields: { "Form 1": { "Q2:Email": "non@comcast..net" } },
      },
      form.items,
    );
    expect(err).toBe("Bad email.");
  });

  it("allows valid email + phone", () => {
    const err = validateFibBlanks(
      form,
      {
        formName: "Form 1",
        fields: {
          "Q2:Email": "a@b.com",
          Email: "a@b.com",
          "Q2:Tel": "555-123-4567",
          Tel: "555-123-4567",
        },
        formFields: { "Form 1": { "Q2:Email": "a@b.com", "Q2:Tel": "555-123-4567" } },
      },
      form.items,
    );
    expect(err).toBeNull();
  });

  it("handleFormSubmit blocks bad email before Document/MQL", () => {
    const project = {
      name: "Signup Sheet Template",
      forms: [
        {
          name: "Form 1",
          process: "Process 1",
          items: [
            {
              type: "fib",
              label: "Q2",
              style: "topLabels",
              prompt: "",
              blanks: [
                {
                  name: "Email",
                  alternateLabel: "Email",
                  displayLabel: "Email",
                  length: 40,
                  validation: { type: "email", errorMessage: "Please enter a valid email address." },
                },
              ],
            },
          ],
        },
      ],
      processes: [{ name: "Process 1", commands: [{ cmd: "show", document: "Document 1" }] }],
      documents: [{ name: "Document 1", content: "<p>Doc</p>" }],
    };
    const session = createSession(project);
    const html = handleFormSubmit(
      project,
      "Form 1",
      session,
      { "Q2:Email": "nope", segmentId: "0", submit: "Submit" },
      "http://localhost:5173",
      "uid-val",
    );
    expect(html).toContain("Please enter a valid email address.");
    expect(html).toContain("validation-error");
    expect(session.records["Form 1"]).toBeUndefined();
    expect(html).not.toContain('aria-label="Document 1"');
  });
});
