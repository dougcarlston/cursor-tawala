/**
 * Normalize Web-address URLs for hyperlink Deploy (`<link><url>…</url></link>`).
 * Keep in sync with `src/lib/hyperlinkUrl.ts`.
 */
export function normalizeHyperlinkUrl(raw) {
  const s = String(raw ?? "").trim();
  if (!s) return s;

  if (/<<[^>]+>>/.test(s)) return s;

  if (/^(https?|mailto|ftp|file|tel):/i.test(s)) return s;
  if (s.startsWith("//")) return `https:${s}`;

  if (s.startsWith("/") || s.startsWith("./") || s.startsWith("../")) return s;

  const looksLikeWeb =
    /^www\./i.test(s) ||
    /^localhost(?:[:/]|$)/i.test(s) ||
    /^\d{1,3}(?:\.\d{1,3}){3}(?::\d+)?(?:\/|$)/.test(s) ||
    /^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)+(?::\d+)?(?:[/?#]|$)/i.test(
      s,
    );

  if (!looksLikeWeb) return s;

  if (/^localhost(?:[:/]|$)/i.test(s) || /^\d{1,3}(?:\.\d{1,3}){3}/.test(s)) {
    return `http://${s}`;
  }
  return `https://${s}`;
}
