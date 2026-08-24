/**
 * Legacy My Tawala / Project Manager operation labels recovered from
 * TawalaWebapp-build1700 (projectmanager/*.jsp, blocks/block-projectManagerProjectDetails.jsp,
 * submenus/submenu-mytawala.jsp, confirmationdialogs.jsp).
 *
 * Layout-first mock: listing + Project Details pages stay lean; full catalog lives on
 * project-ops-review.html for memory/archive review.
 *
 * wired: false → disabled control (grey only; no “not wired” label)
 * wired: "purge-local" → confirm + POST /api/purge-responses (uniqueId via resolvePurgeUniqueId)
 * wired: "delete-mytawala" → confirm + remove private My Tawala row (overlay/inbox; not Library)
 *         (falls back to CLI hint if API/Postgres unavailable)
 * wired: "export-mytawala" → TawalaDataOps.handleExportClick (js/data-ops.js) — Excel response
 *         data only, no confirm (non-destructive; downloads a JSON file)
 * wired: "import-mytawala" → TawalaDataOps.handleImportClick — field-mismatch check, then
 *         confirm + POST /api/import-responses (replace mode)
 * wired: "backup-mytawala" → TawalaDataOps.handleBackupClick — no confirm (non-destructive;
 *         downloads a JSON bundle: definition + data + properties, see README § Backup/Restore)
 * wired: "restore-mytawala" → TawalaDataOps.handleRestoreClick — confirm + overlay properties +
 *         POST /api/import-responses (replace mode)
 * wired: "get-from-library" → openGetFromLibraryDialog (Library picker → Save a copy rename;
 *         not a nav duplicate of Library; always enabled on My Tawala listing bar)
 * wired: "pull-library" → openPullDialog (Refresh from Library) — selection + Library link required;
 *         TawalaTransfer.pullFromLibrary refreshes content only (name/deploy/data preserved)
 * wired: "download-version" → download minimal JSON for the selected Versions row
 *         (metadata only — not a full Designer definition restore)
 * wired: "deploy-version" → Deploy-this-version (switch live :8080 definition to selected
 *         row’s snapshot; enabled only for non-current rows with a saved definition)
 * Versions description cells → TawalaTransfer.updateVersionDescription (inline edit; number/current immutable)
 */
