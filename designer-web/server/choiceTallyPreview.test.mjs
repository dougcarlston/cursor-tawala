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

  it("shows zero counts when MCQ has choices but no responses yet", () => {
    const html = renderChoiceTallyTableHtml(
      { field: "Survey:Q1", form: "Survey" },
      { project, records: { Survey: [] } },
    );
    expect(html).toContain("<table");
    expect(html).toContain("Yes");
    expect(html).toContain(">0<");
  });
});

describe("findMcqItem", () => {
  it("finds MCQ by label", () => {
    expect(findMcqItem(project, "Survey", "Q1")?.label).toBe("Q1");
  });
});
