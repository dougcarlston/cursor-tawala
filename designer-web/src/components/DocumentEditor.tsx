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

  const isSimpleString = typeof doc.content === "string";
  const firstParagraph =
    !isSimpleString && Array.isArray(doc.content)
      ? doc.content.find((b) => b.type === "paragraph")
      : undefined;

  const simpleHtml = firstParagraph
    ? nodesToHtml(firstParagraph.nodes ?? [])
    : typeof doc.content === "string"
      ? doc.content
      : "";

  const onSimpleEdit = (html: string) => {
    const block: RichContentBlock = {
      type: "paragraph",
      align: "left",
      nodes: [{ type: "text", text: stripTags(html) }],
    };
    updateDocumentContent(documentName, [block]);
  };

  return (
    <div className="form-editor document-editor">
      <div className="panel-title" style={{ borderBottom: "1px solid #aca899" }}>
        Document: {doc.name}
      </div>
      <div className="document-layout">
        <div className="document-edit">
          {isSimpleString || (Array.isArray(doc.content) && doc.content.length <= 3) ? (
            <>
              <p className="hint">
                Edit document body (mail-merge fields: use «FormName:Field» in text for now).
              </p>
              <RichTextEditor html={simpleHtml} onChange={onSimpleEdit} placeholder="Document text…" />
            </>
          ) : (
            <>
              <p className="hint">
                This document has rich structure (tables, fonts). Edit as JSON or simplify in a
                future pass.
              </p>
              <textarea
                className="code-area"
                rows={20}
                value={JSON.stringify(doc.content, null, 2)}
                onChange={(e) => {
                  try {
                    updateDocumentContent(documentName, JSON.parse(e.target.value));
                  } catch {
                    /* ignore */
                  }
                }}
              />
            </>
          )}
        </div>
        <div className="document-preview">
          <div className="panel-title">Preview</div>
          <div
            className="preview-frame"
            dangerouslySetInnerHTML={{
              __html: isSimpleString
                ? `<p>${doc.content}</p>`
                : Array.isArray(doc.content)
                  ? blocksToHtml(doc.content)
                  : "",
            }}
          />
        </div>
      </div>
    </div>
  );
}

function stripTags(html: string) {
  const d = document.createElement("div");
  d.innerHTML = html;
  return d.textContent ?? "";
}

function nodesToHtml(nodes: { type: string; text?: string; nodes?: unknown[] }[]): string {
  return nodes
    .map((n) => {
      if (n.type === "text") return n.text ?? "";
      if (n.type === "bold") return `<strong>${nodesToHtml((n.nodes as typeof nodes) ?? [])}</strong>`;
      return "";
    })
    .join("");
}

function blocksToHtml(blocks: RichContentBlock[]): string {
  return blocks
    .map((b) => {
      if (b.type === "paragraph") return `<p>${nodesToHtml(b.nodes ?? [])}</p>`;
      if (b.type === "text") return b.text ?? "";
      return "";
    })
    .join("");
}
