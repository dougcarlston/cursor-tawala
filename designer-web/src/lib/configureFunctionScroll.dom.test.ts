import { describe, expect, it } from "vitest";
import { scrollConfigureFieldIntoView } from "./configureFunctionScroll";

describe("scrollConfigureFieldIntoView", () => {
  it("scrolls the .cfg-fn-fields scroller when the field is below the fold", () => {
    const scroller = document.createElement("div");
    scroller.className = "cfg-fn-fields";
    Object.defineProperty(scroller, "scrollTop", { value: 0, writable: true });
    const field = document.createElement("input");
    scroller.appendChild(field);
    document.body.appendChild(scroller);

    scroller.getBoundingClientRect = () =>
      ({ top: 100, bottom: 300, left: 0, right: 200, width: 200, height: 200, x: 0, y: 100, toJSON: () => ({}) });
    field.getBoundingClientRect = () =>
      ({ top: 280, bottom: 320, left: 0, right: 200, width: 200, height: 40, x: 0, y: 280, toJSON: () => ({}) });

    scrollConfigureFieldIntoView(field);
    expect(scroller.scrollTop).toBeGreaterThan(0);

    document.body.innerHTML = "";
  });

  it("scrolls up when the field is above the visible area", () => {
    const scroller = document.createElement("div");
    scroller.className = "cfg-fn-fields";
    Object.defineProperty(scroller, "scrollTop", { value: 80, writable: true });
    const field = document.createElement("input");
    scroller.appendChild(field);
    document.body.appendChild(scroller);

    scroller.getBoundingClientRect = () =>
      ({ top: 100, bottom: 300, left: 0, right: 200, width: 200, height: 200, x: 0, y: 100, toJSON: () => ({}) });
    field.getBoundingClientRect = () =>
      ({ top: 60, bottom: 90, left: 0, right: 200, width: 200, height: 30, x: 0, y: 60, toJSON: () => ({}) });

    scrollConfigureFieldIntoView(field);
    expect(scroller.scrollTop).toBeLessThan(80);

    document.body.innerHTML = "";
  });
});
