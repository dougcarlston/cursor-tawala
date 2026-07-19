/**
 * Observable request for Project → Email Delivery…
 * Same pattern as form-item styles / function picker hosts.
 */

type Listener = () => void;

let open = false;
const listeners = new Set<Listener>();

function emit() {
  listeners.forEach((cb) => cb());
}

export function subscribeEmailDelivery(listener: Listener): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}

export function getEmailDeliveryOpen(): boolean {
  return open;
}

export function openEmailDeliveryDialog(): void {
  open = true;
  emit();
}

export function clearEmailDeliveryDialog(): void {
  open = false;
  emit();
}
