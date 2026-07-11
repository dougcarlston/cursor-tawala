import { useMemo, useSyncExternalStore } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FieldsPalette } from "./FieldsPalette";
import {
  getFieldsPaletteConditionsFormSnapshot,
  subscribeFieldsPaletteConditionsForm,
} from "@/lib/fieldsPaletteContext";

/** Right sidebar — dedicated Fields column (Properties removed July 10, 2026). */
export function InspectorPanel() {
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const openWindows = useProjectStore((s) => s.openWindows);
  const processInsertPath = useProjectStore((s) => s.processInsertPath);

  const conditionsRecordForm = useSyncExternalStore(
    subscribeFieldsPaletteConditionsForm,
    getFieldsPaletteConditionsFormSnapshot,
  );

  const activeWindow = openWindows.find((w) => w.id === activeWindowId);
  const processWindowActive =
    selection.kind === "process" || activeWindow?.kind === "process";

  const activeProcessName =
    selection.kind === "process" && selection.name
      ? selection.name
      : activeWindow?.kind === "process"
        ? activeWindow.name
        : null;

  const processRecordContext = useMemo(() => {
    if (!processWindowActive || !activeProcessName || conditionsRecordForm) return null;
    const proc = project.processes?.find((p) => p.name === activeProcessName);
    if (!proc) return null;
    return { commands: proc.commands ?? [], insertPath: processInsertPath };
  }, [
    processWindowActive,
    activeProcessName,
    conditionsRecordForm,
    project.processes,
    processInsertPath,
  ]);

  const formName =
    selection.kind === "form" && selection.name
      ? selection.name
      : activeWindow?.kind === "form"
        ? activeWindow.name
        : undefined;

  return (
    <div className="inspector-panel inspector-panel-fields-only">
      <div className="panel-title">Fields</div>
      <FieldsPalette
        project={project}
        activeFormName={formName}
        processRecordContext={processRecordContext}
        conditionsRecordForm={conditionsRecordForm}
      />
    </div>
  );
}
