/**
 * Parse JSON FIB prompt strings into labeled rows + blanks.
 * - `//` or soft line breaks = new row
 * - `[hint]` = italic hint above next blank(s) (short legacy hints only — not the Design placeholder)
 * - `/` = label segment then blank (repeated)
 * - `_+` = Designer underscore blanks (must become inputs, not visible underscores)
 */

/** Design default prompt — must not be misread as a legacy `[hint]`. */
const FIB_DESIGN_PLACEHOLDER =
  "[Replace this with your question. Underscores create blanks.]";

export function parseFibPrompt(prompt, blanks) {
  if (!prompt || !blanks?.length) return [];
  const normalized = normalizeFibPromptSource(prompt);
  const rows = splitFibPromptRows(normalized);
  let bi = 0;
  const parsed = [];
  for (const rowStr of rows) {
    const row = parseFibRow(rowStr, blanks, bi);
    bi = row.nextIdx;
    if (row.segments.length) parsed.push(row);
  }
  return parsed;
}

/** Decode entities / soft breaks so Preview never prints `&nbsp;` literally. */
export function normalizeFibPromptSource(prompt) {
  return String(prompt ?? "")
    .replace(/\r\n/g, "\n")
    .replace(/&nbsp;/gi, " ")
    .replace(/&#160;/g, " ")
    .replace(/&amp;/gi, "&")
    .replace(/&lt;/gi, "<")
    .replace(/&gt;/gi, ">")
    .replace(/&quot;/gi, '"')
    .replace(/&#39;/g, "'")
    .replace(/\u00a0/g, " ");
}

/** Soft rows from Design WYSIWYG (`<br>`, paragraphs) plus legacy `//`. */
function splitFibPromptRows(prompt) {
  const withBreaks = String(prompt)
    .replace(/<br\s*\/?>/gi, "\n")
    .replace(/<\/(p|div)>/gi, "\n")
    .replace(/<(p|div)(?:\s[^>]*)?>/gi, "");
  return withBreaks
    .split(/\/\/|\n+/)
    .map((r) => r.trim())
    .filter(Boolean)
    .filter((r) => !isDesignPlaceholderOnly(r));
}

function isDesignPlaceholderOnly(rowStr) {
  const plain = plainForUnderscores(rowStr).trim();
  return plain === FIB_DESIGN_PLACEHOLDER || plain === FIB_DESIGN_PLACEHOLDER.slice(1, -1);
}

/** Plain text for underscore matching (rich Design prompts may carry light HTML). */
function plainForUnderscores(rowStr) {
  return normalizeFibPromptSource(rowStr)
    .replace(/<br\s*\/?>/gi, "\n")
    .replace(/<\/p>/gi, "\n")
    .replace(/<[^>]+>/g, "")
    .replace(/\u00a0/g, " ");
}

/**
 * Split a freeform row on underscore runs into text + blank segments.
 * Underscore characters themselves are never kept as visible text.
 */
export function segmentsFromUnderscorePrompt(rowStr, blanks, startIdx = 0) {
  const plain = plainForUnderscores(rowStr);
  const segments = [];
  let bi = startIdx;
  const re = /_+/g;
  let last = 0;
  let match;
  while ((match = re.exec(plain)) !== null) {
    if (match.index > last) {
      const text = plain.slice(last, match.index);
      if (text) segments.push({ type: "text", text });
    }
    if (bi < blanks.length) {
      segments.push({ type: "blank", blank: blanks[bi], hint: null });
      bi++;
    }
    last = match.index + match[0].length;
  }
  if (last < plain.length) {
    const text = plain.slice(last);
    if (text) segments.push({ type: "text", text });
  }
  return { segments, nextIdx: bi };
}

/** True when `[…]` is a short legacy hint, not the Design FIB placeholder. */
function isLegacyHintBracket(inner) {
  const t = inner.trim();
  if (!t) return false;
  if (/underscores create blanks/i.test(t)) return false;
  if (t.length > 48) return false;
  return true;
}

function parseFibRow(rowStr, blanks, bi) {
  const hints = [];
  let s = rowStr;
  while (s.trimStart().startsWith("[")) {
    s = s.trimStart();
    const end = s.indexOf("]");
    if (end === -1) break;
    const inner = s.slice(1, end);
    if (!isLegacyHintBracket(inner)) break;
    hints.push(inner);
    s = s.slice(end + 1);
  }

  let trailing = "";
  const parenMatch = s.match(/\([^)]+\)\s*$/);
  if (parenMatch) {
    trailing = parenMatch[0];
    s = s.slice(0, s.length - trailing.length).trimEnd();
  }

  const segments = [];

  if (s.includes("/")) {
    const parts = s.split("/");
    for (const part of parts) {
      const t = part.trim();
      if (t) segments.push({ type: "text", text: t });
      if (bi < blanks.length) {
        segments.push({
          type: "blank",
          blank: blanks[bi],
          hint: hints.shift() ?? null,
        });
        bi++;
      }
    }
    return { segments, nextIdx: bi };
  }

  // Designer WYSIWYG: "Name ________" → "Name " + blank input (not underscores + box).
  if (/_+/.test(plainForUnderscores(s))) {
    const fromUnderscores = segmentsFromUnderscorePrompt(s, blanks, bi);
    if (hints.length > 0) {
      for (const seg of fromUnderscores.segments) {
        if (seg.type === "blank" && seg.hint == null) {
          seg.hint = hints.shift() ?? null;
        }
      }
    }
    if (trailing) {
      fromUnderscores.segments.push({ type: "text", text: trailing });
    }
    return fromUnderscores;
  }

  if (s) {
    const text = plainForUnderscores(s);
    if (text) segments.push({ type: "text", text });
  }

  if (hints.length > 0) {
    for (const h of hints) {
      if (bi >= blanks.length) break;
      segments.push({ type: "blank", blank: blanks[bi], hint: h });
      bi++;
    }
  } else if (parenMatch && /mm|dd|yyyy/i.test(trailing)) {
    for (let k = 0; k < 3 && bi < blanks.length; k++) {
      segments.push({ type: "blank", blank: blanks[bi], hint: null });
      bi++;
    }
    segments.push({ type: "text", text: trailing });
  } else if (bi < blanks.length && segments.some((seg) => seg.type === "text")) {
    // Slash-less label-only row without underscores: one blank after the label text.
    // Do not invent blanks for empty/placeholder-only rows (avoids stray Preview boxes).
    segments.push({ type: "blank", blank: blanks[bi], hint: null });
    bi++;
  }

  return { segments, nextIdx: bi };
}

export function fibRowLabel(segments) {
  const first = segments.find((s) => s.type === "text");
  if (!first) return "";
  return first.text.replace(/:\s*$/, "").trim();
}

export function fibRowFields(segments) {
  return segments.filter((s) => s.type === "blank");
}

export function fibUsesLeftLabels(style) {
  return (
    style === "leftAlignLabels" ||
    style === "leftAlignLabelsJustified" ||
    style === "alignedLabels"
  );
}

/** Legacy SignupSheets-style labels: label + blank in one paragraph, no &lt;tab/&gt; (Java drops labels if a tab separates them). */
export function fibUsesRightAlignLabels(style) {
  return style === "rightAlignLabels" || style === "rightAlignLabelsJustified";
}