(function () {
  /** My Tawala top sub-menu (submenu-mytawala.jsp) */
  const MYTAWALA_SUBMENU = [
    { label: "My Projects", title: "Manage your projects", wired: true, href: "mytawala.html" },
    { label: "My Account", title: "Edit your accounts settings", wired: false },
    { label: "Change Password", title: "Change your password", wired: false },
  ];

  /**
   * Library top sub-menu (submenu-library.jsp).
   * Search is the listing itself; Edit Categories / Recent Changes are editor/admin surfaces.
   */
  const LIBRARY_SUBMENU = [
    {
      label: "Search",
      title: "Search the library for projects",
      wired: true,
      href: "library.html",
    },
    {
      label: "Edit Categories",
      title: "Assign Library projects to category groups",
      wired: "edit-categories",
      id: "edit-categories",
    },
    {
      label: "Recent Changes",
      title: "View recent changes",
      wired: false,
    },
  ];

  /** Compact Library chrome — expand/collapse wired; admin manageLibrary.jsp stubs remain grey. */
  const LIBRARY_CATEGORY_STUBS = [
    { id: "expand-all", label: "Expand all", title: "Expand all categories", wired: true },
    { id: "collapse-all", label: "Collapse all", title: "Collapse all categories", wired: true },
  ];

  const LIBRARY_ADMIN_STUBS = [
    {
      id: "edit-categories",
      label: "EDIT CATEGORIES",
      title: "Assign Library projects to category groups",
      wired: "edit-categories",
    },
    {
      id: "reindex",
      label: "Reindex Library",
      title: "Rebuild library search index (admin)",
      wired: false,
    },
    {
      id: "reset-counts",
      label: "Reset Category Counts",
      title: "Reset category project counts (admin)",
      wired: false,
    },
  ];

  function testDriveHonesty() {
    if (typeof TawalaDemo !== "undefined" && TawalaDemo.TEST_DRIVE_HONESTY) {
      return TawalaDemo.TEST_DRIVE_HONESTY;
    }
    if (typeof window !== "undefined" && window.TAWALA_TEST_DRIVE_HONESTY) {
      return window.TAWALA_TEST_DRIVE_HONESTY;
    }
    return {};
  }

  function honestyText(key, fallback) {
    const v = testDriveHonesty()[key];
    return typeof v === "string" && v ? v : fallback;
  }

  function honestyNamed(key, name, fallback) {
    return honestyText(key, fallback).replace(/\{name\}/g, name);
  }

  /**
   * Public Library listing row actions — discovery / acquire only (owner Aug 1 / Aug 9 / Aug 10).
   * Single “Actions” column: icons + metrics beside Test drive / Copy link; Save to MyTawala
   * only when mock-logged-in. **Use** lives on My Tawala (operate), not Library.
   * wired: "test-drive" → active when project has a live :8080 URL; else disabled grey.
   *   Single-start opens directly; 2+ starts open a hot-link list (each start opens Test Drive).
   *   Listing shows **Times used** metric under the Test Drive control.
   * wired: "copy-testdrive-link" → copies the same :8080 URL Test Drive opens (viral share; no account).
   *   Listing shows **Copies downloaded** under Copy link.
   * wired: "save-copy-library" → rename dialog → TawalaTransfer.saveCopyFromLibrary (logged-in only).
   * ≠ My Tawala Deploy / Project Data “Copy link” (owner distributing *their* live starts).
   * Task #14: tooltips tell the mock truth (purge-on-start, shared uniqueId) — see TEST_DRIVE_HONESTY.
   */
  const LIBRARY_LISTING_ACTIONS = [
    {
      id: "test-drive",
      label: "Test drive",
      title: honestyText(
        "tooltipSingle",
        "No account. Clears this Library demo when you start (not when you close the tab), then opens :8080."
      ),
      wired: "test-drive",
      icon: "testdrive",
      metric: "timesUsed",
    },
    {
      id: "copy-testdrive-link",
      label: "Copy link",
      title: honestyText(
        "copyTooltipSingle",
        "Copy the live try-out URL (no account). Same shared Library demo as Test Drive — not a private copy."
      ),
      wired: "copy-testdrive-link",
      icon: "link",
      metric: "cloneCount",
    },
    {
      id: "save-my-tawala",
      label: "Save to MyTawala",
      title: "Save to MyTawala (rename on the way in) — requires login",
      wired: "save-copy-library",
      icon: "save",
      requiresLogin: true,
    },
  ];

  function isMockLoggedIn() {
    return (
      typeof window.TawalaChrome !== "undefined" &&
      typeof window.TawalaChrome.isLoggedIn === "function" &&
      window.TawalaChrome.isLoggedIn()
    );
  }

  function visibleLibraryListingActions() {
    return LIBRARY_LISTING_ACTIONS.filter((op) => !op.requiresLogin || isMockLoggedIn());
  }

  function noteLibraryTestDriveOpen(projectId) {
    if (
      !projectId ||
      typeof TawalaTransfer === "undefined" ||
      typeof TawalaTransfer.bumpLibraryTimesUsed !== "function"
    ) {
      return;
    }
    TawalaTransfer.bumpLibraryTimesUsed(projectId);
  }

  /**
   * Project Details main action bar (detail.jsp Project Actions).
   *
   * Aug 9, 2026 (Project Data banner): Use / Copy link / Export / Import / Purge live on
   * the Project Data line (selection-scoped). This bar keeps Rename / Make a Copy / Backup /
   * Restore / Deploy (share) / Publish. Pull + Delete stay on the My Tawala listing selection bar.
   * Title double-click also opens Rename (no separate Rename control under the title).
   * Description under the title: double-click only (no hint / no button) → edit shortDescription.
   *
   * Owner enablement (Aug 9; Deploy Aug 10; Make a Copy Aug 10):
   *   A) Forms collapsed (expand 0) OR project root highlighted → Backup/Restore/Publish on;
   *      Use/Copy off; Export/Import/Purge on (project scope). Rename + Make a Copy + Deploy always on.
   *   B) Starting-point form highlighted → Use/Copy/Export/Import/Purge/Publish on; Backup/Restore off.
   *   C) Non-start form highlighted → Export/Import/Purge/Publish on; Use/Copy/Backup/Restore off.
   * Deploy (website share/embed) stays available on A/B/C — dialog explains when no live URL yet.
   * Rename = same project, new display name. Make a Copy = new project identity (own fork).
   */
  const PROJECT_ACTIONS = [
    {
      id: "rename",
      label: "RENAME",
      title: "Rename this project in My Tawala (or double-click the title)",
      wired: "rename-project",
    },
    {
      id: "make-copy",
      label: "MAKE A COPY",
      title:
        "Fork this project into a new My Tawala identity (new name; original untouched). Not the same as Rename or Library Save a copy.",
      wired: "make-copy-mytawala",
    },
    { id: "backup", label: "BACKUP", title: "Back up this project (definition + data + properties)", wired: "backup-mytawala" },
    { id: "restore", label: "RESTORE", title: "Restore this project from a backup", wired: "restore-mytawala" },
    {
      id: "deploy",
      label: "DEPLOY",
      title:
        "Go live for participants — copy a form/start link or embed it in a web page (not Publish; not Designer Push)",
      wired: "deploy-share",
    },
    {
      id: "publish",
      label: "PUBLISH",
      title: "Publish this project to the public Library (rename, then optionally replace a stub or outdated Library entry)",
      wired: "publish-mytawala",
    },
  ];

  /** Project Data banner controls — selection-scoped per owner enablement A/B/C above. */
  const PROJECT_DATA_CONTROLS = [
    {
      id: "use",
      label: "Use",
      title:
        "Select a start point to run on :8080 (needs Java runtime). Offline Purge: select project/form → Purge — not Use.",
      wired: "use-project",
    },
    {
      id: "copy-link",
      label: "Copy link",
      title: "Select a start point to copy its :8080 link (works offline; does not open the form)",
      wired: "copy-start-link",
    },
    {
      id: "export",
      label: "Export",
      title: "Export response data (project or highlighted form)",
      wired: "export-mytawala",
    },
    {
      id: "import",
      label: "Import",
      title: "Import response data (project or highlighted form)",
      wired: "import-mytawala",
    },
    {
      id: "purge",
      label: "Purge",
      title: "Purge response data (project or highlighted form)",
      wired: "purge-local",
      confirmId: "purge",
      destructive: true,
    },
  ];

  /**
   * My Projects listing — top action bar (owner Aug 9 breakpoint + option 3; Make a Copy Aug 10).
   * Get from Library… — always on (opens Library picker → Save a copy acquire).
   * Refresh from Library — selected row + linked Library entry (existing Pull metadata refresh).
   * Make a Copy — selected row (own fork; ≠ Rename; ≠ Library Save a copy).
   * Delete — selected row only.
   * Not a dense per-row icon strip.
   */
  const LISTING_BAR_ACTIONS = [
    {
      id: "get-library",
      label: "GET FROM LIBRARY…",
      title: "Pick a public Library project and Save a copy into My Tawala",
      wired: "get-from-library",
      alwaysEnabled: true,
    },
    {
      id: "refresh-library",
      label: "REFRESH FROM LIBRARY",
      title: "Update the selected project from its public Library listing",
      wired: "pull-library",
      requiresSelection: true,
      requiresLibraryLink: true,
    },
    {
      id: "make-copy",
      label: "MAKE A COPY",
      title:
        "Fork the selected project into a new My Tawala identity (new name; original untouched)",
      wired: "make-copy-mytawala",
      requiresSelection: true,
    },
    {
      id: "delete",
      label: "DELETE",
      title: "Delete the selected project from My Tawala",
      wired: "delete-mytawala",
      confirmId: "delete",
      destructive: true,
      requiresSelection: true,
    },
  ];

  /**
   * My Projects listing — lean columns (Aug 9 Task List item 1; usage cols Aug 10).
   * Only **Use** remains as a row action; Export / Import / Backup / Restore / Purge /
   * Publish live on Project Details. Pull / Delete are on the listing selection bar.
   * Click row → highlight/select; double-click → Project Details (no name link / Details cue).
   * Records + Times used / Last used rendered in mytawala.html (usage via TawalaTransfer.getUsageStats).
   */
  const LISTING_ACTIONS = [
    {
      id: "use",
      label: "Use",
      title:
        "Open / run on :8080 (needs Java). Multi-start → Project Details. Offline Purge uses seeded Records — not Use.",
      wired: "use-project",
      icon: "use",
      group: "info",
    },
  ];

  /** CSS classes for listing op columns (group separators + destructive affordance). */
  function listingColClasses(op) {
    const parts = ["col-op"];
    if (op.group) parts.push(`col-group-${op.group}`);
    if (op.groupStart) parts.push("col-group-start");
    if (op.destructive) parts.push("col-op-destructive");
    return parts.join(" ");
  }

  /** Compact SVG glyphs for listing icon cells (12×12 viewBox). */
  const LISTING_ICONS = {
    export:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M6 1.5v6M3.5 4L6 1.5 8.5 4M2 9.5h8" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    import:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M6 7.5v-6M3.5 5L6 7.5 8.5 5M2 9.5h8" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    backup:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M2.5 3.5h7v7h-7zM4 3.5V2.5h4v1M4.5 6.5h3M4.5 8.5h3" fill="none" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    restore:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M2.5 6a3.5 3.5 0 1 0 1-2.4M2.5 2.5v3h3" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    purge:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M3 4.5h6l-.6 5.5H3.6zM4.5 4.5V3h3v1.5M2.5 4.5h7" fill="none" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    delete:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M3 3.5l6 6M9 3.5l-6 6" fill="none" stroke="currentColor" stroke-width="1.4" stroke-linecap="round"/></svg>',
    publish:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M2 8.5v1.5h8V8.5M6 8V2.5M3.5 4.5L6 2 8.5 4.5" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    pull:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M2 3.5v-1.5h8v1.5M6 4v5.5M3.5 7.5L6 10 8.5 7.5" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    /* Library listing */
    testdrive:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M3.5 2.2v7.6L10 6z" fill="currentColor" stroke="none"/></svg>',
    link:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M5.2 6.8a2.4 2.4 0 0 0 3.4 0l1.2-1.2a2.4 2.4 0 0 0-3.4-3.4L5.8 2.8M6.8 5.2a2.4 2.4 0 0 0-3.4 0L2.2 6.4a2.4 2.4 0 1 0 3.4 3.4L6.2 9.2" fill="none" stroke="currentColor" stroke-width="1.25" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    save:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M6 1.5v6M3.5 5L6 7.5 8.5 5M2.5 10h7" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    /* My Tawala Use — open/run (not CloneAndCustomize) */
    use:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M3.5 2.2v7.6L10 6z" fill="currentColor" stroke="none"/></svg>',
  };

  /**
   * Sidebar Invite / Include — Task #10 doorway (Aug 10): same Deploy share panel as the
   * Details action-bar DEPLOY button (copy link + iframe embed). Not ACL invites.
   * REVISE / ONLINE-OFFLINE live as Edit in Designer / Active on the same rail.
   */
  const PROJECT_SIDEBAR_OPS = [
    {
      label: "Include Project in Web Page",
      title: "Open Deploy — copy an iframe embed for a start URL",
      wired: "deploy-share",
    },
    {
      label: "Invite Other People to This Project",
      title: "Open Deploy — copy a form/start link to email participants",
      wired: "deploy-share",
    },
  ];

  /** Legacy Format → Project Themes (labels/paths from Designer theme-config; mock dropdown only). */
  const PROJECT_THEMES = [
    { label: "Baseball", path: "baseball" },
    { label: "Basic Blue", path: "basicblue" },
    { label: "Basic Green", path: "basicgreen" },
    { label: "Basic Pink", path: "basicpink" },
    { label: "Basic Yellow", path: "basicyellow" },
    { label: "Big Q", path: "style2" },
    { label: "Blue Lined Paper", path: "blueline" },
    { label: "Chocolate", path: "chocolate" },
    { label: "Dark", path: "dark" },
    { label: "Default", path: "default" },
    { label: "Dirtbowl", path: "dirtbowl" },
    { label: "Dirtbowl - Variable Width", path: "dirtbowl2" },
    { label: "Full Moon", path: "fullmoon" },
    { label: "Green Lined Paper", path: "greenline" },
    { label: "Green Tea", path: "greentea" },
    { label: "Light Green", path: "litegreen" },
    { label: "Lime", path: "lime" },
    { label: "MVSC", path: "mvsc" },
    { label: "Orange Swirl", path: "orangeswirl" },
    { label: "Plain", path: "plain" },
    { label: "Purple Haze", path: "purplehaze" },
    { label: "Red", path: "red" },
    { label: "Red Rays", path: "redrays" },
    { label: "Salzburg", path: "salzburg" },
    { label: "Soup's On", path: "soup" },
    { label: "Tennis", path: "tennis" },
    { label: "Tin Car Bell", path: "tincarbell" },
    { label: "Yellow", path: "yellow" },
  ];

  /** Project Data section (detail.jsp form table + filters) */
  const PROJECT_DATA_OPS = [
    { label: "SHOW ALL FORMS", title: "Show All Forms", wired: false },
    { label: "SHOW SELECTED FORMS ONLY", title: "Show Selected Forms Only", wired: false },
    { label: "View data", title: "View data for form", wired: false },
    { label: "View summary", title: "View summary of data for form", wired: false },
    { label: "Export (CSV / Excel)", title: "Export data from form", wired: false },
    { label: "Import", title: "Import data to form", wired: false },
    { label: "Purge (form)", title: "Purge data for form — confirm: Erase Form Data", wired: false, confirmId: "erase" },
  ];

  /** Versions section — Deploy-this-version wired; delete stays grey; Download = metadata. */
  const VERSION_OPS = [
    {
      label: "Push this version",
      title: "Make this version the active deployed definition on :8080 (same uniqueId)",
      wired: "deploy-version",
      id: "deploy-version",
    },
    { label: "Delete this version", title: "Delete this version", wired: false, confirmId: "deleteversion" },
    {
      label: "Download this version of the project",
      title: "Download this version’s metadata (minimal JSON)",
      wired: "download-version",
      id: "download-version",
    },
    {
      label: "Delete Selected Items",
      title: "Delete selected versions",
      wired: false,
      confirmId: "deleteselected",
    },
  ];

  /** Shown when an older Versions row has no definition snapshot (pre–Aug 7, 2026 history). */
  const NO_VERSION_SNAPSHOT_MSG =
    "This version has no saved definition — only versions created after Deploy → Show in My Tawala started saving snapshots (Aug 7, 2026+) can be redeployed.";

  /**
   * Backups / emails / publish dialogs (admin-ish) — archive labels only.
   * Removed from Project Details UI Aug 10, 2026; kept for project-ops-review.html memory.
   */
  const OTHER_OPS = [
    { label: "SCHEDULE BACKUP", title: "Schedule daily backup", wired: false },
    { label: "CHANGE BACKUP", title: "Change backup schedule", wired: false },
    { label: "CANCEL BACKUP", title: "Stop Backups", wired: false },
    { label: "RESTORE (from online backup)", title: "Restore Project from This Backup", wired: false },
    { label: "Delete this backup", title: "Delete this backup", wired: false, confirmId: "deletebackup" },
    {
      label: "DELETE ALL PROJECT BACKUPS",
      title: "Delete all backups for this project",
      wired: false,
      confirmId: "deleteallbackups",
    },
    { label: "View all project emails", title: "View all project emails", wired: false },
    {
      label: "DELETE ALL PROJECT EMAILS",
      title: "Delete All Project Emails",
      wired: false,
      confirmId: "deleteallemails",
    },
    { label: "Publish as a New Project to the library", title: "Copy this app to the library…", wired: false },
    { label: "Update an Existing Library Project", title: "Update an app in the library with this version", wired: false },
    { label: "Upgrade Project", title: "Replace with newer Library version", wired: false },
    { label: "UPDATE (Additional Project Details)", title: "Admin project properties", wired: false },
  ];

  /** Symbiotic transfer flows — Library ↔ My Tawala ↔ Designer (labels first; wire later). */
  const RELATED_SAVE_CLONE = [
    {
      label: "Save a copy",
      source: "Library → My Tawala (acquire; rename-on-acquire; empty data until Deploy)",
      wired: "save-copy-library",
    },
    {
      label: "Use (run or pick start point)",
      source:
        "My Tawala listing / Details — single-start opens :8080; multi-start opens Project Details (not Library; not CloneAndCustomize)",
      wired: "use-project",
    },
    { label: "Publish / move to Library", source: "My Tawala → Library", wired: "publish-mytawala" },
    { label: "Get from Library…", source: "My Tawala listing → Library picker → Save a copy", wired: "get-from-library" },
    { label: "Refresh from Library", source: "Library → My Tawala upgrade (linked row)", wired: "pull-library" },
    { label: "Push from Web Designer", source: "Designer :5173 → My Tawala inbox", wired: false },
  ];

  /** Confirm dialog titles/copy from confirmationdialogs.jsp */
  const CONFIRMS = {
    purge: {
      title: "Purge Project Data",
      body: "Are you sure you want to purge all the data from this project?",
      submit: "Purge",
    },
    delete: {
      title: "Delete Project",
      body: "Are you sure you want to delete this project from My Tawala?",
      submit: "Delete",
    },
    erase: {
      title: "Erase Form Data",
      body: "Are you sure you want to erase the data for this form?",
      submit: "Erase",
    },
    deleteversion: {
      title: "Delete Project Version",
      body: "Are you sure you want to delete this project version?",
      submit: "Delete Version",
    },
    deleteselected: {
      title: "Delete Selected Items",
      body: "Are you sure you want to delete the selected versions?",
      submit: "Delete",
    },
    deletebackup: {
      title: "Delete Backup",
      body: "Are you sure you want to delete this backup?",
      submit: "Delete",
    },
    deleteallbackups: {
      title: "Delete All Project Backups",
      body: "Are you sure you want to delete all backups for this project?",
      submit: "Delete All Backups",
    },
    deleteallemails: {
      title: "Delete All Project Emails",
      body: "Are you sure you want to delete all project emails?",
      submit: "Delete All Emails",
    },
    importResponses: {
      title: "Import Project Data",
      body:
        "Are you sure you want to import this data? It replaces the current response data for the matching form(s) in this project. Import does not change the project definition.",
      submit: "Import",
    },
    restoreProject: {
      title: "Restore Project",
      body:
        "Are you sure you want to restore this project from a backup? This replaces the project's My Tawala properties and response data with the backed-up version.",
      submit: "Restore",
    },
  };

  const ARCHIVE_NOTE =
    "Archive sources: projectmanager/detail.jsp (Project Actions), view.jsp (listing Purge/Delete historically), " +
    "block-projectManagerProjectDetails.jsp (REVISE / ONLINE-OFFLINE / Include / Invite), " +
    "submenu-mytawala.jsp, submenu-library.jsp, confirmationdialogs.jsp. " +
    "Mock My Tawala listing is lean (Aug 9/10): Name · Created · Updated · Records · Times used · Last used · Use; " +
    "selection bar Get/Refresh/Delete; Details bar Backup/Restore/Publish; " +
    "Project Data banner Use/Copy/Export/Import/Purge. " +
    "Catalog is split: Public Library controls vs My Tawala / Project Manager. " +
    "Library Actions = Test drive | Copy link | Save a copy; Use is on My Tawala only (single-start → :8080; multi-start → Project Details). " +
    "SportsDashboards (not SportsBoard).";

  /** One-liner for API/plumbing failures only — never dump CLI / DirtBowl / Test-drive notes into Purge alerts. */
  const LOCAL_PURGE_HELP =
    "Needs designer-web API on :3001 and Docker Postgres (Online Exam Builder can show/purge seeded demo Records when live counts are unavailable).";

  /** Catalog sections — split by product surface (Library vs My Tawala / Project Manager). */
  const OPS_CATALOG_SECTIONS = [
    {
      surface: "library",
      id: "library-submenu",
      title: "Library sub-menu",
      where: "submenu-library.jsp — Search / Edit Categories / Recent Changes",
      items: LIBRARY_SUBMENU,
    },
    {
      surface: "library",
      id: "library-chrome",
      title: "Library chrome (category tree + admin)",
      where: "Library listing — Expand/Collapse wired; manageLibrary.jsp admin stubs grey",
      items: [...LIBRARY_CATEGORY_STUBS, ...LIBRARY_ADMIN_STUBS],
    },
    {
      surface: "library",
      id: "library-listing",
      title: "Public Library listing row",
      where:
        "Library mock — Actions sub-labels: Test drive | Copy link | Save a copy (icons in rows; Test drive / Copy when :8080 deployed; no account). Use moved to My Tawala. searchLibrary.jsp was click-to-detail only.",
      items: LIBRARY_LISTING_ACTIONS,
    },
    {
      surface: "library",
      id: "wizards",
      title: "Related Save / transfer hops",
      where: "Library Save a copy (stub) + My Tawala Use (run) + Publish/Pull/Deploy hops",
      items: RELATED_SAVE_CLONE,
    },
    {
      surface: "mytawala",
      id: "submenu",
      title: "My Tawala sub-menu (removed from mock UI)",
      where:
        "submenu-mytawala.jsp archive — My Projects / My Account / Change Password bar removed from " +
        "mytawala.html + mytawala-project.html (Aug 9); Account stays in primary chrome Welcome menu.",
      items: MYTAWALA_SUBMENU,
    },
    {
      surface: "mytawala",
      id: "listing-bar",
      title: "My Projects listing — selection bar",
      where:
        "mytawala.html — Get from Library… (always) · Refresh from Library (selection + Library link) · Delete (selection). " +
        "Owner Aug 9 option 3: acquire ≠ refresh; both stay off Project Details.",
      items: LISTING_BAR_ACTIONS,
    },
    {
      surface: "mytawala",
      id: "listing",
      title: "My Projects listing row",
      where:
        "mytawala.html — lean listing (Aug 9/10): click row to select · double-click → Details · " +
        "Name · Created · Updated · Records · Times used · Last used · Use. " +
        "Use: single-start opens :8080; multi-start opens Project Details (no purge). Get/Refresh/Delete via listing bar.",
      items: LISTING_ACTIONS,
    },
    {
      surface: "mytawala",
      id: "actions",
      title: "Project Details — action bar",
      where:
        "projectmanager/detail.jsp — RENAME · MAKE A COPY · BACKUP · RESTORE · DEPLOY · PUBLISH (flush right). " +
        "Use / Copy / Export / Import / Purge moved to Project Data banner (Aug 9). " +
        "Get/Refresh/Delete on listing bar (Aug 9 option 3).",
      items: PROJECT_ACTIONS,
    },
    {
      surface: "mytawala",
      id: "sidebar",
      title: "Project Details — left sidebar",
      where:
        "Wider left rail (Aug 9; Deploy share Aug 10): identity stack (Author / Version / Published / Status / Theme), " +
        "then Invite / Include (→ Deploy share panel) under Theme, then Edit in Designer. " +
        "Main column = title + Project Data / Versions / Comments (Project Data higher above the fold).",
      items: PROJECT_SIDEBAR_OPS,
    },
    {
      surface: "mytawala",
      id: "data",
      title: "Project Details — Project Data (collapsible)",
      where:
        "One unified form list (Aug 9): collapsed → starts (▶) → all forms; same tight row style. " +
        "Banner: Records / Times used / Last used heads + Use / Copy link | Export Import | Purge. " +
        "Records = submissions; Times used / Last used = mock My Tawala Use→:8080 sessions (not Test Drive). " +
        "Project caret (▸) expands; start rows use a muted left-pointing cue (not an expander). " +
        "Project-level Times used / Last used on a row under the banner; form rows show Records only. " +
        "Enablement: collapsed/project → E/I/Purge + Backup/Restore/Publish (Use/Copy off); " +
        "start ▶ → Use/Copy + E/I/Purge/Publish (Backup/Restore off); " +
        "non-start form → E/I/Purge/Publish only. Legacy per-form View chips stay grey.",
      items: [...PROJECT_DATA_CONTROLS, ...PROJECT_DATA_OPS],
    },
    {
      surface: "mytawala",
      id: "versions",
      title: "Project Details — Versions (collapsible)",
      where: "detail.jsp versions section",
      items: VERSION_OPS,
    },
    {
      surface: "mytawala",
      id: "other",
      title: "Archive — Backups / emails / publish / admin (removed from Details UI)",
      where:
        "Was detail.jsp + confirmation dialogs; labels kept on project-ops-review only (Aug 10, 2026)",
      items: OTHER_OPS,
    },
  ];

  const SURFACE_INTRO = {
    library: {
      heading: "Public Library controls",
      blurb:
        "library.html listing — Test drive + Copy link (public); Save to MyTawala when logged in. Public Library detail pages retired. Use (run) is on My Tawala.",
    },
    mytawala: {
      heading: "My Tawala / Project Manager controls",
      blurb:
        "mytawala.html (lean listing + selection-bar Pull/Delete) + mytawala-project.html (Details). Guests see snapshot taste (mytawala-demo.html). Private projects; Use opens :8080 when single-start, else Project Details.",
    },
  };

  function escapeHtml(s) {
    return String(s)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  function isOpActive(op) {
    return (
      op.wired === true ||
      op.wired === "purge-local" ||
      op.wired === "delete-mytawala" ||
      op.wired === "delete-mock" ||
      op.wired === "edit-categories" ||
      op.wired === "publish-mytawala" ||
      op.wired === "deploy-share" ||
      op.wired === "export-mytawala" ||
      op.wired === "import-mytawala" ||
      op.wired === "backup-mytawala" ||
      op.wired === "restore-mytawala" ||
      op.wired === "pull-library" ||
      op.wired === "save-copy-library" ||
      op.wired === "copy-testdrive-link" ||
      op.wired === "rename-project" ||
      op.wired === "make-copy-mytawala" ||
      op.wired === "download-version" ||
      op.wired === "deploy-version"
    );
  }

  function wiredNote(item) {
    if (item.wired === "purge-local") return "active (purge via :3001 API)";
    if (item.wired === "delete-mytawala" || item.wired === "delete-mock") {
      return "active (Delete My Tawala row — overlay/inbox; not Library)";
    }
    if (item.wired === "edit-categories") return "active (local category assignment)";
    if (item.wired === "publish-mytawala") return "active (Publish dialog → Library overlay)";
    if (item.wired === "deploy-share") {
      return "active (Deploy share panel — copy start link / iframe embed)";
    }
    if (item.wired === "rename-project") return "active (Rename dialog → My Tawala overlay)";
    if (item.wired === "make-copy-mytawala") {
      return "active (Make a Copy dialog → new My Tawala fork; original untouched)";
    }
    if (item.wired === "export-mytawala") return "active (download JSON — data only, see README)";
    if (item.wired === "import-mytawala") return "active (field-mismatch check + POST :3001)";
    if (item.wired === "backup-mytawala") return "active (download JSON — definition + data + properties)";
    if (item.wired === "restore-mytawala") return "active (properties overlay + POST :3001 data)";
    if (item.wired === "pull-library") return "active (Pull dialog → overlay content refresh from Library)";
    if (item.wired === "save-copy-library") {
      return "active (rename dialog → new My Tawala row; empty data until Deploy)";
    }
    if (item.wired === "download-version") {
      return "active (minimal version metadata JSON — Details Versions only)";
    }
    if (item.wired === "deploy-version") {
      return "active when a non-current version with a saved definition snapshot is selected";
    }
    if (item.wired === "use-project") {
      return "active when :8080 start URL exists (single → run; multi → Project Details; no purge; My Tawala only)";
    }
    if (item.wired === "test-drive") {
      return "active when :8080 deployed (single → open; multi → start hot links; purge-on-start not leave; no account)";
    }
    if (item.wired === "copy-testdrive-link") {
      return "active when :8080 deployed — copy shared Library uniqueId URL (multi → start hot links; ≠ Deploy Copy link)";
    }
    if (item.wired === true) return "active";
    return "disabled in mock";
  }

  function listingIconHtml(iconId) {
    return LISTING_ICONS[iconId] || "";
  }

  function renderSubmenuItems(items, active) {
    return (
      "<ul>" +
      items
        .map((item) => {
          const sel = item.label === active ? " selected" : "";
          if (!item.wired) {
            return `<li><span class="link-pending${sel}" title="${escapeHtml(item.title)}" aria-disabled="true">${escapeHtml(item.label)}</span></li>`;
          }
          if (item.wired === "edit-categories" || item.id === "edit-categories") {
            return (
              `<li><a class="${sel.trim()}" href="#edit-categories" title="${escapeHtml(item.title)}" ` +
              `data-op="edit-categories" data-wired="edit-categories">${escapeHtml(item.label)}</a></li>`
            );
          }
          return `<li><a class="${sel.trim()}" href="${item.href}" title="${escapeHtml(item.title)}">${escapeHtml(item.label)}</a></li>`;
        })
        .join("") +
      "</ul>"
    );
  }

  function renderSubmenu(active) {
    return (
      '<div class="sub-menu pm-submenu">' +
      renderSubmenuItems(MYTAWALA_SUBMENU, active) +
      "</div>"
    );
  }

  function renderLibrarySubmenu(active) {
    return (
      '<div class="sub-menu pm-submenu library-submenu">' +
      renderSubmenuItems(LIBRARY_SUBMENU, active || "Search") +
      "</div>"
    );
  }

  function renderLibraryChromeStubs() {
    const treeBtns = LIBRARY_CATEGORY_STUBS.map((op) => {
      const active = isOpActive(op);
      const disabled = active ? "" : " disabled";
      return (
        `<button type="button" class="pm-icon-action"${disabled} ` +
        `title="${escapeHtml(op.title)}" data-op="${escapeHtml(op.id)}" ` +
        `data-wired="${escapeHtml(active ? String(op.wired) : "false")}">` +
        `${escapeHtml(op.label)}</button>`
      );
    }).join(" ");
    const adminBtns = LIBRARY_ADMIN_STUBS.map((op) => actionButton(op, "")).join("");
    return (
      '<div class="library-chrome-stubs">' +
      '<div class="library-tree-controls" role="group" aria-label="Category tree">' +
      treeBtns +
      "</div>" +
      '<div class="pm-actions-bar library-admin-stubs" role="toolbar" aria-label="Library admin">' +
      adminBtns +
      "</div>" +
      "</div>"
    );
  }

  /**
   * Library “Actions” header — group label plus sub-labels aligned over icon slots.
   * Labels: Test drive | Copy link | Save to MyTawala (Save only when logged in).
   */
  function renderLibraryListingActionHeaders() {
    const actions = visibleLibraryListingActions();
    const slots = Math.max(actions.length, 1);
    const subs = actions
      .map((op) => {
        let extra = "";
        if (op.metric === "timesUsed") {
          extra = `<span class="library-action-metric-head">Times used</span>`;
        } else if (op.metric === "cloneCount") {
          extra = `<span class="library-action-metric-head">Copies</span>`;
        }
        return (
          `<span class="library-action-subhead" title="${escapeHtml(op.title)}">` +
          `<span class="library-action-subhead-label">${escapeHtml(op.label)}</span>` +
          extra +
          `</span>`
        );
      })
      .join("");
    return (
      `<div class="lib-col lib-col-actions" role="columnheader" ` +
      `style="--library-action-slots: repeat(${slots}, minmax(0, 1fr))">` +
      `<div class="library-actions-head">` +
      `<span class="th-label">Actions</span>` +
      `<span class="library-action-subheads" role="group" aria-label="Action columns">` +
      subs +
      `</span>` +
      `</div>` +
      `</div>`
    );
  }

  /**
   * Library listing — one Actions grid cell with spaced icons (sub-labels in header).
   * Test drive + Copy link when deployed; Save a copy always on (rename → My Tawala).
   * Multi-start (2+ :8080 URLs): Test Drive / Copy open a hot-link list of starts.
   * Single-start: open / copy directly. Copy link = same :8080 URL Test Drive opens (viral share).
   */
  function libraryDriveUrl(project) {
    if (
      typeof TawalaDemo !== "undefined" &&
      typeof TawalaDemo.libraryTestDriveUrl === "function"
    ) {
      return TawalaDemo.libraryTestDriveUrl(project);
    }
    return project && project.testDriveUrl ? project.testDriveUrl : null;
  }

  function resolveLibraryProject(projectId) {
    if (!projectId || typeof TawalaDemo === "undefined") return null;
    if (typeof TawalaDemo.getLibrary !== "function") return null;
    const raw = TawalaDemo.getLibrary(projectId);
    if (!raw) return null;
    return { ...raw, id: raw.id || projectId };
  }

  function libraryStartPointsWithUrls(project) {
    return ((project && project.startPoints) || []).filter((s) => s && s.url);
  }

  function isLibraryMultiStart(project) {
    return libraryStartPointsWithUrls(project).length >= 2;
  }

  /** Prefer Library Test Drive ranking (Admin/Setup for Exam apps); else first start. */
  function preferredLibraryStartIndex(starts, project) {
    if (!starts || !starts.length) return 0;
    let preferred = null;
    if (
      typeof TawalaDemo !== "undefined" &&
      typeof TawalaDemo.pickLibraryTestDriveStartPoint === "function"
    ) {
      preferred = TawalaDemo.pickLibraryTestDriveStartPoint(
        project && project.startPoints ? project.startPoints : starts
      );
    }
    if (preferred && preferred.url) {
      const idx = starts.findIndex((s) => s && s.url === preferred.url);
      if (idx >= 0) return idx;
    }
    return 0;
  }

  function renderLibrarySaveCopyButton(project, variant) {
    /* Prefer project.id; libraryEntries() always sets it. getLibrary() now stamps id too
     * (Aug 10 libsc1) — empty pid used to break library-detail Save a copy. */
    const pid = (project && project.id) || "";
    const title = "Save to MyTawala (rename on the way in)";
    if (variant === "text") {
      return (
        `<button type="button" class="pm-action is-active library-save-copy" ` +
        `title="${escapeHtml(title)}" aria-label="Save to MyTawala" ` +
        `data-op="save-my-tawala" data-wired="save-copy-library" ` +
        `data-project="${escapeHtml(pid)}">Save to MyTawala</button>`
      );
    }
    return (
      `<button type="button" class="pm-icon-action pm-op-icon-btn is-active library-row-action" ` +
      `title="${escapeHtml(title)}" aria-label="Save to MyTawala" ` +
      `data-op="save-my-tawala" data-wired="save-copy-library" ` +
      `data-project="${escapeHtml(pid)}">${listingIconHtml("save")}</button>`
    );
  }

  /**
   * Library Test Drive — listing icon or detail text button.
   * Single-start: direct :8080 open (purge-on-start). Multi-start: opens start hot-link list.
   */
  function renderLibraryTestDriveButton(project, variant) {
    const pid = (project && project.id) || "";
    const driveUrl = libraryDriveUrl(project);
    const deployed =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project)) || !!driveUrl;
    const multi = isLibraryMultiStart(project);
    const title = multi
      ? honestyText(
          "tooltipMulti",
          "Choose a start. Clears this Library demo when you start (not when you close the tab). All starts stay usable during the drive."
        )
      : honestyText(
          "tooltipSingle",
          "No account. Clears this Library demo when you start (not when you close the tab), then opens :8080."
        );
    if (variant === "text") {
      if (!deployed || !driveUrl) {
        return (
          `<button type="button" class="pm-action" disabled title="No local test-drive yet">Test Drive</button>`
        );
      }
      if (multi) {
        return (
          `<button type="button" class="pm-action is-active library-testdrive-pick" ` +
          `title="${escapeHtml(title)}" aria-label="Test Drive — choose start" ` +
          `data-op="test-drive" data-wired="test-drive" data-testdrive-intent="open" ` +
          `data-project="${escapeHtml(pid)}">Test Drive</button>`
        );
      }
      return (
        `<a class="pm-action is-active js-testdrive" href="${escapeHtml(driveUrl)}" ` +
        `data-testdrive-url="${escapeHtml(driveUrl)}" target="_blank" rel="noopener" ` +
        `data-project="${escapeHtml(pid)}" ` +
        `title="${escapeHtml(title)}">Test Drive</a>`
      );
    }
    /* Listing icon */
    if (!deployed || !driveUrl) {
      return (
        `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
        `title="No local test-drive yet" aria-label="Test drive unavailable" ` +
        `data-op="test-drive" data-wired="false">${listingIconHtml("testdrive")}</button>`
      );
    }
    if (multi) {
      return (
        `<button type="button" class="pm-icon-action pm-op-icon-btn is-active library-row-action library-testdrive-pick" ` +
        `title="${escapeHtml(title)}" aria-label="Test Drive — choose start" ` +
        `data-op="test-drive" data-wired="test-drive" data-testdrive-intent="open" ` +
        `data-project="${escapeHtml(pid)}">${listingIconHtml("testdrive")}</button>`
      );
    }
    return (
      `<a class="pm-icon-action pm-op-icon-btn is-active library-row-action js-testdrive" ` +
      `href="${escapeHtml(driveUrl)}" data-testdrive-url="${escapeHtml(driveUrl)}" ` +
      `target="_blank" rel="noopener" data-project="${escapeHtml(pid)}" ` +
      `title="${escapeHtml(title)}" aria-label="Test drive this project" ` +
      `data-op="test-drive" data-wired="true">${listingIconHtml("testdrive")}</a>`
    );
  }

  /**
   * Library Copy Test Drive link — listing icon or detail text button.
   * Single-start: copies libraryTestDriveUrl. Multi-start: hot-link list then copy chosen start.
   */
  function renderLibraryCopyTestDriveButton(project, variant) {
    const pid = (project && project.id) || "";
    const driveUrl = libraryDriveUrl(project);
    const deployed =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project)) || !!driveUrl;
    const multi = isLibraryMultiStart(project);
    const title = multi
      ? honestyText(
          "copyTooltipMulti",
          "Choose a start, then copy its try-out URL. Same shared Library demo as Test Drive — not a private copy."
        )
      : honestyText(
          "copyTooltipSingle",
          "Copy the live try-out URL (no account). Same shared Library demo as Test Drive — not a private copy."
        );
    if (variant === "text") {
      if (deployed && driveUrl) {
        return (
          `<button type="button" class="pm-action is-active library-copy-testdrive" ` +
          `title="${escapeHtml(title)}" aria-label="Copy Test Drive link" ` +
          `data-op="copy-testdrive-link" data-wired="copy-testdrive-link" ` +
          `data-project="${escapeHtml(pid)}"` +
          (multi ? "" : ` data-testdrive-copy-url="${escapeHtml(driveUrl)}"`) +
          `>Copy link</button>`
        );
      }
      return (
        `<button type="button" class="pm-action library-copy-testdrive" disabled ` +
        `title="No local test-drive URL to copy yet" aria-label="Copy Test Drive link unavailable">Copy link</button>`
      );
    }
    if (deployed && driveUrl) {
      return (
        `<button type="button" class="pm-icon-action pm-op-icon-btn is-active library-row-action library-copy-testdrive" ` +
        `title="${escapeHtml(title)}" aria-label="Copy Test Drive link" ` +
        `data-op="copy-testdrive-link" data-wired="copy-testdrive-link" ` +
        `data-project="${escapeHtml(pid)}"` +
        (multi ? "" : ` data-testdrive-copy-url="${escapeHtml(driveUrl)}"`) +
        `>${listingIconHtml("link")}</button>`
      );
    }
    return (
      `<button type="button" class="pm-icon-action pm-op-icon-btn library-row-action" disabled ` +
      `title="No local test-drive URL to copy yet" aria-label="Copy Test Drive link unavailable" ` +
      `data-op="copy-testdrive-link" data-wired="false">${listingIconHtml("link")}</button>`
    );
  }

  function renderLibraryListingControlCells(project) {
    const actions = visibleLibraryListingActions();
    const slots = Math.max(actions.length, 1);
    const timesUsed =
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.formatLibraryTimesUsed === "function"
        ? TawalaTransfer.formatLibraryTimesUsed(project)
        : Number(project && project.timesUsed) > 0
          ? Math.floor(Number(project.timesUsed))
          : 0;
    const cloneCount =
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.formatCloneCount === "function"
        ? TawalaTransfer.formatCloneCount(project)
        : Number(project && project.cloneCount) > 0
          ? Math.floor(Number(project.cloneCount))
          : 0;
    const icons = actions
      .map((op) => {
        let ctrl = "";
        if (op.wired === "test-drive") {
          ctrl = renderLibraryTestDriveButton(project, "icon");
        } else if (op.wired === "copy-testdrive-link") {
          ctrl = renderLibraryCopyTestDriveButton(project, "icon");
        } else if (op.wired === "save-copy-library") {
          ctrl = renderLibrarySaveCopyButton(project, "icon");
        } else {
          ctrl =
            `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
            `title="${escapeHtml(op.title)}" aria-label="${escapeHtml(op.label)}" ` +
            `data-op="${escapeHtml(op.id)}" data-wired="false">${listingIconHtml(op.icon)}</button>`;
        }
        let metric = "";
        if (op.metric === "timesUsed") {
          metric =
            `<span class="library-action-metric" title="Times used — Library Test Drive opens">` +
            `${escapeHtml(String(timesUsed))}</span>`;
        } else if (op.metric === "cloneCount") {
          metric =
            `<span class="library-action-metric" title="Copies downloaded — Save to MyTawala / Get from Library">` +
            `${escapeHtml(String(cloneCount))}</span>`;
        }
        return `<span class="library-action-slot">${ctrl}${metric}</span>`;
      })
      .join("");
    /* Do not stopPropagation here — document-level bind()/Test Drive handlers are bubble-phase
     * and would never see Save / purge-on-start clicks. */
    return (
      `<div class="lib-col lib-col-actions" style="--library-action-slots: repeat(${slots}, minmax(0, 1fr))">` +
      `<span class="library-row-actions">${icons}</span>` +
      `</div>`
    );
  }

  /** @deprecated Prefer renderLibraryListingControlCells — single-cell text controls. */
  function renderLibraryListingControls(project) {
    return (
      '<div class="controls pm-listing-controls library-listing-controls">' +
      visibleLibraryListingActions()
        .map((op) => {
          if (op.wired === "test-drive") {
            return renderLibraryTestDriveButton(project, "icon");
          }
          if (op.wired === "copy-testdrive-link") {
            return renderLibraryCopyTestDriveButton(project, "icon");
          }
          if (op.wired === "save-copy-library") {
            return renderLibrarySaveCopyButton(project, "icon");
          }
          return (
            `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
            `title="${escapeHtml(op.title)}" aria-label="${escapeHtml(op.title)}" ` +
            `data-op="${escapeHtml(op.id)}" data-wired="false">${listingIconHtml(op.icon)}</button>`
          );
        })
        .join(" ") +
      "</div>"
    );
  }

  /** Resolve a My Tawala project for Use / deploy checks. */
  function resolveMyTawalaProject(projectId) {
    if (!projectId || typeof TawalaDemo === "undefined") return null;
    if (typeof TawalaDemo.getMyTawala === "function") {
      const p = TawalaDemo.getMyTawala(projectId);
      if (p) return p;
    }
    if (typeof TawalaDemo.get === "function") return TawalaDemo.get(projectId) || null;
    return null;
  }

  /**
   * Single-start :8080 URL for Use (operate) — no purge-on-start.
   * Prefer examinee form (Exam / Registration / Survey) over Admin/Setup when ranking.
   * Multi-start Use does not open this directly — see projectUseTarget.
   */
  function projectUseUrl(project) {
    if (!project) return null;
    if (typeof TawalaDemo !== "undefined" && typeof TawalaDemo.primaryStartUrl === "function") {
      const preferred = TawalaDemo.primaryStartUrl(project.startPoints, null);
      if (preferred) return preferred;
    }
    if (project.testDriveUrl) return project.testDriveUrl;
    const sp = (project.startPoints || []).find((s) => s && s.url);
    return sp ? sp.url : null;
  }

  /** Honest grey-Use tip when Save a copy left the row without :8080 start URLs. */
  const USE_NEEDS_DEPLOY_TITLE =
    "Deploy this project before Use — no :8080 start URL yet (Save a copy starts empty)";

  function projectHasUseRuntime(project) {
    return !!(projectUseUrl(project) || startPointsWithUrls(project).length);
  }

  /**
   * Local calendar YMD key (browser timezone) for same-day chip expiry.
   * Accepts ISO, epoch ms, or short `M/D/YY` listing dates.
   */
  function localCalendarDayKey(value) {
    if (value == null || value === "" || value === "—") return "";
    let ms = 0;
    if (typeof value === "number" && Number.isFinite(value)) {
      ms = value > 0 ? value : 0;
    } else {
      const str = String(value).trim();
      if (!str) return "";
      if (/^\d{4}-\d{2}-\d{2}/.test(str) || str.includes("T")) {
        ms = Date.parse(str);
      } else if (/^\d{10,13}$/.test(str)) {
        const n = Number(str);
        ms = n < 1e12 ? n * 1000 : n;
      } else {
        const m = str.match(/^(\d{1,2})\/(\d{1,2})\/(\d{2,4})$/);
        if (m) {
          let y = Number(m[3]);
          if (y < 100) y += 2000;
          ms = new Date(y, Number(m[1]) - 1, Number(m[2])).getTime();
        } else {
          ms = Date.parse(str);
        }
      }
    }
    if (!ms || Number.isNaN(ms)) return "";
    const d = new Date(ms);
    return `${d.getFullYear()}-${d.getMonth() + 1}-${d.getDate()}`;
  }

  /**
   * My Tawala listing chip only — blue “from Library” when Save a copy / Get from Library
   * happened today (same local calendar date as now). Hidden after midnight next day.
   * Details sidebar Source / provenance stays forever (fromLibraryProvenanceLine).
   */
  function fromLibraryAcquireBadge(project) {
    if (!project || !(project.fromLibraryAcquire || project.sourcePile === "library-acquire")) {
      return "";
    }
    const acquiredDay =
      localCalendarDayKey(project.createdAt) || localCalendarDayKey(project.created);
    if (!acquiredDay || acquiredDay !== localCalendarDayKey(Date.now())) {
      return "";
    }
    const src = librarySourceDisplayName(project) || "Library";
    return (
      ` <span class="pm-status-badge is-from-library" ` +
      `title="from ${escapeHtml(src)} in Library">from Library</span>`
    );
  }

  /** Resolve Library template display name for provenance (acquire / pull link). */
  function librarySourceDisplayName(project) {
    if (!project) return "";
    if (project.pulledFromLibraryName) {
      return window.TawalaDemo && typeof window.TawalaDemo.displayName === "function"
        ? window.TawalaDemo.displayName(project.pulledFromLibraryName)
        : String(project.pulledFromLibraryName);
    }
    const libId = project.pulledFromLibraryId ? String(project.pulledFromLibraryId) : "";
    if (
      libId &&
      typeof TawalaDemo !== "undefined" &&
      typeof TawalaDemo.getLibrary === "function"
    ) {
      const lib = TawalaDemo.getLibrary(libId);
      if (lib && lib.name) {
        return typeof TawalaDemo.displayName === "function"
          ? TawalaDemo.displayName(lib.name)
          : String(lib.name);
      }
    }
    return libId;
  }

  /**
   * Sidebar Source line wording helper — "from … in Library".
   * Main Details header no longer shows provenance (sidebar only).
   */
  function fromLibraryProvenanceLine(project) {
    if (
      !project ||
      !(project.fromLibraryAcquire || project.sourcePile === "library-acquire" || project.pulledFromLibraryId)
    ) {
      return "";
    }
    const libId = project.pulledFromLibraryId ? String(project.pulledFromLibraryId) : "";
    const libName = librarySourceDisplayName(project) || "Library";
    const libHref = libId
      ? `library.html?highlight=${encodeURIComponent(libId)}`
      : "library.html";
    return (
      `<p class="pm-detail-provenance">from ` +
      `<a href="${escapeHtml(libHref)}">${escapeHtml(libName)}</a> in Library</p>`
    );
  }

  /** Original Designer name for Push provenance (Rename must not overwrite this field). */
  function designerSourceDisplayName(project) {
    if (!project) return "";
    const raw = String(project.designerName || "").trim();
    if (!raw) return "";
    return window.TawalaDemo && typeof window.TawalaDemo.displayName === "function"
      ? window.TawalaDemo.displayName(raw)
      : raw;
  }

  /**
   * Sidebar Source helper — quiet “from Designer: …” (same rail style as Library).
   */
  function fromDesignerProvenanceLine(project) {
    const name = designerSourceDisplayName(project);
    if (!name) return "";
    return `<p class="pm-detail-provenance">from Designer: ${escapeHtml(name)}</p>`;
  }

  /** True when href targets local Tomcat (:8080) — shared helper on TawalaDemo. */
  function isLocalJavaRuntimeUrl(url) {
    if (window.TawalaDemo && typeof window.TawalaDemo.isLocalJavaRuntimeUrl === "function") {
      return window.TawalaDemo.isLocalJavaRuntimeUrl(url);
    }
    return /(?:localhost|127\.0\.0\.1):8080/i.test(String(url || ""));
  }

  /** Probe local Java/Tomcat — same cache/path as Library Test Drive (TawalaDemo). */
  async function probeLocalJavaRuntime(timeoutMs) {
    if (window.TawalaDemo && typeof window.TawalaDemo.probeLocalJavaRuntime === "function") {
      return window.TawalaDemo.probeLocalJavaRuntime(timeoutMs);
    }
    return false;
  }

  const USE_OFFLINE_ALERT =
    "Use needs the Java runtime on http://localhost:8080 — it isn’t reachable right now.\n\n" +
    "You’re still on the :5500 mock. For offline Purge review with seeded demo Records:\n" +
    "  1. Stay on Project Details (Online Exam Builder)\n" +
    "  2. Expand Project Data (▸)\n" +
    "  3. Select the project row (or a form)\n" +
    "  4. Click Purge — do not click Use\n\n" +
    "Copy link still copies the real :8080 start URL if you need it later.";

  async function openUseRuntimeUrl(url, opts) {
    if (!url || url === "#") return;
    setStatus("Checking Java runtime on :8080…");
    const up = await probeLocalJavaRuntime();
    if (!up) {
      setStatus("Use blocked — :8080 unreachable. For offline review: select project/form → Purge (not Use).");
      window.alert(USE_OFFLINE_ALERT);
      return;
    }
    const projectId = opts && opts.projectId ? String(opts.projectId) : "";
    /* Mock Times used / Last used (Task #13): count a successful Use → :8080 open.
     * Not Library Test Drive. Not live respondent telemetry. */
    if (
      projectId &&
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.recordRespondentSession === "function"
    ) {
      const stats = TawalaTransfer.recordRespondentSession(projectId);
      if (stats) setUsageStatsDisplay(document, stats);
    }
    setStatus("Opening start form on :8080…");
    window.open(url, "_blank", "noopener");
  }

  function startPointsWithUrls(project) {
    return ((project && project.startPoints) || []).filter((s) => s && s.url);
  }

  function listedStartPoints(project) {
    return ((project && project.startPoints) || []).filter(
      (s) => s && (s.url || s.label || s.form)
    );
  }

  function projectDetailsHref(projectId, hash, queryExtra) {
    const params = new URLSearchParams();
    params.set("project", projectId || "");
    if (queryExtra && typeof queryExtra === "object") {
      Object.keys(queryExtra).forEach((key) => {
        const val = queryExtra[key];
        if (val == null || val === "") return;
        params.set(key, String(val));
      });
    }
    const base = `mytawala-project.html?${params.toString()}`;
    return hash ? `${base}#${hash}` : base;
  }

  /**
   * Listing Use (multi-start) lands on Details with starts expanded + first start selected.
   * Double-click / plain Details links omit this — tree stays collapsed.
   */
  function wantsOpenStartsFromLocation() {
    if (typeof location === "undefined") return false;
    try {
      const params = new URLSearchParams(location.search || "");
      if (params.get("openStarts") === "1" || params.get("fromUse") === "1") return true;
    } catch {
      /* ignore */
    }
    const h = location.hash || "";
    return h === "#pmSecData" || h === "#pmDataStartPoints";
  }

  /**
   * Use navigation: multi-entry apps (2+ start points) go to Project Details so the
   * user picks Setup vs Exam (etc.). Single-start opens the one :8080 URL in a new tab.
   * Owner Aug 4, 2026 — jumping straight to Exam skipped setup and hit stale sessions.
   * Owner Aug 10 — Details entry opens Project Data to starts + first start highlighted
   * so banner Use / Copy link are immediately meaningful.
   */
  function projectUseTarget(project) {
    if (!project) return null;
    const listed = listedStartPoints(project);
    const withUrls = startPointsWithUrls(project);
    const runtimeUrl = projectUseUrl(project);
    if (!withUrls.length && !runtimeUrl) return null;
    if (listed.length > 1) {
      return {
        href: projectDetailsHref(project.id, "pmSecData", { openStarts: "1" }),
        openInNewTab: false,
        title: "Choose a start point on Project Details (multi-entry project)",
        mode: "details",
      };
    }
    if (!runtimeUrl) return null;
    return {
      href: runtimeUrl,
      openInNewTab: true,
      title:
        "Open / run this project on :8080 (needs Java runtime; offline Purge demo Records work without Use)",
      mode: "runtime",
    };
  }

  function renderUseAnchor(op, projectId, variant) {
    const project = resolveMyTawalaProject(projectId);
    const target = projectUseTarget(project);
    const deployed =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project)) || !!target;
    const asIcon = variant === "listing-icon" || variant === "listing-inline";
    if (!(deployed && target)) {
      const offTitle = USE_NEEDS_DEPLOY_TITLE;
      if (asIcon) {
        return (
          `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
          `title="${escapeHtml(offTitle)}" ` +
          `aria-label="Use unavailable — deploy first" data-op="${escapeHtml(op.id)}" ` +
          `data-project="${escapeHtml(projectId || "")}" data-wired="false">${listingIconHtml(op.icon)}</button>`
        );
      }
      return (
        `<button type="button" class="pm-action" disabled ` +
        `title="${escapeHtml(offTitle)}" ` +
        `aria-label="Use unavailable — deploy first" data-op="${escapeHtml(op.id)}" ` +
        `data-project="${escapeHtml(projectId || "")}" data-wired="false">${escapeHtml(op.label)}</button>`
      );
    }
    const title = target.title || op.title;
    const blank = target.openInNewTab ? ` target="_blank" rel="noopener"` : "";
    if (asIcon) {
      return (
        `<a class="pm-icon-action pm-op-icon-btn is-active" href="${escapeHtml(target.href)}"${blank} ` +
        `title="${escapeHtml(title)}" aria-label="${escapeHtml(op.label)}" ` +
        `data-op="${escapeHtml(op.id)}" data-project="${escapeHtml(projectId || "")}" ` +
        `data-wired="use-project" data-use-mode="${escapeHtml(target.mode)}">` +
        `${listingIconHtml(op.icon)}</a>`
      );
    }
    return (
      `<a class="pm-action is-active" href="${escapeHtml(target.href)}"${blank} ` +
      `title="${escapeHtml(title)}" data-op="${escapeHtml(op.id)}" ` +
      `data-project="${escapeHtml(projectId || "")}" data-wired="use-project" ` +
      `data-use-mode="${escapeHtml(target.mode)}">${escapeHtml(op.label)}</a>`
    );
  }

  /** Active ops are clickable; inactive ops are disabled (no “not wired” label — grey is enough). */
  function actionButton(op, projectId) {
    if (op.wired === "use-project") {
      return renderUseAnchor(op, projectId, "action");
    }
    const active = isOpActive(op);
    const data =
      `data-op="${escapeHtml(op.id || op.label)}" data-project="${escapeHtml(projectId || "")}"` +
      (op.confirmId ? ` data-confirm="${escapeHtml(op.confirmId)}"` : "") +
      ` data-wired="${escapeHtml(active ? String(op.wired) : "false")}"`;
    const disabled = active ? "" : " disabled";
    const dangerClass = op.destructive ? " is-destructive" : "";
    return (
      `<button type="button" class="pm-action${dangerClass}"${disabled} title="${escapeHtml(op.title || op.label)}" ${data}>` +
      `${escapeHtml(op.label)}</button>`
    );
  }

  function renderProjectActionsBar(projectId) {
    return (
      '<div class="pm-actions-bar pm-detail-actions buttons" role="toolbar" aria-label="Project Actions">' +
      PROJECT_ACTIONS.map((op) => actionButton(op, projectId)).join("") +
      "</div>"
    );
  }

  /**
   * Whether a My Tawala row is linked to a public Library entry (Refresh gate).
   * Prefer an explicit pull record; else exact id/name twin in the Library catalog.
   */
  function resolveLibraryLink(projectId) {
    if (!projectId || typeof TawalaDemo === "undefined") return null;
    const project = TawalaDemo.getMyTawala ? TawalaDemo.getMyTawala(projectId) : null;
    if (!project) return null;

    const pulledId = project.pulledFromLibraryId ? String(project.pulledFromLibraryId) : "";
    if (pulledId && TawalaDemo.getLibrary && TawalaDemo.getLibrary(pulledId)) {
      const lib = TawalaDemo.getLibrary(pulledId);
      return {
        id: pulledId,
        name: project.pulledFromLibraryName || (lib && lib.name) || pulledId,
        kind: "pulled",
      };
    }

    if (
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.findPullCandidates === "function"
    ) {
      const exact = TawalaTransfer.findPullCandidates(projectId, project.name).filter(
        (m) => m.matchKind === "exact"
      );
      if (exact.length === 1) {
        return { id: exact[0].id, name: exact[0].name, kind: "exact" };
      }
    }

    if (TawalaDemo.getLibrary && TawalaDemo.getLibrary(projectId)) {
      const lib = TawalaDemo.getLibrary(projectId);
      return { id: projectId, name: (lib && lib.name) || projectId, kind: "same-id" };
    }
    return null;
  }

  /**
   * Top-of-list Get / Refresh / Delete — Get always on; Refresh needs selection + Library link;
   * Delete needs selection.
   */
  function renderListingSelectionBar() {
    return (
      '<div class="pm-actions-bar pm-listing-bar buttons" id="myProjectsListingBar" role="toolbar" aria-label="My Tawala list actions">' +
      LISTING_BAR_ACTIONS.map((op) => {
        const dangerClass = op.destructive ? " is-destructive" : "";
        const data =
          `data-op="${escapeHtml(op.id || op.label)}" data-project=""` +
          (op.confirmId ? ` data-confirm="${escapeHtml(op.confirmId)}"` : "") +
          ` data-wired="${escapeHtml(String(op.wired))}"` +
          (op.alwaysEnabled ? ' data-always-enabled="true"' : "") +
          (op.requiresLibraryLink ? ' data-requires-library-link="true"' : "");
        const startDisabled = op.alwaysEnabled ? "" : " disabled";
        const activeClass = op.wired === "get-from-library" ? " is-active" : "";
        return (
          `<button type="button" class="pm-action${activeClass}${dangerClass}"${startDisabled} title="${escapeHtml(op.title || op.label)}" ${data}>` +
          `${escapeHtml(op.label)}</button>`
        );
      }).join("") +
      '<span class="pm-listing-bar-hint" id="myProjectsListingBarHint">Get from Library saves a copy into My Tawala · select a project to Make a Copy or Delete · select a linked project to Refresh</span>' +
      "</div>"
    );
  }

  /** Enable/disable listing bar buttons for the selected project id (or none). */
  function syncListingSelectionBar(projectId) {
    const bar = document.getElementById("myProjectsListingBar");
    if (!bar) return;
    const id = projectId ? String(projectId) : "";
    const link = id ? resolveLibraryLink(id) : null;
    const hint = document.getElementById("myProjectsListingBarHint");
    let selectedName = "";
    if (id && typeof TawalaDemo !== "undefined" && TawalaDemo.getMyTawala) {
      const p = TawalaDemo.getMyTawala(id);
      selectedName = p
        ? TawalaDemo.displayName
          ? TawalaDemo.displayName(p.name)
          : String(p.name || id)
        : id;
    }

    bar.querySelectorAll("[data-wired]").forEach((el) => {
      const opId = el.dataset.op || "";
      if (el.dataset.alwaysEnabled === "true" || opId === "get-library") {
        el.dataset.project = "";
        if ("disabled" in el) el.disabled = false;
        el.title = "Pick a public Library project and Save a copy into My Tawala";
        return;
      }

      el.dataset.project = id;

      if (opId === "refresh-library" || el.dataset.wired === "pull-library") {
        const enabled = !!(id && link);
        el.disabled = !enabled;
        if (!id) {
          el.title = "Select a project that is linked to the Library to refresh";
        } else if (!link) {
          el.title =
            "This project is not linked to a public Library entry — use Get from Library… to acquire a copy";
        } else {
          el.title = `Update “${selectedName}” from its Library listing (“${link.name}”)`;
        }
        return;
      }

      if (opId === "make-copy" || el.dataset.wired === "make-copy-mytawala") {
        el.disabled = !id;
        el.title = id
          ? `Fork “${selectedName}” into a new My Tawala project (original untouched)`
          : "Select a project to Make a Copy";
        return;
      }

      if (opId === "delete" || el.dataset.wired === "delete-mytawala") {
        el.disabled = !id;
        el.title = id
          ? `Delete “${selectedName}” from My Tawala`
          : "Select a project to delete";
      }
    });

    if (hint) {
      if (!id) {
        hint.textContent =
          "Get from Library saves a copy into My Tawala · select a project to Make a Copy or Delete · select a linked project to Refresh";
      } else if (link) {
        hint.textContent = `Selected “${selectedName}” · linked to Library “${link.name}” — Make a Copy / Refresh / Delete available`;
      } else {
        hint.textContent = `Selected “${selectedName}” · not linked to Library — Make a Copy / Delete available; use Get from Library… to acquire`;
      }
    }
  }

  /** Header cells for listing action columns — label in bar, icon lives in each row. */
  function renderListingActionHeaders() {
    return LISTING_ACTIONS.map((op) => {
      return (
        `<th class="${listingColClasses(op)}" scope="col" data-op-col="${escapeHtml(op.id)}" ` +
        `data-col-group="${escapeHtml(op.group || "")}" ` +
        `title="${escapeHtml(op.title)}"><span class="th-label">${escapeHtml(op.label)}</span></th>`
      );
    }).join("");
  }

  /** Per-row icon cells (one &lt;td&gt; per listing action). Name click → Project Details. */
  function renderListingControlCells(projectId) {
    return LISTING_ACTIONS.map((op) => {
      const colClass = listingColClasses(op);
      if (op.wired === "use-project") {
        return `<td class="${colClass}">${renderUseAnchor(op, projectId, "listing-icon")}</td>`;
      }
      const active = isOpActive(op);
      const wired = active ? String(op.wired) : "false";
      const disabled = active ? "" : " disabled";
      const activeClass = active ? " is-active" : "";
      const dangerClass = op.destructive ? " is-destructive" : "";
      return (
        `<td class="${colClass}">` +
        `<button type="button" class="pm-icon-action pm-op-icon-btn${activeClass}${dangerClass}"${disabled} ` +
        `title="${escapeHtml(op.title)}" aria-label="${escapeHtml(op.label)}" ` +
        `data-op="${escapeHtml(op.id)}" data-project="${escapeHtml(projectId)}" ` +
        `data-confirm="${escapeHtml(op.confirmId || "")}" data-wired="${escapeHtml(wired)}">` +
        `${listingIconHtml(op.icon)}</button>` +
        `</td>`
      );
    }).join("");
  }

  /** @deprecated Prefer renderListingControlCells — kept for callers expecting a single controls cell. */
  function renderListingControls(projectId) {
    return (
      '<div class="controls pm-listing-controls">' +
      LISTING_ACTIONS.map((op) => {
        if (op.wired === "use-project") {
          return renderUseAnchor(op, projectId, "listing-inline");
        }
        const active = isOpActive(op);
        const wired = active ? String(op.wired) : "false";
        const disabled = active ? "" : " disabled";
        const activeClass = active ? " is-active" : "";
        const dangerClass = op.destructive ? " is-destructive" : "";
        return (
          `<button type="button" class="pm-icon-action pm-op-icon-btn${activeClass}${dangerClass}"${disabled} ` +
          `title="${escapeHtml(op.title)}" aria-label="${escapeHtml(op.label)}" ` +
          `data-op="${escapeHtml(op.id)}" data-project="${escapeHtml(projectId)}" ` +
          `data-confirm="${escapeHtml(op.confirmId || "")}" data-wired="${escapeHtml(wired)}">` +
          `${listingIconHtml(op.icon)}</button>`
        );
      }).join(" ") +
      "</div>"
    );
  }

  function disabledChip(op) {
    return (
      `<button type="button" class="pm-action" disabled title="${escapeHtml(op.title || op.label)}" ` +
      `data-op="${escapeHtml(op.id || op.label)}" data-wired="false">${escapeHtml(op.label)}</button>`
    );
  }

  /** Active or grey chip for Versions / similar section toolbars. */
  function sectionChip(op, projectId, opts) {
    const options = opts || {};
    if (op.wired === "deploy-version") {
      const enabled = !!options.deployVersionEnabled;
      if (!enabled) {
        return (
          `<button type="button" class="pm-action" disabled ` +
          `title="${escapeHtml(options.deployVersionTitle || op.title || op.label)}" ` +
          `data-op="${escapeHtml(op.id || op.label)}" data-wired="deploy-version" ` +
          `data-project="${escapeHtml(projectId || "")}">${escapeHtml(op.label)}</button>`
        );
      }
      return (
        `<button type="button" class="pm-action is-active" ` +
        `title="${escapeHtml(op.title || op.label)}" ` +
        `data-op="${escapeHtml(op.id || op.label)}" data-wired="deploy-version" ` +
        `data-project="${escapeHtml(projectId || "")}">${escapeHtml(op.label)}</button>`
      );
    }
    if (!isOpActive(op)) return disabledChip(op);
    return (
      `<button type="button" class="pm-action is-active" ` +
      `title="${escapeHtml(op.title || op.label)}" ` +
      `data-op="${escapeHtml(op.id || op.label)}" data-wired="${escapeHtml(String(op.wired))}" ` +
      `data-project="${escapeHtml(projectId || "")}">${escapeHtml(op.label)}</button>`
    );
  }

  function formatVersionDate(iso) {
    if (!iso) return "—";
    try {
      const dt = new Date(iso);
      if (Number.isNaN(dt.getTime())) return "—";
      return `${dt.getMonth() + 1}/${dt.getDate()}/${String(dt.getFullYear()).slice(-2)}`;
    } catch {
      return "—";
    }
  }

  function versionIsRedeployable(v) {
    if (
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.versionHasRedeployableDefinition === "function"
    ) {
      return TawalaTransfer.versionHasRedeployableDefinition(v);
    }
    return !!(v && (v.definition || v.snapshotId || v.hasDefinition));
  }

  /** Normalize overlay versions[] for Details display (newest first). */
  function projectVersionRows(project) {
    const raw = Array.isArray(project && project.versions) ? project.versions.slice() : [];
    if (!raw.length && project && (project.versionNumber != null || project.lastDeployAt)) {
      raw.push({
        versionNumber: project.versionNumber != null ? Number(project.versionNumber) : 1,
        description: project.versionDescription || "",
        at: project.lastDeployAt || project.updatedAt || null,
        uniqueId: project.uniqueId || null,
        mode: project.mode || null,
        startPoints: project.startPoints || [],
        deployed: !!project.deployed,
        snapshotId: null,
        definition: null,
        hasDefinition: false,
      });
    }
    raw.sort((a, b) => Number(b.versionNumber) - Number(a.versionNumber));
    return raw.filter((v) => v && Number.isFinite(Number(v.versionNumber)));
  }

  function getSelectedVersionRow(project, host) {
    const rows = projectVersionRows(project);
    const scope = host || document.getElementById("pmDetailHost") || document;
    const picked =
      (scope && scope.querySelector && scope.querySelector('input[name="pmVersionPick"]:checked')) ||
      document.querySelector('input[name="pmVersionPick"]:checked');
    if (picked && picked.value) {
      const n = Number(picked.value);
      const found = rows.find((v) => Number(v.versionNumber) === n);
      if (found) return found;
    }
    return rows[0] || null;
  }

  function deployVersionEnabledForSelection(project, version) {
    if (!project || !version) return false;
    const rows = projectVersionRows(project);
    const currentNum =
      project.versionNumber != null
        ? Number(project.versionNumber)
        : rows[0]
          ? Number(rows[0].versionNumber)
          : null;
    if (currentNum != null && Number(version.versionNumber) === currentNum) return false;
    return versionIsRedeployable(version);
  }

  function syncDeployVersionChip(root) {
    const host = root || document.getElementById("pmDetailHost") || document;
    const btn = host.querySelector && host.querySelector('[data-wired="deploy-version"]');
    if (!btn) return;
    const projectId = btn.dataset.project || "";
    const project =
      (projectId &&
        typeof TawalaDemo !== "undefined" &&
        TawalaDemo.getMyTawala &&
        TawalaDemo.getMyTawala(projectId)) ||
      null;
    const version = getSelectedVersionRow(project, host);
    const enabled = deployVersionEnabledForSelection(project, version);
    btn.disabled = !enabled;
    if (enabled) {
      btn.classList.add("is-active");
      btn.title = "Make this version the active deployed definition on :8080 (same uniqueId)";
    } else {
      btn.classList.remove("is-active");
      if (version && !versionIsRedeployable(version)) {
        btn.title = NO_VERSION_SNAPSHOT_MSG;
      } else if (version) {
        btn.title = "Already the current version — pick an older version to switch";
      } else {
        btn.title = "Select a non-current version that has a saved definition snapshot";
      }
    }
  }

  function renderVersionsSection(project) {
    const rows = projectVersionRows(project);
    const currentNum =
      project.versionNumber != null
        ? Number(project.versionNumber)
        : rows[0]
          ? Number(rows[0].versionNumber)
          : null;
    const defaultSelected =
      rows.find((v) => currentNum != null && Number(v.versionNumber) === currentNum) || rows[0] || null;
    const deployEnabled = deployVersionEnabledForSelection(project, defaultSelected);
    const chips =
      '<div class="pm-chip-row">' +
      VERSION_OPS.map((op) =>
        sectionChip(op, project.id, {
          deployVersionEnabled: deployEnabled,
          deployVersionTitle: deployEnabled
            ? undefined
            : defaultSelected && !versionIsRedeployable(defaultSelected)
              ? NO_VERSION_SNAPSHOT_MSG
              : "Select a non-current version that has a saved definition snapshot",
        })
      ).join("") +
      "</div>";
    if (!rows.length) {
      return (
        chips +
        '<p class="pm-hint">No versions yet — <b>Save a copy</b> / <b>Make a Copy</b> start at version <b>1</b>; ' +
        "open in Web Designer, Push (Deploy), optionally add a version note, then <b>Show in My Tawala</b> to mint the next. " +
        "Listing shows the current Version number only; full history appears here.</p>"
      );
    }
    const body = rows
      .map((v, idx) => {
        const num = Number(v.versionNumber);
        const isCurrent = currentNum != null ? num === currentNum : idx === 0;
        const isDeployed = v.deployed === true || isCurrent;
        const hasSnap = versionIsRedeployable(v);
        const statusBits = [];
        if (isCurrent) statusBits.push("Current");
        if (isDeployed) statusBits.push("Deployed");
        if (!hasSnap) statusBits.push("No snapshot");
        const status = statusBits.length ? statusBits.join(" · ") : "—";
        const descRaw = String(v.description || "").trim();
        return (
          `<tr class="${isCurrent ? "pm-version-current" : ""}" data-version-number="${escapeHtml(String(num))}" ` +
          `data-has-snapshot="${hasSnap ? "1" : "0"}">` +
          `<td class="pm-version-pick">` +
          `<input type="radio" name="pmVersionPick" value="${escapeHtml(String(num))}" ` +
          `${isCurrent ? "checked " : ""}` +
          `aria-label="Select version ${escapeHtml(String(num))}" />` +
          `</td>` +
          `<td class="pm-version-num">${escapeHtml(String(num))}</td>` +
          `<td class="pm-version-desc">` +
          `<input type="text" class="pm-version-desc-input" ` +
          `data-project="${escapeHtml(project.id || "")}" ` +
          `data-version-number="${escapeHtml(String(num))}" ` +
          `value="${escapeHtml(descRaw)}" ` +
          `placeholder="Add description…" ` +
          `aria-label="Description for version ${escapeHtml(String(num))} (editable)" ` +
          `title="Edit description — version number and current/deployed stay fixed" />` +
          `</td>` +
          `<td class="pm-version-date">${escapeHtml(formatVersionDate(v.at))}</td>` +
          `<td class="pm-version-status">${escapeHtml(status)}</td>` +
          `</tr>`
        );
      })
      .join("");
    return (
      chips +
      '<table class="pm-versions-table" aria-label="Project versions">' +
      "<thead><tr>" +
      "<th scope=\"col\"></th>" +
      "<th scope=\"col\">#</th>" +
      "<th scope=\"col\">Description</th>" +
      "<th scope=\"col\">Date</th>" +
      "<th scope=\"col\">Status</th>" +
      "</tr></thead>" +
      `<tbody>${body}</tbody></table>` +
      '<p class="pm-hint"><b>Push this version</b> switches the live :8080 definition to the selected row ' +
      "(same uniqueId; does not mint a new version). Needs a saved snapshot from " +
      "<b>Deploy → Show in My Tawala</b>. <b>Download</b> is still metadata-only. Delete version stays deferred. " +
      "Listing remains one row per project. Response data may not match an older schema — you’ll be asked to confirm.</p>"
    );
  }

  function saveVersionDescriptionFromInput(input) {
    if (!input || !input.classList || !input.classList.contains("pm-version-desc-input")) return;
    const projectId = input.dataset.project || "";
    const versionNumber = input.dataset.versionNumber || "";
    const next = String(input.value || "").trim();
    const prev = String(input.dataset.savedValue != null ? input.dataset.savedValue : input.defaultValue || "").trim();
    if (next === prev) {
      input.value = next;
      return;
    }
    if (
      typeof TawalaTransfer === "undefined" ||
      typeof TawalaTransfer.updateVersionDescription !== "function"
    ) {
      setStatus("Couldn’t save version description — transfer script not loaded.");
      window.alert("Couldn't save description\n\nTransfer support didn’t load. Refresh and try again.");
      input.value = prev;
      return;
    }
    const result = TawalaTransfer.updateVersionDescription(projectId, versionNumber, next);
    if (!result || !result.ok) {
      const err = (result && result.error) || "unknown";
      setStatus(`Couldn’t save version ${versionNumber} description (${err}).`);
      window.alert(
        `Couldn't save description for version ${versionNumber}\n\n` +
          (err === "no-overlay"
            ? "This project has no Deploy overlay in this browser yet. Deploy → Show in My Tawala first."
            : err === "version-not-found"
              ? "That version wasn’t found on the overlay."
              : `Error: ${err}`)
      );
      input.value = prev;
      return;
    }
    input.value = result.description || "";
    input.dataset.savedValue = result.description || "";
    input.defaultValue = result.description || "";
    setStatus(`Saved description for version ${result.versionNumber}.`);
  }

  function downloadSelectedVersion(projectId) {
    const project =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.getMyTawala && TawalaDemo.getMyTawala(projectId)) ||
      null;
    if (!project) {
      setStatus("Download version — project not found.");
      window.alert("Couldn't download version\n\nProject not found in My Tawala.");
      return;
    }
    const host = document.getElementById("pmDetail");
    const picked =
      (host && host.querySelector('input[name="pmVersionPick"]:checked')) ||
      document.querySelector('input[name="pmVersionPick"]:checked');
    const rows = projectVersionRows(project);
    let version = null;
    if (picked && picked.value) {
      const n = Number(picked.value);
      version = rows.find((v) => Number(v.versionNumber) === n) || null;
    }
    if (!version) version = rows[0] || null;
    if (!version) {
      setStatus("Download version — no versions on this project.");
      window.alert("Couldn't download version\n\nNo Deploy versions recorded yet.");
      return;
    }
    const payload = {
      kind: "tawala.project-version",
      format: 1,
      note: "Minimal metadata download. Push this version uses the saved definition snapshot, not this file.",
      projectId: project.id || projectId,
      projectName: project.name || null,
      versionNumber: Number(version.versionNumber),
      description: version.description || "",
      at: version.at || null,
      uniqueId: version.uniqueId || project.uniqueId || null,
      mode: version.mode || project.mode || null,
      startPoints: version.startPoints || project.startPoints || [],
      deployed: !!version.deployed,
      hasDefinition: versionIsRedeployable(version),
      snapshotId: version.snapshotId || null,
      downloadedAt: new Date().toISOString(),
    };
    const slug =
      (typeof TawalaTransfer !== "undefined" && TawalaTransfer.slugifyProjectId
        ? TawalaTransfer.slugifyProjectId(project.name || projectId)
        : String(projectId || "project")) || "project";
    const filename = `${slug}-v${payload.versionNumber}.version.json`;
    const blob = new Blob([JSON.stringify(payload, null, 2)], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    a.remove();
    setTimeout(() => URL.revokeObjectURL(url), 1000);
    setStatus(`Downloaded ${filename}`);
  }

  async function resolveDefinitionForVersion(version) {
    if (version && version.definition && typeof version.definition === "object" && version.definition.name) {
      return { ok: true, definition: version.definition, from: "overlay" };
    }
    const snapshotId = version && version.snapshotId ? String(version.snapshotId).trim() : "";
    if (!snapshotId) {
      return { ok: false, error: "no-snapshot", message: NO_VERSION_SNAPSHOT_MSG };
    }
    if (typeof TawalaDemo === "undefined" || typeof TawalaDemo.fetchVersionSnapshot !== "function") {
      return {
        ok: false,
        error: "no-api",
        message: "Couldn’t load definition snapshot — demo-urls support missing. Refresh and try again.",
      };
    }
    const snap = await TawalaDemo.fetchVersionSnapshot(snapshotId);
    if (!snap || snap.status === "failure" || !snap.project) {
      return {
        ok: false,
        error: "fetch-failed",
        message:
          "Couldn’t load the saved definition for this version.\n\n" +
          ((snap && snap.error) || "snapshot not found") +
          "\n\nIs designer-web API on :3001? Snapshots live under designer-web/.deployed/version-snapshots/.",
      };
    }
    return { ok: true, definition: snap.project, from: "api", snapshotId };
  }

  /**
   * Stamp My Tawala Theme / Appearance onto a Designer definition before Push / Redeploy.
   * Java/Tomcat CSS follows project (+ form) themePath in the uploaded XML.
   */
  function stampThemeOnDefinition(definition, themePath) {
    const path = String(themePath || "default").trim() || "default";
    if (!definition || typeof definition !== "object") return definition;
    let next;
    try {
      next = JSON.parse(JSON.stringify(definition));
    } catch {
      next = { ...definition };
    }
    next.themePath = path;
    if (Array.isArray(next.forms)) {
      next.forms = next.forms.map((f) =>
        f && typeof f === "object" ? { ...f, themePath: path } : f
      );
    }
    return next;
  }

  async function deploySelectedVersion(projectId) {
    const project =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.getMyTawala && TawalaDemo.getMyTawala(projectId)) ||
      null;
    if (!project) {
      setStatus("Push this version — project not found.");
      window.alert("Couldn't deploy version\n\nProject not found in My Tawala.");
      return;
    }
    const host = document.getElementById("pmDetailHost") || document;
    const version = getSelectedVersionRow(project, host);
    if (!version) {
      setStatus("Push this version — no version selected.");
      window.alert("Couldn't deploy version\n\nNo Deploy versions recorded yet.");
      return;
    }
    if (!deployVersionEnabledForSelection(project, version)) {
      if (!versionIsRedeployable(version)) {
        setStatus(`Version ${version.versionNumber}: no saved definition.`);
        window.alert("Couldn't deploy version\n\n" + NO_VERSION_SNAPSHOT_MSG);
      } else {
        setStatus(`Version ${version.versionNumber} is already current.`);
        window.alert(
          "Couldn't deploy version\n\nThat version is already current. Select an older version to switch."
        );
      }
      return;
    }

    const resolved = await resolveDefinitionForVersion(version);
    if (!resolved.ok) {
      setStatus(`Push this version failed — ${resolved.error || "no definition"}.`);
      window.alert("Couldn't deploy version\n\n" + (resolved.message || NO_VERSION_SNAPSHOT_MSG));
      return;
    }

    if (
      resolved.from === "api" &&
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.attachVersionDefinition === "function"
    ) {
      TawalaTransfer.attachVersionDefinition(
        projectId,
        version.versionNumber,
        resolved.definition,
        resolved.snapshotId || version.snapshotId
      );
    }

    const ok = window.confirm(
      `Push version ${version.versionNumber} as the live definition?\n\n` +
        "Responses collected under a newer form may not match this version. Continue?\n\n" +
        "This re-uploads the saved definition to :8080 (same project name / uniqueId when Java matches by name). " +
        "My Tawala Theme / Appearance is applied to this Push. " +
        "It does not mint a new Versions row."
    );
    if (!ok) {
      setStatus("Push this version cancelled.");
      return;
    }

    if (typeof TawalaDemo === "undefined" || typeof TawalaDemo.deployProjectDefinition !== "function") {
      setStatus("Push this version — Push API helper missing.");
      window.alert("Couldn't push version\n\nPush support didn’t load. Refresh and try again.");
      return;
    }

    const overlayTheme = resolveProjectThemePath(project);
    const stamped = stampThemeOnDefinition(resolved.definition, overlayTheme);

    setStatus(`Pushing version ${version.versionNumber} (theme ${overlayTheme})…`);
    const result = await TawalaDemo.deployProjectDefinition(stamped);
    if (!result || result.status === "failure") {
      const err = (result && result.error) || "unknown";
      setStatus(`Push this version failed: ${err}`);
      window.alert(`Couldn't deploy version ${version.versionNumber}\n\n${err}`);
      return;
    }

    if (
      typeof TawalaTransfer === "undefined" ||
      typeof TawalaTransfer.markVersionCurrentAndDeployed !== "function"
    ) {
      setStatus("Deploy succeeded on :8080, but My Tawala overlay wasn’t updated (transfer missing).");
      window.alert(
        `Deployed version ${version.versionNumber} to runtime, but couldn’t update My Tawala flags.\n\nRefresh and check start points.`
      );
      return;
    }

    const marked = TawalaTransfer.markVersionCurrentAndDeployed(projectId, version.versionNumber, result);
    if (!marked || !marked.ok) {
      setStatus(`Deploy OK, overlay update failed (${(marked && marked.error) || "unknown"}).`);
      window.alert(
        `Runtime deploy succeeded, but My Tawala version flags weren’t updated (${(marked && marked.error) || "unknown"}).`
      );
      return;
    }

    const refreshed =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.getMyTawala && TawalaDemo.getMyTawala(projectId)) ||
      null;
    const detailHost = document.getElementById("pmDetailHost");
    if (detailHost && refreshed) {
      detailHost.innerHTML = renderDetailPanel({ id: projectId, ...refreshed });
      syncDeployVersionChip(detailHost);
    }
    setStatus(
      `Deployed version ${version.versionNumber} — now current/deployed` +
        (result.uniqueId ? ` (uniqueId ${result.uniqueId})` : "") +
        "."
    );
    window.alert(
      `Version ${version.versionNumber} is now the deployed definition.\n\n` +
        (result.mode === "java" ? "Java :8080 updated. " : "Dev runtime updated. ") +
        "Start points on this page were refreshed. " +
        "If you collected responses under a newer form, fields may not line up — Export/Import can fail until you switch back or purge."
    );
  }

  function renderCollapsibleSection(id, title, innerHtml, open, opts) {
    const options = opts || {};
    const extraClass = options.greyed ? " is-stub-greyed" : "";
    const summaryTitle = options.summaryTitle
      ? ` title="${escapeHtml(options.summaryTitle)}"`
      : "";
    return (
      `<details class="pm-section${extraClass}" id="${escapeHtml(id)}"${open ? " open" : ""}>` +
      `<summary${summaryTitle}>${escapeHtml(title)}</summary>` +
      `<div class="pm-section-body">${innerHtml}</div>` +
      "</details>"
    );
  }

  function projectDisplayName(project) {
    if (!project) return "";
    if (window.TawalaDemo && window.TawalaDemo.displayName) {
      return window.TawalaDemo.displayName(project.name);
    }
    return String(project.name || "")
      .replace(/\.tawala\.xml$/i, "")
      .replace(/\.tawala$/i, "")
      .replace(/\.json$/i, "");
  }

  /** Ordered start points for Project Data (Exam/Registration first when multi-start). */
  function orderedStartPoints(project) {
    let startPoints = (project && project.startPoints) || [];
    if (typeof TawalaDemo !== "undefined" && typeof TawalaDemo.pickPrimaryStartPoint === "function") {
      const primary = TawalaDemo.pickPrimaryStartPoint(startPoints);
      if (primary && startPoints.length > 1) {
        startPoints = [primary].concat(startPoints.filter((s) => s !== primary));
      }
    }
    return startPoints.filter((s) => s && (s.url || s.label || s.form));
  }

  function startPointFormKey(sp) {
    return String((sp && (sp.form || sp.label)) || "Start");
  }

  /** Encode a website-mock-relative path for fetch (spaces in JSON filenames). */
  function encodeMockRelPath(relPath) {
    return String(relPath || "")
      .split("/")
      .map((seg) => encodeURIComponent(seg))
      .join("/");
  }

  /**
   * All form names for the Forms branch: catalog formNames ∪ definition ∪ json hydrate ∪ starts.
   * Non-start forms appear only after second expand (Forms [+]).
   */
  function collectFormNames(project, extraNames) {
    const seen = new Set();
    const out = [];
    function add(name) {
      const n = String(name || "").trim();
      if (!n || seen.has(n)) return;
      seen.add(n);
      out.push(n);
    }
    /* Catalog seed (e.g. Online Exam) — sync first paint without waiting on fetch. */
    const seeded = (project && project.formNames) || [];
    if (Array.isArray(seeded)) seeded.forEach(add);
    /* Overlay / Deploy may attach a forms array of names or {name} objects. */
    const topForms = (project && project.forms) || [];
    if (Array.isArray(topForms)) {
      topForms.forEach((f) => add(typeof f === "string" ? f : f && f.name));
    }
    const versions = (project && project.versions) || [];
    for (let i = 0; i < versions.length; i++) {
      const def = versions[i] && versions[i].definition;
      const forms = def && Array.isArray(def.forms) ? def.forms : [];
      forms.forEach((f) => add(f && f.name));
    }
    if (project && project.definition && Array.isArray(project.definition.forms)) {
      project.definition.forms.forEach((f) => add(f && f.name));
    }
    (extraNames || []).forEach(add);
    orderedStartPoints(project).forEach((sp) => add(startPointFormKey(sp)));
    return out;
  }

  /** Per-form count label: known success → number (0 ok); unknown/failed → "—". */
  function formCountLabel(countsState, formName) {
    if (!countsState || !countsState.known) return "—";
    const n = countsState.byForm && countsState.byForm[formName];
    return String(n == null ? 0 : n);
  }

  function startPointKeys(project) {
    return new Set(orderedStartPoints(project).map(startPointFormKey));
  }

  /** Map form key → first matching start point (idx + url + label). */
  function startInfoByForm(project) {
    const map = new Map();
    orderedStartPoints(project).forEach((sp, idx) => {
      const key = startPointFormKey(sp);
      if (!map.has(key)) {
        map.set(key, {
          idx,
          url: sp.url ? String(sp.url) : "",
          label: sp.label || sp.form || key,
        });
      }
    });
    return map;
  }

  /**
   * Unified form list: start points first (ordered), then remaining forms.
   * Each entry: { name, start: null | { idx, url, label } }.
   */
  function orderedFormEntries(project, formNames) {
    const startMap = startInfoByForm(project);
    const ordered = [];
    const seen = new Set();
    orderedStartPoints(project).forEach((sp) => {
      const key = startPointFormKey(sp);
      if (!key || seen.has(key)) return;
      seen.add(key);
      ordered.push({ name: key, start: startMap.get(key) || null });
    });
    (formNames || []).forEach((name) => {
      const n = String(name || "").trim();
      if (!n || seen.has(n)) return;
      seen.add(n);
      ordered.push({ name: n, start: startMap.get(n) || null });
    });
    return ordered;
  }

  function formStatCells(countsState, formName) {
    const count = formCountLabel(countsState, formName);
    const countTitle =
      count === "—"
        ? "Response count unavailable (needs :3001 + Postgres/Docker, or Deploy)"
        : `Records for form “${formName}”: ${count}`;
    /* Form rows: Records only — Times used / Last used are project-level (banner). */
    return (
      `<span class="pm-data-stat-val pm-data-stat-records" data-pm-form-count="1" title="${escapeHtml(
        countTitle
      )}">${escapeHtml(count)}</span>` +
      `<span class="pm-data-stat-val pm-data-stat-empty pm-data-stat-times" aria-hidden="true"></span>` +
      `<span class="pm-data-stat-val pm-data-stat-empty pm-data-stat-last" aria-hidden="true"></span>` +
      `<span class="pm-data-col-spacer" aria-hidden="true"></span>`
    );
  }

  /**
   * Form-row label: full name in the DOM; CSS ellipsis shortens only when the
   * name track is tight (minmax ~6ch last resort). title keeps the full string.
   */
  function formRowHtml(name, startInfo, countsState, opts) {
    const isStart = !!startInfo;
    const selected = !!(opts && opts.selected);
    const label = isStart && startInfo.label ? startInfo.label : name;
    const full = String(label || "");
    const urlAttr =
      isStart && startInfo.url ? ` data-pm-url="${escapeHtml(startInfo.url)}"` : "";
    const idxAttr = isStart ? ` data-pm-start-idx="${startInfo.idx}"` : "";
    const selClass = selected ? " is-selected" : "";
    const ariaSel = selected ? ` aria-selected="true"` : "";
    return (
      `<li class="pm-data-tree-row pm-data-tree-form${isStart ? " is-start" : ""}${selClass}" role="treeitem" tabindex="0" ` +
      `data-pm-sel="${isStart ? "start" : "form"}" data-pm-form="${escapeHtml(name)}" ` +
      `data-pm-is-start="${isStart ? "1" : "0"}"${idxAttr}${urlAttr}${ariaSel}>` +
      (isStart
        ? `<span class="pm-data-tree-play" title="Starting point" aria-label="Starting point">▶</span>`
        : `<span class="pm-data-tree-play-spacer" aria-hidden="true"></span>`) +
      `<span class="pm-data-tree-label" title="${escapeHtml(full)}" ` +
      `aria-label="${escapeHtml(full)}">${escapeHtml(full)}</span>` +
      formStatCells(countsState, name) +
      `</li>`
    );
  }

  /** Caret toggle (▸) — same family as .pm-section summary; CSS rotates when expanded. */
  function renderDataTreeToggle(title, expanded) {
    const open = !!expanded;
    return (
      `<button type="button" class="pm-data-tree-toggle${open ? " is-expanded" : ""}" data-pm-tree-toggle="1" ` +
      `aria-expanded="${open ? "true" : "false"}" ` +
      `title="${escapeHtml(title)}" ` +
      `aria-label="${escapeHtml(title)}">` +
      `<span class="pm-data-tree-toggle-glyph" aria-hidden="true">▸</span>` +
      `</button>`
    );
  }

  /** Project toggle titles for expand levels 0 / 1 / 2 (caret via CSS is-expanded). */
  function projectToggleChrome(level) {
    const n = Number(level) || 0;
    if (n <= 0) return { title: "Show starting points", expanded: false };
    if (n === 1) return { title: "Show all forms", expanded: true };
    return { title: "Collapse project data", expanded: true };
  }

  function renderProjectDataBannerControls(projectId) {
    const pid = escapeHtml(projectId || "");
    const project = resolveMyTawalaProject(projectId);
    const useOffTitle = projectHasUseRuntime(project)
      ? "Select a start point to Use"
      : USE_NEEDS_DEPLOY_TITLE;
    const copyOffTitle = projectHasUseRuntime(project)
      ? "Select a start point to copy its link"
      : "Deploy this project before Copy link — no :8080 start URL yet";
    const vrule = `<span class="pm-data-vrule" aria-hidden="true"></span>`;
    /* Nested in last shared grid track so form-row stats align with banner heads. */
    return (
      `<div class="pm-data-project-controls">` +
      vrule +
      `<div class="pm-data-ctrl-group pm-data-banner-controls" role="toolbar" aria-label="Project Data actions">` +
      `<a class="pm-data-ctrl pm-data-ctrl-use is-scope-disabled" href="#" ` +
      `data-op="use" data-wired="use-project" data-project="${pid}" ` +
      `title="${escapeHtml(useOffTitle)}" aria-disabled="true">Use</a>` +
      `<button type="button" class="pm-data-ctrl is-scope-disabled" data-op="copy-link" ` +
      `data-wired="copy-start-link" data-project="${pid}" aria-disabled="true" ` +
      `title="${escapeHtml(copyOffTitle)}">Copy link</button>` +
      `</div>` +
      vrule +
      `<div class="pm-data-ctrl-group">` +
      `<button type="button" class="pm-data-ctrl" data-op="export" data-wired="export-mytawala" ` +
      `data-project="${pid}" data-data-scope="project" title="Export project response data">Export</button>` +
      `<button type="button" class="pm-data-ctrl" data-op="import" data-wired="import-mytawala" ` +
      `data-project="${pid}" data-data-scope="project" title="Import response data into this project">Import</button>` +
      `</div>` +
      vrule +
      `<div class="pm-data-ctrl-group">` +
      `<button type="button" class="pm-data-ctrl is-destructive" data-op="purge" data-wired="purge-local" ` +
      `data-confirm="purge" data-project="${pid}" data-data-scope="project" title="Purge project data">Purge</button>` +
      `</div>` +
      `</div>`
    );
  }

  /**
   * Project Data — one unified form list (owner Aug 9):
   *   level 0 — project banner only
   *   level 1 — start points (muted ◀ cue) in the same list
   *   level 2 — all forms (starts still marked)
   * Banner row: name + stats headings + Use / Copy link | Export Import | Purge (nowrap).
   * Values row under that: project Records / Times used / Last used.
   * Full-height vrules: stats|controls, Copy|Export, Import|Purge.
   */
  function renderProjectDataTree(project) {
    const deployed = typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project);
    const displayName = projectDisplayName(project);
    const formNames = collectFormNames(project);
    const entries = orderedFormEntries(project, formNames);
    /* Listing Use → #pmSecData / ?openStarts=1: level 1 (starts). Plain Details stays 0. */
    const preferLevel = wantsOpenStartsFromLocation() ? 1 : 0;
    const firstStartEntry =
      preferLevel >= 1 ? entries.find((e) => e && e.start) || null : null;
    const selectFirstStart = !!(firstStartEntry && firstStartEntry.name);
    const projectChrome = projectToggleChrome(preferLevel);
    const formRows = entries
      .map((e) =>
        formRowHtml(e.name, e.start, null, {
          selected: selectFirstStart && e.name === firstStartEntry.name && !!e.start,
        })
      )
      .join("");
    const hasStarts = entries.some((e) => e.start);
    const scrollClass = entries.length > 10 ? " is-scrollable" : "";
    const selKind = selectFirstStart ? "start" : "project";
    const selKey = selectFirstStart ? firstStartEntry.name : "";
    const projectSelClass = selectFirstStart ? "" : " is-selected";
    const projectAria = selectFirstStart ? "" : ` aria-selected="true"`;
    const usage =
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.getUsageStats === "function"
        ? TawalaTransfer.getUsageStats(project.id)
        : { timesUsed: 0, lastUsed: null };
    const timesLabel = String(usage.timesUsed || 0);
    const lastLabel = usage.lastUsed || "—";
    const timesTitle =
      "Times used — mock count of My Tawala Use sessions that opened a start URL (not Test Drive; not live telemetry)";
    const lastTitle = usage.lastUsed
      ? `Last used — ${usage.lastUsed} (most recent My Tawala Use → :8080)`
      : "Last used — no My Tawala Use session recorded yet in this browser";

    const hint = deployed
      ? '<p class="pm-hint"><b>▸</b> expands starts, then all forms. ' +
        "<b>Start ▶</b> → <b>Use</b> (run for yourself on <code>:8080</code>) + banner Copy link; " +
        "<b>any form</b> → Export / Import / Purge; <b>project</b> (collapsed) → Backup / Restore. " +
        "<b>Deploy</b> (Details bar) = go live + share/embed for others — not the same as Use. " +
        "Use keeps data; Purge clears. Offline: select project/form → Purge (not Use). " +
        "Online Exam Builder can seed demo Records when <code>:3001</code>/Postgres is down — <b>Reseed demo Records</b> after Purge.</p>"
      : '<p class="pm-hint deploy-hint-quiet">No live start URL yet — Expand still lists labels. ' +
        "<b>Use</b> (run for yourself) stays grey until Designer push → <b>Show in My Tawala</b> " +
        "(UI still says Deploy) creates <code>:8080</code> starts. " +
        "<b>Deploy</b> on Details shares for others after that — not Use. " +
        "Export / Import / Purge need a live uniqueId too.</p>";

    return (
      `<div class="pm-data-tree" id="pmDataTree" role="tree" ` +
      `data-project-id="${escapeHtml(project.id || "")}" ` +
      `data-expand-level="${preferLevel}" ` +
      `data-project-open="${preferLevel >= 1 ? "1" : "0"}" ` +
      `data-forms-open="${preferLevel >= 2 ? "1" : "0"}" ` +
      `data-sel-kind="${escapeHtml(selKind)}" data-sel-key="${escapeHtml(selKey)}">` +
      /* One shared column grid for banner heads, banner values, and every form row. */
      `<div class="pm-data-grid${scrollClass}">` +
      `<div class="pm-data-project-block${projectSelClass}" role="treeitem" tabindex="0" ` +
      `data-pm-sel="project"${projectAria} id="pmDataStartPoints">` +
      /* Row 1 — name + column heads + command groups (vrules span both rows). */
      renderDataTreeToggle(projectChrome.title, projectChrome.expanded) +
      `<span class="pm-data-tree-label pm-data-tree-project-name" title="${escapeHtml(
        displayName
      )}">${escapeHtml(displayName)}</span>` +
      `<span class="pm-data-stat-head pm-data-stat-records">Records</span>` +
      `<span class="pm-data-stat-head pm-data-stat-times">Times used</span>` +
      `<span class="pm-data-stat-head pm-data-stat-last">Last used</span>` +
      renderProjectDataBannerControls(project.id) +
      /* Row 2 — project-level values under the heads (form rows: Records only). */
      `<span class="pm-data-col-spacer" aria-hidden="true"></span>` +
      `<span class="pm-data-col-spacer" aria-hidden="true"></span>` +
      `<span class="pm-data-stat-val pm-data-stat-records" id="pmDataProjectRecords" title="Project-wide Records (Responses)">—</span>` +
      `<span class="pm-data-stat-val pm-data-stat-times" id="pmDataProjectTimesUsed" title="${escapeHtml(
        timesTitle
      )}">${escapeHtml(timesLabel)}</span>` +
      `<span class="pm-data-stat-val pm-data-stat-last" id="pmDataProjectLastUsed" title="${escapeHtml(
        lastTitle
      )}">${escapeHtml(lastLabel)}</span>` +
      `</div>` +
      `<ul class="pm-data-tree-list pm-data-forms-list${preferLevel >= 1 ? "" : " is-collapsed"}" ` +
      `id="pmDataFormsList" role="group" aria-label="Forms">` +
      (formRows ||
        `<li class="pm-data-tree-empty">${
          hasStarts ? "No forms discovered yet." : "No start points or forms listed."
        }</li>`) +
      `</ul>` +
      `</div>` +
      `<p class="pm-demo-records-tools" id="pmDemoRecordsTools" hidden>` +
      `<span class="pm-demo-records-badge" title="Seeded demo counts (not live Postgres)">Demo data</span>` +
      `<button type="button" class="pm-demo-reseed" data-pm-reseed-demo="1" ` +
      `title="Restore seeded demo Records (project total 50) after Purge">Reseed demo Records</button>` +
      `</p>` +
      hint +
      "</div>"
    );
  }

  /** Select the first start-marked form (banner Use / Copy enablement). */
  function selectFirstStartRow(tree) {
    if (!tree) return false;
    const row =
      tree.querySelector('.pm-data-tree-form.is-start[data-pm-is-start="1"]') ||
      tree.querySelector('.pm-data-tree-form[data-pm-is-start="1"]');
    if (!row) return false;
    setDataTreeSelection(tree, "start", row);
    return true;
  }

  /**
   * Use → Details entry: expand to starts (level 1) and highlight the first start.
   * No-op when location is a plain Details open (double-click / no openStarts hash).
   */
  function applyOpenStartsEntry(tree) {
    if (!tree || !wantsOpenStartsFromLocation()) return false;
    setExpandLevel(tree, 1);
    return selectFirstStartRow(tree);
  }

  function readDataTreeSelection(tree) {
    if (!tree) return { kind: "project", key: "", formName: "", startIdx: -1, url: "" };
    const kind = tree.dataset.selKind || "project";
    const key = tree.dataset.selKey || "";
    const row =
      tree.querySelector(
        ".pm-data-tree-row.is-selected, .pm-data-project-block.is-selected, .pm-data-project-line.is-selected"
      ) || tree.querySelector("[data-pm-sel].is-selected");
    const formName =
      kind === "form" || kind === "start"
        ? (row && row.dataset.pmForm) || key || ""
        : "";
    return {
      kind,
      key,
      formName,
      startIdx: kind === "start" && row ? Number(row.dataset.pmStartIdx) : -1,
      url: kind === "start" && row && row.dataset.pmUrl ? row.dataset.pmUrl : "",
    };
  }

  function setDataTreeSelection(tree, kind, row) {
    if (!tree) return;
    tree.querySelectorAll(".is-selected[data-pm-sel]").forEach((el) => {
      el.classList.remove("is-selected");
      el.removeAttribute("aria-selected");
    });
    const target =
      row ||
      (kind === "project" ? tree.querySelector('[data-pm-sel="project"]') : null);
    if (target) {
      target.classList.add("is-selected");
      target.setAttribute("aria-selected", "true");
    }
    tree.dataset.selKind = kind || "project";
    if ((kind === "form" || kind === "start") && target) {
      tree.dataset.selKey = target.dataset.pmForm || "";
    } else {
      tree.dataset.selKey = "";
    }
    syncProjectActionsForDataSelection(tree);
  }

  /** Clamp/normalize expand level: 0 = closed, 1 = starts, 2 = all forms. */
  function normalizeExpandLevel(level) {
    const n = Number(level);
    if (!Number.isFinite(n) || n <= 0) return 0;
    if (n >= 2) return 2;
    return 1;
  }

  /**
   * Apply expand level to the tree (CSS data attrs + toggle chrome).
   * Keeps legacy data-project-open / data-forms-open in sync for CSS.
   */
  function setExpandLevel(tree, level) {
    if (!tree) return;
    const next = normalizeExpandLevel(level);
    tree.dataset.expandLevel = String(next);
    tree.dataset.projectOpen = next >= 1 ? "1" : "0";
    tree.dataset.formsOpen = next >= 2 ? "1" : "0";

    const list = tree.querySelector("#pmDataFormsList");
    if (list) list.classList.toggle("is-collapsed", next < 1);

    const projectChrome = projectToggleChrome(next);
    const projectLine = tree.querySelector('[data-pm-sel="project"]');
    const projectToggle = projectLine && projectLine.querySelector("[data-pm-tree-toggle]");
    if (projectToggle) {
      projectToggle.setAttribute("aria-expanded", projectChrome.expanded ? "true" : "false");
      projectToggle.classList.toggle("is-expanded", projectChrome.expanded);
      projectToggle.title = projectChrome.title;
      projectToggle.setAttribute("aria-label", projectChrome.title);
    }

    /* Collapsed (level 0) = project-level enablement; clear any form/start highlight. */
    const sel = readDataTreeSelection(tree);
    if (next <= 0 && sel.kind !== "project") {
      setDataTreeSelection(tree, "project");
    } else if (next === 1 && sel.kind === "form") {
      /* Level 1 hides non-start rows — drop a highlight that is no longer visible. */
      setDataTreeSelection(tree, "project");
    } else {
      syncProjectActionsForDataSelection(tree);
    }
  }

  /**
   * Selection that drives enablement. Expand level 0 (forms list hidden) always
   * behaves as project-level (owner rule A), even if a stale form key remains.
   */
  function effectiveDataSelection(tree) {
    const expandLevel = normalizeExpandLevel(tree && tree.dataset.expandLevel);
    if (expandLevel <= 0) {
      return { kind: "project", key: "", formName: "", startIdx: -1, url: "" };
    }
    return readDataTreeSelection(tree);
  }

  /** Project caret cycles 0 → 1 → 2 → 0. */
  function cycleProjectExpand(tree) {
    if (!tree) return;
    const cur = normalizeExpandLevel(tree.dataset.expandLevel);
    setExpandLevel(tree, cur >= 2 ? 0 : cur + 1);
  }

  /** @deprecated use setExpandLevel — kept name for any stray callers */
  function setProjectBranchOpen(tree, open) {
    setExpandLevel(tree, open ? 1 : 0);
  }

  /** @deprecated use setExpandLevel */
  function setFormsBranchOpen(tree, open) {
    setExpandLevel(tree, open ? 2 : Math.min(normalizeExpandLevel(tree && tree.dataset.expandLevel), 1));
  }

  function setCtrlEnabled(el, enabled, titleWhenOff) {
    if (!el) return;
    /* Soft-disable pm-data-ctrl / pm-action: keep hoverable so honest grey tooltips work.
     * Native disabled=true swallows title tooltips in Chromium. */
    const softDisable =
      el.classList.contains("pm-data-ctrl") || el.classList.contains("pm-action");
    if (enabled) {
      el.classList.remove("is-scope-disabled");
      el.removeAttribute("aria-disabled");
      if (el.tagName === "BUTTON") el.disabled = false;
    } else {
      el.classList.add("is-scope-disabled");
      el.setAttribute("aria-disabled", "true");
      if (el.tagName === "BUTTON") el.disabled = softDisable ? false : true;
      if (titleWhenOff) el.title = titleWhenOff;
    }
  }

  function applyDataScopeToCtrl(el, sel, op) {
    if (!el) return;
    if ((sel.kind === "form" || sel.kind === "start") && sel.formName) {
      el.dataset.dataScope = "form";
      el.dataset.formName = sel.formName;
      el.title =
        op === "export"
          ? `Export response data for form “${sel.formName}” only`
          : op === "import"
            ? `Import into form “${sel.formName}” only (other forms’ Records stay intact)`
            : `Purge response data for form “${sel.formName}” only`;
      /* Short visible cue — full form name stays in the title (toolbar width is tight). */
      if (op === "export" || op === "import" || op === "purge") {
        el.textContent = op === "export" ? "Export · form" : op === "import" ? "Import · form" : "Purge · form";
      }
    } else {
      el.dataset.dataScope = "project";
      delete el.dataset.formName;
      el.title =
        op === "export"
          ? "Export all project response data (forms list collapsed or project selected)"
          : op === "import"
            ? "Import whole-project response data (collapsed / project selected). Form-scoped export files still merge that form only — siblings stay intact. Expand and highlight a form to target it from the button."
            : "Purge all project response data (forms list collapsed or project selected)";
      if (op === "export" || op === "import" || op === "purge") {
        el.textContent = op === "export" ? "Export" : op === "import" ? "Import" : "Purge";
      }
    }
  }

  /**
   * Enable/disable Project Data banner + Details Backup/Restore/Publish from highlight.
   * Owner matrix A/B/C — see PROJECT_ACTIONS comment. Collapsed tree ⇒ rule A.
   */
  function syncProjectActionsForDataSelection(tree) {
    const detail = document.getElementById("pmDetail");
    const dataTree = tree || document.getElementById("pmDataTree");
    const sel = effectiveDataSelection(dataTree);
    const projectId =
      (detail && detail.dataset.projectId) || (dataTree && dataTree.dataset.projectId) || "";

    const projectSelected = sel.kind === "project";
    const startReady = sel.kind === "start" && !!sel.url;
    const formRowHighlighted = sel.kind === "form" || sel.kind === "start";
    /* A/B/C: Export / Import / Purge / Publish always on for project, start, or form. */
    const dataOpsEnabled =
      sel.kind === "project" || sel.kind === "form" || sel.kind === "start";
    /* A only: Backup / Restore when project-level (collapsed or project root highlighted). */
    const backupRestoreEnabled = projectSelected && !formRowHighlighted;

    /* Main Details bar: Rename + Make a Copy + Deploy always; Backup / Restore (A only); Publish always (A/B/C). */
    const bar = detail && detail.querySelector(".pm-detail-actions");
    if (bar) {
      bar.querySelectorAll("button.pm-action, a.pm-action").forEach((el) => {
        const op = el.dataset.op || "";
        const wired = el.dataset.wired || "";
        if (op === "rename" || wired === "rename-project") {
          setCtrlEnabled(el, true);
          el.title = "Rename this project in My Tawala (or double-click the title)";
          if ("disabled" in el) el.disabled = false;
          return;
        }
        if (op === "make-copy" || wired === "make-copy-mytawala") {
          setCtrlEnabled(el, true);
          el.title =
            "Fork this project into a new My Tawala identity (new name; original untouched). Not the same as Rename or Library Save a copy.";
          if ("disabled" in el) el.disabled = false;
          return;
        }
        if (op === "deploy" || wired === "deploy-share") {
          setCtrlEnabled(el, true);
          el.title =
            "Go live for participants — copy a form/start link or embed it in a web page (not Publish; not Designer Push)";
          if ("disabled" in el) el.disabled = false;
          return;
        }
        if (op === "publish" || wired === "publish-mytawala") {
          setCtrlEnabled(el, true);
          el.title =
            "Publish this project to the public Library (rename, then optionally replace a stub or outdated Library entry)";
          if ("disabled" in el) el.disabled = false;
          return;
        }
        if (
          op === "backup" ||
          op === "restore" ||
          wired === "backup-mytawala" ||
          wired === "restore-mytawala"
        ) {
          setCtrlEnabled(
            el,
            backupRestoreEnabled,
            formRowHighlighted
              ? "Backup / Restore are project-level — highlight the project (or collapse the form list)"
              : "Select the project in Project Data (or collapse the form list) to enable Backup / Restore"
          );
          if (backupRestoreEnabled) {
            el.title =
              op === "restore" || wired === "restore-mytawala"
                ? "Restore this project from a backup"
                : "Back up this project (definition + data + properties)";
          }
        }
      });
    }

    /* Project Data banner controls (groups live as project-block grid children). */
    const controls =
      (dataTree && dataTree.querySelector(".pm-data-project-block")) ||
      document.querySelector(".pm-data-project-block");
    if (!controls) return;

    controls.querySelectorAll("[data-wired]").forEach((el) => {
      const op = el.dataset.op || "";
      const wired = el.dataset.wired || "";

      if (wired === "use-project" || op === "use") {
        /* B only: start-point highlight with a real :8080 URL. Empty Save-a-copy rows stay honest. */
        const project = resolveMyTawalaProject(projectId);
        const hasRuntime = projectHasUseRuntime(project);
        if (startReady && hasRuntime) {
          setCtrlEnabled(el, true);
          if (el.tagName === "A") {
            el.href = sel.url;
            el.target = "_blank";
            el.rel = "noopener";
            el.dataset.useMode = "runtime";
          }
          el.title =
            "Use the highlighted start point on :8080 (needs Java runtime). Offline Purge: select project/form → Purge — not Use.";
          el.classList.add("is-active");
        } else {
          let offTitle = USE_NEEDS_DEPLOY_TITLE;
          if (hasRuntime) {
            offTitle =
              sel.kind === "form"
                ? "Use is only for start points (▶). For offline Purge, keep this form selected and click Purge — not Use."
                : "Select a start point (▶) to Use — or keep the project selected and click Purge for offline demo Records.";
          } else if (sel.kind === "start") {
            offTitle =
              "Deploy this project before Use — this start point has no :8080 URL yet (Save a copy starts empty)";
          }
          setCtrlEnabled(el, false, offTitle);
          if (el.tagName === "A") {
            el.href = "#";
            el.removeAttribute("target");
            el.removeAttribute("rel");
            el.dataset.useMode = "none";
          }
          el.classList.remove("is-active");
        }
        return;
      }

      if (wired === "copy-start-link" || op === "copy-link") {
        const project = resolveMyTawalaProject(projectId);
        const hasRuntime = projectHasUseRuntime(project);
        let offTitle = "Select a start point (▶) to copy its link";
        if (!hasRuntime) {
          offTitle =
            sel.kind === "start"
              ? "Deploy this project before Copy link — this start point has no :8080 URL yet"
              : "Deploy this project before Copy link — no :8080 start URL yet";
        } else if (sel.kind === "form") {
          offTitle = "Copy link is only for start points (▶) — highlight a starting form";
        }
        setCtrlEnabled(el, startReady && hasRuntime, offTitle);
        if (startReady && hasRuntime) {
          el.title = "Copy the highlighted start point’s :8080 URL (does not open the form; fine offline)";
        }
        return;
      }

      if (wired === "export-mytawala" || wired === "import-mytawala" || wired === "purge-local") {
        if (dataOpsEnabled) {
          setCtrlEnabled(el, true);
          applyDataScopeToCtrl(el, sel, op);
        } else {
          setCtrlEnabled(
            el,
            false,
            op === "purge"
              ? "Select the project or a form to Purge response data"
              : "Select the project (all data) or a form (that form only)"
          );
        }
      }
    });

    /* Keep projectId on banner controls if missing. */
    if (projectId) {
      controls.querySelectorAll("[data-project]").forEach((el) => {
        if (!el.dataset.project) el.dataset.project = projectId;
      });
    }
  }

  async function copyUrlToClipboard(url) {
    if (!url) {
      window.alert("This start point has no :8080 URL to copy yet.");
      return;
    }
    try {
      if (navigator.clipboard && navigator.clipboard.writeText) {
        await navigator.clipboard.writeText(url);
      } else {
        window.prompt("Copy this start link:", url);
        return;
      }
      setStatus("Copied start link.");
    } catch {
      window.prompt("Copy this start link:", url);
    }
  }

  /**
   * Library viral share — copy the same :8080 URL Test Drive opens.
   * Alert includes #14 honesty (shared uniqueId; wipe-on-start not leave).
   * ≠ My Tawala Deploy Copy link.
   */
  async function copyTestDriveLinkToClipboard(url) {
    if (!url) {
      window.alert("No Test Drive URL to copy yet — this project isn’t live on :8080.");
      return false;
    }
    const copied = honestyText(
      "copyAlert",
      "Link copied.\n\nThis is the shared Library demo URL (same uniqueId for every visitor). Answers clear when someone starts Test Drive from the Library, not when they close the tab."
    );
    const promptLabel = honestyText(
      "copyPromptLabel",
      "Copy this Test Drive link (shared Library demo — not a private copy):"
    );
    try {
      if (navigator.clipboard && navigator.clipboard.writeText) {
        await navigator.clipboard.writeText(url);
        window.alert(copied);
        setStatus("Copied Test Drive link (shared Library demo).");
        return true;
      }
    } catch {
      /* fall through to prompt */
    }
    window.prompt(promptLabel, url);
    return false;
  }

  /**
   * Library multi-start Test Drive / Copy picker — minimal hot-link list.
   * Test Drive CTA → each start name opens that start (purge + :8080).
   * Copy link CTA → each start name copies that start’s :8080 URL.
   * @param {string} projectId
   * @param {{intent?: "open"|"copy"}} opts
   */
  function openLibraryTestDrivePicker(projectId, opts) {
    const options = opts || {};
    const intent = options.intent === "copy" ? "copy" : "open";
    const project = resolveLibraryProject(projectId);
    if (!project) {
      window.alert(`Can't open Test Drive — unknown Library project: ${projectId || "(none)"}`);
      return;
    }
    const starts = libraryStartPointsWithUrls(project);
    if (!starts.length) {
      window.alert("No local test-drive URL yet — this project isn’t live on :8080.");
      return;
    }
    /* Single-start should not reach here; fall through to direct open/copy. */
    if (starts.length < 2) {
      const url = starts[0].url;
      if (intent === "copy") {
        void copyTestDriveLinkToClipboard(url);
      } else {
        noteLibraryTestDriveOpen(projectId);
        if (typeof TawalaDemo !== "undefined" && typeof TawalaDemo.openTestDrive === "function") {
          void TawalaDemo.openTestDrive(url, { purge: true });
        } else {
          window.open(url, "_blank", "noopener");
        }
      }
      return;
    }

    closeTestDrivePickModal();
    closeDeployShareModal();

    const displayName =
      typeof TawalaDemo !== "undefined" && typeof TawalaDemo.displayName === "function"
        ? TawalaDemo.displayName(project.name || projectId)
        : String(project.name || projectId);
    const isCopy = intent === "copy";
    const title = isCopy ? "Copy link — choose a start" : "Test Drive — choose a start";
    const safeName = escapeHtml(displayName);
    const lede = isCopy
      ? honestyNamed(
          "pickerCopyLede",
          safeName,
          `“${safeName}” has more than one start form. Click a name to copy its Test Drive URL. Same shared Library demo — not a private copy.`
        )
      : honestyNamed(
          "pickerOpenLede",
          safeName,
          `“${safeName}” has more than one start form. Click a name to open it. Demo answers clear when you start (not when you close the tab). Use every start during this drive.`
        );
    const linkTitle = isCopy
      ? honestyText(
          "pickerCopyLinkTitle",
          "Copy this start’s URL (shared Library demo, not a private copy)."
        )
      : honestyText(
          "pickerOpenLinkTitle",
          "Open this start. Clears demo answers on start, not when you close the tab."
        );
    const pickerHint = honestyText(
      "pickerHint",
      "No account needed. Closing the Test Drive tab does not wipe answers."
    );

    const linksHtml = starts
      .map((sp, i) => {
        const label = sp.label || sp.form || `Start ${i + 1}`;
        return (
          `<li><a href="#" class="testdrive-pick-link" data-start-idx="${i}" ` +
          `title="${escapeHtml(linkTitle)}">${escapeHtml(label)}</a></li>`
        );
      })
      .join("");

    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = TESTDRIVE_PICK_MODAL_ID;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish tawala-modal--testdrive-pick" role="dialog" ' +
      'aria-modal="true" aria-labelledby="testDrivePickModalTitle">' +
      `<h3 id="testDrivePickModalTitle">${title}</h3>` +
      `<p class="pm-hint tawala-modal-lede">${lede}</p>` +
      '<div class="tawala-modal-body">' +
      `<ul class="testdrive-pick-list" role="list">${linksHtml}</ul>` +
      `<p class="pm-hint tawala-modal-hint-tight">${escapeHtml(pickerHint)}</p>` +
      "</div>" +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="testDrivePickClose">Close</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);
    document.addEventListener("keydown", handleTestDrivePickModalKeydown, true);

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closeTestDrivePickModal();
    });
    const closeBtn = backdrop.querySelector("#testDrivePickClose");
    if (closeBtn) closeBtn.addEventListener("click", closeTestDrivePickModal);

    const listEl = backdrop.querySelector(".testdrive-pick-list");
    if (listEl) {
      listEl.addEventListener("click", (ev) => {
        const a = ev.target && ev.target.closest ? ev.target.closest("a.testdrive-pick-link") : null;
        if (!a || !listEl.contains(a)) return;
        ev.preventDefault();
        const idx = Number(a.getAttribute("data-start-idx"));
        const sp = starts[idx] || starts[0];
        const url = (sp && sp.url) || "";
        if (!url) {
          window.alert("No start URL for that selection.");
          return;
        }
        if (isCopy) {
          closeTestDrivePickModal();
          void copyTestDriveLinkToClipboard(url);
          return;
        }
        closeTestDrivePickModal();
        noteLibraryTestDriveOpen(projectId);
        if (typeof TawalaDemo !== "undefined" && typeof TawalaDemo.openTestDrive === "function") {
          void TawalaDemo.openTestDrive(url, { purge: true });
        } else {
          window.open(url, "_blank", "noopener");
        }
      });
    }

    const firstLink = backdrop.querySelector("a.testdrive-pick-link");
    if (firstLink) firstLink.focus();
  }

  async function copyLibraryTestDriveLink(projectId, explicitUrl) {
    const project = projectId ? resolveLibraryProject(projectId) : null;
    if (project && isLibraryMultiStart(project)) {
      openLibraryTestDrivePicker(projectId, { intent: "copy" });
      return false;
    }
    let url = explicitUrl || "";
    if (!url && project) {
      url = libraryDriveUrl(project) || "";
    }
    return copyTestDriveLinkToClipboard(url);
  }

  async function copyStartLinkFromTree(tree, startIdx) {
    const row = tree.querySelector(
      `.pm-data-tree-form[data-pm-start-idx="${startIdx}"], .pm-data-tree-start[data-pm-start-idx="${startIdx}"]`
    );
    await copyUrlToClipboard(row && row.dataset.pmUrl);
  }

  function rebuildFormsList(tree, formNames, project, countsState) {
    const list = tree.querySelector("#pmDataFormsList");
    if (!list) return;
    const grid = tree.querySelector(".pm-data-grid");
    const entries = orderedFormEntries(project, formNames);
    if (!entries.length) {
      list.innerHTML = '<li class="pm-data-tree-empty">No forms discovered yet.</li>';
      if (grid) grid.classList.remove("is-scrollable");
    } else {
      list.innerHTML = entries.map((e) => formRowHtml(e.name, e.start, countsState)).join("");
      if (grid) grid.classList.toggle("is-scrollable", entries.length > 10);
    }
  }

  function setProjectRecordsDisplay(scope, countsState) {
    const treeCount =
      (scope && scope.querySelector && scope.querySelector("#pmDataProjectRecords")) ||
      document.getElementById("pmDataProjectRecords");
    const tools =
      (scope && scope.querySelector && scope.querySelector("#pmDemoRecordsTools")) ||
      document.getElementById("pmDemoRecordsTools");
    const known = !!(countsState && countsState.known);
    /* Prefer numeral 0 after Purge of demo data — em-dash only when unknown. */
    const label = known ? String(countsState.total) : "—";
    const title = known
      ? `Records (Responses): ${countsState.total}` +
        (countsState.source === "mock-seed"
          ? " · demo data (live counts unavailable)"
          : countsState.source
            ? ` · ${countsState.source}`
            : "")
      : countsState && countsState.error
        ? `Records unavailable: ${countsState.error}`
        : "Records (Responses) — count unavailable (needs :3001 + Postgres/Docker, or a uniqueId)";
    if (treeCount) {
      treeCount.textContent = label;
      treeCount.title = title;
    }
    if (tools) {
      const showDemo = !!(known && countsState.source === "mock-seed");
      tools.hidden = !showDemo;
    }
  }

  /**
   * Project Data banner — Times used / Last used (Task #13 mock).
   * Records stay separate (submissions). Copies downloaded (`cloneCount`) is Library-only.
   */
  function setUsageStatsDisplay(scope, stats) {
    const root = scope && scope.querySelector ? scope : document;
    const timesEl =
      (root.querySelector && root.querySelector("#pmDataProjectTimesUsed")) ||
      document.getElementById("pmDataProjectTimesUsed");
    const lastEl =
      (root.querySelector && root.querySelector("#pmDataProjectLastUsed")) ||
      document.getElementById("pmDataProjectLastUsed");
    const timesUsed = stats && Number(stats.timesUsed) > 0 ? Math.floor(Number(stats.timesUsed)) : 0;
    const lastUsed = (stats && stats.lastUsed) || null;
    if (timesEl) {
      timesEl.textContent = String(timesUsed);
      timesEl.title =
        "Times used — mock count of My Tawala Use sessions that opened a start URL (not Test Drive; not live telemetry)" +
        (timesUsed ? `: ${timesUsed}` : "");
      timesEl.classList.remove("pm-data-stat-placeholder");
    }
    if (lastEl) {
      lastEl.textContent = lastUsed || "—";
      lastEl.title = lastUsed
        ? `Last used — ${lastUsed} (most recent My Tawala Use → :8080)`
        : "Last used — no My Tawala Use session recorded yet in this browser";
      lastEl.classList.toggle("pm-data-stat-placeholder", !lastUsed);
    }
  }

  /** Restore Online Exam (etc.) demo Records after Purge — no DevTools needed. */
  async function reseedDemoRecords(tree) {
    const projectId = tree && tree.dataset.projectId;
    const uniqueId =
      typeof TawalaDemo !== "undefined" && typeof TawalaDemo.resolvePurgeUniqueId === "function"
        ? TawalaDemo.resolvePurgeUniqueId(projectId)
        : null;
    if (
      !uniqueId ||
      typeof TawalaDemo === "undefined" ||
      typeof TawalaDemo.reseedMockResponseCounts !== "function"
    ) {
      window.alert("Couldn't reseed demo Records — project has no demo seed.");
      return;
    }
    const result = TawalaDemo.reseedMockResponseCounts(uniqueId);
    if (!result || result.status !== "success") {
      window.alert(
        `Couldn't reseed demo Records\n\n${(result && result.error) || "unknown error"}`
      );
      return;
    }
    setStatus(`Reseeded demo Records — ${result.count} row(s).`);
    await hydrateProjectDataTree(tree.closest(".pm-section") || document);
  }

  /**
   * Enrich Forms branch from catalog jsonFile / export, and fill Records counts.
   * Safe no-op when offline / undeployed — form names still come from catalog formNames.
   */
  async function hydrateProjectDataTree(root) {
    const scope = root || document;
    const tree = scope.querySelector("#pmDataTree");
    if (!tree) return;
    const projectId = tree.dataset.projectId;
    const project = resolveMyTawalaProject(projectId);
    if (!project) return;
    const extras = [];
    let countsState = null;

    const jsonFile = project.jsonFile ? String(project.jsonFile) : "";
    if (jsonFile && !jsonFile.includes("..") && !jsonFile.startsWith("/")) {
      /* Only paths the mock static server can serve (projects/… under website-mock). */
      if (jsonFile.startsWith("projects/")) {
        try {
          const res = await fetch(encodeMockRelPath(jsonFile));
          if (res.ok) {
            const def = await res.json();
            (def.forms || []).forEach((f) => {
              if (f && f.name) extras.push(f.name);
            });
          }
        } catch {
          /* ignore */
        }
      }
    }

    const uniqueId =
      typeof TawalaDemo !== "undefined" && typeof TawalaDemo.resolvePurgeUniqueId === "function"
        ? TawalaDemo.resolvePurgeUniqueId(projectId)
        : null;
    if (uniqueId && typeof TawalaDemo.countResponses === "function") {
      try {
        const result = await TawalaDemo.countResponses(uniqueId);
        if (result && result.status === "success") {
          Object.keys(result.byForm || {}).forEach((n) => extras.push(n));
          countsState = {
            known: true,
            total: result.count,
            byForm: result.byForm || {},
            source: result.source || "",
          };
        } else {
          countsState = {
            known: false,
            error: (result && result.error) || "count failed",
          };
        }
      } catch (e) {
        countsState = { known: false, error: String((e && e.message) || e) };
      }
    } else if (!uniqueId) {
      countsState = { known: false, error: "no uniqueId — Deploy first" };
    }

    const formNames = collectFormNames(project, extras);
    rebuildFormsList(tree, formNames, project, countsState);
    setProjectRecordsDisplay(scope, countsState);
    if (
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.getUsageStats === "function"
    ) {
      setUsageStatsDisplay(scope, TawalaTransfer.getUsageStats(projectId));
    }

    /* Preserve selection highlight after rebuild. */
    const sel = readDataTreeSelection(tree);
    if ((sel.kind === "form" || sel.kind === "start") && sel.formName) {
      const rows = tree.querySelectorAll(".pm-data-tree-form");
      let restored = false;
      for (let i = 0; i < rows.length; i++) {
        if (rows[i].dataset.pmForm === sel.formName) {
          setDataTreeSelection(tree, rows[i].dataset.pmSel || sel.kind, rows[i]);
          restored = true;
          break;
        }
      }
      if (!restored) {
        /* Use entry: prefer first start over falling back to grey Use/Copy. */
        if (!applyOpenStartsEntry(tree)) syncProjectActionsForDataSelection(tree);
      }
    } else if (wantsOpenStartsFromLocation()) {
      applyOpenStartsEntry(tree);
    } else {
      syncProjectActionsForDataSelection(tree);
    }
  }

  function handleDataTreeClick(ev) {
    const tree = ev.target.closest("#pmDataTree");
    if (!tree) return;

    /* Banner controls must not steal / reset the form highlight. */
    if (ev.target.closest(".pm-data-ctrl-group, .pm-data-banner-controls")) {
      return;
    }

    const reseedBtn = ev.target.closest("[data-pm-reseed-demo]");
    if (reseedBtn) {
      ev.preventDefault();
      ev.stopPropagation();
      void reseedDemoRecords(tree);
      return;
    }

    const copyBtn = ev.target.closest("[data-pm-copy-start]");
    if (copyBtn) {
      ev.preventDefault();
      ev.stopPropagation();
      void copyStartLinkFromTree(tree, copyBtn.getAttribute("data-pm-copy-start"));
      return;
    }

    const toggle = ev.target.closest("[data-pm-tree-toggle]");
    if (toggle) {
      ev.preventDefault();
      ev.stopPropagation();
      cycleProjectExpand(tree);
      return;
    }

    const row = ev.target.closest("[data-pm-sel]");
    if (!row || !tree.contains(row)) return;
    const kind = row.dataset.pmSel || "project";
    setDataTreeSelection(tree, kind, row);
  }

  /** Mock account label for Author rail (session user, else body data-tawala-user, else "dev"). */
  function projectAuthorLabel(project) {
    if (project && project.author) return String(project.author);
    if (
      typeof window.TawalaChrome !== "undefined" &&
      typeof window.TawalaChrome.currentUser === "function"
    ) {
      const sessionUser = window.TawalaChrome.currentUser();
      if (sessionUser) return sessionUser;
    }
    if (typeof document !== "undefined" && document.body) {
      const u = document.body.getAttribute("data-tawala-user");
      if (u) return u;
    }
    return "dev";
  }

  /**
   * Library entry this My Tawala project is published as (or catalog twin by id).
   * Returns null when not in the public Library.
   */
  function resolvePublishedLibraryLink(project) {
    if (!project || typeof TawalaDemo === "undefined") return null;
    const publishedId = project.publishedToLibraryId ? String(project.publishedToLibraryId) : "";
    if (publishedId && typeof TawalaDemo.getLibrary === "function") {
      const lib = TawalaDemo.getLibrary(publishedId);
      if (lib) {
        let versionLabel = "";
        if (project.publishedToLibraryVersion != null && Number.isFinite(Number(project.publishedToLibraryVersion))) {
          versionLabel = String(Math.floor(Number(project.publishedToLibraryVersion)));
        } else if (
          typeof TawalaTransfer !== "undefined" &&
          typeof TawalaTransfer.formatCurrentVersion === "function"
        ) {
          const v = TawalaTransfer.formatCurrentVersion(lib);
          if (v && v !== "—") versionLabel = v;
        } else if (lib.versionNumber != null && Number.isFinite(Number(lib.versionNumber))) {
          versionLabel = String(Math.floor(Number(lib.versionNumber)));
        }
        return {
          id: publishedId,
          name: project.publishedToLibraryName || lib.name || publishedId,
          inactive: lib.inactive === true || lib.libraryActive === false,
          version: versionLabel || null,
        };
      }
    }
    if (project.id && typeof TawalaDemo.getLibrary === "function") {
      const twin = TawalaDemo.getLibrary(project.id);
      if (twin) {
        let versionLabel = "";
        if (
          typeof TawalaTransfer !== "undefined" &&
          typeof TawalaTransfer.formatCurrentVersion === "function"
        ) {
          const v = TawalaTransfer.formatCurrentVersion(twin);
          if (v && v !== "—") versionLabel = v;
        } else if (twin.versionNumber != null && Number.isFinite(Number(twin.versionNumber))) {
          versionLabel = String(Math.floor(Number(twin.versionNumber)));
        }
        return {
          id: project.id,
          name: twin.name || project.name,
          inactive: twin.inactive === true || twin.libraryActive === false,
          version: versionLabel || null,
        };
      }
    }
    return null;
  }

  function themeLabelForPath(themePath) {
    const path = String(themePath || "").trim();
    if (!path) return "Default";
    const hit = PROJECT_THEMES.find((t) => t.path === path);
    return hit ? hit.label : path;
  }

  /**
   * Prefer the project's recommended / stored theme (overlay, catalog, definition fields).
   * "Default" stays in the dropdown, but is only the selection when no project theme is present.
   */
  function resolveProjectThemePath(project) {
    if (!project || typeof project !== "object") return "default";
    const candidates = [
      project.themePath,
      project.theme,
      project.themeId,
      project.recommendedTheme,
      project.themeName,
    ];
    for (let i = 0; i < candidates.length; i++) {
      const raw = candidates[i];
      if (raw == null) continue;
      const s = String(raw).trim();
      if (!s) continue;
      const lower = s.toLowerCase();
      const byPath = PROJECT_THEMES.find((t) => t.path === s || t.path === lower);
      if (byPath) return byPath.path;
      const byLabel = PROJECT_THEMES.find((t) => t.label.toLowerCase() === lower);
      if (byLabel) return byLabel.path;
      return s;
    }
    return "default";
  }

  function themeOptionsHtml(selectedPath) {
    const sel = resolveProjectThemePath({ themePath: selectedPath });
    const known = PROJECT_THEMES.some((t) => t.path === sel);
    let html = PROJECT_THEMES.map((t) => {
      const selected = t.path === sel ? " selected" : "";
      return `<option value="${escapeHtml(t.path)}"${selected}>${escapeHtml(t.label)}</option>`;
    }).join("");
    if (!known) {
      html =
        `<option value="${escapeHtml(sel)}" selected>${escapeHtml(sel)} (custom)</option>` + html;
    }
    return html;
  }

  /**
   * If catalog/overlay lacks themePath, recover from Library twin, version snapshot,
   * or project JSON definition and select that theme.
   * Does not overwrite an explicit overlay themePath the user already chose.
   */
  async function hydrateThemeFromProjectJson(detailRoot, project) {
    if (!detailRoot || !project) return;
    const sel = detailRoot.querySelector('select[data-wired="theme-select"]');
    if (!sel) return;
    const overlayHasTheme =
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.getOverlayEntry === "function" &&
      (() => {
        const ov = TawalaTransfer.getOverlayEntry(project.id);
        return !!(ov && (ov.themePath || ov.theme || ov.themeId));
      })();
    if (overlayHasTheme) return;

    const current = resolveProjectThemePath(project);
    /* Catalog already has a non-default theme — leave it. */
    if (current && current !== "default") return;

    let fromSource = "";

    /* Library acquire / Refresh twin — catalog themePath (e.g. Horses → style2). */
    const libId = project.pulledFromLibraryId || project.publishedToLibraryId || "";
    if (
      libId &&
      typeof TawalaDemo !== "undefined" &&
      typeof TawalaDemo.getLibrary === "function"
    ) {
      const lib = TawalaDemo.getLibrary(libId);
      const fromLib = resolveProjectThemePath(lib);
      if (fromLib && fromLib !== "default") fromSource = fromLib;
    }

    /* Current Deploy version snapshot definition. */
    if (!fromSource && Array.isArray(project.versions)) {
      const currentVer =
        project.versions.find((v) => v && v.deployed) ||
        project.versions.find((v) => v && v.definition) ||
        null;
      if (currentVer && currentVer.definition) {
        const fromDef = resolveProjectThemePath(currentVer.definition);
        if (fromDef && fromDef !== "default") fromSource = fromDef;
      }
    }

    /* Catalog / disk project JSON. */
    if (!fromSource && project.jsonFile) {
      try {
        const res = await fetch(String(project.jsonFile), { cache: "no-store" });
        if (res.ok) {
          const def = await res.json();
          const fromJson = resolveProjectThemePath(def);
          if (fromJson && fromJson !== current) fromSource = fromJson;
        }
      } catch {
        /* ignore — mock may be offline of the jsonFile */
      }
    }

    if (!fromSource || fromSource === current) return;

    let hasOpt = false;
    for (let oi = 0; oi < sel.options.length; oi++) {
      if (sel.options[oi].value === fromSource) {
        hasOpt = true;
        break;
      }
    }
    if (!hasOpt) {
      const opt = document.createElement("option");
      opt.value = fromSource;
      opt.textContent = `${fromSource} (custom)`;
      sel.insertBefore(opt, sel.firstChild);
    }
    sel.value = fromSource;
    const rail = detailRoot.querySelector(".pm-identity-rail");
    if (rail) rail.setAttribute("data-theme-path", fromSource);
    /* Persist onto overlay so later paints keep the definition theme without re-fetch. */
    if (
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.upsertMyTawalaProperties === "function"
    ) {
      TawalaTransfer.upsertMyTawalaProperties(project.id, { themePath: fromSource });
    }
  }

  /**
   * Details identity / ops rail (Task List 5–7, 11–12): Published, author, version #,
   * Active/De-activate, Theme / Appearance, Invite / Include (→ Deploy share), Edit in Designer.
   * Project shortDescription (blurb) is NOT here — only under the main-column title.
   */
  function renderSidebarProjectOps(projectId) {
    const pid = escapeHtml(projectId || "");
    return (
      '<div class="pm-sidebar-ops pm-sidebar-stack" aria-label="Project share options">' +
      PROJECT_SIDEBAR_OPS.map((op) => {
        const active = isOpActive(op);
        return (
          `<button type="button" class="pm-action${active ? " is-active" : ""}"${active ? "" : " disabled"} ` +
          `title="${escapeHtml(op.title)}" data-op="${escapeHtml(op.label)}" ` +
          `data-project="${pid}" data-wired="${escapeHtml(active ? String(op.wired) : "false")}">` +
          `${escapeHtml(op.label)}</button>`
        );
      }).join("") +
      "</div>"
    );
  }

  function renderIdentityRail(project) {
    const pid = escapeHtml(project.id || "");
    const author = escapeHtml(projectAuthorLabel(project));
    const versionNum = project.versionNumber != null ? String(project.versionNumber) : "—";
    /* Version rail = structural Deploy note only. Never fall back to shortDescription
     * (marketing blurb) — that lives once under the main title (Task #7 / Aug 9). */
    const versionDesc = project.versionDescription
      ? String(project.versionDescription).trim()
      : "";
    const published = resolvePublishedLibraryLink(project);
    const inactive = project.inactive === true || project.libraryActive === false;
    const themePath = resolveProjectThemePath(project);

    let publishedHtml;
    if (published) {
      const libHref = `library.html?highlight=${encodeURIComponent(published.id)}`;
      const offlineNote = published.inactive
        ? ' <span class="pm-identity-muted">(hidden from Library while Offline)</span>'
        : "";
      /* Indicator only — Library name / theme label is not repeated here. Version = what's live in Library. */
      publishedHtml =
        `<a href="${escapeHtml(libHref)}" title="Open the public Library entry">` +
        `<b>Yes</b></a>` +
        (published.version
          ? ` <span class="pm-identity-muted">(Library v${escapeHtml(String(published.version))})</span>`
          : "") +
        offlineNote;
    } else {
      publishedHtml = `<span class="pm-identity-muted">No</span>`;
    }

    const statusHtml = inactive
      ? `<span class="pm-status-badge is-offline" title="Hidden from public Library; still on My Tawala">Offline</span>` +
        `<button type="button" class="pm-action pm-identity-action" data-wired="activate-project" ` +
        `data-project="${pid}" title="Show again in the public Library (if published)">Activate</button>`
      : `<span class="pm-status-badge is-active" title="Visible in public Library when published">Active</span>` +
        `<button type="button" class="pm-action pm-identity-action" data-wired="deactivate-project" ` +
        `data-project="${pid}" title="Hide from public Library; keep on My Tawala with Offline marker">De-activate</button>`;

    let sourceHtml = "";
    if (project.fromLibraryAcquire || project.sourcePile === "library-acquire" || project.pulledFromLibraryId) {
      const libId = project.pulledFromLibraryId ? String(project.pulledFromLibraryId) : "";
      const libName = librarySourceDisplayName(project) || libId || "Library";
      const libHref = libId
        ? `library.html?highlight=${encodeURIComponent(libId)}`
        : "library.html";
      /* Quiet secondary — “from … in Library” lives in this rail only. */
      sourceHtml =
        `<div class="pm-identity-row"><dt>Source</dt>` +
        `<dd class="pm-identity-source">` +
        `from <a href="${escapeHtml(libHref)}">${escapeHtml(libName)}</a> in Library` +
        `</dd></div>`;
    } else {
      const designerName = designerSourceDisplayName(project);
      if (designerName) {
        /* Quiet secondary — parallel to Library provenance; no link / bold / underline. */
        sourceHtml =
          `<div class="pm-identity-row"><dt>Source</dt>` +
          `<dd class="pm-identity-source">from Designer: ${escapeHtml(designerName)}</dd></div>`;
      }
    }

    /* Version number sits on the same line as the label; description (if any) is a
     * separate unindented line below — not “Version / hanging — — Build …” */
    const versionLabel =
      `Version <b class="pm-version-num">${escapeHtml(versionNum)}</b>`;
    const versionDd = versionDesc
      ? `<dd class="pm-version-desc">${escapeHtml(versionDesc)}</dd>`
      : `<dd class="pm-version-desc pm-version-desc-empty"></dd>`;

    const themeTitle =
      `Theme: ${themeLabelForPath(themePath)} (${themePath}). ` +
      "Changing Theme saves on My Tawala and Pushes CSS to :8080 when a live definition exists.";

    return (
      `<div class="pm-identity-rail" id="pmIdentityRail" data-theme-path="${escapeHtml(themePath)}">` +
      `<dl class="pm-identity-grid">` +
      `<div class="pm-identity-row"><dt>Author</dt><dd>${author}</dd></div>` +
      `<div class="pm-identity-row pm-identity-version"><dt>${versionLabel}</dt>` +
      versionDd +
      `</div>` +
      sourceHtml +
      `<div class="pm-identity-row"><dt>Published</dt><dd>${publishedHtml}</dd></div>` +
      `<div class="pm-identity-row"><dt>Status</dt><dd class="pm-identity-status">${statusHtml}</dd></div>` +
      `<div class="pm-identity-row"><dt>Theme / Appearance</dt><dd>` +
      `<label class="pm-theme-label"><span class="visually-hidden">Theme</span>` +
      `<select class="pm-theme-select" data-wired="theme-select" data-project="${pid}" ` +
      `title="${escapeHtml(themeTitle)}">${themeOptionsHtml(themePath)}</select></label>` +
      `</dd></div>` +
      `</dl>` +
      renderSidebarProjectOps(project.id) +
      `<div class="pm-identity-actions">` +
      `<button type="button" class="pm-action is-active" data-wired="edit-in-designer" ` +
      `data-project="${pid}" title="Open this project in the browser Designer">Edit project in Designer</button>` +
      `</div>` +
      `</div>`
    );
  }

  /** Open browser Designer (:5173) with this project's definition loaded (not a blank canvas). */
  async function openEditInDesigner(projectId) {
    const project =
      projectId && typeof TawalaDemo !== "undefined" && typeof TawalaDemo.getMyTawala === "function"
        ? TawalaDemo.getMyTawala(projectId)
        : null;
    const name =
      project && window.TawalaDemo && window.TawalaDemo.displayName
        ? window.TawalaDemo.displayName(project.name)
        : (project && project.name) || projectId || "this project";
    const jsonFile = (project && project.jsonFile) || "";
    const designerApp = "http://localhost:5173/";
    const stubQs =
      "designer.html?project=" +
      encodeURIComponent(projectId || "") +
      "&name=" +
      encodeURIComponent(name) +
      (jsonFile ? "&json=" + encodeURIComponent(jsonFile) : "");

    if (!project) {
      window.alert("Couldn't open in Designer\n\nProject not found in My Tawala.");
      return;
    }

    const resolved = await resolveDefinitionForEdit(project);
    if (!resolved.ok) {
      window.alert(
        `Can't open “${name}” in Designer — no project definition is available to edit.\n\n` +
          (resolved.message ||
            "Library copies need a catalog JSON (or Deploy → Show in My Tawala) before Edit works.")
      );
      setStatus(`Edit in Designer — no definition for “${name}”.`);
      return;
    }

    let openUrl = "";
    if (resolved.snapshotId) {
      openUrl =
        designerApp + "?snapshot=" + encodeURIComponent(String(resolved.snapshotId));
    } else if (resolved.mockJson) {
      openUrl =
        designerApp + "?mockJson=" + encodeURIComponent(String(resolved.mockJson));
    } else if (resolved.definition && typeof TawalaDemo.saveVersionSnapshot === "function") {
      const saved = await TawalaDemo.saveVersionSnapshot({
        project: resolved.definition,
        uniqueId: project.uniqueId || null,
        projectId: project.id || projectId,
        versionDescription: "Open in Designer",
      });
      if (!saved || saved.status === "failure" || !saved.snapshotId) {
        window.alert(
          `Couldn't prepare “${name}” for Designer.\n\n` +
            ((saved && saved.error) || "snapshot save failed") +
            "\n\nIs designer-web API on :3001? (cd designer-web && npm run keep)"
        );
        setStatus(`Edit in Designer failed — could not save snapshot.`);
        return;
      }
      openUrl = designerApp + "?snapshot=" + encodeURIComponent(String(saved.snapshotId));
    }

    if (!openUrl) {
      window.alert(
        `Can't open “${name}” in Designer — no open path resolved.\n\n` +
          "Try again after confirming Designer API (:3001) is running."
      );
      return;
    }

    const lines = [
      `Open “${name}” in the browser Designer with its definition loaded.`,
      "",
      "OK opens Designer at localhost:5173. Cancel opens the Designer stub page instead.",
    ];
    if (resolved.from === "jsonFile" && resolved.mockJson) {
      lines.splice(1, 0, "", `Source: ${resolved.mockJson}`);
    } else if (resolved.from === "overlay" || resolved.from === "api") {
      lines.splice(1, 0, "", "Source: My Tawala version snapshot.");
    }

    const goLive = window.confirm(lines.join("\n"));
    if (goLive) {
      window.open(openUrl, "_blank", "noopener");
      setStatus(`Opened “${name}” in Designer.`);
    } else {
      location.href = stubQs;
    }
  }

  /**
   * Resolve a Designer-loadable definition for Edit project in Designer.
   * Prefers version snapshot / cached definition, then catalog jsonFile under projects/.
   */
  async function resolveDefinitionForEdit(project) {
    const versions = Array.isArray(project.versions) ? project.versions.slice() : [];
    const ordered = [];
    const current = versions.find((v) => v && v.deployed);
    if (current) ordered.push(current);
    for (let i = versions.length - 1; i >= 0; i--) {
      const v = versions[i];
      if (v && v !== current) ordered.push(v);
    }
    for (let i = 0; i < ordered.length; i++) {
      const v = ordered[i];
      if (!versionIsRedeployable(v)) continue;
      const r = await resolveDefinitionForVersion(v);
      if (r.ok) {
        return {
          ok: true,
          definition: r.definition,
          snapshotId: r.snapshotId || (v && v.snapshotId) || null,
          from: r.from || "version",
        };
      }
    }

    const jsonFile = project.jsonFile ? String(project.jsonFile) : "";
    const catalogPath =
      jsonFile &&
      !jsonFile.includes("..") &&
      !jsonFile.startsWith("/") &&
      (/^projects\/(mytawala|library)\//.test(jsonFile) ||
        /^designer-web\/public\/samples\//.test(jsonFile))
        ? jsonFile
        : "";
    if (catalogPath) {
      /* Prefer API disk read via Designer ?mockJson= (works even if :5500 CORS is off). */
      if (catalogPath.startsWith("projects/")) {
        try {
          const res = await fetch(encodeMockRelPath(catalogPath), { cache: "no-store" });
          if (res.ok) {
            const def = await res.json();
            if (def && def.name) {
              return { ok: true, definition: def, mockJson: catalogPath, from: "jsonFile" };
            }
          }
        } catch {
          /* still offer mockJson deep-link — Designer API may read the file from disk */
        }
      }
      return { ok: true, mockJson: catalogPath, from: "jsonFile" };
    }

    return {
      ok: false,
      error: "no-definition",
      message:
        "This My Tawala row has no Designer definition yet.\n\n" +
        "Seeded apps (e.g. Online Exam Builder) should have a catalog JSON. " +
        "A Library acquire with no Push / Show in My Tawala snapshot also cannot be edited until a definition exists.",
    };
  }

  async function applyThemeSelection(projectId, themePath) {
    if (!projectId || !themePath) return;
    if (typeof TawalaTransfer === "undefined" || typeof TawalaTransfer.upsertMyTawalaProperties !== "function") {
      window.alert("Couldn't save theme — transfer script didn't load.");
      return;
    }
    const path = String(themePath);
    const ok = TawalaTransfer.upsertMyTawalaProperties(projectId, { themePath: path });
    if (!ok) {
      window.alert("Couldn't save theme on this browser overlay.");
      return;
    }
    const rail = document.getElementById("pmIdentityRail");
    if (rail) rail.setAttribute("data-theme-path", path);
    const sel = document.querySelector('select[data-wired="theme-select"][data-project="' + projectId + '"]');
    if (sel) {
      sel.title =
        `Theme: ${themeLabelForPath(path)} (${path}). ` +
        "Saved on My Tawala; Push / Redeploy applies CSS on :8080.";
    }

    const project =
      typeof TawalaDemo !== "undefined" && typeof TawalaDemo.getMyTawala === "function"
        ? TawalaDemo.getMyTawala(projectId)
        : null;
    const uniqueId =
      (project && project.uniqueId) ||
      (typeof TawalaDemo !== "undefined" &&
        typeof TawalaDemo.resolvePurgeUniqueId === "function" &&
        TawalaDemo.resolvePurgeUniqueId(projectId)) ||
      null;

    if (!project || !uniqueId) {
      setStatus(
        `Theme set to ${themeLabelForPath(path)} (overlay only — Push from Designer → Show in My Tawala to apply on :8080).`
      );
      return;
    }

    if (typeof TawalaDemo.deployProjectDefinition !== "function") {
      setStatus(
        `Theme set to ${themeLabelForPath(path)} (overlay saved — Push API missing; refresh and try again).`
      );
      return;
    }

    setStatus(`Applying theme ${themeLabelForPath(path)} on :8080…`);
    const resolved = await resolveDefinitionForEdit(project);
    if (!resolved.ok || !resolved.definition) {
      setStatus(
        `Theme set to ${themeLabelForPath(path)} (overlay only — no definition snapshot to Push; use Designer Push first).`
      );
      return;
    }

    const stamped = stampThemeOnDefinition(resolved.definition, path);
    const result = await TawalaDemo.deployProjectDefinition(stamped);
    if (!result || result.status === "failure") {
      const err = (result && result.error) || "unknown";
      setStatus(`Theme overlay saved; :8080 Push failed: ${err}`);
      window.alert(
        `Theme saved on My Tawala, but couldn’t apply CSS on :8080.\n\n${err}\n\n` +
          "Is Designer API on :3001 and Tomcat on :8080? Try Push this version or Push from Designer."
      );
      return;
    }

    /* Keep the current version’s cached definition in sync so later Push this version matches. */
    const versions = Array.isArray(project.versions) ? project.versions : [];
    const current = versions.find((v) => v && (v.deployed || v.current));
    if (
      current &&
      typeof TawalaTransfer.attachVersionDefinition === "function"
    ) {
      TawalaTransfer.attachVersionDefinition(
        projectId,
        current.versionNumber,
        stamped,
        current.snapshotId || resolved.snapshotId || null
      );
    }

    setStatus(
      `Theme ${themeLabelForPath(path)} applied on :8080 (hard-refresh the form if CSS looks cached).`
    );
  }

  function toggleProjectActive(projectId, active) {
    if (!projectId) return;
    if (typeof TawalaTransfer === "undefined" || typeof TawalaTransfer.setProjectLibraryActive !== "function") {
      window.alert("Couldn't update Active / Offline — transfer script didn't load.");
      return;
    }
    const label = active ? "Activate" : "De-activate";
    const confirmMsg = active
      ? "Activate this project?\n\nIf it is published, it will show again in the public Library (Test Drive / Save a copy)."
      : "De-activate this project?\n\nIt stays on My Tawala with an Offline marker, but is hidden from the public Library (no Test Drive / Save a copy). This is not Purge or Delete.";
    if (!window.confirm(confirmMsg)) return;
    const result = TawalaTransfer.setProjectLibraryActive(projectId, active);
    if (!result || !result.ok) {
      window.alert(`${label} failed\n\n${(result && result.error) || "Unknown error"}`);
      return;
    }
    setStatus(
      active
        ? "Project is Active — visible in Library when published."
        : "Project is Offline — hidden from Library; still on My Tawala."
    );
    document.dispatchEvent(
      new CustomEvent("tawala:project-active-changed", {
        detail: { projectId, active: !!active, result },
      })
    );
  }

  /**
   * Full Project Details layout (separate page).
   * Owner Aug 9: wider left sidebar — identity (incl. Theme), then Invite/Include under Theme,
   * then Edit in Designer; main column = title + actions + Project Data / Versions / Comments.
   * (Backups/emails/publish collapsible removed Aug 10 — OTHER_OPS archive-only.)
   * Project Data tree: expand (project → starts → all forms); selection scopes E/I/Purge.
   * Start URLs open live :8080 when deployed — do NOT purge-on-click (unlike Library Test drive).
   * @param {object} project
   * @param {{ justAcquired?: boolean }} [opts]
   */
  function renderDetailPanel(project, opts) {
    if (!project) return '<p class="pm-hint">Project not found.</p>';
    const projectDataSection = renderProjectDataTree(project);

    const versionOps = renderVersionsSection(project);

    const commentsStub =
      '<p class="pm-hint">Comments / reputation are parked (Aug 9). Stub only — not wired in this mock.</p>';

    const versionsOpen = projectVersionRows(project).length > 0;
    const displayName =
      window.TawalaDemo && window.TawalaDemo.displayName
        ? window.TawalaDemo.displayName(project.name)
        : String(project.name || "")
            .replace(/\.tawala\.xml$/i, "")
            .replace(/\.tawala$/i, "")
            .replace(/\.json$/i, "");
    const inactive = project.inactive === true || project.libraryActive === false;
    const justAcquired = !!(opts && opts.justAcquired);
    const justForked = !!(opts && opts.justForked);
    const libSrcName = librarySourceDisplayName(project);
    const acquiredBanner = justAcquired
      ? `<div class="pm-acquire-success" role="status">` +
        `<p><b>Saved a copy</b> as “${escapeHtml(displayName)}” in your My Tawala.` +
        (libSrcName ? ` Source: ${escapeHtml(libSrcName)}.` : "") +
        `</p>` +
        `<p class="pm-hint">Records start empty. <b>Use</b> stays unavailable until you Push from Designer ` +
        `(hover the grey Use control for the tip). ` +
        `<a href="mytawala.html">← Back to My Tawala</a> and sort by <b>Created</b> to see this row with today’s date.</p>` +
        `</div>`
      : justForked
        ? `<div class="pm-acquire-success" role="status">` +
          `<p><b>Made a copy</b> as “${escapeHtml(displayName)}”. Original project unchanged.</p>` +
          `<p class="pm-hint">Records start empty — this fork does <b>not</b> share the source’s live data. ` +
          `<b>Use</b> stays grey until you Push from Designer → Show in My Tawala. ` +
          `Rename anytime (button or double-click the title). ` +
          `<a href="mytawala.html">← Back to My Tawala</a></p>` +
          `</div>`
        : "";

    return (
      `<div class="pm-detail-layout" id="pmDetail" data-project-id="${escapeHtml(project.id)}">` +
      `<aside class="pm-detail-sidebar" aria-label="Project options">` +
      "<h3>Project options</h3>" +
      `<div class="pm-sidebar-identity">` +
      renderIdentityRail(project) +
      `</div>` +
      `<p class="pm-hint">Theme, Invite / Include (Deploy share), and Designer / Active controls stay in this rail. ` +
      `<a href="project-ops-review.html">Archive labels</a></p>` +
      "</aside>" +
      `<div class="pm-detail-main">` +
      acquiredBanner +
      `<div class="pm-detail-header">` +
      `<p class="pm-back"><a href="mytawala.html">← My Tawala</a></p>` +
      `<h2 class="pm-detail-title" tabindex="0" title="Double-click to rename" ` +
      `data-wired="rename-project" data-project="${escapeHtml(project.id)}">` +
      `${escapeHtml(displayName)}` +
      (inactive
        ? ` <span class="pm-status-badge is-offline" title="Offline — hidden from public Library">Offline</span>`
        : "") +
      `</h2>` +
      (() => {
        const blurb = String(project.shortDescription || "").trim();
        const empty = !blurb;
        return (
          `<p class="pm-detail-meta${empty ? " is-empty" : ""}" tabindex="0" ` +
          `data-wired="edit-description" data-project="${escapeHtml(project.id)}">` +
          (empty
            ? `<span class="pm-detail-meta-ph" aria-hidden="true">Add a description</span>`
            : escapeHtml(blurb)) +
          `</p>`
        );
      })() +
      "</div>" +
      renderProjectActionsBar(project.id) +
      `<p class="pm-detail-data-nav-hint" id="pmDataNavHint">` +
      `Click on the ▸ before the project name once to show the Start Link forms, a second time to see all forms.` +
      `</p>` +
      renderCollapsibleSection("pmSecData", "Project Data", projectDataSection, true) +
      renderCollapsibleSection("pmSecVersions", "Versions", versionOps, versionsOpen) +
      renderCollapsibleSection("pmSecComments", "Comments", commentsStub, false, {
        greyed: true,
        summaryTitle: "Comments / reputation — stub (not wired)",
      }) +
      '<p class="pm-hint" id="pmOpStatus" role="status"></p>' +
      "</div>" +
      "</div>"
    );
  }

  /** Full catalog for project-ops-review.html — Library surface first, then My Tawala. */
  function renderOpsCatalog() {
    function sectionHtml(sec) {
      const rows = sec.items
        .map((i) => {
          const src = i.source ? `<td class="pm-cat-src">${escapeHtml(i.source)}</td>` : "<td></td>";
          const title = i.title ? escapeHtml(i.title) : "";
          return (
            `<tr>` +
            `<td><code>${escapeHtml(i.label)}</code></td>` +
            `<td class="pm-cat-title">${title}</td>` +
            src +
            `<td class="pm-cat-wired">${escapeHtml(wiredNote(i))}</td>` +
            `</tr>`
          );
        })
        .join("");
      return (
        `<section class="pm-catalog-section" id="${escapeHtml(sec.id)}">` +
        `<h3>${escapeHtml(sec.title)}</h3>` +
        `<p class="pm-catalog-where">${escapeHtml(sec.where)}</p>` +
        `<table class="pm-catalog-table stripe">` +
        "<thead><tr><th>Label</th><th>Title / tooltip</th><th>Source note</th><th>Mock</th></tr></thead>" +
        `<tbody>${rows}</tbody></table>` +
        "</section>"
      );
    }

    function surfaceBlock(surfaceKey) {
      const intro = SURFACE_INTRO[surfaceKey];
      const secs = OPS_CATALOG_SECTIONS.filter((s) => s.surface === surfaceKey);
      const toc =
        '<ul class="pm-surface-toc">' +
        secs
          .map((sec) => `<li><a href="#${escapeHtml(sec.id)}">${escapeHtml(sec.title)}</a></li>`)
          .join("") +
        "</ul>";
      return (
        `<div class="pm-catalog-surface" id="surface-${escapeHtml(surfaceKey)}">` +
        `<h2>${escapeHtml(intro.heading)}</h2>` +
        `<p class="pm-catalog-where">${escapeHtml(intro.blurb)}</p>` +
        toc +
        secs.map(sectionHtml).join("") +
        "</div>"
      );
    }

    return (
      '<div class="pm-catalog">' +
      `<p class="pm-archive-source">${escapeHtml(ARCHIVE_NOTE)}</p>` +
      '<nav class="pm-catalog-toc" aria-label="Control surfaces"><ul>' +
      '<li><a href="#surface-library">Public Library controls</a></li>' +
      '<li><a href="#surface-mytawala">My Tawala / Project Manager controls</a></li>' +
      "</ul></nav>" +
      surfaceBlock("library") +
      surfaceBlock("mytawala") +
      "</div>"
    );
  }

  function showConfirm(confirmId) {
    const c = CONFIRMS[confirmId];
    if (!c) return Promise.resolve(true);
    return Promise.resolve(window.confirm(`${c.title}\n\n${c.body}`));
  }

  /**
   * Destructive Purge confirm — clear project-vs-form scope (Aug 9 selection model).
   * Clears response/submission data only; never the project definition.
   */
  function showPurgeConfirm({ displayName, formName }) {
    const project = displayName || "this project";
    if (formName) {
      return Promise.resolve(
        window.confirm(
          `Purge Form Data\n\n` +
            `Permanently delete all response/submission data for form “${formName}” in “${project}”?\n\n` +
            `Other forms are left alone. The project definition is not changed.\n\n` +
            `Use keeps data — only Purge clears it.`
        )
      );
    }
    return Promise.resolve(
      window.confirm(
        `Purge Project Data\n\n` +
          `Permanently delete ALL response/submission data for “${project}”?\n\n` +
          `The project stays on My Tawala; only Records/responses are cleared (not Delete, not De-activate).\n\n` +
          `Use keeps data — only Purge clears it.`
      )
    );
  }

  function setStatus(msg) {
    const el = document.getElementById("pmOpStatus");
    if (el) {
      el.textContent = msg;
      try {
        el.scrollIntoView({ block: "nearest", behavior: "smooth" });
      } catch {
        /* ignore */
      }
    }
  }

  const PUBLISH_MODAL_ID = "tawalaPublishModal";
  const DEPLOY_SHARE_MODAL_ID = "tawalaDeployShareModal";
  const TESTDRIVE_PICK_MODAL_ID = "tawalaTestDrivePickModal";
  const SAVE_COPY_MODAL_ID = "tawalaSaveCopyModal";
  const MAKE_COPY_MODAL_ID = "tawalaMakeCopyModal";
  const GET_LIBRARY_MODAL_ID = "tawalaGetLibraryModal";
  const RENAME_MODAL_ID = "tawalaRenameModal";
  const DESC_MODAL_ID = "tawalaDescModal";

  function closePublishModal() {
    const el = document.getElementById(PUBLISH_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handlePublishModalKeydown, true);
  }

  function handlePublishModalKeydown(ev) {
    if (ev.key === "Escape") closePublishModal();
  }

  function closeDeployShareModal() {
    const el = document.getElementById(DEPLOY_SHARE_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handleDeployShareModalKeydown, true);
  }

  function handleDeployShareModalKeydown(ev) {
    if (ev.key === "Escape") closeDeployShareModal();
  }

  function closeTestDrivePickModal() {
    const el = document.getElementById(TESTDRIVE_PICK_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handleTestDrivePickModalKeydown, true);
  }

  function handleTestDrivePickModalKeydown(ev) {
    if (ev.key === "Escape") closeTestDrivePickModal();
  }

  function closeSaveCopyModal() {
    const el = document.getElementById(SAVE_COPY_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handleSaveCopyModalKeydown, true);
  }

  function handleSaveCopyModalKeydown(ev) {
    if (ev.key === "Escape") closeSaveCopyModal();
  }

  function closeMakeCopyModal() {
    const el = document.getElementById(MAKE_COPY_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handleMakeCopyModalKeydown, true);
  }

  function handleMakeCopyModalKeydown(ev) {
    if (ev.key === "Escape") closeMakeCopyModal();
  }

  function closeGetLibraryModal() {
    const el = document.getElementById(GET_LIBRARY_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handleGetLibraryModalKeydown, true);
  }

  function handleGetLibraryModalKeydown(ev) {
    if (ev.key === "Escape") closeGetLibraryModal();
  }

  function closeRenameModal() {
    const el = document.getElementById(RENAME_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handleRenameModalKeydown, true);
  }

  function handleRenameModalKeydown(ev) {
    if (ev.key === "Escape") closeRenameModal();
  }

  function closeDescModal() {
    const el = document.getElementById(DESC_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handleDescModalKeydown, true);
  }

  function handleDescModalKeydown(ev) {
    if (ev.key === "Escape") closeDescModal();
  }

  /**
   * Project Details — double-click title → rename My Tawala name (overlay, like Save a copy).
   */
  function openRenameDialog(projectId) {
    if (typeof TawalaTransfer === "undefined" || typeof TawalaDemo === "undefined") {
      window.alert("Rename isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    if (typeof TawalaTransfer.renameMyTawalaProject !== "function") {
      window.alert("Rename isn't available — transfer support is outdated. Hard-refresh and try again.");
      return;
    }
    /* Prefer explicit id; fall back to Details host so a stale/empty data-project never no-ops. */
    let id = String(projectId || "").trim();
    if (!id) {
      const host = document.getElementById("pmDetail");
      id = (host && host.dataset.projectId) || "";
    }
    const project = id && TawalaDemo.getMyTawala ? TawalaDemo.getMyTawala(id) : null;
    if (!project) {
      window.alert(`Can't rename — unknown My Tawala project: ${id || "(none)"}`);
      return;
    }
    closeRenameModal();
    closeSaveCopyModal();
    closeMakeCopyModal();
    closeDescModal();

    const currentName = TawalaDemo.displayName(project.name || id);
    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = RENAME_MODAL_ID;
    backdrop.dataset.projectId = id;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish" role="dialog" aria-modal="true" aria-labelledby="renameModalTitle">' +
      `<h3 id="renameModalTitle">Rename</h3>` +
      `<p class="pm-hint tawala-modal-lede">Pick a name you’ll recognize in My Tawala. This does not create a new project or change any Library entry — only the display name on this private copy.</p>` +
      '<div class="tawala-modal-body">' +
      '<label class="tawala-modal-field" for="renameNameInput">Your project name' +
      `<input type="text" id="renameNameInput" name="tawala-rename-name" value="${escapeHtml(currentName)}" autocomplete="off" spellcheck="false" />` +
      "</label>" +
      '<p class="pm-hint tawala-modal-hint-tight">If the name matches another My Tawala project (not case-sensitive), you’ll be asked to confirm before replacing that other project.</p>' +
      '<p class="pm-hint" id="renameModalError" role="alert" style="display:none;"></p>' +
      "</div>" +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="renameModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="renameModalConfirm">Rename</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);

    const nameInput = backdrop.querySelector("#renameNameInput");
    const errEl = backdrop.querySelector("#renameModalError");
    function showErr(msg) {
      if (!errEl) return;
      errEl.textContent = msg || "";
      errEl.style.display = msg ? "" : "none";
    }
    function readRenameInput() {
      /* Always read live from the open modal — never a stale closure value. */
      const live =
        document.querySelector("#tawalaRenameModal #renameNameInput") || nameInput;
      return live ? String(live.value || "").trim() : "";
    }
    function checkRenameCollisionLive() {
      const nameVal = readRenameInput();
      if (!nameVal) {
        showErr("");
        return;
      }
      if (nameVal === currentName) {
        showErr("");
        return;
      }
      if (
        typeof TawalaTransfer.myTawalaNameTaken === "function" &&
        TawalaTransfer.myTawalaNameTaken(nameVal, id)
      ) {
        showErr(
          `Warning: you already have a project named “${nameVal}”. Renaming will replace that other project (this project keeps its identity under the new name).`
        );
        return;
      }
      showErr("");
    }

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closeRenameModal();
    });
    backdrop.querySelector("#renameModalCancel").addEventListener("click", closeRenameModal);
    document.addEventListener("keydown", handleRenameModalKeydown, true);

    function confirmRename() {
      const nameVal = readRenameInput();
      if (!nameVal) {
        showErr("Enter a project name.");
        if (nameInput) nameInput.focus();
        return;
      }
      const targetId = backdrop.dataset.projectId || id;
      const sameAsCurrent = nameVal === currentName;
      const conflict =
        !sameAsCurrent && typeof TawalaTransfer.findMyTawalaByName === "function"
          ? TawalaTransfer.findMyTawalaByName(nameVal, targetId)
          : null;
      if (conflict && conflict.id !== targetId) {
        const conflictLabel =
          typeof TawalaDemo.displayName === "function"
            ? TawalaDemo.displayName(conflict.name)
            : conflict.name || conflict.id;
        const ok = window.confirm(
          `You already have a project named “${nameVal}”.\n\n` +
            `Replace “${conflictLabel}” with this rename?\n\n` +
            `The other project will be removed from My Tawala (its Deploy / response identity goes with it). ` +
            `This project keeps its own identity under the new name.`
        );
        if (!ok) {
          if (nameInput) nameInput.focus();
          return;
        }
      }
      const result = TawalaTransfer.renameMyTawalaProject(targetId, nameVal, {
        overwrite: !!(conflict && conflict.id !== targetId),
      });
      if (!result || !result.ok) {
        showErr((result && result.error) || "Unknown error");
        if (nameInput) nameInput.focus();
        return;
      }
      closeRenameModal();
      /* Paint title immediately so a slow re-render cannot look like “ignored”. */
      const title = document.querySelector("h2.pm-detail-title");
      if (title && result.name) {
        const badge = title.querySelector(".pm-status-badge");
        const badgeClone = badge ? badge.cloneNode(true) : null;
        title.textContent = "";
        title.appendChild(document.createTextNode(result.name));
        if (badgeClone) {
          title.appendChild(document.createTextNode(" "));
          title.appendChild(badgeClone);
        }
      }
      if (!result.unchanged) {
        setStatus(
          result.replacedId
            ? `Renamed to “${result.name}” (replaced the other project with that name).`
            : `Renamed to “${result.name}”.`
        );
      }
      document.dispatchEvent(
        new CustomEvent("tawala:project-renamed", {
          detail: { projectId: targetId, name: result.name, result },
        })
      );
    }

    backdrop.querySelector("#renameModalConfirm").addEventListener("click", (ev) => {
      ev.preventDefault();
      ev.stopPropagation();
      confirmRename();
    });
    nameInput.addEventListener("input", checkRenameCollisionLive);
    nameInput.addEventListener("keydown", (ev) => {
      if (ev.key === "Enter") {
        ev.preventDefault();
        ev.stopPropagation();
        confirmRename();
      }
    });
    setTimeout(() => {
      nameInput.focus();
      nameInput.select();
    }, 0);
  }

  /**
   * Project Details — double-click blurb under title → edit shortDescription (My Tawala overlay).
   * Quieter than Rename: no instructional “double-click” hint on the page; discoverable only that way.
   */
  function openEditDescriptionDialog(projectId) {
    if (typeof TawalaTransfer === "undefined" || typeof TawalaDemo === "undefined") {
      window.alert("Description edit isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    if (typeof TawalaTransfer.upsertMyTawalaProperties !== "function") {
      window.alert("Description edit isn't available — transfer support is outdated. Hard-refresh and try again.");
      return;
    }
    const project = projectId && TawalaDemo.getMyTawala ? TawalaDemo.getMyTawala(projectId) : null;
    if (!project) {
      window.alert(`Can't edit description — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    closeDescModal();
    closeRenameModal();
    closeSaveCopyModal();
    closeMakeCopyModal();

    const current = String(project.shortDescription || "").trim();
    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = DESC_MODAL_ID;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish tawala-modal--desc" role="dialog" aria-modal="true" aria-labelledby="descModalTitle">' +
      `<h3 id="descModalTitle">Description</h3>` +
      '<div class="tawala-modal-body">' +
      '<label class="tawala-modal-field" for="descModalInput">Under the project title' +
      `<textarea id="descModalInput" rows="3" autocomplete="off">${escapeHtml(current)}</textarea>` +
      "</label>" +
      '<p class="pm-hint" id="descModalError" role="alert" style="display:none;"></p>' +
      "</div>" +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="descModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="descModalConfirm">Save</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);

    const input = backdrop.querySelector("#descModalInput");
    const errEl = backdrop.querySelector("#descModalError");
    function showErr(msg) {
      if (!errEl) return;
      errEl.textContent = msg || "";
      errEl.style.display = msg ? "" : "none";
    }

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closeDescModal();
    });
    backdrop.querySelector("#descModalCancel").addEventListener("click", closeDescModal);
    document.addEventListener("keydown", handleDescModalKeydown, true);

    function confirmDesc() {
      const next = String(input.value || "").trim();
      if (next === current) {
        closeDescModal();
        return;
      }
      const ok = TawalaTransfer.upsertMyTawalaProperties(projectId, {
        shortDescription: next,
      });
      if (!ok) {
        showErr("Could not save to My Tawala (localStorage).");
        input.focus();
        return;
      }
      closeDescModal();
      setStatus(next ? "Description updated." : "Description cleared.");
      document.dispatchEvent(
        new CustomEvent("tawala:project-description-changed", {
          detail: { projectId, shortDescription: next },
        })
      );
    }

    backdrop.querySelector("#descModalConfirm").addEventListener("click", confirmDesc);
    input.addEventListener("keydown", (ev) => {
      if (ev.key === "Enter" && (ev.metaKey || ev.ctrlKey)) {
        ev.preventDefault();
        confirmDesc();
      }
    });
    setTimeout(() => {
      input.focus();
      input.select();
    }, 0);
  }

  /**
   * Library Save a copy — rename-on-acquire into private My Tawala (Aug 9 Task #8).
   * libraryId is the public catalog id (not a My Tawala row).
   */
  /**
   * Get from Library… (My Tawala listing) — pick a public Library project, then the same
   * Save a copy rename flow. Not a nav duplicate of Library chrome.
   */
  function openGetFromLibraryDialog() {
    if (typeof TawalaDemo === "undefined") {
      window.alert("Get from Library isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    closeGetLibraryModal();
    closeSaveCopyModal();

    const entries =
      typeof TawalaDemo.libraryEntries === "function" ? TawalaDemo.libraryEntries() : [];
    const list = entries
      .slice()
      .filter((p) => p && p.id && p.inactive !== true && p.libraryActive !== false)
      .sort((a, b) => {
        const ca = String(a.category || "Uncategorized").localeCompare(String(b.category || "Uncategorized"));
        if (ca !== 0) return ca;
        const na = TawalaDemo.displayName ? TawalaDemo.displayName(a.name) : String(a.name || a.id);
        const nb = TawalaDemo.displayName ? TawalaDemo.displayName(b.name) : String(b.name || b.id);
        return na.localeCompare(nb);
      });

    if (!list.length) {
      window.alert(
        "No public Library projects are available to copy right now.\n\n" +
          "Open Library from the top nav if you want to browse; the catalog may be empty in this browser."
      );
      return;
    }

    const rowsHtml = list
      .map((p, idx) => {
        const title = TawalaDemo.displayName
          ? TawalaDemo.displayName(p.name)
          : String(p.name || p.id);
        const cat = p.category || "Uncategorized";
        const blurb = String(p.shortDescription || "").trim();
        const live =
          p.liveReady === true || (p.deployed && p.testDriveUrl)
            ? ' <span class="get-lib-live" title="Live try-out on :8080">Live</span>'
            : "";
        const checked = idx === 0 ? " checked" : "";
        return (
          `<label class="get-lib-row">` +
          `<input type="radio" name="getLibPick" value="${escapeHtml(p.id)}"${checked} />` +
          `<span class="get-lib-row-body">` +
          `<span class="get-lib-name">${escapeHtml(title)}${live}</span>` +
          `<span class="get-lib-meta">${escapeHtml(cat)}</span>` +
          (blurb ? `<span class="get-lib-blurb">${escapeHtml(blurb)}</span>` : "") +
          `</span></label>`
        );
      })
      .join("");

    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = GET_LIBRARY_MODAL_ID;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish tawala-modal--get-library" role="dialog" aria-modal="true" aria-labelledby="getLibModalTitle">' +
      `<h3 id="getLibModalTitle">Get from Library</h3>` +
      `<p class="pm-hint tawala-modal-lede">Pick a public Library project to <b>Save a copy</b> into your private My Tawala. ` +
      `You’ll name it next. This is not the same as browsing Library in the top nav.</p>` +
      '<div class="tawala-modal-body">' +
      `<div class="get-lib-list" role="radiogroup" aria-label="Public Library projects">${rowsHtml}</div>` +
      '<p class="pm-hint" id="getLibModalError" role="alert" style="display:none;"></p>' +
      "</div>" +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="getLibModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="getLibModalContinue">Continue…</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);
    document.addEventListener("keydown", handleGetLibraryModalKeydown, true);

    const errEl = backdrop.querySelector("#getLibModalError");
    function showErr(msg) {
      if (!errEl) return;
      errEl.textContent = msg || "";
      errEl.style.display = msg ? "" : "none";
    }

    function selectedLibraryId() {
      const picked = backdrop.querySelector('input[name="getLibPick"]:checked');
      return picked ? picked.value : "";
    }

    function continueAcquire() {
      const libraryId = selectedLibraryId();
      if (!libraryId) {
        showErr("Pick a Library project to continue.");
        return;
      }
      closeGetLibraryModal();
      openSaveCopyDialog(libraryId);
    }

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closeGetLibraryModal();
    });
    backdrop.querySelector("#getLibModalCancel").addEventListener("click", closeGetLibraryModal);
    backdrop.querySelector("#getLibModalContinue").addEventListener("click", continueAcquire);
    backdrop.querySelectorAll(".get-lib-row").forEach((row) => {
      row.addEventListener("dblclick", (ev) => {
        const radio = row.querySelector('input[name="getLibPick"]');
        if (radio) radio.checked = true;
        ev.preventDefault();
        continueAcquire();
      });
    });

    const first = backdrop.querySelector('input[name="getLibPick"]');
    if (first) first.focus();
  }

  function openSaveCopyDialog(libraryId) {
    if (typeof TawalaTransfer === "undefined" || typeof TawalaDemo === "undefined") {
      window.alert("Save to MyTawala isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    if (typeof TawalaTransfer.saveCopyFromLibrary !== "function") {
      window.alert("Save to MyTawala isn't available — transfer support is outdated. Hard-refresh and try again.");
      return;
    }
    if (!isMockLoggedIn()) {
      window.alert(
        "Save to MyTawala requires a free account.\n\nLog in or Register, then try again."
      );
      return;
    }
    const project = libraryId && TawalaDemo.getLibrary ? TawalaDemo.getLibrary(libraryId) : null;
    if (!project) {
      window.alert(`Can't Save to MyTawala — unknown Library project: ${libraryId || "(none)"}`);
      return;
    }
    closeSaveCopyModal();
    closeGetLibraryModal();
    closeRenameModal();
    closeMakeCopyModal();
    closeDescModal();

    const sourceName = TawalaDemo.displayName(project.name || libraryId);
    const defaultName =
      typeof TawalaTransfer.suggestUniqueMyTawalaName === "function"
        ? TawalaTransfer.suggestUniqueMyTawalaName(sourceName)
        : TawalaTransfer.stripStubSuffix(sourceName);

    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = SAVE_COPY_MODAL_ID;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish" role="dialog" aria-modal="true" aria-labelledby="saveCopyModalTitle">' +
      `<h3 id="saveCopyModalTitle">Save to MyTawala</h3>` +
      `<p class="pm-hint tawala-modal-lede">Save “${escapeHtml(sourceName)}” into your private My Tawala. ` +
      `<b>Choose a name</b> you’ll recognize later — the suggestion below is only a starting point (you can keep it or type your own). ` +
      `This creates a new project identity; renaming alone later does not. ` +
      `<b>Use</b> works when the copy lands (mock shares the Library live start URLs for review — production must mint a private uniqueId).</p>` +
      '<div class="tawala-modal-body">' +
      '<label class="tawala-modal-field" for="saveCopyNameInput">Your project name' +
      `<input type="text" id="saveCopyNameInput" value="${escapeHtml(defaultName)}" autocomplete="off" />` +
      "</label>" +
      '<p class="pm-hint tawala-modal-hint-tight">If the name matches an existing My Tawala project (not case-sensitive), you’ll be asked to confirm before replacing it.</p>' +
      '<p class="pm-hint" id="saveCopyModalError" role="alert" style="display:none;"></p>' +
      "</div>" +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="saveCopyModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="saveCopyModalConfirm">Save to MyTawala</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);

    const nameInput = backdrop.querySelector("#saveCopyNameInput");
    const errEl = backdrop.querySelector("#saveCopyModalError");
    function showErr(msg) {
      if (!errEl) return;
      errEl.textContent = msg || "";
      errEl.style.display = msg ? "" : "none";
    }
    function checkSaveCopyCollisionLive() {
      const nameVal = nameInput.value.trim();
      if (!nameVal) {
        showErr("");
        return;
      }
      if (
        typeof TawalaTransfer.myTawalaNameTaken === "function" &&
        TawalaTransfer.myTawalaNameTaken(nameVal)
      ) {
        showErr(
          `Warning: you already have a project named “${nameVal}”. Saving will replace that My Tawala project with this copy.`
        );
        return;
      }
      showErr("");
    }

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closeSaveCopyModal();
    });
    backdrop.querySelector("#saveCopyModalCancel").addEventListener("click", closeSaveCopyModal);
    document.addEventListener("keydown", handleSaveCopyModalKeydown, true);

    function confirmSave() {
      const nameVal = nameInput.value.trim();
      if (!nameVal) {
        showErr("Enter a name for your My Tawala copy.");
        nameInput.focus();
        return;
      }
      const conflict =
        typeof TawalaTransfer.findMyTawalaByName === "function"
          ? TawalaTransfer.findMyTawalaByName(nameVal)
          : null;
      if (conflict) {
        const conflictLabel =
          typeof TawalaDemo.displayName === "function"
            ? TawalaDemo.displayName(conflict.name)
            : conflict.name || conflict.id;
        const ok = window.confirm(
          `You already have a project named “${nameVal}”.\n\n` +
            `Replace “${conflictLabel}” with this Save a copy?\n\n` +
            `The existing project will be removed from My Tawala (its Deploy / response identity goes with it). ` +
            `This acquire becomes the sole row with that name.`
        );
        if (!ok) {
          nameInput.focus();
          return;
        }
      }
      const result = TawalaTransfer.saveCopyFromLibrary({
        libraryId,
        name: nameVal,
        overwrite: !!conflict,
      });
      if (!result || !result.ok) {
        showErr((result && result.error) || "Unknown error");
        nameInput.focus();
        return;
      }
      closeSaveCopyModal();
      setStatus(
        result.replacedId
          ? `Saved a copy as “${nameVal}” in My Tawala (replaced the previous project with that name).`
          : `Saved a copy as “${nameVal}” in My Tawala.`
      );
      document.dispatchEvent(
        new CustomEvent("tawala:library-saved-copy", {
          detail: { libraryId, myTawalaId: result.id, name: nameVal, result },
        })
      );
      /* Land on My Tawala listing with the new row selected (not Project Details). */
      window.location.href =
        "mytawala.html?highlight=" + encodeURIComponent(result.id);
    }

    backdrop.querySelector("#saveCopyModalConfirm").addEventListener("click", confirmSave);
    nameInput.addEventListener("input", checkSaveCopyCollisionLive);
    nameInput.addEventListener("keydown", (ev) => {
      if (ev.key === "Enter") {
        ev.preventDefault();
        confirmSave();
      }
    });
    setTimeout(() => {
      nameInput.focus();
      nameInput.select();
      checkSaveCopyCollisionLive();
    }, 0);
  }

  /**
   * Make a Copy — fork own My Tawala project (Aug 9 Task #9).
   * Distinct from Rename (same id, new name) and Library Save a copy (acquire from catalog).
   */
  function openMakeCopyDialog(sourceId) {
    if (typeof TawalaTransfer === "undefined" || typeof TawalaDemo === "undefined") {
      window.alert("Make a Copy isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    if (typeof TawalaTransfer.makeCopyOfMyTawalaProject !== "function") {
      window.alert("Make a Copy isn't available — transfer support is outdated. Hard-refresh and try again.");
      return;
    }
    const project = sourceId && TawalaDemo.getMyTawala ? TawalaDemo.getMyTawala(sourceId) : null;
    if (!project) {
      window.alert(`Can't Make a Copy — unknown My Tawala project: ${sourceId || "(none)"}`);
      return;
    }
    closeMakeCopyModal();
    closeSaveCopyModal();
    closeRenameModal();
    closeDescModal();
    closeGetLibraryModal();

    const sourceName = TawalaDemo.displayName(project.name || sourceId);
    const defaultName =
      typeof TawalaTransfer.suggestMakeCopyName === "function"
        ? TawalaTransfer.suggestMakeCopyName(sourceName)
        : `Copy of ${sourceName}`;

    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = MAKE_COPY_MODAL_ID;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish" role="dialog" aria-modal="true" aria-labelledby="makeCopyModalTitle">' +
      `<h3 id="makeCopyModalTitle">Make a Copy</h3>` +
      `<p class="pm-hint tawala-modal-lede">Fork “${escapeHtml(sourceName)}” into a <b>new</b> My Tawala project. ` +
      `The original stays as-is. This is <b>not</b> Rename (same project, new name) and <b>not</b> Library Save a copy. ` +
      `Copies the definition only — <b>Records start empty</b>; <b>Use</b> stays grey until you Push from Designer ` +
      `(does not share the source’s live :8080 data).</p>` +
      '<div class="tawala-modal-body">' +
      '<label class="tawala-modal-field" for="makeCopyNameInput">Name for the copy' +
      `<input type="text" id="makeCopyNameInput" value="${escapeHtml(defaultName)}" autocomplete="off" />` +
      "</label>" +
      '<p class="pm-hint tawala-modal-hint-tight">Pick a name different from the original. If it matches another My Tawala project (not case-sensitive), you’ll be asked to confirm before replacing that other project.</p>' +
      '<p class="pm-hint" id="makeCopyModalError" role="alert" style="display:none;"></p>' +
      "</div>" +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="makeCopyModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="makeCopyModalConfirm">Make a Copy</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);

    const nameInput = backdrop.querySelector("#makeCopyNameInput");
    const errEl = backdrop.querySelector("#makeCopyModalError");
    function showErr(msg) {
      if (!errEl) return;
      errEl.textContent = msg || "";
      errEl.style.display = msg ? "" : "none";
    }
    function checkMakeCopyCollisionLive() {
      const nameVal = nameInput.value.trim();
      if (!nameVal) {
        showErr("");
        return;
      }
      if (
        typeof TawalaTransfer.compactNameKey === "function" &&
        TawalaTransfer.compactNameKey(nameVal) === TawalaTransfer.compactNameKey(sourceName)
      ) {
        showErr(
          "Choose a different name — Make a Copy keeps the original. Use Rename to change this project’s name."
        );
        return;
      }
      if (
        typeof TawalaTransfer.myTawalaNameTaken === "function" &&
        TawalaTransfer.myTawalaNameTaken(nameVal, sourceId)
      ) {
        showErr(
          `Warning: you already have a project named “${nameVal}”. Making a copy will replace that other project.`
        );
        return;
      }
      showErr("");
    }

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closeMakeCopyModal();
    });
    backdrop.querySelector("#makeCopyModalCancel").addEventListener("click", closeMakeCopyModal);
    document.addEventListener("keydown", handleMakeCopyModalKeydown, true);

    function confirmMakeCopy() {
      const nameVal = nameInput.value.trim();
      if (!nameVal) {
        showErr("Enter a name for the copy.");
        nameInput.focus();
        return;
      }
      if (
        typeof TawalaTransfer.compactNameKey === "function" &&
        TawalaTransfer.compactNameKey(nameVal) === TawalaTransfer.compactNameKey(sourceName)
      ) {
        showErr(
          "Choose a different name — Make a Copy keeps the original. Use Rename to change this project’s name."
        );
        nameInput.focus();
        return;
      }
      const conflict =
        typeof TawalaTransfer.findMyTawalaByName === "function"
          ? TawalaTransfer.findMyTawalaByName(nameVal, sourceId)
          : null;
      if (conflict) {
        const conflictLabel =
          typeof TawalaDemo.displayName === "function"
            ? TawalaDemo.displayName(conflict.name)
            : conflict.name || conflict.id;
        const ok = window.confirm(
          `You already have a project named “${nameVal}”.\n\n` +
            `Replace “${conflictLabel}” with this Make a Copy?\n\n` +
            `The other project will be removed from My Tawala (its Deploy / response identity goes with it). ` +
            `The original “${sourceName}” stays untouched.`
        );
        if (!ok) {
          nameInput.focus();
          return;
        }
      }
      const result = TawalaTransfer.makeCopyOfMyTawalaProject({
        sourceId,
        name: nameVal,
        overwrite: !!conflict,
      });
      if (!result || !result.ok) {
        showErr((result && result.error) || "Unknown error");
        nameInput.focus();
        return;
      }
      closeMakeCopyModal();
      setStatus(
        result.replacedId
          ? `Made a copy as “${nameVal}” (replaced the previous project with that name). Original “${sourceName}” unchanged.`
          : `Made a copy as “${nameVal}”. Original “${sourceName}” unchanged.`
      );
      document.dispatchEvent(
        new CustomEvent("tawala:mytawala-made-copy", {
          detail: { sourceId, myTawalaId: result.id, name: nameVal, result },
        })
      );
      /* Land on the new fork’s Details so Rename is obvious and Records show empty. */
      window.location.href =
        "mytawala-project.html?project=" +
        encodeURIComponent(result.id) +
        "&forked=1";
    }

    backdrop.querySelector("#makeCopyModalConfirm").addEventListener("click", confirmMakeCopy);
    nameInput.addEventListener("input", checkMakeCopyCollisionLive);
    nameInput.addEventListener("keydown", (ev) => {
      if (ev.key === "Enter") {
        ev.preventDefault();
        confirmMakeCopy();
      }
    });
    setTimeout(() => {
      nameInput.focus();
      nameInput.select();
      checkMakeCopyCollisionLive();
    }, 0);
  }

  /** Option label spelling out what picking this target will actually do (collision safety). */
  function publishTargetOptionLabel(candidate, matchFlag) {
    const name = escapeHtml(candidate.name);
    const action = candidate.stub
      ? "retires to My Tawala, kept marked (stub)"
      : "replaces in place — overlay only";
    return `${name} — ${action}${matchFlag ? ` (${matchFlag} match)` : ""}`;
  }

  /** "Uncategorized" is always offered even though it isn't a real Library group — Publish
   * falls back to it when there's no replace-target and no sensible source category. */
  const UNCATEGORIZED_LABEL = "Uncategorized";

  function publishCategoryOptionsHtml(selectedLabel) {
    const cats =
      typeof TawalaDemo !== "undefined" && typeof TawalaDemo.libraryCategories === "function"
        ? TawalaDemo.libraryCategories()
        : [];
    const labels = cats.map((c) => c.label);
    if (selectedLabel && !labels.includes(selectedLabel) && selectedLabel !== UNCATEGORIZED_LABEL) {
      labels.push(selectedLabel);
    }
    if (!labels.includes(UNCATEGORIZED_LABEL)) labels.push(UNCATEGORIZED_LABEL);
    return labels
      .map((label) => {
        const sel = label === (selectedLabel || UNCATEGORIZED_LABEL) ? " selected" : "";
        return `<option value="${escapeHtml(label)}"${sel}>${escapeHtml(label)}</option>`;
      })
      .join("");
  }

  /** Owner Aug 1, 2026 — category is easy to miss on Publish; pick a sane default so the
   * select is never blank: replace-target's current category first, else the source
   * project's own category (if it's a real Library group), else "Uncategorized". */
  function defaultPublishCategory(target, sourceProject) {
    if (target && target.category) return target.category;
    if (
      sourceProject &&
      sourceProject.category &&
      typeof TawalaDemo !== "undefined" &&
      typeof TawalaDemo.categoryByLabel === "function" &&
      TawalaDemo.categoryByLabel(sourceProject.category)
    ) {
      return sourceProject.category;
    }
    return UNCATEGORIZED_LABEL;
  }

  function publishTargetOptionsHtml(matches, allCandidates) {
    const matchIds = new Set(matches.map((m) => m.id));
    const exact = matches.filter((m) => m.matchKind === "exact");
    const loose = matches.filter((m) => m.matchKind === "loose");
    const rest = allCandidates
      .filter((c) => !matchIds.has(c.id))
      .sort((a, b) => a.name.localeCompare(b.name));

    let html = '<option value="">— None (add as new Library entry) —</option>';
    if (exact.length || loose.length) {
      html += '<optgroup label="Possible matches">';
      html += exact.map((m) => `<option value="${escapeHtml(m.id)}">${publishTargetOptionLabel(m, "exact")}</option>`).join("");
      html += loose.map((m) => `<option value="${escapeHtml(m.id)}">${publishTargetOptionLabel(m, "possible")}</option>`).join("");
      html += "</optgroup>";
    }
    if (rest.length) {
      html += '<optgroup label="Other Library projects">';
      html += rest.map((c) => `<option value="${escapeHtml(c.id)}">${publishTargetOptionLabel(c, null)}</option>`).join("");
      html += "</optgroup>";
    }
    return html;
  }

  /**
   * Website Deploy (My Tawala Details) — Task #10 doorway.
   * Admin go-live + share help: copy a start URL / iframe embed. Not Publish, not Designer Push.
   * Empty Library acquires (no uniqueId / :8080 URLs) get an honest “need a live project first” message.
   * Seeded live projects (e.g. Online Exam Builder) share immediately.
   */
  function iframeEmbedSnippet(url, title) {
    const safeTitle = String(title || "Tawala form").replace(/"/g, "&quot;");
    return (
      `<iframe src="${url}" title="${safeTitle}" width="100%" height="640" ` +
      `style="border:0;" loading="lazy"></iframe>`
    );
  }

  function openDeployShareDialog(projectId) {
    if (typeof TawalaDemo === "undefined") {
      window.alert("Deploy isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    const project = resolveMyTawalaProject(projectId);
    if (!project) {
      window.alert(`Can't open Deploy — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    closeDeployShareModal();
    closePublishModal();

    const displayName =
      typeof TawalaDemo.displayName === "function"
        ? TawalaDemo.displayName(project.name || projectId)
        : String(project.name || projectId);
    const starts = startPointsWithUrls(project);
    const hasLive = starts.length > 0;
    const fromAcquire =
      !!(project.fromLibraryAcquire || project.sourcePile === "library-acquire") && !hasLive;

    let bodyHtml;
    if (!hasLive) {
      bodyHtml =
        '<div class="tawala-modal-body">' +
        '<p class="pm-hint" role="status">' +
        (fromAcquire
          ? "<b>This copy is not live yet.</b> Library <b>Save a copy</b> creates an empty private row " +
            "(no :8080 uniqueId / start URLs). Open it in Designer, <b>Push</b> the definition to the runtime, " +
            "then <b>Show in My Tawala</b> — " +
            "after that, Deploy here can copy links and embed snippets."
          : "<b>No live start URLs on this project yet.</b> Deploy share needs a :8080 uniqueId and start points. " +
            "From Designer, <b>Push</b> the project → <b>Show in My Tawala</b>, " +
            "or open a seeded live project such as <b>Online Exam Builder</b>.") +
        "</p>" +
        '<p class="pm-hint tawala-modal-hint-tight">Use is for trying the form yourself; Publish puts a copy in the public Library. ' +
        "Deploy (this dialog) is for administrators sharing with participants.</p>" +
        "</div>" +
        '<div class="tawala-modal-actions">' +
        '<button type="button" class="pm-action is-active" id="deployShareClose">Close</button>' +
        "</div>";
    } else {
      const options = starts
        .map((sp, i) => {
          const label = sp.label || sp.form || `Start ${i + 1}`;
          return `<option value="${i}">${escapeHtml(label)}</option>`;
        })
        .join("");
      bodyHtml =
        '<div class="tawala-modal-body">' +
        '<p class="pm-hint tawala-modal-hint-tight">Pick a start point (multi-start apps like Online Exam often have Exam vs Admin/Setup). ' +
        "Defaults to the first listed start.</p>" +
        '<label class="tawala-modal-field" for="deployShareStartSelect">Start point' +
        `<select id="deployShareStartSelect">${options}</select>` +
        "</label>" +
        '<label class="tawala-modal-field" for="deployShareLink">Form / start link (email to participants)' +
        '<textarea id="deployShareLink" class="tawala-modal-code" rows="2" readonly></textarea>' +
        "</label>" +
        '<p class="tawala-modal-inline-actions">' +
        '<button type="button" class="pm-action is-active" id="deployShareCopyLink">Copy link</button>' +
        "</p>" +
        '<label class="tawala-modal-field" for="deployShareEmbed">Include in Web Page (iframe embed)' +
        '<textarea id="deployShareEmbed" class="tawala-modal-code" rows="4" readonly></textarea>' +
        "</label>" +
        '<p class="tawala-modal-inline-actions">' +
        '<button type="button" class="pm-action is-active" id="deployShareCopyEmbed">Copy embed</button>' +
        "</p>" +
        '<p class="pm-hint tawala-modal-hint-tight">Paste the embed into your site HTML. ' +
        "Participants open the same :8080 start URL as the copied link.</p>" +
        "</div>" +
        '<div class="tawala-modal-actions">' +
        '<button type="button" class="pm-action" id="deployShareClose">Close</button>' +
        "</div>";
    }

    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = DEPLOY_SHARE_MODAL_ID;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish tawala-modal--deploy-share" role="dialog" ' +
      'aria-modal="true" aria-labelledby="deployShareModalTitle">' +
      '<h3 id="deployShareModalTitle">Deploy — share with participants</h3>' +
      `<p class="pm-hint tawala-modal-lede">For administrators: go live for <b>others</b> on “${escapeHtml(displayName)}”. ` +
      "Copy a form link or embed a start URL. This is not Publish (Library) and not Designer Push.</p>" +
      bodyHtml +
      "</div>";
    document.body.appendChild(backdrop);
    document.addEventListener("keydown", handleDeployShareModalKeydown, true);

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closeDeployShareModal();
    });
    const closeBtn = backdrop.querySelector("#deployShareClose");
    if (closeBtn) closeBtn.addEventListener("click", closeDeployShareModal);

    if (!hasLive) {
      if (closeBtn) closeBtn.focus();
      return;
    }

    const startSelect = backdrop.querySelector("#deployShareStartSelect");
    const linkArea = backdrop.querySelector("#deployShareLink");
    const embedArea = backdrop.querySelector("#deployShareEmbed");

    function syncFields() {
      const idx = Number(startSelect.value) || 0;
      const sp = starts[idx] || starts[0];
      const url = (sp && sp.url) || "";
      const label = (sp && (sp.label || sp.form)) || displayName;
      linkArea.value = url;
      embedArea.value = iframeEmbedSnippet(url, label);
    }
    syncFields();
    startSelect.addEventListener("change", syncFields);
    startSelect.focus();

    backdrop.querySelector("#deployShareCopyLink").addEventListener("click", () => {
      void copyUrlToClipboard(linkArea.value).then(() => {
        setStatus("Copied Deploy start link.");
      });
    });
    backdrop.querySelector("#deployShareCopyEmbed").addEventListener("click", async () => {
      const text = embedArea.value;
      if (!text) {
        window.alert("No embed snippet yet — pick a start point with a :8080 URL.");
        return;
      }
      try {
        if (navigator.clipboard && navigator.clipboard.writeText) {
          await navigator.clipboard.writeText(text);
        } else {
          window.prompt("Copy this iframe embed:", text);
          return;
        }
        setStatus("Copied iframe embed snippet.");
      } catch {
        window.prompt("Copy this iframe embed:", text);
      }
    });
  }

  /**
   * Publish dialog (My Tawala → Library, owner Aug 1, 2026). Editable name (default = current
   * project name, " (stub)" suffix stripped since a Publish target is never itself a stub).
   * Target picker lists every current Library entry — stub or not — with an explicit label of
   * what choosing it will do, so a name/slug collision (e.g. a Deploy-named "Signup sheet" vs
   * the catalog "Sign-up Sheet Template") never silently replaces the wrong row. Preselects only
   * on an *exact* id/name match; anything looser is surfaced but left for the owner to confirm.
   */
  function openPublishDialog(projectId) {
    if (typeof TawalaTransfer === "undefined" || typeof TawalaDemo === "undefined") {
      window.alert("Publish isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    const project = projectId && TawalaDemo.getMyTawala ? TawalaDemo.getMyTawala(projectId) : null;
    if (!project) {
      window.alert(`Can't publish — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    closePublishModal();

    const currentName = TawalaDemo.displayName(project.name || projectId);
    const defaultName = TawalaTransfer.stripStubSuffix(currentName);
    const allCandidates =
      typeof TawalaTransfer.libraryReplaceCandidates === "function"
        ? TawalaTransfer.libraryReplaceCandidates()
        : [];

    function optionsForName(nameVal) {
      const matches =
        typeof TawalaTransfer.findMatchingLibraryTargets === "function"
          ? TawalaTransfer.findMatchingLibraryTargets(nameVal, projectId, project.name)
          : [];
      const exact = matches.filter((m) => m.matchKind === "exact");
      return { html: publishTargetOptionsHtml(matches, allCandidates), preselectId: exact.length === 1 ? exact[0].id : "" };
    }

    const initial = optionsForName(defaultName);

    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = PUBLISH_MODAL_ID;
    backdrop.innerHTML =
      '<div class="tawala-modal tawala-modal--publish" role="dialog" aria-modal="true" aria-labelledby="publishModalTitle">' +
      `<h3 id="publishModalTitle">Publish to Library</h3>` +
      `<p class="pm-hint tawala-modal-lede">Publishing “${escapeHtml(currentName)}” copies it into the public Library (mock — browser overlay; see README § Publish).</p>` +
      '<div class="tawala-modal-body">' +
      '<div class="tawala-modal-grid">' +
      '<label class="tawala-modal-field" for="publishNameInput">Library name' +
      `<input type="text" id="publishNameInput" value="${escapeHtml(defaultName)}" />` +
      "</label>" +
      '<label class="tawala-modal-field" for="publishCategorySelect">Library category (required)' +
      `<select id="publishCategorySelect" title="Defaults to the replace target\u2019s category, or Uncategorized">${publishCategoryOptionsHtml(defaultPublishCategory(null, project))}</select>` +
      "</label>" +
      '<label class="tawala-modal-field tawala-modal-field--full" for="publishStubSelect">Replace an existing Library project (optional)' +
      `<select id="publishStubSelect">${initial.html}</select>` +
      "</label>" +
      "</div>" +
      '<p class="pm-hint tawala-modal-hint-tight">Stubs retire to My Tawala marked <code>(stub)</code> (never hard-deleted). Non-stubs overlay in place. No public Library Delete.</p>' +
      '<label class="tawala-modal-checkbox" for="publishPurgeCheckbox">' +
      '<input type="checkbox" id="publishPurgeCheckbox" checked /> Purge responses when publishing' +
      "</label>" +
      '<p class="pm-hint tawala-modal-hint-tight">Clears this project\u2019s :8080 submissions after Publish (same as My Tawala <b>Purge</b>). Leave checked unless you have a reason not to.</p>' +
      '<p class="pm-hint" id="publishModalError" role="alert" style="display:none;"></p>' +
      "</div>" +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="publishModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="publishModalConfirm">Publish</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);
    document.addEventListener("keydown", handlePublishModalKeydown, true);

    const nameInput = backdrop.querySelector("#publishNameInput");
    const stubSelect = backdrop.querySelector("#publishStubSelect");
    const categorySelect = backdrop.querySelector("#publishCategorySelect");
    stubSelect.value = initial.preselectId;
    nameInput.focus();
    nameInput.select();

    /* Category follows the replace-target pick until the owner explicitly overrides it —
     * mirrors "Default: current category of replace-target if replacing" from the spec. */
    let categoryTouchedByUser = false;
    categorySelect.addEventListener("change", () => {
      categoryTouchedByUser = true;
    });
    function syncCategoryToTarget() {
      if (categoryTouchedByUser) return;
      const target = stubSelect.value ? allCandidates.find((c) => c.id === stubSelect.value) : null;
      const label = defaultPublishCategory(target, project);
      categorySelect.innerHTML = publishCategoryOptionsHtml(label);
    }
    syncCategoryToTarget();
    stubSelect.addEventListener("change", syncCategoryToTarget);

    nameInput.addEventListener("input", () => {
      const prevChoice = stubSelect.value;
      const next = optionsForName(nameInput.value);
      stubSelect.innerHTML = next.html;
      // Keep an explicit owner choice even if it drops out of the "exact" bucket on edit;
      // only fall back to the fresh preselect when nothing was chosen yet.
      const stillPresent = Array.from(stubSelect.options).some((o) => o.value === prevChoice);
      stubSelect.value = prevChoice && stillPresent ? prevChoice : next.preselectId;
      syncCategoryToTarget();
    });

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closePublishModal();
    });
    backdrop.querySelector("#publishModalCancel").addEventListener("click", closePublishModal);

    backdrop.querySelector("#publishModalConfirm").addEventListener("click", async () => {
      const nameVal = nameInput.value.trim();
      const errEl = backdrop.querySelector("#publishModalError");
      if (!nameVal) {
        errEl.textContent = "Enter a name for the published Library project.";
        errEl.style.display = "";
        return;
      }
      const replaceLibraryId = stubSelect.value || "";
      const target = replaceLibraryId ? allCandidates.find((c) => c.id === replaceLibraryId) : null;
      const categoryVal = categorySelect.value.trim();
      if (!categoryVal) {
        errEl.textContent = "Pick a Library category for the published project.";
        errEl.style.display = "";
        return;
      }
      const purgeOnPublish = !!backdrop.querySelector("#publishPurgeCheckbox").checked;
      const confirmMsg =
        `Publish “${nameVal}” to the public Library (category: ${categoryVal})?` +
        (target
          ? target.stub
            ? `\n\nThis retires the stub “${target.name}” — removed from Library, kept in My Tawala marked (stub).`
            : `\n\nThis replaces the existing Library entry “${target.name}” with this version (overlay only, old entry not moved to My Tawala).`
          : "") +
        (purgeOnPublish
          ? "\n\nThis project's :8080 responses will be purged right after publishing."
          : "\n\nResponses will NOT be purged (checkbox unchecked).");
      if (!window.confirm(confirmMsg)) return;

      const result = TawalaTransfer.publishToLibrary({
        sourceProjectId: projectId,
        name: nameVal,
        replaceLibraryId: replaceLibraryId || null,
        category: categoryVal,
      });
      closePublishModal();
      if (!result || !result.ok) {
        const msg = `Couldn't publish “${nameVal}”\n\n${(result && result.error) || "Unknown error"}`;
        setStatus(msg.split("\n")[0]);
        window.alert(msg);
        return;
      }
      const parts = [`Published “${nameVal}” to the Library.`];
      if (result.retired) {
        parts.push(`Retired stub “${result.retired.name}” → moved to My Tawala (kept marked (stub)).`);
      } else if (result.replacedNonStub) {
        parts.push(`Replaced existing Library entry “${result.replacedName || replaceLibraryId}” in place.`);
      }

      let purgeResult = null;
      if (purgeOnPublish && typeof TawalaDemo.purgeAfterPublish === "function") {
        setStatus(parts.join(" ") + " Purging responses…");
        purgeResult = await TawalaDemo.purgeAfterPublish(projectId);
        if (purgeResult.status === "success") {
          const n =
            purgeResult.javaDb && purgeResult.javaDb.deleted != null ? purgeResult.javaDb.deleted : "?";
          parts.push(`Purged prior responses (deleted ${n} submission row(s)) — no one else will see them.`);
        } else if (purgeResult.status === "skipped") {
          parts.push(
            `⚠️ Responses were NOT purged — “${nameVal}” isn’t linked to a live :8080 deploy yet.`
          );
        } else {
          parts.push(
            `⚠️ Responses were NOT purged — purge failed: ${purgeResult.error || "unknown error"}.`
          );
        }
      } else if (!purgeOnPublish) {
        parts.push("Responses were not purged (checkbox unchecked).");
      }

      const msg = parts.join(" ");
      setStatus(msg);
      window.alert(msg);
      document.dispatchEvent(
        new CustomEvent("tawala:project-published", {
          detail: { sourceProjectId: projectId, result, purge: purgeResult },
        })
      );
    });
  }

  const PULL_MODAL_ID = "tawalaPullModal";

  function closePullModal() {
    const el = document.getElementById(PULL_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handlePullModalKeydown, true);
  }

  function handlePullModalKeydown(ev) {
    if (ev.key === "Escape") closePullModal();
  }

  /** Option label — Pull always shows what's already known: name + last-updated. */
  function pullTargetOptionLabel(candidate, matchFlag) {
    const name = escapeHtml(candidate.name);
    const updated = candidate.updated ? ` (updated ${escapeHtml(candidate.updated)})` : "";
    return `${name}${updated}${matchFlag ? ` — ${matchFlag} match` : ""}`;
  }

  function pullTargetOptionsHtml(matches, allCandidates) {
    const matchIds = new Set(matches.map((m) => m.id));
    const exact = matches.filter((m) => m.matchKind === "exact");
    const loose = matches.filter((m) => m.matchKind === "loose");
    const rest = allCandidates
      .filter((c) => !matchIds.has(c.id))
      .sort((a, b) => a.name.localeCompare(b.name));

    let html = '<option value="">— Pick a Library project —</option>';
    if (exact.length || loose.length) {
      html += '<optgroup label="Possible matches">';
      html += exact.map((m) => `<option value="${escapeHtml(m.id)}">${pullTargetOptionLabel(m, "exact")}</option>`).join("");
      html += loose.map((m) => `<option value="${escapeHtml(m.id)}">${pullTargetOptionLabel(m, "possible")}</option>`).join("");
      html += "</optgroup>";
    }
    if (rest.length) {
      html += '<optgroup label="Other Library projects">';
      html += rest.map((c) => `<option value="${escapeHtml(c.id)}">${pullTargetOptionLabel(c, null)}</option>`).join("");
      html += "</optgroup>";
    }
    return html;
  }

  /**
   * Pull dialog (Library → My Tawala upgrade, owner Aug 1, 2026 — was a grey stub). Picks a
   * Library project to refresh THIS My Tawala project's content from — see
   * TawalaTransfer.pullFromLibrary for exactly what gets overwritten vs preserved (this
   * project's own name / deploy identity / submissions are never touched).
   */
  function openPullDialog(projectId) {
    if (typeof TawalaTransfer === "undefined" || typeof TawalaDemo === "undefined") {
      window.alert("Pull isn't available — required scripts didn't load. Refresh and try again.");
      return;
    }
    const project = projectId && TawalaDemo.getMyTawala ? TawalaDemo.getMyTawala(projectId) : null;
    if (!project) {
      window.alert(`Can't pull — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    closePullModal();

    const displayName = TawalaDemo.displayName(project.name || projectId);
    const allCandidates =
      typeof TawalaTransfer.libraryReplaceCandidates === "function"
        ? TawalaTransfer.libraryReplaceCandidates()
        : [];
    if (!allCandidates.length) {
      window.alert("No Library projects are available to pull from right now.");
      return;
    }
    const matches =
      typeof TawalaTransfer.findPullCandidates === "function"
        ? TawalaTransfer.findPullCandidates(projectId, project.name)
        : [];
    const exact = matches.filter((m) => m.matchKind === "exact");
    const preselectId = exact.length === 1 ? exact[0].id : "";

    const backdrop = document.createElement("div");
    backdrop.className = "tawala-modal-backdrop";
    backdrop.id = PULL_MODAL_ID;
    const link =
      typeof resolveLibraryLink === "function" ? resolveLibraryLink(projectId) : null;
    const linkedPreselect =
      link && allCandidates.some((c) => c.id === link.id) ? link.id : "";
    const initialSelect = linkedPreselect || preselectId;

    backdrop.innerHTML =
      '<div class="tawala-modal" role="dialog" aria-modal="true" aria-labelledby="pullModalTitle">' +
      `<h3 id="pullModalTitle">Refresh from Library</h3>` +
      `<p class="pm-hint">Refreshes “${escapeHtml(displayName)}” with a public Library project's current description, category, and reference start points (mock — browser overlay only).</p>` +
      '<label class="tawala-modal-field" for="pullSourceSelect">Refresh content from' +
      `<select id="pullSourceSelect">${pullTargetOptionsHtml(matches, allCandidates)}</select>` +
      "</label>" +
      '<p class="pm-hint">Keeps this project\u2019s own name, rating, comments, and — importantly — its own <code>:8080</code> deploy / submission data. Only descriptive content is refreshed; use EXPORT/BACKUP first if you want a safety copy.</p>' +
      '<p class="pm-hint" id="pullModalError" role="alert" style="display:none;"></p>' +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="pullModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="pullModalConfirm">Refresh</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);
    document.addEventListener("keydown", handlePullModalKeydown, true);

    const sourceSelect = backdrop.querySelector("#pullSourceSelect");
    sourceSelect.value = initialSelect;
    sourceSelect.focus();

    backdrop.addEventListener("click", (ev) => {
      if (ev.target === backdrop) closePullModal();
    });
    backdrop.querySelector("#pullModalCancel").addEventListener("click", closePullModal);

    backdrop.querySelector("#pullModalConfirm").addEventListener("click", () => {
      const libraryId = sourceSelect.value;
      const errEl = backdrop.querySelector("#pullModalError");
      if (!libraryId) {
        errEl.textContent = "Pick a Library project to pull from.";
        errEl.style.display = "";
        return;
      }
      const source = allCandidates.find((c) => c.id === libraryId);
      const sourceName = source ? source.name : libraryId;
      const confirmMsg =
        `Refresh “${displayName}” from Library “${sourceName}”?\n\n` +
        "This overwrites the description, category, and reference start points on this My Tawala project. " +
        "Your own name, rating, comments, and :8080 deploy / submission data are not touched.";
      if (!window.confirm(confirmMsg)) return;

      const result = TawalaTransfer.pullFromLibrary({ myTawalaProjectId: projectId, libraryId });
      closePullModal();
      if (!result || !result.ok) {
        const msg = `Couldn't refresh from “${sourceName}”\n\n${(result && result.error) || "Unknown error"}`;
        setStatus(msg.split("\n")[0]);
        window.alert(msg);
        return;
      }
      const msg = `Refreshed “${displayName}” from “${result.sourceName}” — description, category, and reference start points updated.`;
      setStatus(msg);
      window.alert(msg);
      document.dispatchEvent(
        new CustomEvent("tawala:project-pulled", { detail: { projectId, result } })
      );
    });
  }

  async function handleOpClick(ev) {
    const btn = ev.target.closest("[data-op], .pm-action, .pm-icon-action");
    if (!btn || !btn.dataset) return;
    const wired = btn.dataset.wired;
    const confirmId = btn.dataset.confirm;
    const projectId = btn.dataset.project || "";
    const op = btn.dataset.op || "";
    const scopeOff =
      !!btn.disabled ||
      btn.classList.contains("is-scope-disabled") ||
      btn.getAttribute("aria-disabled") === "true";

    if (wired === "false" || !wired) return;

    /*
     * Use: multi-start → Project Details (same mock host); single-start / banner start
     * → :8080. Probe Tomcat first so a dead :8080 never silently dumps the owner off :5500.
     * preventDefault must run before any await (popup / navigation race).
     */
    if (wired === "use-project") {
      if (scopeOff) {
        ev.preventDefault();
        return;
      }
      const href = btn.tagName === "A" ? btn.getAttribute("href") || "" : "";
      const mode = btn.dataset.useMode || "";
      if (mode === "details" || (href && !isLocalJavaRuntimeUrl(href))) {
        return;
      }
      if (href && isLocalJavaRuntimeUrl(href)) {
        ev.preventDefault();
        void openUseRuntimeUrl(href, { projectId });
      }
      return;
    }

    if (wired === "copy-start-link") {
      ev.preventDefault();
      if (scopeOff) return;
      const tree = document.getElementById("pmDataTree");
      const sel = effectiveDataSelection(tree);
      if (sel.kind === "start" && sel.url) {
        void copyUrlToClipboard(sel.url);
      } else {
        window.alert("Select a start point (▶) first, then Copy link.");
      }
      return;
    }

    /* Soft-disabled Backup/Restore/etc. — ignore click; title explains why. */
    if (scopeOff) {
      ev.preventDefault();
      return;
    }

    if (op === "expand-all" || op === "collapse-all") {
      const open = op === "expand-all";
      document.querySelectorAll("details.library-category").forEach((el) => {
        el.open = open;
      });
      return;
    }

    if (wired === "edit-categories" || op === "edit-categories") {
      /* library.html listens for data-op=edit-categories and opens the editor. */
      document.dispatchEvent(new CustomEvent("tawala:edit-categories"));
      return;
    }

    if (wired === "publish-mytawala") {
      openPublishDialog(projectId);
      return;
    }

    if (wired === "deploy-share") {
      openDeployShareDialog(projectId);
      return;
    }

    if (wired === "save-copy-library") {
      ev.preventDefault();
      if (!isMockLoggedIn()) {
        window.alert(
          "Save to MyTawala requires a free account.\n\nLog in or Register, then try again."
        );
        return;
      }
      /* data-project on Library surfaces is the public Library id. */
      openSaveCopyDialog(projectId);
      return;
    }

    if (wired === "test-drive") {
      ev.preventDefault();
      if (!projectId) return;
      const project = resolveLibraryProject(projectId);
      if (project && isLibraryMultiStart(project)) {
        openLibraryTestDrivePicker(projectId, { intent: "open" });
        return;
      }
      const url =
        (btn && btn.getAttribute && btn.getAttribute("data-testdrive-url")) ||
        libraryDriveUrl(project) ||
        "";
      if (!url) {
        window.alert("No local test-drive yet — this project isn’t live on :8080.");
        return;
      }
      noteLibraryTestDriveOpen(projectId);
      if (typeof TawalaDemo !== "undefined" && typeof TawalaDemo.openTestDrive === "function") {
        void TawalaDemo.openTestDrive(url, { purge: true });
      } else {
        window.open(url, "_blank", "noopener");
      }
      return;
    }

    if (wired === "copy-testdrive-link" || op === "copy-testdrive-link") {
      ev.preventDefault();
      const explicit =
        (btn && btn.getAttribute && btn.getAttribute("data-testdrive-copy-url")) || "";
      void copyLibraryTestDriveLink(projectId, explicit);
      return;
    }

    if (wired === "rename-project" || op === "rename") {
      ev.preventDefault();
      let renameId = projectId;
      if (!renameId) {
        const host = document.getElementById("pmDetail");
        renameId = (host && host.dataset.projectId) || "";
      }
      openRenameDialog(renameId);
      return;
    }

    if (wired === "make-copy-mytawala" || op === "make-copy") {
      ev.preventDefault();
      openMakeCopyDialog(projectId);
      return;
    }

    if (wired === "edit-in-designer") {
      openEditInDesigner(projectId);
      return;
    }

    if (wired === "deactivate-project") {
      toggleProjectActive(projectId, false);
      return;
    }

    if (wired === "activate-project") {
      toggleProjectActive(projectId, true);
      return;
    }

    if (wired === "get-from-library") {
      ev.preventDefault();
      openGetFromLibraryDialog();
      return;
    }

    if (wired === "pull-library") {
      openPullDialog(projectId);
      return;
    }

    if (wired === "download-version") {
      downloadSelectedVersion(projectId);
      return;
    }

    if (wired === "deploy-version") {
      if (btn.dataset.busy === "1") return;
      btn.dataset.busy = "1";
      const prevDisabled = btn.disabled;
      btn.disabled = true;
      try {
        await deploySelectedVersion(projectId);
      } finally {
        btn.dataset.busy = "";
        btn.disabled = prevDisabled;
        syncDeployVersionChip(document.getElementById("pmDetailHost") || document);
      }
      return;
    }

    if (
      wired === "export-mytawala" ||
      wired === "import-mytawala" ||
      wired === "backup-mytawala" ||
      wired === "restore-mytawala"
    ) {
      if (typeof TawalaDataOps === "undefined") {
        setStatus("Data ops unavailable — js/data-ops.js didn't load.");
        window.alert("Couldn't run this action\n\nData ops support script didn't load. Refresh the page and try again.");
        return;
      }
      if (btn.dataset.busy === "1") return;
      btn.dataset.busy = "1";
      const prevDisabled = btn.disabled;
      btn.disabled = true;
      const formName =
        btn.dataset.dataScope === "form" && btn.dataset.formName ? btn.dataset.formName : "";
      try {
        if (wired === "export-mytawala") {
          await TawalaDataOps.handleExportClick(projectId, formName ? { formName } : undefined);
        } else if (wired === "import-mytawala") {
          await TawalaDataOps.handleImportClick(projectId, formName ? { formName } : undefined);
        } else if (wired === "backup-mytawala") await TawalaDataOps.handleBackupClick(projectId);
        else if (wired === "restore-mytawala") await TawalaDataOps.handleRestoreClick(projectId);
      } finally {
        btn.dataset.busy = "";
        btn.disabled = prevDisabled;
      }
      return;
    }

    if (wired === "purge-local") {
      const formName =
        btn.dataset.dataScope === "form" && btn.dataset.formName ? btn.dataset.formName : "";
      const project =
        typeof TawalaDemo !== "undefined" && projectId && TawalaDemo.getMyTawala
          ? TawalaDemo.getMyTawala(projectId)
          : typeof TawalaDemo !== "undefined" && projectId && TawalaDemo.get
            ? TawalaDemo.get(projectId)
            : null;
      const displayName =
        project && typeof TawalaDemo !== "undefined" && TawalaDemo.displayName
          ? TawalaDemo.displayName(project.name || projectId)
          : projectId || "project";

      /* Form-scoped Purge = export → drop form → replace (data-ops). Whole-project uses API purge. */
      if (formName) {
        if (typeof TawalaDataOps === "undefined" || typeof TawalaDataOps.handleFormPurgeClick !== "function") {
          setStatus("Form purge unavailable — js/data-ops.js didn't load.");
          window.alert("Couldn't purge form\n\nData ops support script didn't load. Refresh and try again.");
          return;
        }
        const okForm = await showPurgeConfirm({ displayName, formName });
        if (!okForm) return;
        if (btn.dataset.busy === "1") return;
        btn.dataset.busy = "1";
        const prevDisabled = btn.disabled;
        btn.disabled = true;
        try {
          await TawalaDataOps.handleFormPurgeClick(projectId, formName, { confirmed: true });
        } finally {
          btn.dataset.busy = "";
          btn.disabled = prevDisabled;
        }
        return;
      }

      const okProject = await showPurgeConfirm({ displayName });
      if (!okProject) return;

      /* Purge = clear :8080 submission data only — not Delete (row remove). */
      if (typeof TawalaDemo === "undefined" || !TawalaDemo.purgeResponses) {
        setStatus(`PURGE unavailable — purge support not loaded.`);
        window.alert(`Couldn't purge\n\nPurge support isn’t loaded. Refresh the page and try again.`);
        return;
      }
      const uniqueId =
        typeof TawalaDemo.resolvePurgeUniqueId === "function"
          ? TawalaDemo.resolvePurgeUniqueId(projectId)
          : TawalaDemo.uniqueIdForProject
            ? TawalaDemo.uniqueIdForProject(project)
            : null;
      if (!uniqueId) {
        const hint = project
          ? `“${displayName}” isn’t linked to a live :8080 deploy yet. Push from Designer (or use a deployed project), then try Purge again.`
          : `Unknown My Tawala project: ${projectId || "(none)"}`;
        setStatus(`PURGE for “${displayName}” — not linked to a live deploy.`);
        window.alert(`Couldn't purge “${displayName}”\n\n${hint}`);
        return;
      }
      if (btn.dataset.busy === "1") return;
      btn.dataset.busy = "1";
      const prevDisabled = btn.disabled;
      btn.disabled = true;
      setStatus(`Purging “${displayName}”…`);
      try {
        const result = await TawalaDemo.purgeResponses(uniqueId);
        if (result.status === "success") {
          const n =
            result.javaDb && result.javaDb.deleted != null ? result.javaDb.deleted : "?";
          const warn = result.warning ? `\n\n${result.warning}` : "";
          const msg = `Purged “${displayName}” — deleted ${n} submission row(s).${warn}`;
          setStatus(
            `Purged “${displayName}” — deleted ${n} submission row(s).` +
              (result.source === "mock-seed" ? " (demo Records)" : "")
          );
          window.alert(msg);
          document.dispatchEvent(
            new CustomEvent("tawala:project-purged", {
              detail: { projectId, uniqueId, result, displayName },
            })
          );
        } else {
          const err = result.error || "unknown error";
          setStatus(`Couldn't purge “${displayName}”: ${err}`);
          window.alert(`Couldn't purge “${displayName}”\n\n${err}\n\n${LOCAL_PURGE_HELP}`);
        }
      } finally {
        btn.dataset.busy = "";
        btn.disabled = prevDisabled;
      }
      return;
    }

    if (confirmId) {
      const ok = await showConfirm(confirmId);
      if (!ok) return;
    }

    if (wired === "delete-mytawala" || wired === "delete-mock") {
      /* Delete = remove this account’s My Tawala row (not Purge submissions; not Library). */
      let result = { ok: false, projectId, hadOverlay: false, hadInbox: false };
      if (
        typeof TawalaTransfer !== "undefined" &&
        typeof TawalaTransfer.deleteMyTawalaProject === "function"
      ) {
        result = TawalaTransfer.deleteMyTawalaProject(projectId);
      } else if (
        typeof TawalaTransfer !== "undefined" &&
        TawalaTransfer.removeMyTawalaOverlay
      ) {
        const hadOverlay = !!(
          TawalaTransfer.getOverlayEntry && TawalaTransfer.getOverlayEntry(projectId)
        );
        TawalaTransfer.removeMyTawalaOverlay(projectId);
        result = { ok: true, projectId, hadOverlay, hadInbox: false };
      }
      const onListing = !!document.getElementById("projectRows");
      const onDetails = !!document.getElementById("pmDetailHost");
      setStatus(
        `Deleted “${projectId || "project"}” from My Tawala` +
          (result.hadOverlay || result.hadInbox
            ? " (cleared Deploy overlay / inbox)."
            : " (private pile).") +
          " Public Library unchanged."
      );
      document.dispatchEvent(
        new CustomEvent("tawala:project-deleted", {
          detail: { projectId, result, source: onDetails && !onListing ? "details" : "listing" },
        })
      );
      if (onDetails && !onListing) {
        window.location.href = "mytawala.html";
      }
    }
  }

  function bind(root) {
    const scope = root || document;
    /* Capture phase so Library Actions still fire if a parent later re-adds stopPropagation
     * (row navigation must not swallow Save a copy). */
    scope.addEventListener(
      "click",
      (ev) => {
        if (ev.target.closest("#pmDataTree")) {
          handleDataTreeClick(ev);
        }
        if (ev.target.closest("[data-op], .pm-action, .pm-icon-action")) {
          void handleOpClick(ev);
        }
      },
      true
    );
    /* Project Details title — double-click opens Rename (My Tawala overlay). */
    scope.addEventListener("dblclick", (ev) => {
      const title =
        ev.target.closest && ev.target.closest('h2.pm-detail-title[data-wired="rename-project"]');
      if (title) {
        ev.preventDefault();
        openRenameDialog(title.dataset.project || "");
        return;
      }
      const meta =
        ev.target.closest && ev.target.closest('.pm-detail-meta[data-wired="edit-description"]');
      if (!meta) return;
      ev.preventDefault();
      openEditDescriptionDialog(meta.dataset.project || "");
    });
    scope.addEventListener("keydown", (ev) => {
      const title =
        ev.target.closest && ev.target.closest('h2.pm-detail-title[data-wired="rename-project"]');
      if (title && ev.key === "Enter" && ev.target === title) {
        ev.preventDefault();
        openRenameDialog(title.dataset.project || "");
        return;
      }
      const meta =
        ev.target.closest && ev.target.closest('.pm-detail-meta[data-wired="edit-description"]');
      if (!meta || ev.key !== "Enter" || ev.target !== meta) return;
      ev.preventDefault();
      openEditDescriptionDialog(meta.dataset.project || "");
    });
    scope.addEventListener("change", (ev) => {
      const sel = ev.target.closest && ev.target.closest('select[data-wired="theme-select"]');
      if (!sel) return;
      void applyThemeSelection(sel.dataset.project || "", sel.value);
    });
    scope.addEventListener("keydown", (ev) => {
      const row = ev.target.closest && ev.target.closest("#pmDataTree [data-pm-sel]");
      if (!row || (ev.key !== "Enter" && ev.key !== " ")) return;
      if (ev.target.closest("a, button, input, textarea")) return;
      ev.preventDefault();
      const tree = row.closest("#pmDataTree");
      setDataTreeSelection(tree, row.dataset.pmSel || "project", row);
    });
    scope.addEventListener("focusin", (ev) => {
      const input = ev.target.closest && ev.target.closest(".pm-version-desc-input");
      if (!input) return;
      if (input.dataset.savedValue == null) {
        input.dataset.savedValue = String(input.value || "").trim();
      }
    });
    scope.addEventListener("change", (ev) => {
      const input = ev.target.closest && ev.target.closest(".pm-version-desc-input");
      if (input) saveVersionDescriptionFromInput(input);
      if (ev.target && ev.target.name === "pmVersionPick") {
        syncDeployVersionChip(document.getElementById("pmDetailHost") || scope);
      }
    });
    scope.addEventListener("click", (ev) => {
      /* Radios sometimes fire click without change when re-selecting; keep chip in sync. */
      if (ev.target && ev.target.name === "pmVersionPick") {
        syncDeployVersionChip(document.getElementById("pmDetailHost") || scope);
      }
    });
    scope.addEventListener("keydown", (ev) => {
      const input = ev.target.closest && ev.target.closest(".pm-version-desc-input");
      if (!input) return;
      if (ev.key === "Enter") {
        ev.preventDefault();
        input.blur();
      } else if (ev.key === "Escape") {
        ev.preventDefault();
        input.value = input.dataset.savedValue != null ? input.dataset.savedValue : input.defaultValue;
        input.blur();
      }
    });
  }

  window.TawalaProjectOps = {
    MYTAWALA_SUBMENU,
    LIBRARY_SUBMENU,
    LIBRARY_CATEGORY_STUBS,
    LIBRARY_ADMIN_STUBS,
    LIBRARY_LISTING_ACTIONS,
    PROJECT_ACTIONS,
    PROJECT_DATA_CONTROLS,
    PROJECT_THEMES,
    LISTING_BAR_ACTIONS,
    LISTING_ACTIONS,
    LISTING_ICONS,
    PROJECT_SIDEBAR_OPS,
    PROJECT_DATA_OPS,
    VERSION_OPS,
    OTHER_OPS,
    RELATED_SAVE_CLONE,
    OPS_CATALOG_SECTIONS,
    CONFIRMS,
    ARCHIVE_NOTE,
    LOCAL_PURGE_HELP,
    resolveLibraryLink,
    renderSubmenu,
    renderLibrarySubmenu,
    renderLibraryChromeStubs,
    renderListingControls,
    renderListingControlCells,
    renderListingActionHeaders,
    renderListingSelectionBar,
    syncListingSelectionBar,
    renderLibraryListingControlCells,
    renderLibraryListingActionHeaders,
    renderLibraryListingControls,
    renderDetailPanel,
    fromLibraryAcquireBadge,
    fromLibraryProvenanceLine,
    fromDesignerProvenanceLine,
    designerSourceDisplayName,
    syncDeployVersionChip,
    hydrateProjectDataTree,
    hydrateThemeFromProjectJson,
    resolveProjectThemePath,
    syncProjectActionsForDataSelection,
    applyOpenStartsEntry,
    renderOpsCatalog,
    openPublishDialog,
    openDeployShareDialog,
    openLibraryTestDrivePicker,
    openSaveCopyDialog,
    openMakeCopyDialog,
    openGetFromLibraryDialog,
    openRenameDialog,
    openEditDescriptionDialog,
    renderLibrarySaveCopyButton,
    renderLibraryTestDriveButton,
    renderLibraryCopyTestDriveButton,
    bind,
  };
})();
