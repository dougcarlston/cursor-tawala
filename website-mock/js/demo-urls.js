/**
 * Library vs My Tawala catalogs + local :8080 start URLs.
 *
 * Source piles (JSON backups):
 *   website-mock/projects/library/  ← ~/Projects/Tawala Projects/WebLibrary
 *   website-mock/projects/mytawala/ ← ~/Projects/Tawala Projects/MyTawala
 *
 * Public Library holds vetted liveReady try-outs only (Aug 10, 2026 cleanup). New Project
 * starters live under designer-web/public/samples/templates/ (catalog.ts). Sign-up Sheet and
 * Sign-up Sheet w Email are Designer New Project only until a good Deploy is re-Published.
 * Empty/Blank, Form with Process, Form with Process & Document stay Designer-only.
 *
 * Library listing groups mirror Designer File → New Project categories
 * (Activities / Meetings and Gatherings / Polls and Surveys), plus WebLibrary
 * extras (Sports / Business / Entertainment / Advanced). Basic is Designer-only — never listed.
 *
 * liveReady: true — owner-vetted product with a working :8080 test-drive (quiet “Live” cue in Library list).
 * Update live URLs after Deploy (Designer File→Deploy or POST /api/deploy) / deploy-tawala-template.mjs.
 *
 * Library catalog (Aug 10, 2026): only owner-vetted liveReady try-outs. WebLibrary stubs and the
 * broken Sign-up Sheet Template Library seed were removed — do not reintroduce stubs. New Project
 * starters (including Sign-up Sheet / Sign-up Sheet w Email) live on Designer → File → New Project
 * (`designer-web/src/templates/catalog.ts` + `public/samples/templates/`). Re-Publish to Library
 * when a finished Deploy exists. See README.md § "Library vs New Project templates".
 */

/**
 * Collapsible Library groups — New Project order first, then WebLibrary extras.
 * slug → ?cat= filter; label → project.category display string.
 */
