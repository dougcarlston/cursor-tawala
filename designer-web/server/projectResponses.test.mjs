import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import { afterEach, describe, expect, it } from "vitest";
import {
  buildSubmissionContentsXml,
  exportProjectResponsesByUniqueId,
  importProjectResponsesByUniqueId,
  isValidUniqueId,
  parseSubmissionContentsXml,
} from "./projectResponses.mjs";
import { getSession, saveSession } from "./sessionStore.mjs";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const SESSION_DIR = path.join(__dirname, "..", ".deployed", "sessions");

/**
 * Regression coverage for the Aug 1, 2026 bug: index.mjs imported
 * exportProjectResponsesByUniqueId/importProjectResponsesByUniqueId, but this module only
 * exported differently-named/-shaped functions — a missing ES-module named export crashes the
 * *entire* dev server at startup (not a normal caught error), so these two functions existing
 * with the right shape is itself the most important thing this file asserts.
 */
describe("projectResponses XML round-trip", () => {
  it("parses and rebuilds the XStream linked-hash-map shape", () => {
    const xml =
      "<linked-hash-map>" +
      "<entry><string>Q1</string><string-array><string>b</string><string>c</string></string-array></entry>" +
      "<entry><string>Name</string><string-array><string>Doug</string></string-array></entry>" +
      "</linked-hash-map>";
    const { fields, order } = parseSubmissionContentsXml(xml);
    expect(order).toEqual(["Q1", "Name"]);
    expect(fields).toEqual({ Q1: ["b", "c"], Name: ["Doug"] });
    expect(buildSubmissionContentsXml(fields, order)).toBe(xml);
  });

  it("round-trips a self-closed (unanswered) string-array", () => {
    const xml = "<linked-hash-map><entry><string>MCQ1</string><string-array/></entry></linked-hash-map>";
    const { fields, order } = parseSubmissionContentsXml(xml);
    expect(fields).toEqual({ MCQ1: [] });
    expect(buildSubmissionContentsXml(fields, order)).toBe(xml);
  });

  it("escapes/unescapes XML entities in field names and values", () => {
    const xml = buildSubmissionContentsXml({ 'A&B<C>"D\'E': ["x&y"] }, ['A&B<C>"D\'E']);
    const { fields } = parseSubmissionContentsXml(xml);
    expect(fields['A&B<C>"D\'E']).toEqual(["x&y"]);
  });
});

describe("isValidUniqueId", () => {
  it("matches the shared 1-20 alphanumeric rule", () => {
    expect(isValidUniqueId("gy1zssbrwm4fgfm")).toBe(true);
    expect(isValidUniqueId("")).toBe(false);
    expect(isValidUniqueId("bad-id")).toBe(false);
    expect(isValidUniqueId("x".repeat(21))).toBe(false);
  });
});

/**
 * Dev-session fallback (no Postgres/Docker needed) — same fallback purgeProjectResponses.mjs
 * already uses for dev-only deploys. Postgres lookups in these two calls will fail fast in any
 * environment without the docker-compose stack up; what matters here is that they then fall
 * through to the session-store path instead of hard-failing.
 */
describe("dev-session export/import fallback", () => {
  const TEST_ID = "vitestDevSess01";

  afterEach(() => {
    const file = path.join(SESSION_DIR, `${TEST_ID}.json`);
    if (fs.existsSync(file)) fs.unlinkSync(file);
  });

  it("exports session.records as forms when there is no Postgres row", async () => {
    saveSession(TEST_ID, {
      fields: {},
      formFields: {},
      records: {
        Survey: [
          { Name: "Doug", Q1: "b" },
          { Name: "Larry", Q1: "d" },
        ],
      },
      formState: {},
      preProcessDone: {},
    });

    const result = await exportProjectResponsesByUniqueId(TEST_ID);
    expect(result.status).toBe("success");
    expect(result.source).toBe("dev-session");
    expect(result.count).toBe(2);
    expect(result.forms).toEqual([
      {
        form: "Survey",
        rows: [
          { createdAt: null, fields: { Name: ["Doug"], Q1: ["b"] } },
          { createdAt: null, fields: { Name: ["Larry"], Q1: ["d"] } },
        ],
      },
    ]);
    expect(result.fieldsByForm).toEqual({ Survey: ["Name", "Q1"] });
  });

  it("reports zero rows (not failure) for a known session with no records yet", async () => {
    saveSession(TEST_ID, { fields: {}, formFields: {}, records: {}, formState: {}, preProcessDone: {} });
    const result = await exportProjectResponsesByUniqueId(TEST_ID);
    expect(result.status).toBe("success");
    expect(result.source).toBe("dev-session");
    expect(result.count).toBe(0);
    expect(result.forms).toEqual([]);
  });

  it("fails for a uniqueId that is neither a Postgres row nor a dev session", async () => {
    const result = await exportProjectResponsesByUniqueId("noSuchIdAtAll99");
    expect(result.status).toBe("failure");
  });

  it("replaces session.records on import (mode: replace) and re-export reflects it", async () => {
    saveSession(TEST_ID, {
      fields: {},
      formFields: {},
      records: { Survey: [{ Name: "Old", Q1: "a" }] },
      formState: {},
      preProcessDone: {},
    });

    const importResult = await importProjectResponsesByUniqueId(TEST_ID, [
      {
        form: "Survey",
        rows: [
          { createdAt: null, fields: { Name: ["New1"], Q1: ["b"] } },
          { createdAt: null, fields: { Name: ["New2"], Q1: ["c"] } },
        ],
      },
    ]);
    expect(importResult.status).toBe("success");
    expect(importResult.inserted).toBe(2);

    const session = getSession(TEST_ID);
    expect(session.records.Survey).toEqual([
      { Name: "New1", Q1: "b" },
      { Name: "New2", Q1: "c" },
    ]);

    const reExported = await exportProjectResponsesByUniqueId(TEST_ID);
    expect(reExported.count).toBe(2);
  });

  it("rejects a non-array forms payload without touching the session", async () => {
    const result = await importProjectResponsesByUniqueId(TEST_ID, "not-an-array");
    expect(result.status).toBe("failure");
  });
});
