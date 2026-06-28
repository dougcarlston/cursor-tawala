import {
  buildContext,
  expandDynamicChoices,
  getFieldValue,
  itemKey,
  resolveTemplate,
  runCommands,
  runProcessByName,
  visibleItems,
} from "./runtimeEngine.mjs";
import { fibRowFields, fibRowLabel, fibUsesLeftLabels, parseFibPrompt } from "./fibPrompt.mjs";
import { getThemeCss, themeBodyClass } from "./themes/index.mjs";
import {
  buildFormSegments,
  findSegmentForSkip,
  segmentVisibleItems,
} from "./formSegments.mjs";
import {
  isRegistrationForm,
  registrationPage2Footer,
  renderRegistrationFib,
  renderRegistrationMc,
  renderRegistrationText,
} from "./registrationLayout.mjs";
import { validateRegistrationPage1 } from "./registrationPage1Validation.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function formState(session, formName) {
  if (!session.formState) session.formState = {};
  if (!session.formState[formName]) {
    session.formState[formName] = { segmentIndex: 0, skipStartLabel: null };
  }
  return session.formState[formName];
}

function runSkipBlocks(skipBlocks, ctx) {
  for (const commands of skipBlocks ?? []) {
    const target = runCommands(commands, ctx);
    if (target) return target;
  }
  return null;
}

function blankInput(item, blank, ctx) {
  const fname = `${item.label}:${blank.name}`;
  const alt = blank.alternateLabel || blank.name;
  const val =
    getFieldValue(ctx, `${ctx.formName}:${alt}`) ||
    getFieldValue(ctx, fname) ||
    getFieldValue(ctx, alt) ||
    "";
  const len = blank.length ?? 20;
  let cls = "fib-medium";
  if (len <= 8) cls = "fib-date";
  else if (len >= 30) cls = "fib-wide";
  const width = len >= 30 ? "" : ` size="${len}"`;
  return `<input type="text" class="${cls}" name="${esc(fname)}" value="${esc(val)}"${width} />`;
}

function renderFib(item, ctx) {
  if (isRegistrationForm(ctx.formName)) {
    const reg = renderRegistrationFib(item, ctx, ctx.formName);
    if (reg) return reg;
  }

  const prompt = typeof item.prompt === "string" ? item.prompt : "";
  const blanks = item.blanks ?? [];
  const rows = parseFibPrompt(prompt, blanks);
  const left = fibUsesLeftLabels(item.style);

  const rowHtml = rows
    .map((row) => {
      const label = fibRowLabel(row.segments);
      const fields = fibRowFields(row.segments);
      const trailing = row.segments.filter((s) => s.type === "text").slice(1);

      const fieldHtml = fields
        .map((f) => {
          const hint = f.hint ? `<em class="fib-hint">${esc(f.hint)}</em>` : "";
          return `<span class="fib-field">${hint}${blankInput(item, f.blank, ctx)}</span>`;
        })
        .join("");

      const trailHtml = trailing
        .map((t) => `<span class="fib-inline-text">${esc(t.text)}</span>`)
        .join("");

      if (left && label) {
        return `<div class="fib-row">
          <span class="fib-label">${esc(label)}:</span>
          <span class="fib-fields">${fieldHtml}${trailHtml}</span>
        </div>`;
      }

      const inline = row.segments
        .map((seg) => {
          if (seg.type === "text") return `<span class="fib-inline-text">${esc(seg.text)}</span>`;
          if (seg.type === "blank") {
            const hint = seg.hint ? `<em class="fib-hint">${esc(seg.hint)}</em>` : "";
            return `<span class="fib-field">${hint}${blankInput(item, seg.blank, ctx)}</span>`;
          }
          return "";
        })
        .join("");
      return `<div class="fib-row fib-row-stacked">${inline}</div>`;
    })
    .join("");

  return `<div class="fib fib-style-${esc(item.style || "default")}" id="item-${esc(itemKey(item))}">${rowHtml}</div>`;
}

function enhancePlainText(content, ctx, item) {
  let text = content;
  if (text.includes('""')) {
    const league = getFieldValue(ctx, "League") || "Dirt Bowl";
    text = text.replace(/""/g, league);
  }
  return resolveTemplate(text, ctx);
}

