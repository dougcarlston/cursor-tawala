/**
 * @vitest-environment happy-dom
 *
 * Invitation / Hyperlink double-click open (design canvas edit).
 * Converted Document chips carry data-invitation-config; open must surface that form.
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  clearLinkInsertRequest,
  getLinkInsertRequest,
  INVITATION_TOKEN_CLASS,
  openLinkTokenForEdit,
  parseInvitationConfig,
  replaceLinkToken,
  selectLinkToken,
  type InvitationDraft,
} from "./linkInsert";

const CYO_CONFIG =
  '{"form":"ViewAll","project":"","displayText":"View Full Report","isPrivate":false,"authToken":""}';

describe("openLinkTokenForEdit", () => {
  afterEach(() => {
    clearLinkInsertRequest();
    document.body.innerHTML = "";
  });

  it("opens invitation edit with converted CYO Dashboard-style config", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const token = document.createElement("span");
    token.className = INVITATION_TOKEN_CLASS;
    token.contentEditable = "false";
    // Convert path uses escHtml on the attr; after DOM parse getAttribute is raw JSON.
    token.setAttribute("data-invitation-config", CYO_CONFIG);
    token.textContent = "View Full Report";
    editor.append(token);
    document.body.append(editor);

    expect(openLinkTokenForEdit(token, editor)).toBe(true);
    const req = getLinkInsertRequest();
    expect(req?.kind).toBe("invitation");
    expect(req?.editEl).toBe(token);
    expect(parseInvitationConfig(token.getAttribute("data-invitation-config"))).toEqual({
      form: "ViewAll",
      project: "",
      displayText: "View Full Report",
      isPrivate: false,
      authToken: "",
    });
  });

  it("selectLinkToken highlights without opening", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const token = document.createElement("span");
    token.className = INVITATION_TOKEN_CLASS;
    token.setAttribute("data-invitation-config", CYO_CONFIG);
    token.textContent = "View Full Report";
    editor.append(token);
    document.body.append(editor);

    expect(selectLinkToken(token, editor)).toBe(true);
    expect(getLinkInsertRequest()).toBeNull();
  });

  it("replaceLinkToken updates chip in place (no duplicate)", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const token = document.createElement("span");
    token.className = INVITATION_TOKEN_CLASS;
    token.setAttribute("data-invitation-config", CYO_CONFIG);
    token.textContent = "View Full Report";
    editor.append(token);
    document.body.append(editor);

    const draft: InvitationDraft = {
      form: "ViewMailing",
      project: "",
      displayText: "View Mailing Data",
      isPrivate: false,
      authToken: "",
    };
    replaceLinkToken(editor, token, "invitation", draft);
    expect(editor.querySelectorAll(`.${INVITATION_TOKEN_CLASS}`).length).toBe(1);
    const next = editor.querySelector(`.${INVITATION_TOKEN_CLASS}`);
    expect(next?.textContent).toBe("View Mailing Data");
    expect(parseInvitationConfig(next?.getAttribute("data-invitation-config") ?? null)?.form).toBe(
      "ViewMailing",
    );
  });

  it("returns false for non-link elements", () => {
    const editor = document.createElement("div");
    const plain = document.createElement("span");
    plain.textContent = "hello";
    editor.append(plain);
    document.body.append(editor);
    expect(openLinkTokenForEdit(plain, editor)).toBe(false);
  });
});
