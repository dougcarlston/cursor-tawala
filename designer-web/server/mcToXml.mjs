import { tabPositionsXmlFromInches } from "./tabPositionsXml.mjs";

const TAB_MC_DEFAULT = '<tabPositions><tabStop position="2880"/></tabPositions>';

function templateToField(expr) {
  const m = String(expr ?? "").match(/<<([^>]+)>>/);
  return m ? m[1] : String(expr ?? "").trim();
}

/**
 * Dynamic MCQ expressions evaluate inside a record mapping.
 * Palette drops often look like `Form:Field`; Java needs `Record:Form:Field`
 * or display/value come back blank and choices are skipped.
 */
function normalizeDynamicMcqFieldName(field, sourceForm) {
  const raw = String(field ?? "").trim();
  if (!raw) return "";
  if (/^Record:/i.test(raw)) {
    return raw.startsWith("Record:") ? raw : `Record:${raw.slice(raw.indexOf(":") + 1)}`;
  }
  if (raw.includes(":")) return `Record:${raw}`;
  if (sourceForm) return `Record:${sourceForm}:${raw}`;
  return `Record:${raw}`;
}

function expressionFieldXml(expr, sourceForm) {
  const field = normalizeDynamicMcqFieldName(templateToField(expr), sourceForm);
  if (!field) return "";
  return `<field name="${field}"/>\n`;
}

function normalizeXmlConditionOp(op) {
  const raw = String(op ?? "equals").trim() || "equals";
  const fromLabel = {
    "does not equal": "doesNotEqual",
    "does not contain": "doesNotContain",
    "begins with": "beginsWith",
    "ends with": "endsWith",
    "is less than": "isLessThan",
    "is less than or equal to": "isLessThanOrEqualTo",
    "is greater than": "isGreaterThan",
    "is greater than or equal to": "isGreaterThanOrEqualTo",
    "is blank": "isBlank",
    "is not blank": "isNotBlank",
  };
  return fromLabel[raw.toLowerCase()] ?? raw;
}

function nestConditionOps(parts, combinator) {
  if (parts.length === 0) return "";
  if (parts.length === 1) return parts[0];
  let nested = parts[parts.length - 1];
  for (let i = parts.length - 2; i >= 0; i--) {
    nested = `<${combinator}>${parts[i]}${nested}</${combinator}>`;
  }
  return nested;
}

/** Emit record-selector form + optional Configure Function condition rows. */
function recordSelectorXml(choice, escAttr) {
  const sourceForm = choice.sourceForm ?? "";
  const formTag = sourceForm ? `<form name="${escAttr(sourceForm)}" />` : "";
  const rows = Array.isArray(choice.conditionsRows) ? choice.conditionsRows : [];
  const filled = rows.filter((r) => String(r?.field ?? "").trim());
  if (!filled.length) {
    return `<record-selector>${formTag}</record-selector>`;
  }
  const combinator = choice.conditionsCombinator === "or" ? "or" : "and";
  const parts = filled.map((row) => {
    const op = normalizeXmlConditionOp(row.op);
    const field = escAttr(
      normalizeDynamicMcqFieldName(String(row.field ?? "").trim(), sourceForm),
    );
    if (op === "isBlank" || op === "isNotBlank" || op === "mcIsBlank" || op === "mcIsNotBlank") {
      return `<${op} field="${field}"/>`;
    }
    return `<${op} field="${field}"><string value="${escAttr(row.value ?? "")}"/></${op}>`;
  });
  const body = nestConditionOps(parts, combinator);
  return `<record-selector>${formTag}<conditions>${body}</conditions></record-selector>`;
}

