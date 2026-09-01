/**
 * ProcessEditor drag-reorder into ForEach/If containers and Copy/Paste shortcuts.
 * @vitest-environment happy-dom
 */
import { describe, expect, it, beforeEach, afterEach } from "vitest";
import { act, createElement } from "react";
import { createRoot } from "react-dom/client";
import { ProcessEditor } from "@/components/ProcessEditor";
import { SkipScriptView } from "@/components/SkipScriptView";
import { buildProcessScriptLines, moveProcessCommandBefore } from "@/lib/processScript";
import { getProcessClipboard, setProcessClipboard } from "@/lib/processClipboard";
import { readProcessStatementReorderDrag, setProcessStatementReorderDrag } from "@/lib/designerDrag";
import { runShellEditCommand } from "@/lib/shellCommands";
import { resetProcessCommandHistoryForTests } from "@/lib/processCommandHistory";
import { useProjectStore } from "@/store/projectStore";
import type { TawalaProcessCommand } from "@/types/tawala";

describe("moveProcessCommandBefore with ForEach and If containers", () => {
  it("moves a bottom Send statement into ForEach do list at index 2 (between statements and close paren)", () => {
    const commands: TawalaProcessCommand[] = [
      {
        cmd: "foreach",
        recordName: "Record",
        recordList: "Record List 2",
        do: [
          { cmd: "set", field: "Addressee", value: "<<Record:Form 1:FIB1:a>>" },
          { cmd: "set", field: "RecipEmail", value: "<<Record:Form 1:FIB2:a>>" },
        ],
      },
      {
        cmd: "send",
        document: "Document 1",
        to: "RecipEmail",
      },
    ];

    const moved = moveProcessCommandBefore(commands, "root/1", "root/0/do", 2);
    expect(moved).not.toBeNull();
    expect(moved!.newPath).toBe("root/0/do/2");
    expect(moved!.commands).toHaveLength(1);
    const foreachCmd = moved!.commands[0];
    expect(foreachCmd.cmd).toBe("foreach");
    expect(foreachCmd.do).toHaveLength(3);
    expect(foreachCmd.do![0]).toEqual({ cmd: "set", field: "Addressee", value: "<<Record:Form 1:FIB1:a>>" });
    expect(foreachCmd.do![1]).toEqual({ cmd: "set", field: "RecipEmail", value: "<<Record:Form 1:FIB2:a>>" });
    expect(foreachCmd.do![2]).toEqual({ cmd: "send", document: "Document 1", to: "RecipEmail" });
  });

  it("moves a bottom Send statement into ForEach do list at index 0 (after open paren, before first statement)", () => {
    const commands: TawalaProcessCommand[] = [
      {
        cmd: "foreach",
        recordName: "Record",
        recordList: "Record List 2",
        do: [
          { cmd: "set", field: "Addressee", value: "<<Record:Form 1:FIB1:a>>" },
          { cmd: "set", field: "RecipEmail", value: "<<Record:Form 1:FIB2:a>>" },
        ],
      },
      {
        cmd: "send",
        document: "Document 1",
        to: "RecipEmail",
      },
    ];

    const moved = moveProcessCommandBefore(commands, "root/1", "root/0/do", 0);
    expect(moved).not.toBeNull();
    expect(moved!.newPath).toBe("root/0/do/0");
    expect(moved!.commands).toHaveLength(1);
    const foreachCmd = moved!.commands[0];
    expect(foreachCmd.do).toHaveLength(3);
    expect(foreachCmd.do![0]).toEqual({ cmd: "send", document: "Document 1", to: "RecipEmail" });
    expect(foreachCmd.do![1]).toEqual({ cmd: "set", field: "Addressee", value: "<<Record:Form 1:FIB1:a>>" });
  });

  it("moves a statement from inside ForEach back to top-level after ForEach", () => {
    const commands: TawalaProcessCommand[] = [
      {
        cmd: "foreach",
        recordName: "Record",
        recordList: "Record List 2",
        do: [
          { cmd: "set", field: "Addressee", value: "test" },
          { cmd: "send", document: "Document 1", to: "RecipEmail" },
        ],
      },
    ];

    const moved = moveProcessCommandBefore(commands, "root/0/do/1", "root", 1);
    expect(moved).not.toBeNull();
    expect(moved!.newPath).toBe("root/1");
    expect(moved!.commands).toHaveLength(2);
    expect(moved!.commands[0].cmd).toBe("foreach");
    expect(moved!.commands[0].do).toHaveLength(1);
    expect(moved!.commands[1]).toEqual({ cmd: "send", document: "Document 1", to: "RecipEmail" });
  });

  it("moves a statement into If else branch", () => {
    const commands: TawalaProcessCommand[] = [
      {
        cmd: "if",
        condition: { op: "equals", field: "Match?", value: "Yes" },
        then: [{ cmd: "set", field: "A", value: "1" }],
        else: [{ cmd: "set", field: "B", value: "2" }],
      },
      { cmd: "send", document: "Doc", to: "Email" },
    ];

    const moved = moveProcessCommandBefore(commands, "root/1", "root/0/else", 1);
    expect(moved).not.toBeNull();
    expect(moved!.newPath).toBe("root/0/else/1");
    expect(moved!.commands).toHaveLength(1);
    expect(moved!.commands[0].else).toHaveLength(2);
    expect(moved!.commands[0].else![1]).toEqual({ cmd: "send", document: "Doc", to: "Email" });
  });
});

