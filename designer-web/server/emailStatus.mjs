/** Parse Java Client API emailStatus XML into a JSON object (no secrets). */

function attr(block, name) {
  const re = new RegExp(`\\b${name}="([^"]*)"`);
  const m = block.match(re);
  return m ? m[1] : "";
}

export function parseEmailStatusXml(text, mode = "java") {
  const failure = text.match(/<error[^>]*message="([^"]*)"/);
  if (text.includes('status="failure"') || (failure && !text.includes("<emailStatus"))) {
    return {
      available: false,
      mode,
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
      error: failure?.[1] ?? "Email status request failed",
    };
  }
  const blockMatch = text.match(/<emailStatus\b[^>]*/);
  const block = blockMatch?.[0] ?? text;
  return {
    available: true,
    mode,
    enabled: attr(block, "enabled") === "true",
    configured: attr(block, "configured") === "true",
    host: attr(block, "host"),
    port: Number(attr(block, "port") || 0),
    auth: attr(block, "auth") === "true",
    starttls: attr(block, "starttls") === "true",
    fromAddress: attr(block, "fromAddress"),
    fromName: attr(block, "fromName"),
    workerEnabled: attr(block, "workerEnabled") === "true",
    readyCount: Number(attr(block, "readyCount") || 0),
    sendingCount: Number(attr(block, "sendingCount") || 0),
    sentCount: Number(attr(block, "sentCount") || 0),
    errorCount: Number(attr(block, "errorCount") || 0),
    lastError: attr(block, "lastError"),
    lastErrorAt: Number(attr(block, "lastErrorAt") || 0),
    lastSuccessAt: Number(attr(block, "lastSuccessAt") || 0),
    lastWorkerRunAt: Number(attr(block, "lastWorkerRunAt") || 0),
    message: attr(block, "message") || undefined,
  };
}

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;");
}

export function buildQueryEmailStatusXml(credentials) {
  return `<?xml version="1.0" encoding="utf-8"?>
<request type="queryEmailStatus" protocol="1.0">
  <credentials user="${esc(credentials.user)}" password="${esc(credentials.password)}"/>
</request>`;
}

export function buildSendTestEmailXml(credentials, to) {
  return `<?xml version="1.0" encoding="utf-8"?>
<request type="sendTestEmail" protocol="1.0">
  <credentials user="${esc(credentials.user)}" password="${esc(credentials.password)}"/>
  <testEmail to="${esc(to)}"/>
</request>`;
}
