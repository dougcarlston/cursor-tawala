import { Fragment, useState, useEffect, useRef, ReactNode, useSyncExternalStore, type MouseEvent as ReactMouseEvent } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FORM_ITEM_PALETTE } from "@/types/tawala";
import {
  getActivePaletteEditor,
  getFormattingFocusState,
  subscribeFormattingFocus,
} from "@/lib/formattingPaletteContext";
import {
  openDisplayImageConfigureFromEditor,
  openFunctionPickerFromEditor,
} from "@/lib/functionPicker";
import { openLocalImageInsertFromEditor } from "@/lib/localImageInsert";
import {
  canDeleteSelection,
  canDeployProject,
  openProjectManagerLocal,
  runShellEditCommand,
  saveAcceleratorLabel,
  saveAsAcceleratorLabel,
  saveProjectAs,
  saveProjectToDownload,
  shellEditContextActive,
  type ShellEditCommand,
} from "@/lib/shellCommands";
import {
  PROCESS_PANEL_LABELS,
  PROCESS_STATEMENT_PALETTE,
} from "@/processStatements";

interface Props {
  onNewProject: () => void;
  onOpen: () => void;
  onDeploy: () => void;
  onDelete: () => void;
}

export function MenuBar({ onNewProject, onOpen, onDeploy, onDelete }: Props) {
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const insertProcessCommand = useProjectStore((s) => s.insertProcessCommand);
  const toggleProcessStatementPanel = useProjectStore((s) => s.toggleProcessStatementPanel);
  const editorTab = useProjectStore((s) => s.editorTab);
  const openWindows = useProjectStore((s) => s.openWindows);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const formCount = useProjectStore((s) => s.project.forms.length);
  const setStatus = useProjectStore((s) => s.setStatus);
  const dirty = useProjectStore((s) => s.dirty);
  const focus = useSyncExternalStore(
    subscribeFormattingFocus,
    getFormattingFocusState,
    getFormattingFocusState,
  );
  const activeWindow = openWindows.find((w) => w.id === activeWindowId) ?? null;
  const activeKind = activeWindow?.kind ?? null;
  const designActive = activeKind === "document" || editorTab === "design";

  // Touch selection index so Delete enable state re-renders with the toolbar.
  useProjectStore((s) => s.selectedItemIndex);

  const editActive = shellEditContextActive();
  const canDeploy = canDeployProject();
  const canDelete = canDeleteSelection();
  const saveAccel = saveAcceleratorLabel();
  const saveAsAccel = saveAsAcceleratorLabel();

  const edit = (cmd: ShellEditCommand) => () => {
    runShellEditCommand(cmd);
  };

  return (
    <nav className="menu-bar">
      <MenuDrop label="File">
        <button type="button" onClick={onNewProject}>
          New Project…
        </button>
        <button type="button" onClick={onOpen}>
          Open Project…
        </button>
        <div className="menu-separator" />
        {/*
          Save is ALWAYS enabled (legacy + DESIGNER_MENU_SPEC). Never gate on dirty —
          dirty only drives the “modified” cue and status bar. Accel text is secondary
          chrome, not a disabled state.
        */}
        <button
          type="button"
          className={dirty ? "menu-save-dirty" : undefined}
          onClick={() => void saveProjectToDownload()}
        >
          Save{dirty ? " · modified" : ""}
          <span className="menu-accel">{saveAccel}</span>
        </button>
        {/*
          Save As opens an in-app name dialog, then Chromium native picker (clears handle)
          or Safari/Firefox download under the chosen name. Never gated on dirty.
        */}
        <button type="button" onClick={() => saveProjectAs()}>
          Save As…
          <span className="menu-accel">{saveAsAccel}</span>
        </button>
        <div className="menu-separator" />
        <button type="button" disabled={!canDeploy} onClick={onDeploy}>
          Deploy…
        </button>
      </MenuDrop>
      <MenuDrop label="Insert">
        <InsertMenuBody
          activeKind={activeKind}
          designActive={designActive}
          formCount={formCount}
          focusKind={focus.kind}
          onInsertFormItem={insertFormItem}
          onInsertProcess={(label, template) => {
            if (PROCESS_PANEL_LABELS.has(label)) {
              toggleProcessStatementPanel(label);
            } else {
              insertProcessCommand(template);
            }
          }}
          onStub={(msg) => setStatus(msg)}
        />
      </MenuDrop>
      <MenuDrop label="Edit">
        <button type="button" disabled={!editActive} onClick={edit("cut")}>
          Cut
        </button>
        <button type="button" disabled={!editActive} onClick={edit("copy")}>
          Copy
        </button>
        <button type="button" disabled={!editActive} onClick={edit("paste")}>
          Paste
        </button>
        <button type="button" disabled={!canDelete} onClick={onDelete}>
          Delete
        </button>
        <div className="menu-separator" />
        <button type="button" disabled={!editActive} onClick={edit("undo")}>
          Undo
        </button>
        <button type="button" disabled={!editActive} onClick={edit("redo")}>
          Redo
        </button>
      </MenuDrop>
      <MenuDrop label="View">
        <button type="button" disabled>
          Project Explorer
        </button>
        <button type="button" disabled>
          Fields Palette
        </button>
      </MenuDrop>
      <MenuDrop label="Project">
        <button type="button" disabled={!canDeploy} onClick={onDeploy}>
          Deploy
        </button>
        <button type="button" onClick={openProjectManagerLocal}>
          Project Manager…
        </button>
        <button type="button" disabled>
          Page Header…
        </button>
        <button type="button" disabled>
          Themes…
        </button>
      </MenuDrop>
      <MenuDrop label="Help">
        <button type="button" disabled>
          About Tawala Designer
        </button>
      </MenuDrop>
    </nav>
  );
}

