/**
 * Windows menu layout math for the MDI canvas (`.mdi-surface`).
 * Cascade / Tile Horizontal / Tile Vertical.
 */

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
const MIN_W = 280;
const MIN_H = 200;
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

function clampSize(w: number, h: number, viewport: MdiViewport): { w: number; h: number } {
  return {
    w: Math.max(MIN_W, Math.min(w, viewport.width - PAD * 2)),
    h: Math.max(MIN_H, Math.min(h, viewport.height - PAD * 2)),
  };
}

/** Overlapping cascade (same step as new-window open). */
export function cascadeBounds(count: number, viewport: MdiViewport): MdiBounds[] {
  const size = clampSize(DEFAULT_W, DEFAULT_H, viewport);
  const out: MdiBounds[] = [];
  for (let i = 0; i < count; i++) {
    const offset = (i % CASCADE_WRAP) * CASCADE_STEP;
    const x = Math.min(PAD + offset, Math.max(PAD, viewport.width - size.w - PAD));
    const y = Math.min(PAD + offset, Math.max(PAD, viewport.height - size.h - PAD));
    out.push({ x, y, w: size.w, h: size.h });
  }
  return out;
}

/** Horizontal strips (stacked top → bottom). */
export function tileHorizontalBounds(count: number, viewport: MdiViewport): MdiBounds[] {
  if (count <= 0) return [];
  const h = Math.max(MIN_H, Math.floor((viewport.height - PAD * 2) / count));
  const w = Math.max(MIN_W, viewport.width - PAD * 2);
  const out: MdiBounds[] = [];
  for (let i = 0; i < count; i++) {
    out.push({ x: PAD, y: PAD + i * h, w, h });
  }
  return out;
}

/** Vertical strips (side by side). */
export function tileVerticalBounds(count: number, viewport: MdiViewport): MdiBounds[] {
  if (count <= 0) return [];
  const w = Math.max(MIN_W, Math.floor((viewport.width - PAD * 2) / count));
  const h = Math.max(MIN_H, viewport.height - PAD * 2);
  const out: MdiBounds[] = [];
  for (let i = 0; i < count; i++) {
    out.push({ x: PAD + i * w, y: PAD, w, h });
  }
  return out;
}

export function windowMenuLabel(kind: "form" | "process" | "document", name: string): string {
  const prefix = kind === "form" ? "Form" : kind === "process" ? "Process" : "Document";
  return `${prefix} - ${name}`;
}
