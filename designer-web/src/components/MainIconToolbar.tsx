import type { ReactNode } from "react";
import { useProjectStore } from "@/store/projectStore";
import {
  canDeleteSelection,
  canDeployProject,
  openProjectManagerLocal,
  runShellEditCommand,
  saveProjectToDownload,
  shellEditContextActive,
  type ShellEditCommand,
} from "@/lib/shellCommands";

export interface MainIconToolbarProps {
  onNewProject: () => void;
  onOpen: () => void;
  onDeploy: () => void;
  onDelete: () => void;
  /** Width of the zone above Project Explorer (+ Items/Statements when present). */
  zoneWidth: number;
}

/**
 * Legacy main icon toolbar (`mainToolStrip`) — frequently-used actions below the menu bar,
 * above Project Explorer, to the left of the Formatting Palette.
 * Spec: DESIGNER_MENU_SPEC.md → “Main icon toolbar”.
 */
export function MainIconToolbar({
  onNewProject,
  onOpen,
  onDeploy,
  onDelete,
  zoneWidth,
}: MainIconToolbarProps) {
  // Keep enable rules in sync with menus (forms, selection, active MDI window).
  useProjectStore((s) => s.project.forms.length);
  useProjectStore((s) => s.selection);
  useProjectStore((s) => s.selectedItemIndex);
  useProjectStore((s) => s.activeWindowId);
  useProjectStore((s) => s.editorTab);
  useProjectStore((s) => s.openWindows.length);

  const editActive = shellEditContextActive();
  const canDeploy = canDeployProject();
  const canDelete = canDeleteSelection();
  const width = Math.max(zoneWidth, 200);

  const edit = (cmd: ShellEditCommand) => () => {
    runShellEditCommand(cmd);
  };

  return (
    <div
      className="main-icon-toolbar"
      role="toolbar"
      aria-label="Standard toolbar"
      style={{ width, flex: `0 0 ${width}px` }}
    >
      <ToolIcon tip="New Project" onClick={onNewProject}>
        <NewProjectIcon />
      </ToolIcon>
      <ToolIcon tip="Open Project" onClick={onOpen}>
        <OpenProjectIcon />
      </ToolIcon>
      <ToolIcon tip="Save Project" onClick={saveProjectToDownload}>
        <SaveIcon />
      </ToolIcon>
      <ToolIcon tip="Deploy Project" onClick={onDeploy} disabled={!canDeploy}>
        <DeployIcon />
      </ToolIcon>
      <span className="main-icon-toolbar-sep" aria-hidden />
      <ToolIcon tip="Cut" onClick={edit("cut")} disabled={!editActive}>
        <CutIcon />
      </ToolIcon>
      <ToolIcon tip="Copy" onClick={edit("copy")} disabled={!editActive}>
        <CopyIcon />
      </ToolIcon>
      <ToolIcon tip="Paste" onClick={edit("paste")} disabled={!editActive}>
        <PasteIcon />
      </ToolIcon>
      <ToolIcon tip="Delete" onClick={onDelete} disabled={!canDelete}>
        <DeleteIcon />
      </ToolIcon>
      <span className="main-icon-toolbar-sep" aria-hidden />
      <ToolIcon tip="Undo" onClick={edit("undo")} disabled={!editActive}>
        <UndoIcon />
      </ToolIcon>
      <ToolIcon tip="Redo" onClick={edit("redo")} disabled={!editActive}>
        <RedoIcon />
      </ToolIcon>
      <span className="main-icon-toolbar-sep" aria-hidden />
      <ToolIcon tip="Project Manager" onClick={openProjectManagerLocal}>
        <ProjectManagerIcon />
      </ToolIcon>
    </div>
  );
}

function ToolIcon({
  tip,
  disabled,
  onClick,
  children,
}: {
  tip: string;
  disabled?: boolean;
  onClick: () => void;
  children: ReactNode;
}) {
  // Same pattern as Project Explorer: custom tip on a wrapper so greyed icons still show labels.
  return (
    <span className="explorer-tip" data-tip={tip}>
      <button
        type="button"
        className="main-icon-tool-btn"
        aria-label={tip}
        disabled={disabled}
        onClick={onClick}
      >
        {children}
      </button>
    </span>
  );
}

function NewProjectIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="3" y="2" width="9" height="12" rx="1" fill="#fff" stroke="#555" />
      <path d="M9 2v3h3" fill="none" stroke="#555" />
      <circle cx="11" cy="11" r="3.2" fill="#6a4bbc" />
      <path d="M11 9.5v3M9.5 11h3" stroke="#fff" strokeWidth="1.2" />
    </svg>
  );
}

function OpenProjectIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M2 4h5l1 1.5h6v8H2z" fill="#f4d06f" stroke="#b8860b" />
      <path d="M9 8l3 2-3 2V8z" fill="#3f78c4" />
    </svg>
  );
}

function SaveIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M3 2h9l2 2v10H3z" fill="#4a7ec7" stroke="#2f5f9f" />
      <rect x="5" y="2.5" width="5" height="4" fill="#cfe0f8" />
      <rect x="4.5" y="9" width="7" height="4.5" fill="#eee" stroke="#666" />
    </svg>
  );
}

function DeployIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <circle cx="8" cy="8" r="5.5" fill="#3d9e4a" stroke="#2a6e32" />
      <path d="M5 8.5l2 2 4-4.5" fill="none" stroke="#fff" strokeWidth="1.4" />
    </svg>
  );
}

function CutIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <circle cx="4.5" cy="11" r="2" fill="none" stroke="#2a5db0" strokeWidth="1.3" />
      <circle cx="11.5" cy="11" r="2" fill="none" stroke="#2a5db0" strokeWidth="1.3" />
      <path d="M6 10 L12 3 M10 10 L4 3" stroke="#2a5db0" strokeWidth="1.3" />
    </svg>
  );
}

function CopyIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="5" y="4" width="7" height="9" fill="#fff" stroke="#555" />
      <rect x="3" y="2" width="7" height="9" fill="#e8f0fe" stroke="#555" />
    </svg>
  );
}

function PasteIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="4" y="4" width="8" height="10" fill="#fff" stroke="#555" />
      <rect x="6" y="2" width="4" height="3" fill="#ddd" stroke="#555" />
      <path d="M6 8h4M6 10h4" stroke="#2a5db0" strokeWidth="1.2" />
    </svg>
  );
}

function DeleteIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M4 4 L12 12 M12 4 L4 12" stroke="#c0392b" strokeWidth="2" strokeLinecap="round" />
    </svg>
  );
}

function UndoIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M5 7 H11 a3 3 0 0 1 0 6 H8" fill="none" stroke="#555" strokeWidth="1.4" />
      <path d="M5 7 L7.5 4.5 M5 7 L7.5 9.5" fill="none" stroke="#555" strokeWidth="1.4" />
    </svg>
  );
}

function RedoIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <path d="M11 7 H5 a3 3 0 0 0 0 6 H8" fill="none" stroke="#555" strokeWidth="1.4" />
      <path d="M11 7 L8.5 4.5 M11 7 L8.5 9.5" fill="none" stroke="#555" strokeWidth="1.4" />
    </svg>
  );
}

function ProjectManagerIcon() {
  return (
    <svg width="16" height="16" viewBox="0 0 16 16" aria-hidden focusable="false">
      <rect x="3" y="2" width="9" height="12" rx="1" fill="#fff" stroke="#555" />
      <circle cx="11" cy="11" r="3.2" fill="#6a4bbc" />
      <circle cx="11" cy="11" r="1.4" fill="#3d9e4a" />
    </svg>
  );
}
