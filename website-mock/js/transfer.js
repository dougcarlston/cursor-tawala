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
 */
(function () {
  const CATEGORY_KEY = "tawala.mock.libraryCategoryOverrides";
  const INBOX_KEY = "tawala.mock.deployInbox";
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
      id:
        receipt.id ||
        String(receipt.name || "project")
          .toLowerCase()
          .replace(/[^a-z0-9]+/g, "-")
          .replace(/^-|-$/g, "") ||
        "deploy",
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

  /**
   * Parse ?deployReceipt=… from Designer Deploy dialog “Show in My Tawala”.
   * Accepts base64url JSON or encodeURIComponent(JSON).
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
    recordDeploy(parsed);
    params.delete("deployReceipt");
    const qs = params.toString();
    const next = location.pathname + (qs ? "?" + qs : "") + location.hash;
    if (typeof history !== "undefined" && history.replaceState) {
      history.replaceState(null, "", next);
    }
    return parsed;
  }

  /** Build My Tawala URL with deploy receipt for Designer to open. */
  function myTawalaUrlForDeploy(receipt) {
    const payload = encodeURIComponent(
      JSON.stringify({
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
    getCategoryOverrides,
    setProjectCategory,
    clearProjectCategory,
    withCategoryOverrides,
    effectiveCategory,
    getDeployInbox,
    recordDeploy,
    clearDeployInbox,
    ingestDeployReceiptFromUrl,
    myTawalaUrlForDeploy,
  };
})();
