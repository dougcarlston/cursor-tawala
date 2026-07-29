/**
 * Windows menu layout math for the MDI canvas (`.mdi-surface`).
 * Cascade / Tile Horizontal / Tile Vertical + minimize restore sizing.
 *
 * Legacy (screenshots July 2026): Cascade stacks restored frames diagonally;
 * Tile Vertically = column-primary (side-by-side, grid when many); Tile
 * Horizontally = row-primary. Minimized windows stay as bottom squibs and are
 * excluded from arrange geometry.
 *
 * Drag may slide frames under PE / Items / Fields (`overflow: visible`). Cascade /
 * Tile / Maximize use {@link getMdiArrangeViewport} so title-bar controls stay
 * clear of those docks and the squib strip.
 */

import { MDI_TITLEBAR_H } from "./mdiWindowClamp";

export interface MdiViewport {
  width: number;
  height: number;
}

export interface MdiBounds {
  x: number;
  y: number;
  w: number;
  h: number;
}

const CASCADE_STEP = 28;
const CASCADE_WRAP = 8;
const DEFAULT_W = 640;
const DEFAULT_H = 460;
/** Comfortable minimum for cascade / interactive restore (not dense tile cells). */
export const MDI_RESTORE_MIN_W = 320;
export const MDI_RESTORE_MIN_H = 180;
export const MDI_DEFAULT_W = DEFAULT_W;
export const MDI_DEFAULT_H = DEFAULT_H;
/** Floor for dense tile cells so ~20 windows can fill a typical canvas. */
const TILE_CELL_MIN_W = 100;
const TILE_CELL_MIN_H = 72;
const PAD = 8;

/** Read live MDI surface size; falls back if the canvas is not mounted. */
export function getMdiSurfaceViewport(): MdiViewport {
  if (typeof document === "undefined") {
    return { width: 800, height: 560 };
  }
  const el = document.querySelector(".mdi-surface");
  if (!el) return { width: 800, height: 560 };
  const r = el.getBoundingClientRect();
  return {
    width: Math.max(200, Math.floor(r.width)),
    height: Math.max(160, Math.floor(r.height)),
  };
}

export interface ArrangeSurfaceRect {
  left: number;
  right: number;
  top: number;
  bottom: number;
  clientWidth: number;
  clientHeight: number;
}

export interface ArrangeDockInsets {
  /** Absolute x of a left dock's right edge (clips arrange left). */
  left?: number;
  /** Absolute x of a right dock's left edge (clips arrange right). */
  right?: number;
  /** Absolute y of a bottom dock's top edge (clips arrange bottom). */
  bottom?: number;
  top?: number;
}

/**
 * Pure arrange-size math: intersect the surface box with dock edges, then take
 * the tighter of layout client size vs clipped screen rect.
 */
export function clipArrangeViewport(
  surface: ArrangeSurfaceRect,
  docks: ArrangeDockInsets,
): MdiViewport {
  let left = surface.left;
  let right = surface.right;
  let top = surface.top;
  let bottom = surface.bottom;

  if (docks.left != null && docks.left > left && docks.left < right + 1) {
    left = Math.max(left, docks.left);
  }
  if (docks.right != null && docks.right < right && docks.right > left - 1) {
    right = Math.min(right, docks.right);
  }
  if (docks.top != null && docks.top > top && docks.top < bottom) {
    top = Math.max(top, docks.top);
  }
  if (docks.bottom != null && docks.bottom < bottom && docks.bottom > top) {
    bottom = Math.min(bottom, docks.bottom);
  }

  const layoutW = surface.clientWidth;
  const layoutH = surface.clientHeight;
  const clippedW = Math.floor(right - left);
  const clippedH = Math.floor(bottom - top);
  return {
    width: Math.max(200, Math.min(layoutW || clippedW, clippedW)),
    height: Math.max(160, Math.min(layoutH || clippedH, clippedH)),
  };
}

