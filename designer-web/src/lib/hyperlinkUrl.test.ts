import { describe, expect, it } from "vitest";
import { normalizeHyperlinkUrl } from "./hyperlinkUrl";

describe("normalizeHyperlinkUrl", () => {
  it("adds https for bare domains", () => {
    expect(normalizeHyperlinkUrl("example.com")).toBe("https://example.com");
    expect(normalizeHyperlinkUrl("dirtbowl.com/about")).toBe("https://dirtbowl.com/about");
    expect(normalizeHyperlinkUrl("www.example.com")).toBe("https://www.example.com");
  });

  it("leaves explicit schemes unchanged", () => {
    expect(normalizeHyperlinkUrl("http://example.com")).toBe("http://example.com");
    expect(normalizeHyperlinkUrl("https://example.com")).toBe("https://example.com");
    expect(normalizeHyperlinkUrl("mailto:user@example.com")).toBe("mailto:user@example.com");
  });

  it("does not rewrite field expressions", () => {
    expect(normalizeHyperlinkUrl("<<Registration:Website>>")).toBe("<<Registration:Website>>");
    expect(normalizeHyperlinkUrl("https://x.com/<<Registration:Id>>")).toBe(
      "https://x.com/<<Registration:Id>>",
    );
  });

  it("leaves site-relative paths alone", () => {
    expect(normalizeHyperlinkUrl("/images/foo.png")).toBe("/images/foo.png");
    expect(normalizeHyperlinkUrl("./page.html")).toBe("./page.html");
  });

  it("uses http for localhost and IPv4 hosts", () => {
    expect(normalizeHyperlinkUrl("localhost:8080/p")).toBe("http://localhost:8080/p");
    expect(normalizeHyperlinkUrl("127.0.0.1:3001/api")).toBe("http://127.0.0.1:3001/api");
  });

  it("trims surrounding whitespace", () => {
    expect(normalizeHyperlinkUrl("  example.com  ")).toBe("https://example.com");
  });
});
