import { describe, expect, it, vi, afterEach } from "vitest";
import { canSeeOwnerOpsMenu } from "./ownerMenuAccess";

describe("canSeeOwnerOpsMenu", () => {
  afterEach(() => {
    vi.unstubAllEnvs();
  });

  it("hides when no credentials user", () => {
    expect(canSeeOwnerOpsMenu(null)).toBe(false);
    expect(canSeeOwnerOpsMenu("")).toBe(false);
    expect(canSeeOwnerOpsMenu("  ")).toBe(false);
  });

  it("hides default local demo user when no allowlist", () => {
    vi.stubEnv("VITE_OWNER_MENU_USERS", "");
    expect(canSeeOwnerOpsMenu("dev")).toBe(false);
    expect(canSeeOwnerOpsMenu("Dev")).toBe(false);
  });

  it("shows non-dev login when no allowlist", () => {
    vi.stubEnv("VITE_OWNER_MENU_USERS", "");
    expect(canSeeOwnerOpsMenu("DougC")).toBe(true);
  });

  it("respects VITE_OWNER_MENU_USERS allowlist", () => {
    vi.stubEnv("VITE_OWNER_MENU_USERS", "DougC, doug.carlston");
    expect(canSeeOwnerOpsMenu("DougC")).toBe(true);
    expect(canSeeOwnerOpsMenu("doug.carlston")).toBe(true);
    expect(canSeeOwnerOpsMenu("dev")).toBe(false);
    expect(canSeeOwnerOpsMenu("someone-else")).toBe(false);
  });
});
