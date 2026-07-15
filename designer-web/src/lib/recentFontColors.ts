/**
 * Recent font colors for the in-app Font Color picker dialog.
 * Newest first, deduped, capped at {@link MAX_RECENT_FONT_COLORS}. Persists in localStorage.
 * (OS Color panel cannot host this row — Choose Color… uses our dialog instead.)
 */

export const MAX_RECENT_FONT_COLORS = 8;

const STORAGE_KEY = "tawala.designer.recentFontColors";

type Listener = (colors: string[]) => void;
const listeners = new Set<Listener>();

let memory: string[] | null = null;

/** Normalize to lowercase `#rrggbb`, or null if not a usable hex color. */
export function normalizeFontColorHex(raw: string): string | null {
  const s = String(raw || "").trim();
  const short = /^#([0-9a-f]{3})$/i.exec(s);
  if (short) {
    const [r, g, b] = short[1].split("");
    return `#${r}${r}${g}${g}${b}${b}`.toLowerCase();
  }
  const full = /^#([0-9a-f]{6})$/i.exec(s);
  if (full) return `#${full[1].toLowerCase()}`;
  const rgb = /^rgba?\(\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)/i.exec(s);
  if (rgb) {
    const hex = [rgb[1], rgb[2], rgb[3]]
      .map((n) => Math.max(0, Math.min(255, Number(n))).toString(16).padStart(2, "0"))
      .join("");
    return `#${hex}`;
  }
  return null;
}

function readStorage(): string[] {
  if (typeof localStorage === "undefined") return [];
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return [];
    const parsed = JSON.parse(raw) as unknown;
    if (!Array.isArray(parsed)) return [];
    const out: string[] = [];
    for (const item of parsed) {
      const hex = typeof item === "string" ? normalizeFontColorHex(item) : null;
      if (!hex || out.includes(hex)) continue;
      out.push(hex);
      if (out.length >= MAX_RECENT_FONT_COLORS) break;
    }
    return out;
  } catch {
    return [];
  }
}

function writeStorage(colors: string[]): void {
  if (typeof localStorage === "undefined") return;
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(colors));
  } catch {
    /* quota / private mode — keep in-memory only */
  }
}

function emit(colors: string[]): void {
  listeners.forEach((cb) => cb(colors));
}

/** Current recent list (newest first). */
export function getRecentFontColors(): string[] {
  if (memory == null) memory = readStorage();
  return memory;
}

/** Subscribe for palette UI; returns unsubscribe. */
export function subscribeRecentFontColors(listener: Listener): () => void {
  listeners.add(listener);
  listener(getRecentFontColors());
  return () => {
    listeners.delete(listener);
  };
}

/**
 * Push a picked color to the front of Recent (dedupe). Returns normalized hex, or null if invalid.
 */
export function pushRecentFontColor(raw: string): string | null {
  const hex = normalizeFontColorHex(raw);
  if (!hex) return null;
  const prev = getRecentFontColors();
  const next = [hex, ...prev.filter((c) => c !== hex)].slice(0, MAX_RECENT_FONT_COLORS);
  memory = next;
  writeStorage(next);
  emit(next);
  return hex;
}

/** Test helper — clear memory + storage. */
export function resetRecentFontColorsForTests(): void {
  memory = [];
  if (typeof localStorage !== "undefined") {
    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch {
      /* ignore */
    }
  }
  emit(memory);
}
