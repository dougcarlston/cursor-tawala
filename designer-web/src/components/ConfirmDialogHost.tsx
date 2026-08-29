import { useSyncExternalStore } from "react";
import { DesignerDialog } from "./DesignerDialog";
import {
  getActiveConfirm,
  subscribeConfirmDialog,
} from "@/lib/confirmDialog";

export function ConfirmDialogHost() {
  const active = useSyncExternalStore(
    subscribeConfirmDialog,
    getActiveConfirm,
    getActiveConfirm,
  );

  if (!active) return null;

  const { options, resolve } = active;
  const title = options.title || "Confirm";
  const okText = options.okText || "OK";
  const cancelText = options.cancelText || "Cancel";

  const handleOk = () => resolve(true);
  const handleCancel = () => resolve(false);

  return (
    <DesignerDialog
      title={title}
      titleId="designer-confirm-dialog-title"
      onClose={handleCancel}
      closeOnBackdrop
      className="designer-confirm-dialog"
      footer={
        <>
          <button
            type="button"
            className="designer-dialog-button secondary"
            onClick={handleCancel}
          >
            {cancelText}
          </button>
          <button
            type="button"
            className={`designer-dialog-button primary${options.destructive ? " destructive" : ""}`}
            onClick={handleOk}
            autoFocus
          >
            {okText}
          </button>
        </>
      }
    >
      <div className="designer-confirm-body">
        <p className="designer-confirm-message">{options.message}</p>
      </div>
    </DesignerDialog>
  );
}
