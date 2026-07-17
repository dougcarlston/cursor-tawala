/** Tab stops: inches on JSON items → Deploy `<tabPositions>` twips (×1440). */

export function tabPositionsXmlFromInches(inches, fallbackXml) {
  if (!Array.isArray(inches) || inches.length === 0) return fallbackXml;
  const stops = inches
    .filter((n) => Number.isFinite(n) && n > 0)
    .map((n) => `<tabStop position="${Math.round(n * 1440)}"/>`)
    .join("");
  if (!stops) return fallbackXml;
  return `<tabPositions>${stops}</tabPositions>`;
}
