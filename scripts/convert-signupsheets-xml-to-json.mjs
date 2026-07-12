#!/usr/bin/env node
/**
 * One-off: SignupSheets legacy .tawala XML → browser Designer JSON (format 2.0).
 *
 * Usage (repo root):
 *   node scripts/convert-signupsheets-xml-to-json.mjs
 *
 * Reads:  designer-web/public/samples/legacy/SignupSheets.tawala.xml
 * Writes: designer-web/public/samples/signup-sheets.json
 *
 * Not a general .tawala importer — lossy-with-notes; see SignupSheets_CONVERSION_GAPS.md.
 */

import fs from "node:fs";
import path from "node:path";
import { createRequire } from "node:module";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "..");
const require = createRequire(path.join(REPO_ROOT, "designer-web/package.json"));
const { XMLParser } = require("fast-xml-parser");

const INPUT = path.join(
  REPO_ROOT,
  "designer-web/public/samples/legacy/SignupSheets.tawala.xml",
);
const OUTPUT = path.join(REPO_ROOT, "designer-web/public/samples/signup-sheets.json");

const warnings = [];
function warn(msg) {
  warnings.push(msg);
  console.warn(`[warn] ${msg}`);
}

const UNARY_OPS = new Set([
  "isBlank",
  "isNotBlank",
  "mcIsBlank",
  "mcIsNotBlank",
]);

const CONDITION_OPS = new Set([
  "equals",
  "doesNotEqual",
  "contains",
  "doesNotContain",
  "beginsWith",
  "endsWith",
  "isLessThan",
  "isLessThanOrEqualTo",
  "isGreaterThan",
  "isGreaterThanOrEqualTo",
  "isBlank",
  "isNotBlank",
  "mcEquals",
  "mcDoesNotEqual",
  "mcContains",
  "mcDoesNotContain",
  "mcIsBlank",
  "mcIsNotBlank",
]);

/** Ordered children of a preserveOrder element array. */
function children(nodes) {
  if (!Array.isArray(nodes)) return [];
  return nodes.filter((n) => {
    if (!n || typeof n !== "object") return false;
    // Attribute-only sibling entries look like { ":@": {...} } with no element key.
    const keys = Object.keys(n).filter((k) => k !== ":@");
    if (keys.length === 0) return false;
    if (keys.length === 1 && keys[0] === "#text") return false;
    if (keys.length === 1 && keys[0] === "#comment") return false;
    return true;
  });
}

function tagName(node) {
  if (!node || typeof node !== "object") return null;
  return Object.keys(node).find((k) => k !== ":@" && k !== "#text" && k !== "#comment") ?? null;
}

function attrs(node) {
  return node?.[":@"] ?? {};
}

function attr(node, name) {
  return attrs(node)[`@_${name}`];
}

function boolAttr(node, name, fallback = false) {
  const v = attr(node, name);
  if (v == null) return fallback;
  return String(v).toLowerCase() === "true";
}

function findChild(nodes, name) {
  for (const n of children(nodes)) {
    if (tagName(n) === name) return n[name];
  }
  return null;
}

function findChildren(nodes, name) {
  return children(nodes)
    .filter((n) => tagName(n) === name)
    .map((n) => n[name]);
}

function textOf(nodes) {
  if (!Array.isArray(nodes)) return "";
  let out = "";
  for (const n of nodes) {
    if (n?.["#text"] != null) out += String(n["#text"]);
    else {
      const t = tagName(n);
      if (t && Array.isArray(n[t])) out += textOf(n[t]);
    }
  }
  return out;
}

function flattenPlainText(nodes) {
  return textOf(nodes).replace(/\s+/g, " ").trim();
}

/** Convert XML expression fragments (<string>, <field>, <add>, …) to a <<>> template string. */
function expressionToString(nodes) {
  if (!Array.isArray(nodes)) return "";
  const parts = [];
  for (const n of children(nodes)) {
    const t = tagName(n);
    if (!t) continue;
    const body = n[t];
    const a = attrs(n);
    switch (t) {
      case "string": {
        if (a["@_field"] != null) parts.push(`<<${a["@_field"]}>>`);
        else if (a["@_value"] != null) parts.push(String(a["@_value"]));
        else parts.push(flattenPlainText(body));
        break;
      }
      case "field": {
        const name = a["@_name"] ?? flattenPlainText(body);
        if (name) parts.push(`<<${name}>>`);
        break;
      }
      case "operand": {
        if (a["@_field"] != null) parts.push(`<<${a["@_field"]}>>`);
        else if (a["@_value"] != null) parts.push(String(a["@_value"]));
        else parts.push(expressionToString(body));
        break;
      }
      case "add":
      case "sub":
      case "mul":
      case "div": {
        const op = { add: "+", sub: "-", mul: "*", div: "/" }[t];
        const operands = children(body)
          .map((c) => {
            const ct = tagName(c);
            if (ct === "operand" || ct === "add" || ct === "sub" || ct === "mul" || ct === "div") {
              return expressionToString([c]);
            }
            return expressionToString(c[ct] ?? []);
          })
          .filter(Boolean);
        // Nested arithmetic may appear as child ops rather than operand wrappers.
        if (operands.length === 0) {
          parts.push(expressionToString(body));
        } else {
          parts.push(operands.join(` ${op} `));
        }
        break;
      }
      case "set":
        parts.push(expressionToString(body));
        break;
      default:
        parts.push(expressionToString(body));
    }
  }
  // Also pick up bare text nodes mixed in (rare).
  for (const n of nodes) {
    if (n?.["#text"] != null && String(n["#text"]).trim()) {
      // already covered via children walk when nested; skip top-level whitespace
    }
  }
  return parts.join("");
}

