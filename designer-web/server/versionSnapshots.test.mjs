import { describe, it, expect, beforeEach, afterEach } from "vitest";
import fs from "fs";
import path from "path";
import {
  saveVersionSnapshot,
  getVersionSnapshot,
  versionSnapshotsDir,
} from "./versionSnapshots.mjs";

describe("versionSnapshots", () => {
  const dir = versionSnapshotsDir();
  const written = [];

  afterEach(() => {
    for (const id of written) {
      const p = path.join(dir, `${id}.json`);
      if (fs.existsSync(p)) fs.unlinkSync(p);
    }
    written.length = 0;
  });

  it("saves and loads a project definition", () => {
    const { snapshotId } = saveVersionSnapshot({
      project: { name: "Simple Survey", forms: [], _freshFromTemplate: true },
      uniqueId: "abc123xyz",
      projectId: "simple-survey",
      versionDescription: "v1",
    });
    written.push(snapshotId);
    const loaded = getVersionSnapshot(snapshotId);
    expect(loaded).toBeTruthy();
    expect(loaded.project.name).toBe("Simple Survey");
    expect(loaded.project._freshFromTemplate).toBeUndefined();
    expect(loaded.uniqueId).toBe("abc123xyz");
    expect(loaded.versionDescription).toBe("v1");
  });

  it("rejects missing project", () => {
    expect(() => saveVersionSnapshot({})).toThrow(/project required/);
  });

  it("returns null for unknown or unsafe ids", () => {
    expect(getVersionSnapshot("nope")).toBeNull();
    expect(getVersionSnapshot("../etc/passwd")).toBeNull();
  });
});
