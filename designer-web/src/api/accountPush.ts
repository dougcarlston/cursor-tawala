import { buildUploadRequest } from "../../server/jsonToXml.mjs";
import type { DeployResult } from "./deploy";
import type { TawalaProject } from "@/types/tawala";

/** Designer served by Tomcat at /app/designer/ — not the laptop dev server. */
export function isPublicDesigner(): boolean {
  if (typeof location === "undefined") return false;
  if (location.port === "5173") return false;
  return location.pathname.indexOf("/app/designer") === 0;
}

type AccountProject = TawalaProject & {
  accountPushUniqueId?: string;
  uniqueId?: string;
};

function xmlAttr(xml: string, name: string): string {
  const match = new RegExp(name + '="([^"]*)"').exec(xml);
  if (!match) return "";
  return match[1]
    .replace(/&quot;/g, '"')
    .replace(/&lt;/g, "<")
    .replace(/&amp;/g, "&");
}

function candidateName(base: string, n: number): string {
  const name = n <= 1 ? base : base + " " + n;
  return name.length > 100 ? name.slice(0, 100).trim() : name;
}

function projectForUpload(project: AccountProject, accountName: string, uniqueId: string): AccountProject {
  const next: AccountProject = {
    ...project,
    name: accountName,
    deployIdentityName: accountName,
  };
  if (uniqueId) {
    next.deployUniqueId = uniqueId;
    next.accountPushUniqueId = uniqueId;
  } else {
    delete next.deployUniqueId;
    delete next.uniqueId;
    delete next.accountPushUniqueId;
  }
  return next;
}

/**
 * Push the open project onto the signed-in account. A public Library name is
 * saved as "Name 2" so the live Library copy is left alone. A later Push of
 * the same Designer file updates that account project.
 */
export async function deploySignedInProject(project: TawalaProject): Promise<DeployResult> {
  const source = project as AccountProject;
  const knownId = String(source.accountPushUniqueId || "").trim();
  const baseName = String(
    knownId ? source.deployIdentityName || source.name : source.name,
  ).trim();
  if (!baseName) {
    return { status: "failure", mode: "account", error: "The project needs a name." };
  }

  let lastCode = "";
  const tries = knownId ? 1 : 20;
  for (let n = 1; n <= tries; n += 1) {
    const accountName = knownId ? baseName : candidateName(baseName, n);
    const body = buildUploadRequest(
      { user: "session", password: "session" },
      projectForUpload(source, accountName, knownId),
    );
    let res: Response;
    try {
      res = await fetch("/projectmanager/upload-project", {
        method: "POST",
        credentials: "same-origin",
        cache: "no-store",
        redirect: "follow",
        headers: { "Content-Type": "text/xml; charset=utf-8" },
        body,
      });
    } catch (e) {
      return {
        status: "failure",
        mode: "account",
        error: e instanceof Error ? e.message : String(e),
      };
    }
    let landed = "";
    try {
      landed = new URL(res.url, location.origin).pathname;
    } catch {
      landed = "";
    }
    if (/\/login$/.test(landed) || res.status === 401) {
      location.assign("/login?r=" + encodeURIComponent("/app/designer/"));
      return { status: "failure", mode: "account", error: "Sign in is required." };
    }
    const xml = await res.text();
    if (res.ok && /status="success"/.test(xml)) {
      const uniqueId = xmlAttr(xml, "uniqueId");
      const savedName = xmlAttr(xml, "name") || accountName;
      if (!uniqueId) {
        return { status: "failure", mode: "account", error: "The project was not saved." };
      }
      return {
        status: "success",
        mode: "account",
        project: source.name,
        deployIdentityName: savedName,
        uniqueId,
        identityMinted: savedName !== source.name,
      };
    }
    lastCode = xmlAttr(xml, "code");
    if (knownId || (lastCode !== "name-taken" && lastCode !== "library-project")) {
      break;
    }
  }

  const error =
    lastCode === "library-project" || lastCode === "name-taken"
      ? "This account already has a project with that name. Choose another."
      : lastCode === "uniqueId"
        ? "That address is already in use."
        : "The project was not saved.";
  return { status: "failure", mode: "account", code: lastCode, error };
}
