import fs from "fs";
import path from "path";
import crypto from "crypto";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const DATA_DIR = path.join(__dirname, "..", ".deployed");

function ensureDir() {
  if (!fs.existsSync(DATA_DIR)) fs.mkdirSync(DATA_DIR, { recursive: true });
}

function userDir(userId) {
  const safe = userId.replace(/[^a-zA-Z0-9_-]/g, "_");
  const dir = path.join(DATA_DIR, safe);
  if (!fs.existsSync(dir)) fs.mkdirSync(dir, { recursive: true });
  return dir;
}

function randomId() {
  return crypto.randomBytes(8).toString("hex");
}

/**
 * Forget a prior Node Deploy slot for this project name (sessions + package file).
 * Next saveProject mints a new uniqueId so File→New does not inherit old responses.
 */
export function forgetDeployedProjectByName(userId, projectName) {
  ensureDir();
  const dir = userDir(userId);
  const metaPath = path.join(dir, "_index.json");
  if (!fs.existsSync(metaPath)) return null;
  let index = JSON.parse(fs.readFileSync(metaPath, "utf8"));
  const entry = index.find((e) => e.name === projectName);
  if (!entry) return null;
  index = index.filter((e) => e.name !== projectName);
  fs.writeFileSync(metaPath, JSON.stringify(index, null, 2));
  const filePath = path.join(dir, `${entry.uniqueId}.json`);
  if (fs.existsSync(filePath)) fs.unlinkSync(filePath);
  return entry.uniqueId;
}

export function saveProject(userId, project, { forceNewId = false } = {}) {
  ensureDir();
  const dir = userDir(userId);
  const metaPath = path.join(dir, "_index.json");
  let index = [];
  if (fs.existsSync(metaPath)) {
    index = JSON.parse(fs.readFileSync(metaPath, "utf8"));
  }

  let entry = index.find((e) => e.name === project.name);
  if (entry && forceNewId) {
    const filePath = path.join(dir, `${entry.uniqueId}.json`);
    if (fs.existsSync(filePath)) fs.unlinkSync(filePath);
    index = index.filter((e) => e.name !== project.name);
    entry = null;
  }
  if (!entry) {
    entry = {
      name: project.name,
      uniqueId: randomId(),
      updatedAt: new Date().toISOString(),
      author: userId,
      user: userId,
    };
    index.push(entry);
  } else {
    entry.updatedId = entry.uniqueId;
    entry.updatedAt = new Date().toISOString();
    entry.author = entry.author || userId;
    entry.user = entry.user || userId;
  }

  // Never persist Designer-only flags into the .deployed package.
  const { _freshFromTemplate: _f, ...projectBody } = project ?? {};
  const filePath = path.join(dir, `${entry.uniqueId}.json`);
  fs.writeFileSync(
    filePath,
    JSON.stringify({ userId, project: projectBody, uniqueId: entry.uniqueId }, null, 2),
  );
  fs.writeFileSync(metaPath, JSON.stringify(index, null, 2));
  return entry;
}

export function listProjects(userId) {
  ensureDir();
  const metaPath = path.join(userDir(userId), "_index.json");
  if (!fs.existsSync(metaPath)) return [];
  return JSON.parse(fs.readFileSync(metaPath, "utf8"));
}

export function getProjectByUniqueId(uniqueId) {
  ensureDir();
  // Never scan session/cache dirs — session files use the same `{uniqueId}.json`
  // name (e.g. `preview-designer.json`) and would be mistaken for projects.
  // That made Form Preview Submit crash with TypeError on `project.name`.
  const skipDirs = new Set(["sessions"]);
  for (const user of fs.readdirSync(DATA_DIR)) {
    if (skipDirs.has(user)) continue;
    const dir = path.join(DATA_DIR, user);
    if (!fs.statSync(dir).isDirectory()) continue;
    const file = path.join(dir, `${uniqueId}.json`);
    if (!fs.existsSync(file)) continue;
    const data = JSON.parse(fs.readFileSync(file, "utf8"));
    if (data && typeof data === "object" && data.project) return data;
  }
  return null;
}

