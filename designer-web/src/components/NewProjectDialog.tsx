import { useEffect, useState } from "react";
import {
  PROJECT_TEMPLATES,
  TEMPLATE_CATEGORIES,
  templatesByCategory,
  type TemplateEntry,
} from "@/templates/catalog";
import { DesignerDialog } from "./DesignerDialog";

/** Exact 32×32 bitmap from legacy NewProjectDialog ImageList (`document.png`). */
const TEMPLATE_ICON_SRC = "/icons/new-project-template.png";

interface Props {
  open: boolean;
  onClose: () => void;
  onSelect: (template: TemplateEntry) => void;
}

/**
 * New Project picker — compact icon grid (no left “Project type” tree).
 * Descriptions appear as hover tooltips (`title`). Click selects; OK / double-click opens.
 * Chrome matches other Designer dialogs (`DesignerDialog` Aero frame).
 */
export function NewProjectDialog({ open, onClose, onSelect }: Props) {
  const byCategory = templatesByCategory();
  const [selectedId, setSelectedId] = useState(PROJECT_TEMPLATES[0]?.id ?? "empty");

  useEffect(() => {
    if (!open) return;
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") onClose();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [open, onClose]);

  if (!open) return null;

  const selected = PROJECT_TEMPLATES.find((t) => t.id === selectedId) ?? PROJECT_TEMPLATES[0] ?? null;

  const confirm = () => {
    if (!selected) return;
    onSelect(selected);
  };

  return (
    <DesignerDialog
      title="New Project"
      titleId="new-project-title"
      onClose={onClose}
      closeOnBackdrop
      className="new-project-dialog"
      overlayClassName="new-project-overlay"
      bodyClassName="new-project-body"
      footer={
        <>
          <button type="button" disabled={!selected} onClick={confirm}>
            OK
          </button>
          <button type="button" onClick={onClose}>
            Cancel
          </button>
        </>
      }
    >
      <div className="new-project-templates-pane">
        <div className="new-project-templates-label">Templates:</div>
        <div className="new-project-list legacy-scrollbar">
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
    </DesignerDialog>
  );
}
