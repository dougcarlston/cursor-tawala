import { FibItem, TawalaBlank } from "@/types/tawala";

function blankDisplayLabel(blank: { name: string; displayLabel?: string }): string {
  return blank.displayLabel?.trim() || blank.name;
}

function plainPromptText(prompt: string): string {
  return String(prompt ?? "")
    .replace(/\r\n/g, "\n")
    .replace(/&nbsp;/gi, " ")
    .replace(/&#160;/g, " ")
    .replace(/&amp;/gi, "&")
    .replace(/&lt;/gi, "<")
    .replace(/&gt;/gi, ">")
    .replace(/&quot;/gi, '"')
    .replace(/&#39;/g, "'")
    .replace(/\u00a0/g, " ")
    .replace(/<br\s*\/?>/gi, "\n")
    .replace(/<\/p>/gi, "\n")
    .replace(/<[^>]+>/g, "");
}

type PreviewSeg =
  | { type: "text"; text: string }
  | { type: "blank"; blank: TawalaBlank };

/**
 * Preview/Deploy-like FIB: underscore runs become inputs (never `_` + box).
 * Design canvas idle keeps underscores via FibCanvasRow — do not reuse this there.
 */
function segmentsFromPrompt(prompt: string, blanks: TawalaBlank[]): PreviewSeg[] {
  const plain = plainPromptText(prompt);
  if (!plain.trim() || !blanks.length) return [];

  if (/_+/.test(plain)) {
    const segments: PreviewSeg[] = [];
    let bi = 0;
    const re = /_+/g;
    let last = 0;
    let match: RegExpExecArray | null;
    while ((match = re.exec(plain)) !== null) {
      if (match.index > last) {
        const text = plain.slice(last, match.index);
        if (text) segments.push({ type: "text", text });
      }
      if (bi < blanks.length) {
        segments.push({ type: "blank", blank: blanks[bi] });
        bi++;
      }
      last = match.index + match[0].length;
    }
    if (last < plain.length) {
      const text = plain.slice(last);
      if (text) segments.push({ type: "text", text });
    }
    return segments;
  }

  // No Design underscores: show stripped intro text + trailing inputs for blanks.
  const text = plain.replace(/_+/g, "").trim();
  const segments: PreviewSeg[] = [];
  if (text && !/underscores create blanks/i.test(text)) {
    segments.push({ type: "text", text });
  }
  for (const blank of blanks) {
    segments.push({ type: "blank", blank });
  }
  return segments;
}

function BlankInput({ blank }: { blank: TawalaBlank }) {
  return (
    <input
      type="text"
      className="fib-preview-input"
      size={Math.min(Math.max(blank.length ?? 20, 5), 120)}
      readOnly
      tabIndex={-1}
      aria-label={blankDisplayLabel(blank)}
      title={blank.name}
    />
  );
}

/** Deploy-like FIB preview — real inputs sized by blank.length (HTML size units). */
export function FibFieldPreview({ item }: { item: FibItem }) {
  const blanks = item.blanks ?? [];
  const prompt = typeof item.prompt === "string" ? item.prompt : "";
  const plain = plainPromptText(prompt);
  const hasUnderscoreRuns = /_+/.test(plain);

  // Legacy topLabels without Design `_` runs: labeled rows + inputs.
  if (item.style === "topLabels" && !hasUnderscoreRuns) {
    const intro =
      prompt.trim() && !prompt.includes("//") && !prompt.includes("/")
        ? plain.replace(/_+/g, "").replace(/\s+/g, " ").trim()
        : "";
    return (
      <div className="fib-preview fib-preview-toplabels">
        {intro && !/underscores create blanks/i.test(intro) ? (
          <p className="fib-preview-intro">{intro}</p>
        ) : null}
        {blanks.map((blank) => (
          <div key={blank.name} className="fib-preview-toplabel-row">
            <label className="fib-preview-toplabel-text">{blankDisplayLabel(blank)}</label>
            <BlankInput blank={blank} />
          </div>
        ))}
      </div>
    );
  }

  const segments = segmentsFromPrompt(prompt, blanks);
  return (
    <div className="fib-preview">
      <div className="fib-preview-inline">
        {segments.map((seg, i) =>
          seg.type === "text" ? (
            <span key={`t-${i}`} className="fib-preview-text">
              {seg.text}
            </span>
          ) : (
            <BlankInput key={`b-${seg.blank.name}-${i}`} blank={seg.blank} />
          ),
        )}
      </div>
    </div>
  );
}
