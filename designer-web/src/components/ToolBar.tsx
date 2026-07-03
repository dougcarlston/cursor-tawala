import { useProjectStore } from "@/store/projectStore";
import { FormInsertButtons } from "./FormInsertMenu";

interface Props {
  onNewProject: () => void;
}

export function ToolBar({ onNewProject }: Props) {
  const addForm = useProjectStore((s) => s.addForm);
  const addProcess = useProjectStore((s) => s.addProcess);
  const addDocument = useProjectStore((s) => s.addDocument);
  const exportJson = useProjectStore((s) => s.exportJson);
  const deploy = useProjectStore((s) => s.deploy);
  const deleteSelectedFormItem = useProjectStore((s) => s.deleteSelectedFormItem);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const selection = useProjectStore((s) => s.selection);
  const canDelete =
    selection.kind === "form" && selection.name != null && selectedItemIndex !== null;
  const addAction =
    selection.kind === "process" || selection.kind === "processes"
      ? { label: "+Process", title: "New Process", onClick: addProcess }
      : selection.kind === "document" || selection.kind === "documents"
        ? { label: "+Document", title: "New Document", onClick: addDocument }
        : { label: "+Form", title: "New Form", onClick: addForm };

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
      <button type="button" title="New Project" onClick={onNewProject}>
        New
      </button>
      <button type="button" title="Save" onClick={save}>
        Save
      </button>
      <button type="button" title={addAction.title} onClick={addAction.onClick}>
        {addAction.label}
      </button>
      <button
        type="button"
        title="Delete selected item"
        disabled={!canDelete}
        onClick={() => deleteSelectedFormItem()}
      >
        Delete
      </button>
      <button type="button" title="Deploy" onClick={() => void deploy()}>
        Deploy
      </button>
      <span className="toolbar-sep" />
      <FormInsertButtons compact />
    </div>
  );
}
