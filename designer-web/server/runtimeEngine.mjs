/** Dev runtime v2 — skip, conditions, basic process commands, field refs */

export function itemKey(item) {
  return item.alternateLabel || item.name || item.label;
}

const FIB_NAME_TO_ITEM = { Friend: "Q10" };

export function getFieldValue(ctx, fieldRef) {
  if (!fieldRef) return "";
  const f = String(fieldRef);
  if (ctx.fields[f] !== undefined) return ctx.fields[f];
  if (ctx.formFields) {
    const parts = f.split(":");
    if (parts.length === 3) {
      const [a, b, c] = parts;
      const itemLabel = FIB_NAME_TO_ITEM[b] ?? b;
      const itemBlank = `${itemLabel}:${c}`;
      if (ctx.formFields[a]) {
        if (ctx.formFields[a][itemBlank] !== undefined) return ctx.formFields[a][itemBlank];
        if (ctx.formFields[a][`${b}:${c}`] !== undefined) return ctx.formFields[a][`${b}:${c}`];
        if (!FIB_NAME_TO_ITEM[b] && ctx.formFields[a][c] !== undefined) return ctx.formFields[a][c];
      }
      if (ctx.fields[itemBlank] !== undefined) return ctx.fields[itemBlank];
      if (ctx.fields[`${b}:${c}`] !== undefined) return ctx.fields[`${b}:${c}`];
      if (!FIB_NAME_TO_ITEM[b] && ctx.fields[`${a}:${c}`] !== undefined) return ctx.fields[`${a}:${c}`];
      const bound = ctx.recordBindings?.[a];
      if (bound && bound[c] !== undefined) return bound[c];
      const list = ctx.records?.[b];
      if (Array.isArray(list) && list[0]) return list[0][c] ?? "";
    }
    if (parts.length === 2) {
      const [first, blank] = parts;
      const itemBlank = `${first}:${blank}`;
      if (ctx.fields[itemBlank] !== undefined) return ctx.fields[itemBlank];
      const ff = ctx.formFields[ctx.formName ?? "Registration"];
      if (ff?.[itemBlank] !== undefined) return ff[itemBlank];
      if (/^Q\d+$/i.test(first)) return "";
      if (ctx.formFields[first]?.[blank] !== undefined) return ctx.formFields[first][blank];
      return ctx.fields[f] ?? ctx.fields[blank] ?? "";
    }
  }
  return ctx.fields[f] ?? "";
}

export function resolveTemplate(str, ctx) {
  if (str == null) return "";
  return String(str).replace(/<<([^>]+)>>/g, (_, ref) => getFieldValue(ctx, ref.trim()));
}

export function compareValues(left, op, right) {
  const l = String(left ?? "");
  const r = String(right ?? "");
  switch (op) {
    case "equals":
      return l === r;
    case "doesNotEqual":
      return l !== r;
    case "contains":
      return l.includes(r);
    case "doesNotContain":
      return !l.includes(r);
    case "beginsWith":
      return l.startsWith(r);
    case "endsWith":
      return l.endsWith(r);
    case "isBlank":
      return l.trim() === "";
    case "isNotBlank":
      return l.trim() !== "";
    case "isGreaterThan":
      return Number(l) > Number(r);
    case "isLessThan":
      return Number(l) < Number(r);
    case "isGreaterThanOrEqualTo":
      return Number(l) >= Number(r);
    case "isLessThanOrEqualTo":
      return Number(l) <= Number(r);
    case "mcEquals":
      return l.split(",").map((s) => s.trim()).includes(r);
    case "mcDoesNotEqual":
      return !l.split(",").map((s) => s.trim()).includes(r);
    case "mcContains":
      return l.split(",").map((s) => s.trim()).includes(r);
    case "mcDoesNotContain":
      return !l.split(",").map((s) => s.trim()).includes(r);
    case "mcIsBlank":
      return l.trim() === "";
    case "mcIsNotBlank":
      return l.trim() !== "";
    default:
      return false;
  }
}

export function evalCondition(cond, ctx) {
  if (!cond) return true;
  const andList = cond.op === "and" ? cond.conditions : cond.and;
  if (andList) {
    return andList.every((c) => evalCondition(c, ctx));
  }
  const orList = cond.op === "or" ? cond.conditions : cond.or;
  if (orList) {
    return orList.some((c) => evalCondition(c, ctx));
  }
  const left = getFieldValue(ctx, cond.field);
  let right = cond.value;
  if (typeof right === "string" && right.includes("<<")) {
    right = resolveTemplate(right, ctx);
  }
  return compareValues(left, cond.op ?? "equals", right);
}

