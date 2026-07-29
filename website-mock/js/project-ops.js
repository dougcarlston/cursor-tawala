/**
 * Legacy My Tawala / Project Manager operation labels recovered from
 * TawalaWebapp-build1700 (projectmanager/*.jsp, blocks/block-projectManagerProjectDetails.jsp,
 * submenus/submenu-mytawala.jsp, confirmationdialogs.jsp).
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

  /** Listing-row icon actions (view.jsp) */
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

  function escapeHtml(s) {
    return String(s)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
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
    const badge = op.wired === "purge-local" ? "" : op.wired ? "" : ' <span class="pm-stub-tag">not wired</span>';
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
      `<button type="button" class="pm-manage-btn" data-manage="${escapeHtml(projectId)}" title="Show full Project Actions (legacy Project Details)">Manage…</button>` +
      "</div>"
    );
  }

  function renderOpsCatalog() {
    function list(title, items) {
      return (
        `<details class="pm-archive-group"><summary>${escapeHtml(title)}</summary><ul>` +
        items
          .map((i) => {
            const note = i.wired === "purge-local" ? " — local cleanup hint" : i.wired ? "" : " — not wired";
            const src = i.source ? ` <em>(${escapeHtml(i.source)})</em>` : "";
            return `<li><code>${escapeHtml(i.label)}</code>${src}${note}</li>`;
          })
          .join("") +
        "</ul></details>"
      );
    }
    return (
      '<div class="pm-archive-note block">' +
      "<h3>Archive labels (memory jog)</h3>" +
      `<p class="pm-archive-source">${escapeHtml(ARCHIVE_NOTE)}</p>` +
      list("Project Actions bar", PROJECT_ACTIONS) +
      list("Listing row", LISTING_ACTIONS) +
      list("Project sidebar", PROJECT_SIDEBAR_OPS) +
      list("Project Data (per form)", PROJECT_DATA_OPS) +
      list("Versions", VERSION_OPS) +
      list("Backups / emails / publish", OTHER_OPS) +
      list("Related Save / Clone (Library & Customize)", RELATED_SAVE_CLONE) +
      "</div>"
    );
  }

  function renderDetailPanel(project) {
    if (!project) return "";
    const startLinks = (project.startPoints || [])
      .map(
        (sp) =>
          `<a href="${escapeHtml(sp.url)}" target="_blank" rel="noopener">${escapeHtml(sp.label)}</a>`
      )
      .join(" · ");
    return (
      `<div class="pm-detail section" id="pmDetail" data-project-id="${escapeHtml(project.id)}">` +
      `<h2>${escapeHtml(project.name)}</h2>` +
      '<h3 class="sectionHeading">Project Actions</h3>' +
      renderProjectActionsBar(project.id) +
      '<p class="pm-hint">Legacy titles: Export / Import / Backup / Restore / Purge project data / Delete Project / Publish to Community Library.</p>' +
      '<h3 class="sectionHeading">Also on Project Details (sidebar)</h3>' +
      '<div class="pm-sidebar-ops">' +
      PROJECT_SIDEBAR_OPS.map((op) => {
        return `<button type="button" class="pm-action pm-action-stub" title="${escapeHtml(op.title)}" data-wired="false">${escapeHtml(op.label)} <span class="pm-stub-tag">not wired</span></button>`;
      }).join("") +
      "</div>" +
      `<h3 class="sectionHeading">Start points (test drive → :8080)</h3>` +
      `<p>${startLinks || "—"}</p>` +
      '<p class="pm-hint" id="pmOpStatus" role="status"></p>' +
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
      setStatus(
        `PURGE requested for “${projectId || "project"}”. ${LOCAL_PURGE_HELP}`
      );
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
      const manage = ev.target.closest("[data-manage]");
      if (manage) {
        const id = manage.getAttribute("data-manage");
        const project = window.TawalaDemo && window.TawalaDemo.get(id);
        const host = document.getElementById("pmDetailHost");
        if (host) {
          host.innerHTML = renderDetailPanel(project);
          host.scrollIntoView({ behavior: "smooth", block: "start" });
        }
        return;
      }
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
