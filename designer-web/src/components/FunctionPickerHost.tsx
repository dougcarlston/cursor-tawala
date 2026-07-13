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

type Step = "pick" | "configure";

/**
 * Orchestrates Insert Function → Configure Function and commits tokens into the
 * active palette editor. Mounted once in `App.tsx`.
 */
export function FunctionPickerHost() {
  const request = useSyncExternalStore(
    subscribeFunctionPicker,
    getFunctionPickerRequest,
    () => null,
  );
  const [step, setStep] = useState<Step>("pick");
  const [selectedDef, setSelectedDef] = useState<FunctionDef | null>(null);
  const [editRef, setEditRef] = useState<FunctionTokenRef | null>(null);
  const [initialConfig, setInitialConfig] = useState<FunctionConfig | undefined>();

  useEffect(() => {
    if (!request) {
      setStep("pick");
      setSelectedDef(null);
      setEditRef(null);
      setInitialConfig(undefined);
      return;
    }

    if (request.mode === "edit" && request.existing) {
      const def = getFunctionDef(request.existing.functionId);
      if (def) {
        setEditRef(request.existing);
        setSelectedDef(def);
        setInitialConfig(request.existing.config);
        setStep("configure");
        return;
      }
    }

    if (request.configureFunctionId) {
      const def = getFunctionDef(request.configureFunctionId);
      if (def) {
        setEditRef(null);
        setSelectedDef(def);
        setInitialConfig(undefined);
        setStep("configure");
        return;
      }
    }

    setEditRef(request.existing ?? null);
    setSelectedDef(null);
    setInitialConfig(undefined);
    setStep("pick");
  }, [request]);

  const close = () => clearFunctionPickerRequest();

  const commitToken = (def: FunctionDef, config: FunctionConfig) => {
    // Width-only DISPLAY IMAGE: drop any leftover height so Deploy preserves aspect ratio.
    const nextConfig =
      def.id === "display-image" ? { ...config, height: "" } : config;
    const commitConfig = request?.commitConfig;
    if (commitConfig) {
      commitConfig(def, nextConfig);
      close();
      return;
    }
    const handle = request?.editor ?? getActivePaletteEditor();
    if (!handle) {
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

  if (step === "pick") {
    return (
      <InsertFunctionDialog
        initialFunctionId={editRef?.functionId}
        onCancel={close}
        onSelect={(def) => {
          setSelectedDef(def);
          setInitialConfig(editRef ? editRef.config : undefined);
          setStep("configure");
        }}
      />
    );
  }

  if (step === "configure" && selectedDef) {
    return (
      <ConfigureFunctionDialog
        def={selectedDef}
        initialConfig={initialConfig}
        onCancel={close}
        onSave={(config) => commitToken(selectedDef, config)}
      />
    );
  }

  return null;
}
