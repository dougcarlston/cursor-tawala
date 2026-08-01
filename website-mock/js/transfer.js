/**
 * Symbiotic transfer + Library category overrides (website-mock).
 *
 * Cross-origin note: Designer (:5173) and this mock (:5500) do not share
 * localStorage. Deploy → My Tawala uses a query receipt opened from Designer,
 * or a manual paste into the My Tawala inbox panel.
 *
 * Keys (local to :5500):
 *   tawala.mock.libraryCategoryOverrides — { [projectId]: categoryLabel }
 *   tawala.mock.deployInbox — recent Designer deploy receipts
 *   tawala.mock.myTawalaOverlay — { [projectId]: catalog-shaped entry } merged into My Tawala pile
 *   tawala.mock.myTawalaDeleted — { [projectId]: true } account-private My Tawala removals
 *     (does not touch public Library / TAWALA_LIBRARY / liveReady)
 */
(function () {
  const CATEGORY_KEY = "tawala.mock.libraryCategoryOverrides";
  const INBOX_KEY = "tawala.mock.deployInbox";
  const PILE_KEY = "tawala.mock.myTawalaOverlay";
  const DELETED_KEY = "tawala.mock.myTawalaDeleted";
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

  function formatListDate(d) {
    const dt = d instanceof Date ? d : new Date(d || Date.now());
    if (Number.isNaN(dt.getTime())) return "—";
    const y = String(dt.getFullYear()).slice(-2);
    return `${dt.getMonth() + 1}/${dt.getDate()}/${y}`;
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
    const now = formatListDate();
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
      updated: now,
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
      lastDeployAt: receipt.at || new Date().toISOString(),
    };
    overlay[id] = entry;
    writeJson(PILE_KEY, overlay);
    return { id, ...entry };
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
    slugifyProjectId,
    getCategoryOverrides,
    setProjectCategory,
    clearProjectCategory,
    withCategoryOverrides,
    effectiveCategory,
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
    removeMyTawalaOverlay,
    clearMyTawalaOverlay,
    withMyTawalaOverlay,
    ingestDeployReceiptFromUrl,
    myTawalaUrlForDeploy,
    myTawalaProjectUrlForDeploy,
  };
})();
