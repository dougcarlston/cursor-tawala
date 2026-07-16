/**
 * Shared Form/Document Preview helpers — field substitution + DISPLAY IMAGE placeholders.
 * Kept separate so documentRenderer and runtime can both use it without circular imports.
 */

import {
  itemizationNodeFromConfig,
  renderItemizationTableHtml,
  countFormRecordsFromConfig,
} from "./itemizationPreview.mjs";
import { readAttr, replaceMatchingSpans } from "./htmlSpanReplace.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function parseDisplayImagePx(raw) {
  const s = String(raw ?? "").trim();
  if (!s || s.includes("<<")) return null;
  const n = Number.parseInt(s, 10);
  return Number.isFinite(n) && n > 0 ? n : null;
}

function displayImagePreviewLabel(config) {
  const alt = String(config.alt_title ?? "").trim();
  if (alt && !/^https?:\/\//i.test(alt)) return alt;

  const source = String(config.source ?? "").trim();
  if (source.startsWith("<<") && source.endsWith(">>")) {
    const field = source.slice(2, -2).trim();
    if (field) return field;
  }

  const pathPart = source.split(/[?#]/)[0];
  try {
    const u = new URL(source);
    const base = u.pathname.split("/").filter(Boolean).pop();
    if (base) return decodeURIComponent(base);
  } catch {
    const base = pathPart.split("/").filter(Boolean).pop();
    if (base && !base.includes("://")) {
      try {
        return decodeURIComponent(base);
      } catch {
        return base;
      }
    }
  }
  return "Image";
}

function renderDisplayImagePlaceholder(config) {
  const width = parseDisplayImagePx(config.width) ?? 320;
  const height = parseDisplayImagePx(config.height) ?? 80;
  const label = displayImagePreviewLabel(config);
  return (
    `<span class="preview-display-image" role="img" aria-label="${esc(label)}" ` +
    `style="width:${width}px;height:${height}px">` +
    `<span class="preview-display-image-label">${esc(label)}</span>` +
    `</span>`
  );
}

function parseFunctionConfigAttr(attrs) {
  const raw = readAttr(attrs, "data-function-config");
  if (!raw) return {};
  try {
    const parsed = JSON.parse(raw);
    return parsed && typeof parsed === "object" ? parsed : {};
  } catch {
    return {};
  }
}

function parseStructuredNodeAttr(attrs) {
  const raw = readAttr(attrs, "data-tawala-structured-node");
  if (!raw) return null;
  try {
    const parsed = JSON.parse(decodeURIComponent(raw));
    if (parsed?.type === "itemizationTable") return parsed;
  } catch {
    /* ignore */
  }
  return null;
}

function matchFunctionId(attrs, id) {
  return readAttr(attrs, "data-function-id") === id;
}

/** Document MQL chips may be structured nodes (no data-function-id). */
function matchItemizationToken(attrs) {
  if (matchFunctionId(attrs, "itemization-table")) return true;
  if (readAttr(attrs, "data-itemization-token") === "true") return true;
  return Boolean(parseStructuredNodeAttr(attrs));
}

function replaceDisplayImageTokens(html) {
  return replaceMatchingSpans(
    html,
    (attrs) => matchFunctionId(attrs, "display-image"),
    (attrs) => renderDisplayImagePlaceholder(parseFunctionConfigAttr(attrs)),
  );
}

function replaceDisplayMcqTokens(html) {
  return replaceMatchingSpans(
    html,
    (attrs) => matchFunctionId(attrs, "display-mcq-label"),
    (attrs) => {
      const config = parseFunctionConfigAttr(attrs);
      let field = String(config["field-name"] ?? "").trim();
      if (field.startsWith("<<") && field.endsWith(">>")) field = field.slice(2, -2).trim();
      const label = field || "MCQ responses";
      return `<span class="preview-display-mcq">${esc(`<<Responses to ${label}>>`)}</span>`;
    },
  );
}

function replaceRecordCountTokens(html, opts) {
  return replaceMatchingSpans(
    html,
    (attrs) => matchFunctionId(attrs, "record-count"),
    (attrs) => {
      const config = parseFunctionConfigAttr(attrs);
      const n = countFormRecordsFromConfig(config, {
        records: opts.records ?? {},
        formName: opts.formName ?? "",
        blankAliases: opts.blankAliases ?? {},
      });
      return `<span class="preview-record-count">${esc(String(n))}</span>`;
    },
  );
}

function replaceItemizationTokens(html, opts) {
  const ctx = {
    records: opts.records ?? {},
    formName: opts.formName ?? "",
    blankAliases: opts.blankAliases ?? {},
  };
  return replaceMatchingSpans(html, matchItemizationToken, (attrs) => {
    const structured = parseStructuredNodeAttr(attrs);
    if (structured) {
      return renderItemizationTableHtml(
        {
          form: structured.form ?? "",
          columns: structured.columns ?? [],
          showPrint: structured.showPrint ?? structured["show-print-control"],
          showExport: structured.showExport ?? structured["show-export-control"],
          conditions: structured.conditions,
          combinator: structured.combinator,
        },
        ctx,
      );
    }
    const config = parseFunctionConfigAttr(attrs);
    const node = itemizationNodeFromConfig(config, attrs, ctx.formName);
    return renderItemizationTableHtml(node, ctx);
  });
}

/** Function display names that must not be treated as field refs. */
const FUNCTION_DISPLAY_NAME_RE =
  /^(MULTIPLE QUESTION LIST|ITEMIZATION|DISPLAY\s+IMAGE|DISPLAY\s+MULTIPLE|CHOICE\s+TALLY|QUESTION\s+CORRELATION|SIMPLE\s+LIST|FORM\s+RECORD\s+COUNT)\b/i;

const RICH_TEXT_HTML_TAG_RE = /<\/?[a-z][\s\S]*>/i;

export function looksLikeRichHtml(content) {
  return RICH_TEXT_HTML_TAG_RE.test(String(content ?? ""));
}

/**
 * @param {string} content
 * @param {(ref: string) => string} getField
 * @param {{ records?: Record<string, object[]>, formName?: string }} [opts]
 */
export function enhanceRichTextHtml(content, getField, opts = {}) {
  let html = String(content ?? "");
  html = replaceDisplayImageTokens(html);
  html = replaceDisplayMcqTokens(html);
  html = replaceRecordCountTokens(html, opts);
  // Replace function spans before <<...>> field substitution — nested <<field>> inside
  // function display strings otherwise leave scraps like `equals "Doug")>>`.
  html = replaceItemizationTokens(html, opts);
  const replaceTemplate = (_match, ref) => {
    const key = String(ref).trim();
    if (FUNCTION_DISPLAY_NAME_RE.test(key)) return "";
    if (/^Responses to /i.test(key)) return esc(key);
    return esc(getField(key) ?? "");
  };
  html = html
    .replace(/&lt;&lt;([\s\S]+?)&gt;&gt;/g, replaceTemplate)
    .replace(/<<([^>]+)>>/g, replaceTemplate);
  // Orphaned condition tails if a nested <<field>> broke an outer function display string.
  html = html.replace(/\b(?:equals|contains|does not equal|is blank|is not blank)\s+"[^"]*"\)?>>/gi, "");
  html = html.replace(/,\s*\)>>/g, "");
  return html;
}
