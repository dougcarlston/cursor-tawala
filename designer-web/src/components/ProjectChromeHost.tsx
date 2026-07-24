import { useSyncExternalStore } from "react";
import { FormItemStylesDialog } from "./FormItemStylesDialog";
import { EmailDeliveryDialog } from "./EmailDeliveryDialog";
import { PageHeaderDialog } from "./PageHeaderDialog";
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
import {
  clearPageHeaderDialog,
  getPageHeaderDialogOpen,
  subscribePageHeaderDialog,
} from "@/lib/pageHeaderDialog";
import { useProjectStore } from "@/store/projectStore";
import type { FibItem, McItem, TextItem } from "@/types/tawala";

/** Hosts Project → Styles… / Email Delivery… / Page Header… dialogs. */
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
  const pageHeaderOpen = useSyncExternalStore(
    subscribePageHeaderDialog,
    getPageHeaderDialogOpen,
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

  let stylesDialog = null;
  if (stylesKind) {
    const selectedOfKind =
      selectedItem && itemMatchesStylesKind(selectedItem, stylesKind)
        ? ([selectedItem] as Array<FibItem | McItem | TextItem>)
        : [];
    const formItemCount = countFormItemsOfKind(formItems, stylesKind);
    const kindLabel = stylesKindLabel(stylesKind);

    stylesDialog = (
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

  return (
    <>
      {stylesDialog}
      {emailOpen ? <EmailDeliveryDialog /> : null}
      <PageHeaderDialog open={pageHeaderOpen} onClose={() => clearPageHeaderDialog()} />
    </>
  );
}
