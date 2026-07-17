import { tabPositionsXmlFromInches } from "./tabPositionsXml.mjs";

const TAB_MC_DEFAULT = '<tabPositions><tabStop position="2880"/></tabPositions>';

function templateToField(expr) {
  const m = String(expr ?? "").match(/<<([^>]+)>>/);
  return m ? m[1] : String(expr ?? "");
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

function dynamicMcBody(choice) {
  const displayField = templateToField(choice.displayExpr);
  const valueField = templateToField(choice.valueExpr);
  const sourceForm = choice.sourceForm ?? "";
  return `<data-provider><dynamic-mcq version="1"><display-expression><field name="${displayField}"/>
</display-expression><value-expression><field name="${valueField}"/>
</value-expression><sort-expression></sort-expression><record-selector><form name="${sourceForm}" /></record-selector></dynamic-mcq></data-provider>`;
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
    ? dynamicMcBody(dynamic)
    : staticChoicesXml(choices, escAttr, escText, tabsXml);

  return `<mc label="${escAttr(item.label)}"${altLabel} onlyone="${item.onlyone !== false ? "true" : "false"}" required="${item.required ? "true" : "false"}"${styleAttr}${colAttr}${padAttr}><question>${questionParagraph(item.question, escText, tabsXml)}</question>${body}</mc>`;
}
