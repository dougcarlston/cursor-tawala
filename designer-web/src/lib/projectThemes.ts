/**
 * Legacy Format → Project Themes (theme-config.xml display names + paths).
 *
 * G10 #1 (Sep 2026): **Project → Themes** and My Tawala use `PROJECT_THEMES` (curated).
 * `ALL_PROJECT_THEMES` retains the full legacy catalog for labels on old `themePath` values.
 *
 * `hasLocalCss`: Tomcat CSS under docker/tomcat/css/project/{path}/ (plus root default.css).
 */

export type ProjectThemeEntry = {
  /** Display label (legacy menu text). */
  label: string;
  /** `themePath` written on the project / forms. */
  path: string;
  /** Local Deploy CSS is present under docker/tomcat/css/project/. */
  hasLocalCss: boolean;
};

/** Theme paths we ship under docker/tomcat/css/project/ (plus root default.css). */
const LOCAL_CSS_PATHS = new Set([
  "baseball",
  "basicblue",
  "basicgreen",
  "basicpink",
  "basicyellow",
  "blueline",
  "chocolate",
  "dark",
  "default",
  "dirtbowl",
  "dirtbowl2",
  "fullmoon",
  "greenline",
  "greentea",
  "lime",
  "litegreen",
  "mvsc",
  "orangeswirl",
  "plain",
  "purplehaze",
  "red",
  "redrays",
  "salzburg",
  "soup",
  "style2",
  "tennis",
  "tincarbell",
  "yellow",
]);

/**
 * Full legacy visible theme list from theme-config.xml (excludes hidden "setup").
 * Order = alphabetical by label (legacy Designer menu).
 */
const THEME_DEFS: Array<{ label: string; path: string }> = [
  { label: "Baseball", path: "baseball" },
  { label: "Basic Blue", path: "basicblue" },
  { label: "Basic Green", path: "basicgreen" },
  { label: "Basic Pink", path: "basicpink" },
  { label: "Big Q", path: "style2" },
  { label: "Blue Lined Paper", path: "blueline" },
  { label: "Blue-Green Frame", path: "lime" },
  { label: "Chocolate", path: "chocolate" },
  { label: "Dark", path: "dark" },
  { label: "Default", path: "default" },
  { label: "Dirtbowl", path: "dirtbowl" },
  { label: "Dirtbowl — Wide", path: "dirtbowl2" },
  { label: "Full Moon", path: "fullmoon" },
  { label: "Green Lined Paper", path: "greenline" },
  { label: "Green Serif", path: "tennis" },
  { label: "Green Tea", path: "greentea" },
  { label: "Intense Yellow", path: "yellow" },
  { label: "Light Green", path: "litegreen" },
  { label: "MVSC", path: "mvsc" },
  { label: "Orange Swirl", path: "orangeswirl" },
  { label: "Pale Yellow", path: "basicyellow" },
  { label: "Plain", path: "plain" },
  { label: "Purple Haze", path: "purplehaze" },
  { label: "Red", path: "red" },
  { label: "Red & Green Accents", path: "tincarbell" },
  { label: "Red Rays", path: "redrays" },
  { label: "Salzburg", path: "salzburg" },
  { label: "Warm Brown", path: "soup" },
];

/** G10 #1 — owner-vetted shortlist (Sep 2026). Same set on My Tawala and Designer. */
export const CURATED_PROJECT_THEME_PATHS: readonly string[] = [
  "basicblue",
  "basicpink",
  "style2",
  "blueline",
  "lime",
  "dirtbowl2",
  "greenline",
  "tennis",
  "yellow",
  "litegreen",
  "basicyellow",
  "plain",
  "tincarbell",
  "soup",
] as const;

function themeEntry(def: { label: string; path: string }): ProjectThemeEntry {
  return {
    ...def,
    hasLocalCss: LOCAL_CSS_PATHS.has(def.path),
  };
}

/** Full legacy catalog — labels for retired paths, not shown in theme menus. */
export const ALL_PROJECT_THEMES: ProjectThemeEntry[] = THEME_DEFS.map(themeEntry);

/** Curated themes for Project → Themes and My Tawala (alphabetical by label). */
export const PROJECT_THEMES: ProjectThemeEntry[] = CURATED_PROJECT_THEME_PATHS.map((path) => {
  const def = THEME_DEFS.find((t) => t.path === path);
  if (!def) {
    throw new Error(`CURATED_PROJECT_THEME_PATHS missing THEME_DEFS entry: ${path}`);
  }
  return themeEntry(def);
}).sort((a, b) => a.label.localeCompare(b.label));

/**
 * Themes for the Designer menu — curated list, plus the project's current path when
 * it is not curated (legacy / Default / Green Tea, etc.).
 */
export function projectThemesForMenu(currentThemePath?: string | null): ProjectThemeEntry[] {
  const current = String(currentThemePath || "default").trim() || "default";
  if (PROJECT_THEMES.some((t) => t.path === current)) {
    return PROJECT_THEMES;
  }
  const legacy =
    ALL_PROJECT_THEMES.find((t) => t.path === current) ??
    themeEntry({ label: current, path: current });
  return [
    { ...legacy, label: `${legacy.label} (legacy)` },
    ...PROJECT_THEMES,
  ];
}

export function themeLabelForPath(themePath: string | undefined | null): string {
  const path = String(themePath || "default").trim() || "default";
  return ALL_PROJECT_THEMES.find((t) => t.path === path)?.label ?? path;
}

export function isKnownThemePath(themePath: string): boolean {
  return ALL_PROJECT_THEMES.some((t) => t.path === themePath);
}
