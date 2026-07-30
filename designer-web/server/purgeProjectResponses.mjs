/**
 * Local purge of form submissions by deployed uniqueId (`/p/{uniqueId}/…`).
 * Shells out to scripts/purge-project-by-unique-id.sh (Docker Postgres).
 * Also clears Node session store records when that uniqueId exists locally.
 */
import { spawn } from "child_process";
import path from "path";
import { fileURLToPath } from "url";
import * as store from "./store.mjs";
import { resetSession } from "./sessionStore.mjs";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "../..");
const PURGE_SCRIPT = path.join(REPO_ROOT, "scripts/purge-project-by-unique-id.sh");

const UNIQUE_ID_RE = /^[A-Za-z0-9]{1,20}$/;

export function isValidUniqueId(uniqueId) {
  return typeof uniqueId === "string" && UNIQUE_ID_RE.test(uniqueId);
}

/** Extract uniqueId from a local runtime URL (`…/p/{id}/Form`). */
export function uniqueIdFromRuntimeUrl(url) {
  if (!url || typeof url !== "string") return null;
  const m = url.match(/\/p\/([A-Za-z0-9]{1,20})(?:\/|$)/);
  return m ? m[1] : null;
}

function runScript(uniqueId) {
  return new Promise((resolve) => {
    const child = spawn("bash", [PURGE_SCRIPT, uniqueId], {
      cwd: REPO_ROOT,
      env: process.env,
    });
    let stdout = "";
    let stderr = "";
    child.stdout.on("data", (chunk) => {
      stdout += chunk.toString();
    });
    child.stderr.on("data", (chunk) => {
      stderr += chunk.toString();
    });
    child.on("error", (err) => {
      resolve({
        ok: false,
        mode: "java-db",
        error: String(err.message || err),
        stdout,
        stderr,
        deleted: null,
      });
    });
    child.on("close", (code) => {
      const deletedMatch = stdout.match(/Deleted\s+(\d+)\s+submission/i);
      const deleted = deletedMatch ? Number(deletedMatch[1]) : null;
      const projectMatch = stdout.match(/^\s*(\S.+?)\s+\|\s+[A-Za-z0-9]+\s+\|/m);
      resolve({
        ok: code === 0,
        mode: "java-db",
        exitCode: code,
        deleted,
        projectHint: projectMatch ? projectMatch[1].trim() : null,
        error: code === 0 ? null : (stderr.trim() || stdout.trim() || `exit ${code}`),
        stdout: stdout.trim(),
        stderr: stderr.trim(),
      });
    });
  });
}

/**
 * @param {string} uniqueId
 * @returns {Promise<{
 *   status: "success" | "failure",
 *   uniqueId: string,
 *   javaDb?: object,
 *   nodeSession?: { cleared: boolean },
 *   error?: string
 * }>}
 */
export async function purgeProjectResponsesByUniqueId(uniqueId) {
  if (!isValidUniqueId(uniqueId)) {
    return { status: "failure", uniqueId, error: "invalid uniqueId" };
  }

  const javaDb = await runScript(uniqueId);

  let nodeSession = { cleared: false };
  const data = store.getProjectByUniqueId(uniqueId);
  if (data?.project) {
    resetSession(uniqueId, data.project);
    nodeSession = { cleared: true };
  }

  // Success if Postgres purge worked, OR Node session cleared when no Java row
  // (dev-only deploys). Failure when both miss / DB unreachable.
  if (javaDb.ok) {
    return { status: "success", uniqueId, javaDb, nodeSession };
  }

  if (nodeSession.cleared) {
    return {
      status: "success",
      uniqueId,
      javaDb,
      nodeSession,
      warning: javaDb.error || "Java DB purge skipped; Node session cleared",
    };
  }

  return {
    status: "failure",
    uniqueId,
    javaDb,
    nodeSession,
    error: javaDb.error || "purge failed",
  };
}
