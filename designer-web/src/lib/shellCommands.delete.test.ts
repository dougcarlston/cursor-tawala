/**
 * Guards Explorer entity Delete enable rules (Form / Process / Document).
 * Confirm dialog is not exercised here — see runShellDelete / confirmAndDeleteProjectEntity.
 */
import { describe, expect, it, beforeEach } from "vitest";
import { canDeleteProjectEntity, canDeleteSelection } from "@/lib/shellCommands";
import { useProjectStore } from "@/store/projectStore";

describe("canDeleteSelection / canDeleteProjectEntity", () => {
  beforeEach(() => {
    useProjectStore.getState().newProject({ empty: true });
    useProjectStore.getState().addDocument();
  });

  it("enables Delete for a selected Document (no form item)", () => {
    const docs = useProjectStore.getState().project.documents ?? [];
    const name = docs[0]?.name;
    expect(name).toBeTruthy();
    useProjectStore.setState({
      selection: { kind: "document", name },
      selectedItemIndex: null,
    });
    expect(canDeleteSelection()).toBe(true);
    expect(canDeleteProjectEntity()).toBe(true);
  });

  it("prefers form-item delete when a canvas row is selected", () => {
    useProjectStore.getState().addForm();
    const formName = useProjectStore.getState().project.forms[0]!.name;
    useProjectStore.setState({
      selection: { kind: "form", name: formName },
      selectedItemIndex: 0,
    });
    expect(canDeleteSelection()).toBe(true);
    expect(canDeleteProjectEntity()).toBe(false);
  });

  it("deleteDocument removes the entity and closes its window", () => {
    const name = useProjectStore.getState().project.documents![0]!.name;
    useProjectStore.getState().openWindow("document", name);
    expect(useProjectStore.getState().openWindows.some((w) => w.name === name)).toBe(true);
    useProjectStore.getState().deleteDocument(name);
    expect(useProjectStore.getState().project.documents?.some((d) => d.name === name)).toBe(
      false,
    );
    expect(useProjectStore.getState().openWindows.some((w) => w.name === name)).toBe(false);
    expect(useProjectStore.getState().dirty).toBe(true);
  });
});
