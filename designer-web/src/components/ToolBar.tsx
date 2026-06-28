import { useProjectStore } from "@/store/projectStore";

export function ToolBar() {
  const newProject = useProjectStore((s) => s.newProject);
  const addForm = useProjectStore((s) => s.addForm);
  const exportJson = useProjectStore((s) => s.exportJson);
  const deploy = useProjectStore((s) => s.deploy);

  const save = () => {
    const blob = new Blob([exportJson()], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = "project.json";
    a.click();
    URL.revokeObjectURL(url);
  };

  return (
    <div className="toolbar" title="Standard toolbar (mirrors legacy Designer)">
      <button type="button" title="New" onClick={newProject}>
        New
      </button>
      <button type="button" title="Save" onClick={save}>
        Save
      </button>
      <button type="button" title="New Form" onClick={addForm}>
        +Form
      </button>
      <button type="button" title="Deploy" onClick={() => void deploy()}>
        Deploy
      </button>
    </div>
  );
}
