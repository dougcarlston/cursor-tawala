import { useCallback, useEffect, useRef, useState, useSyncExternalStore } from "react";
import {
  MAX_RECENT_FONT_COLORS,
  getRecentFontColors,
  normalizeFontColorHex,
  pushRecentFontColor,
  subscribeRecentFontColors,
} from "@/lib/recentFontColors";

interface Props {
  /** Current sticky / draft color (`#rrggbb`). */
  color: string;
  /** Live change — parent updates sticky A-bar + applies to selection. */
  onColorChange: (hex: string) => void;
  onClose: () => void;
}

type Hsv = { h: number; s: number; v: number };
type Rgb = { r: number; g: number; b: number };

function hexToRgb(hex: string): Rgb {
  const n = normalizeFontColorHex(hex) ?? "#000000";
  return {
    r: parseInt(n.slice(1, 3), 16),
    g: parseInt(n.slice(3, 5), 16),
    b: parseInt(n.slice(5, 7), 16),
  };
}

function rgbToHex(r: number, g: number, b: number): string {
  const clamp = (n: number) => Math.max(0, Math.min(255, Math.round(n)));
  return `#${[clamp(r), clamp(g), clamp(b)]
    .map((n) => n.toString(16).padStart(2, "0"))
    .join("")}`;
}

function rgbToHsv({ r, g, b }: Rgb): Hsv {
  const rn = r / 255;
  const gn = g / 255;
  const bn = b / 255;
  const max = Math.max(rn, gn, bn);
  const min = Math.min(rn, gn, bn);
  const d = max - min;
  let h = 0;
  if (d !== 0) {
    if (max === rn) h = ((gn - bn) / d) % 6;
    else if (max === gn) h = (bn - rn) / d + 2;
    else h = (rn - gn) / d + 4;
    h *= 60;
    if (h < 0) h += 360;
  }
  const s = max === 0 ? 0 : d / max;
  return { h, s, v: max };
}

function hsvToRgb({ h, s, v }: Hsv): Rgb {
  const c = v * s;
  const x = c * (1 - Math.abs(((h / 60) % 2) - 1));
  const m = v - c;
  let rp = 0;
  let gp = 0;
  let bp = 0;
  if (h < 60) [rp, gp, bp] = [c, x, 0];
  else if (h < 120) [rp, gp, bp] = [x, c, 0];
  else if (h < 180) [rp, gp, bp] = [0, c, x];
  else if (h < 240) [rp, gp, bp] = [0, x, c];
  else if (h < 300) [rp, gp, bp] = [x, 0, c];
  else [rp, gp, bp] = [c, 0, x];
  return {
    r: Math.round((rp + m) * 255),
    g: Math.round((gp + m) * 255),
    b: Math.round((bp + m) * 255),
  };
}

function hueCss(h: number): string {
  const { r, g, b } = hsvToRgb({ h, s: 1, v: 1 });
  return `rgb(${r}, ${g}, ${b})`;
}

function eyeDropperAvailable(): boolean {
  return typeof window !== "undefined" && "EyeDropper" in window;
}

/**
 * Choose Color… — in-app Mac-like picker (SV + hue + RGB + optional eyedropper).
 * Recent row sits under RGB because the OS Color panel cannot host custom chrome.
 * Outside click (backdrop) or Escape dismisses.
 */
