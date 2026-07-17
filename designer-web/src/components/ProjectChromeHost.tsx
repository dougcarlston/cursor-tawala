import { useSyncExternalStore } from "react";
import { FormItemStylesDialog } from "./FormItemStylesDialog";
import { TabsDialog } from "./TabsDialog";
import {
  clearFormItemStylesRequest,
  getFormItemStylesRequest,
  itemMatchesStylesKind,
  subscribeFormItemStyles,
} from "@/lib/formItemStyles";
import {
  clearTabsDialog,
  isTabsDialogOpen,
  subscribeTabsDialog,
} from "@/lib/tabStops";
import { useProjectStore } from "@/store/projectStore";
import type { FibItem, FormItem, McItem, TextItem } from "@/types/tawala";

/** Hosts Project → Styles… and Project → Tabs… dialogs. */
export function ProjectChromeHost() {
  const stylesKind = useSyncExternalStore(
    subscribeFormItemStyles,
    getFormItemStylesRequest,
    () => null,
  );
  const tabsOpen = useSyncExternalStore(subscribeTabsDialog, isTabsDialogOpen, () => false);
  const project = useProjectStore((s) => s.project);
  const selection = useProjectStore((s) => s.selection);
  const selectedItemIndex = useProjectStore((s) => s.selectedItemIndex);
  const updateFormItem = useProjectStore((s) => s.updateFormItem);
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

    return (
      <FormItemStylesDialog
        kind={stylesKind}
        selectedItems={selectedOfKind}
        onCancel={() => clearFormItemStylesRequest()}
        onApply={(patched) => {
          if (!formName || selectedItemIndex == null) return;
          updateFormItem(formName, selectedItemIndex, patched);
          clearFormItemStylesRequest();
          setStatus(`Applied ${stylesKind} style to selected item`);
        }}
      />
    );
  }

  if (tabsOpen) {
    const canApply =
      !!formName &&
      !!selectedItem &&
      (selectedItem.type === "text" ||
        selectedItem.type === "fib" ||
        selectedItem.type === "mc" ||
        selectedItem.type === "heading");
    const initialStops =
      canApply && Array.isArray(selectedItem.tabPositions) ? selectedItem.tabPositions : [];

    return (
      <TabsDialog
        initialStops={initialStops}
        canApply={canApply}
        onCancel={() => clearTabsDialog()}
        onOk={(stops) => {
          if (!canApply || !formName || selectedItemIndex == null || !selectedItem) {
            clearTabsDialog();
            return;
          }
          let next: FormItem;
          if (stops.length > 0) {
            next = { ...selectedItem, tabPositions: stops };
          } else {
            const { tabPositions: _removed, ...rest } = selectedItem;
            next = rest as FormItem;
          }
          updateFormItem(formName, selectedItemIndex, next);
          clearTabsDialog();
          setStatus(
            stops.length
              ? `Tab stops updated (${stops.length}) — used on Deploy`
              : "Tab stops cleared — Deploy uses defaults",
          );
        }}
      />
    );
  }

  return null;
}
