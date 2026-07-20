import { createPortal } from "react-dom";
import { formLinksForProcess } from "@/lib/projectModel";
import { useProjectStore } from "@/store/projectStore";
import type { TawalaForm } from "@/types/tawala";

interface Props {
  processName: string;
  onClose: () => void;
}

/**
 * Legacy ProcessViewInfoBar popup — checklist of forms for Pre and Post.
 * One process may be Pre and/or Post on many forms (e.g. Potluck "Show Results"
 * is preProcess on both Organizer and Report). Each form still has at most one
 * Pre and one Post slot; a slot occupied by another process stays disabled.
 */
export function ProcessConnectionDialog({ processName, onClose }: Props) {
  const project = useProjectStore((s) => s.project);
  const linkProcessToForm = useProjectStore((s) => s.linkProcessToForm);
  const unlinkProcessFromForm = useProjectStore((s) => s.unlinkProcessFromForm);
  const links = formLinksForProcess(project, processName);
  const forms = project.forms;

  const toggle = (form: TawalaForm, role: "Pre" | "Post", checked: boolean) => {
    if (checked) {
      linkProcessToForm(processName, form.name, role);
    } else {
      unlinkProcessFromForm(processName, form.name, role);
    }
  };

  return createPortal(
    <div
      className="modal-overlay process-connection-modal-overlay"
      role="presentation"
      onClick={onClose}
    >
      <div
        className="modal-dialog process-connection-dialog"
        role="dialog"
        aria-labelledby="process-connection-title"
        aria-modal="true"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="modal-header">
          <h2 id="process-connection-title">Connect Process — {processName}</h2>
          <button type="button" className="modal-close" onClick={onClose} aria-label="Close">
            ×
          </button>
        </div>
        <div className="modal-body process-connection-dialog-body">
          {forms.length === 0 ? (
            <p className="hint">This project has no forms.</p>
          ) : (
            <>
              <p className="hint process-connection-multi-hint">
                Check every form that should run this process. The same process may be
                Pre-process and/or Post-process on more than one form.
              </p>

              <FormRoleChecklist
                title="Pre-process"
                hint="Runs when the form loads"
                role="Pre"
                forms={forms}
                processName={processName}
                occupantOf={(f) => f.preProcess}
                onToggle={toggle}
              />

              <FormRoleChecklist
                title="Post-process"
                hint="Runs after the form is submitted"
                role="Post"
                forms={forms}
                processName={processName}
                occupantOf={(f) => f.process}
                onToggle={toggle}
              />

              {links.length === 0 ? (
                <p className="hint">This process is not connected to any form yet.</p>
              ) : null}
            </>
          )}
        </div>
        <div className="modal-footer">
          <button type="button" onClick={onClose}>
            Close
          </button>
        </div>
      </div>
    </div>,
    document.body,
  );
}

function FormRoleChecklist({
  title,
  hint,
  role,
  forms,
  processName,
  occupantOf,
  onToggle,
}: {
  title: string;
  hint: string;
  role: "Pre" | "Post";
  forms: TawalaForm[];
  processName: string;
  occupantOf: (form: TawalaForm) => string | undefined;
  onToggle: (form: TawalaForm, role: "Pre" | "Post", checked: boolean) => void;
}) {
  return (
    <div className="process-connection-role-group">
      <p className="process-connection-role-heading">
        {title}
        <span className="process-connection-role-hint"> — {hint}</span>
      </p>
      <ul className="process-connection-form-checklist">
        {forms.map((form) => {
          const occupant = occupantOf(form);
          const isThis = occupant === processName;
          const blockedByOther = !!occupant && !isThis;
          return (
            <li key={`${role}-${form.name}`}>
              <label
                className={
                  blockedByOther
                    ? "process-connection-check process-connection-check-blocked"
                    : "process-connection-check"
                }
              >
                <input
                  type="checkbox"
                  checked={isThis}
                  disabled={blockedByOther}
                  onChange={(e) => onToggle(form, role, e.target.checked)}
                />
                <span className="process-connection-check-form">{form.name}</span>
                {blockedByOther ? (
                  <span className="process-connection-check-other">
                    (uses &apos;{occupant}&apos;)
                  </span>
                ) : null}
              </label>
            </li>
          );
        })}
      </ul>
    </div>
  );
}