describe("readProcessStatementReorderDrag fallback", () => {
  it("reads from custom MIME or falls back to text/plain", () => {
    const dt = new DataTransfer();
    setProcessStatementReorderDrag(dt, "root/1");
    expect(readProcessStatementReorderDrag(dt)).toBe("root/1");

    const plainDt = new DataTransfer();
    plainDt.setData("text/plain", "reorder:root/0/do/2");
    expect(readProcessStatementReorderDrag(plainDt)).toBe("root/0/do/2");
  });
});

describe("SkipScriptView parenthesis click insertion", () => {
  it("clicking open paren ( sets insert point at start of block (index 0)", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);
    const lines = buildProcessScriptLines([
      {
        cmd: "foreach",
        recordName: "Record",
        recordList: "Record List 2",
        do: [
          { cmd: "set", field: "Addressee", value: "test" },
          { cmd: "set", field: "RecipEmail", value: "test" },
        ],
      },
    ]);

    const picks: Array<{ path: string; index: number }> = [];

    act(() => {
      root.render(
        createElement(SkipScriptView, {
          lines,
          insertPath: "root",
          insertIndex: 0,
          showLineControls: true,
          showAllInsertionGaps: true,
          onSelectInsertPoint: (path, index) => {
            picks.push({ path, index });
          },
        }),
      );
    });

    const openParen = host.querySelector(".skip-block-open");
    expect(openParen).toBeTruthy();
    act(() => {
      openParen?.dispatchEvent(new MouseEvent("click", { bubbles: true }));
    });
    expect(picks.length).toBe(1);
    expect(picks[0]).toEqual({ path: "root/0/do", index: 0 });

    const closeParen = host.querySelector(".skip-block-close");
    expect(closeParen).toBeTruthy();
    act(() => {
      closeParen?.dispatchEvent(new MouseEvent("click", { bubbles: true }));
    });
    expect(picks.length).toBe(2);
    expect(picks[1]).toEqual({ path: "root/0/do", index: 2 });

    act(() => {
      root.unmount();
    });
    host.remove();
  });
});

