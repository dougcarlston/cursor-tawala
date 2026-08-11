import { useSyncExternalStore } from "react";
import { InsertLinkDialog } from "./InsertLinkDialog";
import {
  clearLinkInsertRequest,
  getLinkInsertRequest,
  insertLinkTokenAtSelection,
  parseHyperlinkConfig,
  parseInvitationConfig,
  replaceLinkToken,
  subscribeLinkInsert,
} from "@/lib/linkInsert";
import { getActivePaletteEditor } from "@/lib/formattingPaletteContext";
import { useProjectStore } from "@/store/projectStore";

/** Hosts unified Insert → Link… and commits invitation / hyperlink tokens. */
export function LinkInsertHost() {
  const request = useSyncExternalStore(subscribeLinkInsert, getLinkInsertRequest, () => null);
  if (!request) return null;

  const close = () => clearLinkInsertRequest();
  const editing = Boolean(request.editEl?.isConnected);
  const mode = request.kind === "invitation" ? ("form" as const) : ("url" as const);

  const invitationInitial =
    request.kind === "invitation" && editing
      ? parseInvitationConfig(request.editEl!.getAttribute("data-invitation-config")) ?? undefined
      : undefined;
  const hyperlinkInitial =
    request.kind === "hyperlink" && editing
      ? parseHyperlinkConfig(request.editEl!.getAttribute("data-hyperlink-config")) ?? undefined
      : undefined;

  return (
    <InsertLinkDialog
      initialMode={mode}
      lockMode={editing}
      invitationInitial={invitationInitial}
      hyperlinkInitial={hyperlinkInitial}
      onCancel={close}
      onSave={(result) => {
        const handle = request.editor ?? getActivePaletteEditor();
        if (!handle?.el?.isConnected) {
          useProjectStore
            .getState()
            .setStatus("Could not insert — click inside Form Text or Document and try again");
          close();
          return;
        }
        handle.el.focus();
        if (editing) {
          replaceLinkToken(handle.el, request.editEl, result.kind, result.draft);
        } else {
          handle.restoreSelection();
          insertLinkTokenAtSelection(handle.el, result.kind, result.draft);
        }
        handle.commit();
        useProjectStore.getState().setStatus(
          editing
            ? result.kind === "invitation"
              ? "Form link updated"
              : "Web link updated"
            : result.kind === "invitation"
              ? "Form link inserted"
              : "Web link inserted",
        );
        close();
      }}
    />
  );
}
