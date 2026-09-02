/**
 * Library vs My Tawala catalogs + local :8080 start URLs.
 *
 * Source piles (JSON backups):
 *   website-mock/projects/library/  ← ~/Projects/Tawala Projects/WebLibrary
 *   website-mock/projects/mytawala/ ← ~/Projects/Tawala Projects/MyTawala
 *
 * Public Library holds vetted liveReady try-outs only (Aug 10, 2026 cleanup). New Project
 * starters live under designer-web/public/samples/templates/ (catalog.ts). Sign-up Sheet and
 * Sign-up Sheet w Email are Designer New Project only until a good Push is re-Published.
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
    "name": "Simple Survey",
    "category": "Polls and Surveys",
    "featured": true,
    "iconLabel": "SS",
    "rating": 4,
    "comments": 12,
    "updated": "8/28/26",
    "shortDescription": "A one-question survey with a thank-you page and instant results report.",
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
    "name": "Simple Potluck",
    "category": "Meetings and Gatherings",
    "featured": true,
    "iconLabel": "PL",
    "rating": 4,
    "comments": 15,
    "updated": "8/28/26",
    "shortDescription": "Headcount, regrets, who brings what",
    "longDescription": "Invite guests to a potluck, collect RSVPs and what each person will bring. Includes skip logic for non-attendees and captions for Adults/Kids. Uses Potluck Organizer (start), Report, documents, and processes for thanks and delete.",
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
    "name": "Simple Get Together",
    "category": "Meetings and Gatherings",
    "featured": true,
    "iconLabel": "GT",
    "rating": 4,
    "comments": 11,
    "updated": "8/28/26",
    "shortDescription": "Basic scheduling, with table of responses, top choice",
    "longDescription": "Two MCQs: which dates work (multi-select) and top preference (single). Green Line theme for high-contrast title readability. Report includes a question-correlation table to see the best overlap.",
    "jsonFile": "designer-web/public/samples/templates/get-together.json",
    "themePath": "greenline",
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
    "shortDescription": "Fun quiz from two young animal lovers - who's right?",
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
    "name": "Simple Multiple Question Survey",
    "category": "Polls and Surveys",
    "featured": false,
    "iconLabel": "MQ",
    "rating": 4,
    "comments": 9,
    "updated": "8/28/26",
    "shortDescription": "Multi-question poll with bar-graph tallies and a response table on Report.",
    "longDescription": "Owner-vetted Polls and Surveys try-out. Survey collects name, several multiple-choice questions, and optional results link. Report shows choice-tally tables per MCQ plus an itemization table of all responses.",
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
    "updated": "8/28/26",
    "shortDescription": "Build and administer an online exam — questions, scoring, and examinee results.",
    "longDescription": "Owner-vetted Polls and Surveys Live app. Decoupled QuestionId and SequenceNumber with robust add/delete/re-sequence question management and 100% accurate score attribution.",
    "jsonFile": "projects/library/Online Exam Builder.json",
    "formNames": ["Question", "SetupVariables", "Exam", "Administration", "Setup", "CustomizationPreview", "Answer"],
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

/**
 * Task #14 honesty (Aug 24, 2026). Product contract is wipe-on-leave; this static mock
 * can only purge-on-start. Closing the :8080 tab is invisible to :5500. Copy link shares
 * the Library uniqueId. Single source for Library / home / picker copy — do not invent
 * leave-detection here. Rewire notes: website-mock/README.md § Test Drive leave/wipe.
 */
