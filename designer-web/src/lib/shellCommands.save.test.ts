import { afterEach, describe, expect, it } from "vitest";
import {
  applyDirtyBeforeUnload,
  buildOpenProjectPickerOptions,
  buildSaveProjectPickerOptions,
  cancelSaveAsDialog,
  eventIsNewProjectChord,
  eventIsOpenProjectChord,
  eventIsSaveChord,
  isProjectJsonFileName,
  isSaveAsDialogOpen,
  saveAcceleratorLabel,
  saveAsAcceleratorLabel,
  saveProjectAs,
  suggestedProjectFileName,
  projectDisplayNameFromFileName,
} from "@/lib/shellCommands";

describe("Save chord helpers", () => {
  it("reports a non-empty Save accelerator label", () => {
    expect(saveAcceleratorLabel().length).toBeGreaterThan(0);
  });

  it("reports a non-empty Save As accelerator label", () => {
    expect(saveAsAcceleratorLabel().length).toBeGreaterThan(0);
    expect(saveAsAcceleratorLabel().toLowerCase()).toContain("s");
  });

  it("recognizes Ctrl/Cmd+S via code or key", () => {
    expect(
      eventIsSaveChord({ ctrlKey: true, metaKey: false, altKey: false, code: "KeyS", key: "s" }),
    ).toBe(true);
    expect(
      eventIsSaveChord({ ctrlKey: false, metaKey: true, altKey: false, code: "KeyS", key: "s" }),
    ).toBe(true);
    expect(
      eventIsSaveChord({ ctrlKey: true, metaKey: false, altKey: false, code: "", key: "S" }),
    ).toBe(true);
  });

  it("ignores non-Save chords", () => {
    expect(
      eventIsSaveChord({ ctrlKey: true, metaKey: false, altKey: false, code: "KeyA", key: "a" }),
    ).toBe(false);
    expect(
      eventIsSaveChord({ ctrlKey: true, metaKey: false, altKey: true, code: "KeyS", key: "s" }),
    ).toBe(false);
    expect(
      eventIsSaveChord({ ctrlKey: false, metaKey: false, altKey: false, code: "KeyS", key: "s" }),
    ).toBe(false);
  });
});

describe("New / Open file chords", () => {
  it("recognizes Ctrl/Cmd+N and Ctrl/Cmd+O", () => {
    const base = { ctrlKey: true, metaKey: false, altKey: false, shiftKey: false };
    expect(eventIsNewProjectChord({ ...base, code: "KeyN", key: "n" })).toBe(true);
    expect(eventIsOpenProjectChord({ ...base, code: "KeyO", key: "o" })).toBe(true);
    expect(eventIsNewProjectChord({ ...base, shiftKey: true, code: "KeyN", key: "n" })).toBe(false);
  });
});

describe("applyDirtyBeforeUnload", () => {
  it("does nothing when clean (no leave warning)", () => {
    let prevented = false;
    const e = {
      preventDefault: () => {
        prevented = true;
      },
      returnValue: "",
    };
    expect(applyDirtyBeforeUnload(e, false)).toBe(false);
    expect(prevented).toBe(false);
    expect(e.returnValue).toBe("");
  });

  it("sets returnValue when dirty (leave warning)", () => {
    let prevented = false;
    const e = {
      preventDefault: () => {
        prevented = true;
      },
      returnValue: "",
    };
    expect(applyDirtyBeforeUnload(e, true)).toBe(true);
    expect(prevented).toBe(true);
    expect(e.returnValue).toBe("You have unsaved changes.");
  });
});

describe("project JSON picker helpers", () => {
  it("open picker omits strict MIME filter (macOS greyed-json workaround)", () => {
    expect(buildOpenProjectPickerOptions()).toEqual({ multiple: false });
  });

  it("save picker accepts common macOS JSON MIME aliases", () => {
    expect(buildSaveProjectPickerOptions("Demo.json")).toMatchObject({
      suggestedName: "Demo.json",
      types: [
        {
          description: "Tawala project JSON",
          accept: expect.objectContaining({
            "application/json": [".json"],
            "text/plain": [".json"],
          }),
        },
      ],
    });
  });

  it("isProjectJsonFileName checks the leaf extension", () => {
    expect(isProjectJsonFileName("MyProject.json")).toBe(true);
    expect(isProjectJsonFileName("/tmp/foo.JSON")).toBe(true);
    expect(isProjectJsonFileName("notes.txt")).toBe(false);
  });
});

describe("suggestedProjectFileName", () => {
  it("uses the project name with a .json suffix", () => {
    expect(suggestedProjectFileName("MyProject")).toBe("MyProject.json");
    expect(suggestedProjectFileName("DirtBowl")).toBe("DirtBowl.json");
  });

  it("keeps an existing .json suffix and maps empty → MyProject.json", () => {
    expect(suggestedProjectFileName("Demo.json")).toBe("Demo.json");
    expect(suggestedProjectFileName("")).toBe("MyProject.json");
    expect(suggestedProjectFileName("   ")).toBe("MyProject.json");
  });

  it("uses Untitled.json when the empty template name is Untitled", () => {
    expect(suggestedProjectFileName("Untitled")).toBe("Untitled.json");
  });

  it("strips reserved filename characters", () => {
    expect(suggestedProjectFileName('a/b:c*"')).toBe("a_b_c_.json");
  });
});

describe("projectDisplayNameFromFileName / syncProjectNameFromFileName", () => {
  it("strips .json and path for the Explorer display name", () => {
    expect(projectDisplayNameFromFileName("Demo.json")).toBe("Demo");
    expect(projectDisplayNameFromFileName("/tmp/Foo Bar.json")).toBe("Foo Bar");
    expect(projectDisplayNameFromFileName("Untitled.json")).toBe("Untitled");
  });
});

describe("in-app Save As dialog request", () => {
  afterEach(() => {
    if (isSaveAsDialogOpen()) cancelSaveAsDialog();
  });

  it("saveProjectAs opens the dialog without writing", () => {
    expect(isSaveAsDialogOpen()).toBe(false);
    saveProjectAs();
    expect(isSaveAsDialogOpen()).toBe(true);
  });

  it("cancelSaveAsDialog closes the prompt", () => {
    saveProjectAs();
    expect(isSaveAsDialogOpen()).toBe(true);
    cancelSaveAsDialog();
    expect(isSaveAsDialogOpen()).toBe(false);
  });
});
