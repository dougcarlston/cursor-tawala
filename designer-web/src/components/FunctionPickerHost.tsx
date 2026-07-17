import { useEffect, useState, useSyncExternalStore } from "react";
import { ConfigureFunctionDialog } from "./ConfigureFunctionDialog";
import { InsertFunctionDialog } from "./InsertFunctionDialog";
import {
  clearFunctionPickerRequest,
  getFunctionPickerRequest,
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
 * UI is derived from the current request on every render so we never paint a blank
 * frame while useEffect catches up (that looked like fx “doing nothing”).
 */
export function FunctionPickerHost() {
  const request = useSyncExternalStore(
    subscribeFunctionPicker,
    getFunctionPickerRequest,
    () => null,
  );
  /** Set when the user picks a function from the Insert list (insert flow only). */
  const [picked, setPicked] = useState<{
    def: FunctionDef;
    config?: FunctionConfig;
    editRef: FunctionTokenRef | null;
  } | null>(null);

  useEffect(() => {
    setPicked(null);
  }, [request]);

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
    request.mode === "edit" && request.existing
      ? getFunctionDef(request.existing.functionId)
      : undefined;
  const skipListDef = request.configureFunctionId
    ? getFunctionDef(request.configureFunctionId)
    : undefined;

  const configure =
    picked ??
    (editDef && request.existing
      ? { def: editDef, config: request.existing.config, editRef: request.existing }
      : null) ??
    (skipListDef
      ? { def: skipListDef, config: undefined, editRef: null as FunctionTokenRef | null }
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
        setPicked({
          def,
          config: request.existing?.config,
          editRef: request.existing ?? null,
        });
      }}
    />
  );
}
