import { useEffect, useRef, useState, useSyncExternalStore } from "react";
import { useProjectStore } from "@/store/projectStore";
import { MenuBar } from "./components/MenuBar";
import { MainIconToolbar } from "./components/MainIconToolbar";
import { FormattingPalette } from "./components/FormattingPalette";
import { ProjectExplorer } from "./components/ProjectExplorer";
import { FormItemsPalette } from "./components/FormItemsPalette";
import { ProcessStatementsPalette } from "./components/ProcessStatementsPalette";
import { InspectorPanel } from "./components/InspectorPanel";
import { CanvasWindowManager } from "./components/mdi/CanvasWindowManager";
import { StatusBar } from "./components/StatusBar";
import { LoginDialog } from "./components/LoginDialog";
import { DeployDialog } from "./components/DeployDialog";
import { FunctionPickerHost } from "./components/FunctionPickerHost";
import { LinkInsertHost } from "./components/LinkInsertHost";
import { NewProjectDialog } from "./components/NewProjectDialog";
import { SaveAsDialog } from "./components/SaveAsDialog";
import type { TemplateEntry } from "@/templates/catalog";
import {
  clearProjectFileHandle,
  installDesignerShellGuards,
  isSaveAsDialogOpen,
  openProjectFromDisk,
  runShellDelete,
  setShellFileActions,
  subscribeSaveAsDialog,
} from "@/lib/shellCommands";

const ITEMS_COLUMN_WIDTH = 76 + 1; // .designer-items + border
const SPLITTER_WIDTH = 4;
/** Browser localStorage — panel widths are per-user/machine, not saved into project JSON. */
const PANEL_WIDTHS_KEY = "tawala.designer.panelWidths";
const DEFAULT_LEFT_WIDTH = 220;
const DEFAULT_RIGHT_WIDTH = 280;

function loadPanelWidths(): { left: number; right: number } {
  try {
    const raw = localStorage.getItem(PANEL_WIDTHS_KEY);
    if (!raw) return { left: DEFAULT_LEFT_WIDTH, right: DEFAULT_RIGHT_WIDTH };
    const parsed = JSON.parse(raw) as { left?: unknown; right?: unknown };
    const left = Number(parsed.left);
    const right = Number(parsed.right);
    return {
      left: Number.isFinite(left) ? Math.max(56, Math.min(left, 640)) : DEFAULT_LEFT_WIDTH,
      right: Number.isFinite(right) ? Math.max(60, Math.min(right, 640)) : DEFAULT_RIGHT_WIDTH,
    };
  } catch {
    return { left: DEFAULT_LEFT_WIDTH, right: DEFAULT_RIGHT_WIDTH };
  }
}

function savePanelWidths(left: number, right: number): void {
  try {
    localStorage.setItem(PANEL_WIDTHS_KEY, JSON.stringify({ left, right }));
  } catch {
    /* private mode / quota — ignore */
  }
}

