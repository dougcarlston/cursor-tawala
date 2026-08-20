/**
 * Detect import gaps that remain in project JSON so Design can cue authors
 * (Explorer → canvas → Configure Function). Driven by data markers the converter
 * leaves — primarily **uneditable** leftovers (e.g. per-column `displayCondition`).
 *
 * Item-level Form Item `displayCondition` is editable (Aug 10: Display conditionally…)
 * so it is **not** a preserved gap — braces mark it as product state instead.
 *
 * See DESIGNER_OPEN_BUGS.md § Hierarchical convert / preserved-warning cues.
 */

import type {
  FormItem,
  RichContentBlock,
  RichTextNode,
  TawalaDocument,
  TawalaForm,
  TawalaProject,
} from "@/types/tawala";

export const PRESERVED_CONDITION_TOOLTIP =
  "Displayed conditionally — right-click the badge to edit";

export const PRESERVED_COLUMN_CONDITION_TOOLTIP =
  "Column visibility condition preserved; Designer cannot edit yet";

export const PRESERVED_FUNCTION_TOOLTIP =
  "Function has preserved visibility conditions; Designer cannot edit them yet";

/** True when a value looks like a stored displayCondition tree/object. */
export function hasDisplayCondition(value: unknown): boolean {
  if (value == null) return false;
  if (typeof value === "string") return value.trim().length > 0;
  if (typeof value === "object") return Object.keys(value as object).length > 0;
  return true;
}

export function formItemHasDisplayCondition(item: FormItem | { displayCondition?: unknown }): boolean {
  return hasDisplayCondition((item as { displayCondition?: unknown }).displayCondition);
}

/** Walk rich / structured content for itemization columns with displayCondition. */
export function contentHasColumnDisplayCondition(
  content: string | RichContentBlock[] | RichTextNode[] | undefined,
): boolean {
  if (content == null) return false;
  if (typeof content === "string") {
    // Document HTML embeds structured nodes as URI-encoded JSON in attributes.
    return (
      content.includes("displayCondition") ||
      content.includes("display-conditions") ||
      content.includes("displayConditions")
    );
  }
  return walkNodesForColumnDc(content);
}

function walkNodesForColumnDc(nodes: unknown): boolean {
  if (!nodes || typeof nodes !== "object") return false;
  if (Array.isArray(nodes)) {
    return nodes.some((n) => walkNodesForColumnDc(n));
  }
  const n = nodes as Record<string, unknown>;
  if (n.type === "itemizationTable" && Array.isArray(n.columns)) {
    if (
      (n.columns as { displayCondition?: unknown }[]).some((c) =>
        hasDisplayCondition(c?.displayCondition),
      )
    ) {
      return true;
    }
  }
  if (Array.isArray(n.nodes) && walkNodesForColumnDc(n.nodes)) return true;
  if (Array.isArray(n.content) && walkNodesForColumnDc(n.content)) return true;
  if (Array.isArray(n.rows)) {
    for (const row of n.rows as { cells?: { content?: unknown }[] }[]) {
      for (const cell of row.cells ?? []) {
        if (walkNodesForColumnDc(cell.content)) return true;
      }
    }
  }
  return false;
}

/**
 * True when a form item still has an **uneditable** convert leftover cue.
 * Item-level displayCondition is editable → not a gap (use braces only).
 * Embedded itemization column displayCondition remains a gap.
 */
export function formItemHasPreservedGap(item: FormItem): boolean {
  if (item.type !== "text") return false;
  const content = item.content;
  if (Array.isArray(content)) {
    if (contentHasColumnDisplayCondition(content)) return true;
    // Belt-and-suspenders: nested columns / URI payloads some walks miss.
    return JSON.stringify(content).includes('"displayCondition"');
  }
  if (typeof content === "string") {
    return contentHasColumnDisplayCondition(content);
  }
  return false;
}

/** Form Text items that still embed uneditable column visibility (Explorer drill-down). */
export function formItemsWithPreservedGaps(
  form: TawalaForm,
): { index: number; label: string }[] {
  const out: { index: number; label: string }[] = [];
  (form.items ?? []).forEach((item, index) => {
    if (formItemHasPreservedGap(item)) {
      out.push({ index, label: String(item.label ?? `Item ${index + 1}`) });
    }
  });
  return out;
}

export function formHasPreservedGaps(form: TawalaForm): boolean {
  return (form.items ?? []).some((item) => formItemHasPreservedGap(item));
}

export function documentHasPreservedGaps(doc: TawalaDocument): boolean {
  return contentHasColumnDisplayCondition(doc.content);
}

export function projectFormNamesWithGaps(project: TawalaProject): string[] {
  return (project.forms ?? []).filter(formHasPreservedGaps).map((f) => f.name);
}

export function projectDocumentNamesWithGaps(project: TawalaProject): string[] {
  return (project.documents ?? []).filter(documentHasPreservedGaps).map((d) => d.name);
}

