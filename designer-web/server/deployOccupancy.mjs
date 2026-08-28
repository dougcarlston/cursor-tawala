/**
 * Tomcat name occupancy for Push (`/api/deploy`).
 *
 * Java `ProjectsHibernateImpl.put` reuses UserProject when (userId, name) matches
 * and keeps submissions. Catalog JSON titled "Simple Survey Template" therefore
 * Redeploys the public Library Test Drive. Copy to MyTawala / File→New mint a
 * suffixed `deployIdentityName`; later Pushes of that identity are allowed.
 *
 * Occupied name + no claim on that live uniqueId → refuse. Administrator hatch
 * (`allowOccupiedNameRedeploy` + matching `TAWALA_OCCUPANCY_HATCH`) is the
 * in-place Library cleanup path. Designer UI never sends the hatch.
 */
import { parseStartpointsFromBlock, uniqueIdFromStartpoints } from "./deployParse.mjs";

const MINTED_SUFFIX_RE = /\s[0-9a-f]{8}$/i;

export function isMintedDeployName(name) {
  return MINTED_SUFFIX_RE.test(String(name || "").trim());
}

export function claimedUniqueId(project) {
  if (!project || typeof project !== "object") return "";
  return String(project.deployUniqueId || project.uniqueId || "").trim();
}

export function claimedDeployIdentityName(project) {
  if (!project || typeof project !== "object") return "";
  return String(project.deployIdentityName || "").trim();
}

export function claimedAuthorId(project) {
  if (!project || typeof project !== "object") return "";
  return String(project.authorId || project.author || project.userId || "").trim();
}

/**
 * Hatch is off unless the API process has TAWALA_OCCUPANCY_HATCH set and the
 * request repeats that token. Designer Push never sends these fields.
 */
export function occupancyHatchAllowed(body, env = process.env) {
  const secret = String(env?.TAWALA_OCCUPANCY_HATCH || "").trim();
  if (!secret) return false;
  if (!body || body.allowOccupiedNameRedeploy !== true) return false;
  return String(body.occupancyHatch || "").trim() === secret;
}

export function findOccupantInDeploymentsXml(xml, tomcatName) {
  const want = String(tomcatName || "").trim();
  if (!xml || !want) return null;
  const allDeployments = [
    ...String(xml).matchAll(/<deployment project="([^"]*)"[^>]*>([\s\S]*?)<\/deployment>/g),
  ];
  const lower = want.toLowerCase();
  for (const [, name, body] of allDeployments) {
    if (String(name).toLowerCase() !== lower) continue;
    const startpoints = parseStartpointsFromBlock(body);
    return {
      name,
      uniqueId: uniqueIdFromStartpoints(startpoints) || null,
    };
  }
  return null;
}

export function findOccupantInNodeIndex(entries, tomcatName, user = null) {
  const want = String(tomcatName || "").trim().toLowerCase();
  if (!want || !Array.isArray(entries)) return null;
  const hit = entries.find((e) => e && String(e.name || "").trim().toLowerCase() === want);
  if (!hit) return null;
  const author = hit.author || hit.authorId || hit.userId || hit.user || user || null;
  return {
    name: String(hit.name || "").trim(),
    uniqueId: hit.uniqueId ? String(hit.uniqueId) : null,
    ...(author ? { author, user: author } : {}),
  };
}

export function incomingOwnsOccupant(incomingProject, occupant, options = {}) {
  if (!occupant) return false;
  const uid = claimedUniqueId(incomingProject);
  if (uid && occupant.uniqueId && uid === String(occupant.uniqueId)) return true;
  const identity = claimedDeployIdentityName(incomingProject);
  if (
    identity &&
    occupant.name &&
    identity === occupant.name &&
    isMintedDeployName(identity)
  ) {
    return true;
  }
  const user = String(options?.user || options?.currentUser || "").trim();
  const occupantAuthor = String(
    occupant?.authorId || occupant?.author || occupant?.userId || occupant?.user || ""
  ).trim();
  const incomingAuthor = claimedAuthorId(incomingProject);

  if (user && occupantAuthor && user.toLowerCase() === occupantAuthor.toLowerCase()) {
    return true;
  }
  if (
    incomingAuthor &&
    occupantAuthor &&
    incomingAuthor.toLowerCase() === occupantAuthor.toLowerCase()
  ) {
    return true;
  }
  return false;
}

export function occupancyRefuseMessage(occupant, tomcatName) {
  const idBit = occupant?.uniqueId ? ` (uniqueId ${occupant.uniqueId})` : "";
  const name = occupant?.name || tomcatName || "that name";
  return (
    `That live project name is already in use: “${name}”${idBit}. ` +
    `Push under a different name, or retire the occupant first. ` +
    `In-place update of a public Library Test Drive is administrator-only ` +
    `(occupancy hatch) — it is not a normal Push.`
  );
}

export function occupancyLookupFailedMessage() {
  return (
    "Couldn't check whether that project name is already live on :8080. " +
    "Push refused so a public Library uniqueId cannot be overwritten by accident. " +
    "Confirm Tomcat is up, then retry."
  );
}

/**
 * @returns {{
 *   allow: boolean,
 *   code?: string,
 *   message?: string,
 * }}
 */
export function decideNameOccupancy({
  tomcatName,
  incomingProject,
  occupant,
  user,
  hatch = false,
  lookupFailed = false,
} = {}) {
  if (lookupFailed) {
    return {
      allow: false,
      code: "occupancy-lookup-failed",
      message: occupancyLookupFailedMessage(),
    };
  }
  if (!occupant) {
    return { allow: true, code: "unoccupied" };
  }
  if (hatch) {
    return { allow: true, code: "hatch" };
  }
  if (incomingOwnsOccupant(incomingProject, occupant, { user })) {
    return { allow: true, code: "owns-occupant" };
  }
  return {
    allow: false,
    code: "name-occupied",
    message: occupancyRefuseMessage(occupant, tomcatName),
  };
}

export function queryDeploymentsRequestXml(user, password) {
  const esc = (s) =>
    String(s ?? "")
      .replace(/&/g, "&amp;")
      .replace(/"/g, "&quot;")
      .replace(/</g, "&lt;");
  return (
    `<?xml version="1.0" encoding="utf-8"?>\n` +
    `<request type="queryDeployments" protocol="1.0">\n` +
    `  <credentials user="${esc(user)}" password="${esc(password)}"/>\n` +
    `</request>`
  );
}
