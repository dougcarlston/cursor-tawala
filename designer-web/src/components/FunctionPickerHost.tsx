import { useSyncExternalStore } from "react";
import { ConfigureFunctionDialog } from "./ConfigureFunctionDialog";
import { InsertFunctionDialog } from "./InsertFunctionDialog";
import {
  clearFunctionPickerRequest,
  getFunctionPickerRequest,
  requestFunctionPicker,
  subscribeFunctionPicker,
} from "@/lib/functionPicker";
import { getFunctionDef, type FunctionConfig, type FunctionDef } from "@/lib/functionCatalog";
import { getActivePaletteEditor } from "@/lib/formattingPaletteContext";
import {
  insertFunctionTokenAtSelection,
  type FunctionTokenRef,
} from "@/lib/functionTokens";
import { useProjectStore } from "@/store/projectStore";

/**
 * Orchestrates Insert Function → Configure Function and commits tokens into the
 * active palette editor. Mounted once in `App.tsx`.
 *
 * Insert → Configure advances by writing `configureFunctionId` onto the pending
 * request (not local React state). Local `picked` was cleared whenever `request`
 * changed or the host remounted, which resurfaced the Insert list after OK —
 * especially for no-parameter functions where Configure is small and the OK
 * click falls through onto **fx**.
 */
export function FunctionPickerHost() {
  const request = useSyncExternalStore(
    subscribeFunctionPicker,
    getFunctionPickerRequest,
    () => null,
  );

  const close = () => clearFunctionPickerRequest();

  const commitToken = (
    def: FunctionDef,
    config: FunctionConfig,
    editRef: FunctionTokenRef | null,
  ) => {
    const nextConfig =
      def.id === "display-image" ? { ...config, height: "" } : config;
    const commitConfig = request?.commitConfig;
    if (commitConfig) {
      commitConfig(def, nextConfig);
      close();
      return;
    }
    const handle = request?.editor ?? getActivePaletteEditor();
    if (!handle?.el?.isConnected) {
      useProjectStore
        .getState()
        .setStatus("Could not insert function — click inside Form Text or Document and try again");
      close();
      return;
    }
    handle.el.focus();
    handle.restoreSelection();
    insertFunctionTokenAtSelection(handle.el, def, nextConfig, editRef);
    handle.commit();
    close();
  };

  if (!request) return null;

  const editDef =
    request.mode === "edit" && request.existing && !request.configureFunctionId
      ? getFunctionDef(request.existing.functionId)
      : undefined;
  const skipListDef = request.configureFunctionId
    ? getFunctionDef(request.configureFunctionId)
    : undefined;

  const configure =
    (editDef && request.existing
      ? { def: editDef, config: request.existing.config, editRef: request.existing }
      : null) ??
    (skipListDef
      ? {
          def: skipListDef,
          config:
            request.existing?.functionId === skipListDef.id
              ? request.existing.config
              : undefined,
          editRef:
            request.mode === "edit" && request.existing ? request.existing : null,
        }
      : null);

  if (configure) {
    return (
      <ConfigureFunctionDialog
        def={configure.def}
        initialConfig={configure.config}
        onCancel={close}
        onSave={(config) => commitToken(configure.def, config, configure.editRef)}
      />
    );
  }

  return (
    <InsertFunctionDialog
      initialFunctionId={request.existing?.functionId}
      onCancel={close}
      onSelect={(def) => {
        // Persist the choice on the request so a remount / **fx** ghost-click
        // cannot wipe it and reopen the Insert list.
        requestFunctionPicker({
          mode: request.mode,
          existing: request.existing,
          editor: request.editor,
          commitConfig: request.commitConfig,
          configureFunctionId: def.id,
        });
      }}
    />
  );
}
