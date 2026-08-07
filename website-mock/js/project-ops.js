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
 * wired: "pull-library" → openPullDialog — pick a Library project, confirm, then
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

  /**
   * Public Library listing row actions — discovery / acquire only (owner Aug 1, 2026).
   * Single “Actions” column: two icons + column sub-labels under the Actions header
   * (Test drive | Save a copy). **Use** lives on My Tawala (operate), not Library.
   * Test drive still purges-on-start. Save a copy remains a grey stub.
   * wired: "test-drive" → active when project has a live :8080 URL; else disabled grey.
   */
  const LIBRARY_LISTING_ACTIONS = [
    {
      id: "test-drive",
      label: "Test drive",
      title: "Test drive this project (local :8080)",
      wired: "test-drive",
      icon: "testdrive",
    },
    {
      id: "save-my-tawala",
      label: "Save a copy",
      title: "Save this project under My Tawala",
      wired: false,
      icon: "save",
    },
  ];

  /**
   * Project Actions bar on Project Details (detail.jsp) — exact button labels.
   * Titles from button title= attributes.
   *
   * No PUBLISH here (owner Aug 1, 2026): Publish only appears where the owner has the
   * My Tawala **listing** row in front of them (icon strip) — Details stays USE…PULL.
   * See LISTING_ACTIONS below for the wired Publish control.
   *
   * USE (owner Aug 1 / Aug 4, 2026): single-start → open that :8080 URL (no purge);
   * multi-start (Exam+Setup, etc.) → Project Details so the user picks an entry point.
   * Not legacy CloneAndCustomize. Disabled when no deploy URL.
   */
  const PROJECT_ACTIONS = [
    {
      id: "use",
      label: "USE",
      title: "Open / run this project — or choose a start point when there are several",
      wired: "use-project",
    },
    { id: "export", label: "EXPORT", title: "Export project response data (Excel-format mock — see README)", wired: "export-mytawala" },
    { id: "import", label: "IMPORT", title: "Import response data into this project (Excel/JSON mock — field mismatch fails)", wired: "import-mytawala" },
    { id: "backup", label: "BACKUP", title: "Back up this project (definition + data + properties)", wired: "backup-mytawala" },
    { id: "restore", label: "RESTORE", title: "Restore this project from a backup", wired: "restore-mytawala" },
    { id: "purge", label: "PURGE", title: "Purge project data", wired: "purge-local", confirmId: "purge" },
    { id: "delete", label: "DELETE", title: "Delete Project", wired: "delete-mytawala", confirmId: "delete" },
    {
      id: "pull-library",
      label: "PULL FROM LIBRARY",
      title: "Replace this project's content with a newer public Library version",
      wired: "pull-library",
    },
  ];

  /**
   * My Projects listing row — same ops as Project Actions, icon-per-row / label-in-header.
   * (Legacy view.jsp was Purge/Delete only; owner asked for the full listing-appropriate strip.)
   * Use is first: primary “run this project” affordance (My Tawala = operate).
   *
   * Column groups (flat listing — no version piles): Project info (Name…Use in HTML +
   * this strip’s Use) · Data transfer · Backup · Destructive (red) · Library transfer.
   * `groupStart` marks the first op column of a new visual group (CSS left rule + gap).
   */
  const LISTING_ACTIONS = [
    {
      id: "use",
      label: "Use",
      title: "Open / run this project — or choose a start point when there are several",
      wired: "use-project",
      icon: "use",
      group: "info",
    },
    {
      id: "export",
      label: "Export",
      title: "Export project response data (Excel-format mock — see README)",
      wired: "export-mytawala",
      icon: "export",
      group: "transfer",
      groupStart: true,
    },
    {
      id: "import",
      label: "Import",
      title: "Import response data into this project (field mismatch fails)",
      wired: "import-mytawala",
      icon: "import",
      group: "transfer",
    },
    {
      id: "backup",
      label: "Backup",
      title: "Back up this project (definition + data + properties)",
      wired: "backup-mytawala",
      icon: "backup",
      group: "backup",
      groupStart: true,
    },
    {
      id: "restore",
      label: "Restore",
      title: "Restore this project from a backup",
      wired: "restore-mytawala",
      icon: "restore",
      group: "backup",
    },
    {
      id: "purge",
      label: "Purge",
      title: "Purge project data",
      wired: "purge-local",
      confirmId: "purge",
      icon: "purge",
      group: "destructive",
      groupStart: true,
      destructive: true,
    },
    {
      id: "delete",
      label: "Delete",
      title: "Delete project",
      wired: "delete-mytawala",
      confirmId: "delete",
      icon: "delete",
      group: "destructive",
      destructive: true,
    },
    {
      id: "publish",
      label: "Publish",
      title: "Publish this project to the public Library (rename, then optionally replace a stub or outdated Library entry)",
      wired: "publish-mytawala",
      icon: "publish",
      group: "library",
      groupStart: true,
    },
    {
      id: "pull-library",
      label: "Pull",
      title: "Replace this project's content with a newer public Library version",
      wired: "pull-library",
      icon: "pull",
      group: "library",
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
    save:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M6 1.5v6M3.5 5L6 7.5 8.5 5M2.5 10h7" fill="none" stroke="currentColor" stroke-width="1.3" stroke-linecap="round" stroke-linejoin="round"/></svg>',
    /* My Tawala Use — open/run (not CloneAndCustomize) */
    use:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M3.5 2.2v7.6L10 6z" fill="currentColor" stroke="none"/></svg>',
  };

  /** Sidebar / related project detail ops (block-projectManagerProjectDetails.jsp + invite/webpage) */
  const PROJECT_SIDEBAR_OPS = [
    { label: "REVISE PROJECT", title: "Customize / revise a customizable project", wired: false },
    { label: "TAKE PROJECT ONLINE", title: "Project is Inactive → take online", wired: false },
    { label: "TAKE PROJECT OFFLINE", title: "Project is Active → take offline", wired: false },
    { label: "Include Project in Web Page", title: "Incorporate this project into a web page", wired: false },
    { label: "Invite Other People to This Project", title: "Invitation options for start-point links", wired: false },
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
      label: "Deploy this version",
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

  /** Backups / emails / publish dialogs (admin-ish; still useful memory jogs) */
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
    { label: "Save this project under My Tawala", source: "Library → My Tawala (acquire)", wired: false },
    {
      label: "Use (run or pick start point)",
      source:
        "My Tawala listing / Details — single-start opens :8080; multi-start opens Project Details (not Library; not CloneAndCustomize)",
      wired: "use-project",
    },
    { label: "Publish / move to Library", source: "My Tawala → Library", wired: false },
    { label: "Pull from Library", source: "Library → My Tawala upgrade", wired: false },
    { label: "Deploy from Web Designer", source: "Designer :5173 → My Tawala inbox", wired: false },
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
    "Mock My Tawala listing shows Use + Export…Publish icon strip (labels in headers). " +
    "Catalog is split: Public Library controls vs My Tawala / Project Manager. " +
    "Library Actions = Test drive | Save a copy; Use is on My Tawala only (single-start → :8080; multi-start → Project Details). " +
    "SportsDashboards (not SportsBoard).";

  /** One-liner for API/plumbing failures only — never dump CLI / DirtBowl / Test-drive notes into Purge alerts. */
  const LOCAL_PURGE_HELP = "Needs designer-web API on :3001 and Docker Postgres.";

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
        "Library mock — Actions sub-labels: Test drive | Save a copy (icons in rows; Test drive when :8080 deployed). Use moved to My Tawala. searchLibrary.jsp was click-to-detail only.",
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
      title: "My Tawala sub-menu",
      where: "submenu-mytawala.jsp — shallow 3-item bar above listings",
      items: MYTAWALA_SUBMENU,
    },
    {
      surface: "mytawala",
      id: "listing",
      title: "My Projects listing row",
      where:
        "mytawala.html — icon strip per row (labels in column headers): Use · Export · Import · Backup · Restore · Purge · Delete · Publish · Pull. Visual column groups + red Purge/Delete. Use: single-start opens :8080; multi-start opens Project Details (no purge).",
      items: LISTING_ACTIONS,
    },
    {
      surface: "mytawala",
      id: "actions",
      title: "Project Details — action bar",
      where:
        "projectmanager/detail.jsp — USE · EXPORT · IMPORT · BACKUP · RESTORE · PURGE · DELETE · PULL FROM LIBRARY. " +
        "USE = single-start opens :8080; multi-start → Project Details (owner Aug 4, 2026). " +
        "No PUBLISH here — Publish only lives on the My Tawala listing row.",
      items: PROJECT_ACTIONS,
    },
    {
      surface: "mytawala",
      id: "sidebar",
      title: "Project Details — left sidebar",
      where: "blocks/block-projectManagerProjectDetails.jsp + invite / webpage flows",
      items: PROJECT_SIDEBAR_OPS,
    },
    {
      surface: "mytawala",
      id: "data",
      title: "Project Details — Project Data (collapsible)",
      where: "detail.jsp form table + filters — per-form View / Export / Import / Purge",
      items: PROJECT_DATA_OPS,
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
      title: "Project Details — Backups / emails / publish / admin",
      where: "detail.jsp + confirmation dialogs — schedule, restore, emails, library publish",
      items: OTHER_OPS,
    },
  ];

  const SURFACE_INTRO = {
    library: {
      heading: "Public Library controls",
      blurb:
        "library.html (+ library-detail.html for descriptions / test-drive). Browse & acquire: Test drive + Save a copy. Use (run) is on My Tawala.",
    },
    mytawala: {
      heading: "My Tawala / Project Manager controls",
      blurb:
        "mytawala.html (listing icon strip) + mytawala-project.html (Project Details). Private projects; Use opens :8080 when single-start, else Project Details; PURGE and DELETE are active on listing and Details.",
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
      op.wired === "export-mytawala" ||
      op.wired === "import-mytawala" ||
      op.wired === "backup-mytawala" ||
      op.wired === "restore-mytawala" ||
      op.wired === "pull-library" ||
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
    if (item.wired === "export-mytawala") return "active (download JSON — data only, see README)";
    if (item.wired === "import-mytawala") return "active (field-mismatch check + POST :3001)";
    if (item.wired === "backup-mytawala") return "active (download JSON — definition + data + properties)";
    if (item.wired === "restore-mytawala") return "active (properties overlay + POST :3001 data)";
    if (item.wired === "pull-library") return "active (Pull dialog → overlay content refresh from Library)";
    if (item.wired === "download-version") {
      return "active (minimal version metadata JSON — Details Versions only)";
    }
    if (item.wired === "deploy-version") {
      return "active when a non-current version with a saved definition snapshot is selected";
    }
    if (item.wired === "use-project") {
      return "active when :8080 start URL exists (single → run; multi → Project Details; no purge; My Tawala only)";
    }
    if (item.wired === "test-drive") return "active when :8080 deployed (purge-on-start)";
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
   * Library “Actions” header — group label plus two sub-labels aligned over icon slots.
   * Labels: Test drive | Save a copy (Use is on My Tawala).
   */
  function renderLibraryListingActionHeaders() {
    const subs = LIBRARY_LISTING_ACTIONS.map(
      (op) =>
        `<span class="library-action-subhead" title="${escapeHtml(op.title)}">${escapeHtml(op.label)}</span>`
    ).join("");
    return (
      `<div class="lib-col lib-col-actions" role="columnheader">` +
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
   * Test drive when deployed; Save a copy grey stub.
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

  function renderLibraryListingControlCells(project) {
    const driveUrl = libraryDriveUrl(project);
    const deployed =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project)) || !!driveUrl;
    const icons = LIBRARY_LISTING_ACTIONS.map((op) => {
      if (op.wired === "test-drive") {
        if (deployed && driveUrl) {
          return (
            `<a class="pm-icon-action pm-op-icon-btn is-active library-row-action js-testdrive" ` +
            `href="${escapeHtml(driveUrl)}" data-testdrive-url="${escapeHtml(driveUrl)}" ` +
            `target="_blank" rel="noopener" ` +
            `title="Purge prior responses, then open :8080" aria-label="${escapeHtml(op.title)}" ` +
            `data-op="${escapeHtml(op.id)}" data-wired="true">${listingIconHtml(op.icon)}</a>`
          );
        }
        return (
          `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
          `title="No local test-drive yet" aria-label="Test drive unavailable" ` +
          `data-op="${escapeHtml(op.id)}" data-wired="false">${listingIconHtml(op.icon)}</button>`
        );
      }
      return (
        `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
        `title="${escapeHtml(op.title)}" aria-label="${escapeHtml(op.label)}" ` +
        `data-op="${escapeHtml(op.id)}" data-wired="false">${listingIconHtml(op.icon)}</button>`
      );
    }).join("");
    return (
      `<div class="lib-col lib-col-actions" onclick="event.stopPropagation()">` +
      `<span class="library-row-actions">${icons}</span>` +
      `</div>`
    );
  }

  /** @deprecated Prefer renderLibraryListingControlCells — single-cell text controls. */
  function renderLibraryListingControls(project) {
    return (
      '<div class="controls pm-listing-controls library-listing-controls" onclick="event.stopPropagation()">' +
      LIBRARY_LISTING_ACTIONS.map((op) => {
        const driveUrl = libraryDriveUrl(project);
        const deployed =
          (typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project)) || !!driveUrl;
        if (op.wired === "test-drive") {
          if (deployed && driveUrl) {
            return (
              `<a class="pm-icon-action pm-op-icon-btn is-active library-row-action js-testdrive" ` +
              `href="${escapeHtml(driveUrl)}" data-testdrive-url="${escapeHtml(driveUrl)}" ` +
              `target="_blank" rel="noopener" ` +
              `title="Purge prior responses, then open :8080" aria-label="${escapeHtml(op.title)}" ` +
              `data-op="${escapeHtml(op.id)}" data-wired="true">${listingIconHtml(op.icon)}</a>`
            );
          }
          return (
            `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
            `title="No local test-drive yet" aria-label="Test drive unavailable" ` +
            `data-op="${escapeHtml(op.id)}" data-wired="false">${listingIconHtml(op.icon)}</button>`
          );
        }
        return (
          `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
          `title="${escapeHtml(op.title)}" aria-label="${escapeHtml(op.title)}" ` +
          `data-op="${escapeHtml(op.id)}" data-wired="false">${listingIconHtml(op.icon)}</button>`
        );
      }).join(" ") +
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

  function startPointsWithUrls(project) {
    return ((project && project.startPoints) || []).filter((s) => s && s.url);
  }

  function listedStartPoints(project) {
    return ((project && project.startPoints) || []).filter(
      (s) => s && (s.url || s.label || s.form)
    );
  }

  function projectDetailsHref(projectId, hash) {
    const base = `mytawala-project.html?project=${encodeURIComponent(projectId || "")}`;
    return hash ? `${base}#${hash}` : base;
  }

  /**
   * Use navigation: multi-entry apps (2+ start points) go to Project Details so the
   * user picks Setup vs Exam (etc.). Single-start opens the one :8080 URL in a new tab.
   * Owner Aug 4, 2026 — jumping straight to Exam skipped setup and hit stale sessions.
   */
  function projectUseTarget(project) {
    if (!project) return null;
    const listed = listedStartPoints(project);
    const withUrls = startPointsWithUrls(project);
    const runtimeUrl = projectUseUrl(project);
    if (!withUrls.length && !runtimeUrl) return null;
    if (listed.length > 1) {
      return {
        href: projectDetailsHref(project.id, "pmSecStart"),
        openInNewTab: false,
        title: "Choose a start point on Project Details (multi-entry project)",
        mode: "details",
      };
    }
    if (!runtimeUrl) return null;
    return {
      href: runtimeUrl,
      openInNewTab: true,
      title: "Open / run this project (start link)",
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
      if (asIcon) {
        return (
          `<button type="button" class="pm-icon-action pm-op-icon-btn" disabled ` +
          `title="No local deploy URL yet — Deploy from Designer first" ` +
          `aria-label="Use unavailable" data-op="${escapeHtml(op.id)}" ` +
          `data-project="${escapeHtml(projectId || "")}" data-wired="false">${listingIconHtml(op.icon)}</button>`
        );
      }
      return (
        `<button type="button" class="pm-action" disabled ` +
        `title="No local deploy URL yet — Deploy from Designer first" ` +
        `aria-label="Use unavailable" data-op="${escapeHtml(op.id)}" ` +
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
    return (
      `<button type="button" class="pm-action"${disabled} title="${escapeHtml(op.title || op.label)}" ${data}>` +
      `${escapeHtml(op.label)}</button>`
    );
  }

  function renderProjectActionsBar(projectId) {
    return (
      '<div class="pm-actions-bar buttons" role="toolbar" aria-label="Project Actions">' +
      PROJECT_ACTIONS.map((op) => actionButton(op, projectId)).join("") +
      "</div>"
    );
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
        '<p class="pm-hint">No Deploy versions yet — open in Web Designer, Deploy, optionally add a version note, then <b>Show in My Tawala</b>. Listing stays flat; history appears here only.</p>'
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
      '<p class="pm-hint"><b>Deploy this version</b> switches the live :8080 definition to the selected row ' +
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
      note: "Minimal metadata download. Deploy this version uses the saved definition snapshot, not this file.",
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

  async function deploySelectedVersion(projectId) {
    const project =
      (typeof TawalaDemo !== "undefined" && TawalaDemo.getMyTawala && TawalaDemo.getMyTawala(projectId)) ||
      null;
    if (!project) {
      setStatus("Deploy this version — project not found.");
      window.alert("Couldn't deploy version\n\nProject not found in My Tawala.");
      return;
    }
    const host = document.getElementById("pmDetailHost") || document;
    const version = getSelectedVersionRow(project, host);
    if (!version) {
      setStatus("Deploy this version — no version selected.");
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
      setStatus(`Deploy this version failed — ${resolved.error || "no definition"}.`);
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
      `Deploy version ${version.versionNumber} as the live definition?\n\n` +
        "Responses collected under a newer form may not match this version. Continue?\n\n" +
        "This re-uploads the saved definition to :8080 (same project name / uniqueId when Java matches by name). " +
        "It does not mint a new Versions row."
    );
    if (!ok) {
      setStatus("Deploy this version cancelled.");
      return;
    }

    if (typeof TawalaDemo === "undefined" || typeof TawalaDemo.deployProjectDefinition !== "function") {
      setStatus("Deploy this version — deploy API helper missing.");
      window.alert("Couldn't deploy version\n\nDeploy support didn’t load. Refresh and try again.");
      return;
    }

    setStatus(`Deploying version ${version.versionNumber}…`);
    const result = await TawalaDemo.deployProjectDefinition(resolved.definition);
    if (!result || result.status === "failure") {
      const err = (result && result.error) || "unknown";
      setStatus(`Deploy this version failed: ${err}`);
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

  function renderCollapsibleSection(id, title, innerHtml, open) {
    return (
      `<details class="pm-section" id="${escapeHtml(id)}"${open ? " open" : ""}>` +
      `<summary>${escapeHtml(title)}</summary>` +
      `<div class="pm-section-body">${innerHtml}</div>` +
      "</details>"
    );
  }

  /**
   * Full Project Details layout (separate page): action bar + sidebar ops + collapsible sections.
   * Inactive section chips are disabled (grey only). Start points open live :8080 URLs when deployed.
   * Do NOT purge-on-click here (unlike Library Test drive): Online Exam Admin setup / questions
   * must survive when the owner then opens Exam. Use PURGE action when a clean slate is wanted.
   */
  function renderDetailPanel(project) {
    if (!project) return '<p class="pm-hint">Project not found.</p>';
    const deployed = typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project);
    let startPoints = project.startPoints || [];
    // Surface Exam / Registration before Admin when multi-start (list order only).
    if (typeof TawalaDemo !== "undefined" && typeof TawalaDemo.pickPrimaryStartPoint === "function") {
      const primary = TawalaDemo.pickPrimaryStartPoint(startPoints);
      if (primary && startPoints.length > 1) {
        startPoints = [primary].concat(startPoints.filter((s) => s !== primary));
      }
    }
    const startLinks = startPoints
      .map((sp) => {
        if (typeof TawalaDemo !== "undefined" && TawalaDemo.startPointHtml) {
          // My Tawala: no purge (preserve Question / SetupVariables for Exam flow).
          return `<li>${TawalaDemo.startPointHtml(sp, { purge: false })}</li>`;
        }
        if (sp.url) {
          return `<li><a href="${escapeHtml(sp.url)}" target="_blank" rel="noopener">${escapeHtml(sp.label || sp.form || "Start")}</a></li>`;
        }
        return `<li><span class="start-point-pending" title="No local :8080 URL yet">${escapeHtml(sp.label || sp.form || "Start")}</span></li>`;
      })
      .join("");

    const dataOps =
      '<div class="pm-chip-row">' +
      PROJECT_DATA_OPS.map(disabledChip).join("") +
      "</div>" +
      '<p class="pm-hint">Form table — SHOW ALL / SELECTED filters and per-form View / Export / Import / Purge.</p>';

    const versionOps = renderVersionsSection(project);

    const backupOps =
      '<div class="pm-chip-row">' +
      OTHER_OPS.map(disabledChip).join("") +
      "</div>" +
      '<p class="pm-hint">Backups, project emails, library publish dialogs, and admin UPDATE.</p>';

    const commentsStub =
      '<p class="pm-hint">Legacy Library listed a community comment count per project. ' +
      "Comment threads are not wired in this mock — stub for later.</p>";

    const startSection =
      `<ul class="pm-start-points">${startLinks || "<li>—</li>"}</ul>` +
      (deployed
        ? '<p class="pm-hint">Start links → local Java :8080 (full form-token URLs from Deploy). Keeps project data; use <b>PURGE</b> to clear responses. Multi-entry apps (e.g. Online Exam): open <b>Setup</b>/<b>Administration</b> first, then <b>Exam</b>. <b>Use</b> from the listing brings you here to choose.</p>'
        : '<p class="pm-hint deploy-hint-quiet">No local :8080 deploy yet — open in Web Designer, then Deploy → Show in My Tawala for live start links.</p>');

    const versionsOpen = projectVersionRows(project).length > 0;

    return (
      `<div class="pm-detail-layout" id="pmDetail" data-project-id="${escapeHtml(project.id)}">` +
      `<div class="pm-detail-main">` +
      `<div class="pm-detail-header">` +
      `<p class="pm-back"><a href="mytawala.html">← My Projects</a></p>` +
      `<h2>${escapeHtml(
        (window.TawalaDemo && window.TawalaDemo.displayName
          ? window.TawalaDemo.displayName(project.name)
          : String(project.name || "")
              .replace(/\.tawala\.xml$/i, "")
              .replace(/\.tawala$/i, "")
              .replace(/\.json$/i, ""))
      )}</h2>` +
      `<p class="pm-detail-meta">${escapeHtml(project.shortDescription || "")}</p>` +
      (project.versionNumber != null
        ? `<p class="pm-detail-version">Version ${escapeHtml(String(project.versionNumber))}` +
          (project.versionDescription
            ? ` — ${escapeHtml(String(project.versionDescription))}`
            : "") +
          `</p>`
        : "") +
      "</div>" +
      '<h3 class="sectionHeading">Project Actions</h3>' +
      renderProjectActionsBar(project.id) +
      renderCollapsibleSection("pmSecStart", "Start points", startSection, true) +
      renderCollapsibleSection("pmSecComments", "Comments", commentsStub, false) +
      renderCollapsibleSection("pmSecData", "Project Data", dataOps, false) +
      renderCollapsibleSection("pmSecVersions", "Versions", versionOps, versionsOpen) +
      renderCollapsibleSection("pmSecOther", "Backups, emails & library publish", backupOps, false) +
      '<p class="pm-hint" id="pmOpStatus" role="status"></p>' +
      "</div>" +
      `<aside class="pm-detail-sidebar" aria-label="Project options">` +
      "<h3>Project options</h3>" +
      '<div class="pm-sidebar-ops pm-sidebar-stack">' +
      PROJECT_SIDEBAR_OPS.map((op) => {
        return (
          `<button type="button" class="pm-action" disabled title="${escapeHtml(op.title)}" ` +
          `data-op="${escapeHtml(op.label)}" data-wired="false">${escapeHtml(op.label)}</button>`
        );
      }).join("") +
      "</div>" +
      `<p class="pm-hint"><a href="project-ops-review.html">Archive labels / unimplemented features</a></p>` +
      "</aside>" +
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

  function closePublishModal() {
    const el = document.getElementById(PUBLISH_MODAL_ID);
    if (el) el.remove();
    document.removeEventListener("keydown", handlePublishModalKeydown, true);
  }

  function handlePublishModalKeydown(ev) {
    if (ev.key === "Escape") closePublishModal();
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
    backdrop.innerHTML =
      '<div class="tawala-modal" role="dialog" aria-modal="true" aria-labelledby="pullModalTitle">' +
      `<h3 id="pullModalTitle">Pull from Library</h3>` +
      `<p class="pm-hint">Refreshes “${escapeHtml(displayName)}” with a public Library project's current description, category, and reference start points (mock — browser overlay only).</p>` +
      '<label class="tawala-modal-field" for="pullSourceSelect">Pull content from' +
      `<select id="pullSourceSelect">${pullTargetOptionsHtml(matches, allCandidates)}</select>` +
      "</label>" +
      '<p class="pm-hint">Keeps this project\u2019s own name, rating, comments, and — importantly — its own <code>:8080</code> deploy / submission data. Only descriptive content is refreshed; use EXPORT/BACKUP first if you want a safety copy.</p>' +
      '<p class="pm-hint" id="pullModalError" role="alert" style="display:none;"></p>' +
      '<div class="tawala-modal-actions">' +
      '<button type="button" class="pm-action" id="pullModalCancel">Cancel</button>' +
      '<button type="button" class="pm-action is-active" id="pullModalConfirm">Pull</button>' +
      "</div>" +
      "</div>";
    document.body.appendChild(backdrop);
    document.addEventListener("keydown", handlePullModalKeydown, true);

    const sourceSelect = backdrop.querySelector("#pullSourceSelect");
    sourceSelect.value = preselectId;
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
        `Pull “${sourceName}” into “${displayName}”?\n\n` +
        "This overwrites the description, category, and reference start points on this My Tawala project. " +
        "Your own name, rating, comments, and :8080 deploy / submission data are not touched.";
      if (!window.confirm(confirmMsg)) return;

      const result = TawalaTransfer.pullFromLibrary({ myTawalaProjectId: projectId, libraryId });
      closePullModal();
      if (!result || !result.ok) {
        const msg = `Couldn't pull “${sourceName}”\n\n${(result && result.error) || "Unknown error"}`;
        setStatus(msg.split("\n")[0]);
        window.alert(msg);
        return;
      }
      const msg = `Pulled “${result.sourceName}” into “${displayName}” — description, category, and reference start points refreshed.`;
      setStatus(msg);
      window.alert(msg);
      document.dispatchEvent(
        new CustomEvent("tawala:project-pulled", { detail: { projectId, result } })
      );
    });
  }

  async function handleOpClick(ev) {
    const btn = ev.target.closest("[data-op], .pm-action, .pm-icon-action");
    if (!btn || !btn.dataset || btn.disabled) return;
    const wired = btn.dataset.wired;
    const confirmId = btn.dataset.confirm;
    const projectId = btn.dataset.project || "";
    const op = btn.dataset.op || "";

    if (wired === "false" || !wired) return;

    /* Use = native <a>: single-start → :8080; multi-start → Project Details. Let browser follow. */
    if (wired === "use-project") return;

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
      try {
        if (wired === "export-mytawala") await TawalaDataOps.handleExportClick(projectId);
        else if (wired === "import-mytawala") await TawalaDataOps.handleImportClick(projectId);
        else if (wired === "backup-mytawala") await TawalaDataOps.handleBackupClick(projectId);
        else if (wired === "restore-mytawala") await TawalaDataOps.handleRestoreClick(projectId);
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

    if (wired === "purge-local") {
      /* Purge = clear :8080 submission data only — not Delete (row remove). */
      if (typeof TawalaDemo === "undefined" || !TawalaDemo.purgeResponses) {
        setStatus(`PURGE unavailable — purge support not loaded.`);
        window.alert(`Couldn't purge\n\nPurge support isn’t loaded. Refresh the page and try again.`);
        return;
      }
      const project =
        (projectId && TawalaDemo.getMyTawala && TawalaDemo.getMyTawala(projectId)) ||
        (projectId && TawalaDemo.get && TawalaDemo.get(projectId)) ||
        null;
      const displayName =
        project && TawalaDemo.displayName
          ? TawalaDemo.displayName(project.name || projectId)
          : projectId || "project";
      const uniqueId =
        typeof TawalaDemo.resolvePurgeUniqueId === "function"
          ? TawalaDemo.resolvePurgeUniqueId(projectId)
          : TawalaDemo.uniqueIdForProject
            ? TawalaDemo.uniqueIdForProject(project)
            : null;
      if (!uniqueId) {
        const hint = project
          ? `“${displayName}” isn’t linked to a live :8080 deploy yet. Deploy from Designer (or use a deployed project), then try Purge again.`
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
          const warn = result.warning ? ` (${result.warning})` : "";
          const msg = `Purged “${displayName}” — deleted ${n} submission row(s).${warn}`;
          setStatus(msg);
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
    scope.addEventListener("click", (ev) => {
      if (ev.target.closest("[data-op], .pm-action, .pm-icon-action")) {
        void handleOpClick(ev);
      }
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
    renderSubmenu,
    renderLibrarySubmenu,
    renderLibraryChromeStubs,
    renderListingControls,
    renderListingControlCells,
    renderListingActionHeaders,
    renderLibraryListingControlCells,
    renderLibraryListingActionHeaders,
    renderLibraryListingControls,
    renderDetailPanel,
    syncDeployVersionChip,
    renderOpsCatalog,
    openPublishDialog,
    bind,
  };
})();
