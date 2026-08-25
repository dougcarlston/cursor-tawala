import { describe, expect, it } from "vitest";
import {
  getFieldValue,
  evalCondition,
  runCommands,
  lookupFormFieldBucket,
} from "./runtimeEngine.mjs";

describe("lookupFormFieldBucket", () => {
  it("matches direct keys and Item:blank suffixes", () => {
    const bucket = { Q7: "b", "Q2:State": "California CA" };
    expect(lookupFormFieldBucket(bucket, "Q7")).toBe("b");
    expect(lookupFormFieldBucket(bucket, "State")).toBe("California CA");
    expect(lookupFormFieldBucket(bucket, "Missing")).toBeUndefined();
  });
});

describe("getFieldValue Form:Field", () => {
  it("resolves LivingWill:Q7 from bare Q7 when formFields is unset", () => {
    const ctx = { fields: { Q7: "b" }, formName: "LivingWill" };
    expect(getFieldValue(ctx, "LivingWill:Q7")).toBe("b");
    expect(
      evalCondition({ field: "LivingWill:Q7", op: "mcEquals", value: "b" }, ctx),
    ).toBe(true);
  });

  it("still prefers formFields when present for qualified refs", () => {
    const ctx = {
      fields: { Q7: "a" },
      formFields: { LivingWill: { Q7: "b" } },
      formName: "LivingWill",
    };
    expect(getFieldValue(ctx, "LivingWill:Q7")).toBe("b");
  });

  it("resolves bare MCQ refs from formFields when ctx.fields is empty", () => {
    const ctx = {
      fields: {},
      formFields: { LivingWill: { Q7: "b" } },
      formName: "LivingWill",
    };
    expect(getFieldValue(ctx, "Q7")).toBe("b");
    expect(
      evalCondition({ field: "Q7", op: "mcEquals", value: "b" }, ctx),
    ).toBe(true);
    const target = runCommands(
      [{ cmd: "if", condition: { field: "Q7", op: "mcEquals", value: "b" }, then: [{ cmd: "skip", to: "Q9" }] }],
      ctx,
    );
    expect(target).toBe("Q9");
  });

  it("resolves FIB blank alternate labels from Item:blank keys", () => {
    const ctx = {
      fields: {},
      formFields: { LivingWill: { "Q2:State": "California CA" } },
      formName: "LivingWill",
    };
    expect(getFieldValue(ctx, "State")).toBe("California CA");
    expect(getFieldValue(ctx, "LivingWill:State")).toBe("California CA");
    expect(
      evalCondition({ field: "State", op: "contains", value: "CA" }, ctx),
    ).toBe(true);
  });

  it("prefers non-empty formFields over empty ctx.fields (Process If State)", () => {
    const ctx = {
      fields: { State: "", City: "", County: "" },
      formFields: { LivingWill: { "Q2:State": "California CA" } },
      formName: "LivingWill",
    };
    const cond = {
      or: [
        { field: "State", op: "contains", value: "CA" },
        { field: "State", op: "contains", value: "Ca" },
      ],
    };
    expect(getFieldValue(ctx, "State")).toBe("California CA");
    expect(evalCondition(cond, ctx)).toBe(true);
  });
});
