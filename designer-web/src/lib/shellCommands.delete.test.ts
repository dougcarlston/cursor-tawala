/**
 * Guards Explorer entity Delete enable rules (Form / Process / Document).
 * Confirm dialog is driven via showDesignerConfirm / confirmDialog for form items + entities.
 */
import { describe, expect, it, beforeEach } from "vitest";
import {
  canDeleteProjectEntity,
  canDeleteSelection,
  confirmAndDeleteFormItem,
  confirmAndDeleteSelectedFormItem,
} from "@/lib/shellCommands";
import { getActiveConfirm } from "@/lib/confirmDialog";
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
  beforeEach(() => {
    useProjectStore.getState().newProject({ empty: true });
    useProjectStore.getState().addForm();
    useProjectStore.getState().insertFormItem("text");
  });

  it("cancels without deleting when confirm is rejected", async () => {
    const formName = useProjectStore.getState().project.forms[0]!.name;
    const before = useProjectStore.getState().project.forms[0]!.items.length;
    confirmAndDeleteFormItem(formName, 0);
    const active = getActiveConfirm();
    expect(active).not.toBeNull();
    active?.resolve(false);
    await Promise.resolve();
    expect(useProjectStore.getState().project.forms[0]!.items.length).toBe(before);
  });

  it("deletes when confirm is accepted", async () => {
    const formName = useProjectStore.getState().project.forms[0]!.name;
    const before = useProjectStore.getState().project.forms[0]!.items.length;
    confirmAndDeleteFormItem(formName, 0);
    const active = getActiveConfirm();
    expect(active).not.toBeNull();
    active?.resolve(true);
    await Promise.resolve();
    expect(useProjectStore.getState().project.forms[0]!.items.length).toBe(before - 1);
  });

  it("confirmAndDeleteSelectedFormItem uses selected row", async () => {
    const formName = useProjectStore.getState().project.forms[0]!.name;
    useProjectStore.setState({
      selection: { kind: "form", name: formName },
      selectedItemIndex: 0,
    });
    const before = useProjectStore.getState().project.forms[0]!.items.length;
    confirmAndDeleteSelectedFormItem();
    const active = getActiveConfirm();
    expect(active).not.toBeNull();
    active?.resolve(true);
    await Promise.resolve();
    expect(useProjectStore.getState().project.forms[0]!.items.length).toBe(before - 1);
  });
});

