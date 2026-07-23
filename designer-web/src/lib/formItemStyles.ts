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
  // Re-applying Instructional/Error: drop default-black chrome on image wrappers so
  // Style color/weight show on the whole paragraph (Potluck T1/T3 graphic lines).
  if (
    (draft.style === "instructional" || draft.style === "error") &&
    typeof next.content === "string" &&
    next.content
  ) {
    next.content = stripDefaultBlackAroundEmbeddedImages(next.content);
  }
  return next;
}

/**
 * Embedded images often sit in `<span style="font-size:10pt; color:#000000">`.
 * That default black blocks Instructional/Error navy on Design and leaves the
 * graphic line looking “not fully switched” when Style is re-applied.
 */
export function stripDefaultBlackAroundEmbeddedImages(html: string): string {
  return String(html).replace(/<span\b([^>]*)>/gi, (full, attrs: string) => {
    if (!/\bstyle="/i.test(attrs)) return full;
    // Only touch spans that wrap (or immediately precede) an embedded image in practice
    // — we strip default black from any span whose style is only size/face + default black.
    const styleM = attrs.match(/\bstyle="([^"]*)"/i);
    if (!styleM) return full;
    const style = styleM[1];
    const colorM = style.match(/(?:^|;)\s*color\s*:\s*([^;]+)/i);
    if (!colorM) return full;
    const raw = colorM[1].trim().toLowerCase().replace(/\s+/g, "");
    const isDefaultBlack =
      raw === "#000" ||
      raw === "#000000" ||
      raw === "black" ||
      raw === "rgb(0,0,0)" ||
      raw === "rgba(0,0,0,1)";
    if (!isDefaultBlack) return full;
    const nextStyle = style
      .replace(/(?:^|;)\s*color\s*:\s*[^;]+/i, "")
      .replace(/^;\s*|;\s*$/g, "")
      .replace(/;;+/g, ";")
      .trim();
    if (!nextStyle) {
      return full.replace(/\s*style="[^"]*"/i, "");
    }
    return full.replace(/\bstyle="[^"]*"/i, `style="${nextStyle}"`);
  });
}

/** How many form items of this Styles dialog kind exist on the form. */
export function countFormItemsOfKind(items: readonly FormItem[], kind: StylesDialogKind): number {
  return items.filter((item) => itemMatchesStylesKind(item, kind)).length;
}

/**
 * Apply a Styles draft to every matching item on one form (browser Apply to All —
 * active form only, not project-wide like the disabled legacy button).
 * Returns `{ items, changed }` where `changed` is the number of items patched.
 */
export function applyStyleToAllFormItems(
  items: readonly FormItem[],
  kind: StylesDialogKind,
  draft: FibStyleDraft | McStyleDraft | TextStyleDraft,
): { items: FormItem[]; changed: number } {
  let changed = 0;
  const next = items.map((item) => {
    if (!itemMatchesStylesKind(item, kind)) return item;
    changed += 1;
    if (kind === "fib" && item.type === "fib") {
      return applyFibStyle(item, draft as FibStyleDraft);
    }
    if (kind === "mc" && item.type === "mc") {
      return applyMcStyle(item, draft as McStyleDraft);
    }
    if (kind === "text" && item.type === "text") {
      return applyTextStyle(item, draft as TextStyleDraft);
    }
    return item;
  });
  return { items: next, changed };
}

export function itemMatchesStylesKind(item: FormItem, kind: StylesDialogKind): boolean {
  if (kind === "fib") return item.type === "fib";
  if (kind === "mc") return item.type === "mc";
  return item.type === "text";
}

/** Styles dialog kind for a selected form item, or null if not styleable
 * (Heading, Hidden Field, Page Break, Skip Instructions, etc.). */
export function stylesKindForFormItem(item: FormItem | null | undefined): StylesDialogKind | null {
  if (!item) return null;
  if (item.type === "fib") return "fib";
  if (item.type === "mc") return "mc";
  if (item.type === "text") return "text";
  return null;
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