export default function App() {
  const openWindows = useProjectStore((s) => s.openWindows);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const importJson = useProjectStore((s) => s.importJson);
  const loadTemplate = useProjectStore((s) => s.loadTemplate);
  const deploy = useProjectStore((s) => s.deploy);
  const fileRef = useRef<HTMLInputElement>(null);
  const [showNewProject, setShowNewProject] = useState(false);
  const showSaveAs = useSyncExternalStore(
    subscribeSaveAsDialog,
    isSaveAsDialogOpen,
    isSaveAsDialogOpen,
  );
  const [leftWidth, setLeftWidth] = useState(() => loadPanelWidths().left);
  const [rightWidth, setRightWidth] = useState(() => loadPanelWidths().right);

  // Re-assert boot guards after Fast Refresh of App (idempotent in shellCommands).
  useEffect(() => {
    installDesignerShellGuards();
  }, []);

  // Project Explorer: drag its right edge. Min ≈ icon-toolbar strip; leave room for Fields.
  const startResizeLeft = (e: React.PointerEvent<HTMLDivElement>) => {
    e.preventDefault();
    const startX = e.clientX;
    const startWidth = leftWidth;
    let latest = leftWidth;
    const onMove = (ev: PointerEvent) => {
      const next = Math.max(56, Math.min(startWidth + (ev.clientX - startX), window.innerWidth - rightWidth - 280));
      latest = next;
      setLeftWidth(next);
    };
    const onUp = () => {
      window.removeEventListener("pointermove", onMove);
      window.removeEventListener("pointerup", onUp);
      document.body.style.cursor = "";
      savePanelWidths(latest, rightWidth);
    };
    document.body.style.cursor = "ew-resize";
    window.addEventListener("pointermove", onMove);
    window.addEventListener("pointerup", onUp);
  };

  // Legacy FieldsPanel.cs: drag the left margin to resize; min 60px, capped near full width.
  const startResizeRight = (e: React.PointerEvent<HTMLDivElement>) => {
    e.preventDefault();
    const startX = e.clientX;
    const startWidth = rightWidth;
    let latest = rightWidth;
    const onMove = (ev: PointerEvent) => {
      const next = Math.max(60, Math.min(startWidth + (startX - ev.clientX), window.innerWidth - leftWidth - 280));
      latest = next;
      setRightWidth(next);
    };
    const onUp = () => {
      window.removeEventListener("pointermove", onMove);
      window.removeEventListener("pointerup", onUp);
      document.body.style.cursor = "";
      savePanelWidths(leftWidth, latest);
    };
    document.body.style.cursor = "ew-resize";
    window.addEventListener("pointermove", onMove);
    window.addEventListener("pointerup", onUp);
  };

  const onOpenFile = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
    // Hidden <input> open cannot keep a writable handle — next Save will ask where to put it.
    clearProjectFileHandle();
    const reader = new FileReader();
    reader.onload = () => {
      try {
        importJson(String(reader.result));
      } catch {
        alert("Could not parse project JSON.");
      }
    };
    reader.readAsText(file);
    e.target.value = "";
  };

  const onOpen = async () => {
    if (showNewProject) return;
    const opened = await openProjectFromDisk();
    if (!opened) fileRef.current?.click();
  };

  // File → New / Open accelerators (⌘N / ⌘O) call into App dialogs.
  useEffect(() => {
    setShellFileActions({
      onNewProject: () => setShowNewProject(true),
      onOpen: () => {
        if (showNewProject) return;
        void onOpen();
      },
    });
    return () => setShellFileActions({});
  }, [showNewProject]);

  const onPickTemplate = async (template: TemplateEntry) => {
    setShowNewProject(false);
    clearProjectFileHandle();
    try {
      if (template.id === "empty") {
        useProjectStore.getState().newProject({ empty: true });
        return;
      }
      await loadTemplate(template.samplePath);
    } catch (err) {
      alert(err instanceof Error ? err.message : "Could not load template.");
    }
  };

  // Owner Issue 1 (July 2026): the docked left palette CONTEXT-SWAPS on the active
  // MDI window kind — Items for a Form, the Statements palette ("Processes") for a
  // Process, and nothing at all for a Document (or an empty canvas), matching the
  // legacy Designer where the toolbox column tracks the active-node type.
  const activeWindow = openWindows.find((w) => w.id === activeWindowId) ?? null;
  const activeKind = activeWindow?.kind ?? null;

  // Main icon toolbar sits above PE (+ Items/Statements); Formatting Palette starts at canvas.
  const midPalette =
    activeKind === "form" || activeKind === "process" ? ITEMS_COLUMN_WIDTH : 0;
  const mainIconZoneWidth = leftWidth + SPLITTER_WIDTH + midPalette;

  const shell = {
    onNewProject: () => setShowNewProject(true),
    onOpen: () => {
      if (showNewProject) return;
      void onOpen();
    },
    onDeploy: () => {
      if (showNewProject) return;
      void deploy();
    },
    onDelete: () => {
      if (showNewProject) return;
      runShellDelete();
    },
  };

  return (
    <div className="designer-app">
      <input
        ref={fileRef}
        type="file"
        accept=".json,application/json"
        hidden
        onChange={onOpenFile}
      />
      <MenuBar {...shell} />
      <div className="designer-chrome-row">
        <MainIconToolbar {...shell} zoneWidth={mainIconZoneWidth} />
        <FormattingPalette activeKind={activeKind} flushLeft />
      </div>
      <div className="designer-main">
        <aside
          className="designer-left"
          style={{ width: leftWidth, minWidth: 56, flex: "0 0 auto" }}
        >
          <ProjectExplorer />
        </aside>
        <div
          className="designer-left-splitter"
          role="separator"
          aria-orientation="vertical"
          aria-label="Resize Project Explorer"
          onPointerDown={startResizeLeft}
        />
        {activeKind === "form" && (
          <aside className="designer-items">
            <FormItemsPalette />
          </aside>
        )}
        {activeKind === "process" && (
          <aside className="designer-items">
            <ProcessStatementsPalette />
          </aside>
        )}
        <main className="designer-center">
          <CanvasWindowManager />
        </main>
        <div
          className="designer-right-splitter"
          role="separator"
          aria-orientation="vertical"
          aria-label="Resize Fields column"
          onPointerDown={startResizeRight}
        />
        <aside
          className="designer-right"
          style={{ width: rightWidth, minWidth: 60, flex: "0 0 auto" }}
        >
          <InspectorPanel />
        </aside>
      </div>
      <StatusBar />
      <LoginDialog />
      <DeployDialog />
      <NewProjectDialog
        open={showNewProject}
        onClose={() => setShowNewProject(false)}
        onSelect={(t) => void onPickTemplate(t)}
      />
      <SaveAsDialog open={showSaveAs} />
      <FunctionPickerHost />
      <LinkInsertHost />
    </div>
  );
}
