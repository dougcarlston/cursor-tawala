/**
 * Design FIB prompt HTML → legacy paragraph inner XML (font/b/i/u + blanks).
 * Preserves character formatting while turning underscore runs into <blank/>.
 */

import { normalizeFibPromptSource } from "./fibPrompt.mjs";

/** @typedef {{ bold: boolean, italic: boolean, underline: boolean, face: string, size: number, color: string }} FibFormat */

function defaultFormat() {
  return {
    bold: false,
    italic: false,
    underline: false,
    face: "Arial",
    size: 200,
    color: "000000",
  };
}

function cloneFormat(f) {
  return { ...f };
}

/** Legacy FIB font size: Design CSS pt → twentieths of a point (200 = 10pt). */
export function cssFontSizeToLegacy(raw) {
  const s = String(raw ?? "").trim().toLowerCase();
  if (!s) return 200;
  const px = s.match(/^([\d.]+)\s*px$/);
  if (px) return Math.round((Number(px[1]) * 72) / 96) * 20 || 200;
  const pt = s.match(/^([\d.]+)\s*pt$/);
  if (pt) return Math.round(Number(pt[1]) * 20) || 200;
  const n = Number.parseFloat(s);
  return Number.isFinite(n) && n > 0 ? Math.round(n * 20) : 200;
}

