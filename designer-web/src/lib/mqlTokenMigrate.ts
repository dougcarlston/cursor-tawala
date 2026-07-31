/**
 * Normalize MULTIPLE QUESTION LIST chips to the present-day function-token path
 * (`<<MULTIPLE QUESTION LIST(...)>>` + data-function-config / conditionsRows).
 *
 * Legacy brace chips (`{ MULTIPLE QUESTION LIST }` + data-tawala-structured-node)
 * and structured Form Text arrays must upgrade on open / Configure so Redeploy
 * emits Where from the same path as Insert → Function.
 */

import type { FormItem, RichContentBlock, RichTextNode, TawalaProject } from "@/types/tawala";
import {
  STRUCTURED_NODE_DATA_ATTR,
  functionConfigToItemizationNode,
  itemizationFunctionTokenHtml,
  itemizationToConfig,
  type ItemizationNode,
} from "./structuredItemizationEdit";

const STRUCTURED_NODE_RE = new RegExp(
  `<span\\b([^>]*\\b${STRUCTURED_NODE_DATA_ATTR}=["']([^"']+)["'][^>]*)>([\\s\\S]*?)<\\/span>`,
  "gi",
);

function decodeStructuredNode(raw: string): ItemizationNode | null {
  try {
    const parsed = JSON.parse(decodeURIComponent(raw)) as ItemizationNode;
    if (parsed?.type === "itemizationTable") return parsed;
  } catch {
    try {
      const parsed = JSON.parse(raw) as ItemizationNode;
      if (parsed?.type === "itemizationTable") return parsed;
    } catch {
      /* ignore */
    }
  }
  return null;
}

function isLegacyMqlSpanAttrs(attrs: string): boolean {
  if (/\bdata-function-id\s*=\s*["']itemization-table["']/i.test(attrs)) return false;
  if (/\bdata-itemization-token\s*=\s*["']true["']/i.test(attrs)) return true;
  if (new RegExp(`\\b${STRUCTURED_NODE_DATA_ATTR}\\s*=`, "i").test(attrs)) {
    const m = attrs.match(
      new RegExp(`${STRUCTURED_NODE_DATA_ATTR}\\s*=\\s*["']([^"']+)["']`, "i"),
    );
    return Boolean(m && decodeStructuredNode(m[1]));
  }
  return false;
}

/** Upgrade legacy brace / structured-node MQL spans in HTML to modern function-tokens. */
export function upgradeLegacyMqlTokensInHtml(html: string): string {
  if (!html || !html.includes(STRUCTURED_NODE_DATA_ATTR)) return html;
  return html.replace(STRUCTURED_NODE_RE, (full, attrs: string, encoded: string) => {
    if (!isLegacyMqlSpanAttrs(attrs)) return full;
    const node = decodeStructuredNode(encoded);
    if (!node) return full;
    return itemizationFunctionTokenHtml(node);
  });
}

function visitNodes(
  nodes: RichTextNode[] | undefined,
  visit: (node: RichTextNode) => void,
): void {
  if (!Array.isArray(nodes)) return;
  for (const n of nodes) {
    visit(n);
    if (n.nodes) visitNodes(n.nodes, visit);
  }
}

function structuredBlocksHaveItemization(blocks: RichContentBlock[]): boolean {
  let found = false;
  for (const b of blocks) {
    visitNodes(b.nodes, (node) => {
      if (node.type === "itemizationTable") found = true;
    });
    if (found) break;
  }
  return found;
}

function escHtml(s: string): string {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function richNodesToHtml(nodes: RichTextNode[] | undefined): string {
  if (!nodes?.length) return "";
  return nodes
    .map((node) => {
      switch (node.type) {
        case "text":
          return escHtml(node.text ?? "");
        case "bold":
          return `<b>${richNodesToHtml(node.nodes)}</b>`;
        case "italic":
          return `<i>${richNodesToHtml(node.nodes)}</i>`;
        case "underline":
          return `<u>${richNodesToHtml(node.nodes)}</u>`;
        case "itemizationTable": {
          const cfg = itemizationToConfig(node as ItemizationNode);
          return itemizationFunctionTokenHtml(functionConfigToItemizationNode(cfg));
        }
        case "font": {
          const style: string[] = [];
          if (node.face) style.push(`font-family:${node.face}`);
          if (node.size) style.push(`font-size:${Number(node.size) / 20}pt`);
          if (node.color) style.push(`color:#${String(node.color).replace(/^#/, "")}`);
          const styleAttr = style.length ? ` style="${escHtml(style.join(";"))}"` : "";
          return `<span${styleAttr}>${richNodesToHtml(node.nodes)}</span>`;
        }
        default:
          return richNodesToHtml(node.nodes);
      }
    })
    .join("");
}

function structuredBlocksToHtml(blocks: RichContentBlock[]): string {
  const html = blocks
    .map((block) => {
      if (block.type === "paragraph") {
        const align = block.align ? ` style="text-align:${escHtml(block.align)}"` : "";
        return `<p${align}>${richNodesToHtml(block.nodes) || "<br>"}</p>`;
      }
      if (block.type === "text") return `<p>${escHtml(block.text ?? "")}</p>`;
      return "";
    })
    .join("");
  return html || "<p></p>";
}

/**
 * On Open / New Project template load: upgrade MQL to modern HTML function-tokens
 * so Configure → conditionsRows → Deploy always share one path.
 */
export function migrateProjectMqlTokens(project: TawalaProject): TawalaProject {
  const forms = (project.forms ?? []).map((form) => ({
    ...form,
    items: (form.items ?? []).map((item) => migrateTextItem(item)),
  }));
  const documents = (project.documents ?? []).map((doc) => {
    if (typeof doc.content !== "string") return doc;
    const next = upgradeLegacyMqlTokensInHtml(doc.content);
    return next === doc.content ? doc : { ...doc, content: next };
  });
  return { ...project, forms, documents };
}

function migrateTextItem(item: FormItem): FormItem {
  if (item.type !== "text") return item;
  if (Array.isArray(item.content) && structuredBlocksHaveItemization(item.content)) {
    return { ...item, content: structuredBlocksToHtml(item.content) };
  }
  if (typeof item.content === "string") {
    const next = upgradeLegacyMqlTokensInHtml(item.content);
    return next === item.content ? item : { ...item, content: next };
  }
  return item;
}

/**
 * After Configure on an MQL chip: drop sibling legacy brace MQL spans in the same
 * editor so Redeploy cannot keep showing the unfiltered old table.
 */
export function removeSiblingLegacyMqlTokens(editor: HTMLElement, keep: HTMLElement): void {
  editor.querySelectorAll(`[${STRUCTURED_NODE_DATA_ATTR}]`).forEach((node) => {
    if (!(node instanceof HTMLElement) || node === keep) return;
    if (
      node.classList.contains("function-token") &&
      node.getAttribute("data-function-id") === "itemization-table"
    ) {
      return;
    }
    const encoded = node.getAttribute(STRUCTURED_NODE_DATA_ATTR);
    if (!encoded || !decodeStructuredNode(encoded)) return;
    node.remove();
  });
  editor.querySelectorAll(".function-table-inline.function-table-token").forEach((node) => {
    if (!(node instanceof HTMLElement) || node === keep) return;
    if (node.classList.contains("function-token")) return;
    const label = (node.textContent ?? "").replace(/\s+/g, " ").trim();
    if (label === "{ MULTIPLE QUESTION LIST }" || label === "MULTIPLE QUESTION LIST") {
      node.remove();
    }
  });
}
