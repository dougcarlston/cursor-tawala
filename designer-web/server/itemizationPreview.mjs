/**
 * Shared MULTIPLE QUESTION LIST (itemization-table) HTML for Form/Document Preview.
 * Used by runtime.mjs (structured nodes) and richHtmlPreview.mjs (function-token spans).
 */

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

export function parseRecordField(field) {
  let raw = String(field ?? "").trim();
  if (raw.startsWith("<<") && raw.endsWith(">>")) raw = raw.slice(2, -2).trim();
  const parts = raw.split(":");
  if (parts[0] === "Record" && parts.length >= 3) {
    return { form: parts[1], name: parts.slice(2).join(":") };
  }
  if (parts.length >= 2) {
    return { form: parts[0], name: parts.slice(1).join(":") };
  }
  return { form: null, name: raw };
}

export function recordCellValue(row, ref, defaultForm, aliases = {}) {
  const name = ref.name;
  if (!name) return "";
  const mapped = aliases[name];
  const keys = mapped && mapped !== name ? [name, mapped] : [name];
  for (const key of keys) {
    if (row[key] != null && row[key] !== "") return row[key];
    const form = ref.form ?? defaultForm;
    if (form) {
      const qualified = `${form}:${key}`;
      if (row[qualified] != null && row[qualified] !== "") return row[qualified];
    }
  }
  return "";
}

/**
 * Map alternateLabel / displayLabel / blank.name → blank.name for MQL column lookups
 * (`<<Form 1:First>>` when the blank is named `a` with alternateLabel `First`).
 */
export function blankAliasesFromForm(form) {
  /** @type {Record<string, string>} */
  const aliases = {};
  for (const item of form?.items ?? []) {
    if (item.type !== "fib") continue;
    for (const blank of item.blanks ?? []) {
      const name = String(blank.name ?? "").trim();
      if (!name) continue;
      aliases[name] = name;
      for (const label of [blank.alternateLabel, blank.displayLabel]) {
        const alt = String(label ?? "").trim();
        if (alt) aliases[alt] = name;
      }
    }
  }
  return aliases;
}

function formFromFieldRef(raw) {
  const ref = parseRecordField(raw);
  return ref.form || "";
}

