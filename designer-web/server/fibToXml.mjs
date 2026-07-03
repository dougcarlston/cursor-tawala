import { parseFibPrompt, fibUsesLeftLabels, fibRowFields } from "./fibPrompt.mjs";

const TAB_LEFT =
  '<tabPositions><tabStop position="4031"/><tabStop position="6192"/></tabPositions>';
const TAB_HINT =
  '<tabPositions><tabStop position="4896"/><tabStop position="8496"/></tabPositions>';

function blankLetter(i) {
  return String.fromCharCode(97 + (i % 26));
}

function fontXml(text, escText, { bold = false, italic = false, size = 200 } = {}) {
  let inner = escText(text);
  if (italic) inner = `<i>${inner}</i>`;
  if (bold) inner = `<b>${inner}</b>`;
  return `<font face="Arial" size="${size}" color="000000">${inner}</font>`;
}

function paragraph(inner, tabs = TAB_LEFT) {
  return `<paragraph indent="0" align="left">${tabs}${inner}</paragraph>`;
}

function blankXml(blank, letter, escAttr) {
  const alt = blank.alternateLabel ?? blank.name;
  const req = blank.required ? "true" : "false";
  const len = blank.length ?? 20;
  const heightAttr = blank.height ? ` height="${blank.height}"` : "";
  const altAttr = alt && alt !== letter ? ` alternateLabel="${escAttr(alt)}"` : "";
  return `<blank label="${escAttr(letter)}" length="${len}" required="${req}"${altAttr}${heightAttr}/>`;
}

function labelFont(text, escText, { bold = false } = {}) {
  const t = text.trim();
  if (!t) return "";
  const withColon = t.endsWith(":") ? `${t} ` : `${t}: `;
  return fontXml(withColon, escText, { bold });
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

function isBoldLabel(text) {
  return /name|email|phone|parent/i.test(text);
}

function isDobRow(row) {
  const fields = row.segments.filter((s) => s.type === "blank");
  const texts = row.segments.filter((s) => s.type === "text");
  const trailing = texts[texts.length - 1]?.text ?? "";
  return fields.length === 3 && /mm|dd|yyyy/i.test(trailing);
}

/** Date of Birth row — label + inline (mm/dd/yyyy) hint, then mo/day/year (5173 layout). */
function dobRowsXml(row, letters, escAttr, escText) {
  const texts = row.segments.filter((s) => s.type === "text");
  const fields = row.segments.filter((s) => s.type === "blank");
  const label = texts[0]?.text ?? "Date of Birth:";
  const trailing = texts[texts.length - 1]?.text ?? "(mm/dd/yyyy)";

  let body = labelFont(label, escText, { bold: true });
  body += fontXml(trailing, escText, { italic: true });
  body += "<tab/>";
  body += blankXml(fields[0].blank, letters.get(fields[0].blank), escAttr);
  body += fontXml("/", escText);
  body += blankXml(fields[1].blank, letters.get(fields[1].blank), escAttr);
  body += fontXml("/", escText);
  body += blankXml(fields[2].blank, letters.get(fields[2].blank), escAttr);

  return [paragraph(body)];
}

/** leftAlignLabels: no <tab/> — Java only reads labels from simple paragraphs. */
function leftAlignRowXml(row, letters, escAttr, escText) {
  const fields = row.segments.filter((s) => s.type === "blank");
  const texts = row.segments.filter((s) => s.type === "text");
  let body = labelFont(texts[0]?.text ?? "", escText, { bold: true });
  for (let i = 1; i < texts.length; i++) {
    const t = texts[i].text.trim();
    if (t.startsWith("(")) {
      body += fontXml(t, escText, { italic: true });
    } else {
      body += fontXml(t, escText);
    }
  }
  for (const field of fields) {
    body += blankXml(field.blank, letters.get(field.blank), escAttr);
  }
  return paragraph(body);
}

/** Default/freeform: tabbed paragraphs for multi-field rows. */
function defaultRowXml(row, letters, escAttr, escText) {
  if (isDobRow(row)) {
    return dobRowsXml(row, letters, escAttr, escText);
  }

  const fields = row.segments.filter((s) => s.type === "blank");
  const texts = row.segments.filter((s) => s.type === "text");
  const hints = fields.map((f) => f.hint).filter(Boolean);
  const parts = [];

  if (hints.length >= 2) {
    parts.push(hintParagraph(hints, escText));
  }

  if (fields.length > 1 && texts.length <= 1) {
    const labelText = texts[0]?.text ?? "";
    let body = labelFont(labelText, escText, { bold: isBoldLabel(labelText) });
    for (const field of fields) {
      body += "<tab/>";
      body += blankXml(field.blank, letters.get(field.blank), escAttr);
    }
    parts.push(paragraph(body));
    return parts;
  }

  let pending = "";
  for (const seg of row.segments) {
    if (seg.type === "text") {
      const { note, rest } = splitNoteText(seg.text.trim());
      if (note) {
        parts.push(paragraph(fontXml(note, escText, { italic: true, size: 180 })));
      }
      pending = rest || pending;
      continue;
    }
    const lt = pending.trim();
    let body = labelFont(lt, escText, { bold: isBoldLabel(lt) }) + "<tab/>";
    body += blankXml(seg.blank, letters.get(seg.blank), escAttr);
    parts.push(paragraph(body));
    pending = "";
  }
  return parts;
}

const TAB_FREEFORM = '<tabPositions><tabStop position="4031"/></tabPositions>';

const TAB_TOPLABELS = '<tabPositions><tabStop position="2880"/></tabPositions>';

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
    body += blankXml(blank, letters.get(blank), escAttr);
    parts.push(paragraph(body, TAB_TOPLABELS));
  }
  return parts;
}

