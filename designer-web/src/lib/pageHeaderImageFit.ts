/**
 * Page Header banner image viewport — pan / zoom / stretch inside a fixed frame.
 * Bake crops at source resolution (no downscale to banner CSS size) so Deploy stays sharp.
 */

/** Dialog + Deploy banner preview aspect (CSS caps live banners ~160px tall). */
export const PAGE_HEADER_FRAME_W = 520;
export const PAGE_HEADER_FRAME_H = 160;

export interface PageHeaderImageViewport {
  /** Top-left of the image in frame coordinates (px). */
  x: number;
  y: number;
  /** Display width / natural width. */
  scaleX: number;
  /** Display height / natural height. Independent of scaleX → allows sideways stretch. */
  scaleY: number;
}

/** Cover-fit the image in the frame (legacy object-fit: cover default). */
export function coverViewport(naturalW: number, naturalH: number): PageHeaderImageViewport {
  const nw = Math.max(1, naturalW);
  const nh = Math.max(1, naturalH);
  const cover = Math.max(PAGE_HEADER_FRAME_W / nw, PAGE_HEADER_FRAME_H / nh);
  return {
    x: (PAGE_HEADER_FRAME_W - nw * cover) / 2,
    y: (PAGE_HEADER_FRAME_H - nh * cover) / 2,
    scaleX: cover,
    scaleY: cover,
  };
}

/** Clamp pan so the frame stays filled when possible (letterbox only if image too small). */
export function clampViewport(
  vp: PageHeaderImageViewport,
  naturalW: number,
  naturalH: number,
): PageHeaderImageViewport {
  const dw = Math.max(1, naturalW * vp.scaleX);
  const dh = Math.max(1, naturalH * vp.scaleY);
  let { x, y } = vp;
  if (dw >= PAGE_HEADER_FRAME_W) {
    x = Math.min(0, Math.max(PAGE_HEADER_FRAME_W - dw, x));
  } else {
    x = (PAGE_HEADER_FRAME_W - dw) / 2;
  }
  if (dh >= PAGE_HEADER_FRAME_H) {
    y = Math.min(0, Math.max(PAGE_HEADER_FRAME_H - dh, y));
  } else {
    y = (PAGE_HEADER_FRAME_H - dh) / 2;
  }
  return { ...vp, x, y };
}

/**
 * Source-image rectangle that maps onto the banner frame under `vp`.
 * Coordinates are in natural image pixels (may be slightly outside bounds if letterboxed).
 */
export function sourceRectForFrame(
  vp: PageHeaderImageViewport,
  _naturalW: number,
  _naturalH: number,
): { sx: number; sy: number; sw: number; sh: number } {
  return {
    sx: -vp.x / vp.scaleX,
    sy: -vp.y / vp.scaleY,
    sw: PAGE_HEADER_FRAME_W / vp.scaleX,
    sh: PAGE_HEADER_FRAME_H / vp.scaleY,
  };
}

const MAX_BAKE_EDGE = 2400;

/**
 * Rasterize the current viewport to a PNG data URL at source crop resolution
 * (stretched into banner aspect). Does not downscale to the 520×160 CSS box.
 */
export async function bakePageHeaderViewport(
  imageSrc: string,
  naturalW: number,
  naturalH: number,
  vp: PageHeaderImageViewport,
): Promise<{ dataUrl: string; width: number; height: number }> {
  const img = await loadHtmlImage(imageSrc);
  const rect = sourceRectForFrame(vp, naturalW, naturalH);
  // Output matches banner aspect at the larger of the two source edges (sharp).
  let outW = Math.round(Math.abs(rect.sw));
  let outH = Math.round(Math.abs(rect.sh));
  // Force banner aspect. Deploy uses object-fit:fill so this crop maps 1:1 into the
  // live 100%×160px banner (cover was re-cropping top/bottom on wide forms).
  const aspect = PAGE_HEADER_FRAME_W / PAGE_HEADER_FRAME_H;
  if (outW / outH > aspect) {
    outH = Math.max(1, Math.round(outW / aspect));
  } else {
    outW = Math.max(1, Math.round(outH * aspect));
  }
  const edge = Math.max(outW, outH);
  if (edge > MAX_BAKE_EDGE) {
    const k = MAX_BAKE_EDGE / edge;
    outW = Math.max(1, Math.round(outW * k));
    outH = Math.max(1, Math.round(outH * k));
  }

  const canvas = document.createElement("canvas");
  canvas.width = outW;
  canvas.height = outH;
  const ctx = canvas.getContext("2d");
  if (!ctx) throw new Error("Could not create canvas for Page Header bake");
  ctx.fillStyle = "#ffffff";
  ctx.fillRect(0, 0, outW, outH);
  ctx.imageSmoothingEnabled = true;
  ctx.imageSmoothingQuality = "high";

  // Clamp source sample to the bitmap; map that sub-rect into the output.
  const sx0 = Math.max(0, rect.sx);
  const sy0 = Math.max(0, rect.sy);
  const sx1 = Math.min(naturalW, rect.sx + rect.sw);
  const sy1 = Math.min(naturalH, rect.sy + rect.sh);
  const sw = Math.max(1, sx1 - sx0);
  const sh = Math.max(1, sy1 - sy0);
  const dx0 = ((sx0 - rect.sx) / rect.sw) * outW;
  const dy0 = ((sy0 - rect.sy) / rect.sh) * outH;
  const dw = (sw / rect.sw) * outW;
  const dh = (sh / rect.sh) * outH;
  ctx.drawImage(img, sx0, sy0, sw, sh, dx0, dy0, dw, dh);
  const dataUrl = canvas.toDataURL("image/png");
  return { dataUrl, width: outW, height: outH };
}

function loadHtmlImage(src: string): Promise<HTMLImageElement> {
  return new Promise((resolve, reject) => {
    const img = new Image();
    img.onload = () => resolve(img);
    img.onerror = () => reject(new Error("Could not load Page Header image"));
    img.src = src;
  });
}