function conditionValueFromNode(opNode) {
  const body = opNode[tagName(opNode)];
  const a = attrs(opNode);
  if (a["@_value"] != null) return String(a["@_value"]);
  const expr = expressionToString(body);
  if (expr) return expr;
  // Some conditions put value as attribute on the op itself (mcEquals value="a")
  return undefined;
}

function parseSimpleCondition(opNode) {
  const op = tagName(opNode);
  if (!op || !CONDITION_OPS.has(op)) return null;
  const field = attr(opNode, "field");
  const cond = { field, op };
  if (!UNARY_OPS.has(op)) {
    const valueAttr = attr(opNode, "value");
    const value = valueAttr != null ? String(valueAttr) : conditionValueFromNode(opNode);
    if (value !== undefined) cond.value = value;
  }
  return cond;
}

/**
 * Parse <conditions> children into JSON condition object.
 * Handles: single op, and/or wrappers, sibling ops with intervening <and/>/<or/>.
 */
function parseConditions(condNodes) {
  if (!condNodes) return undefined;
  const kids = children(condNodes);

  // Single and/or wrapper
  if (kids.length === 1) {
    const t = tagName(kids[0]);
    if (t === "and" || t === "or") {
      const inner = children(kids[0][t])
        .map((c) => {
          const ct = tagName(c);
          if (ct === "and" || ct === "or") {
            return parseConditions([c]);
          }
          return parseSimpleCondition(c);
        })
        .filter(Boolean);
      return { [t]: inner };
    }
    return parseSimpleCondition(kids[0]) ?? undefined;
  }

  // Mixed: equals, and, equals  OR  equals, or, equals
  const simples = [];
  let combinator = null;
  for (const k of kids) {
    const t = tagName(k);
    if (t === "and" || t === "or") {
      // Empty and/or as separator between sibling conditions
      if (children(k[t]).length === 0) {
        combinator = t;
        continue;
      }
      const nested = parseConditions([k]);
      if (nested) simples.push(nested);
      continue;
    }
    if (CONDITION_OPS.has(t)) {
      const c = parseSimpleCondition(k);
      if (c) simples.push(c);
    }
  }
  if (simples.length === 0) return undefined;
  if (simples.length === 1) return simples[0];
  return { [combinator ?? "and"]: simples };
}

function displayConditionFromItem(itemBody) {
  const dc = findChild(itemBody, "displayConditions");
  if (!dc) return undefined;
  const cond = parseConditions(dc);
  if (cond) {
    warn(
      `Preserved displayCondition in JSON (Designer UI cannot edit yet): ${JSON.stringify(cond)}`,
    );
  }
  return cond;
}

