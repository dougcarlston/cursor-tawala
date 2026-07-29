/**
 * MDI child-window drag/resize clamps for `.mdi-surface`.
 *
 * Docked palettes (Project Explorer, Items/Statements, Fields) sit as flex
 * siblings beside the surface and stack above it in z-index. Windows may
 * extend under those docks so title-bar minimize/close can be dragged clear
 * of palette occlusion — especially after palettes were hidden, a window was
 * enlarged, then palettes restored (surface shrinks; frame stays oversized).
 *
 * Keep a strip of the title bar intersecting the surface so a window can
 * always be grabbed and dragged back.
 */

/** Minimize + maximize/restore + close (~16×3 + gaps) plus a small margin. */
export const MDI_TITLEBAR_CONTROLS_W = 72;
export const MDI_TITLEBAR_H = 28;

export interface MdiBounds {
  x: number;
  y: number;
  w: number;
  h: number;
}

/**
 * Clamp origin so at least {@link MDI_TITLEBAR_CONTROLS_W} of the frame stays
 * inside the surface horizontally (and the title bar stays reachable
 * vertically). Negative x / x past `parentW - w` are allowed — that is how
 * frames slide under left/right palettes.
 */
export function clampMdiWindowOrigin(
  x: number,
  y: number,
  w: number,
  _h: number,
  parentW: number,
  parentH: number,
): { x: number; y: number } {
  // Fully left: only the right CONTROLS_W of the frame remains on-canvas.
  const minX = MDI_TITLEBAR_CONTROLS_W - w;
  // Fully right: only the left CONTROLS_W remains on-canvas (under Fields side).
  const maxX = Math.max(minX, parentW - MDI_TITLEBAR_CONTROLS_W);
  const maxY = Math.max(0, parentH - MDI_TITLEBAR_H);
  return {
    x: Math.max(minX, Math.min(x, maxX)),
    y: Math.max(0, Math.min(y, maxY)),
  };
}

/**
 * After a resize, enforce minimums and keep size from exploding past the
 * surface. Origin may still sit under palette docks (via
 * {@link clampMdiWindowOrigin}); width/height stay within the parent so
 * maximize/tile math remains sane.
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
    // Prefer keeping the frame in-surface when resizing, but allow the
    // subsequent origin clamp to slide under docks if the user already had.
    if (x + w > parentW) {
      x = parentW - w;
    }
  }
  if (parentH > 0) {
    h = Math.min(h, Math.max(minH, parentH));
    if (y + h > parentH) {
      y = parentH - h;
    }
    y = Math.max(0, y);
  }

  const origin = clampMdiWindowOrigin(x, y, w, h, parentW, parentH);
  return { x: origin.x, y: origin.y, w, h };
}
