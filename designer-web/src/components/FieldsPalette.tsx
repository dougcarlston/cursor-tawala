import { useMemo, useState, useSyncExternalStore } from "react";
import type { TawalaProcessCommand, TawalaProject } from "@/types/tawala";
import {
  collectProjectVariables,
  formFieldNames,
  type FieldLeaf,
} from "@/lib/projectModel";
import {
  fieldInsertText,
  fieldLeafAcceptedByActiveTarget,
  getActiveFieldTargetContextSnapshot,
  insertFieldIntoActiveTarget,
  paletteLeafInsertName,
  setFieldDragActive,
  setFieldDragData,
  subscribeActiveFieldTargetContext,
} from "@/lib/fieldInsertion";
import { setFieldsPaletteSelection } from "@/lib/fieldsPaletteSelection";
import {
  recordBranchForConditionsForm,
  recordBranchesAtInsertPath,
  type RecordPaletteBranch,
} from "@/lib/processRecordContext";
import type { InsertPath } from "@/lib/skipInsertPath";

interface FormBranch {
  name: string;
  fields: FieldLeaf[];
  startPoint?: boolean;
}

export interface ProcessRecordPaletteContext {
  commands: TawalaProcessCommand[];
  insertPath: InsertPath;
}

/**
 * Right-hand Fields tree (legacy `FieldsPalette.cs` parity):
 * form branches, ForEach record branches (when insertion is inside a loop),
 * Show Stored Record `Record:` branch, and Variables.
 */