/** Occupancy index row for a uniqueId (Node Deploy), or null. */
export function findDeployedIndexByUniqueId(uniqueId) {
  const want = String(uniqueId || "").trim();
  if (!want) return null;
  ensureDir();
  if (!fs.existsSync(DATA_DIR)) return null;
  const skipDirs = new Set(["sessions"]);
  for (const user of fs.readdirSync(DATA_DIR)) {
    if (skipDirs.has(user)) continue;
    const dir = path.join(DATA_DIR, user);
    if (!fs.statSync(dir).isDirectory()) continue;
    const metaPath = path.join(dir, "_index.json");
    if (!fs.existsSync(metaPath)) continue;
    const index = JSON.parse(fs.readFileSync(metaPath, "utf8"));
    if (!Array.isArray(index)) continue;
    const entry = index.find((e) => e && String(e.uniqueId) === want);
    if (entry) return { userId: user, entry };
  }
  return null;
}

/**
 * Free a Node Deploy *name* for occupancy without minting a new uniqueId.
 * Renames the `_index.json` slot (and project.name in the package file). File stays `{uniqueId}.json`.
 * @returns {{ uniqueId: string, previousName: string, name: string, alreadyVacated: boolean, userId: string } | null}
 */
export function vacateDeployedNameByUniqueId(uniqueId, vacatedName) {
  const want = String(uniqueId || "").trim();
  const next = String(vacatedName || "").trim();
  if (!want || !next) return null;
  ensureDir();
  if (!fs.existsSync(DATA_DIR)) return null;
  const skipDirs = new Set(["sessions"]);
  for (const user of fs.readdirSync(DATA_DIR)) {
    if (skipDirs.has(user)) continue;
    const dir = path.join(DATA_DIR, user);
    if (!fs.statSync(dir).isDirectory()) continue;
    const metaPath = path.join(dir, "_index.json");
    if (!fs.existsSync(metaPath)) continue;
    const index = JSON.parse(fs.readFileSync(metaPath, "utf8"));
    if (!Array.isArray(index)) continue;
    const entry = index.find((e) => e && String(e.uniqueId) === want);
    if (!entry) continue;
    const previousName = String(entry.name || "").trim();
    const alreadyVacated = previousName === next;
    if (!alreadyVacated) {
      entry.name = next;
      entry.updatedAt = new Date().toISOString();
      fs.writeFileSync(metaPath, JSON.stringify(index, null, 2));
      const filePath = path.join(dir, `${want}.json`);
      if (fs.existsSync(filePath)) {
        const data = JSON.parse(fs.readFileSync(filePath, "utf8"));
        if (data && typeof data === "object") {
          if (data.project && typeof data.project === "object") {
            data.project.name = next;
          }
          fs.writeFileSync(filePath, JSON.stringify(data, null, 2));
        }
      }
    }
    return {
      uniqueId: want,
      previousName,
      name: next,
      alreadyVacated,
      userId: user,
    };
  }
  return null;
}

/**
 * Designer Form Preview session/runtime id.
 * Previously only `preview-{userId}` — shared across every New Project, so answers
 * and Report/itemization rows from the prior template bled into the next File→New.
 * Scope by project name hash (short, stable while authoring one project).
 */
export function previewRuntimeId(userId, projectName) {
  const u = String(userId ?? "designer").replace(/[^a-zA-Z0-9_-]/g, "_");
  const h = crypto
    .createHash("sha1")
    .update(String(projectName ?? ""))
    .digest("hex")
    .slice(0, 8);
  return `pv-${u}-${h}`;
}

export function getPreview(userId, projectName) {
  const dir = userDir(userId);
  const previewPath = path.join(dir, `_preview_${projectName.replace(/[^a-zA-Z0-9_-]/g, "_")}.json`);
  if (!fs.existsSync(previewPath)) return null;
  return JSON.parse(fs.readFileSync(previewPath, "utf8"));
}

export function putPreview(userId, project, { resetSession = false } = {}) {
  ensureDir();
  const dir = userDir(userId);
  const previewPath = path.join(dir, `_preview_${project.name.replace(/[^a-zA-Z0-9_-]/g, "_")}.json`);
  fs.writeFileSync(previewPath, JSON.stringify({ userId, project }, null, 2));
  // Form Preview embeds uniqueId in the form action (/p/…). File under that id so
  // Submit resolves (sessions/ is not a project store).
  const runtimeId = previewRuntimeId(userId, project.name);
  fs.writeFileSync(
    path.join(dir, `${runtimeId}.json`),
    JSON.stringify({ userId, project, uniqueId: runtimeId }, null, 2),
  );
  return { previewPath, runtimeId, resetSession };
}
