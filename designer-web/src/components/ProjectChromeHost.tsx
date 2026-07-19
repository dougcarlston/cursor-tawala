import { useSyncExternalStore } from "react";
import { FormItemStylesDialog } from "./FormItemStylesDialog";
import { EmailDeliveryDialog } from "./EmailDeliveryDialog";
import {
  applyStyleToAllFormItems,
  clearFormItemStylesRequest,
  countFormItemsOfKind,
  getFormItemStylesRequest,
  itemMatchesStylesKind,
  stylesKindLabel,
  subscribeFormItemStyles,
  type FibStyleDraft,
  type McStyleDraft,
  type TextStyleDraft,
} from "@/lib/formItemStyles";
import {
  getEmailDeliveryOpen,
  subscribeEmailDelivery,
} from "@/lib/emailDelivery";
import { useProjectStore } from "@/store/projectStore";
import type { FibItem, McItem, TextItem } from "@/types/tawala";

/** Hosts Project → Styles… and Email Delivery… dialogs. */
export function ProjectChromeHost() {
  const stylesKind = useSyncExternalStore(
    subscribeFormItemStyles,
    getFormItemStylesRequest,
    () => null,
  );
  const emailOpen = useSyncExternalStore(
    subscribeEmailDelivery,
    getEmailDeliveryOpen,
    () => false,
  );
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
  const updateForm = useProjectStore((s) => s.updateForm);
  const setStatus = useProjectStore((s) => s.setStatus);

  const formName = selection.kind === "form" ? selection.name : null;
  const form = formName ? project.forms.find((f) => f.name === formName) : null;
  const formItems = form?.items ?? [];
  const selectedItem =
    selectedItemIndex != null ? formItems[selectedItemIndex] ?? null : null;

  if (stylesKind) {
    const selectedOfKind =
      selectedItem && itemMatchesStylesKind(selectedItem, stylesKind)
        ? ([selectedItem] as Array<FibItem | McItem | TextItem>)
        : [];
    const formItemCount = countFormItemsOfKind(formItems, stylesKind);
    const kindLabel = stylesKindLabel(stylesKind);

    return (
      <FormItemStylesDialog
        kind={stylesKind}
        selectedItems={selectedOfKind}
        formItemCount={formItemCount}
        onCancel={() => clearFormItemStylesRequest()}
        onApplySelected={(patched) => {
          if (!formName || selectedItemIndex == null) return;
          updateFormItem(formName, selectedItemIndex, patched);
          clearFormItemStylesRequest();
          setStatus(`Applied ${kindLabel} style to selected item`);
        }}
        onApplyAll={(draft) => {
          if (!formName) return;
          const { items, changed } = applyStyleToAllFormItems(
            formItems,
            stylesKind,
            draft as FibStyleDraft | McStyleDraft | TextStyleDraft,
          );
          if (changed === 0) {
            clearFormItemStylesRequest();
            setStatus(`No ${kindLabel} items on this form`);
            return;
          }
          updateForm(formName, { items });
          clearFormItemStylesRequest();
          setStatus(
            `Applied ${kindLabel} style to all ${changed} item${changed === 1 ? "" : "s"} on ${formName}`,
          );
        }}
      />
    );
  }

  if (emailOpen) {
    return <EmailDeliveryDialog />;
  }

  return null;
}
