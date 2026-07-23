import { describe, expect, it } from "vitest";
import {
  normalizeFibPromptSource,
  parseFibPrompt,
  plainForUnderscores,
  segmentsFromUnderscorePrompt,
} from "./fibPrompt.mjs";

describe("FIB underscore → blanks (Preview / fibPrompt)", () => {
  const blanks = [
    { name: "a", length: 8, alternateLabel: "FIB1:a" },
    { name: "b", length: 6, alternateLabel: "FIB1:b" },
    { name: "c", length: 6, alternateLabel: "FIB1:c" },
  ];

  it("replaces underscore runs with blank segments (no leftover _ characters)", () => {
    const rows = parseFibPrompt("Name ________", blanks);
    expect(rows).toHaveLength(1);
    const types = rows[0].segments.map((s) => s.type);
    expect(types).toEqual(["text", "blank"]);
    expect(rows[0].segments[0]).toMatchObject({ type: "text", text: "Name " });
    expect(rows[0].segments[1].type).toBe("blank");
    expect(rows[0].segments[1].blank.name).toBe("a");
    const textJoined = rows[0].segments
      .filter((s) => s.type === "text")
      .map((s) => s.text)
      .join("");
    expect(textJoined).not.toMatch(/_/);
  });

  it("handles multiple underscore blanks on one line", () => {
    const { segments } = segmentsFromUnderscorePrompt("A ____ and ____", blanks, 0);
    expect(segments.map((s) => s.type)).toEqual(["text", "blank", "text", "blank"]);
    expect(segments.filter((s) => s.type === "text").every((s) => !/_/.test(s.text))).toBe(true);
  });

  it("still supports slash-separated label/blank rows", () => {
    const rows = parseFibPrompt("City/", blanks.slice(0, 1));
    expect(rows[0].segments.map((s) => s.type)).toEqual(["text", "blank"]);
    expect(rows[0].segments[0].text).toBe("City");
  });

  it("does not treat the Design FIB placeholder as a legacy [hint]", () => {
    const prompt =
      "[Replace this with your question. Underscores create blanks.]\nName ________ Phone ________";
    const rows = parseFibPrompt(prompt, blanks);
    // Placeholder-only first soft row is dropped; Name/Phone line becomes one row of blanks.
    expect(rows.length).toBeGreaterThanOrEqual(1);
    const last = rows[rows.length - 1];
    expect(last.segments.some((s) => s.type === "blank")).toBe(true);
    const texts = rows.flatMap((r) => r.segments.filter((s) => s.type === "text").map((s) => s.text));
    expect(texts.join(" ")).not.toMatch(/Underscores create blanks/i);
    expect(texts.join("")).not.toMatch(/_/);
  });

  it("decodes &nbsp; so Preview text never shows the entity", () => {
    expect(normalizeFibPromptSource("A&nbsp;B")).toBe("A B");
    const rows = parseFibPrompt("Parent&nbsp;Email ________", blanks.slice(0, 1));
    const text = rows[0].segments.find((s) => s.type === "text").text;
    expect(text).toContain("Parent Email");
    expect(text).not.toContain("&nbsp;");
  });

  it("does not invent a stray blank for an empty instructional leftover", () => {
    const rows = parseFibPrompt(
      "[Replace this with your question. Underscores create blanks.]",
      blanks,
    );
    expect(rows).toHaveLength(0);
  });

  it("does not treat </span> as a legacy slash separator (contenteditable around underscores)", () => {
    const signupBlanks = [
      { name: "a", alternateLabel: "First", length: 20 },
      { name: "b", alternateLabel: "Last", length: 11 },
      { name: "c", alternateLabel: "Email", length: 20 },
      { name: "d", alternateLabel: "Tel", length: 19 },
    ];
    const prompt =
      `First Name: ________ Last Name: ________<br>` +
      `Email <span style="font-family: inherit; background-color: transparent;">________</span> ` +
      `Phone <span style="font-family: inherit; background-color: transparent;">________</span>`;
    const rows = parseFibPrompt(prompt, signupBlanks);
    expect(rows.length).toBeGreaterThanOrEqual(2);
    const allText = rows
      .flatMap((r) => r.segments.filter((s) => s.type === "text").map((s) => s.text))
      .join("");
    expect(allText).not.toMatch(/span/i);
    expect(allText).not.toMatch(/_/);
    expect(allText).not.toMatch(/font-family/i);
    const blanksUsed = rows.flatMap((r) => r.segments.filter((s) => s.type === "blank"));
    expect(blanksUsed.map((s) => s.blank.alternateLabel)).toEqual([
      "First",
      "Last",
      "Email",
      "Tel",
    ]);
  });

  it("keeps text segments in prompt order for multi-blank left-style rows", () => {
    const rows = parseFibPrompt("Name ________ Email ________", blanks.slice(0, 2));
    expect(rows[0].segments.map((s) => s.type)).toEqual(["text", "blank", "text", "blank"]);
    expect(rows[0].segments[0].text).toMatch(/Name/);
    expect(rows[0].segments[2].text).toMatch(/Email/);
  });

  it("preserves <<Field>> tokens (Signup ContactType labels — not HTML tags)", () => {
    expect(plainForUnderscores("Your <<ContactType1>>:")).toBe("Your <<ContactType1>>:");
    const rows = parseFibPrompt(
      "Please provide your <<ContactType1>>:_________________________________________",
      [{ name: "a", length: 41 }],
    );
    expect(rows[0].segments[0].text).toBe("Please provide your <<ContactType1>>:");
    expect(rows[0].segments[0].text).not.toMatch(/^Please provide your >:$/);
  });

  it("does not let an intro soft-row steal the blank from a later Name:____ line", () => {
    const prompt =
      'Add your name to the list:<div><br><div><span style="font-family: inherit">Name:__________________________________________</span></div></div>';
    const blanks = [{ name: "a", length: 42, alternateLabel: "Name" }];
    const rows = parseFibPrompt(prompt, blanks);
    expect(rows.length).toBe(2);
    expect(rows[0].segments.map((s) => s.type)).toEqual(["text"]);
    expect(rows[0].segments[0].text).toMatch(/Add your name/);
    expect(rows[1].segments.map((s) => s.type)).toEqual(["text", "blank"]);
    expect(rows[1].segments[0].text).toMatch(/^Name:\s*$/);
  });
});
