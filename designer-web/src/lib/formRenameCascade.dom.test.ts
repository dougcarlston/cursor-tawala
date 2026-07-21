/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  cascadeFormRenameInProject,
  rewriteFormNameInHtml,
  rewriteFormNameInString,
} from "./formRenameCascade";
import {
  FUNCTION_CONFIG_ATTR,
  FUNCTION_TOKEN_CLASS,
  FUNCTION_TOKEN_ATTR,
  buildFunctionDisplayString,
  serializeFunctionConfig,
} from "./functionTokens";
import { getFunctionDef } from "./functionCatalog";
import type { TawalaProject } from "@/types/tawala";

describe("rewriteFormNameInString", () => {
  it("rewrites Form:Field and Record:Form:Field prefixes", () => {
    expect(rewriteFormNameInString("Form 1:MCQ4", "Form 1", "Survey")).toBe("Survey:MCQ4");
    expect(rewriteFormNameInString("Record:Form 1:Name", "Form 1", "Survey")).toBe(
      "Record:Survey:Name",
    );
    expect(rewriteFormNameInString("<<Form 1:MCQ1>>", "Form 1", "Survey")).toBe(
      "<<Survey:MCQ1>>",
    );
  });

  it("rewrites whole-string form names only", () => {
    expect(rewriteFormNameInString("Form 1", "Form 1", "Survey")).toBe("Survey");
    expect(rewriteFormNameInString("See Form 1 here", "Form 1", "Survey")).toBe(
      "See Form 1 here",
    );
  });
});

describe("rewriteFormNameInHtml chip styles", () => {
  it("preserves inline font-size when refreshing function chip labels", () => {
    const def = getFunctionDef("itemization-table")!;
    const config = {
      numberOfColumns: 1,
      column: [{ header: "N", contents: "<<Form 1:Name>>" }],
    };
    const span = document.createElement("span");
    span.className = FUNCTION_TOKEN_CLASS;
    span.setAttribute(FUNCTION_TOKEN_ATTR, def.id);
    span.setAttribute(FUNCTION_CONFIG_ATTR, serializeFunctionConfig(config));
    span.style.fontSize = "12pt";
    span.textContent = buildFunctionDisplayString(def, config);

    const html = `<p class="doc-placed-text" style="font-size: 20pt">${span.outerHTML}</p>`;
    const next = rewriteFormNameInHtml(html, "Form 1", "Survey");
    expect(next).toContain("Survey:Name");
    expect(next).toContain("font-size: 12pt");
    expect(next).toContain("font-size: 20pt");
  });
});

describe("cascadeFormRenameInProject", () => {
  it("updates Document function configs and Form Text HTML", () => {
    const def = getFunctionDef("simple-list")!;
    const config = { "simple-list-field": "<<Form 1:Name>>" };
    const chip =
      `<span class="${FUNCTION_TOKEN_CLASS}" ${FUNCTION_TOKEN_ATTR}="${def.id}" ` +
      `${FUNCTION_CONFIG_ATTR}="${serializeFunctionConfig(config).replace(/"/g, "&quot;")}">` +
      `${buildFunctionDisplayString(def, config)}</span>`;

    const project: TawalaProject = {
      name: "T",
      format: "2.0",
      forms: [
        {
          name: "Form 1",
          items: [{ type: "text", label: "T1", content: `<p>${chip}</p>` }],
        },
      ],
      processes: [
        {
          name: "P1",
          commands: [{ cmd: "show", form: "Form 1" }],
        },
      ],
      documents: [{ name: "Doc", content: `<p>${chip}</p>` }],
    };

    const next = cascadeFormRenameInProject(project, "Form 1", "Survey");
    expect(next.forms[0].name).toBe("Form 1"); // caller renames the form object
    const textItem = next.forms[0].items[0];
    expect(textItem.type).toBe("text");
    if (textItem.type === "text") {
      expect(String(textItem.content)).toContain("Survey:Name");
    }
    expect(String(next.documents?.[0].content)).toContain("Survey:Name");
    expect(next.processes?.[0].commands?.[0]).toMatchObject({ form: "Survey" });
  });
});
