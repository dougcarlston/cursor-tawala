import { getFieldValue, resolveTemplate } from "./runtimeEngine.mjs";
import { enhanceRichTextHtml, looksLikeRichHtml } from "./richHtmlPreview.mjs";
import { blankAliasesFromForm, renderItemizationTableHtml } from "./itemizationPreview.mjs";
import { renderChoiceTallyTableHtml } from "./choiceTallyPreview.mjs";
import { BASE_FORM_CSS, resolveTheme, themeBodyClass } from "./themes/index.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

const DOC_COMPONENT_TABLE_CSS = `
table.component { border-collapse: collapse; font-size: 1em; margin: 12px 0; }
table.component.outline { border: 1px solid #cccccc; }
table.component thead { background-color: #888888; color: #eeeeee; }
table.component thead th { padding: .2em 1em; border: 1px solid #cccccc; }
table.component tbody tr.odd { background-color: #f8f8f8; }
table.component tbody tr.even { background-color: #ffffff; }
table.component tbody tr:hover { background-color: #e8e8e8; }
table.component td { padding-left: 1em; padding-right: 1em; line-height: 1.5em; border: 1px solid #dddddd; }
table.component .graph { background: #e8e8e8; min-width: 120px; height: 1.2em; position: relative; }
table.component .graph .bar { display: block; height: 100%; background: #6a9fd8; min-width: 0; }
table.component .graph .bar span { padding-left: 4px; font-size: 0.85em; white-space: nowrap; }
.preview-itemization-table { margin: 1rem 0; }
.preview-choice-tally { margin: 1rem 0; }
`;

function renderNodes(nodes, ctx, baseUrl, uniqueId) {
  if (!nodes) return "";
  return nodes
    .map((n) => {
      switch (n.type) {
        case "text":
          return esc(n.text);
        case "bold":
          return `<strong>${renderNodes(n.nodes, ctx, baseUrl, uniqueId)}</strong>`;
        case "italic":
          return `<em>${renderNodes(n.nodes, ctx, baseUrl, uniqueId)}</em>`;
        case "underline":
          return `<span class="doc-underline">${renderNodes(n.nodes, ctx, baseUrl, uniqueId)}</span>`;
        case "field":
          return esc(getFieldValue(ctx, n.name ?? n.field));
        case "itemizationTable":
          return renderItemizationTableHtml(n, ctx);
        case "choiceTallyTable":
          return renderChoiceTallyTableHtml(n, ctx);
        case "font": {
          const color = n.color ? ` style="color:${esc(n.color)}"` : "";
          return `<span${color}>${renderNodes(n.nodes, ctx, baseUrl, uniqueId)}</span>`;
        }
        case "invitation": {
          const href = `${baseUrl}/p/${uniqueId}/${encodeURIComponent(n.form ?? "")}`;
          return `<a class="doc-invitation" href="${href}">${esc(n.text ?? n.form ?? "Continue")}</a>`;
        }
        case "paragraph":
          return `<p style="text-align:${esc(n.align ?? "left")}">${renderNodes(n.nodes, ctx, baseUrl, uniqueId)}</p>`;
        default:
          return renderNodes(n.nodes, ctx, baseUrl, uniqueId);
      }
    })
    .join("");
}

function renderDocumentBody(doc, ctx, baseUrl, uniqueId) {
  if (typeof doc.content === "string") {
    if (looksLikeRichHtml(doc.content)) {
      return enhanceRichTextHtml(doc.content, (ref) => getFieldValue(ctx, ref), {
        records: ctx.records,
        formName: ctx.formName,
        blankAliases: ctx.blankAliases,
        project: ctx.project,
      });
    }
    return `<p>${esc(resolveTemplate(doc.content, ctx))}</p>`;
  }
  if (!Array.isArray(doc.content)) return "";
  return doc.content
    .map((block) => {
      if (block.type === "paragraph") {
        return `<p style="text-align:${esc(block.align ?? "left")}">${renderNodes(block.nodes, ctx, baseUrl, uniqueId)}</p>`;
      }
      if (block.type === "text") return esc(block.text);
      return renderNodes([block], ctx, baseUrl, uniqueId);
    })
    .join("\n");
}

