/**
 * Legacy Format → Project Themes list (theme-config.xml display names + paths).
 * Sorted alphabetically by label — matches C# SortedDictionary in DesignerView.
 *
 * `hasLocalCss`: Tomcat CSS under docker/tomcat/css/project/{path}/ (or default.css).
 * Blue in the menu when true; stubs stay grey but are still selectable (sets themePath).
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
  "default",
  "dirtbowl2",
  "greentea",
  "mvsc",
  "redrays",
  "style2",
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
  { label: "Basic Yellow", path: "basicyellow" },
  { label: "Big Q", path: "style2" },
  { label: "Blue Lined Paper", path: "blueline" },
  { label: "Chocolate", path: "chocolate" },
  { label: "Dark", path: "dark" },
  { label: "Default", path: "default" },
  { label: "Dirtbowl", path: "dirtbowl" },
  { label: "Dirtbowl - Variable Width", path: "dirtbowl2" },
  { label: "Full Moon", path: "fullmoon" },
  { label: "Green Lined Paper", path: "greenline" },
  { label: "Green Tea", path: "greentea" },
  { label: "Light Green", path: "litegreen" },
  { label: "Lime", path: "lime" },
  { label: "MVSC", path: "mvsc" },
  { label: "Orange Swirl", path: "orangeswirl" },
  { label: "Plain", path: "plain" },
  { label: "Purple Haze", path: "purplehaze" },
  { label: "Red", path: "red" },
  { label: "Red Rays", path: "redrays" },
  { label: "Salzburg", path: "salzburg" },
  { label: "Soup's On", path: "soup" },
  { label: "Tennis", path: "tennis" },
  { label: "Tin Car Bell", path: "tincarbell" },
  { label: "Yellow", path: "yellow" },
];

export const PROJECT_THEMES: ProjectThemeEntry[] = THEME_DEFS.map((t) => ({
  ...t,
  hasLocalCss: LOCAL_CSS_PATHS.has(t.path),
}));

export function themeLabelForPath(themePath: string | undefined | null): string {
  const path = String(themePath || "default").trim() || "default";
  return PROJECT_THEMES.find((t) => t.path === path)?.label ?? path;
}

export function isKnownThemePath(themePath: string): boolean {
  return PROJECT_THEMES.some((t) => t.path === themePath);
}