/** Viewport for Cascade/Tile/Maximize — clipped so frames stay clear of dock overlays. */
export function getMdiArrangeViewport(): MdiViewport {
  if (typeof document === "undefined") {
    return { width: 800, height: 560 };
  }
  const surface = document.querySelector(".mdi-surface");
  if (!surface) return { width: 800, height: 560 };
  const sr = surface.getBoundingClientRect();
  let left = sr.left;
  let right = sr.right;
  let top = sr.top;
  let bottom = sr.bottom;

  const clipRight = (sel: string) => {
    const el = document.querySelector(sel);
    if (!el) return;
    const r = el.getBoundingClientRect();
    if (r.width <= 0 || r.height <= 0) return;
    const vOverlap = Math.min(bottom, r.bottom) - Math.max(top, r.top);
    if (vOverlap < 8) return;
    if (r.left < right && r.left > left - 1) right = Math.min(right, r.left);
  };
  const clipLeft = (sel: string) => {
    const el = document.querySelector(sel);
    if (!el) return;
    const r = el.getBoundingClientRect();
    if (r.width <= 0 || r.height <= 0) return;
    const vOverlap = Math.min(bottom, r.bottom) - Math.max(top, r.top);
    if (vOverlap < 8) return;
    if (r.right > left && r.right < right + 1) left = Math.max(left, r.right);
  };
  const clipBottom = (sel: string) => {
    const el = document.querySelector(sel);
    if (!el) return;
    const r = el.getBoundingClientRect();
    if (r.width <= 0 || r.height <= 0) return;
    if (r.top < bottom && r.top > top) bottom = Math.min(bottom, r.top);
  };

  clipRight(".designer-right");
  clipRight(".designer-right-splitter");
  clipLeft(".designer-left");
  clipLeft(".designer-left-splitter");
  clipLeft(".designer-items");
  clipBottom(".mdi-taskbar");

  const layoutW = (surface as HTMLElement).clientWidth;
  const layoutH = (surface as HTMLElement).clientHeight;
  const clippedW = Math.floor(right - left);
  const clippedH = Math.floor(bottom - top);
  // Take the tighter of layout box vs dock-clipped rect (fixes overflow:visible paint under Fields).
  return {
    width: Math.max(200, Math.min(layoutW || clippedW, clippedW)),
    height: Math.max(160, Math.min(layoutH || clippedH, clippedH)),
  };
}

function clampCascadeSize(w: number, h: number, viewport: MdiViewport): { w: number; h: number } {
  return {
    w: Math.max(MDI_RESTORE_MIN_W, Math.min(w, Math.max(MDI_RESTORE_MIN_W, viewport.width - PAD * 2))),
    h: Math.max(MDI_RESTORE_MIN_H, Math.min(h, Math.max(MDI_RESTORE_MIN_H, viewport.height - PAD * 2))),
  };
}

/**
 * Capture a restore rect when minimizing. If the frame is already tiny (owner
 * Windows6 case: highly-shrunk then minimized → tiny box on restore), replace
 * with a sensible default so restore never permanently inherits a stub size.
 */
export function sanitizeRestoreBounds(
  bounds: MdiBounds,
  viewport?: MdiViewport,
): MdiBounds {
  const vp = viewport ?? { width: 1200, height: 800 };
  const tooSmall = bounds.w < MDI_RESTORE_MIN_W || bounds.h < MDI_RESTORE_MIN_H;
  if (!tooSmall) {
    return {
      x: bounds.x,
      y: bounds.y,
      w: bounds.w,
      h: bounds.h,
    };
  }
  const size = clampCascadeSize(DEFAULT_W, DEFAULT_H, vp);
  return {
    x: Math.max(PAD, Math.min(bounds.x, Math.max(PAD, vp.width - size.w - PAD))),
    y: Math.max(PAD, Math.min(bounds.y, Math.max(PAD, vp.height - size.h - PAD))),
    w: size.w,
    h: size.h,
  };
}

/** Bounds that fill the MDI surface (maximize). */
export function maximizedBounds(viewport: MdiViewport): MdiBounds {
  return {
    x: 0,
    y: 0,
    w: Math.max(TILE_CELL_MIN_W, viewport.width),
    h: Math.max(TILE_CELL_MIN_H, viewport.height),
  };
}

/**
 * Overlapping cascade (same step as new-window open). Restored windows only.
 *
 * Cascades down-right from a lower-left origin. When the next step would pass
 * the right edge — or after {@link CASCADE_WRAP} windows in the current stack —
 * starts a new diagonal whose originY drops below all title-bar offsets of the
 * finished stack (classic MDI wrap that clears prior title bars).
 * Vertical room is gated on title-bar visibility so frames may hang off the
 * bottom; never clamp into a flush-right column.
 */