describe("ProcessEditor Copy & Paste keyboard shortcuts and insertion", () => {
  beforeEach(() => {
    setProcessClipboard(null);
    useProjectStore.setState({
      project: {
        name: "TestProject",
        forms: [{ name: "Form 1", items: [] }],
        processes: [
          {
            name: "Process 1",
            commands: [
              {
                cmd: "foreach",
                recordName: "Record",
                recordList: "Record List 2",
                do: [
                  { cmd: "set", field: "Addressee", value: "<<Record:Form 1:FIB1:a>>" },
                  { cmd: "set", field: "RecipEmail", value: "<<Record:Form 1:FIB2:a>>" },
                ],
              },
              {
                cmd: "send",
                document: "Document 1",
                to: "RecipEmail",
              },
            ],
          },
        ],
        documents: [],
      },
      selection: { kind: "process", name: "Process 1" },
      openWindows: [{ id: "proc-1", kind: "process", name: "Process 1", z: 1, x: 0, y: 0, w: 600, h: 400 }],
      activeWindowId: "proc-1",
      selectedProcessCommandPath: null,
      processInsertPath: "root",
      processInsertIndex: 0,
      processStatementPanel: "none",
    });
  });

  it("copies selected statement with Cmd/Ctrl+C, then pastes into ForEach with Cmd/Ctrl+V", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);

    act(() => {
      root.render(createElement(ProcessEditor, { processName: "Process 1" }));
    });

    // Select the Send statement at root/1
    act(() => {
      useProjectStore.getState().setSelectedProcessCommandPath("root/1");
    });

    // Press Cmd+C / Ctrl+C
    act(() => {
      window.dispatchEvent(
        new KeyboardEvent("keydown", {
          key: "c",
          code: "KeyC",
          ctrlKey: true,
          bubbles: true,
        }),
      );
    });

    const clipboard = getProcessClipboard();
    expect(clipboard).toEqual({
      cmd: "send",
      document: "Document 1",
      to: "RecipEmail",
    });

    // Set insertion point inside ForEach at index 2
    act(() => {
      useProjectStore.getState().setProcessInsertPoint("root/0/do", 2);
    });

    // Press Cmd+V / Ctrl+V
    act(() => {
      window.dispatchEvent(
        new KeyboardEvent("keydown", {
          key: "v",
          code: "KeyV",
          ctrlKey: true,
          bubbles: true,
        }),
      );
    });

    const proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands).toHaveLength(2);
    const foreachCmd = proc?.commands[0];
    expect(foreachCmd?.cmd).toBe("foreach");
    expect(foreachCmd?.do).toHaveLength(3);
    expect(foreachCmd?.do?.[2]).toEqual({
      cmd: "send",
      document: "Document 1",
      to: "RecipEmail",
    });

    act(() => {
      root.unmount();
    });
    host.remove();
  });

  it("cuts selected statement with Cmd/Ctrl+X, then pastes inside ForEach", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);

    act(() => {
      root.render(createElement(ProcessEditor, { processName: "Process 1" }));
    });

    // Select Send statement at root/1
    act(() => {
      useProjectStore.getState().setSelectedProcessCommandPath("root/1");
    });

    // Press Ctrl+X / Cmd+X
    act(() => {
      window.dispatchEvent(
        new KeyboardEvent("keydown", {
          key: "x",
          code: "KeyX",
          metaKey: true,
          bubbles: true,
        }),
      );
    });

    let proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands).toHaveLength(1);

    // Set insert point inside ForEach at index 0
    act(() => {
      useProjectStore.getState().setProcessInsertPoint("root/0/do", 0);
    });

    // Press Cmd+V
    act(() => {
      window.dispatchEvent(
        new KeyboardEvent("keydown", {
          key: "v",
          code: "KeyV",
          metaKey: true,
          bubbles: true,
        }),
      );
    });

    proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands).toHaveLength(1);
    expect(proc?.commands[0].do).toHaveLength(3);
    expect(proc?.commands[0].do?.[0]).toEqual({
      cmd: "send",
      document: "Document 1",
      to: "RecipEmail",
    });

    act(() => {
      root.unmount();
    });
    host.remove();
  });

  it("supports MenuBar / Toolbar Edit commands (runShellEditCommand) on process statements", () => {
    useProjectStore.getState().setSelectedProcessCommandPath("root/1");

    act(() => {
      runShellEditCommand("copy");
    });
    expect(getProcessClipboard()).toEqual({
      cmd: "send",
      document: "Document 1",
      to: "RecipEmail",
    });

    useProjectStore.getState().setProcessInsertPoint("root/0/do", 1);
    act(() => {
      runShellEditCommand("paste");
    });

    const proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands[0].do).toHaveLength(3);
    expect(proc?.commands[0].do?.[1]).toEqual({
      cmd: "send",
      document: "Document 1",
      to: "RecipEmail",
    });
  });

  it("cut then paste restores the statement at the same index (not one line lower)", () => {
    useProjectStore.getState().setSelectedProcessCommandPath("root/1");

    act(() => {
      runShellEditCommand("cut");
    });

    let proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands).toHaveLength(1);
    expect(useProjectStore.getState().processInsertPath).toBe("root");
    expect(useProjectStore.getState().processInsertIndex).toBe(1);

    act(() => {
      runShellEditCommand("paste");
    });

    proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands).toHaveLength(2);
    expect(proc?.commands[1]).toEqual({
      cmd: "send",
      document: "Document 1",
      to: "RecipEmail",
    });
  });
});

