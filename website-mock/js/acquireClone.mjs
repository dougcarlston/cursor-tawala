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

/** Keep in sync with `transfer.js` Make a Copy refuse / fail-closed copy. */
export const MAKE_COPY_NO_LIVE_FORM_ERROR =
  "Can't copy until the original has a live form.";

export const CLONE_TRY_AGAIN_ERROR =
  "Couldn't copy the live form. Start Tomcat and the Designer API, then try again.";

export const DATA_DRIVEN_TEST_DRIVE_TITLE =
  "No account. Try the teacher setup flow — nothing you enter is saved. Copy to MyTawala (free) to use it for real.";

/** @deprecated */
export const DATA_DRIVEN_NO_TEST_DRIVE_TITLE = DATA_DRIVEN_TEST_DRIVE_TITLE;

/**
 * All named form payloads from an export — copy onto a new uniqueId.
 * Do not filter by form name (Question / SetupVariables / Exam all go).
 */
export function formsFromExport(forms) {
  const list = Array.isArray(forms) ? forms : [];
  return list.filter((f) => f && String(f.form || f.name || "").trim());
}

function formKeysOfProject(project) {
  const keys = [];
  const add = (n) => {
    const s = String(n || "").trim();
    if (s) keys.push(s);
  };
  if (!project || typeof project !== "object") return keys;
  (Array.isArray(project.formNames) ? project.formNames : []).forEach(add);
  (Array.isArray(project.startPoints) ? project.startPoints : []).forEach((sp) => {
    add(sp && (sp.form || sp.label));
  });
  return keys;
}

/**
 * Data-driven / derivative apps (Online Exam Builder, Mongolia-style published exams).
 * Questions live in Question + SetupVariables submissions — Library Test Drive is off;
 * use from My Tawala after Copy to MyTawala / Make a Copy.
 */
export function isDataDrivenProject(project) {
  if (!project || typeof project !== "object") return false;
  const names = formKeysOfProject(project).map((n) => n.toLowerCase());
  if (names.includes("question") && names.includes("setupvariables")) return true;
  const id = String(project.id || "").trim().toLowerCase();
  if (id === "online-exam-builder" || id.includes("online-exam")) return true;
  const json = String(project.jsonFile || "").replace(/\\/g, "/").toLowerCase();
  if (json.includes("online exam builder")) return true;
  const pulled = String(project.pulledFromLibraryId || "").trim().toLowerCase();
  if (pulled === "online-exam-builder") return true;
  const name = String(project.name || "").toLowerCase();
  if (/online\s*exam/.test(name) || /mongolia/.test(name)) return true;
  const starts = (Array.isArray(project.startPoints) ? project.startPoints : []).map((s) =>
    String((s && (s.form || s.label)) || "")
      .trim()
      .toLowerCase()
  );
  const hasExam = starts.some((s) => s === "exam");
  const hasAdmin = starts.some((s) => s === "administration" || s === "setup" || s === "admin");
  return hasExam && hasAdmin;
}

/**
 * True when Make a Copy has something to Push: clone snapshot, catalog jsonFile,
 * cached definition, or a live uniqueId / start URL (export path).
 */
export function hasMyTawalaCloneDefinitionSource(source) {
  if (!source || typeof source !== "object") return false;
  if (source.snapshotId) return true;
  if (source.jsonFile) return true;
  if (source.uniqueId) return true;
  if (source.definition && typeof source.definition === "object" && source.definition.name) {
    return true;
  }
  const versions = Array.isArray(source.versions) ? source.versions : [];
  if (
    versions.some(
      (v) =>
        v &&
        (v.snapshotId || (v.definition && typeof v.definition === "object" && v.definition.name))
    )
  ) {
    return true;
  }
  if (source.testDriveUrl) return true;
  const sps = Array.isArray(source.startPoints) ? source.startPoints : [];
  return sps.some((s) => s && s.url);
}

/**
 * Scrub leaked Make a Copy runtimes that still share the source uniqueId.
 * Private clones (own uniqueId, not mockSharedForkRuntime) are left alone.
 */
