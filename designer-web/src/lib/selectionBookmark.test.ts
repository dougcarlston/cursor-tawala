/**
 * Selection bookmarks must round-trip across wrapper rewrites so Face/Size
 * keeps the same highlight (not lost / not last-word-only / no ghost range).
 * Node-environment tests (no jsdom): exercise offset math with a tiny fake DOM.
 *
 * Critical: ZWSP caret pads around field chips must not shift the logical end
 * inward on each simulated rewrite (owner: ~2 chars per chip per Size).
 */
import { describe, expect, it } from "vitest";
import {
  findTextOffsetsNear,
  positionFromTextOffset,
  textBetweenOffsets,
  textOffsetAt,
} from "@/lib/selectionBookmark";
import { CARET_ZWSP } from "@/lib/tokenCaretLanding";

const TEXT = 3;
const ELEMENT = 1;

function textNode(data: string): Node {
  return {
    nodeType: TEXT,
    textContent: data,
    childNodes: [] as unknown as NodeListOf<ChildNode>,
  } as unknown as Node;
}

function elem(children: Node[]): Node {
  const node = {
    nodeType: ELEMENT,
    childNodes: children as unknown as NodeListOf<ChildNode>,
    get textContent() {
      return children.map((c) => c.textContent ?? "").join("");
    },
  };
  return node as unknown as Node;
}

