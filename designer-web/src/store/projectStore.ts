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

function nextLabel(prefix: string, existing: string[]): string {
  let n = 1;
  while (existing.includes(`${prefix}${n}`)) n++;
  return `${prefix}${n}`;
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
  newProject: () => void;
  loadTemplate: (samplePath: string) => Promise<void>;
  addForm: () => void;
  addProcess: () => void;
  addDocument: () => void;
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

  newProject: () => {
    const project = emptyProject();
    set({
      project,
      dirty: true,
      selection: { kind: "form", name: "Form 1" },
      statusMessage: "New empty project",
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
    const name = nextLabel("Form", names);
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
    const name = nextLabel("Process", names);
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
    const name = nextLabel("Document", names);
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
