import { create } from "zustand";
import {
  createDefaultItem,
  emptyProject,
  TawalaForm,
  FormItem,
  FormItemType,
  Selection,
  TawalaProject,
  EditorTab,
  TawalaProcessCommand,
  RichContentBlock,
} from "@/types/tawala";
import { setActiveFieldTarget } from "@/lib/fieldInsertion";
import { nextHiddenFieldName } from "@/lib/fieldNames";
import { nextLinkedProcessName } from "@/lib/projectModel";
import {
  moveProcessCommandAtPath,
  getProcessCommandAtPath,
} from "@/lib/processScript";
import { insertCommandAtPoint } from "@/lib/processInsert";
import { ROOT_INSERT_PATH } from "@/lib/skipInsertPath";
import { DeployCredentials, DeployResult, loadCredentials, saveCredentials } from "@/api/deploy";
import { deployProject as apiDeploy } from "@/api/deploy";
import type { ProcessStatementPanel } from "@/processStatements";
import { processPanelKeyForCommand, processPanelKeyForLabel } from "@/processStatements";
import {
  remapProcessUiCache,
  snapshotProcessUi,
  transitionProcessUi,
  type ProcessUiSnapshot,
} from "@/lib/processUiState";

// Legacy naming (TawalaDesigner Project.AddForm/AddProcess/AddDocument):
// nodes are "Form 1", "Process 1", "Document 1" (base name + space + number),
// while form-item labels are "Q1"/"T1"/"H1" (no space). Pass `separator` to
// pick the right convention.
function nextLabel(prefix: string, existing: string[], separator = ""): string {
  let n = 1;
  while (existing.includes(`${prefix}${separator}${n}`)) n++;
  return `${prefix}${separator}${n}`;
}

// --- MDI window shell (backlog §2 Multi-window / MDI, Pass 1, July 2026) ------
// Legacy Designer opens Forms, Processes, and Documents as separate overlapping
// MDI child windows on the center canvas. `openWindows` models those children;
// `id` is `${kind}:${name}` so re-opening the same entity focuses its window.

export type WindowKind = "form" | "process" | "document";

export interface DesignerWindow {
  id: string;
  kind: WindowKind;
  name: string;
  x: number;
  y: number;
  w: number;
  h: number;
  z: number;
  minimized: boolean;
}

const WINDOW_DEFAULT_SIZE = { w: 640, h: 460 };
/**
 * Cascade step for newly opened windows so each steps down-and-right from the
 * previous one (owner decision, July 2026). Must clear a title bar so the prior
 * window's `Type - Name` heading stays visible — ~28px ≈ title-bar height + a
 * small nudge.
 */
const WINDOW_CASCADE_STEP = 28;
/** Wrap the cascade back to the origin after this many steps so windows stay on-canvas. */
const WINDOW_CASCADE_WRAP = 8;

function windowId(kind: WindowKind, name: string): string {
  return `${kind}:${name}`;
}

function maxZ(windows: DesignerWindow[]): number {
  return windows.reduce((m, w) => Math.max(m, w.z), 0);
}

/**
 * Position/size for the next cascaded window. `step` is the running cascade
 * index (0 for the first window after an empty canvas); each increment offsets
 * the window down and to the right, wrapping back to the origin after
 * `WINDOW_CASCADE_WRAP` so long sessions never march off-screen.
 */
function cascadeBounds(step: number): Pick<DesignerWindow, "x" | "y" | "w" | "h"> {
  const offset = (step % WINDOW_CASCADE_WRAP) * WINDOW_CASCADE_STEP;
  return { x: 20 + offset, y: 20 + offset, w: WINDOW_DEFAULT_SIZE.w, h: WINDOW_DEFAULT_SIZE.h };
}

/** Re-key any open window after its underlying entity is renamed. */
function remapWindows(
  windows: DesignerWindow[],
  kind: WindowKind,
  oldName: string,
  newName: string,
): DesignerWindow[] {
  return windows.map((w) =>
    w.kind === kind && w.name === oldName
      ? { ...w, name: newName, id: windowId(kind, newName) }
      : w,
  );
}

/** Default insertion index for a form: append at end (`items.length`). */
function insertIndexForForm(project: TawalaProject, formName: string): number {
  return project.forms.find((f) => f.name === formName)?.items.length ?? 0;
}

function insertIndexWhenSwitchingEntity(
  project: TawalaProject,
  kind: WindowKind,
  name: string,
  sameEntity: boolean,
  prev: number,
): number {
  if (sameEntity) return prev;
  return kind === "form" ? insertIndexForForm(project, name) : 0;
}

function pickProcessUi(state: {
  processInsertPath: string;
  processInsertIndex: number;
  selectedProcessCommandPath: string | null;
  processStatementPanel: ProcessStatementPanel;
}): ProcessUiSnapshot {
  return snapshotProcessUi({
    processInsertPath: state.processInsertPath,
    processInsertIndex: state.processInsertIndex,
    selectedProcessCommandPath: state.selectedProcessCommandPath,
    processStatementPanel: state.processStatementPanel,
  });
}

function selectionWindowTarget(selection: Selection): { kind: WindowKind; name: string } {
  if (selection.kind === "form" || selection.kind === "process" || selection.kind === "document") {
    return { kind: selection.kind, name: selection.name ?? "" };
  }
  return { kind: "form", name: "" };
}