function InsertMenuBody({
  activeKind,
  designActive,
  formCount,
  focusKind,
  onInsertFormItem,
  onInsertProcess,
  onStub,
}: {
  activeKind: "form" | "process" | "document" | null;
  designActive: boolean;
  formCount: number;
  focusKind: string;
  onInsertFormItem: (type: (typeof FORM_ITEM_PALETTE)[number]["type"]) => void;
  onInsertProcess: (
    label: string,
    template: (typeof PROCESS_STATEMENT_PALETTE)[number]["template"],
  ) => void;
  onStub: (message: string) => void;
}) {
  if (!activeKind) {
    return (
      <button type="button" disabled>
        Open a Form, Process, or Document window
      </button>
    );
  }

  if (activeKind === "process") {
    return (
      <>
        {PROCESS_STATEMENT_PALETTE.map((def, i) => {
          const prev = PROCESS_STATEMENT_PALETTE[i - 1];
          const newGroup = prev != null && prev.group !== def.group;
          return (
            <Fragment key={def.label}>
              {newGroup && <div className="menu-separator" />}
              <button type="button" onClick={() => onInsertProcess(def.label, def.template)}>
                {def.label}
              </button>
            </Fragment>
          );
        })}
      </>
    );
  }

  if (activeKind === "document") {
    const canFunction = formCount >= 1;
    const canImage = true;
    return (
      <>
        <button
          type="button"
          disabled
          title="Select a field in the Fields palette first (not tracked yet)"
        >
          Field
        </button>
        <MenuSubmenu label="Image…" disabled={!canImage}>
          <button
            type="button"
            onClick={() => {
              if (!getActivePaletteEditor()) {
                onStub("Place the cursor in the document text first");
                return;
              }
              openLocalImageInsertFromEditor();
            }}
          >
            From your PC…
          </button>
          <button
            type="button"
            onClick={() => {
              if (!getActivePaletteEditor()) {
                onStub("Place the cursor in the document text first");
                return;
              }
              openDisplayImageConfigureFromEditor();
            }}
          >
            From the Web or Tawala Upload…
          </button>
        </MenuSubmenu>
        <button
          type="button"
          disabled
          title="Insert Invitation dialog not implemented yet"
        >
          Invitation…
        </button>
        <button
          type="button"
          disabled
          title="Hyperlink dialog not implemented yet"
        >
          Hyperlink…
        </button>
        <button
          type="button"
          disabled={!canFunction}
          onClick={openFunctionPickerFromEditor}
        >
          Function…
        </button>
      </>
    );
  }

  // Form context — top 7 match Items palette (no File Uploader); then Image / Invitation / Hyperlink / Function.
  const canFormItems = designActive;
  const inTextBody = focusKind === "text";
  const canRichImage = inTextBody;
  const canTextExtras = inTextBody;

  return (
    <>
      {FORM_ITEM_PALETTE.map(({ label, type }) => (
        <button
          key={type}
          type="button"
          disabled={!canFormItems}
          onClick={() => onInsertFormItem(type)}
        >
          {label}
        </button>
      ))}
      <div className="menu-separator" />
      <MenuSubmenu label="Image…" disabled={!canRichImage}>
        <button
          type="button"
          disabled={!canRichImage}
          onClick={() => {
            if (!canRichImage) return;
            openLocalImageInsertFromEditor();
          }}
        >
          From your PC…
        </button>
        <button
          type="button"
          disabled={!canRichImage}
          onClick={() => {
            if (!canRichImage) return;
            openDisplayImageConfigureFromEditor();
          }}
        >
          From the Web or Tawala Upload…
        </button>
      </MenuSubmenu>
      <button
        type="button"
        disabled={!canTextExtras}
        title="Insert Invitation dialog not implemented yet"
        onClick={() => onStub("Insert Invitation… is not implemented yet")}
      >
        Invitation…
      </button>
      <button
        type="button"
        disabled={!canTextExtras}
        title="Hyperlink dialog not implemented yet"
        onClick={() => onStub("Insert Hyperlink… is not implemented yet")}
      >
        Hyperlink…
      </button>
      <button type="button" disabled={!canTextExtras} onClick={openFunctionPickerFromEditor}>
        Function…
      </button>
    </>
  );
}

