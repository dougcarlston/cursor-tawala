/**
 * My Tawala data-lifecycle ops (owner Aug 1, 2026): BACKUP / RESTORE (paired definition + data
 * ZIP in Java; JSON bundle here) and EXPORT / IMPORT (Excel response data only). See
 * website-mock/README.md § Save/Deploy/Publish glossary for the product contract these follow.
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
      const msg = `“${displayName}” isn’t linked to a live deploy yet. Push from Designer, then try Export again.`;
      setStatus(`Export unavailable for “${displayName}” — not deployed.`);
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
      const msg = `“${displayName}” isn’t linked to a live deploy yet. Push from Designer, then try Import again.`;
      setStatus(`Import unavailable for “${displayName}” — not deployed.`);
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
      const msg = `“${displayName}” isn’t linked to a live deploy yet. Push from Designer, then try Purge again.`;
      setStatus(`Purge unavailable for “${displayName}” — not deployed.`);
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
  // BACKUP — paired definition + data (+ properties/links); mock JSON bundle, not a Java ZIP.
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
              note: "Submission data capture failed — Backup still includes catalog properties. See status message.",
            };
    } else {
      submissions = {
        note: "Not linked to a live deploy yet — Backup captured catalog properties only, no submission data.",
      };
    }

    let definition = null;
    let definitionNote;
    if (project.jsonFile) {
      if (/^projects\//.test(project.jsonFile)) {
        try {
          const res = await fetch(project.jsonFile);
          if (res.ok) {
            definition = await res.json();
            definitionNote = `Fetched from ${project.jsonFile} (repo backup copy — may lag the owner's live Designer working copy).`;
          } else {
            definitionNote = `Could not fetch ${project.jsonFile} (HTTP ${res.status}).`;
          }
        } catch (e) {
          definitionNote = `Could not fetch ${project.jsonFile}: ${e.message}`;
        }
      } else {
        definitionNote = `Definition JSON lives outside website-mock's served root (${project.jsonFile}) — not fetchable from :5500. See README § Backup/Restore gaps.`;
      }
    } else {
      definitionNote =
        "No definition JSON file on disk for this project (e.g. a Deploy-overlay project) — Backup captured catalog properties only.";
    }

    const properties = {
      name: project.name,
      category: project.category,
      shortDescription: project.shortDescription,
      longDescription: project.longDescription,
      iconLabel: project.iconLabel,
      rating: project.rating,
      comments: project.comments,
      created: project.created,
      startPoints: project.startPoints,
      testDriveUrl: project.testDriveUrl,
      uniqueId: project.uniqueId || uniqueId || null,
      mode: project.mode || null,
    };

    const bundle = {
      tawalaBackupFormat: 1,
      kind: "backup",
      projectId,
      displayName,
      createdAt: new Date().toISOString(),
      properties,
      definition,
      definitionNote,
      definitionSource: project.jsonFile || null,
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
    parts.push(definition ? "Definition JSON included." : `No definition JSON included (${definitionNote})`);
    const msg = parts.join(" ");
    setStatus(msg);
    window.alert(msg + "\n\nMock backup format is a JSON bundle, not a real Java .backup ZIP — see README § Backup/Restore gaps.");
  }

  // ---------------------------------------------------------------------
  // RESTORE — re-applies the matching definition (mock: catalog properties), then data.
  // ---------------------------------------------------------------------
  async function handleRestoreClick(projectId) {
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't restore — unknown My Tawala project: ${projectId || "(none)"}`);
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
      const msg = 'Selected file doesn\'t look like a Tawala Backup JSON (missing "tawalaBackupFormat").';
      setStatus(`Restore failed for “${displayName}”: ${msg}`);
      window.alert(`Couldn't restore “${displayName}”\n\n${msg}`);
      return;
    }

    const nameNote =
      payload.displayName && payload.displayName !== displayName
        ? `\n\nNote: this backup was made from “${payload.displayName}” — you're restoring it into “${displayName}”.`
        : "";
    const c = TawalaProjectOps.CONFIRMS.restoreProject;
    if (!window.confirm(`${c.title} — “${displayName}”\n\n${c.body}${nameNote}`)) return;

    setStatus(`Restoring “${displayName}”…`);

    // 1) Properties ("definition" surface available to this mock) — see transfer.js
    //    upsertMyTawalaProperties + README § Backup/Restore for what "definition" means here.
    if (payload.properties && typeof TawalaTransfer !== "undefined" && typeof TawalaTransfer.upsertMyTawalaProperties === "function") {
      const restoredAt = isoNow();
      TawalaTransfer.upsertMyTawalaProperties(projectId, {
        ...payload.properties,
        updated: shortDate(restoredAt),
        updatedAt: restoredAt,
      });
    }

    // 2) Submission data
    let dataMsg = "no submission data in this backup.";
    if (payload.submissions && Array.isArray(payload.submissions.forms) && payload.submissions.source) {
      const uniqueId = (payload.properties && payload.properties.uniqueId) || resolveUniqueId(projectId);
      if (!uniqueId) {
        dataMsg =
          "backup includes submission data, but this project isn't linked to a live deploy — Deploy first, then Restore again to bring back the data.";
      } else {
        const result = await TawalaDemo.importResponses(uniqueId, payload.submissions.forms, {
          source: payload.submissions.source,
          mode: "replace",
        });
        dataMsg =
          result.status === "success"
            ? `restored ${result.inserted} submission row(s).`
            : `data restore failed: ${result.error || "unknown error"}.`;
      }
    } else if (payload.submissions && payload.submissions.note) {
      dataMsg = payload.submissions.note;
    }

    const defMsg = payload.definition
      ? " Definition JSON was in the backup, but the mock does not push project definitions into a live Designer session (Designer :5173 and this mock :5500 don't share storage) — open it manually if you need the design itself back."
      : "";

    const msg = `Restored “${displayName}” — ${dataMsg}${defMsg}`;
    setStatus(msg);
    window.alert(msg);
    document.dispatchEvent(new CustomEvent("tawala:project-restored", { detail: { projectId, payload } }));
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
