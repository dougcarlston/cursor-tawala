/**
 * Shared File / Edit / Project actions for MenuBar and the main icon toolbar
 * so duplicates always invoke the same handlers.
 */

import { useProjectStore } from "@/store/projectStore";

export type ShellEditCommand = "cut" | "copy" | "paste" | "undo" | "redo";

/** Active disk file for quiet re-save (File System Access API). Cleared on New/Open elsewhere. */
let projectFileHandle: FileSystemFileHandle | null = null;

/** Blocks overlapping Save / Save As so ⌘S spam cannot open two native pickers. */
let saveInFlight = false;

type SavePickerWindow = Window &
  typeof globalThis & {
    showSaveFilePicker?: (options?: {
      suggestedName?: string;
      types?: Array<{
        description?: string;
        accept: Record<string, string[]>;
      }>;
    }) => Promise<FileSystemFileHandle>;
    showOpenFilePicker?: (options?: {
      multiple?: boolean;
      types?: Array<{
        description?: string;
        accept: Record<string, string[]>;
      }>;
    }) => Promise<FileSystemFileHandle[]>;
  };

function savePickerWindow(): SavePickerWindow {
  return window as SavePickerWindow;
}

/**
 * Suggested disk name for File System Access / download fallback.
 * Uses the project `name` (empty template defaults to `Untitled` → `Untitled.json`).
 * Exported for unit tests.
 */
