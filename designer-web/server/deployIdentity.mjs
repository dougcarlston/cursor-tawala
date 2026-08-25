import crypto from "crypto";

/** Short alphanumeric suffix for Tomcat project names (collision-proof in practice). */
export function mintDeploySuffix(bytes = 4) {
  return crypto.randomBytes(bytes).toString("hex");
}

/**
 * Java `ProjectsHibernateImpl.put` reuses UserProject when (userId, name) matches and
 * **keeps submissions**. New Project / File→New must Push under a name that cannot
 * match an existing row so Tomcat mints a fresh uniqueRandomId.
 *
 * Display `project.name` may stay friendly (coach/teacher labels); this value is what
 * goes in upload XML `<project name="…">`.
 *
 * @param {string} baseName Designer display name
 * @param {string} [suffix] optional fixed suffix (tests)
 */
export function mintNonCollidingDeployName(baseName, suffix = mintDeploySuffix()) {
  const base = String(baseName ?? "").trim() || "Project";
  const tag = String(suffix ?? "").trim() || mintDeploySuffix();
  // Space-separated so legacy UIs remain readable ("Exam Builder a1b2c3d4").
  return `${base} ${tag}`;
}

/**
 * Resolve the project body to upload and whether a new Tomcat identity was minted.
 *
 * - `_freshFromTemplate`: always mint a new deploy name (never Redeploy-by-name).
 * - Else if `deployIdentityName` is set: Push under that name (stable Redeploy).
 * - Else: Push under display `name` (projects authored before this field existed).
 *
 * @param {object} project
 * @returns {{
 *   projectForDeploy: object,
 *   deployIdentityName: string,
 *   displayName: string,
 *   freshFromTemplate: boolean,
 *   identityMinted: boolean,
 * }}
 */
export function resolveProjectForDeploy(project, { mintSuffix } = {}) {
  const freshFromTemplate = project?._freshFromTemplate === true;
  const {
    _freshFromTemplate: _drop,
    ...rest
  } = project && typeof project === "object" ? project : {};
  const displayName = String(rest.name ?? "").trim() || "Project";

  if (freshFromTemplate) {
    const deployIdentityName = mintNonCollidingDeployName(displayName, mintSuffix);
    return {
      projectForDeploy: {
        ...rest,
        name: deployIdentityName,
        deployIdentityName,
      },
      deployIdentityName,
      displayName,
      freshFromTemplate: true,
      identityMinted: true,
    };
  }

  const existing = String(rest.deployIdentityName ?? "").trim();
  const deployIdentityName = existing || displayName;
  return {
    projectForDeploy: {
      ...rest,
      name: deployIdentityName,
      deployIdentityName,
    },
    deployIdentityName,
    displayName,
    freshFromTemplate: false,
    identityMinted: false,
  };
}
