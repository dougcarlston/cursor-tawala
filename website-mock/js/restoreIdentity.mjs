/**
 * Restore identity: a backup may only time-machine onto this My Tawala row’s uniqueId.
 * Keep in sync with the copy inside website-mock/js/data-ops.js.
 */

const UNIQUE_ID_RE = /^[A-Za-z0-9]{1,20}$/;
const UNIQUE_FROM_URL_RE = /\/p\/([A-Za-z0-9]{1,20})(?:\/|$)/;

export function isValidUniqueId(value) {
  return typeof value === "string" && UNIQUE_ID_RE.test(value);
}

export function uniqueIdFromUrl(url) {
  if (!url) return null;
  const m = String(url).match(UNIQUE_FROM_URL_RE);
  return m ? m[1] : null;
}

function pushId(into, value) {
  const s = value == null ? "" : String(value).trim();
  if (UNIQUE_ID_RE.test(s) && !into.includes(s)) into.push(s);
}

function collectFromUrls(into, urls) {
  (urls || []).forEach((u) => {
    const id = uniqueIdFromUrl(u);
    if (id) pushId(into, id);
  });
}

/** uniqueId values found in backup identity fields — never displayName. */
export function collectBackupUniqueIds(payload) {
  const ids = [];
  if (!payload || typeof payload !== "object") return ids;
  pushId(ids, payload.uniqueId);
  const props = payload.properties && typeof payload.properties === "object" ? payload.properties : {};
  pushId(ids, props.uniqueId);
  collectFromUrls(ids, [props.testDriveUrl]);
  if (Array.isArray(props.startPoints)) {
    collectFromUrls(
      ids,
      props.startPoints.map((sp) => sp && sp.url)
    );
  }
  const def = payload.definition && typeof payload.definition === "object" ? payload.definition : {};
  pushId(ids, def.deployUniqueId);
  pushId(ids, def.uniqueId);
  return ids;
}

export function backupProjectLabel(payload) {
  if (!payload || typeof payload !== "object") return "another project";
  const fromDisplay = String(payload.displayName || "").trim();
  if (fromDisplay) return fromDisplay;
  const props = payload.properties && typeof payload.properties === "object" ? payload.properties : {};
  const fromProps = String(props.name || "").trim();
  if (fromProps) return fromProps;
  const def = payload.definition && typeof payload.definition === "object" ? payload.definition : {};
  const fromDef = String(def.name || "").trim();
  return fromDef || "another project";
}

export const RESTORE_RECOVERY_HINT =
  "To recover: open the .backup.json for the project you want (for example Exam Maker) and find uniqueId. " +
  "On Project Details, match that id to uniqueId under Project options, or to the /p/{id}/ in a DEPLOY Form link. " +
  "Ignore the title — both rows may look like Get Together if the wrong definition was Redeployed. " +
  "Restore that backup only onto the matching row.";

export function restoreMismatchMessage({ backupName, rowName, backupUniqueId, rowUniqueId }) {
  const backup = String(backupName || "another project").trim() || "another project";
  const row = String(rowName || "this project").trim() || "this project";
  const backupId = backupUniqueId ? ` (uniqueId ${backupUniqueId})` : "";
  const rowId = rowUniqueId ? ` (uniqueId ${rowUniqueId})` : "";
  return (
    `This backup belongs to “${backup}”${backupId}, not “${row}”${rowId}. Restore only works on the same project.\n\n` +
    RESTORE_RECOVERY_HINT
  );
}

export function restoreMissingIdMessage() {
  return (
    "This backup has no uniqueId. Restore only works on the same project, and this file doesn’t identify which one. " +
    "Do not guess by the name on the file — two Get Togethers would collide. Make a new Backup from this project, or pick a file that includes uniqueId."
  );
}

export function restoreConfirmNote({ backupName, backupUniqueId, rowName, rowUniqueId }) {
  const backup = String(backupName || "backup").trim() || "backup";
  const row = String(rowName || "this project").trim() || "this project";
  return (
    `This backup is “${backup}” (uniqueId ${backupUniqueId}).\n` +
    `This My Tawala row is “${row}” (uniqueId ${rowUniqueId}).`
  );
}

/**
 * Allow Restore only when every uniqueId in the file matches this row.
 * Missing uniqueId → refuse (do not match by display name).
 */
export function matchRestoreToRow(payload, rowUniqueId, rowName) {
  const backupName = backupProjectLabel(payload);
  const rowLabel = String(rowName || "this project").trim() || "this project";
  const rowId = rowUniqueId == null ? "" : String(rowUniqueId).trim();
  const ids = collectBackupUniqueIds(payload);
  if (!ids.length) {
    return { ok: false, error: restoreMissingIdMessage(), backupName, backupUniqueId: null };
  }
  const foreign = ids.filter((id) => id !== rowId);
  if (foreign.length) {
    return {
      ok: false,
      error: restoreMismatchMessage({
        backupName,
        rowName: rowLabel,
        backupUniqueId: ids[0],
        rowUniqueId: rowId,
      }),
      backupName,
      backupUniqueId: ids[0],
    };
  }
  return { ok: true, backupUniqueId: ids[0], backupName };
}
