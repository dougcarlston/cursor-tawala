import { describe, expect, it } from "vitest";
import { enhanceRichTextHtml } from "./richHtmlPreview.mjs";

describe("enhanceRichTextHtml itemization", () => {
  it("replaces itemization-table function span with HTML table from records", () => {
    const config = JSON.stringify({
      numberOfColumns: 2,
      column: [
        { header: "First", contents: "FirstName" },
        { header: "Last", contents: "LastName" },
      ],
      "form-name": "Form 1",
      conditionsRows: [{ field: "FirstName", op: "equals", value: "Doug" }],
    });
    const html =
      `<p>Signups</p>` +
      `<span class="function-token" data-function-id="itemization-table" ` +
      `data-function-config='${config}'>` +
      `<<MULTIPLE QUESTION LIST(2, FirstName, FirstName equals "Doug")>>` +
      `</span>`;

    const out = enhanceRichTextHtml(html, () => "", {
      records: { "Form 1": [{ FirstName: "Doug", LastName: "C" }] },
      formName: "Form 1",
    });

    expect(out).toContain("<table");
    expect(out).toContain("<th>First</th>");
    expect(out).toContain("<th>Last</th>");
    expect(out).toContain("Doug");
    expect(out).toContain("C");
    expect(out).not.toContain("MULTIPLE QUESTION LIST");
    expect(out).not.toContain('equals "Doug")>>');
    expect(out).not.toContain("function-token");
  });

  it("does not leave condition scrap for bare MULTIPLE QUESTION LIST <<>>", () => {
    const html = `Before <<MULTIPLE QUESTION LIST(2, <<FirstName>>, FirstName equals "Doug")>> after`;
    const out = enhanceRichTextHtml(html, (ref) => (ref === "FirstName" ? "Doug" : ""), {
      records: {},
      formName: "Form 1",
    });
    expect(out).not.toContain('equals "Doug")>>');
    expect(out).not.toContain("MULTIPLE QUESTION LIST");
    // Nested field may resolve; function remnant must not leak.
    expect(out).not.toMatch(/\)>>/);
  });

  it("returns empty for ITEMIZATION/MULTIPLE QUESTION LIST template keys", () => {
    const out = enhanceRichTextHtml("<<ITEMIZATION>>", () => "SHOULD_NOT_APPEAR");
    expect(out).toBe("");
    expect(out).not.toContain("SHOULD_NOT_APPEAR");
  });

  it("parses data-function-config when column contents contain <<Field>> (>)", () => {
    const config = JSON.stringify({
      numberOfColumns: 1,
      column: [{ header: "First Name", contents: "<<FirstName>>" }],
      "form-name": "Form 1",
    });
    const encoded = config.replace(/&/g, "&amp;").replace(/"/g, "&quot;");
    const html =
      `<span class="function-token" data-function-id="itemization-table" ` +
      `data-function-config="${encoded}">` +
      `<<MULTIPLE QUESTION LIST(1, <<FirstName>>)>>` +
      `</span>`;
    const out = enhanceRichTextHtml(html, () => "", {
      records: { "Form 1": [{ FirstName: "Doug" }] },
      formName: "Form 1",
    });
    expect(out).toContain("<table");
    expect(out).toContain("Doug");
    expect(out).not.toContain("no columns configured");
  });

  it("shows Print This List / Export to Excel when Configure toggles are yes", () => {
    const config = JSON.stringify({
      numberOfColumns: 1,
      column: [{ header: "First", contents: "FirstName" }],
      "form-name": "Form 1",
      "show-print-control": "true",
      "show-export-control": "true",
    });
    const html =
      `<span class="function-token" data-function-id="itemization-table" ` +
      `data-function-config='${config}'>fx</span>`;
    const out = enhanceRichTextHtml(html, () => "", {
      records: { "Form 1": [{ FirstName: "Ada" }] },
      formName: "Form 1",
    });
    expect(out).toContain("Print This List");
    expect(out).toContain("Export to Excel");
    expect(out).toContain("Ada");
  });

  it("shows print/export from structured Document MQL node", () => {
    const node = encodeURIComponent(
      JSON.stringify({
        type: "itemizationTable",
        form: "Form 1",
        columns: [{ header: "First", field: "FirstName" }],
        showPrint: true,
        showExport: true,
      }),
    );
    const html =
      `<span class="function-table-token" data-itemization-token="true" ` +
      `data-tawala-structured-node="${node}">fx</span>`;
    const out = enhanceRichTextHtml(html, () => "", {
      records: { "Form 1": [{ FirstName: "Ada" }] },
      formName: "Form 1",
    });
    expect(out).toContain("Print This List");
    expect(out).toContain("Export to Excel");
  });
});
