/**
 * Normalize a Web-address URL entered in Insert → Link… (Web address mode).
 * Field expressions and paths that already carry a scheme are left unchanged.
 * Deploy mirror: `server/hyperlinkUrl.mjs` (keep in sync).
 */
export function normalizeHyperlinkUrl(raw: string): string {
  const s = raw.trim();
  if (!s) return s;

  // Field reference or mixed literal + field — do not rewrite.
  if (/<<[^>]+>>/.test(s)) return s;

  if (/^(https?|mailto|ftp|file|tel):/i.test(s)) return s;
  if (s.startsWith("//")) return `https:${s}`;

  // Site-relative or sibling paths stay relative.
  if (s.startsWith("/") || s.startsWith("./") || s.startsWith("../")) return s;

  const looksLikeWeb =
    /^www\./i.test(s) ||
    /^localhost(?:[:/]|$)/i.test(s) ||
    /^\d{1,3}(?:\.\d{1,3}){3}(?::\d+)?(?:\/|$)/.test(s) ||
    /^[a-z0-9](?:[a-z0-9-]*[a-z0-9])?(?:\.[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)+(?::\d+)?(?:[/?#]|$)/i.test(
      s,
    );

  if (!looksLikeWeb) return s;

  // Local dev hosts often lack TLS; public domains default to https (browser-like).
  if (/^localhost(?:[:/]|$)/i.test(s) || /^\d{1,3}(?:\.\d{1,3}){3}/.test(s)) {
    return `http://${s}`;
  }
  return `https://${s}`;
}
