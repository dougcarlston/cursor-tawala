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
const transfer = readFileSync(join(dir, "transfer.js"), "utf8");

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

const honestySrc = demo.slice(
  demo.indexOf("window.TAWALA_TEST_DRIVE_HONESTY"),
  demo.indexOf("window.TAWALA_DATA_DRIVEN_TEST_DRIVE_TITLE")
);
assert.doesNotMatch(
  honestySrc,
  /:8080/,
  "Library Test Drive honesty copy must not mention :8080"
);
assert.match(demo, /nothing you enter is saved/);
assert.match(demo, /rowDemoBadge:/);
assert.match(ops, /function renderLibraryDemoBadge/);
assert.match(ops, /projectIsDataDriven\(project\)/);
assert.match(ops, /function isLibraryMultiStart/);
assert.match(
  ops,
  /return TawalaTransfer\.getTestDriveDoor\(project\.id\)\.mode === "picker"/,
  "Library Test Drive picker is admin opt-in; default is a single door"
);
assert.doesNotMatch(
  ops.slice(ops.indexOf("function renderLibraryTestDriveButton"), ops.indexOf("function renderLibraryCopyTestDriveButton")),
  /if \(projectIsDataDriven\(project\)\)/
);
assert.match(ops, /function bindDismissOnBackdropClick/);
assert.match(ops, /Choose a start\./);
assert.match(ops, /publishDescInput/);
assert.doesNotMatch(
  ops.slice(
    ops.indexOf("function openLibraryTestDrivePicker"),
    ops.indexOf("async function copyLibraryTestDriveLink")
  ),
  /pickerHint/
);

assert.match(demo, /tooltipSingleEmail:/);
assert.match(demo, /this app sends real email/i);
assert.match(demo, /isSendsRealEmailProject/);
assert.match(demo, /rowEmailBadge:/);
assert.match(ops, /projectSendsRealEmail\(project\)/);
assert.match(ops, /library-email-badge/);

assert.match(demo, /isSharedToDoProject/);
assert.match(demo, /tooltipSingleTodo:/);
assert.match(demo, /Opens Setup only/);
assert.match(ops, /function projectIsSharedToDo/);
assert.match(
  demo,
  /if \(project && this\.isSharedToDoProject\(project\)\)/,
  "Library Test Drive must prefer Setup for Shared To-Do"
);
assert.match(transfer, /TEST_DRIVE_DOOR_KEY/);
assert.match(transfer, /function setTestDriveDoor/);
assert.match(transfer, /function getTestDriveDoor/);

console.log("testDriveHonesty contract ok");
