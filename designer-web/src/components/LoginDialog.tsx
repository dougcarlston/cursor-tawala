import { useState } from "react";
import { useProjectStore } from "@/store/projectStore";

export function LoginDialog() {
  const show = useProjectStore((s) => s.showLogin);
  const setShow = useProjectStore((s) => s.setShowLogin);
  const setCredentials = useProjectStore((s) => s.setCredentials);
  const deploy = useProjectStore((s) => s.deploy);
  const [user, setUser] = useState("dev");
  const [password, setPassword] = useState("dev");

  if (!show) return null;

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    setCredentials({ user, password });
    void deploy();
  };

  return (
    <div className="modal-backdrop" role="presentation">
      <div className="modal" role="dialog" aria-modal="true" aria-labelledby="login-dialog-title">
        <h2 id="login-dialog-title">Designer Login</h2>
        <p className="hint">
          Credentials for deploy (not DirtBowl participant login). Dev server accepts{" "}
          <code>dev/dev</code>.
        </p>
        <form onSubmit={submit}>
          <label>
            User ID
            <input value={user} onChange={(e) => setUser(e.target.value)} autoFocus />
          </label>
          <label>
            Password
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
            />
          </label>
          <div className="modal-actions">
            <button type="button" onClick={() => setShow(false)}>
              Cancel
            </button>
            <button type="submit">Login &amp; Deploy</button>
          </div>
        </form>
      </div>
    </div>
  );
}