export function FieldsPalette({
  project,
  activeFormName,
  processRecordContext = null,
  conditionsRecordForm = null,
}: {
  project: TawalaProject;
  activeFormName?: string;
  processRecordContext?: ProcessRecordPaletteContext | null;
  /** Show → Stored Record tab: single `Record:` branch for the selected form. */
  conditionsRecordForm?: string | null;
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

  const recordBranches: RecordPaletteBranch[] = useMemo(() => {
    if (conditionsRecordForm?.trim()) {
      const branch = recordBranchForConditionsForm(conditionsRecordForm, project);
      return branch ? [branch] : [];
    }
    if (!processRecordContext) return [];
    return recordBranchesAtInsertPath(
      processRecordContext.commands,
      processRecordContext.insertPath,
      project,
    );
  }, [conditionsRecordForm, processRecordContext, project]);

  const variables = useMemo(() => collectProjectVariables(project), [project]);
  const activeFieldContext = useSyncExternalStore(
    subscribeActiveFieldTargetContext,
    getActiveFieldTargetContextSnapshot,
  );
  const variablesDisabled = activeFieldContext.formFieldsOnly === true;

  const defaultCollapsed = () => {
    const initial = new Set<string>();
    for (const form of project.forms) initial.add(`form:${form.name}`);
    for (const branch of recordBranches) initial.add(`record:${branch.recordName}`);
    initial.add("node:Variables");
    return initial;
  };

  const [collapsed, setCollapsed] = useState<Set<string>>(defaultCollapsed);
  const [selectedKey, setSelectedKey] = useState<string | null>(null);

  const formSignature = project.forms.map((f) => f.name).join("|");
  const recordSignature = recordBranches.map((b) => b.recordName).join("|");
  const treeSignature = `${formSignature}|${recordSignature}|${conditionsRecordForm ?? ""}|${processRecordContext?.insertPath ?? ""}`;
  const [prevSignature, setPrevSignature] = useState(treeSignature);
  if (treeSignature !== prevSignature) {
    setPrevSignature(treeSignature);
    setCollapsed(defaultCollapsed());
    setSelectedKey(null);
    setFieldsPaletteSelection(null);
  }

  const isOpen = (key: string) => !collapsed.has(key);
  const toggle = (key: string) =>
    setCollapsed((prev) => {
      const next = new Set(prev);
      if (next.has(key)) next.delete(key);
      else next.add(key);
      return next;
    });

  if (project.forms.length === 0 && variables.length === 0 && recordBranches.length === 0) {
    return <p className="hint fields-tree-empty">No forms or variables yet.</p>;
  }

  return (
    <div
      className="fields-tree"
      role="tree"
      aria-label="Fields"
      onKeyDown={(e) => {
        if (e.key === "Escape") {
          setSelectedKey(null);
          setFieldsPaletteSelection(null);
        }
      }}
      onClick={(e) => {
        // Click empty tree chrome (not a leaf) clears selection so Insert → Field greys out.
        const t = e.target as HTMLElement;
        if (t.closest(".fields-leaf")) return;
        if (t.closest(".fields-tree-toggle")) return;
        setSelectedKey(null);
        setFieldsPaletteSelection(null);
      }}
    >
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
              onToggle={() => {
                setSelectedKey(null);
                setFieldsPaletteSelection(null);
                toggle(key);
              }}
            />
            {open && branch.fields.length > 0 ? (
              <ul className="fields-leaf-list">
                {branch.fields.map((field) => {
                  const leafKey = `${key}::${field.name}`;
                  return (
                    <FieldLeafRow
                      key={leafKey}
                      leaf={field}
                      formName={branch.name}
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

      {recordBranches.map((branch) => {
        const key = `record:${branch.recordName}`;
        const open = isOpen(key);
        return (
          <div key={key} className="fields-branch fields-record-branch">
            <BranchHeader
              label={branch.recordName}
              open={open}
              hasChildren={branch.leaves.length > 0}
              record
              onToggle={() => {
                setSelectedKey(null);
                setFieldsPaletteSelection(null);
                toggle(key);
              }}
            />
            {open && branch.leaves.length > 0 ? (
              <ul className="fields-leaf-list">
                {branch.leaves.map((leaf) => {
                  const leafKey = `${key}::${leaf.name}`;
                  return (
                    <RecordFieldLeafRow
                      key={leafKey}
                      label={leaf.name}
                      insertName={leaf.insertName}
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
            onToggle={() => {
              setSelectedKey(null);
              setFieldsPaletteSelection(null);
              toggle("node:Variables");
            }}
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
                    disabled={variablesDisabled}
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
  record,
  onToggle,
}: {
  label: string;
  open: boolean;
  hasChildren: boolean;
  active?: boolean;
  star?: boolean;
  record?: boolean;
  onToggle: () => void;
}) {
  return (
    <div
      className={`fields-branch-header${active ? " active" : ""}${record ? " fields-record-header" : ""}`}
    >
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
  formName,
  selected,
  disabled,
  onSelect,
}: {
  leaf: FieldLeaf;
  formName?: string;
  selected: boolean;
  disabled?: boolean;
  onSelect: () => void;
}) {
  const activeFieldContext = useSyncExternalStore(
    subscribeActiveFieldTargetContext,
    getActiveFieldTargetContextSnapshot,
  );
  const insertName = paletteLeafInsertName(leaf.dragValue, formName, activeFieldContext);
  const canInsert = !disabled && fieldLeafAcceptedByActiveTarget(insertName);
  const preview = fieldInsertText(insertName, activeFieldContext);
  return (
    <li>
      <span
        className={`fields-leaf${selected ? " selected" : ""}${disabled ? " fields-leaf-disabled" : ""}`}
        role="treeitem"
        aria-selected={selected}
        aria-disabled={disabled || undefined}
        tabIndex={disabled ? -1 : 0}
        draggable={!disabled}
        title={
          disabled
            ? "Variables cannot be used in this field"
            : `Drag or double-click to insert ${preview}`
        }
        onClick={(e) => {
          e.stopPropagation();
          onSelect();
          setFieldsPaletteSelection(insertName);
        }}
        onDoubleClick={(e) => {
          e.stopPropagation();
          if (!canInsert) return;
          onSelect();
          setFieldsPaletteSelection(insertName);
          insertFieldIntoActiveTarget(insertName);
        }}
        onKeyDown={(e) => {
          if (disabled) return;
          if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            e.stopPropagation();
            onSelect();
            setFieldsPaletteSelection(insertName);
            if (canInsert) insertFieldIntoActiveTarget(insertName);
          }
        }}
        onDragStart={(ev) => {
          if (disabled) {
            ev.preventDefault();
            return;
          }
          ev.stopPropagation();
          setFieldsPaletteSelection(insertName);
          setFieldDragActive(true);
          setFieldDragData(ev.dataTransfer, leaf.dragValue, formName);
        }}
        onDragEnd={() => {
          setFieldDragActive(false);
        }}
      >
        {leaf.name}
      </span>
    </li>
  );
}

function RecordFieldLeafRow({
  label,
  insertName,
  selected,
  onSelect,
}: {
  label: string;
  insertName: string;
  selected: boolean;
  onSelect: () => void;
}) {
  const activeFieldContext = useSyncExternalStore(
    subscribeActiveFieldTargetContext,
    getActiveFieldTargetContextSnapshot,
  );
  const canInsert = fieldLeafAcceptedByActiveTarget(insertName);
  const preview = fieldInsertText(insertName, activeFieldContext);
  return (
    <li>
      <span
        className={`fields-leaf fields-record-leaf${selected ? " selected" : ""}`}
        role="treeitem"
        aria-selected={selected}
        tabIndex={0}
        draggable
        title={`Drag or double-click to insert ${preview}`}
        onClick={(e) => {
          e.stopPropagation();
          onSelect();
          setFieldsPaletteSelection(insertName);
        }}
        onDoubleClick={(e) => {
          e.stopPropagation();
          if (!canInsert) return;
          onSelect();
          setFieldsPaletteSelection(insertName);
          insertFieldIntoActiveTarget(insertName);
        }}
        onKeyDown={(e) => {
          if (e.key === "Enter" || e.key === " ") {
            e.preventDefault();
            e.stopPropagation();
            onSelect();
            setFieldsPaletteSelection(insertName);
            if (canInsert) insertFieldIntoActiveTarget(insertName);
          }
        }}
        onDragStart={(ev) => {
          ev.stopPropagation();
          setFieldsPaletteSelection(insertName);
          setFieldDragActive(true);
          setFieldDragData(ev.dataTransfer, insertName);
        }}
        onDragEnd={() => {
          setFieldDragActive(false);
        }}
      >
        {label}
      </span>
    </li>
  );
}
