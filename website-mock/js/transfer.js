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
 *   tawala.mock.myTawalaDeleted — { [projectId]: true } account-private My Tawala removals
 *     (does not touch public Library / TAWALA_LIBRARY / liveReady)
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

  /**
   * Account-private My Tawala Delete: remove this account’s row + Deploy overlay /
   * inbox receipt. Does not touch public Library catalog, liveReady, or :8080 XML.
   * Seed rows from TAWALA_MYTAWALA stay hidden via deleted set until re-Deploy restores them.
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
   * Returns { id, ...entry } or null.
   */
  function upsertMyTawalaFromDeploy(receipt) {
    if (!receipt || !receipt.name) return null;
    const id = receipt.id || slugifyProjectId(receipt.name);
    /* Re-Deploy / Show in My Tawala restores a previously deleted row. */
    clearMyTawalaDeleted(id);
    const startPoints = (Array.isArray(receipt.startpoints) ? receipt.startpoints : []).map((sp) => ({
      label: sp.form || sp.label || "Start",
      url: sp.url || null,
    }));
    const firstUrl = (startPoints.find((s) => s && s.url) || {}).url || null;
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
    const overlay = getMyTawalaOverlay();
    const prev = overlay[id] || null;
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
    const overlay = getMyTawalaOverlay();
    const deleted = getMyTawalaDeleted();
    const byId = new Map();
    (entries || []).forEach((p) => {
      if (p && p.id && !deleted[p.id]) byId.set(p.id, p);
    });
    Object.keys(overlay).forEach((id) => {
      if (deleted[id]) return;
      const data = overlay[id];
      if (!data || typeof data !== "object") return;
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

  /**
   * Merge Publish overlay on top of catalog Library entries. Retired **stub** ids are dropped
   * from the catalog side (they left the public Library); an overlay entry at that same id
   * (e.g. a fresh Publish that happens to reuse the slug) still shows — retirement only hides
   * the old catalog row, it never blocks a new Publish from using that id.
   */
  function withLibraryOverlay(entries) {
    const overlay = getLibraryOverlay();
    const retired = getLibraryRetired();
    const byId = new Map();
    (entries || []).forEach((p) => {
      if (p && p.id && !retired[p.id]) byId.set(p.id, p);
    });
    Object.keys(overlay).forEach((id) => {
      const data = overlay[id];
      if (!data || typeof data !== "object") return;
      const existing = byId.get(id);
      byId.set(
        id,
        existing
          ? { ...existing, ...data, id, fromPublishOverlay: true }
          : { id, ...data, fromPublishOverlay: true }
      );
    });
    return Array.from(byId.values());
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
      startPoints: (sourceProject && sourceProject.startPoints) || [],
      testDriveUrl: (sourceProject && sourceProject.testDriveUrl) || null,
      uniqueId: (sourceProject && sourceProject.uniqueId) || null,
    };
    /* liveReady is an owner-vetted cue — a fresh Publish overlay never claims it automatically. */

    upsertLibraryOverlay(libraryId, entry);

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
    recordDeploy(parsed);
    upsertMyTawalaFromDeploy(parsed);
    if (parsed.id && !params.get("project")) {
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
      })
    );
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
    getLibraryRetired,
    isLibraryRetired,
    markLibraryRetired,
    clearLibraryRetired,
    withLibraryOverlay,
    libraryReplaceCandidates,
    findMatchingLibraryTargets,
    publishToLibrary,
    findPullCandidates,
    pullFromLibrary,
    ADMIN_KEY,
    isLibraryAdmin,
    setLibraryAdmin,
    renameLibraryEntry,
    retireLibraryEntry,
    restoreLibraryEntry,
  };
})();
