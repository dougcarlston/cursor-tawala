/**
 * Legacy `.tawala` / project XML → browser Designer JSON (format 2.0).
 *
 * Shared by CLI (`scripts/tawala-to-json.mjs`) and File → Open.
 * Lossy-with-warnings — see SignupSheets_CONVERSION_GAPS.md and DESIGNER_OPEN_TODOS.
 */

import { XMLParser } from "fast-xml-parser";

/** @type {string[]} */
let warnings = [];

function warn(msg) {
  warnings.push(msg);
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

/**
 * Legacy display-function element ids that must become function chips.
 * Unknown tags fall through to recursive text extraction — that concatenates
 * child field paths (e.g. question-correlation-table → AbleNamePreferred dump).
 * itemization-table stays on its structured-node path.
 */
const DISPLAY_FUNCTION_IDS = new Set([
  "sum",
  "record-count",
  "question-correlation-table",
  "choice-tally-table",
  "response-totals-table",
  "popular-choice-correlation-table",
  "popular-choice-display",
  "popular-choice-count",
  "simple-list",
  "display-mcq-label",
  "display-image",
  "project-email-count",
  "paypal-single-item-button",
  "categorizer",
  "link-to-project-details",
  "export-team-roster",
]);

/** Chip title (legacy Insert → Function display name). */
const DISPLAY_FUNCTION_TITLES = {
  sum: "SUM",
  "record-count": "FORM RECORD COUNT",
  "question-correlation-table": "QUESTION CORRELATION TABLE",
  "choice-tally-table": "RESPONSE BAR GRAPH",
  "response-totals-table": "RESPONSE TOTALS",
  "popular-choice-correlation-table": "RANKED MULTIQUESTION RESPONSE LIST",
  "popular-choice-display": "RANKED RESPONSE NAME",
  "popular-choice-count": "RANKED RESPONSE COUNTS",
  "simple-list": "SINGLE QUESTION LIST",
  "display-mcq-label": "DISPLAY MULTIPLE-CHOICE QUESTION RESPONSES",
  "display-image": "DISPLAY IMAGE",
  "project-email-count": "PROJECT EMAIL COUNT",
  "paypal-single-item-button": "PAYPAL SINGLE ITEM PURCHASE BUTTON",
  categorizer: "CATEGORIZER",
  "link-to-project-details": "LINK TO PROJECT DETAILS IN MY TAWALA",
  "export-team-roster": "EXPORT TEAM ROSTER",
};

/** Params whose text is a Record:Form:Field / Form:Field path → strip Record:. */
const FIELD_PATH_PARAMS = new Set([
  "field",
  "field-name",
  "question-field-name",
  "display-field-name",
  "preferred-choice-field-name",
  "popular-choice-field-name",
  "choice-available-field-name",
  "choice-preferred-field-name",
  "popular-choice-display-field-name",
  "simple-list-field",
  "category-names",
  "category-ids",
  "category-storage-field",
  "status-field",
  "amount-field",
]);

/** Params that are nested expressions (`<string value|field>`, arithmetic, …). */
const EXPRESSION_PARAMS = new Set([
  "source",
  "width",
  "height",
  "alt_title",
  "description",
  "item",
  "amount",
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
        const name = a["@_name"] ?? flattenPlainText(body);
        if (name) out.push({ type: "field", name });
        break;
      }
      case "image": {
        const id = a["@_id"];
        if (!id) {
          warn("Inline <image> missing id — skipped");
          break;
        }
        const width = a["@_width"] != null ? Number(a["@_width"]) : undefined;
        const height = a["@_height"] != null ? Number(a["@_height"]) : undefined;
        const node = { type: "image", id };
        if (width != null && !Number.isNaN(width)) node.width = width;
        if (height != null && !Number.isNaN(height)) node.height = height;
        out.push(node);
        break;
      }
      case "invitation": {
        const form = a["@_form"] ?? "";
        const project = a["@_project"] ?? "";
        const priv = String(a["@_private"] ?? "").toLowerCase() === "true";
        // Auth first — display extraction must not treat auth <string value> as the label.
        // Sign-up Sheet ViewFinalList: private invitation with
        //   <authenticationTokenValue><string value="fromSignupSheet"/></…>click here
        // (label is #text; auth is a sibling expression — not <<Field>>).
        const auth = findChild(body, "authenticationTokenValue");
        let authToken = "";
        if (auth) {
          const authExpr = expressionToString(auth).trim();
          if (authExpr) {
            const m = authExpr.match(/^<<([^>]+)>>$/);
            // Keep <<Field>> as field name; keep literal strings as-is (fromSignupSheet).
            authToken = m ? m[1] : authExpr;
          }
        }
        // Legacy often puts label in <displayText><string value="…"/></displayText>
        // (Form Text). Documents more often use bare #text after authenticationTokenValue.
        const displayBody = findChild(body, "displayText");
        let text = "";
        if (displayBody != null) {
          text =
            expressionToString(displayBody) || flattenPlainText(displayBody);
        } else {
          // Prefer #text siblings — do NOT expressionToString(whole body): that walks
          // authenticationTokenValue and steals its <string value> as the display label.
          text = flattenPlainText(body);
          if (!text) {
            const withoutAuth = children(body).filter(
              (c) => tagName(c) !== "authenticationTokenValue",
            );
            text = expressionToString(withoutAuth);
          }
        }
        if (!text) {
          warn(
            `Document/text invitation → form="${form}" text="" (limited Designer round-trip)`,
          );
        }
        const inv = { type: "invitation", form, text, project };
        if (priv) inv.private = true;
        if (authToken) inv.authenticationTokenField = authToken;
        out.push(inv);
        break;
      }
      case "itemization-table":
        out.push(convertItemizationTable(n, ctx));
        break;
      case "link": {
        // Legacy: <link><new-window/><description>…</description><url>…</url>
        // description/url are <string value> or <field name> (not #text).
        const openNewWindow = children(body).some(
          (c) => tagName(c) === "new-window",
        );
        const descBody = findChild(body, "description");
        const urlBody = findChild(body, "url");
        const displayText = descBody
          ? expressionToString(descBody) || flattenPlainText(descBody)
          : "";
        const url = urlBody
          ? expressionToString(urlBody) || flattenPlainText(urlBody)
          : "";
        const dcNodes = findChild(body, "displayConditions");
        const cond = dcNodes ? parseConditions(dcNodes) : null;
        const conditions = [];
        let conditional = false;
        if (cond && typeof cond.field === "string") {
          conditional = true;
          conditions.push({
            field: cond.field,
            op: cond.op ?? "equals",
            value: String(cond.value ?? ""),
          });
        } else if (cond && Array.isArray(cond.and)) {
          conditional = true;
          for (const row of cond.and) {
            if (row && typeof row.field === "string") {
              conditions.push({
                field: row.field,
                op: row.op ?? "equals",
                value: String(row.value ?? ""),
              });
            }
          }
        } else if (cond && Array.isArray(cond.or)) {
          conditional = true;
          for (const row of cond.or) {
            if (row && typeof row.field === "string") {
              conditions.push({
                field: row.field,
                op: row.op ?? "equals",
                value: String(row.value ?? ""),
              });
            }
          }
        }
        if (!displayText && !url) {
          warn("Hyperlink in rich content — empty description and url");
        }
        out.push({
          type: "hyperlink",
          url,
          text: displayText,
          openNewWindow,
          conditional,
          conditions,
        });
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
        if (DISPLAY_FUNCTION_IDS.has(t)) {
          out.push(convertDisplayFunction(n, t));
          break;
        }
        // Unknown wrappers: recurse. Do NOT do this for display functions —
        // their parameter text would concatenate onto the page.
        out.push(...richNodesFromXml(body, ctx));
    }
  }
  return out;
}

