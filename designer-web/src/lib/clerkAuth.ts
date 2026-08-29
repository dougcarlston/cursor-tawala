export interface AuthUser {
  id: string;
  fullName?: string | null;
  primaryEmail?: string | null;
  username?: string | null;
}

let currentAuthUser: AuthUser | null = null;
const listeners = new Set<(user: AuthUser | null) => void>();

export function setGlobalAuthUser(user: AuthUser | null): void {
  currentAuthUser = user;
  listeners.forEach((l) => l(currentAuthUser));
}

export function getGlobalAuthUser(): AuthUser | null {
  return currentAuthUser;
}

export function subscribeGlobalAuthUser(listener: (user: AuthUser | null) => void): () => void {
  listeners.add(listener);
  return () => listeners.delete(listener);
}
