/**
 * Process insert / IF selection must keep working when Explorer selection
 * drifts (e.g. New Document) while the Process MDI window stays active.
 */
import { describe, expect, it, beforeEach } from "vitest";
import {
  resolveActiveProcessName,
  useProjectStore,
} from "@/store/projectStore";
import { resetProcessCommandHistoryForTests } from "@/lib/processCommandHistory";

describe("resolveActiveProcessName / Process focus after Explorer drift", () => {
  beforeEach(() => {
    resetProcessCommandHistoryForTests();
    useProjectStore.setState({
      project: {
        name: "Focus Test",
        forms: [],
        processes: [
          {
            name: "Post-Administration",
            commands: [
              {
                cmd: "if",
                condition: { field: "Administration:MCQ1", op: "equals", value: "c" },
                then: [
                  { cmd: "delete", form: "Questionnaire", from: "Questionnaire" },
                ],
              },
            ],
          },
        ],
        documents: [{ name: "Results", content: "" }],
      },
      selection: { kind: "process", name: "Post-Administration" },
      openWindows: [
        {
          id: "process:Post-Administration",
          kind: "process",
          name: "Post-Administration",
          z: 2,
          x: 40,
          y: 40,
          w: 640,
          h: 480,
          minimized: false,
          maximized: false,
        },
      ],
      activeWindowId: "process:Post-Administration",
      selectedProcessCommandPath: "root/0",
      processStatementPanel: "if",
      processInsertPath: "root/0/then",
      processInsertIndex: 1,
      dirty: false,
      statusMessage: "",
    });
  });

  it("prefers the active Process window over a drifted Document selection", () => {
    useProjectStore.setState({ selection: { kind: "document", name: "Records Deleted" } });
    const name = resolveActiveProcessName(useProjectStore.getState());
    expect(name).toBe("Post-Administration");
  });

  it("still selects IF and keeps the If panel after New Document steals Explorer selection", () => {
    useProjectStore.getState().addDocument();
    const afterAdd = useProjectStore.getState();
    expect(afterAdd.selection.kind).toBe("document");
    expect(afterAdd.activeWindowId).toBe("process:Post-Administration");

    useProjectStore.getState().setSelectedProcessCommandPath("root/0");
    const afterSelect = useProjectStore.getState();
    expect(afterSelect.selection).toEqual({
      kind: "process",
      name: "Post-Administration",
    });
    expect(afterSelect.selectedProcessCommandPath).toBe("root/0");
    expect(afterSelect.processStatementPanel).toBe("if");
  });

  it("sets insert point inside If then after Explorer drifted to a Document", () => {
    useProjectStore.setState({
      selection: { kind: "document", name: "Records Deleted" },
      selectedProcessCommandPath: null,
    });
    useProjectStore.getState().setProcessInsertPoint("root/0/then", 1);
    const s = useProjectStore.getState();
    expect(s.processInsertPath).toBe("root/0/then");
    expect(s.processInsertIndex).toBe(1);
    expect(s.selection).toEqual({ kind: "process", name: "Post-Administration" });
  });

  it("inserts Show Document at the Process insert point while Document is selected in Explorer", () => {
    useProjectStore.setState({
      selection: { kind: "document", name: "Records Deleted" },
      selectedProcessCommandPath: null,
      processInsertPath: "root/0/then",
      processInsertIndex: 1,
    });
    useProjectStore.getState().insertProcessCommand({
      cmd: "showDocument",
      document: "Records Deleted",
      reset: false,
    });
    const proc = useProjectStore
      .getState()
      .project.processes?.find((p) => p.name === "Post-Administration");
    expect(proc?.commands[0]?.cmd).toBe("if");
    const thenList = proc?.commands[0]?.cmd === "if" ? proc.commands[0].then : [];
    expect(thenList).toHaveLength(2);
    expect(thenList?.[1]).toMatchObject({
      cmd: "showDocument",
      document: "Records Deleted",
    });
  });
});