/** Strip leading `Record:` so Design Configure uses Form:Field like Insert → Function. */
function designFieldFromRecordPath(raw) {
  let s = String(raw ?? "").trim();
  if (!s) return "";
  s = s.replace(/^<<|>>$/g, "");
  if (/^Record:/i.test(s)) s = s.slice("Record:".length);
  return s;
}

/**
 * Read a display-function parameter body: plain text, or nested expression nodes.
 */
function displayFunctionParamRaw(paramTag, body) {
  const kids = children(body);
  const hasElements = kids.some((k) => tagName(k));
  if (EXPRESSION_PARAMS.has(paramTag) || hasElements) {
    const expr = expressionToString(body);
    if (expr) return expr;
  }
  return (
    textOf(body ?? []).trim() ||
    flattenPlainText(body ?? []) ||
    ""
  );
}

/**
 * Legacy `<conditions><form name="X"/>…` (+ optional nested filter) → Configure rows.
 */
function extractDisplayFunctionConditions(condBody) {
  const formNames = [];
  let where;
  const kids = children(condBody);
  for (const inner of kids) {
    const it = tagName(inner);
    if (it === "form") {
      const name = attr(inner, "name");
      if (name) formNames.push(name);
    } else if (it === "conditions") {
      where = parseConditions(inner.conditions);
    }
  }
  // Rare: ops directly under conditions without a nested <conditions> wrapper.
  if (!where) {
    const hasOp = kids.some((k) => {
      const t = tagName(k);
      return t && (CONDITION_OPS.has(t) || t === "and" || t === "or");
    });
    if (hasOp) where = parseConditions(condBody);
  }
  const flat = flattenWhereToConditionRows(where);
  const rows = (flat?.rows ?? [{ field: "", op: "equals", value: "" }]).map((r) => ({
    field: designFieldFromRecordPath(r.field),
    op: r.op,
    value: String(r.value ?? ""),
  }));
  return {
    formNames,
    conditionsRows: rows,
    conditionsCombinator: flat?.combinator ?? "and",
  };
}

/**
 * Legacy display-function XML → `{ type: "function", functionId, config }` chip node.
 * Covers Question Correlation, Form Record Count, Response Totals, etc.
 * (itemization-table stays on convertItemizationTable.)
 */