function dynamicMcBody(choice, escAttr) {
  const sourceForm = choice.sourceForm ?? "";
  const displayXml = expressionFieldXml(choice.displayExpr, sourceForm);
  const valueXml = expressionFieldXml(choice.valueExpr, sourceForm);
  const sortXml = expressionFieldXml(choice.sortExpr, sourceForm);
  return (
    `<data-provider><dynamic-mcq version="1">` +
    `<display-expression>${displayXml}</display-expression>` +
    `<value-expression>${valueXml}</value-expression>` +
    `<sort-expression>${sortXml}</sort-expression>` +
    `${recordSelectorXml(choice, escAttr)}` +
    `</dynamic-mcq></data-provider>`
  );
}

function fontXml(text, escText, { bold = false, italic = false } = {}) {
  let inner = escText(text);
  if (italic) inner = `<i>${inner}</i>`;
  if (bold) inner = `<b>${inner}</b>`;
  return `<font face="Arial" size="200" color="000000">${inner}</font>`;
}

/** MCQ question may be canvas-inline HTML; export/runtime use plain text (formatting parity TBD). */
function questionPlainText(question) {
  return String(question ?? "").replace(/<[^>]+>/g, "");
}

function tabsFor(item) {
  return tabPositionsXmlFromInches(item.tabPositions, TAB_MC_DEFAULT);
}

function questionParagraph(question, escText, tabsXml) {
  const q = questionPlainText(question);
  const italicMatch = q.match(/^(.*?)(\([^)]+\))\s*$/);
  if (italicMatch) {
    const main = italicMatch[1].trim();
    const note = italicMatch[2];
    const mainPart = main ? fontXml(`${main} `, escText) : "";
    return `<paragraph indent="0" align="left">${tabsXml}${mainPart}${fontXml(note, escText, { italic: true })}</paragraph>`;
  }
  if (!q) {
    return `<paragraph indent="0" align="left">${tabsXml}</paragraph>`;
  }
  return `<paragraph indent="0" align="left">${tabsXml}${fontXml(q, escText, { bold: true })}</paragraph>`;
}

function staticChoicesXml(choices, escAttr, escText, tabsXml) {
  return choices
    .map((c, i) => {
      const label = c.label ?? c.name ?? String.fromCharCode(97 + i);
      return `<choice label="${escAttr(label)}"><paragraph indent="0" align="left">${tabsXml}${fontXml(c.text ?? "", escText)}</paragraph></choice>`;
    })
    .join("");
}

/**
 * Prefer explicit Styles-dialog `style` (vertical / horizontal / multicolumn).
 * Legacy signup-sheets used `displayAs: "radio"` to mean horizontal layout — only
 * apply that when `style` is absent. Dynamic choices default to multicolumn likewise.
 */
function mcStyleAttr(item) {
  if (item.style) return item.style;
  const dynamic = (item.choices ?? []).some((c) => c.type === "dynamic");
  if (dynamic) return "multicolumn";
  if (item.displayAs === "radio") return "horizontal";
  return "";
}

/** Convert JSON multiple-choice item → legacy Java <mc> XML. */
export function mcToXml(item, escAttr, escText) {
  const choices = item.choices ?? [];
  const dynamic = choices.find((c) => c.type === "dynamic");
  const altLabel = item.name ? ` alternateLabel="${escAttr(item.name)}"` : "";
  const style = mcStyleAttr(item);
  const styleAttr = style ? ` style="${escAttr(style)}"` : "";
  const colAttr =
    style === "multicolumn" && typeof item.columnCount === "number" && item.columnCount > 0
      ? ` columnCount="${escAttr(String(item.columnCount))}"`
      : "";
  const padAttr = item.paddingBottom === false ? ` paddingBottom="false"` : "";
  const tabsXml = tabsFor(item);
  const body = dynamic
    ? dynamicMcBody(dynamic, escAttr)
    : staticChoicesXml(choices, escAttr, escText, tabsXml);

  return `<mc label="${escAttr(item.label)}"${altLabel} onlyone="${item.onlyone !== false ? "true" : "false"}" required="${item.required ? "true" : "false"}"${styleAttr}${colAttr}${padAttr}><question>${questionParagraph(item.question, escText, tabsXml)}</question>${body}</mc>`;
}
