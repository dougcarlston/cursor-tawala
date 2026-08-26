/**
 * My Tawala data-lifecycle ops (owner Aug 1, 2026; Restore Redeploy Aug 25): BACKUP / RESTORE
 * (paired live definition + data + properties; Java ZIP / mock JSON) and EXPORT / IMPORT
 * (Excel response data only). Restore Redeploys the bundled definition onto :8080 (same
 * uniqueId) via /api/deploy, then imports responses. A backup whose uniqueId does not
 * match this My Tawala row is refused before Redeploy/import/overlay. See website-mock/README.md § Backup.
 *
 * Server plumbing: designer-web/server/projectResponses.mjs via TawalaDemo.exportResponses /
 * TawalaDemo.importResponses (designer-web/js/demo-urls.js) — same :3001 dev API as Purge.
 *
 * Wiring: js/project-ops.js dispatches data-op="export"|"import"|"backup"|"restore" (wired:
 * "export-mytawala" etc.) to the handlers below. Loaded after transfer.js + project-ops.js on
 * mytawala.html / mytawala-project.html.
 */
(function () {
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

  function resolveProject(projectId) {
    return projectId && typeof TawalaDemo !== "undefined" && TawalaDemo.getMyTawala
      ? TawalaDemo.getMyTawala(projectId)
      : null;
  }

  function displayNameFor(project, projectId) {
    return project && typeof TawalaDemo !== "undefined" && TawalaDemo.displayName
      ? TawalaDemo.displayName(project.name || projectId)
      : projectId || "project";
  }

  function resolveUniqueId(projectId) {
    return typeof TawalaDemo !== "undefined" && typeof TawalaDemo.resolvePurgeUniqueId === "function"
      ? TawalaDemo.resolvePurgeUniqueId(projectId)
      : null;
  }

  /** `M/D/YY` — matches the short date format used elsewhere in the mock (transfer.js formatListDate). */
  function shortDate(d) {
    const dt = d instanceof Date ? d : new Date(d || Date.now());
    if (Number.isNaN(dt.getTime())) return "—";
    const y = String(dt.getFullYear()).slice(-2);
    return `${dt.getMonth() + 1}/${dt.getDate()}/${y}`;
  }

  function isoNow(d) {
    const dt = d instanceof Date ? d : new Date(d || Date.now());
    if (Number.isNaN(dt.getTime())) return new Date().toISOString();
    return dt.toISOString();
  }

  function triggerJsonDownload(filename, dataObj) {
    const blob = new Blob([JSON.stringify(dataObj, null, 2)], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = filename;
    a.style.display = "none";
    document.body.appendChild(a);
    a.click();
    a.remove();
    setTimeout(() => URL.revokeObjectURL(url), 4000);
  }

  /** Opens a native file picker; resolves with the parsed JSON (rejects on cancel/parse error). */
  function pickJsonFile() {
    return new Promise((resolve, reject) => {
      const input = document.createElement("input");
      input.type = "file";
      input.accept = ".json,application/json";
      input.style.display = "none";
      let settled = false;
      input.addEventListener("change", () => {
        const file = input.files && input.files[0];
        input.remove();
        if (!file) {
          settled = true;
          reject(new Error("No file selected"));
          return;
        }
        const reader = new FileReader();
        reader.onload = () => {
          settled = true;
          try {
            const text = String(reader.result || "");
            resolve({ file, text, json: JSON.parse(text) });
          } catch (e) {
            reject(new Error("Selected file isn't valid JSON: " + e.message));
          }
        };
        reader.onerror = () => {
          settled = true;
          reject(new Error("Could not read the selected file"));
        };
        reader.readAsText(file);
      });
      document.body.appendChild(input);
      input.click();
      // Cancel gives no 'change' event in most browsers — fall back to a window focus check.
      window.addEventListener(
        "focus",
        function onFocus() {
          window.removeEventListener("focus", onFocus);
          setTimeout(() => {
            if (!settled && !(input.files && input.files.length)) {
              settled = true;
              input.remove();
              reject(new Error("No file selected"));
            }
          }, 300);
        },
        { once: true },
      );
    });
  }

  function validateExportShape(payload) {
    if (!payload || typeof payload !== "object") {
      return { ok: false, error: "File is not a Tawala Export JSON object." };
    }
    if (!Array.isArray(payload.forms)) {
      return { ok: false, error: 'File is missing a "forms" array — not a Tawala Export file.' };
    }
    if (payload.source !== "postgres" && payload.source !== "dev-session") {
      return {
        ok: false,
        error: `File has an unknown/missing "source" (${payload.source || "none"}) — expected "postgres" or "dev-session".`,
      };
    }
    return { ok: true };
  }

  function sameFieldSet(a, b) {
    if (a.length !== b.length) return false;
    const setB = new Set(b);
    return a.every((f) => setB.has(f));
  }

  /**
   * Field-mismatch check for IMPORT (data-only — see README glossary: "field mismatch fails").
   * Compares the uploaded file's field names per form against the CURRENT project's own data
   * for that form. Forms with no current data are skipped (no baseline to compare against) —
   * documented mock gap: real validation would compare against the live form definition
   * regardless of whether data exists yet.
   */
  function findFieldMismatches(fileFieldsByForm, currentFieldsByForm) {
    const hard = [];
    const skipped = [];
    Object.keys(fileFieldsByForm || {}).forEach((form) => {
      const fileFields = (fileFieldsByForm[form] || []).slice().sort();
      const currentFields = currentFieldsByForm && currentFieldsByForm[form];
      if (!currentFields || !currentFields.length) {
        skipped.push(form);
        return;
      }
      const sortedCurrent = currentFields.slice().sort();
      if (!sameFieldSet(fileFields, sortedCurrent)) {
        hard.push({ form, fileFields, currentFields: sortedCurrent });
      }
    });
    return { hard, skipped };
  }

  function requireDataApi() {
    if (typeof TawalaDemo === "undefined" || typeof TawalaDemo.exportResponses !== "function") {
      window.alert("Data ops unavailable — support scripts didn't load. Refresh the page and try again.");
      return false;
    }
    return true;
  }

  /** Filter an export wire payload down to one form (client-side form-scoped Export). */
  function filterExportToForm(result, formName) {
    const name = String(formName || "");
    const forms = (result.forms || []).filter((f) => f && f.form === name);
    const fieldsByForm = {};
    if (result.fieldsByForm && result.fieldsByForm[name]) {
      fieldsByForm[name] = result.fieldsByForm[name];
    }
    let count = 0;
    forms.forEach((f) => {
      count += (f.rows && f.rows.length) || 0;
    });
    return { forms, fieldsByForm, count };
  }

  /**
   * Merge one form from `incomingForms` into a full current export, then replace-write.
   *
   * REGRESSION (Aug 10, 2026): `/api/import-responses` replace deletes ALL submissions for the
   * uniqueId, then inserts only the forms array you send. Sending a single-form payload alone
   * (e.g. Answer export while Project Data is collapsed / project-scoped) wipes every other
   * form’s Records. Form-scoped Import MUST always merge into a successful full export first.
   */
  function mergeFormIntoExport(currentForms, incomingFormEntry, formName) {
    const name = String(formName || "");
    const others = (currentForms || []).filter((f) => f && f.form !== name);
    const incoming =
      incomingFormEntry ||
      (currentForms || []).find((f) => f && f.form === name) ||
      { form: name, rows: [] };
    return others.concat([{ form: name, rows: (incoming.rows || []).slice() }]);
  }

  /** Non-empty `payload.formName` from a form-scoped Export file (see handleExportClick bundle). */
  function formNameFromExportPayload(payload) {
    if (!payload || payload.formName == null) return "";
    const name = String(payload.formName).trim();
    return name;
  }

  const RESTORE_RUNTIME_HELP =
    "Restore Redeploys the backed-up definition onto :8080 (same uniqueId), then restores responses. " +
    "Needs designer-web API on :3001 and Tomcat on :8080.";

  /* Keep in sync with restoreIdentity.mjs */
  const RESTORE_UNIQUE_ID_RE = /^[A-Za-z0-9]{1,20}$/;
  const RESTORE_UNIQUE_FROM_URL_RE = /\/p\/([A-Za-z0-9]{1,20})(?:\/|$)/;

  function restoreUniqueIdFromUrl(url) {
    if (!url) return null;
    const m = String(url).match(RESTORE_UNIQUE_FROM_URL_RE);
    return m ? m[1] : null;
  }

  function pushRestoreUniqueId(into, value) {
    const s = value == null ? "" : String(value).trim();
    if (RESTORE_UNIQUE_ID_RE.test(s) && !into.includes(s)) into.push(s);
  }

  function collectBackupUniqueIds(payload) {
    const ids = [];
    if (!payload || typeof payload !== "object") return ids;
    pushRestoreUniqueId(ids, payload.uniqueId);
    const props = payload.properties && typeof payload.properties === "object" ? payload.properties : {};
    pushRestoreUniqueId(ids, props.uniqueId);
    [props.testDriveUrl]
      .concat((Array.isArray(props.startPoints) ? props.startPoints : []).map((sp) => sp && sp.url))
      .forEach((u) => {
        const id = restoreUniqueIdFromUrl(u);
        if (id) pushRestoreUniqueId(ids, id);
      });
    const def = payload.definition && typeof payload.definition === "object" ? payload.definition : {};
    pushRestoreUniqueId(ids, def.deployUniqueId);
    pushRestoreUniqueId(ids, def.uniqueId);
    return ids;
  }

  function backupProjectLabel(payload) {
    if (!payload || typeof payload !== "object") return "another project";
    const fromDisplay = String(payload.displayName || "").trim();
    if (fromDisplay) return fromDisplay;
    const props = payload.properties && typeof payload.properties === "object" ? payload.properties : {};
    const fromProps = String(props.name || "").trim();
    if (fromProps) return fromProps;
    const def = payload.definition && typeof payload.definition === "object" ? payload.definition : {};
    const fromDef = String(def.name || "").trim();
    return fromDef || "another project";
  }

  const RESTORE_RECOVERY_HINT =
    "To recover: open the .backup.json for the project you want (for example Exam Maker) and find uniqueId. " +
    "On Project Details, match that id to uniqueId under Project options, or to the /p/{id}/ in a DEPLOY Form link. " +
    "Ignore the title — both rows may look like Get Together if the wrong definition was Redeployed. " +
    "Restore that backup only onto the matching row.";

  function restoreMismatchMessage({ backupName, rowName, backupUniqueId, rowUniqueId }) {
    const backup = String(backupName || "another project").trim() || "another project";
    const row = String(rowName || "this project").trim() || "this project";
    const backupId = backupUniqueId ? ` (uniqueId ${backupUniqueId})` : "";
    const rowId = rowUniqueId ? ` (uniqueId ${rowUniqueId})` : "";
    return (
      `This backup belongs to “${backup}”${backupId}, not “${row}”${rowId}. Restore only works on the same project.\n\n` +
      RESTORE_RECOVERY_HINT
    );
  }

  function restoreMissingIdMessage() {
    return (
      "This backup has no uniqueId. Restore only works on the same project, and this file doesn’t identify which one. " +
      "Do not guess by the name on the file — two Get Togethers would collide. Make a new Backup from this project, or pick a file that includes uniqueId."
    );
  }

  function restoreConfirmNote({ backupName, backupUniqueId, rowName, rowUniqueId }) {
    const backup = String(backupName || "backup").trim() || "backup";
    const row = String(rowName || "this project").trim() || "this project";
    return (
      `This backup is “${backup}” (uniqueId ${backupUniqueId}).\n` +
      `This My Tawala row is “${row}” (uniqueId ${rowUniqueId}).`
    );
  }

  function matchRestoreToRow(payload, rowUniqueId, rowName) {
    const backupName = backupProjectLabel(payload);
    const rowLabel = String(rowName || "this project").trim() || "this project";
    const rowId = rowUniqueId == null ? "" : String(rowUniqueId).trim();
    const ids = collectBackupUniqueIds(payload);
    if (!ids.length) {
      return { ok: false, error: restoreMissingIdMessage(), backupName, backupUniqueId: null };
    }
    const foreign = ids.filter((id) => id !== rowId);
    if (foreign.length) {
      return {
        ok: false,
        error: restoreMismatchMessage({
          backupName,
          rowName: rowLabel,
          backupUniqueId: ids[0],
          rowUniqueId: rowId,
        }),
        backupName,
        backupUniqueId: ids[0],
      };
    }
    return { ok: true, backupUniqueId: ids[0], backupName };
  }

  function cloneJson(value) {
    return JSON.parse(JSON.stringify(value));
  }

  function definitionIsDeployable(def) {
    return !!(def && typeof def === "object" && String(def.name || "").trim());
  }

  function liveVersionRow(project) {
    const versions = Array.isArray(project && project.versions) ? project.versions : [];
    const deployed = versions.find((v) => v && (v.deployed || v.current));
    if (deployed) return deployed;
    const currentNum = project && project.versionNumber != null ? Number(project.versionNumber) : null;
    if (Number.isFinite(currentNum)) {
      const hit = versions.find((v) => v && Number(v.versionNumber) === currentNum);
      if (hit) return hit;
    }
    return null;
  }

  /** Stamp this My Tawala row's Tomcat identity so /api/deploy keeps the same uniqueId. */
  function attachRowIdentity(definition, project, uniqueId) {
    if (!definitionIsDeployable(definition)) return null;
    const next = cloneJson(definition);
    delete next._freshFromTemplate;
    const identity = String(
      (project && project.deployIdentityName) || next.deployIdentityName || ""
    ).trim();
    if (identity) next.deployIdentityName = identity;
    if (uniqueId) next.deployUniqueId = uniqueId;
    return next;
  }

  function stampThemePath(definition, themePath) {
    const path = String(themePath || "").trim();
    if (!path || !definitionIsDeployable(definition)) return definition;
    const next = cloneJson(definition);
    next.themePath = path;
    if (Array.isArray(next.forms)) {
      next.forms = next.forms.map((f) =>
        f && typeof f === "object" ? Object.assign({}, f, { themePath: path }) : f
      );
    }
    return next;
  }

  function trimmedTheme(value) {
    if (value == null) return "";
    return String(value).trim();
  }

  /**
   * Theme the Details dropdown persists (`tawala.mock.myTawala` overlay only).
   * Not catalog merge, not a buried definition.themePath.
   */
  function overlayChromeThemePath(projectId) {
    if (
      !projectId ||
      typeof TawalaTransfer === "undefined" ||
      typeof TawalaTransfer.getOverlayEntry !== "function"
    ) {
      return "";
    }
    const ov = TawalaTransfer.getOverlayEntry(projectId);
    if (!ov || typeof ov !== "object") return "";
    return trimmedTheme(ov.themePath) || trimmedTheme(ov.theme) || trimmedTheme(ov.themeId);
  }

  /** What the Theme dropdown is showing right now (overlay / catalog / Default). */
  function displayedThemePath(project) {
    if (
      typeof TawalaProjectOps !== "undefined" &&
      typeof TawalaProjectOps.resolveProjectThemePath === "function"
    ) {
      return trimmedTheme(TawalaProjectOps.resolveProjectThemePath(project)) || "default";
    }
    return trimmedTheme(project && project.themePath) || "default";
  }

  /**
   * Restore Theme only from backup My Tawala properties.
   * Empty/missing chrome theme → do not apply; never fall back to definition.themePath.
   */
  function themeFromBackupProperties(backupProps) {
    if (!backupProps || typeof backupProps !== "object") {
      return { apply: false, path: "" };
    }
    const path =
      trimmedTheme(backupProps.themePath) ||
      trimmedTheme(backupProps.theme) ||
      trimmedTheme(backupProps.themeId);
    if (path) return { apply: true, path };
    return { apply: false, path: "" };
  }

  function encodeMockRelPath(relPath) {
    return String(relPath || "")
      .split("/")
      .map((seg) => encodeURIComponent(seg))
      .join("/");
  }

  /**
   * Capture the currently live pushed definition (now), not the Versions pile.
   * Prefers the current/deployed overlay snapshot, then :3001 snapshot API, then catalog JSON.
   */
  async function captureLiveDefinition(project, uniqueId) {
    const live = liveVersionRow(project);
    if (live && definitionIsDeployable(live.definition)) {
      return {
        definition: attachRowIdentity(live.definition, project, uniqueId),
        definitionSource: "overlay-current-version",
        definitionNote:
          "Current live Push snapshot from this My Tawala row (the deployed version only — not the whole Versions pile).",
      };
    }

    const snapId = String((live && live.snapshotId) || (project && project.snapshotId) || "").trim();
    if (snapId && typeof TawalaDemo.fetchVersionSnapshot === "function") {
      const snap = await TawalaDemo.fetchVersionSnapshot(snapId);
      if (snap && snap.status !== "failure" && definitionIsDeployable(snap.project)) {
        return {
          definition: attachRowIdentity(snap.project, project, uniqueId),
          definitionSource: "version-snapshot:" + snapId,
          definitionNote: "Fetched current Push snapshot from designer-web :3001 /api/version-snapshots.",
        };
      }
    }

    if (project && definitionIsDeployable(project.definition)) {
      return {
        definition: attachRowIdentity(project.definition, project, uniqueId),
        definitionSource: "overlay-definition",
        definitionNote: "Definition cached on this My Tawala overlay row.",
      };
    }

    const jsonFile = project && project.jsonFile ? String(project.jsonFile).replace(/^website-mock\//, "") : "";
    if (jsonFile && typeof TawalaDemo.fetchCatalogProject === "function") {
      const fetched = await TawalaDemo.fetchCatalogProject(jsonFile);
      if (fetched && fetched.status === "success" && definitionIsDeployable(fetched.project)) {
        return {
          definition: attachRowIdentity(fetched.project, project, uniqueId),
          definitionSource: jsonFile,
          definitionNote:
            "Fetched from " +
            jsonFile +
            " via :3001 (repo/catalog copy — may lag a Designer working copy that was never Pushed).",
        };
      }
    }

    if (/^projects\//.test(jsonFile)) {
      try {
        const res = await fetch(encodeMockRelPath(jsonFile), { cache: "no-store" });
        if (res.ok) {
          const def = await res.json();
          if (definitionIsDeployable(def)) {
            return {
              definition: attachRowIdentity(def, project, uniqueId),
              definitionSource: jsonFile,
              definitionNote: "Fetched from " + jsonFile + " on :5500.",
            };
          }
        }
      } catch {
        /* fall through */
      }
    }

    return {
      definition: null,
      definitionSource: jsonFile || null,
      definitionNote:
        "No deployable live definition found (need a current Push snapshot, :3001 snapshot, or catalog JSON). Restore will refuse this file until Backup includes a definition.",
    };
  }

  function startPointsFromDeployResult(deployResult) {
    const raw =
      (deployResult && Array.isArray(deployResult.startpoints) && deployResult.startpoints) ||
      (deployResult && Array.isArray(deployResult.startPoints) && deployResult.startPoints) ||
      [];
    return raw
      .map((sp) => ({
        form: (sp && (sp.form || sp.label)) || "Start",
        label: (sp && (sp.label || sp.form)) || "Start",
        url: (sp && sp.url) || null,
      }))
      .filter((sp) => sp.form && sp.url);
  }

  async function probeDesignerApi() {
    if (typeof TawalaDemo === "undefined" || typeof TawalaDemo.purgeApiBase !== "function") {
      return false;
    }
    try {
      /* /api/health has no mock CORS; OPTIONS /api/deploy does (same Push path Restore uses). */
      const url = TawalaDemo.purgeApiBase().replace(/\/$/, "") + "/api/deploy";
      const res = await fetch(url, { method: "OPTIONS", cache: "no-store" });
      return res.ok || res.status === 204;
    } catch {
      return false;
    }
  }

  function failRestore(displayName, msg) {
    setStatus(`Restore failed for “${displayName}”: ${msg}`);
    window.alert(`Couldn't restore “${displayName}”\n\n${msg}`);
  }

  // ---------------------------------------------------------------------
  // EXPORT — Excel response data only (see README glossary).
  // opts.formName → download that form's rows only (still one Export file).
  // ---------------------------------------------------------------------
  async function handleExportClick(projectId, opts) {
    const options = opts || {};
    const formName = options.formName ? String(options.formName) : "";
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't export — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    const uniqueId = resolveUniqueId(projectId);
    if (!uniqueId) {
      const msg = `“${displayName}” isn’t linked to a live :8080 uniqueId yet. Edit in Designer, then Push → Show in My Tawala, then try Export again.`;
      setStatus(`Export unavailable for “${displayName}” — no live uniqueId.`);
      window.alert(`Couldn't export “${displayName}”\n\n${msg}`);
      return;
    }
    const scopeLabel = formName ? `form “${formName}” in “${displayName}”` : `“${displayName}”`;
    setStatus(`Exporting ${scopeLabel}…`);
    const result = await TawalaDemo.exportResponses(uniqueId);
    if (result.status !== "success") {
      const msg = `Couldn't export ${scopeLabel}\n\n${result.error || "unknown error"}\n\n${TawalaProjectOps.LOCAL_PURGE_HELP}`;
      setStatus(`Export failed for ${scopeLabel}: ${result.error || "unknown error"}`);
      window.alert(msg);
      return;
    }
    const sliced = formName ? filterExportToForm(result, formName) : null;
    const forms = sliced ? sliced.forms : result.forms;
    const fieldsByForm = sliced ? sliced.fieldsByForm : result.fieldsByForm;
    const count = sliced ? sliced.count : result.count;
    if (formName && !forms.length) {
      const msg =
        `No submission rows for form “${formName}” in “${displayName}” (export file will list the form with 0 rows).`;
      // Still download an empty-form export so Import round-trips stay honest.
      forms.push({ form: formName, rows: [] });
      if (!fieldsByForm[formName]) fieldsByForm[formName] = [];
      setStatus(msg);
    }
    const bundle = {
      tawalaExportFormat: 1,
      kind: "export",
      projectId,
      displayName,
      uniqueId,
      formName: formName || null,
      source: result.source,
      exportedAt: new Date().toISOString(),
      fieldsByForm,
      forms,
      count,
    };
    const slug = TawalaTransfer.slugifyProjectId(displayName);
    const filename = formName
      ? `${slug}.${TawalaTransfer.slugifyProjectId(formName)}.export.json`
      : `${slug}.export.json`;
    triggerJsonDownload(filename, bundle);
    const msg = formName
      ? `Exported form “${formName}” from “${displayName}” — ${count} submission row(s). Saved as ${filename}.`
      : `Exported “${displayName}” — ${count} submission row(s) across ${forms.length} form(s). Saved as ${filename} (mock JSON, not Excel — see README § Export/Import).`;
    setStatus(msg);
    window.alert(msg);
  }

  // ---------------------------------------------------------------------
  // IMPORT — restore messed-up data into the CURRENT project; field mismatch fails.
  // Form scope (merge → replace-write; never send one form alone):
  //   • opts.formName — Project Data highlight (form / start row)
  //   • payload.formName — form-scoped Export file (even when the tree is collapsed /
  //     project-selected). Without this, Answer-only files wipe sibling forms.
  // ---------------------------------------------------------------------
  async function handleImportClick(projectId, opts) {
    const options = opts || {};
    const uiFormName = options.formName ? String(options.formName) : "";
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't import — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    const uniqueId = resolveUniqueId(projectId);
    if (!uniqueId) {
      const msg = `“${displayName}” isn’t linked to a live :8080 uniqueId yet. Edit in Designer, then Push → Show in My Tawala, then try Import again.`;
      setStatus(`Import unavailable for “${displayName}” — no live uniqueId.`);
      window.alert(`Couldn't import into “${displayName}”\n\n${msg}`);
      return;
    }

    let picked;
    try {
      picked = await pickJsonFile();
    } catch (e) {
      if (e && e.message !== "No file selected") {
        window.alert(`Couldn't read the selected file\n\n${e.message}`);
      }
      return;
    }
    const payload = picked.json;
    const check = validateExportShape(payload);
    if (!check.ok) {
      setStatus(`Import failed for “${displayName}”: ${check.error}`);
      window.alert(`Couldn't import into “${displayName}”\n\n${check.error}`);
      return;
    }

    const fileFormName = formNameFromExportPayload(payload);
    if (uiFormName && fileFormName && uiFormName !== fileFormName) {
      const msg =
        `Couldn't import into form “${uiFormName}”\n\n` +
        `That file is a form-scoped export for “${fileFormName}”, not “${uiFormName}”. ` +
        `Highlight the matching form (or collapse to project scope and import this file — siblings stay intact), or pick a whole-project export.`;
      setStatus(`Import failed — file is for form “${fileFormName}”, not “${uiFormName}”.`);
      window.alert(msg);
      return;
    }

    /* Highlight wins; else form-scoped file metadata (project collapsed still safe). */
    const formName = uiFormName || fileFormName;
    const scopedFromFileOnly = !uiFormName && !!fileFormName;

    let formsForImport = payload.forms;
    let fieldsForCheck = payload.fieldsByForm || {};
    if (formName) {
      const entry = (payload.forms || []).find((f) => f && f.form === formName);
      if (!entry) {
        const msg =
          `Couldn't import into form “${formName}”\n\n` +
          `The selected file has no data for that form. Export that form first, or pick a whole-project export that includes it.`;
        setStatus(`Import failed — file has no form “${formName}”.`);
        window.alert(msg);
        return;
      }
      fieldsForCheck = {};
      if (payload.fieldsByForm && payload.fieldsByForm[formName]) {
        fieldsForCheck[formName] = payload.fieldsByForm[formName];
      }
      formsForImport = [entry];
    }

    const scopeLabel = formName ? `form “${formName}” in “${displayName}”` : `“${displayName}”`;
    setStatus(`Checking fields for ${scopeLabel}…`);
    const current = await TawalaDemo.exportResponses(uniqueId);
    /*
     * Form-scoped path must read live siblings before replace. An empty merge base would
     * reintroduce the wipe-siblings bug (import API deletes everything first).
     */
    if (formName && current.status !== "success") {
      const msg =
        `Couldn't import into form “${formName}”\n\n` +
        `Need the project's current response data to preserve other forms, but export failed:\n` +
        `${current.error || "unknown error"}\n\n${TawalaProjectOps.LOCAL_PURGE_HELP}`;
      setStatus(`Import failed for ${scopeLabel}: could not read current rows to merge.`);
      window.alert(msg);
      return;
    }

    const currentFieldsByForm = current.status === "success" ? current.fieldsByForm : {};
    const mismatch = findFieldMismatches(fieldsForCheck, currentFieldsByForm);
    if (mismatch.hard.length) {
      const details = mismatch.hard
        .map((m) => `  • ${m.form}: file has [${m.fileFields.join(", ")}] — current data has [${m.currentFields.join(", ")}]`)
        .join("\n");
      const msg =
        `Import failed — field mismatch for ${scopeLabel}.\n\n` +
        `The imported file's fields don't match the current data for these form(s):\n${details}\n\n` +
        "Import does not roll back the project definition — fix the export file or the project's fields, then try again.";
      setStatus(`Import failed for ${scopeLabel} — field mismatch (${mismatch.hard.length} form(s)).`);
      window.alert(msg);
      return;
    }
    const skipNote = mismatch.skipped.length
      ? ` (${mismatch.skipped.length} form(s) had no current data to check field names against — imported without a mismatch check.)`
      : "";

    if (formName) {
      const fromFileNote = scopedFromFileOnly
        ? `\n\nThis file is a form-scoped export for “${formName}” (Project Data is at project scope / collapsed). ` +
          `Import still replaces that form only — other forms keep their current rows.`
        : "";
      const body =
        `Replace response data for form “${formName}” only?\n\n` +
        "Other forms in this project keep their current rows. Import does not change the project definition." +
        fromFileNote;
      if (!window.confirm(`Import Form Data — “${formName}”\n\n${body}`)) return;
      const merged = mergeFormIntoExport(current.forms || [], formsForImport[0], formName);
      /* Defensive: merged payload must include every sibling form from `current`. */
      formsForImport = merged;
    } else {
      const c = TawalaProjectOps.CONFIRMS.importResponses;
      if (!window.confirm(`${c.title} — “${displayName}”\n\n${c.body}`)) return;
    }

    setStatus(`Importing into ${scopeLabel}…`);
    const result = await TawalaDemo.importResponses(uniqueId, formsForImport, {
      source: payload.source,
      mode: "replace",
    });
    if (result.status !== "success") {
      const msg = `Couldn't import into ${scopeLabel}\n\n${result.error || "unknown error"}`;
      setStatus(`Import failed for ${scopeLabel}: ${result.error || "unknown error"}`);
      window.alert(msg);
      return;
    }
    const msg = formName
      ? `Imported form “${formName}” into “${displayName}” — project now has ${result.inserted} submission row(s) total.${skipNote}`
      : `Imported ${result.inserted} submission row(s) into “${displayName}”.${skipNote}`;
    setStatus(msg);
    window.alert(msg);
    document.dispatchEvent(
      new CustomEvent("tawala:project-imported", { detail: { projectId, result, formName: formName || null } })
    );
  }

  /**
   * Form-scoped Purge: export all → drop the form → replace-write the rest.
   * Whole-project Purge still uses TawalaDemo.purgeResponses (faster DB delete).
   * @param {{ confirmed?: boolean }} [opts] — pass confirmed:true when the caller already showed the Purge confirm.
   */
  async function handleFormPurgeClick(projectId, formName, opts) {
    const name = String(formName || "");
    if (!name) return;
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't purge — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    const uniqueId = resolveUniqueId(projectId);
    if (!uniqueId) {
      const msg = `“${displayName}” isn’t linked to a live :8080 uniqueId yet. Edit in Designer, then Push → Show in My Tawala, then try Purge again.`;
      setStatus(`Purge unavailable for “${displayName}” — no live uniqueId.`);
      window.alert(`Couldn't purge form “${name}”\n\n${msg}`);
      return;
    }
    if (!(opts && opts.confirmed)) {
      const ok = window.confirm(
        `Purge Form Data\n\n` +
          `Permanently delete all response/submission data for form “${name}” in “${displayName}”?\n\n` +
          `Other forms are left alone. The project definition is not changed.`
      );
      if (!ok) return;
    }
    setStatus(`Purging form “${name}” in “${displayName}”…`);
    const current = await TawalaDemo.exportResponses(uniqueId);
    if (current.status !== "success") {
      /*
       * Live export failed (:3001 down, or up but Docker/Postgres unreachable).
       * Clear seeded demo form counts when this uniqueId has a catalog seed.
       */
      const canMock =
        typeof TawalaDemo.hasDemoResponseSeed === "function"
          ? TawalaDemo.hasDemoResponseSeed(uniqueId)
          : !!(window.TAWALA_DEMO_RESPONSE_SEEDS && window.TAWALA_DEMO_RESPONSE_SEEDS[uniqueId]);
      if (canMock && typeof TawalaDemo.purgeMockFormResponses === "function") {
        const mock = TawalaDemo.purgeMockFormResponses(uniqueId, name);
        if (mock.status === "success") {
          const msg =
            `Purged form “${name}” from “${displayName}” — removed ${mock.removed} demo row(s); ` +
            `${mock.remaining} remain in the project.\n\n` +
            (mock.warning || "Mock offline purge — seeded demo Records only.");
          setStatus(msg);
          window.alert(msg);
          document.dispatchEvent(
            new CustomEvent("tawala:project-purged", {
              detail: { projectId, uniqueId, result: mock, displayName, formName: name },
            })
          );
          return;
        }
      }
      const msg = `Couldn't purge form “${name}”\n\n${current.error || "export failed"}\n\n${TawalaProjectOps.LOCAL_PURGE_HELP}`;
      setStatus(`Purge failed for form “${name}”: ${current.error || "unknown error"}`);
      window.alert(msg);
      return;
    }
    const before = (current.forms || []).reduce((n, f) => n + ((f.rows && f.rows.length) || 0), 0);
    const kept = (current.forms || []).filter((f) => f && f.form !== name);
    const removed = before - kept.reduce((n, f) => n + ((f.rows && f.rows.length) || 0), 0);
    const result = await TawalaDemo.importResponses(uniqueId, kept, {
      source: current.source,
      mode: "replace",
    });
    if (result.status !== "success") {
      const msg = `Couldn't purge form “${name}”\n\n${result.error || "unknown error"}`;
      setStatus(`Purge failed for form “${name}”: ${result.error || "unknown error"}`);
      window.alert(msg);
      return;
    }
    const msg = `Purged form “${name}” from “${displayName}” — removed ${removed} row(s); ${result.inserted} row(s) remain in the project.`;
    setStatus(msg);
    window.alert(msg);
    document.dispatchEvent(
      new CustomEvent("tawala:project-purged", {
        detail: { projectId, uniqueId, result, displayName, formName: name },
      })
    );
  }

  // ---------------------------------------------------------------------
  // BACKUP — currently live pushed definition + current responses + My Tawala properties.
  // Mock JSON bundle (not a Java .backup ZIP). Snapshot of *now*, not the Versions pile.
  // ---------------------------------------------------------------------
  async function handleBackupClick(projectId) {
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't back up — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }

    setStatus(`Backing up “${displayName}”…`);
    const uniqueId = resolveUniqueId(projectId);

    let submissions;
    if (uniqueId) {
      const result = await TawalaDemo.exportResponses(uniqueId);
      submissions =
        result.status === "success"
          ? { source: result.source, forms: result.forms, fieldsByForm: result.fieldsByForm, count: result.count }
          : {
              error: result.error || "export failed",
              note: "Submission data capture failed — Backup still includes definition + properties when those are available.",
            };
    } else {
      submissions = {
        note: "Not linked to a live deploy yet — Backup captured properties (and definition if a snapshot exists), no submission data.",
      };
    }

    const captured = await captureLiveDefinition(project, uniqueId);
    const definition = captured.definition;
    const definitionNote = captured.definitionNote;
    const definitionSource = captured.definitionSource;

    const properties = {
      name: project.name,
      category: project.category,
      shortDescription: project.shortDescription,
      longDescription: project.longDescription,
      iconLabel: project.iconLabel,
      themePath: overlayChromeThemePath(projectId) || displayedThemePath(project),
      rating: project.rating,
      comments: project.comments,
      created: project.created,
      startPoints: project.startPoints,
      testDriveUrl: project.testDriveUrl,
      uniqueId: project.uniqueId || uniqueId || null,
      deployIdentityName: project.deployIdentityName || (definition && definition.deployIdentityName) || null,
      mode: project.mode || null,
    };

    const bundle = {
      tawalaBackupFormat: 2,
      kind: "backup",
      projectId,
      displayName,
      uniqueId: uniqueId || properties.uniqueId || null,
      createdAt: new Date().toISOString(),
      properties,
      definition,
      definitionNote,
      definitionSource: definitionSource || project.jsonFile || null,
      submissions,
    };

    const filename = `${TawalaTransfer.slugifyProjectId(displayName)}.backup.json`;
    triggerJsonDownload(filename, bundle);

    const parts = [`Backed up “${displayName}” → ${filename}.`];
    if (submissions && typeof submissions.count === "number") {
      parts.push(`${submissions.count} submission row(s) included.`);
    } else if (submissions && submissions.note) {
      parts.push(submissions.note);
    }
    if (definition) {
      parts.push("Live definition included (Restore can Redeploy it onto :8080).");
    } else {
      parts.push(`No deployable definition included (${definitionNote}). Restore will refuse this file.`);
    }
    const msg = parts.join(" ");
    setStatus(msg);
    window.alert(msg + "\n\nMock backup format is a JSON bundle, not a real Java .backup ZIP — see README § Backup.");
  }

  // ---------------------------------------------------------------------
  // RESTORE — time machine: Redeploy bundled definition onto :8080 (same uniqueId), then data.
  // Fails closed if :3001 / :8080 is down — does not update overlay and claim success.
  // ---------------------------------------------------------------------
  async function handleRestoreClick(projectId) {
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't restore — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }

    const uniqueId = resolveUniqueId(projectId);
    if (!uniqueId) {
      failRestore(
        displayName,
        `“${displayName}” isn’t linked to a live :8080 uniqueId yet. Edit in Designer, then Push → Show in My Tawala, then try Restore again.\n\n${RESTORE_RUNTIME_HELP}`
      );
      return;
    }
    if (typeof TawalaDemo.deployProjectDefinition !== "function") {
      failRestore(displayName, "Push/Restore API helper missing. Refresh the page and try again.");
      return;
    }

    let picked;
    try {
      picked = await pickJsonFile();
    } catch (e) {
      if (e && e.message !== "No file selected") {
        window.alert(`Couldn't read the selected file\n\n${e.message}`);
      }
      return;
    }
    const payload = picked.json;
    if (!payload || typeof payload !== "object" || !payload.tawalaBackupFormat) {
      failRestore(displayName, 'Selected file doesn\'t look like a Tawala Backup JSON (missing "tawalaBackupFormat").');
      return;
    }
    if (!definitionIsDeployable(payload.definition)) {
      failRestore(
        displayName,
        "This backup has no deployable project definition. " +
          "Make a new Backup from a My Tawala project that has a live Push snapshot, then Restore that file.\n\n" +
          "Chrome-only / properties-only snapshots cannot Redeploy onto :8080."
      );
      return;
    }

    const match = matchRestoreToRow(payload, uniqueId, displayName);
    if (!match.ok) {
      failRestore(displayName, match.error);
      return;
    }

    const confirmNote = restoreConfirmNote({
      backupName: match.backupName,
      backupUniqueId: match.backupUniqueId,
      rowName: displayName,
      rowUniqueId: uniqueId,
    });
    const c = TawalaProjectOps.CONFIRMS.restoreProject;
    if (!window.confirm(`${c.title} — “${displayName}”\n\n${confirmNote}\n\n${c.body}`)) return;

    setStatus(`Restoring “${displayName}” — checking :3001 / :8080…`);

    const apiUp = await probeDesignerApi();
    if (!apiUp) {
      failRestore(
        displayName,
        "Designer API on :3001 is not reachable. Restore did not change this project.\n\n" +
          "Start designer-web (`cd designer-web && npm run keep` / ensure-dev-api.sh), then try again.\n\n" +
          RESTORE_RUNTIME_HELP
      );
      return;
    }
    if (typeof TawalaDemo.probeLocalJavaRuntime === "function") {
      const tomcatUp = await TawalaDemo.probeLocalJavaRuntime();
      if (!tomcatUp) {
        failRestore(
          displayName,
          "Java runtime on http://localhost:8080 isn’t reachable. Restore did not change this project.\n\n" +
            "Start Tomcat, then try Restore again.\n\n" +
            RESTORE_RUNTIME_HELP
        );
        return;
      }
    }

    const liveBefore = liveVersionRow(project);
    const backupProps = payload.properties && typeof payload.properties === "object" ? payload.properties : {};
    const backupTheme = themeFromBackupProperties(backupProps);
    /* Overlay properties win. Empty backup Theme → keep the row; stamp that so :8080
     * does not pick up a conflicting themePath buried in the bundled definition. */
    const stampTheme = backupTheme.apply
      ? backupTheme.path
      : overlayChromeThemePath(projectId) || displayedThemePath(project);
    let stamped = attachRowIdentity(payload.definition, project, uniqueId);
    stamped = stampThemePath(stamped, stampTheme);
    if (!stamped) {
      failRestore(displayName, "Could not prepare the bundled definition for Redeploy.");
      return;
    }

    setStatus(`Restoring “${displayName}” — Redeploying definition onto uniqueId ${uniqueId}…`);
    const deploy = await TawalaDemo.deployProjectDefinition(stamped);
    if (!deploy || deploy.status !== "success") {
      const err = (deploy && deploy.error) || "unknown error";
      failRestore(
        displayName,
        `Couldn’t Redeploy the backed-up definition onto :8080.\n\n${err}\n\n` +
          "My Tawala properties and response data were not changed.\n\n" +
          RESTORE_RUNTIME_HELP
      );
      return;
    }
    if (deploy.uniqueId && String(deploy.uniqueId) !== String(uniqueId)) {
      failRestore(
        displayName,
        `Redeploy returned a different uniqueId (${deploy.uniqueId}) than this project (${uniqueId}). ` +
          "Data was not imported and My Tawala properties were not changed, so this row still points at the original live copy.\n\n" +
          RESTORE_RUNTIME_HELP
      );
      return;
    }

    setStatus(`Restoring “${displayName}” — importing response data…`);
    let dataOk = false;
    let dataMsg = "no submission data in this backup.";
    if (payload.submissions && Array.isArray(payload.submissions.forms) && payload.submissions.source) {
      const result = await TawalaDemo.importResponses(uniqueId, payload.submissions.forms, {
        source: payload.submissions.source,
        mode: "replace",
      });
      if (result.status === "success") {
        dataOk = true;
        dataMsg = `restored ${result.inserted} submission row(s).`;
      } else {
        dataMsg = `definition Redeployed, but data restore failed: ${result.error || "unknown error"}.`;
      }
    } else if (payload.submissions && payload.submissions.note) {
      dataMsg = payload.submissions.note;
      dataOk = true;
    } else {
      dataOk = true;
    }

    const restoredAt = isoNow();
    let startPoints = startPointsFromDeployResult(deploy);
    if (
      typeof TawalaDemo !== "undefined" &&
      typeof TawalaDemo.mergeStartPointsPreservingLabels === "function"
    ) {
      startPoints = TawalaDemo.mergeStartPointsPreservingLabels(
        startPoints,
        backupProps.startPoints || project.startPoints,
        uniqueId
      ).filter((sp) => sp && sp.url);
    }
    /* Never copy backup uniqueId onto this row. Restore is a time machine onto
     * this row’s uniqueId — attachRowIdentity already stamps deployUniqueId. */
    const overlayPatch = {};
    if (backupProps.name != null) overlayPatch.name = backupProps.name;
    else if (project.name != null) overlayPatch.name = project.name;
    ["category", "shortDescription", "longDescription", "iconLabel", "rating", "comments"].forEach((key) => {
      if (backupProps[key] !== undefined) overlayPatch[key] = backupProps[key];
    });
    if (backupTheme.apply) overlayPatch.themePath = backupTheme.path;
    if (backupProps.created != null) overlayPatch.created = backupProps.created;
    if (backupProps.mode != null) overlayPatch.mode = backupProps.mode;
    overlayPatch.updated = shortDate(restoredAt);
    overlayPatch.updatedAt = restoredAt;
    if (startPoints.length) {
      overlayPatch.startPoints = startPoints;
      overlayPatch.testDriveUrl = startPoints[0].url || project.testDriveUrl || null;
    }
    if (typeof TawalaTransfer !== "undefined" && typeof TawalaTransfer.upsertMyTawalaProperties === "function") {
      TawalaTransfer.upsertMyTawalaProperties(projectId, overlayPatch);
    }
    const live = liveBefore;
    if (
      live &&
      live.versionNumber != null &&
      typeof TawalaTransfer !== "undefined" &&
      typeof TawalaTransfer.attachVersionDefinition === "function"
    ) {
      TawalaTransfer.attachVersionDefinition(projectId, live.versionNumber, stamped, live.snapshotId || null);
    }

    const defMsg = ` Redeployed definition onto uniqueId ${uniqueId}` + (deploy.mode ? ` (${deploy.mode}).` : ".");
    if (!dataOk) {
      const msg = `Restore incomplete for “${displayName}” —${defMsg} ${dataMsg}`;
      setStatus(msg);
      window.alert(msg + "\n\nMy Tawala properties from the backup were applied. Fix :3001/Postgres, then Restore again (or Import the data).");
      document.dispatchEvent(
        new CustomEvent("tawala:project-restored", { detail: { projectId, payload, deploy, dataOk: false } })
      );
      return;
    }

    const msg = `Restored “${displayName}” — ${dataMsg}${defMsg}`;
    setStatus(msg);
    window.alert(msg);
    document.dispatchEvent(
      new CustomEvent("tawala:project-restored", { detail: { projectId, payload, deploy, dataOk: true } })
    );
  }

  window.TawalaDataOps = {
    handleExportClick,
    handleImportClick,
    handleFormPurgeClick,
    handleBackupClick,
    handleRestoreClick,
    validateExportShape,
    findFieldMismatches,
    filterExportToForm,
    mergeFormIntoExport,
  };
})();
