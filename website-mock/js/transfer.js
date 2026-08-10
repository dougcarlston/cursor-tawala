/**
 * Symbiotic transfer + Library category overrides (website-mock).
 *
 * Cross-origin note: Designer (:5173) and this mock (:5500) do not share
 * localStorage. Deploy → My Tawala uses a query receipt opened from Designer,
 * or a manual paste into the My Tawala inbox panel.
 *
 * Keys (local to :5500):
 *   tawala.mock.libraryCategoryOverrides — { [projectId]: categoryLabel }
 *   tawala.mock.libraryCategoryDefs — { added: [{slug,label,source}], renamed: {slug:label},
 *     deleted: {slug:true} } — admin Category Add / Rename / Delete overlay on top of the
 *     repo's TAWALA_LIBRARY_CATEGORIES (js/demo-urls.js). Rename migrates every affected
 *     project's stored category label via tawala.mock.libraryCategoryOverrides so matching
 *     keeps working; Delete never touches project data — orphaned projects just show under
 *     the Library "Other" bucket until re-categorized. Admin-only (library-admin.html); the
 *     repo categories stay the base until a maintainer copies an add/rename/delete into
 *     demo-urls.js by hand — same "overlay until shipped" pattern as everything else here.
 *   tawala.mock.deployInbox — recent Designer deploy receipts
 *   tawala.mock.myTawalaOverlay — { [projectId]: catalog-shaped entry } merged into My Tawala pile
 *     Deploy overlay rows may carry:
 *       versionNumber — monotonic int (latest Deploy)
 *       versionDescription — optional note from Designer Deploy dialog
 *       versions — [{ versionNumber, description, at, uniqueId, mode, startPoints, deployed,
 *         snapshotId?, definition?, hasDefinition? }]
 *       history is Details-only (listing stays flat; see README Sequencing / hold)
 *       description is the only mutable field on an existing version (updateVersionDescription)
 *       Deploy-this-version needs snapshotId and/or definition (saved from Show in My Tawala)
 *   tawala.mock.myTawalaDeleted — { [projectId]: true } account-private My Tawala removals
 *     (does not touch public Library / TAWALA_LIBRARY / liveReady). Also used to hide discarded
 *     fake seed archive ids (TAWALA_MYTAWALA_DISCARDED_SEED_IDS) that are not real Push /
 *     Save-a-copy overlay rows.
 *   tawala.mock.libraryOverlay — { [libraryId]: catalog-shaped entry } merged into the public
 *     Library — Publish (My Tawala → Library) writes here. Mock-only; shipping an overlay entry
 *     into the repo catalog / demo-urls.js (and liveReady) stays a separate maintainer step.
 *   tawala.mock.libraryRetired — { [libraryId]: true } Library **stub** ids retired by Publish
 *     (removed from the public Library listing; safety copy lives in the My Tawala overlay,
 *     still marked " (stub)"). Replacing a **non-stub** Library entry (e.g. an outdated Main
 *     Menu template) does NOT set this — it just overwrites that id in libraryOverlay in place.
 *
 * Publish (My Tawala → Library, owner Aug 1, 2026):
 *   - Owner can rename the project on the way in (temporary until real versioning exists).
 *   - Matching an existing Library entry to replace ignores the " (stub)" suffix on both sides
 *     (findMatchingLibraryTargets / compactNameKey) — avoids e.g. a Deploy-named "Signup sheet"
 *     failing to suggest the catalog "Sign-up Sheet Template" entry, and avoids id collisions
 *     between unrelated My Tawala / Library slugs (dialog always lets the owner pick explicitly).
 *   - Replacing a **stub** (`stub: true`) retires it: removed from Library, copied into the My
 *     Tawala overlay as a safety-net row that KEEPS its " (stub)" name suffix and `stub: true`
 *     flag (never hard-deleted).
 *   - Replacing a **non-stub** entry (liveReady or main-menu, e.g. an out-of-date template) just
 *     overlays that same Library id with the new content in place — no My Tawala copy is made,
 *     because it was never a placeholder stub.
 *
 * Library admin path (owner Aug 1, 2026 — "for when you won't always be available"):
 *   tawala.mock.libraryAdmin — "1" enables admin-only actions on library-admin.html (rename any
 *     Library entry, overwrite one from a My Tawala project, retire without Publishing first).
 *     Mock-only client gate — no real auth; see README § Library admin path for how to turn it on.
 *   Retiring any Library id (stub or not) from the admin page always keeps a My Tawala safety
 *     copy (never hard-deleted) and is reversible via "Restore to Library" as long as no new
 *     Publish/overwrite has since reused that id.
 */
