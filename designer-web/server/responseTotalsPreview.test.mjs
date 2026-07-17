import { describe, expect, it } from "vitest";
import { renderResponseTotalsTableHtml } from "./responseTotalsPreview.mjs";

describe("renderResponseTotalsTableHtml", () => {
  const project = {
    forms: [
      {
        name: "Form 1",
        items: [
          {
            type: "mc",
            label: "Q1",
            question: "Where is Greenland?",
            choices: [
              { name: "a", text: "Europe" },
              { name: "b", text: "North America" },
            ],
          },
        ],
      },
    ],
  };

  it("puts MCQ question text above Choice/Count table", () => {
    const html = renderResponseTotalsTableHtml(
      { field: "Record:Form 1:Q1", form: "Form 1" },
      {
        project,
        formName: "Form 1",
        records: { "Form 1": [{ Q1: "a" }, { Q1: "b" }, { Q1: "a" }] },
      },
    );
    expect(html).toContain('<div class="response-totals-title"><strong>Where is Greenland?</strong></div>');
    expect(html).toContain("<th> Where is Greenland? </th>");
    expect(html).toContain("<th> Count </th>");
    expect(html).toContain(">Europe</td><td>2</td>");
    expect(html).toContain(">North America</td><td>1</td>");
  });
});
