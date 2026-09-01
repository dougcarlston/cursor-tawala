/**
 * Document WYSIWYG HTML → legacy `<xmlData>` XML (deploy path).
 * Handles paragraphs, tables, field/function tokens, and basic inline formatting.
 */

import { conditionOperandXml } from "./conditionOperandXml.mjs";
import { normalizeHyperlinkUrl } from "./hyperlinkUrl.mjs";
import { collectProjectVariableNames } from "./projectVariables.mjs";

/** Empty paragraph used as a Deploy spacer (matches Form Text / response-totals habit). */
const BLANK_PARAGRAPH_XML =
  `<paragraph indent="0" align="left"><tabPositions><tabStop position="2880"/></tabPositions></paragraph>`;

/**
 * CSS length → twips for `<cell width="…">`.
 * Design stores `pt`; Open/.tawala import often emits `px` via twips/15 — treat px as that invert.
 */
function cssLengthToTwips(raw) {
  const s = String(raw ?? "").trim();
  if (!s) return 0;
  const m = s.match(/^([0-9.]+)\s*(pt|px|in)?$/i);
  if (!m) return 0;
  const n = Number.parseFloat(m[1]);
  if (!Number.isFinite(n)) return 0;
  const unit = (m[2] || "pt").toLowerCase();
  if (unit === "px") return Math.round(n * 15);
  if (unit === "in") return Math.round(n * 1440);
  return Math.round(n * 20);
}

/** @deprecated use cssLengthToTwips — kept name for call sites that pass bare pt numbers */
function ptToTwips(pt) {
  return cssLengthToTwips(pt);
}

function parseStyleAttr(tagHtml) {
  const m = tagHtml.match(/\bstyle="([^"]*)"/i);
  if (!m) return {};
  const out = {};
  for (const part of m[1].split(";")) {
    const [k, v] = part.split(":").map((s) => s.trim());
    if (k && v) out[k.toLowerCase()] = v;
  }
  return out;
}

function parseClassAttr(tagHtml) {
  // Accept single/double quotes and optional spaces around `=` — strict
  // `class="…"` misses chips some serializers emit, and Deploy then turns the
  // visible `<<MULTIPLE QUESTION LIST(...)>>` label into a junk `<field>` (`<>`).
  const m = String(tagHtml ?? "").match(/\bclass\s*=\s*(["'])([\s\S]*?)\1/i);
  return m ? m[2].split(/\s+/).filter(Boolean) : [];
}

/** Form Text / Document local image → `<image id width height/>` (not display-image). */
function embeddedImageToXml(attrs, escAttr) {
  const idM = attrs.match(/data-tawala-image-id="([^"]*)"/i);
  if (!idM?.[1]) return "";
  const id = idM[1];
  const width =
    attrs.match(/data-image-width="([^"]*)"/i)?.[1] ??
    attrs.match(/\bwidth="([^"]*)"/i)?.[1] ??
    "0";
  const height =
    attrs.match(/data-image-height="([^"]*)"/i)?.[1] ??
    attrs.match(/\bheight="([^"]*)"/i)?.[1] ??
    "0";
  // Legacy Form Text wraps embeds in <font> (Get Together / DirtBowl samples).
  return (
    `<font face="Arial" size="200" color="000000">` +
    `<image id="${escAttr(id)}" width="${escAttr(width)}" height="${escAttr(height)}"></image>` +
    `</font>`
  );
}

/** Normalize CSS/legacy align to values Java Div emits as text-align. */
function normalizeAlign(raw) {
  const a = String(raw ?? "")
    .trim()
    .toLowerCase();
  if (a === "left" || a === "center" || a === "right" || a === "justify") return a;
  return null;
}

function parseAlignFromTag(tagHtml) {
  const style = parseStyleAttr(tagHtml);
  return normalizeAlign(style["text-align"]) ?? "left";
}

/**
 * Design stores table alignment on the cell (`td`/`th` style) and/or nested
 * `<p>`/`<div>` (Signup Sheet contact labels). Legacy Deploy puts it on
 * `<division align="…">` — Java Div → inline `text-align`. Never nest
 * `<paragraph>` in cells (Column factory only accepts division/font/…).
 */
function parseTableCellAlign(openTagAttrs, cellInnerHtml) {
  const fromCell = normalizeAlign(
    parseStyleAttr(`<x ${openTagAttrs}>`)["text-align"],
  );
  if (fromCell) return fromCell;

  const nestedRe = /<(?:p|div)\b([^>]*)>/gi;
  let m;
  while ((m = nestedRe.exec(String(cellInnerHtml ?? "")))) {
    const fromNested = normalizeAlign(
      parseStyleAttr(`<x ${m[1]}>`)["text-align"],
    );
    if (fromNested) return fromNested;
  }
  return "left";
}

function stripTags(html) {
  return String(html ?? "")
    .replace(/<br\s*\/?>/gi, "\n")
    .replace(/<[^>]+>/g, "")
    .replace(/\u200B/g, "")
    .replace(/&nbsp;/gi, " ")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&");
}

/**
 * Legacy Form Text / Document fields use `Form:Field` (e.g. `Potluck Organizer:attendeeName`).
 * Design may store bare `<<attendeeName>>` from the Fields palette — qualify on Deploy.
 * Project/admin vars (DirtBowl `AdminAdrss`, `League`, fee) must stay bare — they are
 * Set by Process, not Registration FIB blanks. Always-qualifying emptied T4 cells.
 */
function collectFormFieldNames(form) {
  const names = new Set();
  if (!form) return names;
  for (const item of form.items ?? []) {
    if (item.type === "fib") {
      for (const b of item.blanks ?? []) {
        const alt = String(b.alternateLabel ?? "").trim();
        const nm = String(b.name ?? "").trim();
        if (alt) names.add(alt);
        if (nm) names.add(nm);
        const label = String(item.label ?? "").trim();
        if (label && nm) names.add(`${label}:${nm}`);
      }
    } else if (item.type === "mc") {
      for (const key of [item.alternateLabel, item.label, item.name]) {
        const s = String(key ?? "").trim();
        if (s) names.add(s);
      }
    } else if (item.type === "field") {
      for (const key of [item.fieldName, item.name]) {
        const s = String(key ?? "").trim();
        if (s) names.add(s);
      }
    }
  }
  return names;
}

/**
 * Qualify a Document/Form Text `<<field>>` for Deploy.
 * FIB blanks like `FIB1:a` already contain `:` but still need `Form:FIB1:a` —
 * only skip when already prefixed with this form or `Record:`.
 * When `opts.formFieldNames` is provided, bare names not on the form stay unqualified.
 */
function qualifyFieldRef(name, formName, opts = {}) {
  const s = String(name ?? "").trim();
  const form = String(formName ?? "").trim();
  if (!s || !form) return s;
  if (/^Record:/i.test(s)) return s;
  if (s === form || s.startsWith(`${form}:`)) return s;

  const known = opts.formFieldNames;
  if (known instanceof Set) {
    if (known.has(s)) return `${form}:${s}`;
    // `FIB1:a` style already on the form's blank list
    return s;
  }
  // No form inventory (Document / older callers): always qualify (Potluck).
  return `${form}:${s}`;
}

/**
 * Plain text that may include `<<Field>>` (or entity-decoded from `&lt;&lt;Field&gt;&gt;`)
 * → mix of escaped text and `<field name="…"/>` for Deploy.
 */
