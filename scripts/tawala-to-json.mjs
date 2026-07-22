#!/usr/bin/env node
/**
 * Convert a legacy `.tawala` (or project `.xml`) to browser Designer JSON (format 2.0).
 *
 * Usage (repo root):
 *   node scripts/tawala-to-json.mjs <input.tawala|xml> [output.json]
 *
 * Warnings go to stderr. Summary + output path go to stdout.
 * Default output: same path as input with `.json` extension.
 */

import fs from "node:fs";
import path from "node:path";
import { fileURLToPath, pathToFileURL } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "..");
const converterUrl = pathToFileURL(
  path.join(REPO_ROOT, "designer-web/src/lib/tawalaXmlToJson.mjs"),
).href;

const { convertTawalaXmlToProject, countProcessCommands } = await import(converterUrl);

function usage() {
  console.error("Usage: node scripts/tawala-to-json.mjs <input.tawala|xml> [output.json]");
  process.exit(2);
}

function defaultOutputPath(inputPath) {
  const lower = inputPath.toLowerCase();
  if (lower.endsWith(".tawala.xml")) return inputPath.slice(0, -".tawala.xml".length) + ".json";
  if (lower.endsWith(".tawala")) return inputPath.slice(0, -".tawala".length) + ".json";
  if (lower.endsWith(".xml")) return inputPath.slice(0, -".xml".length) + ".json";
  return inputPath + ".json";
}

function main() {
  const inputArg = process.argv[2];
  const outputArg = process.argv[3];
  if (!inputArg || inputArg === "-h" || inputArg === "--help") usage();

  const inputPath = path.resolve(process.cwd(), inputArg);
  if (!fs.existsSync(inputPath)) {
    console.error(`Input not found: ${inputPath}`);
    process.exit(1);
  }

  const outputPath = path.resolve(
    process.cwd(),
    outputArg ?? defaultOutputPath(inputPath),
  );

  const xml = fs.readFileSync(inputPath, "utf8");
  const sourceLabel = path.relative(REPO_ROOT, inputPath) || inputPath;

  let project;
  let warnings;
  try {
    ({ project, warnings } = convertTawalaXmlToProject(xml, { sourceLabel }));
  } catch (err) {
    console.error(err instanceof Error ? err.message : String(err));
    process.exit(1);
  }

  for (const w of warnings) {
    console.error(`[warn] ${w}`);
  }

  fs.mkdirSync(path.dirname(outputPath), { recursive: true });
  fs.writeFileSync(outputPath, JSON.stringify(project, null, 2) + "\n", "utf8");

  const forms = project.forms ?? [];
  const processes = project.processes ?? [];
  const documents = project.documents ?? [];
  const images = project.images ?? [];
  const items = forms.reduce((n, f) => n + (f.items?.length ?? 0), 0);
  const commands = processes.reduce((n, p) => n + countProcessCommands(p.commands), 0);

  console.log(`Wrote ${outputPath}`);
  console.log(
    `Forms: ${forms.length}, processes: ${processes.length}, documents: ${documents.length}, images: ${images.length}, warnings: ${warnings.length}`,
  );
  console.log(`Items: ${items}, commands: ${commands}`);
}

main();
