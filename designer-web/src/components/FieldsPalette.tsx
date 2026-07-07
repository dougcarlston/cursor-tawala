import { useMemo, useState } from "react";
import type { TawalaProject } from "@/types/tawala";
import {
  collectProjectVariables,
  formFieldNames,
  type FieldLeaf,
} from "@/lib/projectModel";

interface FormBranch {
  name: string;
  fields: FieldLeaf[];
  startPoint?: boolean;
}

/**
 * Right-hand Fields tree (legacy `FieldsPalette.cs` parity):
 * one branch per form (all forms, field-name leaves) plus a Variables branch.
 * Per-node `[-]`/`[+]` collapse keeps DirtBowl-scale lists usable; leaves are drag
 * sources (editor drop targets are Phase 2). Selection highlight tracks the active leaf.
 */
export function FieldsPalette({
  project,
  activeFormName,
}: {
  project: TawalaProject;
  activeFormName?: string;
}) {
  const branches: FormBranch[] = useMemo(
    () =>
      project.forms.map((form) => ({
        name: form.name,
        fields: formFieldNames(form),
        startPoint: form.startPoint,
      })),
    [project.forms],
  );

  const variables = useMemo(() => collectProjectVariables(project), [project]);

  // Owner Q3: on project open every form folder AND Variables start collapsed so large
  // projects (DirtBowl) stay scannable. Collapse state is session-only (useState), never
  // saved to the project file. Toggling in-session persists until the form set changes.
  const defaultCollapsed = () => {
    const initial = new Set<string>();
    for (const form of project.forms) initial.add(`form:${form.name}`);
    initial.add("node:Variables");
    return initial;
  };

  const [collapsed, setCollapsed] = useState<Set<string>>(defaultCollapsed);
  const [selectedKey, setSelectedKey] = useState<string | null>(null);

  // Reset collapse defaults when the form set changes (e.g. a new project is loaded),
  // so a freshly opened DirtBowl-scale project isn't rendered fully expanded.
  const formSignature = project.forms.map((f) => f.name).join("|");
  const [prevSignature, setPrevSignature] = useState(formSignature);
  if (formSignature !== prevSignature) {
    setPrevSignature(formSignature);
    setCollapsed(defaultCollapsed());
    setSelectedKey(null);
  }

  const isOpen = (key: string) => !collapsed.has(key);
  const toggle = (key: string) =>
    setCollapsed((prev) => {
      const next = new Set(prev);
      if (next.has(key)) next.delete(key);
      else next.add(key);
      return next;
    });

  if (project.forms.length === 0 && variables.length === 0) {
    return <p className="hint fields-tree-empty">No forms or variables yet.</p>;
  }

  return (
    <div className="fields-tree" role="tree" aria-label="Fields">
      {branches.map((branch) => {
        const key = `form:${branch.name}`;
        const open = isOpen(key);
        return (
          <div key={key} className="fields-branch">
            <BranchHeader
              label={branch.name}
              open={open}
              hasChildren={branch.fields.length > 0}
              active={branch.name === activeFormName}
              star={branch.startPoint}
              onToggle={() => toggle(key)}
            />
            {open && branch.fields.length > 0 ? (
              <ul className="fields-leaf-list">
                {branch.fields.map((field) => {
                  const leafKey = `${key}::${field.name}`;
                  return (
                    <FieldLeafRow
                      key={leafKey}
                      leaf={field}
                      selected={selectedKey === leafKey}
                      onSelect={() => setSelectedKey(leafKey)}
                    />
                  );
                })}
              </ul>
            ) : null}
          </div>
        );
      })}

      {variables.length > 0 ? (
        <div className="fields-branch">
          <BranchHeader
            label="Variables"
            open={isOpen("node:Variables")}
            hasChildren
            onToggle={() => toggle("node:Variables")}
          />
          {isOpen("node:Variables") ? (
            <ul className="fields-leaf-list">
              {variables.map((name) => {
                const leafKey = `var::${name}`;
                return (
                  <FieldLeafRow
                    key={leafKey}
                    leaf={{ name, dragValue: name }}
                    selected={selectedKey === leafKey}
                    onSelect={() => setSelectedKey(leafKey)}
                  />
                );
              })}
            </ul>
          ) : null}
        </div>
      ) : null}
    </div>
  );
}

function BranchHeader({
  label,
  open,
  hasChildren,
  active,
  star,
  onToggle,
}: {
  label: string;
  open: boolean;
  hasChildren: boolean;
  active?: boolean;
  star?: boolean;
  onToggle: () => void;
}) {
  return (
    <div className={`fields-branch-header${active ? " active" : ""}`}>
      <button
        type="button"
        className="fields-tree-toggle"
        onClick={onToggle}
        aria-expanded={open}
        aria-label={open ? `Collapse ${label}` : `Expand ${label}`}
        disabled={!hasChildren}
      >
        {hasChildren ? (open ? "[-]" : "[+]") : "\u00a0\u00a0\u00a0"}
      </button>
      <span className="fields-branch-name" title={label}>
        {label}
        {star ? <span className="fields-branch-star"> ★</span> : null}
      </span>
    </div>
  );
}

function FieldLeafRow({
  leaf,
  selected,
  onSelect,
}: {
  leaf: FieldLeaf;
  selected: boolean;
  onSelect: () => void;
}) {
  return (
    <li>
      <span
        className={`fields-leaf${selected ? " selected" : ""}`}
        role="treeitem"
        aria-selected={selected}
        tabIndex={0}
        draggable
        title={leaf.dragValue}
        onClick={onSelect}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            onSelect();
          }
        }}
        onDragStart={(ev) => {
          ev.dataTransfer.setData("text/plain", leaf.dragValue);
          ev.dataTransfer.effectAllowed = "copy";
        }}
      >
        {leaf.name}
      </span>
    </li>
  );
}
