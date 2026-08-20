import { describe, expect, it } from "vitest";
import {
  contentHasColumnDisplayCondition,
  formHasPreservedGaps,
  formItemHasPreservedGap,
  summarizePreservedGaps,
} from "./preservedImportGaps";
import type { TawalaForm, TawalaProject } from "@/types/tawala";

describe("preservedImportGaps", () => {
  it("does not flag editable item-level displayCondition as a preserved gap", () => {
    const item = {
      type: "fib" as const,
      label: "Q1",
      displayCondition: { field: "X", op: "equals", value: "1" },
    };
    expect(formItemHasPreservedGap(item)).toBe(false);
  });

  it("flags nested itemization column displayCondition", () => {
    const content = [
      {
        type: "paragraph",
        nodes: [
          {
            type: "font",
            nodes: [
                {
                  type: "itemizationTable",
                  form: "Data",
                  columns: [
                    { header: "A", field: "Record:Data:A" },
                    {
                      header: "B",
                      field: "Record:Data:B",
                      displayCondition: { field: "Show", op: "equals", value: "Yes" },
                    },
                  ],
                } as never,
            ],
          },
        ],
      },
    ];
    expect(contentHasColumnDisplayCondition(content)).toBe(true);
    expect(
      formItemHasPreservedGap({ type: "text", label: "T1", content }),
    ).toBe(true);
  });

  it("flags Form Text HTML string content with preserved-import-warning", () => {
    expect(
      formItemHasPreservedGap({
        type: "text",
        label: "T1",
        content:
          '<span class="function-token preserved-import-warning" data-function-config="%22displayCondition%22"></span>',
      }),
    ).toBe(true);
  });

  it("flags document HTML that embeds displayCondition", () => {
    expect(
      contentHasColumnDisplayCondition(
        `<span data-tawala-structured-node="${encodeURIComponent(
          JSON.stringify({
            type: "itemizationTable",
            columns: [{ header: "X", field: "F", displayCondition: { op: "equals" } }],
          }),
        )}"></span>`,
      ),
    ).toBe(true);
  });

  it("summarizes project markers — item DC informational; column DC are gaps", () => {
    const form: TawalaForm = {
      name: "F",
      items: [
        {
          type: "mc",
          label: "Q1",
          displayCondition: { field: "A", op: "equals", value: "1" },
        },
        {
          type: "text",
          label: "T1",
          content: [
            {
              type: "paragraph",
              nodes: [
                {
                  type: "itemizationTable",
                  columns: [
                    {
                      header: "C",
                      field: "f",
                      displayCondition: { field: "Z", op: "equals", value: "1" },
                    },
                  ],
                } as never,
              ],
            },
          ],
        },
      ],
    };
    const project: TawalaProject = {
      name: "P",
      format: "2.0",
      forms: [form],
      documents: [{ name: "D", content: '..."displayCondition":{}...' }],
    };
    const s = summarizePreservedGaps(project);
    expect(s.itemDisplayConditions).toBe(1);
    expect(s.columnDisplayConditions).toBe(2);
    expect(s.formsAffected).toBe(1);
    expect(s.documentsAffected).toBe(1);
    expect(s.totalMarkers).toBe(2);
    expect(formHasPreservedGaps(form)).toBe(true);
    expect(
      formHasPreservedGaps({
        name: "OnlyItems",
        items: [
          {
            type: "fib",
            label: "Q2",
            displayCondition: { field: "A", op: "equals", value: "1" },
          },
        ],
      }),
    ).toBe(false);
  });
});
