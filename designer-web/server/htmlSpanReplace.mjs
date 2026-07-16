/**
 * Replace `<span …>` trees whose opening-tag attributes match a predicate.
 * Attribute-aware (quoted values may contain `>` e.g. `<<FirstName>>` in JSON).
 */

/** Index of `>` that closes the opening tag starting at `start` (`<…`). */
export function findOpenTagEnd(html, start) {
  let quote = null;
  for (let i = start + 1; i < html.length; i++) {
    const c = html[i];
    if (quote) {
      if (c === quote) quote = null;
      continue;
    }
    if (c === '"' || c === "'") {
      quote = c;
      continue;
    }
    if (c === ">") return i;
  }
  return -1;
}

export function decodeHtmlAttr(value) {
  return String(value ?? "")
    .replace(/&quot;/g, '"')
    .replace(/&#39;/g, "'")
    .replace(/&apos;/g, "'")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&");
}

/** Read one attribute value from an opening-tag attribute string. */
export function readAttr(attrs, name) {
  const re = new RegExp(`\\b${name}\\s*=\\s*(["'])([\\s\\S]*?)\\1`, "i");
  const m = String(attrs).match(re);
  return m ? decodeHtmlAttr(m[2]) : "";
}

/**
 * @param {string} html
 * @param {(attrs: string) => boolean} matchAttrs
 * @param {(attrs: string, inner: string) => string} replacer
 */
export function replaceMatchingSpans(html, matchAttrs, replacer) {
  const src = String(html ?? "");
  let out = "";
  let cursor = 0;
  const openRe = /<span\b/gi;
  let m;
  while ((m = openRe.exec(src)) !== null) {
    const start = m.index;
    if (start < cursor) continue;
    const openEnd = findOpenTagEnd(src, start);
    if (openEnd < 0) break;
    const attrs = src.slice(start + "<span".length, openEnd);
    if (!matchAttrs(attrs)) continue;
    const close = src.indexOf("</span>", openEnd + 1);
    if (close < 0) break;
    const inner = src.slice(openEnd + 1, close);
    out += src.slice(cursor, start);
    out += replacer(attrs, inner);
    cursor = close + "</span>".length;
    openRe.lastIndex = cursor;
  }
  out += src.slice(cursor);
  return out;
}
