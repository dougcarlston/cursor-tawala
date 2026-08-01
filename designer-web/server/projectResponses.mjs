/**
 * Local EXPORT / IMPORT / BACKUP / RESTORE response-data plumbing for website-mock.
 *
 * Response data (`submission.contents`) is legacy XStream XML for a `Map<String,String[]>`
 * (see TawalaWebapp-build1700/src/com/tawala/project/FormSubmission.java `getContents` /
 * `setContents`, confirmed against real rows and scripts/seed-dirtbowl-*.sql):
 *
 *   <linked-hash-map>
 *     <entry><string>FieldName</string><string-array><string>Value</string></string-array></entry>
 *     ...
 *   </linked-hash-map>
 *
 * This module parses that XML into flat `{ field: [values...] }` objects (for Export/Backup)
 * and rebuilds it from the same shape (for Import/Restore) — same operation is shared by both
 * ops verb pairs at the DB layer; the EXPORT/IMPORT vs BACKUP/RESTORE product distinction
 * (data-only vs paired definition+data, field-mismatch checks) lives in website-mock/js/*.
 *
 * Mock gaps vs Java Project Manager (see website-mock/README.md for the full write-up):
 *   - Real EXPORT produces an Excel workbook; this produces CSV/JSON for a static mock.
 *   - Real IMPORT validates uploaded columns against the live Form field definitions; this
 *     validates against the project's own most-recent Export/current submissions (client-side
 *     in website-mock/js/transfer.js), since the mock has no server-side access to the Designer
 *     project's actual field list for Java-deployed (non-designer-web-store) projects.
 *   - Real BACKUP is a `.backup` ZIP with project definition XML + data + properties/links;
 *     this bundles the My Tawala catalog metadata (not the full Designer definition) + this
 *     same response data as one downloadable JSON file.
 */
import { spawn } from "child_process";
import path from "path";
import { fileURLToPath } from "url";
import { getSession, saveSession } from "./sessionStore.mjs";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "../..");
const EXPORT_SCRIPT = path.join(REPO_ROOT, "scripts/export-project-responses.sh");
const RESTORE_SCRIPT = path.join(REPO_ROOT, "scripts/restore-project-responses.sh");

const UNIQUE_ID_RE = /^[A-Za-z0-9]{1,20}$/;

export function isValidUniqueId(uniqueId) {
  return typeof uniqueId === "string" && UNIQUE_ID_RE.test(uniqueId);
}

function runCapture(scriptPath, args, { input } = {}) {
  return new Promise((resolve) => {
    const child = spawn("bash", [scriptPath, ...args], { cwd: REPO_ROOT, env: process.env });
    let stdout = "";
    let stderr = "";
    child.stdout.on("data", (c) => (stdout += c.toString()));
    child.stderr.on("data", (c) => (stderr += c.toString()));
    child.on("error", (err) => {
      resolve({ ok: false, exitCode: null, stdout, stderr, error: String(err.message || err) });
    });
    child.on("close", (code) => {
      resolve({ ok: code === 0, exitCode: code, stdout, stderr });
    });
    if (input != null) {
      child.stdin.write(input);
    }
    child.stdin.end();
  });
}

/** Un-escape XML entities the way XStream writes them (minimal, matches our own writer too). */
function unescapeXml(s) {
  return String(s)
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&")
    .replace(/&quot;/g, '"')
    .replace(/&apos;/g, "'");
}

