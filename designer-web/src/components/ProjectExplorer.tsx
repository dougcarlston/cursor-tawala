import { useState, type ReactNode } from "react";
import { useProjectStore } from "@/store/projectStore";
import type { Selection } from "@/types/tawala";
import { linkedProcessesForForm } from "@/lib/projectModel";

export function ProjectExplorer() {
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const setSelection = useProjectStore((s) => s.setSelection);
  const addForm = useProjectStore((s) => s.addForm);
  const addProcess = useProjectStore((s) => s.addProcess);
  const addDocument = useProjectStore((s) => s.addDocument);

  const [expanded, setExpanded] = useState({
    forms: true,
    processes: true,
    documents: true,
  });
  // Owner Q3: on project open every form node starts expanded so linked Pre/Post processes
  // are visible. Session-only state (not saved to the project); resets when the form set changes.
  const [expandedForms, setExpandedForms] = useState<Set<string>>(
    () => new Set(project.forms.map((f) => f.name)),
  );

  const formSignature = project.forms.map((f) => f.name).join("|");
  const [prevSignature, setPrevSignature] = useState(formSignature);
  if (formSignature !== prevSignature) {
    setPrevSignature(formSignature);
    setExpandedForms(new Set(project.forms.map((f) => f.name)));
  }

  const toggle = (key: keyof typeof expanded) =>
    setExpanded((e) => ({ ...e, [key]: !e[key] }));

  const toggleForm = (name: string) =>
    setExpandedForms((prev) => {
      const next = new Set(prev);
      if (next.has(name)) next.delete(name);
      else next.add(name);
      return next;
    });

  const isSelected = (sel: Selection) =>
    selection.kind === sel.kind && selection.name === sel.name;

  return (
    <>
      <div className="panel-title">Project Explorer</div>
      <div className="explorer-toolbar">
        <button type="button" title="New Form" onClick={addForm}>
          F
        </button>
        <button type="button" title="New Process" onClick={addProcess}>
          P
        </button>
        <button type="button" title="New Document" onClick={addDocument}>
          D
        </button>
      </div>
      <div className="tree">
        <div className="tree-root">
          <TreeNode
            label={project.name}
            expanded
            onToggle={() => {}}
            selected={false}
            onSelect={() => setSelection({ kind: "forms" })}
            icon={<ProjectIcon />}
          />
          <ul>
            <li>
              <TreeNode
                label="Forms"
                expanded={expanded.forms}
                onToggle={() => toggle("forms")}
                selected={selection.kind === "forms"}
                onSelect={() => setSelection({ kind: "forms" })}
                icon={<FolderIcon />}
              />
              {expanded.forms && (
                <ul>
                  {project.forms.map((form) => {
                    const links = linkedProcessesForForm(form, project.processes);
                    const formExpanded = expandedForms.has(form.name);
                    return (
                      <li key={form.name}>
                        <TreeNode
                          label={`${form.name}${form.startPoint ? " ★" : ""}`}
                          expanded={formExpanded}
                          onToggle={() => toggleForm(form.name)}
                          selected={isSelected({ kind: "form", name: form.name })}
                          onSelect={() => setSelection({ kind: "form", name: form.name })}
                          leaf={links.length === 0}
                          icon={<FormNodeIcon />}
                        />
                        {formExpanded && links.length > 0 && (
                          <ul>
                            {links.map((link) => (
                              <li key={`${form.name}:${link.role}:${link.name}`}>
                                <TreeNode
                                  label={link.name}
                                  expanded={false}
                                  onToggle={() => {}}
                                  selected={isSelected({ kind: "process", name: link.name })}
                                  onSelect={() =>
                                    setSelection({ kind: "process", name: link.name })
                                  }
                                  leaf
                                  icon={
                                    link.role === "Pre" ? (
                                      <PreProcessIcon />
                                    ) : (
                                      <PostProcessIcon />
                                    )
                                  }
                                />
                              </li>
                            ))}
                          </ul>
                        )}
                      </li>
                    );
                  })}
                </ul>
              )}
            </li>
            <li>
              <TreeNode
                label="Processes"
                expanded={expanded.processes}
                onToggle={() => toggle("processes")}
                selected={selection.kind === "processes"}
                onSelect={() => setSelection({ kind: "processes" })}
                icon={<FolderIcon />}
              />
              {expanded.processes && (
                <ul>
                  {(project.processes ?? []).map((proc) => (
                    <li key={proc.name}>
                      <TreeNode
                        label={proc.name}
                        expanded={false}
                        onToggle={() => {}}
                        selected={isSelected({ kind: "process", name: proc.name })}
                        onSelect={() =>
                          setSelection({ kind: "process", name: proc.name })
                        }
                        leaf
                        icon={<PreProcessIcon />}
                      />
                    </li>
                  ))}
                </ul>
              )}
            </li>
            <li>
              <TreeNode
                label="Documents"
                expanded={expanded.documents}
                onToggle={() => toggle("documents")}
                selected={selection.kind === "documents"}
                onSelect={() => setSelection({ kind: "documents" })}
                icon={<FolderIcon />}
              />
              {expanded.documents && (
                <ul>
                  {(project.documents ?? []).map((doc) => (
                    <li key={doc.name}>
                      <TreeNode
                        label={doc.name}
                        expanded={false}
                        onToggle={() => {}}
                        selected={isSelected({ kind: "document", name: doc.name })}
                        onSelect={() =>
                          setSelection({ kind: "document", name: doc.name })
                        }
                        leaf
                        icon={<DocumentIcon />}
                      />
                    </li>
                  ))}
                </ul>
              )}
            </li>
          </ul>
        </div>
      </div>
    </>
  );
}

