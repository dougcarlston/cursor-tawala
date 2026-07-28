/**
 * Legacy Configure Function window chrome — NOT generic DesignerDialog.
 * Matches WinForms ConfigureFunctionDialog: fx title icon, Aero frame,
 * yellow help pane full-height, circular OK/CANCEL in the left footer.
 */

import type { ReactNode } from "react";

export interface ConfigureFunctionHelp {
  /** Focused parameter / column heading shown in the help pane. */
  title: string;
  /** Parameter description body. */
  body: string;
  /** Blue instructional line (e.g. "Choose from the drop-down list"). */
  hint?: string;
  /** Show bold red REQUIRED under the field help. */
  required?: boolean;
}

interface Props {
  titleId: string;
  onClose: () => void;
  /** ALL-CAPS function name for the help pane header. */
  functionTitle: string;
  functionDescription: string;
  help: ConfigureFunctionHelp;
  canOk: boolean;
  onOk: () => void;
  onCancel: () => void;
  /** Optional column +/-/↑/↓ toolbar (left side of footer). */
  toolbar?: ReactNode;
  /** Extra overlay classes (Fields interactivity / chrome blocking). */
  overlayClassName?: string;
  children: ReactNode;
}

function FxTitleIcon() {
  return (
    <span className="cfg-fn-fx" aria-hidden>
      <em>f</em>
      <sub>x</sub>
    </span>
  );
}

function OkIcon() {
  return (
    <span className="cfg-fn-action-icon cfg-fn-ok-icon" aria-hidden>
      ✓
    </span>
  );
}

function CancelIcon() {
  return (
    <span className="cfg-fn-action-icon cfg-fn-cancel-icon" aria-hidden>
      ×
    </span>
  );
}

export function ConfigureFunctionShell({
  titleId,
  onClose,
  functionTitle,
  functionDescription,
  help,
  canOk,
  onOk,
  onCancel,
  toolbar,
  overlayClassName = "",
  children,
}: Props) {
  return (
    <div
      className={`modal-overlay configure-function-overlay configure-function-blocks-chrome ${overlayClassName}`.trim()}
      role="presentation"
    >
      <div
        className="modal-dialog cfg-fn-dialog configure-function-dialog"
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
      >
        <div className="cfg-fn-titlebar">
          <div className="cfg-fn-title-row">
            <FxTitleIcon />
            <h2 id={titleId}>Configure Function</h2>
          </div>
          <button
            type="button"
            className="cfg-fn-close"
            onClick={onClose}
            aria-label="Close"
          >
            ×
          </button>
        </div>

        <div className="cfg-fn-main">
          <div className="cfg-fn-left">
            <div className="cfg-fn-fields configure-function-fields">{children}</div>
            <div className="cfg-fn-footer">
              {toolbar ? (
                <div className="cfg-fn-footer-toolbar configure-function-column-toolbar">
                  {toolbar}
                </div>
              ) : (
                <span className="cfg-fn-footer-toolbar-spacer" aria-hidden />
              )}
              <div className="cfg-fn-footer-actions">
                <button
                  type="button"
                  className="cfg-fn-action cfg-fn-ok"
                  disabled={!canOk}
                  onClick={onOk}
                >
                  <OkIcon />
                  <span>OK</span>
                </button>
                <button
                  type="button"
                  className="cfg-fn-action cfg-fn-cancel"
                  onClick={onCancel}
                >
                  <CancelIcon />
                  <span>CANCEL</span>
                </button>
              </div>
            </div>
          </div>

          <aside className="cfg-fn-help" aria-live="polite">
            <h3 className="cfg-fn-help-fn-title">{functionTitle}</h3>
            {functionDescription ? (
              <p className="cfg-fn-help-fn-desc">{functionDescription}</p>
            ) : null}
            {help.title ? <h4 className="cfg-fn-help-field-title">{help.title}</h4> : null}
            {help.body ? <p className="cfg-fn-help-field-body">{help.body}</p> : null}
            {help.hint ? <p className="cfg-fn-help-hint">{help.hint}</p> : null}
            {help.required ? <p className="cfg-fn-help-required">REQUIRED</p> : null}
          </aside>
        </div>
      </div>
    </div>
  );
}
