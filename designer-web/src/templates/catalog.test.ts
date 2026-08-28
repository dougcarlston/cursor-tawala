import { describe, expect, it } from "vitest";
import fs from "fs";
import path from "path";
import { PROJECT_TEMPLATES, TEMPLATE_CATEGORIES, templatesByCategory } from "./catalog";
import { projectToXml } from "../../server/jsonToXml.mjs";

describe("New Project Template Catalog (B6)", () => {
  it("defines all expected template categories in order", () => {
    expect(TEMPLATE_CATEGORIES).toEqual([
      "Basic",
      "Activities",
      "Meetings and Gatherings",
      "Polls and Surveys",
    ]);
  });

  it("organizes templates by category without orphans", () => {
    const map = templatesByCategory();
    expect([...map.keys()]).toEqual([...TEMPLATE_CATEGORIES]);
    let total = 0;
    for (const [, list] of map) {
      total += list.length;
    }
    expect(total).toBe(PROJECT_TEMPLATES.length);
  });

  it("contains all 9 canonical templates", () => {
    const ids = PROJECT_TEMPLATES.map((t) => t.id);
    expect(ids).toEqual([
      "empty",
      "form-with-process",
      "form-process-document",
      "signup-sheet",
      "signup-sheet-w-email",
      "get-together",
      "potluck",
      "simple-survey",
      "multiple-question-survey",
    ]);
  });

  const templatesDir = path.resolve(__dirname, "../../public/samples/templates");

  for (const template of PROJECT_TEMPLATES) {
    describe(`Template: ${template.label} (${template.samplePath})`, () => {
      const filePath = path.join(templatesDir, template.samplePath);

      it("file exists on disk", () => {
        expect(fs.existsSync(filePath)).toBe(true);
      });

      it("parses as valid project JSON with valid structure", () => {
        const raw = fs.readFileSync(filePath, "utf8");
        const json = JSON.parse(raw);
        expect(json).toBeTypeOf("object");
        expect(json.name).toBeTypeOf("string");
        expect(json.name.length).toBeGreaterThan(0);
        expect(Array.isArray(json.forms)).toBe(true);
        expect(Array.isArray(json.processes)).toBe(true);
        expect(Array.isArray(json.documents)).toBe(true);
      });

      it("exports cleanly to legacy XML via projectToXml", () => {
        const raw = fs.readFileSync(filePath, "utf8");
        const json = JSON.parse(raw);
        const xml = projectToXml(json);
        expect(xml).toBeTypeOf("string");
        expect(xml.length).toBeGreaterThan(0);
        expect(xml).toContain("<project");
        expect(xml).toContain("</project>");
      });
    });
  }
});

describe("Root Sample JSON files (B6)", () => {
  const samplesDir = path.resolve(__dirname, "../../public/samples");
  const sampleFiles = ["fib-caption-smoke.json", "signup-sheets.json", "sum-smoke-test.json"];

  for (const filename of sampleFiles) {
    describe(`Sample: ${filename}`, () => {
      const filePath = path.join(samplesDir, filename);

      it("file exists and is valid project JSON", () => {
        expect(fs.existsSync(filePath)).toBe(true);
        const raw = fs.readFileSync(filePath, "utf8");
        const json = JSON.parse(raw);
        expect(json.name).toBeTypeOf("string");
      });

      it("exports cleanly to legacy XML", () => {
        const raw = fs.readFileSync(filePath, "utf8");
        const json = JSON.parse(raw);
        const xml = projectToXml(json);
        expect(xml).toContain("<project");
        expect(xml).toContain("</project>");
      });
    });
  }
});
