import { beforeEach, describe, expect, it } from "vitest";
import { cascadeBounds } from "@/lib/mdiWindowLayout";
import { useProjectStore, type DesignerWindow } from "@/store/projectStore";

const VP = { width: 2000, height: 2000 };

function win(id: string, z: number): DesignerWindow {
  return {
    id,
    kind: "document",
    name: id,
    x: 0,
    y: 0,
    w: 640,
    h: 460,
    z,
    minimized: false,
    maximized: false,
  };
}

describe("cascadeWindows z-order", () => {
  beforeEach(() => {
    useProjectStore.getState().newProject({ empty: true });
  });

  it("assigns cascade slots back-to-front by z, not openWindows array order", () => {
    const back = win("doc:back", 1);
    const mid = win("doc:mid", 2);
    const front = win("doc:front", 3);
    // Array order differs from z-order (pre-fix bug: front got first slot).
    useProjectStore.setState({
      openWindows: [front, back, mid],
      activeWindowId: back.id,
    });

    useProjectStore.getState().cascadeWindows(VP);

    const expected = cascadeBounds(3, VP);
    const { openWindows, activeWindowId } = useProjectStore.getState();
    const byId = new Map(openWindows.map((w) => [w.id, w]));

    expect(byId.get(back.id)).toMatchObject({ x: expected[0]!.x, y: expected[0]!.y });
    expect(byId.get(mid.id)).toMatchObject({ x: expected[1]!.x, y: expected[1]!.y });
    expect(byId.get(front.id)).toMatchObject({ x: expected[2]!.x, y: expected[2]!.y });
    expect(activeWindowId).toBe(front.id);
  });

  it("skips minimized windows but still sorts restored by z", () => {
    const visibleLow = win("doc:low", 1);
    const hidden = { ...win("doc:hidden", 5), minimized: true };
    const visibleHigh = win("doc:high", 10);
    useProjectStore.setState({
      openWindows: [visibleHigh, hidden, visibleLow],
      activeWindowId: visibleLow.id,
    });

    useProjectStore.getState().cascadeWindows(VP);

    const expected = cascadeBounds(2, VP);
    const { openWindows, activeWindowId } = useProjectStore.getState();
    const byId = new Map(openWindows.map((w) => [w.id, w]));

    expect(byId.get(visibleLow.id)).toMatchObject({ x: expected[0]!.x, y: expected[0]!.y });
    expect(byId.get(visibleHigh.id)).toMatchObject({ x: expected[1]!.x, y: expected[1]!.y });
    expect(byId.get(hidden.id)?.minimized).toBe(true);
    expect(activeWindowId).toBe(visibleHigh.id);
  });
});
