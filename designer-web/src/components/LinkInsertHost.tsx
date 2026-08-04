import { useSyncExternalStore } from "react";
import { InsertInvitationDialog } from "./InsertInvitationDialog";
import { InsertHyperlinkDialog } from "./InsertHyperlinkDialog";
import {
  clearLinkInsertRequest,
  getLinkInsertRequest,
  insertLinkTokenAtSelection,
  parseHyperlinkConfig,
  parseInvitationConfig,
  replaceLinkToken,
  subscribeLinkInsert,
  type HyperlinkDraft,
  type InvitationDraft,
} from "@/lib/linkInsert";
import { getActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { useProjectStore } from "@/store/projectStore";

/** Hosts Insert Invitation / Hyperlink dialogs and commits tokens into the palette editor. */
export function LinkInsertHost() {
  const request = useSyncExternalStore(subscribeLinkInsert, getLinkInsertRequest, () => null);
  if (!request) return null;

  const close = () => clearLinkInsertRequest();

  const commit = (kind: "invitation" | "hyperlink", draft: InvitationDraft | HyperlinkDraft) => {
    const handle = request.editor ?? getActivePaletteEditor();
    if (!handle?.el?.isConnected) {
      useProjectStore
        .getState()
        .setStatus("Could not insert — click inside Form Text or Document and try again");
      close();
      return;
    }
    handle.el.focus();
    const editing = Boolean(request.editEl?.isConnected);
    if (editing) {
      // Prefer in-place replace so double-click edit does not stack chips.
      replaceLinkToken(handle.el, request.editEl, kind, draft);
    } else {
      handle.restoreSelection();
      insertLinkTokenAtSelection(handle.el, kind, draft);
    }
    handle.commit();
    useProjectStore
      .getState()
      .setStatus(
        editing
          ? kind === "invitation"
            ? "Invitation updated"
            : "Hyperlink updated"
          : kind === "invitation"
            ? "Invitation inserted"
            : "Hyperlink inserted",
      );
    close();
  };

  if (request.kind === "invitation") {
    const initial = request.editEl
      ? parseInvitationConfig(request.editEl.getAttribute("data-invitation-config")) ?? undefined
      : undefined;
    return (
      <InsertInvitationDialog
        initial={initial ?? undefined}
        onCancel={close}
        onSave={(draft) => commit("invitation", draft)}
      />
    );
  }

  const initial = request.editEl
    ? parseHyperlinkConfig(request.editEl.getAttribute("data-hyperlink-config")) ?? undefined
    : undefined;
  return (
    <InsertHyperlinkDialog
      initial={initial ?? undefined}
      onCancel={close}
      onSave={(draft) => commit("hyperlink", draft)}
    />
  );
}