window.TAWALA_LIBRARY_CATEGORIES = [
  { slug: "activities", label: "Activities", source: "new-project" },
  { slug: "meetings", label: "Meetings and Gatherings", source: "new-project" },
  { slug: "polls", label: "Polls and Surveys", source: "new-project" },
  { slug: "sports", label: "Sports", source: "weblibrary" },
  { slug: "business", label: "Business", source: "weblibrary" },
  { slug: "entertainment", label: "Entertainment", source: "weblibrary" },
  { slug: "advanced", label: "Advanced", source: "weblibrary" },
];
window.TAWALA_LIBRARY = {
  "simple-survey": {
    "name": "Simple Survey Template",
    "category": "Polls and Surveys",
    "featured": true,
    "iconLabel": "SS",
    "rating": 4,
    "comments": 12,
    "updated": "6/27/26",
    "shortDescription": "A one-question survey with an instant results report.",
    "longDescription": "Replace the sample question with your own multiple-choice question. Respondents pick an answer; the Report form shows live tallies.",
    "jsonFile": "designer-web/public/samples/templates/simple-survey.json",
    "themePath": "mvsc",
    "sourcePile": "main-menu",
    "liveReady": true,
    "deployed": true,
    "versionNumber": 1,
    "timesUsed": 210,
    "cloneCount": 56,
    "startPoints": [
      {
        "label": "Survey",
        "url": "http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/gy1zssbrwm4fgfm/d6ceolx.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey"
  },
  "potluck": {
    "name": "Potluck Template",
    "category": "Meetings and Gatherings",
    "featured": true,
    "iconLabel": "PL",
    "rating": 4,
    "comments": 15,
    "updated": "7/29/26",
    "shortDescription": "Potluck invitation \u2014 headcount, dish contributions, and a shared report.",
    "longDescription": "Invite guests to a potluck, collect RSVPs and what each person will bring. Uses Potluck Organizer (start), Report, documents, and processes for thanks and delete.",
    "jsonFile": "designer-web/public/samples/templates/potluck.json",
    "themePath": "default",
    "sourcePile": "main-menu",
    "liveReady": true,
    "deployed": true,
    "versionNumber": 1,
    "timesUsed": 128,
    "cloneCount": 34,
    "startPoints": [
      {
        "label": "Potluck Organizer",
        "url": "http://localhost:8080/p/52ozm3kqd58zlss/uhqc1kc.Potluck+Organizer"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/52ozm3kqd58zlss/cni7mae.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/52ozm3kqd58zlss/uhqc1kc.Potluck+Organizer"
  },
  "get-together": {
    "name": "Get Together Template",
    "category": "Meetings and Gatherings",
    "featured": true,
    "iconLabel": "GT",
    "rating": 4,
    "comments": 11,
    "updated": "7/2/26",
    "shortDescription": "Find the best date for an event \u2014 availability plus top preference.",
    "longDescription": "Two MCQs: which dates work (multi-select) and top preference (single). Report includes a question-correlation table to see the best overlap.",
    "jsonFile": "designer-web/public/samples/templates/get-together.json",
    "themePath": "greentea",
    "sourcePile": "main-menu",
    "liveReady": true,
    "deployed": true,
    "versionNumber": 1,
    "timesUsed": 67,
    "cloneCount": 18,
    "startPoints": [
      {
        "label": "Survey",
        "url": "http://localhost:8080/p/b6do4s50iq64vl8/g6zi1ar.Survey"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/b6do4s50iq64vl8/ejeypox.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/b6do4s50iq64vl8/g6zi1ar.Survey"
  },
  "horses-and-penguins-test": {
    "name": "Horses and Penguins Test",
    "category": "Entertainment",
    "featured": false,
    "iconLabel": "HA",
    "rating": 0,
    "comments": 0,
    "updated": "7/30/26",
    "shortDescription": "Fun quiz — horses vs penguins. Score tracking with Process math.",
    "longDescription": "Owner-vetted Entertainment try-out. One form, one process (score/wrong math), six answer documents. Theme style2. Start point: Form 1.",
    "jsonFile": "projects/library/Horses and Penguins Test.json",
    "themePath": "style2",
    "sourcePile": "library",
    "liveReady": true,
    "deployed": true,
    "versionNumber": 1,
    "timesUsed": 19,
    "cloneCount": 5,
    "startPoints": [
      {
        "label": "Form 1",
        "url": "http://localhost:8080/p/wg77ytn0bgq1x70/wdq78g1.Form+1"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/wg77ytn0bgq1x70/wdq78g1.Form+1"
  },
  "multiple-question-survey": {
    "name": "Multiple Question Survey Template",
    "category": "Polls and Surveys",
    "featured": false,
    "iconLabel": "MQ",
    "rating": 4,
    "comments": 9,
    "updated": "7/30/26",
    "shortDescription": "Multi-question poll with bar-graph tallies and a response table on Report.",
    "longDescription": "Owner-vetted Polls and Surveys try-out (corrected port). Survey collects name, several multiple-choice questions, and optional results link. Report shows choice-tally tables per MCQ plus an itemization table of all responses.",
    "jsonFile": "projects/library/Multiple Question Survey Template.json",
    "themePath": "default",
    "sourcePile": "library",
    "liveReady": true,
    "deployed": true,
    "versionNumber": 1,
    "timesUsed": 44,
    "cloneCount": 12,
    "startPoints": [
      {
        "label": "Survey",
        "url": "http://localhost:8080/p/grniytf6dvmobqe/y7ucha7.Survey"
      },
      {
        "label": "Report",
        "url": "http://localhost:8080/p/grniytf6dvmobqe/w2licdd.Report"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/grniytf6dvmobqe/y7ucha7.Survey"
  },
  "online-exam-builder": {
    "name": "Online Exam Builder",
    "category": "Polls and Surveys",
    "featured": false,
    "iconLabel": "OE",
    "rating": 0,
    "comments": 0,
    "updated": "8/4/26",
    "shortDescription": "Build and administer an online exam — questions, scoring, and examinee results.",
    "longDescription": "Owner-vetted Polls and Surveys Live app (8-3-26 build). Administration/Setup to configure the exam and questions; Exam for examinees; CustomizationPreview for branding. Library Test Drive opens Administration first.",
    "jsonFile": "projects/library/Online Exam Builder.json",
    "themePath": "default",
    "sourcePile": "library",
    "liveReady": true,
    "deployed": true,
    "uniqueId": "u3hkqgwtrepjlur",
    "versionNumber": 1,
    "timesUsed": 95,
    "cloneCount": 22,
    "startPoints": [
      {
        "label": "Exam",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/sto3lpi.Exam"
      },
      {
        "label": "Administration",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/ef6sx16.Administration"
      },
      {
        "label": "CustomizationPreview",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/oio6z9y.CustomizationPreview"
      },
      {
        "label": "Setup",
        "url": "http://localhost:8080/p/u3hkqgwtrepjlur/dc2nyex.Setup"
      }
    ],
    "testDriveUrl": "http://localhost:8080/p/u3hkqgwtrepjlur/ef6sx16.Administration"
  }
  // signup-sheet — removed from public Library (owner Aug 10, 2026):
  // old/broken Library seed + Test Drive uniqueId; use Designer → File → New Project
  // → Sign-up Sheet (`designer-web/public/samples/templates/signup-sheet.json`).
  // signup-sheet-email — retired Aug 1, 2026 (Designer New Project only).
  // WebLibrary stubs (AlexTimon, DirtBowl, …) discarded Aug 10, 2026 — do not re-seed.
};

/**
 * Former public-Library seed ids that must never reappear via localStorage overlay
 * (Save a copy / Rename used to snapshot full catalog rows into tawala.mock.libraryOverlay).
 * transfer.js scrub + withLibraryOverlay filter against this list every Library load.
 */
window.TAWALA_LIBRARY_DISCARDED_IDS = [
  "signup-sheet",
  "signup-sheet-email",
  "sign-up-sheet",
  "sign-up-sheet-template",
  "alextimon",
  "automated-list-builder",
  "clientprofiler",
  "cyo-checkdeposit-request1",
  "cyo-exceptions-app",
  "dirtbowl",
  "genericlistmanager",
  "league-age-calculator",
  "lunch-order-menu",
  "mvsc-communicator",
  "mvsc-registration",
  "sportsdashboards-template",
  "st-patrick-sportsdashboards",
  "tawala-invoicing",
];
/**
 * My Tawala seed — empty by design (owner Aug 10, 2026).
 * Listing shows only: Designer Push / Show in My Tawala (overlay) + Library Save a copy
 * (overlay). Not a fake catalog of converted-backup / archive rows. JSON under
 * projects/mytawala/ may remain on disk for Edit-in-Designer / archives — do not re-seed
 * broken undeployed stubs into TAWALA_MYTAWALA. Online Exam lives on Library; Save a copy
 * or Push to get it into My Tawala.
 */
window.TAWALA_MYTAWALA = {};

/**
 * Former fake My Tawala seed ids (converted backups + fake live Online Exam seed).
 * transfer.js marks these deleted on load when they are not a real Push/Save-a-copy overlay
 * row, so hard-refresh never resurrects archive stubs from an old browser cache of the seed.
 */
window.TAWALA_MYTAWALA_DISCARDED_SEED_IDS = [
  "bbbulkmail",
  "campaigndashboards",
  "cyo-check-request",
  "cyo-checkdeposit-request",
  "designer-candidate-app-01",
  "dirtbowl-communicator",
  "displaylabeltest",
  "emailer-with-signup",
  "online-exam-builder",
  "paypal-tester",
  "potluck-kids-too",
  "realdirtwheader",
  "shared-to-do",
  "single-question-poll-or-survey",
];

/** @deprecated Use TAWALA_LIBRARY — kept so older snippets keep working. */
window.TAWALA_DEMO_URLS = window.TAWALA_LIBRARY;

/** Helpers shared by library / home / My Tawala / detail pages. */
/**
 * Offline demo Records for Purge review when live counts are unavailable (owner cannot
 * Use forms to mint real submissions). Keyed by uniqueId. First read seeds into
 * localStorage `tawala.mock.responseCounts`; Purge clears that entry (does not re-seed
 * until Reseed / localStorage key removed). Never overrides a successful :3001 export
 * (including real count 0). Falls back when :3001 is down *or* answers but cannot reach
 * Postgres/Docker (typical: health 200 + export 502 “Docker daemon”).
 */
window.TAWALA_DEMO_RESPONSE_SEEDS = {
  u3hkqgwtrepjlur: {
    projectId: "online-exam-builder",
    byForm: {
      Exam: 12,
      Answer: 15,
      Question: 8,
      ShowExamineeDetail: 6,
      Scoring: 4,
      Administration: 3,
      Setup: 2,
    },
  },
};

/** Short-lived :8080 probe cache — shared by Library Test Drive and My Tawala Use. */
let _runtimeProbeCache = { at: 0, ok: null };
const _RUNTIME_PROBE_TTL_MS = 4000;

window.TawalaDemo = {
  /** Listing title — never show file extensions (.json / .tawala). On-disk format may still be JSON. */
  displayName(name) {
    return String(name || "")
      .replace(/\.tawala\.xml$/i, "")
      .replace(/\.tawala$/i, "")
      .replace(/\.json$/i, "");
  },
  /** True when href targets local Tomcat (:8080) — live form runtime Test Drive / Use open. */
  isLocalJavaRuntimeUrl(url) {
    if (!url || url === "#") return false;
    try {
      const u = new URL(url, typeof location !== "undefined" ? location.href : "http://localhost/");
      const host = (u.hostname || "").toLowerCase();
      if (host !== "localhost" && host !== "127.0.0.1") return false;
      const port = String(u.port || (u.protocol === "https:" ? "443" : "80"));
      return port === "8080";
    } catch {
      return /(?:localhost|127\.0\.0\.1):8080/i.test(String(url));
    }
  },
  /**
   * Probe local Java/Tomcat. Uses no-cors so CORS never false-negatives when :8080 is up;
   * connection refused / abort → unreachable. Hits `/home` (not `/` — that 404s and used to
   * mark Docker unhealthy). Does not prove World finished init or that a project token exists.
   */
  async probeLocalJavaRuntime(timeoutMs) {
    const now = Date.now();
    if (_runtimeProbeCache.ok !== null && now - _runtimeProbeCache.at < _RUNTIME_PROBE_TTL_MS) {
      return _runtimeProbeCache.ok;
    }
    const ms = typeof timeoutMs === "number" ? timeoutMs : 1200;
    const ctrl = new AbortController();
    const timer = setTimeout(() => ctrl.abort(), ms);
    try {
      await fetch("http://127.0.0.1:8080/home", {
        method: "GET",
        mode: "no-cors",
        cache: "no-store",
        signal: ctrl.signal,
      });
      _runtimeProbeCache = { at: Date.now(), ok: true };
      return true;
    } catch {
      _runtimeProbeCache = { at: Date.now(), ok: false };
      return false;
    } finally {
      clearTimeout(timer);
    }
  },
  /**
   * Server-side check via :3001 (Tomcat has no CORS). Detects the legacy “We are very sorry”
   * fail page when World is not initialized or the form path is missing — HTTP 200 alone is
   * not enough. Returns { ok:true } | { ok:false, reason, detail }.
   */
  async probeFormStartUrl(url, timeoutMs) {
    if (!url || !this.isLocalJavaRuntimeUrl(url)) {
      return { ok: true, skipped: true };
    }
    const ms = typeof timeoutMs === "number" ? timeoutMs : 2500;
    const api =
      this.purgeApiBase().replace(/\/$/, "") +
      "/api/probe-java-url?url=" +
      encodeURIComponent(url);
    const ctrl = new AbortController();
    const timer = setTimeout(() => ctrl.abort(), ms);
    try {
      const res = await fetch(api, { method: "GET", cache: "no-store", signal: ctrl.signal });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return {
          ok: false,
          reason: "probe-http",
          detail: data.error || `HTTP ${res.status}`,
        };
      }
      if (data.ok === true) return { ok: true, status: data.status, title: data.title };
      return {
        ok: false,
        reason: data.reason || "fail-page",
        detail: data.detail || data.error || "Java form URL is not ready",
        status: data.status,
      };
    } catch (e) {
      /* :3001 down — don’t block Test Drive; Tomcat reachability was already probed. */
      return {
        ok: true,
        skipped: true,
        detail: String((e && e.message) || e),
      };
    } finally {
      clearTimeout(timer);
    }
  },
  /** Base repo categories + admin Add/Rename/Delete overlay (see transfer.js § category defs). */
  libraryCategories() {
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.effectiveLibraryCategories === "function"
    ) {
      return window.TawalaTransfer.effectiveLibraryCategories();
    }
    return window.TAWALA_LIBRARY_CATEGORIES || [];
  },
  categoryBySlug(slug) {
    return this.libraryCategories().find((c) => c.slug === slug) || null;
  },
  categoryByLabel(label) {
    return this.libraryCategories().find((c) => c.label === label) || null;
  },
  libraryEntries() {
    // Strip discarded stub snapshots from localStorage before merging (Save a copy / Rename
    // used to freeze full catalog rows into tawala.mock.libraryOverlay).
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.scrubDiscardedLibraryStubs === "function"
    ) {
      window.TawalaTransfer.scrubDiscardedLibraryStubs();
    }
    const base = Object.keys(window.TAWALA_LIBRARY).map((id) => ({
      id,
      ...window.TAWALA_LIBRARY[id],
    }));
    // Publish (My Tawala → Library) writes a localStorage overlay (transfer.js) and can
    // retire a matching stub out of the listing. See README § Publish.
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.withLibraryOverlay === "function"
    ) {
      return window.TawalaTransfer.withLibraryOverlay(base);
    }
    return base;
  },
  myTawalaEntries() {
    const base = Object.keys(window.TAWALA_MYTAWALA).map((id) => ({
      id,
      ...window.TAWALA_MYTAWALA[id],
    }));
    // Deploy → Show in My Tawala writes a localStorage overlay (transfer.js).
    // Deleted ids (account-private) are filtered inside withMyTawalaOverlay.
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.withMyTawalaOverlay === "function"
    ) {
      return window.TawalaTransfer.withMyTawalaOverlay(base);
    }
    return base;
  },
  /** Prefer Library, then My Tawala (detail pages that accept either id). */
  get(id) {
    return this.getLibrary(id) || this.getMyTawala(id) || null;
  },
  getLibrary(id) {
    if (!id) return null;
    const hasTransfer = typeof window !== "undefined" && window.TawalaTransfer;
    // A retired stub is gone from the public Library — only a fresh Publish overlay at the
    // same id (rare) should still resolve here; otherwise callers fall back to My Tawala.
    if (hasTransfer && typeof window.TawalaTransfer.isLibraryRetired === "function" && window.TawalaTransfer.isLibraryRetired(id)) {
      const overlayOnly =
        typeof window.TawalaTransfer.getLibraryOverlayEntry === "function"
          ? window.TawalaTransfer.getLibraryOverlayEntry(id)
          : null;
      if (
        overlayOnly &&
        typeof window.TawalaTransfer.isDiscardedPublicLibraryEntry === "function" &&
        window.TawalaTransfer.isDiscardedPublicLibraryEntry(id, overlayOnly)
      ) {
        return null;
      }
      /* Catalog keys are not on the row object — always stamp id (library-detail Save a copy). */
      return overlayOnly ? { ...overlayOnly, id } : null;
    }
    const base = window.TAWALA_LIBRARY[id] || null;
    if (hasTransfer && typeof window.TawalaTransfer.getLibraryOverlayEntry === "function") {
      const overlay = window.TawalaTransfer.getLibraryOverlayEntry(id);
      if (overlay) {
        const merged = base ? { ...base, ...overlay, id } : { ...overlay, id };
        if (
          typeof window.TawalaTransfer.isDiscardedPublicLibraryEntry === "function" &&
          window.TawalaTransfer.isDiscardedPublicLibraryEntry(id, merged)
        ) {
          return null;
        }
        return merged;
      }
    }
    if (
      !base &&
      hasTransfer &&
      typeof window.TawalaTransfer.isDiscardedPublicLibraryEntry === "function" &&
      window.TawalaTransfer.isDiscardedPublicLibraryEntry(id, null)
    ) {
      return null;
    }
    /* Seed rows live under TAWALA_LIBRARY[id] without an id field — stamp it for callers
     * (library-detail Save a copy uses project.id; libraryEntries() already injects id). */
    return base ? { ...base, id } : null;
  },
  getMyTawala(id) {
    if (!id) return null;
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer
    ) {
      if (typeof window.TawalaTransfer.scrubDiscardedMyTawalaSeeds === "function") {
        window.TawalaTransfer.scrubDiscardedMyTawalaSeeds();
      }
      if (typeof window.TawalaTransfer.rehydrateAcquireLiveUrls === "function") {
        window.TawalaTransfer.rehydrateAcquireLiveUrls();
      }
      if (
        typeof window.TawalaTransfer.isMyTawalaDeleted === "function" &&
        window.TawalaTransfer.isMyTawalaDeleted(id)
      ) {
        return null;
      }
    }
    const base = window.TAWALA_MYTAWALA[id] || null;
    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.getOverlayEntry === "function"
    ) {
      const overlay = window.TawalaTransfer.getOverlayEntry(id);
      if (overlay) {
        if (!base) return { ...overlay, id };
        const merged = { ...base, ...overlay, id };
        /* Empty overlay theme must not wipe catalog / definition themePath. */
        if (!merged.themePath && base.themePath) merged.themePath = base.themePath;
        if (!merged.jsonFile && base.jsonFile) merged.jsonFile = base.jsonFile;
        if (!Array.isArray(merged.formNames) || !merged.formNames.length) {
          if (Array.isArray(base.formNames) && base.formNames.length) {
            merged.formNames = base.formNames.slice();
          }
        }
        return merged;
      }
    }
    return base ? { ...base, id } : null;
  },
  /** @deprecated Prefer libraryEntries() */
  entries() {
    return this.libraryEntries();
  },
  featured() {
    return this.libraryEntries().filter((p) => p.featured);
  },
  /** Non-featured library projects (home “Featured Solutions” list below the icons). */
  moreSolutions() {
    return this.libraryEntries().filter((p) => !p.featured);
  },
  isDeployed(p) {
    return !!(p && p.deployed && p.testDriveUrl);
  },
  /** Owner-vetted Library product with a live :8080 test-drive (distinct from placeholders). */
  isLiveReady(p) {
    return !!(p && p.liveReady && this.isDeployed(p));
  },
  /** Quiet “Live” cue for vetted rows — not a banner; placeholders omit this. */
  liveReadyHtml(p) {
    try {
      if (!this.isLiveReady(p)) return "";
      return (
        '<span class="library-live-ready" title="Vetted · live on localhost:8080">Live</span>'
      );
    } catch {
      return "";
    }
  },
  /** Same contract as designer-web `isValidUniqueId` (1–20 alphanumeric). */
  isValidUniqueId(uniqueId) {
    return typeof uniqueId === "string" && /^[A-Za-z0-9]{1,20}$/.test(uniqueId);
  },
  /** Extract uniqueId from `/p/{uniqueId}/…` (Library / My Tawala test-drive URLs). */
  uniqueIdFromUrl(url) {
    if (!url) return null;
    const m = String(url).match(/\/p\/([A-Za-z0-9]{1,20})(?:\/|$)/);
    return m ? m[1] : null;
  },
  /**
   * Prefer explicit deploy uniqueId (overlay / receipt), then testDriveUrl,
   * then first start point with a URL.
   */
  uniqueIdForProject(p) {
    if (!p) return null;
    if (this.isValidUniqueId(p.uniqueId)) return p.uniqueId;
    const fromTest = this.uniqueIdFromUrl(p.testDriveUrl);
    if (fromTest) return fromTest;
    const sps = p.startPoints || [];
    for (let i = 0; i < sps.length; i++) {
      const id = this.uniqueIdFromUrl(sps[i] && sps[i].url);
      if (id) return id;
    }
    return null;
  },
  /**
   * Resolve :8080 uniqueId for My Tawala Purge.
   * Prefer My Tawala overlay/seed, then latest deploy-inbox receipt.
   * Does not read public Library catalog (avoids purging a twin’s data by slug collision).
   */
  resolvePurgeUniqueId(projectId) {
    if (!projectId) return null;
    const mine =
      typeof this.getMyTawala === "function" ? this.getMyTawala(projectId) : null;
    const fromMine = this.uniqueIdForProject(mine);
    if (fromMine) return fromMine;

    if (
      typeof window !== "undefined" &&
      window.TawalaTransfer &&
      typeof window.TawalaTransfer.getDeployInbox === "function"
    ) {
      const receipts = window.TawalaTransfer.getDeployInbox().filter(
        (e) => e && e.id === projectId
      );
      for (let i = 0; i < receipts.length; i++) {
        const r = receipts[i];
        if (this.isValidUniqueId(r.uniqueId)) return r.uniqueId;
        const sps = Array.isArray(r.startpoints) ? r.startpoints : [];
        for (let j = 0; j < sps.length; j++) {
          const id = this.uniqueIdFromUrl(sps[j] && sps[j].url);
          if (id) return id;
        }
      }
    }

    return null;
  },
  /**
   * Dev API base for purge (designer-web :3001). Override with window.TAWALA_DEV_API.
   */
  purgeApiBase() {
    return (typeof window !== "undefined" && window.TAWALA_DEV_API) || "http://localhost:3001";
  },

  _mockResponseCountsKey: "tawala.mock.responseCounts",

  hasDemoResponseSeed(uniqueId) {
    return !!(
      this.isValidUniqueId(uniqueId) &&
      window.TAWALA_DEMO_RESPONSE_SEEDS &&
      window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId]
    );
  },

  _readMockResponseStore() {
    try {
      const raw = localStorage.getItem(this._mockResponseCountsKey);
      if (!raw) return {};
      const parsed = JSON.parse(raw);
      return parsed && typeof parsed === "object" ? parsed : {};
    } catch {
      return {};
    }
  },

  _writeMockResponseStore(store) {
    try {
      localStorage.setItem(this._mockResponseCountsKey, JSON.stringify(store || {}));
    } catch {
      /* ignore quota / private mode */
    }
  },

  _sumByForm(byForm) {
    let n = 0;
    Object.keys(byForm || {}).forEach((k) => {
      const v = Number(byForm[k]);
      if (!Number.isNaN(v) && v >= 0) n += v;
    });
    return n;
  },

  _mockCountSuccess(uniqueId, mock, liveError) {
    if (!mock) return null;
    const out = {
      status: "success",
      uniqueId,
      count: mock.count,
      byForm: mock.byForm,
      source: "mock-seed",
      purged: !!mock.purged,
    };
    if (liveError) out.liveError = String(liveError);
    return out;
  },

  /**
   * Ensure demo uniqueId has a localStorage entry (seed once). After Purge the entry
   * stays with empty byForm so reload does not resurrect demo counts.
   */
  ensureMockResponseCounts(uniqueId) {
    if (!this.isValidUniqueId(uniqueId)) return null;
    const seeds = window.TAWALA_DEMO_RESPONSE_SEEDS || {};
    const seed = seeds[uniqueId];
    if (!seed) return null;
    const store = this._readMockResponseStore();
    if (store[uniqueId]) {
      const byForm = store[uniqueId].byForm && typeof store[uniqueId].byForm === "object"
        ? store[uniqueId].byForm
        : {};
      return {
        uniqueId,
        byForm,
        count: this._sumByForm(byForm),
        source: "mock-seed",
        purged: !!store[uniqueId].purged,
      };
    }
    const byForm = { ...(seed.byForm || {}) };
    store[uniqueId] = {
      projectId: seed.projectId || null,
      byForm,
      seededAt: new Date().toISOString(),
    };
    this._writeMockResponseStore(store);
    return {
      uniqueId,
      byForm,
      count: this._sumByForm(byForm),
      source: "mock-seed",
      purged: false,
    };
  },

  /**
   * Force-restore catalog demo byForm counts (owner Reseed control / DevTools-free).
   * Only for uniqueIds in TAWALA_DEMO_RESPONSE_SEEDS.
   */
  reseedMockResponseCounts(uniqueId) {
    if (!this.hasDemoResponseSeed(uniqueId)) {
      return { status: "failure", uniqueId, error: "No demo Records seed for this project" };
    }
    const seed = window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId];
    const store = this._readMockResponseStore();
    const byForm = { ...(seed.byForm || {}) };
    store[uniqueId] = {
      projectId: seed.projectId || null,
      byForm,
      seededAt: new Date().toISOString(),
      reseededAt: new Date().toISOString(),
    };
    this._writeMockResponseStore(store);
    return {
      status: "success",
      uniqueId,
      byForm,
      count: this._sumByForm(byForm),
      source: "mock-seed",
      purged: false,
    };
  },

  /** Clear mock counts for uniqueId (project-wide offline Purge). */
  clearMockResponseCounts(uniqueId) {
    if (!this.isValidUniqueId(uniqueId)) {
      return { status: "failure", uniqueId, error: "uniqueId required" };
    }
    const before = this.ensureMockResponseCounts(uniqueId);
    const deleted = before ? before.count : 0;
    const store = this._readMockResponseStore();
    store[uniqueId] = {
      projectId: (before && window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId]
        ? window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId].projectId
        : null) || (store[uniqueId] && store[uniqueId].projectId) || null,
      byForm: {},
      purged: true,
      purgedAt: new Date().toISOString(),
    };
    this._writeMockResponseStore(store);
    return {
      status: "success",
      uniqueId,
      source: "mock-seed",
      javaDb: { deleted },
      warning:
        "Mock offline purge — :3001 unreachable; cleared seeded demo Records only (not live Postgres).",
    };
  },

  /**
   * Form-scoped offline Purge against seeded demo counts.
   * @returns {{status:"success"|"failure", uniqueId?:string, removed?:number, remaining?:number, error?:string, warning?:string, source?:string}}
   */
  purgeMockFormResponses(uniqueId, formName) {
    const name = String(formName || "");
    if (!this.isValidUniqueId(uniqueId) || !name) {
      return { status: "failure", error: "uniqueId and formName required" };
    }
    const cur = this.ensureMockResponseCounts(uniqueId);
    if (!cur) {
      return {
        status: "failure",
        uniqueId,
        error: "No offline demo Records for this project (only Online Exam Builder is seeded).",
      };
    }
    const byForm = { ...(cur.byForm || {}) };
    const removed = Number(byForm[name]) || 0;
    delete byForm[name];
    const remaining = this._sumByForm(byForm);
    const store = this._readMockResponseStore();
    store[uniqueId] = {
      projectId: (store[uniqueId] && store[uniqueId].projectId) ||
        (window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId] &&
          window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId].projectId) ||
        null,
      byForm,
      purged: remaining === 0,
      purgedAt: remaining === 0 ? new Date().toISOString() : undefined,
    };
    this._writeMockResponseStore(store);
    return {
      status: "success",
      uniqueId,
      formName: name,
      removed,
      remaining,
      inserted: remaining,
      source: "mock-seed",
      warning:
        "Mock offline purge — :3001 unreachable; cleared seeded demo Records for this form only.",
    };
  },

  _isApiUnreachableError(err) {
    const msg = String((err && err.message) || err || "");
    return (
      /failed to fetch/i.test(msg) ||
      /networkerror/i.test(msg) ||
      /load failed/i.test(msg) ||
      /is designer-web API on :3001/i.test(msg)
    );
  },

  /**
   * Project (+ per-form) Records counts. Prefers a successful :3001 export (live wins,
   * including real 0). When :3001 is down *or* returns failure (e.g. Docker daemon
   * unavailable while health is still 200), fall back to seeded mock for
   * TAWALA_DEMO_RESPONSE_SEEDS uniqueIds — otherwise honest failure ("—").
   */
  async countResponses(uniqueId) {
    if (!uniqueId) return { status: "failure", error: "uniqueId required" };
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/export-responses";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          uniqueId,
          credentials: { user: "dev", password: "dev" },
        }),
      });
      const data = await res.json().catch(() => ({}));
      if (res.ok && data.status === "success") {
        const byForm = {};
        (data.forms || []).forEach((f) => {
          if (f && f.form) byForm[f.form] = (f.rows && f.rows.length) || 0;
        });
        return {
          status: "success",
          uniqueId,
          count: typeof data.count === "number" ? data.count : this._sumByForm(byForm),
          byForm,
          source: data.source || "api",
        };
      }
      /* API answered but could not produce counts — mock only for seeded demos. */
      const liveErr = data.error || `HTTP ${res.status}`;
      const mockOk = this._mockCountSuccess(
        uniqueId,
        this.ensureMockResponseCounts(uniqueId),
        liveErr
      );
      if (mockOk) return mockOk;
      return {
        status: "failure",
        uniqueId,
        error: liveErr,
      };
    } catch (e) {
      const mockOk = this._mockCountSuccess(
        uniqueId,
        this.ensureMockResponseCounts(uniqueId),
        e && e.message
      );
      if (mockOk) return mockOk;
      if (!this._isApiUnreachableError(e)) {
        return {
          status: "failure",
          uniqueId,
          error: String(e.message || e),
        };
      }
      return {
        status: "failure",
        uniqueId,
        error:
          String(e.message || e) +
          " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },

  /**
   * Purge Postgres submissions (+ Node session if present) for a uniqueId.
   * When :3001 is unreachable *or* returns an error (Docker down, etc.) and this
   * uniqueId has demo seed Records, clears the mock store instead (honest warning).
   * Live API success also clears any mock copy.
   */
  async purgeResponses(uniqueId) {
    if (!uniqueId) {
      return { status: "failure", error: "uniqueId required" };
    }
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/purge-responses";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          uniqueId,
          credentials: { user: "dev", password: "dev" },
        }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok || data.status === "failure") {
        if (this.hasDemoResponseSeed(uniqueId)) {
          return this.clearMockResponseCounts(uniqueId);
        }
        return {
          status: "failure",
          uniqueId,
          error: data.error || data.javaDb?.error || `HTTP ${res.status}`,
          ...data,
        };
      }
      /* Keep mock store in sync after a real purge so offline reload stays at 0. */
      if (this.hasDemoResponseSeed(uniqueId)) {
        const store = this._readMockResponseStore();
        const seed = window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId];
        store[uniqueId] = {
          projectId: (store[uniqueId] && store[uniqueId].projectId) || seed.projectId || null,
          byForm: {},
          purged: true,
          purgedAt: new Date().toISOString(),
        };
        this._writeMockResponseStore(store);
      }
      return data;
    } catch (e) {
      if (this.hasDemoResponseSeed(uniqueId)) {
        return this.clearMockResponseCounts(uniqueId);
      }
      return {
        status: "failure",
        uniqueId,
        error:
          String(e.message || e) +
          " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Persist a Designer definition for deep-link open / Deploy-this-version.
   * POST /api/version-snapshots (same store as Show in My Tawala).
   */
  async saveVersionSnapshot(input) {
    const project = input && input.project;
    if (!project || !project.name) {
      return { status: "failure", error: "project required" };
    }
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/version-snapshots";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          project,
          uniqueId: (input && input.uniqueId) || null,
          projectId: (input && input.projectId) || null,
          versionDescription: (input && input.versionDescription) || "",
          at: (input && input.at) || null,
        }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return {
          status: "failure",
          error: data.error || `HTTP ${res.status}`,
        };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        error:
          String(e.message || e) +
          " — is designer-web API on :3001? (cd designer-web && npm run keep)",
      };
    }
  },
  /**
   * Fetch a Designer definition snapshot saved at Show in My Tawala
   * (POST /api/version-snapshots). Used by Deploy-this-version when the overlay
   * has snapshotId but no cached definition body.
   */
  async fetchVersionSnapshot(snapshotId) {
    if (!snapshotId) {
      return { status: "failure", error: "snapshotId required" };
    }
    const url =
      this.purgeApiBase().replace(/\/$/, "") +
      "/api/version-snapshots/" +
      encodeURIComponent(snapshotId);
    try {
      const res = await fetch(url);
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return {
          status: "failure",
          snapshotId,
          error: data.error || `HTTP ${res.status}`,
        };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        snapshotId,
        error:
          String(e.message || e) +
          " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Redeploy a project definition to :8080 / Node runtime (same /api/deploy as Designer).
   * Java keeps the same uniqueId when the project name matches an existing deployment.
   */
  async deployProjectDefinition(project) {
    if (!project || !project.name) {
      return { status: "failure", error: "project required" };
    }
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/deploy";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          credentials: { user: "dev", password: "dev" },
          project,
        }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return {
          status: "failure",
          error: data.error || `HTTP ${res.status}`,
          ...data,
        };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        error:
          String(e.message || e) +
          " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Purge :8080 submissions for a My Tawala project after Publish (owner Aug 1, 2026 — Publish
   * to Library must never leave one account's prior test/demo responses visible to whoever
   * uses the newly-published Library project next). Resolves the same uniqueId as My Tawala
   * PURGE (`resolvePurgeUniqueId`) and reuses `purgeResponses`. Returns `{ status: "skipped" }`
   * when the source project has no linked :8080 deploy yet — Publish still succeeds; callers
   * must surface that responses were NOT cleared rather than staying silent about it.
   */
  async purgeAfterPublish(projectId) {
    const uniqueId = this.resolvePurgeUniqueId(projectId);
    if (!uniqueId) {
      return { status: "skipped", reason: "no-uniqueid", uniqueId: null };
    }
    const result = await this.purgeResponses(uniqueId);
    return { ...result, uniqueId };
  },
  /**
   * Purge project response data then open the :8080 form (clean slate each Test drive).
   * Opens a blank tab synchronously (keeps the user gesture for popup blockers),
   * probes Tomcat before navigating (same offline gate as My Tawala Use), then
   * navigates after purge. Failed / timed-out purge never blocks opening when :8080 is up.
   * If :8080 is down: close the blank tab, alert, stay on :5500 — never “site can’t be reached.”
   * Post-tab-close purge is not available in this static mock.
   */
  async openTestDrive(url, opts) {
    const options = opts || {};
    const purgeFirst = options.purge !== false;
    const purgeMs = typeof options.purgeTimeoutMs === "number" ? options.purgeTimeoutMs : 8000;
    const target = url || null;
    if (!target) return { opened: false, purge: null };

    // Capture gesture before any await — otherwise browsers block the popup.
    const tab = window.open("about:blank", "_blank");

    const closeBlankTab = () => {
      if (tab && !tab.closed) {
        try {
          tab.close();
        } catch {
          /* ignore */
        }
      }
    };

    // Same :8080 gate as Project Data Use — don’t dump the owner onto a dead host
    // or the legacy “We are very sorry” fail page (Tomcat up, World not initialized).
    if (this.isLocalJavaRuntimeUrl(target)) {
      const up = await this.probeLocalJavaRuntime();
      if (!up) {
        closeBlankTab();
        window.alert(
          "Test Drive needs the Java runtime on http://localhost:8080 — it isn’t reachable right now.\n\n" +
            "You’re still on the :5500 mock (no navigation to a dead host).\n\n" +
            "Start Tomcat / the local Java runtime, then try Test Drive again.\n" +
            "Save a copy still works offline."
        );
        return { opened: false, purge: null, offline: true };
      }
      const formProbe = await this.probeFormStartUrl(target);
      if (formProbe && formProbe.ok === false && formProbe.reason !== "probe-http") {
        closeBlankTab();
        window.alert(
          "Test Drive cannot open this form on :8080 right now.\n\n" +
            (formProbe.detail || "The Java runtime returned the legacy fail page.") +
            "\n\nCommon fix when Postgres is healthy: docker restart tawala-tomcat\n" +
            "(World must finish initializing before /p/… form URLs work).\n\n" +
            "You’re still on the :5500 mock."
        );
        return { opened: false, purge: null, failPage: true, formProbe };
      }
    }

    let purge = null;
    try {
      if (purgeFirst) {
        const id = this.uniqueIdFromUrl(target);
        if (id) {
          const timedOut = new Promise((resolve) => {
            setTimeout(
              () => resolve({ status: "failure", uniqueId: id, error: `purge timed out after ${purgeMs}ms` }),
              purgeMs
            );
          });
          purge = await Promise.race([this.purgeResponses(id), timedOut]);
          if (purge.status !== "success") {
            console.warn("[TawalaDemo] purge before test drive failed:", purge.error || purge);
          }
        }
      }
    } catch (e) {
      purge = { status: "failure", error: String((e && e.message) || e) };
      console.warn("[TawalaDemo] purge before test drive threw:", purge.error);
    }

    // Always navigate when runtime is up — blank tab must not stick on purge failure.
    if (tab && !tab.closed) {
      try {
        tab.opener = null;
      } catch {
        /* ignore */
      }
      try {
        tab.location.href = target;
        return { opened: true, purge };
      } catch (navErr) {
        console.warn("[TawalaDemo] tab navigate failed, falling back:", navErr);
      }
    }
    // Popup blocked or navigate failed — last resort (may also be blocked after await).
    window.open(target, "_blank", "noopener");
    return { opened: true, purge, popupBlocked: !tab };
  },
  /**
   * Export submission data for a deployed project (EXPORT / data half of BACKUP).
   * Postgres (Java) or dev session store — see designer-web/server/projectResponses.mjs.
   * @returns {Promise<{status:"success"|"failure", uniqueId:string, source?:"postgres"|"dev-session",
   *   forms?:Array, fieldsByForm?:Record<string,string[]>, count?:number, error?:string}>}
   */
  async exportResponses(uniqueId) {
    if (!uniqueId) return { status: "failure", error: "uniqueId required" };
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/export-responses";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ uniqueId, credentials: { user: "dev", password: "dev" } }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return { status: "failure", uniqueId, error: data.error || `HTTP ${res.status}`, ...data };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        uniqueId,
        error: String(e.message || e) + " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Replace submission data for a deployed project (IMPORT / data half of RESTORE).
   * `forms` must match the shape returned by exportResponses() for the given `source`.
   * @param {string} uniqueId
   * @param {Array} forms
   * @param {{source?: "postgres"|"dev-session", mode?: "replace"|"merge"}} opts
   */
  async importResponses(uniqueId, forms, opts) {
    const options = opts || {};
    if (!uniqueId) return { status: "failure", error: "uniqueId required" };
    if (!Array.isArray(forms)) return { status: "failure", error: "forms array required" };
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/import-responses";
    try {
      const res = await fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          uniqueId,
          forms,
          source: options.source || null,
          mode: options.mode || "replace",
          credentials: { user: "dev", password: "dev" },
        }),
      });
      const data = await res.json().catch(() => ({}));
      if (!res.ok) {
        return { status: "failure", uniqueId, error: data.error || `HTTP ${res.status}`, ...data };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        uniqueId,
        error: String(e.message || e) + " — is designer-web API on :3001? (cd designer-web && npm run dev)",
      };
    }
  },
  /**
   * Prefer end-user start forms over Admin/Setup/Preview when ranking a single
   * operate URL (Online Exam Builder: Exam vs Administration). Used when Use
   * opens a single-start runtime link, and for start-point list ordering.
   * Multi-start Use navigates to Project Details instead (see projectUseTarget).
   * @param {Array<{label?:string,form?:string,url?:string|null}>} startPoints
   * @returns {{label?:string,form?:string,url?:string|null}|null}
   */
  pickPrimaryStartPoint(startPoints) {
    const list = (startPoints || []).filter((s) => s && s.url);
    if (!list.length) return null;
    const labelOf = (s) => String(s.label || s.form || "").trim();
    // Exact / high-value public entry forms first.
    const prefer = [
      /^exam$/i,
      /^registration$/i,
      /^survey$/i,
      /^form\s*\+?\s*1$/i,
      /^form\s*1$/i,
      /^sign[-\s]?up/i,
      /^potluck/i,
      /^exception\s*request/i,
    ];
    for (let p = 0; p < prefer.length; p++) {
      const hit = list.find((s) => prefer[p].test(labelOf(s)));
      if (hit) return hit;
    }
    // Skip admin / setup / preview / reports when a plain operate form exists.
    const deprioritize =
      /admin|setup|customiz|preview|utility|report|dash|config|scoring|answer/i;
    const nonAdmin = list.find((s) => !deprioritize.test(labelOf(s)));
    return nonAdmin || list[0];
  },
  /**
   * Library Test Drive entry point — for apps with Exam + Administration/Setup
   * (Online Exam Builder), open Admin/Setup first so the drive starts where you
   * configure questions. My Tawala Use for multi-start goes to Project Details
   * (not Exam); single-start Use still uses pickPrimaryStartPoint.
   */
  pickLibraryTestDriveStartPoint(startPoints) {
    const list = (startPoints || []).filter((s) => s && s.url);
    if (!list.length) return null;
    const labelOf = (s) => String(s.label || s.form || "").trim();
    const hasExam = list.some((s) => /^exam$/i.test(labelOf(s)));
    if (hasExam) {
      const setupPrefer = [/^administration$/i, /^setup$/i, /^admin$/i];
      for (let p = 0; p < setupPrefer.length; p++) {
        const hit = list.find((s) => setupPrefer[p].test(labelOf(s)));
        if (hit) return hit;
      }
    }
    return this.pickPrimaryStartPoint(startPoints);
  },
  /** Preferred :8080 URL when ranking start points (Exam over Admin when both exist). */
  primaryStartUrl(startPoints, fallbackUrl) {
    const primary = this.pickPrimaryStartPoint(startPoints);
    if (primary && primary.url) return primary.url;
    return fallbackUrl || null;
  },
  /**
   * Library listing / detail Test Drive URL. Prefer Setup/Admin for Exam apps;
   * fall back to stored testDriveUrl.
   */
  libraryTestDriveUrl(project) {
    if (!project) return null;
    const preferred = this.pickLibraryTestDriveStartPoint(project.startPoints);
    if (preferred && preferred.url) return preferred.url;
    return project.testDriveUrl || null;
  },
  /** Delegated clicks for elements with data-testdrive-url (or .js-testdrive href). */
  bindTestDriveClicks(root) {
    const scope = root || document;
    if (scope.__tawalaTestDriveBound) return;
    scope.__tawalaTestDriveBound = true;
    /* Capture: Library Actions cell used to stopPropagation and skip purge-on-start. */
    scope.addEventListener(
      "click",
      (ev) => {
        const el = ev.target.closest("[data-testdrive-url], a.js-testdrive");
        if (!el) return;
        const href =
          el.getAttribute("data-testdrive-url") ||
          (el.classList.contains("js-testdrive") ? el.getAttribute("href") : null);
        if (!href || href === "#") return;
        ev.preventDefault();
        // data-testdrive-purge="false" → open without wiping DB (My Tawala operate / exam after Admin setup).
        const purgeAttr = el.getAttribute("data-testdrive-purge");
        const purge = purgeAttr !== "false" && purgeAttr !== "0";
        const libraryId = el.getAttribute("data-project") || "";
        if (
          libraryId &&
          purge &&
          typeof window.TawalaTransfer !== "undefined" &&
          typeof window.TawalaTransfer.bumpLibraryTimesUsed === "function"
        ) {
          window.TawalaTransfer.bumpLibraryTimesUsed(libraryId);
        }
        void this.openTestDrive(href, { purge });
      },
      true
    );
  },
  /** Inline stars after the project name (no separate Rating column). Unrated → omit. */
  starsHtml(rating) {
    const n = Number(rating) || 0;
    if (n <= 0) return "";
    let html = '<span class="rating-stars" aria-label="' + n + ' of 5 stars">';
    for (let i = 1; i <= 5; i++) {
      html += '<span class="star' + (i <= n ? " on" : "") + '">★</span>';
    }
    return html + "</span>";
  },
  /**
   * Start-point link HTML.
   * @param {{label?:string,url?:string|null}} sp
   * @param {{purge?:boolean}} opts — Default purge true (listing / primary Test Drive).
   *   Project Details (Library and My Tawala) pass purge:false so Admin/Setup writes
   *   (questions / config) survive when opening Exam / Registration next.
   */
  startPointHtml(sp, opts) {
    const options = opts || {};
    const purge = options.purge !== false;
    const label = String(sp.label || sp.form || "Start");
    const escLabel = label.replace(/&/g, "&amp;").replace(/</g, "&lt;");
    if (sp.url) {
      const safeUrl = String(sp.url).replace(/"/g, "&quot;");
      if (purge) {
        return (
          '<a class="js-testdrive" href="' +
          sp.url +
          '" data-testdrive-url="' +
          safeUrl +
          '" target="_blank" rel="noopener" title="Purges prior responses for this project, then opens :8080">' +
          escLabel +
          "</a>"
        );
      }
      // Project Details / operate: full token URL as-is, no purge (use PURGE / primary Test Drive to clear).
      return (
        '<a href="' +
        sp.url +
        '" target="_blank" rel="noopener" title="Open this start form on :8080 (keeps project data; use PURGE or primary Test Drive to clear)">' +
        escLabel +
        "</a>"
      );
    }
    return (
      '<span class="start-point-pending" title="No local :8080 URL yet (dev)">' +
      escLabel +
      "</span>"
    );
  },
};

/** Bind once when the catalog script loads (Library / home / My Tawala pages). */
if (typeof document !== "undefined") {
  if (document.readyState === "loading") {
    document.addEventListener("DOMContentLoaded", () => window.TawalaDemo.bindTestDriveClicks(document));
  } else {
    window.TawalaDemo.bindTestDriveClicks(document);
  }
}