export function suggestedProjectFileName(projectName?: string): string {
  const raw = (projectName ?? useProjectStore.getState().project.name)?.trim() || "MyProject";
  // Strip path separators / reserved chars so the native picker stays on the leaf name.
  const safe = raw.replace(/[\\/:*?"<>|]+/g, "_").replace(/\s+/g, " ").trim() || "MyProject";
  return safe.toLowerCase().endsWith(".json") ? safe : `${safe}.json`;
}

function downloadJsonFallback(json: string, filename: string): void {
  const blob = new Blob([json], { type: "application/json" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = filename;
  a.click();
  URL.revokeObjectURL(url);
}

async function writeJsonToHandle(handle: FileSystemFileHandle, json: string): Promise<void> {
  const writable = await handle.createWritable();
  await writable.write(json);
  await writable.close();
}

/** Drop the remembered file path (New Project, Open another file, cancelled context). */
export function clearProjectFileHandle(): void {
  projectFileHandle = null;
}

export function setProjectFileHandle(handle: FileSystemFileHandle | null): void {
  projectFileHandle = handle;
}

/**
 * Save the current project to disk.
 *
 * Uses the File System Access API when available so Chrome does not open its
 * Downloads shelf on every Save (the old `<a download>` path stacked Temp.json /
 * Save1.json entries across the toolbar). First Save may ask where to put the
 * file (suggested name = project name + `.json`); later Saves rewrite that file
 * quietly. Never opens a picker spontaneously — only File → Save / floppy / ⌘S
 * (or Shift+⌘S Save As).
 */
export async function saveProjectToDownload(): Promise<void> {
  if (saveInFlight) return;
  saveInFlight = true;
  try {
    const { exportJson } = useProjectStore.getState();
    const json = exportJson();
    const filename = suggestedProjectFileName();
    const win = savePickerWindow();

    try {
      if (typeof win.showSaveFilePicker === "function") {
        if (!projectFileHandle) {
          projectFileHandle = await win.showSaveFilePicker({
            suggestedName: filename,
            types: [
              {
                description: "Tawala project JSON",
                accept: { "application/json": [".json"] },
              },
            ],
          });
        }
        await writeJsonToHandle(projectFileHandle, json);
        useProjectStore.setState({
          dirty: false,
          statusMessage: `Saved ${projectFileHandle.name}`,
        });
        return;
      }
    } catch (err) {
      const name = err instanceof DOMException ? err.name : "";
      if (name === "AbortError") {
        useProjectStore.getState().setStatus("Save cancelled");
        return;
      }
      // Permission / API quirks → fall back to a one-shot download below.
      console.warn("File System Access save failed; using download fallback", err);
    }

    downloadJsonFallback(json, filename);
    useProjectStore.setState({
      dirty: false,
      statusMessage: "Project saved (download)",
    });
  } finally {
    saveInFlight = false;
  }
}

/**
 * Force a new location (legacy Save As). Clears the remembered handle first.
 * Only from File → Save As or Shift+⌘S — not from ordinary Save.
 */
export async function saveProjectAs(): Promise<void> {
  clearProjectFileHandle();
  await saveProjectToDownload();
}

/**
 * Open a project JSON via the File System Access API when possible so later Save
 * can rewrite the same file. Returns true if a file was opened.
 */
export async function openProjectFromDisk(): Promise<boolean> {
  const win = savePickerWindow();
  if (typeof win.showOpenFilePicker !== "function") return false;

  try {
    const [handle] = await win.showOpenFilePicker({
      multiple: false,
      types: [
        {
          description: "Tawala project JSON",
          accept: { "application/json": [".json"] },
        },
      ],
    });
    const file = await handle.getFile();
    const text = await file.text();
    useProjectStore.getState().importJson(text);
    projectFileHandle = handle;
    useProjectStore.getState().setStatus(`Opened ${handle.name}`);
    return true;
  } catch (err) {
    const name = err instanceof DOMException ? err.name : "";
    if (name === "AbortError") {
      useProjectStore.getState().setStatus("Open cancelled");
      return true; // handled; do not fall back to <input>
    }
    console.warn("File System Access open failed", err);
    return false;
  }
}

/** True when an MDI Form/Process/Document window is active (legacy IEditMenu context). */
export function shellEditContextActive(): boolean {
  const { openWindows, activeWindowId, editorTab } = useProjectStore.getState();
  const active = openWindows.find((w) => w.id === activeWindowId);
  if (!active) return false;
  if (active.kind === "form" && editorTab !== "design") return false;
  return true;
}

export function canDeployProject(): boolean {
  return useProjectStore.getState().project.forms.length > 0;
}

export function canDeleteSelection(): boolean {
  const { selection, selectedItemIndex } = useProjectStore.getState();
  // Form-item delete (canvas row selected).
  if (selection.kind === "form" && selection.name != null && selectedItemIndex !== null) {
    return true;
  }
  // Explorer entity delete (Form / Process / Document node selected).
  if (
    (selection.kind === "form" || selection.kind === "process" || selection.kind === "document") &&
    selection.name
  ) {
    return true;
  }
  return false;
}

/** True when Delete should remove a Form/Process/Document (not a form item). */
export function canDeleteProjectEntity(): boolean {
  const { selection, selectedItemIndex } = useProjectStore.getState();
  if (selection.kind === "form" && selectedItemIndex !== null) return false;
  return (
    (selection.kind === "form" || selection.kind === "process" || selection.kind === "document") &&
    !!selection.name
  );
}

/**
 * Confirm + delete the Explorer-selected Form / Process / Document.
 * Message mirrors legacy ConfirmDialog (`Delete {Type} "{name}"?`) with an Are-you-sure cue.
 * Returns true when the user confirmed and the entity was removed.
 */
export function confirmAndDeleteProjectEntity(): boolean {
  const { selection } = useProjectStore.getState();
  if (!canDeleteProjectEntity() || !selection.name) return false;
  const typeLabel =
    selection.kind === "form" ? "Form" : selection.kind === "process" ? "Process" : "Document";
  const ok = window.confirm(
    `Are you sure you want to delete ${typeLabel} "${selection.name}"?`,
  );
  if (!ok) {
    useProjectStore.getState().setStatus("Delete cancelled");
    return false;
  }
  return useProjectStore.getState().deleteSelectedEntity();
}

/**
 * Main toolbar / Edit → Delete: form item when a canvas row is selected, otherwise
 * the selected Form / Process / Document (with confirm).
 */
export function runShellDelete(): void {
  const { selection, selectedItemIndex } = useProjectStore.getState();
  if (selection.kind === "form" && selection.name && selectedItemIndex !== null) {
    useProjectStore.getState().deleteSelectedFormItem();
    return;
  }
  confirmAndDeleteProjectEntity();
}

/** Prefer rich-text / contenteditable commands when focus is in an editor. */
export function runShellEditCommand(command: ShellEditCommand): boolean {
  if (!shellEditContextActive()) return false;
  try {
    return document.execCommand(command);
  } catch {
    return false;
  }
}

export function openProjectManagerLocal(): void {
  useProjectStore
    .getState()
    .setStatus(
      "Project Manager opens the online Library. Locally use File → New Project… or Open Project…",
    );
}

/** Platform accelerator label for File → Save (DESIGNER_MENU_SPEC: Ctrl+S). */
export function saveAcceleratorLabel(): string {
  if (typeof navigator !== "undefined" && /Mac|iPhone|iPad|iPod/i.test(navigator.platform)) {
    return "⌘S";
  }
  return "Ctrl+S";
}

/** Platform accelerator for File → Save As (Shift+⌘S / Shift+Ctrl+S). */
export function saveAsAcceleratorLabel(): string {
  if (typeof navigator !== "undefined" && /Mac|iPhone|iPad|iPod/i.test(navigator.platform)) {
    return "⇧⌘S";
  }
  return "Shift+Ctrl+S";
}

function isSaveChord(e: Pick<KeyboardEvent, "ctrlKey" | "metaKey" | "altKey" | "code" | "key">): boolean {
  if (!(e.ctrlKey || e.metaKey) || e.altKey) return false;
  // Prefer `code` so layout / Caps Lock still maps to physical S.
  if (e.code === "KeyS") return true;
  return e.key.toLowerCase() === "s";
}

/** Exported for unit tests — true when the event is Ctrl/Cmd+S (Save chord). */
export function eventIsSaveChord(
  e: Pick<KeyboardEvent, "ctrlKey" | "metaKey" | "altKey" | "code" | "key">,
): boolean {
  return isSaveChord(e);
}

type ShellGuardBag = {
  keydown: (e: KeyboardEvent) => void;
  beforeunload: (e: BeforeUnloadEvent) => void;
};

const SHELL_GUARDS_KEY = "__tawalaDesignerShellGuards__";

function shellGuardBag(): Window & { [SHELL_GUARDS_KEY]?: ShellGuardBag } {
  return window as Window & { [SHELL_GUARDS_KEY]?: ShellGuardBag };
}

/**
 * File menu accelerators (DESIGNER_MENU_SPEC.md → File: Save = Ctrl+S).
 * Capture-phase on window + document so contenteditable Document/Form surfaces
 * cannot swallow the chord. Idempotent across StrictMode and Vite HMR — replaces
 * any prior handlers stored on window (App-only useEffect was easy to lose on HMR).
 */
export function installShellKeyboardShortcuts(): () => void {
  installDesignerShellGuards();
  return () => {
    /* Boot/HMR owns lifecycle — do not tear down on App unmount. */
  };
}

/** Prompt before refresh / tab close when the project has unsaved edits. */
export function installDirtyUnloadGuard(): () => void {
  installDesignerShellGuards();
  return () => {
    /* Boot/HMR owns lifecycle. */
  };
}

/**
 * Leave-warning contract: only prompt when `dirty` is true.
 * Soft refresh (Cmd+R), hard refresh (Cmd+Shift+R), and tab close all navigate —
 * none of them skip this guard. Exported for unit tests.
 */
export function applyDirtyBeforeUnload(
  e: { preventDefault: () => void; returnValue: string },
  dirty: boolean = useProjectStore.getState().dirty,
): boolean {
  if (!dirty) return false;
  e.preventDefault();
  // Chromium still requires returnValue to show the leave dialog.
  e.returnValue = "You have unsaved changes.";
  return true;
}

/**
 * Boot-time shell guards. Call from main.tsx (and safely from App remount).
 * Survives Vite HMR of App.tsx alone — prior fix only lived in App useEffects
 * and often left the owner with no Cmd+S / no leave warning after soft reloads.
 */
export function installDesignerShellGuards(): void {
  if (typeof window === "undefined") return;
  const host = shellGuardBag();
  const prev = host[SHELL_GUARDS_KEY];
  if (prev) {
    window.removeEventListener("keydown", prev.keydown, true);
    document.removeEventListener("keydown", prev.keydown, true);
    window.removeEventListener("beforeunload", prev.beforeunload);
  }

  const keydown = (e: KeyboardEvent) => {
    if (!isSaveChord(e)) return;
    // Window + document both listen in capture — handle only once per event.
    const marked = e as KeyboardEvent & { __tawalaSaveHandled?: boolean };
    if (marked.__tawalaSaveHandled) return;
    marked.__tawalaSaveHandled = true;
    // Capture as hard as the page can: stopDefault + stop immediate so nested
    // editors / other capture listeners cannot re-fire Save. Note: if Designer
    // runs inside Cursor Simple Browser / an Electron webview, the *host* IDE
    // may still receive ⌘S independently — prefer a normal Chrome/Safari tab
    // at localhost:5173 when authoring.
    e.preventDefault();
    e.stopPropagation();
    e.stopImmediatePropagation();
    if (e.shiftKey) {
      void saveProjectAs();
    } else {
      void saveProjectToDownload();
    }
  };

  const beforeunload = (e: BeforeUnloadEvent) => {
    applyDirtyBeforeUnload(e);
  };

  host[SHELL_GUARDS_KEY] = { keydown, beforeunload };
  window.addEventListener("keydown", keydown, true);
  document.addEventListener("keydown", keydown, true);
  window.addEventListener("beforeunload", beforeunload);
}
