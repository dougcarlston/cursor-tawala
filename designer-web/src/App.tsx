import { useRef } from "react";
import { useProjectStore } from "@/store/projectStore";
import { MenuBar } from "./components/MenuBar";
import { ToolBar } from "./components/ToolBar";
import { ProjectExplorer } from "./components/ProjectExplorer";
import { FieldsPalette } from "./components/FieldsPalette";
import { FormEditor } from "./components/FormEditor";
import { ProcessEditor } from "./components/ProcessEditor";
import { DocumentEditor } from "./components/DocumentEditor";
import { StatusBar } from "./components/StatusBar";
import { LoginDialog } from "./components/LoginDialog";
import { DeployDialog } from "./components/DeployDialog";

export default function App() {
  const selection = useProjectStore((s) => s.selection);
  const importJson = useProjectStore((s) => s.importJson);
  const deploy = useProjectStore((s) => s.deploy);
  const fileRef = useRef<HTMLInputElement>(null);

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

  const loadDirtbowlSample = async () => {
    try {
      const res = await fetch("/samples/dirtbowl_definition_v3.json");
      if (!res.ok) throw new Error("not found");
      importJson(await res.text());
    } catch {
      alert(
        "DirtBowl sample not found. Copy dirtbowl_definition_v3.json to designer-web/public/samples/",
      );
    }
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
      <MenuBar onOpen={() => fileRef.current?.click()} onLoadSample={loadDirtbowlSample} onDeploy={() => void deploy()} />
      <ToolBar />
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
        <aside className="designer-right">
          <FieldsPalette />
        </aside>
      </div>
      <StatusBar />
      <LoginDialog />
      <DeployDialog />
    </div>
  );
}
