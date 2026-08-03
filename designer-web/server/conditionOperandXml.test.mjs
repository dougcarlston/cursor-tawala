/**
 * Condition RHS: `<<Field>>` → `<string field>`; else `<string value>` (literal).
 * shared by Document MQL, Dynamic MC record-selector, and deploy audits.
 */
import { describe, expect, it } from "vitest";
import {
  conditionOperandXml,
  isFieldTokenEmittedAsValueLiteral,
} from "./conditionOperandXml.mjs";

const esc = (s) =>
  String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;");

describe("conditionOperandXml (field token vs literal)", () => {
  it("emits string field for pure <<Form:Field>> tokens", () => {
    expect(conditionOperandXml("<<Exam:id>>", esc)).toBe('<string field="Exam:id"/>');
    expect(conditionOperandXml("<<Record:Answer:QuestionId>>", esc)).toBe(
      '<string field="Record:Answer:QuestionId"/>',
    );
  });

  it("accepts entity-encoded tokens", () => {
    expect(conditionOperandXml("&lt;&lt;Exam:id&gt;&gt;", esc)).toBe(
      '<string field="Exam:id"/>',
    );
  });

  it("emits string value for plain literals", () => {
    expect(conditionOperandXml("completed", esc)).toBe('<string value="completed"/>');
    expect(conditionOperandXml("yes", esc)).toBe('<string value="yes"/>');
  });

  it("does not treat mixed text+field as a pure field operand", () => {
    // Process Set uses valueToXml / compileSetExpression; condition RHS is atomic.
    expect(conditionOperandXml("prefix <<Exam:id>>", esc)).toContain("string value=");
    expect(conditionOperandXml("prefix <<Exam:id>>", esc)).not.toMatch(
      /^<string field=/,
    );
  });

  it("detects forbidden field-as-text emission", () => {
    expect(isFieldTokenEmittedAsValueLiteral('<string value="<<Exam:id>>"/>')).toBe(
      true,
    );
    expect(
      isFieldTokenEmittedAsValueLiteral('<string value="&lt;&lt;Exam:id&gt;&gt;"/>'),
    ).toBe(true);
    expect(isFieldTokenEmittedAsValueLiteral('<string field="Exam:id"/>')).toBe(false);
    expect(isFieldTokenEmittedAsValueLiteral('<string value="completed"/>')).toBe(
      false,
    );
  });
});
