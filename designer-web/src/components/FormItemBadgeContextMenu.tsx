import { useEffect, useLayoutEffect, useRef, useState } from "react";
import { createPortal } from "react-dom";
import { openConditionalDisplayDialog } from "@/lib/conditionalDisplay";
import { confirmAndDeleteFormItem } from "@/lib/shellCommands";
import { useProjectStore } from "@/store/projectStore";

type MenuState = {
  x: number;
  y: number;
  formName: string;
  itemIndex: number;
};

/**
 * Legacy form-item badge context menu (Cut/Copy/Paste/Delete + Display conditionally…).
 * Cut/Copy/Paste are stubs (no form-item clipboard yet); Paste stays disabled.
 */
export function FormItemBadgeContextMenu({
  open,
  onClose,
}: {
  open: MenuState | null;
  onClose: () => void;
}) {
  const ref = useRef<HTMLDivElement>(null);

  useLayoutEffect(() => {
    if (!open || !ref.current) return;
    const el = ref.current;
    const pad = 8;
    const rect = el.getBoundingClientRect();
    let left = open.x;
    let top = open.y;
    if (left + rect.width > window.innerWidth - pad) {
      left = Math.max(pad, window.innerWidth - rect.width - pad);
    }
    if (top + rect.height > window.innerHeight - pad) {
      top = Math.max(pad, window.innerHeight - rect.height - pad);
    }
    el.style.left = `${left}px`;
    el.style.top = `${top}px`;
  }, [open]);

  useEffect(() => {
    if (!open) return;
    const onDown = (e: MouseEvent) => {
      if (ref.current?.contains(e.target as Node)) return;
      onClose();
    };
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("mousedown", onDown, true);
    window.addEventListener("keydown", onKey);
    return () => {
      window.removeEventListener("mousedown", onDown, true);
      window.removeEventListener("keydown", onKey);
    };
  }, [open, onClose]);

  if (!open) return null;

  const run = (fn: () => void) => {
    onClose();
    fn();
  };

  return createPortal(
    <div
      ref={ref}
      className="form-item-badge-context-menu"
      role="menu"
      style={{ position: "fixed", left: open.x, top: open.y, zIndex: 10000 }}
    >
      <button type="button" role="menuitem" disabled title="Not yet in browser Designer">
        Cut
        <span className="form-item-badge-context-accel">Ctrl+X</span>
      </button>
      <button type="button" role="menuitem" disabled title="Not yet in browser Designer">
        Copy
        <span className="form-item-badge-context-accel">Ctrl+C</span>
      </button>
      <button type="button" role="menuitem" disabled>
        Paste
        <span className="form-item-badge-context-accel">Ctrl+V</span>
      </button>
      <button
        type="button"
        role="menuitem"
        onClick={() =>
          run(() => {
            confirmAndDeleteFormItem(open.formName, open.itemIndex);
          })
        }
      >
        Delete
        <span className="form-item-badge-context-accel">Del</span>
      </button>
      <hr />
      <button
        type="button"
        role="menuitem"
        onClick={() =>
          run(() => {
            useProjectStore.getState().setSelectedItemIndex(open.itemIndex);
            openConditionalDisplayDialog(open.formName, open.itemIndex);
          })
        }
      >
        Display conditionally…
      </button>
    </div>,
    document.body,
  );
}
