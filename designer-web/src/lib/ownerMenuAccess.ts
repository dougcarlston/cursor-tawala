/**
 * Owner-only Designer chrome (ops dialogs not ready for public users).
 *
 * Push credentials (`LoginDialog` / localStorage) are the current identity signal.
 * When `VITE_OWNER_MENU_USERS` is set (comma-separated), only those usernames see
 * owner menus. When unset: hide for missing/`dev` (local demo), show for any other
 * stored login so the owner’s real account works without env setup.
 */
export function canSeeOwnerOpsMenu(user: string | null | undefined): boolean {
  const trimmed = (user ?? "").trim();
  if (!trimmed) return false;

  const raw = (import.meta.env.VITE_OWNER_MENU_USERS as string | undefined) ?? "";
  const allow = raw
    .split(",")
    .map((s) => s.trim().toLowerCase())
    .filter(Boolean);

  const key = trimmed.toLowerCase();
  if (allow.length > 0) return allow.includes(key);
  return key !== "dev";
}
