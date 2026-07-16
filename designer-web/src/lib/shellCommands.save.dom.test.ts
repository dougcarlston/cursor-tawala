/**
 * Chromium Save As: showSaveFilePicker must run before exportJson so a large
 * project (embedded images) does not burn Chrome’s user-gesture window.
 */
import { afterEach, describe, expect, it, vi } from "vitest";
import {
  cancelSaveAsDialog,
  confirmSaveAs,
  isSaveAsDialogOpen,
  saveProjectAs,
} from "@/lib/shellCommands";
import { useProjectStore } from "@/store/projectStore";

describe("Chromium Save As picker before export", () => {
  afterEach(() => {
    if (isSaveAsDialogOpen()) cancelSaveAsDialog();
    delete (window as Window & { showSaveFilePicker?: unknown }).showSaveFilePicker;
    vi.restoreAllMocks();
  });

  it("confirmSaveAs opens the picker before exportJson", async () => {
    const order: string[] = [];
    const writable = {
      write: vi.fn(async () => undefined),
      close: vi.fn(async () => undefined),
    };
    const handle = {
      name: "Smoke.json",
      createWritable: vi.fn(async () => writable),
    };
    (window as Window & { showSaveFilePicker?: () => Promise<unknown> }).showSaveFilePicker =
      async () => {
        order.push("picker");
        return handle;
      };

    const origExport = useProjectStore.getState().exportJson;
    useProjectStore.setState({
      exportJson: () => {
        order.push("export");
        return '{"name":"Smoke"}';
      },
    });

    try {
      saveProjectAs();
      expect(isSaveAsDialogOpen()).toBe(true);
      await confirmSaveAs("Smoke");
      expect(order).toEqual(["picker", "export"]);
      expect(isSaveAsDialogOpen()).toBe(false);
      expect(writable.write).toHaveBeenCalled();
    } finally {
      useProjectStore.setState({ exportJson: origExport });
    }
  });
});