function renderRichNodes(nodes, ctx) {
  if (!nodes) return "";
  return nodes
    .map((n) => {
      switch (n.type) {
        case "text":
          return esc(n.text);
        case "bold":
          return `<strong>${renderRichNodes(n.nodes, ctx)}</strong>`;
        case "italic":
          return `<em>${renderRichNodes(n.nodes, ctx)}</em>`;
        case "underline":
          return `<u>${renderRichNodes(n.nodes, ctx)}</u>`;
        case "font": {
          const style = [];
          if (n.face) style.push(`font-family:${esc(n.face)}`);
          if (n.size) style.push(`font-size:${n.size / 20}pt`);
          if (n.color) style.push(`color:#${esc(n.color).replace(/^#/, "")}`);
          return `<span style="${style.join(";")}">${renderRichNodes(n.nodes, ctx)}</span>`;
        }
        case "field": {
          const val = getFieldValue(ctx, n.name ?? n.field);
          return esc(val || `«${n.name ?? n.field}»`);
        }
        default:
          return renderRichNodes(n.nodes, ctx);
      }
    })
    .join("");
}

function renderRichContent(content, ctx, item) {
  if (!content) return "";
  if (typeof content === "string") {
    const text = enhancePlainText(content, ctx, item);
    return `<p>${esc(text)}</p>`;
  }
  if (!Array.isArray(content)) return "";
  return content
    .map((block) => {
      if (block.type === "paragraph") {
        const align = block.align ? ` style="text-align:${block.align}"` : "";
        return `<p${align}>${renderRichNodes(block.nodes, ctx)}</p>`;
      }
      if (block.type === "text") return esc(block.text);
      return "";
    })
    .join("\n");
}

function renderMcChoices(item, ctx) {
  if (isRegistrationForm(ctx.formName)) {
    return renderRegistrationMc(item, ctx);
  }

  const inputType = item.onlyone !== false ? "radio" : "checkbox";
  const name = itemKey(item);
  const choices = item.choices ?? [];
  const expanded = [];
  for (const c of choices) {
    if (c.type === "dynamic") expanded.push(...expandDynamicChoices(c, ctx));
    else expanded.push(c);
  }
  return expanded
    .map((c) => {
      const val = c.value ?? c.label ?? c.name;
      const checked = String(getFieldValue(ctx, name) || "")
        .split(",")
        .includes(String(val))
        ? " checked"
        : "";
      return `<label class="preview-mc-choice"><input type="${inputType}" name="${esc(name)}" value="${esc(val)}"${checked} /> ${esc(c.text)}</label>`;
    })
    .join("");
}

function renderItem(item, ctx, project) {
  switch (item.type) {
    case "heading":
    case "subheading":
      return item.type === "subheading"
        ? `<h3>${esc(item.content)}</h3>`
        : `<h2>${esc(item.content)}</h2>`;
    case "text": {
      if (isRegistrationForm(ctx.formName)) {
        const reg = renderRegistrationText(item, ctx, ctx.formName, project);
        if (reg !== null) return reg;
      }
      const style = item.style && item.style !== "normal" ? ` ${esc(item.style)}` : "";
      const inner = renderRichContent(item.content, ctx, item);
      if (!inner.trim()) return "";
      const wrapCls = item.style === "instructional" ? "text instructional reg-page-footer" : `text${style}`;
      return `<div class="${wrapCls}">${inner}</div>`;
    }
    case "fib":
      return renderFib(item, ctx);
    case "mc": {
      const choices = renderMcChoices(item, ctx);
      const q = item.question ?? "";
      if (isRegistrationForm(ctx.formName) && (item.label === "Q7" || item.label === "Q8")) {
        return choices;
      }
      if (isRegistrationForm(ctx.formName) && typeof choices === "string" && /^\s*<(fieldset|div)\b/.test(choices)) {
        return choices;
      }
      return `<fieldset class="mc" id="item-${esc(itemKey(item))}"><legend>${esc(q)}</legend><div class="mc-choices">${choices}</div></fieldset>`;
    }
    case "field": {
      const n = item.name ?? item.fieldName;
      const val = getFieldValue(ctx, n) || "";
      return `<input type="hidden" name="${esc(n)}" value="${esc(val)}" />`;
    }
    case "break":
      return "";
    case "skipInstructions":
      return "";
    default:
      return `<!-- unsupported ${esc(item.type)} -->`;
  }
}

