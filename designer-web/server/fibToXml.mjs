import {
  parseFibPrompt,
  fibUsesLeftLabels,
  fibUsesRightAlignLabels,
  fibRowFields,
  normalizeFibPromptSource,
  splitFibPromptRows,
  plainForUnderscores,
} from "./fibPrompt.mjs";
import { richFibRowHtmlToXml } from "./fibRichPromptToXml.mjs";
import { tabPositionsXmlFromInches } from "./tabPositionsXml.mjs";

const TAB_LEFT_DEFAULT =
  '<tabPositions><tabStop position="4031"/><tabStop position="6192"/></tabPositions>';
const TAB_HINT_DEFAULT =
  '<tabPositions><tabStop position="4896"/><tabStop position="8496"/></tabPositions>';
const TAB_FREEFORM_DEFAULT = '<tabPositions><tabStop position="4031"/></tabPositions>';
const TAB_TOPLABELS_DEFAULT = '<tabPositions><tabStop position="2880"/></tabPositions>';

/** Active tab XML for nested helpers (set for the duration of `fibToXml`). */
let TAB_LEFT = TAB_LEFT_DEFAULT;
let TAB_HINT = TAB_HINT_DEFAULT;
let TAB_FREEFORM = TAB_FREEFORM_DEFAULT;
let TAB_TOPLABELS = TAB_TOPLABELS_DEFAULT;

function blankLetter(i) {
  return String.fromCharCode(97 + (i % 26));
}

function fontXml(text, escText, { bold = false, italic = false, size = 200 } = {}) {
  let inner = escText(text);
  if (italic) inner = `<i>${inner}</i>`;
  if (bold) inner = `<b>${inner}</b>`;
  return `<font face="Arial" size="${size}" color="000000">${inner}</font>`;
}

/** One `<field name="…"/>` wrapped in the default FIB font. */
function fieldRefXml(name, escAttr) {
  return `<font face="Arial" size="200" color="000000"><field name="${escAttr(String(name).trim())}"/></font>`;
}

/**
 * Text that may embed `<<Field>>` mid-string (Signup ContactType labels).
 * Emits real `<field/>` nodes — never escaped `<<…>>` text (Java would not substitute).
 */
function textWithFieldRefsXml(text, escAttr, escText, fontOpts) {
  const raw = String(text ?? "");
  if (!raw.includes("<<")) {
    return fontOpts ? fontXml(raw, escText, fontOpts) : fontXml(raw, escText);
  }
  const re = /<<([^<>]+)>>/g;
  let last = 0;
  let m;
  let out = "";
  while ((m = re.exec(raw)) !== null) {
    if (m.index > last) {
      const chunk = raw.slice(last, m.index);
      out += fontOpts ? fontXml(chunk, escText, fontOpts) : fontXml(chunk, escText);
    }
    out += fieldRefXml(m[1], escAttr);
    last = m.index + m[0].length;
  }
  if (last < raw.length) {
    const chunk = raw.slice(last);
    out += fontOpts ? fontXml(chunk, escText, fontOpts) : fontXml(chunk, escText);
  }
  return out;
}

/** Prompt like `<<Custom>>:` → field token + trailing punctuation (SignupSheets Q10). */
function promptRunXml(text, escAttr, escText) {
  const m = String(text).trim().match(/^<<([^<>]+)>>(.*)$/);
  if (!m) {
    const t = text.trim();
    const shown =
      t.endsWith(":") || t.endsWith(": ") ? (t.endsWith(" ") ? t : `${t} `) : `${t} `;
    return textWithFieldRefsXml(shown, escAttr, escText);
  }
  const field = fieldRefXml(m[1], escAttr);
  const rest = m[2].trim();
  if (!rest) return field;
  const trailing = rest.startsWith(":") ? `: ` : `${rest} `;
  return field + fontXml(trailing, escText);
}

function paragraph(inner, tabs = TAB_LEFT) {
  return `<paragraph indent="0" align="left">${tabs}${inner}</paragraph>`;
}