export function forkRuntimeShouldScrub(entry, sourceUniqueId) {
  if (!entry || typeof entry !== "object") return false;
  if (entry.mockSharedForkRuntime === true) return true;
  return uniqueIdsEqual(entry.uniqueId, sourceUniqueId);
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

/** Keep in sync with `transfer.js` PUBLISH_CLONE_FAILED. */
export const PUBLISH_CLONE_FAILED =
  "Couldn't make a separate Library copy of the live form. Your My Tawala project was not changed. Start Tomcat and the Designer API, then try again.";

/** True when a :3001 count result means the source has submissions (Publish must strip first). */
export function countShowsSavedResponses(countResult) {
  if (!countResult || countResult.status !== "success") return false;
  const n = Number(countResult.count);
  return Number.isFinite(n) && n > 0;
}

/** Keep in sync with `transfer.js` stripStubSuffix / compactNameKey. */
export function stripStubSuffix(name) {
  return String(name || "").replace(/\s*\(stub\)\s*$/i, "").trim();
}

export function compactNameKey(name) {
  return stripStubSuffix(name).toLowerCase().replace(/[^a-z0-9]+/g, "");
}

export function normalizeJsonFileKey(jsonFile) {
  let rel = String(jsonFile || "").trim().replace(/\\/g, "/");
  if (!rel) return "";
  if (rel.startsWith("website-mock/")) rel = rel.slice("website-mock/".length);
  return rel.toLowerCase();
}

/**
 * Mock account for Publish author checks. Same fallbacks as Details
 * `projectAuthorLabel` without a row-level `author` (chrome → body → "dev").
 */
export function currentMockUser({ chromeUser, bodyUser } = {}) {
  const fromChrome = String(chromeUser || "").trim();
  if (fromChrome) return fromChrome;
  const fromBody = String(bodyUser || "").trim();
  if (fromBody) return fromBody;
  return "dev";
}

export function isListedLibraryAuthor(entry, currentUser) {
  const author = String((entry && entry.author) || "").trim();
  if (!author) return false;
  return author.toLowerCase() === String(currentUser || "").trim().toLowerCase();
}

/** Owner refuse copy — keep in sync with `transfer.js`. */
export function publishDuplicateRefuseMessage(libraryName) {
  const name = String(libraryName || "that project").trim() || "that project";
  return (
    "This is the same as “" +
    name +
    "” already in the Library. You can only publish an update if you are that listing’s author."
  );
}

/**
 * pulledFromLibraryId on this row, else walk forkedFromId → that acquire.
 */
export function pulledLibraryIdFromProject(project, lookupMyTawala) {
  let cur = project;
  const seen = new Set();
  for (let i = 0; i < 20 && cur && typeof cur === "object"; i++) {
    const pulled = String(cur.pulledFromLibraryId || "").trim();
    if (pulled) return pulled;
    const forkId = String(cur.forkedFromId || "").trim();
    if (!forkId || seen.has(forkId)) break;
    seen.add(forkId);
    cur = typeof lookupMyTawala === "function" ? lookupMyTawala(forkId) : null;
  }
  return "";
}

/**
 * Visible Library rows that are the same product: acquire lineage, same
 * template jsonFile, or compact-equal Publish name. Empty jsonFile is not a twin.
 */
export function findIdenticalLibraryListings({
  sourceProject,
  publishName,
  libraryEntries,
  lookupMyTawala,
} = {}) {
  const entries = Array.isArray(libraryEntries) ? libraryEntries : [];
  const byId = new Map();
  entries.forEach((e) => {
    if (e && e.id != null && String(e.id).trim()) byId.set(String(e.id), e);
  });
  const hits = new Map();
  function add(entry, reason) {
    if (!entry || entry.id == null) return;
    const id = String(entry.id);
    if (!id || hits.has(id)) return;
    hits.set(id, { ...entry, id, matchReason: reason });
  }

  const pulledId = pulledLibraryIdFromProject(sourceProject, lookupMyTawala);
  if (pulledId && byId.has(pulledId)) add(byId.get(pulledId), "pulledFromLibraryId");

  const jsonKey = normalizeJsonFileKey(sourceProject && sourceProject.jsonFile);
  if (jsonKey) {
    entries.forEach((e) => {
      if (normalizeJsonFileKey(e && e.jsonFile) === jsonKey) add(e, "jsonFile");
    });
  }

  const nameKey = compactNameKey(publishName || (sourceProject && sourceProject.name) || "");
  if (nameKey) {
    entries.forEach((e) => {
      if (compactNameKey(e && e.name) === nameKey) add(e, "name");
    });
  }

  return Array.from(hits.values());
}

/**
 * Null when Publish may proceed (no twin, or replace target is that twin and
 * the current mock user is its listed author). Otherwise the refuse string.
 */
export function refusePublishDuplicate({
  identicalListings,
  replaceLibraryId,
  currentUser,
} = {}) {
  const list = Array.isArray(identicalListings) ? identicalListings : [];
  if (!list.length) return null;
  const replaceId = String(replaceLibraryId || "").trim();
  const target = replaceId ? list.find((e) => String(e.id) === replaceId) : null;
  if (target && isListedLibraryAuthor(target, currentUser)) return null;
  const cited =
    (target && !isListedLibraryAuthor(target, currentUser) && target) ||
    list.find((e) => !isListedLibraryAuthor(e, currentUser)) ||
    list[0];
  return publishDuplicateRefuseMessage((cited && (cited.name || cited.id)) || "that project");
}
