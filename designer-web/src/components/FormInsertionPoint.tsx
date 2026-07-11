import { useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { hasFormItemDrag, readFormItemDrag } from "@/lib/designerDrag";

interface Props {
  /** Index in `form.items` where the next palette insert will land (0 = before first). */
  beforeIndex: number;
  formName: string;
  /** True when the parent canvas highlights this gap as the live drop target. */
  dropHighlight?: boolean;
  /** Hide the click-selected marker so only the live drop line shows while dragging. */
  suppressActive?: boolean;
}

/**
 * Legacy parity: blue insertion-point arrow between form rows (`droparea.htc`
 * `InsertionPoint` + `Insert.png`). Click a gap to choose where the next Items
 * palette insert lands; drop an Items palette drag here to insert at this spot.
 */
export function FormInsertionPoint({
  beforeIndex,
  formName,
  dropHighlight = false,
  suppressActive = false,
}: Props) {
  const selection = useProjectStore((s) => s.selection);
  const insertBeforeIndex = useProjectStore((s) => s.insertBeforeIndex);
  const setInsertBeforeIndex = useProjectStore((s) => s.setInsertBeforeIndex);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
  const openWindow = useProjectStore((s) => s.openWindow);
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const [dragOver, setDragOver] = useState(false);

  const thisFormSelected = selection.kind === "form" && selection.name === formName;
  const active =
    !suppressActive && thisFormSelected && insertBeforeIndex === beforeIndex;
  const showDrop = dragOver || dropHighlight;

  return (
    <div
      className={`form-insertion-point${active ? " active" : ""}${showDrop ? " drop-target" : ""}`}
      data-insert-before={beforeIndex}
      role="button"
      tabIndex={-1}
      title={
        active
          ? "Next item inserts here"
          : `Drop or click to insert before item ${beforeIndex + 1}`
      }
      onClick={(e) => {
        e.stopPropagation();
        openWindow("form", formName);
        setInsertBeforeIndex(beforeIndex);
        setSelectedItemIndex(null);
      }}
      onDragOver={(e) => {
        if (!hasFormItemDrag(e.dataTransfer)) return;
        e.preventDefault();
        e.stopPropagation();
        e.dataTransfer.dropEffect = "copy";
        if (!dragOver) setDragOver(true);
      }}
      onDragLeave={(e) => {
        if (!e.currentTarget.contains(e.relatedTarget as Node)) setDragOver(false);
      }}
      onDrop={(e) => {
        setDragOver(false);
        const type = readFormItemDrag(e.dataTransfer);
        if (!type) return;
        e.preventDefault();
        e.stopPropagation();
        openWindow("form", formName);
        insertFormItem(type, { formName, beforeIndex });
      }}
    >
      <span className="form-insertion-point-marker" aria-hidden>
        ▶
      </span>
      <span className="form-insertion-point-line" aria-hidden />
    </div>
  );
}
