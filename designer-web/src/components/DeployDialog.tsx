import { useEffect, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { DesignerDialog } from "./DesignerDialog";
import {
  LOCAL_WEBSITE_MOCK_MYTAWALA_URL,
  mockProjectIdFromName,
  websiteMockProjectDetailsUrl,
} from "@/lib/shellCommands";

export function DeployDialog() {
  const show = useProjectStore((s) => s.showDeployResult);
  const setShow = useProjectStore((s) => s.setShowDeployResult);
  const lastDeploy = useProjectStore((s) => s.lastDeploy);
  const [versionDescription, setVersionDescription] = useState("");

  useEffect(() => {
    if (show) setVersionDescription("");
  }, [show, lastDeploy]);

  if (!show || !lastDeploy) return null;

  const failed = lastDeploy.status === "failure";
  const close = () => {
    setVersionDescription("");
    setShow(false);
  };

  const openMyTawala = () => {
    if (failed) return;
    const name = lastDeploy.project ?? "Project";
    const id = mockProjectIdFromName(name);
    const note = versionDescription.trim();
    const receipt = {
      id,
      name,
      uniqueId: lastDeploy.uniqueId ?? null,
      startpoints: (lastDeploy.startpoints ?? []).map((sp) => ({
        form: sp.form,
        url: sp.url,
      })),
      mode: lastDeploy.mode ?? null,
      at: new Date().toISOString(),
      versionDescription: note,
    };
    // Project Details deep link + receipt → mock upserts My Tawala pile overlay
    // (mints monotonic versionNumber; history on Details Versions only).
    const url =
      websiteMockProjectDetailsUrl(id) +
      "&deployReceipt=" +
      encodeURIComponent(JSON.stringify(receipt));
    window.open(url, "_blank", "noopener,noreferrer");
  };

  return (
    <DesignerDialog
      title={failed ? "Deploy Failed" : "Project Deployed"}
      titleId="deploy-dialog-title"
      onClose={close}
      closeOnBackdrop
      className="designer-dialog-wide"
      footer={
        <>
          {!failed ? (
            <button
              type="button"
              onClick={openMyTawala}
              title="Add/update this project in mock My Tawala and open Project Details"
            >
              Show in My Tawala
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
            {lastDeploy.error ?? "Unknown deploy error."}
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
              <p className="hint">Deploy succeeded. Check server response for URLs.</p>
            )}
            <label className="deploy-version-note">
              <span>Version description (optional)</span>
              <textarea
                rows={2}
                value={versionDescription}
                onChange={(e) => setVersionDescription(e.target.value)}
                placeholder="What changed in this deploy?"
                maxLength={500}
              />
            </label>
            <p className="hint">
              <strong>Show in My Tawala</strong> opens Project Details on :5500, mints the next
              version number on the mock overlay, and records the note above. Listing stays flat —
              history is under Project Details → Versions. Mock must be running:{" "}
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
