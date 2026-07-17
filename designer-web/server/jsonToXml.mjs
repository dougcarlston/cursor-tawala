/** Best-effort JSON format 2.0 → legacy XML for Java /client upload. */

import { fibToXml } from "./fibToXml.mjs";
import { registrationFibToXml } from "./registrationFibToXml.mjs";
import { registrationTextToXml } from "./registrationTextToXml.mjs";
import { mcToXml } from "./mcToXml.mjs";
import { documentHtmlToXml } from "./documentHtmlToXml.mjs";

const TAB_MC = '<tabPositions><tabStop position="2880"/></tabPositions>';

function escAttr(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;");
}

function escText(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;");
}

/**
 * Heading content may carry inline per-run size markup (`<span class="heading-size-*">`).
 * The legacy `<heading>` XML has a single whole-item `type`, so per-run sizes can't be
 * expressed here — reduce to plain text (decoding the entities the editor introduced) so we
 * emit the heading text, never escaped `<span>` tags. Full per-run parity is deferred.
 */
function headingPlainText(content) {
  return String(content ?? "")
    .replace(/\u200b/g, "")
    .replace(/<[^>]*>/g, "")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&");
}

/** JSON `{ field, op, value }` → legacy `<displayConditions>` (inside form items). */
function displayConditionsXml(cond) {
  if (!cond?.field) return "";
  const field = escAttr(cond.field);
  const op = cond.op ?? "equals";
  if (op === "isBlank" || op === "isNotBlank") {
    return `<displayConditions><${op} field="${field}"/></displayConditions>`;
  }
  return (
    `<displayConditions>` +
    `<${op} field="${field}">${conditionValueXml(cond.value)}</${op}>` +
    `</displayConditions>`
  );
}

/** Column-level conditions use hyphenated tag in legacy itemization XML. */
function columnDisplayConditionsXml(cond) {
  if (!cond?.field) return "";
  const field = escAttr(cond.field);
  const op = cond.op ?? "equals";
  if (op === "isBlank" || op === "isNotBlank") {
    return `<display-conditions><${op} field="${field}"/></display-conditions>`;
  }
  return (
    `<display-conditions>` +
    `<${op} field="${field}">${conditionValueXml(cond.value)}</${op}>` +
    `</display-conditions>`
  );
}

function itemizationHeaderXml(header) {
  const h = String(header ?? "");
  const m = h.trim().match(/^<<([^<>]+)>>$/);
  if (m) {
    return `<header><field name="${escAttr(m[1].trim())}"/></header>`;
  }
  return `<header><string value="${escText(h)}"/></header>`;
}

/** Legacy Signup Sheet cells use Record:Form:Field — not <<Form:Field>>. */
function itemizationContentsFieldName(raw) {
  let s = String(raw ?? "").trim();
  if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
  if (!s) return "";
  if (/^Record:/i.test(s)) return s;
  const colon = s.indexOf(":");
  if (colon > 0) {
    return `Record:${s.slice(0, colon).trim()}:${s.slice(colon + 1).trim()}`;
  }
  return s;
}

/**
 * Record-list table used on Form Text and Documents.
 * Legacy shape: forms + optional nested &lt;conditions&gt; filter (e.g. SheetChosen).
 */
