/**
 * MCQ choice labels with <<Variable>> must expand (or show escaped placeholders)
 * in Preview HTML — never raw `<<` (browsers show `<>`) or empty wipe.
 */
import { describe, expect, it } from "vitest";
import { createSession } from "./sessionStore.mjs";
import { expandChoiceLabelHtml, getFieldValue } from "./runtimeEngine.mjs";
import { renderFormPage } from "./runtime.mjs";
import { mcToXml } from "./mcToXml.mjs";

function escAttr(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}
function escText(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}

function playerFormProject() {
  return {
    name: "PlayerForm",
    themePath: "default",
    forms: [
      {
        name: "Form 1",
        themePath: "default",
        items: [
          {
            type: "mc",
            label: "MCQ1",
            question: "Married?",
            onlyone: true,
            choices: [
              { label: "a", text: "Yes (or in a long term relationship)" },
              { label: "b", text: "Married to <<CoachName>>" },
              { label: "c", text: "Separated" },
            ],
          },
        ],
      },
    ],
    processes: [{ name: "Init", commands: [{ cmd: "set", field: "CoachName", value: "" }] }],
  };
}

describe("getFieldValue resolves project Variables", () => {
  it("reads bare CoachName from session.fields (Variables store)", () => {
    const ctx = {
      fields: { CoachName: "Jordan" },
      formFields: { "Form 1": { Email: "a@b.com" } },
      formName: "Form 1",
    };
    expect(getFieldValue(ctx, "CoachName")).toBe("Jordan");
    expect(getFieldValue(ctx, "MissingVar")).toBe("");
  });
});

describe("expandChoiceLabelHtml", () => {
  it("keeps escaped <<CoachName>> when unset (not raw <> / empty)", () => {
    const html = expandChoiceLabelHtml("Married to <<CoachName>>", {
      fields: {},
      formName: "Form 1",
    });
    expect(html).toBe("Married to &lt;&lt;CoachName&gt;&gt;");
    expect(html).not.toMatch(/<<CoachName>>/);
    expect(html).not.toContain("Married to <>");
  });

  it("substitutes escaped variable value when set", () => {
    const html = expandChoiceLabelHtml("Married to <<CoachName>>", {
      fields: { CoachName: "Alex <Jr>" },
      formName: "Form 1",
    });
    expect(html).toBe("Married to Alex &lt;Jr&gt;");
    expect(html).not.toContain("<<");
  });
});

describe("Preview MCQ choice <<Variable>>", () => {
  it("renders escaped placeholder when CoachName unset", () => {
    const project = playerFormProject();
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "preview-mc-vars",
      createSession(project),
      { designerPreview: true },
    );
    expect(html).toContain("Married to &lt;&lt;CoachName&gt;&gt;");
    expect(html).not.toMatch(/Married to <<CoachName>>/);
    expect(html).not.toContain("Married to <>");
  });

  it("renders CoachName value when set on the session", () => {
    const project = playerFormProject();
    const session = createSession(project);
    session.fields.CoachName = "Pat";
    const html = renderFormPage(
      project,
      "Form 1",
      "http://localhost:5173",
      "preview-mc-vars-set",
      session,
      { designerPreview: true },
    );
    expect(html).toContain("Married to Pat");
    expect(html).not.toContain("&lt;&lt;CoachName&gt;&gt;");
    expect(html).not.toContain("Married to <>");
  });
});

describe("mcToXml field tokens in choice text", () => {
  it("emits <field name=\"CoachName\"/> for <<CoachName>> in static choice text", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ1",
        onlyone: true,
        question: "Married?",
        choices: [
          { label: "a", text: "Yes" },
          { label: "b", text: "Married to <<CoachName>>" },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain('<field name="CoachName"/>');
    expect(xml).not.toContain("&lt;&lt;CoachName&gt;&gt;");
    expect(xml).toContain("Married to");
  });
});
