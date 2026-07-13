import { getFieldValue, resolveTemplate } from "./runtimeEngine.mjs";
import { enhanceRichTextHtml, looksLikeRichHtml } from "./richHtmlPreview.mjs";
import { getThemeCss, themeBodyClass } from "./themes/index.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

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
      return enhanceRichTextHtml(doc.content, (ref) => getFieldValue(ctx, ref));
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
  const theme = project.themePath || "default";
  const ctx = {
    fields: { ...session.fields },
    formFields: session.formFields,
    records: session.records,
    recordLists: {},
    recordBindings: {},
    formName: opts.fromForm ?? "",
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
  const thenForm = opts.thenForm
    ? `${baseUrl}/p/${uniqueId}/${encodeURIComponent(opts.thenForm)}`
    : null;

  // Show only the documents themselves — no DirtBowl "Registration complete" chrome.
  const body = `
  ${sections.join("\n<hr/>\n")}
  ${
    thenForm
      ? `<p class="doc-continue"><a href="${thenForm}">Continue →</a></p>`
      : back
        ? `<p class="doc-back"><a href="${back}">← Back</a></p>`
        : ""
  }`;

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>${esc(project.name)}</title>
  <style>
    ${getThemeCss(theme)}
    body { max-width: 800px; margin: 2rem auto; padding: 0 1rem; }
    .dev-banner { background: #fff3cd; border: 1px solid #ffc107; padding: 8px 12px; margin-bottom: 1rem; font-size: 13px; }
    .doc-section { margin: 1.5rem 0; position: relative; min-height: 12rem; }
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
<body class="${themeBodyClass(theme)}">
  <div class="dev-banner">Tawala local runtime — ${esc(project.name)}</div>
  ${body}
</body>
</html>`;
}
