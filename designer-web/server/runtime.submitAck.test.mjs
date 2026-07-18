import { describe, expect, it } from "vitest";
import { appendFormRecord, clearFormAnswers, createSession } from "./sessionStore.mjs";
import { handleFormSubmit, renderFormPage, renderSubmitAck } from "./runtime.mjs";

describe("renderSubmitAck copy", () => {
  it("uses generic thank-you wording for Signup Sheet Form 1", () => {
    const project = { name: "Signup Sheet Template", themePath: "baseball", forms: [] };
    const form = { name: "Form 1", themePath: "baseball" };
    const html = renderSubmitAck(project, form, createSession(project), "http://localhost:5173", "uid1");
    expect(html).toContain("Thank you");
    expect(html).toContain("Your response has been recorded.");
    expect(html).toContain("← Back to Form 1");
    expect(html).toContain("?fresh=1");
    expect(html).not.toContain("Registration step complete");
    expect(html).not.toContain("Back to registration");
  });

  it("keeps registration wording for Registration form", () => {
    const project = { name: "DirtBowl", themePath: "dirtbowl2", forms: [] };
    const form = { name: "Registration", themePath: "dirtbowl2" };
    const html = renderSubmitAck(project, form, createSession(project), "http://localhost:5173", "uid2");
    expect(html).toContain("Registration step complete");
    expect(html).toContain("← Back to registration");
  });
});