export function renderDocumentsPage(project, documentNames, session, baseUrl, uniqueId, opts = {}) {
  const { name: themeName, css: themeCss } = resolveTheme(project.themePath || "default");
  const fromForm = opts.fromForm ?? "";
  const formDef = fromForm ? project.forms?.find((f) => f.name === fromForm) : null;
  const ctx = {
    fields: { ...session.fields },
    formFields: session.formFields,
    records: session.records,
    recordLists: {},
    recordBindings: {},
    formName: fromForm,
    blankAliases: blankAliasesFromForm(formDef),
    project,
  };

  const sections = [];
  for (const name of documentNames) {
    const doc = project.documents?.find((d) => d.name === name);
    if (!doc) {
      sections.push(
        `<section class="doc-section"><p><em>Document “${esc(name)}” not found in project.</em></p></section>`,
      );
      continue;
    }
    sections.push(
      `<section class="doc-section" aria-label="${esc(name)}">${renderDocumentBody(doc, ctx, baseUrl, uniqueId)}</section>`,
    );
  }

  const back = opts.fromForm
    ? `${baseUrl}/p/${uniqueId}/${encodeURIComponent(opts.fromForm)}`
    : null;
  const thenFresh = opts.freshThenForm ? "?fresh=1" : "";
  const thenForm = opts.thenForm
    ? `${baseUrl}/p/${uniqueId}/${encodeURIComponent(opts.thenForm)}${thenFresh}`
    : null;
  const backFresh = opts.freshThenForm || opts.freshBack ? "?fresh=1" : "";
  const backHref = back ? `${back}${backFresh}` : null;
  // When Form follows Document in the same process, prefer stacking (appendHtml) so the
  // questionnaire appears under the Document — no injected "Continue →" artifact.
  const stackedForm = String(opts.appendHtml ?? "").trim();

  // Show documents (+ optional following form). No DirtBowl "Registration complete" chrome.
  const body = `
  ${sections.join("\n<hr/>\n")}
  ${stackedForm ? `<div class="doc-following-form">${stackedForm}</div>` : ""}
  ${
    stackedForm
      ? ""
      : thenForm
        ? `<p class="doc-continue"><a href="${thenForm}">Continue →</a></p>`
        : backHref
          ? `<p class="doc-back"><a href="${backHref}">← Back</a></p>`
          : ""
  }`;

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>${esc(project.name)}</title>
  <style>
    ${BASE_FORM_CSS}
    ${DOC_COMPONENT_TABLE_CSS}
    ${themeCss}
    .doc-section { margin: 1rem 0; position: relative; }
    .doc-following-form { margin-top: 1.5rem; padding-top: 1rem; border-top: 1px solid #ccc; }
    .doc-title { font-size: 1.1rem; color: #333; }
    .doc-invitation { color: #000080; font-weight: bold; }
    .doc-underline { text-decoration: underline; }
    .preview-display-image {
      display: inline-flex; align-items: center; justify-content: center;
      box-sizing: border-box; vertical-align: middle; margin: 4px 0; max-width: 100%;
      border: 1px dashed #888; background: #f0f0f0; color: #444;
      font-size: 12px; font-family: Arial, sans-serif; text-align: center; overflow: hidden;
    }
    .preview-display-image-label { padding: 4px 8px; word-break: break-word; max-width: 100%; line-height: 1.2; }
    .preview-display-mcq { font-style: italic; color: #555; }
  </style>
</head>
<body class="${themeBodyClass(themeName)}">
  <div class="dev-banner">Tawala local runtime — ${esc(project.name)}</div>
  ${body}
</body>
</html>`;
}
