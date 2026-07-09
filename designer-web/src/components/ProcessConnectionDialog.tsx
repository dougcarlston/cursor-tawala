import { useState } from "react";
import { createPortal } from "react-dom";
import { formLinksForProcess } from "@/lib/projectModel";
import { useProjectStore } from "@/store/projectStore";

interface Props {
  processName: string;
  onClose: () => void;
}

function ConnectionSlot({
  role,
  formName,
  occupant,
  processName,
  onDisconnect,
}: {
  role: "Pre" | "Post";
  formName: string;
  occupant: string | undefined;
  processName: string;
  onDisconnect: (formName: string, role: "Pre" | "Post") => void;
}) {
  const label = role === "Pre" ? "Pre-process" : "Post-process";
  const hint = role === "Pre" ? "runs when form loads" : "runs after form submit";

  if (!occupant) {
    return (
      <div className="process-connection-slot process-connection-slot-empty">
        <span className="process-connection-slot-role">{label}</span>
        <span className="process-connection-slot-detail">(not connected)</span>
        <span className="process-connection-slot-hint">{hint}</span>
      </div>
    );
  }

  const isThis = occupant === processName;
  return (
    <div className={`process-connection-slot${isThis ? " process-connection-slot-mine" : ""}`}>
      <span className="process-connection-slot-role">{label}</span>
      <span className="process-connection-slot-detail">
        {isThis ? "this process" : `'${occupant}'`}
      </span>
      <span className="process-connection-slot-hint">{hint}</span>
      {isThis ? (
        <button
          type="button"
          className="process-connection-disconnect"
          onClick={() => onDisconnect(formName, role)}
        >
          Disconnect {label}
        </button>
      ) : null}
    </div>
  );
}

/**
 * Legacy "Click here to change" popup — attach this process to a form as Pre- or Post-process,
 * or disconnect either role independently.
 */
export function ProcessConnectionDialog({ processName, onClose }: Props) {
  const project = useProjectStore((s) => s.project);
  const linkProcessToForm = useProjectStore((s) => s.linkProcessToForm);
  const unlinkProcessFromForm = useProjectStore((s) => s.unlinkProcessFromForm);
  const links = formLinksForProcess(project, processName);
  const [formName, setFormName] = useState(project.forms[0]?.name ?? "");

  const selectedForm = project.forms.find((f) => f.name === formName);
  const preOccupant = selectedForm?.preProcess;
  const postOccupant = selectedForm?.process;
  const preIsThis = preOccupant === processName;
  const postIsThis = postOccupant === processName;

  const preLinks = links.filter((l) => l.role === "Pre");
  const postLinks = links.filter((l) => l.role === "Post");

  const attach = (role: "Pre" | "Post") => {
    if (!formName) return;
    linkProcessToForm(processName, formName, role);
  };

  const disconnect = (targetForm: string, role: "Pre" | "Post") => {
    unlinkProcessFromForm(processName, targetForm, role);
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
          {links.length > 0 ? (
            <div className="process-connection-current">
              <p className="process-connection-section-label">Current connections</p>
              {preLinks.length > 0 ? (
                <div className="process-connection-role-group">
                  <p className="process-connection-role-heading">Pre-process</p>
                  <ul>
                    {preLinks.map((link) => (
                      <li key={`pre-${link.formName}`}>
                        <span>Form &apos;{link.formName}&apos;</span>
                        <button
                          type="button"
                          className="process-connection-disconnect"
                          onClick={() => disconnect(link.formName, "Pre")}
                        >
                          Disconnect Pre
                        </button>
                      </li>
                    ))}
                  </ul>
                </div>
              ) : null}
              {postLinks.length > 0 ? (
                <div className="process-connection-role-group">
                  <p className="process-connection-role-heading">Post-process</p>
                  <ul>
                    {postLinks.map((link) => (
                      <li key={`post-${link.formName}`}>
                        <span>Form &apos;{link.formName}&apos;</span>
                        <button
                          type="button"
                          className="process-connection-disconnect"
                          onClick={() => disconnect(link.formName, "Post")}
                        >
                          Disconnect Post
                        </button>
                      </li>
                    ))}
                  </ul>
                </div>
              ) : null}
            </div>
          ) : (
            <p className="hint">This process is not connected to any form.</p>
          )}

          <div className="process-connection-attach">
            <p className="process-connection-section-label">Attach to form</p>
            <label>
              Form
              <select value={formName} onChange={(e) => setFormName(e.target.value)}>
                {project.forms.map((f) => (
                  <option key={f.name} value={f.name}>
                    {f.name}
                  </option>
                ))}
              </select>
            </label>

            {selectedForm ? (
              <div className="process-connection-slots">
                <ConnectionSlot
                  role="Pre"
                  formName={formName}
                  occupant={preOccupant}
                  processName={processName}
                  onDisconnect={disconnect}
                />
                <ConnectionSlot
                  role="Post"
                  formName={formName}
                  occupant={postOccupant}
                  processName={processName}
                  onDisconnect={disconnect}
                />
              </div>
            ) : null}

            <div className="process-connection-attach-actions">
              <button
                type="button"
                onClick={() => attach("Pre")}
                disabled={!formName || preIsThis}
              >
                Attach as Pre-process
              </button>
              <button
                type="button"
                onClick={() => attach("Post")}
                disabled={!formName || postIsThis}
              >
                Attach as Post-process
              </button>
            </div>
            {preOccupant && !preIsThis ? (
              <p className="hint process-connection-replace-hint">
                Attaching as Pre-process will replace &apos;{preOccupant}&apos; on this form.
              </p>
            ) : null}
            {postOccupant && !postIsThis ? (
              <p className="hint process-connection-replace-hint">
                Attaching as Post-process will replace &apos;{postOccupant}&apos; on this form.
              </p>
            ) : null}
          </div>
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
