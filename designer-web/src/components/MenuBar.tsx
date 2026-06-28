import { useState, useEffect, useRef, ReactNode } from "react";
import { useProjectStore } from "@/store/projectStore";

interface Props {
  onOpen: () => void;
  onLoadSample: () => void;
  onDeploy: () => void;
}

export function MenuBar({ onOpen, onLoadSample, onDeploy }: Props) {
  const newProject = useProjectStore((s) => s.newProject);
  const exportJson = useProjectStore((s) => s.exportJson);
  const setStatus = useProjectStore((s) => s.setStatus);

  const saveJson = () => {
    const blob = new Blob([exportJson()], { type: "application/json" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `${useProjectStore.getState().project.name || "project"}.json`;
    a.click();
    URL.revokeObjectURL(url);
    setStatus("Project saved");
  };

  return (
    <nav className="menu-bar">
      <MenuDrop label="File">
        <button type="button" onClick={newProject}>
          New Project…
        </button>
        <button type="button" onClick={onOpen}>
          Open Project…
        </button>
        <button type="button" onClick={onLoadSample}>
          Open DirtBowl Sample…
        </button>
        <div className="menu-separator" />
        <button type="button" onClick={saveJson}>
          Save
        </button>
        <div className="menu-separator" />
        <button type="button" onClick={onDeploy}>
          Deploy…
        </button>
      </MenuDrop>
      <MenuDrop label="Edit">
        <button type="button" disabled>
          Cut
        </button>
        <button type="button" disabled>
          Copy
        </button>
        <button type="button" disabled>
          Paste
        </button>
        <button type="button" disabled>
          Delete
        </button>
      </MenuDrop>
      <MenuDrop label="View">
        <button type="button" disabled>
          Project Explorer
        </button>
        <button type="button" disabled>
          Fields Palette
        </button>
      </MenuDrop>
      <MenuDrop label="Project">
        <button type="button" onClick={onDeploy}>
          Deploy
        </button>
        <button type="button" disabled>
          Page Header…
        </button>
        <button type="button" disabled>
          Themes…
        </button>
      </MenuDrop>
      <MenuDrop label="Help">
        <button type="button" disabled>
          About Tawala Designer
        </button>
      </MenuDrop>
    </nav>
  );
}

function MenuDrop({ label, children }: { label: string; children: ReactNode }) {
  const [open, setOpen] = useState(false);
  const ref = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const close = (e: MouseEvent) => {
      if (ref.current && !ref.current.contains(e.target as Node)) setOpen(false);
    };
    document.addEventListener("mousedown", close);
    return () => document.removeEventListener("mousedown", close);
  }, []);

  return (
    <div className={`menu-item${open ? " open" : ""}`} ref={ref}>
      <button
        type="button"
        className="menu-trigger"
        onClick={() => setOpen((v) => !v)}
        onMouseEnter={() => open && setOpen(true)}
      >
        {label}
      </button>
      <div className="menu-dropdown">{children}</div>
    </div>
  );
}
