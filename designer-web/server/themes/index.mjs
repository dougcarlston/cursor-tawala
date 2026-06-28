import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));

const BUILTIN = {
  default: `body.tawala-theme-default { font-family: Arial, sans-serif; max-width: 800px; margin: 2rem auto; padding: 0 1rem; line-height: 1.4; }
.dev-banner { background: #fff3cd; border: 1px solid #ffc107; padding: 8px 12px; margin-bottom: 1rem; font-size: 13px; }`,
};

export function getThemeCss(themePath) {
  const name = themePath || "default";
  if (name === "default") return BUILTIN.default;
  const file = path.join(__dirname, `${name}.css`);
  if (fs.existsSync(file)) return fs.readFileSync(file, "utf8");
  return BUILTIN.default;
}

export function themeBodyClass(themePath) {
  const name = themePath || "default";
  return `tawala-theme-${name.replace(/[^a-zA-Z0-9_-]/g, "")}`;
}
