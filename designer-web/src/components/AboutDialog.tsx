/**
 * Help → About — attorney-approved short notice (Jul 24, 2026).
 * Two copyright lines stay separate (legacy Tawala Systems vs 2026 Douglas G. Carlston).
 * No OS / .NET / File Versions; third-party acknowledgments only.
 */

import { useEffect } from "react";

interface Props {
  open: boolean;
  onClose: () => void;
}

const BANNER_SRC = "/icons/about-tawala.png";

export function AboutDialog({ open, onClose }: Props) {
  useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [open, onClose]);

  if (!open) return null;

  return (
    <div
      className="modal-overlay about-overlay"
      role="presentation"
      onClick={(e) => {
        if (e.target === e.currentTarget) onClose();
      }}
    >
      <div
        className="modal-dialog about-dialog"
        role="dialog"
        aria-labelledby="about-dialog-title"
        aria-modal="true"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="about-titlebar">
          <h2 id="about-dialog-title">About Tawala Project Designer</h2>
          <button type="button" className="designer-dialog-close" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>

        <div
          className="about-panel"
          style={{ backgroundImage: `url(${BANNER_SRC})` }}
        >
          <div className="about-banner">
            <span className="about-brand" aria-hidden>
              Tawala
            </span>
            <span className="about-edition">Beta Version</span>
          </div>

          <div className="about-body">
            <p className="about-copyright">
              Copyright © 2005 - 2009 Tawala Systems, Inc.
            </p>
            <p className="about-copyright">
              Copyright © 2026 Douglas G. Carlston. All rights reserved.
            </p>
            <p className="about-third-party">
              This software uses third-party tools and libraries including React, Vite,
              Express, Node.js, and Apache Tomcat. Other names and trademarks are the
              property of their respective owners.
            </p>
          </div>
        </div>

        <div className="modal-footer about-footer">
          <button type="button" className="about-ok" onClick={onClose} autoFocus>
            OK
          </button>
        </div>
      </div>
    </div>
  );
}
