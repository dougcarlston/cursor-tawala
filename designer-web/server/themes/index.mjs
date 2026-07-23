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
/* Horizontal FIB layout (default/baseball fallback — dirtbowl2 layers its own).
   Mirror Deploy form-layout-core contracts (Preview-only; Design canvas unchanged). */
.fib-row { display: flex; flex-wrap: nowrap; align-items: flex-end; gap: 8px 12px; margin-bottom: 0; }
/* Shared label slot so field boxes left-align across FIB rows (owner Jul 22). */
.fib-row .fib-label { flex: 0 0 var(--tawala-fib-label-width, 9.5em); min-width: var(--tawala-fib-label-width, 9.5em); max-width: var(--tawala-fib-label-width, 9.5em); padding-top: 0; padding-bottom: 4px; box-sizing: border-box; text-align: left; }
.fib-style-rightAlignLabels .fib-row .fib-label,
.fib-style-rightAlignLabelsJustified .fib-row .fib-label { text-align: right; }
.fib-row .fib-fields { flex: 1 1 auto; display: flex; flex-wrap: nowrap; align-items: flex-end; gap: 8px 12px; min-width: 0; }
.fib-row .fib-fields .fib-field { flex: 0 1 auto; min-width: 0; }
.fib-row .fib-fields input[type=text] { max-width: 100%; min-width: 3ch; box-sizing: content-box; }
/* Stack FIB items (do not sit Email | Address side-by-side). */
.fib:not(.fib-style-topLabels) {
  display: block;
  width: 100%;
  max-width: var(--tawala-fib-table-max-width, 36em);
  margin: 0 0 0.75em 0;
  box-sizing: border-box;
}
/* Align right side (…Justified): shared field-column right edge = farthest blank or trailing text. */
.fib-style-leftAlignLabelsJustified .fib-fields,
.fib-style-rightAlignLabelsJustified .fib-fields {
  flex: 1 1 0;
  width: auto;
  gap: 0.35em;
}
.fib-style-leftAlignLabelsJustified .fib-fields > .fib-field,
.fib-style-rightAlignLabelsJustified .fib-fields > .fib-field {
  flex: 1 1 0;
  min-width: 8ch;
}
.fib-style-leftAlignLabelsJustified .fib-fields > .fib-field input[type=text],
.fib-style-rightAlignLabelsJustified .fib-fields > .fib-field input[type=text] {
  width: 100%;
  max-width: 100%;
  min-width: 8ch;
  box-sizing: border-box;
}
.fib-style-leftAlignLabelsJustified .fib-fields > .fib-inline-text,
.fib-style-rightAlignLabelsJustified .fib-fields > .fib-inline-text {
  flex: 0 0 auto;
  white-space: nowrap;
}
.fib-row.fib-row-stacked {
  display: flex;
  flex-wrap: nowrap;
  justify-content: space-between;
  align-items: baseline;
  width: 100%;
  gap: 0.25em 0.5em;
  line-height: 1.8;
  box-sizing: border-box;
}
.fib-row.fib-row-stacked .fib-inline-text { white-space: pre-wrap; }
.fib-row.fib-row-stacked .fib-field { display: inline; flex: 0 0 auto; }
.fib-row.fib-row-stacked input[type=text] {
  display: inline-block;
  vertical-align: baseline;
  margin: 0 0.15em;
  font: inherit;
  max-width: 100%;
  box-sizing: content-box;
}
.fib-row.fib-top-label { flex-direction: row; align-items: center; }
.fib-top-label-text { flex: 0 0 140px; min-width: 100px; font-size: 13px; color: #444; }
.fib-top-label-field { flex: 0 1 auto; }
.fib-top-label-field input { box-sizing: content-box; }
.preview-itemization-controls { margin: 0.35rem 0 0.5rem; font-size: 13px; }
.preview-itemization-controls a { color: #000080; margin-right: 12px; }
.preview-itemization-table {
  width: fit-content;
  max-width: min(var(--tawala-list-table-max-width, 6in), 100%);
  overflow-x: auto;
}
.preview-itemization-table table {
  width: max-content;
  max-width: min(var(--tawala-list-table-max-width, 6in), 100%);
  table-layout: auto;
  border-collapse: collapse;
  margin: 0.75em 0 1em;
  font-size: 14px;
}
.preview-itemization-table th {
  background: #555;
  color: #fff;
  font-weight: normal;
  text-align: left;
  padding: 4px 10px;
  border: 1px solid #444;
  white-space: nowrap;
}
.preview-itemization-table td {
  border: 1px solid #ccc;
  padding: 6px 10px;
  background: #fff;
  white-space: nowrap;
}
.validation-error { background: #fdecea; border: 1px solid #e74c3c; color: #922; padding: 8px 12px; margin: 0 0 1rem; }

/* Text item Styles (Format → Styles → Text…) — Design + Deploy parity for Form
   Preview and Document runtime pages. Bold/italic defaults come from export XML;
   container keeps italic+color so Design “unbold” survives Deploy. */
div.text.instructional,
.text-block.instructional {
  font-weight: normal;
  font-style: italic;
  color: #000080;
  background: transparent !important;
}
div.text.instructional b,
div.text.instructional strong,
.text-block.instructional b,
.text-block.instructional strong {
  font-weight: bold;
}
div.text.error,
div.text.text-item-error,
.text-block.error {
  font-weight: normal;
  font-style: italic;
  color: #c00000;
  background: transparent !important;
}
div.text.error b,
div.text.error strong,
div.text.text-item-error b,
div.text.text-item-error strong,
.text-block.error b,
.text-block.error strong {
  font-weight: bold;
}
/* Default black from Design export must not block item Style (same as canvas). */
div.text.instructional font[color="000000"],
div.text.instructional font[color="#000000"],
div.text.instructional font[color="#000"],
div.text.error font[color="000000"],
div.text.error font[color="#000000"],
div.text.error font[color="#000"],
div.text.text-item-error font[color="000000"],
div.text.text-item-error font[color="#000000"],
div.text.text-item-error font[color="#000"],
div.text.instructional [style*="color: rgb(0, 0, 0)"],
div.text.instructional [style*="color:rgb(0, 0, 0)"],
div.text.instructional [style*="color: #000"],
div.text.instructional [style*="color:#000"],
div.text.error [style*="color: rgb(0, 0, 0)"],
div.text.error [style*="color:rgb(0, 0, 0)"],
div.text.error [style*="color: #000"],
div.text.error [style*="color:#000"],
div.text.text-item-error [style*="color: rgb(0, 0, 0)"],
div.text.text-item-error [style*="color:rgb(0, 0, 0)"],
div.text.text-item-error [style*="color: #000"],
div.text.text-item-error [style*="color:#000"] {
  color: inherit !important;
}
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