export function runCommand(cmd, ctx) {
  switch (cmd.cmd) {
    case "comment":
      return null;
    case "set": {
      const val = resolveTemplate(cmd.value ?? "", ctx);
      ctx.fields[cmd.field] = val;
      return null;
    }
    case "get": {
      const listName = cmd.recordList;
      const forms = cmd.sourceForms ?? [];
      const rows = [];
      for (const formName of forms) {
        const recs = ctx.records[formName] ?? [];
        for (const rec of recs) {
          rows.push({ _form: formName, ...rec });
        }
      }
      ctx.recordLists = ctx.recordLists ?? {};
      ctx.recordLists[listName] = rows;
      return null;
    }
    case "foreach": {
      const rows = ctx.recordLists?.[cmd.recordList] ?? [];
      for (const row of rows) {
        ctx.recordBindings = { ...(ctx.recordBindings ?? {}), [cmd.recordName]: row };
        for (const inner of cmd.do ?? []) {
          runCommand(inner, ctx);
        }
      }
      ctx.recordBindings = {};
      return null;
    }
    case "if": {
      const branch = evalCondition(cmd.condition, ctx) ? cmd.then : cmd.else;
      for (const inner of branch ?? []) {
        const skip = runCommand(inner, ctx);
        if (skip) return skip;
      }
      return null;
    }
    case "skip":
      return cmd.to;
    case "send":
      return null;
    case "showDocument": {
      ctx._nav = ctx._nav ?? { showForm: null, documents: [] };
      if (cmd.document) ctx._nav.documents.push(cmd.document);
      return null;
    }
    case "show": {
      ctx._nav = ctx._nav ?? { showForm: null, documents: [] };
      // Builder stores Show Document as `{ cmd: "show", document }` (not showDocument).
      if (cmd.document) {
        ctx._nav.documents.push(cmd.document);
        return null;
      }
      if (cmd.url) {
        ctx._nav.url = cmd.url;
        return "__PROCESS_DONE__";
      }
      if (cmd.form) {
        ctx._nav.showForm = cmd.form;
        return "__PROCESS_DONE__";
      }
      return null;
    }
    default:
      return null;
  }
}

export function runCommands(commands, ctx) {
  for (const cmd of commands ?? []) {
    const skip = runCommand(cmd, ctx);
    if (skip === "__PROCESS_DONE__") return skip;
    if (skip) return skip;
  }
  return null;
}

export function runProcessByName(project, processName, ctx) {
  const proc = project.processes?.find((p) => p.name === processName);
  if (!proc) return { type: "none" };
  ctx._nav = { showForm: null, documents: [], url: null };
  runCommands(proc.commands, ctx);
  // Legacy ProcessCommandList accumulates Show Document HTML, then may Show Form.
  // Prefer documents when both are present — otherwise Form loops hide the Document.
  if (ctx._nav.documents.length) {
    return {
      type: "documents",
      documents: [...ctx._nav.documents],
      thenForm: ctx._nav.showForm || null,
    };
  }
  if (ctx._nav.showForm) return { type: "form", form: ctx._nav.showForm };
  if (ctx._nav.url) return { type: "url", url: ctx._nav.url };
  return { type: "none" };
}

// Legacy helper — kept for skipInstructions-only callers
export function runSkipInstructions(form, ctx) {
  let skipTarget = null;
  for (const item of form.items ?? []) {
    if (item.type !== "skipInstructions") continue;
    const result = runCommands(item.commands, ctx);
    if (result && result !== "__PROCESS_DONE__") skipTarget = result;
  }
  return skipTarget;
}

export function buildContext(session, formName) {
  return {
    fields: { ...session.fields },
    formFields: session.formFields,
    records: session.records,
    recordLists: {},
    recordBindings: {},
    formName,
  };
}

export function expandDynamicChoices(choice, ctx) {
  if (choice.type !== "dynamic") return [choice];
  const formName = choice.sourceForm;
  const rows = ctx.records[formName] ?? [];
  return rows.map((row, i) => {
    const subCtx = {
      ...ctx,
      recordBindings: { Record: { ...row, _form: formName } },
    };
  return {
      label: String(i),
      name: String(i),
      text: resolveTemplate(choice.displayExpr ?? "", subCtx) || JSON.stringify(row),
      value: resolveTemplate(choice.valueExpr ?? "", subCtx) || row.DivisionID || String(i),
    };
  });
}

export function visibleItems(form, fromLabel) {
  const items = form.items ?? [];
  if (!fromLabel) return items.filter((i) => i.type !== "skipInstructions");
  let start = false;
  const result = [];
  for (const item of items) {
    if (item.type === "skipInstructions") continue;
    const key = itemKey(item);
    if (!start) {
      if (key === fromLabel || item.label === fromLabel) start = true;
      else continue;
    }
    result.push(item);
    if (item.type === "break") break;
  }
  return result.length ? result : items.filter((i) => i.type !== "skipInstructions");
}
