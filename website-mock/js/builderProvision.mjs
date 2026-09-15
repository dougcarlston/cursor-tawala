/**
 * Version 1 provision for Builder vs user-contributed exams (LIBRARY_WHERE_WE_ARE § D).
 * Public Publish-as-exam is Version 2. These fields must not change a teacher's
 * private My Tawala Use of OEB (questions, students, uniqueId).
 *
 * Keep transfer.js stripBuilderProvisionFromPrivateRow / isPublicLibraryBuilder in sync.
 */

export const BUILDER_PROVISION_KEYS = [
  "builder",
  "builderVersion",
  "builderKeepForms",
  "builderStripForms",
  "builderTransientForms",
  "derivedFromBuilderId",
  "builderSnapshotId",
];

/** OEB keep/strip map — stored on the Library Builder listing; unused by Publish in Version 1. */
export const OEB_BUILDER_ROLE_MAP = {
  keepForms: ["SetupVariables", "Question"],
  stripForms: ["Exam", "Answer", "Scoring", "ShowExamineeDetail"],
  transientForms: ["Edit Question", "Delete Question", "CustomizationPreview"],
};

export function stripBuilderProvisionFromPrivateRow(row) {
  if (!row || typeof row !== "object") return row;
  const next = { ...row };
  for (let i = 0; i < BUILDER_PROVISION_KEYS.length; i++) {
    delete next[BUILDER_PROVISION_KEYS[i]];
  }
  return next;
}

/**
 * True only for the public Library Builder listing (blank instrument).
 * My Tawala copies, forks, and Designer Pushes are never Builders — even if a
 * provision field leaked onto the overlay.
 */
export function isPublicLibraryBuilder(entry) {
  if (!entry || entry.builder !== true) return false;
  if (entry.fromLibraryAcquire || entry.sourcePile === "library-acquire") return false;
  if (entry.fromDeployOverlay || entry.sourcePile === "deploy-overlay") return false;
  if (entry.sourcePile === "mytawala-fork") return false;
  if (entry.pulledFromLibraryId) return false;
  return true;
}