function TreeNode({
  label,
  expanded,
  onToggle,
  selected,
  onSelect,
  leaf,
  icon,
}: {
  label: string;
  expanded: boolean;
  onToggle: () => void;
  selected: boolean;
  onSelect: () => void;
  leaf?: boolean;
  icon?: ReactNode;
}) {
  return (
    <div
      className={`tree-node${selected ? " selected" : ""}`}
      onClick={onSelect}
      onDoubleClick={(e) => {
        if (!leaf) {
          e.stopPropagation();
          onToggle();
        }
      }}
    >
      <button
        type="button"
        className="tree-collapse-toggle"
        disabled={leaf}
        aria-hidden={leaf}
        aria-expanded={leaf ? undefined : expanded}
        aria-label={leaf ? undefined : expanded ? `Collapse ${label}` : `Expand ${label}`}
        onClick={(e) => {
          if (!leaf) {
            e.stopPropagation();
            onToggle();
          }
        }}
      >
        {leaf ? "\u00a0\u00a0\u00a0" : expanded ? "[-]" : "[+]"}
      </button>
      {icon ? (
        <span className="tree-icon" aria-hidden>
          {icon}
        </span>
      ) : null}
      <span className="tree-label">{label}</span>
    </div>
  );
}

/** Project root — stacked-window "project" mark. */
function ProjectIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="3" y="2.5" width="9" height="7" rx="1" fill="#eef3fb" stroke="#4a6fa5" />
      <rect x="4.5" y="6.5" width="9" height="7" rx="1" fill="#fff" stroke="#4a6fa5" />
      <rect x="4.5" y="6.5" width="9" height="2" fill="#4a6fa5" />
    </svg>
  );
}

/** Section folder (Forms / Processes / Documents). */
function FolderIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path
        d="M1.5 4.5a1 1 0 0 1 1-1h3l1.2 1.4h6.8a1 1 0 0 1 1 1v6.1a1 1 0 0 1-1 1h-11a1 1 0 0 1-1-1z"
        fill="#f6c95b"
        stroke="#c48f16"
        strokeWidth="0.8"
      />
    </svg>
  );
}

/** Form node — small window with internal grid lines (legacy `Form_InTree`). */
function FormNodeIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="2" y="2.5" width="12" height="11" rx="1" fill="#ffffff" stroke="#4a6fa5" />
      <rect x="2" y="2.5" width="12" height="2.6" fill="#4a6fa5" />
      <line x1="2" y1="8.4" x2="14" y2="8.4" stroke="#9db4d4" />
      <line x1="6" y1="5.1" x2="6" y2="13.5" stroke="#9db4d4" />
      <line x1="10" y1="5.1" x2="10" y2="13.5" stroke="#9db4d4" />
    </svg>
  );
}

const GEAR_PATH =
  "M19.14 12.94c.04-.3.06-.61.06-.94 0-.32-.02-.64-.07-.94l2.03-1.58a.49.49 0 0 0 .12-.61l-1.92-3.32a.488.488 0 0 0-.59-.22l-2.39.96c-.5-.38-1.03-.7-1.62-.94l-.36-2.54a.484.484 0 0 0-.48-.41h-3.84c-.24 0-.43.17-.47.41l-.36 2.54c-.59.24-1.13.57-1.62.94l-2.39-.96c-.22-.08-.47 0-.59.22L2.74 8.87c-.12.21-.08.47.12.61l2.03 1.58c-.05.3-.09.63-.09.94s.02.64.07.94l-2.03 1.58a.49.49 0 0 0-.12.61l1.92 3.32c.12.22.37.29.59.22l2.39-.96c.5.38 1.03.7 1.62.94l.36 2.54c.05.24.24.41.48.41h3.84c.24 0 .44-.17.47-.41l.36-2.54c.59-.24 1.13-.56 1.62-.94l2.39.96c.22.08.47 0 .59-.22l1.92-3.32c.12-.22.07-.47-.12-.61l-2.01-1.58zM12 15.6c-1.98 0-3.6-1.62-3.6-3.6s1.62-3.6 3.6-3.6 3.6 1.62 3.6 3.6-1.62 3.6-3.6 3.6z";

/** Linked Pre-process (or standalone process) — solid purple gear (legacy `Form_PreProcess`). */
function PreProcessIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" aria-hidden focusable="false">
      <path d={GEAR_PATH} fill="#8a2be2" />
    </svg>
  );
}

/** Linked Post-process — form icon with a small purple gear overlaid (legacy `Form_PostProcess`). */
function PostProcessIcon() {
  return (
    <span className="tree-icon-overlay">
      <FormNodeIcon />
      <svg
        className="tree-icon-gear-overlay"
        width="10"
        height="10"
        viewBox="0 0 24 24"
        aria-hidden
        focusable="false"
      >
        <circle cx="12" cy="12" r="11" fill="#ffffff" />
        <path d={GEAR_PATH} fill="#8a2be2" />
      </svg>
    </span>
  );
}

/** Document node. */
function DocumentIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path
        d="M4 1.5h5.5L13 5v9.5a.5.5 0 0 1-.5.5h-8a.5.5 0 0 1-.5-.5z"
        fill="#ffffff"
        stroke="#7a7a7a"
        strokeWidth="0.8"
      />
      <path d="M9.5 1.7v3.3H13" fill="none" stroke="#7a7a7a" strokeWidth="0.8" />
      <line x1="5.6" y1="8" x2="11" y2="8" stroke="#9aa0a6" />
      <line x1="5.6" y1="10" x2="11" y2="10" stroke="#9aa0a6" />
      <line x1="5.6" y1="12" x2="9.5" y2="12" stroke="#9aa0a6" />
    </svg>
  );
}
