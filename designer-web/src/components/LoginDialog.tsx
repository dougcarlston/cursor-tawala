import { useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { DesignerDialog } from "./DesignerDialog";

export function LoginDialog() {
  const show = useProjectStore((s) => s.showLogin);
  const setShow = useProjectStore((s) => s.setShowLogin);
  const setCredentials = useProjectStore((s) => s.setCredentials);
  const deploy = useProjectStore((s) => s.deploy);
  const [user, setUser] = useState("dev");
  const [password, setPassword] = useState("dev");

  if (!show) return null;

  const close = () => setShow(false);

  const submit = (e: React.FormEvent) => {
    e.preventDefault();
    setCredentials({ user, password });
    void deploy();
  };

  return (
    <DesignerDialog
      title="Designer Login"
      titleId="login-dialog-title"
      onClose={close}
      closeOnBackdrop
      footer={
        <>
          <button type="button" onClick={close}>
            Cancel
          </button>
          <button type="submit" form="login-dialog-form">
            Login &amp; Deploy
          </button>
        </>
      }
    >
      <form id="login-dialog-form" className="designer-dialog-panel" onSubmit={submit}>
        <p className="hint">
          Credentials for deploy (not DirtBowl participant login). Dev server accepts{" "}
          <code>dev/dev</code>.
        </p>
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
      </form>
    </DesignerDialog>
  );
}