export function cascadeBounds(count: number, viewport: MdiViewport): MdiBounds[] {
  const size = clampCascadeSize(DEFAULT_W, DEFAULT_H, viewport);
  const maxX = Math.max(PAD, viewport.width - size.w - PAD);
  // Title bar must stay on-canvas; body may hang past the bottom edge.
  const maxOriginY = Math.max(PAD, viewport.height - MDI_TITLEBAR_H - PAD);
  const out: MdiBounds[] = [];

  let originY = PAD;
  let step = 0;

  for (let i = 0; i < count; i++) {
    if (
      step > 0 &&
      (step >= CASCADE_WRAP || PAD + step * CASCADE_STEP > maxX)
    ) {
      // Always reset to the left — never grow step past maxX (flush-right column).
      // Drop below all title-bar offsets of the stack just finished, then restart left.
      // If title-bar wrap room is exhausted, clamp to maxOriginY but still reset step.
      originY = originY + step * CASCADE_STEP;
      if (originY > maxOriginY) {
        originY = maxOriginY;
      }
      step = 0;
    }

    out.push({
      x: Math.min(PAD + step * CASCADE_STEP, maxX),
      y: Math.min(originY + step * CASCADE_STEP, maxOriginY),
      w: size.w,
      h: size.h,
    });
    step += 1;
  }
  return out;
}

/**
 * Grid dimensions for tiling.
 * - Vertical (column-primary): prefer a single row of columns when each cell is
 *   wide enough; otherwise a near-square column-primary grid (20 → 5×4).
 * - Horizontal (row-primary): prefer a single column of rows when tall enough;
 *   otherwise a near-square row-primary grid.
 */
export function tileGridDimensions(
  count: number,
  viewport: MdiViewport,
  direction: "horizontal" | "vertical",
): { cols: number; rows: number } {
  if (count <= 0) return { cols: 0, rows: 0 };
  const availW = Math.max(1, viewport.width - PAD * 2);
  const availH = Math.max(1, viewport.height - PAD * 2);

  if (direction === "vertical") {
    if (availW / count >= TILE_CELL_MIN_W) {
      return { cols: count, rows: 1 };
    }
    const cols = Math.max(1, Math.ceil(Math.sqrt(count)));
    const rows = Math.max(1, Math.ceil(count / cols));
    return { cols, rows };
  }

  if (availH / count >= TILE_CELL_MIN_H) {
    return { cols: 1, rows: count };
  }
  const rows = Math.max(1, Math.ceil(Math.sqrt(count)));
  const cols = Math.max(1, Math.ceil(count / rows));
  return { cols, rows };
}

function packTileBounds(
  count: number,
  viewport: MdiViewport,
  cols: number,
  rows: number,
): MdiBounds[] {
  if (count <= 0 || cols <= 0 || rows <= 0) return [];
  const availW = Math.max(1, viewport.width - PAD * 2);
  const availH = Math.max(1, viewport.height - PAD * 2);
  // Integer division may leave a remainder; last col/row absorbs it so the grid fills.
  const cellW = Math.max(1, Math.floor(availW / cols));
  const cellH = Math.max(1, Math.floor(availH / rows));
  const out: MdiBounds[] = [];
  for (let i = 0; i < count; i++) {
    const col = i % cols;
    const row = Math.floor(i / cols);
    const x = PAD + col * cellW;
    const y = PAD + row * cellH;
    const w =
      col === cols - 1 ? Math.max(1, viewport.width - PAD - x) : cellW;
    const h =
      row === rows - 1 ? Math.max(1, viewport.height - PAD - y) : cellH;
    out.push({ x, y, w, h });
  }
  return out;
}

/** Horizontal strips / row-primary grid (stacked top → bottom). */
export function tileHorizontalBounds(count: number, viewport: MdiViewport): MdiBounds[] {
  const { cols, rows } = tileGridDimensions(count, viewport, "horizontal");
  return packTileBounds(count, viewport, cols, rows);
}

/** Vertical strips / column-primary grid (side by side). */
export function tileVerticalBounds(count: number, viewport: MdiViewport): MdiBounds[] {
  const { cols, rows } = tileGridDimensions(count, viewport, "vertical");
  return packTileBounds(count, viewport, cols, rows);
}

export function windowMenuLabel(kind: "form" | "process" | "document", name: string): string {
  const prefix = kind === "form" ? "Form" : kind === "process" ? "Process" : "Document";
  return `${prefix} - ${name}`;
}