function pageShell(title, body, banner, themePath) {
  const theme = themePath || "default";
  const bodyClass = themeBodyClass(theme);
  const themeCss = getThemeCss(theme);
  const isThemed = theme !== "default";

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>${esc(title)}</title>
  <style>
    ${!isThemed ? `body { font-family: Arial, sans-serif; max-width: 800px; margin: 2rem auto; padding: 0 1rem; line-height: 1.4; }
    h1 { font-size: 1.25rem; color: #333; border-bottom: 1px solid #ccc; padding-bottom: 0.5rem; }
    .dev-banner { background: #fff3cd; border: 1px solid #ffc107; padding: 8px 12px; margin-bottom: 1rem; font-size: 13px; }
    fieldset.mc { margin: 1rem 0; border: 1px solid #ccc; padding: 8px 12px; }
    .preview-mc-choice { display: block; margin: 4px 0; }
    .fib label { display: inline-block; margin: 4px 8px 4px 0; }
    input[type=submit] { margin-top: 1rem; padding: 8px 24px; font-size: 14px; }
    .text-block p { margin: 0.5rem 0; }` : themeCss}
  </style>
</head>
<body class="${bodyClass}">
  <div class="dev-banner">${banner}</div>
  ${body}
</body>
</html>`;
}

function resolveTheme(project, form) {
  return form.themePath || project.themePath || "default";
}

export function prepareFormContext(project, form, session) {
  const ctx = buildContext(session, form.name);
  ctx.formName = form.name;

  if (form.preProcess && !session.preProcessDone[form.name]) {
    runProcessByName(project, form.preProcess, ctx);
    session.preProcessDone[form.name] = true;
    Object.assign(session.fields, ctx.fields);
  }

  return ctx;
}

function getSegmentItems(form, session) {
  const { segments } = buildFormSegments(form);
  const state = formState(session, form.name);
  const seg = segments[state.segmentIndex] ?? segments[0];
  let items = segmentVisibleItems(seg, state.skipStartLabel);

  if (isRegistrationForm(form.name) && state.segmentIndex === 1) {
    const hasFooter = items.some((i) => i.label === "T10" || (i.style === "instructional" && String(i.content).includes("Press Submit to continue")));
    if (!hasFooter) {
      items = [...items, { type: "text", label: "__footer", style: "instructional", content: "__page2footer__" }];
    }
  }

  return { segments, seg, items, state };
}

export function renderFormPage(project, formName, baseUrl, uniqueId, session, options = {}) {
  const form = project.forms?.find((f) => f.name === formName);
  if (!form) {
    return pageShell("Not found", "<h1>Form not found</h1>", "Error", "default");
  }

  const theme = resolveTheme(project, form);
  const ctx = prepareFormContext(project, form, session);

  if (options.fromLabel) {
    const dest = findSegmentForSkip(buildFormSegments(form).segments, options.fromLabel);
    const state = formState(session, formName);
    state.segmentIndex = dest.index;
    state.skipStartLabel = dest.startLabel;
  }

  const { segments, state, items } = getSegmentItems(form, session);

  const itemHtml = items
    .map((item) => {
      if (item.content === "__page2footer__") return registrationPage2Footer();
      return renderItem(item, ctx, project);
    })
    .join("\n");

  const action = `${baseUrl}/p/${uniqueId}/${encodeURIComponent(formName)}`;
  const submitLabel = theme === "dirtbowl2" ? "Submit →" : "Submit";
  const resetUrl = `${action}?reset=1`;
  const startOver = isRegistrationForm(formName)
    ? `<p class="reg-start-over"><a href="${resetUrl}">Start over with a blank form</a></p>`
    : "";
  const formAttrs = isRegistrationForm(formName) ? ' autocomplete="off"' : "";
  const bannerExtra = isRegistrationForm(formName)
    ? ` · <a href="${resetUrl}">Start over</a> · runtime 2026-06-28`
    : "";

  const body = `
  ${isRegistrationForm(formName) ? "" : `<div class="project-title">${esc(project.name)}</div>`}
  <h1 class="form-title">${esc(formName)}</h1>
  <form class="tawala-form" method="post" action="${action}"${formAttrs}>
    <input type="hidden" name="segmentId" value="${state.segmentIndex}" />
    ${itemHtml}
    <div class="form-footer">
      <input type="submit" name="submit" value="${esc(submitLabel)}" />
      ${startOver}
    </div>
  </form>`;

  return pageShell(
    `${project.name} — ${formName}`,
    body,
    `Tawala dev runtime — ${esc(project.name)} / ${esc(formName)} — page ${state.segmentIndex + 1} of ${segments.length}${bannerExtra}`,
    theme,
  );
}

export function handleFormSubmit(project, formName, session, body, baseUrl, uniqueId) {
  const form = project.forms?.find((f) => f.name === formName);
  if (!form) return pageShell("Error", "<h1>Form not found</h1>", "", "default");

  applySubmissionToSession(session, formName, body);
  const ctx = buildContext(session, formName);
  ctx.formName = formName;

  if (form.preProcess) {
    runProcessByName(project, form.preProcess, ctx);
  }
  Object.assign(session.fields, ctx.fields);

  const { segments } = buildFormSegments(form);
  const state = formState(session, formName);
  const segIdx = Number(body.segmentId ?? state.segmentIndex ?? 0);
  const prevSegment = segments[segIdx] ?? segments[0];

  if (formName === "Registration" && segIdx === 0) {
    const pageErr = validateRegistrationPage1(ctx);
    if (pageErr) {
      session.fields.Message = pageErr;
      ctx.fields.Message = pageErr;
      state.segmentIndex = 0;
      state.skipStartLabel = "T1";
      return renderFormPage(project, formName, baseUrl, uniqueId, session);
    }
    session.fields.Message = " ";
    ctx.fields.Message = " ";
  }

  const skipTarget = runSkipBlocks(prevSegment.skipBlocks, ctx);
  Object.assign(session.fields, ctx.fields);

  if (skipTarget === "__EndOfForm__") {
    return renderSubmitAck(project, form, session, baseUrl, uniqueId, {
      note: "Registration saved in dev session. Payment/waiver (RegStep2) requires full Java backend.",
    });
  }

  if (skipTarget) {
    const dest = findSegmentForSkip(segments, skipTarget);
    state.segmentIndex = dest.index;
    state.skipStartLabel = dest.startLabel;
    return renderFormPage(project, formName, baseUrl, uniqueId, session);
  }

  if (segIdx + 1 < segments.length) {
    state.segmentIndex = segIdx + 1;
    state.skipStartLabel = null;
    return renderFormPage(project, formName, baseUrl, uniqueId, session);
  }

  if (form.process) {
    return renderSubmitAck(project, form, session, baseUrl, uniqueId, {
      note: `Post-process “${form.process}” is not fully executed in dev runtime.`,
    });
  }

  return renderSubmitAck(project, form, session, baseUrl, uniqueId);
}

function applySubmissionToSession(session, formName, body) {
  if (!session.formFields[formName]) session.formFields[formName] = {};
  const sharedSlots = new Set(["a", "b", "c"]);
  for (const [key, value] of Object.entries(body)) {
    if (key === "submit" || key === "from" || key === "segmentId") continue;
    session.formFields[formName][key] = value;
    session.fields[key] = value;
    if (key.includes(":")) {
      const parts = key.split(":");
      const blank = parts[parts.length - 1];
      if (parts.length === 2) {
        session.fields[`${formName}:${parts[0]}:${parts[1]}`] = value;
        if (!sharedSlots.has(blank)) {
          session.fields[blank] = value;
          session.fields[`${formName}:${blank}`] = value;
        }
      }
    }
    const mcNames = ["SexMCQ", "DivRequest", "LastDiv", "ShirtSize", "InfoCorrect", "ParentWillingToCoach", "WillingToUmpire"];
    if (mcNames.includes(key)) {
      session.fields[`${formName}:${key}`] = value;
    }
  }
}

export function renderSubmitAck(project, form, session, baseUrl, uniqueId, opts = {}) {
  const theme = resolveTheme(project, form);
  const back = `${baseUrl}/p/${uniqueId}/${encodeURIComponent(form.name)}`;
  const body = `
  <h1>Registration step complete</h1>
  <p><strong>${esc(project.name)}</strong> — ${esc(form.name)}</p>
  <p>${esc(opts.note ?? "Dev runtime recorded field values.")}</p>
  <p><a href="${back}">← Back to registration</a></p>`;
  return pageShell("Submitted", body, "Tawala dev runtime", theme);
}
