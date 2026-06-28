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
  project?: string;
  uniqueId?: string;
  startpoints?: StartPoint[];
  error?: string;
  raw?: string;
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

export async function deployProject(
  project: unknown,
  credentials: DeployCredentials,
): Promise<DeployResult> {
  const res = await fetch("/api/deploy", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ credentials, project }),
  });
  const data = await res.json();
  if (!res.ok) {
    return { status: "failure", error: data.error ?? res.statusText };
  }
  return data as DeployResult;
}

export async function queryDeployments(credentials: DeployCredentials): Promise<StartPoint[]> {
  const xml = `<?xml version="1.0" encoding="utf-8"?>
<request type="queryDeployments" protocol="1.0">
  <credentials user="${escapeXml(credentials.user)}" password="${escapeXml(credentials.password)}"/>
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