function escapeXml(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;")
    .replace(/'/g, "&apos;");
}

/**
 * Parse one `<linked-hash-map>…</linked-hash-map>` XStream blob into `{ field: [values] }`,
 * preserving field order. Tolerates self-closed `<string-array/>` (no answer / unanswered MCQ).
 */
export function parseSubmissionContentsXml(xml) {
  const fields = {};
  const order = [];
  if (!xml || typeof xml !== "string") return { fields, order };
  const entryRe = /<entry>\s*<string>([\s\S]*?)<\/string>\s*(<string-array\s*\/>|<string-array>([\s\S]*?)<\/string-array>)\s*<\/entry>/g;
  let m;
  while ((m = entryRe.exec(xml))) {
    const name = unescapeXml(m[1]);
    const inner = m[3] || "";
    const values = [];
    const valRe = /<string>([\s\S]*?)<\/string>/g;
    let vm;
    while ((vm = valRe.exec(inner))) {
      values.push(unescapeXml(vm[1]));
    }
    if (!(name in fields)) order.push(name);
    fields[name] = values;
  }
  return { fields, order };
}

/** Rebuild the XStream `<linked-hash-map>` blob from `{ field: value | [values] }`. */
export function buildSubmissionContentsXml(fields, order) {
  const names = order && order.length ? order : Object.keys(fields || {});
  const entries = names
    .map((name) => {
      const raw = fields ? fields[name] : undefined;
      const values = Array.isArray(raw) ? raw : raw == null ? [] : [raw];
      const inner = values.map((v) => `<string>${escapeXml(v)}</string>`).join("");
      const arrayXml = values.length ? `<string-array>${inner}</string-array>` : "<string-array/>";
      return `<entry><string>${escapeXml(name)}</string>${arrayXml}</entry>`;
    })
    .join("");
  return `<linked-hash-map>${entries}</linked-hash-map>`;
}

/** CSV cell: multi-value fields join with "|" (documented mock convention, see README). */
function csvCell(value) {
  const s = Array.isArray(value) ? value.join("|") : value == null ? "" : String(value);
  if (/[",\n\r]/.test(s)) {
    return `"${s.replace(/"/g, '""')}"`;
  }
  return s;
}

export function rowsToCsv(columns, rows) {
  const header = ["Form", "Submitted", ...columns].map(csvCell).join(",");
  const lines = rows.map((r) =>
    [r.form || "", r.createdAt || "", ...columns.map((c) => csvCell(r.fields ? r.fields[c] : ""))]
      .map(csvCell)
      .join(",")
  );
  return [header, ...lines].join("\r\n");
}

/**
 * @param {string} uniqueId
 * @returns {Promise<{status:"success"|"failure", uniqueId, columns?, rows?, csv?, error?}>}
 */
export async function exportProjectResponses(uniqueId) {
  if (!isValidUniqueId(uniqueId)) {
    return { status: "failure", uniqueId, error: "invalid uniqueId" };
  }
  const result = await runCapture(EXPORT_SCRIPT, [uniqueId]);
  if (!result.ok) {
    return {
      status: "failure",
      uniqueId,
      error: (result.stderr || result.stdout || `exit ${result.exitCode}`).trim(),
    };
  }
  const m = result.stdout.match(/EXPORT_JSON_BEGIN\s*\n([\s\S]*?)\nEXPORT_JSON_END/);
  if (!m) {
    return { status: "failure", uniqueId, error: "Unexpected export script output" };
  }
  let raw;
  try {
    raw = JSON.parse(m[1].trim());
  } catch (e) {
    return { status: "failure", uniqueId, error: `Bad export JSON: ${e.message}` };
  }
  const columns = [];
  const seen = new Set();
  const rows = (raw || []).map((r) => {
    const { fields, order } = parseSubmissionContentsXml(r.contents);
    order.forEach((name) => {
      if (!seen.has(name)) {
        seen.add(name);
        columns.push(name);
      }
    });
    return {
      submissionId: r.submission_id,
      form: r.form,
      createdAt: r.created_dt,
      fields,
    };
  });
  return {
    status: "success",
    uniqueId,
    columns,
    rows,
    csv: rowsToCsv(columns, rows),
  };
}

/** Unique dollar-quote tag that does not occur in `text` — SQL-injection-proof literal wrapping. */
function dollarTagFor(text, seed) {
  let tag = `TW${seed}`;
  while (text.includes(`$${tag}$`)) tag += "X";
  return tag;
}

function dollarQuote(text, seed) {
  const tag = dollarTagFor(String(text ?? ""), seed);
  return `$${tag}$${text ?? ""}$${tag}$`;
}

/**
 * Replace all submissions for `uniqueId` with `rows` ({form, createdAt, fields}).
 * Shared DB primitive for RESTORE (paired snapshot data) and IMPORT (data-only) — both are
 * "replace current submissions with this set"; product-level checks live in website-mock/js/*.
 *
 * @param {string} uniqueId
 * @param {Array<{form?:string, createdAt?:string, fields:Record<string,string|string[]>}>} rows
 */
export async function restoreProjectResponses(uniqueId, rows) {
  if (!isValidUniqueId(uniqueId)) {
    return { status: "failure", uniqueId, error: "invalid uniqueId" };
  }
  const list = Array.isArray(rows) ? rows : [];

  const sqlParts = ["BEGIN;"];
  sqlParts.push(
    `WITH gone AS (DELETE FROM submission s USING user_project up WHERE s.project_id = up.project_id AND up.unique_random_id = '${uniqueId}' RETURNING s.submission_id) SELECT 'DELETED_COUNT:' || COUNT(*) FROM gone;`
  );
  list.forEach((row, i) => {
    const xml = buildSubmissionContentsXml(row.fields || {}, row.order);
    const form = row.form || "Form 1";
    const createdAt = row.createdAt && !Number.isNaN(new Date(row.createdAt).getTime())
      ? new Date(row.createdAt).toISOString()
      : new Date().toISOString();
    const contentsLit = dollarQuote(xml, `C${i}_`);
    const formLit = dollarQuote(form, `F${i}_`);
    const createdLit = dollarQuote(createdAt, `D${i}_`);
    sqlParts.push(
      `INSERT INTO submission (submission_id, contents, form, created_dt, project_id) ` +
        `SELECT nextval('seq_submission_id'), ${contentsLit}, ${formLit}, ${createdLit}::timestamptz, up.project_id ` +
        `FROM user_project up WHERE up.unique_random_id = '${uniqueId}';`
    );
  });
  sqlParts.push(
    `SELECT 'INSERTED_COUNT:' || COUNT(*) FROM submission s JOIN user_project up ON s.project_id = up.project_id WHERE up.unique_random_id = '${uniqueId}';`
  );
  sqlParts.push("COMMIT;");
  const sql = sqlParts.join("\n");

  const result = await runCapture(RESTORE_SCRIPT, [uniqueId], { input: sql });
  if (!result.ok) {
    return {
      status: "failure",
      uniqueId,
      error: (result.stderr || result.stdout || `exit ${result.exitCode}`).trim(),
    };
  }
  const deletedMatch = result.stdout.match(/DELETED_COUNT:(\d+)/);
  const insertedMatch = result.stdout.match(/INSERTED_COUNT:(\d+)/);
  return {
    status: "success",
    uniqueId,
    deleted: deletedMatch ? Number(deletedMatch[1]) : null,
    inserted: insertedMatch ? Number(insertedMatch[1]) : list.length,
  };
}

/** Group flat `{form, createdAt, fields}` rows into the `forms: [{form, rows:[{createdAt,fields}]}]` wire shape. */
function groupRowsByForm(rows) {
  const order = [];
  const byForm = new Map();
  (rows || []).forEach((r) => {
    const formName = r.form || "Form 1";
    if (!byForm.has(formName)) {
      byForm.set(formName, []);
      order.push(formName);
    }
    byForm.get(formName).push({ createdAt: r.createdAt ?? null, fields: r.fields || {} });
  });
  return order.map((form) => ({ form, rows: byForm.get(form) }));
}

/** `{formName: [fieldName, ...]}` — field-mismatch check shape (website-mock/js/data-ops.js). */
function fieldsByFormFrom(rows) {
  const out = {};
  (rows || []).forEach((r) => {
    const formName = r.form || "Form 1";
    if (!out[formName]) out[formName] = [];
    Object.keys(r.fields || {}).forEach((f) => {
      if (!out[formName].includes(f)) out[formName].push(f);
    });
  });
  return out;
}

function toExportResult(uniqueId, source, rows) {
  return {
    status: "success",
    uniqueId,
    source,
    forms: groupRowsByForm(rows),
    fieldsByForm: fieldsByFormFrom(rows),
    count: rows.length,
  };
}

/** Flatten the wire shape (`forms: [{form, rows:[{createdAt,fields}]}]`) back into DB rows. */
function flattenFormsToRows(forms) {
  const rows = [];
  (forms || []).forEach((f) => {
    const formName = f && f.form;
    const list = Array.isArray(f && f.rows) ? f.rows : [];
    list.forEach((r) => {
      rows.push({ form: formName, createdAt: r && r.createdAt, fields: (r && r.fields) || {} });
    });
  });
  return rows;
}

/**
 * Dev-session fallback (mirrors purgeProjectResponses.mjs "dev-only deploy, no Postgres row"
 * case). Node-store `session.records[formName]` are flat `{field: value}` rows written by
 * itemization/Signup-style forms during Preview/runtime — see sessionStore.mjs appendFormRecord.
 * Returns `null` (not `[]`) when there is no session file at all, so callers can tell
 * "unknown uniqueId" apart from "known dev session, zero submissions yet".
 */
function exportDevSessionRows(uniqueId) {
  const session = getSession(uniqueId);
  if (!session) return null;
  const rows = [];
  Object.entries(session.records || {}).forEach(([formName, list]) => {
    (list || []).forEach((row) => {
      const fields = {};
      Object.entries(row || {}).forEach(([k, v]) => {
        fields[k] = [v == null ? "" : String(v)];
      });
      rows.push({ form: formName, createdAt: null, fields });
    });
  });
  return rows;
}

/** Replace `session.records` from flattened rows; returns `null` when no session file exists. */
function importDevSessionRows(uniqueId, rows) {
  const session = getSession(uniqueId);
  if (!session) return null;
  const grouped = {};
  (rows || []).forEach((r) => {
    const formName = r.form || "Form 1";
    const flat = {};
    Object.entries(r.fields || {}).forEach(([k, v]) => {
      flat[k] = Array.isArray(v) ? (v.length <= 1 ? (v[0] ?? "") : v.join(", ")) : (v ?? "");
    });
    if (!grouped[formName]) grouped[formName] = [];
    grouped[formName].push(flat);
  });
  session.records = grouped;
  saveSession(uniqueId, session);
  return { status: "success", uniqueId, deleted: null, inserted: rows.length, source: "dev-session" };
}

/**
 * EXPORT (data-only) and the data half of BACKUP — website-mock `/api/export-responses`.
 * Tries Postgres (Java-deployed / real projects) first, then falls back to the Node dev-session
 * store (dev-only deploys with no `user_project` row) the same way purgeProjectResponses.mjs
 * already falls back for PURGE. Fails only when neither source has this uniqueId at all.
 *
 * @param {string} uniqueId
 * @returns {Promise<{status:"success"|"failure", uniqueId, source?:"postgres"|"dev-session",
 *   forms?:Array<{form:string, rows:Array<{createdAt:?string, fields:Record<string,string[]>}>}>,
 *   fieldsByForm?:Record<string,string[]>, count?:number, error?:string}>}
 */
export async function exportProjectResponsesByUniqueId(uniqueId) {
  if (!isValidUniqueId(uniqueId)) {
    return { status: "failure", uniqueId, error: "invalid uniqueId" };
  }
  const pg = await exportProjectResponses(uniqueId);
  if (pg.status === "success") {
    return toExportResult(uniqueId, "postgres", pg.rows);
  }
  const devRows = exportDevSessionRows(uniqueId);
  if (devRows) {
    return toExportResult(uniqueId, "dev-session", devRows);
  }
  return { status: "failure", uniqueId, error: pg.error || "export failed" };
}

/**
 * IMPORT (data-only, replaces current submissions) and the data half of RESTORE —
 * website-mock `/api/import-responses`. `forms` is the same shape `exportProjectResponsesByUniqueId`
 * returns. Writes to whichever store actually has this uniqueId (Postgres, else dev-session);
 * `opts.source` is informational only (client already ran the field-mismatch check).
 * `opts.mode` — only `"replace"` is implemented; the mock does not support merge-import.
 *
 * @param {string} uniqueId
 * @param {Array<{form:string, rows:Array<{createdAt?:string, fields:Record<string,string[]>}>}>} forms
 * @param {{source?: "postgres"|"dev-session", mode?: "replace"}} [opts]
 */
export async function importProjectResponsesByUniqueId(uniqueId, forms, opts = {}) {
  if (!isValidUniqueId(uniqueId)) {
    return { status: "failure", uniqueId, error: "invalid uniqueId" };
  }
  if (!Array.isArray(forms)) {
    return { status: "failure", uniqueId, error: "forms array required" };
  }
  const rows = flattenFormsToRows(forms);
  const pg = await restoreProjectResponses(uniqueId, rows);
  if (pg.status === "success") {
    return pg;
  }
  const devResult = importDevSessionRows(uniqueId, rows);
  if (devResult) {
    return devResult;
  }
  return { status: "failure", uniqueId, error: pg.error || "import failed" };
}
