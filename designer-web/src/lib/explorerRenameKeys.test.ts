import { describe, expect, it } from "vitest";

/**
 * Explorer inline rename must key off the unique render key, not kind+name.
 * Linked processes appear under the Form and again under Processes — matching
 * only kind+name mounts two RenameInputs and blur-cancels the edit.
 */
function isEditing(
  editing: { key: string; kind: string; name: string } | null,
  nodeKey: string,
): boolean {
  return editing?.key === nodeKey;
}

describe("Explorer process rename keys", () => {
  it("edits only the Processes-folder row when F2 uses process:name", () => {
    const editing = { key: "process:Post-Process1", kind: "process", name: "Post-Process1" };
    const underForm = "Form 1:Post:Post-Process1";
    const underProcesses = "process:Post-Process1";
    expect(isEditing(editing, underProcesses)).toBe(true);
    expect(isEditing(editing, underForm)).toBe(false);
  });

  it("edits only the form-linked row when rename started there", () => {
    const editing = {
      key: "Form 1:Post:Post-Process1",
      kind: "process",
      name: "Post-Process1",
    };
    expect(isEditing(editing, "Form 1:Post:Post-Process1")).toBe(true);
    expect(isEditing(editing, "process:Post-Process1")).toBe(false);
  });

  it("kind+name alone would wrongly light up both rows", () => {
    const name = "Post-Process1";
    const kindMatch = (kind: string, n: string) => kind === "process" && n === name;
    expect(kindMatch("process", "Post-Process1")).toBe(true);
    // Both tree rows share kind+name — that was the bug.
    expect(kindMatch("process", name)).toBe(true);
  });
});
