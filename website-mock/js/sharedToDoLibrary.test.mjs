/**
 * Shared To-Do Library listing: catalog JSON is the full three-start app.
 * Test Drive prefers Setup; Copy to MyTawala clones this JSON (not a Setup-only sibling).
 * Run: node js/sharedToDoLibrary.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const dir = dirname(fileURLToPath(import.meta.url));
const root = join(dir, "..");
const demo = readFileSync(join(dir, "demo-urls.js"), "utf8");
const json = JSON.parse(readFileSync(join(root, "projects/library/Shared To-Do.json"), "utf8"));

assert.equal(json.name, "Shared To-Do");
assert.equal(json.deployUniqueId, undefined);
assert.equal(json.deployIdentityName, undefined);
const starts = (json.forms || []).filter((f) => f && f.startPoint === true).map((f) => f.name);
assert.deepEqual([...starts].sort(), ["Administration", "Setup", "Signup"]);

const start = demo.indexOf('"shared-to-do":');
assert.ok(start >= 0, "catalog must include shared-to-do");
const catalog = demo.slice(start, start + 2200);
assert.match(catalog, /"name": "Shared To-Do"/);
assert.match(catalog, /projects\/library\/Shared To-Do\.json/);
assert.match(catalog, /bx44wpdnspbsi3k/);
assert.doesNotMatch(catalog, /qggyhqoy8m4td23/, "Library uniqueId must not be the owner My Tawala copy");
assert.match(catalog, /cswcunp\.Setup/);

console.log("sharedToDoLibrary contract ok");
