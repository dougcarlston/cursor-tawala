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
 * Two-line Address preset:
 * Line 1: Street Address: ______________________________
 * Line 2: City, State, Zip: __________________  _____  __________ (with captions)
 * Uses two paragraphs so each line gets its own left-aligned label and blanks fit standard columns.
 */
export function createAddressFibPreset(label: string): FibItem {
  return {
    type: "fib",
    label,
    prompt: "Street: ______________________________<br>City, State, Zip: ____________________ _____ ________",
    blanks: [
      {
        name: "a",
        length: 30,
        height: 1,
        alternateLabel: "Street",
        caption: "Street Address",
      },
      {
        name: "b",
        length: 20,
        height: 1,
        alternateLabel: "City",
        caption: "City",
      },
      {
        name: "c",
        length: 5,
        height: 1,
        alternateLabel: "State",
        caption: "State",
      },
      {
        name: "d",
        length: 8,
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
