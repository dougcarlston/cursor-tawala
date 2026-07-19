/**
 * Document rename must update Process Show / Send / Append references.
 */
import { describe, expect, it } from "vitest";
import {
  cascadeDocumentRenameInProject,
  rewriteDocumentNameInCommand,
} from "./documentRenameCascade";
import type { TawalaProject } from "@/types/tawala";

describe("document rename cascade", () => {
  it("rewrites Show Document on the command", () => {
    const next = rewriteDocumentNameInCommand(
      { cmd: "show", document: "Document 1" },
      "Document 1",
      "Old Doc",
    );
    expect(next).toEqual({ cmd: "show", document: "Old Doc" });
  });

  it("does not rewrite a longer name that shares a prefix", () => {
    const next = rewriteDocumentNameInCommand(
      { cmd: "show", document: "Document 10" },
      "Document 1",
      "Old Doc",
    );
    expect(next.document).toBe("Document 10");
  });

  it("rewrites Send body.document and Append appendage/document", () => {
    expect(
      rewriteDocumentNameInCommand(
        { cmd: "send", body: { document: "Document 1", reset: true }, to: "a@b.com" },
        "Document 1",
        "Old Doc",
      ),
    ).toEqual({
      cmd: "send",
      body: { document: "Old Doc", reset: true },
      to: "a@b.com",
    });

    expect(
      rewriteDocumentNameInCommand(
        { cmd: "append", appendage: "Document 1", document: "Header" },
        "Document 1",
        "Old Doc",
      ),
    ).toEqual({ cmd: "append", appendage: "Old Doc", document: "Header" });
  });

  it("rewrites nested If / ForEach branches", () => {
    const next = rewriteDocumentNameInCommand(
      {
        cmd: "if",
        condition: { field: "x", op: "equals", value: "1" },
        then: [{ cmd: "show", document: "Document 1" }],
        else: [
          {
            cmd: "foreach",
            recordName: "r",
            recordList: "list",
            commands: [{ cmd: "show", document: "Document 1" }],
          },
        ],
      },
      "Document 1",
      "Old Doc",
    );
    expect((next.then as { document: string }[])[0].document).toBe("Old Doc");
    const foreach = (next.else as { commands: { document: string }[] }[])[0];
    expect(foreach.commands[0].document).toBe("Old Doc");
  });

  it("cascades across all processes in a project", () => {
    const project: TawalaProject = {
      name: "Test",
      format: "tawala-json",
      forms: [{ name: "Form 1", items: [] }],
      processes: [
        {
          name: "Process 1",
          commands: [
            { cmd: "show", document: "Document 1" },
            { cmd: "show", form: "Form 1" },
          ],
        },
        {
          name: "Process 2",
          commands: [{ cmd: "send", body: { document: "Document 1" }, to: "x@y.z" }],
        },
      ],
      documents: [
        { name: "Old Doc", content: "" },
        { name: "Document 2", content: "" },
      ],
    };

    const next = cascadeDocumentRenameInProject(project, "Document 1", "Old Doc");
    expect(next.processes?.[0].commands[0]).toEqual({
      cmd: "show",
      document: "Old Doc",
    });
    expect(next.processes?.[0].commands[1]).toEqual({
      cmd: "show",
      form: "Form 1",
    });
    expect(next.processes?.[1].commands[0]).toEqual({
      cmd: "send",
      body: { document: "Old Doc" },
      to: "x@y.z",
    });
  });
});