(function () {
  const CATEGORY_KEY = "tawala.mock.libraryCategoryOverrides";
  const CATEGORY_DEFS_KEY = "tawala.mock.libraryCategoryDefs";
  const INBOX_KEY = "tawala.mock.deployInbox";
  const PILE_KEY = "tawala.mock.myTawalaOverlay";
  const DELETED_KEY = "tawala.mock.myTawalaDeleted";
  const LIBRARY_OVERLAY_KEY = "tawala.mock.libraryOverlay";
  const LIBRARY_RETIRED_KEY = "tawala.mock.libraryRetired";
  const ADMIN_KEY = "tawala.mock.libraryAdmin";
  const INBOX_MAX = 12;

  function readJson(key, fallback) {
    try {
      const raw = localStorage.getItem(key);
      if (!raw) return fallback;
      return JSON.parse(raw);
    } catch {
      return fallback;
    }
  }

  function writeJson(key, value) {
    try {
      localStorage.setItem(key, JSON.stringify(value));
      return true;
    } catch {
      return false;
    }
  }

  /** Match demo-urls.js catalog ids (e.g. "Potluck - Kids Too" → potluck-kids-too). */
  function slugifyProjectId(name) {
    return (
      String(name || "project")
        .toLowerCase()
        .replace(/[^a-z0-9]+/g, "-")
        .replace(/^-|-$/g, "") || "project"
    );
  }

  /** Display `M/D/YY` for listing columns (seed catalog still uses this). */
  function formatListDate(d) {
    const dt = d instanceof Date ? d : new Date(d || Date.now());
    if (Number.isNaN(dt.getTime())) return "—";
    const y = String(dt.getFullYear()).slice(-2);
    return `${dt.getMonth() + 1}/${dt.getDate()}/${y}`;
  }

  /** Real sort/timestamp — ISO. Pair with formatListDate for the visible Updated/Created cells. */
  function timestampNow(d) {
    const dt = d instanceof Date ? d : new Date(d == null || d === "" ? Date.now() : d);
    if (Number.isNaN(dt.getTime())) return new Date().toISOString();
    return dt.toISOString();
  }

  function iconLabelFromName(name) {
    const letters = String(name || "P").replace(/[^A-Za-z0-9]/g, "");
    return (letters.slice(0, 2) || "P").toUpperCase();
  }

  function getCategoryOverrides() {
    const o = readJson(CATEGORY_KEY, {});
    return o && typeof o === "object" ? o : {};
  }

  function setProjectCategory(projectId, categoryLabel) {
    if (!projectId) return false;
    const o = getCategoryOverrides();
    const label = String(categoryLabel || "").trim();
    if (!label) {
      delete o[projectId];
    } else {
      o[projectId] = label;
    }
    return writeJson(CATEGORY_KEY, o);
  }

  function clearProjectCategory(projectId) {
    return setProjectCategory(projectId, "");
  }

  /** Apply overrides onto catalog entry objects (mutates copies via map). */
  function withCategoryOverrides(entries) {
    const o = getCategoryOverrides();
    return (entries || []).map((p) => {
      const next = o[p.id];
      return next && next !== p.category ? { ...p, category: next, categoryOverridden: true } : p;
    });
  }

  function effectiveCategory(projectId, fallback) {
    const o = getCategoryOverrides();
    return o[projectId] || fallback || "";
  }

  /** Admin Category Add / Rename / Delete overlay — see key doc above. */
  function getCategoryDefsOverlay() {
    const o = readJson(CATEGORY_DEFS_KEY, {});
    return {
      added: o && Array.isArray(o.added) ? o.added : [],
      renamed: o && o.renamed && typeof o.renamed === "object" ? o.renamed : {},
      deleted: o && o.deleted && typeof o.deleted === "object" ? o.deleted : {},
    };
  }

  function writeCategoryDefsOverlay(overlay) {
    return writeJson(CATEGORY_DEFS_KEY, overlay);
  }

  /**
   * Effective Library category list — base TAWALA_LIBRARY_CATEGORIES (demo-urls.js) with the
   * admin add/rename/delete overlay applied. TawalaDemo.libraryCategories() prefers this when
   * transfer.js is loaded, so library.html / the Publish dialog / the category editor all see
   * admin changes without extra wiring.
   */
  function effectiveLibraryCategories() {
    const base = (typeof window !== "undefined" && window.TAWALA_LIBRARY_CATEGORIES) || [];
    const overlay = getCategoryDefsOverlay();
    const fromBase = base
      .filter((c) => !overlay.deleted[c.slug])
      .map((c) => ({ ...c, label: overlay.renamed[c.slug] || c.label }));
    const fromAdded = overlay.added
      .filter((c) => !overlay.deleted[c.slug])
      .map((c) => ({ ...c, label: overlay.renamed[c.slug] || c.label, source: c.source || "admin" }));
    return [...fromBase, ...fromAdded];
  }

  function categorySlugTaken(slug) {
    return effectiveLibraryCategories().some((c) => c.slug === slug);
  }

  /** Admin: add a new Library category group (mock-only overlay; see README § Library admin path). */
  function addLibraryCategory(label) {
    const trimmed = String(label || "").trim();
    if (!trimmed) return { ok: false, error: "Category name is required." };
    const slug = slugifyProjectId(trimmed);
    if (categorySlugTaken(slug)) {
      return { ok: false, error: `A category named "${trimmed}" already exists.` };
    }
    const overlay = getCategoryDefsOverlay();
    overlay.added.push({ slug, label: trimmed, source: "admin" });
    writeCategoryDefsOverlay(overlay);
    return { ok: true, slug, label: trimmed };
  }

  /**
   * Admin: rename a Library category (base or admin-added). Migrates every project currently
   * showing the old label to the new one via tawala.mock.libraryCategoryOverrides — otherwise
   * those rows would silently fall out of the group (matching is by label, not slug) and land
   * in the Library "Other" bucket unexpectedly.
   */
  function renameLibraryCategory(slug, newLabel) {
    const trimmed = String(newLabel || "").trim();
    if (!trimmed) return { ok: false, error: "Category name is required." };
    const current = effectiveLibraryCategories().find((c) => c.slug === slug);
    if (!current) return { ok: false, error: `Unknown category: ${slug}` };
    if (current.label === trimmed) return { ok: true, slug, label: trimmed, migrated: 0 };
    const dupe = effectiveLibraryCategories().find((c) => c.slug !== slug && c.label === trimmed);
    if (dupe) return { ok: false, error: `"${trimmed}" is already the name of another category.` };

    const overlay = getCategoryDefsOverlay();
    const addedIdx = overlay.added.findIndex((c) => c.slug === slug);
    if (addedIdx >= 0) {
      overlay.added[addedIdx] = { ...overlay.added[addedIdx], label: trimmed };
    } else {
      overlay.renamed[slug] = trimmed;
    }
    writeCategoryDefsOverlay(overlay);

    const oldLabel = current.label;
    let migrated = 0;
    if (
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.libraryEntries === "function"
    ) {
      window.TawalaDemo.libraryEntries().forEach((p) => {
        if (p.category === oldLabel) {
          setProjectCategory(p.id, trimmed);
          migrated++;
        }
      });
    }
    return { ok: true, slug, label: trimmed, migrated };
  }

  /**
   * Admin: remove a category group from the picker/tree. Never edits or deletes project data —
   * any project still carrying the old label just becomes an "Other" orphan on library.html
   * until re-categorized (drag/drop or the category select). Reversible via restoreLibraryCategory
   * as long as nothing has since reused the same slug.
   */
  function deleteLibraryCategory(slug) {
    if (!slug) return { ok: false, error: "slug required" };
    const overlay = getCategoryDefsOverlay();
    if (overlay.deleted[slug]) return { ok: false, error: "Already deleted." };
    const current = effectiveLibraryCategories().find((c) => c.slug === slug);
    if (!current) return { ok: false, error: `Unknown category: ${slug}` };
    overlay.deleted[slug] = true;
    writeCategoryDefsOverlay(overlay);
    return { ok: true, slug, label: current.label };
  }

  function restoreLibraryCategory(slug) {
    if (!slug) return { ok: false, error: "slug required" };
    const overlay = getCategoryDefsOverlay();
    if (!overlay.deleted[slug]) return { ok: false, error: "That category isn't deleted." };
    delete overlay.deleted[slug];
    writeCategoryDefsOverlay(overlay);
    return { ok: true, slug };
  }

  /** Deleted category groups (for the admin "Deleted categories" restore list). */
  function getDeletedLibraryCategories() {
    const overlay = getCategoryDefsOverlay();
    const base = (typeof window !== "undefined" && window.TAWALA_LIBRARY_CATEGORIES) || [];
    return Object.keys(overlay.deleted).map((slug) => {
      const fromBase = base.find((c) => c.slug === slug);
      const fromAdded = overlay.added.find((c) => c.slug === slug);
      const label =
        overlay.renamed[slug] || (fromAdded && fromAdded.label) || (fromBase && fromBase.label) || slug;
      return { slug, label };
    });
  }

  function getDeployInbox() {
    const list = readJson(INBOX_KEY, []);
    return Array.isArray(list) ? list : [];
  }

  function recordDeploy(receipt) {
    if (!receipt || !receipt.name) return getDeployInbox();
    const entry = {
      id: receipt.id || slugifyProjectId(receipt.name),
      name: String(receipt.name),
      uniqueId: receipt.uniqueId || null,
      startpoints: Array.isArray(receipt.startpoints) ? receipt.startpoints : [],
      mode: receipt.mode || null,
      at: receipt.at || new Date().toISOString(),
      versionNumber: receipt.versionNumber != null ? Number(receipt.versionNumber) : null,
      versionDescription:
        receipt.versionDescription != null
          ? String(receipt.versionDescription)
          : receipt.description != null
            ? String(receipt.description)
            : null,
    };
    const next = [entry, ...getDeployInbox().filter((e) => e.id !== entry.id || e.at !== entry.at)].slice(
      0,
      INBOX_MAX
    );
    writeJson(INBOX_KEY, next);
    return next;
  }

  function clearDeployInbox() {
    writeJson(INBOX_KEY, []);
  }

  /** Drop inbox receipts for one My Tawala project id (orphans after Delete). */
  function removeDeployInboxForProject(projectId) {
    if (!projectId) return false;
    const next = getDeployInbox().filter((e) => e && e.id !== projectId);
    return writeJson(INBOX_KEY, next);
  }

  function getMyTawalaOverlay() {
    const o = readJson(PILE_KEY, {});
    return o && typeof o === "object" && !Array.isArray(o) ? o : {};
  }

  function getMyTawalaDeleted() {
    const o = readJson(DELETED_KEY, {});
    return o && typeof o === "object" && !Array.isArray(o) ? o : {};
  }

  function isMyTawalaDeleted(projectId) {
    if (!projectId) return false;
    return !!getMyTawalaDeleted()[projectId];
  }

  function markMyTawalaDeleted(projectId) {
    if (!projectId) return false;
    const o = getMyTawalaDeleted();
    o[projectId] = true;
    return writeJson(DELETED_KEY, o);
  }

  function clearMyTawalaDeleted(projectId) {
    if (!projectId) return false;
    const o = getMyTawalaDeleted();
    if (!Object.prototype.hasOwnProperty.call(o, projectId)) return false;
    delete o[projectId];
    return writeJson(DELETED_KEY, o);
  }

  /** Former fake My Tawala catalog seed ids — see window.TAWALA_MYTAWALA_DISCARDED_SEED_IDS. */
  function discardedMyTawalaSeedIdSet() {
    const list =
      (typeof window !== "undefined" && window.TAWALA_MYTAWALA_DISCARDED_SEED_IDS) || [];
    return new Set(Array.isArray(list) ? list.map(String) : []);
  }

  /**
   * True when an overlay row is a real user acquire (Push / Save a copy / Publish retire
   * safety copy) — not a stray snapshot of an old fake seed.
   */
  function isRealMyTawalaOverlayRow(data) {
    if (!data || typeof data !== "object") return false;
    if (data.fromLibraryAcquire === true || data.sourcePile === "library-acquire") return true;
    if (data.pulledFromLibraryId) return true;
    if (data.lastDeployAt || data.fromDeployOverlay === true) return true;
    if (data.sourcePile === "publish-overlay" || data.retiredFromLibraryId) return true;
    if (data.sourcePile === "library-retired") return true;
    if (Array.isArray(data.versions) && data.versions.length) return true;
    return false;
  }

  /**
   * Save a copy / Publish-retire safety rows must survive Push at the same slug.
   * Push mints a new id instead of replacing them (owner Aug 10 — merge keeps acquires).
   */
  function isProtectedAcquireOverlayRow(data) {
    if (!data || typeof data !== "object") return false;
    if (data.fromLibraryAcquire === true || data.sourcePile === "library-acquire") return true;
    if (data.sourcePile === "library-retired" || data.retiredFromLibraryId) return true;
    return false;
  }

  /**
   * Hide discarded fake seed archive ids on every My Tawala load. Does not touch real
   * Push / Save-a-copy overlay rows (even when they reuse a former seed slug).
   */
  function scrubDiscardedMyTawalaSeeds() {
    const discarded = discardedMyTawalaSeedIdSet();
    if (!discarded.size) return { ok: true, marked: [] };
    const overlay = getMyTawalaOverlay();
    const marked = [];
    let overlayChanged = false;
    discarded.forEach((id) => {
      const data = overlay[id];
      if (data && isRealMyTawalaOverlayRow(data)) {
        clearMyTawalaDeleted(id);
        return;
      }
      if (data) {
        delete overlay[id];
        overlayChanged = true;
      }
      if (!isMyTawalaDeleted(id)) {
        markMyTawalaDeleted(id);
        marked.push(id);
      }
    });
    if (overlayChanged) writeJson(PILE_KEY, overlay);
    return { ok: true, marked };
  }

  /**
   * Copy Library live start URLs / uniqueId onto a Save-a-copy row so Use works in the mock.
   * Demo limitation: shares the Library try-out :8080 identity until production mints a
   * private uniqueId (clone-on-acquire). Flag: mockSharedLibraryRuntime.
   */
  function liveRuntimeFromLibrarySource(source) {
    const startPoints = (Array.isArray(source && source.startPoints) ? source.startPoints : [])
      .map((sp) => ({
        label: (sp && (sp.label || sp.form)) || "Start",
        url: (sp && sp.url) || null,
      }))
      .filter((sp) => sp.label);
    let uniqueId = (source && source.uniqueId) || null;
    if (
      !uniqueId &&
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.uniqueIdForProject === "function"
    ) {
      uniqueId = window.TawalaDemo.uniqueIdForProject(source);
    }
    if (
      !uniqueId &&
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.uniqueIdFromUrl === "function"
    ) {
      uniqueId = window.TawalaDemo.uniqueIdFromUrl(source && source.testDriveUrl);
      if (!uniqueId) {
        for (let i = 0; i < startPoints.length; i++) {
          uniqueId = window.TawalaDemo.uniqueIdFromUrl(startPoints[i].url);
          if (uniqueId) break;
        }
      }
    }
    let testDriveUrl = (source && source.testDriveUrl) || null;
    if (
      !testDriveUrl &&
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.primaryStartUrl === "function"
    ) {
      testDriveUrl = window.TawalaDemo.primaryStartUrl(startPoints, null);
    }
    if (!testDriveUrl) {
      const withUrl = startPoints.find((s) => s && s.url);
      testDriveUrl = (withUrl && withUrl.url) || null;
    }
    const hasLive = !!(uniqueId || testDriveUrl || startPoints.some((s) => s && s.url));
    return {
      startPoints,
      testDriveUrl,
      uniqueId: uniqueId || null,
      deployed: hasLive,
      mockSharedLibraryRuntime: hasLive,
    };
  }

  function acquireHasLiveStart(entry) {
    if (!entry) return false;
    if (entry.testDriveUrl) return true;
    if (entry.uniqueId) return true;
    const sps = entry.startPoints || [];
    return sps.some((s) => s && s.url);
  }

  /**
   * Fix older Save-a-copy overlay rows that stripped uniqueId / :8080 URLs (Use was grey).
   * Re-copies live start metadata from pulledFromLibraryId. Persists into overlay.
   */
  function rehydrateAcquireLiveUrls() {
    const overlay = getMyTawalaOverlay();
    let changed = false;
    const hydrated = [];
    Object.keys(overlay).forEach((id) => {
      const entry = overlay[id];
      if (!entry || typeof entry !== "object") return;
      const libId = entry.pulledFromLibraryId;
      if (!libId) return;
      if (entry.fromLibraryAcquire !== true && entry.sourcePile !== "library-acquire") {
        /* Pull-refresh may set pulledFromLibraryId without being an acquire — only
         * rehydrate empty acquires (no live starts). */
        if (acquireHasLiveStart(entry)) return;
      }
      if (acquireHasLiveStart(entry) && entry.mockSharedLibraryRuntime === true) return;
      if (acquireHasLiveStart(entry) && !entry.mockSharedLibraryRuntime) {
        /* Push / private deploy already has live URLs — leave alone. */
        if (entry.lastDeployAt || (Array.isArray(entry.versions) && entry.versions.length)) return;
      }
      if (acquireHasLiveStart(entry)) return;

      const source =
        (typeof window !== "undefined" &&
          window.TawalaDemo &&
          typeof window.TawalaDemo.getLibrary === "function" &&
          window.TawalaDemo.getLibrary(libId)) ||
        libraryReplaceCandidates().find((c) => c.id === libId) ||
        null;
      if (!source) return;
      const live = liveRuntimeFromLibrarySource(source);
      if (!acquireHasLiveStart(live)) return;
      overlay[id] = {
        ...entry,
        startPoints: live.startPoints,
        testDriveUrl: live.testDriveUrl,
        uniqueId: live.uniqueId,
        deployed: live.deployed,
        mockSharedLibraryRuntime: true,
        fromLibraryAcquire: entry.fromLibraryAcquire !== false,
      };
      changed = true;
      hydrated.push(id);
    });
    if (changed) writeJson(PILE_KEY, overlay);
    return { ok: true, hydrated };
  }

  /**
   * Account-private My Tawala Delete: remove this account’s row + Deploy overlay /
   * inbox receipt. Does not touch public Library catalog, liveReady, or :8080 XML.
   * Former seed rows stay hidden via deleted set; re-Deploy / Save a copy clears the mark.
   */
  function deleteMyTawalaProject(projectId) {
    if (!projectId) {
      return { ok: false, projectId: null, hadOverlay: false, hadInbox: false };
    }
    const hadOverlay = !!getMyTawalaOverlay()[projectId];
    const hadInbox = getDeployInbox().some((e) => e && e.id === projectId);
    removeMyTawalaOverlay(projectId);
    removeDeployInboxForProject(projectId);
    markMyTawalaDeleted(projectId);
    return { ok: true, projectId, hadOverlay, hadInbox };
  }

  /**
   * Build / merge a catalog-shaped My Tawala row from a Designer deploy receipt.
   * Mints a monotonic versionNumber and appends to versions[] (Details-only history).
   * Returns { id, ...entry } or null.
   */
  function upsertMyTawalaFromDeploy(receipt) {
    if (!receipt || !receipt.name) return null;
    let id = receipt.id || slugifyProjectId(receipt.name);
    const overlay = getMyTawalaOverlay();
    /* Never replace a Save a copy / retire-safety row at the same slug — mint a sibling id.
     * Re-Push onto an existing deploy-overlay row still merges versions in place. */
    let prev = overlay[id] || null;
    if (prev && isProtectedAcquireOverlayRow(prev)) {
      id = uniqueMyTawalaSlug(id);
      receipt.id = id;
      prev = null;
    }
    /* Re-Deploy / Show in My Tawala restores a previously deleted row. */
    clearMyTawalaDeleted(id);
    const startPoints = (Array.isArray(receipt.startpoints) ? receipt.startpoints : []).map((sp) => ({
      label: sp.form || sp.label || "Start",
      url: sp.url || null,
    }));
    // Prefer Exam / Registration when ranking a primary URL among multi-start receipts.
    let primaryUrl = null;
    if (
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.primaryStartUrl === "function"
    ) {
      primaryUrl = window.TawalaDemo.primaryStartUrl(startPoints, null);
    }
    const firstUrl =
      primaryUrl || (startPoints.find((s) => s && s.url) || {}).url || null;
    let uniqueId = receipt.uniqueId || null;
    if (
      !uniqueId &&
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.uniqueIdFromUrl === "function"
    ) {
      uniqueId = window.TawalaDemo.uniqueIdFromUrl(firstUrl);
      if (!uniqueId) {
        for (let i = 0; i < startPoints.length; i++) {
          uniqueId = window.TawalaDemo.uniqueIdFromUrl(startPoints[i] && startPoints[i].url);
          if (uniqueId) break;
        }
      }
    }
    if (
      uniqueId &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.isValidUniqueId === "function" &&
      !window.TawalaDemo.isValidUniqueId(uniqueId)
    ) {
      uniqueId = null;
    }
    const nowIso = timestampNow(receipt.at);
    const now = formatListDate(nowIso);
    const versionDescription = String(
      receipt.versionDescription != null
        ? receipt.versionDescription
        : receipt.description != null
          ? receipt.description
          : ""
    ).trim();
    const prevVersions = Array.isArray(prev && prev.versions) ? prev.versions.slice() : [];
    // Backfill a single historical row when an older overlay had a deploy but no versions[].
    if (!prevVersions.length && prev && (prev.lastDeployAt || prev.uniqueId || prev.testDriveUrl)) {
      const backfillNum =
        prev.versionNumber != null && Number(prev.versionNumber) >= 1
          ? Number(prev.versionNumber)
          : 1;
      prevVersions.push({
        versionNumber: backfillNum,
        description: String(prev.versionDescription || "").trim() || "(prior deploy)",
        at: prev.lastDeployAt || prev.updatedAt || nowIso,
        uniqueId: prev.uniqueId || null,
        mode: prev.mode || null,
        startPoints: Array.isArray(prev.startPoints) ? prev.startPoints : [],
        deployed: false,
      });
    }
    let maxNum = 0;
    for (let i = 0; i < prevVersions.length; i++) {
      const n = Number(prevVersions[i] && prevVersions[i].versionNumber);
      if (Number.isFinite(n) && n > maxNum) maxNum = n;
    }
    const versionNumber =
      receipt.versionNumber != null && Number(receipt.versionNumber) > maxNum
        ? Number(receipt.versionNumber)
        : maxNum + 1;
    const snapshotId =
      receipt.snapshotId != null && String(receipt.snapshotId).trim()
        ? String(receipt.snapshotId).trim()
        : null;
    const definition =
      receipt.definition && typeof receipt.definition === "object" ? receipt.definition : null;
    const versionEntry = {
      versionNumber,
      description: versionDescription,
      at: nowIso,
      uniqueId: uniqueId || (prev && prev.uniqueId) || null,
      mode: receipt.mode || (prev && prev.mode) || null,
      startPoints: startPoints.length ? startPoints : (prev && prev.startPoints) || [],
      deployed: true,
      snapshotId,
      definition,
      hasDefinition: !!(definition || snapshotId),
    };
    const versions = prevVersions
      .map((v) => (v && typeof v === "object" ? { ...v, deployed: false } : v))
      .filter((v) => v && Number(v.versionNumber) !== versionNumber);
    versions.push(versionEntry);
    versions.sort((a, b) => Number(b.versionNumber) - Number(a.versionNumber));
    // Stamp receipt so callers (inbox / UI) see the minted number.
    receipt.versionNumber = versionNumber;
    if (versionDescription) receipt.versionDescription = versionDescription;
    const entry = {
      name: String(receipt.name),
      category: (prev && prev.category) || receipt.category || "My Projects",
      featured: false,
      iconLabel: (prev && prev.iconLabel) || iconLabelFromName(receipt.name),
      rating: (prev && prev.rating) || 0,
      comments: (prev && prev.comments) || 0,
      created: (prev && prev.created) || now,
      createdAt: (prev && prev.createdAt) || nowIso,
      updated: now,
      updatedAt: nowIso,
      shortDescription:
        (prev && prev.shortDescription) || "Deployed from Web Designer (browser overlay).",
      longDescription:
        (prev && prev.longDescription) ||
        "Added via Deploy → Show in My Tawala. Stored in this browser’s localStorage until copied into demo-urls.js / the MyTawala pile.",
      sourcePile: "deploy-overlay",
      fromDeployOverlay: true,
      deployed: !!firstUrl,
      startPoints: startPoints.length ? startPoints : (prev && prev.startPoints) || [],
      testDriveUrl: firstUrl || (prev && prev.testDriveUrl) || null,
      uniqueId: uniqueId || (prev && prev.uniqueId) || null,
      mode: receipt.mode || (prev && prev.mode) || null,
      lastDeployAt: nowIso,
      versionNumber,
      versionDescription: versionDescription || "",
      versions,
    };
    overlay[id] = entry;
    writeJson(PILE_KEY, overlay);
    return { id, ...entry };
  }

  /**
   * Merge arbitrary catalog-shaped properties onto a My Tawala overlay row (owner Aug 1, 2026 —
   * BACKUP/RESTORE). Unlike upsertMyTawalaFromDeploy, this does not require a Deploy-receipt
   * shape and is used by Restore to reapply a backed-up name/description/category/startPoints/
   * etc. onto the current row — the mock's best-effort stand-in for "re-applies the matching
   * definition" (see README § Save/Deploy/Publish glossary and § Backup/Restore).
   */
  function upsertMyTawalaProperties(projectId, properties) {
    if (!projectId || !properties || typeof properties !== "object") return false;
    clearMyTawalaDeleted(projectId);
    const overlay = getMyTawalaOverlay();
    const prev = overlay[projectId] || {};
    overlay[projectId] = { ...prev, ...properties };
    return writeJson(PILE_KEY, overlay);
  }

  /**
   * Edit description on an existing Deploy version (legacy: versions immutable except description).
   * Does not change versionNumber, deployed flag, or “make current.” Persists to myTawalaOverlay.
   * @returns {{ ok: boolean, projectId?: string, versionNumber?: number, description?: string, error?: string }}
   */
  function updateVersionDescription(projectId, versionNumber, description) {
    if (!projectId) return { ok: false, error: "missing-project" };
    const num = Number(versionNumber);
    if (!Number.isFinite(num) || num < 1) return { ok: false, error: "invalid-version" };
    const desc = String(description != null ? description : "").trim();

    const overlay = getMyTawalaOverlay();
    const prev = overlay[projectId] || null;
    if (!prev) {
      return { ok: false, error: "no-overlay" };
    }

    let versions = Array.isArray(prev.versions)
      ? prev.versions.map((v) => (v && typeof v === "object" ? { ...v } : v))
      : [];
    // Materialize a single row when older overlays only had top-level version fields.
    if (!versions.length && (prev.versionNumber != null || prev.lastDeployAt || prev.uniqueId)) {
      versions.push({
        versionNumber:
          prev.versionNumber != null && Number(prev.versionNumber) >= 1
            ? Number(prev.versionNumber)
            : 1,
        description: String(prev.versionDescription || "").trim(),
        at: prev.lastDeployAt || prev.updatedAt || null,
        uniqueId: prev.uniqueId || null,
        mode: prev.mode || null,
        startPoints: Array.isArray(prev.startPoints) ? prev.startPoints : [],
        deployed: !!prev.deployed,
      });
    }

    const idx = versions.findIndex((v) => v && Number(v.versionNumber) === num);
    if (idx < 0) return { ok: false, error: "version-not-found" };

    const existing = versions[idx];
    versions[idx] = {
      ...existing,
      versionNumber: Number(existing.versionNumber),
      description: desc,
    };

    const patch = { versions };
    const currentNum = prev.versionNumber != null ? Number(prev.versionNumber) : null;
    if (currentNum != null && currentNum === num) {
      patch.versionDescription = desc;
    }

    overlay[projectId] = { ...prev, ...patch };
    if (!writeJson(PILE_KEY, overlay)) return { ok: false, error: "write-failed" };
    return { ok: true, projectId, versionNumber: num, description: desc };
  }

  /** True when a versions[] row can be redeployed (cached definition and/or :3001 snapshotId). */
  function versionHasRedeployableDefinition(version) {
    if (!version || typeof version !== "object") return false;
    if (version.definition && typeof version.definition === "object" && version.definition.name) {
      return true;
    }
    if (version.snapshotId && String(version.snapshotId).trim()) return true;
    return version.hasDefinition === true;
  }

  /**
   * Attach / cache a definition body (and optional snapshotId) on an existing version row.
   * Used after Show in My Tawala hydrates from :3001, or when a receipt carries definition inline.
   */
  function attachVersionDefinition(projectId, versionNumber, definition, snapshotId) {
    if (!projectId) return { ok: false, error: "missing-project" };
    const num = Number(versionNumber);
    if (!Number.isFinite(num) || num < 1) return { ok: false, error: "invalid-version" };
    if (!definition || typeof definition !== "object" || !definition.name) {
      return { ok: false, error: "invalid-definition" };
    }
    const overlay = getMyTawalaOverlay();
    const prev = overlay[projectId] || null;
    if (!prev) return { ok: false, error: "no-overlay" };
    let versions = Array.isArray(prev.versions)
      ? prev.versions.map((v) => (v && typeof v === "object" ? { ...v } : v))
      : [];
    const idx = versions.findIndex((v) => v && Number(v.versionNumber) === num);
    if (idx < 0) return { ok: false, error: "version-not-found" };
    const existing = versions[idx];
    const snap =
      snapshotId != null && String(snapshotId).trim()
        ? String(snapshotId).trim()
        : existing.snapshotId || null;
    const withDef = {
      ...existing,
      versionNumber: Number(existing.versionNumber),
      snapshotId: snap,
      definition,
      hasDefinition: true,
    };
    versions[idx] = withDef;
    const tryWrite = (rows) => {
      overlay[projectId] = { ...prev, versions: rows };
      return writeJson(PILE_KEY, overlay);
    };
    if (!tryWrite(versions)) {
      // Quotas: keep snapshotId only so Deploy-this-version can still fetch from :3001.
      versions[idx] = {
        ...existing,
        versionNumber: Number(existing.versionNumber),
        snapshotId: snap,
        definition: null,
        hasDefinition: !!snap,
      };
      if (!tryWrite(versions)) return { ok: false, error: "write-failed" };
      return { ok: true, projectId, versionNumber: num, cached: false, snapshotId: snap };
    }
    return { ok: true, projectId, versionNumber: num, cached: true, snapshotId: snap };
  }

  /**
   * After a successful Deploy-this-version: mark that row current/deployed (no new version mint).
   * Updates start points / uniqueId from the deploy API result.
   */
  function markVersionCurrentAndDeployed(projectId, versionNumber, deployResult) {
    if (!projectId) return { ok: false, error: "missing-project" };
    const num = Number(versionNumber);
    if (!Number.isFinite(num) || num < 1) return { ok: false, error: "invalid-version" };
    const overlay = getMyTawalaOverlay();
    const prev = overlay[projectId] || null;
    if (!prev) return { ok: false, error: "no-overlay" };

    let versions = Array.isArray(prev.versions)
      ? prev.versions.map((v) => (v && typeof v === "object" ? { ...v } : v))
      : [];
    const idx = versions.findIndex((v) => v && Number(v.versionNumber) === num);
    if (idx < 0) return { ok: false, error: "version-not-found" };

    const startPoints = (
      Array.isArray(deployResult && deployResult.startpoints)
        ? deployResult.startpoints
        : Array.isArray(deployResult && deployResult.startPoints)
          ? deployResult.startPoints
          : versions[idx].startPoints || prev.startPoints || []
    ).map((sp) => ({
      label: sp.form || sp.label || "Start",
      url: sp.url || null,
    }));
    let primaryUrl = null;
    if (
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.primaryStartUrl === "function"
    ) {
      primaryUrl = window.TawalaDemo.primaryStartUrl(startPoints, null);
    }
    const firstUrl =
      primaryUrl || (startPoints.find((s) => s && s.url) || {}).url || null;
    const uniqueId =
      (deployResult && deployResult.uniqueId) ||
      versions[idx].uniqueId ||
      prev.uniqueId ||
      null;
    const mode = (deployResult && deployResult.mode) || versions[idx].mode || prev.mode || null;
    const nowIso = timestampNow();

    versions = versions.map((v) =>
      v && typeof v === "object" ? { ...v, deployed: Number(v.versionNumber) === num } : v
    );
    versions[idx] = {
      ...versions[idx],
      versionNumber: num,
      deployed: true,
      uniqueId,
      mode,
      startPoints: startPoints.length ? startPoints : versions[idx].startPoints || [],
      at: versions[idx].at || nowIso,
    };

    overlay[projectId] = {
      ...prev,
      versions,
      versionNumber: num,
      versionDescription: String(versions[idx].description || "").trim(),
      uniqueId,
      mode,
      startPoints: startPoints.length ? startPoints : prev.startPoints || [],
      testDriveUrl: firstUrl || prev.testDriveUrl || null,
      deployed: !!firstUrl || !!prev.deployed,
      lastDeployAt: nowIso,
      updated: formatListDate(nowIso),
      updatedAt: nowIso,
    };
    if (!writeJson(PILE_KEY, overlay)) return { ok: false, error: "write-failed" };
    return {
      ok: true,
      projectId,
      versionNumber: num,
      uniqueId,
      startPoints: overlay[projectId].startPoints,
      testDriveUrl: overlay[projectId].testDriveUrl,
    };
  }

  function removeMyTawalaOverlay(projectId) {
    if (!projectId) return false;
    const overlay = getMyTawalaOverlay();
    if (!Object.prototype.hasOwnProperty.call(overlay, projectId)) return false;
    delete overlay[projectId];
    return writeJson(PILE_KEY, overlay);
  }

  function clearMyTawalaOverlay() {
    return writeJson(PILE_KEY, {});
  }

  /** Merge overlay on top of catalog entries (overlay wins on id collision for deploy fields). */
  function withMyTawalaOverlay(entries) {
    scrubDiscardedMyTawalaSeeds();
    rehydrateAcquireLiveUrls();
    const overlay = getMyTawalaOverlay();
    const deleted = getMyTawalaDeleted();
    const discarded = discardedMyTawalaSeedIdSet();
    const byId = new Map();
    (entries || []).forEach((p) => {
      if (!p || !p.id || deleted[p.id]) return;
      /* Empty seed is the contract — never list discarded archive ids from a stale base. */
      if (discarded.has(p.id) && !isRealMyTawalaOverlayRow(overlay[p.id])) return;
      byId.set(p.id, p);
    });
    Object.keys(overlay).forEach((id) => {
      if (deleted[id]) return;
      const data = overlay[id];
      if (!data || typeof data !== "object") return;
      if (discarded.has(id) && !isRealMyTawalaOverlayRow(data)) return;
      const existing = byId.get(id);
      if (existing) {
        byId.set(id, {
          ...existing,
          ...data,
          id,
          name: data.name || existing.name,
          fromDeployOverlay: true,
        });
      } else {
        byId.set(id, { id, ...data, fromDeployOverlay: true });
      }
    });
    return Array.from(byId.values());
  }

  function getOverlayEntry(projectId) {
    if (!projectId || isMyTawalaDeleted(projectId)) return null;
    const data = getMyTawalaOverlay()[projectId];
    return data ? { id: projectId, ...data } : null;
  }

  /** Strip the visible " (stub)" suffix used on Library stub display names (case-insensitive). */
  function stripStubSuffix(name) {
    return String(name || "").replace(/\s*\(stub\)\s*$/i, "").trim();
  }

  /** Slug of a name with the " (stub)" suffix ignored — for id-style comparisons. */
  function baseNameSlug(name) {
    return slugifyProjectId(stripStubSuffix(name));
  }

  /**
   * Loose match key: stub suffix stripped, lowercased, punctuation/spacing removed entirely
   * (not just hyphenated). Lets "Signup sheet" (a Deploy-overlay name) suggest the catalog
   * "Sign-up Sheet Template" entry even though slugifyProjectId would not produce the same id.
   */
  function compactNameKey(name) {
    return stripStubSuffix(name).toLowerCase().replace(/[^a-z0-9]+/g, "");
  }

  function getLibraryOverlay() {
    const o = readJson(LIBRARY_OVERLAY_KEY, {});
    return o && typeof o === "object" && !Array.isArray(o) ? o : {};
  }

  function getLibraryRetired() {
    const o = readJson(LIBRARY_RETIRED_KEY, {});
    return o && typeof o === "object" && !Array.isArray(o) ? o : {};
  }

  function isLibraryRetired(libraryId) {
    if (!libraryId) return false;
    return !!getLibraryRetired()[libraryId];
  }

  function markLibraryRetired(libraryId) {
    if (!libraryId) return false;
    const o = getLibraryRetired();
    o[libraryId] = true;
    return writeJson(LIBRARY_RETIRED_KEY, o);
  }

  function clearLibraryRetired(libraryId) {
    if (!libraryId) return false;
    const o = getLibraryRetired();
    if (!Object.prototype.hasOwnProperty.call(o, libraryId)) return false;
    delete o[libraryId];
    return writeJson(LIBRARY_RETIRED_KEY, o);
  }

  function getLibraryOverlayEntry(libraryId) {
    if (!libraryId) return null;
    const data = getLibraryOverlay()[libraryId];
    return data ? { id: libraryId, ...data } : null;
  }

  function upsertLibraryOverlay(libraryId, entry) {
    if (!libraryId || !entry || typeof entry !== "object") return false;
    const o = getLibraryOverlay();
    o[libraryId] = entry;
    return writeJson(LIBRARY_OVERLAY_KEY, o);
  }

  function removeLibraryOverlay(libraryId) {
    if (!libraryId) return false;
    const o = getLibraryOverlay();
    if (!Object.prototype.hasOwnProperty.call(o, libraryId)) return false;
    delete o[libraryId];
    return writeJson(LIBRARY_OVERLAY_KEY, o);
  }

  function clearLibraryOverlay() {
    return writeJson(LIBRARY_OVERLAY_KEY, {});
  }

  /** Discarded seed ids (Aug 10 cleanup) — never list / never keep in Library overlay. */
  function discardedLibraryIdSet() {
    const raw =
      (typeof window !== "undefined" && window.TAWALA_LIBRARY_DISCARDED_IDS) || [];
    const set = new Set();
    (Array.isArray(raw) ? raw : []).forEach((id) => {
      if (id) set.add(String(id));
    });
    return set;
  }

  /**
   * True when an id/entry must not appear in the public Library (retired WebLibrary stubs,
   * Sign-up Sheet seed, or any stub-marked overlay snapshot). A later Publish at the same
   * slug (`sourcePile: "publish-overlay"`, not stub-marked) is allowed through.
   */
  function isDiscardedPublicLibraryEntry(libraryId, data) {
    const id = String(libraryId || "");
    if (!id) return false;
    if (data && typeof data === "object") {
      if (data.stub === true) return true;
      if (/\(\s*stub\s*\)\s*$/i.test(String(data.name || ""))) return true;
      const uid = String(data.uniqueId || data.uniqueid || "");
      if (uid === "cicw55xxhvwrrh7") return true;
      try {
        if (JSON.stringify(data).indexOf("cicw55xxhvwrrh7") !== -1) return true;
      } catch {
        /* ignore */
      }
      /* Real Publish replacement at a formerly-discarded slug — keep. */
      if (
        data.sourcePile === "publish-overlay" &&
        data.stub !== true &&
        !/\(\s*stub\s*\)\s*$/i.test(String(data.name || ""))
      ) {
        return false;
      }
    }
    if (discardedLibraryIdSet().has(id)) return true;
    return false;
  }

  /**
   * Strip discarded / stub snapshots out of tawala.mock.libraryOverlay so they cannot
   * resurrect after TAWALA_LIBRARY seed cleanup. Also marks them retired. Safe to call
   * on every Library load. Does not touch My Tawala safety copies.
   */
  function scrubDiscardedLibraryStubs() {
    const overlay = getLibraryOverlay();
    const removed = [];
    let changed = false;
    Object.keys(overlay).forEach((id) => {
      const data = overlay[id];
      if (!isDiscardedPublicLibraryEntry(id, data)) return;
      delete overlay[id];
      removed.push(id);
      changed = true;
      markLibraryRetired(id);
    });
    if (changed) writeJson(LIBRARY_OVERLAY_KEY, overlay);

    /* Also retire discarded ids even when overlay was already empty (blocks Restore). */
    discardedLibraryIdSet().forEach((id) => {
      if (!isLibraryRetired(id)) markLibraryRetired(id);
    });

    /* Drop category overrides for discarded ids (cosmetic; they have no Library row). */
    const cats = getCategoryOverrides();
    let catChanged = false;
    Object.keys(cats).forEach((id) => {
      if (!isDiscardedPublicLibraryEntry(id, null) && !discardedLibraryIdSet().has(id)) return;
      delete cats[id];
      catChanged = true;
    });
    if (catChanged) writeJson(CATEGORY_KEY, cats);

    return { ok: true, removed };
  }

  /**
   * Merge Publish overlay on top of catalog Library entries. Retired **stub** ids are dropped
   * from the catalog side (they left the public Library); an overlay entry at that same id
   * (e.g. a fresh Publish that happens to reuse the slug) still shows — retirement only hides
   * the old catalog row, it never blocks a new Publish from using that id.
   *
   * Aug 10, 2026: always scrub discarded stub snapshots first. Overlay-only rows that are
   * stubs / discarded seed ids never re-enter the listing (Save a copy used to snapshot them).
   */
  function withLibraryOverlay(entries) {
    scrubDiscardedLibraryStubs();
    const overlay = getLibraryOverlay();
    const retired = getLibraryRetired();
    const byId = new Map();
    (entries || []).forEach((p) => {
      if (!p || !p.id) return;
      if (retired[p.id]) return;
      if (isDiscardedPublicLibraryEntry(p.id, p)) return;
      byId.set(p.id, p);
    });
    Object.keys(overlay).forEach((id) => {
      const data = overlay[id];
      if (!data || typeof data !== "object") return;
      if (isDiscardedPublicLibraryEntry(id, data)) return;
      const existing = byId.get(id);
      /* Overlay-only: keep real Publishes; drop stray snapshots of removed seed rows. */
      if (!existing) {
        const isPublish =
          data.sourcePile === "publish-overlay" || data.fromPublishOverlay === true;
        if (!isPublish && !data.liveReady) return;
      }
      byId.set(
        id,
        existing
          ? { ...existing, ...data, id, fromPublishOverlay: true }
          : { id, ...data, fromPublishOverlay: true }
      );
    });
    /* Active / De-activate (Aug 9): inactive Library rows stay off the public listing. */
    return Array.from(byId.values()).filter(
      (p) =>
        p &&
        p.inactive !== true &&
        p.libraryActive !== false &&
        !isDiscardedPublicLibraryEntry(p.id, p)
    );
  }

  /**
   * Flip public-Library visibility for a My Tawala project (Details Active / De-activate).
   * Inactive → hidden from Library listing (no Test Drive / Save a copy); stays on My Tawala
   * with Offline marker. Persists on My Tawala overlay + Library overlay when a twin exists.
   */
  function setProjectLibraryActive(projectId, active) {
    if (!projectId) return { ok: false, error: "missing-project" };
    const wantActive = !!active;
    const inactive = !wantActive;
    const nowIso = timestampNow();
    const now = formatListDate(nowIso);

    clearMyTawalaDeleted(projectId);
    const myOverlay = getMyTawalaOverlay();
    const prevMine = myOverlay[projectId] || {};
    myOverlay[projectId] = {
      ...prevMine,
      inactive,
      libraryActive: wantActive,
      updated: now,
      updatedAt: nowIso,
    };
    if (!writeJson(PILE_KEY, myOverlay)) return { ok: false, error: "write-failed" };

    const publishedId =
      (prevMine.publishedToLibraryId && String(prevMine.publishedToLibraryId)) ||
      (myOverlay[projectId].publishedToLibraryId && String(myOverlay[projectId].publishedToLibraryId)) ||
      "";
    const twinId =
      publishedId ||
      (window.TAWALA_LIBRARY && Object.prototype.hasOwnProperty.call(window.TAWALA_LIBRARY, projectId)
        ? projectId
        : "") ||
      (getLibraryOverlayEntry(projectId) ? projectId : "");

    if (twinId) {
      const base = (window.TAWALA_LIBRARY && window.TAWALA_LIBRARY[twinId]) || {};
      const prevLib = getLibraryOverlayEntry(twinId) || {};
      const { id: _drop, ...prevFields } = prevLib;
      upsertLibraryOverlay(twinId, {
        ...base,
        ...prevFields,
        inactive,
        libraryActive: wantActive,
        updated: now,
        updatedAt: nowIso,
      });
    }

    return {
      ok: true,
      projectId,
      active: wantActive,
      libraryId: twinId || null,
    };
  }

  /** Full current Library (catalog + overlay, minus retired stubs) — Publish match candidates. */
  function libraryReplaceCandidates() {
    if (
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.libraryEntries === "function"
    ) {
      return window.TawalaDemo.libraryEntries();
    }
    const base = window.TAWALA_LIBRARY
      ? Object.keys(window.TAWALA_LIBRARY).map((id) => ({ id, ...window.TAWALA_LIBRARY[id] }))
      : [];
    return withLibraryOverlay(base);
  }

  /** Free slug for a brand-new Library entry — never collides with a currently-visible id. */
  function uniqueLibrarySlug(baseSlug) {
    const taken = new Set(libraryReplaceCandidates().map((c) => c.id));
    if (!taken.has(baseSlug)) return baseSlug;
    let n = 2;
    while (taken.has(`${baseSlug}-${n}`)) n++;
    return `${baseSlug}-${n}`;
  }

  /**
   * Smart-default Publish target picker. Compares the proposed Library name (and the source
   * My Tawala id / name) against every current Library entry — stub or not — ignoring the
   * " (stub)" suffix on both sides. `matchKind: "exact"` (id or compact-name equality) is safe
   * to preselect; `matchKind: "loose"` (substring either direction) is surfaced in the dialog
   * as a hint only — the owner still picks explicitly (Aug 1, 2026: avoid slug collisions like
   * a Deploy-named "Signup sheet" silently overwriting the wrong catalog row).
   */
  function findMatchingLibraryTargets(proposedName, sourceProjectId, sourceProjectName) {
    const candidates = libraryReplaceCandidates();
    const proposedSlug = baseNameSlug(proposedName);
    const proposedCompact = compactNameKey(proposedName);
    const sourceSlug = sourceProjectId ? baseNameSlug(sourceProjectId) : null;
    const sourceCompact = sourceProjectName ? compactNameKey(sourceProjectName) : null;

    return candidates
      .map((c) => {
        const compact = compactNameKey(c.name);
        const exact =
          c.id === proposedSlug ||
          (!!proposedCompact && compact === proposedCompact) ||
          (!!sourceSlug && c.id === sourceSlug) ||
          (!!sourceCompact && compact === sourceCompact);
        const loose =
          !exact &&
          compact.length > 2 &&
          ((proposedCompact.length > 2 &&
            (compact.includes(proposedCompact) || proposedCompact.includes(compact))) ||
            (!!sourceCompact &&
              sourceCompact.length > 2 &&
              (compact.includes(sourceCompact) || sourceCompact.includes(compact))));
        return { ...c, matchKind: exact ? "exact" : loose ? "loose" : null };
      })
      .filter((c) => c.matchKind);
  }

  /**
   * Publish a My Tawala project into the public Library (mock — localStorage overlay only;
   * shipping into the repo catalog / demo-urls.js, and flipping `liveReady`, stays a separate
   * maintainer step — see README § Publish).
   *
   * `replaceLibraryId`, when set, names an existing Library entry (stub or not) to target:
   *   - stub (`stub: true`, not already retired) → retired: dropped from the Library listing,
   *     copied into the My Tawala overlay as a safety-net row that KEEPS the " (stub)" name
   *     suffix and `stub: true` flag.
   *   - non-stub (liveReady / main-menu / previously-published) → overlaid in place at the same
   *     id with the new content; no My Tawala copy is made (it was never a placeholder stub).
   * With no `replaceLibraryId`, Publish just adds a new Library overlay entry at a fresh slug.
   */
  function publishToLibrary({ sourceProjectId, name, replaceLibraryId, category } = {}) {
    const sourceProject =
      sourceProjectId && typeof window !== "undefined" && window.TawalaDemo
        ? window.TawalaDemo.getMyTawala(sourceProjectId)
        : null;
    const publishName = String(name || (sourceProject && sourceProject.name) || "").trim();
    if (!publishName) {
      return { ok: false, error: "Library name is required." };
    }

    const targetId = replaceLibraryId || null;
    const baseTarget = targetId && window.TAWALA_LIBRARY ? window.TAWALA_LIBRARY[targetId] : null;
    const targetIsActiveStub = !!(baseTarget && baseTarget.stub === true && !isLibraryRetired(targetId));
    /*
     * Collision guard (owner Aug 1, 2026): picking "None" must never silently overwrite an
     * unrelated Library row just because the new name happens to slugify to the same id (e.g.
     * publishing "Signup sheet" would naturally slug to "signup-sheet", the real catalog id for
     * "Sign-up Sheet Template"). Only an *explicit* replaceLibraryId is allowed to reuse an
     * existing id — a fresh "add as new" always gets a free slug, appending -2/-3/… if needed.
     */
    const libraryId = targetId || uniqueLibrarySlug(slugifyProjectId(publishName));
    const nowIso = timestampNow();
    const now = formatListDate(nowIso);

    const sourceStartPoints = (sourceProject && sourceProject.startPoints) || [];
    // Prefer one uniqueId for all start points (Admin writes + Exam reads same project).
    let uniqueId = (sourceProject && sourceProject.uniqueId) || null;
    if (
      !uniqueId &&
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.uniqueIdForProject === "function"
    ) {
      uniqueId = window.TawalaDemo.uniqueIdForProject(sourceProject);
    }
    // Library Test Drive: Administration/Setup first for Online Exam (not Exam).
    let testDriveUrl = null;
    if (
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.libraryTestDriveUrl === "function"
    ) {
      testDriveUrl = window.TawalaDemo.libraryTestDriveUrl({
        startPoints: sourceStartPoints,
        testDriveUrl: (sourceProject && sourceProject.testDriveUrl) || null,
      });
    } else {
      testDriveUrl = (sourceProject && sourceProject.testDriveUrl) || null;
    }

    const entry = {
      name: publishName,
      category:
        category ||
        (baseTarget && baseTarget.category) ||
        (sourceProject && sourceProject.category) ||
        "Uncategorized",
      featured: (baseTarget && baseTarget.featured) || false,
      iconLabel: iconLabelFromName(publishName),
      rating: (baseTarget && baseTarget.rating) || 0,
      comments: (baseTarget && baseTarget.comments) || 0,
      updated: now,
      updatedAt: nowIso,
      shortDescription:
        (sourceProject && sourceProject.shortDescription) ||
        "Published from My Tawala (browser overlay).",
      longDescription:
        (sourceProject && sourceProject.longDescription) ||
        `Published from My Tawala project "${
          (sourceProject && sourceProject.name) || sourceProjectId || "unknown"
        }" via the Publish dialog. Stored in this browser's localStorage until copied into demo-urls.js (see README § Publish).`,
      sourcePile: "publish-overlay",
      publishedFromId: sourceProjectId || null,
      publishedAt: nowIso,
      replacesLibraryId: targetId || null,
      deployed: !!(sourceProject && sourceProject.deployed),
      startPoints: sourceStartPoints,
      testDriveUrl,
      uniqueId,
    };
    /* liveReady is an owner-vetted cue — a fresh Publish overlay never claims it automatically. */

    upsertLibraryOverlay(libraryId, entry);

    /* Stamp the source My Tawala row so Details can show Published → Library link. */
    if (sourceProjectId) {
      const myOverlay = getMyTawalaOverlay();
      const prevMine = myOverlay[sourceProjectId] || {};
      myOverlay[sourceProjectId] = {
        ...prevMine,
        publishedToLibraryId: libraryId,
        publishedToLibraryName: publishName,
        publishedAt: nowIso,
        updated: now,
        updatedAt: nowIso,
      };
      writeJson(PILE_KEY, myOverlay);
    }

    let retired = null;
    if (targetIsActiveStub) {
      const myOverlay = getMyTawalaOverlay();
      const collides =
        Object.prototype.hasOwnProperty.call(myOverlay, targetId) ||
        (window.TAWALA_MYTAWALA && Object.prototype.hasOwnProperty.call(window.TAWALA_MYTAWALA, targetId));
      const myId = collides ? `${targetId}-retired` : targetId;
      const retiredEntry = {
        name: baseTarget.name, // keep the " (stub)" suffix — owner Aug 1, 2026 (never dropped)
        stub: true, // keep the stub flag as a safety-net marker inside My Tawala
        category: baseTarget.category,
        featured: false,
        iconLabel: baseTarget.iconLabel || iconLabelFromName(baseTarget.name),
        rating: baseTarget.rating || 0,
        comments: baseTarget.comments || 0,
        created: now,
        createdAt: nowIso,
        updated: now,
        updatedAt: nowIso,
        shortDescription: baseTarget.shortDescription || "",
        longDescription:
          `Retired Library stub — safety copy kept after Publish (${nowIso.slice(0, 10)}). ` +
          (baseTarget.longDescription || ""),
        jsonFile: baseTarget.jsonFile || null,
        sourcePile: "library-retired",
        retiredFromLibraryId: targetId,
        retiredAt: nowIso,
        deployed: !!baseTarget.deployed,
        startPoints: baseTarget.startPoints || [],
        testDriveUrl: baseTarget.testDriveUrl || null,
      };
      clearMyTawalaDeleted(myId);
      const nextOverlay = getMyTawalaOverlay();
      nextOverlay[myId] = retiredEntry;
      writeJson(PILE_KEY, nextOverlay);
      markLibraryRetired(targetId);
      retired = { stubId: targetId, myTawalaId: myId, name: baseTarget.name };
    }

    return {
      ok: true,
      libraryId,
      entry: { id: libraryId, ...entry },
      retired,
      replacedNonStub: !!(targetId && !targetIsActiveStub),
      replacedName: targetId && !targetIsActiveStub ? (baseTarget && baseTarget.name) || targetId : null,
    };
  }

  /**
   * Pull candidates (Library → My Tawala upgrade, owner Aug 1, 2026). Mirrors
   * findMatchingLibraryTargets but in the opposite direction: given a My Tawala project's own
   * id/name, which current Library entries look like the "source of truth" template it was
   * likely copied from? `matchKind: "exact"` (id or compact-name equality) is safe to preselect
   * in the Pull dialog; `"loose"` is shown as a hint only — Pull always requires an explicit
   * pick, there is no "just pull the closest match" auto mode.
   */
  function findPullCandidates(myTawalaProjectId, myTawalaProjectName) {
    const candidates = libraryReplaceCandidates();
    const idSlug = myTawalaProjectId ? baseNameSlug(myTawalaProjectId) : null;
    const nameCompact = myTawalaProjectName ? compactNameKey(myTawalaProjectName) : null;
    return candidates
      .map((c) => {
        const compact = compactNameKey(c.name);
        const exact = c.id === idSlug || (!!nameCompact && compact === nameCompact);
        const loose =
          !exact &&
          compact.length > 2 &&
          !!nameCompact &&
          nameCompact.length > 2 &&
          (compact.includes(nameCompact) || nameCompact.includes(compact));
        return { ...c, matchKind: exact ? "exact" : loose ? "loose" : null };
      })
      .filter((c) => c.matchKind);
  }

  /**
   * Pull (Library → My Tawala upgrade, owner Aug 1, 2026 — was a grey stub). Replaces this My
   * Tawala project's *content* fields with the current public Library entry's — category,
   * descriptions, icon, definition reference (`jsonFile`), and the Library's own reference
   * start points / test-drive URL (useful to compare against, e.g. re-open in Designer).
   *
   * Deliberately preserves everything private/identity-bearing on the My Tawala row: `name`
   * (the owner's customized title, not the Library's), `uniqueId` / `deployed` (this account's
   * own `:8080` deploy — pulling must never repoint Export/Import/Purge at the shared public
   * Library demo's submission data), and `rating` / `comments` / `created`. Records
   * `pulledFromLibraryId` / `pulledFromLibraryName` / `pulledAt` for traceability, same pattern
   * as Publish's `retiredFromLibraryId`.
   */
  function pullFromLibrary({ myTawalaProjectId, libraryId } = {}) {
    if (!myTawalaProjectId) {
      return { ok: false, error: "My Tawala project id is required." };
    }
    if (!libraryId) {
      return { ok: false, error: "Pick a Library project to pull from." };
    }
    const source = libraryReplaceCandidates().find((c) => c.id === libraryId);
    if (!source) {
      return { ok: false, error: `Unknown Library project: ${libraryId}` };
    }
    const overlay = getMyTawalaOverlay();
    const prev = overlay[myTawalaProjectId] || {};
    const nowIso = timestampNow();
    const now = formatListDate(nowIso);
    const entry = {
      ...prev,
      category: source.category || prev.category || "Uncategorized",
      shortDescription: source.shortDescription || prev.shortDescription || "",
      longDescription: source.longDescription || prev.longDescription || "",
      iconLabel: prev.iconLabel || source.iconLabel || iconLabelFromName(prev.name || myTawalaProjectId),
      jsonFile: source.jsonFile || prev.jsonFile || null,
      startPoints: source.startPoints && source.startPoints.length ? source.startPoints : prev.startPoints || [],
      testDriveUrl: source.testDriveUrl || prev.testDriveUrl || null,
      pulledFromLibraryId: libraryId,
      pulledFromLibraryName: source.name,
      pulledAt: nowIso,
      updated: now,
      updatedAt: nowIso,
    };
    clearMyTawalaDeleted(myTawalaProjectId);
    overlay[myTawalaProjectId] = entry;
    writeJson(PILE_KEY, overlay);
    return { ok: true, libraryId, sourceName: source.name, entry: { id: myTawalaProjectId, ...entry } };
  }

  /** True when a My Tawala id is already in the seed pile or overlay (deleted ids are free). */
  function myTawalaIdTaken(id) {
    if (!id) return false;
    if (isMyTawalaDeleted(id)) return false;
    if (Object.prototype.hasOwnProperty.call(getMyTawalaOverlay(), id)) return true;
    return !!(window.TAWALA_MYTAWALA && Object.prototype.hasOwnProperty.call(window.TAWALA_MYTAWALA, id));
  }

  function uniqueMyTawalaSlug(baseSlug) {
    const base = baseSlug || "project";
    if (!myTawalaIdTaken(base)) return base;
    let n = 2;
    while (myTawalaIdTaken(`${base}-${n}`)) n++;
    return `${base}-${n}`;
  }

  /** Case-insensitive display-name collision among current My Tawala rows. */
  function myTawalaNameTaken(name, exceptId) {
    return !!findMyTawalaByName(name, exceptId);
  }

  /**
   * First My Tawala row whose display name matches (case-insensitive trim), or null.
   * exceptId skips that row (rename-in-place keeping the same name / self).
   */
  function findMyTawalaByName(name, exceptId) {
    const key = compactNameKey(name);
    if (!key) return null;
    let rows = [];
    if (
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.myTawalaEntries === "function"
    ) {
      rows = window.TawalaDemo.myTawalaEntries();
    }
    for (const p of rows) {
      if (!p || (exceptId && p.id === exceptId)) continue;
      const other =
        typeof window.TawalaDemo.displayName === "function"
          ? window.TawalaDemo.displayName(p.name)
          : p.name;
      if (compactNameKey(other) === key) return p;
    }
    return null;
  }

  /**
   * Suggest a free My Tawala name for Save a copy.
   * Prefer the Library title when free; if taken, "Copy of …" then "Copy of … 2", …
   * (avoids opening on a collision — user can still type a taken name and confirm overwrite).
   */
  function suggestUniqueMyTawalaName(baseName) {
    const display = stripStubSuffix(baseName) || "Project";
    if (!myTawalaNameTaken(display)) return display;
    const copyBase = `Copy of ${display}`;
    if (!myTawalaNameTaken(copyBase)) return copyBase;
    let n = 2;
    while (myTawalaNameTaken(`${copyBase} ${n}`)) n++;
    return `${copyBase} ${n}`;
  }

  /**
   * Rename a My Tawala project in place (Project Details double-click / Rename). Persists to
   * myTawalaOverlay — does not mint a new id or touch Library.
   *
   * Name collision (owner Aug 10): warn + confirm overwrite — never silent.
   * On overwrite:true, deleteMyTawalaProject the *other* conflicting row, then keep this
   * project's id with the new display name (A→B's name removes B; A stays A with name B).
   */
  function renameMyTawalaProject(projectId, newName, opts) {
    const overwrite = !!(opts && opts.overwrite);
    if (!projectId) {
      return { ok: false, error: "Project id is required." };
    }
    const current =
      (typeof window !== "undefined" &&
        window.TawalaDemo &&
        typeof window.TawalaDemo.getMyTawala === "function" &&
        window.TawalaDemo.getMyTawala(projectId)) ||
      getOverlayEntry(projectId) ||
      null;
    if (!current) {
      return { ok: false, error: `Unknown My Tawala project: ${projectId}` };
    }
    const name = String(newName || "").trim();
    if (!name) {
      return { ok: false, error: "Name is required." };
    }
    const currentDisplay =
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.displayName === "function"
        ? window.TawalaDemo.displayName(current.name)
        : String(current.name || projectId);
    if (compactNameKey(name) === compactNameKey(currentDisplay)) {
      return { ok: true, id: projectId, name: currentDisplay, unchanged: true };
    }
    const conflict = findMyTawalaByName(name, projectId);
    if (conflict) {
      if (!overwrite) {
        const conflictName =
          typeof window !== "undefined" &&
          window.TawalaDemo &&
          typeof window.TawalaDemo.displayName === "function"
            ? window.TawalaDemo.displayName(conflict.name)
            : String(conflict.name || conflict.id);
        return {
          ok: false,
          needsOverwrite: true,
          conflictId: conflict.id,
          conflictName,
          error: `You already have a project named “${name}”. Confirm to replace it.`,
        };
      }
      deleteMyTawalaProject(conflict.id);
    }
    const nowIso = timestampNow();
    const now = formatListDate(nowIso);
    if (
      !upsertMyTawalaProperties(projectId, {
        name,
        iconLabel: iconLabelFromName(name),
        updated: now,
        updatedAt: nowIso,
      })
    ) {
      return { ok: false, error: "Could not write My Tawala overlay (localStorage)." };
    }
    return {
      ok: true,
      id: projectId,
      name,
      replacedId: conflict ? conflict.id : null,
      entry: { id: projectId, ...current, name, iconLabel: iconLabelFromName(name), updated: now, updatedAt: nowIso },
    };
  }

  /**
   * Save a copy (Library → My Tawala acquire, Aug 9 Task #8; Use-ready Aug 10).
   * Mints a *new* private My Tawala row with rename-on-acquire. Owner priority: Use must work
   * when the copy lands — mock copies Library :8080 start URLs + uniqueId
   * (`mockSharedLibraryRuntime: true`). Production must mint a private uniqueId (not share
   * Library demo data forever). Bumps Library cloneCount (overlay) for Times-used later.
   *
   * Name collision (owner Aug 10): warn + confirm overwrite — never silent.
   * On overwrite:true, deleteMyTawalaProject the existing same-name row, then write this acquire
   * as the sole My Tawala row with that display name (new id; old row gone).
   */
  function saveCopyFromLibrary({ libraryId, name, overwrite } = {}) {
    if (!libraryId) {
      return { ok: false, error: "Library project id is required." };
    }
    const source =
      (typeof window !== "undefined" &&
        window.TawalaDemo &&
        typeof window.TawalaDemo.getLibrary === "function" &&
        window.TawalaDemo.getLibrary(libraryId)) ||
      libraryReplaceCandidates().find((c) => c.id === libraryId) ||
      null;
    if (!source) {
      return { ok: false, error: `Unknown Library project: ${libraryId}` };
    }
    if (source.inactive === true || source.libraryActive === false) {
      return { ok: false, error: "That Library project is inactive — Save a copy is unavailable." };
    }
    const copyName = String(name || "").trim();
    if (!copyName) {
      return { ok: false, error: "Name is required." };
    }
    const conflict = findMyTawalaByName(copyName);
    if (conflict) {
      if (!overwrite) {
        const conflictName =
          typeof window !== "undefined" &&
          window.TawalaDemo &&
          typeof window.TawalaDemo.displayName === "function"
            ? window.TawalaDemo.displayName(conflict.name)
            : String(conflict.name || conflict.id);
        return {
          ok: false,
          needsOverwrite: true,
          conflictId: conflict.id,
          conflictName,
          error: `You already have a project named “${copyName}”. Confirm to replace it.`,
        };
      }
      deleteMyTawalaProject(conflict.id);
    }

    const id = uniqueMyTawalaSlug(slugifyProjectId(copyName));
    const nowIso = timestampNow();
    const now = formatListDate(nowIso);
    const live = liveRuntimeFromLibrarySource(source);

    const entry = {
      name: copyName,
      category: source.category || "Uncategorized",
      featured: false,
      iconLabel: iconLabelFromName(copyName),
      rating: 0,
      comments: 0,
      created: now,
      createdAt: nowIso,
      updated: now,
      updatedAt: nowIso,
      shortDescription: source.shortDescription || "",
      longDescription: source.longDescription || "",
      jsonFile: source.jsonFile || null,
      formNames: Array.isArray(source.formNames) ? source.formNames.slice() : undefined,
      sourcePile: "library-acquire",
      fromLibraryAcquire: true,
      pulledFromLibraryId: libraryId,
      pulledFromLibraryName: source.name,
      pulledAt: nowIso,
      /* Mock: Use works via Library live start URLs. Production must mint a private uniqueId. */
      uniqueId: live.uniqueId,
      deployed: live.deployed,
      testDriveUrl: live.testDriveUrl,
      startPoints: live.startPoints,
      mockSharedLibraryRuntime: live.mockSharedLibraryRuntime,
    };

    clearMyTawalaDeleted(id);
    const overlay = getMyTawalaOverlay();
    overlay[id] = entry;
    if (!writeJson(PILE_KEY, overlay)) {
      return { ok: false, error: "Could not write My Tawala overlay (localStorage)." };
    }

    /* Clone count = times Save a copy was used (overlay bump; catalog seed may already have a number).
     * Only bump cloneCount — never snapshot the full catalog row into libraryOverlay.
     * Spreading baseLib used to freeze discarded stubs in localStorage so they reappeared
     * after TAWALA_LIBRARY seed cleanup. */
    const prevCount = Number(source.cloneCount);
    const nextCount = (Number.isFinite(prevCount) ? prevCount : 0) + 1;
    if (!isDiscardedPublicLibraryEntry(libraryId, source)) {
      const prevLib = getLibraryOverlayEntry(libraryId) || {};
      const { id: _drop, stub: _stub, ...prevFields } = prevLib;
      upsertLibraryOverlay(libraryId, { ...prevFields, cloneCount: nextCount });
    }

    return {
      ok: true,
      id,
      libraryId,
      sourceName: source.name,
      cloneCount: nextCount,
      replacedId: conflict ? conflict.id : null,
      entry: { id, ...entry },
    };
  }

  /**
   * Suggest a free My Tawala name for Make a Copy (own fork).
   * Always starts with "Copy of …" (unlike Save a copy, which prefers the Library title when free).
   */
  function suggestMakeCopyName(baseName) {
    const display = stripStubSuffix(baseName) || "Project";
    const copyBase = `Copy of ${display}`;
    if (!myTawalaNameTaken(copyBase)) return copyBase;
    let n = 2;
    while (myTawalaNameTaken(`${copyBase} ${n}`)) n++;
    return `${copyBase} ${n}`;
  }

  /**
   * Make a Copy — fork an existing My Tawala project (Aug 9 Task #9).
   * Mints a *new* overlay id + display name. Original row is untouched.
   * Copies definition metadata (jsonFile, descriptions, formNames, start labels).
   * Response data stays empty by default (no local count copy). Mock: if the source has
   * live :8080 starts / uniqueId, copy them so Use works immediately
   * (`mockSharedForkRuntime` — same demo limitation as Save a copy; production must mint
   * a private uniqueId with empty private data).
   *
   * Name collision (owner Aug 10): warn + confirm overwrite — never silent.
   * Cannot reuse the source project's own display name (that would destroy the original).
   */
  function makeCopyOfMyTawalaProject({ sourceId, name, overwrite } = {}) {
    if (!sourceId) {
      return { ok: false, error: "Source project id is required." };
    }
    const source =
      (typeof window !== "undefined" &&
        window.TawalaDemo &&
        typeof window.TawalaDemo.getMyTawala === "function" &&
        window.TawalaDemo.getMyTawala(sourceId)) ||
      getOverlayEntry(sourceId) ||
      null;
    if (!source) {
      return { ok: false, error: `Unknown My Tawala project: ${sourceId}` };
    }
    const copyName = String(name || "").trim();
    if (!copyName) {
      return { ok: false, error: "Name is required." };
    }
    const sourceDisplay =
      typeof window !== "undefined" &&
      window.TawalaDemo &&
      typeof window.TawalaDemo.displayName === "function"
        ? window.TawalaDemo.displayName(source.name)
        : String(source.name || sourceId);
    if (compactNameKey(copyName) === compactNameKey(sourceDisplay)) {
      return {
        ok: false,
        error:
          "Choose a different name — Make a Copy keeps the original project. Use Rename to change this project’s name.",
      };
    }
    const conflict = findMyTawalaByName(copyName, sourceId);
    if (conflict) {
      if (!overwrite) {
        const conflictName =
          typeof window !== "undefined" &&
          window.TawalaDemo &&
          typeof window.TawalaDemo.displayName === "function"
            ? window.TawalaDemo.displayName(conflict.name)
            : String(conflict.name || conflict.id);
        return {
          ok: false,
          needsOverwrite: true,
          conflictId: conflict.id,
          conflictName,
          error: `You already have a project named “${copyName}”. Confirm to replace it.`,
        };
      }
      deleteMyTawalaProject(conflict.id);
    }

    const id = uniqueMyTawalaSlug(slugifyProjectId(copyName));
    const nowIso = timestampNow();
    const now = formatListDate(nowIso);
    const live = liveRuntimeFromLibrarySource(source);

    const entry = {
      name: copyName,
      category: source.category || "Uncategorized",
      featured: false,
      iconLabel: iconLabelFromName(copyName),
      rating: 0,
      comments: 0,
      created: now,
      createdAt: nowIso,
      updated: now,
      updatedAt: nowIso,
      shortDescription: source.shortDescription || "",
      longDescription: source.longDescription || "",
      jsonFile: source.jsonFile || null,
      formNames: Array.isArray(source.formNames) ? source.formNames.slice() : undefined,
      themePath: source.themePath || undefined,
      sourcePile: "mytawala-fork",
      forkedFromId: sourceId,
      forkedFromName: sourceDisplay,
      forkedAt: nowIso,
      /* Keep Library link so Refresh still works when forking an acquire. */
      pulledFromLibraryId: source.pulledFromLibraryId || undefined,
      pulledFromLibraryName: source.pulledFromLibraryName || undefined,
      /* Mock: Use works via source live start URLs. Production must mint a private uniqueId. */
      uniqueId: live.uniqueId,
      deployed: live.deployed,
      testDriveUrl: live.testDriveUrl,
      startPoints: live.startPoints,
      mockSharedForkRuntime: live.mockSharedLibraryRuntime,
    };

    clearMyTawalaDeleted(id);
    const overlay = getMyTawalaOverlay();
    overlay[id] = entry;
    if (!writeJson(PILE_KEY, overlay)) {
      return { ok: false, error: "Could not write My Tawala overlay (localStorage)." };
    }

    return {
      ok: true,
      id,
      sourceId,
      sourceName: sourceDisplay,
      replacedId: conflict ? conflict.id : null,
      entry: { id, ...entry },
    };
  }

  /** Mock-only client gate for library-admin.html — no real auth. See README § Library admin path. */
  function isLibraryAdmin() {
    try {
      return localStorage.getItem(ADMIN_KEY) === "1";
    } catch {
      return false;
    }
  }

  function setLibraryAdmin(on) {
    try {
      if (on) localStorage.setItem(ADMIN_KEY, "1");
      else localStorage.removeItem(ADMIN_KEY);
      return true;
    } catch {
      return false;
    }
  }

  /** Drop transient/computed fields before an entry goes back into a storage overlay. */
  function sanitizeLibraryEntryForOverlay(merged) {
    const clone = { ...merged };
    delete clone.id;
    delete clone.fromPublishOverlay;
    delete clone.matchKind;
    return clone;
  }

  /**
   * Admin rename — edit the display name of any current Library entry (stub, liveReady,
   * main-menu, or a prior Publish overlay) without going through Publish. Matching elsewhere
   * (findMatchingLibraryTargets) already ignores the " (stub)" suffix, so renaming a stub to
   * drop/add that suffix is fine and does not by itself flip the underlying `stub` flag —
   * use Retire (or Publish over it) to actually remove it from the public Library.
   */
  function renameLibraryEntry(libraryId, newName) {
    if (!libraryId) return { ok: false, error: "libraryId required." };
    if (isLibraryRetired(libraryId)) {
      return { ok: false, error: "That entry is retired — Publish/admin overwrite can still reuse its id." };
    }
    const trimmed = String(newName || "").trim();
    if (!trimmed) return { ok: false, error: "Name is required." };
    const merged = libraryReplaceCandidates().find((c) => c.id === libraryId);
    if (!merged) return { ok: false, error: `Unknown Library id: ${libraryId}` };
    const entry = sanitizeLibraryEntryForOverlay(merged);
    entry.name = trimmed;
    const renamedAt = timestampNow();
    entry.updated = formatListDate(renamedAt);
    entry.updatedAt = renamedAt;
    upsertLibraryOverlay(libraryId, entry);
    return { ok: true, libraryId, name: trimmed };
  }

  /**
   * Admin retire — remove any Library id (stub or not) from the public listing with no
   * replacement, always keeping a My Tawala safety copy (never hard-deleted; see README).
   * Same destination for two different reasons:
   *   - **Stub** (`stub: true`) → keeps its " (stub)" name suffix + `stub: true` flag.
   *   - **Unvetted non-stub** (owner Aug 1, 2026 — "a lot of these": real entries
   *     catalogued without ever being checked) → name is left as-is,
   *     NOT given a fake " (stub)" suffix; marked instead via `retiredFromLibraryId` /
   *     `retiredWasStub: false` plus a note in the description, so the owner can pull it into
   *     Designer and re-Publish later once it's actually vetted.
   */
  function retireLibraryEntry(libraryId) {
    if (!libraryId) return { ok: false, error: "libraryId required." };
    if (isLibraryRetired(libraryId)) return { ok: false, error: "Already retired." };
    const merged = libraryReplaceCandidates().find((c) => c.id === libraryId);
    if (!merged) return { ok: false, error: `Unknown Library id: ${libraryId}` };

    const nowIso = timestampNow();
    const now = formatListDate(nowIso);
    const isStub = merged.stub === true;
    const myOverlay = getMyTawalaOverlay();
    const collides =
      Object.prototype.hasOwnProperty.call(myOverlay, libraryId) ||
      (window.TAWALA_MYTAWALA && Object.prototype.hasOwnProperty.call(window.TAWALA_MYTAWALA, libraryId));
    const myId = collides ? `${libraryId}-retired` : libraryId;
    const retiredEntry = {
      name: merged.name,
      stub: isStub, // stubs keep the flag + " (stub)" suffix already in the name; non-stubs stay false
      category: merged.category,
      featured: false,
      iconLabel: merged.iconLabel || iconLabelFromName(merged.name),
      rating: merged.rating || 0,
      comments: merged.comments || 0,
      created: now,
      createdAt: nowIso,
      updated: now,
      updatedAt: nowIso,
      shortDescription: merged.shortDescription || "",
      longDescription:
        (isStub
          ? `Retired Library stub — safety copy kept after admin retirement (${nowIso.slice(0, 10)}). `
          : `Retired from the public Library by admin action (${nowIso.slice(0, 10)}) — kept in My Tawala as a safety copy, not deleted. `) +
        (merged.longDescription || ""),
      jsonFile: merged.jsonFile || null,
      sourcePile: "library-retired",
      retiredFromLibraryId: libraryId,
      retiredAt: nowIso,
      retiredWasStub: isStub,
      deployed: !!merged.deployed,
      startPoints: merged.startPoints || [],
      testDriveUrl: merged.testDriveUrl || null,
    };
    clearMyTawalaDeleted(myId);
    const nextOverlay = getMyTawalaOverlay();
    nextOverlay[myId] = retiredEntry;
    writeJson(PILE_KEY, nextOverlay);
    markLibraryRetired(libraryId);
    removeLibraryOverlay(libraryId); // fully vacate the id — no replacement content stays behind
    return { ok: true, libraryId, myTawalaId: myId, wasStub: isStub, name: merged.name };
  }

  /**
   * Undo an admin/Publish retirement — brings the original catalog row back into the Library
   * listing (does not remove the My Tawala safety copy; delete that row separately if unwanted).
   * No-op if something has since Published/overwritten that same id back into the overlay.
   */
  function restoreLibraryEntry(libraryId) {
    if (!libraryId) return { ok: false, error: "libraryId required." };
    if (!isLibraryRetired(libraryId)) return { ok: false, error: "That id isn't retired." };
    clearLibraryRetired(libraryId);
    return { ok: true, libraryId };
  }

  /**
   * Parse ?deployReceipt=… from Designer Deploy dialog “Show in My Tawala”.
   * Accepts base64url JSON or encodeURIComponent(JSON).
   * Also upserts the My Tawala pile overlay.
   */
  function ingestDeployReceiptFromUrl(search) {
    const params = new URLSearchParams(search || location.search);
    const raw = params.get("deployReceipt");
    if (!raw) return null;
    let parsed = null;
    try {
      parsed = JSON.parse(decodeURIComponent(raw));
    } catch {
      try {
        parsed = JSON.parse(atob(raw.replace(/-/g, "+").replace(/_/g, "/")));
      } catch {
        return null;
      }
    }
    if (!parsed || typeof parsed !== "object") return null;
    if (!parsed.id && parsed.name) parsed.id = slugifyProjectId(parsed.name);
    upsertMyTawalaFromDeploy(parsed);
    recordDeploy(parsed);
    /* Always sync ?project= — upsert may mint a new id when a Save a copy already owns the slug. */
    if (parsed.id) {
      params.set("project", parsed.id);
    }
    params.delete("deployReceipt");
    const qs = params.toString();
    const next = location.pathname + (qs ? "?" + qs : "") + location.hash;
    if (typeof history !== "undefined" && history.replaceState) {
      history.replaceState(null, "", next);
    }
    return parsed;
  }

  /** Build My Tawala Project Details URL with deploy receipt for Designer to open. */
  function myTawalaProjectUrlForDeploy(receipt) {
    const id = receipt.id || slugifyProjectId(receipt.name);
    const payload = encodeURIComponent(
      JSON.stringify({
        id,
        name: receipt.name,
        uniqueId: receipt.uniqueId || null,
        startpoints: receipt.startpoints || [],
        mode: receipt.mode || null,
        at: new Date().toISOString(),
        versionDescription:
          receipt.versionDescription != null
            ? String(receipt.versionDescription)
            : receipt.description != null
              ? String(receipt.description)
              : "",
        snapshotId: receipt.snapshotId || null,
      })
    );
    /* localhost — same origin as owner review tabs (127.0.0.1 ≠ localhost localStorage). */
    return (
      "http://localhost:5500/mytawala-project.html?project=" +
      encodeURIComponent(id) +
      "&deployReceipt=" +
      payload
    );
  }

  /** @deprecated Prefer myTawalaProjectUrlForDeploy — listing + inbox still works. */
  function myTawalaUrlForDeploy(receipt) {
    const payload = encodeURIComponent(
      JSON.stringify({
        id: receipt.id || slugifyProjectId(receipt.name),
        name: receipt.name,
        uniqueId: receipt.uniqueId || null,
        startpoints: receipt.startpoints || [],
        mode: receipt.mode || null,
        at: new Date().toISOString(),
        versionDescription:
          receipt.versionDescription != null
            ? String(receipt.versionDescription)
            : receipt.description != null
              ? String(receipt.description)
              : "",
        snapshotId: receipt.snapshotId || null,
      })
    );
    return "http://localhost:5500/mytawala.html?deployReceipt=" + payload;
  }

  window.TawalaTransfer = {
    CATEGORY_KEY,
    INBOX_KEY,
    PILE_KEY,
    DELETED_KEY,
    LIBRARY_OVERLAY_KEY,
    LIBRARY_RETIRED_KEY,
    slugifyProjectId,
    getCategoryOverrides,
    setProjectCategory,
    clearProjectCategory,
    withCategoryOverrides,
    effectiveCategory,
    CATEGORY_DEFS_KEY,
    effectiveLibraryCategories,
    addLibraryCategory,
    renameLibraryCategory,
    deleteLibraryCategory,
    restoreLibraryCategory,
    getDeletedLibraryCategories,
    getDeployInbox,
    recordDeploy,
    clearDeployInbox,
    removeDeployInboxForProject,
    getMyTawalaOverlay,
    getOverlayEntry,
    getMyTawalaDeleted,
    isMyTawalaDeleted,
    markMyTawalaDeleted,
    clearMyTawalaDeleted,
    deleteMyTawalaProject,
    upsertMyTawalaFromDeploy,
    upsertMyTawalaProperties,
    updateVersionDescription,
    versionHasRedeployableDefinition,
    attachVersionDefinition,
    markVersionCurrentAndDeployed,
    removeMyTawalaOverlay,
    clearMyTawalaOverlay,
    withMyTawalaOverlay,
    ingestDeployReceiptFromUrl,
    myTawalaUrlForDeploy,
    myTawalaProjectUrlForDeploy,
    stripStubSuffix,
    baseNameSlug,
    compactNameKey,
    getLibraryOverlay,
    getLibraryOverlayEntry,
    upsertLibraryOverlay,
    removeLibraryOverlay,
    clearLibraryOverlay,
    scrubDiscardedLibraryStubs,
    scrubDiscardedMyTawalaSeeds,
    rehydrateAcquireLiveUrls,
    isDiscardedPublicLibraryEntry,
    getLibraryRetired,
    isLibraryRetired,
    markLibraryRetired,
    clearLibraryRetired,
    withLibraryOverlay,
    libraryReplaceCandidates,
    findMatchingLibraryTargets,
    publishToLibrary,
    setProjectLibraryActive,
    findPullCandidates,
    pullFromLibrary,
    myTawalaIdTaken,
    uniqueMyTawalaSlug,
    myTawalaNameTaken,
    findMyTawalaByName,
    suggestUniqueMyTawalaName,
    renameMyTawalaProject,
    saveCopyFromLibrary,
    suggestMakeCopyName,
    makeCopyOfMyTawalaProject,
    ADMIN_KEY,
    isLibraryAdmin,
    setLibraryAdmin,
    renameLibraryEntry,
    retireLibraryEntry,
    restoreLibraryEntry,
  };
})();