/** Walk rich XML nodes → JSON rich nodes (DirtBowl-style). */
function richNodesFromXml(nodes, ctx = {}) {
  if (!Array.isArray(nodes)) return [];
  const out = [];
  for (const n of nodes) {
    if (n?.["#text"] != null) {
      const text = String(n["#text"]);
      if (text) out.push({ type: "text", text });
      continue;
    }
    const t = tagName(n);
    if (!t) continue;
    const body = n[t];
    const a = attrs(n);

    switch (t) {
      case "tabPositions":
      case "sp":
        break;
      case "font": {
        const face = a["@_face"];
        const sizeTwips = a["@_size"] != null ? Number(a["@_size"]) : undefined;
        const colorRaw = a["@_color"];
        const node = {
          type: "font",
          nodes: richNodesFromXml(body, ctx),
        };
        if (face) node.face = face;
        if (sizeTwips != null && !Number.isNaN(sizeTwips)) node.size = sizeTwips / 20;
        if (colorRaw) node.color = colorRaw.startsWith("#") ? colorRaw : `#${colorRaw}`;
        out.push(node);
        break;
      }
      case "b":
        out.push({ type: "bold", nodes: richNodesFromXml(body, ctx) });
        break;
      case "i":
        out.push({ type: "italic", nodes: richNodesFromXml(body, ctx) });
        break;
      case "u":
        out.push({ type: "underline", nodes: richNodesFromXml(body, ctx) });
        break;
      case "field": {
        const name = a["@_name"];
        if (name) out.push({ type: "field", name });
        break;
      }
      case "invitation": {
        const form = a["@_form"] ?? "";
        const project = a["@_project"] ?? "";
        const priv = String(a["@_private"] ?? "").toLowerCase() === "true";
        const text = flattenPlainText(body);
        warn(`Document/text invitation → form="${form}" text="${text}" (limited Designer round-trip)`);
        const inv = { type: "invitation", form, text, project };
        if (priv) inv.private = true;
        // auth token if present
        const auth = findChild(body, "authenticationTokenValue");
        if (auth) {
          const authExpr = expressionToString(auth);
          const m = authExpr.match(/^<<([^>]+)>>$/);
          if (m) inv.authenticationTokenField = m[1];
        }
        out.push(inv);
        break;
      }
      case "itemization-table":
        out.push(convertItemizationTable(n, ctx));
        break;
      case "link": {
        warn("Hyperlink in rich content — approximated as text");
        out.push({ type: "text", text: flattenPlainText(body) });
        break;
      }
      case "paragraph":
        // Nested paragraph inside cell etc. — flatten as division-like
        out.push({
          type: "paragraph",
          align: a["@_align"] ?? "left",
          indent: a["@_indent"] != null ? Number(a["@_indent"]) : 0,
          nodes: richNodesFromXml(body, ctx),
        });
        break;
      default:
        out.push(...richNodesFromXml(body, ctx));
    }
  }
  return out;
}

function convertItemizationTable(tableNode, ctx = {}) {
  const body = tableNode["itemization-table"] ?? tableNode;
  const a = attrs(tableNode);
  const version = a["@_version"] != null ? Number(a["@_version"]) : 2;
  const columns = [];
  let formName = "";
  let where;
  let multiForm = false;

  for (const c of children(body)) {
    const t = tagName(c);
    if (t === "column") {
      const colBody = c.column;
      const headerNode = findChild(colBody, "header");
      let header = "";
      if (headerNode) {
        // header may be <string value="…"/> or <field name="…"/>
        const headerKids = children(headerNode);
        if (headerKids.length === 1 && tagName(headerKids[0]) === "field") {
          header = `<<${attr(headerKids[0], "name")}>>`;
        } else {
          header = expressionToString(headerNode) || flattenPlainText(headerNode);
        }
      }
      const contents = findChild(colBody, "contents");
      let field = "";
      if (contents) {
        const fieldEl = children(contents).find((x) => tagName(x) === "field");
        if (fieldEl) field = attr(fieldEl, "name") ?? "";
      }
      const col = { header, field };
      const colDc = findChild(colBody, "display-conditions");
      if (colDc) {
        const dc = parseConditions(colDc);
        if (dc) {
          col.displayCondition = dc;
          warn(
            `Itemization column "${header}" has display-conditions (Designer cannot edit per-column conditions yet)`,
          );
        }
      }
      columns.push(col);
    } else if (t === "conditions") {
      // <conditions><form name="X"/><form name="Y"/><conditions>…</conditions></conditions>
      const forms = findChildren(c.conditions, "form").map((f) => {
        // form node is array; attrs on the wrapper
        return null;
      });
      // Re-walk with attrs: children entries that are form
      const formNames = [];
      let nestedCond;
      for (const inner of children(c.conditions)) {
        const it = tagName(inner);
        if (it === "form") {
          formNames.push(attr(inner, "name"));
        } else if (it === "conditions") {
          nestedCond = parseConditions(inner.conditions);
        }
      }
      if (formNames.length > 1) {
        multiForm = true;
        warn(
          `Itemization table uses multiple source forms (${formNames.join(", ")}) — JSON keeps primary form only`,
        );
      }
      formName = formNames[0] ?? "";
      if (nestedCond) {
        where = nestedCond;
        warn(
          `Itemization table filter conditions preserved as where (Designer UI limited): ${JSON.stringify(nestedCond)}`,
        );
      }
    } else if (t === "show-print-control" || t === "show-export-control" || t === "number-of-columns") {
      // ignored — defaults in Designer
    }
  }

  const node = {
    type: "itemizationTable",
    version,
    form: formName,
    columns: columns.map(({ header, field, displayCondition }) => {
      const col = { header, field };
      if (displayCondition) col.displayCondition = displayCondition;
      return col;
    }),
  };
  if (where) node.where = where;
  return node;
}

