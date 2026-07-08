import { Fragment, useEffect, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { syncPreviewProject } from "@/api/preview";
import { FormItem } from "@/types/tawala";
import { FibFieldPreview } from "./FibFieldPreview";
import { FibCanvasRow } from "./FibCanvasRow";
import { McqCanvasRow } from "./McqCanvasRow";
import { HiddenFieldCanvasRow } from "./HiddenFieldCanvasRow";
import { BreakCanvasRow } from "./BreakCanvasRow";
import { SkipCanvasRow } from "./SkipCanvasRow";
import { FunctionTableBadge } from "./FunctionTableBadge";
import { HeadingCanvasRow } from "./HeadingCanvasRow";
import { TextCanvasRow } from "./TextCanvasRow";
import { FormInsertionPoint } from "./FormInsertionPoint";

interface Props {
  formName: string;
}

export function FormEditor({ formName }: Props) {
  const project = useProjectStore((s) => s.project);
  const editorTab = useProjectStore((s) => s.editorTab);
  const setEditorTab = useProjectStore((s) => s.setEditorTab);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const setInsertBeforeIndex = useProjectStore((s) => s.setInsertBeforeIndex);
  const moveSelectedFormItem = useProjectStore((s) => s.moveSelectedFormItem);
  const deleteFormItem = useProjectStore((s) => s.deleteFormItem);
  const deleteSelectedFormItem = useProjectStore((s) => s.deleteSelectedFormItem);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [previewError, setPreviewError] = useState<string | null>(null);

  const form = project.forms.find((f) => f.name === formName);
  const itemCount = form?.items.length ?? 0;
  const canMoveUp = selectedItemIndex !== null && selectedItemIndex > 0;
  const canMoveDown = selectedItemIndex !== null && selectedItemIndex < itemCount - 1;

  useEffect(() => {
    if (!form) return;
    const onKeyDown = (e: KeyboardEvent) => {
      if (editorTab !== "design") return;
      const target = e.target as HTMLElement | null;
      if (target?.closest("input, textarea, select, [contenteditable='true']")) return;

      if (selectedItemIndex !== null) {
        if (e.altKey && e.key === "ArrowUp" && canMoveUp) {
          e.preventDefault();
          moveSelectedFormItem("up");
          return;
        }
        if (e.altKey && e.key === "ArrowDown" && canMoveDown) {
          e.preventDefault();
          moveSelectedFormItem("down");
          return;
        }
        if (e.key === "Delete" || e.key === "Backspace") {
          e.preventDefault();
          deleteSelectedFormItem();
        }
      }
    };
    window.addEventListener("keydown", onKeyDown);
    return () => window.removeEventListener("keydown", onKeyDown);
  }, [
    editorTab,
    selectedItemIndex,
    canMoveUp,
    canMoveDown,
    deleteSelectedFormItem,
    moveSelectedFormItem,
    form,
  ]);

  useEffect(() => {
    if (!form || editorTab !== "preview") return;
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
  }, [editorTab, project, formName, form]);

  if (!form) {
    return <div className="placeholder-editor">Form not found: {formName}</div>;
  }

  function renderFormItem(item: FormItem, i: number) {
    if (item.type === "heading") {
      return (
        <HeadingCanvasRow
          item={item}
          index={i}
          formName={formName}
          selected={selectedItemIndex === i}
        />
      );
    }
    if (item.type === "text" && !Array.isArray(item.content)) {
      return (
        <TextCanvasRow
          item={item}
          index={i}
          formName={formName}
          selected={selectedItemIndex === i}
        />
      );
    }
    if (item.type === "fib") {
      return (
        <FibCanvasRow
          item={item}
          index={i}
          formName={formName}
          selected={selectedItemIndex === i}
        />
      );
    }
    if (item.type === "mc") {
      return (
        <McqCanvasRow
          item={item}
          index={i}
          formName={formName}
          selected={selectedItemIndex === i}
        />
      );
    }
    if (item.type === "field") {
      return (
        <HiddenFieldCanvasRow
          item={item}
          index={i}
          formName={formName}
          selected={selectedItemIndex === i}
        />
      );
    }
    if (item.type === "break") {
      return <BreakCanvasRow item={item} index={i} selected={selectedItemIndex === i} />;
    }
    if (item.type === "skipInstructions") {
      return (
        <SkipCanvasRow
          item={item}
          index={i}
          formName={formName}
          selected={selectedItemIndex === i}
        />
      );
    }
    return (
      <div
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
    );
  }

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
        {editorTab === "design" && selectedItemIndex !== null ? (
          <span className="form-item-move-toolbar" role="toolbar" aria-label="Move form item">
            <button
              type="button"
              title="Move item up (Alt+↑)"
              disabled={!canMoveUp}
              onClick={() => moveSelectedFormItem("up")}
            >
              ↑
            </button>
            <button
              type="button"
              title="Move item down (Alt+↓)"
              disabled={!canMoveDown}
              onClick={() => moveSelectedFormItem("down")}
            >
              ↓
            </button>
            <button
              type="button"
              className="form-item-delete-btn"
              title="Delete selected item (Del)"
              onClick={() => deleteSelectedFormItem()}
            >
              ×
            </button>
          </span>
        ) : null}
      </div>

      {editorTab === "design" ? (
        // The Items palette used to live inside each form window (a left strip).
        // Owner decision D-Items-palette-placement (July 2026): it now docks beside
        // Project Explorer (see App.tsx `.designer-items`), matching legacy layout,
        // so the window body is just the canvas.
        <div className="form-design-body">
          <div
            className="form-canvas"
            onClick={(e) => {
              if (e.target === e.currentTarget) {
                setSelectedItemIndex(null);
                setInsertBeforeIndex(form.items.length);
              }
            }}
          >
            <FormInsertionPoint beforeIndex={0} formName={formName} />
            {form.items.length === 0 ? (
              <p className="hint form-canvas-hint">
                Click a blue insertion arrow or select a position, then use the Items palette to
                insert. Drag items from the palette on the left, or click a palette button.
              </p>
            ) : (
              form.items.map((item, i) => (
                <Fragment key={`${item.label}-${i}`}>
                  {renderFormItem(item, i)}
                  <FormInsertionPoint beforeIndex={i + 1} formName={formName} />
                </Fragment>
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

// Headings render via `HeadingCanvasRow` (canvas WYSIWYG). CanvasItem handles the
// remaining item types inside the generic `.form-item-block` wrapper.
function CanvasItem({ item }: { item: Exclude<FormItem, { type: "heading" }> }) {
  switch (item.type) {
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
      return (
        <p className="hint" style={{ fontSize: 11, color: "#666" }}>
          Hidden field: {item.fieldName ?? item.name ?? "(unnamed)"}
        </p>
      );
    case "break":
      return <div className="break-hatch preview-break" aria-hidden />;
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