function applyProcessWindowTransition(
  from: Selection,
  toKind: WindowKind,
  toName: string,
  sameEntity: boolean,
  processUiByName: Record<string, ProcessUiSnapshot>,
  current: ProcessUiSnapshot,
): { processUiByName: Record<string, ProcessUiSnapshot> } & ProcessUiSnapshot {
  const { cache, ui } = transitionProcessUi(
    from,
    toKind,
    toName,
    sameEntity,
    current,
    processUiByName,
  );
  return { processUiByName: cache, ...ui };
}

interface ProjectState {
  project: TawalaProject;
  dirty: boolean;
  selection: Selection;
  editorTab: EditorTab;
  statusMessage: string;
  selectedItemIndex: number | null;
  /** Next Items palette insert lands at this index in the active form (0..items.length). */
  insertBeforeIndex: number;
  /** Per-process statement panel + insertion state (survives MDI focus changes). */
  processUiByName: Record<string, ProcessUiSnapshot>;
  /** Process script insertion path for the active process (`root` or `root/0/then`, …). */
  processInsertPath: string;
  /** Index within the insertion path's command array (0 = top of that branch). */
  processInsertIndex: number;
  /** Selected process command path for the JSON property panel (`root/0`, …). */
  selectedProcessCommandPath: string | null;
  /** Active statement property panel in the process window (`if`, `set`, …). */
  processStatementPanel: ProcessStatementPanel;
  credentials: DeployCredentials | null;
  lastDeploy: DeployResult | null;
  showLogin: boolean;
  showDeployResult: boolean;
  openWindows: DesignerWindow[];
  activeWindowId: string | null;
  /** Running cascade index for the next opened window; resets when the canvas empties. */
  cascadeIndex: number;
  openWindow: (kind: WindowKind, name: string) => void;
  closeWindow: (id: string) => void;
  focusWindow: (id: string) => void;
  minimizeWindow: (id: string) => void;
  restoreWindow: (id: string) => void;
  setWindowBounds: (
    id: string,
    bounds: Partial<Pick<DesignerWindow, "x" | "y" | "w" | "h">>,
  ) => void;
  setProject: (project: TawalaProject) => void;
  setSelection: (selection: Selection) => void;
  setEditorTab: (tab: EditorTab) => void;
  setStatus: (message: string) => void;
  setSelectedItemIndex: (index: number | null) => void;
  setInsertBeforeIndex: (index: number) => void;
  moveSelectedFormItem: (direction: "up" | "down") => void;
  setProcessInsertPath: (path: string) => void;
  setProcessInsertPoint: (path: string, index: number) => void;
  setSelectedProcessCommandPath: (path: string | null) => void;
  setProcessStatementPanel: (panel: ProcessStatementPanel) => void;
  toggleProcessStatementPanel: (label: string) => void;
  moveSelectedProcessCommand: (direction: "up" | "down") => void;
  createLinkedProcessForForm: (formName: string, role: "Pre" | "Post") => void;
  linkProcessToForm: (processName: string, formName: string, role: "Pre" | "Post") => void;
  unlinkProcessFromForm: (
    processName: string,
    formName: string,
    role: "Pre" | "Post",
  ) => void;
  setShowLogin: (show: boolean) => void;
  setShowDeployResult: (show: boolean) => void;
  setCredentials: (credentials: DeployCredentials) => void;
  newProject: (options?: { empty?: boolean }) => void;
  loadTemplate: (samplePath: string) => Promise<void>;
  addForm: () => void;
  addProcess: () => void;
  addDocument: () => void;
  toggleFormStartPoint: (name: string) => void;
  toggleFormBlockBack: (name: string) => void;
  moveSelectedNode: (direction: "up" | "down") => void;
  renameForm: (oldName: string, newName: string) => boolean;
  renameProcess: (oldName: string, newName: string) => boolean;
  renameDocument: (oldName: string, newName: string) => boolean;
  insertFormItem: (type: FormItemType) => void;
  updateFormItem: (formName: string, index: number, item: FormItem) => void;
  updateForm: (formName: string, patch: Partial<TawalaForm>) => void;
  deleteFormItem: (formName: string, index: number) => void;
  deleteSelectedFormItem: () => void;
  insertProcessCommand: (command: TawalaProcessCommand) => void;
  updateProcessCommands: (processName: string, commands: TawalaProcessCommand[]) => void;
  updateDocumentContent: (documentName: string, content: string | RichContentBlock[]) => void;
  selectForm: (name: string) => void;
  exportJson: () => string;
  importJson: (raw: string) => void;
  deploy: () => Promise<void>;
}

