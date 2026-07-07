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

  setProject: (project) =>
    set({ project, dirty: false, statusMessage: `Opened ${project.name}`, selectedItemIndex: null }),
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
    set({
      project,
      dirty: true,
      selection: empty ? { kind: "forms" } : { kind: "form", name: "Form 1" },
      statusMessage: empty ? "New empty project" : "New project",
      selectedItemIndex: null,
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
    const { project, selection } = get();
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
    set({
      project: { ...project, forms },
      dirty: true,
      selection: selectionMoved ? { kind: "form", name: trimmed } : selection,
      statusMessage: `Renamed form to ${trimmed}`,
    });
    return true;
  },

  renameProcess: (oldName, newName) => {
    const { project, selection } = get();
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
    set({
      project: { ...project, processes: nextProcesses, forms },
      dirty: true,
      selection: selectionMoved ? { kind: "process", name: trimmed } : selection,
      statusMessage: `Renamed process to ${trimmed}`,
    });
    return true;
  },

  renameDocument: (oldName, newName) => {
    const { project, selection } = get();
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
    set({
      project: { ...project, documents: nextDocuments },
      dirty: true,
      selection: selectionMoved ? { kind: "document", name: trimmed } : selection,
      statusMessage: `Renamed document to ${trimmed}`,
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
    const label = nextLabel(prefix, labels);
    const item = createDefaultItem(type, label);
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
    set({
      project,
      dirty: false,
      selection: firstForm ? { kind: "form", name: firstForm } : { kind: "forms" },
      statusMessage: `Loaded ${project.name}`,
      selectedItemIndex: null,
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
