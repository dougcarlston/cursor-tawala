import { useProjectStore } from "@/store/projectStore";

export function DeployDialog() {
  const show = useProjectStore((s) => s.showDeployResult);
  const setShow = useProjectStore((s) => s.setShowDeployResult);
  const lastDeploy = useProjectStore((s) => s.lastDeploy);

  if (!show || !lastDeploy || lastDeploy.status !== "success") return null;

  return (
    <div className="modal-backdrop" onClick={() => setShow(false)}>
      <div className="modal modal-wide" onClick={(e) => e.stopPropagation()}>
        <h2>Project Deployed</h2>
        <p>
          <strong>{lastDeploy.project}</strong>
          {lastDeploy.mode === "java" ? " → Java backend" : " → dev runtime"}
        </p>
        {lastDeploy.startpoints && lastDeploy.startpoints.length > 0 ? (
          <>
            <p className="hint">Open a start point to test the live form:</p>
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
