import { useEffect, useState } from "react";
import {
  fetchEmailDeliveryStatus,
  sendTestEmailDelivery,
  type EmailDeliveryStatus,
} from "@/api/email";
import { loadCredentials } from "@/api/deploy";
import { clearEmailDeliveryDialog } from "@/lib/emailDelivery";

/** Project → Email Delivery… — server-owned SMTP status + test send (no secrets in UI). */
export function EmailDeliveryDialog() {
  const [status, setStatus] = useState<EmailDeliveryStatus | null>(null);
  const [loading, setLoading] = useState(true);
  const [testTo, setTestTo] = useState("");
  const [busy, setBusy] = useState(false);
  const [flash, setFlash] = useState<string | null>(null);

  const refresh = async () => {
    setLoading(true);
    setFlash(null);
    try {
      const next = await fetchEmailDeliveryStatus(loadCredentials());
      setStatus(next);
    } catch (e) {
      setStatus({
        available: false,
        mode: "offline",
        enabled: false,
        configured: false,
        host: "",
        port: 0,
        auth: false,
        starttls: false,
        fromAddress: "",
        fromName: "",
        workerEnabled: false,
        readyCount: 0,
        sendingCount: 0,
        sentCount: 0,
        errorCount: 0,
        lastError: "",
        lastErrorAt: 0,
        lastSuccessAt: 0,
        lastWorkerRunAt: 0,
        error: e instanceof Error ? e.message : String(e),
      });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    void refresh();
  }, []);

  const onTest = async () => {
    setBusy(true);
    setFlash(null);
    try {
      const next = await sendTestEmailDelivery(testTo.trim(), loadCredentials());
      setStatus(next);
      if (next.error) setFlash(next.error);
      else setFlash(next.message ?? "Test email sent.");
    } catch (e) {
      setFlash(e instanceof Error ? e.message : String(e));
    } finally {
      setBusy(false);
    }
  };

  return (
    <div className="modal-backdrop" role="presentation" onClick={(e) => {
      if (e.target === e.currentTarget) clearEmailDeliveryDialog();
    }}>
      <div
        className="modal modal-wide email-delivery-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby="email-delivery-title"
      >
        <h2 id="email-delivery-title">Email Delivery</h2>
        <p className="hint">
          Outbound email uses one <strong>server-owned</strong> SMTP account (Brevo, Postmark,
          Mailpit, or any relay). Provider credentials stay on the Tomcat host — they are never
          stored in the project JSON. Process <strong>From</strong> becomes Reply-To; the
          verified server address is the visible From.
        </p>

        {loading && !status ? (
          <p className="hint">Loading status…</p>
        ) : status ? (
          <div className="email-delivery-status">
            <dl className="email-delivery-dl">
              <div>
                <dt>Runtime</dt>
                <dd>{status.mode === "java" ? "Java / Tomcat (:8080)" : status.mode === "dev" ? "Dev (no Java mail)" : "Offline"}</dd>
              </div>
              <div>
                <dt>Configured</dt>
                <dd>{status.configured ? "Yes" : "No"}{status.enabled ? "" : " (disabled)"}</dd>
              </div>
              <div>
                <dt>SMTP host</dt>
                <dd>{status.host || "—"}{status.port ? `:${status.port}` : ""}</dd>
              </div>
              <div>
                <dt>TLS / Auth</dt>
                <dd>
                  STARTTLS {status.starttls ? "on" : "off"} · Auth {status.auth ? "on" : "off"}
                </dd>
              </div>
              <div>
                <dt>Verified From</dt>
                <dd>
                  {status.fromName ? `${status.fromName} <${status.fromAddress}>` : status.fromAddress || "—"}
                </dd>
              </div>
              <div>
                <dt>Queue</dt>
                <dd>
                  ready {status.readyCount} · sending {status.sendingCount} · sent{" "}
                  {status.sentCount} · error {status.errorCount}
                </dd>
              </div>
              {status.lastError ? (
                <div>
                  <dt>Last error</dt>
                  <dd className="email-delivery-error">{status.lastError}</dd>
                </div>
              ) : null}
            </dl>
            {status.error ? (
              <p className="hint" role="alert">
                {status.error}
              </p>
            ) : null}
            {status.mode !== "java" ? (
              <p className="hint">
                Start Tomcat on :8080 with Mailpit (or a real SMTP relay) to send mail. Preview
                never sends email.
              </p>
            ) : null}
          </div>
        ) : null}

        <div className="email-delivery-test">
          <label htmlFor="email-delivery-test-to">
            Test recipient
            <input
              id="email-delivery-test-to"
              type="email"
              value={testTo}
              onChange={(e) => setTestTo(e.target.value)}
              placeholder="you@example.com"
              disabled={busy || status?.mode !== "java" || !status?.configured}
            />
          </label>
          <button
            type="button"
            disabled={
              busy ||
              status?.mode !== "java" ||
              !status?.configured ||
              !testTo.trim().includes("@")
            }
            onClick={() => void onTest()}
          >
            {busy ? "Sending…" : "Send Test"}
          </button>
          <button type="button" disabled={loading || busy} onClick={() => void refresh()}>
            Refresh
          </button>
        </div>
        {flash ? (
          <p className="hint" role="status">
            {flash}
          </p>
        ) : null}

        <div className="modal-actions">
          <button type="button" onClick={() => clearEmailDeliveryDialog()}>
            Close
          </button>
        </div>
      </div>
    </div>
  );
}
