import { useEffect, useRef, useState } from "react";
import { FormItem, RichContentBlock } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import { FormItemDeleteButton } from "./FormItemDeleteButton";
import { RichTextEditor } from "./RichTextEditor";
import {
  editorHtmlToStructuredContent,
  findEditableStructuredFunctionNode,
  structuredContentToEditorHtml,
} from "./StructuredTextProperties";

interface Props {
  item: Extract<FormItem, { type: "text" }>;
  index: number;
  formName: string;
  selected: boolean;
}

/**
 * Form Text items whose content is structured (Multiple Question List, Question Correlation, …).
 * Canvas-editable via RichTextEditor — type around the function token; click the token to Configure.
 */
export function StructuredTextCanvasRow({ item, index, formName, selected }: Props) {
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const [editingLabel, setEditingLabel] = useState(false);
  const labelInputRef = useRef<HTMLInputElement>(null);

  const content: RichContentBlock[] = Array.isArray(item.content) ? item.content : [];
  const table = findEditableStructuredFunctionNode(content);

  const update = (next: RichContentBlock[]) => {
    updateFormItem(formName, index, { ...item, content: next });
  };

  useEffect(() => {
    if (!editingLabel) return;
    const el = labelInputRef.current;
    if (!el) return;
    el.focus();
    el.select();
  }, [editingLabel]);

  const commitLabel = (raw: string) => {
    const trimmed = raw.trim();
    if (trimmed && trimmed !== item.label) {
      updateFormItem(formName, index, { ...item, label: trimmed });
    }
    setEditingLabel(false);
  };

  if (!table) {
    return (
      <div className={`text-canvas-row idle${selected ? " selected" : ""}`}>
        <p className="hint">
          This text item has structured content that cannot be edited on the canvas yet.
        </p>
      </div>
    );
  }

  return (
    <div
      className={`text-canvas-row structured-text-canvas-row editing${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
      }}
    >
      <FormItemDeleteButton formName={formName} index={index} visible={selected} />
      {editingLabel ? (
        <input
          ref={labelInputRef}
          className="text-badge-input"
          defaultValue={item.label}
          maxLength={12}
          onClick={(e) => e.stopPropagation()}
          onKeyDown={(e) => {
            if (e.key === "Enter") {
              e.preventDefault();
              commitLabel(e.currentTarget.value);
            } else if (e.key === "Escape") {
              e.preventDefault();
              setEditingLabel(false);
            }
          }}
          onBlur={(e) => commitLabel(e.currentTarget.value)}
        />
      ) : (
        <div
          className="text-badge editing"
          draggable={selected}
          title={selected ? "Drag to reorder, or click to edit text label" : "Click to select"}
          onDragStart={(e) => {
            if (!selected) {
              e.preventDefault();
              return;
            }
            e.dataTransfer.effectAllowed = "move";
          }}
          onClick={(e) => {
            e.stopPropagation();
            setSelectedItemIndex(index);
            if (selected) setEditingLabel(true);
          }}
        >
          {item.label}
        </div>
      )}
      <div className="text-canvas-main">
        <div className="text-rich-wrap">
          <RichTextEditor
            html={structuredContentToEditorHtml(content)}
            onChange={(html) => update(editorHtmlToStructuredContent(html, table))}
            placeholder="Enter text…"
            formattingKind="text"
          />
        </div>
      </div>
    </div>
  );
}
