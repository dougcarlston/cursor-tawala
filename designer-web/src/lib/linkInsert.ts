/**
 * Insert → Invitation… / Hyperlink… — request store + Design canvas tokens.
 * Spec: DESIGNER_INSERT_MENU_AND_FUNCTIONS.md
 */

import {
  getActivePaletteEditor,
  type PaletteEditorHandle,
} from "./formattingPaletteContext";
import { useProjectStore } from "@/store/projectStore";

export const INVITATION_TOKEN_CLASS = "invitation-token";
export const HYPERLINK_TOKEN_CLASS = "hyperlink-token";

export type InvitationDraft = {
  form: string;
  project: string; // "" = current project
  displayText: string;
  isPrivate: boolean;
  authToken: string;
};

export type HyperlinkConditionRow = {
  field: string;
  op: string;
  value: string;
};

export type HyperlinkDraft = {
  url: string;
  displayText: string;
  openNewWindow: boolean;
  conditional: boolean;
  conditions: HyperlinkConditionRow[];
};

export type LinkInsertRequest =
  | { kind: "invitation"; editor: PaletteEditorHandle; editEl?: HTMLSpanElement | null }
  | { kind: "hyperlink"; editor: PaletteEditorHandle; editEl?: HTMLSpanElement | null };

let request: LinkInsertRequest | null = null;
const listeners = new Set<() => void>();

function notify(): void {
  for (const l of listeners) l();
}

export function subscribeLinkInsert(cb: () => void): () => void {
  listeners.add(cb);
  return () => listeners.delete(cb);
}

export function getLinkInsertRequest(): LinkInsertRequest | null {
  return request;
}

export function clearLinkInsertRequest(): void {
  request = null;
  notify();
}

export function openInvitationInsertFromEditor(): void {
  const editor = getActivePaletteEditor();
  if (!editor?.el?.isConnected) {
    useProjectStore
      .getState()
      .setStatus("Click inside a Form Text or Document first, then Insert → Invitation…");
    return;
  }
  editor.saveSelection();
  request = { kind: "invitation", editor };
  notify();
}

export function openHyperlinkInsertFromEditor(): void {
  const editor = getActivePaletteEditor();
  if (!editor?.el?.isConnected) {
    useProjectStore
      .getState()
      .setStatus("Click inside a Form Text or Document first, then Insert → Hyperlink…");
    return;
  }
  editor.saveSelection();
  request = { kind: "hyperlink", editor };
  notify();
}

/** Serialize draft for a DOM attribute. Pass raw JSON — the browser encodes entities in innerHTML. */
function serializeConfig(obj: object): string {
  return JSON.stringify(obj);
}

/** Decode HTML entities that may appear when reading attrs from serialized HTML. */
function decodeConfigAttr(raw: string): string {
  let out = raw;
  for (let i = 0; i < 4; i++) {
    const next = out
      .replace(/&amp;/gi, "&")
      .replace(/&quot;/gi, '"')
      .replace(/&#39;/g, "'")
      .replace(/&apos;/gi, "'")
      .replace(/&lt;/gi, "<")
      .replace(/&gt;/gi, ">");
    if (next === out) break;
    out = next;
  }
  return out;
}

export function createInvitationTokenElement(draft: InvitationDraft): HTMLSpanElement {
  const span = document.createElement("span");
  span.className = INVITATION_TOKEN_CLASS;
  span.contentEditable = "false";
  span.setAttribute("data-invitation-config", serializeConfig(draft));
  span.style.color = "#000080";
  span.style.textDecoration = "underline";
  span.textContent = draft.displayText.trim() || draft.form || "Invitation";
  return span;
}

export function createHyperlinkTokenElement(draft: HyperlinkDraft): HTMLSpanElement {
  const span = document.createElement("span");
  span.className = HYPERLINK_TOKEN_CLASS;
  span.contentEditable = "false";
  span.setAttribute("data-hyperlink-config", serializeConfig(draft));
  span.style.color = "#000080";
  span.style.textDecoration = "underline";
  const label = draft.displayText.trim() || draft.url.trim() || "(Link appears here)";
  span.textContent = label;
  return span;
}

export function parseInvitationConfig(raw: string | null): InvitationDraft | null {
  if (!raw) return null;
  try {
    const parsed = JSON.parse(decodeConfigAttr(raw)) as InvitationDraft;
    if (!parsed || typeof parsed !== "object") return null;
    return {
      form: String(parsed.form ?? ""),
      project: String(parsed.project ?? ""),
      displayText: String(parsed.displayText ?? ""),
      isPrivate: Boolean(parsed.isPrivate),
      authToken: String(parsed.authToken ?? ""),
    };
  } catch {
    return null;
  }
}

export function parseHyperlinkConfig(raw: string | null): HyperlinkDraft | null {
  if (!raw) return null;
  try {
    const parsed = JSON.parse(decodeConfigAttr(raw)) as HyperlinkDraft;
    if (!parsed || typeof parsed !== "object") return null;
    return {
      url: String(parsed.url ?? ""),
      displayText: String(parsed.displayText ?? ""),
      openNewWindow: Boolean(parsed.openNewWindow),
      conditional: Boolean(parsed.conditional),
      conditions: Array.isArray(parsed.conditions)
        ? parsed.conditions.map((r) => ({
            field: String(r?.field ?? ""),
            op: String(r?.op ?? "equals"),
            value: String(r?.value ?? ""),
          }))
        : [{ field: "", op: "equals", value: "" }],
    };
  } catch {
    return null;
  }
}

/** Insert invitation or hyperlink token at the saved palette selection. */
export function insertLinkTokenAtSelection(
  root: HTMLElement,
  kind: "invitation" | "hyperlink",
  draft: InvitationDraft | HyperlinkDraft,
): void {
  const sel = window.getSelection();
  if (!sel) return;
  const span =
    kind === "invitation"
      ? createInvitationTokenElement(draft as InvitationDraft)
      : createHyperlinkTokenElement(draft as HyperlinkDraft);

  let range: Range | null = sel.rangeCount > 0 ? sel.getRangeAt(0) : null;
  if (!range || !root.contains(range.commonAncestorContainer)) {
    range = document.createRange();
    range.selectNodeContents(root);
    range.collapse(false);
  }
  if (!range.collapsed) range.deleteContents();
  range.insertNode(span);
  const after = document.createRange();
  after.setStartAfter(span);
  after.collapse(true);
  sel.removeAllRanges();
  sel.addRange(after);
}
