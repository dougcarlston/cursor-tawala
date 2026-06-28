import { itemKey } from "./runtimeEngine.mjs";

/** Mirrors Java FormSegments — splits a form into browser pages at skipInstructions/break. */
export function buildFormSegments(form) {
  const items = form.items ?? [];
  const segments = [];
  let initialSkips = [];
  let current = { items: [], skipBlocks: [] };
  let newBlockRequired = false;

  const pushSegment = () => {
    if (current.items.length || current.skipBlocks.length) {
      segments.push(current);
    }
    current = { items: [], skipBlocks: [] };
  };

  const isBlank = () => segments.length === 0 && current.items.length === 0;

  for (const item of items) {
    if (item.type === "skipInstructions") {
      if (isBlank()) {
        initialSkips = initialSkips.concat(item.commands ?? []);
        continue;
      }
      current.skipBlocks.push(item.commands ?? []);
      newBlockRequired = true;
      continue;
    }

    if (item.type === "break") {
      newBlockRequired = true;
      continue;
    }

    if (item.type === "field") {
      continue;
    }

    if (newBlockRequired) {
      pushSegment();
      newBlockRequired = false;
    }
    current.items.push(item);
  }

  if (current.items.length || current.skipBlocks.length) {
    segments.push(current);
  }

  if (segments.length === 0) {
    segments.push({ items: items.filter((i) => i.type !== "skipInstructions" && i.type !== "field"), skipBlocks: [] });
  }

  return { segments, initialSkips };
}

export function findSegmentForSkip(segments, label) {
  for (let i = 0; i < segments.length; i++) {
    const seg = segments[i];
    for (let j = 0; j < seg.items.length; j++) {
      const item = seg.items[j];
      if (item.label === label || itemKey(item) === label) {
        return { index: i, startLabel: label };
      }
    }
  }
  return { index: 0, startLabel: null };
}

export function segmentVisibleItems(segment, startLabel) {
  const items = segment.items;
  if (!startLabel) return items;
  let start = false;
  const result = [];
  for (const item of items) {
    const key = itemKey(item);
    if (!start) {
      if (key === startLabel || item.label === startLabel) start = true;
      else continue;
    }
    result.push(item);
  }
  return result.length ? result : items;
}
