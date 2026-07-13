/**
 * Shared File / Edit / Project actions for MenuBar and the main icon toolbar
 * so duplicates always invoke the same handlers.
 */

import { useProjectStore } from "@/store/projectStore";

export type ShellEditCommand = "cut" | "copy" | "paste" | "undo" | "redo";

/** Active disk file for quiet re-save (File System Access API). Cleared on New/Open elsewhere. */
let projectFileHandle: FileSystemFileHandle | null = null;

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

function suggestedProjectFileName(): string {
  const name = useProjectStore.getState().project.name?.trim() || "project";
  return name.toLowerCase().endsWith(".json") ? name : `${name}.json`;
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
 * file; later Saves rewrite that file quietly.
 */
export async function saveProjectToDownload(): Promise<void> {
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
}

/**
 * Force a new location (legacy Save As). Clears the remembered handle first.
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
  return selection.kind === "form" && selection.name != null && selectedItemIndex !== null;
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
