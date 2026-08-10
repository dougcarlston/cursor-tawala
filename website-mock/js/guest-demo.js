/**
 * Guest My Tawala taste — canned portfolio + read-only Details.
 * Used when owner cannot supply live My Tawala screenshots.
 * Logged-in users never see this catalog (redirect to real mytawala.html).
 */
(function () {
  function escapeHtml(s) {
    return String(s == null ? "" : s)
      .replace(/&/g, "&amp;")
      .replace(/</g, "&lt;")
      .replace(/>/g, "&gt;")
      .replace(/"/g, "&quot;");
  }

  const DEMO_PROJECTS = [
    {
      id: "demo-office-potluck",
      name: "Office Potluck — Spring Picnic",
      versionNumber: 3,
      created: "3/12/26",
      updated: "8/2/26",
      records: 48,
      timesUsed: 62,
      lastUsed: "8/1/26",
      shortDescription:
        "Team potluck RSVPs and dish list — based on the public Potluck template.",
      themePath: "default",
      startPoints: [{ label: "Potluck Organizer" }, { label: "Report" }],
      formNames: ["Potluck Organizer", "Report", "Thanks"],
      versions: [
        { versionNumber: 3, description: "Added allergy notes field", at: "8/2/26", current: true },
        { versionNumber: 2, description: "Fixed Report tallies", at: "6/18/26" },
        { versionNumber: 1, description: "Saved from Library (Potluck)", at: "3/12/26" },
      ],
    },
    {
      id: "demo-johns-first",
      name: "John's First Tawala Project",
      versionNumber: 1,
      created: "7/4/26",
      updated: "7/4/26",
      records: 3,
      timesUsed: 4,
      lastUsed: "7/5/26",
      shortDescription:
        "Learning the ropes — a rough first survey. (Typical new-account experiment.)",
      themePath: "mvsc",
      startPoints: [{ label: "Survey" }],
      formNames: ["Survey", "Report"],
      versions: [
        { versionNumber: 1, description: "First Push from Designer", at: "7/4/26", current: true },
      ],
    },
    {
      id: "demo-biology-midterm",
      name: "Biology Mid-term 2026",
      versionNumber: 5,
      created: "1/20/26",
      updated: "8/8/26",
      records: 214,
      timesUsed: 220,
      lastUsed: "8/9/26",
      shortDescription: "Online exam for Biology 101 — Administration + Exam starts.",
      themePath: "default",
      startPoints: [
        { label: "Administration" },
        { label: "Exam" },
        { label: "Setup" },
      ],
      formNames: ["Administration", "Exam", "Setup", "CustomizationPreview"],
      versions: [
        { versionNumber: 5, description: "Updated question bank for fall", at: "8/8/26", current: true },
        { versionNumber: 4, description: "Scoring fix", at: "5/2/26" },
        { versionNumber: 3, description: "Added Setup start", at: "3/1/26" },
      ],
    },
    {
      id: "demo-weekend-get-together",
      name: "Weekend Get Together",
      versionNumber: 2,
      created: "5/1/26",
      updated: "7/22/26",
      records: 31,
      timesUsed: 40,
      lastUsed: "7/21/26",
      shortDescription: "Date finder for a family reunion — Get Together variant.",
      themePath: "greentea",
      startPoints: [{ label: "Survey" }, { label: "Report" }],
      formNames: ["Survey", "Report"],
      versions: [
        { versionNumber: 2, description: "More date options", at: "7/22/26", current: true },
        { versionNumber: 1, description: "Saved from Library", at: "5/1/26" },
      ],
    },
    {
      id: "demo-scratch-blank",
      name: "asdf test 2",
      versionNumber: 1,
      created: "8/6/26",
      updated: "8/6/26",
      records: 0,
      timesUsed: 0,
      lastUsed: "—",
      shortDescription: "Another first attempt — blank-ish project left after exploring Designer.",
      themePath: "default",
      startPoints: [{ label: "Form 1" }],
      formNames: ["Form 1"],
      versions: [
        { versionNumber: 1, description: "Pushed from Designer", at: "8/6/26", current: true },
      ],
    },
    {
      id: "demo-customer-pulse",
      name: "Customer Pulse Survey",
      versionNumber: 4,
      created: "2/14/26",
      updated: "7/30/26",
      records: 89,
      timesUsed: 102,
      lastUsed: "7/29/26",
      shortDescription:
        "Multi-question survey with Report tallies — Multiple Question Survey flavor.",
      themePath: "style2",
      startPoints: [{ label: "Survey" }, { label: "Report" }],
      formNames: ["Survey", "Report"],
      versions: [
        { versionNumber: 4, description: "New NPS question", at: "7/30/26", current: true },
        { versionNumber: 3, description: "Theme Big Q", at: "6/1/26" },
        { versionNumber: 2, description: "Copy of Simple Survey", at: "2/14/26" },
      ],
    },
    {
      id: "demo-horses-quiz",
      name: "Horses vs Penguins — Club Night",
      versionNumber: 2,
      created: "6/10/26",
      updated: "7/31/26",
      records: 56,
      timesUsed: 60,
      lastUsed: "7/30/26",
      shortDescription: "Fun quiz night — from the Horses and Penguins Library try-out.",
      themePath: "style2",
      startPoints: [{ label: "Form 1" }],
      formNames: ["Form 1"],
      versions: [
        { versionNumber: 2, description: "Club branding", at: "7/31/26", current: true },
        { versionNumber: 1, description: "Saved from Library", at: "6/10/26" },
      ],
    },
    {
      id: "demo-marias-practice",
      name: "Maria's practice form",
      versionNumber: 1,
      created: "8/9/26",
      updated: "8/9/26",
      records: 1,
      timesUsed: 2,
      lastUsed: "8/9/26",
      shortDescription: "Practice Push after signing up — not meant for real respondents yet.",
      themePath: "mvsc",
      startPoints: [{ label: "Form 1" }],
      formNames: ["Form 1"],
      versions: [
        { versionNumber: 1, description: "First Push", at: "8/9/26", current: true },
      ],
    },
  ];

  function getDemoProject(id) {
    return DEMO_PROJECTS.find((p) => p.id === id) || null;
  }

  function registerBannerHtml(where) {
    const place = where || "My Tawala";
    return (
      `<div class="guest-demo-banner" role="status">` +
      `<p><b>Preview only</b> — this is a sample ${escapeHtml(place)} so you can see how Tawala looks. ` +
      `None of the controls work until you register.</p>` +
      `<p class="guest-demo-cta">` +
      `<a class="pm-action is-active" href="signup.html">Register free</a> ` +
      `<a class="pm-action" href="login.html?next=mytawala.html">Log in</a> ` +
      `<a href="library.html">← Back to Library</a>` +
      `</p>` +
      `</div>`
    );
  }

  function listingRowsHtml() {
    return DEMO_PROJECTS.map((p) => {
      const href = `mytawala-demo-project.html?project=${encodeURIComponent(p.id)}`;
      return (
        `<tr class="guest-demo-row" data-demo-id="${escapeHtml(p.id)}" tabindex="0" ` +
        `role="link" data-href="${escapeHtml(href)}">` +
        `<td class="left"><a href="${href}">${escapeHtml(p.name)}</a></td>` +
        `<td>${escapeHtml(String(p.versionNumber))}</td>` +
        `<td>${escapeHtml(p.created)}</td>` +
        `<td>${escapeHtml(p.updated)}</td>` +
        `<td>${escapeHtml(String(p.records))}</td>` +
        `<td>${escapeHtml(String(p.timesUsed))}</td>` +
        `<td>${escapeHtml(p.lastUsed)}</td>` +
        `<td><span class="link-pending" title="Register to Use your own projects">Use</span></td>` +
        `</tr>`
      );
    }).join("");
  }

  function inert(label, title) {
    return (
      `<button type="button" class="pm-action" disabled ` +
      `title="${escapeHtml(title || "Register to use this control")}">${escapeHtml(label)}</button>`
    );
  }

  function detailHtml(project) {
    if (!project) {
      return (
        '<p class="pm-hint">Unknown demo project. <a href="mytawala-demo.html">← Guest My Tawala</a></p>'
      );
    }
    const starts = (project.startPoints || [])
      .map(
        (s) =>
          `<li class="guest-demo-start"><span class="guest-demo-start-label">${escapeHtml(
            s.label
          )}</span> <span class="link-pending" title="Register to run your own starts">Open</span></li>`
      )
      .join("");
    const forms = (project.formNames || [])
      .map((f) => `<li>${escapeHtml(f)}</li>`)
      .join("");
    const versions = (project.versions || [])
      .map((v) => {
        const cur = v.current ? ' <span class="pm-status-badge">current</span>' : "";
        return (
          `<tr>` +
          `<td>${escapeHtml(String(v.versionNumber))}${cur}</td>` +
          `<td class="left">${escapeHtml(v.description || "")}</td>` +
          `<td>${escapeHtml(v.at || "")}</td>` +
          `<td><span class="link-pending" title="Register to deploy">Deploy</span></td>` +
          `</tr>`
        );
      })
      .join("");

    return (
      `<div class="pm-detail-layout guest-demo-detail" id="pmDetail">` +
      registerBannerHtml("Project Details") +
      `<aside class="pm-detail-sidebar" aria-label="Project options">` +
      "<h3>Project options</h3>" +
      `<div class="pm-sidebar-identity">` +
      `<div class="pm-identity-row"><dt>Author</dt><dd>demo user</dd></div>` +
      `<div class="pm-identity-row"><dt>Theme</dt><dd>${escapeHtml(project.themePath || "default")}</dd></div>` +
      `<div class="pm-identity-row"><dt>Version</dt><dd>${escapeHtml(String(project.versionNumber))}</dd></div>` +
      `<div class="pm-identity-row"><dt>Source</dt><dd class="pm-identity-muted">Guest preview (not your account)</dd></div>` +
      `</div>` +
      `<p class="pm-hint">Theme, Invite / Include, and Designer controls stay here on a real project. ` +
      `In this preview they do nothing.</p>` +
      `<div class="pm-sidebar-ops pm-sidebar-stack" style="margin-top:8px;">` +
      inert("Edit in Designer", "Register to open Designer") +
      inert("Invite / Include", "Register to share your live starts") +
      inert("De-activate", "Register to manage Library visibility") +
      `</div>` +
      "</aside>" +
      `<div class="pm-detail-main">` +
      `<div class="pm-detail-header">` +
      `<p class="pm-back"><a href="mytawala-demo.html">← My Tawala (preview)</a></p>` +
      `<h2 class="pm-detail-title">${escapeHtml(project.name)}</h2>` +
      `<p class="pm-detail-meta">${escapeHtml(project.shortDescription || "")}</p>` +
      "</div>" +
      `<div class="pm-actions-bar" role="toolbar" aria-label="Project actions (preview)">` +
      inert("Rename", "Register to rename") +
      inert("Make a Copy", "Register to fork your own projects") +
      inert("Backup", "Register to backup") +
      inert("Restore", "Register to restore") +
      inert("Deploy", "Register to Deploy / share") +
      inert("Publish", "Register to Publish to Library") +
      `</div>` +
      `<details class="pm-collapsible" open>` +
      `<summary>Project Data</summary>` +
      `<div class="pm-collapsible-body">` +
      `<p class="pm-hint">Start links and forms — expandable on a real account. Controls below are inert.</p>` +
      `<div class="pm-data-ctrl-group" style="margin-bottom:8px;">` +
      inert("Use", "Register to Use") +
      inert("Copy link", "Register to copy your start link") +
      inert("Export", "Register to Export") +
      inert("Import", "Register to Import") +
      inert("Purge", "Register to Purge") +
      `</div>` +
      `<p><b>Start points</b></p>` +
      `<ul class="guest-demo-starts">${starts || "<li>(none)</li>"}</ul>` +
      `<p style="margin-top:10px;"><b>All forms</b></p>` +
      `<ul>${forms || "<li>(none)</li>"}</ul>` +
      `<p class="pm-hint" style="margin-top:8px;">Times used: <b>${escapeHtml(
        String(project.timesUsed)
      )}</b> · Last used: <b>${escapeHtml(project.lastUsed)}</b> · Records: <b>${escapeHtml(
        String(project.records)
      )}</b></p>` +
      `</div></details>` +
      `<details class="pm-collapsible" open>` +
      `<summary>Versions</summary>` +
      `<div class="pm-collapsible-body">` +
      `<table class="pm-versions-table" aria-label="Project versions (preview)">` +
      "<thead><tr><th>Ver</th><th class=\"left\">Description</th><th>Date</th><th></th></tr></thead>" +
      `<tbody>${versions}</tbody></table>` +
      `<p class="pm-hint">On a real project you can edit the description and Deploy a selected version after Push.</p>` +
      `</div></details>` +
      `<details class="pm-collapsible">` +
      `<summary>Comments</summary>` +
      `<div class="pm-collapsible-body"><p class="pm-hint">Comments / reputation — stub (not wired).</p></div>` +
      `</details>` +
      `<p class="pm-hint guest-demo-footer-cta">Like what you see? ` +
      `<a href="signup.html"><b>Register</b></a> for your own empty My Tawala, then Save templates from the Library.</p>` +
      "</div></div>"
    );
  }

  function bindListingClicks(root) {
    const scope = root || document;
    scope.addEventListener("click", (ev) => {
      const row = ev.target.closest && ev.target.closest("tr.guest-demo-row");
      if (!row) return;
      if (ev.target.closest("a,button")) return;
      const href = row.getAttribute("data-href");
      if (href) location.href = href;
    });
    scope.addEventListener("keydown", (ev) => {
      if (ev.key !== "Enter") return;
      const row = ev.target.closest && ev.target.closest("tr.guest-demo-row");
      if (!row) return;
      const href = row.getAttribute("data-href");
      if (href) location.href = href;
    });
  }

  window.TawalaGuestDemo = {
    DEMO_PROJECTS,
    getDemoProject,
    registerBannerHtml,
    listingRowsHtml,
    detailHtml,
    bindListingClicks,
    escapeHtml,
  };
})();
