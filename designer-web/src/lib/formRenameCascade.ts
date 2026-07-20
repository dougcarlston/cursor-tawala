/**
 * When a Form is renamed in the Explorer, rewrite Form:Field refs in Documents,
 * Form Text / function chips, and Process commands (legacy DocumentEditorTest /
 * RenameForm → live field references).
 */

import { fieldToken } from "@/lib/fieldInsertion";
import {
  buildFunctionDisplayString,
  FUNCTION_CONFIG_ATTR,
  FUNCTION_TOKEN_ATTR,
  FUNCTION_TOKEN_CLASS,
  parseFunctionConfig,
  serializeFunctionConfig,
} from "@/lib/functionTokens";
import { getFunctionDef } from "@/lib/functionCatalog";
import { FIELD_NAME_ATTR, FIELD_TOKEN_CLASS } from "@/lib/fieldTokens";
import type {
  RichContentBlock,
  RichTextNode,
  TawalaProcessCommand,
  TawalaProject,
} from "@/types/tawala";

/** Rewrite Form-qualified field refs and whole-string form names. */
export function rewriteFormNameInString(
  value: string,
  oldForm: string,
  newForm: string,
): string {
  if (!value || !oldForm || oldForm === newForm) return value;
  // Config / Show Form: the entire value is the form name.
  if (value === oldForm) return newForm;
  const pairs: [string, string][] = [
    [`Record:${oldForm}:`, `Record:${newForm}:`],
    [`<<Record:${oldForm}:`, `<<Record:${newForm}:`],
    [`<<${oldForm}:`, `<<${newForm}:`],
    [`${oldForm}:`, `${newForm}:`],
  ];
  pairs.sort((a, b) => b[0].length - a[0].length);
  let out = value;
  for (const [from, to] of pairs) {
    // Prefix replace — from already ends with ":" so Field tails stay intact.
    out = out.split(from).join(to);
  }
  return out;
}

export function rewriteFormNameInValue(
  value: unknown,
  oldForm: string,
  newForm: string,
): unknown {
  if (typeof value === "string") {
    return rewriteFormNameInString(value, oldForm, newForm);
  }
  if (Array.isArray(value)) {
    return value.map((v) => rewriteFormNameInValue(v, oldForm, newForm));
  }
  if (value && typeof value === "object") {
    const out: Record<string, unknown> = {};
    for (const [k, v] of Object.entries(value as Record<string, unknown>)) {
      out[k] = rewriteFormNameInValue(v, oldForm, newForm);
    }
    return out;
  }
  return value;
}

/** Update field tokens + function chips for a form rename; preserve chip styles. */
export function rewriteFormNameInHtml(
  html: string,
  oldForm: string,
  newForm: string,
): string {
  if (!html || !oldForm || oldForm === newForm) return html;
  if (typeof document === "undefined") {
    return rewriteFormNameInString(html, oldForm, newForm);
  }

  const root = document.createElement("div");
  root.innerHTML = html;
  let changed = false;

  root.querySelectorAll(`.${FIELD_TOKEN_CLASS}`).forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    const raw = node.getAttribute(FIELD_NAME_ATTR) ?? "";
    const next = rewriteFormNameInString(raw, oldForm, newForm);
    if (next !== raw) {
      node.setAttribute(FIELD_NAME_ATTR, next);
      node.setAttribute("title", next);
      node.textContent = fieldToken(next);
      changed = true;
    }
  });

  root.querySelectorAll(`.${FUNCTION_TOKEN_CLASS}`).forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    const functionId = node.getAttribute(FUNCTION_TOKEN_ATTR) ?? "";
    const config = parseFunctionConfig(node.getAttribute(FUNCTION_CONFIG_ATTR));
    const nextConfig = rewriteFormNameInValue(
      config as Record<string, unknown>,
      oldForm,
      newForm,
    ) as Record<string, unknown>;
    const before = serializeFunctionConfig(config);
    const after = serializeFunctionConfig(nextConfig);
    if (before === after) return;
    // Keep inline face/size — only refresh config + label text.
    node.setAttribute(FUNCTION_CONFIG_ATTR, after);
    const def = getFunctionDef(functionId);
    if (def) {
      node.textContent = buildFunctionDisplayString(def, nextConfig);
    } else {
      node.textContent = rewriteFormNameInString(
        node.textContent ?? "",
        oldForm,
        newForm,
      );
    }
    changed = true;
  });

  root.querySelectorAll("[data-itemization-form]").forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    const raw = node.getAttribute("data-itemization-form") ?? "";
    const next = rewriteFormNameInString(raw, oldForm, newForm);
    if (next !== raw) {
      node.setAttribute("data-itemization-form", next);
      changed = true;
    }
  });

  if (!changed) {
    return rewriteFormNameInString(html, oldForm, newForm);
  }
  return root.innerHTML;
}

