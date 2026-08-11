/**
 * Keep the focused Configure Function control visible inside `.cfg-fn-fields`
 * (MQL columns / long parameter lists grow past the dialog viewport).
 */
export function scrollConfigureFieldIntoView(el: HTMLElement): void {
  const scroller = el.closest(".cfg-fn-fields") as HTMLElement | null;
  if (!scroller) {
    el.scrollIntoView({ block: "nearest", inline: "nearest" });
    return;
  }
  const er = el.getBoundingClientRect();
  const sr = scroller.getBoundingClientRect();
  const pad = 10;
  if (er.bottom > sr.bottom - pad) {
    scroller.scrollTop += er.bottom - sr.bottom + pad;
  } else if (er.top < sr.top + pad) {
    scroller.scrollTop -= sr.top - er.top + pad;
  }
}

function escapeFocusKey(key: string): string {
  if (typeof CSS !== "undefined" && typeof CSS.escape === "function") return CSS.escape(key);
  return key.replace(/\\/g, "\\\\").replace(/"/g, '\\"');
}

/** Focus + scroll a Configure field marked with `data-cfg-focus`. */
export function focusConfigureField(focusKey: string, opts?: { focus?: boolean }): void {
  if (!focusKey || typeof document === "undefined") return;
  const root = document.querySelector(".cfg-fn-fields");
  if (!root) return;
  const el = root.querySelector(
    `[data-cfg-focus="${escapeFocusKey(focusKey)}"]`,
  ) as HTMLElement | null;
  if (!el) return;
  const shouldFocus = opts?.focus !== false;
  if (shouldFocus && document.activeElement !== el && typeof el.focus === "function") {
    try {
      el.focus({ preventScroll: true });
    } catch {
      el.focus();
    }
  }
  scrollConfigureFieldIntoView(el);
}
