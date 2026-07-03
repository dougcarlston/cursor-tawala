import { RichContentBlock, RichTextNode } from "@/types/tawala";

interface ItemizationColumn {
  header: string;
  field: string;
}

interface ItemizationNode extends RichTextNode {
  type: "itemizationTable";
  form?: string;
  columns?: ItemizationColumn[];
}

interface Props {
  content: RichContentBlock[];
  onChange: (content: RichContentBlock[]) => void;
}

function findItemizationTable(content: RichContentBlock[]): ItemizationNode | null {
  for (const block of content) {
    for (const node of block.nodes ?? []) {
      if (node.type === "itemizationTable") return node as ItemizationNode;
    }
  }
  return null;
}

function patchItemizationColumns(
  content: RichContentBlock[],
  columns: { header: string; field: string }[],
): RichContentBlock[] {
  return content.map((block) => ({
    ...block,
    nodes: (block.nodes ?? []).map((node) =>
      node.type === "itemizationTable" ? { ...node, columns } : node,
    ),
  }));
}

/** Properties for text items that embed function tables (not plain rich text). */
export function StructuredTextProperties({ content, onChange }: Props) {
  const table = findItemizationTable(content);

  if (table?.type === "itemizationTable") {
    const columns = table.columns ?? [];
    return (
      <div className="structured-text-props">
        <p className="hint">
          <strong>MULTIPLE QUESTION LIST</strong> — lists prior submissions from this form (legacy
          Functions menu name). Edit column headers below; field bindings stay fixed in the template.
        </p>
        {columns.map((col, i) => (
          <label key={col.field}>
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
  const node = findItemizationTable(content);
  if (!node?.columns?.length) return "MULTIPLE QUESTION LIST";
  return `MULTIPLE QUESTION LIST (${node.columns.map((c) => c.header).join(", ")})`;
}
