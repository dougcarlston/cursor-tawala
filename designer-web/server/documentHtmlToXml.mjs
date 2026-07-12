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
    const open = extractOpenTag(rest);
    if (!open) {
      out += escText(stripTags(rest));
      break;
    }
    const before = rest.slice(0, rest.indexOf(open.full));
    if (before) out += escText(stripTags(before));

    const closeRe = new RegExp(`</${open.name}>`, "i");
    const closeIdx = rest.search(closeRe);
    if (closeIdx < 0) {
      out += escText(stripTags(rest));
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
        out += functionTokenToXml(open.attrs, escAttr, escText);
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
        configM[1].replace(/&quot;/g, '"').replace(/&amp;/g, "&"),
      );
    } catch {
      config = {};
    }
  }

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
    case "itemization-table": {
      const cols = config.column ?? [];
      const n = Number(config.numberOfColumns ?? cols.length) || cols.length;
      const colXml = cols
        .slice(0, n)
        .map((col) => {
          const heading = escText(col.header ?? "");
          return (
            `<column><header><string value="${heading}"/></header>` +
            `<contents><field name="${escAttr(col.contents ?? "")}"/></contents></column>`
          );
        })
        .join("");
      return (
        `<itemization-table version="2">` +
        `<show-print-control>false</show-print-control>` +
        `<number-of-columns>${n}</number-of-columns>${colXml}` +
        conditionsXml(config, escAttr) +
        `</itemization-table>`
      );
    }
    default:
      return `<!-- function ${escText(id)} -->`;
  }
}

function conditionsXml(config, escAttr, escText) {
  const form = config["form-name"];
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
        if (op === "isBlank" || op === "isNotBlank") {
          return `<${op} field="${escAttr(row.field.trim())}"/>`;
        }
        return `<${op} field="${escAttr(row.field.trim())}"><string value="${escAttr(row.value ?? "")}"/></${op}>`;
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
      const left = ptToTwips(style.left ?? "0");
      const top = ptToTwips(style.top ?? "0");
      return (
        `<paragraph indent="0" align="${escAttr(align)}">` +
        `<division indent="0" align="${escAttr(align)}" left="${left}" top="${top}">` +
        `<font>${body || "<sp/>"}</font></division></paragraph>`
      );
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
