import { describe, expect, it } from "vitest";
import { blankLiveRichTextEnabled, FIB_NEW_MULTILINE_RICH_TEXT_DEFAULT } from "./fibBlankRichText";

describe("fibBlankRichText", () => {
  it("defaults new multi-line blanks to plain (no live toolbar)", () => {
    expect(FIB_NEW_MULTILINE_RICH_TEXT_DEFAULT).toBe(false);
  });

  it("leaves live formatter off when richText is unset on multi-line blanks", () => {
    expect(blankLiveRichTextEnabled({ height: 5 })).toBe(false);
  });

  it("enables live formatter only when richText is explicitly true", () => {
    expect(blankLiveRichTextEnabled({ height: 5, richText: true })).toBe(true);
  });

  it("respects richText false on multi-line blanks", () => {
    expect(blankLiveRichTextEnabled({ height: 5, richText: false })).toBe(false);
  });

  it("never enables formatter on single-line blanks", () => {
    expect(blankLiveRichTextEnabled({ height: 1, richText: true })).toBe(false);
  });
});
