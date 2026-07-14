/** CSS selectors for form-item reorder grab chrome (item badge / badge rename field). */
export const FORM_ITEM_REORDER_HANDLE_SELECTOR = [
  ".fib-badge",
  ".fib-badge-input",
  ".text-badge",
  ".text-badge-input",
  ".heading-badge",
  ".heading-badge-input",
  ".mcq-badge",
  ".mcq-badge-input",
  ".hidden-field-badge",
  ".skip-badge",
  ".break-badge",
].join(", ");

/** Resolve event target to an Element (contenteditable often yields Text nodes). */
export function dragEventElement(target: EventTarget | null): Element | null {
  if (target instanceof Element) return target;
  if (target instanceof Node) return target.parentElement;
  return null;
}

/** True when the drag started on badge chrome (not the item body). */
export function isFormItemReorderHandle(target: EventTarget | null): boolean {
  const el = dragEventElement(target);
  return !!el?.closest(FORM_ITEM_REORDER_HANDLE_SELECTOR);
}
