/** Browser Designer client for server-owned outbound email status / test send. */

import { loadCredentials, type DeployCredentials } from "./deploy";

export interface EmailDeliveryStatus {
  available: boolean;
  mode: "java" | "dev" | "offline";
  enabled: boolean;
  configured: boolean;
  host: string;
  port: number;
  auth: boolean;
  starttls: boolean;
  fromAddress: string;
  fromName: string;
  workerEnabled: boolean;
  readyCount: number;
  sendingCount: number;
  sentCount: number;
  errorCount: number;
  lastError: string;
  lastErrorAt: number;
  lastSuccessAt: number;
  lastWorkerRunAt: number;
  message?: string;
  error?: string;
}

function escapeXml(s: string) {
  return s.replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/</g, "&lt;");
}

function attr(xml: string, name: string): string {
  const re = new RegExp(`\\b${name}="([^"]*)"`);
  const m = xml.match(re);
  return m ? m[1] : "";
}

function parseEmailStatusXml(text: string, mode: EmailDeliveryStatus["mode"]): EmailDeliveryStatus {
  const failure = text.match(/<error[^>]*message="([^"]*)"/);
  if (text.includes('status="failure"') || failure) {
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

export async function fetchEmailDeliveryStatus(
  credentials?: DeployCredentials | null,
): Promise<EmailDeliveryStatus> {
  const creds = credentials ?? loadCredentials();
  const res = await fetch("/api/email/status", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ credentials: creds }),
  });
  const data = await res.json();
  if (!res.ok) {
    return {
      available: false,
      mode: data.mode ?? "offline",
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
      error: data.error ?? res.statusText,
    };
  }
  return data as EmailDeliveryStatus;
}

export async function sendTestEmailDelivery(
  to: string,
  credentials?: DeployCredentials | null,
): Promise<EmailDeliveryStatus> {
  const creds = credentials ?? loadCredentials();
  const res = await fetch("/api/email/test", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ credentials: creds, to }),
  });
  const data = await res.json();
  if (!res.ok) {
    const fallback = await fetchEmailDeliveryStatus(creds).catch(() => null);
    return {
      available: false,
      mode: (data.mode ?? fallback?.mode ?? "offline") as EmailDeliveryStatus["mode"],
      enabled: fallback?.enabled ?? false,
      configured: fallback?.configured ?? false,
      host: fallback?.host ?? "",
      port: fallback?.port ?? 0,
      auth: fallback?.auth ?? false,
      starttls: fallback?.starttls ?? false,
      fromAddress: fallback?.fromAddress ?? "",
      fromName: fallback?.fromName ?? "",
      workerEnabled: fallback?.workerEnabled ?? false,
      readyCount: fallback?.readyCount ?? 0,
      sendingCount: fallback?.sendingCount ?? 0,
      sentCount: fallback?.sentCount ?? 0,
      errorCount: fallback?.errorCount ?? 0,
      lastError: fallback?.lastError ?? "",
      lastErrorAt: fallback?.lastErrorAt ?? 0,
      lastSuccessAt: fallback?.lastSuccessAt ?? 0,
      lastWorkerRunAt: fallback?.lastWorkerRunAt ?? 0,
      error: data.error ?? res.statusText,
    };
  }
  return data as EmailDeliveryStatus;
}

/** Used by Express when proxying Java XML responses. */
export function parseJavaEmailStatus(xml: string, mode: EmailDeliveryStatus["mode"]): EmailDeliveryStatus {
  return parseEmailStatusXml(xml, mode);
}

export function buildEmailStatusXmlRequest(credentials: DeployCredentials): string {
  return `<?xml version="1.0" encoding="utf-8"?>
<request type="queryEmailStatus" protocol="1.0">
  <credentials user="${escapeXml(credentials.user)}" password="${escapeXml(credentials.password)}"/>
</request>`;
}

export function buildEmailTestXmlRequest(credentials: DeployCredentials, to: string): string {
  return `<?xml version="1.0" encoding="utf-8"?>
<request type="sendTestEmail" protocol="1.0">
  <credentials user="${escapeXml(credentials.user)}" password="${escapeXml(credentials.password)}"/>
  <testEmail to="${escapeXml(to)}"/>
</request>`;
}
