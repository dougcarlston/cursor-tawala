import { BreakItem } from "@/types/tawala";
import { useProjectStore } from "@/store/projectStore";

interface Props {
  item: BreakItem;
  index: number;
  selected: boolean;
}

/**
 * Page break — fixed BREAK badge and hatched bar (legacy `BreakItemView`).
 * No editable content; marks a runtime page break.
 */
export function BreakCanvasRow({ item: _item, index, selected }: Props) {
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);

  return (
    <div
      className={`break-canvas-row${selected ? " selected" : ""}`}
      onClick={(e) => {
        e.stopPropagation();
        setSelectedItemIndex(index);
      }}
    >
      <div className="break-badge" title="Page break">
        BREAK
      </div>
      <div className="break-hatch" aria-hidden />
    </div>
  );
}