export type PreservedGapSummary = {
  /** Item-level conditions (editable) — informational only; not counted in totalMarkers. */
  itemDisplayConditions: number;
  /** Uneditable column / embed conditions — real preserved gaps. */
  columnDisplayConditions: number;
  formsAffected: number;
  documentsAffected: number;
  /** Uneditable gap cues only (column markers). */
  totalMarkers: number;
};

/** Count gap markers still present in JSON (for status / reconversion checks). */
export function summarizePreservedGaps(project: TawalaProject): PreservedGapSummary {
  let itemDisplayConditions = 0;
  let columnDisplayConditions = 0;
  let formsAffected = 0;
  let documentsAffected = 0;

  for (const form of project.forms ?? []) {
    let gapHit = false;
    for (const item of form.items ?? []) {
      if (formItemHasDisplayCondition(item)) {
        itemDisplayConditions += 1;
      }
      if (item.type === "text" && Array.isArray(item.content)) {
        const n = countColumnDisplayConditions(item.content);
        if (n > 0) {
          columnDisplayConditions += n;
          gapHit = true;
        }
      }
    }
    if (gapHit) formsAffected += 1;
  }

  for (const doc of project.documents ?? []) {
    if (typeof doc.content === "string") {
      const matches = doc.content.match(/"displayCondition"\s*:/g);
      if (matches?.length) {
        columnDisplayConditions += matches.length;
        documentsAffected += 1;
      } else if (contentHasColumnDisplayCondition(doc.content)) {
        documentsAffected += 1;
      }
    } else if (Array.isArray(doc.content)) {
      const n = countColumnDisplayConditions(doc.content);
      if (n > 0) {
        columnDisplayConditions += n;
        documentsAffected += 1;
      }
    }
  }

  return {
    itemDisplayConditions,
    columnDisplayConditions,
    formsAffected,
    documentsAffected,
    totalMarkers: columnDisplayConditions,
  };
}

function countColumnDisplayConditions(nodes: unknown): number {
  let count = 0;
  if (!nodes || typeof nodes !== "object") return 0;
  if (Array.isArray(nodes)) {
    for (const n of nodes) count += countColumnDisplayConditions(n);
    return count;
  }
  const n = nodes as Record<string, unknown>;
  if (n.type === "itemizationTable" && Array.isArray(n.columns)) {
    for (const c of n.columns as { displayCondition?: unknown }[]) {
      if (hasDisplayCondition(c?.displayCondition)) count += 1;
    }
  }
  if (Array.isArray(n.nodes)) count += countColumnDisplayConditions(n.nodes);
  if (Array.isArray(n.content)) count += countColumnDisplayConditions(n.content);
  if (Array.isArray(n.rows)) {
    for (const row of n.rows as { cells?: { content?: unknown }[] }[]) {
      for (const cell of row.cells ?? []) {
        count += countColumnDisplayConditions(cell.content);
      }
    }
  }
  return count;
}

/** Columns from Configure Function / itemization that still carry displayCondition. */
export function columnsWithDisplayCondition<T extends { displayCondition?: unknown }>(
  columns: T[] | undefined,
): T[] {
  return (columns ?? []).filter((c) => hasDisplayCondition(c.displayCondition));
}

/**
 * Annotate existing Design HTML so function chips that embed `displayCondition`
 * in data attributes get the warning chrome (Documents / older Form HTML).
 */
export function decoratePreservedWarningChipsHtml(html: string): string {
  if (!html || !html.includes("displayCondition")) return html;
  // Match function / structured chips; add class + title hint when payload has displayCondition.
  return html.replace(
    /<span\b([^>]*\b(?:function-table-token|function-token|data-tawala-structured-node|data-function-config|data-itemization-token)[^>]*)>/gi,
    (full, attrs: string) => {
      if (!/displayCondition/i.test(attrs) && !/displayCondition/i.test(decodeUriLoose(attrs))) {
        // Attribute may be URI-encoded; check decoded attrs blob.
        if (!attrs.includes("displayCondition") && !/%22displayCondition%22/i.test(attrs)) {
          return full;
        }
      }
      if (/\bpreserved-import-warning\b/.test(attrs)) return full;
      let next = attrs;
      if (/\bclass\s*=\s*"/i.test(next)) {
        next = next.replace(/\bclass\s*=\s*"/i, 'class="preserved-import-warning ');
      } else {
        next += ' class="preserved-import-warning"';
      }
      if (!/\btitle\s*=/i.test(next)) {
        next += ` title="${PRESERVED_FUNCTION_TOOLTIP.replace(/"/g, "&quot;")}"`;
      }
      return `<span${next}>`;
    },
  );
}

function decodeUriLoose(s: string): string {
  try {
    return decodeURIComponent(s);
  } catch {
    return s;
  }
}
