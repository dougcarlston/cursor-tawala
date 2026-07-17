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

/** In-app Save As dialog open state (Menu / ⇧⌘S → name prompt before picker/download). */
let saveAsDialogOpen = false;
type SaveAsDialogListener = (open: boolean) => void;
const saveAsDialogListeners = new Set<SaveAsDialogListener>();

function emitSaveAsDialog(): void {
  saveAsDialogListeners.forEach((cb) => cb(saveAsDialogOpen));
}

/** Subscribe to in-app Save As dialog visibility. Exported for App host. */
export function subscribeSaveAsDialog(listener: SaveAsDialogListener): () => void {
  saveAsDialogListeners.add(listener);
  listener(saveAsDialogOpen);
  return () => {
    saveAsDialogListeners.delete(listener);
  };
}

export function isSaveAsDialogOpen(): boolean {
  return saveAsDialogOpen;
}

/** Open the in-app Save As name prompt (does not write yet). */
export function requestSaveAsDialog(): void {
  saveAsDialogOpen = true;
  emitSaveAsDialog();
}

/** Cancel the in-app Save As prompt without writing. */
export function cancelSaveAsDialog(): void {
  if (!saveAsDialogOpen) return;
  saveAsDialogOpen = false;
  emitSaveAsDialog();
  useProjectStore.getState().setStatus("Save As cancelled");
}

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
 * Suggested disk name for File System Access / download fallback / Save As dialog.
 * Uses the project `name` (empty template defaults to `Untitled` → `Untitled.json`).
 * Ensures a single `.json` suffix; strips path / reserved characters.
 * Exported for unit tests.
 */
