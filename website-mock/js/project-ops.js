/**
 * Legacy My Tawala / Project Manager operation labels recovered from
 * TawalaWebapp-build1700 (projectmanager/*.jsp, blocks/block-projectManagerProjectDetails.jsp,
 * submenus/submenu-mytawala.jsp, confirmationdialogs.jsp).
 *
 * Layout-first mock: listing + Project Details pages stay lean; full catalog lives on
 * project-ops-review.html for memory/archive review.
 *
 * wired: false → stub / memory jog only
 * wired: "purge-local" → confirm + local cleanup hint (no Tomcat Project Manager purge API in mock)
 */
(function () {
  /** My Tawala top sub-menu (submenu-mytawala.jsp) */
  const MYTAWALA_SUBMENU = [
    { label: "My Projects", title: "Manage your projects", wired: true, href: "mytawala.html" },
    { label: "My Account", title: "Edit your accounts settings", wired: false },
    { label: "Change Password", title: "Change your password", wired: false },
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
    { id: "delete", label: "DELETE", title: "Delete Project", wired: false, confirmId: "delete" },
    { id: "publish", label: "PUBLISH", title: "Publish project to the Community Library", wired: false },
  ];

  /** Listing-row icon actions (view.jsp) — lean: Purge / Delete only */
  const LISTING_ACTIONS = [
    { id: "purge", label: "Purge", title: "Purge project data", wired: "purge-local", confirmId: "purge" },
    { id: "delete", label: "Delete", title: "Delete project", wired: false, confirmId: "delete" },
  ];

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
    { label: "Delete Selected Items", title: "Delete selected versions", wired: false },
  ];

  /** Backups / emails / publish dialogs (admin-ish; still useful memory jogs) */
  const OTHER_OPS = [
    { label: "SCHEDULE BACKUP", title: "Schedule daily backup", wired: false },
    { label: "CHANGE BACKUP", title: "Change backup schedule", wired: false },
    { label: "CANCEL BACKUP", title: "Stop Backups", wired: false },
    { label: "RESTORE (from online backup)", title: "Restore Project from This Backup", wired: false },
    { label: "Delete this backup", title: "Delete this backup", wired: false },
    { label: "DELETE ALL PROJECT BACKUPS", title: "Delete all backups for this project", wired: false },
    { label: "View all project emails", title: "View all project emails", wired: false },
    { label: "DELETE ALL PROJECT EMAILS", title: "Delete All Project Emails", wired: false },
    { label: "Publish as a New Project to the library", title: "Copy this app to the library…", wired: false },
    { label: "Update an Existing Library Project", title: "Update an app in the library with this version", wired: false },
    { label: "Upgrade Project", title: "Upgrade with newer library version", wired: false },
    { label: "UPDATE (Additional Project Details)", title: "Admin project properties", wired: false },
  ];

  /** Customization / Library flows that save/clone into My Tawala (not the PM action bar) */
  const RELATED_SAVE_CLONE = [
    { label: "Save this project under My Tawala", source: "customization saveTile/publishTile", wired: false },
    { label: "Save your project to My Tawala", source: "customization", wired: false },
    { label: "DEPLOY TO MY TAWALA", source: "library project detail (admin)", wired: false },
    { label: "Clone and Customize", source: "library CloneAndCustomizeController", wired: false },
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
      body: "Are you sure you want to delete this project?",
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
    deleteallemails: {
      title: "Delete All Project Emails",
      body: "Are you sure you want to delete all project emails?",
      submit: "Delete All Emails",
    },
  };

  const ARCHIVE_NOTE =
    "Archive sources: projectmanager/detail.jsp (Project Actions), view.jsp (listing Purge/Delete), " +
    "block-projectManagerProjectDetails.jsp (REVISE / ONLINE-OFFLINE / Include / Invite), " +
    "submenu-mytawala.jsp, confirmationdialogs.jsp. " +
    "No separate Rename/Clone labels on the Project Actions bar — Save/Clone appear in customization & Library flows. " +
    "SportsDashboards (not SportsBoard).";

  const LOCAL_PURGE_HELP =
    "Full project PURGE needs legacy Project Manager → purgeProjectResponses (not exposed on this mock or designer-web :3001). " +
    "Closest local helper for DirtBowl Registration test rows only:\n\n" +
    "  ./scripts/dev-data.sh cleanup-registrations\n\n" +
    "(Requires docker compose postgres. Does not delete the project or all forms.)";

  /** Catalog sections for the dedicated review page (legacy location → labels). */
  const OPS_CATALOG_SECTIONS = [
    {
      id: "submenu",
      title: "My Tawala sub-menu",
      where: "submenu-mytawala.jsp — shallow 3-item bar above listings",
      items: MYTAWALA_SUBMENU,
    },
    {
      id: "listing",
      title: "Project listing row",
      where: "projectmanager/view.jsp — Purge / Delete only beside each project",
      items: LISTING_ACTIONS,
    },
    {
      id: "actions",
      title: "Project Details — action bar",
      where: "projectmanager/detail.jsp — EXPORT · IMPORT · BACKUP · RESTORE · PURGE · DELETE · PUBLISH",
      items: PROJECT_ACTIONS,
    },
    {
      id: "sidebar",
      title: "Project Details — left sidebar",
      where: "blocks/block-projectManagerProjectDetails.jsp + invite / webpage flows",
      items: PROJECT_SIDEBAR_OPS,
    },
    {
      id: "data",
      title: "Project Details — Project Data (collapsible)",
      where: "detail.jsp form table + filters — per-form View / Export / Import / Purge",
      items: PROJECT_DATA_OPS,
    },
    {
      id: "versions",
      title: "Project Details — Versions (collapsible)",
      where: "detail.jsp versions section",
      items: VERSION_OPS,
    },
    {
      id: "other",
      title: "Project Details — Backups / emails / publish / admin",
      where: "detail.jsp + confirmation dialogs — schedule, restore, emails, library publish",
      items: OTHER_OPS,
    },
    {
      id: "wizards",
      title: "Related Save / Clone wizards (Library & Customize)",
      where: "Not on Project Actions bar — customization tiles + Library CloneAndCustomize",
      items: RELATED_SAVE_CLONE,
    },
  ];

  function escapeHtml(s) {
    return String(s)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  function wiredNote(item) {
    if (item.wired === "purge-local") return "local cleanup hint";
    if (item.wired === true) return "wired in mock";
    return "not wired";
  }

  function renderSubmenu(active) {
    return (
      '<div class="sub-menu pm-submenu">' +
      "<ul>" +
      MYTAWALA_SUBMENU.map((item) => {
        const sel = item.label === active ? " selected" : "";
        if (!item.wired) {
          return `<li><span class="link-pending${sel}" title="${escapeHtml(item.title)} — not wired in mock">${escapeHtml(item.label)}</span></li>`;
        }
        return `<li><a class="${sel.trim()}" href="${item.href}" title="${escapeHtml(item.title)}">${escapeHtml(item.label)}</a></li>`;
      }).join("") +
      "</ul></div>"
    );
  }

  function actionButton(op, projectId) {
    const cls = op.wired ? "pm-action" : "pm-action pm-action-stub";
    const badge = op.wired === "purge-local" || op.wired === true ? "" : ' <span class="pm-stub-tag">not wired</span>';
    const data =
      `data-op="${escapeHtml(op.id || op.label)}" data-project="${escapeHtml(projectId || "")}"` +
      (op.confirmId ? ` data-confirm="${escapeHtml(op.confirmId)}"` : "") +
      (op.wired ? ` data-wired="${escapeHtml(String(op.wired))}"` : ' data-wired="false"');
    return `<button type="button" class="${cls}" title="${escapeHtml(op.title || op.label)}" ${data}>${escapeHtml(op.label)}${badge}</button>`;
  }

  function renderProjectActionsBar(projectId) {
    return (
      '<div class="pm-actions-bar buttons" role="toolbar" aria-label="Project Actions">' +
      PROJECT_ACTIONS.map((op) => actionButton(op, projectId)).join("") +
      "</div>"
    );
  }

  /** Lean listing controls — Purge / Delete only (name click → Project Details). */
  function renderListingControls(projectId) {
    return (
      '<div class="controls pm-listing-controls">' +
      LISTING_ACTIONS.map((op) => {
        const wired = op.wired ? String(op.wired) : "false";
        return (
          `<button type="button" class="pm-icon-action${op.wired ? "" : " pm-action-stub"}" ` +
          `title="${escapeHtml(op.title)}" data-op="${escapeHtml(op.id)}" data-project="${escapeHtml(projectId)}" ` +
          `data-confirm="${escapeHtml(op.confirmId || "")}" data-wired="${escapeHtml(wired)}">${escapeHtml(op.label)}</button>`
        );
      }).join(" ") +
      "</div>"
    );
  }

  function stubChip(op) {
    return (
      `<button type="button" class="pm-action pm-action-stub" title="${escapeHtml(op.title || op.label)}" ` +
      `data-op="${escapeHtml(op.id || op.label)}" data-wired="false">${escapeHtml(op.label)} ` +
      `<span class="pm-stub-tag">not wired</span></button>`
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
   * Content under sections is stubbed; start points keep working :8080 test-drives.
   */
  function renderDetailPanel(project) {
    if (!project) return '<p class="pm-hint">Project not found.</p>';
    const startLinks = (project.startPoints || [])
      .map(
        (sp) =>
          `<li><a href="${escapeHtml(sp.url)}" target="_blank" rel="noopener">${escapeHtml(sp.label)}</a></li>`
      )
      .join("");

    const dataOps =
      '<div class="pm-chip-row">' +
      PROJECT_DATA_OPS.map(stubChip).join("") +
      "</div>" +
      '<p class="pm-hint">Form table stub — SHOW ALL / SELECTED filters and per-form View / Export / Import / Purge.</p>';

    const versionOps =
      '<div class="pm-chip-row">' +
      VERSION_OPS.map(stubChip).join("") +
      "</div>" +
      '<p class="pm-hint">Version list stub — Deploy / Delete / Download.</p>';

    const backupOps =
      '<div class="pm-chip-row">' +
      OTHER_OPS.map(stubChip).join("") +
      "</div>" +
      '<p class="pm-hint">Backups, project emails, library publish dialogs, and admin UPDATE — labels only.</p>';

    const startSection =
      `<ul class="pm-start-points">${startLinks || "<li>—</li>"}</ul>` +
      '<p class="pm-hint">Test-drive links → local Java :8080 (from demo-urls.js).</p>';

    return (
      `<div class="pm-detail-layout" id="pmDetail" data-project-id="${escapeHtml(project.id)}">` +
      `<div class="pm-detail-main">` +
      `<div class="pm-detail-header">` +
      `<p class="pm-back"><a href="mytawala.html">← My Projects</a></p>` +
      `<h2>${escapeHtml(project.name)}</h2>` +
      `<p class="pm-detail-meta">${escapeHtml(project.shortDescription || "")}</p>` +
      "</div>" +
      '<h3 class="sectionHeading">Project Actions</h3>' +
      renderProjectActionsBar(project.id) +
      renderCollapsibleSection("pmSecStart", "Start points (test drive)", startSection, true) +
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
          `<button type="button" class="pm-action pm-action-stub" title="${escapeHtml(op.title)}" ` +
          `data-op="${escapeHtml(op.label)}" data-wired="false">${escapeHtml(op.label)} ` +
          `<span class="pm-stub-tag">not wired</span></button>`
        );
      }).join("") +
      "</div>" +
      `<p class="pm-hint"><a href="project-ops-review.html">Archive labels / unimplemented features</a></p>` +
      "</aside>" +
      "</div>"
    );
  }

  /** Full catalog for project-ops-review.html — organized by legacy location. */
  function renderOpsCatalog() {
    const sections = OPS_CATALOG_SECTIONS.map((sec) => {
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
        `<h2>${escapeHtml(sec.title)}</h2>` +
        `<p class="pm-catalog-where">${escapeHtml(sec.where)}</p>` +
        `<table class="pm-catalog-table stripe">` +
        "<thead><tr><th>Label</th><th>Title / tooltip</th><th>Source note</th><th>Mock</th></tr></thead>" +
        `<tbody>${rows}</tbody></table>` +
        "</section>"
      );
    }).join("");

    const toc =
      '<nav class="pm-catalog-toc" aria-label="Catalog sections"><ul>' +
      OPS_CATALOG_SECTIONS.map(
        (sec) => `<li><a href="#${escapeHtml(sec.id)}">${escapeHtml(sec.title)}</a></li>`
      ).join("") +
      "</ul></nav>";

    return (
      '<div class="pm-catalog">' +
      `<p class="pm-archive-source">${escapeHtml(ARCHIVE_NOTE)}</p>` +
      toc +
      sections +
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
    if (el) el.textContent = msg;
  }

  async function handleOpClick(ev) {
    const btn = ev.target.closest("[data-op], .pm-action, .pm-icon-action");
    if (!btn || !btn.dataset) return;
    const wired = btn.dataset.wired;
    const confirmId = btn.dataset.confirm;
    const projectId = btn.dataset.project || "";
    const op = btn.dataset.op || btn.textContent.trim();

    if (confirmId) {
      const ok = await showConfirm(confirmId);
      if (!ok) return;
    }

    if (wired === "purge-local") {
      setStatus(`PURGE requested for “${projectId || "project"}”. ${LOCAL_PURGE_HELP}`);
      window.alert(
        `Purge Project Data\n\nConfirmed for mock project: ${projectId || "(unknown)"}\n\n${LOCAL_PURGE_HELP}`
      );
      return;
    }

    if (wired === "false" || !wired) {
      setStatus(`“${op}” — not wired in website mock (legacy label only).`);
      return;
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
    PROJECT_ACTIONS,
    LISTING_ACTIONS,
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
    renderListingControls,
    renderDetailPanel,
    renderOpsCatalog,
    bind,
  };
})();
