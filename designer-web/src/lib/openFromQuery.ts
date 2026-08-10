/**
 * Deep-link open for My Tawala → Edit project in Designer.
 *
 * Supported query params (first match wins):
 *   ?snapshot=<id>     — GET /api/version-snapshots/:id (Show in My Tawala / redeploy cache)
 *   ?mockJson=<path>   — GET /api/open-mock-json?path=… (website-mock/projects/{mytawala|library}/*.json)
 *
 * Clears the query string after a successful load so refresh does not re-import.
 */
import { clearProjectFileHandle } from "@/lib/shellCommands";
import { useProjectStore } from "@/store/projectStore";

const MOCK_JSON_RE =
  /^(projects\/(mytawala|library)\/[^/]+\.json|designer-web\/public\/samples\/.+\.json)$/i;
const SNAPSHOT_RE = /^[a-f0-9]{16,64}$/i;

function stripOpenQueryParams(): void {
  try {
    const url = new URL(window.location.href);
    if (!url.searchParams.has("snapshot") && !url.searchParams.has("mockJson")) return;
    url.searchParams.delete("snapshot");
    url.searchParams.delete("mockJson");
    const qs = url.searchParams.toString();
    window.history.replaceState({}, "", url.pathname + (qs ? `?${qs}` : "") + url.hash);
  } catch {
    /* ignore */
  }
}

async function loadProjectObject(project: unknown, label: string): Promise<void> {
  if (!project || typeof project !== "object" || !("name" in project)) {
    throw new Error(`Invalid project from ${label}`);
  }
  clearProjectFileHandle();
  useProjectStore.getState().importJson(JSON.stringify(project));
  useProjectStore.getState().setStatus(`Opened ${String((project as { name: string }).name)} from My Tawala`);
}

/**
 * If the URL asks to open a project, fetch and load it. Returns true when a load was attempted.
 */
export async function tryOpenProjectFromQuery(): Promise<boolean> {
  const params = new URLSearchParams(window.location.search);
  const snapshotId = (params.get("snapshot") || "").trim();
  const mockJson = (params.get("mockJson") || "").trim();

  if (snapshotId) {
    if (!SNAPSHOT_RE.test(snapshotId)) {
      alert("Couldn’t open project — invalid snapshot id in the URL.");
      stripOpenQueryParams();
      return true;
    }
    try {
      const res = await fetch(`/api/version-snapshots/${encodeURIComponent(snapshotId)}`);
      const data = (await res.json().catch(() => ({}))) as {
        status?: string;
        project?: unknown;
        error?: string;
      };
      if (!res.ok || !data.project) {
        throw new Error(data.error || `HTTP ${res.status}`);
      }
      await loadProjectObject(data.project, "snapshot");
      stripOpenQueryParams();
    } catch (err) {
      const msg = err instanceof Error ? err.message : String(err);
      alert(
        `Couldn’t open project from snapshot.\n\n${msg}\n\nIs designer-web API on :3001? (cd designer-web && npm run keep)`,
      );
      useProjectStore.getState().setStatus(`Open from My Tawala failed: ${msg}`);
      stripOpenQueryParams();
    }
    return true;
  }

  if (mockJson) {
    if (!MOCK_JSON_RE.test(mockJson) || mockJson.includes("..")) {
      alert("Couldn’t open project — invalid mockJson path in the URL.");
      stripOpenQueryParams();
      return true;
    }
    try {
      const res = await fetch(
        `/api/open-mock-json?path=${encodeURIComponent(mockJson)}`,
      );
      const data = (await res.json().catch(() => ({}))) as {
        status?: string;
        project?: unknown;
        error?: string;
      };
      if (!res.ok || !data.project) {
        throw new Error(data.error || `HTTP ${res.status}`);
      }
      await loadProjectObject(data.project, "mockJson");
      stripOpenQueryParams();
    } catch (err) {
      const msg = err instanceof Error ? err.message : String(err);
      alert(
        `Couldn’t open catalog project JSON.\n\n${msg}\n\nIs designer-web API on :3001? (cd designer-web && npm run keep)`,
      );
      useProjectStore.getState().setStatus(`Open from My Tawala failed: ${msg}`);
      stripOpenQueryParams();
    }
    return true;
  }

  return false;
}
