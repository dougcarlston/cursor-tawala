import { useEffect, useRef, useState, type ButtonHTMLAttributes, type ReactNode } from "react";
import { useProjectStore } from "@/store/projectStore";
import type { Selection } from "@/types/tawala";
import { linkedProcessesForForm } from "@/lib/projectModel";

/** Category of a renamable tree leaf. */
type RenameKind = "form" | "process" | "document";

/** Which tree node (if any) is currently in inline-edit mode. */
interface EditingNode {
  /** Unique per-rendered-node key so only the clicked node shows an input. */
  key: string;
  kind: RenameKind;
  name: string;
}

export function ProjectExplorer() {
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const setSelection = useProjectStore((s) => s.setSelection);
  const addForm = useProjectStore((s) => s.addForm);
  const addProcess = useProjectStore((s) => s.addProcess);
  const addDocument = useProjectStore((s) => s.addDocument);
  const toggleFormStartPoint = useProjectStore((s) => s.toggleFormStartPoint);
  const toggleFormBlockBack = useProjectStore((s) => s.toggleFormBlockBack);
  const moveSelectedNode = useProjectStore((s) => s.moveSelectedNode);
  const renameForm = useProjectStore((s) => s.renameForm);
  const renameProcess = useProjectStore((s) => s.renameProcess);
  const renameDocument = useProjectStore((s) => s.renameDocument);

  // Inline rename state. A renamable node enters edit mode after a click-and-hold
  // (long press) on the already-selected row — see TreeNode's press timer.
  const [editing, setEditing] = useState<EditingNode | null>(null);

  // Commit (or cancel) an inline rename. `next === null` cancels (Escape / empty).
  // Duplicate names are rejected by the store, which surfaces a minimal alert.
  const submitRename = (node: EditingNode, next: string | null) => {
    setEditing(null);
    if (next === null) return;
    const trimmed = next.trim();
    if (!trimmed || trimmed === node.name) return;
    const rename =
      node.kind === "form"
        ? renameForm
        : node.kind === "process"
          ? renameProcess
          : renameDocument;
    const ok = rename(node.name, trimmed);
    if (!ok) {
      window.alert(
        `Another ${node.kind} is already named "${trimmed}". The name was left unchanged.`,
      );
    }
  };

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

  // Legacy enable/disable logic (see ProjectExplorer.cs: updateMoveNodeButtonsStatus,
  // showFormSpecificItems, startPointToolStripButton_Click).
  const selectedList =
    selection.kind === "form"
      ? project.forms
      : selection.kind === "process"
        ? project.processes ?? []
        : selection.kind === "document"
          ? project.documents ?? []
          : null;
  const selectedIndex = selectedList
    ? selectedList.findIndex((n) => n.name === selection.name)
    : -1;
  const isLeafNode = selectedList !== null && selectedIndex >= 0;
  const canMoveUp = isLeafNode && selectedIndex > 0;
  const canMoveDown = isLeafNode && selectedIndex < (selectedList?.length ?? 0) - 1;

  const selectedForm =
    selection.kind === "form"
      ? project.forms.find((f) => f.name === selection.name)
      : undefined;
  const isFormSelected = !!selectedForm;

  return (
    <>
      <div className="panel-title">Project Explorer</div>
      <div className="explorer-toolbar" role="toolbar" aria-label="Project Explorer">
        <ToolbarButton tip="New Form" onClick={addForm}>
          <NewFormIcon />
        </ToolbarButton>
        <ToolbarButton tip="New Process" onClick={addProcess}>
          <NewProcessIcon />
        </ToolbarButton>
        <ToolbarButton tip="New Document" onClick={addDocument}>
          <NewDocumentIcon />
        </ToolbarButton>
        <span className="explorer-toolbar-sep" aria-hidden />
        <ToolbarButton tip="Move Node Up" disabled={!canMoveUp} onClick={() => moveSelectedNode("up")}>
          <MoveUpIcon />
        </ToolbarButton>
        <ToolbarButton
          tip="Move Node Down"
          disabled={!canMoveDown}
          onClick={() => moveSelectedNode("down")}
        >
          <MoveDownIcon />
        </ToolbarButton>
        <span className="explorer-toolbar-sep" aria-hidden />
        <ToolbarButton
          tip="Toggle Form Starting Point"
          aria-pressed={isFormSelected ? !!selectedForm?.startPoint : undefined}
          className={selectedForm?.startPoint ? "toggled" : undefined}
          disabled={!isFormSelected}
          onClick={() => selectedForm && toggleFormStartPoint(selectedForm.name)}
        >
          <StartPointIcon />
        </ToolbarButton>
        <ToolbarButton
          tip="Block Back Button"
          aria-pressed={isFormSelected ? !!selectedForm?.blockBackButton : undefined}
          className={selectedForm?.blockBackButton ? "toggled" : undefined}
          disabled={!isFormSelected}
          onClick={() => selectedForm && toggleFormBlockBack(selectedForm.name)}
        >
          <BlockBackIcon />
        </ToolbarButton>
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
                    const formKey = `form:${form.name}`;
                    return (
                      <li key={form.name}>
                        <TreeNode
                          label={form.name}
                          expanded={formExpanded}
                          onToggle={() => toggleForm(form.name)}
                          selected={isSelected({ kind: "form", name: form.name })}
                          onSelect={() => setSelection({ kind: "form", name: form.name })}
                          leaf={links.length === 0}
                          editing={editing?.key === formKey}
                          onBeginRename={() =>
                            setEditing({ key: formKey, kind: "form", name: form.name })
                          }
                          onRenameSubmit={(next) =>
                            submitRename({ key: formKey, kind: "form", name: form.name }, next)
                          }
                          icon={
                            <FormNodeIcon
                              startPoint={form.startPoint}
                              blockBack={form.blockBackButton}
                            />
                          }
                        />
                        {formExpanded && links.length > 0 && (
                          <ul>
                            {links.map((link) => {
                              const linkKey = `${form.name}:${link.role}:${link.name}`;
                              return (
                                <li key={linkKey}>
                                  <TreeNode
                                    label={link.name}
                                    expanded={false}
                                    onToggle={() => {}}
                                    selected={isSelected({ kind: "process", name: link.name })}
                                    onSelect={() =>
                                      setSelection({ kind: "process", name: link.name })
                                    }
                                    leaf
                                    editing={editing?.key === linkKey}
                                    onBeginRename={() =>
                                      setEditing({
                                        key: linkKey,
                                        kind: "process",
                                        name: link.name,
                                      })
                                    }
                                    onRenameSubmit={(next) =>
                                      submitRename(
                                        { key: linkKey, kind: "process", name: link.name },
                                        next,
                                      )
                                    }
                                    icon={
                                      link.role === "Pre" ? (
                                        <PreProcessIcon />
                                      ) : (
                                        <PostProcessIcon />
                                      )
                                    }
                                  />
                                </li>
                              );
                            })}
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
                  {(project.processes ?? []).map((proc) => {
                    const procKey = `process:${proc.name}`;
                    return (
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
                          editing={editing?.key === procKey}
                          onBeginRename={() =>
                            setEditing({ key: procKey, kind: "process", name: proc.name })
                          }
                          onRenameSubmit={(next) =>
                            submitRename(
                              { key: procKey, kind: "process", name: proc.name },
                              next,
                            )
                          }
                          icon={<PreProcessIcon />}
                        />
                      </li>
                    );
                  })}
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
                  {(project.documents ?? []).map((doc) => {
                    const docKey = `document:${doc.name}`;
                    return (
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
                          editing={editing?.key === docKey}
                          onBeginRename={() =>
                            setEditing({ key: docKey, kind: "document", name: doc.name })
                          }
                          onRenameSubmit={(next) =>
                            submitRename(
                              { key: docKey, kind: "document", name: doc.name },
                              next,
                            )
                          }
                          icon={<DocumentIcon />}
                        />
                      </li>
                    );
                  })}
                </ul>
              )}
            </li>
          </ul>
        </div>
      </div>
    </>
  );
}

/**
 * Explorer toolbar button with a custom hover tooltip. Native `title` tooltips
 * do not appear on disabled buttons in most browsers, and the owner needs the
 * tooltip on greyed buttons too. The tip lives on a wrapping span (via
 * `data-tip` + CSS `::after`); the disabled inner button uses
 * `pointer-events: none` (see styles.css) so hover still reaches the wrapper.
 */
function ToolbarButton({
  tip,
  children,
  ...props
}: { tip: string } & ButtonHTMLAttributes<HTMLButtonElement>) {
  return (
    <span className="explorer-tip" data-tip={tip}>
      <button type="button" aria-label={tip} {...props}>
        {children}
      </button>
    </span>
  );
}

/** Long-press threshold (ms) for click-and-hold rename (legacy BeginEdit). */
const RENAME_HOLD_MS = 500;
/** Pointer movement (px) that cancels a pending long press (treats it as a drag). */
const RENAME_MOVE_TOLERANCE = 4;

function TreeNode({
  label,
  expanded,
  onToggle,
  selected,
  onSelect,
  leaf,
  icon,
  editing,
  onBeginRename,
  onRenameSubmit,
}: {
  label: string;
  expanded: boolean;
  onToggle: () => void;
  selected: boolean;
  onSelect: () => void;
  leaf?: boolean;
  icon?: ReactNode;
  editing?: boolean;
  onBeginRename?: () => void;
  onRenameSubmit?: (next: string | null) => void;
}) {
  const renamable = !!onBeginRename;
  const holdTimer = useRef<number | null>(null);
  const pressOrigin = useRef<{ x: number; y: number } | null>(null);

  const clearHold = () => {
    if (holdTimer.current !== null) {
      window.clearTimeout(holdTimer.current);
      holdTimer.current = null;
    }
    pressOrigin.current = null;
  };

  useEffect(() => clearHold, []);

  // Click-and-hold on an already-selected renamable row starts inline edit.
  const handleMouseDown = (e: React.MouseEvent) => {
    if (e.button !== 0 || !renamable || !selected || editing) return;
    pressOrigin.current = { x: e.clientX, y: e.clientY };
    holdTimer.current = window.setTimeout(() => {
      onBeginRename?.();
      clearHold();
    }, RENAME_HOLD_MS);
  };

  const handleMouseMove = (e: React.MouseEvent) => {
    if (holdTimer.current === null || !pressOrigin.current) return;
    const dx = Math.abs(e.clientX - pressOrigin.current.x);
    const dy = Math.abs(e.clientY - pressOrigin.current.y);
    if (dx > RENAME_MOVE_TOLERANCE || dy > RENAME_MOVE_TOLERANCE) clearHold();
  };

  return (
    <div
      className={`tree-node${selected ? " selected" : ""}`}
      onClick={editing ? undefined : onSelect}
      onMouseDown={handleMouseDown}
      onMouseMove={handleMouseMove}
      onMouseUp={clearHold}
      onMouseLeave={clearHold}
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
      {editing ? (
        <RenameInput initial={label} onSubmit={(next) => onRenameSubmit?.(next)} />
      ) : (
        <span className="tree-label">{label}</span>
      )}
    </div>
  );
}

/**
 * Inline rename editor. Focuses and selects all on mount; Enter commits, Escape
 * cancels, blur commits (legacy AfterLabelEdit). `onSubmit(null)` cancels;
 * `onSubmit(value)` requests a commit. Guarded so it fires exactly once.
 */
function RenameInput({
  initial,
  onSubmit,
}: {
  initial: string;
  onSubmit: (next: string | null) => void;
}) {
  const [value, setValue] = useState(initial);
  const inputRef = useRef<HTMLInputElement>(null);
  const done = useRef(false);

  useEffect(() => {
    const input = inputRef.current;
    if (!input) return;
    input.focus();
    input.select();
  }, []);

  const finish = (commit: boolean) => {
    if (done.current) return;
    done.current = true;
    onSubmit(commit ? value : null);
  };

  return (
    <input
      ref={inputRef}
      className="tree-rename-input"
      value={value}
      spellCheck={false}
      onChange={(e) => setValue(e.target.value)}
      onKeyDown={(e) => {
        e.stopPropagation();
        if (e.key === "Enter") {
          e.preventDefault();
          finish(true);
        } else if (e.key === "Escape") {
          e.preventDefault();
          finish(false);
        }
      }}
      onBlur={() => finish(true)}
      onClick={(e) => e.stopPropagation()}
      onDoubleClick={(e) => e.stopPropagation()}
      onMouseDown={(e) => e.stopPropagation()}
    />
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

/**
 * Form node — small window with internal grid lines (legacy `Form_InTree`).
 * Overlays a green start-point flag and/or magenta block-back arrow, matching
 * the legacy `Form_IsStartPoint_Overlay` / `Form_BlockBackButton_Overlay` icons.
 */
function FormNodeIcon({
  startPoint,
  blockBack,
}: {
  startPoint?: boolean;
  blockBack?: boolean;
} = {}) {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="2" y="2.5" width="12" height="11" rx="1" fill="#ffffff" stroke="#4a6fa5" />
      <rect x="2" y="2.5" width="12" height="2.6" fill="#4a6fa5" />
      <line x1="2" y1="8.4" x2="14" y2="8.4" stroke="#9db4d4" />
      <line x1="6" y1="5.1" x2="6" y2="13.5" stroke="#9db4d4" />
      <line x1="10" y1="5.1" x2="10" y2="13.5" stroke="#9db4d4" />
      {blockBack ? (
        <g>
          <path d="M11 14 L6.5 11.5 L11 9 Z" fill="#d6006e" />
          <rect x="10.6" y="9.4" width="1.6" height="4.2" fill="#d6006e" />
        </g>
      ) : null}
      {startPoint ? (
        <g>
          <line x1="4" y1="6" x2="4" y2="14.5" stroke="#1b7a2e" strokeWidth="1.2" />
          <path d="M4 6 L10.5 7.6 L4 9.6 Z" fill="#2ea043" stroke="#1b7a2e" strokeWidth="0.5" />
        </g>
      ) : null}
    </svg>
  );
}

/** Toolbar: New Form — form page with a yellow "new" star. */
function NewFormIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="1.5" y="1.5" width="10" height="11" rx="1" fill="#ffffff" stroke="#4a6fa5" />
      <rect x="1.5" y="1.5" width="10" height="2.4" fill="#4a6fa5" />
      <line x1="3.5" y1="6" x2="9.5" y2="6" stroke="#9db4d4" />
      <line x1="3.5" y1="8" x2="9.5" y2="8" stroke="#9db4d4" />
      <line x1="3.5" y1="10" x2="7.5" y2="10" stroke="#9db4d4" />
      <NewStar />
    </svg>
  );
}

