import { useEffect, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { DesignerDialog } from "./DesignerDialog";
import {
  LOCAL_WEBSITE_MOCK_MYTAWALA_URL,
  mockProjectIdFromName,
  websiteMockProjectDetailsUrl,
} from "@/lib/shellCommands";

/** Strip Designer-only flags before snapshot / receipt. */
function projectForSnapshot(project: Record<string, unknown> | null | undefined) {
  if (!project || typeof project !== "object") return null;
  const { _freshFromTemplate: _drop, ...rest } = project;
  return rest;
}

export function DeployDialog() {
  const show = useProjectStore((s) => s.showDeployResult);
  const setShow = useProjectStore((s) => s.setShowDeployResult);
  const lastDeploy = useProjectStore((s) => s.lastDeploy);
  const project = useProjectStore((s) => s.project);
  const [versionDescription, setVersionDescription] = useState("");
  const [busy, setBusy] = useState(false);

  useEffect(() => {
    if (show) {
      setVersionDescription("");
      setBusy(false);
    }
  }, [show, lastDeploy]);

  if (!show || !lastDeploy) return null;

  const failed = lastDeploy.status === "failure";
  const close = () => {
    setVersionDescription("");
    setBusy(false);
    setShow(false);
  };

  const openMyTawala = async () => {
    if (failed || busy) return;
    const name = lastDeploy.project ?? "Project";
    const id = mockProjectIdFromName(name);
    const note = versionDescription.trim();
    let snapshotId: string | null = null;
    const snapProject = projectForSnapshot(project as Record<string, unknown>);

    setBusy(true);
    try {
      if (snapProject) {
        const res = await fetch("/api/version-snapshots", {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({
            project: snapProject,
            uniqueId: lastDeploy.uniqueId ?? null,
            projectId: id,
            versionDescription: note,
            at: new Date().toISOString(),
          }),
        });
        const data = (await res.json().catch(() => ({}))) as {
          status?: string;
          snapshotId?: string;
          error?: string;
        };
        if (res.ok && data.snapshotId) {
          snapshotId = data.snapshotId;
        } else {
          window.alert(
            "Couldn’t save a definition snapshot for this version.\n\n" +
              (data.error || `HTTP ${res.status}`) +
              "\n\nMy Tawala will still record version metadata, but “Push this version” " +
              "won’t be able to re-push this row until a later Push → Show in My Tawala succeeds with :3001 up.",
          );
        }
      }
    } catch (e) {
      window.alert(
        "Couldn’t reach the Designer API to save a definition snapshot.\n\n" +
          String((e as Error)?.message || e) +
          "\n\nIs designer-web on :3001? My Tawala will still open with metadata only.",
      );
    } finally {
      setBusy(false);
    }

    const currentAuthor =
      (project as Record<string, unknown>)?.author ||
      (project as Record<string, unknown>)?.authorId ||
      (project as Record<string, unknown>)?.userId ||
      undefined;

    const receipt = {
      id,
      name,
      uniqueId: lastDeploy.uniqueId ?? null,
      deployUniqueId: lastDeploy.uniqueId ?? null,
      deployIdentityName: lastDeploy.deployIdentityName ?? undefined,
      startpoints: (lastDeploy.startpoints ?? []).map((sp) => ({
        form: sp.form,
        url: sp.url,
      })),
      mode: lastDeploy.mode ?? null,
      at: new Date().toISOString(),
      versionDescription: note,
      snapshotId,
      themePath: String(project?.themePath || "default").trim() || "default",
      ...(currentAuthor ? { author: String(currentAuthor) } : {}),
    };
    // Project Details deep link + receipt → mock upserts My Tawala pile overlay
    // (mints monotonic versionNumber; history on Details Versions only).
    // Definition body stays on :3001 (snapshotId) — URL cannot carry full project JSON.
    const url =
      websiteMockProjectDetailsUrl(id) +
      "&deployReceipt=" +
      encodeURIComponent(JSON.stringify(receipt));
    window.open(url, "_blank", "noopener,noreferrer");
  };

  return (
    <DesignerDialog
      title={failed ? "Push Failed" : "Project pushed"}
      titleId="deploy-dialog-title"
      onClose={close}
      closeOnBackdrop
      className="designer-dialog-wide"
      footer={
        <>
          {!failed ? (
            <button
              type="button"
              onClick={() => void openMyTawala()}
              disabled={busy}
              title="Add/update this project in mock My Tawala and open Project Details"
            >
              {busy ? "Saving snapshot…" : "Show in My Tawala"}
            </button>
          ) : null}
          <button type="button" onClick={close}>
            Close
          </button>
        </>
      }
    >
      <div className="designer-dialog-panel">
        <p>
          <strong>{lastDeploy.project ?? "Project"}</strong>
          {lastDeploy.mode === "java"
            ? " → Java backend (:8080)"
            : lastDeploy.mode === "dev"
              ? " → dev runtime"
              : null}
        </p>
        {failed ? (
          <p className="hint" role="alert">
            {lastDeploy.error ?? "Unknown Push error."}
          </p>
        ) : (
          <>
            {lastDeploy.startpoints && lastDeploy.startpoints.length > 0 ? (
              <>
                <p className="hint">
                  Only forms marked <strong>Starting Point</strong> (Project Explorer flag) are
                  listed here.
                </p>
                <ul className="deploy-urls">
                  {lastDeploy.startpoints.map((sp) => (
                    <li key={sp.form}>
                      <span>{sp.form}</span>
                      <a href={sp.url} target="_blank" rel="noreferrer">
                        {sp.url}
                      </a>
                    </li>
                  ))}
                </ul>
              </>
            ) : (
              <p className="hint">Push succeeded. Check server response for URLs.</p>
            )}
            <label className="deploy-version-note">
              <span>Version description (optional)</span>
              <textarea
                rows={2}
                value={versionDescription}
                onChange={(e) => setVersionDescription(e.target.value)}
                placeholder="What changed in this push?"
                maxLength={500}
              />
            </label>
            <p className="hint">
              <strong>Show in My Tawala</strong> opens Project Details on :5500, mints the next
              version number on the mock overlay, saves a definition snapshot (for later{" "}
              <strong>Push this version</strong>), and records the note above. Listing stays
              flat — history is under Project Details → Versions. Mock must be running:{" "}
              <code>cd website-mock && ./serve.sh</code>. Listing:{" "}
              <a href={LOCAL_WEBSITE_MOCK_MYTAWALA_URL} target="_blank" rel="noreferrer">
                My Tawala
              </a>
              .
            </p>
          </>
        )}
      </div>
    </DesignerDialog>
  );
}
