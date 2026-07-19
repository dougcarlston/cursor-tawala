/**
 * Field rename must refresh Document function chips + field tokens (and Process refs).
 */
import { describe, expect, it } from "vitest";
import {
  cascadeFieldRenameInProject,
  detectFormItemFieldRenames,
  replaceExactFieldToken,
  rewriteFieldRefString,
  rewriteFieldRefsInHtml,
} from "./fieldRenameCascade";
import {
  buildFunctionDisplayString,
  FUNCTION_CONFIG_ATTR,
  FUNCTION_TOKEN_ATTR,
  FUNCTION_TOKEN_CLASS,
  serializeFunctionConfig,
} from "./functionTokens";
import { getFunctionDef } from "./functionCatalog";
import { FIELD_NAME_ATTR, FIELD_TOKEN_CLASS } from "./fieldTokens";
import type { FormItem, TawalaProject } from "@/types/tawala";

describe("field rename cascade", () => {
  it("detects hidden-field and FIB blank renames", () => {
    const hiddenPrev: FormItem = { type: "field", label: "F", fieldName: "Field1", name: "Field1" };
    const hiddenNext: FormItem = { type: "field", label: "F", fieldName: "Email", name: "Email" };
    expect(detectFormItemFieldRenames(hiddenPrev, hiddenNext)).toEqual([
      { oldName: "Field1", newName: "Email" },
    ]);

    const fibPrev: FormItem = {
      type: "fib",
      label: "FIB1",
      blanks: [{ name: "a", length: 8, alternateLabel: "OldName" }],
    };
    const fibNext: FormItem = {
      type: "fib",
      label: "FIB1",
      blanks: [{ name: "a", length: 8, alternateLabel: "NewName" }],
    };
    expect(detectFormItemFieldRenames(fibPrev, fibNext)).toEqual([
      { oldName: "OldName", newName: "NewName" },
    ]);
  });

  it("does not rewrite longer names that share a prefix", () => {
    expect(replaceExactFieldToken("Form 1:Name20", "Form 1:Name", "Form 1:X")).toBe(
      "Form 1:Name20",
    );
    expect(rewriteFieldRefString("<<Form 1:Name>> and <<Form 1:Name2>>", "Form 1", "Name", "X")).toBe(
      "<<Form 1:X>> and <<Form 1:Name2>>",
    );
  });

  it("rewrites function config + display label in Document HTML", () => {
    const config = {
      field: "<<Form 1:FavoriteColor>>",
      "layout-type": "vertical",
      conditionsRows: [{ field: "Form 1:FavoriteColor", op: "isNotBlank", value: "" }],
    };
    const def = getFunctionDef("response-totals-table")!;
    const span = document.createElement("span");
    span.className = FUNCTION_TOKEN_CLASS;
    span.setAttribute(FUNCTION_TOKEN_ATTR, def.id);
    span.setAttribute(FUNCTION_CONFIG_ATTR, serializeFunctionConfig(config));
    span.textContent = buildFunctionDisplayString(def, config);
    const wrap = document.createElement("p");
    wrap.append(span);

    const next = rewriteFieldRefsInHtml(wrap.innerHTML, "Form 1", "FavoriteColor", "FavColor");
    expect(next).toContain("FavColor");
    expect(next).not.toContain("FavoriteColor");
    expect(next).toContain("RESPONSE TOTALS");
  });

  it("rewrites field token chips", () => {
    const root = document.createElement("div");
    root.innerHTML =
      `<span class="${FIELD_TOKEN_CLASS}" ${FIELD_NAME_ATTR}="Form 1:Field1" ` +
      `title="Form 1:Field1"><<Form 1:Field1>></span>`;
    const next = rewriteFieldRefsInHtml(root.innerHTML, "Form 1", "Field1", "Email");
    expect(next).toContain(`${FIELD_NAME_ATTR}="Form 1:Email"`);
    expect(next).toMatch(/Form 1:Email/);
    expect(next).not.toContain("Field1");
  });

  it("cascades through project documents and process Set targets", () => {
    const config = { "simple-list-field": "<<Form 1:Field1>>" };
    const def = getFunctionDef("simple-list")!;
    const span = document.createElement("span");
    span.className = FUNCTION_TOKEN_CLASS;
    span.setAttribute(FUNCTION_TOKEN_ATTR, def.id);
    span.setAttribute(FUNCTION_CONFIG_ATTR, serializeFunctionConfig(config));
    span.textContent = buildFunctionDisplayString(def, config);
    const wrap = document.createElement("p");
    wrap.append(span);

    const project: TawalaProject = {
      name: "P",
      format: "1.12",
      forms: [
        {
          name: "Form 1",
          startPoint: true,
          items: [{ type: "field", label: "F", fieldName: "Email", name: "Email" }],
        },
      ],
      documents: [{ name: "Doc 1", content: wrap.innerHTML }],
      processes: [
        {
          name: "Main",
          commands: [{ cmd: "set", field: "Record:Form 1:Field1", value: "1" }],
        },
      ],
    };

    const next = cascadeFieldRenameInProject(project, "Form 1", "Field1", "Email");
    const docHtml = String(next.documents?.[0]?.content ?? "");
    expect(docHtml).toContain("Form 1:Email");
    expect(docHtml).not.toContain("Field1");
    expect(next.processes?.[0]?.commands[0]).toMatchObject({
      field: "Record:Form 1:Email",
    });
  });
});
