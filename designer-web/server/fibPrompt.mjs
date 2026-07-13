/**
 * Parse JSON FIB prompt strings into labeled rows + blanks.
 * - `//` = new row
 * - `[hint]` = italic hint above next blank(s)
 * - `/` = label segment then blank (repeated)
 * - `_+` = Designer underscore blanks (must become inputs, not visible underscores)
 */

export function parseFibPrompt(prompt, blanks) {
  if (!prompt || !blanks?.length) return [];
  const rows = prompt.split("//").map((r) => r.trim()).filter(Boolean);
  let bi = 0;
  const parsed = [];
  for (const rowStr of rows) {
    const row = parseFibRow(rowStr, blanks, bi);
    bi = row.nextIdx;
    parsed.push(row);
  }
  return parsed;
}

/** Plain text for underscore matching (rich Design prompts may carry light HTML). */
function plainForUnderscores(rowStr) {
  return String(rowStr ?? "")
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

function parseFibRow(rowStr, blanks, bi) {
  const hints = [];
  let s = rowStr;
  while (s.startsWith("[")) {
    const end = s.indexOf("]");
    if (end === -1) break;
    hints.push(s.slice(1, end));
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

  if (s) segments.push({ type: "text", text: s });

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
  } else if (bi < blanks.length) {
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
