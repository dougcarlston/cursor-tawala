/**
 * Persist Designer project JSON for My Tawala “Deploy this version”.
 * Receipts in the :5500 URL cannot carry a full definition — Show in My Tawala
 * POSTs here, then the mock stores snapshotId (and may hydrate definition into
 * localStorage). Redeploy fetches by id when the overlay has no cached body.
 */
import fs from "fs";
import path from "path";
import crypto from "crypto";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const SNAP_DIR = path.join(__dirname, "..", ".deployed", "version-snapshots");

function ensureDir() {
  if (!fs.existsSync(SNAP_DIR)) fs.mkdirSync(SNAP_DIR, { recursive: true });
}

function randomId() {
  return crypto.randomBytes(12).toString("hex");
}

function stripDesignerOnlyFlags(project) {
  if (!project || typeof project !== "object") return project;
  const { _freshFromTemplate: _drop, ...rest } = project;
  return rest;
}

/**
 * @param {{ project: object, uniqueId?: string|null, projectId?: string|null, versionDescription?: string, at?: string }} input
 * @returns {{ snapshotId: string, at: string }}
 */
export function saveVersionSnapshot(input) {
  const project = stripDesignerOnlyFlags(input?.project);
  if (!project || typeof project !== "object" || !project.name) {
    throw new Error("project required");
  }
  ensureDir();
  const snapshotId = randomId();
  const at = input.at || new Date().toISOString();
  const record = {
    snapshotId,
    at,
    uniqueId: input.uniqueId || null,
    projectId: input.projectId || null,
    versionDescription:
      input.versionDescription != null ? String(input.versionDescription) : "",
    project,
  };
  const filePath = path.join(SNAP_DIR, `${snapshotId}.json`);
  fs.writeFileSync(filePath, JSON.stringify(record, null, 2), "utf8");
  return { snapshotId, at };
}

/**
 * @param {string} snapshotId
 * @returns {object|null}
 */
export function getVersionSnapshot(snapshotId) {
  if (!snapshotId || typeof snapshotId !== "string") return null;
  if (!/^[a-f0-9]{16,64}$/i.test(snapshotId)) return null;
  const filePath = path.join(SNAP_DIR, `${snapshotId}.json`);
  if (!fs.existsSync(filePath)) return null;
  try {
    return JSON.parse(fs.readFileSync(filePath, "utf8"));
  } catch {
    return null;
  }
}

export function versionSnapshotsDir() {
  return SNAP_DIR;
}
