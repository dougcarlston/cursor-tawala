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
import { enhanceRichTextHtml as enhanceRichHtmlShared, looksLikeRichHtml } from "./richHtmlPreview.mjs";
import { renderDocumentsPage } from "./documentRenderer.mjs";
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

/**
 * Heading content may carry inline per-run size markup (`<span class="heading-size-*">`).
 * The runtime renders headings at a single theme size (`<h2>`/`<h3>`), so reduce to plain
 * text (decoding editor entities) rather than emitting escaped `<span>` tags. Per-run size
 * parity in the runtime is deferred.
 */
function headingPlainText(content) {
  return String(content ?? "")
    .replace(/\u200b/g, "")
    .replace(/<[^>]*>/g, "")
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&");
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

  if (item.style === "topLabels") {
    const intro =
      prompt.trim() && !prompt.includes("//") && !prompt.includes("/") ? prompt.trim() : "";
    const fieldRows = blanks;
    const rowHtml = fieldRows
      .map((blank) => {
        const label = blank.displayLabel?.trim() || "";
        return `<div class="fib-row fib-top-label">
          ${label ? `<div class="fib-top-label-text">${esc(label)}</div>` : ""}
          <div class="fib-top-label-field"><input type="text" class="text" name="${esc(`${item.label}:${blank.name}`)}" size="${Math.max(blank.length ?? 20, 5)}" readonly="readonly" /></div>
        </div>`;
      })
      .join("");
    const introHtml = intro ? `<p class="fib-intro">${esc(intro)}</p>` : "";
    return `<div class="fib fib-style-topLabels" id="item-${esc(itemKey(item))}">${introHtml}${rowHtml}</div>`;
  }

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

const RICH_TEXT_HTML_TAG_RE = /<\/?(?:p|div|span|strong|b|em|i|u|font|br)(?:\s[^>]*)*\/?>/i;

function applyLegacyTextSubstitutions(content, ctx) {
  let text = String(content ?? "");
  if (text.includes('""')) {
    const league = getFieldValue(ctx, "League") || "Dirt Bowl";
    text = text.replace(/""/g, league);
  }
  return text;
}

function enhancePlainText(content, ctx, item) {
  const text = applyLegacyTextSubstitutions(content, ctx);
  return resolveTemplate(text, ctx);
}

function containsRichTextHtml(content) {
  return RICH_TEXT_HTML_TAG_RE.test(content) || looksLikeRichHtml(content);
}

function enhanceRichTextHtml(content, ctx) {
  const html = applyLegacyTextSubstitutions(content, ctx);
  return enhanceRichHtmlShared(html, (ref) => getFieldValue(ctx, ref));
}

function parseRecordField(field) {
  const parts = String(field ?? "").split(":");
  if (parts[0] === "Record" && parts.length >= 3) {
    return { form: parts[1], name: parts.slice(2).join(":") };
  }
  return { form: null, name: String(field ?? "") };
}

function recordCellValue(row, ref, defaultForm) {
  const name = ref.name;
  if (row[name] != null && row[name] !== "") return row[name];
  const form = ref.form ?? defaultForm;
  if (form) {
    const qualified = `${form}:${name}`;
    if (row[qualified] != null && row[qualified] !== "") return row[qualified];
  }
  return "";
}

/** HTML facsimile of legacy itemization (MULTIPLE QUESTION LIST) tables. */
function renderItemizationTableHtml(node, ctx) {
  const columns = node.columns ?? [];
  if (columns.length === 0) return "";

  const sourceForm = node.form ?? ctx.formName;
  const records = ctx.records?.[sourceForm] ?? [];
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
                const val = recordCellValue(row, ref, sourceForm);
                return `<td>${esc(val)}</td>`;
              })
              .join("");
            return `<tr class="${cls}">${cells}</tr>`;
          })
          .join("\n");

  const fixWidth = columns.length > 3;
  const containerClass = fixWidth ? ' class="tawalaDataTable dtFixTableWidth"' : "";

  return `<div${containerClass}><table class="component outline sortable stripe">
<thead><tr>${headerCells}</tr></thead>
<tbody>${bodyRows}</tbody>
</table></div>`;
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
        case "itemizationTable":
          return renderItemizationTableHtml(n, ctx);
        case "choiceTallyTable":
          return `<div class="preview-function-table"><em>Choice tally</em> (rendered on Java deploy)</div>`;
        case "questionCorrelationTable":
          return `<div class="preview-function-table"><em>Date correlation table</em> (rendered on Java deploy)</div>`;
        default:
          return renderRichNodes(n.nodes, ctx);
      }
    })
    .join("");
}

function isBlockFunctionNode(node) {
  return (
    node?.type === "itemizationTable" ||
    node?.type === "choiceTallyTable" ||
    node?.type === "questionCorrelationTable"
  );
}

