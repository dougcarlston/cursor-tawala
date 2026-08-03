/**
 * Patch Online Exam Builder Pre-Answer to resolve questions by QuestionId
 * (admin #) first, then SequenceNumber fallback. Also ensure Post-Questions
 * backfills SequenceNumber from QuestionId when blank.
 *
 * Usage: node scripts/patch-online-exam-pre-answer.mjs
 */
import fs from "fs";
import path from "path";

const ROBUST_PRE_ANSWER = {
  name: "Pre-Answer",
  commands: [
    {
      cmd: "comment",
      text: "- Load question for exam step QNumber. Admin # is QuestionId — try that first; fall back to SequenceNumber (exam order).",
    },
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
      do: setFromQueryCommands(),
    },
    {
      cmd: "if",
      condition: { field: "Q", op: "isBlank" },
      then: [
        {
          cmd: "comment",
          text: "- Fallback when QuestionId is blank/missing: match SequenceNumber to QNumber",
        },
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
          do: setFromQueryCommands(),
        },
      ],
    },
  ],
};

function setFromQueryCommands() {
  return [
    { cmd: "set", field: "Q", value: "<<Query:Question:Question>>", concat: false, arithmeticAsText: false },
    { cmd: "set", field: "QType", value: "<<Query:Question:Type>>", concat: false, arithmeticAsText: false },
    { cmd: "set", field: "A", value: "<<Query:Question:Choice1>>", concat: false, arithmeticAsText: false },
    { cmd: "set", field: "B", value: "<<Query:Question:Choice2>>", concat: false, arithmeticAsText: false },
    { cmd: "set", field: "C", value: "<<Query:Question:Choice3>>", concat: false, arithmeticAsText: false },
    { cmd: "set", field: "D", value: "<<Query:Question:Choice4>>", concat: false, arithmeticAsText: false },
    { cmd: "set", field: "E", value: "<<Query:Question:Choice5>>", concat: false, arithmeticAsText: false },
    { cmd: "set", field: "F", value: "<<Query:Question:Choice6>>", concat: false, arithmeticAsText: false },
    {
      cmd: "set",
      field: "CorrectAnswer",
      value: "<<Query:Question:Answer>>",
      concat: false,
      arithmeticAsText: false,
    },
    {
      cmd: "set",
      field: "LastQuestionId",
      value: "<<Query:Question:QuestionId>>",
      concat: false,
      arithmeticAsText: false,
    },
  ];
}

const SEQ_BACKFILL = {
  cmd: "if",
  condition: { field: "Question:SequenceNumber", op: "isBlank" },
  then: [
    {
      cmd: "comment",
      text: "- if SequenceNumber never assigned, mirror QuestionId so exam order matches admin #",
    },
    {
      cmd: "set",
      field: "Question:SequenceNumber",
      value: "<<Question:QuestionId>>",
      concat: false,
      arithmeticAsText: false,
    },
  ],
};

function projectRoot(data) {
  if (data.project && Array.isArray(data.project.processes)) return data.project;
  return data;
}

function ensureSeqBackfill(commands) {
  const blob = JSON.stringify(commands);
  if (blob.includes("mirror QuestionId so exam order matches admin")) return false;
  for (let i = 0; i < commands.length; i++) {
    const c = commands[i];
    if (
      c.cmd === "if" &&
      c.condition?.field === "Question:QuestionId" &&
      c.condition?.op === "isBlank"
    ) {
      commands.splice(i + 1, 0, structuredClone(SEQ_BACKFILL));
      return true;
    }
  }
  commands.unshift(structuredClone(SEQ_BACKFILL));
  return true;
}

function patchProject(data) {
  const root = projectRoot(data);
  const processes = root.processes;
  if (!Array.isArray(processes)) throw new Error("no processes");
  const changed = [];
  for (let i = 0; i < processes.length; i++) {
    if (processes[i].name === "Pre-Answer") {
      processes[i] = structuredClone(ROBUST_PRE_ANSWER);
      changed.push("Pre-Answer");
    }
    if (processes[i].name === "Post-Questions") {
      changed.push(
        ensureSeqBackfill(processes[i].commands)
          ? "Post-Questions-backfill"
          : "Post-Questions-already",
      );
    }
  }
  return changed;
}

const paths = [
  "/Users/DougC1/Projects/Tawala Projects/Priority Library Projects/Online Exam Builder-8-3-26.json",
  "/Users/DougC1/Projects/Tawala Projects/Priority Library Projects/Online Exam Builder-fresh.json",
  "/Users/DougC1/Projects/Tawala/website-mock/projects/library/Online Exam Builder.json",
  "/Users/DougC1/Projects/Tawala Projects/Master List JSON/Online Exam Builder/Online Exam Builder.json",
  "/Users/DougC1/Projects/Tawala/designer-web/.deployed/designer/_preview_Online_Exam_Builder.json",
  "/Users/DougC1/Projects/Tawala/designer-web/.deployed/designer/_preview_Online_Exam_Builder1.json",
];

for (const p of paths) {
  if (!fs.existsSync(p)) {
    console.log("MISSING", p);
    continue;
  }
  const data = JSON.parse(fs.readFileSync(p, "utf8"));
  const changed = patchProject(data);
  fs.writeFileSync(p, JSON.stringify(data, null, 2) + "\n");
  console.log(path.basename(p), "->", changed.join(", "));
}
