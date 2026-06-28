import { useProjectStore } from "@/store/projectStore";
import { FormItem, RichContentBlock } from "@/types/tawala";
import { FormItemsPalette } from "./FormItemsPalette";
import { RichTextEditor } from "./RichTextEditor";
import { FormItemProperties } from "./FormItemProperties";

interface Props {
  formName: string;
}

export function FormEditor({ formName }: Props) {
  const project = useProjectStore((s) => s.project);
  const editorTab = useProjectStore((s) => s.editorTab);
  const setEditorTab = useProjectStore((s) => s.setEditorTab);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const deleteFormItem = useProjectStore((s) => s.deleteFormItem);

  const form = project.forms.find((f) => f.name === formName);
  if (!form) {
    return <div className="placeholder-editor">Form not found: {formName}</div>;
  }

  const selectedItem =
    selectedItemIndex !== null ? form.items[selectedItemIndex] : undefined;

  const patchItem = (patch: Partial<FormItem>) => {
    if (selectedItemIndex === null || !selectedItem) return;
    updateFormItem(formName, selectedItemIndex, { ...selectedItem, ...patch } as FormItem);
  };

  return (
    <div className="form-editor">
      <div className="form-tabs">
        <button
          type="button"
          className={editorTab === "design" ? "active" : ""}
          onClick={() => setEditorTab("design")}
        >
          Design
        </button>
        <button
          type="button"
          className={editorTab === "preview" ? "active" : ""}
          onClick={() => setEditorTab("preview")}
        >
          Preview
        </button>
      </div>

      {editorTab === "design" ? (
        <div className="form-design-layout">
          <div className="form-design-main">
            <FormItemsPalette />
            <div className="form-canvas">
              {form.items.length === 0 ? (
                <p className="hint">
                  Insert items from the palette, then click an item to edit properties and rich
                  text.
                </p>
              ) : (
                form.items.map((item, i) => (
                  <div
                    key={`${item.label}-${i}`}
                    className={`form-item-block${selectedItemIndex === i ? " selected" : ""}`}
                    onClick={() => setSelectedItemIndex(i)}
                  >
                    <div className="form-item-label">
                      [{item.type}] {item.label}
                      <button
                        type="button"
                        className="item-delete"
                        title="Delete"
                        onClick={(e) => {
                          e.stopPropagation();
                          deleteFormItem(formName, i);
                        }}
                      >
                        ×
                      </button>
                    </div>
                    <PreviewItem item={item} />
                  </div>
                ))
              )}
            </div>
          </div>
          <aside className="form-properties">
            <div className="panel-title">Properties</div>
            {selectedItem ? (
              <FormItemProperties item={selectedItem} onChange={patchItem} />
            ) : (
              <p className="hint" style={{ padding: 8 }}>
                Select a form item to edit
              </p>
            )}
          </aside>
        </div>
      ) : (
        <div className="preview-frame">
          <PreviewForm items={form.items} />
        </div>
      )}
    </div>
  );
}

function PreviewForm({ items }: { items: FormItem[] }) {
  return (
    <form onSubmit={(e) => e.preventDefault()}>
      {items.map((item, i) => (
        <div key={`${item.label}-${i}`}>
          <PreviewItem item={item} />
        </div>
      ))}
    </form>
  );
}

export function PreviewItem({ item }: { item: FormItem }) {
  switch (item.type) {
    case "heading":
      return item.level === "sub" ? (
        <h3 className="preview-heading-sub">{item.content ?? item.label}</h3>
      ) : (
        <h2 className="preview-heading-main">{item.content ?? item.label}</h2>
      );
    case "text":
      return (
        <div
          className="text-block"
          dangerouslySetInnerHTML={{
            __html:
              typeof item.content === "string"
                ? item.content
                : Array.isArray(item.content)
                  ? richBlocksToHtml(item.content as RichContentBlock[])
                  : "",
          }}
        />
      );
    case "fib":
      return (
        <div>
          <p>{typeof item.prompt === "string" ? item.prompt : ""}</p>
          {(item.blanks ?? []).map((b) => (
            <span key={b.name} className="preview-fib-blank" title={b.name}>
              &nbsp;
            </span>
          ))}
        </div>
      );
    case "mc":
      return (
        <fieldset>
          <legend>{item.question ?? item.label}</legend>
          {(item.choices ?? []).map((c) => (
            <label key={c.name} className="preview-mc-choice">
              <input
                type={item.onlyone !== false ? "radio" : "checkbox"}
                name={item.label}
                value={c.name}
              />{" "}
              {c.text}
            </label>
          ))}
        </fieldset>
      );
    case "field":
      return <input type="hidden" name={item.fieldName ?? item.label} />;
    case "break":
      return <hr />;
    case "skipInstructions":
      return (
        <pre style={{ fontSize: 11, color: "#666" }}>
          Skip: {JSON.stringify(item.commands ?? [], null, 2)}
        </pre>
      );
    default:
      return null;
  }
}

function richBlocksToHtml(blocks: RichContentBlock[]): string {
  return blocks
    .map((b) => {
      if (b.type === "paragraph") {
        const inner = (b.nodes ?? [])
          .map((n) => {
            if (n.type === "text") return n.text ?? "";
            if (n.type === "bold") return `<strong>${n.nodes?.[0]?.text ?? ""}</strong>`;
            return "";
          })
          .join("");
        return `<p>${inner}</p>`;
      }
      return "";
    })
    .join("");
}