/** Toolbar: New Process — purple gear with a yellow "new" star. */
function NewProcessIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 24 24" aria-hidden focusable="false">
      <path d={GEAR_PATH} fill="#8a2be2" transform="translate(-2 -2) scale(0.85)" />
      <g transform="translate(13 13) scale(1.15)">
        <NewStarRaw />
      </g>
    </svg>
  );
}

/** Toolbar: New Document — paper/document page with a yellow "new" star. */
function NewDocumentIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path
        d="M2.5 1.5h5L11 4.6v9.4a.4.4 0 0 1-.4.4H2.9a.4.4 0 0 1-.4-.4z"
        fill="#ffffff"
        stroke="#7a7a7a"
        strokeWidth="0.8"
      />
      <path d="M7.5 1.7v3h3" fill="none" stroke="#7a7a7a" strokeWidth="0.8" />
      <line x1="4" y1="7" x2="9" y2="7" stroke="#9aa0a6" />
      <line x1="4" y1="9" x2="9" y2="9" stroke="#9aa0a6" />
      <line x1="4" y1="11" x2="7.5" y2="11" stroke="#9aa0a6" />
      <NewStar />
    </svg>
  );
}

/** Small four-point yellow "new" star for the create buttons (bottom-right). */
function NewStar() {
  return (
    <g transform="translate(11 11)">
      <NewStarRaw />
    </g>
  );
}