export function suggestedProjectFileName(projectName?: string): string {
  const raw = (projectName ?? useProjectStore.getState().project.name)?.trim() || "MyProject";
  // Strip path separators / reserved chars so the native picker stays on the leaf name.
  const safe = raw.replace(/[\\/:*?"<>|]+/g, "_").replace(/\s+/g, " ").trim() || "MyProject";
  return safe.toLowerCase().endsWith(".json") ? safe : `${safe}.json`;
}

/**
 * Apple Safari (not Chrome / Edge / Firefox / Android WebView).
 * Exported for unit tests.
 */
export function isAppleSafari(): boolean {
  if (typeof navigator === "undefined") return false;
  const ua = navigator.userAgent;
  return /Safari/i.test(ua) && !/Chrome|Chromium|CriOS|Edg|Firefox|Android/i.test(ua);
}

/**
 * Same-origin form POST → `/api/download-project` with Content-Disposition.
 * Safari honors this far more reliably than a Blob `<a download>` click (which
 * can silently no-op after a menu close, and historically nudged `.json`→`.html`).
 * Exported for unit tests.
 */
export function downloadJsonViaFormPost(json: string, filename: string): boolean {
  try {
    const iframeName = "tawala-json-dl";
    let iframe = document.querySelector<HTMLIFrameElement>(`iframe[name="${iframeName}"]`);
    if (!iframe) {
      iframe = document.createElement("iframe");
      iframe.name = iframeName;
      iframe.title = "Download";
      iframe.setAttribute("aria-hidden", "true");
      iframe.style.cssText =
        "position:fixed;left:-9999px;top:0;width:1px;height:1px;border:0;opacity:0";
      document.body.appendChild(iframe);
    }

    const form = document.createElement("form");
    form.method = "POST";
    form.action = "/api/download-project";
    form.target = iframeName;
    form.enctype = "application/x-www-form-urlencoded";
    form.style.display = "none";

    const nameField = document.createElement("input");
    nameField.type = "hidden";
    nameField.name = "filename";
    nameField.value = filename;
    form.appendChild(nameField);

    const jsonField = document.createElement("textarea");
    jsonField.name = "json";
    jsonField.value = json;
    form.appendChild(jsonField);

    document.body.appendChild(form);
    form.submit();
    form.remove();
    return true;
  } catch (err) {
    console.warn("Form download failed; falling back to blob anchor", err);
    return false;
  }
}

/**
 * Blob `<a download>` path (Firefox / Chromium fallback when File System Access
 * fails). Appends in-DOM and uses MouseEvent — WebKit no-ops detached `a.click()`.
 * Exported for unit tests.
 */
export function downloadJsonViaBlobAnchor(json: string, filename: string): void {
  const blob = new Blob([json], { type: "application/json;charset=utf-8" });
  const url = URL.createObjectURL(blob);
  // FileSaver uses createElementNS; some WebKit builds are picky about `<a download>`.
  const a = document.createElementNS("http://www.w3.org/1999/xhtml", "a") as HTMLAnchorElement;
  a.href = url;
  a.download = filename;
  a.rel = "noopener";
  a.style.display = "none";
  document.body.appendChild(a);
  try {
    a.dispatchEvent(new MouseEvent("click", { bubbles: true, cancelable: true, view: window }));
  } catch {
    a.click();
  }
  // Do not remove in the same turn — Safari can miss the download if the node vanishes immediately.
  window.setTimeout(() => {
    a.remove();
    URL.revokeObjectURL(url);
  }, 2_000);
}

/**
 * One-shot JSON download when File System Access save picker is unavailable
 * (Safari / Firefox) or fails.
 *
 * Safari: prefer form POST + Content-Disposition (via local API). Other browsers
 * (and Safari if the form path throws): Blob anchor.
 * Exported for unit tests.
 */
export function downloadJsonFallback(json: string, filename: string): void {
  if (isAppleSafari() && downloadJsonViaFormPost(json, filename)) return;
  downloadJsonViaBlobAnchor(json, filename);
}

/** True when the browser exposes a usable Save file picker (Chromium). */
export function canUseSaveFilePicker(): boolean {
  // Safari never ships disk pickers; refuse even if a stub appears in TP builds.
  if (isAppleSafari()) return false;
  return typeof savePickerWindow().showSaveFilePicker === "function";
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
 * Shared one-shot download used by Safari/Firefox Save and Save As (and as the
 * Chromium fallback when File System Access fails).
 */
function completeDownloadSave(json: string, filename: string): void {
  downloadJsonFallback(json, filename);
  useProjectStore.setState({
    dirty: false,
    statusMessage: isAppleSafari()
      ? `Saved to Downloads: ${filename} (Safari has no Save As folder picker)`
      : `Project saved (download: ${filename})`,
  });
}

/**
 * Write project JSON to disk under `filename`.
 *
 * `preferExistingHandle`: ordinary Save reuses a remembered Chromium handle.
 * Save As always clears the handle first and passes false so the picker reopens.
 *
 * Important (Chrome): call `showSaveFilePicker` **before** `exportJson()`. Large
 * projects (embedded images) make stringify slow enough that user activation expires
 * and the native Save As picker never appears.
 */
async function writeProjectToDisk(
  filename: string,
  options: { preferExistingHandle: boolean },
): Promise<void> {
  if (saveInFlight) return;
  saveInFlight = true;
  try {
    const win = savePickerWindow();

    // Safari/Firefox: never enter the File System Access branch.
    if (!canUseSaveFilePicker()) {
      const json = useProjectStore.getState().exportJson();
      completeDownloadSave(json, filename);
      return;
    }

    try {
      if (!options.preferExistingHandle || !projectFileHandle) {
        // Must run while the click gesture is still active — before heavy JSON work.
        projectFileHandle = await win.showSaveFilePicker!({
          suggestedName: filename,
          types: [
            {
              description: "Tawala project JSON",
              accept: { "application/json": [".json"] },
            },
          ],
        });
      }
      const json = useProjectStore.getState().exportJson();
      await writeJsonToHandle(projectFileHandle, json);
      useProjectStore.setState({
        dirty: false,
        statusMessage: `Saved ${projectFileHandle.name}`,
      });
      return;
    } catch (err) {
      const name = err instanceof DOMException ? err.name : "";
      if (name === "AbortError") {
        useProjectStore.getState().setStatus("Save cancelled");
        return;
      }
      // Permission / API quirks → fall back to a one-shot download below.
      console.warn("File System Access save failed; using download fallback", err);
      clearProjectFileHandle();
    }

    const json = useProjectStore.getState().exportJson();
    completeDownloadSave(json, filename);
  } finally {
    saveInFlight = false;
  }
}

/**
 * Save the current project to disk.
 *
 * Uses the File System Access API when available so Chrome does not open its
 * Downloads shelf on every Save (the old `<a download>` path stacked Temp.json /
 * Save1.json entries across the toolbar). First Save may ask where to put the
 * file (suggested name = project name + `.json`); later Saves rewrite that file
 * quietly. Never opens a picker spontaneously — only File → Save / floppy / ⌘S.
 *
 * Safari / Firefox: force-download (no picker). Save As uses an in-app name
 * dialog first, then this same download path with the chosen name.
 */
export async function saveProjectToDownload(): Promise<void> {
  await writeProjectToDisk(suggestedProjectFileName(), { preferExistingHandle: true });
}

/**
 * File → Save As / Shift+⌘S — open the in-app name dialog (does not write yet).
 * Writing happens in {@link confirmSaveAs} after the user confirms.
 */
export function saveProjectAs(): void {
  requestSaveAsDialog();
}

/**
 * After the in-app Save As dialog confirms a name: clear any remembered Chromium
 * handle, ensure `.json`, then open the native picker (Chromium) or download
 * (Safari / no picker) under that name.
 *
 * Keep the name dialog open until the picker is requested so Chrome still has a
 * user gesture when `showSaveFilePicker` runs (then close the dialog).
 */
export async function confirmSaveAs(chosenName: string): Promise<void> {
  const filename = suggestedProjectFileName(chosenName);
  clearProjectFileHandle();
  try {
    await writeProjectToDisk(filename, { preferExistingHandle: false });
  } finally {
    if (saveAsDialogOpen) {
      saveAsDialogOpen = false;
      emitSaveAsDialog();
    }
  }
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
 * Confirm + delete a form canvas item.
 * Same “Are you sure?” cue as Explorer entity delete — Del/× used to remove items
 * with no prompt (easy to hit after focusing FIB Required / property strip).
 */
export function confirmAndDeleteFormItem(formName: string, index: number): boolean {
  const form = useProjectStore.getState().project.forms.find((f) => f.name === formName);
  const item = form?.items[index];
  if (!item) return false;
  const kind =
    item.type === "fib"
      ? "Fill-in-the-Blank"
      : item.type === "mc"
        ? "Multiple Choice"
        : item.type === "text"
          ? "Text"
          : item.type === "heading"
            ? "Heading"
            : item.type === "field"
              ? "Hidden Field"
              : item.type === "break"
                ? "Page Break"
                : item.type === "skipInstructions"
                  ? "Skip Instructions"
                  : "item";
  const label = "label" in item && item.label ? String(item.label) : String(index + 1);
  const ok = globalThis.confirm(`Are you sure you want to delete ${kind} "${label}"?`);
  if (!ok) {
    useProjectStore.getState().setStatus("Delete cancelled");
    return false;
  }
  useProjectStore.getState().deleteFormItem(formName, index);
  return true;
}

/** Confirm + delete the currently selected form canvas row. */
export function confirmAndDeleteSelectedFormItem(): boolean {
  const { selection, selectedItemIndex } = useProjectStore.getState();
  if (selection.kind !== "form" || !selection.name || selectedItemIndex === null) return false;
  return confirmAndDeleteFormItem(selection.name, selectedItemIndex);
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
  const ok = globalThis.confirm(
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
 * the selected Form / Process / Document (always with confirm).
 */
export function runShellDelete(): void {
  const { selection, selectedItemIndex } = useProjectStore.getState();
  if (selection.kind === "form" && selection.name && selectedItemIndex !== null) {
    confirmAndDeleteSelectedFormItem();
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
  return modKeyLabel() + "S";
}

/** Platform accelerator for File → Save As (Shift+⌘S / Shift+Ctrl+S). */
export function saveAsAcceleratorLabel(): string {
  return isApplePlatform() ? "⇧⌘S" : "Shift+Ctrl+S";
}

/** File → New Project (Ctrl+N / ⌘N). */
export function newProjectAcceleratorLabel(): string {
  return modKeyLabel() + "N";
}

/** File → Open Project (Ctrl+O / ⌘O). */
export function openProjectAcceleratorLabel(): string {
  return modKeyLabel() + "O";
}

export function cutAcceleratorLabel(): string {
  return modKeyLabel() + "X";
}
export function copyAcceleratorLabel(): string {
  return modKeyLabel() + "C";
}
export function pasteAcceleratorLabel(): string {
  return modKeyLabel() + "V";
}
export function deleteAcceleratorLabel(): string {
  return "Del";
}
export function undoAcceleratorLabel(): string {
  return modKeyLabel() + "Z";
}
/** Redo: Ctrl+Y on Windows/Linux; ⇧⌘Z on Mac (platform convention). */
export function redoAcceleratorLabel(): string {
  return isApplePlatform() ? "⇧⌘Z" : "Ctrl+Y";
}

function isApplePlatform(): boolean {
  return typeof navigator !== "undefined" && /Mac|iPhone|iPad|iPod/i.test(navigator.platform);
}

function modKeyLabel(): string {
  return isApplePlatform() ? "⌘" : "Ctrl+";
}

type ShellFileActions = {
  onNewProject?: () => void;
  onOpen?: () => void;
};

let shellFileActions: ShellFileActions = {};

/** App registers New/Open so File accelerators can open dialogs without circular imports. */
export function setShellFileActions(actions: ShellFileActions): void {
  shellFileActions = actions;
}

function isSaveChord(e: Pick<KeyboardEvent, "ctrlKey" | "metaKey" | "altKey" | "code" | "key" | "shiftKey">): boolean {
  if (!(e.ctrlKey || e.metaKey) || e.altKey) return false;
  if (e.code === "KeyS") return true;
  return e.key.toLowerCase() === "s";
}

function isModLetterChord(
  e: Pick<KeyboardEvent, "ctrlKey" | "metaKey" | "altKey" | "shiftKey" | "code" | "key">,
  letter: string,
): boolean {
  if (!(e.ctrlKey || e.metaKey) || e.altKey || e.shiftKey) return false;
  const code = `Key${letter.toUpperCase()}`;
  if (e.code === code) return true;
  return e.key.toLowerCase() === letter.toLowerCase();
}

/** Exported for unit tests — true when the event is Ctrl/Cmd+S (Save chord). */
export function eventIsSaveChord(
  e: Pick<KeyboardEvent, "ctrlKey" | "metaKey" | "altKey" | "code" | "key">,
): boolean {
  return isSaveChord(e);
}

/** Exported for unit tests — Ctrl/Cmd+N (no Shift/Alt). */
export function eventIsNewProjectChord(
  e: Pick<KeyboardEvent, "ctrlKey" | "metaKey" | "altKey" | "shiftKey" | "code" | "key">,
): boolean {
  return isModLetterChord(e, "n");
}

/** Exported for unit tests — Ctrl/Cmd+O (no Shift/Alt). */
export function eventIsOpenProjectChord(
  e: Pick<KeyboardEvent, "ctrlKey" | "metaKey" | "altKey" | "shiftKey" | "code" | "key">,
): boolean {
  return isModLetterChord(e, "o");
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
    // File → New / Open (before Save so Shift+S still wins for Save As).
    if (eventIsNewProjectChord(e)) {
      const marked = e as KeyboardEvent & { __tawalaFileChordHandled?: boolean };
      if (marked.__tawalaFileChordHandled) return;
      marked.__tawalaFileChordHandled = true;
      e.preventDefault();
      e.stopPropagation();
      e.stopImmediatePropagation();
      shellFileActions.onNewProject?.();
      return;
    }
    if (eventIsOpenProjectChord(e)) {
      const marked = e as KeyboardEvent & { __tawalaFileChordHandled?: boolean };
      if (marked.__tawalaFileChordHandled) return;
      marked.__tawalaFileChordHandled = true;
      e.preventDefault();
      e.stopPropagation();
      e.stopImmediatePropagation();
      shellFileActions.onOpen?.();
      return;
    }

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
      saveProjectAs();
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
