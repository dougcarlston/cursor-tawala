import { useProjectStore } from "@/store/projectStore";

interface Props {
  /** Index in `form.items` where the next palette insert will land (0 = before first). */
  beforeIndex: number;
  formName: string;
}

/**
 * Legacy parity: blue insertion-point arrow between form rows (`droparea.htc`
 * `InsertionPoint` + `Insert.png`). Click a gap to choose where the next Items
 * palette insert lands.
 */
export function FormInsertionPoint({ beforeIndex, formName }: Props) {
  const selection = useProjectStore((s) => s.selection);
  const insertBeforeIndex = useProjectStore((s) => s.insertBeforeIndex);
  const setInsertBeforeIndex = useProjectStore((s) => s.setInsertBeforeIndex);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);

  if (selection.kind !== "form" || selection.name !== formName) return null;

  const active = insertBeforeIndex === beforeIndex;

  return (
    <div
      className={`form-insertion-point${active ? " active" : ""}`}
      role="button"
      tabIndex={-1}
      title={
        active
          ? "Next item inserts here"
          : `Insert before item ${beforeIndex + 1}`
      }
      onClick={(e) => {
        e.stopPropagation();
        setInsertBeforeIndex(beforeIndex);
        setSelectedItemIndex(null);
      }}
    >
      <span className="form-insertion-point-marker" aria-hidden>
        ▶
      </span>
      <span className="form-insertion-point-line" aria-hidden />
    </div>
  );
}
