import { useProjectStore } from "@/store/projectStore";
import { RichContentBlock } from "@/types/tawala";
import { decoratePreservedWarningChipsHtml } from "@/lib/preservedImportGaps";
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
export function documentContentToHtml(content: string | RichContentBlock[] | undefined): string {
  if (content == null) return "";
  if (typeof content === "string") return decoratePreservedWarningChipsHtml(content);
  if (
    content.length === 1 &&
    content[0]?.type === "paragraph" &&
    !(content[0].nodes?.length ?? 0)
  ) {
    return "";
  }
  return decoratePreservedWarningChipsHtml(blocksToHtml(content));
}

type RichNode = {
  type: string;
  text?: string;
  name?: string;
  field?: string;
  face?: string;
  size?: number;
  color?: string;
  id?: string;
  width?: number;
  height?: number;
  nodes?: RichNode[];
  form?: string;
  rows?: { cells: { width?: number; content?: RichContentBlock[] }[] }[];
};

function nodesToHtml(nodes: RichNode[] | undefined): string {
  return (nodes ?? [])
    .map((n) => {
      if (n.type === "text") return escapeHtml(n.text ?? "");
      if (n.type === "bold") return `<b>${nodesToHtml(n.nodes)}</b>`;
      if (n.type === "italic") return `<i>${nodesToHtml(n.nodes)}</i>`;
      if (n.type === "underline") return `<u>${nodesToHtml(n.nodes)}</u>`;
      if (n.type === "font") {
        const styles: string[] = [];
        if (n.face) styles.push(`font-family:${n.face}`);
        if (n.size != null) styles.push(`font-size:${n.size}pt`);
        if (n.color) styles.push(`color:${n.color}`);
        const styleAttr = styles.length ? ` style="${escapeAttr(styles.join(";"))}"` : "";
        return `<span${styleAttr}>${nodesToHtml(n.nodes)}</span>`;
      }
      if (n.type === "field") {
        const name = n.name ?? n.field;
        if (!name) return "";
        return (
          `<span class="field-token function-table-token" contenteditable="false" ` +
          `data-field-name="${escapeAttr(name)}" title="${escapeAttr(name)}" draggable="true">` +
          `&lt;&lt;${escapeHtml(name)}&gt;&gt;</span>`
        );
      }
      if (n.type === "image" && n.id) {
        return (
          `<img class="tawala-embedded-image" data-tawala-image-id="${escapeAttr(n.id)}" alt="" />`
        );
      }
      if (n.nodes) return nodesToHtml(n.nodes);
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
        return `<p${align}>${nodesToHtml((b.nodes as RichNode[] | undefined) ?? [])}</p>`;
      }
      if (b.type === "text") return b.text ?? "";
      if (b.type === "table" && Array.isArray(b.rows)) {
        const rows = b.rows
          .map((row) => {
            const cells = (row.cells ?? [])
              .map((cell) => {
                const inner = blocksToHtml((cell.content as RichContentBlock[]) ?? []);
                return `<td>${inner || "<br>"}</td>`;
              })
              .join("");
            return `<tr>${cells}</tr>`;
          })
          .join("");
        return `<table class="user" border="1" cellpadding="4" cellspacing="0">${rows}</table>`;
      }
      return "";
    })
    .join("");
}

function escapeAttr(value: string): string {
  return value.replace(/&/g, "&amp;").replace(/"/g, "&quot;").replace(/</g, "&lt;");
}

function escapeHtml(value: string): string {
  return value.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
}
