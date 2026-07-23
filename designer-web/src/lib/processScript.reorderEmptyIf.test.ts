/**
 * Process statement reorder — including into empty If/ForEach bodies.
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import { act, createElement } from "react";
import { createRoot } from "react-dom/client";
import { SkipScriptView } from "@/components/SkipScriptView";
import { buildProcessScriptLines, moveProcessCommandBefore } from "@/lib/processScript";
import type { TawalaProcessCommand } from "@/types/tawala";

describe("moveProcessCommandBefore into empty If", () => {
  it("moves a sibling Set into an empty then branch at index 0", () => {
    const commands: TawalaProcessCommand[] = [
      {
        cmd: "if",
        condition: { op: "equals", field: "Match?", value: "Yes" },
        then: [],
      },
      { cmd: "set", field: "Admin:Teams", value: "<<TempName>>" },
      { cmd: "show", form: "Admin" },
    ];
    const moved = moveProcessCommandBefore(commands, "root/1", "root/0/then", 0);
    expect(moved).not.toBeNull();
    expect(moved!.newPath).toBe("root/0/then/0");
    const ifCmd = moved!.commands[0];
    expect(ifCmd.cmd).toBe("if");
    expect(ifCmd.then).toEqual([{ cmd: "set", field: "Admin:Teams", value: "<<TempName>>" }]);
    expect(moved!.commands[1]).toEqual({ cmd: "show", form: "Admin" });
  });

  it("moves a Show before a ForEach into that ForEach do (index rewrite after removal)", () => {
    // Regression: removing root/0 used to leave dest root/1/do stale → fallback to root
    // and the statement looked like it vanished from the loop.
    const commands: TawalaProcessCommand[] = [
      { cmd: "showDocument", document: "SignupList" },
      {
        cmd: "foreach",
        record: "Signup",
        do: [],
      },
    ];
    const moved = moveProcessCommandBefore(commands, "root/0", "root/1/do", 0);
    expect(moved).not.toBeNull();
    expect(moved!.newPath).toBe("root/0/do/0");
    expect(moved!.commands).toHaveLength(1);
    const foreachCmd = moved!.commands[0];
    expect(foreachCmd.cmd).toBe("foreach");
    expect(foreachCmd.do).toEqual([{ cmd: "showDocument", document: "SignupList" }]);
  });

  it("still moves a Show after a ForEach into that ForEach do", () => {
    const commands: TawalaProcessCommand[] = [
      {
        cmd: "foreach",
        record: "Signup",
        do: [],
      },
      { cmd: "showDocument", document: "SignupList" },
    ];
    const moved = moveProcessCommandBefore(commands, "root/1", "root/0/do", 0);
    expect(moved).not.toBeNull();
    expect(moved!.newPath).toBe("root/0/do/0");
    expect(moved!.commands).toHaveLength(1);
    expect(moved!.commands[0].do).toEqual([{ cmd: "showDocument", document: "SignupList" }]);
  });
});

describe("SkipScriptView empty If insert hit", () => {
  it("emits a drag hit target inside an empty If then block", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);
    const lines = buildProcessScriptLines([
      {
        cmd: "if",
        condition: { op: "equals", field: "Match?", value: "Yes" },
        then: [],
      },
      { cmd: "set", field: "Admin:Teams", value: "<<TempName>>" },
    ]);

    act(() => {
      root.render(
        createElement(SkipScriptView, {
          lines,
          insertPath: "root/0/then",
          insertIndex: 0,
          selectedCommandPath: "root/1",
          showLineControls: true,
          showAllInsertionGaps: true,
          insertHitTargets: true,
          onSelectInsertPoint: () => {},
          onSelectCommandPath: () => {},
        }),
      );
    });

    const interior = host.querySelector(
      '[data-process-insert-path="root/0/then"][data-process-insert-index="0"]',
    );
    expect(interior).toBeTruthy();

    act(() => {
      root.unmount();
    });
    host.remove();
  });
});
