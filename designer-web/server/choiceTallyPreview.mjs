/**
 * RESPONSE BAR GRAPH (`choice-tally-table` / choiceTallyTable) Preview HTML.
 */

import { parseRecordField, recordCellValue } from "./itemizationPreview.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

/** Find MCQ item by label / alternateLabel / name on a form. */
export function findMcqItem(project, formName, fieldName) {
  const form = (project?.forms ?? []).find((f) => f.name === formName);
  if (!form) return null;
  const want = String(fieldName ?? "").trim();
  if (!want) return null;
  for (const item of form.items ?? []) {
    if (item.type !== "mc") continue;
    const keys = [item.label, item.alternateLabel, item.name, item.fieldName]
      .map((k) => String(k ?? "").trim())
      .filter(Boolean);
    if (keys.includes(want)) return item;
  }
  return null;
}

function normalizeChoiceValues(raw) {
  if (raw == null || raw === "") return [];
  if (Array.isArray(raw)) return raw.map((v) => String(v));
  const s = String(raw);
  // Multi-select sometimes stored as comma-separated.
  if (s.includes(",") && !s.includes("<<")) {
    return s.split(",").map((p) => p.trim()).filter(Boolean);
  }
  return [s];
}

/**
 * @param {{ field?: string, form?: string }} node
 * @param {{ records?: Record<string, object[]>, project?: object, formName?: string }} ctx
 */
export function renderChoiceTallyTableHtml(node, ctx = {}) {
  const ref = parseRecordField(node.field ?? "");
  const formName = ref.form || node.form || ctx.formName || "";
  const fieldName = ref.name || "";
  const rows = formName ? (ctx.records?.[formName] ?? []) : [];
  const mcq = findMcqItem(ctx.project, formName, fieldName);
  const choices = mcq?.choices ?? [];

  /** @type {Map<string, { label: string, count: number }>} */
  const tallies = new Map();
  for (const c of choices) {
    const id = String(c.name ?? c.id ?? "").trim();
    if (!id) continue;
    tallies.set(id, { label: String(c.text ?? c.label ?? id), count: 0 });
  }

  let total = 0;
  for (const row of rows) {
    const raw = recordCellValue(row, { form: formName, name: fieldName }, formName, {});
    for (const val of normalizeChoiceValues(raw)) {
      total += 1;
      const existing = tallies.get(val);
      if (existing) existing.count += 1;
      else tallies.set(val, { label: val, count: 1 });
    }
  }

  if (total === 0 && tallies.size === 0) {
    return `<div class="preview-choice-tally"><em>Response bar graph</em> (no responses yet)</div>`;
  }

  const bodyRows = [];
  let color = 1;
  for (const [, entry] of tallies) {
    const pct = total > 0 ? Math.round((entry.count * 100) / total) : 0;
    const oddEven = color % 2 === 1 ? "odd" : "even";
    bodyRows.push(
      `<tr class="${oddEven}"><td>${esc(entry.label)}</td>` +
        `<td>${entry.count}</td>` +
        `<td><div class="graph color${color}"><strong class="bar" style="width: ${pct}%"><span>${pct}%</span></strong></div></td></tr>`,
    );
    color += 1;
    if (color > 16) color = 1;
  }

  if (bodyRows.length === 0) {
    return `<div class="preview-choice-tally"><em>Response bar graph</em> (no choices configured)</div>`;
  }

  return (
    `<div class="preview-choice-tally"><table class="component outline sortable stripe">` +
    `<thead><tr><th> Choice </th><th> Count </th><th> Percentage </th></tr></thead>` +
    `<tbody>${bodyRows.join("")}</tbody></table></div>`
  );
}

/** Build a structured-like node from function-token config. */
export function choiceTallyNodeFromConfig(config, fallbackForm = "") {
  const field = String(config.field ?? "").trim();
  const ref = parseRecordField(field);
  return {
    type: "choiceTallyTable",
    field: field || (ref.name ? `Record:${ref.form || fallbackForm}:${ref.name}` : ""),
    form: ref.form || String(config.form ?? fallbackForm).trim(),
  };
}
