import { useProjectStore } from "@/store/projectStore";

export function DeployDialog() {
  const show = useProjectStore((s) => s.showDeployResult);
  const setShow = useProjectStore((s) => s.setShowDeployResult);
  const lastDeploy = useProjectStore((s) => s.lastDeploy);

  if (!show || !lastDeploy || lastDeploy.status !== "success") return null;

  return (
    <div className="modal-backdrop" role="presentation">
      <div className="modal modal-wide" role="dialog" aria-modal="true" aria-labelledby="deploy-dialog-title">
        <h2 id="deploy-dialog-title">Project Deployed</h2>
        <p>
          <strong>{lastDeploy.project}</strong>
          {lastDeploy.mode === "java" ? " → Java backend" : " → dev runtime"}
        </p>
        {lastDeploy.startpoints && lastDeploy.startpoints.length > 0 ? (
          <>
            <p className="hint">
              Only forms marked <strong>Starting Point</strong> in form Properties are listed
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
          </>
        ) : (
          <p className="hint">Deploy succeeded. Check server response for URLs.</p>
        )}
        <div className="modal-actions">
          <button type="button" onClick={() => setShow(false)}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
