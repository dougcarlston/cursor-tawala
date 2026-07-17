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
