/**
 * RESPONSE TOTALS (`response-totals-table`) Preview HTML — Choice/Count table with
 * the MCQ question text as a title above each table.
 */

import { parseRecordField, recordCellValue } from "./itemizationPreview.mjs";
import { findMcqItem } from "./choiceTallyPreview.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function mcqQuestionPlain(item) {
  return String(item?.question ?? "")
    .replace(/\u200b/g, "")
    .replace(/<br\s*\/?>/gi, " ")
    .replace(/<[^>]+>/g, "")
    .replace(/&nbsp;/gi, " ")
    .replace(/&amp;/g, "&")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/\s+/g, " ")
    .trim();
}

function normalizeChoiceValues(raw) {
  if (raw == null || raw === "") return [];
  if (Array.isArray(raw)) return raw.map((v) => String(v));
  const s = String(raw);
  if (s.includes(",") && !s.includes("<<")) {
    return s.split(",").map((p) => p.trim()).filter(Boolean);
  }
  return [s];
}

/**
 * @param {{ field?: string, form?: string, layout?: string }} node
 * @param {{ records?: Record<string, object[]>, project?: object, formName?: string }} ctx
 */
export function renderResponseTotalsTableHtml(node, ctx = {}) {
  const ref = parseRecordField(node.field ?? "");
  const formName = ref.form || node.form || ctx.formName || "";
  const fieldName = ref.name || "";
  const rows = formName ? (ctx.records?.[formName] ?? []) : [];
  const mcq = findMcqItem(ctx.project, formName, fieldName);
  const title = mcqQuestionPlain(mcq);
  const choices = (mcq?.choices ?? []).map((c) => ({
    id: String(c.name ?? c.id ?? "").trim(),
    text: String(c.text ?? c.label ?? c.name ?? "").trim(),
  })).filter((c) => c.id || c.text);

  const counts = new Map();
  for (const ch of choices) counts.set(ch.id || ch.text, 0);

  for (const row of rows) {
    const raw = recordCellValue(row, ref, formName);
    for (const val of normalizeChoiceValues(raw)) {
      const key = String(val).trim();
      // Match by choice name/id or by displayed text.
      let matched = false;
      for (const ch of choices) {
        if (ch.id === key || ch.text === key) {
          counts.set(ch.id || ch.text, (counts.get(ch.id || ch.text) ?? 0) + 1);
          matched = true;
          break;
        }
      }
      if (!matched && key) {
        counts.set(key, (counts.get(key) ?? 0) + 1);
      }
    }
  }

  const bodyRows = [];
  if (choices.length) {
    for (const ch of choices) {
      const key = ch.id || ch.text;
      const n = counts.get(key) ?? 0;
      bodyRows.push(
        `<tr class="odd"><td>${esc(ch.text || ch.id)}</td><td>${n}</td></tr>`,
      );
    }
  } else if (counts.size) {
    let i = 0;
    for (const [label, n] of counts) {
      const cls = i++ % 2 === 0 ? "odd" : "even";
      bodyRows.push(`<tr class="${cls}"><td>${esc(label)}</td><td>${n}</td></tr>`);
    }
  }

  const titleHtml = title
    ? `<div class="response-totals-title"><strong>${esc(title)}</strong></div>`
    : "";
  const choiceHeader = title ? esc(title) : "Choice";
  const tableBody =
    bodyRows.length > 0
      ? bodyRows.join("")
      : `<tr class="even"><td colspan="2"><em>There were no responses to this question.</em></td></tr>`;

  // Tall (vertical): question text is the first-column header so it stays attached to
  // the stacked choices. Wide (horizontal): keep Choice/Count row labels + title above.
  const layout = String(node.layout ?? "vertical").toLowerCase();
  if (layout === "horizontal") {
    const labels = choices.length
      ? choices.map((ch) => `<td>${esc(ch.text || ch.id)}</td>`).join("")
      : `<td colspan="1"><em>There were no responses to this question.</em></td>`;
    const countCells = choices.length
      ? choices
          .map((ch) => {
            const key = ch.id || ch.text;
            return `<td>${counts.get(key) ?? 0}</td>`;
          })
          .join("")
      : "";
    return (
      `<div class="preview-response-totals">` +
      titleHtml +
      `<table class="component outline sortable stripe"><tbody>` +
      `<tr class="odd"><th class="leftHeading"> Choice </th>${labels}</tr>` +
      (choices.length
        ? `<tr class="even"><th class="leftHeading"> Count </th>${countCells}</tr>`
        : "") +
      `</tbody></table></div>`
    );
  }

  return (
    `<div class="preview-response-totals">` +
    titleHtml +
    `<table class="component outline sortable stripe">` +
    `<thead><tr><th> ${choiceHeader} </th><th> Count </th></tr></thead>` +
    `<tbody>${tableBody}</tbody></table></div>`
  );
}

export function responseTotalsNodeFromConfig(config, fallbackForm = "") {
  const field = String(config.field ?? "").trim();
  const ref = parseRecordField(field);
  return {
    type: "responseTotalsTable",
    field: field || (ref.name ? `Record:${ref.form || fallbackForm}:${ref.name}` : ""),
    form: ref.form || String(config.form ?? fallbackForm).trim(),
    layout: String(config["layout-type"] ?? "vertical"),
  };
}