function paragraphsToBlocks(itemBody, ctx = {}) {
  const blocks = [];
  for (const n of children(itemBody)) {
    const t = tagName(n);
    if (t === "paragraph") {
      const a = attrs(n);
      blocks.push({
        type: "paragraph",
        align: a["@_align"] ?? "left",
        indent: a["@_indent"] != null ? Number(a["@_indent"]) : 0,
        nodes: richNodesFromXml(n.paragraph, ctx),
      });
    } else if (t === "displayConditions") {
      // handled separately
    } else if (t === "table") {
      warn("Table inside form text — converting cells as nested paragraphs");
      blocks.push(...tableToBlocks(n.table, ctx));
    }
  }
  return blocks;
}

function tableToBlocks(tableBody, ctx) {
  const rows = [];
  for (const r of findChildren(tableBody, "row")) {
    const cells = [];
    for (const cellWrap of children(r)) {
      if (tagName(cellWrap) !== "cell") continue;
      const width = attr(cellWrap, "width");
      const cell = {
        width: width != null ? Number(width) : undefined,
        content: paragraphsToBlocks(cellWrap.cell, ctx),
      };
      cells.push(cell);
    }
    rows.push({ cells });
  }
  return [{ type: "table", rows }];
}

function blocksAreSimplePlain(blocks) {
  if (!blocks.length) return true;
  return blocks.every((b) => {
    if (b.type !== "paragraph") return false;
    const nodes = b.nodes ?? [];
    if (nodes.length === 0) return true;
    // only text / font / bold / italic wrapping text — no fields/itemization/invitation
    const walk = (ns) =>
      ns.every((n) => {
        if (n.type === "text") return true;
        if (["font", "bold", "italic", "underline"].includes(n.type)) {
          return walk(n.nodes ?? []);
        }
        return false;
      });
    return walk(nodes);
  });
}

function blocksToPlainString(blocks) {
  const flatten = (nodes) =>
    (nodes ?? [])
      .map((n) => {
        if (n.type === "text") return n.text ?? "";
        if (n.type === "field") return `<<${n.name}>>`;
        return flatten(n.nodes);
      })
      .join("");
  return blocks
    .map((b) => flatten(b.nodes))
    .filter((s) => s.length)
    .join("\n")
    .trim();
}

function convertHeading(itemNode) {
  const body = itemNode.heading;
  const label = attr(itemNode, "label") ?? "H1";
  const typeAttr = (attr(itemNode, "type") ?? "Main").toLowerCase();
  const level = typeAttr === "sub" ? "sub" : "main";
  const blocks = paragraphsToBlocks(body, { location: `heading ${label}` });
  let content = blocksToPlainString(blocks);
  // Prefer <<field>> form when heading is only a field
  if (!content) content = flattenPlainText(body);
  const item = { type: "heading", label, level, content };
  return item;
}

function convertText(itemNode) {
  const body = itemNode.text;
  const label = attr(itemNode, "label") ?? "T1";
  const style = attr(itemNode, "style") ?? "normal";
  const blocks = paragraphsToBlocks(body, { location: `text ${label}` });
  const hasSpecial = JSON.stringify(blocks).match(
    /itemizationTable|"type":"field"|"type":"invitation"/,
  );
  const item = { type: "text", label, style };
  if (hasSpecial || !blocksAreSimplePlain(blocks)) {
    item.content = blocks;
  } else {
    item.content = blocksToPlainString(blocks);
  }
  const dc = displayConditionFromItem(body);
  if (dc) item.displayCondition = dc;
  return item;
}

function convertFib(itemNode) {
  const body = itemNode.fib;
  const label = attr(itemNode, "label") ?? "Q1";
  const style = attr(itemNode, "style") ?? "";
  const blanks = [];
  const promptParts = [];

  const walkParagraph = (paraBody) => {
    for (const n of paraBody ?? []) {
      if (n?.["#text"] != null) {
        const t = String(n["#text"]);
        if (t.trim()) promptParts.push(t);
        continue;
      }
      const t = tagName(n);
      if (!t) continue;
      if (t === "blank") {
        const a = attrs(n);
        const blank = {
          name: a["@_alternateLabel"] || a["@_label"] || "a",
          length: a["@_length"] != null ? Number(a["@_length"]) : 20,
        };
        if (a["@_alternateLabel"]) blank.alternateLabel = a["@_alternateLabel"];
        if (String(a["@_required"]).toLowerCase() === "true") blank.required = true;
        if (a["@_height"] != null && Number(a["@_height"]) > 1) {
          blank.height = Number(a["@_height"]);
        }
        blanks.push(blank);
        if (promptParts.length && blanks.length > 1) {
          // ensure / separators between blank regions — rebuild below
        }
      } else if (t === "font" || t === "b" || t === "i" || t === "u") {
        walkParagraph(n[t]);
      } else if (t === "field") {
        promptParts.push(`<<${attr(n, "name")}>>`);
      } else if (t === "sp") {
        promptParts.push(" ");
      } else if (t === "tabPositions") {
        // ignore
      } else {
        walkParagraph(n[t]);
      }
    }
  };

  for (const n of children(body)) {
    if (tagName(n) === "paragraph") walkParagraph(n.paragraph);
  }

  // Build prompt: text segments joined; blanks implied by blanks array.
  // DirtBowl uses / between blank regions — approximate from collected text.
  let prompt = promptParts.join("").replace(/\s+/g, " ").trim();
  if (blanks.length > 1 && !prompt.includes("/")) {
    // leave as-is; single prompt string before first blank is common
  }

  const item = { type: "fib", label, style, prompt, blanks };
  const dc = displayConditionFromItem(body);
  if (dc) item.displayCondition = dc;
  return item;
}

