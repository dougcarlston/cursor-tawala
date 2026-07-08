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
import { nextHiddenFieldName } from "@/lib/fieldNames";
import { DeployCredentials, DeployResult, loadCredentials, saveCredentials } from "@/api/deploy";
import { deployProject as apiDeploy } from "@/api/deploy";

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

interface ProjectState {
  project: TawalaProject;
  dirty: boolean;
  selection: Selection;
  editorTab: EditorTab;
  statusMessage: string;
  selectedItemIndex: number | null;
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
      const { selection, selectedItemIndex } = get();
      const sameEntity = selection.kind === kind && selection.name === name;
      set({
        openWindows: openWindows.map((w) =>
          w.id === id ? { ...w, z: topZ, minimized: false } : w,
        ),
        activeWindowId: id,
        selection: { kind, name },
        selectedItemIndex: sameEntity ? selectedItemIndex : null,
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
    set({
      openWindows: [...openWindows, win],
      activeWindowId: id,
      selection: { kind, name },
      selectedItemIndex: null,
      cascadeIndex: cascadeIndex + 1,
      ...tabPatch,
    });
  },

  closeWindow: (id) => {
    const { openWindows, activeWindowId, selection, selectedItemIndex } = get();
    const next = openWindows.filter((w) => w.id !== id);
    let nextActive = activeWindowId === id ? null : activeWindowId;
    if (nextActive === null && next.length > 0) {
      // Fall back to the top-most remaining window.
      nextActive = next.reduce((top, w) => (w.z > top.z ? w : top), next[0]).id;
    }
    // Bug fix (owner Issue 2, July 2026): keep `selection` — which drives the docked
    // palette (Items/Processes) and every insert target — in sync with the window that
    // becomes active. Previously close/minimize moved `activeWindowId` to a fallback
    // window WITHOUT updating `selection`, so closing a Process/Document that fell back
    // to a Form window left the Items palette greyed even though a Form was active.
    const nextActiveWin = nextActive ? next.find((w) => w.id === nextActive) ?? null : null;
    const sameEntity =
      !nextActiveWin ||
      (selection.kind === nextActiveWin.kind && selection.name === nextActiveWin.name);
    set({
      openWindows: next,
      activeWindowId: nextActive,
      selection: nextActiveWin ? { kind: nextActiveWin.kind, name: nextActiveWin.name } : selection,
      selectedItemIndex: sameEntity ? selectedItemIndex : null,
      // Decision 1: reset the cascade once the canvas is empty so the next window
      // opens back at the origin instead of continuing to march down-right.
      ...(next.length === 0 ? { cascadeIndex: 0 } : {}),
    });
  },

  focusWindow: (id) => {
    const { openWindows, selection, selectedItemIndex } = get();
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
    });
  },

  minimizeWindow: (id) => {
    const { openWindows, activeWindowId, selection, selectedItemIndex } = get();
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
      !nextActiveWin ||
      (selection.kind === nextActiveWin.kind && selection.name === nextActiveWin.name);
    set({
      openWindows: next,
      activeWindowId: nextActive,
      selection: nextActiveWin ? { kind: nextActiveWin.kind, name: nextActiveWin.name } : selection,
      selectedItemIndex: sameEntity ? selectedItemIndex : null,
    });
  },

  restoreWindow: (id) => {
    const { openWindows, selection, selectedItemIndex } = get();
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
      openWindows: [],
      activeWindowId: null,
      cascadeIndex: 0,
    }),
  setSelection: (selection) => set({ selection, selectedItemIndex: null }),
  setEditorTab: (editorTab) => set({ editorTab }),
  setStatus: (statusMessage) => set({ statusMessage }),
  setSelectedItemIndex: (selectedItemIndex) => set({ selectedItemIndex }),
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
    const { project, selection, openWindows, activeWindowId } = get();
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

  selectForm: (name) => set({ selection: { kind: "form", name }, selectedItemIndex: null }),

  insertFormItem: (type) => {
    const { project, selection } = get();
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
    const forms = project.forms.map((f) =>
      f.name === selection.name ? { ...f, items: [...f.items, item] } : f,
    );
    set({
      project: { ...project, forms },
      dirty: true,
      selectedItemIndex: form.items.length,
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
    const { project, selectedItemIndex, selection } = get();
    const forms = project.forms.map((f) => {
      if (f.name !== formName) return f;
      return { ...f, items: f.items.filter((_, i) => i !== index) };
    });
    let nextSelected: number | null = selectedItemIndex;
    if (selection.kind === "form" && selection.name === formName) {
      if (selectedItemIndex === index) nextSelected = null;
      else if (selectedItemIndex !== null && selectedItemIndex > index) {
        nextSelected = selectedItemIndex - 1;
      }
    }
    set({
      project: { ...project, forms },
      dirty: true,
      selectedItemIndex: nextSelected,
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
    const { project, selection } = get();
    if (selection.kind !== "process" || !selection.name) return;
    const processes = project.processes ?? [];
    if (!processes.some((p) => p.name === selection.name)) return;
    const nextProcesses = processes.map((p) =>
      p.name === selection.name
        ? { ...p, commands: [...(p.commands ?? []), structuredClone(command)] }
        : p,
    );
    set({
      project: { ...project, processes: nextProcesses },
      dirty: true,
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