describe("selectionBookmark text offsets", () => {
  it("bookmarks a mid-word range and restores after a font wrapper rewrite", () => {
    const plain = textNode("one two three");
    const root = elem([plain]);
    // "two" is offsets 4..7
    expect(textOffsetAt(root, plain, 4)).toBe(4);
    expect(textOffsetAt(root, plain, 7)).toBe(7);
    expect(textBetweenOffsets(root, 4, 7)).toBe("two");

    // Simulate fontSize rewrite: wrap the selected word.
    const before = textNode("one ");
    const mid = textNode("two");
    const after = textNode(" three");
    const font = elem([mid]);
    const rewritten = elem([before, font, after]);

    expect(textBetweenOffsets(rewritten, 4, 7)).toBe("two");

    const startPos = positionFromTextOffset(rewritten, 4);
    const endPos = positionFromTextOffset(rewritten, 7, true);
    expect(startPos).not.toBeNull();
    expect(endPos).not.toBeNull();
    // Boundary offsets may land at end of previous / start of next text node.
    expect(startPos!.node === mid || startPos!.node === before).toBe(true);
    expect(endPos!.node === mid || endPos!.node === after).toBe(true);
  });

  it("bookmarks a multi-word drag and restores the full span after wrap", () => {
    const plain = textNode("alpha beta gamma");
    const root = elem([plain]);
    // "alpha beta" = 0..10
    expect(textBetweenOffsets(root, 0, 10)).toBe("alpha beta");

    // Split into per-word wrappers (Chrome fontSize shape).
    const w1 = textNode("alpha");
    const sp1 = textNode(" ");
    const w2 = textNode("beta");
    const sp2 = textNode(" ");
    const w3 = textNode("gamma");
    const rewritten = elem([elem([w1]), sp1, elem([w2]), sp2, elem([w3])]);

    expect(textBetweenOffsets(rewritten, 0, 10)).toBe("alpha beta");
    // Must not shrink to only the last word of the original highlight.
    expect(textBetweenOffsets(rewritten, 0, 10)).not.toBe("beta");
    expect(textBetweenOffsets(rewritten, 0, 10)).not.toBe("gamma");

    const endPos = positionFromTextOffset(rewritten, 10, true);
    expect(endPos).not.toBeNull();
    // preferEnd keeps the end on "beta" rather than the following space.
    expect(endPos!.node === w2).toBe(true);
    expect(endPos!.offset).toBe(4);
  });

  it("counts offsets inside element containers via child index", () => {
    const a = textNode("aa");
    const b = textNode("bb");
    const root = elem([a, b]);
    expect(textOffsetAt(root, root, 1)).toBe(2);
    expect(textOffsetAt(root, b, 1)).toBe(3);
  });

  it("ignores ZWSP caret pads so field landings do not shift logical offsets", () => {
    // "Hi " + ZWSP + "<<F>>" + ZWSP + " end"
    const before = textNode("Hi ");
    const pad1 = textNode(CARET_ZWSP);
    const chip = textNode("<<F>>");
    const pad2 = textNode(CARET_ZWSP);
    const after = textNode(" end");
    const root = elem([before, pad1, chip, pad2, after]);

    // Meaningful: "Hi <<F>> end" — len 12. Offset 0 of " end" is the leading space (=8).
    expect(textBetweenOffsets(root, 0, 12)).toBe("Hi <<F>> end");
    expect(textOffsetAt(root, after, 0)).toBe(8);
    expect(textOffsetAt(root, after, 1)).toBe(9); // 'e'
    expect(textOffsetAt(root, after, 4)).toBe(12);

    // Extra stacked pads after a Size wrap must not change logical end.
    const pad3 = textNode(CARET_ZWSP);
    const pad4 = textNode(CARET_ZWSP);
    const rewritten = elem([before, pad1, pad3, chip, pad2, pad4, after]);
    expect(textBetweenOffsets(rewritten, 0, 12)).toBe("Hi <<F>> end");
    expect(textOffsetAt(rewritten, after, 4)).toBe(12);
  });

  it("keeps paragraph end stable across repeated wrap + extra ZWSP rewrites", () => {
    // Mirrors owner field paragraph: three chips × 2 pads; each Size must not
    // drop ~6 trailing glyphs ("ation.").
    const make = (extraPadsPerChip: number) => {
      const parts: Node[] = [textNode("Here is one with fields. ")];
      for (const name of ["a", "c", "b"]) {
        for (let i = 0; i < 1 + extraPadsPerChip; i++) parts.push(textNode(CARET_ZWSP));
        parts.push(textNode(`<<FIB1:${name}>>`));
        for (let i = 0; i < 1 + extraPadsPerChip; i++) parts.push(textNode(CARET_ZWSP));
        parts.push(textNode(" "));
      }
      parts.push(textNode("and more text for modification."));
      return elem(parts);
    };

    const full = "Here is one with fields. <<FIB1:a>> <<FIB1:c>> <<FIB1:b>> and more text for modification.";
    const end = full.length;

    let bookmarkEnd = end;
    let bookmarkText = full;
    for (let pass = 0; pass < 4; pass++) {
      const root = make(pass); // each pass stacks more pads (old bug shape)
      const got = textBetweenOffsets(root, 0, bookmarkEnd);
      expect(got).toBe(bookmarkText);
      expect(got.endsWith("modification.")).toBe(true);
      // Snapshot repair still finds full text if someone passed a shrunk end.
      const found = findTextOffsetsNear(root, bookmarkText, 0);
      expect(found).not.toBeNull();
      expect(found!.end - found!.start).toBe(bookmarkText.length);
      bookmarkEnd = found!.end;
    }
  });

  it("findTextOffsetsNear repairs a shrunk end after stacked ZWSP drift", () => {
    const before = textNode("modific");
    const pads = [textNode(CARET_ZWSP), textNode(CARET_ZWSP), textNode(CARET_ZWSP)];
    const after = textNode("ation.");
    const root = elem([before, ...pads, after]);
    const needle = "modification.";
    // Wrong end that stops at "modific" (7) — as if ZWSPs ate the restore.
    expect(textBetweenOffsets(root, 0, 7)).toBe("modific");
    const found = findTextOffsetsNear(root, needle, 0);
    expect(found).toEqual({ start: 0, end: needle.length });
    expect(textBetweenOffsets(root, found!.start, found!.end)).toBe(needle);
  });
});