function convertDynamicMcq(dataProviderNode, mcLabel) {
  const dyn = findChild(dataProviderNode, "dynamic-mcq");
  if (!dyn) return null;
  const displayExpr = expressionToString(findChild(dyn, "display-expression") ?? []);
  const valueExpr = expressionToString(findChild(dyn, "value-expression") ?? []);
  const sortExpr = expressionToString(findChild(dyn, "sort-expression") ?? []);
  const selector = findChild(dyn, "record-selector");
  let sourceForm = "";
  let where;
  if (selector) {
    for (const c of children(selector)) {
      if (tagName(c) === "form") sourceForm = attr(c, "name") ?? sourceForm;
      if (tagName(c) === "conditions") {
        where = parseConditions(c.conditions);
      }
    }
  }
  const choice = {
    type: "dynamic",
    sourceForm,
    displayExpr: displayExpr.includes("<<") ? displayExpr : displayExpr ? `<<${displayExpr}>>` : "",
    valueExpr: valueExpr.includes("<<") ? valueExpr : valueExpr ? `<<${valueExpr}>>` : "",
  };
  // Normalize: expressionToString already emits << >> for fields
  if (displayExpr) choice.displayExpr = displayExpr;
  if (valueExpr) choice.valueExpr = valueExpr;
  if (sortExpr) choice.sortExpr = sortExpr;
  if (where) {
    choice.where = where;
    warn(
      `Dynamic MCQ ${mcLabel} has record-selector filter (Designer Configure Function UI deferred): ${JSON.stringify(where)}`,
    );
  } else {
    warn(`Dynamic MCQ ${mcLabel} from form "${sourceForm}" — choiceSource=stored; Configure Function UI deferred`);
  }
  return choice;
}

function convertMc(itemNode) {
  const body = itemNode.mc;
  const label = attr(itemNode, "label") ?? "Q1";
  const name = attr(itemNode, "alternateLabel");
  const onlyone = boolAttr(itemNode, "onlyone", true);
  const required = boolAttr(itemNode, "required", false);
  const style = attr(itemNode, "style") ?? "vertical";
  const paddingBottom = attr(itemNode, "paddingBottom");

  const questionNode = findChild(body, "question");
  const question = questionNode
    ? blocksToPlainString(paragraphsToBlocks(questionNode, { location: `mc ${label} question` }))
    : "";

  const dataProvider = findChild(body, "data-provider");
  let choices = [];
  let choiceSource = "manual";
  let displayAs = onlyone ? "radio" : "checkbox";
  if (style === "dropdown") displayAs = "dropdown";

  if (dataProvider) {
    choiceSource = "stored";
    const dyn = convertDynamicMcq(dataProvider, label);
    if (dyn) choices = [dyn];
  } else {
    for (const choiceWrap of children(body)) {
      if (tagName(choiceWrap) !== "choice") continue;
      const clabel = attr(choiceWrap, "label") ?? "a";
      const text = blocksToPlainString(
        paragraphsToBlocks(choiceWrap.choice, { location: `mc ${label} choice` }),
      );
      choices.push({ label: clabel, text });
    }
  }

  const item = {
    type: "mc",
    label,
    onlyone,
    required,
    style,
    displayAs,
    choiceSource,
    question,
    choices,
  };
  if (name) item.name = name;
  if (paddingBottom != null) {
    item.paddingBottom = String(paddingBottom).toLowerCase() === "true";
  }
  const dc = displayConditionFromItem(body);
  if (dc) item.displayCondition = dc;
  return item;
}

function convertField(itemNode) {
  const name = attr(itemNode, "name") ?? "Field";
  return { type: "field", label: "", name };
}

function convertBreak(itemNode) {
  return { type: "break", label: attr(itemNode, "label") ?? "BREAK" };
}

