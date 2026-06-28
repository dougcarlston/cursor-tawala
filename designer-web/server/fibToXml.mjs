import { parseFibPrompt, fibUsesLeftLabels } from "./fibPrompt.mjs";

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
    body += `<tab/>${fontXml(`[${h}]`, escText, { italic: true })}`;
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

/** Convert JSON FIB item → legacy Java <fib> XML with paragraphs + blanks. */
export function fibToXml(item, escAttr, escText) {
  const prompt = typeof item.prompt === "string" ? item.prompt : "";
  const blanks = item.blanks ?? [];
  const style = item.style ?? "";
  const left = fibUsesLeftLabels(style);
  const rows = parseFibPrompt(prompt, blanks);
  const letters = new Map(blanks.map((b, i) => [b, blankLetter(i)]));

  const parts = [];
  for (const row of rows) {
    if (left) {
      parts.push(leftAlignRowXml(row, letters, escAttr, escText));
    } else {
      parts.push(...defaultRowXml(row, letters, escAttr, escText));
    }
  }

  const styleAttr = style ? ` style="${escAttr(style)}"` : "";
  return `<fib label="${escAttr(item.label)}"${styleAttr}>${parts.join("")}</fib>`;
}
