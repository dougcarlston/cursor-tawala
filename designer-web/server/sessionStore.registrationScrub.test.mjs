/**
 * Registration blank scrub must not wipe a/b/c on Signup / Potluck sessions.
 */
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import { afterEach, describe, expect, it } from "vitest";
import { getOrCreateSession, getSession, saveSession } from "./sessionStore.mjs";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const SESSION_DIR = path.join(__dirname, "..", ".deployed", "sessions");

function removeSession(uniqueId) {
  const file = path.join(SESSION_DIR, `${uniqueId}.json`);
  if (fs.existsSync(file)) fs.unlinkSync(file);
}

afterEach(() => {
  removeSession("scrub-potluck-uid");
  removeSession("scrub-reg-uid");
});

describe("scrubRegistrationBlankCollisions gating", () => {
  it("does not delete bare a/b/c for Potluck / Signup projects", () => {
    const uniqueId = "scrub-potluck-uid";
    removeSession(uniqueId);
    const project = {
      name: "potluck",
      forms: [{ name: "Potluck Organizer", items: [] }],
    };
    const session = getOrCreateSession(uniqueId, project);
    session.fields.a = "Alice";
    session.fields.b = "2";
    session.fields.c = "Salad";
    saveSession(uniqueId, session);

    const again = getOrCreateSession(uniqueId, project);
    expect(again.fields.a).toBe("Alice");
    expect(again.fields.b).toBe("2");
    expect(again.fields.c).toBe("Salad");
  });

  it("still scrubs bare a/b/c when the project has a Registration form", () => {
    const uniqueId = "scrub-reg-uid";
    removeSession(uniqueId);
    const project = {
      name: "DirtBowl",
      forms: [{ name: "Registration", items: [] }],
    };
    const session = getOrCreateSession(uniqueId, project);
    session.fields.a = "School";
    session.fields.b = "Friend1";
    session.fields.c = "Friend2";
    saveSession(uniqueId, session);

    const again = getOrCreateSession(uniqueId, project);
    expect(again.fields.a).toBeUndefined();
    expect(again.fields.b).toBeUndefined();
    expect(again.fields.c).toBeUndefined();
    // seed still applied for DirtBowl
    expect(getSession(uniqueId)).toBeTruthy();
  });
});
