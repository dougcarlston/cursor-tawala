/**
 * Task #14 contract: honesty copy must stay explicit (wipe-on-start, not leave;
 * shared Library uniqueId). Run: node js/testDriveHonesty.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const dir = dirname(fileURLToPath(import.meta.url));
const demo = readFileSync(join(dir, "demo-urls.js"), "utf8");
const ops = readFileSync(join(dir, "project-ops.js"), "utf8");

assert.match(demo, /window\.TAWALA_TEST_DRIVE_HONESTY/);
assert.match(demo, /Clears this Library demo when you start \(not when you close the tab\)/);
assert.match(demo, /same uniqueId for every visitor/);
assert.match(demo, /Post-tab-close purge is not available in this static mock — do not fake it/);
assert.match(demo, /Closing the tab does not wipe/);

assert.match(ops, /honestyText\(\s*"copyAlert"/);
assert.match(ops, /honestyText\(\s*"tooltipSingle"/);
assert.match(ops, /honestyNamed\(\s*"pickerOpenLede"/);
assert.doesNotMatch(
  ops,
  /window\.alert\("Link copied"\)/,
  "Copy-link alert must use honesty copy, not a bare “Link copied”"
);

console.log("testDriveHonesty contract ok");
