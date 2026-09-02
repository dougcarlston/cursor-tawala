export interface DeployCredentials {
  user: string;
  password: string;
}

export interface StartPoint {
  form: string;
  url: string;
}

export interface DeployResult {
  status: "success" | "failure";
  mode?: "dev" | "java";
  /** Designer display name (not necessarily the Tomcat project name). */
  project?: string;
  /** Tomcat / Node upload name — stable Redeploy key; minted on File→New first Push. */
  deployIdentityName?: string;
  uniqueId?: string;
  startpoints?: StartPoint[];
  error?: string;
  code?: string;
  occupantName?: string | null;
  occupantUniqueId?: string | null;
  raw?: string;
  /** True when this Push came from File→New / template. */
  freshFromTemplate?: boolean;
  /** True when a new non-colliding Tomcat name was minted on this Push. */
  identityMinted?: boolean;
}

const CREDS_KEY = "tawala.designer.credentials";

export function loadCredentials(): DeployCredentials | null {
  try {
    const raw = localStorage.getItem(CREDS_KEY);
    return raw ? (JSON.parse(raw) as DeployCredentials) : null;
  } catch {
    return null;
  }
}

export function saveCredentials(credentials: DeployCredentials) {
  localStorage.setItem(CREDS_KEY, JSON.stringify(credentials));
}

import { getGlobalAuthUser } from "@/lib/clerkAuth";

export async function deployProject(
  project: unknown,
  credentials: DeployCredentials,
): Promise<DeployResult> {
  const authUser = getGlobalAuthUser();
  const effectiveCredentials = authUser
    ? {
        user: authUser.primaryEmail || authUser.username || authUser.id,
        password: credentials.password || "clerk-auth",
        authProvider: "clerk",
        authorId: authUser.id,
        authorDisplayName: authUser.fullName || authUser.username || authUser.primaryEmail,
      }
    : credentials;

  const res = await fetch("/api/deploy", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ credentials: effectiveCredentials, project }),
  });
  const data = await res.json();
  if (!res.ok) {
    return {
      status: "failure",
      error: data.error ?? res.statusText,
      code: data.code,
      occupantName: data.occupantName ?? null,
      occupantUniqueId: data.occupantUniqueId ?? null,
    };
  }
  return data as DeployResult;
}

export async function queryDeployments(credentials: DeployCredentials): Promise<StartPoint[]> {
  const authUser = getGlobalAuthUser();
  const user = authUser?.primaryEmail || authUser?.username || authUser?.id || credentials.user;
  const password = credentials.password || "clerk-auth";
  const authProviderAttr = authUser ? ` authProvider="clerk"` : "";
  const xml = `<?xml version="1.0" encoding="utf-8"?>
<request type="queryDeployments" protocol="1.0">
  <credentials user="${escapeXml(user)}" password="${escapeXml(password)}"${authProviderAttr}/>
</request>`;
  const res = await fetch("/client", {
    method: "POST",
    headers: { "Content-Type": "text/xml; charset=utf-8" },
    body: xml,
  });
  const text = await res.text();
  const points: StartPoint[] = [];
  const re = /<startpoint form="([^"]+)" url="([^"]+)"/g;
  let m;
  while ((m = re.exec(text))) {
    points.push({ form: m[1], url: m[2] });
  }
  return points;
}

function escapeXml(s: string) {
  return s.replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/</g, "&lt;");
}