function convertDisplayFunction(wrapNode, functionId) {
  const body = wrapNode[functionId] ?? [];
  const config = {};
  let sawConditions = false;

  for (const c of children(body)) {
    const t = tagName(c);
    if (!t) continue;
    if (t === "conditions") {
      sawConditions = true;
      const extracted = extractDisplayFunctionConditions(c.conditions);
      config.conditionsRows = extracted.conditionsRows;
      config.conditionsCombinator = extracted.conditionsCombinator;
      if (extracted.formNames[0] && !config["form-name"]) {
        // record-count prefers explicit <form-name>; others infer form from field paths.
        if (functionId === "record-count") {
          config["form-name"] = extracted.formNames[0];
        }
      }
      continue;
    }
    if (t === "column" || t === "number-of-columns") {
      // categorizer / paypal column collections — preserve via warn; chip still emitted.
      if (!config._skippedColumns) {
        config._skippedColumns = true;
        warn(
          `Document <${functionId}> has column collection — chip imported without columns (Configure to restore)`,
        );
      }
      continue;
    }
    if (t === "show-print-control" || t === "show-export-control") {
      const raw =
        textOf(c[t] ?? []).trim() ||
        flattenPlainText(c[t] ?? []) ||
        "";
      config[t] = raw.toLowerCase() === "true" ? "true" : "false";
      continue;
    }

    const raw =
      displayFunctionParamRaw(t, c[t] ?? []) ||
      (t === "field" ? attr(c, "name") || "" : "");
    if (FIELD_PATH_PARAMS.has(t)) {
      config[t] = designFieldFromRecordPath(raw);
    } else {
      config[t] = raw;
    }
  }

  // Always give Configure a conditions scaffold when the catalog expects it.
  if (!sawConditions && functionId !== "project-email-count" && functionId !== "display-mcq-label" && functionId !== "display-image" && functionId !== "link-to-project-details" && functionId !== "export-team-roster") {
    config.conditionsRows = config.conditionsRows ?? [
      { field: "", op: "equals", value: "" },
    ];
    config.conditionsCombinator = config.conditionsCombinator ?? "and";
  }

  delete config._skippedColumns;

  if (functionId === "sum" && !config.field) {
    warn("Document <sum> missing field — placeholder chip still emitted");
  }
  if (functionId === "question-correlation-table" && !config["question-field-name"]) {
    warn("Document <question-correlation-table> missing question-field-name — placeholder chip still emitted");
  }
  if (functionId === "record-count" && !config["form-name"]) {
    warn("Document <record-count> missing form-name — placeholder chip still emitted");
  }

  // sum used key `field` historically; keep that shape.
  return { type: "function", functionId, config };
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
  if (where) {
    node.where = where;
    // Flatten simple Where trees into Configure Function rows so Design can edit
    // (Deploy reads `conditions` / `conditionsRows`; nested `where` alone was dropped).
    const flat = flattenWhereToConditionRows(where);
    if (flat?.rows?.length) {
      node.conditions = flat.rows.map((r) => ({
        field: designFieldFromRecordPath(r.field),
        op: r.op,
        value: r.value,
      }));
      node.combinator = flat.combinator ?? "and";
    } else {
      warn(
        `Itemization table filter conditions preserved as where (Designer UI limited): ${JSON.stringify(where)}`,
      );
    }
  }
  return node;
}

/**
 * True when a form/document item body uses `<paragraph>` / `<division>` /
 * `<table>` structure (modern Designer export). Older projects (Living Will,
 * Wildcat Week, …) put plain text and `<field>` chips directly under the item.
 */
function itemBodyHasParagraphStructure(itemBody) {
  if (!Array.isArray(itemBody)) return false;
  return children(itemBody).some((n) => {
    const t = tagName(n);
    return t === "paragraph" || t === "division" || t === "table";
  });
}

/**
 * Legacy flat item bodies: text (and mixed field/font) with no <paragraph>.
 * `children()` drops #text, so those used to convert as empty.
 */
function flatItemBodyToBlocks(itemBody, ctx = {}) {
  const blocks = [];
  if (!Array.isArray(itemBody) || itemBody.length === 0) return blocks;

  const contentNodes = itemBody.filter((n) => {
    if (n?.["#text"] != null) return true;
    const t = tagName(n);
    return t != null && t !== "displayConditions";
  });
  if (contentNodes.length === 0) return blocks;

  const onlyPlainText = contentNodes.every((n) => n?.["#text"] != null);
  if (onlyPlainText) {
    const full = textOf(contentNodes).replace(/\r\n/g, "\n");
    const paras = full
      .split(/\n\s*\n/)
      .map((s) => s.replace(/\n/g, " ").replace(/\s+/g, " ").trim())
      .filter(Boolean);
    for (const p of paras) {
      blocks.push({
        type: "paragraph",
        align: "left",
        indent: 0,
        nodes: [{ type: "text", text: p }],
      });
    }
    return blocks;
  }

  // Mixed text + field / font / invitation / image at item root.
  blocks.push({
    type: "paragraph",
    align: "left",
    indent: 0,
    nodes: richNodesFromXml(contentNodes, ctx),
  });
  return blocks;
}

function paragraphsToBlocks(itemBody, ctx = {}) {
  const blocks = [];
  for (const n of children(itemBody)) {
    const t = tagName(n);
    if (t === "paragraph" || t === "division") {
      // Legacy Form Text tables put cell body in <division>, not <paragraph>.
      const a = attrs(n);
      const body = n[t];
      blocks.push({
        type: "paragraph",
        align: a["@_align"] ?? "left",
        indent: a["@_indent"] != null ? Number(a["@_indent"]) : 0,
        nodes: richNodesFromXml(body, ctx),
      });
    } else if (t === "displayConditions") {
      // handled separately
    } else if (t === "table") {
      blocks.push(...tableToBlocks(n.table, ctx, attrs(n)));
    } else if (t === "font" || t === "b" || t === "i" || t === "u" || t === "field") {
      // Bare rich nodes at cell root (rare) — wrap as one paragraph.
      blocks.push({
        type: "paragraph",
        align: "left",
        indent: 0,
        nodes: richNodesFromXml([n], ctx),
      });
    } else if (
      t === "image" ||
      t === "invitation" ||
      t === "link" ||
      t === "itemization-table" ||
      DISPLAY_FUNCTION_IDS.has(t)
    ) {
      blocks.push({
        type: "paragraph",
        align: "left",
        indent: 0,
        nodes: richNodesFromXml([n], ctx),
      });
    }
  }

  // Older .tawala form items: no <paragraph> wrappers (Publishable Living Will, etc.)
  if (blocks.length === 0 && !itemBodyHasParagraphStructure(itemBody)) {
    blocks.push(...flatItemBodyToBlocks(itemBody, ctx));
  }
  return blocks;
}