function parseStyleFormat(styleAttr, base) {
  const next = cloneFormat(base);
  if (!styleAttr) return next;
  const face = styleAttr.match(/font-family\s*:\s*([^;]+)/i);
  if (face) {
    const first = face[1].split(",")[0].replace(/['"]/g, "").trim();
    if (first && first.toLowerCase() !== "inherit") next.face = first;
  }
  const size = styleAttr.match(/font-size\s*:\s*([^;]+)/i);
  if (size) next.size = cssFontSizeToLegacy(size[1].trim());
  const color = styleAttr.match(/(?:^|;)\s*color\s*:\s*([^;]+)/i);
  if (color) {
    const c = color[1].trim();
    const hex = c.match(/^#([0-9a-f]{3}|[0-9a-f]{6})$/i);
    if (hex) {
      let h = hex[1];
      if (h.length === 3) h = h.split("").map((ch) => ch + ch).join("");
      next.color = h.toUpperCase();
    } else if (/^rgb\(/i.test(c)) {
      const m = c.match(/rgb\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/i);
      if (m) {
        next.color = [m[1], m[2], m[3]]
          .map((n) => Number(n).toString(16).padStart(2, "0"))
          .join("")
          .toUpperCase();
      }
    }
  }
  if (/font-weight\s*:\s*(bold|[6-9]00)/i.test(styleAttr)) next.bold = true;
  if (/font-style\s*:\s*italic/i.test(styleAttr)) next.italic = true;
  if (/text-decoration[^;]*underline/i.test(styleAttr)) next.underline = true;
  return next;
}

function decodeEntities(s) {
  return normalizeFibPromptSource(s);
}

/**
 * Emit one formatted text run as legacy <font>…</font> (with optional b/i/u).
 * Mid-string `<<Field>>` becomes `<field name="…"/>` (Signup ContactType labels).
 * @param {string} text
 * @param {FibFormat} fmt
 * @param {(s: string) => string} escAttr
 * @param {(s: string) => string} escText
 */
export function formattedTextToFontXml(text, fmt, escAttr, escText) {
  if (!text) return "";
  if (!String(text).includes("<<")) {
    return formatEscapedRun(escText(text), fmt, escAttr);
  }
  const re = /<<([^<>]+)>>/g;
  let last = 0;
  let m;
  let out = "";
  while ((m = re.exec(text)) !== null) {
    if (m.index > last) {
      out += formatEscapedRun(escText(text.slice(last, m.index)), fmt, escAttr);
    }
    // Field substitution ignores surrounding B/I/U in legacy FIB prompts.
    out += `<font face="${escAttr(fmt.face || "Arial")}" size="${fmt.size || 200}" color="${escAttr((fmt.color || "000000").replace(/^#/, ""))}"><field name="${escAttr(m[1].trim())}"/></font>`;
    last = m.index + m[0].length;
  }
  if (last < text.length) {
    out += formatEscapedRun(escText(text.slice(last)), fmt, escAttr);
  }
  return out;
}

function formatEscapedRun(inner, fmt, escAttr) {
  if (!inner) return "";
  if (fmt.underline) inner = `<u>${inner}</u>`;
  if (fmt.italic) inner = `<i>${inner}</i>`;
  if (fmt.bold) inner = `<b>${inner}</b>`;
  const face = fmt.face || "Arial";
  const size = fmt.size || 200;
  const color = (fmt.color || "000000").replace(/^#/, "");
  return `<font face="${escAttr(face)}" size="${size}" color="${escAttr(color)}">${inner}</font>`;
}

/**
 * Walk one soft-row of Design FIB HTML → XML body + blanks consumed.
 * Underscore runs become blanks from `blanks` starting at `startIdx`.
 *
 * @returns {{ body: string, nextIdx: number }}
 */
export function richFibRowHtmlToXml(rowHtml, blanks, startIdx, escAttr, escText, blankXmlFn) {
  const src = decodeEntities(rowHtml);
  let i = 0;
  let bi = startIdx;
  let body = "";
  /** @type {FibFormat[]} */
  const stack = [defaultFormat()];
  /** Parallel to stack opens: whether this tag pushed a format frame. */
  const pushedFormat = [];

  const fmt = () => stack[stack.length - 1];

  const emitText = (raw) => {
    if (!raw) return;
    const text = raw.replace(/\u00a0/g, " ");
    const re = /_+/g;
    let last = 0;
    let m;
    while ((m = re.exec(text)) !== null) {
      if (m.index > last) {
        body += formattedTextToFontXml(text.slice(last, m.index), fmt(), escAttr, escText);
      }
      if (bi < blanks.length) {
        body += blankXmlFn(blanks[bi], bi);
        bi++;
      }
      last = m.index + m[0].length;
    }
    if (last < text.length) {
      body += formattedTextToFontXml(text.slice(last), fmt(), escAttr, escText);
    }
  };

  while (i < src.length) {
    if (src[i] !== "<") {
      const next = src.indexOf("<", i);
      const chunk = next < 0 ? src.slice(i) : src.slice(i, next);
      emitText(chunk);
      i = next < 0 ? src.length : next;
      continue;
    }

    // Plain `<<Field>>` before any HTML tag parse (otherwise `<ContactType1>` is eaten).
    const fieldPlain = src.slice(i).match(/^<<([^<>]+)>>/);
    if (fieldPlain) {
      body += formattedTextToFontXml(fieldPlain[0], fmt(), escAttr, escText);
      i += fieldPlain[0].length;
      continue;
    }

    const end = src.indexOf(">", i);
    if (end < 0) {
      emitText(src.slice(i));
      break;
    }
    const tag = src.slice(i + 1, end);
    i = end + 1;
    const isClose = tag.startsWith("/");
    const nameMatch = tag.match(/^\/?\s*([a-z0-9:-]+)/i);
    const name = (nameMatch?.[1] ?? "").toLowerCase();
    if (!name || name === "br") continue;

    // Field chip from Design: <span class="field-token" data-field-name="…">
    if (!isClose && name === "span" && /\bfield-token\b/i.test(tag)) {
      const nameAttr = tag.match(/\bdata-field-name\s*=\s*("([^"]*)"|'([^']*)')/i);
      const fieldName = (nameAttr?.[2] ?? nameAttr?.[3] ?? "").trim();
      if (fieldName) {
        body += formattedTextToFontXml(`<<${fieldName}>>`, fmt(), escAttr, escText);
      }
      // Skip until matching </span>
      const close = src.slice(i).match(/^[\s\S]*?<\/span>/i);
      if (close) i += close[0].length;
      continue;
    }

    if (isClose) {
      if (pushedFormat.pop()) stack.pop();
      continue;
    }

    const selfClose = /\/\s*$/.test(tag);
    if (selfClose) continue;

    const attrs = tag.slice(name.length);
    const styleM = attrs.match(/\bstyle\s*=\s*("([^"]*)"|'([^']*)')/i);
    const styleVal = styleM ? styleM[2] ?? styleM[3] ?? "" : "";
    const next = parseStyleFormat(styleVal, fmt());

    const faceAttr = attrs.match(/\bface\s*=\s*("([^"]*)"|'([^']*)')/i);
    if (faceAttr) {
      const face = (faceAttr[2] ?? faceAttr[3] ?? "").trim();
      if (face) next.face = face;
    }
    const sizeAttr = attrs.match(/\bsize\s*=\s*("([^"]*)"|'([^']*)'|(\d+))/i);
    if (sizeAttr) {
      const raw = sizeAttr[2] ?? sizeAttr[3] ?? sizeAttr[4] ?? "";
      const n = Number(raw);
      if (Number.isFinite(n) && n > 0) next.size = n > 50 ? Math.round(n) : Math.round(n * 20);
    }
    const colorAttr = attrs.match(/\bcolor\s*=\s*("([^"]*)"|'([^']*)')/i);
    if (colorAttr) {
      let c = (colorAttr[2] ?? colorAttr[3] ?? "").replace(/^#/, "").trim();
      if (/^[0-9a-f]{3}$/i.test(c)) c = c.split("").map((ch) => ch + ch).join("");
      if (/^[0-9a-f]{6}$/i.test(c)) next.color = c.toUpperCase();
    }

    if (name === "b" || name === "strong") next.bold = true;
    if (name === "i" || name === "em") next.italic = true;
    if (name === "u") next.underline = true;
    const formatTag =
      name === "span" ||
      name === "font" ||
      name === "b" ||
      name === "strong" ||
      name === "i" ||
      name === "em" ||
      name === "u";
    if (formatTag) {
      stack.push(next);
      pushedFormat.push(true);
    } else {
      pushedFormat.push(false);
    }
  }

  return { body, nextIdx: bi };
}