function itemizationTableToXml(n) {
  const cols = (n.columns ?? [])
    .map((col) => {
      const field = itemizationContentsFieldName(col.field ?? col.contents ?? "");
      return (
        `<column>${itemizationHeaderXml(col.header)}` +
        `<contents><field name="${escAttr(field)}"/></contents>` +
        `${columnDisplayConditionsXml(col.displayCondition ?? col.displayConditions)}` +
        `</column>`
      );
    })
    .join("");

  const formNames = new Set();
  if (Array.isArray(n.forms)) {
    for (const f of n.forms) if (f) formNames.add(f);
  } else if (n.form) {
    formNames.add(n.form);
  }
  // Column DCs often reference Setup:… — include that form like legacy PlayerData.
  for (const col of n.columns ?? []) {
    const dc = col.displayCondition ?? col.displayConditions;
    const f = dc?.field;
    if (typeof f === "string" && f.includes(":")) {
      const bare = f.replace(/^<<|>>$/g, "").replace(/^Record:/i, "");
      formNames.add(bare.split(":")[0]);
    }
    const cell = String(col.field ?? col.contents ?? "").replace(/^<<|>>$/g, "");
    if (cell.includes(":")) {
      const bare = cell.replace(/^Record:/i, "");
      formNames.add(bare.split(":")[0]);
    }
  }

  const formTags = [...formNames]
    .map((name) => `<form name="${escAttr(name)}"/>`)
    .join("");
  const filter = n.where ? `<conditions>${conditionToXml(n.where)}</conditions>` : "";
  const nCols = (n.columns ?? []).length;
  const showPrint = n.showPrint === true || n["show-print-control"] === true || n["show-print-control"] === "true";
  const showExport =
    n.showExport === true || n["show-export-control"] === true || n["show-export-control"] === "true";
  return (
    `<itemization-table version="2">` +
    `<show-print-control>${showPrint ? "true" : "false"}</show-print-control>` +
    `<show-export-control>${showExport ? "true" : "false"}</show-export-control>` +
    `<number-of-columns>${nCols}</number-of-columns>${cols}` +
    `<conditions>${formTags}${filter}</conditions>` +
    `</itemization-table>`
  );
}

function withDisplayConditions(itemXml, item) {
  const dc = displayConditionsXml(item.displayCondition ?? item.displayConditions);
  if (!dc) return itemXml;
  // Insert before the item's closing tag (fib/mc/text/…).
  return itemXml.replace(/<\/([a-zA-Z0-9_-]+)>\s*$/, `${dc}</$1>`);
}

function fontXml(text, { bold = false, italic = false, size = 200 } = {}) {
  let inner = escText(text);
  if (italic) inner = `<i>${inner}</i>`;
  if (bold) inner = `<b>${inner}</b>`;
  return `<font face="Arial" size="${size}" color="000000">${inner}</font>`;
}

function textContentToXml(content, style, project = null) {
  if (typeof content === "string") {
    if (!content) {
      return `<paragraph indent="0" align="left">${TAB_MC}</paragraph>`;
    }
    // Canvas Text items store WYSIWYG HTML (field/function tokens). Escape-as-text
    // would Deploy the token chrome instead of `<display-image>` / `<display-mcq-label>`.
    if (/<[a-z][\s\S]*>/i.test(content)) {
      const xml = documentHtmlToXml(content, escAttr, escText);
      return injectResponseTotalsQuestionTitles(xml, project);
    }
    const italic = style === "instructional";
    return `<paragraph indent="0" align="left">${TAB_MC}${fontXml(content, { italic })}</paragraph>`;
  }
  return richContentToXml(content);
}

/**
 * Plain MCQ question text for a totals table field ref (`Form:Label` or `Record:Form:Label`).
 */
