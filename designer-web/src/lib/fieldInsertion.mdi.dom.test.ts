import { describe, expect, it, beforeEach, afterEach } from "vitest";
import {
  insertFieldIntoActiveTarget,
  setActiveFieldTarget,
  syncDesignerTargetsToActiveMdiWindow,
  isInsideActiveMdiWindow,
  fieldLeafAcceptedByActiveTarget,
} from "./fieldInsertion";
import { clearActivePaletteEditor, setActivePaletteEditor } from "./formattingPaletteContext";

describe("MDI-aware field insert targets", () => {
  let surface: HTMLElement;
  let winA: HTMLElement;
  let winB: HTMLElement;
  let inserted: string[];

  beforeEach(() => {
    inserted = [];
    document.body.innerHTML = "";
    winA = document.createElement("div");
    winA.className = "mdi-window active";
    winB = document.createElement("div");
    winB.className = "mdi-window";
    surface = document.createElement("div");
    surface.className = "rich-surface";
    winA.appendChild(surface);
    document.body.append(winA, winB);
    clearActivePaletteEditor();
    setActiveFieldTarget(null);
  });

  afterEach(() => {
    setActiveFieldTarget(null);
    clearActivePaletteEditor();
    document.body.innerHTML = "";
  });

  it("isInsideActiveMdiWindow follows .mdi-window.active", () => {
    expect(isInsideActiveMdiWindow(surface)).toBe(true);
    winA.classList.remove("active");
    winB.classList.add("active");
    const other = document.createElement("div");
    winB.appendChild(other);
    expect(isInsideActiveMdiWindow(surface)).toBe(false);
    expect(isInsideActiveMdiWindow(other)).toBe(true);
  });

  it("sync clears field + palette targets owned by a background window", () => {
    setActiveFieldTarget((name) => inserted.push(name), {}, surface);
    setActivePaletteEditor({
      el: surface,
      commit: () => {},
      saveSelection: () => {},
      restoreSelection: () => {},
    });
    winA.classList.remove("active");
    winB.classList.add("active");
    syncDesignerTargetsToActiveMdiWindow();
    expect(fieldLeafAcceptedByActiveTarget("Form:X")).toBe(false);
    expect(insertFieldIntoActiveTarget("Form:X")).toBe(false);
    expect(inserted).toEqual([]);
  });

  it("insertFieldIntoActiveTarget refuses a stale owner even before sync", () => {
    setActiveFieldTarget((name) => inserted.push(name), {}, surface);
    winA.classList.remove("active");
    winB.classList.add("active");
    expect(insertFieldIntoActiveTarget("Form:X")).toBe(false);
    expect(inserted).toEqual([]);
  });
});
