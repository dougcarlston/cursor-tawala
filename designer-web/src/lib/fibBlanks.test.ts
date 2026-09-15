import { describe, expect, it } from "vitest";
import {
  FIB_DEFAULT_PROMPT,
  FIB_DEFAULT_UNDERSCORES,
  FIB_PLACEHOLDER,
} from "@/types/tawala";
import {
  fibHintHighlightEnd,
  parseUnderscoreRuns,
  syncBlanksFromPrompt,
} from "@/lib/fibBlanks";

describe("FIB underscore → blanks (Design metadata)", () => {
  it("parses underscore runs for blank length", () => {
    const runs = parseUnderscoreRuns("Name ________");
    expect(runs).toHaveLength(1);
    expect(runs[0].length).toBe(8);
  });

  it("syncs blank count and length from underscore runs", () => {
    const blanks = syncBlanksFromPrompt("Name ________ lives in ______.", [], "FIB1");
    expect(blanks).toHaveLength(2);
    expect(blanks[0]).toMatchObject({ name: "a", length: 8, alternateLabel: "FIB1:a" });
    expect(blanks[1]).toMatchObject({ name: "b", length: 6, alternateLabel: "FIB1:b" });
  });

  it("preserves existing blank attributes when run count is unchanged", () => {
    const existing = [
      {
        name: "a",
        length: 3,
        alternateLabel: "Surveyee",
        required: true,
        height: 2,
        caption: "First",
      },
    ];
    const blanks = syncBlanksFromPrompt("Name ________", existing, "FIB1");
    expect(blanks).toHaveLength(1);
    expect(blanks[0]).toMatchObject({
      name: "a",
      length: 8,
      alternateLabel: "Surveyee",
      required: true,
      height: 2,
      caption: "First",
    });
  });
});

describe("fibIdleHtmlWithCaptions", () => {
  it("wraps underscore runs with caption spans when captions are set", async () => {
    const { fibIdleHtmlWithCaptions } = await import("@/lib/fibBlanks");
    const html = fibIdleHtmlWithCaptions(
      "Name: ________ ________",
      [
        { name: "a", length: 8, caption: "First" },
        { name: "b", length: 8, caption: "Last" },
      ],
      (s) => s,
    );
    expect(html).toContain('class="fib-blank-caption">First<');
    expect(html).toContain('class="fib-blank-caption">Last<');
    expect(html).toContain("________");
  });

  it("returns null when no captions (caller keeps normal idle HTML)", async () => {
    const { fibIdleHtmlWithCaptions } = await import("@/lib/fibBlanks");
    expect(
      fibIdleHtmlWithCaptions("Name ________", [{ name: "a", length: 8 }], (s) => s),
    ).toBeNull();
  });

  it("paints required star on the prompt, not on Caption", async () => {
    const { fibIdleHtmlWithCaptions } = await import("@/lib/fibBlanks");
    const html = fibIdleHtmlWithCaptions(
      "Name: ________",
      [{ name: "a", length: 8, caption: "First", required: true }],
      (s) => s,
    );
    expect(html).toContain("Name:");
    expect(html).toContain('class="qinfo"');
    expect(html.indexOf("class=\"qinfo\"")).toBeLessThan(html.indexOf("fib-blank-caption"));
    expect(html).toContain('class="fib-blank-caption">First<');
    expect(html).not.toMatch(/fib-blank-caption">[^<]*class="qinfo"/);
  });

  it("Above idle stacks a one-line prompt without a return", async () => {
    const { fibIdleHtmlTopLabels } = await import("@/lib/fibBlanks");
    const html = fibIdleHtmlTopLabels(
      "Organization Name: ________",
      [{ name: "a", length: 8, required: true }],
      (s) => s,
    );
    expect(html).toContain("fib-toplabel-idle");
    expect(html).toContain("fib-toplabel-prompt");
    expect(html).toContain("Organization Name:");
    expect(html).toContain('class="qinfo"');
    expect(html.indexOf("Organization Name:")).toBeLessThan(html.indexOf("________"));
    expect(html.indexOf("class=\"qinfo\"")).toBeLessThan(html.indexOf("________"));
  });

  it("Above idle puts required star on a two-line Name / blanks prompt", async () => {
    const { fibIdleHtmlTopLabels } = await import("@/lib/fibBlanks");
    const html = fibIdleHtmlTopLabels(
      "Name\n___________________________________",
      [{ name: "attendeeName", length: 35, required: true }],
      (s) => s,
    );
    expect(html).toContain("fib-toplabel-prompt");
    expect(html.indexOf("Name")).toBeLessThan(html.indexOf("class=\"qinfo\""));
    expect(html.indexOf("class=\"qinfo\"")).toBeLessThan(html.indexOf("________"));
  });
});

describe("fibHintHighlightEnd", () => {
  it("selects only the stock hint — not trailing underscores (space-separated)", () => {
    expect(fibHintHighlightEnd(FIB_DEFAULT_PROMPT)).toBe(FIB_PLACEHOLDER.length);
    expect(FIB_DEFAULT_PROMPT.slice(fibHintHighlightEnd(FIB_DEFAULT_PROMPT)!)).toBe(
      ` ${FIB_DEFAULT_UNDERSCORES}`,
    );
  });

  it("selects only the stock hint when blanks wrap onto the next line", () => {
    const plain = `${FIB_PLACEHOLDER}\n${FIB_DEFAULT_UNDERSCORES}`;
    expect(fibHintHighlightEnd(plain)).toBe(FIB_PLACEHOLDER.length);
  });

  it("returns null for customized question prose with blanks", () => {
    expect(fibHintHighlightEnd("Name ________")).toBeNull();
    expect(fibHintHighlightEnd(`${FIB_PLACEHOLDER} Extra prose ________`)).toBeNull();
  });
});
