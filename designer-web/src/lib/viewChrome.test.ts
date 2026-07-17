import { describe, expect, it, beforeEach } from "vitest";
import {
  DEFAULT_VIEW_CHROME,
  getViewChrome,
  resetViewChromeForTests,
  toggleViewChrome,
} from "./viewChrome";

describe("viewChrome", () => {
  beforeEach(() => {
    resetViewChromeForTests({ ...DEFAULT_VIEW_CHROME });
  });

  it("defaults all chrome visible", () => {
    expect(getViewChrome()).toEqual(DEFAULT_VIEW_CHROME);
  });

  it("toggles individual panels", () => {
    toggleViewChrome("toolbar");
    expect(getViewChrome().toolbar).toBe(false);
    toggleViewChrome("toolbar");
    expect(getViewChrome().toolbar).toBe(true);
  });

  it("can hide status and explorer independently", () => {
    toggleViewChrome("statusBar");
    toggleViewChrome("projectExplorer");
    expect(getViewChrome().statusBar).toBe(false);
    expect(getViewChrome().projectExplorer).toBe(false);
    expect(getViewChrome().fieldsPalette).toBe(true);
  });
});
