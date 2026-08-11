import { useEffect, useRef, useState } from "react";
import { FormItem, RichContentBlock } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";
import {
  decoratePreservedWarningChipsHtml,
  formItemHasDisplayCondition,
  formItemHasPreservedGap,
} from "@/lib/preservedImportGaps";
import { CanvasItemBadgeStack } from "./CanvasItemBadgeStack";
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
  const showCond = formItemHasDisplayCondition(item);
  const gapClass = formItemHasPreservedGap(item) ? " has-preserved-gap" : "";

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

  const badge = (
    <CanvasItemBadgeStack showCond={showCond} formName={formName} itemIndex={index}>
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
          className={`text-badge${table ? " editing" : ""}`}
          draggable={!!table && selected}
          title={
            selected
              ? table
                ? "Drag to reorder, or click to edit text label"
                : "Click to rename, or use × to delete"
              : "Click to select"
          }
          onDragStart={(e) => {
            if (!table || !selected) {
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
    </CanvasItemBadgeStack>
  );

  if (!table) {
    return (
      <div
        className={`text-canvas-row idle structured-text-fallback${selected ? " selected" : ""}${gapClass}`}
        onClick={(e) => {
          e.stopPropagation();
          setSelectedItemIndex(index);
        }}
      >
        <FormItemDeleteButton formName={formName} index={index} visible={selected} />
        {badge}
        <p className="hint">
          This text item has structured content that cannot be edited on the canvas yet.
          Select it and press Delete (or click ×) to remove, or re-import after converter
          updates.
        </p>
      </div>
    );
  }

  return (
    <div
      className={`text-canvas-row structured-text-canvas-row editing${selected ? " selected" : ""}${gapClass}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
      }}
    >
      <FormItemDeleteButton formName={formName} index={index} visible={selected} />
      {badge}
      <div className="text-canvas-main">
        <div className="text-rich-wrap">
          <RichTextEditor
            html={decoratePreservedWarningChipsHtml(structuredContentToEditorHtml(content))}
            onChange={(html) => update(editorHtmlToStructuredContent(html, table))}
            placeholder="Enter text…"
            formattingKind="text"
          />
        </div>
      </div>
    </div>
  );
}
