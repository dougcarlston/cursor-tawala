import { describe, expect, it } from "vitest";
import {
  conditionOpsForKind,
  defaultOpForKind,
  isUnaryConditionOp,
  remapConditionOp,
} from "./mcConditionOperators";
import { lookupFormFieldMcItem, parseFormFieldRef } from "./projectModel";
import type { TawalaProject } from "@/types/tawala";

describe("mcConditionOperators", () => {
  it("uses MCMany list (incl. contains) when onlyone is false", () => {
    const ids = conditionOpsForKind("mcMany").map((o) => o.id);
    expect(ids).toContain("mcContains");
    expect(ids).toContain("mcEquals");
    expect(ids).not.toContain("equals");
    expect(defaultOpForKind("mcMany")).toBe("mcContains");
  });

  it("uses MCOne list (no contains) for single-select", () => {
    const ids = conditionOpsForKind("mcOne").map((o) => o.id);
    expect(ids).toContain("mcEquals");
    expect(ids).not.toContain("mcContains");
    expect(defaultOpForKind("mcOne")).toBe("mcEquals");
  });

  it("remaps FIB equals → mcEquals when switching to MCQ", () => {
    expect(remapConditionOp("equals", "mcOne")).toBe("mcEquals");
    expect(remapConditionOp("contains", "mcMany")).toBe("mcContains");
    expect(remapConditionOp("isBlank", "mcOne")).toBe("mcIsBlank");
    expect(remapConditionOp("mcEquals", "hybrid")).toBe("equals");
  });

  it("treats mcIsBlank as unary", () => {
    expect(isUnaryConditionOp("mcIsBlank")).toBe(true);
    expect(isUnaryConditionOp("mcEquals")).toBe(false);
  });
});

describe("lookupFormFieldMcItem", () => {
  const project = {
    name: "P",
    forms: [
      {
        name: "Form 1",
        items: [
          {
            type: "mc",
            label: "MCQ4",
            name: "MCQ4",
            onlyone: false,
            choices: [
              { name: "d", text: "Yellow" },
              { name: "a", text: "Blue" },
            ],
          },
          {
            type: "mc",
            label: "MCQ1",
            name: "MCQ1",
            onlyone: true,
            choices: [{ name: "d", text: "Yellow" }],
          },
          { type: "fib", label: "Name", blanks: [{ name: "a" }] },
        ],
      },
    ],
    processes: [],
    documents: [],
  } as unknown as TawalaProject;

  it("parses Record: and bare Form: refs", () => {
    expect(parseFormFieldRef("Record:Form 1:MCQ4")).toEqual({ form: "Form 1", field: "MCQ4" });
    expect(parseFormFieldRef("<<Form 1:MCQ4>>")).toEqual({ form: "Form 1", field: "MCQ4" });
  });

  it("finds multi and single MCQs; ignores FIB", () => {
    const multi = lookupFormFieldMcItem(project, "Form 1:MCQ4");
    expect(multi?.onlyone).toBe(false);
    const single = lookupFormFieldMcItem(project, "Record:Form 1:MCQ1");
    expect(single?.onlyone).toBe(true);
    expect(lookupFormFieldMcItem(project, "Form 1:Name")).toBeNull();
  });
});
