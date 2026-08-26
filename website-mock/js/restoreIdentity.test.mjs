/**
 * Restore uniqueId contract — refuse a backup from a different project.
 * Run: node js/restoreIdentity.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import {
  backupProjectLabel,
  collectBackupUniqueIds,
  matchRestoreToRow,
  restoreConfirmNote,
  restoreMismatchMessage,
  restoreMissingIdMessage,
  RESTORE_RECOVERY_HINT,
} from "./restoreIdentity.mjs";

const dir = dirname(fileURLToPath(import.meta.url));
const dataOps = readFileSync(join(dir, "data-ops.js"), "utf8");
const projectOps = readFileSync(join(dir, "project-ops.js"), "utf8");

const examId = "examakeruid01";
const gatherId = "gettogethruid1";

const getTogetherBackup = {
  tawalaBackupFormat: 2,
  displayName: "Sophisticated Get Together",
  uniqueId: gatherId,
  properties: { name: "Sophisticated Get Together", uniqueId: gatherId },
  definition: { name: "Sophisticated Get Together", deployUniqueId: gatherId },
};

const examBackup = {
  tawalaBackupFormat: 2,
  displayName: "Exam Maker",
  uniqueId: examId,
  properties: { name: "Exam Maker", uniqueId: examId },
  definition: { name: "Exam Maker", deployUniqueId: examId },
};

assert.deepEqual(collectBackupUniqueIds(getTogetherBackup), [gatherId]);
assert.equal(backupProjectLabel(getTogetherBackup), "Sophisticated Get Together");

const mismatch = matchRestoreToRow(getTogetherBackup, examId, "Exam Maker");
assert.equal(mismatch.ok, false);
assert.equal(
  mismatch.error,
  restoreMismatchMessage({
    backupName: "Sophisticated Get Together",
    rowName: "Exam Maker",
    backupUniqueId: gatherId,
    rowUniqueId: examId,
  })
);
assert.match(
  mismatch.error,
  /This backup belongs to “Sophisticated Get Together” \(uniqueId gettogethruid1\), not “Exam Maker” \(uniqueId examakeruid01\)/
);
assert.match(mismatch.error, /Restore only works on the same project/);
assert.match(mismatch.error, /Ignore the title/);
assert.match(mismatch.error, /DEPLOY Form link/);
assert.equal(mismatch.error.includes(RESTORE_RECOVERY_HINT), true);

const same = matchRestoreToRow(examBackup, examId, "Exam Maker");
assert.equal(same.ok, true);
assert.equal(same.backupUniqueId, examId);

const renamedSameId = matchRestoreToRow(examBackup, examId, "Get Together");
assert.equal(renamedSameId.ok, true, "same uniqueId is this row even if the title was overwritten");

const sameNameDifferentId = matchRestoreToRow(
  { ...getTogetherBackup, displayName: "Exam Maker", properties: { name: "Exam Maker", uniqueId: gatherId } },
  examId,
  "Exam Maker"
);
assert.equal(sameNameDifferentId.ok, false, "do not match by display name");

const noId = matchRestoreToRow(
  { tawalaBackupFormat: 1, displayName: "Exam Maker", properties: { name: "Exam Maker" }, definition: { name: "Exam Maker" } },
  examId,
  "Exam Maker"
);
assert.equal(noId.ok, false);
assert.equal(noId.error, restoreMissingIdMessage());
assert.match(noId.error, /This backup has no uniqueId/);

const propsOnly = matchRestoreToRow(
  { properties: { uniqueId: examId, name: "Exam Maker" }, definition: { name: "Exam Maker" } },
  examId,
  "Exam Maker"
);
assert.equal(propsOnly.ok, true);

const urlOnly = matchRestoreToRow(
  {
    properties: {
      name: "Exam Maker",
      testDriveUrl: `http://localhost:8080/p/${examId}/Customize.FormName`,
    },
    definition: { name: "Exam Maker" },
  },
  examId,
  "Exam Maker"
);
assert.equal(urlOnly.ok, true);

const deployOnly = matchRestoreToRow(
  { definition: { name: "Exam Maker", deployUniqueId: examId } },
  examId,
  "Exam Maker"
);
assert.equal(deployOnly.ok, true);

const confirm = restoreConfirmNote({
  backupName: "Exam Maker",
  backupUniqueId: examId,
  rowName: "Get Together",
  rowUniqueId: examId,
});
assert.equal(
  confirm,
  `This backup is “Exam Maker” (uniqueId ${examId}).\nThis My Tawala row is “Get Together” (uniqueId ${examId}).`
);

assert.match(dataOps, /Keep in sync with restoreIdentity\.mjs/);
assert.match(dataOps, /matchRestoreToRow/);
assert.match(dataOps, /This backup belongs to/);
assert.match(dataOps, /This backup has no uniqueId/);
assert.match(dataOps, /restoreConfirmNote/);
assert.match(dataOps, /Never copy backup uniqueId onto this row/);
assert.doesNotMatch(
  dataOps.slice(dataOps.indexOf("const overlayPatch")),
  /overlayPatch\.uniqueId\s*=/
);

assert.match(projectOps, /pm-identity-uniqueid/);
assert.match(projectOps, />uniqueId<\/dt>/);

console.log("restoreIdentity contract ok");
