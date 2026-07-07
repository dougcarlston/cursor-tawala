import { useRef, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { MenuBar } from "./components/MenuBar";
import { ToolBar } from "./components/ToolBar";
import { ProjectExplorer } from "./components/ProjectExplorer";
import { FormItemsPalette } from "./components/FormItemsPalette";
import { ProcessStatementsPalette } from "./components/ProcessStatementsPalette";
import { InspectorPanel } from "./components/InspectorPanel";
import { CanvasWindowManager } from "./components/mdi/CanvasWindowManager";
import { StatusBar } from "./components/StatusBar";
import { LoginDialog } from "./components/LoginDialog";
import { DeployDialog } from "./components/DeployDialog";
import { NewProjectDialog } from "./components/NewProjectDialog";
import type { TemplateEntry } from "@/templates/catalog";

export default function App() {
  const selection = useProjectStore((s) => s.selection);
  const openWindows = useProjectStore((s) => s.openWindows);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const importJson = useProjectStore((s) => s.importJson);
  const loadTemplate = useProjectStore((s) => s.loadTemplate);
  const deploy = useProjectStore((s) => s.deploy);
  const deleteSelectedFormItem = useProjectStore((s) => s.deleteSelectedFormItem);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const fileRef = useRef<HTMLInputElement>(null);
  const [showNewProject, setShowNewProject] = useState(false);
  const [rightWidth, setRightWidth] = useState(280);

  // Legacy FieldsPanel.cs: drag the left margin to resize; min 60px, capped near full width.
  const startResizeRight = (e: React.PointerEvent<HTMLDivElement>) => {
    e.preventDefault();
    const startX = e.clientX;
    const startWidth = rightWidth;
    const onMove = (ev: PointerEvent) => {
      const next = startWidth + (startX - ev.clientX);
      setRightWidth(Math.max(60, Math.min(next, window.innerWidth - 200)));
    };
    const onUp = () => {
      window.removeEventListener("pointermove", onMove);
      window.removeEventListener("pointerup", onUp);
      document.body.style.cursor = "";
    };
    document.body.style.cursor = "ew-resize";
    window.addEventListener("pointermove", onMove);
    window.addEventListener("pointerup", onUp);
  };

  const onOpenFile = (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;
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

  const onPickTemplate = async (template: TemplateEntry) => {
    setShowNewProject(false);
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

  const canDelete =
    selection.kind === "form" && selection.name != null && selectedItemIndex !== null;

  // Owner Issue 1 (July 2026): the docked left palette CONTEXT-SWAPS on the active
  // MDI window kind — Items for a Form, the Statements palette ("Processes") for a
  // Process, and nothing at all for a Document (or an empty canvas), matching the
  // legacy Designer where the toolbox column tracks the active-node type.
  const activeWindow = openWindows.find((w) => w.id === activeWindowId) ?? null;
  const activeKind = activeWindow?.kind ?? null;

  return (
    <div className="designer-app">
      <input
        ref={fileRef}
        type="file"
        accept=".json,application/json"
        hidden
        onChange={onOpenFile}
      />
      <MenuBar
        onNewProject={() => setShowNewProject(true)}
        onOpen={() => fileRef.current?.click()}
        onDeploy={() => void deploy()}
        onDelete={() => deleteSelectedFormItem()}
        canDelete={canDelete}
      />
      <ToolBar onNewProject={() => setShowNewProject(true)} />
      <div className="designer-main">
        <aside className="designer-left">
          <ProjectExplorer />
        </aside>
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
    </div>
  );
}