function renderRichContent(content, ctx, item) {
  if (!content) return "";
  if (typeof content === "string") {
    if (containsRichTextHtml(content)) {
      return enhanceRichTextHtml(content, ctx);
    }
    const text = enhancePlainText(content, ctx, item);
    return `<p>${esc(text)}</p>`;
  }
  if (!Array.isArray(content)) return "";
  return content
    .map((block) => {
      if (block.type === "paragraph") {
        const nodes = block.nodes ?? [];
        if (nodes.some(isBlockFunctionNode)) {
          const align = block.align ? ` style="text-align:${block.align}"` : "";
          const parts = [];
          let inlineNodes = [];
          const flushInlineNodes = () => {
            if (!inlineNodes.length) return;
            parts.push(`<p${align}>${renderRichNodes(inlineNodes, ctx)}</p>`);
            inlineNodes = [];
          };

          for (const node of nodes) {
            if (isBlockFunctionNode(node)) {
              flushInlineNodes();
              parts.push(renderRichNodes([node], ctx));
            } else {
              inlineNodes.push(node);
            }
          }

          flushInlineNodes();
          return parts.join("\n");
        }
        const align = block.align ? ` style="text-align:${block.align}"` : "";
        return `<p${align}>${renderRichNodes(nodes, ctx)}</p>`;
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
        ? `<h3>${esc(headingPlainText(item.content))}</h3>`
        : `<h2>${esc(headingPlainText(item.content))}</h2>`;
    case "text": {
      if (isRegistrationForm(ctx.formName)) {
        const reg = renderRegistrationText(item, ctx, ctx.formName, project);
        if (reg !== null) return reg;
      }
      let content = item.content;
      if (ctx.formName === "RegStep2" && typeof content === "string") {
        const fee = getFieldValue(ctx, "SignupFeeForIndividual") || "";
        if (fee) content = content.replace(/payment of \$/gi, `payment of $${fee}`);
      }
      const style = item.style && item.style !== "normal" ? ` ${esc(item.style)}` : "";
      const inner = renderRichContent(content, ctx, item);
      if (!inner.trim()) return "";
      const wrapCls = item.style === "instructional" ? "text instructional reg-page-footer" : `text${style}`;
      return `<div class="${wrapCls}">${inner}</div>`;
    }
    case "fib":
      return renderFib(item, ctx);
    case "mc": {
      const choices = renderMcChoices(item, ctx);
      // MCQ question may carry canvas-inline HTML; render plain text in the legend.
      const q = String(item.question ?? "").replace(/<[^>]+>/g, "");
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

const COMPONENT_TABLE_CSS = `
table.component { border-collapse: collapse; font-size: 1em; margin: 12px 0; }
table.component.outline { border: 1px solid #cccccc; }
table.component thead { background-color: #888888; color: #eeeeee; }
table.component thead th { padding: .2em 1em; border: 1px solid #cccccc; }
table.component tbody tr.odd { background-color: #f8f8f8; }
table.component tbody tr.even { background-color: #ffffff; }
table.component tbody tr:hover { background-color: #e8e8e8; }
table.component td { padding-left: 1em; padding-right: 1em; line-height: 1.5em; border: 1px solid #dddddd; }
`;

/** Form Preview stand-in for DISPLAY IMAGE — sized box, image name centered (not live URL fetch). */
const DISPLAY_IMAGE_PREVIEW_CSS = `
.preview-display-image {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  box-sizing: border-box;
  vertical-align: middle;
  margin: 4px 0;
  max-width: 100%;
  border: 1px dashed #888;
  background: #f0f0f0;
  color: #444;
  font-size: 12px;
  font-family: Arial, sans-serif;
  text-align: center;
  overflow: hidden;
}
.preview-display-image-label {
  padding: 4px 8px;
  word-break: break-word;
  max-width: 100%;
  line-height: 1.2;
}
`;

function pageShell(title, body, banner, themePath) {
  const theme = themePath || "default";
  const bodyClass = themeBodyClass(theme);
  const themeCss = getThemeCss(theme);
  const isThemed = theme !== "default";
  const bannerHtml = banner ? `<div class="dev-banner">${banner}</div>` : "";

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>${esc(title)}</title>
  <style>
    ${COMPONENT_TABLE_CSS}
    ${DISPLAY_IMAGE_PREVIEW_CSS}
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
  ${bannerHtml}
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

  if (form.preProcess) {
    runProcessByName(project, form.preProcess, ctx);
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

  const showPageChrome = !options.designerPreview;
  const body = `
  ${showPageChrome && !isRegistrationForm(formName) ? `<div class="project-title">${esc(project.name)}</div>` : ""}
  ${showPageChrome ? `<h1 class="form-title">${esc(formName)}</h1>` : ""}
  <form class="tawala-form" method="post" action="${action}"${formAttrs}>
    <input type="hidden" name="segmentId" value="${state.segmentIndex}" />
    ${itemHtml}
    <div class="form-footer">
      <input type="submit" name="submit" value="${esc(submitLabel)}" />
      ${startOver}
    </div>
  </form>`;

  const banner = options.designerPreview
    ? ""
    : `Tawala dev runtime — ${esc(project.name)} / ${esc(formName)} — page ${state.segmentIndex + 1} of ${segments.length}${bannerExtra}`;

  return pageShell(`${project.name} — ${formName}`, body, banner, theme);
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

  const finishPostProcess = () => {
    if (!form.process) return null;
    const nav = runProcessByName(project, form.process, ctx);
    Object.assign(session.fields, ctx.fields);
    if (nav.type === "form") {
      formState(session, nav.form).segmentIndex = 0;
      formState(session, nav.form).skipStartLabel = null;
      return renderFormPage(project, nav.form, baseUrl, uniqueId, session);
    }
    if (nav.type === "documents") {
      return renderDocumentsPage(project, nav.documents, session, baseUrl, uniqueId, {
        fromForm: formName,
        thenForm: nav.thenForm || null,
      });
    }
    return renderSubmitAck(project, form, session, baseUrl, uniqueId, {
      note: `Post-process “${form.process}” finished without a show step in dev runtime.`,
    });
  };

  if (skipTarget === "__EndOfForm__") {
    const finished = finishPostProcess();
    if (finished) return finished;
    return renderSubmitAck(project, form, session, baseUrl, uniqueId, {
      note: "Dev runtime recorded field values.",
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
    const finished = finishPostProcess();
    if (finished) return finished;
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
