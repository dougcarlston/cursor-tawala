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

console.log("acquireClone contract ok");
