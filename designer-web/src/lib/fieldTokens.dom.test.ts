/**
 * @vitest-environment happy-dom
 *
 * Field chips: create, upgrade plain <<…>>, replace selected chip (never nest).
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  createFieldTokenElement,
  embedPlainFieldTokensAsHtml,
  FIELD_NAME_ATTR,
  FIELD_TOKEN_CLASS,
  insertFieldTokenAtSelection,
  normalizeFieldTokenSpans,
  plainFieldRunAtRange,
  readFieldNameFromToken,
} from "./fieldTokens";
import { FUNCTION_TOKEN_CLASS } from "./functionTokens";

afterEach(() => {
  document.body.innerHTML = "";
  window.getSelection()?.removeAllRanges();
});

describe("createFieldTokenElement", () => {
  it("builds a non-editable <<name>> chip", () => {
    const span = createFieldTokenElement("Form 1:Email");
    expect(span.classList.contains(FIELD_TOKEN_CLASS)).toBe(true);
    expect(span.getAttribute("contenteditable")).toBe("false");
    expect(span.getAttribute(FIELD_NAME_ATTR)).toBe("Form 1:Email");
    expect(span.textContent).toBe("<<Form 1:Email>>");
    expect(span.draggable).toBe(true);
  });
});

describe("plainFieldRunAtRange", () => {
  it("finds a plain <<Field>> run containing the caret", () => {
    const p = document.createElement("p");
    p.textContent = "Hello <<Email>> there";
    document.body.appendChild(p);
    const text = p.firstChild as Text;
    const range = document.createRange();
    // Caret inside "<<Email>>"
    range.setStart(text, 10);
    range.collapse(true);
    const hit = plainFieldRunAtRange(range);
    expect(hit).not.toBeNull();
    expect(hit!.name).toBe("Email");
    expect(text.textContent!.slice(hit!.start, hit!.end)).toBe("<<Email>>");
  });

  it("ignores caret outside any <<…>> run", () => {
    const p = document.createElement("p");
    p.textContent = "Hello <<Email>> there";
    document.body.appendChild(p);
    const text = p.firstChild as Text;
    const range = document.createRange();
    range.setStart(text, 1);
    range.collapse(true);
    expect(plainFieldRunAtRange(range)).toBeNull();
  });
});

describe("normalizeFieldTokenSpans / embedPlainFieldTokensAsHtml", () => {
  it("upgrades plain <<field>> text to chips (idempotent)", () => {
    const editor = document.createElement("div");
    // textContent (not innerHTML): browsers parse <<Name>> as tags otherwise.
    editor.textContent = "Hi <<Name>> and <<Form 1:Email>>";
    document.body.appendChild(editor);
    normalizeFieldTokenSpans(editor);
    const chips = editor.querySelectorAll(`.${FIELD_TOKEN_CLASS}`);
    expect(chips).toHaveLength(2);
    expect(readFieldNameFromToken(chips[0] as HTMLElement)).toBe("Name");
    expect(readFieldNameFromToken(chips[1] as HTMLElement)).toBe("Form 1:Email");
    normalizeFieldTokenSpans(editor);
    expect(editor.querySelectorAll(`.${FIELD_TOKEN_CLASS}`)).toHaveLength(2);
  });

  it("embeds plain tokens in HTML without treating <Name> as a tag", () => {
    const html = embedPlainFieldTokensAsHtml("Your <<ContactType1>>:");
    expect(html).toContain(FIELD_TOKEN_CLASS);
    expect(html).toContain("data-field-name");
    expect(html).toContain("ContactType1");
    expect(html).not.toContain("<<ContactType1>>");
  });
});

describe("insertFieldTokenAtSelection", () => {
  it("replaces a selected field chip instead of nesting", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const old = createFieldTokenElement("Old");
    editor.appendChild(old);
    document.body.appendChild(editor);

    const sel = window.getSelection();
    expect(sel).toBeTruthy();
    const range = document.createRange();
    range.selectNode(old);
    sel!.removeAllRanges();
    sel!.addRange(range);

    insertFieldTokenAtSelection("New");
    const chips = editor.querySelectorAll(`.${FIELD_TOKEN_CLASS}`);
    expect(chips).toHaveLength(1);
    expect(readFieldNameFromToken(chips[0] as HTMLElement)).toBe("New");
    expect(editor.textContent).not.toMatch(/<<Old/);
    expect(editor.innerHTML).not.toMatch(/<<.*<</);
  });

  it("replaces a plain <<Field>> run under the caret", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    editor.appendChild(document.createTextNode("See <<Old>> end"));
    document.body.appendChild(editor);

    const text = editor.firstChild as Text;
    const range = document.createRange();
    range.setStart(text, 6); // inside <<Old>>
    range.collapse(true);
    const sel = window.getSelection()!;
    sel.removeAllRanges();
    sel.addRange(range);

    insertFieldTokenAtSelection("New");
    const chips = editor.querySelectorAll(`.${FIELD_TOKEN_CLASS}`);
    expect(chips).toHaveLength(1);
    expect(readFieldNameFromToken(chips[0] as HTMLElement)).toBe("New");
    expect(editor.textContent).toContain("See");
    expect(editor.textContent).toContain("end");
    expect(editor.textContent).not.toContain("<<Old>>");
  });

  it("does not nest a field chip inside a selected function chip", () => {
    const editor = document.createElement("div");
    editor.contentEditable = "true";
    const fn = document.createElement("span");
    fn.className = FUNCTION_TOKEN_CLASS;
    fn.setAttribute("contenteditable", "false");
    fn.textContent = "<<SUM(Amount)>>";
    editor.appendChild(fn);
    document.body.appendChild(editor);

    const sel = window.getSelection()!;
    const range = document.createRange();
    range.selectNode(fn);
    sel.removeAllRanges();
    sel.addRange(range);

    // Caret/selection on function chip: insert places after (or no nest).
    // Selecting the function token itself uses functionTokenFromSelection path
    // only when range intersects function — insertFieldTokenAtSelection then
    // collapses after the chip before inserting.
    insertFieldTokenAtSelection("Email");
    expect(fn.querySelector(`.${FIELD_TOKEN_CLASS}`)).toBeNull();
    expect(fn.textContent).toBe("<<SUM(Amount)>>");
    const field = editor.querySelector(`.${FIELD_TOKEN_CLASS}`);
    expect(field).toBeTruthy();
    expect(fn.contains(field!)).toBe(false);
  });
});
