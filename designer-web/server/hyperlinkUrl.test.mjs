import { describe, expect, it } from "vitest";
import { normalizeHyperlinkUrl } from "./hyperlinkUrl.mjs";

describe("normalizeHyperlinkUrl (deploy)", () => {
  it("adds https for bare domains", () => {
    expect(normalizeHyperlinkUrl("example.com")).toBe("https://example.com");
    expect(normalizeHyperlinkUrl("www.example.com/path")).toBe("https://www.example.com/path");
  });

  it("leaves field expressions and relative paths alone", () => {
    expect(normalizeHyperlinkUrl("<<Registration:Website>>")).toBe("<<Registration:Website>>");
    expect(normalizeHyperlinkUrl("/images/foo.png")).toBe("/images/foo.png");
  });
});
