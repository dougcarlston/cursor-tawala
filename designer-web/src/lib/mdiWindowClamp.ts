/**
 * Keep MDI child windows inside `.mdi-surface` so title-bar controls stay reachable.
 *
 * Project Explorer sits left of the surface (flex sibling) — clamping x ≥ 0 already
 * prevents sliding under it. Fields sits to the right; the old drag clamp only kept
 * ~80px of the *left* edge on-screen, so minimize/close (upper-right) could slide
 * under Fields / off the clipped canvas edge.
 */

/** Minimize + close (~20+3+20) plus a small margin. */
export const MDI_TITLEBAR_CONTROLS_W = 56;
export const MDI_TITLEBAR_H = 28;

export interface MdiBounds {
  x: number;
  y: number;
  w: number;
  h: number;
}

/**
 * Clamp origin so a fitting window stays fully on-canvas (right edge = controls).
 * Left stays ≥ 0 so frames do not slide toward Project Explorer.
 */
export function clampMdiWindowOrigin(
  x: number,
  y: number,
  w: number,
  h: number,
  parentW: number,
  parentH: number,
): { x: number; y: number } {
  const maxX = Math.max(0, parentW - w);
  const maxY = Math.max(0, parentH - MDI_TITLEBAR_H);
  return {
    x: Math.max(0, Math.min(x, maxX)),
    y: Math.max(0, Math.min(y, maxY)),
  };
}

/**
 * After a resize, keep the frame inside the parent and enforce minimums.
 * East/south growth is capped so controls cannot be pushed under Fields.
 */
export function clampMdiWindowBounds(
  bounds: MdiBounds,
  parentW: number,
  parentH: number,
  minW: number,
  minH: number,
): MdiBounds {
  let { x, y, w, h } = bounds;
  w = Math.max(minW, w);
  h = Math.max(minH, h);

  if (parentW > 0) {
    w = Math.min(w, Math.max(minW, parentW));
    x = Math.max(0, x);
    if (x + w > parentW) {
      x = Math.max(0, parentW - w);
      if (x + w > parentW) w = Math.max(minW, parentW - x);
    }
  }
  if (parentH > 0) {
    h = Math.min(h, Math.max(minH, parentH));
    y = Math.max(0, y);
    if (y + h > parentH) {
      y = Math.max(0, parentH - h);
      if (y + h > parentH) h = Math.max(minH, parentH - y);
    }
  }

  const origin = clampMdiWindowOrigin(x, y, w, h, parentW, parentH);
  return { x: origin.x, y: origin.y, w, h };
}
