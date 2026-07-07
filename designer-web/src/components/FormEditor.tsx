import { useEffect, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { syncPreviewProject } from "@/api/preview";
import { FormItem } from "@/types/tawala";
import { FormItemsPalette } from "./FormItemsPalette";
import { FibFieldPreview } from "./FibFieldPreview";
import { FunctionTableBadge } from "./FunctionTableBadge";

interface Props {
  formName: string;
}

export function FormEditor({ formName }: Props) {
  const project = useProjectStore((s) => s.project);
  const editorTab = useProjectStore((s) => s.editorTab);
  const setEditorTab = useProjectStore((s) => s.setEditorTab);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const deleteFormItem = useProjectStore((s) => s.deleteFormItem);
  const deleteSelectedFormItem = useProjectStore((s) => s.deleteSelectedFormItem);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [previewError, setPreviewError] = useState<string | null>(null);

  const form = project.forms.find((f) => f.name === formName);
  if (!form) {
    return <div className="placeholder-editor">Form not found: {formName}</div>;
  }

  useEffect(() => {
    const onKeyDown = (e: KeyboardEvent) => {
      if (editorTab !== "design") return;
      if (selectedItemIndex === null) return;
      const target = e.target as HTMLElement | null;
      if (target?.closest("input, textarea, select, [contenteditable='true']")) return;
      if (e.key === "Delete" || e.key === "Backspace") {
        e.preventDefault();
        deleteSelectedFormItem();
      }
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [editorTab, selectedItemIndex, deleteSelectedFormItem]);

  useEffect(() => {
    if (editorTab !== "preview") return;
    let cancelled = false;
    setPreviewError(null);
    void syncPreviewProject(project, formName)
      .then(() => {
        if (cancelled) return;
        const url = `/preview/designer/${encodeURIComponent(project.name)}/${encodeURIComponent(formName)}`;
        setPreviewUrl(`${url}?t=${Date.now()}`);
      })
      .catch((e) => {
        if (!cancelled) setPreviewError(e instanceof Error ? e.message : String(e));
      });
    return () => {
      cancelled = true;
    };
  }, [editorTab, project, formName]);

  return (
    // The MDI window title bar (`Form - Name`) is the single window heading now,
    // so the editor body starts straight at the Design/Preview tabs — no duplicate
    // inner title (owner Decision 3, July 2026).
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
        <div className="form-design-body">
          <div className="form-insert-strip">
            <FormItemsPalette />
          </div>
          <div
            className="form-canvas"
            onClick={(e) => {
              if (e.target === e.currentTarget) setSelectedItemIndex(null);
            }}
          >
            {form.items.length === 0 ? (
              <p className="hint form-canvas-hint">
                Drag items from the palette on the left to create your form, or click a palette
                button to insert.
              </p>
            ) : (
              form.items.map((item, i) => (
                <div
                  key={`${item.label}-${i}`}
                  className={`form-item-block${selectedItemIndex === i ? " selected" : ""}`}
                  onClick={() => setSelectedItemIndex(i)}
                >
                  <div className="form-item-label">
                    <span>
                      [{item.type}] {item.label}
                    </span>
                    <button
                      type="button"
                      className="item-delete"
                      title="Delete item"
                      onClick={(e) => {
                        e.stopPropagation();
                        deleteFormItem(formName, i);
                      }}
                    >
                      Delete
                    </button>
                  </div>
                  <CanvasItem item={item} />
                </div>
              ))
            )}
          </div>
        </div>
      ) : (
        <div className="preview-frame preview-frame-runtime">
          {previewError ? (
            <p className="hint">Preview failed: {previewError}</p>
          ) : previewUrl ? (
            <iframe title={`Preview ${formName}`} src={previewUrl} className="form-preview-iframe" />
          ) : (
            <p className="hint">Loading preview…</p>
          )}
        </div>
      )}
    </div>
  );
}

function CanvasItem({ item }: { item: FormItem }) {
  switch (item.type) {
    case "heading":
      return item.level === "sub" ? (
        <h3 className="preview-heading-sub">{item.content ?? item.label}</h3>
      ) : (
        <h2 className="preview-heading-main">{item.content ?? item.label}</h2>
      );
    case "text":
      if (Array.isArray(item.content)) {
        return <FunctionTableBadge content={item.content} />;
      }
      return (
        <div
          className={`text-block${item.style === "instructional" ? " instructional" : ""}`}
          dangerouslySetInnerHTML={{ __html: typeof item.content === "string" ? item.content : "" }}
        />
      );
    case "fib":
      return <FibFieldPreview item={item} />;
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
                readOnly
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
