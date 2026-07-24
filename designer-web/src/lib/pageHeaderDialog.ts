/**
 * Observable request for Project → Page Header…
 * Same pattern as Email Delivery / Styles hosts.
 */

type Listener = () => void;

let open = false;
const listeners = new Set<Listener>();

function emit() {
  listeners.forEach((cb) => cb());
}

export function subscribePageHeaderDialog(listener: Listener): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function getPageHeaderDialogOpen(): boolean {
  return open;
}

export function openPageHeaderDialog(): void {
  open = true;
  emit();
}

export function clearPageHeaderDialog(): void {
  open = false;
  emit();
}
