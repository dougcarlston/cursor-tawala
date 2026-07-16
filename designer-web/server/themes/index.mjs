import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

/** Base form readability — always included; theme CSS layers on top. */
export const BASE_FORM_CSS = `body { font-family: Arial, Helvetica, sans-serif; max-width: 800px; margin: 2rem auto; padding: 0 1rem; line-height: 1.4; color: #222; }
h1 { font-size: 1.25rem; color: #333; border-bottom: 1px solid #ccc; padding-bottom: 0.5rem; }
.dev-banner { background: #fff3cd; border: 1px solid #ffc107; padding: 8px 12px; margin-bottom: 1rem; font-size: 13px; }
fieldset.mc { margin: 1rem 0; border: 1px solid #ccc; padding: 8px 12px; }
.preview-mc-choice { display: block; margin: 4px 0; }
.fib label { display: inline-block; margin: 4px 8px 4px 0; }
input[type=text], input[type=submit], select, textarea { font-family: inherit; font-size: 14px; }
input[type=submit] { margin-top: 1rem; padding: 8px 24px; font-size: 14px; }
.text-block p, .text p { margin: 0.5rem 0; }
/* Horizontal FIB layout (default/baseball fallback — dirtbowl2 layers its own) */
.fib-row { display: flex; flex-wrap: wrap; align-items: flex-start; gap: 8px 12px; margin-bottom: 0.45rem; }
.fib-row .fib-label { flex: 0 0 160px; min-width: 120px; padding-top: 4px; }
.fib-row .fib-fields { flex: 1 1 240px; display: flex; flex-wrap: wrap; align-items: flex-end; gap: 8px 16px; }
.fib-row.fib-top-label { flex-direction: row; align-items: center; }
.fib-top-label-text { flex: 0 0 140px; min-width: 100px; font-size: 13px; color: #444; }
.fib-top-label-field { flex: 1 1 200px; }
.fib-top-label-field input { width: 100%; max-width: 28em; box-sizing: border-box; }
.validation-error { background: #fdecea; border: 1px solid #e74c3c; color: #922; padding: 8px 12px; margin: 0 0 1rem; }
`;

const BUILTIN = {
  default: `body.tawala-theme-default { font-family: Arial, Helvetica, sans-serif; max-width: 800px; margin: 2rem auto; padding: 0 1rem; line-height: 1.4; }
body.tawala-theme-default .dev-banner { background: #fff3cd; border: 1px solid #ffc107; padding: 8px 12px; margin-bottom: 1rem; font-size: 13px; }`,
};

/**
 * Resolve themePath to a CSS file that exists, or fall back to builtin default.
 * Critical: body class name must match selectors in the returned CSS
 * (e.g. missing "baseball.css" → name "default" + body.tawala-theme-default).
 * @returns {{ name: string, css: string }}
 */
export function resolveTheme(themePath) {
  const requested = String(themePath || "default").trim() || "default";
  const safe = requested.replace(/[^a-zA-Z0-9_-]/g, "");
  if (!safe || safe === "default") {
    return { name: "default", css: BUILTIN.default };
  }
  const file = path.join(__dirname, `${safe}.css`);
  if (fs.existsSync(file)) {
    return { name: safe, css: fs.readFileSync(file, "utf8") };
  }
  return { name: "default", css: BUILTIN.default };
}

export function getThemeCss(themePath) {
  return resolveTheme(themePath).css;
}

export function themeBodyClass(themePath) {
  const { name } = resolveTheme(themePath);
  return `tawala-theme-${name}`;
}
