import { RichContentBlock, RichTextNode } from "@/types/tawala";
import { RichTextEditor } from "./RichTextEditor";
import {
  STRUCTURED_NODE_DATA_ATTR as STRUCTURED_NODE_ATTR,
  encodeStructuredNode,
  type ItemizationNode,
  type QuestionCorrelationNode,
  type StructuredFunctionNode,
} from "@/lib/structuredItemizationEdit";

interface ItemizationColumn {
  header: string;
  field: string;
}

interface Props {
  content: RichContentBlock[];
  onChange: (content: RichContentBlock[]) => void;
}

interface StructuredLocation {
  block: RichContentBlock;
  blockIndex: number;
  node: StructuredFunctionNode;
  nodeIndex: number;
}

const EDITOR_CARET_SLOT_ATTR = "data-tawala-caret-slot";
const ITEMIZATION_TOKEN_LABEL = "{ MULTIPLE QUESTION LIST }";
const CORRELATION_TOKEN_LABEL = "{ QUESTION CORRELATION TABLE }";
const EDITOR_CARET_GUARD = "\u200b";

function isEditableStructuredType(type: string): type is StructuredFunctionNode["type"] {
  return type === "itemizationTable" || type === "questionCorrelationTable";
}

function findEditableStructuredFunction(content: RichContentBlock[]): StructuredLocation | null {
  for (let blockIndex = 0; blockIndex < content.length; blockIndex += 1) {
    const block = content[blockIndex];
    const nodes = block.nodes ?? [];
    for (let nodeIndex = 0; nodeIndex < nodes.length; nodeIndex += 1) {
      const node = nodes[nodeIndex];
      if (isEditableStructuredType(node.type)) {
        return {
          block,
          blockIndex,
          node: node as StructuredFunctionNode,
          nodeIndex,
        };
      }
    }
  }
  return null;
}