export const useProjectStore = create<ProjectState>((set, get) => ({
  project: emptyProject(),
  dirty: false,
  selection: { kind: "form", name: "Form 1" },
  editorTab: "design",
  statusMessage: "Ready",
  selectedItemIndex: null,
  insertBeforeIndex: 0,
  processInsertPath: ROOT_INSERT_PATH,
  processInsertIndex: 0,
  processUiByName: {},
  selectedProcessCommandPath: null,
  processStatementPanel: "none",
  credentials: loadCredentials(),
  lastDeploy: null,
  showLogin: false,
  showDeployResult: false,
  // Owner decision (July 2026): the canvas starts EMPTY — no window is auto-opened
  // on first mount or project load. The empty-canvas placeholder shows until the
  // designer single-clicks a form / process / document in Project Explorer.
  openWindows: [],
  activeWindowId: null,
  cascadeIndex: 0,

  openWindow: (kind, name) => {
    const { openWindows, cascadeIndex } = get();
    const id = windowId(kind, name);
    const existing = openWindows.find((w) => w.id === id);
    const topZ = maxZ(openWindows) + 1;
    // Decision 3 (July 2026): Forms always open in Design mode. The Design/Preview
    // tab is still a GLOBAL store field in Pass 1, so opening any form resets the
    // shared tab to Design (true per-window tab state is Pass 2). Processes and
    // Documents have no Design/Preview tab, so leave the tab untouched for them.
    const tabPatch: Partial<ProjectState> = kind === "form" ? { editorTab: "design" } : {};
    if (existing) {
      // Owner rule: re-opening an already-open entity focuses/raises it (no duplicate).
      // Preserve the selected form item when the entity is unchanged so re-focusing the
      // window you're editing does not clear the Inspector's field-drop target (Bug fix,
      // July 2026: "worked once then refused"). Switching to a different entity resets it.
      const s = get();
      const sameEntity = s.selection.kind === kind && s.selection.name === name;
      set({
        openWindows: openWindows.map((w) =>
          w.id === id ? { ...w, z: topZ, minimized: false } : w,
        ),
        activeWindowId: id,
        selection: { kind, name },
        selectedItemIndex: sameEntity ? s.selectedItemIndex : null,
        insertBeforeIndex: insertIndexWhenSwitchingEntity(
          s.project,
          kind,
          name,
          sameEntity,
          s.insertBeforeIndex,
        ),
        ...applyProcessWindowTransition(
          s.selection,
          kind,
          name,
          sameEntity,
          s.processUiByName,
          pickProcessUi(s),
        ),
        ...tabPatch,
      });
      return;
    }
    // Decision 1: each new window cascades down-and-right from the previous one.
    const win: DesignerWindow = {
      id,
      kind,
      name,
      ...cascadeBounds(cascadeIndex),
      z: topZ,
      minimized: false,
    };
    const s = get();
    set({
      openWindows: [...openWindows, win],
      activeWindowId: id,
      selection: { kind, name },
      selectedItemIndex: null,
      insertBeforeIndex: kind === "form" ? insertIndexForForm(s.project, name) : 0,
      ...applyProcessWindowTransition(
        s.selection,
        kind,
        name,
        false,
        s.processUiByName,
        pickProcessUi(s),
      ),
      cascadeIndex: cascadeIndex + 1,
      ...tabPatch,
    });
  },

  closeWindow: (id) => {
    const s = get();
    const { openWindows, activeWindowId, selection, selectedItemIndex, insertBeforeIndex, project } =
      s;
    const next = openWindows.filter((w) => w.id !== id);
    let nextActive = activeWindowId === id ? null : activeWindowId;
    if (nextActive === null && next.length > 0) {
      nextActive = next.reduce((top, w) => (w.z > top.z ? w : top), next[0]).id;
    }
    const nextActiveWin = nextActive ? next.find((w) => w.id === nextActive) ?? null : null;
    const sameEntity =
      !!nextActiveWin &&
      selection.kind === nextActiveWin.kind &&
      selection.name === nextActiveWin.name;
    const toTarget = nextActiveWin
      ? { kind: nextActiveWin.kind, name: nextActiveWin.name }
      : selectionWindowTarget(selection);
    set({
      openWindows: next,
      activeWindowId: nextActive,
      selection: nextActiveWin ? { kind: nextActiveWin.kind, name: nextActiveWin.name } : selection,
      selectedItemIndex: sameEntity ? selectedItemIndex : null,
      insertBeforeIndex: nextActiveWin
        ? insertIndexWhenSwitchingEntity(
            project,
            nextActiveWin.kind,
            nextActiveWin.name,
            sameEntity,
            insertBeforeIndex,
          )
        : 0,
      ...applyProcessWindowTransition(
        selection,
        toTarget.kind,
        toTarget.name,
        sameEntity,
        s.processUiByName,
        pickProcessUi(s),
      ),
      ...(next.length === 0 ? { cascadeIndex: 0 } : {}),
    });
  },

  focusWindow: (id) => {
    const s = get();
    const {
      openWindows,
      selection,
      selectedItemIndex,
      insertBeforeIndex,
      project,
    } = s;
    const target = openWindows.find((w) => w.id === id);
    if (!target) return;
    const topZ = maxZ(openWindows) + 1;
    // Bug fix (July 2026): clicking or DRAGGING a window calls focusWindow, which used to
    // clear selectedItemIndex — silently removing the Inspector's field-drop editor so
    // field drops "worked once then refused" after any window move. Keep the selected item
    // when the focused window is the entity already being edited; only reset when switching.
    const sameEntity = selection.kind === target.kind && selection.name === target.name;
    set({
      openWindows: openWindows.map((w) => (w.id === id ? { ...w, z: topZ } : w)),
      activeWindowId: id,
      selection: { kind: target.kind, name: target.name },
      selectedItemIndex: sameEntity ? selectedItemIndex : null,
      insertBeforeIndex: insertIndexWhenSwitchingEntity(
        project,
        target.kind,
        target.name,
        sameEntity,
        insertBeforeIndex,
      ),
      ...applyProcessWindowTransition(
        selection,
        target.kind,
        target.name,
        sameEntity,
        s.processUiByName,
        pickProcessUi(s),
      ),
    });
  },

  minimizeWindow: (id) => {
    const s = get();
    const {
      openWindows,
      activeWindowId,
      selection,
      selectedItemIndex,
      insertBeforeIndex,
      project,
    } = s;
    const next = openWindows.map((w) => (w.id === id ? { ...w, minimized: true } : w));
    let nextActive = activeWindowId;
    if (activeWindowId === id) {
      const visible = next.filter((w) => !w.minimized);
      nextActive = visible.length
        ? visible.reduce((top, w) => (w.z > top.z ? w : top), visible[0]).id
        : null;
    }
    // Same selection-sync as closeWindow: the palette + insert target must follow the
    // window that becomes active when the current one is minimized (owner Issue 2).
    const nextActiveWin = nextActive ? next.find((w) => w.id === nextActive) ?? null : null;
    const sameEntity =
      !!nextActiveWin &&
      selection.kind === nextActiveWin.kind &&
      selection.name === nextActiveWin.name;
    const toTarget = nextActiveWin
      ? { kind: nextActiveWin.kind, name: nextActiveWin.name }
      : selectionWindowTarget(selection);
    set({
      openWindows: next,
      activeWindowId: nextActive,
      selection: nextActiveWin ? { kind: nextActiveWin.kind, name: nextActiveWin.name } : selection,
      selectedItemIndex: sameEntity ? selectedItemIndex : null,
      insertBeforeIndex: nextActiveWin
        ? insertIndexWhenSwitchingEntity(
            project,
            nextActiveWin.kind,
            nextActiveWin.name,
            sameEntity,
            insertBeforeIndex,
          )
        : 0,
      ...applyProcessWindowTransition(
        selection,
        toTarget.kind,
        toTarget.name,
        sameEntity,
        s.processUiByName,
        pickProcessUi(s),
      ),
    });
  },

  restoreWindow: (id) => {
    const s = get();
    const {
      openWindows,
      selection,
      selectedItemIndex,
      insertBeforeIndex,
      project,
    } = s;
    const target = openWindows.find((w) => w.id === id);
    if (!target) return;
    const topZ = maxZ(openWindows) + 1;
    const sameEntity = selection.kind === target.kind && selection.name === target.name;
    set({
      openWindows: openWindows.map((w) =>
        w.id === id ? { ...w, minimized: false, z: topZ } : w,
      ),
      activeWindowId: id,
      selection: { kind: target.kind, name: target.name },
      selectedItemIndex: sameEntity ? selectedItemIndex : null,
      insertBeforeIndex: insertIndexWhenSwitchingEntity(
        project,
        target.kind,
        target.name,
        sameEntity,
        insertBeforeIndex,
      ),
      ...applyProcessWindowTransition(
        selection,
        target.kind,
        target.name,
        sameEntity,
        s.processUiByName,
        pickProcessUi(s),
      ),
    });
  },

  setWindowBounds: (id, bounds) => {
    const { openWindows } = get();
    set({
      openWindows: openWindows.map((w) => (w.id === id ? { ...w, ...bounds } : w)),
    });
  },

  setProject: (project) =>
    set({
      project,
      dirty: false,
      statusMessage: `Opened ${project.name}`,
      selectedItemIndex: null,
      insertBeforeIndex: 0,
      processInsertPath: ROOT_INSERT_PATH,
      processInsertIndex: 0,
      processUiByName: {},
      selectedProcessCommandPath: null,
      processStatementPanel: "none",
      openWindows: [],
      activeWindowId: null,
      cascadeIndex: 0,
    }),
  setSelection: (selection) => {
    const s = get();
    const sameEntity =
      s.selection.kind === selection.kind && s.selection.name === selection.name;
    const toTarget = selectionWindowTarget(selection);
    set({
      selection,
      selectedItemIndex: null,
      insertBeforeIndex:
        selection.kind === "form" && selection.name
          ? insertIndexForForm(s.project, selection.name)
          : 0,
      ...applyProcessWindowTransition(
        s.selection,
        toTarget.kind,
        toTarget.name,
        sameEntity && toTarget.kind === "process",
        s.processUiByName,
        pickProcessUi(s),
      ),
    });
  },
  setEditorTab: (editorTab) => set({ editorTab }),
  setStatus: (statusMessage) => set({ statusMessage }),
  setSelectedItemIndex: (selectedItemIndex) => {
    const { project, selection, insertBeforeIndex } = get();
    let nextInsert = insertBeforeIndex;
    if (selectedItemIndex !== null && selection.kind === "form" && selection.name) {
      const form = project.forms.find((f) => f.name === selection.name);
      if (form) nextInsert = Math.min(selectedItemIndex + 1, form.items.length);
    }
    set({ selectedItemIndex, insertBeforeIndex: nextInsert });
  },
  setInsertBeforeIndex: (index) => {
    const { project, selection } = get();
    if (selection.kind !== "form" || !selection.name) return;
    const form = project.forms.find((f) => f.name === selection.name);
    if (!form) return;
    const clamped = Math.max(0, Math.min(index, form.items.length));
    set({ insertBeforeIndex: clamped });
  },
  moveSelectedFormItem: (direction) => {
    const { project, selection, selectedItemIndex } = get();
    if (selection.kind !== "form" || !selection.name || selectedItemIndex === null) return;
    const form = project.forms.find((f) => f.name === selection.name);
    if (!form) return;
    const delta = direction === "up" ? -1 : 1;
    const target = selectedItemIndex + delta;
    if (target < 0 || target >= form.items.length) return;
    const items = [...form.items];
    const [moved] = items.splice(selectedItemIndex, 1);
    items.splice(target, 0, moved);
    set({
      project: {
        ...project,
        forms: project.forms.map((f) =>
          f.name === selection.name ? { ...f, items } : f,
        ),
      },
      dirty: true,
      selectedItemIndex: target,
      insertBeforeIndex: target + 1,
      statusMessage: `Moved item ${direction}`,
    });
  },
  setProcessInsertPath: (path) => {
    const { selection } = get();
    if (selection.kind !== "process" || !selection.name) return;
    set({ processInsertPath: path, processInsertIndex: 0 });
  },
  setProcessInsertPoint: (path, index) => {
    const { selection } = get();
    if (selection.kind !== "process" || !selection.name) return;
    set({ processInsertPath: path, processInsertIndex: Math.max(0, index) });
  },
  setSelectedProcessCommandPath: (path) => {
    const { selection, processStatementPanel, project } = get();
    if (selection.kind !== "process" || !selection.name) return;
    if (path == null) {
      set({ selectedProcessCommandPath: null });
      return;
    }
    const proc = project.processes?.find((p) => p.name === selection.name);
    const cmd = proc ? getProcessCommandAtPath(proc.commands ?? [], path) : null;
    let nextPanel = processStatementPanel;
    if (cmd) {
      const panelForCmd = processPanelKeyForCommand(cmd);
      // If panel stays open when selecting nested Show/Set inside an If block (legacy).
      if (panelForCmd && !(processStatementPanel === "if" && cmd.cmd !== "if")) {
        nextPanel = panelForCmd;
      }
    }
    set({ selectedProcessCommandPath: path, processStatementPanel: nextPanel });
  },
  setProcessStatementPanel: (panel) => {
    const { selection } = get();
    if (selection.kind !== "process" || !selection.name) return;
    set({ processStatementPanel: panel });
  },
  toggleProcessStatementPanel: (label) => {
    const { selection, processStatementPanel } = get();
    if (selection.kind !== "process" || !selection.name) return;
    const key = processPanelKeyForLabel(label);
    if (!key) return;
    const nextPanel = processStatementPanel === key ? "none" : key;
    set({
      processStatementPanel: nextPanel,
      selectedProcessCommandPath: null,
    });
    // Drop stale form/document targets so If/Set green boxes own double-click insert.
    setActiveFieldTarget(null);
  },
  moveSelectedProcessCommand: (direction) => {
    const { project, selection, selectedProcessCommandPath } = get();
    if (selection.kind !== "process" || !selection.name || !selectedProcessCommandPath) return;
    const proc = project.processes?.find((p) => p.name === selection.name);
    if (!proc) return;
    const moved = moveProcessCommandAtPath(
      proc.commands ?? [],
      selectedProcessCommandPath,
      direction,
    );
    if (!moved) return;
    const processes = (project.processes ?? []).map((p) =>
      p.name === selection.name ? { ...p, commands: moved.commands } : p,
    );
    set({
      project: { ...project, processes },
      dirty: true,
      selectedProcessCommandPath: moved.newPath,
      statusMessage: `Moved statement ${direction}`,
    });
  },
  createLinkedProcessForForm: (formName, role) => {
    const { project } = get();
    const form = project.forms.find((f) => f.name === formName);
    if (!form) return;
    const processes = project.processes ?? [];
    const name = nextLinkedProcessName(role, processes);
    const linkPatch =
      role === "Pre" ? { preProcess: name } : { process: name };
    set({
      project: {
        ...project,
        processes: [...processes, { name, commands: [] }],
        forms: project.forms.map((f) => (f.name === formName ? { ...f, ...linkPatch } : f)),
      },
      dirty: true,
      selection: { kind: "process", name },
      statusMessage: `Created and linked ${name}`,
    });
    get().openWindow("process", name);
  },
  linkProcessToForm: (processName, formName, role) => {
    const { project } = get();
    const forms = project.forms.map((f) => {
      if (f.name !== formName) return f;
      return role === "Pre" ? { ...f, preProcess: processName } : { ...f, process: processName };
    });
    set({
      project: { ...project, forms },
      dirty: true,
      statusMessage: `Linked ${processName} as ${role}-Process to ${formName}`,
    });
  },
  unlinkProcessFromForm: (processName, formName, role) => {
    const { project } = get();
    const forms = project.forms.map((f) => {
      if (f.name !== formName) return f;
      if (role === "Pre" && f.preProcess === processName) {
        return { ...f, preProcess: undefined };
      }
      if (role === "Post" && f.process === processName) {
        return { ...f, process: undefined };
      }
      return f;
    });
    set({
      project: { ...project, forms },
      dirty: true,
      statusMessage: `Disconnected ${processName} from ${formName}`,
    });
  },
  setShowLogin: (showLogin) => set({ showLogin }),
  setShowDeployResult: (showDeployResult) => set({ showDeployResult }),
  setCredentials: (credentials) => {
    saveCredentials(credentials);
    set({ credentials, showLogin: false });
  },

  newProject: (options) => {
    // Default (blank) project auto-creates one "Form 1"; the explicit "Empty"
    // template must produce a truly empty project with no forms/processes/documents.
    const empty = options?.empty ?? false;
    const project = empty
      ? { ...emptyProject(), forms: [], processes: [], documents: [] }
      : emptyProject();
    // Decision 2 (July 2026): start empty — do NOT auto-open the first form. The
    // canvas shows its placeholder until the designer clicks a node in Explorer.
    set({
      project,
      dirty: true,
      selection: empty ? { kind: "forms" } : { kind: "form", name: "Form 1" },
      statusMessage: empty ? "New empty project" : "New project",
      selectedItemIndex: null,
      insertBeforeIndex: 0,
      processInsertPath: ROOT_INSERT_PATH,
      processInsertIndex: 0,
      processUiByName: {},
      selectedProcessCommandPath: null,
      processStatementPanel: "none",
      openWindows: [],
      activeWindowId: null,
      cascadeIndex: 0,
    });
  },

  loadTemplate: async (samplePath) => {
    const res = await fetch(`/samples/templates/${samplePath}`);
    if (!res.ok) throw new Error(`Template not found: ${samplePath}`);
    const raw = await res.text();
    get().importJson(raw);
    set({ dirty: true, statusMessage: `New project from template` });
  },

  addForm: () => {
    const { project } = get();
    const names = project.forms.map((f) => f.name);
    const name = nextLabel("Form", names, " ");
    const forms = [...project.forms, { name, items: [] }];
    set({
      project: { ...project, forms },
      dirty: true,
      selection: { kind: "form", name },
      statusMessage: `Added form ${name}`,
    });
  },

  addProcess: () => {
    const { project } = get();
    const processes = project.processes ?? [];
    const names = processes.map((p) => p.name);
    const name = nextLabel("Process", names, " ");
    set({
      project: { ...project, processes: [...processes, { name, commands: [] }] },
      dirty: true,
      selection: { kind: "process", name },
      statusMessage: `Added process ${name}`,
    });
  },

  addDocument: () => {
    const { project } = get();
    const documents = project.documents ?? [];
    const names = documents.map((d) => d.name);
    const name = nextLabel("Document", names, " ");
    set({
      project: {
        ...project,
        documents: [...documents, { name, content: [{ type: "paragraph", align: "left", nodes: [] }] }],
      },
      dirty: true,
      selection: { kind: "document", name },
      statusMessage: `Added document ${name}`,
    });
  },

  toggleFormStartPoint: (name) => {
    const { project } = get();
    const target = project.forms.find((f) => f.name === name);
    if (!target) return;
    const next = !target.startPoint;
    const forms = project.forms.map((f) =>
      f.name === name ? { ...f, startPoint: next } : f,
    );
    set({
      project: { ...project, forms },
      dirty: true,
      statusMessage: next
        ? `${name} is now the starting point`
        : `${name} is no longer the starting point`,
    });
  },

  toggleFormBlockBack: (name) => {
    const { project } = get();
    const target = project.forms.find((f) => f.name === name);
    if (!target) return;
    const next = !target.blockBackButton;
    const forms = project.forms.map((f) =>
      f.name === name ? { ...f, blockBackButton: next } : f,
    );
    set({
      project: { ...project, forms },
      dirty: true,
      statusMessage: next
        ? `Back button blocked on ${name}`
        : `Back button allowed on ${name}`,
    });
  },

  moveSelectedNode: (direction) => {
    const { project, selection } = get();
    const delta = direction === "up" ? -1 : 1;

    const reorder = <T extends { name: string }>(list: T[]): T[] | null => {
      const index = list.findIndex((n) => n.name === selection.name);
      const target = index + delta;
      if (index < 0 || target < 0 || target >= list.length) return null;
      const next = [...list];
      const [moved] = next.splice(index, 1);
      next.splice(target, 0, moved);
      return next;
    };

    if (selection.kind === "form") {
      const forms = reorder(project.forms);
      if (!forms) return;
      set({ project: { ...project, forms }, dirty: true, statusMessage: `Moved ${selection.name} ${direction}` });
    } else if (selection.kind === "process") {
      const processes = reorder(project.processes ?? []);
      if (!processes) return;
      set({ project: { ...project, processes }, dirty: true, statusMessage: `Moved ${selection.name} ${direction}` });
    } else if (selection.kind === "document") {
      const documents = reorder(project.documents ?? []);
      if (!documents) return;
      set({ project: { ...project, documents }, dirty: true, statusMessage: `Moved ${selection.name} ${direction}` });
    }
  },

  // Inline rename (legacy ComponentList.Rename): trims whitespace, rejects empty and
  // duplicate names within the same category. Returns true on success, false when the
  // name was rejected so the UI can surface feedback.
  renameForm: (oldName, newName) => {
    const { project, selection, openWindows, activeWindowId } = get();
    const trimmed = newName.trim();
    if (!trimmed) return false;
    if (trimmed === oldName) return true;
    if (!project.forms.some((f) => f.name === oldName)) return false;
    if (project.forms.some((f) => f.name === trimmed)) return false;
    const forms = project.forms.map((f) =>
      f.name === oldName ? { ...f, name: trimmed } : f,
    );
    const selectionMoved =
      selection.kind === "form" && selection.name === oldName;
    const oldWinId = windowId("form", oldName);
    set({
      project: { ...project, forms },
      dirty: true,
      selection: selectionMoved ? { kind: "form", name: trimmed } : selection,
      statusMessage: `Renamed form to ${trimmed}`,
      openWindows: remapWindows(openWindows, "form", oldName, trimmed),
      activeWindowId:
        activeWindowId === oldWinId ? windowId("form", trimmed) : activeWindowId,
    });
    return true;
  },

  renameProcess: (oldName, newName) => {
    const { project, selection, openWindows, activeWindowId, processUiByName } = get();
    const processes = project.processes ?? [];
    const trimmed = newName.trim();
    if (!trimmed) return false;
    if (trimmed === oldName) return true;
    if (!processes.some((p) => p.name === oldName)) return false;
    if (processes.some((p) => p.name === trimmed)) return false;
    const nextProcesses = processes.map((p) =>
      p.name === oldName ? { ...p, name: trimmed } : p,
    );
    // Process references live as name strings on forms (preProcess / process);
    // update every link so form → process wiring survives the rename.
    const forms = project.forms.map((f) => {
      if (f.preProcess !== oldName && f.process !== oldName) return f;
      const next = { ...f };
      if (f.preProcess === oldName) next.preProcess = trimmed;
      if (f.process === oldName) next.process = trimmed;
      return next;
    });
    const selectionMoved =
      selection.kind === "process" && selection.name === oldName;
    const oldWinId = windowId("process", oldName);
    set({
      project: { ...project, processes: nextProcesses, forms },
      dirty: true,
      selection: selectionMoved ? { kind: "process", name: trimmed } : selection,
      statusMessage: `Renamed process to ${trimmed}`,
      openWindows: remapWindows(openWindows, "process", oldName, trimmed),
      processUiByName: remapProcessUiCache(processUiByName, oldName, trimmed),
      activeWindowId:
        activeWindowId === oldWinId ? windowId("process", trimmed) : activeWindowId,
    });
    return true;
  },

  renameDocument: (oldName, newName) => {
    const { project, selection, openWindows, activeWindowId } = get();
    const documents = project.documents ?? [];
    const trimmed = newName.trim();
    if (!trimmed) return false;
    if (trimmed === oldName) return true;
    if (!documents.some((d) => d.name === oldName)) return false;
    if (documents.some((d) => d.name === trimmed)) return false;
    const nextDocuments = documents.map((d) =>
      d.name === oldName ? { ...d, name: trimmed } : d,
    );
    const selectionMoved =
      selection.kind === "document" && selection.name === oldName;
    const oldWinId = windowId("document", oldName);
    set({
      project: { ...project, documents: nextDocuments },
      dirty: true,
      selection: selectionMoved ? { kind: "document", name: trimmed } : selection,
      statusMessage: `Renamed document to ${trimmed}`,
      openWindows: remapWindows(openWindows, "document", oldName, trimmed),
      activeWindowId:
        activeWindowId === oldWinId ? windowId("document", trimmed) : activeWindowId,
    });
    return true;
  },

  selectForm: (name) =>
    set({
      selection: { kind: "form", name },
      selectedItemIndex: null,
      insertBeforeIndex: insertIndexForForm(get().project, name),
    }),

  insertFormItem: (type) => {
    const { project, selection, insertBeforeIndex } = get();
    if (selection.kind !== "form" || !selection.name) return;
    const form = project.forms.find((f) => f.name === selection.name);
    if (!form) return;
    const prefix = type === "text" ? "T" : type === "heading" ? "H" : "Q";
    const labels = form.items.map((i) => i.label);
    const label =
      type === "field"
        ? "FIELD"
        : type === "break"
          ? "BREAK"
          : type === "skipInstructions"
            ? "SKIP"
            : nextLabel(prefix, labels);
    let item: FormItem = createDefaultItem(type, label);
    if (type === "field") {
      const fieldName = nextHiddenFieldName(project);
      item = { ...item, type: "field", fieldName, name: fieldName };
    }
    const insertAt = Math.max(0, Math.min(insertBeforeIndex, form.items.length));
    const items = [...form.items];
    items.splice(insertAt, 0, item);
    const forms = project.forms.map((f) =>
      f.name === selection.name ? { ...f, items } : f,
    );
    set({
      project: { ...project, forms },
      dirty: true,
      selectedItemIndex: insertAt,
      insertBeforeIndex: insertAt + 1,
      statusMessage: `Inserted ${type} (${label})`,
    });
  },

  updateFormItem: (formName, index, item) => {
    const { project } = get();
    const forms = project.forms.map((f) => {
      if (f.name !== formName) return f;
      const items = [...f.items];
      items[index] = item;
      return { ...f, items };
    });
    set({ project: { ...project, forms }, dirty: true });
  },

  updateForm: (formName, patch) => {
    const { project } = get();
    const forms = project.forms.map((f) => (f.name === formName ? { ...f, ...patch } : f));
    set({
      project: { ...project, forms },
      dirty: true,
      statusMessage: `Updated form ${formName}`,
    });
  },

  deleteFormItem: (formName, index) => {
    const { project, selectedItemIndex, selection, insertBeforeIndex } = get();
    const form = project.forms.find((f) => f.name === formName);
    const forms = project.forms.map((f) => {
      if (f.name !== formName) return f;
      return { ...f, items: f.items.filter((_, i) => i !== index) };
    });
    let nextSelected: number | null = selectedItemIndex;
    let nextInsert = insertBeforeIndex;
    if (selection.kind === "form" && selection.name === formName && form) {
      if (selectedItemIndex === index) nextSelected = null;
      else if (selectedItemIndex !== null && selectedItemIndex > index) {
        nextSelected = selectedItemIndex - 1;
      }
      if (index < insertBeforeIndex) nextInsert = insertBeforeIndex - 1;
      const newLength = form.items.length - 1;
      nextInsert = Math.max(0, Math.min(nextInsert, newLength));
    }
    set({
      project: { ...project, forms },
      dirty: true,
      selectedItemIndex: nextSelected,
      insertBeforeIndex: nextInsert,
      statusMessage: "Deleted item",
    });
  },

  deleteSelectedFormItem: () => {
    const { selection, selectedItemIndex } = get();
    if (selection.kind !== "form" || !selection.name || selectedItemIndex === null) return;
    get().deleteFormItem(selection.name, selectedItemIndex);
  },

  // Docked Processes/Statements palette insert (owner Issue 1, July 2026): mirrors
  // `insertFormItem` but targets the active PROCESS window. `selection` tracks the
  // active window (see focusWindow/openWindow/closeWindow), so the palette always
  // appends the statement to the process the designer is looking at.
  insertProcessCommand: (command) => {
    const { project, selection, processInsertPath, processInsertIndex } = get();
    if (selection.kind !== "process" || !selection.name) return;
    const processes = project.processes ?? [];
    const proc = processes.find((p) => p.name === selection.name);
    if (!proc) return;
    const inserted = insertCommandAtPoint(
      proc.commands ?? [],
      processInsertPath,
      processInsertIndex,
      command,
    );
    const nextProcesses = processes.map((p) =>
      p.name === selection.name ? { ...p, commands: inserted.commands } : p,
    );
    set({
      project: { ...project, processes: nextProcesses },
      dirty: true,
      processInsertPath: inserted.insertPath,
      processInsertIndex: inserted.insertIndex,
      statusMessage: `Inserted ${command.cmd} statement`,
    });
  },

  updateProcessCommands: (processName, commands) => {
    const { project } = get();
    const processes = (project.processes ?? []).map((p) =>
      p.name === processName ? { ...p, commands } : p,
    );
    set({ project: { ...project, processes }, dirty: true, statusMessage: "Process updated" });
  },

  updateDocumentContent: (documentName, content) => {
    const { project } = get();
    const documents = (project.documents ?? []).map((d) =>
      d.name === documentName ? { ...d, content } : d,
    );
    set({ project: { ...project, documents }, dirty: true, statusMessage: "Document updated" });
  },

  exportJson: () => JSON.stringify(get().project, null, 2),

  importJson: (raw) => {
    const project = JSON.parse(raw) as TawalaProject;
    const firstForm = project.forms[0]?.name;
    // Decision 2 (July 2026): start empty on load — select the first form in the
    // tree for context, but do NOT auto-open a window; the canvas stays empty
    // until the designer single-clicks a node.
    set({
      project,
      dirty: false,
      selection: firstForm ? { kind: "form", name: firstForm } : { kind: "forms" },
      statusMessage: `Loaded ${project.name}`,
      selectedItemIndex: null,
      insertBeforeIndex: 0,
      processInsertPath: ROOT_INSERT_PATH,
      processInsertIndex: 0,
      processUiByName: {},
      selectedProcessCommandPath: null,
      processStatementPanel: "none",
      openWindows: [],
      activeWindowId: null,
      cascadeIndex: 0,
    });
  },

  deploy: async () => {
    const { project, credentials } = get();
    if (!credentials) {
      set({ showLogin: true, statusMessage: "Enter deploy credentials (dev/dev)" });
      return;
    }
    set({ statusMessage: "Deploying…" });
    try {
      const result = await apiDeploy(project, credentials);
      if (result.status === "failure") {
        set({ statusMessage: `Deploy failed: ${result.error}` });
        return;
      }
      set({
        lastDeploy: result,
        showDeployResult: true,
        dirty: false,
        statusMessage: `Deployed ${project.name}`,
      });
    } catch (e) {
      set({ statusMessage: `Deploy error: ${e instanceof Error ? e.message : String(e)}` });
    }
  },
}));
