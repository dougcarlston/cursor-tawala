/**
 * Heading Design markup → Preview/Deploy.
 *
 * Design may store Main/Sub as `<span class="heading-size-*">` per run (or whole box),
 * with `<br>` between lines. Legacy XML is one `<heading type="Main|Sub">` per size, so
 * mixed Main+Sub lines export as multiple heading elements. Same-size multi-line keeps
 * `\n` in the text (Java `HtmlString` turns `\n` into `<br />`).
 */

function decodeEntities(text) {
  return String(text ?? "")
    .replace(/&nbsp;/gi, " ")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&")
    .replace(/&quot;/g, '"');
}

/**
 * Only matches real HTML tags (start with a letter, optionally after `/`, and the opening
 * `<` is not itself preceded by another `<`) — NOT a literal `<<VariableName>>` token.
 * A naive `/<[^>]*>/` eats `<Name>` out of `<<Name>>` (the *second* `<` still looks like a
 * tag start even once the first `<` is excluded) and leaves a stray `<`/`>` behind (owner
 * bug Aug 1, 2026: `<<Customize_Title>> Administration` collapsed to `> Administration`).
 * Heading has no field-token chrome, so these tokens reach here as plain literal text and
 * must survive tag-stripping intact. Same reasoning applies to the `<br>`/`<div>`/`<p>`
 * literals below — all guarded with the same negative lookbehind.
 */
const HTML_TAG_RE = /(?<!<)<\/?[a-zA-Z][^>]*>/g;

function stripTags(html) {
  return String(html ?? "")
    .replace(/\u200b/g, "")
    .replace(HTML_TAG_RE, "");
}

/**
 * Meaningful (non-whitespace) plain text left after removing size spans.
 * Used to detect bare Main glyphs mixed with Sub runs.
 */
function meaningfulPlain(text) {
  return stripTags(text).replace(/\s+/g, "");
}

/** Strip size spans / tags to plain heading text; `<br>` / block ends → `\n`. */
export function headingPlainText(content) {
  return decodeEntities(
    String(content ?? "")
      .replace(/\u200b/g, "")
      .replace(/(?<!<)<br\s*\/?>/gi, "\n")
      .replace(/(?<!<)<\/(div|p|h[1-6])>/gi, "\n")
      .replace(/(?<!<)<(div|p)[^>]*>/gi, "")
      .replace(HTML_TAG_RE, ""),
  )
    .replace(/\r\n/g, "\n")
    .replace(/\n{3,}/g, "\n\n")
    .replace(/^\n+|\n+$/g, "");
}

/**
 * Infer legacy `Main` / `Sub` when a caller still needs one type for the whole box.
 *
 * Priority:
 * 1. `type === "subheading"` → Sub (old JSON shape)
 * 2. Legacy whole-box `level` when still set
 * 3. Uniform size spans in `content` (all Sub → Sub; all Main / bare → Main)
 * 4. Mixed Main+Sub runs → Main (fallback for single-type callers)
 */
export function headingLegacyType(item) {
  if (!item || typeof item !== "object") return "Main";
  if (item.type === "subheading") return "Sub";
  if (item.level === "sub") return "Sub";
  if (item.level === "main") return "Main";

  const html = String(item.content ?? "");
  if (!/heading-size-(main|sub)/i.test(html)) return "Main";

  const subParts = [];
  let rest = html.replace(
    /<span\b[^>]*class="[^"]*heading-size-sub[^"]*"[^>]*>([\s\S]*?)<\/span>/gi,
    (_, inner) => {
      subParts.push(inner);
      return "";
    },
  );
  const mainParts = [];
  rest = rest.replace(
    /<span\b[^>]*class="[^"]*heading-size-main[^"]*"[^>]*>([\s\S]*?)<\/span>/gi,
    (_, inner) => {
      mainParts.push(inner);
      return "";
    },
  );

  const subText = meaningfulPlain(subParts.join(""));
  const mainText = meaningfulPlain(mainParts.join(""));
  const bareText = meaningfulPlain(rest);

  if (subText && !mainText && !bareText) return "Sub";
  return "Main";
}

