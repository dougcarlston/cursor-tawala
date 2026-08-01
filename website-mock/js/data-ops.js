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

  // ---------------------------------------------------------------------
  // EXPORT — Excel response data only (see README glossary).
  // ---------------------------------------------------------------------
  async function handleExportClick(projectId) {
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't export — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    const uniqueId = resolveUniqueId(projectId);
    if (!uniqueId) {
      const msg = `“${displayName}” isn’t linked to a live deploy yet. Deploy from Designer, then try Export again.`;
      setStatus(`Export unavailable for “${displayName}” — not deployed.`);
      window.alert(`Couldn't export “${displayName}”\n\n${msg}`);
      return;
    }
    setStatus(`Exporting “${displayName}”…`);
    const result = await TawalaDemo.exportResponses(uniqueId);
    if (result.status !== "success") {
      const msg = `Couldn't export “${displayName}”\n\n${result.error || "unknown error"}\n\n${TawalaProjectOps.LOCAL_PURGE_HELP}`;
      setStatus(`Export failed for “${displayName}”: ${result.error || "unknown error"}`);
      window.alert(msg);
      return;
    }
    const bundle = {
      tawalaExportFormat: 1,
      kind: "export",
      projectId,
      displayName,
      uniqueId,
      source: result.source,
      exportedAt: new Date().toISOString(),
      fieldsByForm: result.fieldsByForm,
      forms: result.forms,
      count: result.count,
    };
    const filename = `${TawalaTransfer.slugifyProjectId(displayName)}.export.json`;
    triggerJsonDownload(filename, bundle);
    const msg = `Exported “${displayName}” — ${result.count} submission row(s) across ${result.forms.length} form(s). Saved as ${filename} (mock JSON, not Excel — see README § Export/Import).`;
    setStatus(msg);
    window.alert(msg);
  }

  // ---------------------------------------------------------------------
  // IMPORT — restore messed-up data into the CURRENT project; field mismatch fails.
  // ---------------------------------------------------------------------
  async function handleImportClick(projectId) {
    if (!requireDataApi()) return;
    const project = resolveProject(projectId);
    const displayName = displayNameFor(project, projectId);
    if (!project) {
      window.alert(`Can't import — unknown My Tawala project: ${projectId || "(none)"}`);
      return;
    }
    const uniqueId = resolveUniqueId(projectId);
    if (!uniqueId) {
      const msg = `“${displayName}” isn’t linked to a live deploy yet. Deploy from Designer, then try Import again.`;
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

    setStatus(`Checking fields for “${displayName}”…`);
    const current = await TawalaDemo.exportResponses(uniqueId);
    const currentFieldsByForm = current.status === "success" ? current.fieldsByForm : {};
    const mismatch = findFieldMismatches(payload.fieldsByForm || {}, currentFieldsByForm);
    if (mismatch.hard.length) {
      const details = mismatch.hard
        .map((m) => `  • ${m.form}: file has [${m.fileFields.join(", ")}] — current data has [${m.currentFields.join(", ")}]`)
        .join("\n");
      const msg =
        `Import failed — field mismatch for “${displayName}”.\n\n` +
        `The imported file's fields don't match the current data for these form(s):\n${details}\n\n` +
        "Import does not roll back the project definition — fix the export file or the project's fields, then try again.";
      setStatus(`Import failed for “${displayName}” — field mismatch (${mismatch.hard.length} form(s)).`);
      window.alert(msg);
      return;
    }
    const skipNote = mismatch.skipped.length
      ? ` (${mismatch.skipped.length} form(s) had no current data to check field names against — imported without a mismatch check.)`
      : "";

    const c = TawalaProjectOps.CONFIRMS.importResponses;
    if (!window.confirm(`${c.title} — “${displayName}”\n\n${c.body}`)) return;

    setStatus(`Importing into “${displayName}”…`);
    const result = await TawalaDemo.importResponses(uniqueId, payload.forms, {
      source: payload.source,
      mode: "replace",
    });
    if (result.status !== "success") {
      const msg = `Couldn't import into “${displayName}”\n\n${result.error || "unknown error"}`;
      setStatus(`Import failed for “${displayName}”: ${result.error || "unknown error"}`);
      window.alert(msg);
      return;
    }
    const msg = `Imported ${result.inserted} submission row(s) into “${displayName}”.${skipNote}`;
    setStatus(msg);
    window.alert(msg);
    document.dispatchEvent(new CustomEvent("tawala:project-imported", { detail: { projectId, result } }));
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
    handleBackupClick,
    handleRestoreClick,
    validateExportShape,
    findFieldMismatches,
  };
})();
