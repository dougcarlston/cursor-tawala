/**
 * RHS of process / MQL / display conditions for Deploy XML.
 * `<<Form:Field>>` → `<string field="…"/>` (evaluate at runtime).
 * Anything else → `<string value="…"/>` (literal text).
 *
 * Bug class (Online Exam end table empty): emitting
 * `<string value="<<Exam:id>>"/>` makes Java compare the characters "<<Exam:id>>"
 * to the session id — no rows match.
 */

export function conditionOperandXml(value, escAttr) {
  let s = String(value ?? "").trim();
  // Entity-encoded tokens that slipped through attr decode
  s = s.replace(/&lt;/gi, "<").replace(/&gt;/gi, ">");
  const m = s.match(/^<<\s*([^>]+?)\s*>>$/);
  if (m) return `<string field="${escAttr(m[1].trim())}"/>`;
  return `<string value="${escAttr(s)}"/>`;
}

/** True if export still treats a field token as a plain text literal (forbidden). */
export function isFieldTokenEmittedAsValueLiteral(xmlSnippet) {
  const s = String(xmlSnippet ?? "");
  return /<string\s+value\s*=\s*"(?:&lt;&lt;|<<)[^"]*(?:&gt;&gt;|>>)"/i.test(s);
}
