/**
 * Publish keepResponses + full-copy (not same-uniqueId purge) contract.
 * Run: node js/keepResponses.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import { formsFromExport, isDataDrivenProject, PUBLISH_CLONE_FAILED } from "./acquireClone.mjs";

const dir = dirname(fileURLToPath(import.meta.url));
const demo = readFileSync(join(dir, "demo-urls.js"), "utf8");
const transfer = readFileSync(join(dir, "transfer.js"), "utf8");
const ops = readFileSync(join(dir, "project-ops.js"), "utf8");
const admin = readFileSync(join(dir, "../library-admin.html"), "utf8");

assert.doesNotMatch(demo, /TAWALA_AUTHORING_FORM_NAMES/);
assert.doesNotMatch(demo, /isAuthoringFormName\(/);
assert.doesNotMatch(demo, /authoringFormsFromExport\(/);
assert.match(demo, /formsFromExport\(forms\)/);
assert.match(demo, /uniqueIdKeepsResponses\(uniqueId\)/);
assert.match(demo, /async copyResponsesToUniqueId\(/);
assert.match(demo, /reason: "keepResponses"/);
assert.match(demo, /this\.purgeResponses\(id\)/);
assert.match(demo, /this\.purgeResponses\(uniqueId\)/);
assert.doesNotMatch(demo, /async purgeRespondentResponses/);
assert.doesNotMatch(demo, /formsToKeepAfterRespondentPurge/);
assert.match(
  transfer.slice(transfer.indexOf("async function emptyLibraryRuntimeForPublish")),
  /cloneDefinitionToPrivateRuntime/,
  "Publish mints an empty Library uniqueId"
);
assert.doesNotMatch(
  transfer.slice(
    transfer.indexOf("async function emptyLibraryRuntimeForPublish"),
    transfer.indexOf("async function publishToLibraryAfterStripConfirm")
  ),
  /purgeAfterPublish/,
  "Publish must not purge the author's My Tawala uniqueId"
);
assert.doesNotMatch(
  transfer.slice(
    transfer.indexOf("async function emptyLibraryRuntimeForPublish"),
    transfer.indexOf("async function publishToLibraryAfterStripConfirm")
  ),
  /maybeCopyResponsesOntoClone/,
  "Publish must not copy author submissions onto the Library uniqueId"
);
assert.match(
  demo.slice(demo.indexOf("async copyResponsesToUniqueId")),
  /refused: response copy must use a new uniqueId/
);
assert.doesNotMatch(
  demo.slice(demo.indexOf("async copyResponsesToUniqueId")),
  /Question \/ SetupVariables/
);
assert.match(demo, /uniqueIdIsLibraryDataDriven/);
assert.match(demo, /tooltipSingleExam:/);
assert.match(demo, /nothing you enter is saved/);

assert.match(transfer, /keepResponses: false/);
assert.match(transfer, /publishedKeepResponses: false/);
assert.doesNotMatch(transfer, /Already retired\./);
assert.match(transfer, /function deleteLibraryEntry/);
assert.match(transfer, /catalogHidden: catalogHasSeed\(libraryId\)/);
assert.match(transfer.slice(transfer.indexOf("function withLibraryOverlay")), /retired\[id\] && !isLiveLibraryPublishOverlay/);
assert.match(transfer.slice(transfer.indexOf("async function publishToLibrary")), /clearLibraryRetired\(libraryId\)/);
assert.match(demo.slice(demo.indexOf("getLibrary(id)")), /isLibraryRetired\(id\)/);
assert.doesNotMatch(transfer, /keepResponses: keepResponses === true/);
assert.doesNotMatch(transfer, /publishedKeepResponses: keepResponses === true/);
assert.match(transfer, /maybeCopyResponsesOntoClone/);
assert.doesNotMatch(transfer, /maybeCopyAuthoringOntoClone/);
assert.doesNotMatch(ops, /keepResponses: !purgeOnPublish/);
assert.match(ops, /projectIsDataDriven/);
assert.match(ops, /responseCopyNeedsAlert/);
assert.doesNotMatch(ops, /authoringCopyNeedsAlert/);
assert.doesNotMatch(admin, /keepResponses: !purgeOnPublish/);
assert.doesNotMatch(admin, /adminPublishPurge/);
assert.match(ops, /publishToLibraryAfterStripConfirm/);
assert.match(admin, /publishToLibraryAfterStripConfirm/);
assert.equal(
  PUBLISH_CLONE_FAILED,
  "Couldn't make a separate Library copy of the live form. Your My Tawala project was not changed. Start Tomcat and the Designer API, then try again."
);
assert.match(ops, /cancelledStrip/);
assert.match(admin, /cancelledStrip/);

const mixed = [
  { form: "Exam", rows: [{}, {}] },
  { form: "Question", rows: [{}, {}, {}] },
  { form: "Answer", rows: [{}] },
  { form: "SetupVariables", rows: [{}] },
];
const copied = formsFromExport(mixed);
assert.deepEqual(
  copied.map((f) => f.form),
  ["Exam", "Question", "Answer", "SetupVariables"]
);
assert.equal(formsFromExport([{ form: "Survey", rows: [{}] }]).length, 1);
assert.equal(formsFromExport([{ form: "Exam", rows: [{}] }]).length, 1);

assert.equal(
  isDataDrivenProject({
    id: "online-exam-builder",
    formNames: ["Question", "SetupVariables", "Exam"],
  }),
  true
);
assert.equal(
  isDataDrivenProject({
    name: "Mongolia Test",
    startPoints: [{ label: "Exam" }, { label: "Administration" }],
  }),
  true
);
assert.equal(
  isDataDrivenProject({
    name: "Simple Survey Template",
    startPoints: [{ label: "Survey" }, { label: "Report" }],
  }),
  false
);
assert.equal(
  isDataDrivenProject({
    name: "Horses and Penguins Test",
    startPoints: [{ label: "Form 1" }],
  }),
  false
);
assert.equal(
  isDataDrivenProject({
    name: "Get Together Template",
    startPoints: [{ label: "Survey" }, { label: "Report" }],
  }),
  false
);

console.log("keepResponses contract ok");