function convertSkipCommands(skipBody) {
  const commands = [];
  for (const n of children(skipBody)) {
    const t = tagName(n);
    if (t === "if") {
      commands.push(convertIfCommand(n));
    } else if (t === "skip") {
      commands.push({ cmd: "skip", to: attr(n, "to") ?? "" });
    } else if (t === "set") {
      commands.push(convertSetCommand(n));
    } else if (t === "comment" || n["#comment"] != null) {
      const text =
        n["#comment"] != null ? String(n["#comment"]) : flattenPlainText(n.comment ?? []);
      commands.push({ cmd: "comment", text });
    }
  }
  return commands;
}

function convertSkip(itemNode, skipIndex) {
  const body = itemNode.skipInstructions;
  return {
    type: "skipInstructions",
    label: `skip${skipIndex}`,
    commands: convertSkipCommands(body),
  };
}

function convertSetCommand(setNode) {
  const field = attr(setNode, "field");
  const arithmeticAsText = attr(setNode, "arithmeticAsText");
  const value = expressionToString(setNode.set ?? []);
  const cmd = { cmd: "set", field, value, concat: false };
  if (arithmeticAsText != null) {
    cmd.arithmeticAsText = String(arithmeticAsText).toLowerCase() === "true";
  }
  return cmd;
}

function convertIfCommand(ifNode) {
  const body = ifNode.if ?? ifNode;
  const condNodes = findChild(body, "conditions");
  const condition = parseConditions(condNodes) ?? { field: "", op: "equals", value: "" };
  const trueSet = findChild(body, "trueSet") ?? [];
  const falseSet = findChild(body, "falseSet") ?? [];
  const cmd = {
    cmd: "if",
    condition,
    then: convertProcessCommands(trueSet),
  };
  const elseCmds = convertProcessCommands(falseSet);
  if (elseCmds.length) cmd.else = elseCmds;
  return cmd;
}

function convertGetCommand(getNode) {
  const recordList = attr(getNode, "recordList");
  const body = getNode.get ?? [];
  const formsWrap = findChild(body, "forms");
  const sourceForms = [];
  if (formsWrap) {
    for (const f of children(formsWrap)) {
      if (tagName(f) === "form") sourceForms.push(attr(f, "name"));
    }
  }
  const cmd = { cmd: "get", recordList, sourceForms };
  const cond = findChild(body, "conditions");
  if (cond) {
    const where = parseConditions(cond);
    if (where) cmd.where = where;
  }
  return cmd;
}

function convertForeachCommand(feNode) {
  const recordName = attr(feNode, "record");
  const recordList = attr(feNode, "recordList");
  const body = feNode.foreach ?? [];
  return {
    cmd: "foreach",
    recordName,
    recordList,
    do: convertProcessCommands(body),
  };
}

function convertDeleteCommand(delNode) {
  const body = delNode.delete ?? [];
  let form = "";
  let where;
  for (const c of children(body)) {
    if (tagName(c) === "form") form = attr(c, "name") ?? "";
    if (tagName(c) === "conditions") where = parseConditions(c.conditions);
  }
  // Designer UI / jsonToXml use `form`; mapping doc uses `from`.
  const cmd = { cmd: "delete", form, from: form };
  if (where) cmd.where = where;
  return cmd;
}

function convertShowCommand(showNode) {
  const form = attr(showNode, "form");
  const document = attr(showNode, "document");
  const type = attr(showNode, "type");
  const reset = boolAttr(showNode, "reset", false);
  const body = showNode.show ?? [];

  if (document || type === "document") {
    return {
      cmd: "showDocument",
      document: document ?? attr(showNode, "name"),
      reset,
    };
  }
  if (form) return { cmd: "show", form };

  const url = findChild(body, "url");
  if (url) {
    return { cmd: "show", url: expressionToString(url) };
  }
  warn("Empty <show/> command preserved as { cmd: 'show' }");
  return { cmd: "show" };
}

function convertEditCommand(editNode) {
  const form = attr(editNode, "form");
  const submit = attr(editNode, "submit") ?? "modify";
  const body = editNode.edit ?? [];
  const cmd = { cmd: "edit", form, submit };
  const cond = findChild(body, "conditions");
  if (cond) {
    const condition = parseConditions(cond);
    if (condition) cmd.condition = condition;
  }
  return cmd;
}

