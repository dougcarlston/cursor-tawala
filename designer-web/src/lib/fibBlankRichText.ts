import type { TawalaBlank } from "../types/tawala";

/** True when a pushed multi-line blank should get the Java TinyMCE mini-formatter. */
export function blankLiveRichTextEnabled(blank: Pick<TawalaBlank, "height" | "richText">): boolean {
  if ((blank.height ?? 1) <= 1) return false;
  return blank.richText === true;
}

/** Default for `richText` when the author first raises Height above 1. */
export const FIB_NEW_MULTILINE_RICH_TEXT_DEFAULT = false;