describe("signup clear + append", () => {
  it("clearFormAnswers empties answers but keeps records", () => {
    const session = createSession({ name: "Signup Sheet Template" });
    session.formFields["Form 1"] = { "Q1:FirstName": "Ada", FirstName: "Ada" };
    session.fields.FirstName = "Ada";
    session.fields["Form 1:FirstName"] = "Ada";
    session.records["Form 1"] = [{ FirstName: "Ada" }];

    clearFormAnswers(session, "Form 1");

    expect(session.formFields["Form 1"]).toEqual({});
    expect(session.fields.FirstName).toBeUndefined();
    expect(session.records["Form 1"]).toEqual([{ FirstName: "Ada" }]);
  });

  it("clearFormAnswers also clears alternateLabel aliases (First ↔ a)", () => {
    const form = {
      name: "Form 1",
      items: [
        {
          type: "fib",
          label: "FIB1",
          blanks: [
            { name: "a", alternateLabel: "First" },
            { name: "b", alternateLabel: "Last" },
          ],
        },
      ],
    };
    const session = createSession({ name: "Signup Sheet Template", forms: [form] });
    session.formFields["Form 1"] = { "FIB1:a": "SmokeTest", "FIB1:b": "Alias" };
    session.fields.a = "SmokeTest";
    session.fields.First = "SmokeTest";
    session.fields["Form 1:First"] = "SmokeTest";
    session.fields.Last = "Alias";

    clearFormAnswers(session, "Form 1", form);

    expect(session.formFields["Form 1"]).toEqual({});
    expect(session.fields.First).toBeUndefined();
    expect(session.fields["Form 1:First"]).toBeUndefined();
    expect(session.fields.Last).toBeUndefined();
    expect(session.fields.a).toBeUndefined();
  });

  it("Show Document + Show Form stacks blank Form (no leftover SmokeTest)", () => {
    const form = {
      name: "Form 1",
      themePath: "baseball",
      process: "Process 1",
      items: [
        {
          type: "fib",
          label: "FIB1",
          style: "topLabels",
          prompt: "",
          blanks: [
            { name: "a", alternateLabel: "First", displayLabel: "First Name", length: 20 },
            { name: "b", alternateLabel: "Last", displayLabel: "Last Name", length: 20 },
          ],
        },
      ],
    };
    const project = {
      name: "Signup Sheet Template",
      themePath: "baseball",
      forms: [form],
      processes: [
        {
          name: "Process 1",
          commands: [
            { cmd: "show", document: "Document 1" },
            { cmd: "show", form: "Form 1" },
          ],
        },
      ],
      documents: [{ name: "Document 1", content: "<p>Doc</p>" }],
    };
    const session = createSession(project);
    const html = handleFormSubmit(
      project,
      "Form 1",
      session,
      { "FIB1:a": "SmokeTest", "FIB1:b": "Alias", segmentId: "0", submit: "Submit" },
      "http://localhost:5173",
      "uid-clear",
    );
    expect(html).toContain("doc-following-form");
    expect(html).toContain('name="FIB1:a"');
    // Embedded Form inputs must be empty after Show Form.
    expect(html).toMatch(/name="FIB1:a"[^>]*value=""/);
    expect(html).toMatch(/name="FIB1:b"[^>]*value=""/);
    expect(html).not.toMatch(/name="FIB1:a"[^>]*value="SmokeTest"/);
    expect(session.fields.First).toBeUndefined();
    expect(session.records["Form 1"][0].First).toBe("SmokeTest");
  });

  it("appendFormRecord snapshots blank aliases for itemization", () => {
    const session = createSession({ name: "Signup Sheet Template" });
    session.formFields["Form 1"] = {
      "Q1:FirstName": "Ada",
      "Q1:LastName": "Lovelace",
    };
    appendFormRecord(session, "Form 1");
    expect(session.records["Form 1"]).toHaveLength(1);
    expect(session.records["Form 1"][0].FirstName).toBe("Ada");
    expect(session.records["Form 1"][0].LastName).toBe("Lovelace");
  });

  it("appendFormRecord skips all-empty posts", () => {
    const session = createSession({ name: "Signup Sheet Template" });
    session.formFields["Form 1"] = { "Q1:FirstName": "", "Q2:Email": "  " };
    appendFormRecord(session, "Form 1");
    expect(session.records["Form 1"]).toBeUndefined();
  });

  it("handleFormSubmit records response, clears fields, and thanks for Signup Sheet", () => {
    const project = {
      name: "Signup Sheet Template",
      themePath: "baseball",
      forms: [
        {
          name: "Form 1",
          themePath: "baseball",
          items: [
            {
              type: "fib",
              label: "Q1",
              style: "topLabels",
              prompt: "",
              blanks: [{ name: "FirstName", alternateLabel: "FirstName", length: 20 }],
            },
          ],
        },
      ],
      processes: [],
    };
    const session = createSession(project);
    const html = handleFormSubmit(
      project,
      "Form 1",
      session,
      { "Q1:FirstName": "Ada", segmentId: "0", submit: "Submit" },
      "http://localhost:5173",
      "uid3",
    );
    expect(html).toContain("Thank you");
    expect(html).not.toContain("Registration step complete");
    expect(session.records["Form 1"]).toHaveLength(1);
    expect(session.records["Form 1"][0].FirstName).toBe("Ada");
    expect(session.formFields["Form 1"]).toEqual({});
    expect(session.fields.FirstName).toBeUndefined();
  });

  it("clears form answers before Show Document and keeps records for MQL", () => {
    const mqlConfig = JSON.stringify({
      numberOfColumns: 1,
      column: [{ header: "First", contents: "FirstName" }],
      "form-name": "Form 1",
      conditionsRows: [{ field: "FirstName", op: "equals", value: "Ada" }],
    });
    const project = {
      name: "Signup Sheet Template",
      themePath: "baseball",
      forms: [
        {
          name: "Form 1",
          themePath: "baseball",
          process: "Process 1",
          items: [
            {
              type: "fib",
              label: "Q1",
              style: "topLabels",
              prompt: "",
              blanks: [{ name: "FirstName", alternateLabel: "FirstName", length: 20 }],
            },
          ],
        },
      ],
      processes: [
        {
          name: "Process 1",
          commands: [
            { cmd: "show", document: "Document 1" },
            { cmd: "show", form: "Form 1" },
          ],
        },
      ],
      documents: [
        {
          name: "Document 1",
          content:
            `<p>List</p><span class="function-token" data-function-id="itemization-table" ` +
            `data-function-config='${mqlConfig}'>` +
            `<<MULTIPLE QUESTION LIST(1, FirstName, FirstName equals "Ada")>>` +
            `</span>`,
        },
      ],
    };
    const session = createSession(project);
    const html = handleFormSubmit(
      project,
      "Form 1",
      session,
      { "Q1:FirstName": "Ada", segmentId: "0", submit: "Submit" },
      "http://localhost:5173",
      "uid-doc",
    );

    expect(session.records["Form 1"]).toHaveLength(1);
    expect(session.records["Form 1"][0].FirstName).toBe("Ada");
    expect(session.formFields["Form 1"]).toEqual({});
    expect(session.fields.FirstName).toBeUndefined();

    expect(html).toContain('aria-label="Document 1"');
    expect(html).toContain("<table");
    expect(html).toContain("Ada");
    expect(html).not.toContain('equals "Ada")>>');
    // Document + Show Form: stack Form under Document (no injected Continue →).
    expect(html).not.toContain("Continue →");
    expect(html).toContain("doc-following-form");
    expect(html).toContain('class="tawala-form"');
    expect(html).toContain('name="Q1:FirstName"');
    expect(html).not.toContain("readonly");
    expect(html).toContain("tawala-theme-default");
    expect(html).toContain("font-family");
  });

  it("topLabels Preview inputs are editable (not readonly)", () => {
    const project = {
      name: "Signup Sheet Template",
      themePath: "baseball",
      forms: [
        {
          name: "Form 1",
          themePath: "baseball",
          items: [
            {
              type: "fib",
              label: "Q1",
              style: "topLabels",
              prompt: "",
              blanks: [
                { name: "FirstName", alternateLabel: "FirstName", displayLabel: "First name", length: 20 },
              ],
            },
          ],
        },
      ],
    };
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "uid-edit",
      createSession(project),
    );
    expect(html).toContain('name="Q1:FirstName"');
    expect(html).not.toContain("readonly");
  });

  it("leftAlign Preview keeps interstitial text between blanks (Batch 3)", () => {
    const project = {
      name: "Hold List",
      themePath: "default",
      forms: [
        {
          name: "Form 1",
          themePath: "default",
          items: [
            {
              type: "fib",
              label: "FIB1",
              style: "leftAlignLabels",
              prompt: "Name ________ Email ________",
              blanks: [
                { name: "a", length: 8 },
                { name: "b", length: 8 },
              ],
            },
          ],
        },
      ],
    };
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "uid-left",
      createSession(project),
    );
    expect(html).toContain("fib-label");
    expect(html).toContain("Name");
    // Email must appear between the two inputs, not dumped after both.
    const fieldsStart = html.indexOf('<span class="fib-fields">');
    expect(fieldsStart).toBeGreaterThan(-1);
    const after = html.slice(fieldsStart);
    const emailAt = after.indexOf("Email");
    const inputs = [...after.matchAll(/<input\b/g)].map((m) => m.index ?? -1);
    expect(emailAt).toBeGreaterThan(-1);
    expect(inputs.length).toBeGreaterThanOrEqual(2);
    expect(emailAt).toBeGreaterThan(inputs[0]);
    expect(emailAt).toBeLessThan(inputs[1]);
    expect(html).not.toContain("&nbsp;");
    expect(html).not.toMatch(/fib-inline-text[^>]*>[^<]*_/);
  });
});

describe("form item spacing CSS in runtime page", () => {
  it("includes Form Item spacing rules in submit ack shell", () => {
    const project = { name: "Signup Sheet Template", forms: [] };
    const form = { name: "Form 1" };
    const html = renderSubmitAck(project, form, createSession(project), "http://localhost:5173", "uid4");
    expect(html).toContain(".tawala-form > .fib");
    expect(html).toContain("margin-bottom: 0.85rem");
  });

  it("baseball theme falls back to default body class with readable base CSS", () => {
    const project = {
      name: "Signup Sheet Template",
      themePath: "baseball",
      forms: [{ name: "Form 1", themePath: "baseball", items: [] }],
    };
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "uid5",
      createSession(project),
    );
    expect(html).toContain('class="tawala-theme-default"');
    expect(html).not.toContain("tawala-theme-baseball");
    expect(html).toContain("font-family");
    expect(html).toContain("input[type=submit]");
  });
});
