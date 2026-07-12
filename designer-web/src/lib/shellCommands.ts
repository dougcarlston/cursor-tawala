/**
 * Shared File / Edit / Project actions for MenuBar and the main icon toolbar
 * so duplicates always invoke the same handlers.
 */

import { useProjectStore } from "@/store/projectStore";

export type ShellEditCommand = "cut" | "copy" | "paste" | "undo" | "redo";

export function saveProjectToDownload(): void {
  const { exportJson, project, setStatus } = useProjectStore.getState();
  const blob = new Blob([exportJson()], { type: "application/json" });
  const url = URL.createObjectURL(blob);
  const a = document.createElement("a");
  a.href = url;
  a.download = `${project.name || "project"}.json`;
  a.click();
  URL.revokeObjectURL(url);
  setStatus("Project saved");
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
