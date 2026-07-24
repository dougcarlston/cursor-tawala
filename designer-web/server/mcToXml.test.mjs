import { describe, expect, it } from "vitest";
import { mcToXml } from "./mcToXml.mjs";

const escAttr = (s) =>
  String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
const escText = escAttr;

describe("mcToXml style export", () => {
  it("exports horizontal style when Styles dialog set it", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ3",
        style: "horizontal",
        onlyone: true,
        question: "How do you prefer to hear from us?",
        choices: [
          { label: "a", text: "Email" },
          { label: "b", text: "Text" },
          { label: "c", text: "Phone call" },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain('style="horizontal"');
  });

  it("does not let displayAs=radio override vertical style", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ2",
        style: "vertical",
        displayAs: "radio",
        onlyone: true,
        question: "Favorite pet",
        choices: [{ label: "a", text: "Dog" }],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain('style="vertical"');
    expect(xml).not.toContain('style="horizontal"');
  });

  it("exports multicolumn + columnCount", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ1",
        style: "multicolumn",
        columnCount: 2,
        onlyone: true,
        question: "Color",
        choices: [
          { label: "a", text: "Blue" },
          { label: "b", text: "Orange" },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain('style="multicolumn"');
    expect(xml).toContain('columnCount="2"');
  });

  it("legacy displayAs=radio becomes horizontal when style absent", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "Q2",
        displayAs: "radio",
        onlyone: true,
        question: "Sex",
        choices: [
          { label: "a", text: "Boy" },
          { label: "b", text: "Girl" },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain('style="horizontal"');
  });
});

describe("mcToXml dynamic MCQ", () => {
  it("exports data-provider with form, display, value, sort", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ1",
        choiceSource: "stored",
        onlyone: true,
        question: "Pick a sheet",
        choices: [
          {
            type: "dynamic",
            sourceForm: "Sheet",
            displayExpr: "<<Record:Sheet:SheetName>>",
            valueExpr: "<<Record:Sheet:SheetName>>",
            sortExpr: "<<Record:Sheet:Count>>",
          },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain("<data-provider><dynamic-mcq");
    expect(xml).toContain('<form name="Sheet"');
    expect(xml).toContain('<display-expression><field name="Record:Sheet:SheetName"/>');
    expect(xml).toContain('<value-expression><field name="Record:Sheet:SheetName"/>');
    expect(xml).toContain('<sort-expression><field name="Record:Sheet:Count"/>');
    expect(xml).toContain('style="multicolumn"');
  });

  it("exports Configure Function condition rows under record-selector", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ2",
        choiceSource: "stored",
        onlyone: true,
        question: "Divisions",
        choices: [
          {
            type: "dynamic",
            sourceForm: "Division",
            displayExpr: "<<Record:Division:Name>>",
            valueExpr: "<<Record:Division:Id>>",
            conditionsRows: [
              { field: "Record:Division:Active", op: "equals", value: "yes" },
              { field: "Record:Division:Count", op: "isLessThan", value: "20" },
            ],
            conditionsCombinator: "and",
          },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain('<form name="Division"');
    expect(xml).toContain("<conditions>");
    expect(xml).toContain('<equals field="Record:Division:Active"><string value="yes"/></equals>');
    expect(xml).toContain('<isLessThan field="Record:Division:Count"><string value="20"/></isLessThan>');
    expect(xml).toContain("<and>");
  });

  it("normalizes Form:Field palette refs to Record:Form:Field on Deploy", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ1",
        choiceSource: "stored",
        onlyone: true,
        question: "Pick a team",
        choices: [
          {
            type: "dynamic",
            sourceForm: "Form 1",
            displayExpr: "<<Form 1:ChoiceName>>",
            valueExpr: "Form 1:ChoiceName",
            sortExpr: "ChoiceName",
          },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain('<field name="Record:Form 1:ChoiceName"/>');
    expect(xml).not.toContain('<field name="Form 1:ChoiceName"/>');
    expect(xml).not.toContain('<field name="ChoiceName"/>');
  });
});

describe("mcToXml question rich text", () => {
  it("exports Design B/I/U markup into font/b/i/u (not stripped plain text)", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "MCQ1",
        onlyone: true,
        question: `<b>Where is Greenland?</b> <i>(choose one)</i>`,
        choices: [
          { label: "a", text: "Europe" },
          { label: "b", text: "North America" },
        ],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain("<question>");
    expect(xml).toContain("<b>Where is Greenland?</b>");
    expect(xml).toContain("<i>(choose one)</i>");
    expect(xml).not.toMatch(/<question>[^<]*Where is Greenland/);
  });

  it("maps span font-weight:bold to <b> (palette styleWithCSS path)", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "Q1",
        onlyone: true,
        question: `<span style="font-weight: bold;">Age group</span>`,
        choices: [{ label: "a", text: "Under 18" }],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain("<b>Age group</b>");
    expect(xml).toContain("<font");
  });

  it("keeps plain-string questions bold (legacy JSON without HTML)", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "Q2",
        onlyone: true,
        question: "Favorite color",
        choices: [{ label: "a", text: "Blue" }],
      },
      escAttr,
      escText,
    );
    expect(xml).toContain("<b>Favorite color</b>");
  });

  it("keeps trailing (note) italic heuristic for plain questions", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "Q3",
        onlyone: true,
        question: "Pick a day (optional)",
        choices: [{ label: "a", text: "Mon" }],
      },
      escAttr,
      escText,
    );
    // Legacy plain path: main run normal, trailing (note) italic — not whole-string bold.
    expect(xml).toContain("Pick a day");
    expect(xml).toContain("<i>(optional)</i>");
  });
});
