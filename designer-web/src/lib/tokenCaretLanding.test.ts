import { describe, expect, it } from "vitest";
import {
  CARET_ZWSP,
  caretOffsetAfterTokenLanding,
  ensureTokenCaretLanding,
} from "./tokenCaretLanding";

describe("caretOffsetAfterTokenLanding", () => {
  it("keeps mid-sentence glue text from being skipped (comma stays after caret)", () => {
    expect(caretOffsetAfterTokenLanding(", who rallied the team")).toBe(0);
    expect(caretOffsetAfterTokenLanding(" who continued")).toBe(0);
  });

  it("lands after a pure ZWSP caret pad so typing stays after the chip", () => {
    expect(caretOffsetAfterTokenLanding(CARET_ZWSP)).toBe(1);
  });

  it("does not treat ZWSP-prefixed content as a pure pad", () => {
    expect(caretOffsetAfterTokenLanding(`${CARET_ZWSP}, who`)).toBe(0);
  });
});

/**
 * Minimal linked-list DOM so ensureTokenCaretLanding can run under plain vitest
 * (no jsdom) — Face/Size wraps absorb pads; ensure must extract, not stack.
 */
describe("ensureTokenCaretLanding (no stack after Face/Size wrap)", () => {
  type FakeNode = {
    nodeType: number;
    nodeName: string;
    data?: string;
    parentNode: FakeNode | null;
    previousSibling: FakeNode | null;
    nextSibling: FakeNode | null;
    firstChild: FakeNode | null;
    lastChild: FakeNode | null;
    childNodes: FakeNode[];
    classList: { contains: (c: string) => boolean };
    className: string;
  };

  const TEXT = 3;
  const ELEMENT = 1;

  function linkChildren(parent: FakeNode, kids: FakeNode[]) {
    parent.childNodes = kids;
    parent.firstChild = kids[0] ?? null;
    parent.lastChild = kids[kids.length - 1] ?? null;
    for (let i = 0; i < kids.length; i++) {
      kids[i]!.parentNode = parent;
      kids[i]!.previousSibling = kids[i - 1] ?? null;
      kids[i]!.nextSibling = kids[i + 1] ?? null;
    }
  }

  function text(data: string): FakeNode {
    return {
      nodeType: TEXT,
      nodeName: "#text",
      data,
      parentNode: null,
      previousSibling: null,
      nextSibling: null,
      firstChild: null,
      lastChild: null,
      childNodes: [],
      classList: { contains: () => false },
      className: "",
    };
  }

  function el(tag: string, className = ""): FakeNode {
    const classes = new Set(className.split(/\s+/).filter(Boolean));
    return {
      nodeType: ELEMENT,
      nodeName: tag.toUpperCase(),
      parentNode: null,
      previousSibling: null,
      nextSibling: null,
      firstChild: null,
      lastChild: null,
      childNodes: [],
      classList: { contains: (c: string) => classes.has(c) },
      className,
    };
  }

  function installParentApi(parent: FakeNode) {
    (parent as unknown as { insertBefore: (n: FakeNode, ref: FakeNode | null) => void }).insertBefore =
      (node, ref) => {
        // Detach from old parent first.
        if (node.parentNode) {
          const op = node.parentNode;
          const kids = op.childNodes.filter((c) => c !== node);
          linkChildren(op, kids);
        }
        const kids = [...parent.childNodes];
        const idx = ref ? kids.indexOf(ref) : kids.length;
        kids.splice(idx < 0 ? kids.length : idx, 0, node);
        linkChildren(parent, kids);
      };
    (parent as unknown as { removeChild: (n: FakeNode) => void }).removeChild = (node) => {
      linkChildren(
        parent,
        parent.childNodes.filter((c) => c !== node),
      );
    };
  }

  it("pulls absorbed ZWSPs out of face/size wrappers instead of stacking new ones", () => {
    const parent = el("p");
    installParentApi(parent);

    const beforeWrap = el("span");
    const afterWrap = el("span");
    const chip = el("span", "field-token");

    linkChildren(beforeWrap, [text("before"), text(CARET_ZWSP)]);
    linkChildren(afterWrap, [text(CARET_ZWSP), text("after")]);
    linkChildren(parent, [beforeWrap, chip, afterWrap]);

    const chipEl = chip as unknown as HTMLElement;
    ensureTokenCaretLanding(chipEl);
    ensureTokenCaretLanding(chipEl);
    ensureTokenCaretLanding(chipEl);

    expect(chip.previousSibling?.nodeType).toBe(TEXT);
    expect(chip.previousSibling?.data).toBe(CARET_ZWSP);
    expect(chip.nextSibling?.nodeType).toBe(TEXT);
    expect(chip.nextSibling?.data).toBe(CARET_ZWSP);

    let prevZwsp = 0;
    let n: FakeNode | null = chip.previousSibling;
    while (n && n.nodeType === TEXT && n.data === CARET_ZWSP) {
      prevZwsp += 1;
      n = n.previousSibling;
    }
    let nextZwsp = 0;
    n = chip.nextSibling;
    while (n && n.nodeType === TEXT && n.data === CARET_ZWSP) {
      nextZwsp += 1;
      n = n.nextSibling;
    }
    expect(prevZwsp).toBe(1);
    expect(nextZwsp).toBe(1);
  });
});
