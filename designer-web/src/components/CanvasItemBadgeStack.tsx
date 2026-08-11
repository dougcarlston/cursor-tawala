import { useCallback, useState, type MouseEvent, type ReactNode } from "react";
import { PreservedCondChip } from "./PreservedCondChip";
import { FormItemBadgeContextMenu } from "./FormItemBadgeContextMenu";

/** T/Q/H badge column; when conditional, CSS draws `{label}` inside the badge (legacy). */
export function CanvasItemBadgeStack({
  children,
  showCond,
  formName,
  itemIndex,
  showCondChip = false,
}: {
  children: ReactNode;
  showCond?: boolean;
  /** When set with itemIndex, right-click / Ctrl-click opens Display conditionally… */
  formName?: string;
  itemIndex?: number;
  /** Amber `cond` chip — off by default; braces are the primary cue. */
  showCondChip?: boolean;
}) {
  const [menu, setMenu] = useState<{
    x: number;
    y: number;
    formName: string;
    itemIndex: number;
  } | null>(null);

  const onContextMenu = useCallback(
    (e: MouseEvent) => {
      if (formName == null || itemIndex == null) return;
      e.preventDefault();
      e.stopPropagation();
      setMenu({ x: e.clientX, y: e.clientY, formName, itemIndex });
    },
    [formName, itemIndex],
  );

  const menuEnabled = formName != null && itemIndex != null;

  return (
    <div
      className={`canvas-item-badge-stack${showCond ? " has-display-condition" : ""}${
        menuEnabled ? " has-context-menu" : ""
      }`}
      title={showCond ? "Displayed conditionally — Ctrl-click or right-click to edit" : undefined}
      onContextMenu={menuEnabled ? onContextMenu : undefined}
    >
      {children}
      {showCond && showCondChip ? <PreservedCondChip /> : null}
      <FormItemBadgeContextMenu open={menu} onClose={() => setMenu(null)} />
    </div>
  );
}
