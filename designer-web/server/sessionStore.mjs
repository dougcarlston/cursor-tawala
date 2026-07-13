import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const SESSION_DIR = path.join(__dirname, "..", ".deployed", "sessions");

function ensureDir() {
  if (!fs.existsSync(SESSION_DIR)) fs.mkdirSync(SESSION_DIR, { recursive: true });
}

export function getSession(uniqueId) {
  ensureDir();
  const file = path.join(SESSION_DIR, `${uniqueId}.json`);
  if (!fs.existsSync(file)) return null;
  return JSON.parse(fs.readFileSync(file, "utf8"));
}

export function saveSession(uniqueId, session) {
  ensureDir();
  const file = path.join(SESSION_DIR, `${uniqueId}.json`);
  fs.writeFileSync(file, JSON.stringify(session, null, 2));
}

export function createSession(project) {
  const session = {
    fields: {},
    formFields: {},
    records: {},
    formState: {},
    preProcessDone: {},
  };
  seedDefaultRecords(project, session);
  return session;
}

/** Drop saved answers and return a fresh session (keeps AdminSetup/Divisions seed data). */
export function resetSession(uniqueId, project) {
  ensureDir();
  const file = path.join(SESSION_DIR, `${uniqueId}.json`);
  if (fs.existsSync(file)) fs.unlinkSync(file);
  const session = createSession(project);
  saveSession(uniqueId, session);
  return session;
}

function seedDefaultRecords(project, session) {
  if (!project || project.name !== "DirtBowl") return;

  if (!session.records.AdminSetup?.length) {
    session.records.AdminSetup = [
      {
        AdminsName: "Sample Athletic Director",
        AdminsEmail: "director@example.com",
        AdminPhone: "555-0100",
        AdminAddress: "123 Main St",
        AdminCity: "Anytown",
        AdminST: "CA",
        AdminZIP: "90210",
        LeagueName: "Dirt Bowl",
        Season: "2026",
        IndividualSignupFee: "75",
        ChargeDescription: "Summer league registration",
        "UsingDivisions?": "Yes",
      },
    ];
  }

  if (!session.records.Divisions?.length) {
    session.records.Divisions = [
      { DivNames: "8U Co-ed", DivisionID: "8U" },
      { DivNames: "10U Boys", DivisionID: "10U-B" },
      { DivNames: "10U Girls", DivisionID: "10U-G" },
      { DivNames: "12U Boys", DivisionID: "12U-B" },
      { DivNames: "12U Girls", DivisionID: "12U-G" },
    ];
  }

  session.fields.Divs = session.fields.Divs ?? "Yes";
  session.fields.PaymentReceived = session.fields.PaymentReceived ?? "No";
  session.fields.From = session.fields.From ?? "";
}

export function getOrCreateSession(uniqueId, project) {
  let session = getSession(uniqueId);
  if (!session) {
    session = createSession(project);
    saveSession(uniqueId, session);
  } else {
    seedDefaultRecords(project, session);
  }
  scrubRegistrationBlankCollisions(session);
  return session;
}

/** Remove legacy blank aliases that collide between Q3 school and Q10 friends slots. */
export function scrubRegistrationBlankCollisions(session, formName = "Registration") {
  if (formName !== "Registration") return;
  for (const slot of ["a", "b", "c"]) {
    delete session.fields[slot];
    delete session.fields[`${formName}:${slot}`];
  }
  const ff = session.formFields[formName];
  if (!ff) return;
  const school = ff["Q3:a"];
  for (const slot of ["a", "b", "c"]) {
    const friendKey = `Q10:${slot}`;
    if (school && ff[friendKey] === school) {
      delete ff[friendKey];
      delete session.fields[friendKey];
      delete session.fields[`${formName}:Q10:${slot}`];
    }
  }
}

/** Merge POST body into session field maps */
export function applySubmission(session, formName, body) {
  if (!session.formFields[formName]) session.formFields[formName] = {};
  const form = session.formFields[formName];
  for (const [key, value] of Object.entries(body)) {
    if (key === "submit" || key === "from") continue;
    form[key] = value;
    if (key.includes(":")) {
      const [, blank] = key.split(":");
      session.fields[`${formName}:${blank}`] = value;
      session.fields[blank] = value;
    } else {
      session.fields[key] = value;
    }
    session.fields[`${formName}:${key}`] = value;
  }
}
