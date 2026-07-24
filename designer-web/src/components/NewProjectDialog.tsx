import { useState } from "react";
import {
  PROJECT_TEMPLATES,
  TEMPLATE_CATEGORIES,
  templatesByCategory,
  type TemplateEntry,
} from "@/templates/catalog";

/** Exact 32×32 bitmap from legacy NewProjectDialog ImageList (`document.png`). */
const TEMPLATE_ICON_SRC = "/icons/new-project-template.png";

interface Props {
  open: boolean;
  onClose: () => void;
  onSelect: (template: TemplateEntry) => void;
}

/**
 * New Project picker — compact legacy icon grid (no left “Project type” tree).
 * Descriptions appear as hover tooltips (`title`). Click selects; OK / double-click opens.
 */
export function NewProjectDialog({ open, onClose, onSelect }: Props) {
  const byCategory = templatesByCategory();
  const [selectedId, setSelectedId] = useState(PROJECT_TEMPLATES[0]?.id ?? "empty");

  if (!open) return null;

  const selected = PROJECT_TEMPLATES.find((t) => t.id === selectedId) ?? PROJECT_TEMPLATES[0] ?? null;

  const confirm = () => {
    if (!selected) return;
    onSelect(selected);
  };

  return (
    <div
      className="modal-overlay new-project-overlay"
      role="presentation"
      onClick={(e) => {
        if (e.target === e.currentTarget) onClose();
      }}
    >
      <div
        className="modal-dialog new-project-dialog"
        role="dialog"
        aria-labelledby="new-project-title"
        aria-modal="true"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-header new-project-header">
          <div className="new-project-title-row">
            <img
              className="new-project-title-icon"
              src={TEMPLATE_ICON_SRC}
              width={16}
              height={16}
              alt=""
              draggable={false}
            />
            <h2 id="new-project-title">New Project</h2>
          </div>
          <button type="button" className="new-project-close" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>

        <div className="new-project-templates-pane">
          <div className="new-project-templates-label">Templates:</div>
          <div className="new-project-list">
            {TEMPLATE_CATEGORIES.map((category) => {
              const entries = byCategory.get(category) ?? [];
              if (!entries.length) return null;
              return (
                <section key={category} className="new-project-category">
                  <h3>{category}</h3>
                  <ul className="new-project-tile-grid">
                    {entries.map((t) => {
                      const active = t.id === selectedId;
                      return (
                        <li key={t.id}>
                          <button
                            type="button"
                            className={`new-project-tile${active ? " selected" : ""}`}
                            title={t.description}
                            aria-pressed={active}
                            aria-label={`${t.label}. ${t.description}`}
                            onClick={() => setSelectedId(t.id)}
                            onDoubleClick={() => onSelect(t)}
                          >
                            <img
                              className="new-project-tile-icon"
                              src={TEMPLATE_ICON_SRC}
                              width={32}
                              height={32}
                              alt=""
                              draggable={false}
                            />
                            <span className="new-project-tile-label">{t.label}</span>
                          </button>
                        </li>
                      );
                    })}
                  </ul>
                </section>
              );
            })}
          </div>
        </div>

        <div className="modal-footer new-project-footer">
          <button type="button" className="new-project-ok" disabled={!selected} onClick={confirm}>
            OK
          </button>
          <button type="button" onClick={onClose}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