function tableToBlocks(tableBody, ctx, tableAttrs = {}) {
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
  const borderRaw = tableAttrs["@_border"] ?? tableAttrs.border;
  let border = 1;
  if (borderRaw != null && String(borderRaw).trim() !== "") {
    const s = String(borderRaw).trim();
    if (s === "0" || /^none$/i.test(s)) border = 0;
    else {
      const n = Number(s);
      border = Number.isFinite(n) ? (n <= 0 ? 0 : n >= 2 ? 2 : 1) : 1;
    }
  }
  return [{ type: "table", rows, border }];
}

function blocksAreSimplePlain(blocks) {
  if (!blocks.length) return true;
  return blocks.every((b) => {
    if (b.type !== "paragraph") return false;
    const nodes = b.nodes ?? [];
    if (nodes.length === 0) return true;
    // only text / font / bold / italic wrapping text — no fields/itemization/invitation/image
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

function blocksHaveType(blocks, type) {
  const walk = (ns) => {
    for (const n of ns ?? []) {
      if (n.type === type) return true;
      if (n.nodes && walk(n.nodes)) return true;
    }
    return false;
  };
  for (const b of blocks ?? []) {
    if (b.type === type) return true;
    if (walk(b.nodes)) return true;
    if (b.rows) {
      for (const row of b.rows) {
        for (const cell of row.cells ?? []) {
          if (blocksHaveType(cell.content, type)) return true;
        }
      }
    }
  }
  return false;
}

function escHtml(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

/** Field chip HTML — brackets must be entities or innerHTML parses `<<Name>>` as tags. */
function fieldTokenHtml(name) {
  const n = String(name ?? "").trim();
  if (!n) return "";
  return (
    `<span class="field-token function-table-token" contenteditable="false" ` +
    `data-field-name="${escHtml(n)}" title="${escHtml(n)}" draggable="true">` +
    `&lt;&lt;${escHtml(n)}&gt;&gt;</span>`
  );
}

let nextImportedFunctionInstanceId = 1;

/** Display-function chip HTML with escaped `<<NAME(…)>>` text. */
function displayFunctionTokenHtml(functionId, config) {
  const id = String(functionId ?? "").trim() || "sum";
  const inst = nextImportedFunctionInstanceId++;
  const confJson = JSON.stringify(config ?? {});
  const title = DISPLAY_FUNCTION_TITLES[id] ?? id.toUpperCase().replace(/-/g, " ");
  const parts = [];
  // Stable param order for chip label (mirrors Insert → Function display).
  const order = [
    "form-name",
    "rank",
    "layout-type",
    "field",
    "field-name",
    "question-field-name",
    "display-field-name",
    "preferred-choice-field-name",
    "popular-choice-field-name",
    "choice-available-field-name",
    "choice-preferred-field-name",
    "popular-choice-display-field-name",
    "simple-list-field",
    "display",
    "source",
    "description",
    "open-preference",
  ];
  for (const key of order) {
    const v = config?.[key];
    if (v === undefined || v === null) continue;
    const s = String(v).trim();
    if (!s) continue;
    parts.push(s.length > 36 ? `${s.slice(0, 35)}…` : s);
  }
  const rows = Array.isArray(config?.conditionsRows) ? config.conditionsRows : [];
  for (const row of rows) {
    const field = String(row?.field ?? "").trim();
    if (!field) continue;
    const op = String(row?.op ?? "equals").trim();
    const value = String(row?.value ?? "").trim();
    if (op === "isBlank" || op === "isNotBlank" || op === "mcIsBlank" || op === "mcIsNotBlank") {
      const label =
        op === "isNotBlank" || op === "mcIsNotBlank" ? "is not blank" : "is blank";
      parts.push(`${field} ${label}`);
    } else if (value) {
      parts.push(`${field} ${op} ${value}`);
    } else {
      parts.push(field);
    }
  }
  const display =
    parts.length > 0 ? `<<${title}(${parts.join(", ")})>>` : `<<${title}>>`;
  return (
    `<span class="function-token function-table-token" contenteditable="false" ` +
    `data-function-id="${escHtml(id)}" data-function-instance="${inst}" ` +
    `data-function-config="${escHtml(confJson)}" title="${escHtml(title)}">` +
    `${escHtml(display)}</span>`
  );
}

/**
 * Turn plain / entity-encoded `<<Field>>` in imported htmlData into field chips.
 * Protects existing field-token / function-token spans.
 */
function embedFieldsInImportedHtml(source) {
  const chips = [];
  let s = String(source ?? "").replace(
    /<span\b[^>]*\b(?:field-token|function-token|invitation-token|hyperlink-token)\b[^>]*>[\s\S]*?<\/span>/gi,
    (chip) => {
      const i = chips.length;
      chips.push(chip);
      return `\u0000CHIP${i}\u0000`;
    },
  );
  s = s.replace(/&lt;&lt;([^&<>]+)&gt;&gt;/g, (_, name) => fieldTokenHtml(String(name).trim()));
  s = s.replace(/<<([^<>]+)>>/g, (_, name) => fieldTokenHtml(String(name).trim()));
  return s.replace(/\u0000CHIP(\d+)\u0000/g, (_, i) => chips[Number(i)] ?? "");
}

function dataUrlForImageDef(img) {
  if (!img?.data) return "";
  const mime =
    img.imageFormat === "GIF"
      ? "image/gif"
      : img.imageFormat === "JPEG"
        ? "image/jpeg"
        : "image/png";
  return `data:${mime};base64,${String(img.data).replace(/\s+/g, "")}`;
}

/** Rich nodes → Form Text HTML (TextCanvasRow / Preview string path). */
function richNodesToFormHtml(nodes, imageById = {}) {
  let out = "";
  for (const n of nodes ?? []) {
    if (n.type === "text") {
      out += escHtml(n.text ?? "");
      continue;
    }
    if (n.type === "image") {
      const def = imageById[n.id];
      const src = def ? dataUrlForImageDef(def) : "";
      const w = n.width != null && n.width > 0 ? Math.round(n.width) : 0;
      const h = n.height != null && n.height > 0 ? Math.round(n.height) : 0;
      const sizeAttrs =
        w > 0 && h > 0
          ? ` width="${w}" height="${h}" data-image-width="${w}" data-image-height="${h}" style="width:${w}px;height:${h}px"`
          : "";
      out +=
        `<img class="tawala-embedded-image" data-tawala-image-id="${escHtml(n.id)}"${sizeAttrs}` +
        ` src="${src}" alt="" />`;
      continue;
    }
    if (n.type === "field") {
      const name = String(n.name ?? "");
      out += fieldTokenHtml(name);
      continue;
    }
    if (n.type === "function") {
      out += displayFunctionTokenHtml(n.functionId, n.config ?? {});
      continue;
    }
    if (n.type === "bold") {
      out += `<b>${richNodesToFormHtml(n.nodes, imageById)}</b>`;
      continue;
    }
    if (n.type === "italic") {
      out += `<i>${richNodesToFormHtml(n.nodes, imageById)}</i>`;
      continue;
    }
    if (n.type === "underline") {
      out += `<u>${richNodesToFormHtml(n.nodes, imageById)}</u>`;
      continue;
    }
    if (n.type === "font") {
      const styles = [];
      if (n.face) styles.push(`font-family:${n.face}`);
      if (n.size != null) styles.push(`font-size:${n.size}pt`);
      if (n.color) styles.push(`color:${n.color}`);
      const styleAttr = styles.length ? ` style="${escHtml(styles.join(";"))}"` : "";
      out += `<span${styleAttr}>${richNodesToFormHtml(n.nodes, imageById)}</span>`;
      continue;
    }
    if (n.type === "itemizationTable") {
      const enc = encodeURIComponent(JSON.stringify(n));
      const form = String(n.form ?? "");
      const hasColDc = Array.isArray(n.columns) && n.columns.some((c) => c?.displayCondition != null);
      const warnClass = hasColDc ? " preserved-import-warning" : "";
      const title = hasColDc
        ? "MULTIPLE QUESTION LIST — Visibility condition preserved; Designer cannot edit yet"
        : "MULTIPLE QUESTION LIST";
      out +=
        `<span contenteditable="false" class="function-table-inline function-table-token${warnClass}" ` +
        `data-itemization-token="true" data-itemization-form="${escHtml(form)}" ` +
        `data-tawala-structured-node="${escHtml(enc)}" title="${escHtml(title)}">` +
        `{ MULTIPLE QUESTION LIST }</span>`;
      continue;
    }
    if (n.type === "invitation") {
      const draft = {
        form: String(n.form ?? ""),
        project: String(n.project ?? ""),
        displayText: String(n.text ?? n.form ?? "Invitation"),
        isPrivate: Boolean(n.private),
        authToken: String(n.authenticationTokenField ?? ""),
      };
      // Match Design Insert tokens: raw JSON in the attribute (escHtml for the attr).
      const confJson = JSON.stringify(draft);
      const label = escHtml(draft.displayText || draft.form || "Invitation");
      out +=
        `<span contenteditable="false" class="invitation-token" ` +
        `data-invitation-config="${escHtml(confJson)}" ` +
        `style="color:#000080;text-decoration:underline">${label}</span>`;
      continue;
    }
    if (n.type === "hyperlink") {
      const draft = {
        url: String(n.url ?? ""),
        displayText: String(n.text ?? ""),
        openNewWindow: Boolean(n.openNewWindow),
        conditional: Boolean(n.conditional),
        conditions: Array.isArray(n.conditions)
          ? n.conditions.map((r) => ({
              field: String(r?.field ?? ""),
              op: String(r?.op ?? "equals"),
              value: String(r?.value ?? ""),
            }))
          : [],
      };
      const confJson = JSON.stringify(draft);
      const label = escHtml(
        draft.displayText.trim() ||
          draft.url.trim() ||
          "(Link appears here)",
      );
      out +=
        `<span contenteditable="false" class="hyperlink-token" ` +
        `data-hyperlink-config="${escHtml(confJson)}" ` +
        `style="color:#000080;text-decoration:underline">${label}</span>`;
      continue;
    }
    if (n.nodes) out += richNodesToFormHtml(n.nodes, imageById);
  }
  return out;
}

function richBlocksToFormHtml(blocks, imageById = {}) {
  return (blocks ?? [])
    .map((b) => {
      if (b.type === "paragraph") {
        const align =
          b.align && b.align !== "left" ? ` style="text-align:${escHtml(b.align)}"` : "";
        return `<p${align}>${richNodesToFormHtml(b.nodes, imageById)}</p>`;
      }
      if (b.type === "text") return `<p>${escHtml(b.text ?? "")}</p>`;
      if (b.type === "table") {
        const indentTwips = b.indent != null ? Number(b.indent) : 0;
        const indentPt = indentTwips > 0 ? indentTwips / 20 : 0;
        const margin =
          indentPt > 0 ? ` style="margin-left:${indentPt}pt"` : "";
        const borderN = b.border == null ? 1 : Number(b.border);
        const border =
          !Number.isFinite(borderN) || borderN < 0 ? 1 : borderN <= 0 ? 0 : borderN >= 2 ? 2 : 1;
        const borderClass =
          border === 0
            ? "user user-border-none"
            : border === 2
              ? "user user-border-2"
              : "user user-border-1";
        const rows = (b.rows ?? [])
          .map((row) => {
            const cells = (row.cells ?? [])
              .map((cell) => {
                const w = cell.width != null && cell.width > 0 ? Math.round(cell.width) : 0;
                // Legacy cell width is twips (~1/20 pt); Design tables use px — approximate.
                const widthPx = w > 0 ? Math.max(40, Math.round(w / 15)) : 0;
                const widthAttr = widthPx > 0 ? ` style="width:${widthPx}px"` : "";
                const inner = richBlocksToFormHtml(cell.content ?? [], imageById);
                return `<td${widthAttr}>${inner || "<br>"}</td>`;
              })
              .join("");
            return `<tr>${cells}</tr>`;
          })
          .join("");
        return `<table class="${borderClass}" border="${border}" cellpadding="4" cellspacing="0"${margin}>${rows}</table>`;
      }
      return "";
    })
    .join("");
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

function convertText(itemNode, imageById = {}) {
  const body = itemNode.text;
  const label = attr(itemNode, "label") ?? "T1";
  const name = attr(itemNode, "alternateLabel");
  const style = attr(itemNode, "style") ?? "normal";
  const blocks = paragraphsToBlocks(body, { location: `text ${label}`, imageById });
  const item = { type: "text", label, style };
  if (name) item.name = name;
  const hasMql = blocksHaveType(blocks, "itemizationTable");

  // Prefer Form Text HTML (TextCanvasRow) for fields / invitations / hyperlinks /
  // tables / images so Design can edit. Keep structured arrays only for MQL
  // (itemization) tables that StructuredTextCanvasRow still owns.
  if (hasMql) {
    item.content = blocks;
  } else if (!blocksAreSimplePlain(blocks)) {
    item.content = richBlocksToFormHtml(blocks, imageById);
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
  const name = attr(itemNode, "alternateLabel");
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
        // Design canvas maps each `_+` run → one blank (fibBlanks.parseUnderscoreRuns).
        // Legacy often puts one <blank> per paragraph with no inter-text; joining
        // underscore runs without a break elides multi-choice FIBs (C1 / Online Exam Q5).
        const last = promptParts[promptParts.length - 1];
        if (typeof last === "string" && /^_+$/.test(last)) {
          promptParts.push("\n");
        }
        const len = Math.max(1, Number(blank.length) || 20);
        promptParts.push("_".repeat(len));
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

  const paragraphKids = children(body).filter((n) => tagName(n) === "paragraph");
  if (paragraphKids.length > 0) {
    for (const n of paragraphKids) {
      const beforeLen = promptParts.length;
      walkParagraph(n.paragraph);
      // Preserve paragraph structure so blanks on successive lines stay separate runs.
      if (promptParts.length > beforeLen) {
        const last = promptParts[promptParts.length - 1];
        if (last !== "\n") promptParts.push("\n");
      }
    }
  } else {
    // Flat legacy FIB (Living Will / Wildcat Week): text + <blank> under <fib>, no <paragraph>.
    // Pass the full body array so #text nodes are not dropped by children().
    walkParagraph(body ?? []);
  }

  // Build prompt: text + underscore runs for each <blank> (Design idle contract).
  // DirtBowl uses / between blank regions — approximate from collected text.
  let prompt = promptParts
    .join("")
    .replace(/\r\n/g, "\n")
    // Collapse horizontal whitespace but keep newlines (blank separators).
    .replace(/[^\S\n]+/g, " ")
    .replace(/ *\n */g, "\n")
    .replace(/\n{3,}/g, "\n\n")
    .replace(/ +/g, " ")
    .trim();

  const item = { type: "fib", label, style, prompt, blanks };
  if (name) item.name = name;
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
    // Prefer flat condition rows when the filter is a simple and/or list so
    // Configure Function (Edit) can reopen them; nested trees stay in `where`.
    const flat = flattenWhereToConditionRows(where);
    if (flat) {
      choice.conditionsRows = flat.rows;
      choice.conditionsCombinator = flat.combinator;
    } else {
      warn(
        `Dynamic MCQ ${mcLabel} has a nested record-selector filter; Edit shows Form/Display/Value — reopen conditions manually if needed: ${JSON.stringify(where)}`,
      );
    }
  }
  return choice;
}

/** Simple and/or / single-op where → Configure Function rows (shared with mcDynamicConfig). */
function flattenWhereToConditionRows(where) {
  if (!where || typeof where !== "object") return null;
  if (typeof where.field === "string" && typeof where.op === "string") {
    return {
      rows: [{ field: where.field, op: where.op, value: String(where.value ?? "") }],
      combinator: "and",
    };
  }
  for (const combinator of ["and", "or"]) {
    const list = where[combinator];
    if (!Array.isArray(list) || !list.length) continue;
    const rows = [];
    for (const item of list) {
      if (!item || typeof item !== "object") return null;
      if (typeof item.field !== "string" || typeof item.op !== "string") return null;
      if ("and" in item || "or" in item) return null;
      rows.push({ field: item.field, op: item.op, value: String(item.value ?? "") });
    }
    return { rows, combinator };
  }
  return null;
}

function convertMc(itemNode) {
  const body = itemNode.mc;
  const label = attr(itemNode, "label") ?? "Q1";
  const name = attr(itemNode, "alternateLabel");
  const onlyone = boolAttr(itemNode, "onlyone", true);
  const required = boolAttr(itemNode, "required", false);
  const style = attr(itemNode, "style") ?? "vertical";
  const paddingBottom = attr(itemNode, "paddingBottom");

  // Plain <<field>> text (not chip HTML). Design must embed before innerHTML — see C5 / McqCanvasRow.
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
    const inviteTo = a["@_inviteTo"];
    if (a["@_document"] != null && String(a["@_document"]).trim() !== "") {
      cmd.body = {
        document: a["@_document"],
        reset: String(a["@_reset"] ?? "false").toLowerCase() === "true",
        showHeader: String(a["@_showHeader"] ?? "true").toLowerCase() !== "false",
      };
      if (inviteTo) cmd.body.inviteTo = inviteTo;
    } else {
      // Static body text (cs v95, Alumni List Builder): plain text under <body>,
      // optionally inviteTo="FormName". Must round-trip — Java Send requires a body child.
      const raw = textOf(bodyDoc.body ?? []).replace(/\r\n/g, "\n");
      cmd.body = { text: raw };
      if (inviteTo) cmd.body.inviteTo = inviteTo;
      if (String(a["@_reset"] ?? "").toLowerCase() === "true") cmd.body.reset = true;
      if (String(a["@_showHeader"] ?? "").toLowerCase() === "false") {
        cmd.body.showHeader = false;
      }
    }
  }

  const bodyHint = cmd.body?.document
    ? `document=${cmd.body.document}`
    : cmd.body?.text != null
      ? `text(${String(cmd.body.text).length} chars)`
      : "";
  warn(
    `Send command subject="${cmd.subject ?? ""}" body="${bodyHint}" (email deploy path limited in browser Designer)`,
  );
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

function convertForm(formNode, imageById = {}) {
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
        form.items.push(convertText(n, imageById));
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

/**
 * Document → Design-ready HTML string.
 *
 * Prefer `<xmlData>` (rich paragraphs/tables). Fall back to `<htmlData>` CDATA when
 * xmlData is missing (common in older TX Text docs). `rawHtmlData` is full-page HTML
 * and is still stripped — Design canvas wants body fragments only.
 */
function convertDocument(docNode, imageById = {}) {
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
        const tableBlocks = tableToBlocks(n.table, { location: `document ${name}` }, attrs(n));
        const indentTwips = attr(n, "indent");
        if (indentTwips != null && tableBlocks[0]) {
          const tw = Number(indentTwips);
          if (!Number.isNaN(tw) && tw > 0) tableBlocks[0].indent = tw;
        }
        blocks.push(...tableBlocks);
      }
    }
  }

  if (blocks.length > 0) {
    return { name, content: richBlocksToFormHtml(blocks, imageById) };
  }

  const htmlData = findChild(body, "htmlData");
  if (htmlData) {
    const html = textOf(htmlData).trim();
    if (html) {
      return { name, content: embedFieldsInImportedHtml(html) };
    }
  }

  // Strip rawHtmlData intentionally (full HTML document shell — not Design canvas HTML).
  if (findChild(body, "rawHtmlData")) {
    warn(
      `Document ${name}: rawHtmlData stripped (legacy full-page HTML); no xmlData/htmlData body found`,
    );
  } else if (!xmlData) {
    warn(`Document ${name}: empty (no xmlData or htmlData)`);
  }
  return { name, content: "" };
}

/**
 * Parse one `<imagedef id="…"><imagedata imageFormat="PNG">base64</imagedata></imagedef>`.
 * @returns {{ id: string, imageFormat: string, data: string } | null}
 */
function convertImageDef(imgNode) {
  const id = attr(imgNode, "id");
  if (!id) {
    warn("<imagedef> missing id — skipped");
    return null;
  }
  const body = imgNode.imagedef ?? imgNode;
  let imageFormat = "PNG";
  let data = "";
  for (const n of children(body)) {
    if (tagName(n) !== "imagedata") continue;
    imageFormat = (attr(n, "imageFormat") ?? "PNG").toUpperCase();
    data = textOf(n.imagedata ?? []).replace(/\s+/g, "");
    break;
  }
  if (!data) {
    warn(`<imagedef id="${id}"> has empty imagedata — skipped`);
    return null;
  }
  if (imageFormat === "JPG") imageFormat = "JPEG";
  if (!/^(PNG|GIF|JPEG)$/.test(imageFormat)) imageFormat = "PNG";
  return { id, imageFormat, data };
}

/**
 * `<pageHeader><text>…</text><image id width height/></pageHeader>` → JSON.
 * Nested `<field>` inside text is flattened (plain banner line in Designer dialog).
 */
function convertPageHeader(projectBody) {
  let headerBody = null;
  for (const n of children(projectBody)) {
    if (tagName(n) === "pageHeader") {
      headerBody = n.pageHeader;
      break;
    }
  }
  if (headerBody == null) return undefined;

  let text = "";
  let imageId = "";
  let width;
  let height;
  for (const n of children(headerBody)) {
    const t = tagName(n);
    if (t === "text") {
      const body = n.text;
      if (children(body).some((c) => tagName(c) === "field")) {
        warn("<pageHeader> text field refs flattened to plain text");
      }
      text = flattenPlainText(body);
    } else if (t === "image") {
      imageId = String(attr(n, "id") ?? "").trim();
      const w = Number(attr(n, "width"));
      const h = Number(attr(n, "height"));
      if (Number.isFinite(w) && w > 0) width = Math.round(w);
      if (Number.isFinite(h) && h > 0) height = Math.round(h);
    }
  }

  if (!text && !imageId) return undefined;
  const out = {};
  if (text) out.text = text;
  if (imageId) {
    out.imageId = imageId;
    if (width != null) out.width = width;
    if (height != null) out.height = height;
  }
  return out;
}

/** Collect `<images><imagedef>…` and bare `<imagedef>` under project root. */
function convertProjectImages(projectBody) {
  const out = [];
  const seen = new Set();
  const push = (node) => {
    const img = convertImageDef(node);
    if (!img || seen.has(img.id)) return;
    seen.add(img.id);
    out.push(img);
  };

  const imagesWrap = findChild(projectBody, "images");
  if (imagesWrap) {
    for (const n of children(imagesWrap)) {
      if (tagName(n) === "imagedef") push(n);
    }
  }
  for (const n of children(projectBody)) {
    if (tagName(n) === "imagedef") push(n);
  }
  return out;
}

/**
 * Convert legacy project XML string → format 2.0 JSON project + warnings.
 *
 * @param {string} xmlString
 * @param {{ sourceLabel?: string }} [options]
 * @returns {{ project: object, warnings: string[] }}
 */
export function convertTawalaXmlToProject(xmlString, options = {}) {
  warnings = [];
  nextImportedFunctionInstanceId = 1;
  const sourceLabel = options.sourceLabel ?? undefined;

  const parser = new XMLParser({
    ignoreAttributes: false,
    attributeNamePrefix: "@_",
    textNodeName: "#text",
    preserveOrder: true,
    trimValues: false,
    commentPropName: "#comment",
  });
  const doc = parser.parse(xmlString);
  if (!Array.isArray(doc)) {
    throw new Error("Failed to parse project XML");
  }
  const projectEl = doc.find((e) => e.project);
  if (!projectEl) {
    throw new Error("No <project> root found");
  }

  const projectAttrs = projectEl[":@"] ?? {};
  const projectBody = projectEl.project;

  if (projectAttrs["@_themePath"] === "mvsc") {
    warn('themePath "mvsc" — browser Designer / local Tomcat may lack this theme; UI still opens');
  }
  if (findChild(projectBody, "styles") != null) {
    warn("<styles> dropped (global fib/mc/text defaults not in JSON schema)");
  }

  // Images first so Form Text / Document inline <image id> can embed data-URLs.
  const images = convertProjectImages(projectBody);
  const imageById = Object.fromEntries(images.map((img) => [img.id, img]));
  const pageHeader = convertPageHeader(projectBody);

  const formsWrap = findChild(projectBody, "forms") ?? [];
  const forms = [];
  for (const n of children(formsWrap)) {
    if (tagName(n) === "form") forms.push(convertForm(n, imageById));
  }

  const procsWrap = findChild(projectBody, "processes") ?? [];
  const processes = [];
  for (const n of children(procsWrap)) {
    if (tagName(n) === "process") processes.push(convertProcess(n));
  }

  const docsWrap = findChild(projectBody, "documents") ?? [];
  const documents = [];
  for (const n of children(docsWrap)) {
    if (tagName(n) === "document") documents.push(convertDocument(n, imageById));
  }

  const project = {
    name: projectAttrs["@_name"] ?? "Untitled",
    format: "2.0",
    themePath: projectAttrs["@_themePath"] ?? "default",
    forms,
    processes,
    documents,
  };
  if (images.length) project.images = images;
  if (pageHeader) project.pageHeader = pageHeader;
  if (projectAttrs["@_format"] != null) project._originalFormat = projectAttrs["@_format"];
  if (projectAttrs["@_designerBuild"] != null) {
    project._designerBuild = projectAttrs["@_designerBuild"];
  }
  if (sourceLabel) project._convertedFrom = sourceLabel;

  return { project, warnings: [...warnings] };
}

/** Nested command count (if/foreach bodies). */
export function countProcessCommands(cmds) {
  let n = 0;
  for (const c of cmds ?? []) {
    n += 1;
    if (c.then) n += countProcessCommands(c.then);
    if (c.else) n += countProcessCommands(c.else);
    if (c.do) n += countProcessCommands(c.do);
  }
  return n;
}

/** True when a filename looks like a legacy Tawala project XML file. */
export function isTawalaProjectFileName(filename) {
  const name = filename.trim().toLowerCase();
  if (name.endsWith(".tawala")) return true;
  if (name.endsWith(".tawala.xml")) return true;
  // Bare .xml only when the leaf suggests a project (avoid random XML).
  if (name.endsWith(".xml") && (name.includes("tawala") || name.includes(".tawala."))) {
    return true;
  }
  return false;
}