function convertSendCommand(sendNode) {
  const body = sendNode.send ?? [];
  const cmd = { cmd: "send" };

  const readAddress = (tag) => {
    const nodes = children(body).filter((n) => tagName(n) === tag);
    if (!nodes.length) return undefined;
    const addrs = nodes.map((n) => {
      const a = attrs(n);
      if (a["@_addressField"]) {
        const o = { fieldRef: a["@_addressField"] };
        if (a["@_aliasField"]) o.aliasField = a["@_aliasField"];
        if (a["@_aliasLiteral"]) o.aliasLiteral = a["@_aliasLiteral"];
        return o;
      }
      if (a["@_address"] != null) return { literal: a["@_address"] };
      // nested <field name=…/>
      const fieldEl = children(n[tag]).find((c) => tagName(c) === "field");
      if (fieldEl) return { fieldRef: attr(fieldEl, "name") };
      const lit = children(n[tag]).find((c) => tagName(c) === "literal");
      if (lit) return { literal: attr(lit, "value") };
      return null;
    }).filter(Boolean);
    if (addrs.length === 1) return addrs[0];
    if (addrs.length > 1) return addrs;
    return undefined;
  };

  const to = readAddress("to");
  const from = readAddress("from");
  const cc = readAddress("cc");
  const bcc = readAddress("bcc");
  if (to) cmd.to = to;
  if (from) cmd.from = from;
  if (cc) cmd.cc = cc;
  if (bcc) cmd.bcc = bcc;

  const subjectNode = findChild(body, "subject");
  if (subjectNode) {
    // subject may mix <field name/> and text
    let subject = "";
    for (const n of subjectNode) {
      if (n?.["#text"] != null) subject += String(n["#text"]);
      else if (tagName(n) === "field") subject += `<<${attr(n, "name")}>>`;
      else if (tagName(n) === "string") subject += expressionToString([n]);
      else if (tagName(n)) subject += expressionToString([n]);
    }
    cmd.subject = subject.trim();
  }

  const bodyDoc = children(body).find((n) => tagName(n) === "body");
  if (bodyDoc) {
    const a = attrs(bodyDoc);
    cmd.body = {
      document: a["@_document"],
      reset: String(a["@_reset"] ?? "false").toLowerCase() === "true",
      showHeader: String(a["@_showHeader"] ?? "true").toLowerCase() !== "false",
    };
  }

  warn(`Send command subject="${cmd.subject ?? ""}" body="${cmd.body?.document ?? ""}" (email deploy path limited in browser Designer)`);
  return cmd;
}

function convertProcessCommands(nodes) {
  const cmds = [];
  if (!Array.isArray(nodes)) return cmds;
  for (const n of nodes) {
    if (n?.["#comment"] != null) {
      cmds.push({ cmd: "comment", text: String(n["#comment"]).trim() });
      continue;
    }
    const t = tagName(n);
    if (!t) continue;
    switch (t) {
      case "comment":
        cmds.push({ cmd: "comment", text: flattenPlainText(n.comment) });
        break;
      case "set":
        cmds.push(convertSetCommand(n));
        break;
      case "get":
        cmds.push(convertGetCommand(n));
        break;
      case "if":
        cmds.push(convertIfCommand(n));
        break;
      case "foreach":
        cmds.push(convertForeachCommand(n));
        break;
      case "delete":
        cmds.push(convertDeleteCommand(n));
        break;
      case "show":
        cmds.push(convertShowCommand(n));
        break;
      case "edit":
        cmds.push(convertEditCommand(n));
        break;
      case "send":
        cmds.push(convertSendCommand(n));
        break;
      case "skip":
        cmds.push({ cmd: "skip", to: attr(n, "to") ?? "" });
        break;
      case "append": {
        const field = attr(n, "field");
        const value = expressionToString(n.append ?? []);
        cmds.push({ cmd: "append", field, value });
        break;
      }
      default:
        warn(`Unsupported process command <${t}> — skipped`);
    }
  }
  return cmds;
}

function convertForm(formNode) {
  const name = attr(formNode, "name");
  const form = {
    name,
    startPoint: boolAttr(formNode, "startPoint", false),
    items: [],
  };
  const themePath = attr(formNode, "themePath");
  const process = attr(formNode, "process");
  const preProcess = attr(formNode, "preProcess");
  if (themePath) form.themePath = themePath;
  if (process) form.process = process;
  if (preProcess) form.preProcess = preProcess;
  if (boolAttr(formNode, "dataEntryOnly", false)) form.dataEntryOnly = true;
  const dataSourceName = attr(formNode, "dataSourceName");
  if (dataSourceName) form.dataSourceName = dataSourceName;
  if (boolAttr(formNode, "blockBackButton", false)) form.blockBackButton = true;

  // formNode: { form: [...children...], ':@': { @_name, ... } }
  const formChildren = formNode.form ?? [];
  const list = findChild(formChildren, "items") ?? [];

  let skipIndex = 0;
  for (const n of children(list)) {
    const t = tagName(n);
    switch (t) {
      case "heading":
        form.items.push(convertHeading(n));
        break;
      case "text":
        form.items.push(convertText(n));
        break;
      case "fib":
        form.items.push(convertFib(n));
        break;
      case "mc":
        form.items.push(convertMc(n));
        break;
      case "field":
        form.items.push(convertField(n));
        break;
      case "break":
        form.items.push(convertBreak(n));
        break;
      case "skipInstructions":
        skipIndex += 1;
        form.items.push(convertSkip(n, skipIndex));
        break;
      default:
        if (t) warn(`Form ${name}: unsupported item <${t}> skipped`);
    }
  }
  return form;
}

