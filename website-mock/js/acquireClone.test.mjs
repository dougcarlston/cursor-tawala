/**
 * Task #26 — private uniqueId on Copy to MyTawala.
 * Run: node js/acquireClone.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import {
  catalogPathForOpenApi,
  projectBodyForPrivateClone,
  snapshotProjectAfterClone,
  uniqueIdsEqual,
  hasMyTawalaCloneDefinitionSource,
  forkRuntimeShouldScrub,
  MAKE_COPY_NO_LIVE_FORM_ERROR,
  CLONE_TRY_AGAIN_ERROR,
  isDataDrivenProject,
  formsFromExport,
  DATA_DRIVEN_NO_TEST_DRIVE_TITLE,
  PUBLISH_STRIP_CONFIRM,
  countShowsSavedResponses,
} from "./acquireClone.mjs";

const dir = dirname(fileURLToPath(import.meta.url));
const transfer = readFileSync(join(dir, "transfer.js"), "utf8");
const ops = readFileSync(join(dir, "project-ops.js"), "utf8");

const stock = {
  name: "Simple Survey Template",
  deployIdentityName: "Simple Survey Template",
  uniqueId: "gy1zssbrwm4fgfm",
  _freshFromTemplate: false,
  forms: [{ name: "Survey" }],
};

const cloneBody = projectBodyForPrivateClone(stock, "My Survey");
assert.equal(cloneBody.name, "My Survey");
assert.equal(cloneBody._freshFromTemplate, true);
assert.equal(cloneBody.forms[0].name, "Survey");
assert.equal("deployIdentityName" in cloneBody, false);
assert.equal("uniqueId" in cloneBody, false);
assert.equal("deployUniqueId" in cloneBody, false);

const snap = snapshotProjectAfterClone(stock, {
  displayName: "My Survey",
  deployIdentityName: "My Survey abcd1234",
  uniqueId: "privatenewid01",
});
assert.equal(snap._freshFromTemplate, undefined);
assert.equal(snap.deployIdentityName, "My Survey abcd1234");
assert.equal(snap.deployUniqueId, "privatenewid01");
assert.equal(snap.name, "My Survey");

assert.equal(uniqueIdsEqual("gy1zssbrwm4fgfm", "gy1zssbrwm4fgfm"), true);
assert.equal(uniqueIdsEqual("gy1zssbrwm4fgfm", "privatenewid01"), false);
assert.equal(
  catalogPathForOpenApi("website-mock/projects/library/Online Exam Builder.json"),
  "projects/library/Online Exam Builder.json"
);
assert.equal(
  catalogPathForOpenApi("designer-web/public/samples/templates/simple-survey.json"),
  "designer-web/public/samples/templates/simple-survey.json"
);

assert.match(transfer, /cloneLibraryProjectToPrivateRuntime/);
assert.match(transfer, /scrubSharedAcquireRuntimes/);
assert.match(transfer, /_freshFromTemplate:\s*true/);
assert.match(transfer, /mockSharedLibraryRuntime:\s*false/);
assert.doesNotMatch(
  transfer.slice(transfer.indexOf("function saveCopyFromLibrary")),
  /const live = liveRuntimeFromLibrarySource\(source\)/
);
assert.match(ops, /await TawalaTransfer\.saveCopyFromLibrary/);
assert.match(ops, /Task #26: never open catalog JSON for a Library acquire/);
assert.match(ops, /ownerFacingVersionDescription/);
assert.doesNotMatch(
  transfer.slice(transfer.indexOf("function saveCopyFromLibrary")),
  /Copied from Library \(\$\{/
);
assert.match(transfer, /function isAutoLibraryAcquireVersionNote/);
assert.match(transfer, /function scrubAutoLibraryAcquireVersionNotes/);

function isAutoLibraryAcquireVersionNote(text) {
  const s = String(text == null ? "" : text).trim();
  if (!s) return false;
  return /^Copied from Library(?:\s*\([^)]*\))?\s*$/i.test(s);
}
function ownerFacingVersionDescription(text) {
  const s = String(text == null ? "" : text).trim();
  if (!s || isAutoLibraryAcquireVersionNote(s)) return "";
  return s;
}
assert.equal(ownerFacingVersionDescription("Copied from Library"), "");
assert.equal(ownerFacingVersionDescription("Copied from Library (Online Exam Builder)"), "");
assert.equal(ownerFacingVersionDescription("Copied from Library (Get Together)"), "");
assert.equal(ownerFacingVersionDescription("Ready for Friday quiz"), "Ready for Friday quiz");
assert.equal(ownerFacingVersionDescription(""), "");

assert.equal(MAKE_COPY_NO_LIVE_FORM_ERROR, "Can't copy until the original has a live form.");
assert.equal(
  CLONE_TRY_AGAIN_ERROR,
  "Couldn't copy the live form. Start Tomcat and the Designer API, then try again."
);
assert.equal(
  hasMyTawalaCloneDefinitionSource({ snapshotId: "abc" }),
  true
);
assert.equal(hasMyTawalaCloneDefinitionSource({ jsonFile: "projects/library/x.json" }), true);
assert.equal(hasMyTawalaCloneDefinitionSource({ uniqueId: "liveid01" }), true);
assert.equal(hasMyTawalaCloneDefinitionSource({ name: "Empty" }), false);
assert.equal(
  forkRuntimeShouldScrub({ uniqueId: "srcid", mockSharedForkRuntime: false }, "srcid"),
  true
);
assert.equal(
  forkRuntimeShouldScrub(
    { uniqueId: "newid", mockSharedForkRuntime: false, deployIdentityName: "Copy abcd" },
    "srcid"
  ),
  false
);
assert.equal(forkRuntimeShouldScrub({ uniqueId: "newid", mockSharedForkRuntime: true }, "srcid"), true);

assert.match(transfer, /cloneDefinitionToPrivateRuntime/);
assert.match(transfer, /cloneMyTawalaProjectToPrivateRuntime/);
assert.match(transfer, /async function makeCopyOfMyTawalaProject/);
assert.match(transfer, /Can't copy until the original has a live form/);
assert.match(ops, /await TawalaTransfer\.makeCopyOfMyTawalaProject/);
const makeCopySrc = transfer.slice(
  transfer.indexOf("async function makeCopyOfMyTawalaProject"),
  transfer.indexOf("function isLibraryAdmin")
);
assert.match(makeCopySrc, /uniqueId:\s*clone\.uniqueId/);
assert.doesNotMatch(makeCopySrc, /uniqueId:\s*null/);
assert.match(makeCopySrc, /await cloneMyTawalaProjectToPrivateRuntime/);
assert.match(transfer, /forkRuntimeShouldScrub/);
assert.match(
  transfer.slice(transfer.indexOf("function scrubSharedForkRuntimes")),
  /if \(!forkRuntimeShouldScrub/
);

assert.equal(
  isDataDrivenProject({
    jsonFile: "projects/library/Online Exam Builder.json",
    name: "Online Exam Builder",
  }),
  true
);
assert.equal(DATA_DRIVEN_NO_TEST_DRIVE_TITLE.includes("My Tawala"), true);
assert.equal(formsFromExport([{ form: "Exam", rows: [{}] }]).length, 1);
assert.deepEqual(
  formsFromExport([
    { form: "Exam", rows: [{}] },
    { form: "Question", rows: [{}] },
    { form: "SetupVariables", rows: [{}] },
  ]).map((f) => f.form),
  ["Exam", "Question", "SetupVariables"]
);
assert.match(transfer, /maybeCopyResponsesOntoClone/);
assert.match(transfer, /copyResponsesToUniqueId/);
assert.doesNotMatch(transfer, /isDataDrivenProject\(source\)/);
assert.match(ops, /dataDrivenNoTestDriveTitle/);

const libCloneSrc = transfer.slice(
  transfer.indexOf("async function cloneLibraryProjectToPrivateRuntime"),
  transfer.indexOf("function isLibraryAcquireEntry")
);
assert.doesNotMatch(
  libCloneSrc,
  /maybeCopyResponsesOntoClone/,
  "Library acquire must not copy catalog/author submissions"
);
assert.doesNotMatch(libCloneSrc, /copyResponsesToUniqueId/);

const cloneMineSrc = transfer.slice(
  transfer.indexOf("async function cloneMyTawalaProjectToPrivateRuntime"),
  transfer.indexOf("async function makeCopyOfMyTawalaProject")
);
assert.match(
  cloneMineSrc,
  /maybeCopyResponsesOntoClone/,
  "Make a Copy of own project must still copy all response data"
);

assert.equal(
  PUBLISH_STRIP_CONFIRM,
  "Project will be stripped of data upon publication. Proceed?"
);
assert.match(transfer, /Project will be stripped of data upon publication\. Proceed\?/);
assert.equal(countShowsSavedResponses({ status: "success", count: 12 }), true);
assert.equal(countShowsSavedResponses({ status: "success", count: 0 }), false);
assert.equal(countShowsSavedResponses({ status: "failure", count: 9 }), false);

const publishSrc = transfer.slice(
  transfer.indexOf("async function publishToLibrary"),
  transfer.indexOf("async function publishToLibraryAfterStripConfirm")
);
assert.match(publishSrc, /needsStripConfirm:\s*true/);
assert.match(publishSrc, /stripConfirmed !== true/);
assert.match(publishSrc, /purgeAfterPublish/);
const countAt = publishSrc.indexOf("countResponses");
const upsertAt = publishSrc.indexOf("upsertLibraryOverlay");
const needsAt = publishSrc.indexOf("needsStripConfirm");
assert.ok(countAt >= 0 && needsAt > countAt && upsertAt > needsAt);
assert.match(transfer, /async function publishToLibraryAfterStripConfirm/);
assert.match(ops, /publishToLibraryAfterStripConfirm/);
assert.doesNotMatch(ops, /publishPurgeCheckbox/);
assert.doesNotMatch(ops, /keepResponses: !purgeOnPublish/);

console.log("acquireClone contract ok");
