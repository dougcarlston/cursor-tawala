/**
 * Form item Styles (legacy Format → Styles → FIB / MCQ / Text).
 * Owner Jul 17: live under Project → Styles; Format menu dropped (palette owns B/I/U).
 */

import type { FibItem, FormItem, FormItemType, McItem, TextItem } from "@/types/tawala";

export type StylesDialogKind = "fib" | "mc" | "text";

export type FibLabelPlacement = "above" | "left" | "right" | "freeform";

export interface FibStyleDraft {
  placement: FibLabelPlacement;
  alignRightSide: boolean;
}

export interface McStyleDraft {
  layout: "vertical" | "horizontal" | "multicolumn";
  columnCount: number; // 0 = Auto
  noPaddingBottom: boolean;
}

export interface TextStyleDraft {
  style: "normal" | "instructional" | "error";
  noPaddingBottom: boolean;
}

export function fibStyleToken(draft: FibStyleDraft): string {
  if (draft.placement === "above") return "topLabels";
  if (draft.placement === "freeform") return "freeform";
  if (draft.placement === "left") {
    return draft.alignRightSide ? "leftAlignLabelsJustified" : "leftAlignLabels";
  }
  return draft.alignRightSide ? "rightAlignLabelsJustified" : "rightAlignLabels";
}

export function parseFibStyle(style?: string): FibStyleDraft {
  switch (style) {
    case "leftAlignLabelsJustified":
      return { placement: "left", alignRightSide: true };
    case "leftAlignLabels":
      return { placement: "left", alignRightSide: false };
    case "rightAlignLabelsJustified":
      return { placement: "right", alignRightSide: true };
    case "rightAlignLabels":
      return { placement: "right", alignRightSide: false };
    case "freeform":
      return { placement: "freeform", alignRightSide: false };
    case "topLabels":
    default:
      return { placement: "above", alignRightSide: false };
  }
}

export function parseMcStyle(item: McItem): McStyleDraft {
  const layout =
    item.style === "horizontal"
      ? "horizontal"
      : item.style === "multicolumn"
        ? "multicolumn"
        : "vertical";
  return {
    layout,
    columnCount: typeof item.columnCount === "number" ? item.columnCount : 0,
    noPaddingBottom: item.paddingBottom === false,
  };
}

export function parseTextStyle(item: TextItem): TextStyleDraft {
  const style =
    item.style === "instructional" || item.style === "error" ? item.style : "normal";
  return {
    style,
    noPaddingBottom: item.paddingBottom === false,
  };
}

export function applyFibStyle(item: FibItem, draft: FibStyleDraft): FibItem {
  return { ...item, style: fibStyleToken(draft) };
}

export function applyMcStyle(item: McItem, draft: McStyleDraft): McItem {
  const next: McItem = { ...item, style: draft.layout };
  if (draft.layout === "multicolumn") {
    next.columnCount = draft.columnCount;
  } else {
    delete next.columnCount;
  }
  if (draft.noPaddingBottom) next.paddingBottom = false;
  else delete next.paddingBottom;
  return next;
}

export function applyTextStyle(item: TextItem, draft: TextStyleDraft): TextItem {
  const next: TextItem = { ...item, style: draft.style };
  if (draft.noPaddingBottom) next.paddingBottom = false;
  else delete next.paddingBottom;
  return next;
}

export function itemMatchesStylesKind(item: FormItem, kind: StylesDialogKind): boolean {
  if (kind === "fib") return item.type === "fib";
  if (kind === "mc") return item.type === "mc";
  return item.type === "text";
}

export function stylesKindLabel(kind: StylesDialogKind): string {
  if (kind === "fib") return "Fill in the Blank";
  if (kind === "mc") return "Multiple Choice";
  return "Text";
}

type StylesListener = (kind: StylesDialogKind | null) => void;

let pendingStyles: StylesDialogKind | null = null;
const stylesListeners = new Set<StylesListener>();

function emitStyles() {
  stylesListeners.forEach((cb) => cb(pendingStyles));
}

export function subscribeFormItemStyles(listener: StylesListener): () => void {
  stylesListeners.add(listener);
  listener(pendingStyles);
  return () => stylesListeners.delete(listener);
}

export function getFormItemStylesRequest(): StylesDialogKind | null {
  return pendingStyles;
}

export function openFormItemStylesDialog(kind: StylesDialogKind): void {
  pendingStyles = kind;
  emitStyles();
}

export function clearFormItemStylesRequest(): void {
  pendingStyles = null;
  emitStyles();
}

/** Type guard helper for store updates. */
export function isFormItemType(t: FormItemType, kind: StylesDialogKind): boolean {
  return (
    (kind === "fib" && t === "fib") ||
    (kind === "mc" && t === "mc") ||
    (kind === "text" && t === "text")
  );
}
