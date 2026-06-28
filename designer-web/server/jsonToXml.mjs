/** Best-effort JSON format 2.0 → legacy XML for Java /client upload. */

import { fibToXml } from "./fibToXml.mjs";
import { registrationTextToXml } from "./registrationTextToXml.mjs";

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

function fontXml(text, { bold = false, italic = false, size = 200 } = {}) {
  let inner = escText(text);
  if (italic) inner = `<i>${inner}</i>`;
  if (bold) inner = `<b>${inner}</b>`;
  return `<font face="Arial" size="${size}" color="000000">${inner}</font>`;
}

function textContentToXml(content, style) {
  if (typeof content === "string") {
    if (!content) {
      return `<paragraph indent="0" align="left">${TAB_MC}</paragraph>`;
    }
    const italic = style === "instructional";
    return `<paragraph indent="0" align="left">${TAB_MC}${fontXml(content, { italic })}</paragraph>`;
  }
  return richContentToXml(content);
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
  return `<${op} field="${escAttr(cond.field)}"><string value="${escAttr(cond.value)}"/></${op}>`;
}

function xmlCommentText(s) {
  return String(s ?? "")
    .replace(/--/g, "- -")
    .replace(/-$/g, "");
}

function commandToXml(cmd) {
  switch (cmd.cmd) {
    case "comment":
      return `<!-- ${xmlCommentText(cmd.text)} -->`;
    case "set":
      return `<set field="${escAttr(cmd.field)}"><string value="${escAttr(cmd.value)}"/></set>`;
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
      const thenXml = (cmd.then ?? []).map(commandToXml).join("");
      const elseXml = (cmd.else ?? []).map(commandToXml).join("");
      return `<if><conditions>${conditionToXml(cmd.condition)}</conditions><trueSet>${thenXml}</trueSet><falseSet>${elseXml}</falseSet></if>`;
    }
    case "skip":
      return `<skip to="${escAttr(cmd.to)}"/>`;
    case "foreach": {
      const body = (cmd.do ?? []).map(commandToXml).join("");
      return `<foreach recordName="${escAttr(cmd.recordName)}" recordList="${escAttr(cmd.recordList)}">${body}</foreach>`;
    }
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
        case "field":
          return `<field name="${escAttr(n.name ?? n.field)}"/>`;
        default:
          return richNodesToXml(n.nodes);
      }
    })
    .join("");
}

function itemToXml(item, formName = "") {
  switch (item.type) {
    case "heading":
      return `<heading label="${escAttr(item.label)}" style="main">${escText(item.content)}</heading>`;
    case "subheading":
      return `<heading label="${escAttr(item.label)}" style="sub">${escText(item.content)}</heading>`;
    case "text": {
      const legacy = registrationTextToXml(item, formName);
      const body = legacy ?? textContentToXml(item.content, item.style);
      return `<text label="${escAttr(item.label)}" style="${escAttr(item.style ?? "normal")}">${body}</text>`;
    }
    case "fib":
      return fibToXml(item, escAttr, escText);
    case "mc": {
      const choices = (item.choices ?? [])
        .map(
          (c, i) =>
            `<choice label="${escAttr(c.label ?? c.name ?? String.fromCharCode(97 + i))}"><paragraph indent="0" align="left">${TAB_MC}${fontXml(c.text)}</paragraph></choice>`,
        )
        .join("");
      const mcStyle =
        item.displayAs === "radio" ? "horizontal" : item.style ?? "";
      const altLabel = item.name ? ` alternateLabel="${escAttr(item.name)}"` : "";
      const styleAttr = mcStyle ? ` style="${escAttr(mcStyle)}"` : "";
      const question = item.question ?? "";
      return `<mc label="${escAttr(item.label)}"${altLabel} onlyone="${item.onlyone !== false ? "true" : "false"}" required="${item.required ? "true" : "false"}"${styleAttr}><question><paragraph indent="0" align="left">${TAB_MC}${fontXml(question, { bold: true })}</paragraph></question>${choices}</mc>`;
    }
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
      const items = (form.items ?? []).map((item) => itemToXml(item, form.name)).join("");
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
          ? `<paragraph indent="0" align="left">${escText(d.content)}</paragraph>`
          : richContentToXml(d.content);
      return `<document name="${escAttr(d.name)}"><xmlData>${body}</xmlData></document>`;
    })
    .join("");

  return `<project name="${escAttr(project.name)}" themePath="${escAttr(project.themePath ?? "default")}" format="1.11" designerBuild="204"><forms>${forms}</forms><processes>${processes}</processes><documents>${documents}</documents></project>`;
}

export function buildUploadRequest(credentials, project) {
  const creds = `<credentials user="${escAttr(credentials.user)}" password="${escAttr(credentials.password)}"/>`;
  return `<?xml version="1.0" encoding="utf-8" ?>\n<request type="uploadProject" protocol="1.0">\n${creds}\n${projectToXml(project)}\n</request>\n`;
}
