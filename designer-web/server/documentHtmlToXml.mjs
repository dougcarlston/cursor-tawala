/**
 * Document WYSIWYG HTML → legacy `<xmlData>` XML (deploy path).
 * Handles paragraphs, tables, field/function tokens, and basic inline formatting.
 */

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
  const m = html.trim().match(/^<([a-z0-9]+)\b([^>]*)>/i);
  if (!m) return null;
  return { name: m[1].toLowerCase(), attrs: m[2] ?? "", full: m[0] };
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

    const closeRe = new RegExp(`</${open.name}>`, "i");
    const closeIdx = rest.search(closeRe);
    if (closeIdx < 0) {
      // Self-closing / void tags (e.g. <br>) — no inner content.
      if (/^(br|hr|img|sp)$/i.test(open.name)) {
        if (open.name.toLowerCase() === "br") out += "\n";
        rest = rest.slice(open.full.length);
        continue;
      }
      const plain = stripTags(rest);
      if (plain) out += escText(plain);
      break;
    }
    const closeLen = rest.slice(closeIdx).match(closeRe)[0].length;
    const block = rest.slice(0, closeIdx + closeLen);
    const inner = extractTagInner(block, open.name);
    rest = rest.slice(closeIdx + closeLen);

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
      const style = parseStyleAttr(open.attrs);
      let innerXml = inlineHtmlToXml(inner, escAttr, escText);
      if (style["font-size"] || style["font-family"] || style.color) {
        const face = style["font-family"]?.split(",")[0]?.replace(/['"]/g, "") ?? "";
        const size = style["font-size"] ? fontSizeToLegacy(style["font-size"]) : 200;
        const color = style.color?.replace(/^#/, "") ?? "000000";
        innerXml = `<font${face ? ` face="${escAttr(face)}"` : ""} size="${size}" color="${escAttr(color)}">${innerXml}</font>`;
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
      out += `<font>${inlineHtmlToXml(inner, escAttr, escText)}</font>`;
      continue;
    }

    out += inlineHtmlToXml(inner, escAttr, escText);
  }
  return out;
}

function functionTokenToXml(attrs, escAttr, escText) {
  const idM = attrs.match(/data-function-id="([^"]*)"/i);
  const configM = attrs.match(/data-function-config="([^"]*)"/i);
  const id = idM?.[1] ?? "";
  let config = {};
  if (configM?.[1]) {
    try {
      config = JSON.parse(
        configM[1]
          .replace(/&quot;/g, '"')
          .replace(/&#39;/g, "'")
          .replace(/&apos;/g, "'")
          .replace(/&lt;/g, "<")
          .replace(/&gt;/g, ">")
          .replace(/&amp;/g, "&"),
      );
    } catch {
      config = {};
    }
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
  const formTag = form ? `<form name="${escAttr(form)}"/>` : "";
  if (Array.isArray(rows)) {
    const filled = rows.filter((r) => r?.field?.trim());
    if (!filled.length) {
      return form ? `<conditions>${formTag}</conditions>` : `<conditions/>`;
    }
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
    return `<conditions>${formTag}${inner ? `<conditions>${inner}</conditions>` : ""}</conditions>`;
  }
  if (form) {
    return `<conditions>${formTag}</conditions>`;
  }
  return `<conditions/>`;
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

    if (classes.includes("doc-placed-text") || style.position === "absolute") {
      // Drop empty canvas husks — they become blank gaps on Deploy (Java ignores absolute top
      // and lays paragraphs out in flow, so leftover empty lines stack vertically).
      const meaningful = String(body ?? "")
        .replace(/<sp\s*\/>/gi, "")
        .replace(/\s+/g, "")
        .trim();
      if (!meaningful) return "";
      // Do NOT wrap in <division left/top> — Document Paragraph FACTORY has no "division".
      // Do NOT wrap again in <font> — inlineHtmlToXml already wraps function tokens in <font>,
      // and Java Font FACTORY cannot nest <font> (drops the inner itemization).
      return `<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`;
    }

    if (!String(body ?? "").replace(/<sp\s*\/>/gi, "").replace(/\s+/g, "").trim()) {
      return "";
    }
    return `<paragraph indent="0" align="${escAttr(align)}">${body}</paragraph>`;
  }

  return `<paragraph indent="0" align="left">${inlineHtmlToXml(blockHtml, escAttr, escText)}</paragraph>`;
}

/** Convert document editor HTML string to legacy xmlData body markup. */
export function documentHtmlToXml(html, escAttr, escText) {
  const source = String(html ?? "").trim();
  if (!source) {
    return `<paragraph indent="0" align="left"></paragraph>`;
  }

  const parts = [];
  let rest = source;
  while (rest.trim()) {
    const block = nextTopLevelBlock(rest);
    if (!block) break;
    const xml = blockHtmlToXml(block.html, escAttr, escText);
    if (xml) parts.push(xml);
    rest = block.rest;
  }

  return parts.length ? parts.join("") : `<paragraph indent="0" align="left"></paragraph>`;
}
