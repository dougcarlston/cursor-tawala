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

  it("leftAlign multi-blank soft-row becomes one paragraph per blank (Java table layout)", () => {
    const item = {
      type: "fib",
      label: "FIB1",
      style: "leftAlignLabels",
      prompt: "Name ________ Email ________ Phone ________",
      blanks: [
        { name: "a", alternateLabel: "Name", length: 8 },
        { name: "b", alternateLabel: "Email", length: 8 },
        { name: "c", alternateLabel: "Phone", length: 8 },
      ],
    };
    const xml = fibToXml(item, escAttr, escText);
    expect(xml.match(/<paragraph\b/g)?.length).toBe(3);
    expect(xml.match(/<blank\b/g)?.length).toBe(3);
    // Each paragraph has exactly one blank (no Name+Email collapsed into label cell).
    const paras = xml.match(/<paragraph[\s\S]*?<\/paragraph>/g) ?? [];
    for (const p of paras) {
      expect(p.match(/<blank\b/g)?.length).toBe(1);
    }
    expect(paras[0]).toContain("Name:");
    expect(paras[0]).toContain('alternateLabel="Name"');
    expect(paras[1]).toContain("Email:");
    expect(paras[2]).toContain("Phone:");
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
});
