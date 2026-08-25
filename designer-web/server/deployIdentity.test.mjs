import { describe, expect, it } from "vitest";
import {
  mintNonCollidingDeployName,
  resolveProjectForDeploy,
} from "./deployIdentity.mjs";

describe("mintNonCollidingDeployName", () => {
  it("appends a suffix to the display name", () => {
    expect(mintNonCollidingDeployName("Exam Builder", "a1b2c3d4")).toBe(
      "Exam Builder a1b2c3d4",
    );
  });

  it("falls back when display name is blank", () => {
    expect(mintNonCollidingDeployName("  ", "deadbeef")).toBe("Project deadbeef");
  });

  it("produces distinct names across calls", () => {
    const a = mintNonCollidingDeployName("Sign-up Sheet Template");
    const b = mintNonCollidingDeployName("Sign-up Sheet Template");
    expect(a).not.toBe(b);
    expect(a.startsWith("Sign-up Sheet Template ")).toBe(true);
  });
});

describe("resolveProjectForDeploy", () => {
  it("mints a new Tomcat name on File→New (_freshFromTemplate)", () => {
    const r = resolveProjectForDeploy(
      {
        name: "Exam Builder",
        forms: [],
        _freshFromTemplate: true,
        deployIdentityName: "stale should be ignored",
      },
      { mintSuffix: "aabbccdd" },
    );
    expect(r.identityMinted).toBe(true);
    expect(r.freshFromTemplate).toBe(true);
    expect(r.displayName).toBe("Exam Builder");
    expect(r.deployIdentityName).toBe("Exam Builder aabbccdd");
    expect(r.projectForDeploy.name).toBe("Exam Builder aabbccdd");
    expect(r.projectForDeploy.deployIdentityName).toBe("Exam Builder aabbccdd");
    expect(r.projectForDeploy._freshFromTemplate).toBeUndefined();
  });

  it("reuses deployIdentityName on later Push (not fresh)", () => {
    const r = resolveProjectForDeploy({
      name: "Coach Registration",
      deployIdentityName: "Coach Registration deadbeef",
      forms: [],
    });
    expect(r.identityMinted).toBe(false);
    expect(r.projectForDeploy.name).toBe("Coach Registration deadbeef");
    expect(r.displayName).toBe("Coach Registration");
  });

  it("falls back to display name when no identity was minted yet", () => {
    const r = resolveProjectForDeploy({ name: "Legacy Project", forms: [] });
    expect(r.deployIdentityName).toBe("Legacy Project");
    expect(r.projectForDeploy.name).toBe("Legacy Project");
  });
});
