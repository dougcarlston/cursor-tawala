/**
 * Document load: block trees → HTML chips / tables for the WYSIWYG surface.
 */
import { describe, expect, it } from "vitest";
import { documentContentToHtml } from "@/components/DocumentEditor";
import type { RichContentBlock } from "@/types/tawala";

describe("documentContentToHtml", () => {
  it("passes through HTML strings and empty content", () => {
    expect(documentContentToHtml(undefined)).toBe("");
    expect(documentContentToHtml("<p>Hi</p>")).toBe("<p>Hi</p>");
    expect(
      documentContentToHtml([{ type: "paragraph", nodes: [] } as RichContentBlock]),
    ).toBe("");
  });

  it("renders field nodes as non-editable chips", () => {
    const html = documentContentToHtml([
      {
        type: "paragraph",
        nodes: [
          { type: "text", text: "Hello " },
          { type: "field", name: "Form 1:Email" },
        ],
      } as RichContentBlock,
    ]);
    expect(html).toContain('class="field-token function-table-token"');
    expect(html).toContain('data-field-name="Form 1:Email"');
    expect(html).toContain("contenteditable=\"false\"");
    expect(html).toContain("&lt;&lt;Form 1:Email&gt;&gt;");
  });

  it("renders tables as table.user", () => {
    const html = documentContentToHtml([
      {
        type: "table",
        rows: [
          {
            cells: [
              {
                content: [
                  {
                    type: "paragraph",
                    nodes: [{ type: "text", text: "A" }],
                  },
                ],
              },
              {
                content: [
                  {
                    type: "paragraph",
                    nodes: [{ type: "field", name: "Amount" }],
                  },
                ],
              },
            ],
          },
        ],
      } as RichContentBlock,
    ]);
    expect(html).toContain('<table class="user"');
    expect(html).toContain("<td>");
    expect(html).toContain("data-field-name=\"Amount\"");
  });

  it("escapes plain text and applies face/size spans", () => {
    const html = documentContentToHtml([
      {
        type: "paragraph",
        align: "center",
        nodes: [
          {
            type: "font",
            face: "Arial",
            size: 14,
            nodes: [{ type: "text", text: "a < b" }],
          },
        ],
      } as RichContentBlock,
    ]);
    expect(html).toContain('style="text-align:center"');
    expect(html).toContain("font-family:Arial");
    expect(html).toContain("font-size:14pt");
    expect(html).toContain("a &lt; b");
  });
});