function convertProcess(procNode) {
  const name = attr(procNode, "name");
  return {
    name,
    commands: convertProcessCommands(procNode.process ?? []),
  };
}

function convertDocument(docNode) {
  const name = attr(docNode, "name");
  const body = docNode.document ?? [];
  const xmlData = findChild(body, "xmlData");
  const blocks = [];
  if (xmlData) {
    for (const n of children(xmlData)) {
      const t = tagName(n);
      if (t === "paragraph") {
        const a = attrs(n);
        blocks.push({
          type: "paragraph",
          align: a["@_align"] ?? "left",
          indent: a["@_indent"] != null ? Number(a["@_indent"]) : 0,
          nodes: richNodesFromXml(n.paragraph, { location: `document ${name}` }),
        });
      } else if (t === "table") {
        blocks.push(...tableToBlocks(n.table, { location: `document ${name}` }));
      }
    }
  }
  // Strip rawHtmlData intentionally
  if (findChild(body, "rawHtmlData")) {
    warn(`Document ${name}: rawHtmlData stripped (legacy, ignored by runtime)`);
  }
  return { name, content: blocks };
}

function main() {
  if (!fs.existsSync(INPUT)) {
    console.error(`Input not found: ${INPUT}`);
    process.exit(1);
  }

  const xml = fs.readFileSync(INPUT, "utf8");
  const parser = new XMLParser({
    ignoreAttributes: false,
    attributeNamePrefix: "@_",
    textNodeName: "#text",
    preserveOrder: true,
    trimValues: false,
    commentPropName: "#comment",
  });
  const doc = parser.parse(xml);
  const projectEl = doc.find((e) => e.project);
  if (!projectEl) {
    console.error("No <project> root found");
    process.exit(1);
  }

  const projectAttrs = projectEl[":@"] ?? {};
  const projectBody = projectEl.project;

  if (projectAttrs["@_themePath"] === "mvsc") {
    warn('themePath "mvsc" — browser Designer / local Tomcat may lack this theme; UI still opens');
  }
  if (findChild(projectBody, "pageHeader") != null) {
    warn("<pageHeader> dropped (absorbed into themePath; not in JSON schema)");
  }
  if (findChild(projectBody, "styles") != null) {
    warn("<styles> dropped (global fib/mc/text defaults not in JSON schema)");
  }
  if (findChild(projectBody, "imagedef") != null) {
    warn("<imagedef> image library not represented in JSON schema");
  }

  const formsWrap = findChild(projectBody, "forms") ?? [];
  const forms = [];
  for (const n of children(formsWrap)) {
    if (tagName(n) === "form") forms.push(convertForm(n));
  }

  const procsWrap = findChild(projectBody, "processes") ?? [];
  const processes = [];
  for (const n of children(procsWrap)) {
    if (tagName(n) === "process") processes.push(convertProcess(n));
  }

  const docsWrap = findChild(projectBody, "documents") ?? [];
  const documents = [];
  for (const n of children(docsWrap)) {
    if (tagName(n) === "document") documents.push(convertDocument(n));
  }

  const project = {
    name: projectAttrs["@_name"] ?? "SignupSheets",
    format: "2.0",
    themePath: projectAttrs["@_themePath"] ?? "mvsc",
    _originalFormat: projectAttrs["@_format"],
    _designerBuild: projectAttrs["@_designerBuild"],
    _convertedFrom: "designer-web/public/samples/legacy/SignupSheets.tawala.xml",
    forms,
    processes,
    documents,
  };

  fs.mkdirSync(path.dirname(OUTPUT), { recursive: true });
  fs.writeFileSync(OUTPUT, JSON.stringify(project, null, 2) + "\n", "utf8");

  // Sanity checks
  JSON.parse(fs.readFileSync(OUTPUT, "utf8"));
  console.log(`Wrote ${OUTPUT}`);
  console.log(
    `Forms: ${forms.length}, processes: ${processes.length}, documents: ${documents.length}, warnings: ${warnings.length}`,
  );
  console.log(
    `Items: ${forms.reduce((n, f) => n + f.items.length, 0)}, commands: ${processes.reduce((n, p) => n + countCmds(p.commands), 0)}`,
  );
}

function countCmds(cmds) {
  let n = 0;
  for (const c of cmds ?? []) {
    n += 1;
    if (c.then) n += countCmds(c.then);
    if (c.else) n += countCmds(c.else);
    if (c.do) n += countCmds(c.do);
  }
  return n;
}

main();
