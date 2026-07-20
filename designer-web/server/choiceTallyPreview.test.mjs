import { describe, expect, it } from "vitest";
import {
  findMcqItem,
  renderChoiceTallyTableHtml,
} from "./choiceTallyPreview.mjs";

const project = {
  forms: [
    {
      name: "Survey",
      items: [
        {
          type: "mc",
          label: "Q1",
          choices: [
            { name: "a", text: "Yes" },
            { name: "b", text: "No" },
            { name: "c", text: "Maybe" },
          ],
        },
      ],
    },
  ],
};

describe("renderChoiceTallyTableHtml", () => {
  it("renders choice / count / percentage rows from records", () => {
    const html = renderChoiceTallyTableHtml(
      { field: "Record:Survey:Q1", form: "Survey" },
      {
        project,
        records: {
          Survey: [{ Q1: "a" }, { Q1: "a" }, { Q1: "b" }],
        },
      },
    );
    expect(html).toContain("<table");
    expect(html).toContain("Yes");
    expect(html).toContain("No");
    expect(html).toContain(">2<");
    expect(html).toContain("67%");
    expect(html).toContain("33%");
    expect(html).toContain('class="bar"');
  });

  it("counts every selected choice on multi-select (array values)", () => {
    const html = renderChoiceTallyTableHtml(
      { field: "Record:Survey:Q1", form: "Survey" },
      {
        project,
        records: {
          Survey: [{ Q1: ["a", "b"] }, { Q1: ["b", "c"] }],
        },
      },
    );
    // Four selections total: a=1 (25%), b=2 (50%), c=1 (25%)
    expect(html).toMatch(/>1</);
    expect(html).toContain("25%");
    expect(html).toContain("50%");
  });
});

describe("findMcqItem", () => {
  it("finds MCQ by label", () => {
    expect(findMcqItem(project, "Survey", "Q1")?.label).toBe("Q1");
  });
});