export function lookupMcqQuestionPlain(project, fieldRef) {
  if (!project || !fieldRef) return "";
  const cleaned = String(fieldRef)
    .replace(/^<<|>>$/g, "")
    .replace(/^Record:/i, "")
    .trim();
  const parts = cleaned.split(":");
  if (parts.length < 2) return "";
  const formName = parts[0].trim();
  const fieldId = parts.slice(1).join(":").trim();
  const form = (project.forms ?? []).find((f) => f.name === formName);
  if (!form) return "";
  const item = (form.items ?? []).find(
    (i) =>
      i.type === "mc" &&
      (i.label === fieldId || i.name === fieldId || i.alternateLabel === fieldId),
  );
  if (!item) return "";
  return String(item.question ?? "")
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

/**
 * Before each `<response-totals-table>`, emit a bold paragraph with the MCQ question
 * text, and keep a blank spacer paragraph after the table so Deploy gaps match Preview
 * (Tomcat `table.component` historically has no margin; CSS patches need image rebuild).
 *
 * Several chips often share one `<paragraph>` (inserted without Enter). Split those
 * so every Tall/Wide table gets its own title — a "lone table in paragraph" regex
 * misses shared paragraphs entirely.
 */
export function injectResponseTotalsQuestionTitles(xml, project) {
  if (!xml || !project) return xml;

  const wrapTable = (tableXml) => {
    const fieldM = tableXml.match(/<field>([^<]*)<\/field>/i);
    const title = lookupMcqQuestionPlain(project, fieldM?.[1] ?? "");
    const titlePara = title
      ? `<paragraph indent="0" align="left"><font face="Arial" size="200" color="000000"><b>${escText(title)}</b></font></paragraph>`
      : "";
    const spacer = `<paragraph indent="0" align="left">${TAB_MC}</paragraph>`;
    return (
      `${titlePara}` +
      `<paragraph indent="0" align="left"><font>${tableXml}</font></paragraph>` +
      `${spacer}`
    );
  };

  return xml.replace(/<paragraph\b[^>]*>([\s\S]*?)<\/paragraph>/gi, (full, inner) => {
    if (!/<response-totals-table\b/i.test(inner)) return full;

    const parts = [];
    let last = 0;
    const re =
      /(?:<font\b[^>]*>)?(<response-totals-table\b[\s\S]*?<\/response-totals-table>)(?:<\/font>)?/gi;
    let m;
    while ((m = re.exec(inner))) {
      const before = inner.slice(last, m.index).trim();
      if (before && !/^<tabPositions\b[\s\S]*<\/tabPositions>$/i.test(before)) {
        parts.push(`<paragraph indent="0" align="left">${before}</paragraph>`);
      }
      parts.push(wrapTable(m[1]));
      last = m.index + m[0].length;
    }
    const after = inner.slice(last).trim();
    if (after && !/^<tabPositions\b[\s\S]*<\/tabPositions>$/i.test(after)) {
      parts.push(`<paragraph indent="0" align="left">${after}</paragraph>`);
    }
    return parts.join("") || full;
  });
}

function templateToFieldRef(value) {
  const s = String(value ?? "");
  const m = s.match(/^<<([^>]+)>>$/);
  if (m) return `<string field="${escAttr(m[1])}"/>`;
  return `<string value="${escAttr(s)}"/>`;
}

function valueToXml(value) {
  const s = String(value ?? "");
  const addMatch = s.match(/^<<([^>]+)>>\s*\+\s*(\d+)$/);
  if (addMatch) {
    return `<add><operand field="${escAttr(addMatch[1])}"/><operand value="${escAttr(addMatch[2])}"/></add>`;
  }
  if (!s.includes("<<")) return templateToFieldRef(s);
  return s
    .split(/(<<[^>]+>>)/)
    .filter((part) => part.length > 0)
    .map((part) => templateToFieldRef(part))
    .join("");
}

function setValueToXml(value, arithmeticAsText) {
  const body = valueToXml(value);
  if (arithmeticAsText === true) {
    return { body, attr: ' arithmeticAsText="true"' };
  }
  if (arithmeticAsText === false) {
    return { body, attr: ' arithmeticAsText="false"' };
  }
  const arithmetic =
    body.includes("<add>") || (body.match(/<string/g)?.length ?? 0) > 1;
  const attr = arithmetic ? ' arithmeticAsText="false"' : "";
  return { body, attr };
}

function conditionValueXml(value) {
  return valueToXml(value);
}

function conditionToXml(cond) {
  if (!cond) return "";
  if (Array.isArray(cond.or)) {
    return `<or>${cond.or.map(conditionToXml).join("")}</or>`;
  }
  if (Array.isArray(cond.and)) {
    return `<and>${cond.and.map(conditionToXml).join("")}</and>`;
  }
  if (cond.op === "and" || cond.op === "or") {
    const inner = (cond.conditions ?? []).map(conditionToXml).join("");
    return `<${cond.op}>${inner}</${cond.op}>`;
  }
  const op = cond.op ?? "equals";
  const unaryOps = new Set(["isBlank", "isNotBlank", "mcIsBlank", "mcIsNotBlank"]);
  if (unaryOps.has(op)) {
    return `<${op} field="${escAttr(cond.field)}"/>`;
  }
  if (cond.value === undefined || cond.value === null) {
    return `<${op} field="${escAttr(cond.field)}"><string value=""/></${op}>`;
  }
  return `<${op} field="${escAttr(cond.field)}">${conditionValueXml(cond.value)}</${op}>`;
}

function xmlCommentText(s) {
  return String(s ?? "")
    .replace(/--/g, "- -")
    .replace(/-$/g, "");
}

function addressToXml(tag, addr) {
  if (!addr) return "";
  if (Array.isArray(addr)) {
    return addr.map((a) => addressToXml(tag, a)).join("");
  }
  if (addr.fieldRef) {
    const aliasField = addr.aliasField ? ` aliasField="${escAttr(addr.aliasField)}"` : "";
    const aliasLiteral = addr.aliasLiteral ? ` aliasLiteral="${escAttr(addr.aliasLiteral)}"` : "";
    return `<${tag} addressField="${escAttr(addr.fieldRef)}"${aliasField}${aliasLiteral}/>`;
  }
  if (addr.literal != null) {
    return `<${tag} address="${escAttr(addr.literal)}"/>`;
  }
  return "";
}

const SEND_DOC_DEFAULTS = {
  PostRegistrationLetterWithoutPaymentInstructions: {
    to: "Registration:ParentEmail",
    from: { fieldRef: "AdminEmail", aliasField: "AdminName" },
  },
  PostRegistrationLetterWithPaymentInstructions: {
    to: "Registration:ParentEmail",
    from: { fieldRef: "AdminEmail", aliasField: "AdminName" },
  },
  AdminRegNotification: {
    to: "AdminEmail",
    from: { fieldRef: "AdminEmail", aliasLiteral: "Dirt Bowl Automated Email Server" },
  },
  InvitationToCompleteRegistration: {
    to: "AddPlayerToGroupRegistration:Parents' Email",
    from: { fieldRef: "AdminEmail", aliasField: "AdminName" },
  },
};

function inferSendAddresses(cmd, ctx = {}) {
  const doc = cmd.body?.document;
  const defaults = doc ? SEND_DOC_DEFAULTS[doc] : null;
  if (!defaults) return { to: cmd.to, from: cmd.from };
  const toField = ctx.emailTo ?? defaults.to;
  return {
    to: cmd.to ?? { fieldRef: toField },
    from: cmd.from ?? defaults.from,
  };
}

function sendToXml(cmd, ctx = {}) {
  const { to: toAddr, from: fromAddr } = inferSendAddresses(cmd, ctx);
  const to = addressToXml("to", toAddr);
  const from = addressToXml("from", fromAddr);
  const cc = addressToXml("cc", cmd.cc);
  const bcc = addressToXml("bcc", cmd.bcc);
  const subject = cmd.subject != null ? `<subject>${escText(cmd.subject)}</subject>` : "";
  const body = cmd.body?.document
    ? `<body document="${escAttr(cmd.body.document)}" reset="${cmd.body.reset === true ? "true" : "false"}" showHeader="${cmd.body.showHeader === false ? "false" : "true"}"/>`
    : "";
  if (!to && !from && !subject && !body) {
    return `<!-- incomplete send command -->`;
  }
  return `<send>${to}${from}${cc}${bcc}${subject}${body}</send>`;
}

function showToXml(cmd) {
  if (cmd.form) return `<show form="${escAttr(cmd.form)}"/>`;
  if (cmd.document) {
    const reset = cmd.reset === true ? "true" : "false";
    return `<show document="${escAttr(cmd.document)}" reset="${reset}"/>`;
  }
  if (cmd.url) {
    const parts = String(cmd.url).split(/(<<[^>]+>>)/).filter(Boolean);
    const urlXml = parts
      .map((part) => {
        const m = part.match(/^<<([^>]+)>>$/);
        if (m) return `<string field="${escAttr(m[1])}"/>`;
        return `<string value="${escAttr(part)}"/>`;
      })
      .join("");
    return `<show><url>${urlXml}</url></show>`;
  }
  return "<show/>";
}

function commandToXml(cmd, ctx = {}) {
  switch (cmd.cmd) {
    case "comment":
      return `<!-- ${xmlCommentText(cmd.text)} -->`;
    case "set": {
      const { body, attr } = setValueToXml(cmd.value, cmd.arithmeticAsText);
      return `<set field="${escAttr(cmd.field)}"${attr}>${body}</set>`;
    }
    case "get": {
      const forms = (cmd.sourceForms ?? [])
        .map((n) => `<form name="${escAttr(n)}"/>`)
        .join("");
      const where = cmd.where
        ? `<conditions>${conditionToXml(cmd.where)}</conditions>`
        : "";
      return `<get recordList="${escAttr(cmd.recordList)}"><forms>${forms}</forms>${where}</get>`;
    }
    case "if": {
      const childCtx =
        cmd.condition?.field === "Registration:ParentEmail2" &&
        cmd.condition?.op === "isNotBlank"
          ? { ...ctx, emailTo: "Registration:ParentEmail2" }
          : ctx;
      const thenXml = (cmd.then ?? []).map((c) => commandToXml(c, childCtx)).join("");
      const elseXml = (cmd.else ?? []).map((c) => commandToXml(c, childCtx)).join("");
      return `<if><conditions>${conditionToXml(cmd.condition)}</conditions><trueSet>${thenXml}</trueSet><falseSet>${elseXml}</falseSet></if>`;
    }
    case "skip":
      return `<skip to="${escAttr(cmd.to)}"/>`;
    case "foreach": {
      const body = (cmd.do ?? []).map((c) => commandToXml(c, ctx)).join("");
      return `<foreach record="${escAttr(cmd.recordName)}" recordList="${escAttr(cmd.recordList)}">${body}</foreach>`;
    }
    case "delete": {
      const where = cmd.where
        ? `<conditions>${conditionToXml(cmd.where)}</conditions>`
        : "";
      return `<delete><form name="${escAttr(cmd.form)}"/>${where}</delete>`;
    }
    case "show":
    case "showDocument":
      return showToXml(
        cmd.cmd === "showDocument"
          ? { document: cmd.document, reset: cmd.reset }
          : cmd,
      );
    case "edit": {
      const where = cmd.condition
        ? `<conditions>${conditionToXml(cmd.condition)}</conditions>`
        : "";
      const submit = cmd.submit === "new" ? "new" : "modify";
      return `<edit form="${escAttr(cmd.form)}" submit="${submit}">${where}</edit>`;
    }
    case "send":
      return sendToXml(cmd, ctx);
    default:
      return `<!-- unsupported command ${xmlCommentText(cmd.cmd)} -->`;
  }
}

function skipCommandsToXml(commands) {
  return (commands ?? []).map(commandToXml).join("");
}

function richContentToXml(content) {
  if (typeof content === "string") {
    return `<paragraph indent="0" align="left">${escText(content)}</paragraph>`;
  }
  if (!Array.isArray(content)) return "";
  return content
    .map((block) => {
      if (block.type === "paragraph") {
        return `<paragraph indent="0" align="${escAttr(block.align ?? "left")}">${richNodesToXml(block.nodes)}</paragraph>`;
      }
      return "";
    })
    .join("");
}

function richNodesToXml(nodes) {
  if (!nodes) return "";
  return nodes
    .map((n) => {
      switch (n.type) {
        case "text":
          return escText(n.text);
        case "bold":
          return `<b>${richNodesToXml(n.nodes)}</b>`;
        case "italic":
          return `<i>${richNodesToXml(n.nodes)}</i>`;
        case "underline":
          return `<u>${richNodesToXml(n.nodes)}</u>`;
        case "field":
          return `<field name="${escAttr(n.name ?? n.field)}"/>`;
        case "font": {
          const color = n.color ? ` color="${escAttr(String(n.color).replace(/^#/, ""))}"` : "";
          const face = n.face ? ` face="${escAttr(n.face)}"` : "";
          const size = n.size ? ` size="${Math.round(Number(n.size) * 20)}"` : "";
          return `<font${face}${size}${color}>${richNodesToXml(n.nodes)}</font>`;
        }
        case "invitation": {
          const privateAttr = n.private ? ` private="true"` : "";
          const projectAttr =
            n.project != null ? ` project="${escAttr(n.project)}"` : ` project=""`;
          const authField = n.authenticationTokenField ?? n.authField;
          const auth = authField
            ? `<authenticationTokenValue><string field="${escAttr(authField)}"/></authenticationTokenValue>`
            : "";
          return `<invitation form="${escAttr(n.form)}"${projectAttr}${privateAttr}>${auth}${escText(n.text ?? "")}</invitation>`;
        }
        case "choiceTallyTable":
          return `<choice-tally-table version="1"><field>${escAttr(n.field)}</field><conditions><form name="${escAttr(n.form)}"/></conditions></choice-tally-table>`;
        case "itemizationTable":
          return itemizationTableToXml(n);
        case "questionCorrelationTable":
          return `<question-correlation-table version="1"><question-field-name>${escAttr(n.questionField)}</question-field-name><display-field-name>${escAttr(n.displayField)}</display-field-name><preferred-choice-field-name>${escAttr(n.preferredField)}</preferred-choice-field-name><conditions><form name="${escAttr(n.form)}"/></conditions></question-correlation-table>`;
        default:
          return richNodesToXml(n.nodes);
      }
    })
    .join("");
}

function itemToXml(item, formName = "", project = null) {
  const altLabel = item.alternateLabel ?? item.name;
  const altAttr =
    altLabel && altLabel !== item.label ? ` alternateLabel="${escAttr(altLabel)}"` : "";

  switch (item.type) {
    case "heading":
      return `<heading label="${escAttr(item.label)}" type="Main">${escText(headingPlainText(item.content))}</heading>`;
    case "subheading":
      return `<heading label="${escAttr(item.label)}" type="Sub">${escText(headingPlainText(item.content))}</heading>`;
    case "text": {
      const legacy = registrationTextToXml(item, formName);
      const body = legacy ?? textContentToXml(item.content, item.style, project);
      return `<text label="${escAttr(item.label)}"${altAttr} style="${escAttr(item.style ?? "normal")}">${body}</text>`;
    }
    case "fib": {
      if (formName === "Registration") {
        const regFib = registrationFibToXml(item, escAttr, escText);
        if (regFib) return withDisplayConditions(regFib, item);
      }
      return withDisplayConditions(fibToXml(item, escAttr, escText), item);
    }
    case "mc":
      return withDisplayConditions(mcToXml(item, escAttr, escText), item);
    case "field":
      return `<field name="${escAttr(item.name ?? item.fieldName)}"/>`;
    case "break":
      return "<break/>";
    case "skipInstructions":
      return `<skipInstructions>${skipCommandsToXml(item.commands)}</skipInstructions>`;
    default:
      return "";
  }
}

export function projectToXml(project) {
  const forms = (project.forms ?? [])
    .map((form) => {
      const attrs = [
        `name="${escAttr(form.name)}"`,
        form.startPoint ? `startPoint="true"` : "",
        form.process ? `process="${escAttr(form.process)}"` : "",
        form.preProcess ? `preProcess="${escAttr(form.preProcess)}"` : "",
        form.themePath ? `themePath="${escAttr(form.themePath)}"` : "",
      ]
        .filter(Boolean)
        .join(" ");
      const items = (form.items ?? [])
        .map((item) => itemToXml(item, form.name, project))
        .join("");
      return `<form ${attrs}><items>${items}</items></form>`;
    })
    .join("");

  const processes = (project.processes ?? [])
    .map((p) => {
      const cmds = (p.commands ?? []).map(commandToXml).join("");
      return `<process name="${escAttr(p.name)}">${cmds}</process>`;
    })
    .join("");

  const documents = (project.documents ?? [])
    .map((d) => {
      const body =
        typeof d.content === "string"
          ? injectResponseTotalsQuestionTitles(
              documentHtmlToXml(d.content, escAttr, escText),
              project,
            )
          : richContentToXml(d.content);
      return `<document name="${escAttr(d.name)}"><xmlData>${body}</xmlData></document>`;
    })
    .join("");

  // Merge project.images with any data-URL embeds still only present in HTML
  // (Design/Preview show src=data:…; Deploy needs <imagedef> or Java 404s).
  const imagesXml = imagesToXml(collectProjectImages(project));

  return `<project name="${escAttr(project.name)}" themePath="${escAttr(project.themePath ?? "default")}" format="1.11" designerBuild="204"><forms>${forms}</forms><processes>${processes}</processes><documents>${documents}</documents>${imagesXml}</project>`;
}

/**
 * Collect imagedefs from `project.images` plus `<img data-tawala-image-id src="data:…">`
 * scattered in Form Text / Document HTML so Deploy never drops a Design-visible picture.
 * Exported for unit tests.
 */
export function collectProjectImages(project) {
  const byId = new Map();
  for (const img of Array.isArray(project?.images) ? project.images : []) {
    const id = String(img?.id ?? "").trim();
    const data = String(img?.data ?? "").replace(/\s+/g, "");
    if (!id || !data) continue;
    byId.set(id, {
      id,
      imageFormat: img.imageFormat,
      data,
      fileName: img.fileName,
    });
  }

  const htmlChunks = [];
  for (const form of project?.forms ?? []) {
    for (const item of form?.items ?? []) {
      if (typeof item?.content === "string") htmlChunks.push(item.content);
    }
  }
  for (const doc of project?.documents ?? []) {
    if (typeof doc?.content === "string") htmlChunks.push(doc.content);
  }

  const imgRe =
    /<img\b([^>]*?)(?:\/>|>)/gi;
  for (const html of htmlChunks) {
    let m;
    while ((m = imgRe.exec(html)) !== null) {
      const attrs = m[1] ?? "";
      const idM = attrs.match(/data-tawala-image-id=["']([^"']+)["']/i);
      const srcM = attrs.match(/\bsrc=["'](data:image\/[^"']+)["']/i);
      if (!idM?.[1] || !srcM?.[1]) continue;
      const id = idM[1].trim();
      if (byId.has(id)) continue;
      const parsed = parseDataUrlBase64(srcM[1]);
      if (!parsed) continue;
      byId.set(id, { id, imageFormat: parsed.imageFormat, data: parsed.data });
    }
  }

  return [...byId.values()];
}

function parseDataUrlBase64(dataUrl) {
  const m = /^data:image\/(png|gif|jpeg|jpg);base64,(.+)$/i.exec(String(dataUrl ?? "").trim());
  if (!m) return null;
  const fmt = m[1].toLowerCase();
  const imageFormat = fmt === "png" ? "PNG" : fmt === "gif" ? "GIF" : "JPEG";
  return { imageFormat, data: m[2].replace(/\s+/g, "") };
}

/** Project `images[]` → legacy `<images><imagedef><imagedata imageFormat>…`. Omit when empty. */
export function imagesToXml(images) {
  const list = Array.isArray(images) ? images : [];
  if (list.length === 0) return "";
  const defs = list
    .map((img) => {
      const id = String(img?.id ?? "").trim();
      const data = String(img?.data ?? "").replace(/\s+/g, "");
      if (!id || !data) return "";
      // Java Image.Data.Format is PNG|GIF|JPEG (not C# extension JPG).
      let format = String(img?.imageFormat ?? "PNG").toUpperCase();
      if (format === "JPG") format = "JPEG";
      if (!/^(PNG|GIF|JPEG)$/.test(format)) format = "PNG";
      return `<imagedef id="${escAttr(id)}"><imagedata imageFormat="${escAttr(format)}">${data}</imagedata></imagedef>`;
    })
    .filter(Boolean)
    .join("");
  return defs ? `<images>${defs}</images>` : "";
}

export function buildUploadRequest(credentials, project) {
  const creds = `<credentials user="${escAttr(credentials.user)}" password="${escAttr(credentials.password)}"/>`;
  return `<?xml version="1.0" encoding="utf-8" ?>\n<request type="uploadProject" protocol="1.0">\n${creds}\n${projectToXml(project)}\n</request>\n`;
}