/** Infer source form from function config / structured attrs. */
export function inferItemizationForm(config, attrs = "") {
  const explicit = String(config["form-name"] ?? config.form ?? "").trim();
  if (explicit) return explicit;

  const formAttr = String(attrs).match(/\bdata-itemization-form\s*=\s*(["'])([\s\S]*?)\1/i);
  if (formAttr?.[2]) return formAttr[2].trim();

  const cols = config.column ?? config.columns ?? [];
  for (const col of cols) {
    const form = formFromFieldRef(col.contents ?? col.field ?? "");
    if (form) return form;
  }

  if (Array.isArray(config.conditionsRows)) {
    for (const row of config.conditionsRows) {
      const form = formFromFieldRef(row?.field);
      if (form) return form;
    }
  }
  return "";
}

function conditionRowsFromConfig(config) {
  const rows = Array.isArray(config.conditionsRows) ? config.conditionsRows : [];
  return rows
    .map((r) => ({
      field: String(r?.field ?? "").trim(),
      op: String(r?.op ?? "equals").trim() || "equals",
      value: String(r?.value ?? "").trim(),
    }))
    .filter((r) => r.field);
}

function rowMatchesConditions(row, conditions, combinator, sourceForm, aliases = {}) {
  if (!conditions?.length) return true;
  const check = (cond) => {
    const ref = parseRecordField(cond.field);
    const left = String(recordCellValue(row, ref, sourceForm, aliases) ?? "");
    const op = cond.op;
    if (op === "is blank" || op === "blank") return left.trim() === "";
    if (op === "is not blank" || op === "not blank") return left.trim() !== "";
    const right = cond.value;
    if (op === "contains") return left.toLowerCase().includes(right.toLowerCase());
    if (op === "does not equal" || op === "not equals") return left !== right;
    return left === right; // equals (default)
  };
  return combinator === "or" ? conditions.some(check) : conditions.every(check);
}

function truthyFlag(v) {
  return v === true || v === "true" || v === "yes" || v === 1 || v === "1";
}

/**
 * Normalize designer function config or structured node into
 * `{ form, columns, conditions, combinator, showPrint, showExport }` for rendering.
 */
export function itemizationNodeFromConfig(config, attrs = "", fallbackForm = "") {
  const colsRaw = config.column ?? config.columns ?? [];
  const n = Number(config.numberOfColumns ?? colsRaw.length) || colsRaw.length;
  const columns = colsRaw.slice(0, Math.max(n, colsRaw.length)).map((col) => ({
    header: String(col.header ?? "").trim() || String(col.contents ?? col.field ?? "").trim() || "Column",
    field: String(col.contents ?? col.field ?? "").trim(),
  }));
  const form = inferItemizationForm(config, attrs) || fallbackForm || "";
  return {
    form,
    columns,
    conditions: conditionRowsFromConfig(config),
    combinator: config.conditionsCombinator === "or" ? "or" : "and",
    showPrint: truthyFlag(config["show-print-control"] ?? config.showPrint),
    showExport: truthyFlag(config["show-export-control"] ?? config.showExport),
  };
}

/** HTML facsimile of legacy itemization (MULTIPLE QUESTION LIST) tables. */
export function renderItemizationTableHtml(node, ctx = {}) {
  const columns = node.columns ?? [];
  if (columns.length === 0) {
    return `<div class="preview-itemization-table"><em>Multiple Question List</em> (no columns configured)</div>`;
  }

  const sourceForm = node.form ?? ctx.formName ?? "";
  const aliases = ctx.blankAliases ?? {};
  const all = (sourceForm ? ctx.records?.[sourceForm] : null) ?? [];
  const records = all.filter((row) =>
    rowMatchesConditions(row, node.conditions, node.combinator ?? "and", sourceForm, aliases),
  );
  const headerCells = columns.map((c) => `<th>${esc(c.header)}</th>`).join("");

  const bodyRows =
    records.length === 0
      ? ""
      : records
          .map((row, i) => {
            const cls = i % 2 === 0 ? "even" : "odd";
            const cells = columns
              .map((col) => {
                const ref = parseRecordField(col.field);
                const val = recordCellValue(row, ref, sourceForm, aliases);
                return `<td>${esc(val)}</td>`;
              })
              .join("");
            return `<tr class="${cls}">${cells}</tr>`;
          })
          .join("\n");

  const fixWidth = columns.length > 3;
  const containerClass = fixWidth
    ? ' class="tawalaDataTable dtFixTableWidth preview-itemization-table"'
    : ' class="preview-itemization-table"';

  const controls = [];
  if (truthyFlag(node.showPrint ?? node["show-print-control"])) {
    controls.push(
      `<a href="#" onclick="window.print();return false;">Print This List</a>`,
    );
  }
  if (truthyFlag(node.showExport ?? node["show-export-control"])) {
    // Preview facsimile — CSV download (legacy Excel needs Java export endpoint).
    controls.push(
      `<a href="#" onclick="(function(a){var t=a.closest('.preview-itemization-table')||a.parentElement.nextElementSibling;if(!t)return false;var table=t.querySelector?t.querySelector('table'):null;if(!table)table=t;var rows=[].slice.call(table.querySelectorAll('tr'));var csv=rows.map(function(tr){return[].slice.call(tr.querySelectorAll('th,td')).map(function(c){var s=String(c.textContent||'').replace(/\"/g,'\"\"');return '\"'+s+'\"';}).join(',');}).join('\\n');var blob=new Blob([csv],{type:'text/csv'});var u=URL.createObjectURL(blob);var l=document.createElement('a');l.href=u;l.download='signup-list.csv';l.click();URL.revokeObjectURL(u);return false;})(this);return false;">Export to Excel</a>`,
    );
  }
  const controlsHtml = controls.length
    ? `<p class="preview-itemization-controls">${controls.join("")}</p>`
    : "";

  return `<div${containerClass}>${controlsHtml}<table class="component outline sortable stripe">
<thead><tr>${headerCells}</tr></thead>
<tbody>${bodyRows}</tbody>
</table></div>`;
}
