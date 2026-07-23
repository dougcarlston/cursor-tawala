import { describe, expect, it } from "vitest";
import { fibToXml } from "./fibToXml.mjs";

const escAttr = (s) =>
  String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
const escText = escAttr;

describe("fibToXml WYSIWYG rows", () => {
  it("keeps two underscore blanks on one Design soft-row as one Deploy paragraph", () => {
    const item = {
      type: "fib",
      label: "FIB1",
      prompt:
        "First Name _______________ Last Name _______________<br>" +
        "Email ___________________ Phone __________________",
      blanks: [
        { name: "a", alternateLabel: "First", length: 15 },
        { name: "b", alternateLabel: "Last", length: 15 },
        { name: "c", alternateLabel: "Email", length: 19, required: true },
        { name: "d", alternateLabel: "Tel", length: 18 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    const paragraphs = xml.match(/<paragraph\b/g) ?? [];
    expect(paragraphs.length).toBe(2);
    // First paragraph has both First and Last blanks
    const firstPara = xml.match(/<paragraph[\s\S]*?<\/paragraph>/)?.[0] ?? "";
    expect(firstPara).toContain('alternateLabel="First"');
    expect(firstPara).toContain('alternateLabel="Last"');
    expect(firstPara).not.toMatch(/<\/paragraph>[\s\S]*alternateLabel="Last"/);
  });

  it("mirrors Design B/I/U into Deploy font XML", () => {
    const item = {
      type: "fib",
      label: "FIB1",
      prompt: "<b>First</b> ________ <i>Last</i> ________ <u>Note</u> ____",
      blanks: [
        { name: "a", alternateLabel: "First", length: 8 },
        { name: "b", alternateLabel: "Last", length: 8 },
        { name: "c", alternateLabel: "Note", length: 4 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    expect(xml).toMatch(/<font[^>]*><b>First<\/b><\/font>/);
    expect(xml).toMatch(/<font[^>]*><i>Last<\/i><\/font>/);
    expect(xml).toMatch(/<font[^>]*><u>Note<\/u><\/font>/);
    expect(xml.match(/<blank\b/g)?.length).toBe(3);
    expect(xml.match(/<paragraph\b/g)?.length).toBe(1);
  });

  it("mirrors Design span color and font-size into Deploy font attrs", () => {
    const item = {
      type: "fib",
      label: "FIB1",
      prompt: '<span style="color:#FF0000;font-size:14pt;font-family:Georgia">Label</span> ________',
      blanks: [{ name: "a", alternateLabel: "Field", length: 8 }],
    };
    const xml = fibToXml(item, escAttr, escText);
    expect(xml).toContain('face="Georgia"');
    expect(xml).toContain('size="280"'); // 14pt × 20
    expect(xml).toContain('color="FF0000"');
    expect(xml).toContain(">Label</font>");
  });

  it("leftAlign keeps multi-blank soft-row as one paragraph (remainder on same line)", () => {
    const item = {
      type: "fib",
      label: "FIB1",
      style: "leftAlignLabels",
      prompt: "Email ________ Email (again) ________",
      blanks: [
        { name: "a", alternateLabel: "Email", length: 8 },
        { name: "b", alternateLabel: "EmailAgain", length: 8 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    const paras = xml.match(/<paragraph[\s\S]*?<\/paragraph>/g) ?? [];
    expect(paras.length).toBe(1);
    expect(paras[0].match(/<blank\b/g)?.length).toBe(2);
    expect(paras[0]).toMatch(/Email/);
    expect(paras[0]).toMatch(/Email \(again\)|Email \(again\)/);
  });

  it("rightAlignJustified keeps First Name + Last Name on one soft-row as one paragraph", () => {
    const item = {
      type: "fib",
      label: "FIB1",
      style: "rightAlignLabelsJustified",
      prompt: "First Name ________ Last Name ________",
      blanks: [
        { name: "a", alternateLabel: "First", length: 8 },
        { name: "b", alternateLabel: "Last", length: 8 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    const paras = xml.match(/<paragraph[\s\S]*?<\/paragraph>/g) ?? [];
    expect(paras.length).toBe(1);
    expect(paras[0].match(/<blank\b/g)?.length).toBe(2);
  });

  it("rightAlignJustified keeps Full Name + (first)/(last) as one paragraph (not split)", () => {
    const item = {
      type: "fib",
      label: "FIB3",
      style: "rightAlignLabelsJustified",
      prompt: "Full Name: ____________________ (first) ____________________ (last)",
      blanks: [
        { name: "a", alternateLabel: "firstName", length: 20 },
        { name: "b", alternateLabel: "lastName", length: 20 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    const paras = xml.match(/<paragraph[\s\S]*?<\/paragraph>/g) ?? [];
    expect(paras.length).toBe(1);
    expect(paras[0].match(/<blank\b/g)?.length).toBe(2);
    expect(paras[0]).toMatch(/Full Name:/);
    expect(paras[0]).toMatch(/\(first\)/);
    expect(paras[0]).toMatch(/\(last\)/);
    // Must not promote (first) to its own label-only first paragraph.
    expect(paras[0].indexOf("Full Name:")).toBeLessThan(paras[0].indexOf("<blank"));
  });

  it("leftAlign does not auto-bold Name/Email/Phone labels", () => {
    const item = {
      type: "fib",
      label: "FIB2",
      style: "leftAlignLabels",
      prompt: "Email ________",
      blanks: [{ name: "a", alternateLabel: "Email", length: 12 }],
    };
    const xml = fibToXml(item, escAttr, escText);
    expect(xml).toContain("Email:");
    expect(xml).not.toMatch(/<b>Email:/);
  });

  it("freeform does not auto-bold Name/Email labels", () => {
    const item = {
      type: "fib",
      label: "FIB3",
      prompt: "Name ________ Email ________",
      blanks: [
        { name: "a", alternateLabel: "Name", length: 8 },
        { name: "b", alternateLabel: "Email", length: 8 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    expect(xml).not.toMatch(/<b>Name/);
    expect(xml).not.toMatch(/<b>Email/);
  });

  it("emits <field/> for <<ContactType>> labels (Signup Set→FIB), not Your >:", () => {
    const item = {
      type: "fib",
      label: "Q3",
      prompt:
        "Your <<ContactType1>>:_________________________________________Your <<ContactType2>>:_________________________________________",
      blanks: [
        { name: "a", length: 41 },
        { name: "b", length: 41 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    expect(xml).toContain('<field name="ContactType1"/>');
    expect(xml).toContain('<field name="ContactType2"/>');
    expect(xml).not.toMatch(/Your\s*&gt;:/);
    expect(xml).not.toMatch(/Your\s*>:/);
  });

  it("keeps blank with Name:____ after intro soft-row (not input then Name: below)", () => {
    const item = {
      type: "fib",
      label: "Q1",
      style: "topLabels",
      prompt:
        'Add your name to the list:<div><br><div><span>Name:__________________________________________</span></div></div>',
      blanks: [{ name: "a", length: 42, alternateLabel: "Name" }],
    };
    const xml = fibToXml(item, escAttr, escText);
    const paras = [...xml.matchAll(/<paragraph[\s\S]*?<\/paragraph>/g)].map((m) => m[0]);
    expect(paras.length).toBeGreaterThanOrEqual(2);
    expect(paras[0]).toMatch(/Add your name/);
    expect(paras[0]).not.toMatch(/<blank\b/);
    expect(paras[1]).toMatch(/Name:/);
    expect(paras[1]).toMatch(/<blank\b/);
  });
});
