/**
 * Shared Form/Document Preview helpers — field substitution + DISPLAY IMAGE placeholders.
 * Kept separate so documentRenderer and runtime can both use it without circular imports.
 */

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function decodeHtmlAttr(value) {
  return String(value ?? "")
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'")
    .replace(/&apos;/g, "'")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&");
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
  const m = attrs.match(/\bdata-function-config\s*=\s*(["'])([\s\S]*?)\1/i);
  if (!m) return {};
  try {
    const parsed = JSON.parse(decodeHtmlAttr(m[2]));
    return parsed && typeof parsed === "object" ? parsed : {};
  } catch {
    return {};
  }
}

function replaceDisplayImageTokens(html) {
  return String(html).replace(
    /<span\b([^>]*\bdata-function-id\s*=\s*(["'])display-image\2[^>]*)>[\s\S]*?<\/span>/gi,
    (_match, attrs) => renderDisplayImagePlaceholder(parseFunctionConfigAttr(attrs)),
  );
}

function replaceDisplayMcqTokens(html) {
  return String(html).replace(
    /<span\b([^>]*\bdata-function-id\s*=\s*(["'])display-mcq-label\2[^>]*)>[\s\S]*?<\/span>/gi,
    (_match, attrs) => {
      const config = parseFunctionConfigAttr(attrs);
      let field = String(config["field-name"] ?? "").trim();
      if (field.startsWith("<<") && field.endsWith(">>")) field = field.slice(2, -2).trim();
      const label = field || "MCQ responses";
      return `<span class="preview-display-mcq">${esc(`<<Responses to ${label}>>`)}</span>`;
    },
  );
}

const RICH_TEXT_HTML_TAG_RE = /<\/?[a-z][\s\S]*>/i;

export function looksLikeRichHtml(content) {
  return RICH_TEXT_HTML_TAG_RE.test(String(content ?? ""));
}

/**
 * @param {string} content
 * @param {(ref: string) => string} getField
 */
export function enhanceRichTextHtml(content, getField) {
  let html = String(content ?? "");
  html = replaceDisplayImageTokens(html);
  html = replaceDisplayMcqTokens(html);
  const replaceTemplate = (_match, ref) => {
    const key = String(ref).trim();
    if (/^DISPLAY\s+IMAGE\b/i.test(key)) return "";
    if (/^DISPLAY\s+MULTIPLE/i.test(key)) return "";
    if (/^Responses to /i.test(key)) return esc(key);
    return esc(getField(key) ?? "");
  };
  return html
    .replace(/&lt;&lt;([\s\S]+?)&gt;&gt;/g, replaceTemplate)
    .replace(/<<([^>]+)>>/g, replaceTemplate);
}
