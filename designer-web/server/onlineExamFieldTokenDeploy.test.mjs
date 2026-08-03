/**
 * Full-project Deploy audit: Online Exam Builder must not emit field tokens
 * as plain-text `<string value="<<…>>"/>` (empty MQL / wrong joins).
 *
 * Converter contract: XML `<string field="X"/>` ↔ JSON `"value": "<<X>>"` ↔
 * Deploy `<string field="X"/>`. See conditionOperandXml.mjs + OPEN_BUGS R-
 * field-as-text.
 */
import { readFileSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";
import { projectToXml } from "./jsonToXml.mjs";
import { isFieldTokenEmittedAsValueLiteral } from "./conditionOperandXml.mjs";
import { mcToXml } from "./mcToXml.mjs";

const ROOT = resolve(dirname(fileURLToPath(import.meta.url)), "../..");
const EXAM_JSON = resolve(
  ROOT,
  "website-mock/projects/library/Online Exam Builder.json",
);

const escAttr = (s) =>
  String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;");

describe("Online Exam Builder: field tokens never Deploy as value literals", () => {
  it("projectToXml has no string value=\"<<…>>\" (or entity form)", () => {
    const project = JSON.parse(readFileSync(EXAM_JSON, "utf8"));
    const xml = projectToXml(project);
    expect(isFieldTokenEmittedAsValueLiteral(xml)).toBe(false);
    // Canonical end-of-exam MQL Where (Post-test details table)
    expect(xml).toContain(
      '<equals field="Record:Answer:SessionId"><string field="Exam:id"/></equals>',
    );
    expect(xml).toContain(
      '<equals field="Record:Question:QuestionId"><string field="Record:Answer:QuestionId"/></equals>',
    );
  });

  it("dynamic MC record-selector uses the same field-token rule", () => {
    const xml = mcToXml(
      {
        type: "mc",
        label: "Q",
        choiceSource: "stored",
        onlyone: true,
        question: "Pick",
        choices: [
          {
            type: "dynamic",
            sourceForm: "Answer",
            displayExpr: "<<Record:Answer:Answer>>",
            valueExpr: "<<Record:Answer:Answer>>",
            conditionsRows: [
              {
                field: "Record:Answer:SessionId",
                op: "equals",
                value: "<<Exam:id>>",
              },
            ],
            conditionsCombinator: "and",
          },
        ],
      },
      escAttr,
      (s) => String(s ?? ""),
    );
    expect(xml).toContain(
      '<equals field="Record:Answer:SessionId"><string field="Exam:id"/></equals>',
    );
    expect(xml).not.toContain('string value="<<Exam:id>>"');
  });
});
