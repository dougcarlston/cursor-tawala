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
const testDriveSupport = readFileSync(
  join(dir, "../../TawalaWebapp-build1700/src/com/tawala/web/library/TestDriveSupport.java"),
  "utf8"
);
assert.doesNotMatch(
  testDriveSupport,
  /fullyInitializedResponsesFor/,
  "B1: new /t/ Worlds must not snapshot live /p/ submissions"
);
assert.match(testDriveSupport, /Intentionally empty/);
assert.match(testDriveSupport, /LEAVE_GRACE_MS = 20 \* 1000L/);
assert.match(testDriveSupport, /IDLE_MS = 30 \* 60 \* 1000L/);
assert.match(
  testDriveSupport,
  /purgeTestDriveSessionData/,
  "B2: leave-wipe must drop session variables (Customize_Title), not only the World"
);
const executionContext = readFileSync(
  join(dir, "../../TawalaWebapp-build1700/src/com/tawala/project/commands/ExecutionContext.java"),
  "utf8"
);
assert.match(executionContext, /public static void purgeTestDriveSessionData/);
const defaultJs = readFileSync(
  join(dir, "../../TawalaWebapp-build1700/web/scripts/project/default.js"),
  "utf8"
);
assert.match(defaultJs, /bindTestDriveLeaveWipe/);
assert.match(defaultJs, /\/t\//);
{
	const assigned = defaultJs.indexOf("Tawala.bindTestDriveLeaveWipe = function");
	const ready = defaultJs.indexOf("Event.onDOMReady(Tawala.bindTestDriveLeaveWipe)");
	assert.ok(
		assigned >= 0 && ready > assigned,
		"onDOMReady(bindTestDriveLeaveWipe) must run after the function is assigned"
	);
}
assert.doesNotMatch(
  ops,
  /addEventListener\("pagehide"/,
  "leave-wipe lives on the :8080 /t/ form, not the :5500 listing"
);

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
