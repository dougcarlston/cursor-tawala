/**
 * Push → Show in My Tawala must update the existing uniqueId listing, not mint a twin.
 * Run: node js/deployOverlayMerge.test.mjs
 */
import assert from "node:assert/strict";
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import vm from "node:vm";

const dir = dirname(fileURLToPath(import.meta.url));
const transferSrc = readFileSync(join(dir, "transfer.js"), "utf8");

const UID = "un7qtlf0tcc0rqq";

function loadTransfer(overlaySeed) {
  const store = new Map();
  if (overlaySeed) {
    store.set("tawala.mock.myTawalaOverlay", JSON.stringify(overlaySeed));
  }
  const localStorage = {
    getItem(k) {
      return store.has(k) ? store.get(k) : null;
    },
    setItem(k, v) {
      store.set(k, String(v));
    },
    removeItem(k) {
      store.delete(k);
    },
  };
  const ctx = { localStorage, console };
  ctx.window = ctx;
  ctx.globalThis = ctx;
  vm.runInNewContext(transferSrc, ctx, { filename: "transfer.js" });
  return {
    T: ctx.window.TawalaTransfer,
    overlay() {
      return JSON.parse(store.get("tawala.mock.myTawalaOverlay") || "{}");
    },
  };
}

function oebReceipt(extra = {}) {
  return {
    id: "online-exam-builder",
    name: "Online Exam Builder",
    uniqueId: UID,
    startpoints: [{ form: "Setup", url: `http://localhost:8080/p/${UID}/npwtqlg.Setup` }],
    at: "2026-09-14T17:00:00.000Z",
    versionDescription: "Pre-Setup first-pass",
    ...extra,
  };
}

{
  const { T } = loadTransfer({
    "online-exam-builder-e3c56ca6": {
      name: "Online Exam Builder e3c56ca6",
      uniqueId: UID,
      fromDeployOverlay: true,
      sourcePile: "deploy-overlay",
      createdAt: "2026-08-01T00:00:00.000Z",
      lastDeployAt: "2026-08-01T00:00:00.000Z",
    },
  });
  assert.equal(
    T.resolveMyTawalaDeployListingId(T.getMyTawalaOverlay(), oebReceipt()),
    "online-exam-builder-e3c56ca6",
    "receipt slug must not mint a second card when uniqueId already has a listing"
  );
}

{
  const { T, overlay } = loadTransfer({
    "online-exam-builder-e3c56ca6": {
      name: "Online Exam Builder e3c56ca6",
      uniqueId: UID,
      fromDeployOverlay: true,
      sourcePile: "deploy-overlay",
      createdAt: "2026-08-01T00:00:00.000Z",
      lastDeployAt: "2026-08-01T00:00:00.000Z",
      versionNumber: 3,
    },
  });
  const row = T.upsertMyTawalaFromDeploy(oebReceipt());
  assert.equal(row.id, "online-exam-builder-e3c56ca6");
  assert.equal(row.name, "Online Exam Builder e3c56ca6", "keep the existing listing name");
  assert.equal(row.uniqueId, UID);
  assert.equal(row.versionNumber, 4);
  const pile = overlay();
  assert.equal(Object.keys(pile).length, 1);
  assert.equal(pile["online-exam-builder"], undefined);
}

{
  const { T, overlay } = loadTransfer({
    "online-exam-builder": {
      name: "Online Exam Builder",
      uniqueId: UID,
      fromLibraryAcquire: true,
      sourcePile: "library-acquire",
      createdAt: "2026-08-10T00:00:00.000Z",
    },
  });
  const row = T.upsertMyTawalaFromDeploy(oebReceipt());
  assert.equal(row.id, "online-exam-builder", "same uniqueId updates the acquire in place");
  assert.equal(overlay()["online-exam-builder-2"], undefined);
}

{
  const { T, overlay } = loadTransfer({
    "online-exam-builder": {
      name: "Online Exam Builder",
      uniqueId: "u3hkqgwtrepjlur",
      fromLibraryAcquire: true,
      sourcePile: "library-acquire",
      createdAt: "2026-08-10T00:00:00.000Z",
    },
  });
  const row = T.upsertMyTawalaFromDeploy(oebReceipt());
  assert.equal(row.id, "online-exam-builder-2", "different uniqueId still mints a sibling");
  assert.equal(overlay()["online-exam-builder"].uniqueId, "u3hkqgwtrepjlur");
  assert.equal(overlay()["online-exam-builder-2"].uniqueId, UID);
}

{
  const { T, overlay } = loadTransfer({
    "online-exam-builder-e3c56ca6": {
      name: "Online Exam Builder e3c56ca6",
      uniqueId: UID,
      fromDeployOverlay: true,
      createdAt: "2026-08-01T00:00:00.000Z",
    },
    "online-exam-builder": {
      name: "Online Exam Builder",
      uniqueId: UID,
      fromDeployOverlay: true,
      createdAt: "2026-09-14T17:00:00.000Z",
    },
  });
  const row = T.upsertMyTawalaFromDeploy(oebReceipt());
  assert.equal(row.id, "online-exam-builder-e3c56ca6", "duplicates: update the oldest listing");
  const pile = overlay();
  assert.equal(Object.keys(pile).length, 2, "do not auto-delete the extra listing");
  const removed = T.deleteMyTawalaProject("online-exam-builder");
  assert.equal(removed.ok, true);
  const after = overlay();
  assert.equal(after["online-exam-builder"], undefined);
  assert.equal(after["online-exam-builder-e3c56ca6"].uniqueId, UID);
}

{
  const { T } = loadTransfer({});
  assert.equal(
    T.preferLibraryCatalogJsonFile(
      { jsonFile: "projects/library/Get Together.json" },
      { jsonFile: "designer-web/public/samples/templates/get-together.json" }
    ),
    "projects/library/Get Together.json"
  );
  assert.equal(
    T.preferLibraryCatalogJsonFile(
      { jsonFile: "projects/library/Potluck.json" },
      { jsonFile: "projects/library/Potluck.json" }
    ),
    "projects/library/Potluck.json"
  );
}

console.log("deployOverlayMerge.test.mjs ok");
