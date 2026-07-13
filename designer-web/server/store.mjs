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

export function saveProject(userId, project) {
  ensureDir();
  const dir = userDir(userId);
  const metaPath = path.join(dir, "_index.json");
  let index = [];
  if (fs.existsSync(metaPath)) {
    index = JSON.parse(fs.readFileSync(metaPath, "utf8"));
  }

  let entry = index.find((e) => e.name === project.name);
  if (!entry) {
    entry = { name: project.name, uniqueId: randomId(), updatedAt: new Date().toISOString() };
    index.push(entry);
  } else {
    entry.updatedId = entry.uniqueId;
    entry.updatedAt = new Date().toISOString();
  }

  const filePath = path.join(dir, `${entry.uniqueId}.json`);
  fs.writeFileSync(filePath, JSON.stringify({ userId, project, uniqueId: entry.uniqueId }, null, 2));
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

export function getPreview(userId, projectName) {
  const dir = userDir(userId);
  const previewPath = path.join(dir, `_preview_${projectName.replace(/[^a-zA-Z0-9_-]/g, "_")}.json`);
  if (!fs.existsSync(previewPath)) return null;
  return JSON.parse(fs.readFileSync(previewPath, "utf8"));
}

export function putPreview(userId, project) {
  ensureDir();
  const dir = userDir(userId);
  const previewPath = path.join(dir, `_preview_${project.name.replace(/[^a-zA-Z0-9_-]/g, "_")}.json`);
  fs.writeFileSync(previewPath, JSON.stringify({ userId, project }, null, 2));
  // Form Preview embeds uniqueId `preview-{userId}` in the form action (/p/…).
  // Keep a real project file under that id so Submit resolves (sessions/ is not a project).
  const runtimeId = `preview-${userId.replace(/[^a-zA-Z0-9_-]/g, "_")}`;
  fs.writeFileSync(
    path.join(dir, `${runtimeId}.json`),
    JSON.stringify({ userId, project, uniqueId: runtimeId }, null, 2),
  );
  return previewPath;
}
