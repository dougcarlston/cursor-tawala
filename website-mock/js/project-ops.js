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
   * Public Library listing row actions — recovered from library detail / customizables
   * (Test Drive, Save/Clone into My Tawala). searchLibrary.jsp itself was click-to-detail only.
   * Single “Actions” column: packed icons in each row (hover titles); header is the word
   * “Actions” only — no header glyphs.
   * wired: "test-drive" → active when project has a live :8080 URL; else disabled grey.
   */
  const LIBRARY_LISTING_ACTIONS = [
    {
      id: "test-drive",
      label: "Drive",
      title: "Test drive this project (local :8080)",
      wired: "test-drive",
      icon: "testdrive",
    },
    {
      id: "save-my-tawala",
      label: "Save",
      title: "Save this project under My Tawala",
      wired: false,
      icon: "save",
    },
    {
      id: "clone",
      label: "USE IT",
      title: "USE IT",
      wired: false,
      icon: "clone",
    },
  ];

  /**
   * Project Actions bar on Project Details (detail.jsp) — exact button labels.
   * Titles from button title= attributes.
   */
  const PROJECT_ACTIONS = [
    { id: "export", label: "EXPORT", title: "Export project data to Excel format", wired: false },
    { id: "import", label: "IMPORT", title: "Import data to project from Excel or CSV file", wired: false },
    { id: "backup", label: "BACKUP", title: "Backup project data", wired: false },
    { id: "restore", label: "RESTORE", title: "Restore project data", wired: false },
    { id: "purge", label: "PURGE", title: "Purge project data", wired: "purge-local", confirmId: "purge" },
    { id: "delete", label: "DELETE", title: "Delete Project", wired: "delete-mytawala", confirmId: "delete" },
    {
      id: "publish",
      label: "PUBLISH",
      title: "Publish / move this project to the public Library",
      wired: false,
    },
    {
      id: "pull-library",
      label: "PULL FROM LIBRARY",
      title: "Replace this project with a newer public Library version",
      wired: false,
    },
  ];

  /**
   * My Projects listing row — same ops as Project Actions, icon-per-row / label-in-header.
   * (Legacy view.jsp was Purge/Delete only; owner asked for the full listing-appropriate strip.)
   */
  const LISTING_ACTIONS = [
    {
      id: "export",
      label: "Export",
      title: "Export project data to Excel format",
      wired: false,
      icon: "export",
    },
    {
      id: "import",
      label: "Import",
      title: "Import data to project from Excel or CSV file",
      wired: false,
      icon: "import",
    },
    {
      id: "backup",
      label: "Backup",
      title: "Backup project data",
      wired: false,
      icon: "backup",
    },
    {
      id: "restore",
      label: "Restore",
      title: "Restore project data",
      wired: false,
      icon: "restore",
    },
    {
      id: "purge",
      label: "Purge",
      title: "Purge project data",
      wired: "purge-local",
      confirmId: "purge",
      icon: "purge",
    },
    {
      id: "delete",
      label: "Delete",
      title: "Delete project",
      wired: "delete-mytawala",
      confirmId: "delete",
      icon: "delete",
    },
    {
      id: "publish",
      label: "Publish",
      title: "Publish / move this project to the public Library",
      wired: false,
      icon: "publish",
    },
    {
      id: "pull-library",
      label: "Pull",
      title: "Replace with newer Library version",
      wired: false,
      icon: "pull",
    },
  ];

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
    clone:
      '<svg class="pm-op-icon" viewBox="0 0 12 12" aria-hidden="true"><path d="M4 3.5h5.5v5.5H4zM2.5 2v5.5H8" fill="none" stroke="currentColor" stroke-width="1.2" stroke-linecap="round" stroke-linejoin="round"/></svg>',
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

  /** Versions section */
  const VERSION_OPS = [
    { label: "Deploy", title: "Make this version the active version", wired: false },
    { label: "Delete this version", title: "Delete this version", wired: false, confirmId: "deleteversion" },
    { label: "Download this version of the project", title: "Download this version of the project", wired: false },
    {
      label: "Delete Selected Items",
      title: "Delete selected versions",
      wired: false,
      confirmId: "deleteselected",
    },
  ];

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
    { label: "Save this project under My Tawala", source: "Library → My Tawala", wired: false },
    { label: "USE IT", source: "customizables.jsp → CloneAndCustomizeController (web customize, not Designer)", wired: false },
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
  };

  const ARCHIVE_NOTE =
    "Archive sources: projectmanager/detail.jsp (Project Actions), view.jsp (listing Purge/Delete historically), " +
    "block-projectManagerProjectDetails.jsp (REVISE / ONLINE-OFFLINE / Include / Invite), " +
    "submenu-mytawala.jsp, submenu-library.jsp, confirmationdialogs.jsp. " +
    "Mock My Tawala listing now shows the full Export…Publish icon strip (labels in headers). " +
    "Catalog is split: Public Library controls vs My Tawala / Project Manager. " +
    "No separate Rename/Clone labels on the Project Actions bar — Save under My Tawala / USE IT appear in customization & Library flows. " +
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
        "Library mock — icon columns (hover titles): Test drive (when :8080 deployed), Save this project under My Tawala, USE IT. searchLibrary.jsp was click-to-detail only.",
      items: LIBRARY_LISTING_ACTIONS,
    },
    {
      surface: "library",
      id: "wizards",
      title: "Related Save / Clone wizards (Library → My Tawala)",
      where: "Not on Project Actions bar — customization tiles + Library CloneAndCustomize",
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
        "mytawala.html — icon strip per row (labels in column headers): Export · Import · Backup · Restore · Purge · Delete · Publish. Legacy view.jsp was Purge/Delete only.",
      items: LISTING_ACTIONS,
    },
    {
      surface: "mytawala",
      id: "actions",
      title: "Project Details — action bar",
      where: "projectmanager/detail.jsp — EXPORT · IMPORT · BACKUP · RESTORE · PURGE · DELETE · PUBLISH",
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
        "library.html (+ library-detail.html for descriptions / test-drive). Browse & try public catalog; Save/Clone into My Tawala are grey stubs.",
    },
    mytawala: {
      heading: "My Tawala / Project Manager controls",
      blurb:
        "mytawala.html (listing icon strip) + mytawala-project.html (Project Details). Private projects; PURGE (via :3001 → Postgres by uniqueId) and DELETE (account-private row remove + overlay/inbox clear) are active on listing and Details.",
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
      op.wired === "edit-categories"
    );
  }

  function wiredNote(item) {
    if (item.wired === "purge-local") return "active (purge via :3001 API)";
    if (item.wired === "delete-mytawala" || item.wired === "delete-mock") {
      return "active (Delete My Tawala row — overlay/inbox; not Library)";
    }
    if (item.wired === "edit-categories") return "active (local category assignment)";
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

  /** Single Library “Actions” header — text only (icons live in row cells). */
  function renderLibraryListingActionHeaders() {
    return (
      `<div class="lib-col lib-col-actions" role="columnheader" title="Project actions">` +
      `<span class="th-label">Actions</span>` +
      `</div>`
    );
  }

  /**
   * Library listing — one Actions grid cell with packed icons (hover titles).
   * Test drive when deployed; Save / Clone grey stubs.
   */
  function renderLibraryListingControlCells(project) {
    const deployed =
      typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project);
    const icons = LIBRARY_LISTING_ACTIONS.map((op) => {
      if (op.wired === "test-drive") {
        if (deployed) {
          return (
            `<a class="pm-icon-action pm-op-icon-btn is-active library-row-action js-testdrive" ` +
            `href="${escapeHtml(project.testDriveUrl)}" data-testdrive-url="${escapeHtml(project.testDriveUrl)}" ` +
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
        const deployed =
          typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project);
        if (op.wired === "test-drive") {
          if (deployed) {
            return (
              `<a class="pm-icon-action pm-op-icon-btn is-active library-row-action js-testdrive" ` +
              `href="${escapeHtml(project.testDriveUrl)}" data-testdrive-url="${escapeHtml(project.testDriveUrl)}" ` +
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

  /** Active ops are clickable; inactive ops are disabled (no “not wired” label — grey is enough). */
  function actionButton(op, projectId) {
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
        `<th class="col-op" scope="col" data-op-col="${escapeHtml(op.id)}" ` +
        `title="${escapeHtml(op.title)}"><span class="th-label">${escapeHtml(op.label)}</span></th>`
      );
    }).join("");
  }

  /** Per-row icon cells (one &lt;td&gt; per listing action). Name click → Project Details. */
  function renderListingControlCells(projectId) {
    return LISTING_ACTIONS.map((op) => {
      const active = isOpActive(op);
      const wired = active ? String(op.wired) : "false";
      const disabled = active ? "" : " disabled";
      const activeClass = active ? " is-active" : "";
      return (
        `<td class="col-op">` +
        `<button type="button" class="pm-icon-action pm-op-icon-btn${activeClass}"${disabled} ` +
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
        const active = isOpActive(op);
        const wired = active ? String(op.wired) : "false";
        const disabled = active ? "" : " disabled";
        const activeClass = active ? " is-active" : "";
        return (
          `<button type="button" class="pm-icon-action pm-op-icon-btn${activeClass}"${disabled} ` +
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
   * Inactive section chips are disabled (grey only). Start points keep :8080 test-drives when deployed.
   */
  function renderDetailPanel(project) {
    if (!project) return '<p class="pm-hint">Project not found.</p>';
    const deployed = typeof TawalaDemo !== "undefined" && TawalaDemo.isDeployed(project);
    const startLinks = (project.startPoints || [])
      .map((sp) => {
        if (typeof TawalaDemo !== "undefined" && TawalaDemo.startPointHtml) {
          return `<li>${TawalaDemo.startPointHtml(sp)}</li>`;
        }
        if (sp.url) {
          return `<li><a href="${escapeHtml(sp.url)}" target="_blank" rel="noopener">${escapeHtml(sp.label)}</a></li>`;
        }
        return `<li><span class="start-point-pending" title="No local :8080 URL yet">${escapeHtml(sp.label)}</span></li>`;
      })
      .join("");

    const dataOps =
      '<div class="pm-chip-row">' +
      PROJECT_DATA_OPS.map(disabledChip).join("") +
      "</div>" +
      '<p class="pm-hint">Form table — SHOW ALL / SELECTED filters and per-form View / Export / Import / Purge.</p>';

    const versionOps =
      '<div class="pm-chip-row">' +
      VERSION_OPS.map(disabledChip).join("") +
      "</div>" +
      '<p class="pm-hint">Version list — Deploy / Delete / Download.</p>';

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
        ? '<p class="pm-hint">Test-drive links → local Java :8080 (purge prior responses on click, then open).</p>'
        : '<p class="pm-hint deploy-hint-quiet">No local :8080 deploy yet — open in Web Designer, then Deploy for a live test-drive.</p>');

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
      "</div>" +
      '<h3 class="sectionHeading">Project Actions</h3>' +
      renderProjectActionsBar(project.id) +
      renderCollapsibleSection("pmSecStart", "Start points (test drive)", startSection, true) +
      renderCollapsibleSection("pmSecComments", "Comments", commentsStub, false) +
      renderCollapsibleSection("pmSecData", "Project Data", dataOps, false) +
      renderCollapsibleSection("pmSecVersions", "Versions", versionOps, false) +
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

  async function handleOpClick(ev) {
    const btn = ev.target.closest("[data-op], .pm-action, .pm-icon-action");
    if (!btn || !btn.dataset || btn.disabled) return;
    const wired = btn.dataset.wired;
    const confirmId = btn.dataset.confirm;
    const projectId = btn.dataset.project || "";
    const op = btn.dataset.op || "";

    if (wired === "false" || !wired) return;

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
    renderOpsCatalog,
    bind,
  };
})();
