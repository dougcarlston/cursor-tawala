import { describe, expect, it } from "vitest";
import {
  collectKnownVariables,
  lookupFormFieldMcItem,
} from "./projectModel";
import { isValidIfConditionField } from "./fieldInsertion";
import { rowsAreValid } from "./statementBuilders";
import type { TawalaProject } from "@/types/tawala";

function livingWillStub(): TawalaProject {
  return {
    name: "Living Will",
    forms: [
      {
        name: "LivingWill",
        items: [
          {
            type: "mc",
            label: "Q7",
            name: "Q7",
            onlyone: true,
            choices: [{ value: "a", label: "A" }, { value: "b", label: "B" }],
          },
          {
            type: "fib",
            label: "T5",
            blanks: [{ name: "FirstName" }],
          },
        ],
      },
    ],
    processes: [],
    documents: [],
  } as TawalaProject;
}

describe("collectKnownVariables / If field validation", () => {
  it("includes variables introduced by If condition statements", () => {
    const project: TawalaProject = {
      ...livingWillStub(),
      processes: [
        {
          name: "Process 1",
          commands: [
            {
              cmd: "if",
              condition: { field: "JustStarting", op: "isBlank", value: "" },
              then: [],
            },
          ],
        },
      ],
    };
    const vars = collectKnownVariables(project);
    expect(vars.has("JustStarting")).toBe(true);
  });

  it("accepts form fields, known variables, and newly typed variable names", () => {
    const project = livingWillStub();
    const vars = collectKnownVariables(project);
    expect(vars.has("Q7")).toBe(true);
    expect(vars.has("LivingWill:Q7")).toBe(true);
    expect(vars.has("FirstName")).toBe(true);
    expect(isValidIfConditionField("Q7", vars)).toBe(true);
    expect(isValidIfConditionField("LivingWill:Q7", vars)).toBe(true);
    expect(isValidIfConditionField("JustStarting", vars)).toBe(true);
    expect(rowsAreValid([{ field: "Q7", op: "mcEquals", value: "b" }], vars)).toBe(true);
    expect(rowsAreValid([{ field: "JustStarting", op: "isBlank", value: "" }], vars)).toBe(true);
  });
});

describe("lookupFormFieldMcItem", () => {
  it("resolves bare MCQ labels from convert (Q7) as well as Form:Field", () => {
    const project = livingWillStub();
    const bare = lookupFormFieldMcItem(project, "Q7");
    expect(bare?.label).toBe("Q7");
    expect(bare?.onlyone).toBe(true);
    const qual = lookupFormFieldMcItem(project, "LivingWill:Q7");
    expect(qual?.label).toBe("Q7");
  });
});
