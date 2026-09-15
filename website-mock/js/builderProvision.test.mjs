/**
 * Version 1 Builder provision: data on Library OEB only; private My Tawala copies stay clean.
 * Run: node js/builderProvision.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import {
  findIdenticalLibraryListings,
  refusePublishDuplicate,
  publishDuplicateRefuseMessage,
} from "./acquireClone.mjs";
import {
  BUILDER_PROVISION_KEYS,
  OEB_BUILDER_ROLE_MAP,
  isPublicLibraryBuilder,
  stripBuilderProvisionFromPrivateRow,
} from "./builderProvision.mjs";

const dir = dirname(fileURLToPath(import.meta.url));
const demo = readFileSync(join(dir, "demo-urls.js"), "utf8");
const transfer = readFileSync(join(dir, "transfer.js"), "utf8");
const ops = readFileSync(join(dir, "project-ops.js"), "utf8");

assert.equal(isPublicLibraryBuilder({ builder: true, sourcePile: "library", name: "Online Exam Builder" }), true);
assert.equal(
  isPublicLibraryBuilder({
    builder: true,
    pulledFromLibraryId: "online-exam-builder",
    sourcePile: "library-acquire",
    fromLibraryAcquire: true,
    uniqueId: "teacherprivateid1",
  }),
  false,
  "a teacher's Copy to MyTawala of OEB is not a Builder"
);
assert.equal(
  isPublicLibraryBuilder({
    builder: true,
    fromDeployOverlay: true,
    sourcePile: "deploy-overlay",
  }),
  false,
  "Designer Push / Show in My Tawala is not a Builder"
);

const leaked = {
  name: "Period 3 History",
  uniqueId: "teacherprivateid1",
  pulledFromLibraryId: "online-exam-builder",
  builder: true,
  builderKeepForms: OEB_BUILDER_ROLE_MAP.keepForms.slice(),
  derivedFromBuilderId: "online-exam-builder",
};
const cleaned = stripBuilderProvisionFromPrivateRow(leaked);
assert.equal(cleaned.uniqueId, "teacherprivateid1");
assert.equal(cleaned.pulledFromLibraryId, "online-exam-builder");
for (const key of BUILDER_PROVISION_KEYS) {
  assert.equal(Object.prototype.hasOwnProperty.call(cleaned, key), false, "strip " + key);
}

const OEB_JSON = "projects/library/Online Exam Builder.json";
const catalog = [
  {
    id: "online-exam-builder",
    name: "Online Exam Builder",
    jsonFile: OEB_JSON,
    liveReady: true,
    sourcePile: "library",
    builder: true,
    builderKeepForms: OEB_BUILDER_ROLE_MAP.keepForms,
    builderStripForms: OEB_BUILDER_ROLE_MAP.stripForms,
  },
];
const copyOfOeb = findIdenticalLibraryListings({
  sourceProject: {
    name: "Period 3 History",
    pulledFromLibraryId: "online-exam-builder",
    jsonFile: OEB_JSON,
    uniqueId: "teacherprivateid1",
  },
  publishName: "Period 3 History",
  libraryEntries: catalog,
});
assert.equal(copyOfOeb.length, 1);
assert.equal(copyOfOeb[0].id, "online-exam-builder");
assert.equal(
  refusePublishDuplicate({
    identicalListings: copyOfOeb,
    replaceLibraryId: null,
    currentUser: "teacher",
  }),
  publishDuplicateRefuseMessage("Online Exam Builder"),
  "Version 1: filled private OEB still cannot Publish as another OEB"
);

assert.match(demo, /"builder": true/);
assert.match(demo, /"builderKeepForms": \["SetupVariables", "Question"\]/);
assert.match(transfer, /function stripBuilderProvisionFromPrivateRow/);
assert.match(transfer, /function isPublicLibraryBuilder/);
assert.match(transfer, /\.map\(stripBuilderProvisionFromPrivateRow\)/);
assert.doesNotMatch(ops, /Publish as an exam/);
assert.doesNotMatch(ops, /builderKeepForms/);

console.log("builderProvision contract ok");
