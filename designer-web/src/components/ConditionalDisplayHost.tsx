import { useSyncExternalStore } from "react";
import { ConditionalDisplayDialog } from "./ConditionalDisplayDialog";
import {
  clearConditionalDisplayRequest,
  getConditionalDisplayRequest,
  subscribeConditionalDisplay,
} from "@/lib/conditionalDisplay";
import { useProjectStore } from "@/store/projectStore";
import type { FormItem } from "@/types/tawala";

/** Hosts Form Item → Display conditionally… dialog. */
export function ConditionalDisplayHost() {
  const request = useSyncExternalStore(
    subscribeConditionalDisplay,
    getConditionalDisplayRequest,
    () => null,
  );
  const project = useProjectStore((s) => s.project);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);

  if (!request) return null;

  const form = project.forms.find((f) => f.name === request.formName);
  const item = form?.items?.[request.itemIndex] as FormItem | undefined;
  if (!item) {
    clearConditionalDisplayRequest();
    return null;
  }

  const close = () => clearConditionalDisplayRequest();

  return (
    <ConditionalDisplayDialog
      item={item}
      onCancel={close}
      onSave={(displayCondition) => {
        const next = { ...item } as FormItem & { displayCondition?: unknown };
        if (displayCondition === undefined) {
          delete next.displayCondition;
        } else {
          next.displayCondition = displayCondition;
        }
        updateFormItem(request.formName, request.itemIndex, next);
        useProjectStore.getState().setStatus(
          displayCondition
            ? "Conditional display updated"
            : "Conditional display cleared",
        );
        close();
      }}
    />
  );
}
