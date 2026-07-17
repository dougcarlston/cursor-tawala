/**
 * Project → Tabs… — paragraph tab stops in inches (legacy TabDialog).
 * Stored on the selected form item as `tabPositions` (inches); Deploy converts to twips (×1440).
 */

export const TAB_MAX_INCHES = 6.5;

export function inchesToTwips(inches: number): number {
  return Math.round(inches * 1440);
}

export function twipsToInches(twips: number): number {
  return twips / 1440;
}

export function formatInches(inches: number): string {
  return inches.toFixed(2);
}

export function parseInchesInput(raw: string): number | null {
  const n = Number.parseFloat(String(raw).trim());
  if (!Number.isFinite(n)) return null;
  return n;
}

/** Legacy: Set adds if 0 < tab ≤ 6.5 and not already present. */
export function addTabStop(stops: number[], inches: number): number[] | null {
  if (!(inches > 0 && inches <= TAB_MAX_INCHES)) return null;
  const rounded = Math.round(inches * 100) / 100;
  if (stops.some((s) => Math.abs(s - rounded) < 0.001)) return null;
  return [...stops, rounded].sort((a, b) => a - b);
}

/** XML fragment for Deploy; falls back when item has no custom stops. */
export function tabPositionsXmlFromInches(
  inches: number[] | undefined,
  fallbackXml: string,
): string {
  if (!Array.isArray(inches) || inches.length === 0) return fallbackXml;
  const stops = inches
    .filter((n) => Number.isFinite(n) && n > 0)
    .map((n) => `<tabStop position="${inchesToTwips(n)}"/>`)
    .join("");
  if (!stops) return fallbackXml;
  return `<tabPositions>${stops}</tabPositions>`;
}

type TabsListener = (open: boolean) => void;

let tabsOpen = false;
const tabsListeners = new Set<TabsListener>();

function emitTabs() {
  tabsListeners.forEach((cb) => cb(tabsOpen));
}

export function subscribeTabsDialog(listener: TabsListener): () => void {
  tabsListeners.add(listener);
  listener(tabsOpen);
  return () => tabsListeners.delete(listener);
}

export function isTabsDialogOpen(): boolean {
  return tabsOpen;
}

export function openTabsDialog(): void {
  tabsOpen = true;
  emitTabs();
}

export function clearTabsDialog(): void {
  tabsOpen = false;
  emitTabs();
}
