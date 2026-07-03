import { TEMPLATE_CATEGORIES, templatesByCategory, TemplateEntry } from "@/templates/catalog";

interface Props {
  open: boolean;
  onClose: () => void;
  onSelect: (template: TemplateEntry) => void;
}

export function NewProjectDialog({ open, onClose, onSelect }: Props) {
  if (!open) return null;

  const byCategory = templatesByCategory();

  return (
    <div className="modal-overlay" role="presentation" onClick={onClose}>
      <div
        className="modal-dialog new-project-dialog"
        role="dialog"
        aria-labelledby="new-project-title"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-header">
          <h2 id="new-project-title">New Project</h2>
          <button type="button" className="modal-close" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>
        <p className="new-project-intro">
          Choose a starter template. Featured apps match the home page samples (Simple Survey, Sign-up
          Sheet, Potluck, Get Together).
        </p>
        <div className="new-project-list">
          {TEMPLATE_CATEGORIES.map((category) => {
            const entries = byCategory.get(category) ?? [];
            if (!entries.length) return null;
            return (
              <section key={category} className="new-project-category">
                <h3>{category}</h3>
                <ul>
                  {entries.map((t) => (
                    <li key={t.id}>
                      <button type="button" className="template-row" onClick={() => onSelect(t)}>
                        <span className="template-label">
                          {t.label}
                          {t.featured ? <span className="template-featured">Featured</span> : null}
                        </span>
                        <span className="template-desc">{t.description}</span>
                      </button>
                    </li>
                  ))}
                </ul>
              </section>
            );
          })}
        </div>
        <div className="modal-footer">
          <button type="button" onClick={onClose}>
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}