function escHtml(text: string) {
  return text
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function escAttr(text: string) {
  return escHtml(text).replace(/'/g, "&#39;");
}

function decodeStructuredNodeAttr(value: string | null): RichTextNode | null {
  if (!value) return null;
  try {
    return JSON.parse(decodeURIComponent(value)) as RichTextNode;
  } catch {
    return null;
  }
}

function editorCaretSlotHtml() {
  return `<span ${EDITOR_CARET_SLOT_ATTR}="true">${EDITOR_CARET_GUARD}</span>`;
}

function stripEditorCaretGuards(text: string) {
  return text.replace(/\u200b/g, "");
}

function patchItemizationColumns(content: RichContentBlock[], columns: ItemizationColumn[]): RichContentBlock[] {
  return content.map((block) => ({
    ...block,
    nodes: (block.nodes ?? []).map((node) =>
      node.type === "itemizationTable" ? { ...node, columns } : node,
    ),
  }));
}

function isFunctionNode(node: RichTextNode) {
  return (
    node.type === "itemizationTable" ||
    node.type === "choiceTallyTable" ||
    node.type === "questionCorrelationTable"
  );
}

function mergeAdjacentTextNodes(nodes: RichTextNode[]): RichTextNode[] {
  const merged: RichTextNode[] = [];
  for (const node of nodes) {
    if (node.type === "text" && (node.text ?? "") === "") continue;
    const last = merged[merged.length - 1];
    if (node.type === "text" && last?.type === "text") {
      last.text = `${last.text ?? ""}${node.text ?? ""}`;
    } else {
      merged.push(node);
    }
  }
  return merged;
}

function wrapInlineNodes(
  type: RichTextNode["type"],
  nodes: RichTextNode[],
  attrs: Partial<RichTextNode> = {},
): RichTextNode[] {
  const wrapped: RichTextNode[] = [];
  let group: RichTextNode[] = [];

  const flushGroup = () => {
    const children = mergeAdjacentTextNodes(group);
    if (!children.length) return;
    wrapped.push({ type, ...attrs, nodes: children });
    group = [];
  };

  for (const node of nodes) {
    if (isFunctionNode(node)) {
      flushGroup();
      wrapped.push(node);
    } else {
      group.push(node);
    }
  }

  flushGroup();
  return wrapped;
}

function structuredTokenHtml(node: StructuredFunctionNode, label: string, title: string): string {
  return `${editorCaretSlotHtml()}<span contenteditable="false" class="function-table-inline function-table-token" ${STRUCTURED_NODE_ATTR}="${escAttr(encodeStructuredNode(node))}" title="${escAttr(title)}">${escHtml(label)}</span>${editorCaretSlotHtml()}`;
}

function richNodesToEditorHtml(nodes: RichTextNode[] = []): string {
  return nodes
    .map((node) => {
      switch (node.type) {
        case "text":
          return escHtml(node.text ?? "");
        case "bold":
          return `<strong>${richNodesToEditorHtml(node.nodes ?? [])}</strong>`;
        case "italic":
          return `<em>${richNodesToEditorHtml(node.nodes ?? [])}</em>`;
        case "underline":
          return `<u>${richNodesToEditorHtml(node.nodes ?? [])}</u>`;
        case "font": {
          const style: string[] = [];
          if (node.face) style.push(`font-family:${node.face}`);
          if (node.size) style.push(`font-size:${node.size / 20}pt`);
          if (node.color) style.push(`color:#${String(node.color).replace(/^#/, "")}`);
          const faceAttr = node.face ? ` data-tawala-font-face="${escAttr(node.face)}"` : "";
          const sizeAttr = node.size ? ` data-tawala-font-size="${escAttr(String(node.size))}"` : "";
          const colorAttr = node.color
            ? ` data-tawala-font-color="${escAttr(String(node.color))}"`
            : "";
          const styleAttr = style.length ? ` style="${escAttr(style.join(";"))}"` : "";
          return `<span data-tawala-font-node="true"${faceAttr}${sizeAttr}${colorAttr}${styleAttr}>${richNodesToEditorHtml(node.nodes ?? [])}</span>`;
        }
        case "itemizationTable": {
          const n = node as ItemizationNode;
          return `${editorCaretSlotHtml()}<span contenteditable="false" class="function-table-inline function-table-token" data-itemization-token="true" data-itemization-form="${escAttr(n.form ?? "")}" ${STRUCTURED_NODE_ATTR}="${escAttr(encodeStructuredNode(n))}" title="MULTIPLE QUESTION LIST">${escHtml(ITEMIZATION_TOKEN_LABEL)}</span>${editorCaretSlotHtml()}`;
        }
        case "questionCorrelationTable":
          return structuredTokenHtml(
            node as QuestionCorrelationNode,
            CORRELATION_TOKEN_LABEL,
            "QUESTION CORRELATION TABLE",
          );
        default:
          return richNodesToEditorHtml(node.nodes ?? []);
      }
    })
    .join("");
}

function contentToEditorHtml(content: RichContentBlock[]): string {
  const html = content
    .map((block) => {
      if (block.type === "paragraph") {
        const align = block.align ? ` style="text-align:${escAttr(block.align)}"` : "";
        return `<p${align}>${richNodesToEditorHtml(block.nodes ?? [])}</p>`;
      }
      if (block.type === "text") {
        return `<p>${escHtml(block.text ?? "")}</p>`;
      }
      return "";
    })
    .join("");

  return html || "<p></p>";
}

export function structuredContentToEditorHtml(content: RichContentBlock[]): string {
  return contentToEditorHtml(content);
}

/** First editable structured function node in Form Text content, if any. */
export function findStructuredItemizationTable(
  content: RichContentBlock[],
): StructuredFunctionNode | null {
  return findEditableStructuredFunction(content)?.node ?? null;
}

export function findEditableStructuredFunctionNode(
  content: RichContentBlock[],
): StructuredFunctionNode | null {
  return findEditableStructuredFunction(content)?.node ?? null;
}

function parseInlineNodesFromDom(parent: ParentNode): RichTextNode[] {
  const nodes: RichTextNode[] = [];
  parent.childNodes.forEach((child) => {
    nodes.push(...parseInlineNode(child));
  });
  return mergeAdjacentTextNodes(nodes);
}

function parseInlineNode(node: Node): RichTextNode[] {
  if (node.nodeType === Node.TEXT_NODE) {
    const text = stripEditorCaretGuards(node.textContent ?? "");
    return text ? [{ type: "text", text }] : [];
  }

  if (node.nodeType !== Node.ELEMENT_NODE) return [];

  const el = node as HTMLElement;
  const restoredNode = decodeStructuredNodeAttr(el.getAttribute(STRUCTURED_NODE_ATTR));
  if (restoredNode) return [restoredNode];

  const children = parseInlineNodesFromDom(el);
  const tag = el.tagName.toUpperCase();

  switch (tag) {
    case "B":
    case "STRONG":
      return wrapInlineNodes("bold", children);
    case "I":
    case "EM":
      return wrapInlineNodes("italic", children);
    case "U":
      return wrapInlineNodes("underline", children);
    case "SPAN": {
      if (el.dataset.tawalaCaretSlot === "true") {
        return children;
      }
      if (el.dataset.tawalaFontNode === "true") {
        const attrs: Partial<RichTextNode> = {};
        if (el.dataset.tawalaFontFace) attrs.face = el.dataset.tawalaFontFace;
        if (el.dataset.tawalaFontSize) {
          const size = Number(el.dataset.tawalaFontSize);
          if (Number.isFinite(size)) attrs.size = size;
        }
        if (el.dataset.tawalaFontColor) attrs.color = el.dataset.tawalaFontColor;
        return Object.keys(attrs).length ? wrapInlineNodes("font", children, attrs) : children;
      }
      return children;
    }
    case "BR":
      return [];
    default:
      return children;
  }
}

function hasEditableStructuredFunction(blocks: RichContentBlock[]): boolean {
  const visit = (nodes: RichTextNode[] = []): boolean =>
    nodes.some((node) => isEditableStructuredType(node.type) || visit(node.nodes ?? []));

  return blocks.some((block) => visit(block.nodes ?? []));
}

function htmlToStructuredContent(
  html: string,
  fallbackTable: StructuredFunctionNode,
): RichContentBlock[] {
  const container = document.createElement("div");
  container.innerHTML = html;

  const blocks: RichContentBlock[] = [];
  let looseNodes: RichTextNode[] = [];

  const flushLooseNodes = () => {
    const nodes = mergeAdjacentTextNodes(looseNodes);
    if (nodes.length) {
      blocks.push({ type: "paragraph", nodes });
    }
    looseNodes = [];
  };

  container.childNodes.forEach((child) => {
    if (child.nodeType === Node.ELEMENT_NODE) {
      const el = child as HTMLElement;
      const tag = el.tagName.toUpperCase();

      if (tag === "P" || tag === "DIV") {
        flushLooseNodes();
        const nodes = parseInlineNodesFromDom(el);
        if (nodes.length) {
          blocks.push({
            type: "paragraph",
            align: el.style.textAlign || undefined,
            nodes,
          });
        }
        return;
      }

      if (tag === "BR") {
        flushLooseNodes();
        return;
      }
    }

    looseNodes.push(...parseInlineNode(child));
  });

  flushLooseNodes();

  if (!hasEditableStructuredFunction(blocks)) {
    blocks.push({ type: "paragraph", nodes: [fallbackTable] });
  }

  return blocks.length ? blocks : [{ type: "paragraph", nodes: [fallbackTable] }];
}

export function editorHtmlToStructuredContent(
  html: string,
  fallbackTable: StructuredFunctionNode,
): RichContentBlock[] {
  return htmlToStructuredContent(html, fallbackTable);
}

/** Properties for text items that embed function tables (not plain rich text). */
export function StructuredTextProperties({ content, onChange }: Props) {
  const location = findEditableStructuredFunction(content);
  const table = location?.node;

  if (table?.type === "itemizationTable") {
    const columns = table.columns ?? [];
    return (
      <div className="structured-text-props">
        <p className="hint">
          This normal text item includes the legacy <strong>MULTIPLE QUESTION LIST</strong> function.
          Edit the text on the canvas; click the boxed token to Configure the table.
        </p>
        <label>
          Content
          <RichTextEditor
            html={contentToEditorHtml(content)}
            onChange={(html) => onChange(htmlToStructuredContent(html, table))}
            placeholder="Enter text…"
            formattingKind="text"
          />
        </label>
        {columns.map((col, i) => (
          <label key={`${col.field}-${i}`}>
            Column {i + 1} header
            <input
              value={col.header}
              onChange={(e) => {
                const next = columns.map((c, j) =>
                  j === i ? { ...c, header: e.target.value } : c,
                );
                onChange(patchItemizationColumns(content, next));
              }}
            />
            <span className="hint">Field: {col.field}</span>
          </label>
        ))}
      </div>
    );
  }

  if (table?.type === "questionCorrelationTable") {
    return (
      <div className="structured-text-props">
        <p className="hint">
          This normal text item includes the legacy <strong>QUESTION CORRELATION TABLE</strong>{" "}
          function. Edit surrounding text on the canvas; click the boxed token to Configure the
          table fields.
        </p>
        <label>
          Content
          <RichTextEditor
            html={contentToEditorHtml(content)}
            onChange={(html) => onChange(htmlToStructuredContent(html, table))}
            placeholder="Enter text…"
            formattingKind="text"
          />
        </label>
      </div>
    );
  }

  return (
    <p className="hint">
      This text item contains structured content (functions/tables). Edit the project JSON to
      change it, or replace the item from a fresh template.
    </p>
  );
}

export function hasStructuredTextContent(
  content: string | RichContentBlock[] | undefined,
): content is RichContentBlock[] {
  return Array.isArray(content);
}

export function itemizationPreviewLabel(content: RichContentBlock[]): string | null {
  const node = findEditableStructuredFunction(content)?.node;
  if (!node) return null;
  if (node.type === "questionCorrelationTable") return "QUESTION CORRELATION TABLE";
  if (!node.columns?.length) return "MULTIPLE QUESTION LIST";
  return `MULTIPLE QUESTION LIST (${node.columns.map((c) => c.header).join(", ")})`;
}
