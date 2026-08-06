import {
  PRESERVED_COLUMN_CONDITION_TOOLTIP,
  PRESERVED_CONDITION_TOOLTIP,
} from "@/lib/preservedImportGaps";

type Props = {
  /** Item-level vs column-level copy. */
  kind?: "item" | "column";
  className?: string;
};

/** Compact `cond` cue beside T/Q badges or Configure Function column titles. */
export function PreservedCondChip({ kind = "item", className }: Props) {
  const tip =
    kind === "column" ? PRESERVED_COLUMN_CONDITION_TOOLTIP : PRESERVED_CONDITION_TOOLTIP;
  return (
    <span
      className={`preserved-cond-chip${className ? ` ${className}` : ""}`}
      title={tip}
      aria-label={tip}
    >
      cond
    </span>
  );
}
