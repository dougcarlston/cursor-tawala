import { describe, expect, it } from "vitest";
import { parseFibPrompt, segmentsFromUnderscorePrompt } from "./fibPrompt.mjs";

describe("FIB underscore → blanks (Preview / fibPrompt)", () => {
  const blanks = [
    { name: "a", length: 8, alternateLabel: "FIB1:a" },
    { name: "b", length: 6, alternateLabel: "FIB1:b" },
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
});
