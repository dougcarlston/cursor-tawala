/**
 * Document WYSIWYG HTML → legacy `<xmlData>` XML (deploy path).
 * Handles paragraphs, tables, field/function tokens, and basic inline formatting.
 */

/** Empty paragraph used as a Deploy spacer (matches Form Text / response-totals habit). */
const BLANK_PARAGRAPH_XML =
  `<paragraph indent="0" align="left"><tabPositions><tabStop position="2880"/></tabPositions></paragraph>`;

function ptToTwips(pt) {
  const n = Number.parseFloat(String(pt).replace(/pt$/i, ""));
  return Number.isFinite(n) ? Math.round(n * 20) : 0;
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
  const m = tagHtml.match(/\bclass="([^"]*)"/i);
  return m ? m[1].split(/\s+/) : [];
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

function parseAlignFromTag(tagHtml) {
  const style = parseStyleAttr(tagHtml);
  if (style["text-align"]) return style["text-align"];
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
  for (const tag of ["table", "p", "div"]) {
    const re = new RegExp(`^<${tag}\\b[\\s\\S]*?<\\/${tag}>`, "i");
    const m = trimmed.match(re);
    if (m) return { html: m[0], rest: trimmed.slice(m[0].length) };
  }
  const textEnd = trimmed.search(/<(?:table|p|div)\b/i);
  if (textEnd > 0) {
    return { html: trimmed.slice(0, textEnd), rest: trimmed.slice(textEnd) };
  }
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

function inlineHtmlToXml(html, escAttr, escText) {
  if (!html) return "";
  let out = "";
  let rest = html;
  while (rest.length) {
    // Leading plain text / ZWSP before the next tag (common after function insert).
    const tagIdx = rest.search(/<[a-z][a-z0-9]*\b/i);
    if (tagIdx < 0) {
      const plain = stripTags(rest);
      if (plain) out += escText(plain);
      break;
    }
    if (tagIdx > 0) {
      const plain = stripTags(rest.slice(0, tagIdx));
      if (plain) out += escText(plain);
      rest = rest.slice(tagIdx);
    }

    const open = extractOpenTag(rest);
    if (!open) {
      const plain = stripTags(rest);
      if (plain) out += escText(plain);
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
      if (plain) out += escText(plain);
      break;
    }
    const { inner } = matched;
    rest = matched.rest;

    if (open.name === "span") {
      const classes = parseClassAttr(open.attrs);
      if (classes.includes("field-token")) {
        const nameM = open.attrs.match(/data-field-name="([^"]*)"/i);
        const name = nameM?.[1] ?? stripTags(inner).replace(/^<<|>>$/g, "");
        if (name) out += `<field name="${escAttr(name)}"/>`;
        continue;
      }
      if (classes.includes("function-token")) {
        // Legacy wraps display components in <font> inside paragraphs.
        out += `<font>${functionTokenToXml(open.attrs, escAttr, escText)}</font>`;
        continue;
      }
      if (classes.includes("invitation-token")) {
        out += `<font color="000080"><u>${invitationTokenToXml(open.attrs, inner, escAttr, escText)}</u></font>`;
        continue;
      }
      if (classes.includes("hyperlink-token")) {
        out += `<font color="000080"><u>${hyperlinkTokenToXml(open.attrs, escAttr, escText)}</u></font>`;
        continue;
      }
      const style = parseStyleAttr(open.attrs);
      let innerXml = inlineHtmlToXml(inner, escAttr, escText);
      if (style["font-size"] || style["font-family"] || style.color) {
        const face = style["font-family"]?.split(",")[0]?.replace(/['"]/g, "") ?? "";
        const size = style["font-size"] ? fontSizeToLegacy(style["font-size"]) : 200;
        const color = cssColorToLegacyHex(style.color);
        // Avoid nested <font> — Java Font FACTORY does not register "font" (drops children).
        const unwrapped = unwrapBareFont(innerXml);
        innerXml =
          `<font${face ? ` face="${escAttr(face)}"` : ""} size="${size}" color="${escAttr(color)}">` +
          `${unwrapped}</font>`;
      }
      out += innerXml;
      continue;
    }

    if (open.name === "strong" || open.name === "b") {
      out += `<b>${inlineHtmlToXml(inner, escAttr, escText)}</b>`;
      continue;
    }
    if (open.name === "em" || open.name === "i") {
      out += `<i>${inlineHtmlToXml(inner, escAttr, escText)}</i>`;
      continue;
    }
    if (open.name === "u") {
      out += `<u>${inlineHtmlToXml(inner, escAttr, escText)}</u>`;
      continue;
    }
    if (open.name === "font") {
      // Flatten nested bare <font> so web components are not dropped by Java.
      out += `<font>${unwrapBareFont(inlineHtmlToXml(inner, escAttr, escText))}</font>`;
      continue;
    }

    out += inlineHtmlToXml(inner, escAttr, escText);
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

function invitationTokenToXml(attrs, innerHtml, escAttr, escText) {
  const config = parseJsonConfigAttr(attrs, "data-invitation-config");
  const form = String(config.form ?? "").trim();
  const project = String(config.project ?? "");
  const display =
    String(config.displayText ?? "").trim() || stripTags(innerHtml).trim() || form;
  const isPrivate = config.isPrivate === true || config.isPrivate === "true";
  const projectAttr = ` project="${escAttr(project)}"`;
  if (isPrivate) {
    const auth = expressionToXml(config.authToken, escAttr);
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
  const url = String(config.url ?? "").trim();
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
          return `<${op} field="${escAttr(field)}"><string value="${escAttr(row.value ?? "")}"/></${op}>`;
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

function functionTokenToXml(attrs, escAttr, escText) {
  const idM = attrs.match(/data-function-id="([^"]*)"/i);
  const id = idM?.[1] ?? "";
  let config = parseFunctionConfigAttr(attrs);

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
        `<field>${escText(config.field ?? "")}</field>` +
        conditionsXml(config, escAttr) +
        `</sum>`
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
        `<question-field-name>${escText(bareField(config["question-field-name"]))}</question-field-name>` +
        `<display-field-name>${escText(bareField(config["display-field-name"]))}</display-field-name>` +
        `<preferred-choice-field-name>${escText(bareField(config["preferred-choice-field-name"]))}</preferred-choice-field-name>` +
        conditionsXml(config, escAttr) +
        `</question-correlation-table>`
      );
    }
    case "popular-choice-display": {
      return (
        `<popular-choice-display version="1">` +
        `<rank>${escText(config.rank ?? "1")}</rank>` +
        `<popular-choice-field-name>${escText(bareField(config["popular-choice-field-name"]))}</popular-choice-field-name>` +
        conditionsXml(config, escAttr) +
        `</popular-choice-display>`
      );
    }
    case "popular-choice-count": {
      return (
        `<popular-choice-count version="1">` +
        `<rank>${escText(config.rank ?? "1")}</rank>` +
        `<popular-choice-field-name>${escText(bareField(config["popular-choice-field-name"]))}</popular-choice-field-name>` +
        conditionsXml(config, escAttr) +
        `</popular-choice-count>`
      );
    }
    case "popular-choice-correlation-table": {
      return (
        `<popular-choice-correlation-table version="1">` +
        `<rank>${escText(config.rank ?? "1")}</rank>` +
        `<choice-available-field-name>${escText(bareField(config["choice-available-field-name"]))}</choice-available-field-name>` +
        `<choice-preferred-field-name>${escText(bareField(config["choice-preferred-field-name"]))}</choice-preferred-field-name>` +
        `<popular-choice-display-field-name>${escText(bareField(config["popular-choice-display-field-name"]))}</popular-choice-display-field-name>` +
        conditionsXml(config, escAttr) +
        `</popular-choice-correlation-table>`
      );
    }
    case "simple-list": {
      return (
        `<simple-list version="1">` +
        `<simple-list-field>${escText(bareField(config["simple-list-field"]))}</simple-list-field>` +
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
  const parts = s.split(":").filter(Boolean);
  if (parts.length >= 2) return parts[0];
  return "";
}

/** Prefer explicit form-name; else infer from MCQ/blank field refs (Tables functions). */
function inferFormName(config) {
  const explicit = String(config["form-name"] ?? "").trim();
  if (explicit) return explicit;
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
  for (const c of candidates) {
    const form = formFromFieldRef(c);
    if (form) return form;
  }
  const cols = config.column ?? config.columns;
  if (Array.isArray(cols)) {
    for (const col of cols) {
      const form = formFromFieldRef(col?.contents ?? col?.field);
      if (form) return form;
    }
  }
  if (Array.isArray(config.conditionsRows)) {
    for (const row of config.conditionsRows) {
      const form = formFromFieldRef(row?.field);
      if (form) return form;
    }
  }
  return "";
}

function conditionsXml(config, escAttr, escText) {
  const form = inferFormName(config);
  const rows = config.conditionsRows;
  const combinator = config.conditionsCombinator === "or" ? "or" : "and";
  const formTag = form ? `<form name="${escAttr(form)}"/>` : "";
  if (Array.isArray(rows)) {
    const filled = rows.filter((r) => r?.field?.trim());
    if (!filled.length) {
      return form ? `<conditions>${formTag}</conditions>` : `<conditions/>`;
    }
    const parts = filled.map((row) => {
      const op = normalizeXmlConditionOp(row.op);
      const field = conditionFieldForXml(row.field, form);
      if (op === "isBlank" || op === "isNotBlank" || op === "mcIsBlank" || op === "mcIsNotBlank") {
        return `<${op} field="${escAttr(field)}"/>`;
      }
      return `<${op} field="${escAttr(field)}"><string value="${escAttr(row.value ?? "")}"/></${op}>`;
    });
    // Legacy: inner <conditions>; multi-row is nested binary <and>/<or> (DirtBowl).
    const body = nestConditionOps(parts, combinator);
    return `<conditions>${formTag}<conditions>${body}</conditions></conditions>`;
  }
  if (form) {
    return `<conditions>${formTag}</conditions>`;
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
function conditionFieldForXml(raw, defaultForm) {
  let s = String(raw ?? "").trim();
  if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
  if (!s) return "";
  if (/^Record:/i.test(s)) {
    const rest = s.slice("Record:".length).trim();
    // Record:FIB1:a (item:blank, form missing) → Record:Form 1:FIB1:a
    if (defaultForm && rest && !rest.startsWith(`${defaultForm}:`)) {
      return `Record:${defaultForm}:${rest}`;
    }
    return s;
  }
  if (defaultForm) {
    // Fields palette often inserts FIB1:a (already has ':') without the Form prefix.
    if (s === defaultForm || s.startsWith(`${defaultForm}:`)) {
      return `Record:${s}`;
    }
    return `Record:${defaultForm}:${s}`;
  }
  if (s.includes(":")) return `Record:${s}`;
  return s;
}

function tableHtmlToXml(tableHtml, escAttr, escText) {
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
      const width = ptToTwips(widthPt) || 2160;
      const inner = inlineHtmlToXml(cellMatch[1], escAttr, escText);
      cells.push({ width, inner });
    }
    if (cells.length) rows.push(cells);
  }

  const rowXml = rows
    .map(
      (cells) =>
        `<row>${cells
          .map(
            (c) =>
              `<cell width="${c.width}"><division indent="0" align="left"><font>${c.inner || "<sp/>"}</font></division></cell>`,
          )
          .join("")}</row>`,
    )
    .join("");
  return `<table indent="0">${rowXml}</table>`;
}

function blockHtmlToXml(blockHtml, escAttr, escText) {
  const open = extractOpenTag(blockHtml);
  if (!open) {
    const text = inlineHtmlToXml(blockHtml, escAttr, escText);
    return text ? `<paragraph indent="0" align="left">${text}</paragraph>` : "";
  }

  if (open.name === "table") {
    return tableHtmlToXml(blockHtml, escAttr, escText);
  }

  if (open.name === "p" || open.name === "div") {
    const classes = parseClassAttr(open.attrs);
    const style = parseStyleAttr(open.attrs);
    const align = parseAlignFromTag(open.attrs);
    const inner = extractTagInner(blockHtml, open.name);
    const body = inlineHtmlToXml(inner, escAttr, escText);
    const isPlaced = classes.includes("doc-placed-text") || style.position === "absolute";
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
      return "";
    }
    return `<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`;
  }

  return `<paragraph indent="0" align="left">${inlineHtmlToXml(blockHtml, escAttr, escText)}</paragraph>`;
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

/** Convert document editor HTML string to legacy xmlData body markup. */
export function documentHtmlToXml(html, escAttr, escText) {
  const source = String(html ?? "").trim();
  if (!source) {
    return `<paragraph indent="0" align="left"></paragraph>`;
  }

  const parts = [];
  let lastContentTop = null;
  for (const blockHtml of collectBlocksInDeployOrder(source)) {
    const top = placedTopPt(blockHtml);
    const xml = blockHtmlToXml(blockHtml, escAttr, escText);
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
