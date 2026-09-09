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

assert.match(ops, /honestyForProject\(\s*project,\s*"copyAlert"/);
assert.match(ops, /honestyText\(\s*"tooltipSingle"/);
assert.match(ops, /honestyNamedForProject\(/);
assert.match(ops, /honestyForProject\(\s*project,\s*"tooltipSingle"/);
assert.match(demo, /tooltipSingleKeep:/);
assert.match(demo, /copyAlertKeep:/);
assert.match(demo, /Stored answers stay/);
assert.doesNotMatch(
  ops,
  /window\.alert\("Link copied"\)/,
  "Copy-link alert must use honesty copy, not a bare “Link copied”"
);

assert.match(demo, /tooltipSingleExam:/);
assert.match(demo, /nothing you enter is saved/);
assert.match(demo, /rowDemoBadge:/);
assert.match(ops, /function renderLibraryDemoBadge/);
assert.match(ops, /projectIsDataDriven\(project\)/);
assert.match(ops, /function isLibraryMultiStart/);
assert.match(
  ops,
  /if \(projectIsDataDriven\(project\)\) return false;/,
  "Data-driven exam apps must skip Library Test Drive start picker"
);
assert.doesNotMatch(
  ops.slice(ops.indexOf("function renderLibraryTestDriveButton"), ops.indexOf("function renderLibraryCopyTestDriveButton")),
  /if \(projectIsDataDriven\(project\)\)/
);
assert.doesNotMatch(demo, /purgeRespondentResponses/);

assert.match(demo, /tooltipSingleEmail:/);
assert.match(demo, /this app sends real email/i);
assert.match(demo, /isSendsRealEmailProject/);
assert.match(demo, /rowEmailBadge:/);
assert.match(ops, /projectSendsRealEmail\(project\)/);
assert.match(ops, /library-email-badge/);
assert.match(
  ops,
  /if \(projectSendsRealEmail\(project\)\) return false;/,
  "Send-mail apps must skip Library Test Drive start picker"
);

console.log("testDriveHonesty contract ok");
