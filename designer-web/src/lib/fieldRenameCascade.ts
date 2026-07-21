/**
 * When a Form field is renamed, rewrite stored references in Documents, Form Text,
 * structured function nodes, and Process commands — browser parity for legacy
 * `DocumentEditor.updateFieldsAndFunctions` / live FieldReference objects.
 */

import { blankFieldName } from "@/lib/projectModel";
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
import type { FunctionConfig } from "@/lib/functionCatalog";
import { FIELD_NAME_ATTR, FIELD_TOKEN_CLASS } from "@/lib/fieldTokens";
import type {
  FormItem,
  RichContentBlock,
  RichTextNode,
  TawalaForm,
  TawalaProcessCommand,
  TawalaProject,
} from "@/types/tawala";

export interface FieldRename {
  oldName: string;
  newName: string;
}

/** Field names an item contributes to the Fields tree (before → after for rename detect). */
export function formItemFieldNames(item: FormItem): string[] {
  switch (item.type) {
    case "fib":
      return (item.blanks ?? [])
        .map((_, i) => blankFieldName(item, i))
        .filter((n): n is string => !!n?.trim());
    case "mc": {
      const n = (item.name ?? item.alternateLabel ?? item.label ?? "").trim();
      return n ? [n] : [];
    }
    case "field": {
      const n = (item.fieldName ?? item.name ?? "").trim();
      return n ? [n] : [];
    }
    default:
      return [];
  }
}

/** Detect Fields-tree renames when a form item is updated in place. */
export function detectFormItemFieldRenames(prev: FormItem, next: FormItem): FieldRename[] {
  if (prev.type !== next.type) return [];

  if (prev.type === "fib" && next.type === "fib") {
    const out: FieldRename[] = [];
    const len = Math.max(prev.blanks?.length ?? 0, next.blanks?.length ?? 0);
    for (let i = 0; i < len; i++) {
      const oldName = blankFieldName(prev, i)?.trim() ?? "";
      const newName = blankFieldName(next, i)?.trim() ?? "";
      if (oldName && newName && oldName !== newName) {
        out.push({ oldName, newName });
      }
    }
    return out;
  }

  const before = formItemFieldNames(prev)[0] ?? "";
  const after = formItemFieldNames(next)[0] ?? "";
  if (before && after && before !== after) return [{ oldName: before, newName: after }];
  return [];
}

/**
 * Replace a field reference substring only when it is a whole token
 * (avoids `Name` → `Name2` mangling `Name20`).
 */
export function replaceExactFieldToken(haystack: string, from: string, to: string): string {
  if (!from || from === to || !haystack.includes(from)) return haystack;
  let result = "";
  let i = 0;
  while (i < haystack.length) {
    const idx = haystack.indexOf(from, i);
    if (idx < 0) {
      result += haystack.slice(i);
      break;
    }
    result += haystack.slice(i, idx);
    const end = idx + from.length;
    const beforeOk = idx === 0 || !isFieldTokenContinueChar(haystack[idx - 1]);
    const afterOk = end >= haystack.length || !isFieldTokenContinueChar(haystack[end]);
    result += beforeOk && afterOk ? to : from;
    i = end;
  }
  return result;
}

function isFieldTokenContinueChar(ch: string): boolean {
  return /[A-Za-z0-9_]/.test(ch);
}

/** Rewrite one string that may hold Form:Field / Record:Form:Field / <<…>> refs. */
export function rewriteFieldRefString(
  value: string,
  formName: string,
  oldField: string,
  newField: string,
): string {
  if (!value || !oldField || oldField === newField) return value;
  const pairs: [string, string][] = [
    [`Record:${formName}:${oldField}`, `Record:${formName}:${newField}`],
    [`<<Record:${formName}:${oldField}>>`, `<<Record:${formName}:${newField}>>`],
    [`<<${formName}:${oldField}>>`, `<<${formName}:${newField}>>`],
    [`${formName}:${oldField}`, `${formName}:${newField}`],
    [`<<${oldField}>>`, `<<${newField}>>`],
  ];
  pairs.sort((a, b) => b[0].length - a[0].length);
  let out = value;
  for (const [from, to] of pairs) {
    out = replaceExactFieldToken(out, from, to);
  }
  return out;
}

/** Deep-walk JSON-like values and rewrite string field refs. */
export function rewriteFieldRefsInValue(
  value: unknown,
  formName: string,
  oldField: string,
  newField: string,
): unknown {
  if (typeof value === "string") {
    return rewriteFieldRefString(value, formName, oldField, newField);
  }
  if (Array.isArray(value)) {
    return value.map((v) => rewriteFieldRefsInValue(v, formName, oldField, newField));
  }
  if (value && typeof value === "object") {
    const out: Record<string, unknown> = {};
    for (const [k, v] of Object.entries(value as Record<string, unknown>)) {
      out[k] = rewriteFieldRefsInValue(v, formName, oldField, newField);
    }
    return out;
  }
  return value;
}

function rewriteFunctionConfig(
  config: Record<string, unknown>,
  formName: string,
  oldField: string,
  newField: string,
): Record<string, unknown> {
  return rewriteFieldRefsInValue(config, formName, oldField, newField) as Record<
    string,
    unknown
  >;
}