/** FIB validator metadata id → runtime element name (mirrors `lib/fibBlanks.ts` FIB_VALIDATORS). */
const VALIDATOR_XML_ID = {
  email: "email-validator",
  phone: "phone-number-validator",
  integer: "integer-range-validator",
  usState: "us-state-validator",
  zip: "zip-code-validator",
  properName: "proper-validator",
  dollar: "us-dollar-amount-validator",
};

/** `<validator>` block for a blank (legacy `Blank.ToXml`); null when no validation set. */
function validatorXml(validation, escText) {
  if (!validation?.type) return "";
  const xmlId = VALIDATOR_XML_ID[validation.type];
  if (!xmlId) return "";
  let inner = "";
  if (xmlId !== "proper-validator") {
    const msg = validation.errorMessage ?? "";
    inner += `<error-message><string value="${escText(msg)}"/>\n</error-message>`;
  }
  if (xmlId === "integer-range-validator") {
    const lower = validation.lowerLimit?.trim();
    const upper = validation.upperLimit?.trim();
    inner += lower ? `<lower-limit><string value="${escText(lower)}"/></lower-limit>` : "<lower-limit></lower-limit>";
    inner += upper ? `<upper-limit><string value="${escText(upper)}"/></upper-limit>` : "<upper-limit></upper-limit>";
  }
  return `<validator><${xmlId} version="1">${inner}</${xmlId}></validator>`;
}

function blankXml(blank, letter, escAttr, escText) {
  const alt = blank.alternateLabel ?? blank.name;
  const req = blank.required ? "true" : "false";
  const len = blank.length ?? 20;
  const heightAttr = blank.height && blank.height > 1 ? ` height="${blank.height}"` : "";
  const altAttr = alt && alt !== letter ? ` alternateLabel="${escAttr(alt)}"` : "";
  const validator = escText ? validatorXml(blank.validation, escText) : "";
  if (validator) {
    return `<blank label="${escAttr(letter)}" length="${len}"${heightAttr} required="${req}"${altAttr}>${validator}</blank>`;
  }
  return `<blank label="${escAttr(letter)}" length="${len}" required="${req}"${altAttr}${heightAttr}/>`;
}

function labelFont(text, escAttr, escText, { bold = false } = {}) {
  const t = text.trim();
  if (!t) return "";
  const withColon = t.endsWith(":") ? `${t} ` : `${t}: `;
  return textWithFieldRefsXml(withColon, escAttr, escText, { bold });
}

function hintParagraph(hints, escText) {
  let body = "";
  for (const h of hints) {
    body += `<tab/>${fontXml(h, escText, { italic: true })}`;
  }
  return paragraph(body, TAB_HINT);
}

function splitNoteText(text) {
  const m = text.match(/^\(([^)]+)\)(.*)$/);
  if (!m) return { note: null, rest: text };
  return { note: `(${m[1]})`, rest: m[2].trim() };
}

function isDobRow(row) {
  const fields = row.segments.filter((s) => s.type === "blank");
  const texts = row.segments.filter((s) => s.type === "text");
  const trailing = texts[texts.length - 1]?.text ?? "";
  return fields.length === 3 && /mm|dd|yyyy/i.test(trailing);
}

/**
 * Java AlignedLabelsLayout / JustifiedInputsLayout: text before the *first* blank is
 * the label cell; everything after (more blanks, “Email (again)”, `(first)`/`(last)`)
 * stays in the remainder on the **same row**.
 *
 * Preserve each Design soft-row as one Deploy paragraph. Do **not** split
 * `Email ____ Email (again) ____` or `First Name ____ Last Name ____` into multiple
 * rows — legacy shows those on one line (owner Jul 22). Separate soft-rows (Enter)
 * still become separate table rows.
 */
function paragraphsForAlignedRow(row) {
  return [row];
}