export function FontColorPickerDialog({ color, onColorChange, onClose }: Props) {
  const recent = useSyncExternalStore(
    subscribeRecentFontColors,
    getRecentFontColors,
    getRecentFontColors,
  );
  const initialHex = normalizeFontColorHex(color) ?? "#000000";
  const [hsv, setHsv] = useState<Hsv>(() => rgbToHsv(hexToRgb(initialHex)));
  const hsvRef = useRef(hsv);
  hsvRef.current = hsv;
  const dragKindRef = useRef<"sv" | "hue" | null>(null);
  const svRef = useRef<HTMLDivElement>(null);
  const hueRef = useRef<HTMLDivElement>(null);
  const dialogRef = useRef<HTMLDivElement>(null);
  const openedAtRef = useRef(0);
  const canEyeDropper = eyeDropperAvailable();

  const rgb = hsvToRgb(hsv);
  const hex = rgbToHex(rgb.r, rgb.g, rgb.b);
  const finalHexRef = useRef(hex);
  finalHexRef.current = hex;
  const slots = Array.from({ length: MAX_RECENT_FONT_COLORS }, (_, i) => recent[i] ?? null);

  const commitRecentOnClose = useCallback(() => {
    const chosen = normalizeFontColorHex(finalHexRef.current);
    if (chosen) pushRecentFontColor(chosen);
    onClose();
  }, [onClose]);

  // Sync from parent when sticky color changes without local drag (e.g. recent click).
  useEffect(() => {
    if (dragKindRef.current) return;
    const next = normalizeFontColorHex(color);
    if (!next || next === hex) return;
    setHsv(rgbToHsv(hexToRgb(next)));
    // Only respond to external `color`; `hex` is derived from local hsv.
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [color]);

  useEffect(() => {
    const onKey = (e: KeyboardEvent) => {
      if (e.key === "Escape") commitRecentOnClose();
    };
    window.addEventListener("keydown", onKey);
    return () => window.removeEventListener("keydown", onKey);
  }, [commitRecentOnClose]);

  // Dismiss when pointer goes outside the dialog (backdrop alone is unreliable with flex + pointer-events).
  useEffect(() => {
    openedAtRef.current = performance.now();
    const onPointerDown = (e: PointerEvent) => {
      if (performance.now() - openedAtRef.current < 80) return;
      const panel = dialogRef.current;
      if (!panel || panel.contains(e.target as Node)) return;
      commitRecentOnClose();
    };
    document.addEventListener("pointerdown", onPointerDown, true);
    return () => document.removeEventListener("pointerdown", onPointerDown, true);
  }, [commitRecentOnClose]);

  const applyHsv = useCallback(
    (next: Hsv) => {
      setHsv(next);
      const rgbNext = hsvToRgb(next);
      const hexNext = rgbToHex(rgbNext.r, rgbNext.g, rgbNext.b);
      onColorChange(hexNext);
    },
    [onColorChange],
  );

  const applyHex = useCallback(
    (raw: string) => {
      const next = normalizeFontColorHex(raw);
      if (!next) return;
      setHsv(rgbToHsv(hexToRgb(next)));
      onColorChange(next);
    },
    [onColorChange],
  );

  const pickSv = useCallback(
    (clientX: number, clientY: number) => {
      const el = svRef.current;
      if (!el) return;
      const rect = el.getBoundingClientRect();
      const s = Math.max(0, Math.min(1, (clientX - rect.left) / rect.width));
      const v = Math.max(0, Math.min(1, 1 - (clientY - rect.top) / rect.height));
      applyHsv({ ...hsvRef.current, s, v });
    },
    [applyHsv],
  );

  const pickHue = useCallback(
    (clientX: number) => {
      const el = hueRef.current;
      if (!el) return;
      const rect = el.getBoundingClientRect();
      const t = Math.max(0, Math.min(1, (clientX - rect.left) / rect.width));
      applyHsv({ ...hsvRef.current, h: t * 360 });
    },
    [applyHsv],
  );

  useEffect(() => {
    const onMove = (e: PointerEvent) => {
      const kind = dragKindRef.current;
      if (!kind) return;
      if (kind === "sv") pickSv(e.clientX, e.clientY);
      else pickHue(e.clientX);
    };
    const onUp = (e: PointerEvent) => {
      const kind = dragKindRef.current;
      if (!kind) return;
      dragKindRef.current = null;
      if (kind === "sv") pickSv(e.clientX, e.clientY);
      else pickHue(e.clientX);
    };
    window.addEventListener("pointermove", onMove);
    window.addEventListener("pointerup", onUp);
    return () => {
      window.removeEventListener("pointermove", onMove);
      window.removeEventListener("pointerup", onUp);
    };
  }, [pickHue, pickSv]);

  const setChannel = (channel: keyof Rgb, raw: number) => {
    const value = Number.isFinite(raw) ? Math.max(0, Math.min(255, Math.round(raw))) : 0;
    const next = { ...rgb, [channel]: value };
    applyHex(rgbToHex(next.r, next.g, next.b));
  };

  const runEyedropper = async () => {
    if (!canEyeDropper) return;
    type EyeDropperCtor = new () => { open: () => Promise<{ sRGBHex: string }> };
    const Ctor = (window as unknown as { EyeDropper: EyeDropperCtor }).EyeDropper;
    try {
      const result = await new Ctor().open();
      applyHex(result.sRGBHex);
    } catch {
      /* user cancelled */
    }
  };

  return (
    <div
      className="modal-backdrop font-color-picker-backdrop"
      role="presentation"
      onMouseDown={(e) => {
        if (e.target === e.currentTarget) commitRecentOnClose();
      }}
    >
      <div
        ref={dialogRef}
        className="modal font-color-picker-dialog"
        role="dialog"
        aria-labelledby="font-color-picker-title"
        aria-modal="true"
        onMouseDown={(e) => e.stopPropagation()}
      >
        <h2 id="font-color-picker-title">Color</h2>

        <div className="font-color-picker-body">
          <div
            ref={svRef}
            className="font-color-picker-sv"
            style={{ backgroundColor: hueCss(hsv.h) }}
            role="slider"
            aria-label="Saturation and brightness"
            aria-valuetext={`S ${Math.round(hsv.s * 100)}% V ${Math.round(hsv.v * 100)}%`}
            onPointerDown={(e) => {
              e.preventDefault();
              dragKindRef.current = "sv";
              pickSv(e.clientX, e.clientY);
            }}
          >
            <div className="font-color-picker-sv-white" aria-hidden />
            <div className="font-color-picker-sv-black" aria-hidden />
            <div
              className="font-color-picker-sv-thumb"
              style={{ left: `${hsv.s * 100}%`, top: `${(1 - hsv.v) * 100}%` }}
              aria-hidden
            />
          </div>
        </div>

        <div className="font-color-picker-tools">
          <button
            type="button"
            className="font-color-picker-eyedropper"
            title={canEyeDropper ? "Eyedropper" : "Eyedropper (not available)"}
            disabled={!canEyeDropper}
            aria-label="Eyedropper"
            onMouseDown={(e) => e.preventDefault()}
            onClick={() => void runEyedropper()}
          >
            <svg viewBox="0 0 16 16" width="14" height="14" aria-hidden>
              <path
                fill="currentColor"
                d="M11.5 1.5a2.12 2.12 0 0 1 3 3L6.2 12.8l-3.5.7.7-3.5L11.5 1.5zm1.4 1.4a.62.62 0 0 0-.9 0L11 3.9l.9.9 1-1a.62.62 0 0 0 0-.9zM2.5 13.5l.7-2.1 1.4 1.4-2.1.7z"
              />
            </svg>
          </button>
          <div className="font-color-picker-swatch" style={{ background: hex }} title={hex} />
          <div
            ref={hueRef}
            className="font-color-picker-hue"
            role="slider"
            aria-label="Hue"
            aria-valuemin={0}
            aria-valuemax={360}
            aria-valuenow={Math.round(hsv.h)}
            onPointerDown={(e) => {
              e.preventDefault();
              dragKindRef.current = "hue";
              pickHue(e.clientX);
            }}
          >
            <div
              className="font-color-picker-hue-thumb"
              style={{ left: `${(hsv.h / 360) * 100}%` }}
              aria-hidden
            />
          </div>
        </div>

        <div className="font-color-picker-rgb" aria-label="RGB">
          {(["r", "g", "b"] as const).map((channel) => (
            <label key={channel} className="font-color-picker-rgb-col">
              <input
                type="number"
                min={0}
                max={255}
                size={3}
                inputMode="numeric"
                value={rgb[channel]}
                onChange={(e) => setChannel(channel, Number(e.target.value))}
                onBlur={() => applyHex(hex)}
              />
              <span>{channel.toUpperCase()}</span>
            </label>
          ))}
        </div>

        <div className="font-color-picker-recent" role="group" aria-label="Recent colors">
          {slots.map((slotHex, i) =>
            slotHex ? (
              <button
                key={`${slotHex}-${i}`}
                type="button"
                className="font-color-picker-recent-slot"
                title={slotHex}
                aria-label={`Recent color ${slotHex}`}
                style={{ background: slotHex }}
                onMouseDown={(e) => e.preventDefault()}
                onClick={() => applyHex(slotHex)}
              />
            ) : (
              <span
                key={`empty-${i}`}
                className="font-color-picker-recent-slot font-color-picker-recent-slot-empty"
                aria-hidden
              />
            ),
          )}
        </div>
      </div>
    </div>
  );
}
