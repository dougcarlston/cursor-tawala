import { afterEach, describe, expect, it } from "vitest";
import {
  applyDirtyBeforeUnload,
  buildOpenProjectPickerOptions,
  buildSaveProjectPickerOptions,
  cancelSaveAsDialog,
  eventIsNewProjectChord,
  eventIsOpenProjectChord,
  eventIsSaveChord,
  importProjectFileText,
  isOpenableProjectFileName,
  isProjectJsonFileName,
  isSaveAsDialogOpen,
  projectDisplayNameFromFileName,
  saveAcceleratorLabel,
  saveAsAcceleratorLabel,
  saveProjectAs,
  suggestedProjectFileName,
} from "@/lib/shellCommands";
import { useProjectStore } from "@/store/projectStore";

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

  it("isOpenableProjectFileName accepts JSON and .tawala", () => {
    expect(isOpenableProjectFileName("MyProject.json")).toBe(true);
    expect(isOpenableProjectFileName("DirtBowl.tawala")).toBe(true);
    expect(isOpenableProjectFileName("SignupSheets.tawala.xml")).toBe(true);
    expect(isOpenableProjectFileName("notes.txt")).toBe(false);
  });

  it("projectDisplayNameFromFileName strips .tawala / .json", () => {
    expect(projectDisplayNameFromFileName("DirtBowl.tawala")).toBe("DirtBowl");
    expect(projectDisplayNameFromFileName("SignupSheets.tawala.xml")).toBe("SignupSheets");
    expect(projectDisplayNameFromFileName("Demo.json")).toBe("Demo");
  });
});

describe("importProjectFileText (.tawala)", () => {
  afterEach(() => {
    useProjectStore.getState().newProject({ empty: true });
  });

  it("converts minimal .tawala XML into the store", () => {
    const xml = `<?xml version="1.0" encoding="utf-8" ?>
<project name="OpenMe" themePath="default" format="1.9">
  <forms>
    <form name="Form 1" startPoint="true">
      <items>
        <heading type="Main" label="H1">
          <paragraph align="left" indent="0">Hi</paragraph>
        </heading>
      </items>
    </form>
  </forms>
  <processes><process name="P1"><show form="Form 1"/></process></processes>
  <documents></documents>
</project>`;
    const result = importProjectFileText(xml, "OpenMe.tawala");
    expect(result.kind).toBe("tawala");
    const { project } = useProjectStore.getState();
    expect(project.name).toBe("OpenMe");
    expect(project.forms[0].name).toBe("Form 1");
    expect(project.processes?.[0].commands?.[0]).toMatchObject({
      cmd: "show",
      form: "Form 1",
    });
  });

  it("unwraps Deploy-style { project: {...} } JSON so forms appear", () => {
    const wrapped = JSON.stringify({
      project: {
        name: "potluck",
        format: "2.0",
        forms: [
          {
            name: "Potluck Organizer",
            items: [{ type: "text", label: "T1", content: "You entered the following information" }],
          },
        ],
        processes: [],
        documents: [],
      },
    });
    const result = importProjectFileText(wrapped, "potluck.json");
    expect(result.kind).toBe("json");
    const { project, openWindows } = useProjectStore.getState();
    expect(project.name).toBe("potluck");
    expect(project.forms).toHaveLength(1);
    expect(project.forms[0].name).toBe("Potluck Organizer");
    expect(project.forms[0].items[0]).toMatchObject({ label: "T1" });
    expect(openWindows.some((w) => w.name === "Potluck Organizer")).toBe(true);
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
