/**
 * Skip re-edit must expose indexed insert gaps (not Modify-only).
 * @vitest-environment happy-dom
 */
import { act, createElement } from "react";
import { createRoot } from "react-dom/client";
import { describe, expect, it } from "vitest";
import { SkipScriptView } from "@/components/SkipScriptView";
import { buildScriptLines } from "@/lib/skipScript";
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
});