/** Update field tokens + function chips (config + visible <<NAME(…)>> label) in HTML. */
export function rewriteFieldRefsInHtml(
  html: string,
  formName: string,
  oldField: string,
  newField: string,
): string {
  if (!html || !oldField || oldField === newField) return html;
  if (typeof document === "undefined") {
    // Node-only fallback (browser always has document). Still rewrite ref strings in
    // configs / plain tokens; chip labels need DOM rebuild via buildFunctionDisplayString.
    return rewriteFieldRefString(html, formName, oldField, newField);
  }

  const root = document.createElement("div");
  root.innerHTML = html;
  let changed = false;

  root.querySelectorAll(`.${FIELD_TOKEN_CLASS}`).forEach((node) => {
    if (!(node instanceof HTMLElement)) return;
    const raw = node.getAttribute(FIELD_NAME_ATTR) ?? "";
    const next = rewriteFieldRefString(raw, formName, oldField, newField);
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
    const nextConfig = rewriteFunctionConfig(
      config as Record<string, unknown>,
      formName,
      oldField,
      newField,
    ) as FunctionConfig;
    const before = serializeFunctionConfig(config);
    const after = serializeFunctionConfig(nextConfig);
    if (before === after) return;
    node.setAttribute(FUNCTION_CONFIG_ATTR, after);
    const def = getFunctionDef(functionId);
    if (def) {
      node.textContent = buildFunctionDisplayString(def, nextConfig);
    } else {
      node.textContent = rewriteFieldRefString(
        node.textContent ?? "",
        formName,
        oldField,
        newField,
      );
    }
    changed = true;
  });

  if (!changed) {
    // Plain <<Form:Field>> typed without a chip — still update when present.
    const plain = rewriteFieldRefString(html, formName, oldField, newField);
    return plain;
  }
  return root.innerHTML;
}

function rewriteRichNodes(
  nodes: RichTextNode[] | undefined,
  formName: string,
  oldField: string,
  newField: string,
): RichTextNode[] | undefined {
  if (!nodes?.length) return nodes;
  return nodes.map((node) => {
    const next = rewriteFieldRefsInValue(
      node as unknown as Record<string, unknown>,
      formName,
      oldField,
      newField,
    ) as RichTextNode;
    return next;
  });
}

function rewriteRichBlocks(
  blocks: RichContentBlock[],
  formName: string,
  oldField: string,
  newField: string,
): RichContentBlock[] {
  return blocks.map((block) => {
    const next: RichContentBlock = {
      ...block,
      text: block.text
        ? rewriteFieldRefString(block.text, formName, oldField, newField)
        : block.text,
      nodes: rewriteRichNodes(block.nodes, formName, oldField, newField),
    };
    if (block.rows) {
      next.rows = block.rows.map((row) => ({
        ...row,
        cells: row.cells.map((cell) => ({
          ...cell,
          content: cell.content
            ? rewriteRichBlocks(cell.content, formName, oldField, newField)
            : cell.content,
        })),
      }));
    }
    return next;
  });
}

function rewriteDocumentOrItemContent(
  content: string | RichContentBlock[] | undefined,
  formName: string,
  oldField: string,
  newField: string,
): string | RichContentBlock[] | undefined {
  if (content == null) return content;
  if (typeof content === "string") {
    return rewriteFieldRefsInHtml(content, formName, oldField, newField);
  }
  return rewriteRichBlocks(content, formName, oldField, newField);
}

function rewriteProcessCommands(
  commands: TawalaProcessCommand[],
  formName: string,
  oldField: string,
  newField: string,
): TawalaProcessCommand[] {
  return commands.map(
    (cmd) =>
      rewriteFieldRefsInValue(cmd, formName, oldField, newField) as TawalaProcessCommand,
  );
}

/**
 * Apply one field rename across Documents, Form Text HTML / structured nodes, and Processes.
 */
export function cascadeFieldRenameInProject(
  project: TawalaProject,
  formName: string,
  oldField: string,
  newField: string,
): TawalaProject {
  if (!oldField || !newField || oldField === newField) return project;

  const forms: TawalaForm[] = project.forms.map((form) => ({
    ...form,
    items: form.items.map((item): FormItem => {
      if (item.type === "text") {
        const content = rewriteDocumentOrItemContent(
          item.content,
          formName,
          oldField,
          newField,
        );
        return content === item.content ? item : { ...item, content };
      }
      if (item.type === "heading") {
        const content = rewriteDocumentOrItemContent(
          item.content,
          formName,
          oldField,
          newField,
        ) as string | undefined;
        return content === item.content ? item : { ...item, content };
      }
      return item;
    }),
  }));

  const documents = (project.documents ?? []).map((doc) => {
    const content = rewriteDocumentOrItemContent(
      doc.content,
      formName,
      oldField,
      newField,
    );
    return content === doc.content ? doc : { ...doc, content };
  });

  const processes = (project.processes ?? []).map((proc) => ({
    ...proc,
    commands: rewriteProcessCommands(proc.commands ?? [], formName, oldField, newField),
  }));

  return {
    ...project,
    forms,
    documents,
    processes,
  };
}
