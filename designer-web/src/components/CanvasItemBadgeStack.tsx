import type { ReactNode } from "react";
import { PreservedCondChip } from "./PreservedCondChip";

/** T/Q/H badge column with optional `cond` chip for preserved displayCondition. */
export function CanvasItemBadgeStack({
  children,
  showCond,
}: {
  children: ReactNode;
  showCond?: boolean;
}) {
  return (
    <div className="canvas-item-badge-stack">
      {children}
      {showCond ? <PreservedCondChip /> : null}
    </div>
  );
}
