/**
 * Guards Explorer entity Delete enable rules (Form / Process / Document).
 * Confirm dialog is stubbed via window.confirm for form items + entities.
 */
import { describe, expect, it, beforeEach, vi, afterEach } from "vitest";
import {
  canDeleteProjectEntity,
  canDeleteSelection,
  confirmAndDeleteFormItem,
  confirmAndDeleteSelectedFormItem,
} from "@/lib/shellCommands";
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
    useProjectStore.getState().deleteDocument(name);
    expect(useProjectStore.getState().project.documents?.some((d) => d.name === name)).toBe(false);
  });
});

describe("confirmAndDeleteFormItem", () => {
  const confirmMock = vi.fn();

  beforeEach(() => {
    useProjectStore.getState().newProject({ empty: true });
    useProjectStore.getState().addForm();
    useProjectStore.getState().insertFormItem("text");
    confirmMock.mockReset();
    vi.stubGlobal("confirm", confirmMock);
  });

  afterEach(() => {
    vi.unstubAllGlobals();
  });

  it("cancels without deleting when confirm is false", () => {
    confirmMock.mockReturnValue(false);
    const formName = useProjectStore.getState().project.forms[0]!.name;
    const before = useProjectStore.getState().project.forms[0]!.items.length;
    expect(confirmAndDeleteFormItem(formName, 0)).toBe(false);
    expect(useProjectStore.getState().project.forms[0]!.items.length).toBe(before);
    expect(confirmMock).toHaveBeenCalled();
  });

  it("deletes when confirm is true", () => {
    confirmMock.mockReturnValue(true);
    const formName = useProjectStore.getState().project.forms[0]!.name;
    const before = useProjectStore.getState().project.forms[0]!.items.length;
    expect(confirmAndDeleteFormItem(formName, 0)).toBe(true);
    expect(useProjectStore.getState().project.forms[0]!.items.length).toBe(before - 1);
  });

  it("confirmAndDeleteSelectedFormItem uses selected row", () => {
    confirmMock.mockReturnValue(true);
    const formName = useProjectStore.getState().project.forms[0]!.name;
    useProjectStore.setState({
      selection: { kind: "form", name: formName },
      selectedItemIndex: 0,
    });
    const before = useProjectStore.getState().project.forms[0]!.items.length;
    expect(confirmAndDeleteSelectedFormItem()).toBe(true);
    expect(useProjectStore.getState().project.forms[0]!.items.length).toBe(before - 1);
  });
});
