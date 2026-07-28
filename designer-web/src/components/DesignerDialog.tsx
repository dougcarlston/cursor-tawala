/**
 * Shared Main Menu / Insert dialog chrome — legacy Aero-style frame.
 * Rounded corners, blue Aero border, form+gear title icon, shaded buttons.
 */

import type { CSSProperties, HTMLAttributes, ReactNode, Ref } from "react";

/** Same form + purple gear used for Pre/Post in Project Explorer. */
export function DesignerDialogTitleIcon() {
  return (
    <span className="designer-dialog-title-icon" aria-hidden>
      <svg width="16" height="16" viewBox="0 0 16 16" focusable="false">
        <rect x="1.5" y="1.5" width="10" height="11" rx="0" fill="#ffffff" stroke="#4a6fa5" />
        <rect x="1.5" y="1.5" width="10" height="2.4" fill="#4a6fa5" />
        <line x1="3.5" y1="6" x2="9.5" y2="6" stroke="#9db4d4" />
        <line x1="3.5" y1="8" x2="9.5" y2="8" stroke="#9db4d4" />
        <line x1="3.5" y1="10" x2="7.5" y2="10" stroke="#9db4d4" />
      </svg>
      <svg
        className="designer-dialog-title-gear"
        width="10"
        height="10"
        viewBox="0 0 24 24"
        focusable="false"
      >
        <circle cx="12" cy="12" r="11" fill="#ffffff" />
        <path
          d="M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58a.49.49 0 0 0 .12-.61l-1.92-3.32a.488.488 0 0 0-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54a.484.484 0 0 0-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96c-.22-.08-.47 0-.59.22L2.74 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.09.63-.09.94s.02.64.07.94l-2.03 1.58a.49.49 0 0 0-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z"
          fill="#8a2be2"
        />
      </svg>
    </span>
  );
}

interface Props {
  title: string;
  titleId: string;
  onClose: () => void;
  children: ReactNode;
  footer: ReactNode;
  className?: string;
  overlayClassName?: string;
  bodyClassName?: string;
  footerClassName?: string;
  titleBarClassName?: string;
  titleBarProps?: HTMLAttributes<HTMLDivElement>;
  dialogRef?: Ref<HTMLDivElement>;
  style?: CSSProperties;
  closeOnBackdrop?: boolean;
}

export function DesignerDialog({
  title,
  titleId,
  onClose,
  children,
  footer,
  className = "",
  overlayClassName = "",
  bodyClassName = "",
  footerClassName = "",
  titleBarClassName = "",
  titleBarProps,
  dialogRef,
  style,
  closeOnBackdrop = false,
}: Props) {
  return (
    <div
      className={`modal-overlay designer-dialog-overlay ${overlayClassName}`.trim()}
      role="presentation"
      onClick={(e) => {
        if (closeOnBackdrop && e.target === e.currentTarget) onClose();
      }}
    >
      <div
        ref={dialogRef}
        className={`modal-dialog designer-dialog ${className}`.trim()}
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
        style={style}
        onClick={(e) => e.stopPropagation()}
      >
        <div
          className={`designer-dialog-titlebar ${titleBarClassName}`.trim()}
          {...titleBarProps}
        >
          <div className="designer-dialog-title-row">
            <DesignerDialogTitleIcon />
            <h2 id={titleId}>{title}</h2>
          </div>
          <button
            type="button"
            className="designer-dialog-close"
            onPointerDown={(e) => e.stopPropagation()}
            onClick={onClose}
            aria-label="Close"
          >
            ×
          </button>
        </div>
        <div className={`designer-dialog-body ${bodyClassName}`.trim()}>{children}</div>
        <div className={`designer-dialog-footer ${footerClassName}`.trim()}>{footer}</div>
      </div>
    </div>
  );
}
