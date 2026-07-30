import { useProjectStore } from "@/store/projectStore";
import { DesignerDialog } from "./DesignerDialog";
import { LOCAL_WEBSITE_MOCK_MYTAWALA_URL } from "@/lib/shellCommands";

export function DeployDialog() {
  const show = useProjectStore((s) => s.showDeployResult);
  const setShow = useProjectStore((s) => s.setShowDeployResult);
  const lastDeploy = useProjectStore((s) => s.lastDeploy);

  if (!show || !lastDeploy) return null;

  const failed = lastDeploy.status === "failure";
  const close = () => setShow(false);

  const openMyTawala = () => {
    if (failed) return;
    const receipt = {
      name: lastDeploy.project ?? "Project",
      uniqueId: lastDeploy.uniqueId ?? null,
      startpoints: (lastDeploy.startpoints ?? []).map((sp) => ({
        form: sp.form,
        url: sp.url,
      })),
      mode: lastDeploy.mode ?? null,
      at: new Date().toISOString(),
    };
    const url =
      LOCAL_WEBSITE_MOCK_MYTAWALA_URL +
      "?deployReceipt=" +
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
              title="Open website-mock My Tawala with this deploy receipt"
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
        ) : lastDeploy.startpoints && lastDeploy.startpoints.length > 0 ? (
          <>
            <p className="hint">
              Only forms marked <strong>Starting Point</strong> (Project Explorer flag) are listed
              here.
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
            <p className="hint">
              My Tawala pile is separate from this Deploy — use <strong>Show in My Tawala</strong> to
              drop a receipt into the mock inbox (:5500).
            </p>
          </>
        ) : (
          <p className="hint">Deploy succeeded. Check server response for URLs.</p>
        )}
      </div>
    </DesignerDialog>
  );
}