window.TAWALA_TEST_DRIVE_HONESTY = {
  tooltipSingle:
    "No account. Clears this Library demo when you start (not when you close the tab), then opens :8080.",
  tooltipMulti:
    "Choose a start. Clears this Library demo when you start (not when you close the tab). All starts stay usable during the drive.",
  copyTooltipSingle:
    "Copy the live try-out URL (no account). Same shared Library demo as Test Drive — not a private copy.",
  copyTooltipMulti:
    "Choose a start, then copy its try-out URL. Same shared Library demo as Test Drive — not a private copy.",
  copyAlert:
    "Link copied.\n\nThis is the shared Library demo URL (same uniqueId for every visitor). Answers clear when someone starts Test Drive from the Library, not when they close the tab.",
  copyPromptLabel: "Copy this Test Drive link (shared Library demo — not a private copy):",
  pickerOpenLede:
    "“{name}” has more than one start form. Click a name to open it. Demo answers clear when you start (not when you close the tab). Use every start during this drive.",
  pickerCopyLede:
    "“{name}” has more than one start form. Click a name to copy its Test Drive URL. Same shared Library demo — not a private copy.",
  pickerOpenLinkTitle: "Open this start. Clears demo answers on start, not when you close the tab.",
  pickerCopyLinkTitle: "Copy this start’s URL (shared Library demo, not a private copy).",
  pickerHint: "No account needed. Closing the Test Drive tab does not wipe answers.",
  startLinkTitle:
    "Clears this Library demo when you start, then opens :8080. Closing the tab does not wipe.",
  homeNote:
    "Clears this shared Library demo when you start (not when you close the tab). Needs :8080.",
  listingPile:
    "Browse templates · Test drive (shared demo — not saved) · Copy link · Copy to MyTawala when logged in",
  listingHint:
    "Test Drive opens a shared demo. Simple templates clear when someone starts a new drive. Exam apps (Online Exam Builder): try Administration / Setup — nothing you enter is saved; Copy to MyTawala (free account) to keep and run exams for real.",
  /* Exam / data-driven Library rows — try-before-register (owner Sep 1, 2026). */
  tooltipSingleExam:
    "No account. Try the teacher flow on :8080 — nothing you enter is saved. Copy to MyTawala (free) to keep and run exams.",
  tooltipMultiExam:
    "Choose a start — try Administration or Setup first. No account; nothing you enter is saved. Copy to MyTawala (free) to use it for real.",
  copyTooltipSingleExam:
    "Copy the shared teacher demo URL (no account). Nothing is saved — Copy to MyTawala (free) for a private copy.",
  copyTooltipMultiExam:
    "Choose a start, then copy its demo URL. Shared try-out — nothing saved. Copy to MyTawala (free) for real use.",
  copyAlertExam:
    "Link copied.\n\nShared teacher demo — nothing you enter is saved to an account.\n\nCopy to MyTawala (free) to keep your exam and run it for real.",
  copyPromptLabelExam: "Copy this shared demo link (not saved — Copy to MyTawala for a private copy):",
  pickerOpenLedeExam:
    "“{name}” has more than one start. Teachers: try Administration or Setup first. No account — nothing you enter is saved.",
  pickerCopyLedeExam:
    "“{name}” has more than one start. Copy a demo link — shared try-out, nothing saved. Copy to MyTawala (free) for real use.",
  pickerOpenLinkTitleExam: "Open this start. Shared demo — your entries are not saved.",
  pickerCopyLinkTitleExam: "Copy this start’s demo URL (not saved — Copy to MyTawala for real use).",
  startLinkTitleExam:
    "Opens the shared teacher demo on :8080. Nothing you enter is saved — Copy to MyTawala (free) to keep exams.",
  rowDemoBadge: "Demo - your exam not saved.",
  rowDemoBadgeTitle:
    "Shared teacher try-out on :8080 — nothing you enter is saved. Copy to MyTawala (free account) to keep and run exams for real.",
  /* keepResponses listings (Publish Purge unchecked) — do not claim answers clear on start. */
  tooltipSingleKeep:
    "No account. Opens this published app. Stored answers stay — Test Drive does not clear them.",
  tooltipMultiKeep:
    "Choose a start. Stored answers stay — Test Drive does not clear them.",
  copyTooltipSingleKeep:
    "Copy the live URL (no account). Same published app as Test Drive — answers are not cleared on start.",
  copyTooltipMultiKeep:
    "Choose a start, then copy its URL. Same published app as Test Drive — answers are not cleared on start.",
  copyAlertKeep:
    "Link copied.\n\nThis is the shared Library URL for this published app. Answers are not cleared when someone starts Test Drive.",
  copyPromptLabelKeep:
    "Copy this Test Drive link (shared Library URL — stored answers stay):",
  pickerOpenLedeKeep:
    "“{name}” has more than one start form. Click a name to open it. Stored answers stay (Test Drive does not clear them).",
  pickerOpenLinkTitleKeep: "Open this start. Does not clear stored answers.",
  startLinkTitleKeep: "Opens :8080. This listing does not clear answers on start.",
};

