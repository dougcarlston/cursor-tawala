import { describe, expect, it } from "vitest";
import {
  conditionalDisplayOkEnabled,
  displayConditionToEditorState,
  editorStateToDisplayCondition,
} from "./conditionalDisplay";

describe("conditionalDisplay mapping", () => {
  it("loads a flat equals clause as enabled + one row", () => {
    const { enabled, conditions } = displayConditionToEditorState({
      field: "QEmail2",
      op: "equals",
      value: "Yes",
    });
    expect(enabled).toBe(true);
    expect(conditions.rows).toEqual([{ field: "QEmail2", op: "equals", value: "Yes" }]);
  });

  it("clears when checkbox is off", () => {
    expect(
      editorStateToDisplayCondition(false, {
        combinator: "and",
        rows: [{ field: "QEmail2", op: "equals", value: "Yes" }],
      }),
    ).toBeUndefined();
  });

  it("saves a single complete row as a flat clause", () => {
    expect(
      editorStateToDisplayCondition(true, {
        combinator: "and",
        rows: [{ field: "QEmail2", op: "equals", value: "Yes" }],
      }),
    ).toEqual({ field: "QEmail2", op: "equals", value: "Yes" });
  });

  it("saves multi-row as op/conditions tree", () => {
    expect(
      editorStateToDisplayCondition(true, {
        combinator: "or",
        rows: [
          { field: "A", op: "isBlank", value: "" },
          { field: "B", op: "equals", value: "1" },
        ],
      }),
    ).toEqual({
      op: "or",
      conditions: [
        { field: "A", op: "isBlank" },
        { field: "B", op: "equals", value: "1" },
      ],
    });
  });

  it("requires a complete Where row when enabled", () => {
    expect(
      conditionalDisplayOkEnabled(true, {
        combinator: "and",
        rows: [{ field: "", op: "equals", value: "" }],
      }),
    ).toBe(false);
    expect(
      conditionalDisplayOkEnabled(true, {
        combinator: "and",
        rows: [{ field: "Q1", op: "equals", value: "Yes" }],
      }),
    ).toBe(true);
    expect(
      conditionalDisplayOkEnabled(false, {
        combinator: "and",
        rows: [{ field: "", op: "equals", value: "" }],
      }),
    ).toBe(true);
  });
});
