import { describe, expect, it } from "vitest";
import { isValidUniqueId, uniqueIdFromRuntimeUrl } from "./purgeProjectResponses.mjs";

describe("purgeProjectResponses helpers", () => {
  it("accepts alphanumeric uniqueIds up to 20 chars", () => {
    expect(isValidUniqueId("gy1zssbrwm4fgfm")).toBe(true);
    expect(isValidUniqueId("a")).toBe(true);
    expect(isValidUniqueId("")).toBe(false);
    expect(isValidUniqueId("bad-id")).toBe(false);
    expect(isValidUniqueId("x".repeat(21))).toBe(false);
  });

  it("extracts uniqueId from :8080 runtime URLs", () => {
    expect(
      uniqueIdFromRuntimeUrl("http://localhost:8080/p/gy1zssbrwm4fgfm/npwtqlg.Survey"),
    ).toBe("gy1zssbrwm4fgfm");
    expect(uniqueIdFromRuntimeUrl("http://localhost:8080/p/cicw55xxhvwrrh7/Form+1")).toBe(
      "cicw55xxhvwrrh7",
    );
    expect(uniqueIdFromRuntimeUrl("http://localhost:8080/client")).toBe(null);
    expect(uniqueIdFromRuntimeUrl(null)).toBe(null);
  });
});