/** Date of Birth row — label + inline (mm/dd/yyyy) hint, then mo/day/year (5173 layout). */
function dobRowsXml(row, letters, escAttr, escText) {
  const texts = row.segments.filter((s) => s.type === "text");
  const fields = row.segments.filter((s) => s.type === "blank");
  const label = texts[0]?.text ?? "Date of Birth:";
  const trailing = texts[texts.length - 1]?.text ?? "(mm/dd/yyyy)";

  let body = labelFont(label, escAttr, escText, { bold: true });
  body += fontXml(trailing, escText, { italic: true });
  body += "<tab/>";
  body += blankXml(fields[0].blank, letters.get(fields[0].blank), escAttr, escText);
  body += fontXml("/", escText);
  body += blankXml(fields[1].blank, letters.get(fields[1].blank), escAttr, escText);
  body += fontXml("/", escText);
  body += blankXml(fields[2].blank, letters.get(fields[2].blank), escAttr, escText);

  return [paragraph(body)];
}

/**
 * leftAlignLabels: no <tab/> — Java only reads labels from simple paragraphs.
 * Walk text/blank in order. Multi-blank soft-rows stay one paragraph (remainder).
 * Do not force bold — Styles dialog labels are regular weight unless the author bolds them.
 */
function leftAlignRowXml(row, letters, escAttr, escText) {
  let body = "";
  let seenBlank = false;
  for (const seg of row.segments) {
    if (seg.type === "text") {
      const t = (seg.text ?? "").trim();
      if (!t) continue;
      if (t.startsWith("(")) {
        body += textWithFieldRefsXml(t, escAttr, escText, { italic: true });
      } else if (!seenBlank) {
        body += labelFont(t, escAttr, escText, { bold: false });
      } else {
        body += textWithFieldRefsXml(t, escAttr, escText);
      }
      continue;
    }
    seenBlank = true;
    body += blankXml(seg.blank, letters.get(seg.blank), escAttr, escText);
  }
  return paragraph(body);
}

/**
 * rightAlignLabelsJustified (SignupSheets): same no-tab rule as leftAlign, but legacy
 * uses a single 2880 twip tab stop (not the DirtBowl dual stops).
 */
function rightAlignRowXml(row, letters, escAttr, escText) {
  let body = "";
  let seenBlank = false;
  for (const seg of row.segments) {
    if (seg.type === "text") {
      const t = (seg.text ?? "").trim();
      if (!t) continue;
      if (t.startsWith("(")) {
        body += textWithFieldRefsXml(t, escAttr, escText, { italic: true });
      } else if (!seenBlank) {
        body += promptRunXml(seg.text, escAttr, escText);
      } else {
        body += textWithFieldRefsXml(t, escAttr, escText);
      }
      continue;
    }
    seenBlank = true;
    body += blankXml(seg.blank, letters.get(seg.blank), escAttr, escText);
  }
  return paragraph(body, TAB_TOPLABELS);
}

/** True when Design prompt row carries character formatting to mirror into Deploy XML. */
function rowHasRichFormatting(rowStr) {
  return /<(?:b|i|u|strong|em|span|font)\b/i.test(rowStr) || /\bstyle\s*=/i.test(rowStr);
}

/**
 * Strip leading legacy `[hint]` brackets (same rules as parseFibRow) so rich HTML
 * walk does not emit them as literal label text.
 */
function stripLeadingLegacyHints(rowStr) {
  let s = rowStr;
  const hints = [];
  while (s.trimStart().startsWith("[")) {
    s = s.trimStart();
    const end = s.indexOf("]");
    if (end === -1) break;
    const inner = s.slice(1, end);
    if (inner.length === 0 || inner.length > 48 || /underscores create blanks/i.test(inner)) break;
    hints.push(inner);
    s = s.slice(end + 1);
  }
  return { html: s, hints };
}

