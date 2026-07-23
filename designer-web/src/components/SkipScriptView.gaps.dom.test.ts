/**
 * Skip re-edit must expose indexed insert gaps (not Modify-only).
 * @vitest-environment happy-dom
 */
import { act, createElement } from "react";
import { createRoot } from "react-dom/client";
import { describe, expect, it } from "vitest";
import { SkipScriptView } from "@/components/SkipScriptView";
import { buildScriptLines } from "@/lib/skipScript";
import { buildProcessScriptLines } from "@/lib/processScript";
import type { SkipCommand } from "@/types/tawala";

const sample: SkipCommand[] = [
  {
    cmd: "if",
    condition: { op: "isBlank", field: "FIB1:a" },
    then: [
      { cmd: "set", field: "_InviteeID", value: "Anonymous" },
      { cmd: "skip", to: "FIB1" },
    ],
    else: [{ cmd: "comment", text: "We want to change _InviteeID to the new name" }],
  },
];

describe("SkipScriptView insert gaps on re-edit", () => {
  it("with showAllInsertionGaps, gaps remain clickable while a statement is selected", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);
    const lines = buildScriptLines(sample);
    const picks: Array<{ path: string; index: number }> = [];

    act(() => {
      root.render(
        createElement(SkipScriptView, {
          lines,
          insertPath: "root/0/else",
          insertIndex: 1,
          selectedCommandPath: "root/0/else/0",
          showLineControls: true,
          showAllInsertionGaps: true,
          onSelectInsertPoint: (path: string, index: number) => {
            picks.push({ path, index });
          },
          onSelectCommandPath: () => {},
        }),
      );
    });

    const gaps = host.querySelectorAll("button.skip-insertion-line");
    // Then branch (after open + after each cmd) + else + root — several gaps.
    expect(gaps.length).toBeGreaterThanOrEqual(3);

    // Click an inactive gap (edit mode) — must still fire insert-point callback.
    const inactive = Array.from(gaps).find((el) => !el.classList.contains("active"));
    expect(inactive).toBeTruthy();
    act(() => {
      inactive!.dispatchEvent(new MouseEvent("click", { bubbles: true }));
    });
    expect(picks.length).toBe(1);

    act(() => {
      root.unmount();
    });
    host.remove();
  });

  it("without showAllInsertionGaps, no indexed gaps while a statement is selected", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);
    const lines = buildScriptLines(sample);

    act(() => {
      root.render(
        createElement(SkipScriptView, {
          lines,
          insertPath: "root/0/else",
          insertIndex: 1,
          selectedCommandPath: "root/0/else/0",
          showLineControls: true,
          showAllInsertionGaps: false,
          onSelectInsertPoint: () => {},
          onSelectCommandPath: () => {},
        }),
      );
    });

    // Legacy mode hides the single insert arrow while editing — zero gap buttons.
    expect(host.querySelectorAll("button.skip-insertion-line").length).toBe(0);

    act(() => {
      root.unmount();
    });
    host.remove();
  });

  it("places sibling insert gap after If block (not between header and paren)", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);
    const lines = buildScriptLines([
      { cmd: "set", field: "Match?", value: "No" },
      {
        cmd: "if",
        condition: { op: "equals", field: "Admin:MCQ1", value: "a" },
        then: [{ cmd: "set", field: "TempName", value: "<<Admin:NewName>>" }],
      },
    ]);

    act(() => {
      root.render(
        createElement(SkipScriptView, {
          lines,
          insertPath: "root",
          insertIndex: 0,
          selectedCommandPath: "root/0",
          showLineControls: true,
          showAllInsertionGaps: true,
          insertHitTargets: true,
          onSelectInsertPoint: () => {},
          onSelectCommandPath: () => {},
        }),
      );
    });

    const afterIf = host.querySelector(
      '[data-process-insert-path="root"][data-process-insert-index="2"]',
    );
    expect(afterIf).toBeTruthy();
    expect(afterIf?.tagName).toBe("BUTTON");

    const area = host.querySelector(".skip-script-area");
    const html = area?.innerHTML ?? "";
    const afterIfPos = html.indexOf('data-process-insert-index="2"');
    const lastClosePos = html.lastIndexOf("skip-block-close");
    expect(afterIfPos).toBeGreaterThan(lastClosePos);

    act(() => {
      root.unmount();
    });
    host.remove();
  });

  it("places sibling insert gap after nested ForEach, before outer If close", () => {
    const host = document.createElement("div");
    document.body.appendChild(host);
    const root = createRoot(host);
    const lines = buildProcessScriptLines([
      {
        cmd: "if",
        condition: { op: "equals", field: "Admin:MCQ1", value: "a" },
        then: [
          {
            cmd: "foreach",
            recordName: "Record",
            recordList: "List",
            do: [{ cmd: "set", field: "Temp", value: "1" }],
          },
        ],
      },
    ]);

    act(() => {
      root.render(
        createElement(SkipScriptView, {
          lines,
          insertPath: "root",
          insertIndex: 0,
          selectedCommandPath: null,
          showLineControls: true,
          showAllInsertionGaps: true,
          insertHitTargets: true,
          onSelectInsertPoint: () => {},
          onSelectCommandPath: () => {},
        }),
      );
    });

    const afterForEach = host.querySelector(
      '[data-process-insert-path="root/0/then"][data-process-insert-index="1"]',
    );
    expect(afterForEach).toBeTruthy();
    expect(afterForEach?.tagName).toBe("BUTTON");

    const closes = host.querySelectorAll(".skip-block-close");
    expect(closes.length).toBeGreaterThanOrEqual(2);
    const foreachClose = closes[closes.length - 2];
    const ifClose = closes[closes.length - 1];
    // DOM order: ForEach ) → insert gap → If )
    expect(
      foreachClose.compareDocumentPosition(afterForEach!) &
        Node.DOCUMENT_POSITION_FOLLOWING,
    ).toBeTruthy();
    expect(
      afterForEach!.compareDocumentPosition(ifClose) & Node.DOCUMENT_POSITION_FOLLOWING,
    ).toBeTruthy();

    act(() => {
      root.unmount();
    });
    host.remove();
  });
});
