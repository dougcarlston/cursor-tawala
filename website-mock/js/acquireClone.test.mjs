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
  DATA_DRIVEN_TEST_DRIVE_TITLE,
  PUBLISH_CLONE_FAILED,
  countShowsSavedResponses,
  compactNameKey,
  findIdenticalLibraryListings,
  refusePublishDuplicate,
  publishDuplicateRefuseMessage,
  currentMockUser,
  isListedLibraryAuthor,
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
assert.equal(DATA_DRIVEN_TEST_DRIVE_TITLE.includes("nothing you enter is saved"), true);
assert.equal(DATA_DRIVEN_NO_TEST_DRIVE_TITLE, DATA_DRIVEN_TEST_DRIVE_TITLE);
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
  PUBLISH_CLONE_FAILED,
  "Couldn't make a separate Library copy of the live form. Your My Tawala project was not changed. Start Tomcat and the Designer API, then try again."
);
assert.match(transfer, /Couldn't make a separate Library copy of the live form/);
assert.doesNotMatch(transfer, /Project will be purged of data upon publication\. Proceed\?/);
assert.equal(countShowsSavedResponses({ status: "success", count: 12 }), true);
assert.equal(countShowsSavedResponses({ status: "success", count: 0 }), false);
assert.equal(countShowsSavedResponses({ status: "failure", count: 9 }), false);

const publishSrc = transfer.slice(
  transfer.indexOf("async function emptyLibraryRuntimeForPublish"),
  transfer.indexOf("async function publishToLibraryAfterStripConfirm")
);
assert.match(publishSrc, /cloneDefinitionToPrivateRuntime/);
assert.doesNotMatch(publishSrc, /maybeCopyResponsesOntoClone/);
assert.doesNotMatch(publishSrc, /purgeAfterPublish/);
assert.doesNotMatch(publishSrc, /needsStripConfirm/);
const publishFnSrc = transfer.slice(
  transfer.indexOf("async function publishToLibrary({"),
  transfer.indexOf("async function publishToLibraryAfterStripConfirm")
);
const dupAt = publishFnSrc.indexOf("refusePublishDuplicate");
const cloneCallAt = publishFnSrc.indexOf("await emptyLibraryRuntimeForPublish");
const upsertAt = publishFnSrc.indexOf("upsertLibraryOverlay");
assert.ok(dupAt >= 0 && cloneCallAt > dupAt, "duplicate refuse must run before Library clone");
assert.ok(cloneCallAt >= 0 && upsertAt > cloneCallAt);
assert.match(publishSrc, /duplicateProduct:\s*true/);
assert.match(publishSrc, /clonedEmpty:\s*true/);
assert.match(publishSrc, /author:\s*authorKeep \|\| currentUser/);
assert.match(transfer, /async function publishToLibraryAfterStripConfirm/);
assert.match(ops, /publishToLibraryAfterStripConfirm/);
assert.match(ops, /result\.duplicateProduct/);
assert.match(ops, /Library copy has no saved responses/);
assert.doesNotMatch(ops, /publishPurgeCheckbox/);
assert.doesNotMatch(ops, /keepResponses: !purgeOnPublish/);

const OEB_JSON = "projects/library/Online Exam Builder.json";
const catalog = [
  {
    id: "online-exam-builder",
    name: "Online Exam Builder",
    jsonFile: OEB_JSON,
  },
  {
    id: "get-together",
    name: "Get Together Template",
    jsonFile: "designer-web/public/samples/templates/get-together.json",
  },
];
const oebRefuse = publishDuplicateRefuseMessage("Online Exam Builder");
assert.equal(
  oebRefuse,
  "This is the same as “Online Exam Builder” already in the Library. You can only publish an update if you are that listing’s author."
);
assert.equal(compactNameKey("Copy of Online Exam Builder"), "copyofonlineexambuilder");
assert.notEqual(compactNameKey("Copy of Online Exam Builder"), compactNameKey("Online Exam Builder"));
assert.equal(isListedLibraryAuthor({ author: "dev" }, "dev"), true);
assert.equal(isListedLibraryAuthor({ name: "Online Exam Builder" }, "dev"), false);
assert.equal(currentMockUser({}), "dev");
assert.equal(currentMockUser({ chromeUser: "alice" }), "alice");

const copyOfCatalog = findIdenticalLibraryListings({
  sourceProject: {
    name: "Copy of Online Exam Builder",
    pulledFromLibraryId: "online-exam-builder",
    jsonFile: OEB_JSON,
  },
  publishName: "House Test",
  libraryEntries: catalog,
});
assert.equal(copyOfCatalog.length, 1);
assert.equal(copyOfCatalog[0].id, "online-exam-builder");
assert.equal(
  refusePublishDuplicate({
    identicalListings: copyOfCatalog,
    replaceLibraryId: null,
    currentUser: "dev",
  }),
  oebRefuse
);
assert.equal(
  refusePublishDuplicate({
    identicalListings: copyOfCatalog,
    replaceLibraryId: "online-exam-builder",
    currentUser: "dev",
  }),
  oebRefuse,
  "catalog seed has no listed author — cannot overwrite OEB"
);

const forkLookup = (id) =>
  id === "acquire-oeb" ? { pulledFromLibraryId: "online-exam-builder", jsonFile: OEB_JSON } : null;
const forkHits = findIdenticalLibraryListings({
  sourceProject: { name: "Copy of Copy of Online Exam Builder", forkedFromId: "acquire-oeb" },
  publishName: "House Test",
  libraryEntries: catalog,
  lookupMyTawala: forkLookup,
});
assert.equal(forkHits[0].id, "online-exam-builder");

const overlayCatalog = [
  ...catalog,
  {
    id: "my-exam-overlay",
    name: "My Exam",
    jsonFile: OEB_JSON,
    author: "dev",
  },
];
const authorHits = findIdenticalLibraryListings({
  sourceProject: {
    name: "My Exam",
    pulledFromLibraryId: "online-exam-builder",
    jsonFile: OEB_JSON,
  },
  publishName: "My Exam",
  libraryEntries: overlayCatalog,
});
assert.ok(authorHits.some((h) => h.id === "online-exam-builder"));
assert.ok(authorHits.some((h) => h.id === "my-exam-overlay"));
assert.equal(
  refusePublishDuplicate({
    identicalListings: authorHits,
    replaceLibraryId: "my-exam-overlay",
    currentUser: "dev",
  }),
  null,
  "listed author may update their overlay"
);
assert.equal(
  refusePublishDuplicate({
    identicalListings: authorHits,
    replaceLibraryId: "my-exam-overlay",
    currentUser: "alice",
  }),
  publishDuplicateRefuseMessage("My Exam")
);
assert.equal(
  refusePublishDuplicate({
    identicalListings: authorHits,
    replaceLibraryId: null,
    currentUser: "dev",
  }),
  oebRefuse
);

const originalHits = findIdenticalLibraryListings({
  sourceProject: { name: "Q3 Staff Quiz" },
  publishName: "Q3 Staff Quiz",
  libraryEntries: catalog,
});
assert.equal(originalHits.length, 0, "File→New-like row with no lineage/jsonFile twin can add");
assert.equal(
  refusePublishDuplicate({
    identicalListings: originalHits,
    replaceLibraryId: null,
    currentUser: "dev",
  }),
  null
);

const nameTwin = findIdenticalLibraryListings({
  sourceProject: { name: "Online Exam Builder" },
  publishName: "Online Exam Builder",
  libraryEntries: catalog,
});
assert.equal(nameTwin[0].id, "online-exam-builder");
assert.equal(nameTwin[0].matchReason, "name");

console.log("acquireClone contract ok");
