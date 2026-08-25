import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";
import { renderDocumentsPage } from "./documentRenderer.mjs";
import { buildContext, runProcessByName } from "./runtimeEngine.mjs";

const __dirname = dirname(fileURLToPath(import.meta.url));
const LIVING_WILL_JSON = join(
  __dirname,
  "../../../ Current Tawala Projects/Website Staging/1-Review/Under Construction/Living Will v1.0.json",
);

/** Final Process 1 If — bare State must read Q2:State even when ctx.fields.State is "". */
const STATE_IF = {
  cmd: "if",
  condition: {
    or: [
      { field: "State", op: "contains", value: "CA" },
      { field: "State", op: "contains", value: "Ca" },
    ],
  },
  then: [{ cmd: "showDocument", document: "Living Will", reset: false }],
  else: [{ cmd: "showDocument", document: "NoGo", reset: false }],
};

function loadLivingWillProject() {
  return JSON.parse(readFileSync(LIVING_WILL_JSON, "utf8"));
}

function livingWillSession(overrides = {}) {
  return {
    fields: {
      State: "",
      City: "Los Angeles",
      County: "Los Angeles",
      FirstName: "Jane",
      MiddleName: "Q",
      LastName: "Public",
      Exceptions: "",
      Donation: "",
      Purposes: "",
      ...overrides.fields,
    },
    formFields: {
      LivingWill: {
        "Q2:State": "California CA",
        "Q2:City": "Los Angeles",
        "Q2:County": "Los Angeles",
        "Q1:FirstName": "Jane",
        "Q1:MiddleName": "Q",
        "Q1:LastName": "Public",
        "Q5": "a",
        "Q6:a": "",
        "Q7": "a",
        "Q8:a": "Dr. Smith",
        "Q8:b": "123 Main St",
        "Q9": "a",
        "Q10": "a",
        "Q12": "a,b",
        "Q13:a": "15",
        "Q13:b": "August",
        "Q13:c": "2026",
        ...overrides.formFields?.LivingWill,
      },
    },
    records: {},
    ...overrides,
  };
}

describe("Living Will Process 1 State If", () => {
  it("shows Living Will when State is in formFields but ctx.fields.State is empty", () => {
    const session = {
      fields: { State: "", City: "", County: "" },
      formFields: {
        LivingWill: {
          "Q2:State": "California CA",
          "Q2:City": "Los Angeles",
          "Q2:County": "Los Angeles",
        },
      },
    };
    const ctx = buildContext(session, "LivingWill");
    const nav = runProcessByName(
      { processes: [{ name: "Process 1", commands: [STATE_IF] }] },
      "Process 1",
      ctx,
    );
    expect(nav.type).toBe("documents");
    expect(nav.documents).toEqual(["Living Will"]);
  });

  it("shows NoGo when State truly has no CA", () => {
    const session = {
      fields: { State: "" },
      formFields: { LivingWill: { "Q2:State": "New York NY" } },
    };
    const ctx = buildContext(session, "LivingWill");
    const nav = runProcessByName(
      { processes: [{ name: "Process 1", commands: [STATE_IF] }] },
      "Process 1",
      ctx,
    );
    expect(nav.documents).toEqual(["NoGo"]);
  });
});

describe("Living Will Process 1 append chain", () => {
  it("records appendages on Living Will for a full CA submission", () => {
    const project = loadLivingWillProject();
    const session = livingWillSession();
    const ctx = buildContext(session, "LivingWill");
    const nav = runProcessByName(project, "Process 1", ctx);

    expect(nav.type).toBe("documents");
    expect(nav.documents).toEqual(["Living Will"]);
    expect(nav.virtualDocs?.["Living Will"]?.appendages).toEqual([
      "LifeEndNo",
      "LWPtTwo",
      "Physician",
      "OrganDonation",
    ]);
  });

  it("uses LifeEndYes when Q5 is b", () => {
    const project = loadLivingWillProject();
    const session = livingWillSession({
      formFields: { LivingWill: { Q5: "b" } },
    });
    const ctx = buildContext(session, "LivingWill");
    const nav = runProcessByName(project, "Process 1", ctx);

    expect(nav.virtualDocs?.["Living Will"]?.appendages?.[0]).toBe("LifeEndYes");
  });

  it("renders merged appendage HTML in the document page", () => {
    const project = loadLivingWillProject();
    const session = livingWillSession();
    const ctx = buildContext(session, "LivingWill");
    const nav = runProcessByName(project, "Process 1", ctx);
    Object.assign(session.fields, ctx.fields);

    const html = renderDocumentsPage(
      project,
      nav.documents,
      session,
      "http://localhost:3001",
      "lw-test",
      { fromForm: "LivingWill", virtualDocs: nav.virtualDocs },
    );

    expect(html).toContain("RELIEF FROM PAIN");
    expect(html).toContain("I do not want my life to be prolonged");
    expect(html).toContain("CHOICE OF PHYSICIAN");
    expect(html).toContain("Dr. Smith");
    expect(html).toContain("DONATION OF ORGANS AT DEATH");
    expect(html).toContain("any needed organs, tissues, or parts");
    expect(html).not.toContain("I'm sorry, but this document is only valid");
  });
});
