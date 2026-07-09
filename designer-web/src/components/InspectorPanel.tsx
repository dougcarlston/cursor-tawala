import { useMemo, useSyncExternalStore } from "react";
import { useProjectStore } from "@/store/projectStore";
import { FormItem, TawalaForm } from "@/types/tawala";
import { FormItemProperties } from "./FormItemProperties";
import { FormProperties } from "./FormProperties";
import { FieldsPalette } from "./FieldsPalette";
import {
  getFieldsPaletteConditionsFormSnapshot,
  subscribeFieldsPaletteConditionsForm,
} from "@/lib/fieldsPaletteContext";

/** Right sidebar — form / item properties + fields palette (legacy Designer). */
export function InspectorPanel() {
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const activeWindowId = useProjectStore((s) => s.activeWindowId);
  const openWindows = useProjectStore((s) => s.openWindows);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const updateForm = useProjectStore((s) => s.updateForm);
  const deleteSelectedFormItem = useProjectStore((s) => s.deleteSelectedFormItem);
  const setSelectedItemIndex = useProjectStore((s) => s.setSelectedItemIndex);
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

  const form =
    selection.kind === "form" && selection.name
      ? project.forms.find((f) => f.name === selection.name)
      : undefined;

  const selectedItem =
    form && selectedItemIndex !== null ? form.items[selectedItemIndex] : undefined;

  const patchItem = (patch: Partial<FormItem>) => {
    if (!form || selectedItemIndex === null || !selectedItem) return;
    updateFormItem(form.name, selectedItemIndex, { ...selectedItem, ...patch } as FormItem);
  };

  const patchForm = (patch: Partial<TawalaForm>) => {
    if (!form) return;
    updateForm(form.name, patch);
  };

  return (
    <div className="inspector-panel">
      <div className="panel-title">Properties</div>
      <div className="inspector-properties">
        {processWindowActive ? (
          <p className="hint inspector-process-placeholder">
            Properties for statements appear in the Process window. Use the yellow connection
            banner there to attach or detach this process as Pre- or Post-process to a form.
          </p>
        ) : selectedItem ? (
          <>
            <div className="inspector-item-header">
              <span>
                [{selectedItem.type}] {selectedItem.label}
              </span>
              <button
                type="button"
                className="inspector-delete"
                title="Delete selected item (Del)"
                onClick={() => deleteSelectedFormItem()}
              >
                Delete
              </button>
            </div>
            <FormItemProperties item={selectedItem} onChange={patchItem} />
          </>
        ) : form ? (
          <FormProperties form={form} onChange={patchForm} />
        ) : (
          <p className="hint">Select a form in Project Explorer.</p>
        )}
        {form && selectedItem ? (
          <button
            type="button"
            className="inspector-back-to-form"
            onClick={() => setSelectedItemIndex(null)}
          >
            ← Form settings ({form.name})
          </button>
        ) : null}
      </div>

      <div className="panel-title inspector-fields-title">Fields</div>
      <FieldsPalette
        project={project}
        activeFormName={form?.name}
        processRecordContext={processRecordContext}
        conditionsRecordForm={conditionsRecordForm}
      />
    </div>
  );
}