window.TAWALA_DATA_DRIVEN_TEST_DRIVE_TITLE =
  "No account. Try the teacher setup flow — nothing you enter is saved. Copy to MyTawala (free) to use it for real.";

/** @deprecated Use TAWALA_DATA_DRIVEN_TEST_DRIVE_TITLE — kept for older cached scripts. */
window.TAWALA_DATA_DRIVEN_NO_TEST_DRIVE_TITLE = window.TAWALA_DATA_DRIVEN_TEST_DRIVE_TITLE;

/** Short-lived :8080 probe cache — shared by Library Test Drive and My Tawala Use. */
let _runtimeProbeCache = { at: 0, ok: null };
const _RUNTIME_PROBE_TTL_MS = 4000;

window.TawalaDemo = {
  TEST_DRIVE_HONESTY: window.TAWALA_TEST_DRIVE_HONESTY,
  DATA_DRIVEN_TEST_DRIVE_TITLE: window.TAWALA_DATA_DRIVEN_TEST_DRIVE_TITLE,
  DATA_DRIVEN_NO_TEST_DRIVE_TITLE: window.TAWALA_DATA_DRIVEN_NO_TEST_DRIVE_TITLE,
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
  /**
   * Details URLs often use a Library catalog id (e.g. online-exam-builder). My Tawala
   * copies from Copy to MyTawala have a different overlay id. Resolve one row, or list
   * candidates. Never invent a fake seed row.
   */
  resolveMyTawalaDetailsTarget(queryId) {
    const id = String(queryId || "").trim();
    if (!id) return { status: "missing", id: "", candidates: [] };
    if (this.getMyTawala(id)) return { status: "exact", id, candidates: [] };
    const entries = typeof this.myTawalaEntries === "function" ? this.myTawalaEntries() : [];
    const fromLib = entries.filter(
      (e) =>
        e &&
        (String(e.pulledFromLibraryId || "") === id ||
          String(e.publishedToLibraryId || "") === id)
    );
    if (fromLib.length === 1) {
      return { status: "acquire", id: fromLib[0].id, candidates: fromLib };
    }
    if (fromLib.length > 1) {
      return { status: "ambiguous", id: "", candidates: fromLib };
    }
    const lib = this.getLibrary(id);
    const wantName =
      lib && typeof this.displayName === "function"
        ? String(this.displayName(lib.name) || "").trim().toLowerCase()
        : "";
    if (wantName) {
      const byName = entries.filter((e) => {
        const n =
          typeof this.displayName === "function"
            ? String(this.displayName(e.name) || "").trim().toLowerCase()
            : String(e.name || "").trim().toLowerCase();
        return n === wantName;
      });
      if (byName.length === 1) {
        return { status: "name", id: byName[0].id, candidates: byName };
      }
      if (byName.length > 1) {
        return { status: "ambiguous", id: "", candidates: byName };
      }
    }
    return {
      status: "missing",
      id: "",
      candidates: [],
      libraryId: lib ? id : "",
      libraryName: lib && this.displayName ? this.displayName(lib.name) : "",
    };
  },
  /** Prefer Library, then My Tawala (detail pages that accept either id). */
  get(id) {
    return this.getLibrary(id) || this.getMyTawala(id) || null;
  },
  getLibrary(id) {
    if (!id) return null;
    const hasTransfer = typeof window !== "undefined" && window.TawalaTransfer;
    // Retired ids stay off the public Library (listing + detail). Leftover overlay is
    // skipped — do not resurrect a retired Publish (e.g. House Test). A later Publish
    // calls clearLibraryRetired so this branch no longer matches.
    if (hasTransfer && typeof window.TawalaTransfer.isLibraryRetired === "function" && window.TawalaTransfer.isLibraryRetired(id)) {
      return null;
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
  /** Deploy-field default only — never a Project Data tree title, never persist over a form name. */
  DEFAULT_SHARE_LABEL: "Click here.",
  /** Designer form name for a start point (stable key; not the owner share label). */
  startFormKey(sp) {
    if (!sp) return "";
    const form = String(sp.form || "").trim();
    if (form && form !== this.DEFAULT_SHARE_LABEL) return form;
    const lab = String(sp.label || "").trim();
    if (lab && lab !== this.DEFAULT_SHARE_LABEL) return lab;
    return form;
  },
  /**
   * True when overlay `label` is an owner-chosen share nickname (Deploy / Invite / Include).
   * Empty, the Designer form name, and the dialog default “Click here.” are unnamed.
   */
  isCustomShareLabel(label, formName) {
    const lab = String(label || "").trim();
    const form = String(formName || "").trim();
    if (!lab) return false;
    if (lab === this.DEFAULT_SHARE_LABEL) return false;
    if (form && lab.toLowerCase() === form.toLowerCase()) return false;
    return true;
  },
  /** Owner-chosen share nickname, else the Designer form name (not “Click here.”). */
  startShareLabel(sp) {
    if (!sp) return "Start";
    const form = this.startFormKey(sp);
    const lab = String(sp.label || "").trim();
    if (this.isCustomShareLabel(lab, form)) return lab;
    return form || "Start";
  },
  /**
   * Pin `/p/{uniqueId}/…` on a live form URL (Task #10).
   * Shape: `http://localhost:8080/p/{uniqueId}/{formToken}.FormName` (Java)
   * or `/p/{uniqueId}/{formName}` (dev-session on :3001). Does not invent form tokens.
   * URLs without a `/p/{id}` segment are left unchanged.
   */
  rewriteRuntimeUrlUniqueId(url, uniqueId) {
    if (!url) return url || "";
    if (!this.isValidUniqueId(uniqueId)) return String(url);
    const s = String(url);
    if (!/\/p\/[A-Za-z0-9]{1,20}(?=\/|$|\?|#)/.test(s)) return s;
    return s.replace(/\/p\/[A-Za-z0-9]{1,20}(?=\/|$|\?|#)/, `/p/${uniqueId}`);
  },
  /** Live start URL for Use / Deploy copy — this overlay’s uniqueId, not display name. */
  liveStartUrl(project, sp) {
    const url = (sp && sp.url) || "";
    if (!url) return "";
    const uid = this.uniqueIdForProject(project);
    return this.rewriteRuntimeUrlUniqueId(url, uid) || url;
  },
  /**
   * Incoming Push/Deploy starts keep owner share labels from the overlay.
   * Pins uniqueId on URLs. Does not invent Tomcat form tokens.
   */
  mergeStartPointsPreservingLabels(incoming, previous, uniqueId) {
    const incomingList = Array.isArray(incoming) ? incoming : [];
    const prevList = Array.isArray(previous) ? previous : [];
    const prevByKey = {};
    for (let i = 0; i < prevList.length; i++) {
      const key = this.startFormKey(prevList[i]).toLowerCase();
      if (key && !prevByKey[key]) prevByKey[key] = prevList[i];
    }
    const out = [];
    for (let i = 0; i < incomingList.length; i++) {
      const sp = incomingList[i];
      const form = this.startFormKey(sp);
      if (!form) continue;
      const prev = prevByKey[form.toLowerCase()];
      const prevLab = prev && prev.label;
      const incomingLab = sp && sp.label;
      const label = this.isCustomShareLabel(prevLab, form)
        ? String(prevLab).trim()
        : this.isCustomShareLabel(incomingLab, form)
          ? String(incomingLab).trim()
          : form;
      const rawUrl = (sp && sp.url) || null;
      const url = rawUrl ? this.rewriteRuntimeUrlUniqueId(rawUrl, uniqueId) || rawUrl : null;
      out.push({ form, label, url });
    }
    return out;
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
  formNameOfExport(form) {
    if (!form) return "";
    return String(form.form || form.name || "").trim();
  },
  /**
   * All named form payloads from an export — copy onto a new uniqueId.
   * Do not filter by form name (Question / SetupVariables / Exam all go).
   */
  formsFromExport(forms) {
    const list = Array.isArray(forms) ? forms : [];
    return list.filter((f) => f && this.formNameOfExport(f));
  },
  /**
   * Data-driven / derivative Library rows (Exam Builder, Mongolia-style published exams).
   * Library Test Drive is allowed with honest “nothing saved” copy (owner Sep 1, 2026).
   * Full operate path remains Copy to MyTawala → My Tawala Use.
   */
  isDataDrivenProject(project) {
    if (!project || typeof project !== "object") return false;
    const names = [];
    const add = (n) => {
      const s = String(n || "").trim().toLowerCase();
      if (s) names.push(s);
    };
    (Array.isArray(project.formNames) ? project.formNames : []).forEach(add);
    (Array.isArray(project.startPoints) ? project.startPoints : []).forEach((sp) => {
      add(sp && (sp.form || sp.label));
    });
    if (names.indexOf("question") !== -1 && names.indexOf("setupvariables") !== -1) return true;
    const id = String(project.id || "").trim().toLowerCase();
    if (id === "online-exam-builder" || id.indexOf("online-exam") !== -1) return true;
    const json = String(project.jsonFile || "").replace(/\\/g, "/").toLowerCase();
    if (json.indexOf("online exam builder") !== -1) return true;
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
    return !!(hasExam && hasAdmin);
  },
  libraryProjectForUniqueId(uniqueId) {
    if (!this.isValidUniqueId(uniqueId)) return null;
    const uid = String(uniqueId);
    const libs = typeof this.libraryEntries === "function" ? this.libraryEntries() : [];
    for (let i = 0; i < libs.length; i++) {
      if (this.uniqueIdForProject(libs[i]) === uid) return libs[i];
    }
    return null;
  },
  uniqueIdIsLibraryDataDriven(uniqueId) {
    const row = this.libraryProjectForUniqueId(uniqueId);
    return !!(row && this.isDataDrivenProject(row));
  },
  dataDrivenTestDriveTitle() {
    return (
      this.DATA_DRIVEN_TEST_DRIVE_TITLE ||
      window.TAWALA_DATA_DRIVEN_TEST_DRIVE_TITLE ||
      "No account. Try the teacher setup flow — nothing you enter is saved. Copy to MyTawala (free) to use it for real."
    );
  },
  /** @deprecated */
  dataDrivenNoTestDriveTitle() {
    return this.dataDrivenTestDriveTitle();
  },
  _rowKeepResponsesFlag(p) {
    return !!(p && (p.keepResponses === true || p.publishedKeepResponses === true));
  },
  /**
   * True when this uniqueId was Published with Purge unchecked. Checks Library overlays
   * (including inactive) and any My Tawala row stamped publishedKeepResponses.
   */
  uniqueIdKeepsResponses(uniqueId) {
    if (!this.isValidUniqueId(uniqueId)) return false;
    const uid = String(uniqueId);
    const match = (p) => this.uniqueIdForProject(p) === uid && this._rowKeepResponsesFlag(p);
    const libs = typeof this.libraryEntries === "function" ? this.libraryEntries() : [];
    for (let i = 0; i < libs.length; i++) {
      if (match(libs[i])) return true;
    }
    if (typeof window !== "undefined" && window.TawalaTransfer) {
      if (typeof window.TawalaTransfer.getLibraryOverlay === "function") {
        const overlay = window.TawalaTransfer.getLibraryOverlay() || {};
        const ids = Object.keys(overlay);
        for (let i = 0; i < ids.length; i++) {
          const row = overlay[ids[i]];
          if (match({ ...row, id: ids[i] })) return true;
        }
      }
    }
    const mine = typeof this.myTawalaEntries === "function" ? this.myTawalaEntries() : [];
    for (let i = 0; i < mine.length; i++) {
      if (match(mine[i])) return true;
    }
    return false;
  },
  projectKeepsResponses(project) {
    if (!project) return false;
    if (this._rowKeepResponsesFlag(project)) return true;
    const id = this.uniqueIdForProject(project);
    return id ? this.uniqueIdKeepsResponses(id) : false;
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

  /**
   * Task #27 slice 2: free the live Tomcat/Node Deploy *name* for this uniqueId.
   * Does not mint a uniqueId. Lookup is uniqueId-only (Online Exam catalog id stays).
   */
  async vacateLiveTomcatName(uniqueId) {
    if (!this.isValidUniqueId(uniqueId)) {
      return { status: "failure", error: "uniqueId required (1–20 alphanumeric)" };
    }
    const url = this.purgeApiBase().replace(/\/$/, "") + "/api/retire-name";
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
        return {
          status: "failure",
          uniqueId,
          code: data.code || (res.status === 501 ? "command.unknown" : "retire-failed"),
          error:
            data.error ||
            (res.status === 501
              ? "Tomcat has no retireDeployment yet — rebuild ROOT.war so Retire can free :8080 names."
              : `HTTP ${res.status}`),
        };
      }
      return data;
    } catch (e) {
      return {
        status: "failure",
        uniqueId,
        code: "retire-unreachable",
        error:
          "Couldn't reach designer-web :3001 to free that live name. " +
          "Retire aborted so occupancy is not lying. Is the API up? (cd designer-web && npm run dev)",
      };
    }
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
   * Read a catalog Designer JSON for Copy to MyTawala clone-on-acquire (Task #26).
   * Path-restricted by :3001 `/api/open-mock-json`.
   */
  async fetchCatalogProject(relPath) {
    const rel = String(relPath || "").trim().replace(/\\/g, "/");
    if (!rel) {
      return { status: "failure", error: "path required" };
    }
    const url =
      this.purgeApiBase().replace(/\/$/, "") +
      "/api/open-mock-json?path=" +
      encodeURIComponent(rel);
    try {
      const res = await fetch(url);
      const data = await res.json().catch(() => ({}));
      if (!res.ok || !data.project) {
        return {
          status: "failure",
          error: data.error || `HTTP ${res.status}`,
        };
      }
      return { status: "success", project: data.project, path: data.path || rel };
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
   * Redeploy a project definition to :8080 / Node runtime (same /api/deploy as Designer).
   * Java keeps the same uniqueId when the project name matches an existing deployment.
   * Pass `_freshFromTemplate: true` to mint a new Tomcat name / uniqueId (clone-on-acquire).
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
   * After a private clone (new uniqueId): copy **all** submissions from the source
   * uniqueId onto the destination. No form-name filter — Exam / trial names copy too.
   * Owner Purges for a clean slate. Refuses same-uniqueId (that was the scrambled
   * partial-purge path). Fail-open: callers keep the live clone if this returns failure.
   */
  async copyResponsesToUniqueId(sourceUniqueId, destUniqueId) {
    if (!this.isValidUniqueId(sourceUniqueId) || !this.isValidUniqueId(destUniqueId)) {
      return { status: "failure", error: "source and destination uniqueId required" };
    }
    if (String(sourceUniqueId) === String(destUniqueId)) {
      return {
        status: "failure",
        error: "refused: response copy must use a new uniqueId (not a same-id purge)",
        sourceUniqueId,
        uniqueId: destUniqueId,
      };
    }
    const current = await this.exportResponses(sourceUniqueId);
    if (current.status !== "success") {
      return {
        status: "failure",
        uniqueId: destUniqueId,
        sourceUniqueId,
        error: (current && current.error) || "export failed — response data was not copied",
      };
    }
    const forms = this.formsFromExport(current.forms);
    const copied = forms.reduce((n, f) => n + ((f.rows && f.rows.length) || 0), 0);
    if (!copied) {
      return {
        status: "skipped",
        reason: "no-rows",
        uniqueId: destUniqueId,
        sourceUniqueId,
        copied: 0,
      };
    }
    const result = await this.importResponses(destUniqueId, forms, {
      source: current.source,
      mode: "replace",
    });
    if (result.status !== "success") {
      return { ...result, uniqueId: destUniqueId, sourceUniqueId };
    }
    return {
      ...result,
      uniqueId: destUniqueId,
      sourceUniqueId,
      copied,
      copiedForms: forms.map((f) => this.formNameOfExport(f)),
    };
  },
  /**
   * Optionally purge :8080 submissions for a uniqueId (Test Drive / maintainer).
   * Publish no longer calls this on the author’s My Tawala uniqueId — it mints a
   * separate empty Library uniqueId instead.
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
   * Optionally purge then open the :8080 form. Product contract (#14) is wipe-on-leave; this
   * mock wipes on start because a static :5500 page cannot see the :8080 tab close.
   * Honesty copy: TEST_DRIVE_HONESTY (keepResponses listings must not claim a wipe).
   * keepResponses uniqueIds (Publish Purge unchecked) skip purge entirely — no half-wipe.
   * Data-driven Library rows (Exam Builder) use exam honesty copy; purge-on-start still applies
   * to the shared demo uniqueId (visitor work is not saved — see TEST_DRIVE_HONESTY *Exam keys).
   * Other Test Drive purges use whole-uniqueId purgeResponses.
   * Opens a blank tab synchronously (keeps the user gesture for popup blockers),
   * probes Tomcat before navigating (same offline gate as My Tawala Use), then
   * navigates after purge. Failed / timed-out purge never blocks opening when :8080 is up.
   * If :8080 is down: close the blank tab, alert, stay on :5500 — never “site can’t be reached.”
   * Post-tab-close purge is not available in this static mock — do not fake it.
   */
  async openTestDrive(url, opts) {
    const options = opts || {};
    const purgeFirst = options.purge !== false;
    const purgeMs = typeof options.purgeTimeoutMs === "number" ? options.purgeTimeoutMs : 12000;
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
            "Copy to MyTawala still works offline."
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
        if (id && this.uniqueIdKeepsResponses(id)) {
          purge = { status: "skipped", reason: "keepResponses", uniqueId: id };
        } else if (id) {
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
    const labelOf = (s) => this.startFormKey(s) || String(s.label || s.form || "").trim();
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
    const labelOf = (s) => this.startFormKey(s) || String(s.label || s.form || "").trim();
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
          /* Library catalog Times used (popularity) — not My Tawala usageStats. */
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
        const uid = this.uniqueIdFromUrl(sp.url);
        const keep = uid && typeof this.uniqueIdKeepsResponses === "function" && this.uniqueIdKeepsResponses(uid);
        const honestyTitle = keep
          ? (this.TEST_DRIVE_HONESTY && this.TEST_DRIVE_HONESTY.startLinkTitleKeep) ||
            "Opens :8080. This listing does not clear answers on start."
          : (this.TEST_DRIVE_HONESTY && this.TEST_DRIVE_HONESTY.startLinkTitle) ||
            "Clears this Library demo when you start, then opens :8080. Closing the tab does not wipe.";
        return (
          '<a class="js-testdrive" href="' +
          sp.url +
          '" data-testdrive-url="' +
          safeUrl +
          '" target="_blank" rel="noopener" title="' +
          honestyTitle.replace(/"/g, "&quot;") +
          '">' +
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
