/**
 * Online Exam Builder pattern: empty Scoring form is only “skip to end of form”
 * so the post-process runs without a visible page (Java Form.findNextSegment).
 */
import { describe, expect, it } from "vitest";
import { createSession } from "./sessionStore.mjs";
import { evaluateSetExpression } from "./runtimeEngine.mjs";
import { handleFormSubmit, renderFormPage } from "./runtime.mjs";

function scoringProject() {
  return {
    name: "Exam",
    themePath: "default",
    forms: [
      {
        name: "Answer",
        process: "Post-Answer",
        items: [
          {
            type: "fib",
            label: "Q1",
            blanks: [{ name: "AnswerToMCQ", length: 10 }],
          },
        ],
      },
      {
        name: "Scoring",
        process: "Post-Scoring",
        preProcess: "SetupCustomizationVariables",
        items: [
          {
            type: "skipInstructions",
            label: "skip1",
            commands: [{ cmd: "skip", to: "__EndOfForm__" }],
          },
        ],
      },
    ],
    processes: [
      {
        name: "SetupCustomizationVariables",
        commands: [
          { cmd: "set", field: "Customize_Title", value: "My Exam" },
          { cmd: "set", field: "ShowScore", value: "yes" },
        ],
      },
      {
        name: "Post-Answer",
        commands: [{ cmd: "show", form: "Scoring" }],
      },
      {
        name: "Post-Scoring",
        commands: [
          {
            cmd: "if",
            condition: { field: "ShowScore", op: "equals", value: "yes" },
            then: [{ cmd: "showDocument", document: "End Of Exam With The Score" }],
            else: [{ cmd: "showDocument", document: "Exam End" }],
          },
        ],
      },
    ],
    documents: [
      {
        name: "End Of Exam With The Score",
        content: "<p>Score page for <<Customize_Title>></p>",
      },
      { name: "Exam End", content: "<p>Done</p>" },
    ],
  };
}

describe("Scoring form initial skip-to-end (Online Exam)", () => {
  it("runs Post-Scoring when Show Scoring opens the form (no empty page)", () => {
    const project = scoringProject();
    const session = createSession(project);
    session.fields.ShowScore = "yes";
    session.fields.Customize_Title = "Pre";

    const html = renderFormPage(
      project,
      "Scoring",
      "http://localhost:3001",
      "exam-test",
      session,
    );

    expect(html).toContain("Score page for");
    expect(html).toContain("My Exam");
    expect(html).not.toContain('name="submit"');
  });

  it("Post-Answer show Scoring continues into Post-Scoring document", () => {
    const project = scoringProject();
    const session = createSession(project);
    const html = handleFormSubmit(
      project,
      "Answer",
      session,
      { segmentId: "0", "Q1:AnswerToMCQ": "x", submit: "Submit" },
      "http://localhost:3001",
      "exam-test",
    );

    expect(html).toContain("Score page for");
    expect(html).toContain("My Exam");
  });
});

describe("evaluateSetExpression", () => {
  it("computes score percentage expressions", () => {
    expect(evaluateSetExpression("2 * 100 / 4")).toBe("50");
    expect(evaluateSetExpression("plain text")).toBe("plain text");
  });
});
