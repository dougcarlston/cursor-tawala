import { describe, expect, it, afterEach } from "vitest";
import {
  beginProcessCommandHistory,
  canProcessCommandRedo,
  canProcessCommandUndo,
  endProcessCommandHistory,
  recordProcessCommandChange,
  recordProcessCommandSnapshot,
  redoProcessCommands,
  resetProcessCommandHistoryForTests,
  undoProcessCommands,
} from "./processCommandHistory";
import type { TawalaProcessCommand } from "@/types/tawala";

describe("processCommandHistory", () => {
  afterEach(() => {
    resetProcessCommandHistoryForTests();
  });

  it("undo/redo steps through command snapshots", () => {
    const v0: TawalaProcessCommand[] = [{ cmd: "set", field: "x", value: "1" }];
    const v1: TawalaProcessCommand[] = [{ cmd: "set", field: "x", value: "10" }];
    const v2: TawalaProcessCommand[] = [
      { cmd: "set", field: "x", value: "10" },
      { cmd: "comment", text: "note" },
    ];

    beginProcessCommandHistory("P1", v0);
    recordProcessCommandSnapshot("P1", v1);
    recordProcessCommandSnapshot("P1", v2);

    expect(canProcessCommandUndo()).toBe(true);
    expect(undoProcessCommands()?.[0]).toEqual({ cmd: "set", field: "x", value: "10" });
    expect(undoProcessCommands()?.[0]).toEqual({ cmd: "set", field: "x", value: "1" });
    expect(canProcessCommandRedo()).toBe(true);
    expect(redoProcessCommands()?.[0]).toEqual({ cmd: "set", field: "x", value: "10" });
    expect(canProcessCommandUndo()).toBe(true);
  });

  it("ignores snapshots for other process names", () => {
    beginProcessCommandHistory("P1", []);
    recordProcessCommandSnapshot("P2", [{ cmd: "comment", text: "x" }]);
    expect(canProcessCommandUndo()).toBe(false);
  });

  it("clears session on end", () => {
    beginProcessCommandHistory("P1", []);
    endProcessCommandHistory("P1");
    expect(canProcessCommandUndo()).toBe(false);
  });

  it("undoes If operator change (does not equal → equals)", () => {
    const before: TawalaProcessCommand[] = [
      {
        cmd: "if",
        condition: { field: "Form:Q1", op: "doesNotEqual", value: "No" },
        then: [],
      },
    ];
    const after: TawalaProcessCommand[] = [
      {
        cmd: "if",
        condition: { field: "Form:Q1", op: "equals", value: "No" },
        then: [],
      },
    ];

    beginProcessCommandHistory("P1", before);
    recordProcessCommandChange("P1", before, after);

    expect(undoProcessCommands()?.[0]?.condition).toEqual({
      field: "Form:Q1",
      op: "doesNotEqual",
      value: "No",
    });
  });

  it("skips no-op snapshots and anchors when history tip drifted", () => {
    const a: TawalaProcessCommand[] = [{ cmd: "comment", text: "a" }];
    const b: TawalaProcessCommand[] = [{ cmd: "comment", text: "b" }];
    const c: TawalaProcessCommand[] = [{ cmd: "comment", text: "c" }];

    beginProcessCommandHistory("P1", a);
    // Drift: store is already b but history tip is still a.
    recordProcessCommandChange("P1", b, c);

    expect(undoProcessCommands()).toEqual(b);
    expect(undoProcessCommands()).toEqual(a);
    expect(canProcessCommandUndo()).toBe(false);

    recordProcessCommandSnapshot("P1", b);
    expect(canProcessCommandUndo()).toBe(true);
    recordProcessCommandSnapshot("P1", b);
    expect(canProcessCommandUndo()).toBe(true);
  });
});