/** Default/freeform: one paragraph per Design soft-row (preserve multi-blank WYSIWYG lines). */
function defaultRowXml(row, letters, escAttr, escText) {
  if (isDobRow(row)) {
    return dobRowsXml(row, letters, escAttr, escText);
  }

  const fields = fibRowFields(row.segments);
  const hints = fields.map((f) => f.hint).filter(Boolean);
  const parts = [];

  if (hints.length >= 2) {
    parts.push(hintParagraph(hints, escText));
  }

  // Walk text/blank segments in order so "First ____ Last ____" stays one Deploy line.
  // (Previously each blank became its own <paragraph>, which ignored Design layout.)
  let body = "";
  for (const seg of row.segments) {
    if (seg.type === "text") {
      const raw = seg.text ?? "";
      const trimmed = raw.trim();
      if (!trimmed) {
        if (raw) body += textWithFieldRefsXml(raw, escAttr, escText);
        continue;
      }
      if (trimmed.startsWith("(")) {
        body += textWithFieldRefsXml(raw, escAttr, escText, { italic: true });
      } else {
        // Keep Design spacing; add a trailing space after bare labels ending in ":"
        // Do not auto-bold Name/Email/Phone — only Design B/I/U (rich path) should bold.
        let shown = raw;
        if (trimmed.endsWith(":") && !/\s$/.test(raw)) shown = `${raw} `;
        body += textWithFieldRefsXml(shown, escAttr, escText);
      }
      continue;
    }
    body += blankXml(seg.blank, letters.get(seg.blank), escAttr, escText);
  }

  if (body) {
    // Single tab stop — enough for legacy paragraph chrome; no dual-stop forcing one blank/line.
    parts.push(paragraph(body, TAB_FREEFORM));
  }
  return parts;
}

/**
 * Freeform Deploy paragraphs: mirror Design B/I/U / face / size / color when present;
 * otherwise keep the plain-segment path (no name-heuristic bold).
 */
function freeformRowsXml(prompt, blanks, letters, escAttr, escText) {
  const rowStrs = splitFibPromptRows(normalizeFibPromptSource(prompt));
  const rows = parseFibPrompt(prompt, blanks);
  let bi = 0;
  const parts = [];

  for (let i = 0; i < rows.length; i++) {
    const row = rows[i];
    const rowStr = rowStrs[i] ?? "";
    const blankCount = fibRowFields(row.segments).length;

    if (isDobRow(row)) {
      parts.push(...dobRowsXml(row, letters, escAttr, escText));
      bi += blankCount;
      continue;
    }

    const hasUnderscores = /_+/.test(plainForUnderscores(rowStr));
    if (hasUnderscores && rowHasRichFormatting(rowStr)) {
      const fields = fibRowFields(row.segments);
      const hints = fields.map((f) => f.hint).filter(Boolean);
      if (hints.length >= 2) parts.push(hintParagraph(hints, escText));

      const { html } = stripLeadingLegacyHints(rowStr);
      const blankXmlFn = (blank) => blankXml(blank, letters.get(blank), escAttr, escText);
      const { body } = richFibRowHtmlToXml(html, blanks, bi, escAttr, escText, blankXmlFn);
      if (body) parts.push(paragraph(body, TAB_FREEFORM));
      bi += blankCount;
      continue;
    }

    parts.push(...defaultRowXml(row, letters, escAttr, escText));
    bi += blankCount;
  }

  return parts;
}

/** Convert JSON FIB item → legacy Java <fib> XML with paragraphs + blanks. */
export function fibToXml(item, escAttr, escText) {
  const customTabs =
    Array.isArray(item.tabPositions) && item.tabPositions.length > 0
      ? tabPositionsXmlFromInches(item.tabPositions, TAB_FREEFORM_DEFAULT)
      : null;
  TAB_LEFT = customTabs ?? TAB_LEFT_DEFAULT;
  TAB_HINT = customTabs ?? TAB_HINT_DEFAULT;
  TAB_FREEFORM = customTabs ?? TAB_FREEFORM_DEFAULT;
  TAB_TOPLABELS = customTabs ?? TAB_TOPLABELS_DEFAULT;

  try {
    const prompt = typeof item.prompt === "string" ? item.prompt : "";
    const blanks = item.blanks ?? [];
    const style = item.style ?? "";
    const alternateLabel = item.alternateLabel ?? item.name;
    const left = fibUsesLeftLabels(style);
    const letters = new Map(blanks.map((b, i) => [b, blankLetter(i)]));

    let parts = [];
    if (style === "topLabels") {
      const rows = parseFibPrompt(prompt, blanks);
      const hasDisplayLabels = blanks.some((b) => b.displayLabel?.trim());
      parts = hasDisplayLabels
        ? topLabelsFromBlanks(item, letters, escAttr, escText)
        : rows.length > 0
          ? topLabelsRowsXml(rows, letters, escAttr, escText)
          : emptyPromptBlanksXml(blanks, escAttr, escText);
    } else if (!prompt.trim() && blanks.length > 0) {
      parts = emptyPromptBlanksXml(blanks, escAttr, escText);
    } else {
      const right = fibUsesRightAlignLabels(style);
      if (right || left) {
        const rows = parseFibPrompt(prompt, blanks);
        for (const row of rows) {
          for (const sub of paragraphsForAlignedRow(row)) {
            if (right) {
              parts.push(rightAlignRowXml(sub, letters, escAttr, escText));
            } else {
              parts.push(leftAlignRowXml(sub, letters, escAttr, escText));
            }
          }
        }
      } else {
        // Freeform / default: preserve Design soft-rows + character formatting.
        parts = freeformRowsXml(prompt, blanks, letters, escAttr, escText);
      }
    }

    const styleAttr = style ? ` style="${escAttr(style)}"` : "";
    const altAttr =
      alternateLabel && alternateLabel !== item.label
        ? ` alternateLabel="${escAttr(alternateLabel)}"`
        : "";
    return `<fib label="${escAttr(item.label)}"${altAttr}${styleAttr}>${parts.join("")}</fib>`;
  } finally {
    TAB_LEFT = TAB_LEFT_DEFAULT;
    TAB_HINT = TAB_HINT_DEFAULT;
    TAB_FREEFORM = TAB_FREEFORM_DEFAULT;
    TAB_TOPLABELS = TAB_TOPLABELS_DEFAULT;
  }
}

