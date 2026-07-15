/**
 * Safari Save / Save As: force-download (form POST or in-DOM Blob `<a>`).
 * Detached `a.click()` is a WebKit no-op; Content-Disposition is preferred on Safari.
 */
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  canUseSaveFilePicker,
  downloadJsonFallback,
  downloadJsonViaBlobAnchor,
  downloadJsonViaFormPost,
  isAppleSafari,
} from "@/lib/shellCommands";

describe("Safari / no-picker Save As download fallback", () => {
  afterEach(() => {
    vi.restoreAllMocks();
    vi.unstubAllGlobals();
    document.body.replaceChildren();
  });

  it("isAppleSafari is false under happy-dom / non-Safari UA", () => {
    expect(isAppleSafari()).toBe(false);
  });

  it("canUseSaveFilePicker is false when UA looks like Safari even if a stub exists", () => {
    vi.stubGlobal("navigator", {
      userAgent:
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Safari/605.1.15",
    });
    (window as Window & { showSaveFilePicker?: () => Promise<unknown> }).showSaveFilePicker =
      async () => {
        throw new Error("should not be used");
      };
    expect(isAppleSafari()).toBe(true);
    expect(canUseSaveFilePicker()).toBe(false);
  });

  it("downloadJsonViaFormPost posts filename + json to /api/download-project", () => {
    const submitted: Array<{ action: string; target: string; filename: string; json: string }> =
      [];
    const realCreate = document.createElement.bind(document);
    vi.spyOn(document, "createElement").mockImplementation((tag: string) => {
      const el = realCreate(tag);
      if (tag.toLowerCase() === "form") {
        el.submit = () => {
          const filename =
            el.querySelector<HTMLInputElement>('input[name="filename"]')?.value ?? "";
          const json = el.querySelector<HTMLTextAreaElement>('textarea[name="json"]')?.value ?? "";
          submitted.push({ action: el.action, target: el.target, filename, json });
        };
      }
      return el;
    });

    expect(downloadJsonViaFormPost('{"ok":true}', "Untitled.json")).toBe(true);
    expect(submitted).toHaveLength(1);
    expect(submitted[0].filename).toBe("Untitled.json");
    expect(submitted[0].json).toBe('{"ok":true}');
    expect(submitted[0].target).toBe("tawala-json-dl");
    expect(submitted[0].action).toContain("/api/download-project");
    expect(document.body.querySelector("form")).toBeNull();
    expect(document.body.querySelector('iframe[name="tawala-json-dl"]')).not.toBeNull();
  });

  it("downloadJsonViaBlobAnchor appends a link and dispatches click while in-DOM", () => {
    const createObjectURL = vi.fn(() => "blob:test-json");
    const revokeObjectURL = vi.fn();
    vi.stubGlobal("URL", { createObjectURL, revokeObjectURL });

    let clicked = false;
    let attachedWhenClicked = false;
    const realCreateNS = document.createElementNS.bind(document);
    vi.spyOn(document, "createElementNS").mockImplementation(
      (ns: string, tag: string) => {
        const el = realCreateNS(ns, tag);
        if (tag.toLowerCase() === "a") {
          el.dispatchEvent = ((evt: Event) => {
            if (evt.type === "click") {
              clicked = true;
              attachedWhenClicked = document.body.contains(el);
            }
            return true;
          }) as typeof el.dispatchEvent;
        }
        return el;
      },
    );

    downloadJsonViaBlobAnchor('{"ok":true}', "Untitled.json");

    expect(createObjectURL).toHaveBeenCalled();
    expect(clicked).toBe(true);
    expect(attachedWhenClicked).toBe(true);
    // Anchor stays briefly (Safari) — still in DOM until the delayed cleanup.
    expect(document.body.querySelector("a")).not.toBeNull();
  });

  it("downloadJsonFallback uses blob anchor when not Safari", () => {
    const createObjectURL = vi.fn(() => "blob:test-json");
    vi.stubGlobal("URL", { createObjectURL, revokeObjectURL: vi.fn() });

    let clicked = false;
    const realCreateNS = document.createElementNS.bind(document);
    vi.spyOn(document, "createElementNS").mockImplementation(
      (ns: string, tag: string) => {
        const el = realCreateNS(ns, tag);
        if (tag.toLowerCase() === "a") {
          el.dispatchEvent = ((evt: Event) => {
            if (evt.type === "click") clicked = true;
            return true;
          }) as typeof el.dispatchEvent;
        }
        return el;
      },
    );

    downloadJsonFallback('{"ok":true}', "MyProject.json");
    expect(isAppleSafari()).toBe(false);
    expect(clicked).toBe(true);
    expect(createObjectURL).toHaveBeenCalled();
  });

  it("downloadJsonFallback prefers form POST on Safari UA", () => {
    vi.stubGlobal("navigator", {
      userAgent:
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Safari/605.1.15",
    });
    // Do not stub global URL — happy-dom needs it when the hidden iframe attaches.
    const createObjectURL = vi.spyOn(URL, "createObjectURL");

    let submitted = false;
    const realCreate = document.createElement.bind(document);
    vi.spyOn(document, "createElement").mockImplementation((tag: string) => {
      const el = realCreate(tag);
      if (tag.toLowerCase() === "form") {
        el.submit = () => {
          submitted = true;
        };
      }
      return el;
    });

    downloadJsonFallback('{"safari":true}', "Untitled.json");
    expect(isAppleSafari()).toBe(true);
    expect(submitted).toBe(true);
    expect(createObjectURL).not.toHaveBeenCalled();
  });
});
