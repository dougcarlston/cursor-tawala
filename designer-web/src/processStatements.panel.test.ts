/**
 * Selecting a nested Show (etc.) must map to its property panel key.
 */
import { describe, expect, it } from "vitest";
import { processPanelKeyForCommand } from "./processStatements";

describe("processPanelKeyForCommand", () => {
  it("maps show / showDocument / edit to the Show panel", () => {
    expect(processPanelKeyForCommand({ cmd: "show", form: "Admin" })).toBe("show");
    expect(processPanelKeyForCommand({ cmd: "showDocument", document: "Document 1" })).toBe(
      "show",
    );
    expect(processPanelKeyForCommand({ cmd: "edit", form: "Admin" })).toBe("show");
  });

  it("maps if and nested sibling types distinctly", () => {
    expect(processPanelKeyForCommand({ cmd: "if", condition: {}, then: [] })).toBe("if");
    expect(processPanelKeyForCommand({ cmd: "set", field: "X", value: "1" })).toBe("set");
    expect(processPanelKeyForCommand({ cmd: "foreach", recordName: "R", recordList: "L" })).toBe(
      "foreach",
    );
  });
});