function MenuDrop({ label, children }: { label: string; children: ReactNode }) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const close = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener("mousedown", close);
    return () => document.removeEventListener("mousedown", close);
  }, []);

  // Native file/open dialogs steal focus without a document mousedown, so the File
  // menu would otherwise stay open after Open Project… (and similar) returns.
  useEffect(() => {
    if (!open) return;
    const closeOnFocus = () => setOpen(false);
    window.addEventListener("focus", closeOnFocus);
    return () => window.removeEventListener("focus", closeOnFocus);
  }, [open]);

  const closeAfterMenuAction = (e: ReactMouseEvent) => {
    const target = e.target as HTMLElement | null;
    if (!target) return;
    // Submenu triggers only open a fly-out; keep the parent menu open.
    if (target.closest(".menu-submenu-trigger")) return;
    const btn = target.closest("button");
    if (btn && !btn.disabled) setOpen(false);
  };

  return (
    <div className={`menu-item${open ? " open" : ""}`} ref={ref}>
      <button
        type="button"
        className="menu-trigger"
        onClick={() => setOpen((v) => !v)}
        onMouseEnter={() => open && setOpen(true)}
      >
        {label}
      </button>
      <div className="menu-dropdown" onClick={closeAfterMenuAction}>
        {children}
      </div>
    </div>
  );
}

/** Fly-out submenu inside a dropdown (legacy Insert → Image…). */
function MenuSubmenu({
  label,
  disabled,
  children,
}: {
  label: string;
  disabled?: boolean;
  children: ReactNode;
}) {
  const [open, setOpen] = useState(false);

  return (
    <div
      className={`menu-submenu${open ? " open" : ""}${disabled ? " disabled" : ""}`}
      onMouseEnter={() => !disabled && setOpen(true)}
      onMouseLeave={() => setOpen(false)}
    >
      <button type="button" className="menu-submenu-trigger" disabled={disabled}>
        {label}
        <span className="menu-submenu-arrow" aria-hidden>
          ▸
        </span>
      </button>
      {!disabled && open && <div className="menu-submenu-dropdown">{children}</div>}
    </div>
  );
}