/**
 * @typedef {{ type: 'Main' | 'Sub', text: string, blankLinesBefore?: number }} HeadingSegment
 */

/**
 * Tokenize Design heading HTML into text runs + breaks, then group into segments.
 * Each segment is one Deploy `<heading>` / Preview `h1`|`h2`. Consecutive same-size
 * lines merge with `\n` between them.
 *
 * @returns {HeadingSegment[]}
 */
export function headingSegments(item) {
  if (!item || typeof item !== "object") return [];

  const html = String(item.content ?? "").replace(/\u200b/g, "");
  const hasSizeMarkup = /heading-size-(main|sub)/i.test(html);

  // Whole-box `level` / legacy subheading only when content has no per-run size spans.
  // New headings are created with `level: "main"`; if that sticks, it must not glue
  // Main+Sub spans into one body line (`hotelWe`).
  if (!hasSizeMarkup) {
    if (item.type === "subheading" || item.level === "sub") {
      const text = headingPlainText(item.content);
      return text ? [{ type: "Sub", text }] : [];
    }
    if (item.level === "main") {
      const text = headingPlainText(item.content);
      return text ? [{ type: "Main", text }] : [];
    }
  }

  if (!html.trim()) return [];

  /** @type {{ kind: 'text', type: 'Main' | 'Sub', text: string } | { kind: 'br' }}[] */
  const tokens = [];
  // Plain-text alternative also matches a literal `<<Name>>` token whole, so the trailing
  // catch-all (real tags only, via `HTML_TAG_RE`-equivalent `<\/?[a-zA-Z]…`) never eats its
  // first `<` and turns it into a stray `>` (owner bug Aug 1, 2026).
  const re =
    /<span\b[^>]*class=["'][^"']*heading-size-(main|sub)[^"']*["'][^>]*>([\s\S]*?)<\/span>|<br\s*\/?>|<\/(?:div|p)>|<(?:div|p)[^>]*>|(<<[^<>]*>>|[^<]+)|<\/?[a-zA-Z][^>]*>/gi;
  let m;
  while ((m = re.exec(html))) {
    if (m[1]) {
      const runType = m[1].toLowerCase() === "sub" ? "Sub" : "Main";
      const parts = String(m[2] ?? "").split(/<br\s*\/?>/i);
      parts.forEach((part, i) => {
        // Split on hard newlines inside the run (contenteditable / pre-wrap).
        const linesInPart = decodeEntities(stripTags(part)).split(/\n/);
        linesInPart.forEach((text, j) => {
          if (text) tokens.push({ kind: "text", type: runType, text });
          if (j < linesInPart.length - 1) tokens.push({ kind: "br" });
        });
        if (i < parts.length - 1) tokens.push({ kind: "br" });
      });
      continue;
    }
    if (/^<br/i.test(m[0]) || /^<\/(?:div|p)>/i.test(m[0])) {
      tokens.push({ kind: "br" });
      continue;
    }
    if (/^<(?:div|p)/i.test(m[0])) continue;
    if (m[3] != null) {
      const linesInPart = decodeEntities(m[3]).split(/\n/);
      linesInPart.forEach((text, j) => {
        if (text) tokens.push({ kind: "text", type: "Main", text });
        if (j < linesInPart.length - 1) tokens.push({ kind: "br" });
      });
    }
  }

  /**
   * Adjacent Main then Sub (no `<br>`) is common when the author selects the second
   * visual line and applies Sub — Design wraps at the span boundary so it *looks*
   * like two lines, but storage has no break → Preview used to glue `hotelWe`.
   * Treat a size change as a line boundary.
   *
   * Count `<br>`s between runs so a Design blank line (`<br><br>`) becomes
   * `blankLinesBefore` on the next segment (Deploy/Preview spacing).
   */
  /** @type {{ type: 'Main' | 'Sub', text: string, blankLinesBefore: number }[]} */
  const lines = [];
  /** @type {{ type: 'Main' | 'Sub', text: string, blankLinesBefore: number } | null} */
  let cur = null;
  let breaksBeforeNext = 0;

  const flush = () => {
    if (!cur) return;
    if (meaningfulPlain(cur.text)) lines.push(cur);
    cur = null;
  };

  for (const t of tokens) {
    if (t.kind === "br") {
      flush();
      breaksBeforeNext += 1;
      continue;
    }
    if (cur && cur.type !== t.type) {
      flush();
    }
    if (!cur) {
      // One br separates lines; extras are blank lines the author typed.
      const blankLinesBefore = Math.max(0, breaksBeforeNext - 1);
      breaksBeforeNext = 0;
      cur = { type: t.type, text: t.text, blankLinesBefore };
      continue;
    }
    cur.text += t.text;
  }
  flush();

  /** @type {HeadingSegment[]} */
  const segs = [];
  for (const ln of lines) {
    if (segs.length && segs[segs.length - 1].type === ln.type) {
      const pad = "\n".repeat(1 + (ln.blankLinesBefore || 0));
      segs[segs.length - 1].text += `${pad}${ln.text}`;
    } else {
      segs.push({
        type: ln.type,
        text: ln.text,
        blankLinesBefore: ln.blankLinesBefore || 0,
      });
    }
  }
  return segs;
}

/**
 * Heading text may carry literal `<<VariableName>>` tokens (e.g. a Process
 * `Set Customize_Title` value shown in a title like `<<Customize_Title>> Administration`).
 * Design/Preview show these as literal placeholder text (no field-token chrome; Heading
 * has no rich-text field insertion like Text/Document), but legacy `TextItem.Text`
 * (`HeadingItem` extends `TextItem`) converts any `<<Name>>` run straight into a
 * `<field name="Name"/>` XML element — **unqualified**, no form-name prefix — via
 * `Regex.Replace(text, "<<([^>]+)>>", "<field name=\"$1\"/>")`. Deploy must mirror that
 * exactly or Java never resolves the token and the raw `<<...>>` reaches the browser as
 * unescaped HTML (renders as a stray `>` — see owner bug Aug 1, 2026).
 */
function headingTextToXml(text, escAttr, escText) {
  const s = String(text ?? "");
  if (!s) return "";
  return s
    .split(/(<<[^<>]+>>)/g)
    .map((part) => {
      const m = /^<<\s*([^<>]+?)\s*>>$/.exec(part);
      if (!m) return escText(part);
      return `<field name="${escAttr(m[1].trim())}"/>`;
    })
    .join("");
}

/**
 * One or more legacy `<heading>` elements for Deploy.
 * Extra segments after the first get labels `H1.2`, `H1.3`, … so they stay unique.
 */
export function headingToXml(item, escAttr, escText) {
  const label = item?.label ?? "H1";
  const segs = headingSegments(item);
  if (!segs.length) {
    return `<heading label="${escAttr(label)}" type="Main"></heading>`;
  }
  return segs
    .map((seg, i) => {
      const lab = i === 0 ? label : `${label}.${i + 1}`;
      return `<heading label="${escAttr(lab)}" type="${seg.type}">${headingTextToXml(seg.text, escAttr, escText)}</heading>`;
    })
    .join("");
}

/**
 * Preview HTML: one `h1.heading` / `h2.subheading` per segment; `\n` → `<br>`.
 * Extra Design blank lines → `heading-after-blank` (matches Deploy stack gap CSS).
 */
export function headingToPreviewHtml(item, esc) {
  const segs = headingSegments(item);
  if (!segs.length) return "";
  return segs
    .map((seg, i) => {
      const tag = seg.type === "Sub" ? "h2" : "h1";
      const cls = seg.type === "Sub" ? "subheading" : "heading";
      const gap =
        i > 0 && (seg.blankLinesBefore ?? 0) > 0 ? " heading-after-blank" : i > 0 ? " heading-stack" : "";
      const inner = esc(seg.text).replace(/\r\n/g, "\n").replace(/\n/g, "<br>");
      return `<${tag} class="${cls}${gap}">${inner}</${tag}>`;
    })
    .join("");
}