function NewStarRaw() {
  return (
    <path
      d="M0 -4 L1 -1 L4 0 L1 1 L0 4 L-1 1 L-4 0 L-1 -1 Z"
      fill="#ffd23f"
      stroke="#e0a800"
      strokeWidth="0.5"
    />
  );
}

/** Toolbar: Move Node Up — upward arrow. */
function MoveUpIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M8 3 L13 9 H9.5 V13 H6.5 V9 H3 Z" fill="#2f6f3e" stroke="#1b4a26" strokeWidth="0.6" />
    </svg>
  );
}

/** Toolbar: Move Node Down — downward arrow. */
function MoveDownIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M8 13 L3 7 H6.5 V3 H9.5 V7 H13 Z" fill="#8a3b3b" stroke="#5a2020" strokeWidth="0.6" />
    </svg>
  );
}

/** Toolbar: Toggle Form Starting Point — form page with a green start flag. */
function StartPointIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="1.5" y="2" width="9" height="10" rx="1" fill="#ffffff" stroke="#4a6fa5" />
      <rect x="1.5" y="2" width="9" height="2.2" fill="#4a6fa5" />
      <line x1="3" y1="6.2" x2="9" y2="6.2" stroke="#9db4d4" />
      <line x1="3" y1="8.2" x2="9" y2="8.2" stroke="#9db4d4" />
      <line x1="9" y1="4" x2="9" y2="15" stroke="#1b7a2e" strokeWidth="1.3" />
      <path d="M9 4 L15 5.8 L9 8 Z" fill="#2ea043" stroke="#1b7a2e" strokeWidth="0.5" />
    </svg>
  );
}

/** Toolbar: Block Back Button — form page with a magenta back arrow. */
function BlockBackIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="4" y="2" width="9" height="10" rx="1" fill="#ffffff" stroke="#4a6fa5" />
      <rect x="4" y="2" width="9" height="2.2" fill="#4a6fa5" />
      <line x1="5.5" y1="6.2" x2="11.5" y2="6.2" stroke="#9db4d4" />
      <line x1="5.5" y1="8.2" x2="11.5" y2="8.2" stroke="#9db4d4" />
      <path d="M6 14 L1 11 L6 8 Z" fill="#d6006e" />
      <rect x="5.4" y="8.4" width="1.8" height="5.2" fill="#d6006e" />
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
