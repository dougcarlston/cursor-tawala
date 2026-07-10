import { useState, useEffect, useRef, ReactNode, useSyncExternalStore } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FORM_ITEM_PALETTE } from "@/types/tawala";
import {
  getFormattingFocusState,
  subscribeFormattingFocus,
} from "@/lib/formattingPaletteContext";
import { openFunctionPickerFromEditor } from "@/lib/functionPicker";

interface Props {
  onNewProject: () => void;
  onOpen: () => void;
  onDeploy: () => void;
  onDelete: () => void;
  canDelete: boolean;
}

export function MenuBar({ onNewProject, onOpen, onDeploy, onDelete, canDelete }: Props) {
  const exportJson = useProjectStore((s) => s.exportJson);
  const setStatus = useProjectStore((s) => s.setStatus);
  const insertFormItem = useProjectStore((s) => s.insertFormItem);
  const selection = useProjectStore((s) => s.selection);
  const editorTab = useProjectStore((s) => s.editorTab);
  const openWindows = useProjectStore((s) => s.openWindows);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const formCount = useProjectStore((s) => s.project.forms.length);
  const canInsert = selection.kind === "form" && Boolean(selection.name);
  const focus = useSyncExternalStore(
    subscribeFormattingFocus,
    getFormattingFocusState,
    getFormattingFocusState,
  );
  const activeWindow = openWindows.find((w) => w.id === activeWindowId) ?? null;
  const activeKind = activeWindow?.kind ?? null;
  const designActive = activeKind === "document" || editorTab === "design";
  const canInsertFunction =
    designActive &&
    (focus.kind === "text" ||
      (focus.kind === "document" && formCount >= 1));

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
        <button type="button" onClick={onNewProject}>
          New Project…
        </button>
        <button type="button" onClick={onOpen}>
          Open Project…
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
      <MenuDrop label="Insert">
        {FORM_ITEM_PALETTE.map(({ label, type }) => (
          <button
            key={type}
            type="button"
            disabled={!canInsert}
            onClick={() => insertFormItem(type)}
          >
            {label}
          </button>
        ))}
        <div className="menu-separator" />
        <button type="button" disabled={!canInsertFunction} onClick={openFunctionPickerFromEditor}>
          Function…
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
        <button type="button" disabled={!canDelete} onClick={onDelete}>
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
