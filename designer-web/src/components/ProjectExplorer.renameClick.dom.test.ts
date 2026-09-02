/**
 * Explorer rename vs open policy (owner Sep 2026).
 * @vitest-environment happy-dom
 */
import { describe, expect, it, beforeEach, afterEach } from "vitest";
import { act, createElement } from "react";
import { createRoot } from "react-dom/client";
import { ProjectExplorer } from "@/components/ProjectExplorer";
import { useProjectStore } from "@/store/projectStore";

describe("ProjectExplorer click opens; double-click renames", () => {
  let host: HTMLDivElement;
  let root: ReturnType<typeof createRoot>;

  beforeEach(() => {
    host = document.createElement("div");
    document.body.appendChild(host);
    root = createRoot(host);
    useProjectStore.setState({
      project: {
        name: "Explorer Click",
        forms: [{ name: "Administration", items: [] }],
        processes: [{ name: "Post-Administration", commands: [] }],
        documents: [{ name: "Results", content: "" }],
      },
      selection: { kind: "forms" },
      openWindows: [],
      activeWindowId: null,
      dirty: false,
      statusMessage: "",
    });
  });

  afterEach(() => {
    act(() => root.unmount());
    host.remove();
  });

  it("single-click on an already-selected Form opens the window instead of renaming", () => {
    act(() => {
      root.render(createElement(ProjectExplorer));
    });
    // First click selects + opens.
    const formLabel = Array.from(host.querySelectorAll(".tree-label")).find(
      (el) => el.textContent === "Administration",
    );
    expect(formLabel).toBeTruthy();
    const row = formLabel!.closest(".tree-node") as HTMLElement;
    act(() => {
      row.click();
    });
    expect(useProjectStore.getState().openWindows.some((w) => w.name === "Administration")).toBe(
      true,
    );
    expect(host.querySelector(".tree-rename-input")).toBeNull();

    // Second single-click on the same selected row must open/focus — not rename.
    act(() => {
      row.click();
    });
    expect(host.querySelector(".tree-rename-input")).toBeNull();
    expect(useProjectStore.getState().activeWindowId).toContain("Administration");
  });

  it("double-click on a Form name enters inline rename", () => {
    act(() => {
      root.render(createElement(ProjectExplorer));
    });
    const formLabel = Array.from(host.querySelectorAll(".tree-label")).find(
      (el) => el.textContent === "Administration",
    );
    const row = formLabel!.closest(".tree-node") as HTMLElement;
    act(() => {
      row.dispatchEvent(new MouseEvent("dblclick", { bubbles: true }));
    });
    const input = host.querySelector(".tree-rename-input") as HTMLInputElement | null;
    expect(input).toBeTruthy();
    expect(input!.value).toBe("Administration");
  });
});
