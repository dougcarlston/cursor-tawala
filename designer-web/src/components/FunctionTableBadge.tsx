import { CSSProperties, Fragment, ReactNode } from "react";
import { RichContentBlock, RichTextNode } from "@/types/tawala";

interface ItemizationNode extends RichTextNode {
  columns?: { header: string }[];
}

const ITEMIZATION_TOKEN_LABEL = "MULTIPLE QUESTION LIST";

function findItemizationTable(content: RichContentBlock[]): ItemizationNode | null {
  for (const block of content) {
    for (const node of block.nodes ?? []) {
      if (node.type === "itemizationTable") return node as ItemizationNode;
    }
  }
  return null;
}

function alignmentStyle(align?: string): CSSProperties | undefined {
  return align ? { textAlign: align as CSSProperties["textAlign"] } : undefined;
}

function isFunctionNode(node: RichTextNode) {
  return (
    node.type === "itemizationTable" ||
    node.type === "choiceTallyTable" ||
    node.type === "questionCorrelationTable"
  );
}

function renderFunctionBlock(node: RichTextNode, key: string, style?: CSSProperties) {
  if (node.type === "itemizationTable") {
    return (
      <div key={key} className="function-table-badge function-table-standalone" style={style}>
        <span className="function-table-label function-table-token">{ITEMIZATION_TOKEN_LABEL}</span>
      </div>
    );
  }

  return (
    <div key={key} className="function-table-badge" style={style}>
      Function table
    </div>
  );
}

function renderInlineNodes(nodes: RichTextNode[] = []): ReactNode[] {
  return nodes.map((node, index) => {
    const key = `${node.type}-${index}`;
    switch (node.type) {
      case "text":
        return <Fragment key={key}>{node.text ?? ""}</Fragment>;
      case "bold":
        return <strong key={key}>{renderInlineNodes(node.nodes ?? [])}</strong>;
      case "italic":
        return <em key={key}>{renderInlineNodes(node.nodes ?? [])}</em>;
      case "underline":
        return <u key={key}>{renderInlineNodes(node.nodes ?? [])}</u>;
      case "font": {
        const style: CSSProperties = {};
        if (node.face) style.fontFamily = node.face;
        if (node.size) style.fontSize = `${node.size / 20}pt`;
        if (node.color) style.color = `#${String(node.color).replace(/^#/, "")}`;
        return (
          <span key={key} style={style}>
            {renderInlineNodes(node.nodes ?? [])}
          </span>
        );
      }
      case "itemizationTable":
        return (
          <span key={key} className="function-table-inline function-table-token">
            {ITEMIZATION_TOKEN_LABEL}
          </span>
        );
      default:
        return <Fragment key={key}>{renderInlineNodes(node.nodes ?? [])}</Fragment>;
    }
  });
}

/** Design-canvas badge for embedded function tables (legacy Designer naming). */
export function FunctionTableBadge({ content }: { content: RichContentBlock[] }) {
  const table = findItemizationTable(content);
  if (table?.type !== "itemizationTable") {
    return <div className="function-table-badge">Function table</div>;
  }

  return (
    <div className="function-table-preview">
      {content.map((block, blockIndex) => {
        if (block.type === "paragraph") {
          const nodes = block.nodes ?? [];
          const style = alignmentStyle(block.align);
          if (nodes.some(isFunctionNode)) {
            let segmentIndex = 0;
            const pieces: ReactNode[] = [];
            let inlineNodes: RichTextNode[] = [];

            const flushInlineNodes = () => {
              if (!inlineNodes.length) return;
              pieces.push(
                <p
                  key={`${blockIndex}-text-${segmentIndex}`}
                  className="text-block structured-text-preview"
                  style={style}
                >
                  {renderInlineNodes(inlineNodes)}
                </p>,
              );
              inlineNodes = [];
              segmentIndex += 1;
            };

            for (const node of nodes) {
              if (isFunctionNode(node)) {
                flushInlineNodes();
                pieces.push(renderFunctionBlock(node, `${blockIndex}-fn-${segmentIndex}`, style));
                segmentIndex += 1;
              } else {
                inlineNodes.push(node);
              }
            }

            flushInlineNodes();
            return <Fragment key={blockIndex}>{pieces}</Fragment>;
          }

          return (
            <p key={blockIndex} className="text-block structured-text-preview" style={style}>
              {renderInlineNodes(nodes)}
            </p>
          );
        }

        if (block.type === "text") {
          return (
            <p key={blockIndex} className="text-block structured-text-preview">
              {block.text ?? ""}
            </p>
          );
        }

        return null;
      })}
    </div>
  );
}
