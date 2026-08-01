#!/usr/bin/env node
/**
 * List public Library catalog entries currently marked as stubs (temporary
 * reminders — placeholders / converted-but-unverified projects, NOT real
 * working demos). Read-only: does not edit demo-urls.js or move any files.
 *
 * A Library entry is a stub when `stub: true` is set in js/demo-urls.js
 * (TAWALA_LIBRARY). Its display `name` also carries a visible " (stub)"
 * suffix so the Library listing shows what still needs replacing.
 * `liveReady: true` entries (Simple Survey, Sign-up Sheet, Potluck, Get
 * Together, Horses and Penguins Test, Multiple Question Survey Template)
 * are never stubs. Sign-up Sheet w Email was retired from TAWALA_LIBRARY
 * (Aug 1, 2026) — Designer New Project only, not listed here.
 *
 * Usage:
 *   node scripts/list-library-stubs.mjs            # table of all stubs
 *   node scripts/list-library-stubs.mjs --ids       # ids only, one per line
 *   node scripts/list-library-stubs.mjs --json      # id/name/category/jsonFile as JSON
 *
 * Retirement path (owner asks an agent to run this; see README.md
 * "Retiring Library stubs"): the correct long-term route is Designer →
 * Deploy → Publish once Publish is wired. Until then, an agent retiring a
 * stub by hand should, per id:
 *   1. Confirm the equivalent working copy (Designer project / Deploy) is
 *      ready, or the owner explicitly wants the stub gone without a
 *      replacement.
 *   2. Remove the entry from TAWALA_LIBRARY in js/demo-urls.js — and, if
 *      moving it to My Tawala instead of deleting outright, add an
 *      equivalent entry to TAWALA_MYTAWALA with the " (stub)" suffix and
 *      `stub: true` dropped (My Tawala is private; the public-facing stub
 *      marker no longer applies there).
 *   3. Move the backing JSON from projects/library/<file> to
 *      projects/mytawala/<file> when moving to My Tawala (or delete it
 *      when just retiring the placeholder outright).
 * There is intentionally NO public Library Delete control — this stays an
 * agent/maintainer-run operation, never a button on library.html.
 */
import { readFileSync } from "node:fs";
import { fileURLToPath } from "node:url";
import path from "node:path";
import vm from "node:vm";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const demoUrlsPath = path.join(__dirname, "..", "js", "demo-urls.js");

function loadCatalog() {
  const src = readFileSync(demoUrlsPath, "utf8");
  const sandbox = { window: {}, console };
  vm.createContext(sandbox);
  vm.runInContext(src, sandbox, { filename: demoUrlsPath });
  return {
    library: sandbox.window.TAWALA_LIBRARY || {},
    mytawala: sandbox.window.TAWALA_MYTAWALA || {},
  };
}

function stubEntries(library) {
  return Object.keys(library)
    .map((id) => ({ id, ...library[id] }))
    .filter((p) => p.stub === true)
    .sort((a, b) => a.id.localeCompare(b.id));
}

function main() {
  const { library } = loadCatalog();
  const stubs = stubEntries(library);
  const args = process.argv.slice(2);

  if (args.includes("--ids")) {
    stubs.forEach((s) => console.log(s.id));
    return;
  }
  if (args.includes("--json")) {
    console.log(
      JSON.stringify(
        stubs.map((s) => ({
          id: s.id,
          name: s.name,
          category: s.category,
          jsonFile: s.jsonFile,
        })),
        null,
        2
      )
    );
    return;
  }

  console.log(
    `Library stubs (${stubs.length}) — temporary reminders, not liveReady:\n`
  );
  const idW = Math.max(2, ...stubs.map((s) => s.id.length));
  const nameW = Math.max(4, ...stubs.map((s) => s.name.length));
  stubs.forEach((s) => {
    console.log(
      `${s.id.padEnd(idW)}  ${s.name.padEnd(nameW)}  ${s.category}  ->  ${s.jsonFile}`
    );
  });
  console.log(
    "\nCorrect long-term route: Designer -> Deploy -> Publish (once Publish is wired).\n" +
      'Until then, ask an agent: "retire stub <id>" or "move all stubs to My Tawala".\n' +
      "See website-mock/README.md \u00a7 Retiring Library stubs."
  );
}

main();
