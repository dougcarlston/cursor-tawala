/**
 * DirtBowl Registration layouts assume form name "Registration" + DirtBowl blank names.
 * Other projects also use form name "Registration" (CYO Dance Agreement) with different FIBs.
 */
import { describe, expect, it } from "vitest";
import {
  isDirtBowlRegistrationForm,
  renderRegistrationFib,
} from "./registrationLayout.mjs";
import { registrationFibToXml } from "./registrationFibToXml.mjs";
import { createSession } from "./sessionStore.mjs";
import { renderFormPage } from "./runtime.mjs";

const nonDirtBowlCtx = { fields: {}, formFields: {}, dirtBowlRegLayout: false };
const dirtBowlCtx = { fields: {}, formFields: {}, dirtBowlRegLayout: true };

function cyoLikeQ1() {
  return {
    type: "fib",
    label: "Q1",
    prompt: "Last Name ____________________ First Name ____________________ MI __",
    blanks: [
      { name: "PartLast", alternateLabel: "PartLast", length: 20 },
      { name: "PartFirst", alternateLabel: "PartFirst", length: 20 },
      { name: "PartMI", alternateLabel: "PartMI", length: 2 },
    ],
  };
}

function dirtBowlQ1() {
  return {
    type: "fib",
    label: "Q1",
    prompt: "Name of Registrant",
    blanks: [
      { name: "FirstName", alternateLabel: "FirstName", length: 15 },
      { name: "LastName", alternateLabel: "LastName", length: 15 },
      { name: "RegAgeMo", alternateLabel: "RegAgeMo", length: 2 },
      { name: "RegAgeDay", alternateLabel: "RegAgeDay", length: 2 },
      { name: "RegAgeYr", alternateLabel: "RegAgeYr", length: 4 },
    ],
  };
}

describe("registrationLayout DirtBowl gate", () => {
  it("detects DirtBowl Registration via Q1 blank names", () => {
    expect(
      isDirtBowlRegistrationForm({
        name: "Registration",
        items: [dirtBowlQ1()],
      }),
    ).toBe(true);
    expect(
      isDirtBowlRegistrationForm({
        name: "Registration",
        items: [
          {
            type: "fib",
            label: "Q1",
            blanks: [
              { name: "a", alternateLabel: "FirstName", length: 15 },
              { name: "b", alternateLabel: "LastName", length: 15 },
            ],
          },
        ],
      }),
    ).toBe(true);
    expect(
      isDirtBowlRegistrationForm({
        name: "Registration",
        items: [cyoLikeQ1()],
      }),
    ).toBe(false);
    expect(isDirtBowlRegistrationForm({ name: "Signup", items: [dirtBowlQ1()] })).toBe(false);
  });

  it("returns null for CYO-like Q1 instead of throwing on missing DOB blanks", () => {
    expect(() => renderRegistrationFib(cyoLikeQ1(), nonDirtBowlCtx, "Registration")).not.toThrow();
    expect(renderRegistrationFib(cyoLikeQ1(), nonDirtBowlCtx, "Registration")).toBeNull();
    // Even if form flag were wrongly true, blank-shape gate must still refuse CYO Q1.
    expect(() => renderRegistrationFib(cyoLikeQ1(), dirtBowlCtx, "Registration")).not.toThrow();
    expect(renderRegistrationFib(cyoLikeQ1(), dirtBowlCtx, "Registration")).toBeNull();
  });

  it("skips specialization when dirtBowlRegLayout is not true", () => {
    expect(renderRegistrationFib(dirtBowlQ1(), nonDirtBowlCtx, "Registration")).toBeNull();
    expect(renderRegistrationFib(dirtBowlQ1(), { fields: {} }, "Registration")).toBeNull();
  });

  it("still renders DirtBowl Q1 name + DOB layout", () => {
    const html = renderRegistrationFib(dirtBowlQ1(), dirtBowlCtx, "Registration");
    expect(html).toBeTruthy();
    expect(html).toContain("Name of Registrant");
    expect(html).toContain("Date of Birth");
    expect(html).toContain('name="Q1:FirstName"');
    expect(html).toContain('name="Q1:RegAgeYr"');
  });

  it("registrationFibToXml falls through for CYO-like Q1", () => {
    const esc = (s) => String(s ?? "");
    expect(registrationFibToXml(cyoLikeQ1(), esc, esc)).toBeNull();
    const dirtXml = registrationFibToXml(dirtBowlQ1(), esc, esc);
    expect(dirtXml).toContain("Name of Registrant");
    expect(dirtXml).toContain("FirstName");
  });

  it("Preview of CYO-like Registration does not crash and shows FIB inputs", () => {
    const project = {
      name: "CYO Dance Agreement",
      themePath: "default",
      forms: [
        {
          name: "Registration",
          items: [
            {
              type: "text",
              label: "T1",
              content: "Dance Permission and Participation Agreement",
            },
            cyoLikeQ1(),
            {
              type: "fib",
              label: "Q2",
              prompt: "Participant's Email: ____________________",
              blanks: [{ name: "StudentEmail", length: 20 }],
            },
          ],
        },
      ],
    };
    const html = renderFormPage(
      project,
      "Registration",
      "http://localhost:5173",
      "preview-cyo",
      createSession(project),
      { designerPreview: true },
    );
    expect(html).not.toContain("TypeError");
    expect(html).not.toContain("Cannot read properties of undefined");
    expect(html).toContain("Dance Permission and Participation Agreement");
    expect(html).toContain('name="Q1:PartLast"');
    expect(html).toContain('name="Q1:PartFirst"');
    expect(html).not.toContain("Name of Registrant:");
    expect(html).not.toContain("Summer Basketball League");
  });

  it("full CYO Dance Agreement JSON Preview path does not TypeError", async () => {
    const { readFileSync, existsSync } = await import("node:fs");
    const path =
      process.env.HOME +
      "/Projects/Tawala-JSON-Conversions/converted/CYO Dance Agreement.json";
    if (!existsSync(path)) return; // skip if conversion tree absent
    const project = JSON.parse(readFileSync(path, "utf8"));
    let html;
    expect(() => {
      html = renderFormPage(
        project,
        "Registration",
        "http://localhost:5173",
        "preview-cyo-full",
        createSession(project),
        { designerPreview: true },
      );
    }).not.toThrow();
    expect(html).not.toMatch(/TypeError|Cannot read properties of undefined/);
    expect(html).toMatch(/PartLast|PartFirst/);
    expect(html).not.toContain("Name of Registrant:");
  });
});
