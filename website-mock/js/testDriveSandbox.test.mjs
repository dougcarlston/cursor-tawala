/**
 * Library Test Drive uses Java session sandbox, not the live /p/{uniqueId} pile.
 * Run: node js/testDriveSandbox.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";

const dir = dirname(fileURLToPath(import.meta.url));
const demo = readFileSync(join(dir, "demo-urls.js"), "utf8");
const ops = readFileSync(join(dir, "project-ops.js"), "utf8");

assert.match(demo, /isJavaSessionTestDriveUrl/);
assert.match(demo, /javaTestDrivePrepareUrl/);
assert.match(demo, /libraryTestDriveUrlForStart/);
assert.match(demo, /\/projectmanager\/testdrive\?id=/);
assert.match(
  demo,
  /const sandbox = this\.isJavaSessionTestDriveUrl\(target\);/,
  "openTestDrive must detect the session gate"
);
assert.match(demo, /const purgeFirst = !sandbox && options\.purge !== false/);
assert.match(demo, /const formProbe = sandbox \? \{ ok: true, skipped: true \}/);
assert.match(
  demo,
  /const customize = list\.find\(\(s\) => \/\^customiz\/i\.test\(labelOf\(s\)\)\);/,
  "Library TD must prefer Customize so Sign-up actually customizes"
);
assert.match(ops, /function libraryOpenUrlForStart/);
assert.match(ops, /libraryTestDriveUrlForStart/);
assert.doesNotMatch(
  ops.slice(ops.indexOf("function openLibraryTestDrivePicker"), ops.indexOf("async function copyLibraryTestDriveLink")),
  /purge: true/
);
assert.match(
  ops.slice(ops.indexOf("function openLibraryTestDrivePicker"), ops.indexOf("async function copyLibraryTestDriveLink")),
  /classList\.toggle\("is-open"/,
  "Picking a start marks it and leaves the picker open"
);

console.log("testDriveSandbox contract ok");
