/**
 * Online Exam Pre-Answer: load question by admin # (QuestionId) first,
 * then fall back to SequenceNumber when Q is still blank.
 */
import { describe, expect, it } from "vitest";
import { buildContext, runProcessByName } from "./runtimeEngine.mjs";

const PRE_ANSWER = {
  name: "Pre-Answer",
  commands: [
    {
      cmd: "get",
      recordList: "QuestionRecords",
      sourceForms: ["Question"],
      where: {
        field: "QuestionRecords:Question:QuestionId",
        op: "equals",
        value: "<<QNumber>>",
      },
    },
    {
      cmd: "foreach",
      recordName: "Query",
      recordList: "QuestionRecords",
      do: [
        { cmd: "set", field: "Q", value: "<<Query:Question:Question>>" },
        { cmd: "set", field: "QType", value: "<<Query:Question:Type>>" },
        { cmd: "set", field: "LastQuestionId", value: "<<Query:Question:QuestionId>>" },
      ],
    },
    {
      cmd: "if",
      condition: { field: "Q", op: "isBlank" },
      then: [
        {
          cmd: "get",
          recordList: "QuestionRecords",
          sourceForms: ["Question"],
          where: {
            field: "QuestionRecords:Question:SequenceNumber",
            op: "equals",
            value: "<<QNumber>>",
          },
        },
        {
          cmd: "foreach",
          recordName: "Query",
          recordList: "QuestionRecords",
          do: [
            { cmd: "set", field: "Q", value: "<<Query:Question:Question>>" },
            { cmd: "set", field: "QType", value: "<<Query:Question:Type>>" },
            { cmd: "set", field: "LastQuestionId", value: "<<Query:Question:QuestionId>>" },
          ],
        },
      ],
    },
  ],
};

function examProject(questions) {
  return {
    name: "Online Exam Builder (test)",
    forms: [{ name: "Answer", preProcess: "Pre-Answer", items: [] }],
    processes: [PRE_ANSWER],
    // questions seed lives only on the session; project shell is enough
    __seed: questions,
  };
}

function runPreAnswer(questions, qNumber) {
  const project = examProject(questions);
  const session = {
    fields: { QNumber: String(qNumber) },
    formFields: {},
    records: { Question: questions },
    formState: {},
    preProcessDone: {},
  };
  const ctx = buildContext(session, "Answer");
  runProcessByName(project, "Pre-Answer", ctx);
  return ctx.fields;
}

describe("Online Exam Pre-Answer question lookup", () => {
  it("loads by QuestionId (admin #) when SequenceNumber differs", () => {
    const fields = runPreAnswer(
      [
        {
          Question: "Capital of France?",
          Type: "FIB",
          QuestionId: "1",
          SequenceNumber: "99",
          Answer: "Paris",
        },
        {
          Question: "Other",
          Type: "FIB",
          QuestionId: "2",
          SequenceNumber: "1",
          Answer: "x",
        },
      ],
      1,
    );
    expect(fields.Q).toBe("Capital of France?");
    expect(fields.QType).toBe("FIB");
    expect(fields.LastQuestionId).toBe("1");
  });

  it("falls back to SequenceNumber when QuestionId does not match", () => {
    const fields = runPreAnswer(
      [
        {
          Question: "Seq-only row",
          Type: "MCQ",
          QuestionId: "",
          SequenceNumber: "1",
          Answer: "a",
          Choice1: "a",
        },
      ],
      1,
    );
    expect(fields.Q).toBe("Seq-only row");
    expect(fields.QType).toBe("MCQ");
  });

  it("leaves Q blank when neither key matches (no false hit)", () => {
    const fields = runPreAnswer(
      [
        {
          Question: "Later",
          Type: "FIB",
          QuestionId: "3",
          SequenceNumber: "3",
          Answer: "z",
        },
      ],
      1,
    );
    expect(fields.Q ?? "").toBe("");
  });
});
