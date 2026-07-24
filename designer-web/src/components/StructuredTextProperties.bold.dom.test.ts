/**
 * @vitest-environment happy-dom
 */
import { describe, expect, it } from "vitest";
import {
  editorHtmlToStructuredContent,
  structuredContentToEditorHtml,
} from "@/components/StructuredTextProperties";
import type { ItemizationNode } from "@/lib/structuredItemizationEdit";

const fallbackTable: ItemizationNode = {
  type: "itemizationTable",
  version: 1,
  form: "Survey",
  columns: [{ header: "Age", field: "Record:Survey:Q4:a" }],
};

function hasBoldText(nodes: { type?: string; text?: string; nodes?: unknown[] }[] = [], text: string): boolean {
  for (const n of nodes) {
    if (n.type === "bold") {
      const flat = JSON.stringify(n.nodes ?? []);
      if (flat.includes(text)) return true;
    }
    if (Array.isArray(n.nodes) && hasBoldText(n.nodes as typeof nodes, text)) return true;
  }
  return false;
}

describe("editorHtmlToStructuredContent B/I/U", () => {
  it("keeps styleWithCSS bold spans (Age: on Report Text)", () => {
    const html = `<p><span style="font-weight: bold;">Age:</span></p>`;
    const blocks = editorHtmlToStructuredContent(html, fallbackTable);
    expect(hasBoldText(blocks[0]?.nodes as never[], "Age:")).toBe(true);
    const roundTrip = structuredContentToEditorHtml(blocks);
    expect(roundTrip).toMatch(/<(strong|b)>Age:<\/(strong|b)>/i);
  });

  it("keeps <b>/<strong> tags", () => {
    const html = `<p><strong>Age:</strong></p>`;
    const blocks = editorHtmlToStructuredContent(html, fallbackTable);
    expect(hasBoldText(blocks[0]?.nodes as never[], "Age:")).toBe(true);
  });

  it("keeps numeric font-weight 700", () => {
    const html = `<p><span style="font-weight:700">Age:</span></p>`;
    const blocks = editorHtmlToStructuredContent(html, fallbackTable);
    expect(hasBoldText(blocks[0]?.nodes as never[], "Age:")).toBe(true);
  });

  it("does not invent bold from font-weight: normal", () => {
    const html = `<p><span style="font-weight: normal;">Age:</span></p>`;
    const blocks = editorHtmlToStructuredContent(html, fallbackTable);
    expect(hasBoldText(blocks[0]?.nodes as never[], "Age:")).toBe(false);
  });
});
