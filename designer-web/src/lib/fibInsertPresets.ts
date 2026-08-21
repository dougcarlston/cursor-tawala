import type { FibItem } from "@/types/tawala";

export type FibInsertPresetId = "date" | "address";

/**
 * Insert → Date / Address presets: ordinary FIBs (not composite types).
 * Spec: docs/DIRTBOWL_PAGE1_DESIGNER_EXEMPLAR.md
 *
 * Both use freeform: Java AlignedLabelsLayout keeps only the *first* blank in the
 * field column (rest → remainder), which breaks mm/dd/yyyy and Street/City/Zip.
 * DirtBowl Registration omits style (DefaultLayout) for these rows.
 */

/** mm/dd/yyyy with slash separators — Deploy `isDobRow` + freeform DefaultLayout. */
export function createDateFibPreset(label: string): FibItem {
  return {
    type: "fib",
    label,
    style: "freeform",
    prompt: "Date of Birth: _____ / _____ / ________ (mm/dd/yyyy)",
    blanks: [
      { name: "a", length: 5, height: 1, alternateLabel: "Month" },
      { name: "b", length: 5, height: 1, alternateLabel: "Day" },
      { name: "c", length: 8, height: 1, alternateLabel: "Year" },
    ],
  };
}

/**
 * One soft-row Address with per-blank captions (Street / City / Zip) — same
 * above-hint pattern as Name First/Last. No inline City:/Zip: text.
 */
export function createAddressFibPreset(label: string): FibItem {
  return {
    type: "fib",
    label,
    style: "freeform",
    prompt: "Address: ____________________ ________ _____",
    blanks: [
      {
        name: "a",
        length: 20,
        height: 1,
        alternateLabel: "Street",
        caption: "Street",
      },
      {
        name: "b",
        length: 8,
        height: 1,
        alternateLabel: "City",
        caption: "City",
      },
      {
        name: "c",
        length: 5,
        height: 1,
        alternateLabel: "Zip",
        caption: "Zip",
      },
    ],
  };
}

export function createFibInsertPreset(id: FibInsertPresetId, label: string): FibItem {
  return id === "date" ? createDateFibPreset(label) : createAddressFibPreset(label);
}
