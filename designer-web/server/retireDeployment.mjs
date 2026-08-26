/**
 * Vacate a live Tomcat / Node Deploy *name* so occupancy no longer blocks it.
 * uniqueId stays on the same row (no mint, no catalog retarget).
 *
 * Java `put` reuses UserProject by (user, name). Occupancy looks up that name via
 * queryDeployments. Retire therefore renames the occupant to
 * `{name} (retired {uniqueId})` through Hibernate (`retireDeployment` /client)
 * or the Node `_index.json` slot — never by display-name lookup (Online Exam
 * catalog uniqueId is not the sibling that occupies the bare title).
 */
import * as store from "./store.mjs";

export const TOMCAT_NAME_MAX = 100;
const UNIQUE_ID_RE = /^[A-Za-z0-9]{1,20}$/;

export function isValidUniqueId(uniqueId) {
  return typeof uniqueId === "string" && UNIQUE_ID_RE.test(uniqueId);
}

export function vacatedTomcatName(name, uniqueId) {
  const uid = String(uniqueId || "").trim();
  const suffix = ` (retired ${uid})`;
  let base = String(name || "").trim();
  if (!uid) return base;
  if (base.endsWith(suffix) || base.includes(suffix)) return base;
  if (base.length + suffix.length > TOMCAT_NAME_MAX) {
    base = base.slice(0, Math.max(0, TOMCAT_NAME_MAX - suffix.length));
  }
  return `${base}${suffix}`;
}

export function isVacatedTomcatName(name, uniqueId) {
  const uid = String(uniqueId || "").trim();
  const n = String(name || "");
  if (!uid) return / \(retired [A-Za-z0-9]{1,20}\)$/.test(n);
  return n.includes(` (retired ${uid})`);
}

function xmlEsc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;");
}

export function retireDeploymentRequestXml(user, password, uniqueId) {
  return (
    `<?xml version="1.0" encoding="utf-8"?>\n` +
    `<request type="retireDeployment" protocol="1.0">\n` +
    `  <credentials user="${xmlEsc(user)}" password="${xmlEsc(password)}"/>\n` +
    `  <deployment uniqueId="${xmlEsc(uniqueId)}"/>\n` +
    `</request>`
  );
}

/**
 * @returns {{
 *   status: "success" | "failure",
 *   uniqueId?: string,
 *   previousName?: string,
 *   name?: string,
 *   alreadyVacated?: boolean,
 *   code?: string,
 *   error?: string,
 * }}
 */
export function parseRetireDeploymentXml(xml) {
  const text = String(xml || "");
  const err = text.match(/<error id="([^"]*)" message="([^"]*)"/);
  if (err) {
    return {
      status: "failure",
      code: err[1],
      error: err[2],
    };
  }
  if (/command\.unknown/i.test(text) && /retireDeployment/i.test(text)) {
    return {
      status: "failure",
      code: "command.unknown",
      error:
        "Tomcat /client has no retireDeployment command yet. Rebuild ROOT.war from TawalaWebapp-build1700 and recreate the tawala container so Retire can free the :8080 name.",
    };
  }
  const hit = text.match(
    /<retired\b([^>]*)\/?>/i,
  );
  if (!hit || !/status="success"/i.test(text)) {
    return {
      status: "failure",
      code: "retire-parse-failed",
      error: "Could not parse retireDeployment response from :8080.",
    };
  }
  const attrs = hit[1] || "";
  const grab = (k) => {
    const m = attrs.match(new RegExp(`${k}="([^"]*)"`));
    return m ? m[1] : "";
  };
  return {
    status: "success",
    uniqueId: grab("uniqueId"),
    previousName: grab("previousName"),
    name: grab("name"),
    alreadyVacated: /alreadyVacated="true"/i.test(attrs),
  };
}

export function commandUnknownMessage() {
  return (
    "Tomcat on :8080 does not yet understand retireDeployment, so Retire cannot free that live name. " +
    "Rebuild ROOT.war (`TawalaWebapp-build1700/scripts/build-root-war.sh`) and recreate the tawala container. " +
    "Library listing was left unchanged."
  );
}

/**
 * Node-store path: rename the `_index.json` slot, keep the uniqueId file.
 * @returns {{ uniqueId: string, previousName: string, name: string, alreadyVacated: boolean, userId: string } | null}
 */
export function vacateNodeStoreName(uniqueId) {
  if (!isValidUniqueId(uniqueId)) return null;
  const listed = store.findDeployedIndexByUniqueId(uniqueId);
  if (!listed?.entry) return null;
  const currentName = String(listed.entry.name || "").trim();
  const nextName = vacatedTomcatName(currentName || "project", uniqueId);
  return store.vacateDeployedNameByUniqueId(uniqueId, nextName);
}
