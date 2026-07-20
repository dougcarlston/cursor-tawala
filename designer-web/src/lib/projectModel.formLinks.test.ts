/**
 * One process may be Pre/Post on many forms (Potluck Show Results pattern).
 */
import { describe, expect, it } from "vitest";
import { formLinksForProcess } from "./projectModel";
import type { TawalaProject } from "@/types/tawala";

describe("formLinksForProcess multi-form", () => {
  it("lists the same process under every form that references it", () => {
    const project: TawalaProject = {
      name: "Potluck",
      format: "tawala-json",
      forms: [
        {
          name: "Potluck Organizer",
          items: [],
          preProcess: "Show Results",
          process: "Send Thanks",
        },
        {
          name: "Report",
          items: [],
          preProcess: "Show Results",
          process: "Delete Name",
        },
      ],
      processes: [
        { name: "Show Results", commands: [] },
        { name: "Send Thanks", commands: [] },
        { name: "Delete Name", commands: [] },
      ],
    };

    expect(formLinksForProcess(project, "Show Results")).toEqual([
      { formName: "Potluck Organizer", role: "Pre" },
      { formName: "Report", role: "Pre" },
    ]);
    expect(formLinksForProcess(project, "Send Thanks")).toEqual([
      { formName: "Potluck Organizer", role: "Post" },
    ]);
  });
});
