/**
 * View menu chrome show/hide — Project Explorer, Fields, Toolbar, Status Bar, Items Palette.
 * Persisted in localStorage (session chrome, not project JSON).
 */

export type ViewChromeKey =
  | "projectExplorer"
  | "fieldsPalette"
  | "toolbar"
  | "statusBar"
  | "itemsPalette";

export type ViewChromeState = Record<ViewChromeKey, boolean>;

const STORAGE_KEY = "tawala.designer.viewChrome";

export const DEFAULT_VIEW_CHROME: ViewChromeState = {
  projectExplorer: true,
  fieldsPalette: true,
  toolbar: true,
  statusBar: true,
  itemsPalette: true,
};

function clampState(raw: unknown): ViewChromeState {
  const base = { ...DEFAULT_VIEW_CHROME };
  if (!raw || typeof raw !== "object") return base;
  const o = raw as Record<string, unknown>;
  for (const key of Object.keys(base) as ViewChromeKey[]) {
    if (typeof o[key] === "boolean") base[key] = o[key];
  }
  return base;
}

function load(): ViewChromeState {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    if (!raw) return { ...DEFAULT_VIEW_CHROME };
    return clampState(JSON.parse(raw));
  } catch {
    return { ...DEFAULT_VIEW_CHROME };
  }
}

function save(state: ViewChromeState): void {
  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
  } catch {
    /* private mode / quota */
  }
}

let state: ViewChromeState = typeof localStorage !== "undefined" ? load() : { ...DEFAULT_VIEW_CHROME };
const listeners = new Set<() => void>();

function emit() {
  listeners.forEach((cb) => cb());
}

export function getViewChrome(): ViewChromeState {
  return state;
}

export function subscribeViewChrome(listener: () => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function setViewChrome(patch: Partial<ViewChromeState>): void {
  state = { ...state, ...patch };
  save(state);
  emit();
}

export function toggleViewChrome(key: ViewChromeKey): void {
  setViewChrome({ [key]: !state[key] });
}

/** Reset for unit tests. */
export function resetViewChromeForTests(next: ViewChromeState = DEFAULT_VIEW_CHROME): void {
  state = { ...next };
  emit();
}
