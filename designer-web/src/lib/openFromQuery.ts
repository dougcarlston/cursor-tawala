/**
 * Deep-link open for My Tawala → Edit project in Designer.
 *
 * Supported query params (first match wins):
 *   ?snapshot=<id>     — GET /api/version-snapshots/:id (Show in My Tawala / redeploy cache)
 *   ?mockJson=<path>   — GET /api/open-mock-json?path=… (website-mock/projects/{mytawala|library}/*.json)
 *
 * Clears the query string after a successful load so refresh does not re-import.
 */
import { isPublicDesigner } from "@/api/accountPush";
import { clearProjectFileHandle } from "@/lib/shellCommands";
import { convertTawalaXmlToProject } from "@/lib/tawalaXmlToJson.mjs";
import { useProjectStore } from "@/store/projectStore";

const MOCK_JSON_RE =
  /^(projects\/(mytawala|library)\/[^/]+\.json|designer-web\/public\/samples\/.+\.json)$/i;
const SNAPSHOT_RE = /^[a-f0-9]{16,64}$/i;

function stripOpenQueryParams(): void {
  try {
    const url = new URL(window.location.href);
    if (
      !url.searchParams.has("snapshot") &&
      !url.searchParams.has("mockJson") &&
      !url.searchParams.has("accountVersion")
    ) {
      return;
    }
    url.searchParams.delete("snapshot");
    url.searchParams.delete("mockJson");
    url.searchParams.delete("accountVersion");
    url.searchParams.delete("accountName");
    url.searchParams.delete("accountId");
    const qs = url.searchParams.toString();
    window.history.replaceState({}, "", url.pathname + (qs ? `?${qs}` : "") + url.hash);
  } catch {
    /* ignore */
  }
}

function archiveNeedsFreshPush(project: unknown): boolean {
  if (!project || typeof project !== "object") return false;
  const p = project as {
    deployUniqueId?: string;
    deployIdentityName?: string;
    uniqueId?: string;
  };
  return (
    !String(p.deployUniqueId || p.uniqueId || "").trim() &&
    !String(p.deployIdentityName || "").trim()
  );
}

async function loadProjectObject(
  project: unknown,
  label: string,
  opts?: { freshFromArchive?: boolean },
): Promise<void> {
  if (!project || typeof project !== "object" || !("name" in project)) {
    throw new Error(`Invalid project from ${label}`);
  }
  clearProjectFileHandle();
  useProjectStore.getState().importJson(JSON.stringify(project));
  if (opts?.freshFromArchive) {
    const current = useProjectStore.getState().project;
    useProjectStore.setState({
      project: { ...current, _freshFromTemplate: true },
      statusMessage: `Opened ${String((project as { name: string }).name)} — first Push will mint a private live id`,
    });
    return;
  }
  useProjectStore.getState().setStatus(`Opened ${String((project as { name: string }).name)} from My Tawala`);
}

/**
 * If the URL asks to open a project, fetch and load it. Returns true when a load was attempted.
 */
export async function tryOpenProjectFromQuery(): Promise<boolean> {
  const params = new URLSearchParams(window.location.search);
  const accountVersion = (params.get("accountVersion") || "").trim();
  if (accountVersion) {
    if (!isPublicDesigner()) {
      useProjectStore.getState().setStatus("Open this project from Designer on the public site.");
      stripOpenQueryParams();
      return true;
    }
    const accountName = (params.get("accountName") || "").trim();
    const accountId = (params.get("accountId") || "").trim();
    if (!/^\d+$/.test(accountVersion) || !accountName || !/^[A-Za-z0-9]{1,20}$/.test(accountId)) {
      alert("Couldn’t open this account project. Go back to Project Details and use Edit project in Designer.");
      stripOpenQueryParams();
      return true;
    }
    try {
      const res = await fetch(
        "/projectmanager/downloadversion?project_version=" + encodeURIComponent(accountVersion),
        { credentials: "same-origin", cache: "no-store", redirect: "follow" },
      );
      let landed = "";
      try {
        landed = new URL(res.url, location.origin).pathname;
      } catch {
        landed = "";
      }
      if (/\/login$/.test(landed)) {
        location.assign("/login?r=" + encodeURIComponent(location.pathname + location.search));
        return true;
      }
      const xml = await res.text();
      if (!res.ok || xml.indexOf("<") === -1) {
        throw new Error("The live version did not come back.");
      }
      const converted = convertTawalaXmlToProject(xml, { sourceLabel: accountName + ".tawala" });
      const project = converted.project as {
        name?: string;
        deployIdentityName?: string;
        deployUniqueId?: string;
        uniqueId?: string;
        accountPushUniqueId?: string;
      };
      project.name = accountName;
      project.deployIdentityName = accountName;
      project.deployUniqueId = accountId;
      project.uniqueId = accountId;
      project.accountPushUniqueId = accountId;
      clearProjectFileHandle();
      useProjectStore.getState().importJson(JSON.stringify(project));
      const notes = converted.warnings.length
        ? " Opened with " + converted.warnings.length + " conversion notes."
        : "";
      useProjectStore
        .getState()
        .setStatus("Opened “" + accountName + "” from My Tawala. A later Push updates this project." + notes);
      stripOpenQueryParams();
    } catch (err) {
      const msg = err instanceof Error ? err.message : String(err);
      alert("Couldn’t open this project in Designer.\n\n" + msg);
      useProjectStore.getState().setStatus("Open from My Tawala failed: " + msg);
      stripOpenQueryParams();
    }
    return true;
  }

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
      const isLibraryArchive = /^projects\/library\//i.test(mockJson);
      await loadProjectObject(data.project, "mockJson", {
        freshFromArchive: isLibraryArchive && archiveNeedsFreshPush(data.project),
      });
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
