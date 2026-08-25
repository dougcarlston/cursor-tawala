/**
 * Copy to MyTawala clone-on-acquire helpers (Task #26).
 * Keep `projectBodyForPrivateClone` in sync with `transfer.js`.
 *
 * File→New / clone must set `_freshFromTemplate` so Tomcat mints a new name/uniqueId
 * instead of Redeploying the public Library project by display name.
 */

export function catalogPathForOpenApi(jsonFile) {
  let rel = String(jsonFile || "").trim().replace(/\\/g, "/");
  if (!rel) return "";
  if (rel.startsWith("website-mock/")) rel = rel.slice("website-mock/".length);
  return rel;
}

export function projectBodyForPrivateClone(definition, displayName) {
  if (!definition || typeof definition !== "object") {
    throw new Error("definition required");
  }
  const name = String(displayName || "").trim();
  if (!name) throw new Error("display name required");
  const {
    _freshFromTemplate: _dropFresh,
    deployIdentityName: _dropId,
    deployUniqueId: _dropUid,
    uniqueId: _dropUnique,
    ...rest
  } = definition;
  return {
    ...rest,
    name,
    _freshFromTemplate: true,
  };
}

export function uniqueIdsEqual(a, b) {
  if (!a || !b) return false;
  return String(a) === String(b);
}

export function snapshotProjectAfterClone(definition, { displayName, deployIdentityName, uniqueId } = {}) {
  const body = projectBodyForPrivateClone(definition, displayName);
  const { _freshFromTemplate: _drop, ...rest } = body;
  return {
    ...rest,
    name: String(displayName || rest.name || "").trim() || rest.name,
    ...(deployIdentityName ? { deployIdentityName } : {}),
    ...(uniqueId ? { deployUniqueId: uniqueId } : {}),
  };
}