/** True when `<<…>>` is a function chip label, not a Form field ref. */
function looksLikeFunctionDisplayToken(inner) {
  const s = String(inner ?? "").trim();
  if (!s) return false;
  // `<<MULTIPLE QUESTION LIST(false, false, ...)>>` / `<<QUESTION LIST(...)>>` / `<<SUM(Amount)>>` / `<<RECORD COUNT>>`
  if (/^(?:MULTIPLE\s+)?QUESTION\s+LIST\b/i.test(s)) return true;
  if (/^(?:SINGLE\s+)?(?:QUESTION\s+LIST|SIMPLE\s+LIST)\b/i.test(s)) return true;
  if (/^[A-Z][A-Z0-9 ]*(?:\(|$)/.test(s)) return true;
  return false;
}

function parseOrphanFunctionToXml(match, escAttr, escText, opts = {}) {
  const mqlMatch = /(?:<<\s*)?(?:MULTIPLE\s+)?QUESTION\s+LIST\s*\(([\s\S]*?)\)(?:\s*>>)?/i.exec(match);
  if (mqlMatch) {
    const argsStr = mqlMatch[1];
    const args = [];
    let cur = "";
    let inAngle = 0;
    let inQuote = false;
    for (let i = 0; i < argsStr.length; i++) {
      const ch = argsStr[i];
      if (ch === "\"" || ch === "'") inQuote = !inQuote;
      else if (!inQuote && ch === "<" && argsStr[i + 1] === "<") {
        inAngle++;
        i++;
        cur += "<<";
        continue;
      } else if (!inQuote && ch === ">" && argsStr[i + 1] === ">") {
        inAngle = Math.max(0, inAngle - 1);
        i++;
        cur += ">>";
        continue;
      } else if (ch === "," && inAngle === 0 && !inQuote) {
        args.push(cur.trim());
        cur = "";
        continue;
      }
      cur += ch;
    }
    if (cur.trim()) args.push(cur.trim());

    let showPrint = "false";
    let showExport = "false";
    let argIdx = 0;
    if (args[argIdx] === "true" || args[argIdx] === "false") {
      showPrint = args[argIdx++];
    }
    if (args[argIdx] === "true" || args[argIdx] === "false") {
      showExport = args[argIdx++];
    }
    let numCols = 0;
    if (/^\d+$/.test(args[argIdx])) {
      numCols = parseInt(args[argIdx++], 10);
    }
    const fieldArgs = [];
    const conditionArgs = [];
    for (; argIdx < args.length; argIdx++) {
      const a = args[argIdx];
      if (/\b(?:equals|contains|does not equal|is blank|is not blank)\b/i.test(a)) {
        conditionArgs.push(a);
      } else {
        fieldArgs.push(a);
      }
    }
    let inferredForm = opts.formName ?? "";
    const cols = fieldArgs.map((f) => {
      let clean = f.replace(/^<<|>>$/g, "").trim();
      let header = clean;
      if (clean.includes(":")) {
        const parts = clean.split(":");
        if (!inferredForm && parts.length > 1) inferredForm = parts[0].trim();
        header = parts[parts.length - 1].trim();
      }
      let ref = clean;
      if (!/^Record:/i.test(ref) && ref.includes(":")) {
        ref = `Record:${ref}`;
      }
      return { header, field: ref };
    });
    if (cols.length > 0) {
      const n = numCols || cols.length;
      const colXml = cols
        .slice(0, n)
        .map(
          (c) =>
            `<column><header><string value="${escText(c.header)}"/></header><contents><field name="${escAttr(c.field)}"/></contents></column>`,
        )
        .join("");
      const condXml = inferredForm
        ? `<conditions><form name="${escAttr(inferredForm)}"/></conditions>`
        : "<conditions/>";
      return `<font><itemization-table version="2"><show-print-control>${showPrint}</show-print-control><show-export-control>${showExport}</show-export-control><number-of-columns>${n}</number-of-columns>${colXml}${condXml}</itemization-table></font>`;
    }
    console.warn(
      "[documentHtmlToXml] Orphaned MULTIPLE/QUESTION LIST label skipped (would Deploy as junk field <>). Prefer a function-token with data-function-id=itemization-table.",
    );
    return "";
  }

  const simpleListMatch = /(?:<<\s*)?(?:SINGLE\s+)?(?:QUESTION\s+LIST|SIMPLE\s+LIST)\s*\(([\s\S]*?)\)(?:\s*>>)?/i.exec(match);
  if (simpleListMatch) {
    let rawField = simpleListMatch[1].replace(/^<<|>>$/g, "").trim();
    if (!rawField) return "";
    if (!/^Record:/i.test(rawField) && rawField.includes(":")) {
      rawField = `Record:${rawField}`;
    }
    return `<font><simple-list version="2"><simple-list-field>${escText(rawField)}</simple-list-field><conditions/></simple-list></font>`;
  }

  const recordCountMatch = /(?:<<\s*)?(?:FORM\s+)?RECORD\s+COUNT\s*\(([\s\S]*?)\)(?:\s*>>)?/i.exec(match);
  if (recordCountMatch) {
    const form = recordCountMatch[1].replace(/^<<|>>$/g, "").trim();
    return `<font><record-count version="3"><form-name>${escText(form)}</form-name><conditions/></record-count></font>`;
  }

  return "";
}

function processStandardFieldTokens(str, escAttr, escText, opts = {}) {
  return str
    .split(/(<<[^<>]+>>)/g)
    .map((part) => {
      const m = /^<<\s*([^<>]+?)\s*>>$/.exec(part);
      if (m) {
        const inner = m[1].trim();
        // Orphaned chip labels (span lost / attrs not recognized) must not become
        // `<field name="Form:MULTIPLE QUESTION LIST(...)"/>` → runtime `<>`.
        if (looksLikeFunctionDisplayToken(inner)) {
          if (/(?:MULTIPLE\s+)?QUESTION\s+LIST/i.test(inner)) {
            console.warn(
              "[documentHtmlToXml] Orphaned MULTIPLE QUESTION LIST label skipped (would Deploy as junk field <>). Prefer a function-token with data-function-id=itemization-table.",
            );
          }
          return "";
        }
        const ref = qualifyFieldRef(inner, opts.formName, opts);
        return `<field name="${escAttr(ref)}"/>`;
      }
      return escText(part);
    })
    .join("");
}

function textWithFieldTokensToXml(plain, escAttr, escText, opts = {}) {
  const s = String(plain ?? "");
  if (!s) return "";

  // Extract and handle whole function call expressions (which may contain nested <<...>>)
  // so they are not broken into fragments by <<[^<>]+>>.
  const fnRegex = /(?:<<\s*)?(?:(?:MULTIPLE\s+)?QUESTION\s+LIST|(?:SINGLE\s+)?(?:QUESTION\s+LIST|SIMPLE\s+LIST)|(?:FORM\s+)?RECORD\s+COUNT|SUM|MAX|MIN|CHOICE\s+TALLY|RESPONSE\s+TOTALS|QUESTION\s+CORRELATION|POPULAR\s+CHOICE)\s*\((?:[^()]*|\([^()]*\))*\)(?:\s*>>)?/gi;

  let out = "";
  let lastIdx = 0;
  let match;

  while ((match = fnRegex.exec(s)) !== null) {
    const before = s.slice(lastIdx, match.index);
    if (before) {
      out += processStandardFieldTokens(before, escAttr, escText, opts);
    }
    const fnXml = parseOrphanFunctionToXml(match[0], escAttr, escText, opts);
    out += fnXml;
    lastIdx = fnRegex.lastIndex;
  }

  const remaining = s.slice(lastIdx);
  if (remaining) {
    out += processStandardFieldTokens(remaining, escAttr, escText, opts);
  }

  return out;
}

function extractTagInner(html, tagName) {
  const re = new RegExp(`^<${tagName}\\b[^>]*>([\\s\\S]*)<\\/${tagName}>`, "i");
  const m = html.trim().match(re);
  return m ? m[1] : html;
}

function extractOpenTag(html) {
  const trimmed = html.trim();
  const m = trimmed.match(/^<([a-z0-9]+)\b/i);
  if (!m) return null;
  // Do not use [^>]* — function configs often contain `<<Form:Field>>` with raw `>`.
  let i = m[0].length;
  let quote = null;
  while (i < trimmed.length) {
    const ch = trimmed[i];
    if (quote) {
      if (ch === quote) quote = null;
      i++;
      continue;
    }
    if (ch === '"' || ch === "'") {
      quote = ch;
      i++;
      continue;
    }
    if (ch === ">") {
      const full = trimmed.slice(0, i + 1);
      return {
        name: m[1].toLowerCase(),
        attrs: full.slice(m[0].length, -1),
        full,
      };
    }
    i++;
  }
  return null;
}

function nextTopLevelBlock(html) {
  const trimmed = html.trim();
  if (!trimmed) return null;
  // Prefer real open-tag + matching close (nested <p> inside doc-placed-text is common).
  // A non-greedy `[\s\S]*?</p>` wrongly truncates at the first inner </p>, leaving
  // later lines (e.g. "Contact information:") as unplaced blocks that sort *after*
  // an absolutely-positioned table on Deploy.
  const open = extractOpenTag(trimmed);
  if (open && /^(table|p|div)$/i.test(open.name)) {
    const matched = sliceMatchingElement(trimmed, open);
    if (matched) return { html: matched.block, rest: matched.rest };
    // Unclosed table/p/div — consume the open tag only so we do not loop forever.
    return { html: open.full, rest: trimmed.slice(open.full.length) };
  }
  const textEnd = trimmed.search(/<(?:table|p|div)\b/i);
  if (textEnd > 0) {
    return { html: trimmed.slice(0, textEnd), rest: trimmed.slice(textEnd) };
  }
  // Inline-only fragment (e.g. bare function-token <span>…) — one block.
  return { html: trimmed, rest: "" };
}

function fontSizeToLegacy(sizePt) {
  const pt = Number.parseFloat(sizePt);
  if (!Number.isFinite(pt)) return 200;
  return Math.round(pt * 20);
}

/**
 * CSS color → 6-digit hex without `#` for Java `Font` (`Integer.parseInt(..., 16)`).
 * Browsers often serialize palette colors as `rgb(0, 0, 0)`; emitting that rejects Deploy.
 * Exported for unit tests via documentHtmlToXml behavior.
 */
function cssColorToLegacyHex(raw) {
  const s = String(raw ?? "").trim();
  if (!s) return "000000";
  const hex = /^#?([0-9a-f]{3}|[0-9a-f]{6})$/i.exec(s);
  if (hex) {
    const h = hex[1];
    if (h.length === 3) {
      return (h[0] + h[0] + h[1] + h[1] + h[2] + h[2]).toUpperCase();
    }
    return h.toUpperCase();
  }
  const rgb = /^rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/i.exec(s);
  if (rgb) {
    const to = (n) =>
      Math.max(0, Math.min(255, Number.parseInt(n, 10)))
        .toString(16)
        .padStart(2, "0");
    return (to(rgb[1]) + to(rgb[2]) + to(rgb[3])).toUpperCase();
  }
  // Unknown (named colors, etc.) — omit-safe default black rather than crash Java.
  return "000000";
}

/**
 * Find the matching close tag for `open`, respecting nesting.
 * Returns { block, inner, rest } or null if unclosed.
 */
function sliceMatchingElement(html, open) {
  const name = open.name;
  const openRe = new RegExp(`<${name}\\b`, "i");
  const closeRe = new RegExp(`</${name}\\s*>`, "i");

  // Consume the opening tag we already parsed.
  let i = open.full.length;
  let depth = 1;

  while (i < html.length && depth > 0) {
    const slice = html.slice(i);
    const nextOpen = slice.search(openRe);
    const nextClose = slice.search(closeRe);
    if (nextClose < 0) return null;

    if (nextOpen >= 0 && nextOpen < nextClose) {
      const absOpen = i + nextOpen;
      const tag = extractOpenTag(html.slice(absOpen));
      if (!tag) {
        i = absOpen + 1;
        continue;
      }
      const selfClosing =
        /\/\s*>$/.test(tag.full) || /^(br|hr|img|sp)$/i.test(tag.name);
      if (!selfClosing) depth++;
      i = absOpen + tag.full.length;
      continue;
    }

    const absClose = i + nextClose;
    const closeMatch = html.slice(absClose).match(closeRe);
    if (!closeMatch) return null;
    depth--;
    i = absClose + closeMatch[0].length;
    if (depth === 0) {
      const block = html.slice(0, i);
      return {
        block,
        inner: extractTagInner(block, name),
        rest: html.slice(i),
      };
    }
  }
  return null;
}

/** Unwrap a single leading/trailing bare `<font>…</font>` (no attributes). */
function unwrapBareFont(xml) {
  return String(xml ?? "").replace(/^<font>([\s\S]*)<\/font>$/i, "$1");
}

/**
 * Unwrap one outer `<font…>…</font>` that spans the whole string (attrs OK).
 * Used so a styled `<span>` does not nest another `<font>` around
 * `embeddedImageToXml`’s already-wrapped `<image>` (breaks Deploy XML).
 */
export function unwrapOuterFont(xml) {
  const s = String(xml ?? "").trim();
  const openMatch = s.match(/^<font\b[^>]*>/i);
  if (!openMatch) return String(xml ?? "");
  let depth = 0;
  const re = /<\/?font\b[^>]*>/gi;
  let m;
  while ((m = re.exec(s))) {
    if (/^<\/font/i.test(m[0])) {
      depth -= 1;
      if (depth === 0) {
        if (m.index + m[0].length === s.length) {
          return s.slice(openMatch[0].length, m.index);
        }
        return String(xml ?? "");
      }
    } else {
      depth += 1;
    }
  }
  return String(xml ?? "");
}

/**
 * Legacy Java Font FACTORY cannot nest `<font>` — inner display components are dropped.
 * Function tokens already emit a single `<font><itemization-table|…></font>`.
 */
const FONT_WRAPPED_DISPLAY_COMPONENT_RE =
  /^<font(?:\s[^>]*)?>\s*<(itemization-table|display-mcq-label|record-count|sum|max|min|choice-tally-table|response-totals-table|question-correlation-table|popular-choice-(?:display|count|correlation-table)|simple-list|display-image|project-email-count)\b/i;

function isFontWrappedDisplayComponent(xml) {
  return FONT_WRAPPED_DISPLAY_COMPONENT_RE.test(String(xml ?? "").trim());
}

/**
 * Wrap with `<b>` / `<i>` / `<u>` without putting those tags *outside* `<font>`.
 *
 * Legacy Java TextFormattingContainerElement (Bold/Italics/Underline) does **not**
 * register `font`. Structure `<b><u><font>…<invitation/></font></u></b>` logs
 * "No class registered for font" and drops the invitation → Deploy shows `, .`
 * (Sign-up Sheet ViewFinalList). Legacy order is `<font>…<b><u><invitation/>`.
 */
function wrapInlineFormat(tag, xml) {
  const s = String(xml ?? "");
  const trimmed = s.trim();
  const openMatch = trimmed.match(/^<font\b[^>]*>/i);
  if (!openMatch) return `<${tag}>${s}</${tag}>`;
  let depth = 0;
  const re = /<\/?font\b[^>]*>/gi;
  let m;
  while ((m = re.exec(trimmed))) {
    if (/^<\/font/i.test(m[0])) {
      depth -= 1;
      if (depth === 0) {
        if (m.index + m[0].length !== trimmed.length) {
          return `<${tag}>${s}</${tag}>`;
        }
        const open = openMatch[0];
        const body = trimmed.slice(open.length, m.index);
        const already = new RegExp(`^<${tag}>([\\s\\S]*)<\\/${tag}>$`, "i");
        if (already.test(body.trim())) {
          return `${open}${body}</font>`;
        }
        return `${open}<${tag}>${body}</${tag}></font>`;
      }
    } else {
      depth += 1;
    }
  }
  return `<${tag}>${s}</${tag}>`;
}

function inlineHtmlToXml(html, escAttr, escText, opts = {}) {
  if (!html) return "";
  let out = "";
  let rest = html;
  while (rest.length) {
    // Leading plain text / ZWSP before the next tag (common after function insert).
    const tagIdx = rest.search(/<[a-z][a-z0-9]*\b/i);
    if (tagIdx < 0) {
      const plain = stripTags(rest);
      if (plain) out += textWithFieldTokensToXml(plain, escAttr, escText, opts);
      break;
    }
    if (tagIdx > 0) {
      const plain = stripTags(rest.slice(0, tagIdx));
      if (plain) out += textWithFieldTokensToXml(plain, escAttr, escText, opts);
      rest = rest.slice(tagIdx);
    }

    const open = extractOpenTag(rest);
    if (!open) {
      const plain = stripTags(rest);
      if (plain) out += textWithFieldTokensToXml(plain, escAttr, escText, opts);
      break;
    }

    if (/^(br|hr|img|sp)$/i.test(open.name)) {
      if (open.name.toLowerCase() === "br") out += "\n";
      else if (open.name.toLowerCase() === "img") {
        out += embeddedImageToXml(open.attrs, escAttr);
      }
      rest = rest.slice(open.full.length);
      continue;
    }

    const matched = sliceMatchingElement(rest, open);
    if (!matched) {
      const plain = stripTags(rest);
      if (plain) out += textWithFieldTokensToXml(plain, escAttr, escText, opts);
      break;
    }
    const { inner } = matched;
    rest = matched.rest;

    if (open.name === "span") {
      const classes = parseClassAttr(open.attrs);
      if (classes.includes("field-token")) {
        const name =
          readAttrValue(open.attrs, "data-field-name") ||
          stripTags(inner).replace(/^<<|>>$/g, "");
        if (name) {
          const ref = qualifyFieldRef(name, opts.formName, opts);
          out += `<field name="${escAttr(ref)}"/>`;
        }
        continue;
      }
      // Prefer class, but also accept `data-function-id` alone — class= parsing used
      // to require double quotes with no spaces, so valid chips became plain text.
      const functionId = readAttrValue(open.attrs, "data-function-id");
      if (classes.includes("function-token") || functionId) {
        // Legacy wraps display components in <font> inside paragraphs.
        out += `<font>${functionTokenToXml(open.attrs, escAttr, escText)}</font>`;
        continue;
      }
      // Legacy `{ MULTIPLE QUESTION LIST }` brace chips (converter / old projects):
      // function-table-token + data-tawala-structured-node, no data-function-id.
      // When a modern function-token MQL is also present, drop the brace chip entirely
      // (owner dual-chip: Redeploy must not keep the unfiltered old table).
      const looksLikeLegacyMql =
        classes.includes("function-table-token") ||
        /data-itemization-token\s*=\s*["']true["']/i.test(open.attrs) ||
        /data-tawala-structured-node\s*=/i.test(open.attrs);
      if (looksLikeLegacyMql && !classes.includes("function-token")) {
        if (opts.hasModernMqlToken) {
          continue;
        }
        const structuredXml = structuredItemizationTokenToXml(open.attrs, escAttr, escText);
        if (structuredXml) {
          out += `<font>${structuredXml}</font>`;
          continue;
        }
      }
      if (classes.includes("invitation-token")) {
        // Prefer a single <font> layer. Nested <font> is fatal on Java: Font FACTORY
        // has no "font" child → "No class registered for font" and the link is dropped
        // (CYO Dashboard invitation grid). Outer colored parents strip inner fonts below;
        // when this token is alone, apply the default blue underline wrap here.
        let inv = invitationTokenToXml(open.attrs, inner, escAttr, escText, opts);
        const invStyle = parseStyleAttr(open.attrs);
        const color = invStyle.color
          ? cssColorToLegacyHex(invStyle.color)
          : "000080";
        const face = invStyle["font-family"]?.split(",")[0]?.replace(/['"]/g, "") ?? "";
        const size = invStyle["font-size"]
          ? fontSizeToLegacy(invStyle["font-size"])
          : null;
        inv =
          `<font${face ? ` face="${escAttr(face)}"` : ""}${size != null ? ` size="${size}"` : ""} color="${escAttr(color)}">` +
          `<u>${inv}</u></font>`;
        out += inv;
        continue;
      }
      if (classes.includes("hyperlink-token")) {
        let link = hyperlinkTokenToXml(open.attrs, escAttr, escText);
        const linkStyle = parseStyleAttr(open.attrs);
        const color = linkStyle.color
          ? cssColorToLegacyHex(linkStyle.color)
          : "000080";
        const face = linkStyle["font-family"]?.split(",")[0]?.replace(/['"]/g, "") ?? "";
        const size = linkStyle["font-size"]
          ? fontSizeToLegacy(linkStyle["font-size"])
          : null;
        link =
          `<font${face ? ` face="${escAttr(face)}"` : ""}${size != null ? ` size="${size}"` : ""} color="${escAttr(color)}">` +
          `<u>${link}</u></font>`;
        out += link;
        continue;
      }
      const style = parseStyleAttr(open.attrs);
      let innerXml = inlineHtmlToXml(inner, escAttr, escText, opts);
      if (isFontWrappedDisplayComponent(innerXml)) {
        out += innerXml;
        continue;
      }
      if (style["font-size"] || style["font-family"] || style.color) {
        const face = style["font-family"]?.split(",")[0]?.replace(/['"]/g, "") ?? "";
        const size = style["font-size"] ? fontSizeToLegacy(style["font-size"]) : 200;
        const color = cssColorToLegacyHex(style.color);
        // Avoid nested <font> — Java Font FACTORY does not register "font" (drops children),
        // and Style post-process non-greedy </font> matching corrupts nested wraps.
        // Strip all font tags (not just outer) so invitation/link default wraps from
        // token spans or inner colored spans cannot nest under this wrap.
        let unwrapped = unwrapOuterFont(unwrapBareFont(innerXml));
        unwrapped = unwrapped.replace(/<\/?font\b[^>]*>/gi, "");
        // Collapse stacked <u> from outer HTML underline + token defaults
        while (/^<u>[\s\S]*<\/u>$/i.test(unwrapped.trim())) {
          const next = unwrapped.trim().replace(/^<u>([\s\S]*)<\/u>$/i, "$1");
          if (next === unwrapped.trim()) break;
          unwrapped = next;
        }
        const hasLinkMarkup = /<(?:invitation|link)\b/i.test(unwrapped);
        const body = hasLinkMarkup ? `<u>${unwrapped}</u>` : unwrapped;
        innerXml =
          `<font${face ? ` face="${escAttr(face)}"` : ""} size="${size}" color="${escAttr(color)}">` +
          `${body}</font>`;
      }
      // Palette styleWithCSS leaves B/I/U on span style — map like <b>/<i>/<u> tags.
      const weight = String(style["font-weight"] ?? "").toLowerCase();
      if (
        weight === "bold" ||
        weight === "bolder" ||
        (/^\d+$/.test(weight) && Number(weight) >= 600)
      ) {
        innerXml = wrapInlineFormat("b", innerXml);
      }
      if (String(style["font-style"] ?? "").toLowerCase() === "italic") {
        innerXml = wrapInlineFormat("i", innerXml);
      }
      if (/\bunderline\b/i.test(String(style["text-decoration"] ?? ""))) {
        innerXml = wrapInlineFormat("u", innerXml);
      }
      out += innerXml;
      continue;
    }

    if (open.name === "strong" || open.name === "b") {
      out += wrapInlineFormat("b", inlineHtmlToXml(inner, escAttr, escText, opts));
      continue;
    }
    if (open.name === "em" || open.name === "i") {
      out += wrapInlineFormat("i", inlineHtmlToXml(inner, escAttr, escText, opts));
      continue;
    }
    if (open.name === "u") {
      out += wrapInlineFormat("u", inlineHtmlToXml(inner, escAttr, escText, opts));
      continue;
    }
    if (open.name === "font") {
      const innerXml = inlineHtmlToXml(inner, escAttr, escText, opts);
      // Flatten nested bare <font> so web components are not dropped by Java.
      out += isFontWrappedDisplayComponent(innerXml)
        ? innerXml
        : `<font>${unwrapBareFont(innerXml)}</font>`;
      continue;
    }

    out += inlineHtmlToXml(inner, escAttr, escText, opts);
  }
  return out;
}

/** Decode HTML entities commonly used in contenteditable attribute serialization. */
function decodeHtmlAttrEntities(s) {
  // Decode &amp; first so double-encoded values (&amp;quot;) become usable JSON.
  let out = String(s ?? "");
  for (let i = 0; i < 4; i++) {
    const next = out
      .replace(/&amp;/gi, "&")
      .replace(/&quot;/gi, '"')
      .replace(/&#39;/g, "'")
      .replace(/&apos;/gi, "'")
      .replace(/&lt;/gi, "<")
      .replace(/&gt;/gi, ">");
    if (next === out) break;
    out = next;
  }
  return out;
}

/**
 * Parse `data-function-config` from a tag's attribute string.
 * Handles browser entity-encoded values and raw JSON with embedded quotes
 * (naive `="([^"]*)"` truncates at the first quote inside JSON).
 */
function parseFunctionConfigAttr(attrs) {
  const key = /data-function-config\s*=/i.exec(attrs);
  if (!key) return {};
  const afterEq = attrs.slice(key.index + key[0].length).trimStart();
  if (!afterEq) return {};

  // Entity-encoded attribute: data-function-config="{&quot;field-name&quot;:…}"
  const quoted = /^"([^"]*)"/.exec(afterEq);
  if (quoted) {
    try {
      const parsed = JSON.parse(decodeHtmlAttrEntities(quoted[1]));
      return parsed && typeof parsed === "object" ? parsed : {};
    } catch {
      /* fall through to brace scan */
    }
  }

  // Raw / partially broken: find `{…}` and JSON.parse brace-balanced slice.
  const brace = afterEq.indexOf("{");
  if (brace < 0) return {};
  const decoded = decodeHtmlAttrEntities(afterEq.slice(brace));
  let depth = 0;
  let inStr = false;
  let esc = false;
  for (let i = 0; i < decoded.length; i++) {
    const ch = decoded[i];
    if (inStr) {
      if (esc) esc = false;
      else if (ch === "\\") esc = true;
      else if (ch === '"') inStr = false;
      continue;
    }
    if (ch === '"') {
      inStr = true;
      continue;
    }
    if (ch === "{") depth++;
    else if (ch === "}") {
      depth--;
      if (depth === 0) {
        try {
          const parsed = JSON.parse(decoded.slice(0, i + 1));
          return parsed && typeof parsed === "object" ? parsed : {};
        } catch {
          return {};
        }
      }
    }
  }
  return {};
}

/**
 * Private-invitation auth token for `<authenticationTokenValue>`.
 * Legacy uses `<string field="Recipient"/>` for field refs and `<string value="…"/>` for literals
 * (e.g. Sign-up `fromSignupSheet`). Bare names from Insert Link / .tawala convert (no `<<>>`)
 * resolve against project variables when `knownVariableNames` is provided.
 */
function authTokenExpressionToXml(raw, escAttr, knownVariableNames) {
  let s = String(raw ?? "").trim();
  if (!s) return `<string value=""/>`;
  if (s.startsWith("<<") && s.endsWith(">>")) {
    return conditionOperandXml(s, escAttr);
  }
  if (s.includes(":") && !/\s/.test(s)) {
    return `<string field="${escAttr(s)}"/>`;
  }
  if (knownVariableNames instanceof Set && knownVariableNames.has(s)) {
    return `<string field="${escAttr(s)}"/>`;
  }
  return `<string value="${escAttr(s)}"/>`;
}

/** `<<Form:Field>>` / `Form:Field` → field ref; else literal string value. */
function expressionToXml(raw, escAttr) {
  let s = String(raw ?? "").trim();
  if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
  if (!s) return `<string value=""/>`;
  // Keep real URL schemes as strings. Form:Field refs also contain ":" but are not schemes.
  if (/^(https?|mailto|ftp|file|tel):/i.test(s) || /^www\./i.test(s)) {
    return `<string value="${escAttr(s)}"/>`;
  }
  if (s.includes(":") && !s.includes(" ")) {
    return `<field name="${escAttr(s)}"/>`;
  }
  return `<string value="${escAttr(s)}"/>`;
}

function parseJsonConfigAttr(attrs, keyName) {
  const re = new RegExp(`${keyName}\\s*=`, "i");
  const key = re.exec(attrs);
  if (!key) return {};
  const afterEq = attrs.slice(key.index + key[0].length).trimStart();
  if (!afterEq) return {};
  const quoted = /^"([^"]*)"/.exec(afterEq);
  if (quoted) {
    try {
      const parsed = JSON.parse(decodeHtmlAttrEntities(quoted[1]));
      return parsed && typeof parsed === "object" ? parsed : {};
    } catch {
      /* fall through */
    }
  }
  return parseFunctionConfigAttr(`${keyName}=${afterEq}`);
}

/**
 * Recover nested invitation chips from a bad convert/edit (Sign-up Sheet ViewFinalList):
 * outer private chip had displayText=auth literal + empty authToken; inner chip held "click here".
 */
function recoverNestedInvitationConfig(config, innerHtml) {
  if (!/invitation-token/i.test(innerHtml)) return config;
  const nestedAttr = /data-invitation-config\s*=\s*"([^"]*)"/i.exec(innerHtml);
  if (!nestedAttr) return config;
  let nested;
  try {
    nested = JSON.parse(decodeHtmlAttrEntities(nestedAttr[1]));
  } catch {
    return config;
  }
  if (!nested || typeof nested !== "object") return config;
  const outerAuth = String(config.authToken ?? "").trim();
  const outerDisplay = String(config.displayText ?? "").trim();
  const nestedDisplay = String(nested.displayText ?? "").trim();
  const isPrivate = config.isPrivate === true || config.isPrivate === "true";
  // Mis-parked auth literal in outer displayText (converter bug before auth/label split).
  if (isPrivate && !outerAuth && outerDisplay && nestedDisplay && outerDisplay !== nestedDisplay) {
    return {
      ...config,
      authToken: outerDisplay,
      displayText: nestedDisplay,
      form: String(config.form ?? nested.form ?? "").trim() || config.form,
    };
  }
  // Generic nest: keep outer form/auth/private; visible label from inner text.
  const visible = stripTags(innerHtml).trim();
  if (visible && visible !== outerDisplay) {
    return { ...config, displayText: visible };
  }
  return config;
}

function invitationTokenToXml(attrs, innerHtml, escAttr, escText, opts = {}) {
  let config = parseJsonConfigAttr(attrs, "data-invitation-config");
  config = recoverNestedInvitationConfig(config, innerHtml);
  const form = String(config.form ?? "").trim();
  const project = String(config.project ?? "");
  const display =
    String(config.displayText ?? "").trim() || stripTags(innerHtml).trim() || form;
  const isPrivate = config.isPrivate === true || config.isPrivate === "true";
  const projectAttr = ` project="${escAttr(project)}"`;
  if (isPrivate) {
    const auth = authTokenExpressionToXml(
      config.authToken,
      escAttr,
      opts.knownVariableNames,
    );
    return (
      `<invitation form="${escAttr(form)}"${projectAttr} private="true">` +
      `<authenticationTokenValue>${auth}</authenticationTokenValue>` +
      `${escText(display)}` +
      `</invitation>`
    );
  }
  return `<invitation form="${escAttr(form)}"${projectAttr}>${escText(display)}</invitation>`;
}

function hyperlinkTokenToXml(attrs, escAttr, escText) {
  const config = parseJsonConfigAttr(attrs, "data-hyperlink-config");
  const url = normalizeHyperlinkUrl(String(config.url ?? ""));
  const display = String(config.displayText ?? "").trim() || url || "(Link appears here)";
  const newWindow = config.openNewWindow === true || config.openNewWindow === "true";
  const conditional = config.conditional === true || config.conditional === "true";
  const rows = Array.isArray(config.conditions) ? config.conditions : [];
  let conditions = "";
  if (conditional) {
    const filled = rows.filter((r) => String(r?.field ?? "").trim());
    if (filled.length) {
      const inner = filled
        .map((row) => {
          const op = row.op ?? "equals";
          const field = String(row.field).trim().replace(/^<<|>>$/g, "");
          if (op === "isBlank" || op === "isNotBlank" || op === "mcIsBlank" || op === "mcIsNotBlank") {
            return `<${op} field="${escAttr(field)}"/>`;
          }
          return `<${op} field="${escAttr(field)}">${conditionOperandXml(row.value, escAttr)}</${op}>`;
        })
        .join("");
      conditions = `<displayConditions>${inner}</displayConditions>`;
    }
  }
  return (
    `<link>` +
    (newWindow ? `<new-window/>` : "") +
    `<description><string value="${escAttr(display)}"/></description>` +
    `<url>${expressionToXml(url, escAttr)}</url>` +
    conditions +
    `</link>`
  );
}

/**
 * Legacy brace MQL chip → same itemization-table XML as modern function-tokens.
 * Reads Where from structured `conditions` / `conditionsRows` / `where`.
 */
function structuredItemizationTokenToXml(attrs, escAttr, escText) {
  const raw = readAttrValue(attrs, "data-tawala-structured-node");
  if (!raw) return "";
  let node;
  try {
    node = JSON.parse(decodeURIComponent(raw));
  } catch {
    try {
      node = JSON.parse(decodeHtmlAttrEntities(raw));
    } catch {
      return "";
    }
  }
  if (!node || node.type !== "itemizationTable") return "";

  const cols = Array.isArray(node.columns) ? node.columns : [];
  const form = String(node.form ?? "").trim();
  const config = {
    "show-print-control": node.showPrint === true || node["show-print-control"] === true || node["show-print-control"] === "true",
    "show-export-control":
      node.showExport === true || node["show-export-control"] === true || node["show-export-control"] === "true",
    numberOfColumns: cols.length,
    column: cols.map((c) => ({
      header: c?.header ?? "",
      contents: c?.field ?? c?.contents ?? "",
    })),
    "form-name": form,
    conditionsRows: Array.isArray(node.conditionsRows)
      ? node.conditionsRows
      : Array.isArray(node.conditions)
        ? node.conditions
        : [{ field: "", op: "equals", value: "" }],
    conditionsCombinator: node.conditionsCombinator === "or" || node.combinator === "or" ? "or" : "and",
  };

  // Prefer filled Configure rows; else flatten a simple imported `where` leaf.
  const filled = (config.conditionsRows ?? []).filter((r) => String(r?.field ?? "").trim());
  if (!filled.length && node.where && typeof node.where === "object") {
    const flat = flattenWhereTree(node.where);
    if (flat.rows.length) {
      config.conditionsRows = flat.rows;
      config.conditionsCombinator = flat.combinator;
    }
  }

  // Re-enter the modern emitter via a synthetic function-token attrs string.
  const fakeAttrs =
    `data-function-id="itemization-table" data-function-config="${escAttr(JSON.stringify(config))}"`;
  return functionTokenToXml(fakeAttrs, escAttr, escText);
}

function readAttrValue(attrs, name) {
  const re = new RegExp(`${name}\\s*=\\s*(["'])([\\s\\S]*?)\\1`, "i");
  const m = re.exec(attrs);
  return m ? m[2] : "";
}

function flattenWhereTree(where) {
  const rows = [];
  let combinator = "and";
  const walk = (node) => {
    if (!node || typeof node !== "object") return;
    if (node.and || node.or) {
      combinator = node.or ? "or" : "and";
      const kids = node.and ?? node.or;
      if (Array.isArray(kids)) kids.forEach(walk);
      return;
    }
    if (node.field) {
      rows.push({
        field: String(node.field ?? ""),
        op: String(node.op ?? "equals"),
        value: String(node.value ?? ""),
      });
    }
  };
  walk(where);
  return { rows, combinator };
}

function functionTokenToXml(attrs, escAttr, escText) {
  const id = readAttrValue(attrs, "data-function-id");
  let config = parseFunctionConfigAttr(attrs);
  // Modern chips from structuredContentToEditorHtml also carry structured-node.
  // If data-function-config was truncated / unparseable, recover columns from it.
  if (
    id === "itemization-table" &&
    !(Array.isArray(config.column) && config.column.length) &&
    /data-tawala-structured-node\s*=/i.test(attrs)
  ) {
    const recovered = structuredItemizationTokenToXml(attrs, escAttr, escText);
    if (recovered) return recovered;
  }

  const bareField = (raw) => {
    let s = String(raw ?? "").trim();
    if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
    return s;
  };

  /** Legacy itemization cell refs use Record:Form:Field (see Signup Sheet Template.tawala). */
  const itemizationContentsField = (raw) => {
    const s = bareField(raw);
    if (!s) return "";
    if (/^Record:/i.test(s)) return s;
    const colon = s.indexOf(":");
    if (colon > 0) {
      return `Record:${s.slice(0, colon).trim()}:${s.slice(colon + 1).trim()}`;
    }
    return s;
  };

  switch (id) {
    case "record-count":
      return (
        `<record-count version="3">` +
        `<form-name>${escText(config["form-name"] ?? "")}</form-name>` +
        conditionsXml(config, escAttr) +
        `</record-count>`
      );
    case "sum":
      return (
        `<sum version="1">` +
        `<field>${escText(itemizationContentsField(config.field ?? ""))}</field>` +
        conditionsXml(config, escAttr) +
        `</sum>`
      );
    case "max":
      return (
        `<max version="1">` +
        `<field>${escText(itemizationContentsField(config.field ?? ""))}</field>` +
        conditionsXml(config, escAttr) +
        `</max>`
      );
    case "min":
      return (
        `<min version="1">` +
        `<field>${escText(itemizationContentsField(config.field ?? ""))}</field>` +
        conditionsXml(config, escAttr) +
        `</min>`
      );
    case "project-email-count":
      return `<project-email-count version="1"/>`;
    case "display-image": {
      const source = String(config.source ?? "").trim();
      const width = String(config.width ?? "").trim();
      // Height stays hidden in Configure; always export blank so runtime preserves aspect ratio.
      const alt = String(config.alt_title ?? "").trim();
      const expr = (raw) => {
        const s = String(raw ?? "").trim();
        if (s.startsWith("<<") && s.endsWith(">>")) {
          return `<string field="${escAttr(s.slice(2, -2))}"/>`;
        }
        return `<string value="${escAttr(s)}"/>`;
      };
      return (
        `<display-image version="1">` +
        `<source>${expr(source)}</source>` +
        `<width>${expr(width)}</width>` +
        `<height><string value=""/></height>` +
        `<alt_title>${expr(alt)}</alt_title>` +
        `</display-image>`
      );
    }
    case "display-mcq-label": {
      const fieldName = bareField(config["field-name"]);
      const display = String(config.display ?? "label_only").trim() || "label_only";
      return (
        `<display-mcq-label version="1">` +
        `<field-name>${escText(fieldName)}</field-name>` +
        `<display>${escText(display)}</display>` +
        `</display-mcq-label>`
      );
    }
    case "choice-tally-table": {
      return (
        `<choice-tally-table version="1">` +
        `<field>${escText(bareField(config.field))}</field>` +
        conditionsXml(config, escAttr) +
        `</choice-tally-table>`
      );
    }
    case "response-totals-table": {
      const layout = String(config["layout-type"] ?? "vertical").trim() || "vertical";
      return (
        `<response-totals-table version="1">` +
        `<layout-type>${escText(layout)}</layout-type>` +
        `<field>${escText(bareField(config.field))}</field>` +
        conditionsXml(config, escAttr) +
        `</response-totals-table>`
      );
    }
    case "question-correlation-table": {
      return (
        `<question-correlation-table version="1">` +
        `<question-field-name>${escText(itemizationContentsField(config["question-field-name"]))}</question-field-name>` +
        `<display-field-name>${escText(itemizationContentsField(config["display-field-name"]))}</display-field-name>` +
        `<preferred-choice-field-name>${escText(itemizationContentsField(config["preferred-choice-field-name"]))}</preferred-choice-field-name>` +
        conditionsXml(config, escAttr) +
        `</question-correlation-table>`
      );
    }
    case "popular-choice-display": {
      return (
        `<popular-choice-display version="1">` +
        `<rank>${escText(config.rank ?? "1")}</rank>` +
        `<popular-choice-field-name>${escText(itemizationContentsField(config["popular-choice-field-name"]))}</popular-choice-field-name>` +
        conditionsXml(config, escAttr) +
        `</popular-choice-display>`
      );
    }
    case "popular-choice-count": {
      return (
        `<popular-choice-count version="1">` +
        `<rank>${escText(config.rank ?? "1")}</rank>` +
        `<popular-choice-field-name>${escText(itemizationContentsField(config["popular-choice-field-name"]))}</popular-choice-field-name>` +
        conditionsXml(config, escAttr) +
        `</popular-choice-count>`
      );
    }
    case "popular-choice-correlation-table": {
      // Legacy Designer emits Record:Form:Field (see FunctionInTextItemTest2163).
      return (
        `<popular-choice-correlation-table version="1">` +
        `<rank>${escText(config.rank ?? "1")}</rank>` +
        `<choice-available-field-name>${escText(itemizationContentsField(config["choice-available-field-name"]))}</choice-available-field-name>` +
        `<choice-preferred-field-name>${escText(itemizationContentsField(config["choice-preferred-field-name"]))}</choice-preferred-field-name>` +
        `<popular-choice-display-field-name>${escText(itemizationContentsField(config["popular-choice-display-field-name"]))}</popular-choice-display-field-name>` +
        conditionsXml(config, escAttr) +
        `</popular-choice-correlation-table>`
      );
    }
    case "simple-list": {
      // Java SimpleList.RuntimeProcessor requires version == 2 (not "earlier than 2").
      return (
        `<simple-list version="2">` +
        `<simple-list-field>${escText(itemizationContentsField(config["simple-list-field"]))}</simple-list-field>` +
        conditionsXml(config, escAttr) +
        `</simple-list>`
      );
    }
    case "itemization-table": {
      const cols = config.column ?? [];
      const n = Number(config.numberOfColumns ?? cols.length) || cols.length;
      const colXml = cols
        .slice(0, n)
        .map((col) => {
          const heading = escText(col.header ?? "");
          const field = itemizationContentsField(col.contents ?? col.field ?? "");
          return (
            `<column><header><string value="${heading}"/></header>` +
            `<contents><field name="${escAttr(field)}"/></contents></column>`
          );
        })
        .join("");
      const showPrint =
        config["show-print-control"] === true || config["show-print-control"] === "true";
      const showExport =
        config["show-export-control"] === true || config["show-export-control"] === "true";
      return (
        `<itemization-table version="2">` +
        `<show-print-control>${showPrint ? "true" : "false"}</show-print-control>` +
        `<show-export-control>${showExport ? "true" : "false"}</show-export-control>` +
        `<number-of-columns>${n}</number-of-columns>${colXml}` +
        conditionsXml(config, escAttr) +
        `</itemization-table>`
      );
    }
    default:
      return `<!-- function ${escText(id)} -->`;
  }
}

function formFromFieldRef(raw) {
  let s = String(raw ?? "").trim();
  if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
  if (/^Record:/i.test(s)) s = s.slice("Record:".length).trim();
  if (!s.includes(":")) return "";
  // FIB Item:blank (FIB1:a, Q3:email) uses letter/short blank after first colon —
  // not a Form:Field pair for multi-form itemization.
  const colon = s.indexOf(":");
  const first = s.slice(0, colon).trim();
  const rest = s.slice(colon + 1).trim();
  if (!first || !rest) return "";
  // Single-letter blank slots (legacy a/b/c blanks) never name a form.
  if (/^[a-z]$/i.test(rest.split(":")[0] ?? "")) return "";
  return first;
}

/** Prefer explicit form-name; else infer from MCQ/blank field refs (Tables functions). */
function inferFormName(config) {
  const forms = inferFormNames(config);
  return forms[0] ?? "";
}

/**
 * Itemization often joins multiple forms (Online Exam: Question + Answer).
 * Emit every distinct form referenced by columns / conditions / field refs.
 */
function inferFormNames(config) {
  const found = [];
  const add = (form) => {
    const f = String(form ?? "").trim();
    if (f && !found.includes(f)) found.push(f);
  };
  const explicit = String(config["form-name"] ?? "").trim();
  if (explicit) add(explicit);
  // Comma-separated multi-form (rare) or array formNames
  if (Array.isArray(config.forms)) {
    for (const f of config.forms) add(f);
  }
  const candidates = [
    config.field,
    config["field-name"],
    config["question-field-name"],
    config["display-field-name"],
    config["popular-choice-field-name"],
    config["choice-available-field-name"],
    config["choice-preferred-field-name"],
    config["popular-choice-display-field-name"],
    config["simple-list-field"],
  ];
  for (const c of candidates) add(formFromFieldRef(c));
  const cols = config.column ?? config.columns;
  if (Array.isArray(cols)) {
    for (const col of cols) add(formFromFieldRef(col?.contents ?? col?.field));
  }
  if (Array.isArray(config.conditionsRows)) {
    for (const row of config.conditionsRows) add(formFromFieldRef(row?.field));
  }
  return found;
}

function conditionsXml(config, escAttr, escText) {
  const forms = inferFormNames(config);
  const form = forms[0] ?? "";
  const rows = config.conditionsRows;
  const combinator = config.conditionsCombinator === "or" ? "or" : "and";
  const formTags = forms.map((f) => `<form name="${escAttr(f)}"/>`).join("");
  if (Array.isArray(rows)) {
    const filled = rows.filter((r) => r?.field?.trim());
    if (!filled.length) {
      return formTags ? `<conditions>${formTags}</conditions>` : `<conditions/>`;
    }
    const parts = filled.map((row) => {
      const op = normalizeXmlConditionOp(row.op);
      const field = conditionFieldForXml(row.field, form, forms);
      if (op === "isBlank" || op === "isNotBlank" || op === "mcIsBlank" || op === "mcIsNotBlank") {
        return `<${op} field="${escAttr(field)}"/>`;
      }
      return `<${op} field="${escAttr(field)}">${conditionOperandXml(row.value, escAttr)}</${op}>`;
    });
    // Legacy: inner <conditions>; multi-row is nested binary <and>/<or> (DirtBowl).
    const body = nestConditionOps(parts, combinator);
    return `<conditions>${formTags}<conditions>${body}</conditions></conditions>`;
  }
  if (formTags) {
    return `<conditions>${formTags}</conditions>`;
  }
  return `<conditions/>`;
}

/** Configure stores Skip op ids; emit those as XML element names. */
function normalizeXmlConditionOp(op) {
  const raw = String(op ?? "equals").trim() || "equals";
  const fromLabel = {
    "does not equal": "doesNotEqual",
    "does not contain": "doesNotContain",
    "begins with": "beginsWith",
    "ends with": "endsWith",
    "is less than": "isLessThan",
    "is less than or equal to": "isLessThanOrEqualTo",
    "is greater than": "isGreaterThan",
    "is greater than or equal to": "isGreaterThanOrEqualTo",
    "is blank": "isBlank",
    "is not blank": "isNotBlank",
  };
  return fromLabel[raw.toLowerCase()] ?? raw;
}

function nestConditionOps(parts, combinator) {
  if (parts.length === 0) return "";
  if (parts.length === 1) return parts[0];
  let nested = parts[parts.length - 1];
  for (let i = parts.length - 2; i >= 0; i--) {
    nested = `<${combinator}>${parts[i]}${nested}</${combinator}>`;
  }
  return nested;
}

/** Deploy Where fields use Record:Form:Field (DirtBowl / SportsDashboards). */
function conditionFieldForXml(raw, defaultForm, knownForms = []) {
  let s = String(raw ?? "").trim();
  if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
  if (!s) return "";
  if (/^Record:/i.test(s)) {
    const rest = s.slice("Record:".length).trim();
    // Already Record:Form:… with a known form prefix
    const known = [defaultForm, ...knownForms].filter(Boolean);
    if (known.some((f) => rest === f || rest.startsWith(`${f}:`))) return s;
    // Record:FIB1:a (item:blank, form missing) → Record:Form 1:FIB1:a
    if (defaultForm && rest && !rest.startsWith(`${defaultForm}:`)) {
      return `Record:${defaultForm}:${rest}`;
    }
    return s;
  }
  if (s.includes(":")) {
    const first = s.split(":")[0];
    // Multi-form MQL: Answer:SessionId next to Question columns → Record:Answer:SessionId
    const known = [defaultForm, ...knownForms].filter(Boolean);
    if (known.includes(first)) {
      return `Record:${s}`;
    }
    // FIB Item:blank (FIB1:a) relative to form-name → Record:Form 1:FIB1:a
    if (defaultForm) {
      return `Record:${defaultForm}:${s}`;
    }
    return `Record:${s}`;
  }
  if (defaultForm) {
    // Bare field / blank letter → prefix default form
    return `Record:${defaultForm}:${s}`;
  }
  return s;
}

/**
 * Design Border 1 / Border 2 / No Border → legacy `border` attr on `<table>`.
 * Default is Border 1 (matches insert table + bare `table.user` / `border="1"`).
 */
function tableBorderAttrFromHtml(tableOpenAttrs) {
  const classes = parseClassAttr(tableOpenAttrs);
  if (classes.includes("user-border-none")) return "0";
  if (classes.includes("user-border-2")) return "2";
  if (classes.includes("user-border-1")) return "1";
  const borderM = String(tableOpenAttrs ?? "").match(/\bborder\s*=\s*["']?(\d+)["']?/i);
  if (borderM) {
    const n = Number.parseInt(borderM[1], 10);
    if (n === 0) return "0";
    if (n >= 2) return "2";
  }
  return "1";
}

function tableHtmlToXml(tableHtml, escAttr, escText, opts = {}) {
  const open = extractOpenTag(String(tableHtml ?? "").trim());
  const border = tableBorderAttrFromHtml(open?.attrs ?? "");
  const rows = [];
  const rowRe = /<tr\b[^>]*>([\s\S]*?)<\/tr>/gi;
  let rowMatch;
  while ((rowMatch = rowRe.exec(tableHtml))) {
    const cells = [];
    const cellRe = /<t[dh]\b[^>]*>([\s\S]*?)<\/t[dh]>/gi;
    let cellMatch;
    while ((cellMatch = cellRe.exec(rowMatch[1]))) {
      const openTag = cellMatch[0].match(/^<t[dh]\b([^>]*)>/i)?.[1] ?? "";
      const widthPt = parseStyleAttr(`<x ${openTag}>`).width ?? "";
      const width = cssLengthToTwips(widthPt) || 2160;
      const align = parseTableCellAlign(openTag, cellMatch[1]);
      const inner = inlineHtmlToXml(cellMatch[1], escAttr, escText, opts);
      cells.push({ width, align, inner });
    }
    if (cells.length) rows.push(cells);
  }

  const rowXml = rows
    .map(
      (cells) =>
        `<row>${cells
          .map(
            (c) =>
              `<cell width="${c.width}"><division indent="0" align="${escAttr(
                c.align,
              )}">${wrapTableCellDivision(c.inner)}</division></cell>`,
          )
          .join("")}</row>`,
    )
    .join("");
  return `<table indent="0" border="${border}">${rowXml}</table>`;
}

/**
 * Table cells always need a `<font>` for plain text, but function tokens already
 * emit `<font><sum|itemization-table|…>`. Nesting another `<font>` makes Java’s
 * Font FACTORY drop the inner component (Potluck Details SUM → blank cells).
 */
function wrapTableCellDivision(inner) {
  let body = String(inner ?? "").trim() || "<sp/>";
  // Collapse accidental nested fonts around display components.
  for (let i = 0; i < 4; i++) {
    if (!/^<font\b/i.test(body)) break;
    const unwrapped = unwrapOuterFont(body);
    if (unwrapped === body) break;
    const looksLikeDisplay =
      isFontWrappedDisplayComponent(unwrapped) ||
      isFontWrappedDisplayComponent(`<font>${unwrapped}</font>`) ||
      /^<(sum|max|min|itemization-table|record-count|choice-tally-table|response-totals-table|question-correlation-table|popular-choice-(?:display|count|correlation-table)|simple-list|display-image|display-mcq-label|project-email-count)\b/i.test(
        unwrapped.trim(),
      );
    if (!looksLikeDisplay) break;
    body = unwrapped;
  }
  if (isFontWrappedDisplayComponent(body) || /^<font\b/i.test(body)) {
    return body;
  }
  if (/^<(sum|max|min|itemization-table|record-count|choice-tally-table|response-totals-table|question-correlation-table|popular-choice-(?:display|count|correlation-table)|simple-list|display-image|display-mcq-label|project-email-count)\b/i.test(
    body,
  )) {
    return `<font>${body}</font>`;
  }
  return `<font>${body}</font>`;
}

/**
 * Form Text often wraps a table in `<div data-doc-blank>` (or mixes prose + table).
 * Document placed lines may nest invalid `<p>` inside `<p class="doc-placed-text">`
 * (Signup Sheet contact heading). Split block tags so each becomes its own paragraph
 * / table — inlineHtmlToXml would otherwise flatten nested `<p>` into one line.
 */
function mixedFlowInnerToXml(innerHtml, escAttr, escText, align = "left", opts = {}) {
  const parts = [];
  let rest = String(innerHtml ?? "");
  while (rest.trim()) {
    const blockIdx = rest.search(/<(?:table|p|div)\b/i);
    if (blockIdx < 0) {
      const body = inlineHtmlToXml(rest, escAttr, escText, opts);
      const meaningful = String(body ?? "")
        .replace(/<sp\s*\/>/gi, "")
        .replace(/\s+/g, "")
        .trim();
      if (meaningful) {
        parts.push(`<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`);
      }
      break;
    }
    if (blockIdx > 0) {
      const before = rest.slice(0, blockIdx);
      const body = inlineHtmlToXml(before, escAttr, escText, opts);
      const meaningful = String(body ?? "")
        .replace(/<sp\s*\/>/gi, "")
        .replace(/\s+/g, "")
        .trim();
      if (meaningful) {
        parts.push(`<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`);
      }
      rest = rest.slice(blockIdx);
    }
    const open = extractOpenTag(rest);
    if (!open || !/^(table|p|div)$/i.test(open.name)) {
      const body = inlineHtmlToXml(rest, escAttr, escText, opts);
      if (body.trim()) {
        parts.push(`<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`);
      }
      break;
    }
    const matched = sliceMatchingElement(rest, open);
    if (!matched) {
      const body = inlineHtmlToXml(rest, escAttr, escText, opts);
      if (body.trim()) {
        parts.push(`<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`);
      }
      break;
    }
    if (open.name === "table") {
      parts.push(tableHtmlToXml(matched.block, escAttr, escText, opts));
    } else {
      // Nested p/div: recurse so wrappers that still contain tables/paragraphs expand.
      const nestedAlign = parseAlignFromTag(open.attrs) || align;
      const nestedInner = matched.inner;
      if (/<(?:table|p|div)\b/i.test(nestedInner)) {
        const nested = mixedFlowInnerToXml(nestedInner, escAttr, escText, nestedAlign, opts);
        if (nested) parts.push(nested);
        else if (/\bdata-doc-blank\s*=\s*["']?1["']?/i.test(open.attrs)) {
          parts.push(BLANK_PARAGRAPH_XML);
        }
      } else {
        const body = inlineHtmlToXml(nestedInner, escAttr, escText, opts);
        const meaningful = String(body ?? "")
          .replace(/<sp\s*\/>/gi, "")
          .replace(/\s+/g, "")
          .trim();
        if (meaningful) {
          parts.push(
            `<paragraph indent="0" align="${escAttr(nestedAlign)}">${body}</paragraph>`,
          );
        } else if (
          opts.keepEmptyParagraphs ||
          /\bdata-doc-blank\s*=\s*["']?1["']?/i.test(open.attrs)
        ) {
          parts.push(BLANK_PARAGRAPH_XML);
        }
        // Document: skip unmarked empty nested <p></p> / <br> husks — trailing empties
        // would stack with placed-gap spacers (Signup Sheet contact block).
      }
    }
    rest = matched.rest;
  }
  return parts.join("");
}

function blockHtmlToXml(blockHtml, escAttr, escText, opts = {}) {
  const open = extractOpenTag(blockHtml);
  if (!open) {
    const text = inlineHtmlToXml(blockHtml, escAttr, escText, opts);
    return text ? `<paragraph indent="0" align="left">${text}</paragraph>` : "";
  }

  if (open.name === "table") {
    return tableHtmlToXml(blockHtml, escAttr, escText, opts);
  }

  if (open.name === "p" || open.name === "div") {
    const classes = parseClassAttr(open.attrs);
    const style = parseStyleAttr(open.attrs);
    const align = parseAlignFromTag(open.attrs);
    const inner = extractTagInner(blockHtml, open.name);
    const isPlaced = classes.includes("doc-placed-text") || style.position === "absolute";

    // Nested tables / invalid nested <p> inside doc-placed-text (Signup Sheet) /
    // Form Text confirmation grids — expand to real paragraph/table XML.
    if (/<(?:table|p|div)\b/i.test(inner)) {
      const mixed = mixedFlowInnerToXml(inner, escAttr, escText, align, opts);
      if (mixed) return mixed;
      // Empty wrapper around blank table only — keep intentional blank if marked.
      return /\bdata-doc-blank\s*=\s*["']?1["']?/i.test(open.attrs) ? BLANK_PARAGRAPH_XML : "";
    }

    const body = inlineHtmlToXml(inner, escAttr, escText, opts);
    const meaningful = String(body ?? "")
      .replace(/<sp\s*\/>/gi, "")
      .replace(/\s+/g, "")
      .trim();

    if (isPlaced) {
      // Drop leftover empty husks (select-all delete debris). Keep intentional
      // Double-Return blanks (`data-doc-blank`) so Deploy retains vertical gaps —
      // Java ignores absolute `top` and lays paragraphs in document order.
      if (!meaningful) {
        return /\bdata-doc-blank\s*=\s*["']?1["']?/i.test(open.attrs) ? BLANK_PARAGRAPH_XML : "";
      }
      // Do NOT wrap in <division left/top> — Document Paragraph FACTORY has no "division".
      // Do NOT wrap again in <font> — inlineHtmlToXml already wraps function tokens in <font>,
      // and Java Font FACTORY cannot nest <font> (drops the inner itemization).
      return `<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`;
    }

    if (!meaningful) {
      // Form Text Double-Return stores bare `<p></p>` / `<p><br></p>` (no data-doc-blank).
      // Keep those as Deploy spacers so blank-line separation survives Push.
      // Document placed canvas still drops unmarked husks (Signup Sheet contact block).
      if (
        opts.keepEmptyParagraphs ||
        /\bdata-doc-blank\s*=\s*["']?1["']?/i.test(open.attrs)
      ) {
        return BLANK_PARAGRAPH_XML;
      }
      return "";
    }
    return `<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`;
  }

  return `<paragraph indent="0" align="left">${inlineHtmlToXml(blockHtml, escAttr, escText, opts)}</paragraph>`;
}

/** Absolute `top` in pt for a placed Document block, or null. */
function placedTopPt(blockHtml) {
  const open = extractOpenTag(blockHtml);
  if (!open) return null;
  const classes = parseClassAttr(open.attrs);
  const style = parseStyleAttr(open.attrs);
  if (!classes.includes("doc-placed-text") && style.position !== "absolute") return null;
  const raw = style.top;
  if (!raw) return null;
  const n = Number.parseFloat(String(raw).replace(/pt$/i, ""));
  return Number.isFinite(n) ? n : null;
}

/** Absolute `left` in pt for a placed Document block (secondary sort), or 0. */
function placedLeftPt(blockHtml) {
  const open = extractOpenTag(blockHtml);
  if (!open) return 0;
  const style = parseStyleAttr(open.attrs);
  const raw = style.left;
  if (!raw) return 0;
  const n = Number.parseFloat(String(raw).replace(/pt$/i, ""));
  return Number.isFinite(n) ? n : 0;
}

/**
 * Design canvas gaps are absolute `top` deltas; Deploy is flow layout. When two
 * content lines are clearly separated on the canvas (e.g. two DISPLAY MCQ chips
 * with white space between), emit blank paragraphs so the gap is not collapsed.
 */
function spacerXmlForPlacedGap(prevTopPt, nextTopPt) {
  if (prevTopPt == null || nextTopPt == null) return [];
  const gap = nextTopPt - prevTopPt;
  // One chip/line ~20–24pt; anything beyond that is intentional whitespace.
  const NOMINAL_LINE_PT = 22;
  const extra = gap - NOMINAL_LINE_PT;
  if (extra < 14) return [];
  if (extra < 60) return [BLANK_PARAGRAPH_XML];
  if (extra < 120) return [BLANK_PARAGRAPH_XML, BLANK_PARAGRAPH_XML];
  return [BLANK_PARAGRAPH_XML, BLANK_PARAGRAPH_XML, BLANK_PARAGRAPH_XML];
}

/**
 * Collect top-level blocks, then order placed lines by absolute `top` (then `left`)
 * so Deploy flow matches the Design canvas when lines were dragged out of DOM order.
 */
function collectBlocksInDeployOrder(html) {
  const blocks = [];
  let rest = String(html ?? "");
  while (rest.trim()) {
    const block = nextTopLevelBlock(rest);
    if (!block) break;
    blocks.push(block.html);
    rest = block.rest;
  }
  const indexed = blocks.map((blockHtml, i) => ({
    html: blockHtml,
    i,
    top: placedTopPt(blockHtml),
    left: placedLeftPt(blockHtml),
  }));
  indexed.sort((a, b) => {
    if (a.top != null && b.top != null) {
      if (a.top !== b.top) return a.top - b.top;
      if (a.left !== b.left) return a.left - b.left;
    } else if (a.top != null && b.top == null) {
      return -1;
    } else if (a.top == null && b.top != null) {
      return 1;
    }
    return a.i - b.i;
  });
  return indexed.map((x) => x.html);
}

/** Convert document editor HTML string to legacy xmlData body markup.
 * @param {{ formName?: string, keepEmptyParagraphs?: boolean, project?: object }} [options] — when
 *   `formName` is set (Form Text Deploy), bare `<<attendeeName>>` becomes
 *   `<field name="Form:attendeeName"/>` if that name is a field on the form
 *   (legacy Potluck). Process/admin vars (`AdminAdrss`, `League`) stay bare when
 *   `project` is provided. `keepEmptyParagraphs` (Form Text) keeps bare `<p></p>` /
 *   `<p><br></p>` as Deploy spacer paragraphs.
 */
export function documentHtmlToXml(html, escAttr, escText, options = {}) {
  const source = String(html ?? "").trim();
  const formName = options.formName ?? "";
  let formFieldNames = options.formFieldNames;
  if (!(formFieldNames instanceof Set) && options.project && formName) {
    const form = (options.project.forms ?? []).find(
      (f) => String(f?.name ?? "") === String(formName),
    );
    formFieldNames = collectFormFieldNames(form);
  }
  const opts = {
    formName,
    formFieldNames,
    knownVariableNames: options.project
      ? collectProjectVariableNames(options.project)
      : null,
    keepEmptyParagraphs: options.keepEmptyParagraphs === true,
    // Dual-chip: detect modern MQL on the full document, not per paragraph.
    hasModernMqlToken: /data-function-id\s*=\s*["']itemization-table["']/i.test(source),
  };
  if (!source) {
    return `<paragraph indent="0" align="left"></paragraph>`;
  }

  const parts = [];
  let lastContentTop = null;
  for (const blockHtml of collectBlocksInDeployOrder(source)) {
    const top = placedTopPt(blockHtml);
    const xml = blockHtmlToXml(blockHtml, escAttr, escText, opts);
    if (xml) {
      const isBlankOnly = xml === BLANK_PARAGRAPH_XML;
      if (!isBlankOnly && top != null && lastContentTop != null) {
        for (const spacer of spacerXmlForPlacedGap(lastContentTop, top)) {
          parts.push(spacer);
        }
      }
      parts.push(xml);
      if (!isBlankOnly && top != null) lastContentTop = top;
    }
  }

  return parts.length ? parts.join("") : `<paragraph indent="0" align="left"></paragraph>`;
}