/** topLabels via legacy prompt rows (fallback when no displayLabel on blanks). */
function topLabelsRowsXml(rows, letters, escAttr, escText) {
  const parts = [];
  for (const row of rows) {
    for (const field of fibRowFields(row.segments)) {
      let body = "";
      const hint = field.hint?.trim();
      if (hint) {
        const label = hint.startsWith("[") ? hint : `[${hint}]`;
        body += fontXml(label, escText, { bold: true, italic: true });
      }
      body += fontXml("  ", escText);
      body += blankXml(field.blank, letters.get(field.blank), escAttr);
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
    const body = `${pad}${blankXml(blank, letter, escAttr)}`;
    return paragraph(body, TAB_FREEFORM);
  });
}

/** Convert JSON FIB item → legacy Java <fib> XML with paragraphs + blanks. */
export function fibToXml(item, escAttr, escText) {
  const prompt = typeof item.prompt === "string" ? item.prompt : "";
  const blanks = item.blanks ?? [];
  const style = item.style ?? "";
  const alternateLabel = item.alternateLabel ?? item.name;
  const left = fibUsesLeftLabels(style);
  const letters = new Map(blanks.map((b, i) => [b, blankLetter(i)]));

  let parts = [];
  if (!prompt.trim() && blanks.length > 0) {
    parts = emptyPromptBlanksXml(blanks, escAttr, escText);
  } else {
    const rows = parseFibPrompt(prompt, blanks);
    if (style === "topLabels") {
      const hasDisplayLabels = blanks.some((b) => b.displayLabel?.trim());
      parts = hasDisplayLabels
        ? topLabelsFromBlanks(item, letters, escAttr, escText)
        : topLabelsRowsXml(rows, letters, escAttr, escText);
    } else {
      for (const row of rows) {
        if (left) {
          parts.push(leftAlignRowXml(row, letters, escAttr, escText));
        } else {
          parts.push(...defaultRowXml(row, letters, escAttr, escText));
        }
      }
    }
  }

  const styleAttr = style ? ` style="${escAttr(style)}"` : "";
  const altAttr =
    alternateLabel && alternateLabel !== item.label
      ? ` alternateLabel="${escAttr(alternateLabel)}"`
      : "";
  return `<fib label="${escAttr(item.label)}"${altAttr}${styleAttr}>${parts.join("")}</fib>`;
}