function rewriteRichNodes(
  nodes: RichTextNode[] | undefined,
  oldForm: string,
  newForm: string,
): RichTextNode[] | undefined {
  if (!nodes?.length) return nodes;
  return nodes.map(
    (node) =>
      rewriteFormNameInValue(node as unknown as Record<string, unknown>, oldForm, newForm) as RichTextNode,
  );
}

function rewriteRichBlocks(
  blocks: RichContentBlock[],
  oldForm: string,
  newForm: string,
): RichContentBlock[] {
  return blocks.map((block) => {
    const next: RichContentBlock = {
      ...block,
      text: block.text
        ? rewriteFormNameInString(block.text, oldForm, newForm)
        : block.text,
      nodes: rewriteRichNodes(block.nodes, oldForm, newForm),
    };
    if (block.rows) {
      next.rows = block.rows.map((row) => ({
        ...row,
        cells: row.cells.map((cell) => ({
          ...cell,
          content: cell.content
            ? rewriteRichBlocks(cell.content, oldForm, newForm)
            : cell.content,
        })),
      }));
    }
    if (block.type === "itemizationTable" || (block as { form?: string }).form) {
      const withForm = next as RichContentBlock & { form?: string };
      if (withForm.form === oldForm) withForm.form = newForm;
    }
    return next;
  });
}

function rewriteDocumentOrItemContent(
  content: string | RichContentBlock[] | undefined,
  oldForm: string,
  newForm: string,
): string | RichContentBlock[] | undefined {
  if (content == null) return content;
  if (typeof content === "string") {
    return rewriteFormNameInHtml(content, oldForm, newForm);
  }
  return rewriteRichBlocks(content, oldForm, newForm);
}

function rewriteProcessCommands(
  commands: TawalaProcessCommand[],
  oldForm: string,
  newForm: string,
): TawalaProcessCommand[] {
  return commands.map(
    (cmd) =>
      rewriteFormNameInValue(cmd, oldForm, newForm) as TawalaProcessCommand,
  );
}

/** Apply one form rename across Documents, Form Text, and Processes. */
export function cascadeFormRenameInProject(
  project: TawalaProject,
  oldForm: string,
  newForm: string,
): TawalaProject {
  if (!oldForm || !newForm || oldForm === newForm) return project;

  const forms = project.forms.map((form) => ({
    ...form,
    items: form.items.map((item) => {
      if (item.type === "text" || item.type === "heading") {
        const content = rewriteDocumentOrItemContent(
          item.content,
          oldForm,
          newForm,
        );
        return content === item.content ? item : { ...item, content };
      }
      return item;
    }),
  }));

  const documents = (project.documents ?? []).map((doc) => {
    const content = rewriteDocumentOrItemContent(doc.content, oldForm, newForm);
    return content === doc.content ? doc : { ...doc, content };
  });

  const processes = (project.processes ?? []).map((proc) => ({
    ...proc,
    commands: rewriteProcessCommands(proc.commands ?? [], oldForm, newForm),
  }));

  return {
    ...project,
    forms,
    documents,
    processes,
  };
}