/** topLabels: one paragraph per blank; displayLabel is shown text, name/alternateLabel is stored field. */
function topLabelsFromBlanks(item, letters, escAttr, escText) {
  const parts = [];
  const prompt = typeof item.prompt === "string" ? item.prompt.trim() : "";
  if (prompt && !prompt.includes("//") && !prompt.includes("/")) {
    parts.push(paragraph(fontXml(prompt, escText), TAB_TOPLABELS));
  }
  for (const blank of item.blanks ?? []) {
    let body = "";
    const label = blank.displayLabel?.trim();
    if (label) {
      const shown = label.startsWith("[") ? label : `[${label}]`;
      body += fontXml(shown, escText, { bold: true, italic: true });
    }
    body += fontXml("  ", escText);
    body += blankXml(blank, letters.get(blank), escAttr, escText);
    parts.push(paragraph(body, TAB_TOPLABELS));
  }
  return parts;
}

/** topLabels via legacy prompt rows (fallback when no displayLabel on blanks). */
function topLabelsRowsXml(rows, letters, escAttr, escText) {
  const parts = [];
  for (const row of rows) {
    let body = "";
    let emittedBlank = false;
    for (const seg of row.segments) {
      if (seg.type === "text") {
        const t = seg.text.trim();
        if (!t) continue;
        // Parenthetical notes in the prompt render italic (legacy SignupSheets Max field).
        if (t.startsWith("(")) {
          body += fontXml(t.endsWith(":") ? `${t} ` : t, escText, { italic: true });
        } else {
          body += fontXml(t.endsWith(":") || t.endsWith(": ") ? (t.endsWith(" ") ? t : `${t} `) : `${t} `, escText);
        }
        continue;
      }
      const hint = seg.hint?.trim();
      if (hint) {
        const label = hint.startsWith("[") ? hint : `[${hint}]`;
        body += fontXml(label, escText, { bold: true, italic: true });
        body += fontXml("  ", escText);
      }
      body += blankXml(seg.blank, letters.get(seg.blank), escAttr, escText);
      emittedBlank = true;
    }
    if (emittedBlank || body) {
      parts.push(paragraph(body, TAB_TOPLABELS));
    }
  }
  return parts;
}

/** Legacy Friend rows: empty prompt, one blank per paragraph (DirtBowl Q10). */
function emptyPromptBlanksXml(blanks, escAttr, escText) {
  return blanks.map((blank, i) => {
    const letter = blankLetter(i);
    const pad = fontXml(" ".repeat(68), escText);
    const body = `${pad}${blankXml(blank, letter, escAttr, escText)}`;
    return paragraph(body, TAB_FREEFORM);
  });
}
