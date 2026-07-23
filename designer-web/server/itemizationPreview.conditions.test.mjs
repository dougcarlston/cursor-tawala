import { describe, expect, it } from "vitest";
import {
  countFormRecordsFromConfig,
  normalizeConditionOp,
  rowMatchesConditionsForTest,
} from "./itemizationPreview.mjs";

describe("normalizeConditionOp", () => {
  it("keeps Configure camelCase ids", () => {
    expect(normalizeConditionOp("isNotBlank")).toBe("isNotBlank");
    expect(normalizeConditionOp("doesNotEqual")).toBe("doesNotEqual");
    expect(normalizeConditionOp("equals")).toBe("equals");
  });

  it("maps spaced legacy labels", () => {
    expect(normalizeConditionOp("is not blank")).toBe("isNotBlank");
    expect(normalizeConditionOp("is blank")).toBe("isBlank");
    expect(normalizeConditionOp("does not equal")).toBe("doesNotEqual");
  });
});

describe("rowMatchesConditions Configure op ids", () => {
  it("matches isNotBlank / isBlank", () => {
    expect(
      rowMatchesConditionsForTest(
        { Name: "Ada" },
        [{ field: "Name", op: "isNotBlank", value: "" }],
        "and",
        "Form 1",
      ),
    ).toBe(true);
    expect(
      rowMatchesConditionsForTest(
        { Name: "" },
        [{ field: "Name", op: "isNotBlank", value: "" }],
        "and",
        "Form 1",
      ),
    ).toBe(false);
    expect(
      rowMatchesConditionsForTest(
        { Name: "" },
        [{ field: "Name", op: "isBlank", value: "" }],
        "and",
        "Form 1",
      ),
    ).toBe(true);
  });

  it("matches doesNotEqual and contains", () => {
    expect(
      rowMatchesConditionsForTest(
        { Status: "no" },
        [{ field: "Status", op: "doesNotEqual", value: "ok" }],
        "and",
        "Form 1",
      ),
    ).toBe(true);
    expect(
      rowMatchesConditionsForTest(
        { Note: "hello world" },
        [{ field: "Note", op: "contains", value: "world" }],
        "and",
        "Form 1",
      ),
    ).toBe(true);
  });
});

describe("countFormRecordsFromConfig Where", () => {
  it("filters with isNotBlank on Record:Form:Field", () => {
    const n = countFormRecordsFromConfig(
      {
        "form-name": "Registration",
        conditionsRows: [
          { field: "Record:Registration:WaiverReceived", op: "isNotBlank", value: "" },
        ],
      },
      {
        records: {
          Registration: [{ WaiverReceived: "Yes" }, { WaiverReceived: "" }, {}],
        },
      },
    );
    expect(n).toBe(1);
  });

  it("matches Form 1 FIB1:a equals Bogus when records use POST keys", () => {
    const form = {
      name: "Form 1",
      items: [
        {
          type: "fib",
          label: "FIB1",
          blanks: [{ name: "a", alternateLabel: "FIB1:a" }],
        },
      ],
    };
    const rows = [
      { "FIB1:a": "Bogus", a: "Bogus" },
      { "FIB1:a": "Bogus", a: "Bogus" },
      { "FIB1:a": "Other", a: "Other" },
    ];
    for (const field of ["FIB1:a", "Form 1:FIB1:a", "Record:Form 1:FIB1:a"]) {
      const n = countFormRecordsFromConfig(
        {
          "form-name": "Form 1",
          conditionsRows: [{ field, op: "equals", value: "Bogus" }],
        },
        { records: { "Form 1": rows }, project: { forms: [form] } },
      );
      expect(n, field).toBe(2);
    }
  });
});

describe("formatMcqCellValue", () => {
  it("expands comma-separated multi-select ids to choice labels", async () => {
    const { formatMcqCellValue } = await import("./itemizationPreview.mjs");
    const project = {
      forms: [
        {
          name: "Form 1",
          items: [
            {
              type: "mc",
              label: "MCQ4",
              onlyone: false,
              choices: [
                { label: "a", text: "Blue" },
                { label: "b", text: "Green" },
                { label: "c", text: "Yellow" },
              ],
            },
          ],
        },
      ],
    };
    expect(formatMcqCellValue("a,c", project, "Form 1", "MCQ4")).toBe("Blue, Yellow");
  });
});
describe("renderItemizationTableHtml empty state", () => {
  it("shows No records were found (Deploy parity)", async () => {
    const { renderItemizationTableHtml } = await import("./itemizationPreview.mjs");
    const html = renderItemizationTableHtml(
      {
        type: "itemizationTable",
        columns: [
          { header: "First", field: "<<Form 1:First>>" },
          { header: "Last", field: "<<Form 1:Last>>" },
        ],
      },
      { records: { "Form 1": [] }, project: { forms: [{ name: "Form 1", items: [] }] } },
    );
    expect(html).toContain("No records were found.");
    expect(html).toContain("<th>First</th>");
  });

  it("stays content-sized even with more than 3 columns (no empty right box)", async () => {
    const { renderItemizationTableHtml } = await import("./itemizationPreview.mjs");
    const html = renderItemizationTableHtml(
      {
        type: "itemizationTable",
        columns: [
          { header: "Name", field: "<<Form 1:Name>>" },
          { header: "Adults", field: "<<Form 1:Adults>>" },
          { header: "Kids", field: "<<Form 1:Kids>>" },
          { header: "Dish", field: "<<Form 1:Dish>>" },
        ],
      },
      {
        records: {
          "Form 1": [{ Name: "Ada", Adults: "1", Kids: "", Dish: "Salad" }],
        },
        project: { forms: [{ name: "Form 1", items: [] }] },
      },
    );
    expect(html).toContain('class="preview-itemization-table"');
    expect(html).not.toContain("dtFixTableWidth");
  });
});
