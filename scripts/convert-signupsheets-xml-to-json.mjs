#!/usr/bin/env node
/**
 * Regenerate SignupSheets JSON via the shared `.tawala` converter.
 *
 * Usage (repo root):
 *   node scripts/convert-signupsheets-xml-to-json.mjs
 *
 * Reads:  designer-web/public/samples/legacy/SignupSheets.tawala.xml
 * Writes: designer-web/public/samples/signup-sheets.json
 */

import path from "node:path";
import { fileURLToPath } from "node:url";
import { spawnSync } from "node:child_process";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "..");
const INPUT = path.join(
  REPO_ROOT,
  "designer-web/public/samples/legacy/SignupSheets.tawala.xml",
);
const OUTPUT = path.join(REPO_ROOT, "designer-web/public/samples/signup-sheets.json");
const CLI = path.join(REPO_ROOT, "scripts/tawala-to-json.mjs");

const result = spawnSync(process.execPath, [CLI, INPUT, OUTPUT], {
  cwd: REPO_ROOT,
  stdio: "inherit",
});
process.exit(result.status ?? 1);
