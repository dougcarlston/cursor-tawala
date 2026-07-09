import { useProjectStore } from "@/store/projectStore";
import { RichContentBlock } from "@/types/tawala";
import { RichTextEditor } from "./RichTextEditor";

interface Props {
  documentName: string;
}

export function DocumentEditor({ documentName }: Props) {
  const project = useProjectStore((s) => s.project);
  const updateDocumentContent = useProjectStore((s) => s.updateDocumentContent);
  const doc = project.documents?.find((d) => d.name === documentName);

  if (!doc) {
    return <div className="placeholder-editor">Document not found</div>;
  }

  const html = documentContentToHtml(doc.content);

  return (
    <div className="document-editor">
      <RichTextEditor
        html={html}
        onChange={(nextHtml) => updateDocumentContent(documentName, nextHtml)}
        formattingKind="document"
      />
    </div>
  );
}

/** Load project document content into the WYSIWYG surface (HTML string preferred). */
function documentContentToHtml(content: string | RichContentBlock[] | undefined): string {
  if (content == null) return "";
  if (typeof content === "string") return content;
  if (
    content.length === 1 &&
    content[0]?.type === "paragraph" &&
    !(content[0].nodes?.length ?? 0)
  ) {
    return "";
  }
  return blocksToHtml(content);
}

function nodesToHtml(nodes: { type: string; text?: string; nodes?: unknown[] }[]): string {
  return nodes
    .map((n) => {
      if (n.type === "text") return n.text ?? "";
      if (n.type === "bold") return `<strong>${nodesToHtml((n.nodes as typeof nodes) ?? [])}</strong>`;
      if (n.type === "italic") return `<em>${nodesToHtml((n.nodes as typeof nodes) ?? [])}</em>`;
      if (n.type === "underline") return `<u>${nodesToHtml((n.nodes as typeof nodes) ?? [])}</u>`;
      if (n.type === "field") {
        const name = (n as { name?: string; field?: string }).name ?? (n as { field?: string }).field;
        return name ? `&lt;&lt;${name}&gt;&gt;` : "";
      }
      return "";
    })
    .join("");
}

function blocksToHtml(blocks: RichContentBlock[]): string {
  return blocks
    .map((b) => {
      if (b.type === "paragraph") {
        const align =
          b.align && b.align !== "left" ? ` style="text-align:${escapeAttr(b.align)}"` : "";
        return `<p${align}>${nodesToHtml(b.nodes ?? [])}</p>`;
      }
      if (b.type === "text") return b.text ?? "";
      return "";
    })
    .join("");
}

function escapeAttr(value: string): string {
  return value.replace(/"/g, "&quot;");
}
