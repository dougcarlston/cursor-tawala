import { beforeEach, describe, expect, it } from "vitest";
import { useProjectStore } from "@/store/projectStore";

describe("Form Starting Point auto-assignment", () => {
  beforeEach(() => {
    useProjectStore.getState().newProject({ empty: true });
  });

  it("default new project creates Form 1 marked as starting point", () => {
    useProjectStore.getState().newProject();
    const { project } = useProjectStore.getState();
    expect(project.forms).toHaveLength(1);
    expect(project.forms[0].name).toBe("Form 1");
    expect(project.forms[0].startPoint).toBe(true);
  });

  it("marks the first form added to an empty project as starting point", () => {
    expect(useProjectStore.getState().project.forms).toHaveLength(0);

    useProjectStore.getState().addForm();

    const { project } = useProjectStore.getState();
    expect(project.forms).toHaveLength(1);
    expect(project.forms[0].name).toBe("Form 1");
    expect(project.forms[0].startPoint).toBe(true);
  });

  it("does not mark subsequent forms as starting point when a starting point already exists", () => {
    useProjectStore.getState().addForm();
    useProjectStore.getState().addForm();

    const { project } = useProjectStore.getState();
    expect(project.forms).toHaveLength(2);
    expect(project.forms[0].name).toBe("Form 1");
    expect(project.forms[0].startPoint).toBe(true);
    expect(project.forms[1].name).toBe("Form 2");
    expect(project.forms[1].startPoint).toBeUndefined();
  });

  it("marks new form as starting point if no forms in project are currently marked as starting point", () => {
    useProjectStore.getState().addForm();
    // Toggle startPoint off on Form 1
    useProjectStore.getState().toggleFormStartPoint("Form 1");
    expect(useProjectStore.getState().project.forms[0].startPoint).toBe(false);

    // Adding Form 2 should now auto-mark it as starting point
    useProjectStore.getState().addForm();

    const { project } = useProjectStore.getState();
    expect(project.forms).toHaveLength(2);
    expect(project.forms[0].startPoint).toBe(false);
    expect(project.forms[1].name).toBe("Form 2");
    expect(project.forms[1].startPoint).toBe(true);
  });
});
