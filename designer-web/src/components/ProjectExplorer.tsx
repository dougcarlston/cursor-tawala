import { useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import type { Selection } from "@/types/tawala";

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

  const toggle = (key: keyof typeof expanded) =>
    setExpanded((e) => ({ ...e, [key]: !e[key] }));

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
          />
          <ul>
            <li>
              <TreeNode
                label="Forms"
                expanded={expanded.forms}
                onToggle={() => toggle("forms")}
                selected={selection.kind === "forms"}
                onSelect={() => setSelection({ kind: "forms" })}
              />
              {expanded.forms && (
                <ul>
                  {project.forms.map((form) => (
                    <li key={form.name}>
                      <TreeNode
                        label={`${form.name}${form.startPoint ? " ★" : ""}`}
                        expanded={false}
                        onToggle={() => {}}
                        selected={isSelected({ kind: "form", name: form.name })}
                        onSelect={() => setSelection({ kind: "form", name: form.name })}
                        leaf
                      />
                    </li>
                  ))}
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
}: {
  label: string;
  expanded: boolean;
  onToggle: () => void;
  selected: boolean;
  onSelect: () => void;
  leaf?: boolean;
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
      <span
        className="tree-toggle"
        onClick={(e) => {
          if (!leaf) {
            e.stopPropagation();
            onToggle();
          }
        }}
      >
        {leaf ? "·" : expanded ? "▼" : "▶"}
      </span>
      <span>{label}</span>
    </div>
  );
}
