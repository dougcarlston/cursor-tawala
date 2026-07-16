import { useEffect, useRef, useState } from "react";
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
import { StructuredTextCanvasRow } from "./StructuredTextCanvasRow";
import {
  hasFormItemDrag,
  hasFormItemReorderDrag,
  readFormItemDrag,
  readFormItemReorderDrag,
  setFormItemReorderDrag,
} from "@/lib/designerDrag";
import { isFormItemReorderHandle } from "@/lib/formItemReorder";
import {
  confirmAndDeleteFormItem,
  confirmAndDeleteSelectedFormItem,
} from "@/lib/shellCommands";

interface Props {
  formName: string;
}

/**
 * Form design canvas — legacy insert/move:
 * compact item list (no permanent insert bars); floating caret only while dragging
 * a palette item or reordering a selected row; edge auto-scroll during drag.
 */
export function FormEditor({ formName }: Props) {
  const project = useProjectStore((s) => s.project);
  const editorTab = useProjectStore((s) => s.editorTab);
  const setEditorTab = useProjectStore((s) => s.setEditorTab);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const setInsertBeforeIndex = useProjectStore((s) => s.setInsertBeforeIndex);
  const openWindow = useProjectStore((s) => s.openWindow);
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const moveSelectedFormItem = useProjectStore((s) => s.moveSelectedFormItem);
  const moveFormItemBefore = useProjectStore((s) => s.moveFormItemBefore);
  const [previewUrl, setPreviewUrl] = useState<string | null>(null);
  const [previewError, setPreviewError] = useState<string | null>(null);
  /** Live insert-before index while dragging (palette or reorder). */
  const [dragBeforeIndex, setDragBeforeIndex] = useState<number | null>(null);
  const [dragCaretTop, setDragCaretTop] = useState<number | null>(null);
  const [reorderFromIndex, setReorderFromIndex] = useState<number | null>(null);
  const canvasRef = useRef<HTMLDivElement>(null);

  const form = project.forms.find((f) => f.name === formName);
  const itemCount = form?.items.length ?? 0;
  const canMoveUp = selectedItemIndex !== null && selectedItemIndex > 0;
  const canMoveDown = selectedItemIndex !== null && selectedItemIndex < itemCount - 1;
  const dragActive = dragBeforeIndex !== null;

  useEffect(() => {
    if (!form) return;
    const onKeyDown = (e: KeyboardEvent) => {
      if (editorTab !== "design") return;
      const target = e.target as HTMLElement | null;
      if (target?.closest("input, textarea, select, button, [contenteditable='true']")) return;
      // FIB/MCQ property strips — Del must not remove the whole item while editing blanks.
      if (target?.closest(".fib-property-strip, .mcq-property-strip")) return;

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
          confirmAndDeleteSelectedFormItem();
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

  const clearDragUi = () => {
    setDragBeforeIndex(null);
    setDragCaretTop(null);
    setReorderFromIndex(null);
  };

  const updateDragCaret = (clientY: number) => {
    const canvas = canvasRef.current;
    if (!canvas) return;
    autoScrollFormCanvas(canvas, clientY);
    const beforeIndex = nearestInsertBeforeIndex(canvas, clientY, form.items.length);
    const top = caretOffsetTop(canvas, beforeIndex, form.items.length);
    setDragBeforeIndex(beforeIndex);
    setDragCaretTop(top);
  };

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
    if (item.type === "text" && Array.isArray(item.content)) {
      return (
        <StructuredTextCanvasRow
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
              confirmAndDeleteFormItem(formName, i);
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
              onClick={() => confirmAndDeleteSelectedFormItem()}
            >
              ×
            </button>
          </span>
        ) : null}
      </div>

      {editorTab === "design" ? (
        <div className="form-design-body">
          <div
            ref={canvasRef}
            className={`form-canvas${dragActive ? " form-canvas-item-drag" : ""}`}
            onClick={(e) => {
              if (e.target === e.currentTarget) {
                setSelectedItemIndex(null);
                setInsertBeforeIndex(form.items.length);
              }
            }}
            onDragOver={(e) => {
              const isPalette = hasFormItemDrag(e.dataTransfer);
              const isReorder = hasFormItemReorderDrag(e.dataTransfer);
              if (!isPalette && !isReorder) return;
              e.preventDefault();
              e.stopPropagation();
              e.dataTransfer.dropEffect = isReorder ? "move" : "copy";
              updateDragCaret(e.clientY);
            }}
            onDragLeave={(e) => {
              if (!e.currentTarget.contains(e.relatedTarget as Node)) {
                clearDragUi();
              }
            }}
            onDrop={(e) => {
              const beforeIndex =
                dragBeforeIndex ??
                nearestInsertBeforeIndex(e.currentTarget, e.clientY, form.items.length);
              const reorderFrom = readFormItemReorderDrag(e.dataTransfer);
              const type = readFormItemDrag(e.dataTransfer);
              clearDragUi();
              if (reorderFrom != null) {
                e.preventDefault();
                e.stopPropagation();
                openWindow("form", formName);
                moveFormItemBefore(formName, reorderFrom, beforeIndex);
                return;
              }
              if (!type) return;
              e.preventDefault();
              e.stopPropagation();
              openWindow("form", formName);
              insertFormItem(type, { formName, beforeIndex });
            }}
            onDragEnd={clearDragUi}
          >
            {dragActive && dragCaretTop != null ? (
              <div
                className="form-canvas-insert-caret"
                style={{ top: dragCaretTop }}
                aria-hidden
              >
                <img
                  className="form-canvas-insert-caret-marker"
                  src="/designer/Insert.png"
                  width={16}
                  height={13}
                  alt=""
                />
              </div>
            ) : null}

            {form.items.length === 0 ? (
              <p className="hint form-canvas-hint">
                Drag an item from the Items palette into this window, or click a palette button to
                insert. Drag a selected item by its badge to reorder.
              </p>
            ) : (
              form.items.map((item, i) => (
                <div
                  key={`${item.label}-${i}`}
                  className={`form-item-slot${selectedItemIndex === i ? " selected-slot" : ""}${reorderFromIndex === i ? " dragging" : ""}`}
                  data-form-item-index={i}
                  draggable={false}
                  onDragStart={(e) => {
                    // Drag is initiated on the selected badge (child has draggable=true);
                    // this bubbles here so we can attach reorder MIME and UI state.
                    if (selectedItemIndex !== i || !isFormItemReorderHandle(e.target)) {
                      e.preventDefault();
                      return;
                    }
                    setReorderFromIndex(i);
                    setFormItemReorderDrag(e.dataTransfer, i);
                  }}
                  onDragEnd={clearDragUi}
                >
                  {renderFormItem(item, i)}
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

/** Insert before the first item whose vertical midpoint is below `clientY`. */
function nearestInsertBeforeIndex(
  canvas: HTMLElement,
  clientY: number,
  itemCount: number,
): number {
  const slots = Array.from(canvas.querySelectorAll<HTMLElement>("[data-form-item-index]"));
  if (!slots.length) return 0;
  for (const el of slots) {
    const rect = el.getBoundingClientRect();
    const mid = rect.top + rect.height / 2;
    if (clientY < mid) {
      const raw = Number(el.dataset.formItemIndex);
      return Number.isFinite(raw) ? raw : 0;
    }
  }
  return itemCount;
}

/** Caret Y inside the scrollable canvas — just below the insertion boundary. */
function caretOffsetTop(canvas: HTMLElement, beforeIndex: number, itemCount: number): number {
  const slots = Array.from(canvas.querySelectorAll<HTMLElement>("[data-form-item-index]"));
  if (!slots.length) return canvas.scrollTop + 8;
  if (beforeIndex <= 0) {
    return slots[0].offsetTop;
  }
  if (beforeIndex >= itemCount) {
    const last = slots[slots.length - 1];
    return last.offsetTop + last.offsetHeight;
  }
  return slots[beforeIndex].offsetTop;
}

/** Scroll form canvas when the pointer nears the top/bottom edge (legacy long-form feel). */
function autoScrollFormCanvas(canvas: HTMLElement, clientY: number): void {
  const rect = canvas.getBoundingClientRect();
  const edge = 36;
  const step = 18;
  if (clientY < rect.top + edge) {
    canvas.scrollTop = Math.max(0, canvas.scrollTop - step);
  } else if (clientY > rect.bottom - edge) {
    canvas.scrollTop = Math.min(canvas.scrollHeight - canvas.clientHeight, canvas.scrollTop + step);
  }
}

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
