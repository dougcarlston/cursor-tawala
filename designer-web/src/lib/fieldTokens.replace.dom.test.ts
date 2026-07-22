/**
 * @vitest-environment happy-dom
 *
 * Replacing a selected field chip (or caret mid plain <<Field>>) must swap the
 * whole token — never nest `<<A<<B>>…>>` (Document table / Fields drop bug).
 */
import { afterEach, describe, expect, it } from "vitest";
import {
  createFieldTokenElement,
  insertFieldTokenAtSelection,
  plainFieldRunAtRange,
} from "@/lib/fieldTokens";

afterEach(() => {
  document.body.innerHTML = "";
  window.getSelection()?.removeAllRanges();
});

function selectNode(node: Node): void {
  const sel = window.getSelection();
  if (!sel) throw new Error("no selection");
  const range = document.createRange();
  range.selectNode(node);
  sel.removeAllRanges();
  sel.addRange(range);
}

describe("insertFieldTokenAtSelection replace", () => {
  it("replaces a selectNode'd field chip instead of nesting", () => {
    const td = document.createElement("td");
    const old = createFieldTokenElement("Potluck Organizer:numAdults");
    td.appendChild(old);
    document.body.appendChild(td);
    selectNode(old);

    insertFieldTokenAtSelection("Potluck Organizer:numKids");

    const chips = td.querySelectorAll(".field-token");
    expect(chips).toHaveLength(1);
    expect(chips[0].getAttribute("data-field-name")).toBe("Potluck Organizer:numKids");
    expect(td.textContent?.replace(/\u200b/g, "")).toBe("<<Potluck Organizer:numKids>>");
    expect(td.textContent).not.toContain("numAdults");
  });

  it("replaces when caret is inside the field chip", () => {
    const p = document.createElement("p");
    const old = createFieldTokenElement("Form 1:Name");
    p.appendChild(old);
    document.body.appendChild(p);
    const sel = window.getSelection();
    const range = document.createRange();
    // Simulate browser placing the caret on the chip's text node.
    range.setStart(old.firstChild!, 2);
    range.collapse(true);
    sel!.removeAllRanges();
    sel!.addRange(range);

    insertFieldTokenAtSelection("Form 1:Email");

    expect(p.querySelectorAll(".field-token")).toHaveLength(1);
    expect(p.querySelector(".field-token")?.getAttribute("data-field-name")).toBe(
      "Form 1:Email",
    );
  });

  it("replaces a plain <<Field>> text run when caret is mid-token", () => {
    const td = document.createElement("td");
    const text = document.createTextNode("Name <<Potluck Organizer:attendeeName>> here");
    td.appendChild(text);
    document.body.appendChild(td);
    const sel = window.getSelection();
    const range = document.createRange();
    // Caret inside attendeeName
    const idx = text.textContent!.indexOf("attendee");
    range.setStart(text, idx);
    range.collapse(true);
    sel!.removeAllRanges();
    sel!.addRange(range);

    const plain = plainFieldRunAtRange(range);
    expect(plain?.name).toBe("Potluck Organizer:attendeeName");

    insertFieldTokenAtSelection("Potluck Organizer:contribution");

    expect(td.querySelectorAll(".field-token")).toHaveLength(1);
    expect(td.textContent).toBe("Name <<Potluck Organizer:contribution>> here");
    expect(td.innerHTML).not.toContain("<<Potluck Organizer:<<");
  });

  it("does not nest a field chip inside a function chip", () => {
    const p = document.createElement("p");
    const fn = document.createElement("span");
    fn.className = "function-token function-table-token";
    fn.setAttribute("contenteditable", "false");
    fn.textContent = "<<SUM(Potluck Organizer:numAdults)>>";
    p.appendChild(fn);
    document.body.appendChild(p);

    const sel = window.getSelection();
    const range = document.createRange();
    range.setStart(fn.firstChild!, 8);
    range.collapse(true);
    sel!.removeAllRanges();
    sel!.addRange(range);

    insertFieldTokenAtSelection("Potluck Organizer:numKids");

    expect(fn.querySelector(".field-token")).toBeNull();
    expect(fn.textContent).toBe("<<SUM(Potluck Organizer:numAdults)>>");
    expect(p.querySelectorAll(".field-token")).toHaveLength(1);
    expect(p.querySelector(".field-token")?.getAttribute("data-field-name")).toBe(
      "Potluck Organizer:numKids",
    );
  });
});
