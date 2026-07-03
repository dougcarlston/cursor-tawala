import { RichContentBlock, RichTextNode } from "@/types/tawala";

interface ItemizationNode extends RichTextNode {
  columns?: { header: string }[];
}

function findItemizationTable(content: RichContentBlock[]): ItemizationNode | null {
  for (const block of content) {
    for (const node of block.nodes ?? []) {
      if (node.type === "itemizationTable") return node as ItemizationNode;
    }
  }
  return null;
}

/** Design-canvas badge for embedded function tables (legacy Designer naming). */
export function FunctionTableBadge({ content }: { content: RichContentBlock[] }) {
  const table = findItemizationTable(content);
  if (table?.type === "itemizationTable") {
    const cols = (table.columns as { header: string }[] | undefined) ?? [];
    return (
      <div className="function-table-badge">
        <strong>MULTIPLE QUESTION LIST</strong>
        {cols.length > 0 ? (
          <span className="function-table-badge-cols">
            {cols.map((c) => c.header).join(" · ")}
          </span>
        ) : null}
      </div>
    );
  }
  return <div className="function-table-badge">Function table</div>;
}
