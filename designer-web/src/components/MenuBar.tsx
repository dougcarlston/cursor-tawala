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
  openHyperlinkInsertFromEditor,
  openInvitationInsertFromEditor,
} from "@/lib/linkInsert";
import { openFormItemStylesDialog, stylesKindForFormItem } from "@/lib/formItemStyles";
import { windowMenuLabel } from "@/lib/mdiWindowLayout";
import {
  getViewChrome,
  subscribeViewChrome,
  toggleViewChrome,
  type ViewChromeKey,
} from "@/lib/viewChrome";
import {
  getFieldsPaletteSelection,
  subscribeFieldsPaletteSelection,
} from "@/lib/fieldsPaletteSelection";
import { insertFieldIntoActiveTarget, isInsideActiveMdiWindow } from "@/lib/fieldInsertion";
import { insertFieldTokenAtSelection } from "@/lib/fieldTokens";
import {
  canDeleteSelection,
  canDeployProject,
  copyAcceleratorLabel,
  cutAcceleratorLabel,
  deleteAcceleratorLabel,
  newProjectAcceleratorLabel,
  openProjectAcceleratorLabel,
  openProjectManagerLocal,
  pasteAcceleratorLabel,
  redoAcceleratorLabel,
  runShellEditCommand,
  saveAcceleratorLabel,
  saveAsAcceleratorLabel,
  saveProjectAs,
  saveProjectToDownload,
  shellEditContextActive,
  undoAcceleratorLabel,
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
  const cascadeWindows = useProjectStore((s) => s.cascadeWindows);
  const tileWindows = useProjectStore((s) => s.tileWindows);
  const closeAllWindows = useProjectStore((s) => s.closeAllWindows);
  const focusWindow = useProjectStore((s) => s.focusWindow);
  const restoreWindow = useProjectStore((s) => s.restoreWindow);
  const formCount = useProjectStore((s) => s.project.forms.length);
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const setStatus = useProjectStore((s) => s.setStatus);
  const dirty = useProjectStore((s) => s.dirty);
  const focus = useSyncExternalStore(
    subscribeFormattingFocus,
    getFormattingFocusState,
    getFormattingFocusState,
  );
  const viewChrome = useSyncExternalStore(subscribeViewChrome, getViewChrome, getViewChrome);
  const fieldsPaletteSelection = useSyncExternalStore(
    subscribeFieldsPaletteSelection,
    getFieldsPaletteSelection,
    getFieldsPaletteSelection,
  );
  const activeWindow = openWindows.find((w) => w.id === activeWindowId) ?? null;
  const activeKind = activeWindow?.kind ?? null;
  const designActive = activeKind === "document" || editorTab === "design";

  const selectedFormItem =
    activeKind === "form" &&
    activeWindow &&
    selection.kind === "form" &&
    selection.name === activeWindow.name &&
    selectedItemIndex != null
      ? project.forms.find((f) => f.name === activeWindow.name)?.items[selectedItemIndex] ?? null
      : null;
  const stylesKind = stylesKindForFormItem(selectedFormItem);

  const editActive = shellEditContextActive();
  const canDeploy = canDeployProject();
  const canDelete = canDeleteSelection();
  const newAccel = newProjectAcceleratorLabel();
  const openAccel = openProjectAcceleratorLabel();
  const saveAccel = saveAcceleratorLabel();
  const saveAsAccel = saveAsAcceleratorLabel();
  const cutAccel = cutAcceleratorLabel();
  const copyAccel = copyAcceleratorLabel();
  const pasteAccel = pasteAcceleratorLabel();
  const deleteAccel = deleteAcceleratorLabel();
  const undoAccel = undoAcceleratorLabel();
  const redoAccel = redoAcceleratorLabel();

  const edit = (cmd: ShellEditCommand) => () => {
    runShellEditCommand(cmd);
  };
  /** Keep caret in the canvas editor when opening Edit items (same as Formatting Palette). */
  const keepEditorFocus = (e: ReactMouseEvent) => {
    e.preventDefault();
  };

  return (
    <nav className="menu-bar">
      <MenuDrop label="File">
        <button type="button" onClick={onNewProject}>
          New Project…
          <span className="menu-accel">{newAccel}</span>
        </button>
        <button type="button" onClick={onOpen}>
          Open Project…
          <span className="menu-accel">{openAccel}</span>
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
      <MenuDrop label="Edit">
        <button type="button" disabled={!editActive} onMouseDown={keepEditorFocus} onClick={edit("cut")}>
          Cut
          <span className="menu-accel">{cutAccel}</span>
        </button>
        <button type="button" disabled={!editActive} onMouseDown={keepEditorFocus} onClick={edit("copy")}>
          Copy
          <span className="menu-accel">{copyAccel}</span>
        </button>
        <button type="button" disabled={!editActive} onMouseDown={keepEditorFocus} onClick={edit("paste")}>
          Paste
          <span className="menu-accel">{pasteAccel}</span>
        </button>
        <button type="button" disabled={!canDelete} onClick={onDelete}>
          Delete
          <span className="menu-accel">{deleteAccel}</span>
        </button>
        <div className="menu-separator" />
        <button type="button" disabled={!editActive} onMouseDown={keepEditorFocus} onClick={edit("undo")}>
          Undo
          <span className="menu-accel">{undoAccel}</span>
        </button>
        <button type="button" disabled={!editActive} onMouseDown={keepEditorFocus} onClick={edit("redo")}>
          Redo
          <span className="menu-accel">{redoAccel}</span>
        </button>
      </MenuDrop>
      <MenuDrop label="Insert">
        <InsertMenuBody
          activeKind={activeKind}
          designActive={designActive}
          formCount={formCount}
          focusKind={focus.kind}
          fieldsPaletteSelection={fieldsPaletteSelection}
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
      <MenuDrop label="View">
        <ViewToggle
          checked={viewChrome.projectExplorer}
          chromeKey="projectExplorer"
          label="Project Explorer"
        />
        <ViewToggle
          checked={viewChrome.fieldsPalette}
          chromeKey="fieldsPalette"
          label="Fields Palette"
        />
        <div className="menu-separator" />
        <ViewToggle checked={viewChrome.toolbar} chromeKey="toolbar" label="Toolbar" />
        <ViewToggle checked={viewChrome.statusBar} chromeKey="statusBar" label="Status Bar" />
        <ViewToggle
          checked={viewChrome.itemsPalette}
          chromeKey="itemsPalette"
          label="Items Palette"
        />
      </MenuDrop>
      <MenuDrop label="Project">
        <button type="button" disabled={!canDeploy} onClick={onDeploy}>
          Deploy
        </button>
        <button type="button" onClick={openProjectManagerLocal}>
          Project Manager…
        </button>
        <div className="menu-separator" />
        {/* 8080 / CSS track — stubs until that agent owns page chrome */}
        <button
          type="button"
          disabled
          title="Deployed page banner — park for 8080 / Tomcat / CSS track"
        >
          Page Header…
        </button>
        <button
          type="button"
          disabled
          title="Deployed theme CSS — park for 8080 / Tomcat / CSS track"
        >
          Themes…
        </button>
        <div className="menu-separator" />
        <button
          type="button"
          disabled={!stylesKind}
          title={
            stylesKind
              ? `Style the selected ${
                  stylesKind === "fib"
                    ? "Fill in the Blank"
                    : stylesKind === "mc"
                      ? "Multiple Choice"
                      : "Text"
                } (and optionally all of that type on this form)`
              : activeKind !== "form"
                ? "Open a Form window and select a FIB, Multiple Choice, or Text item"
                : "Select a Fill in the Blank, Multiple Choice, or Text item (not Heading, Hidden Field, Page Break, or Skip Instructions)"
          }
          onClick={() => {
            if (!stylesKind) {
              setStatus(
                "Select a Fill in the Blank, Multiple Choice, or Text item first",
              );
              return;
            }
            openFormItemStylesDialog(stylesKind);
          }}
        >
          Styles…
        </button>
      </MenuDrop>
      <MenuDrop label="Windows">
        <button
          type="button"
          disabled={openWindows.length === 0}
          onClick={() => cascadeWindows()}
        >
          Cascade
        </button>
        <button
          type="button"
          disabled={openWindows.length === 0}
          onClick={() => tileWindows("horizontal")}
        >
          Tile Horizontally
        </button>
        <button
          type="button"
          disabled={openWindows.length === 0}
          onClick={() => tileWindows("vertical")}
        >
          Tile Vertically
        </button>
        <button
          type="button"
          disabled={openWindows.length === 0}
          onClick={() => closeAllWindows()}
        >
          Close All
        </button>
        {openWindows.length > 0 && <div className="menu-separator" />}
        {openWindows.map((win, i) => {
          const active = win.id === activeWindowId;
          return (
            <button
              key={win.id}
              type="button"
              className="menu-check-item"
              aria-checked={active}
              onClick={() => {
                if (win.minimized) restoreWindow(win.id);
                else focusWindow(win.id);
              }}
            >
              <span className="menu-check" aria-hidden>
                {active ? "✓" : ""}
              </span>
              {i + 1} {windowMenuLabel(win.kind, win.name)}
              {win.minimized ? " (minimized)" : ""}
            </button>
          );
        })}
      </MenuDrop>
      <MenuDrop label="Help">
        {/* Stub — owner will paste legacy copyright + build status text later. */}
        <button type="button" disabled title="About dialog stub — copyright / build text TBD">
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
  fieldsPaletteSelection,
  onInsertFormItem,
  onInsertProcess,
  onStub,
}: {
  activeKind: "form" | "process" | "document" | null;
  designActive: boolean;
  formCount: number;
  focusKind: string;
  fieldsPaletteSelection: string | null;
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
    const canField = !!fieldsPaletteSelection;
    return (
      <>
        <button
          type="button"
          disabled={!canField}
          title={
            canField
              ? `Insert ${fieldsPaletteSelection}`
              : "Select a field in the Fields palette first"
          }
          onClick={() => {
            if (!fieldsPaletteSelection) return;
            const editor = getActivePaletteEditor();
            const inActiveDocument =
              !!editor?.el?.isConnected &&
              !!editor.el.closest(".document-editor") &&
              isInsideActiveMdiWindow(editor.el);
            if (!inActiveDocument) {
              onStub("Place the cursor in the document text first");
              return;
            }
            editor.el.focus();
            editor.restoreSelection();
            if (!insertFieldIntoActiveTarget(fieldsPaletteSelection)) {
              insertFieldTokenAtSelection(fieldsPaletteSelection);
              editor.commit();
            }
          }}
        >
          Field
        </button>
        <MenuSubmenu label="Image…" disabled={!canImage}>
          <button
            type="button"
            onClick={() => {
              const editor = getActivePaletteEditor();
              if (!editor?.el || !isInsideActiveMdiWindow(editor.el)) {
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
              const editor = getActivePaletteEditor();
              if (!editor?.el || !isInsideActiveMdiWindow(editor.el)) {
                onStub("Place the cursor in the document text first");
                return;
              }
              openDisplayImageConfigureFromEditor();
            }}
          >
            From the Web…
          </button>
        </MenuSubmenu>
        <button
          type="button"
          disabled={!canFunction}
          onClick={() => {
            const editor = getActivePaletteEditor();
            if (!editor?.el || !isInsideActiveMdiWindow(editor.el)) {
              onStub("Place the cursor in the document text first");
              return;
            }
            openInvitationInsertFromEditor();
          }}
        >
          Invitation…
        </button>
        <button
          type="button"
          disabled={!canFunction}
          onClick={() => {
            const editor = getActivePaletteEditor();
            if (!editor?.el || !isInsideActiveMdiWindow(editor.el)) {
              onStub("Place the cursor in the document text first");
              return;
            }
            openHyperlinkInsertFromEditor();
          }}
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

  // Form context — top 7 match Items palette; then Image / Invitation / Hyperlink / Function.
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
          From the Web…
        </button>
      </MenuSubmenu>
      <button
        type="button"
        disabled={!canTextExtras}
        onClick={() => {
          if (!canTextExtras) return;
          openInvitationInsertFromEditor();
        }}
      >
        Invitation…
      </button>
      <button
        type="button"
        disabled={!canTextExtras}
        onClick={() => {
          if (!canTextExtras) return;
          openHyperlinkInsertFromEditor();
        }}
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

function ViewToggle({
  checked,
  chromeKey,
  label,
}: {
  checked: boolean;
  chromeKey: ViewChromeKey;
  label: string;
}) {
  return (
    <button
      type="button"
      className="menu-check-item"
      aria-checked={checked}
      onClick={() => toggleViewChrome(chromeKey)}
    >
      <span className="menu-check" aria-hidden>
        {checked ? "✓" : ""}
      </span>
      {label}
    </button>
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