describe("ProcessEditor command undo", () => {
  afterEach(() => {
    resetProcessCommandHistoryForTests();
  });

  beforeEach(() => {
    useProjectStore.setState({
      project: {
        name: "TestProject",
        forms: [],
        processes: [
          {
            name: "Process 1",
            commands: [{ cmd: "set", field: "x", value: "x + 1" }],
          },
        ],
        documents: [],
      },
      selection: { kind: "process", name: "Process 1" },
      openWindows: [
        { id: "proc-1", kind: "process", name: "Process 1", z: 1, x: 0, y: 0, w: 600, h: 400 },
      ],
      activeWindowId: "proc-1",
      selectedProcessCommandPath: null,
      processInsertPath: "root",
      processInsertIndex: 0,
      processStatementPanel: "none",
    });
  });

  it("undoes a committed Set Modify via Cmd/Ctrl+Z", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);

    act(() => {
      root.render(createElement(ProcessEditor, { processName: "Process 1" }));
    });

    act(() => {
      useProjectStore.getState().updateProcessCommands("Process 1", [
        { cmd: "set", field: "x", value: "x + 10" },
      ]);
    });

    act(() => {
      window.dispatchEvent(
        new KeyboardEvent("keydown", {
          key: "z",
          code: "KeyZ",
          ctrlKey: true,
          bubbles: true,
        }),
      );
    });

    const proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands[0]).toEqual({ cmd: "set", field: "x", value: "x + 1" });

    act(() => {
      root.unmount();
    });
    host.remove();
  });

  it("undoes via Edit menu runShellEditCommand on Process window", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);

    act(() => {
      root.render(createElement(ProcessEditor, { processName: "Process 1" }));
    });

    act(() => {
      useProjectStore.getState().updateProcessCommands("Process 1", [
        { cmd: "set", field: "x", value: "x + 10" },
      ]);
    });

    act(() => {
      expect(runShellEditCommand("undo")).toBe(true);
    });

    const proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands[0]).toEqual({ cmd: "set", field: "x", value: "x + 1" });

    act(() => {
      root.unmount();
    });
    host.remove();
  });

  it("undoes If operator change (does not equal → equals) in script", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);

    act(() => {
      useProjectStore.setState({
        project: {
          name: "TestProject",
          forms: [{ name: "Form 1", items: [] }],
          processes: [
            {
              name: "Process 1",
              commands: [
                {
                  cmd: "if",
                  condition: { field: "Form 1:Q1", op: "doesNotEqual", value: "No" },
                  then: [],
                },
              ],
            },
          ],
          documents: [],
        },
      });
      root.render(createElement(ProcessEditor, { processName: "Process 1" }));
    });

    act(() => {
      useProjectStore.setState({
        selectedProcessCommandPath: "root/0",
        processStatementPanel: "if",
      });
    });

    act(() => {
      useProjectStore.getState().updateProcessCommands("Process 1", [
        {
          cmd: "if",
          condition: { field: "Form 1:Q1", op: "equals", value: "No" },
          then: [],
        },
      ]);
    });

    act(() => {
      expect(runShellEditCommand("undo")).toBe(true);
    });

    const proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Process 1");
    expect(proc?.commands[0]?.condition).toEqual({
      field: "Form 1:Q1",
      op: "doesNotEqual",
      value: "No",
    });
    expect(host.textContent).toContain("does not equal");
    const opSelect = host.querySelector(".skip-if-operator") as HTMLSelectElement | null;
    expect(opSelect?.value).toBe("doesNotEqual");
    expect(useProjectStore.getState().selectedProcessCommandPath).toBe("root/0");

    act(() => {
      root.unmount();
    });
    host.remove();
  });
});
