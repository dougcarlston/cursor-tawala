import { useRef, useState } from "react";
import { useProjectStore } from "@/store/projectStore";
import { MenuBar } from "./components/MenuBar";
import { ToolBar } from "./components/ToolBar";
import { ProjectExplorer } from "./components/ProjectExplorer";
import { InspectorPanel } from "./components/InspectorPanel";
import { FormEditor } from "./components/FormEditor";
import { ProcessEditor } from "./components/ProcessEditor";
import { DocumentEditor } from "./components/DocumentEditor";
import { StatusBar } from "./components/StatusBar";
import { LoginDialog } from "./components/LoginDialog";
import { DeployDialog } from "./components/DeployDialog";
import { NewProjectDialog } from "./components/NewProjectDialog";
import type { TemplateEntry } from "@/templates/catalog";

export default function App() {
  const selection = useProjectStore((s) => s.selection);
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
        useProjectStore.getState().newProject();
        return;
      }
      await loadTemplate(template.samplePath);
    } catch (err) {
      alert(err instanceof Error ? err.message : "Could not load template.");
    }
  };

  const canDelete =
    selection.kind === "form" && selection.name != null && selectedItemIndex !== null;

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
        <main className="designer-center">
          {selection.kind === "form" && selection.name ? (
            <FormEditor formName={selection.name} />
          ) : selection.kind === "process" && selection.name ? (
            <ProcessEditor processName={selection.name} />
          ) : selection.kind === "document" && selection.name ? (
            <DocumentEditor documentName={selection.name} />
          ) : (
            <div className="placeholder-editor">
              Select a form, process, or document in Project Explorer
            </div>
          )}
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
