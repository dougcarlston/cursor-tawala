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
import { fibRowLabel, fibUsesLeftLabels, normalizeFibPromptSource, parseFibPrompt } from "./fibPrompt.mjs";
import { renderItemizationTableHtml, blankAliasesFromForm } from "./itemizationPreview.mjs";
import { renderChoiceTallyTableHtml } from "./choiceTallyPreview.mjs";
import { BASE_FORM_CSS, resolveTheme as resolveThemeCss, themeBodyClass } from "./themes/index.mjs";
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
import { validateFibBlanks } from "./fibBlankValidation.mjs";
import { appendFormRecord, clearFormAnswers } from "./sessionStore.mjs";

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
  // Underscore-run length from Design — keep size/ch width so multi-blank rows can
  // share left+right edges the way the Designer laid out the underscores.
  const len = Math.max(1, Math.min(Number(blank.length) || 20, 80));
  let cls = "fib-medium";
  if (len <= 8) cls = "fib-date";
  else if (len >= 30) cls = "fib-wide";
  return (
    `<input type="text" class="${cls}" name="${esc(fname)}" value="${esc(val)}" ` +
    `size="${len}" style="width:${len}ch;max-width:100%;box-sizing:content-box" />`
  );
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

  // Legacy topLabels (SignupSheets): label text + separate inputs, no Design `_` runs.
  // Design-authored prompts with `_` must use parseFibPrompt below — never dump
  // underscore runs as fib-intro beside the boxes (Batch 3: boxes replace `_`).
  if (item.style === "topLabels") {
    const plainPrompt = normalizeFibPromptSource(prompt)
      .replace(/<[^>]+>/g, "")
      .trim();
    const hasUnderscoreRuns = /_+/.test(plainPrompt);
    if (!hasUnderscoreRuns) {
      const introRaw =
        prompt.trim() && !prompt.includes("//") && !prompt.includes("/") ? prompt.trim() : "";
      const intro = introRaw
        ? normalizeFibPromptSource(introRaw)
            .replace(/<[^>]+>/g, "")
            .replace(/_+/g, "")
            .replace(/\s+/g, " ")
            .trim()
        : "";
      const fieldRows = blanks;
      const rowHtml = fieldRows
        .map((blank) => {
          const label = blank.displayLabel?.trim() || "";
          // Live Preview inputs — never readonly (that produced empty signup records /
          // blank MULTIPLE QUESTION LIST rows). Reuse blankInput for name + session value.
          return `<div class="fib-row fib-top-label">
          ${label ? `<div class="fib-top-label-text">${esc(label)}</div>` : ""}
          <div class="fib-top-label-field">${blankInput(item, blank, ctx)}</div>
        </div>`;
        })
        .join("");
      const introHtml =
        intro && !/underscores create blanks/i.test(intro)
          ? `<p class="fib-intro">${esc(intro)}</p>`
          : "";
      return `<div class="fib fib-style-topLabels" id="item-${esc(itemKey(item))}">${introHtml}${rowHtml}</div>`;
    }
  }

  const rowHtml = rows
    .map((row) => {
      const label = fibRowLabel(row.segments);

      /** Ordered blank + interstitial text after the leading label (Batch 3 leftAlign). */
      const fieldsAfterLabelHtml = () => {
        let seenBlank = false;
        return row.segments
          .map((seg) => {
            if (seg.type === "text") {
              // Skip the leading label text (first text before any blank).
              if (!seenBlank) return "";
              const text = String(seg.text ?? "").replace(/_+/g, "");
              if (!text) return "";
              return `<span class="fib-inline-text">${esc(text)}</span>`;
            }
            if (seg.type === "blank") {
              seenBlank = true;
              const hint = seg.hint ? `<em class="fib-hint">${esc(seg.hint)}</em>` : "";
              return `<span class="fib-field">${hint}${blankInput(item, seg.blank, ctx)}</span>`;
            }
            return "";
          })
          .join("");
      };

      if (left && label) {
        const cleanLabel = String(label).replace(/_+/g, "");
        return `<div class="fib-row">
          <span class="fib-label">${esc(cleanLabel)}:</span>
          <span class="fib-fields">${fieldsAfterLabelHtml()}</span>
        </div>`;
      }

      const inline = row.segments
        .map((seg) => {
          if (seg.type === "text") {
            // Never paint leftover Design underscores — blanks are the inputs.
            const text = String(seg.text ?? "").replace(/_+/g, "");
            if (!text) return "";
            return `<span class="fib-inline-text">${esc(text)}</span>`;
          }
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

  // Any blanks not matched by underscore runs (e.g. shorter prompt than blanks[]) —
  // do not dump orphan boxes; Design/Preview stay in sync with prompt underscores.
  return `<div class="fib fib-style-${esc(item.style || "default")}" id="item-${esc(itemKey(item))}">${rowHtml}</div>`;
}

const RICH_TEXT_HTML_TAG_RE = /<\/?(?:p|div|span|strong|b|em|i|u|font|br|img)(?:\s[^>]*)*\/?>/i;

/**
 * Legacy Registration plain-text used `""` as a League placeholder.
 * Never run this on rich HTML — empty attributes like `class=""` would become
 * `class=Dirt Bowl` and corrupt tables (Potluck confirmation page).
 * Only Registration (or when League already has a value); never invent "Dirt Bowl"
 * for Signup/Potluck/templates that happen to contain literal `""`.
 */
function applyLegacyPlainTextLeaguePlaceholder(content, ctx) {
  let text = String(content ?? "");
  if (!text.includes('""')) return text;
  const league = getFieldValue(ctx, "League");
  const allow =
    isRegistrationForm(ctx.formName) || (league != null && String(league).length > 0);
  if (!allow) return text;
  if (!league) return text;
  return text.replace(/""/g, String(league));
}

function enhancePlainText(content, ctx, item) {
  const text = applyLegacyPlainTextLeaguePlaceholder(content, ctx);
  return resolveTemplate(text, ctx);
}

function containsRichTextHtml(content) {
  return RICH_TEXT_HTML_TAG_RE.test(content) || looksLikeRichHtml(content);
}

function enhanceRichTextHtml(content, ctx) {
  // Rich HTML only — do not apply `""`→League (breaks empty attrs).
  return enhanceRichHtmlShared(content, (ref) => getFieldValue(ctx, ref), {
    records: ctx.records,
    formName: ctx.formName,
    blankAliases: ctx.blankAliases,
    project: ctx.project,
  });
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
          return renderChoiceTallyTableHtml(n, ctx);
        case "questionCorrelationTable":
          return `<div class="preview-function-table"><em>Question correlation table</em> (rendered on Java deploy)</div>`;
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
      // Prefer `text instructional` / `text error` (Styles dialog contract). Avoid
      // conflating with form validation `.error` by keeping both class tokens.
      const wrapCls =
        item.style === "instructional"
          ? "text instructional"
          : item.style === "error"
            ? "text error text-item-error"
            : `text${style}`;
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
table.component .graph { background: #e8e8e8; min-width: 120px; height: 1.2em; position: relative; }
table.component .graph .bar { display: block; height: 100%; background: #6a9fd8; min-width: 0; }
table.component .graph .bar span { padding-left: 4px; font-size: 0.85em; white-space: nowrap; }
.preview-choice-tally { margin: 0.5rem 0; }
.preview-response-totals { margin: 0.75rem 0; }
.preview-response-totals .response-totals-title { margin: 0 0 0.35rem; font-size: 1em; }
`;

/** Match Design canvas vertical gaps between Form Items (Preview / Deploy runtime only). */
const FORM_ITEM_SPACING_CSS = `
.tawala-form > .text,
.tawala-form > .fib,
.tawala-form > fieldset.mc,
.tawala-form > h2,
.tawala-form > h3,
.tawala-form > .preview-function-table {
  margin-top: 0;
  margin-bottom: 0.85rem;
}
.tawala-form > .text p { margin: 0.35rem 0; }
.tawala-form > h2 { margin-bottom: 0.65rem; }
.tawala-form > h3 { margin-bottom: 0.55rem; }
.tawala-form > fieldset.mc { margin-top: 0.25rem; margin-bottom: 0.85rem; }
.tawala-form > .fib { margin-bottom: 0.75rem; }
/* Form Text may carry Design table chrome (absolute left/top, selection). Flow layout only. */
.tawala-form table.user {
  position: static !important;
  left: auto !important;
  top: auto !important;
  right: auto !important;
  float: none !important;
  margin: 0.5rem 0;
  max-width: 100%;
}
.tawala-form [data-doc-blank] {
  min-height: 0 !important;
  display: block;
}
.tawala-form .table-cell-selected {
  outline: none;
  background: transparent;
}
.tawala-form table.user td,
.tawala-form table.user th {
  border: 1px solid #333;
  padding: 2px 6px;
  vertical-align: top;
}
.tawala-form table.user.user-border-2 td,
.tawala-form table.user.user-border-2 th {
  border-width: 2px;
}
.form-footer-preview {
  display: flex;
  flex-wrap: wrap;
  align-items: center;
  gap: 0.75rem 1rem;
  margin-top: 1rem;
}
.form-footer-preview input[type=submit]:disabled {
  opacity: 0.55;
  cursor: not-allowed;
}
.preview-only-hint {
  font-size: 13px;
  color: #555;
}
.preview-segment-rule {
  border: 0;
  border-top: 1px dashed #bbb;
  margin: 1.25rem 0;
}
.preview-segment-label {
  font-size: 12px;
  color: #666;
  margin: -0.5rem 0 0.75rem;
}
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
  const { name: themeName, css: themeCss } = resolveThemeCss(themePath);
  const bodyClass = themeBodyClass(themeName);
  const bannerHtml = banner ? `<div class="dev-banner">${banner}</div>` : "";

  return `<!doctype html>
<html lang="en">
<head>
  <meta charset="UTF-8" />
  <meta name="viewport" content="width=device-width, initial-scale=1.0" />
  <title>${esc(title)}</title>
  <style>
    ${BASE_FORM_CSS}
    ${COMPONENT_TABLE_CSS}
    ${DISPLAY_IMAGE_PREVIEW_CSS}
    ${FORM_ITEM_SPACING_CSS}
    ${themeCss}
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
  ctx.blankAliases = blankAliasesFromForm(form);
  ctx.project = project;

  if (form.preProcess) {
    runProcessByName(project, form.preProcess, ctx);
    Object.assign(session.fields, ctx.fields);
  }

  return ctx;
}

function getSegmentItems(form, session, options = {}) {
  const { segments } = buildFormSegments(form);
  const state = formState(session, form.name);

  // Designer Preview: show every segment so authors can review the full form
  // without Submit / skip paging (Submit is disabled in that mode).
  if (options.designerPreview && segments.length > 1) {
    const items = [];
    for (let i = 0; i < segments.length; i++) {
      const segItems = segments[i].items ?? [];
      if (!segItems.length) continue;
      if (i > 0) {
        items.push({
          type: "text",
          label: `__preview_seg_${i}`,
          style: "instructional",
          content: `__preview_segment__${i + 1}__`,
        });
      }
      items.push(...segItems);
    }
    return { segments, seg: segments[0], items, state };
  }

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

/** Build form markup (and shell metadata). Used by renderFormPage and Document+Form stacking. */
export function buildFormPageParts(project, formName, baseUrl, uniqueId, session, options = {}) {
  const form = project.forms?.find((f) => f.name === formName);
  if (!form) {
    return {
      ok: false,
      theme: "default",
      banner: "Error",
      body: "<h1>Form not found</h1>",
      title: "Not found",
    };
  }

  const theme = resolveTheme(project, form);
  const ctx = prepareFormContext(project, form, session);

  if (options.fromLabel) {
    const dest = findSegmentForSkip(buildFormSegments(form).segments, options.fromLabel);
    const state = formState(session, formName);
    state.segmentIndex = dest.index;
    state.skipStartLabel = dest.startLabel;
  }

  const { segments, state, items } = getSegmentItems(form, session, options);

  const itemHtml = items
    .map((item) => {
      if (item.content === "__page2footer__") return registrationPage2Footer();
      if (typeof item.content === "string" && item.content.startsWith("__preview_segment__")) {
        const n = item.content.replace(/^__preview_segment__|__$/g, "");
        return `<hr class="preview-segment-rule" /><p class="preview-segment-label">Page ${esc(n)} (after Submit)</p>`;
      }
      return renderItem(item, ctx, project);
    })
    .join("\n");

  const action = `${baseUrl}/p/${uniqueId}/${encodeURIComponent(formName)}`;
  const submitLabel = theme === "dirtbowl2" ? "Submit →" : "Submit";
  const resetUrl = `${action}?reset=1`;
  const startOver =
    !options.designerPreview && isRegistrationForm(formName)
      ? `<p class="reg-start-over"><a href="${resetUrl}">Start over with a blank form</a></p>`
      : "";
  const formAttrs = isRegistrationForm(formName) ? ' autocomplete="off"' : "";
  const bannerExtra = isRegistrationForm(formName)
    ? ` · <a href="${resetUrl}">Start over</a> · runtime 2026-06-28`
    : "";

  const showPageChrome = !options.designerPreview && !options.embedded;
  const message = String(session.fields?.Message ?? "").trim();
  const messageHtml =
    !options.designerPreview && message && message !== " "
      ? `<div class="validation-error" role="alert">${esc(message)}</div>`
      : "";
  const formFooter = options.designerPreview
    ? `<div class="form-footer form-footer-preview">
      <input type="submit" name="submit" value="${esc(submitLabel)}" disabled title="Submit is disabled in Design Preview" />
      <span class="preview-only-hint">Preview only — Submit disabled so you can review the full form. Use Deploy to run it live.</span>
    </div>`
    : `<div class="form-footer">
      <input type="submit" name="submit" value="${esc(submitLabel)}" />
      ${startOver}
    </div>`;
  const formTagAttrs = options.designerPreview
    ? `${formAttrs} onsubmit="return false;"`
    : formAttrs;
  const body = `
  ${showPageChrome && !isRegistrationForm(formName) ? `<div class="project-title">${esc(project.name)}</div>` : ""}
  ${showPageChrome ? `<h1 class="form-title">${esc(formName)}</h1>` : ""}
  ${messageHtml}
  <form class="tawala-form" method="post" action="${action}"${formTagAttrs}>
    <input type="hidden" name="segmentId" value="${state.segmentIndex}" />
    ${itemHtml}
    ${formFooter}
  </form>`;

  const banner = options.designerPreview
    ? ""
    : `Tawala dev runtime — ${esc(project.name)} / ${esc(formName)} — page ${state.segmentIndex + 1} of ${segments.length}${bannerExtra}`;

  return {
    ok: true,
    theme,
    banner,
    body,
    title: `${project.name} — ${formName}`,
    form,
    segments,
  };
}

export function renderFormPage(project, formName, baseUrl, uniqueId, session, options = {}) {
  const parts = buildFormPageParts(project, formName, baseUrl, uniqueId, session, options);
  return pageShell(parts.title, parts.body, parts.banner, parts.theme);
}

export function handleFormSubmit(project, formName, session, body, baseUrl, uniqueId) {
  const form = project.forms?.find((f) => f.name === formName);
  if (!form) return pageShell("Error", "<h1>Form not found</h1>", "", "default");

  applySubmissionToSession(session, formName, body, form);
  const ctx = buildContext(session, formName);
  ctx.formName = formName;
  ctx.blankAliases = blankAliasesFromForm(form);
  ctx.project = project;

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

  {
    const fibErr = validateFibBlanks(form, ctx, prevSegment.items);
    if (fibErr) {
      session.fields.Message = fibErr;
      ctx.fields.Message = fibErr;
      return renderFormPage(project, formName, baseUrl, uniqueId, session);
    }
    if (session.fields.Message && session.fields.Message !== " ") {
      session.fields.Message = " ";
      ctx.fields.Message = " ";
    }
  }

  const skipTarget = runSkipBlocks(prevSegment.skipBlocks, ctx);
  Object.assign(session.fields, ctx.fields);

  const finishFormAndMaybeProcess = () => {
    // Persist this response for itemization tables before clearing answers.
    appendFormRecord(session, formName, form);

    if (form.process) {
      const nav = runProcessByName(project, form.process, ctx);
      Object.assign(session.fields, ctx.fields);
      // Clear after process so Object.assign(ctx.fields) cannot restore submitted answers.
      // Records (for Document MQL tables) are kept separately.
      clearFormAnswers(session, formName, form);
      if (nav.type === "form") {
        const destForm = project.forms?.find((f) => f.name === nav.form);
        if (nav.form !== formName) clearFormAnswers(session, nav.form, destForm);
        formState(session, nav.form).segmentIndex = 0;
        formState(session, nav.form).skipStartLabel = null;
        return renderFormPage(project, nav.form, baseUrl, uniqueId, session);
      }
      if (nav.type === "documents") {
        const thenForm = nav.thenForm || null;
        let appendHtml = "";
        if (thenForm) {
          formState(session, thenForm).segmentIndex = 0;
          formState(session, thenForm).skipStartLabel = null;
          const thenFormDef = project.forms?.find((f) => f.name === thenForm);
          if (thenForm !== formName) clearFormAnswers(session, thenForm, thenFormDef);
          // Ensure aliases for *this* form are gone before stacking the blank questionnaire.
          clearFormAnswers(session, formName, form);
          appendHtml = buildFormPageParts(project, thenForm, baseUrl, uniqueId, session, {
            embedded: true,
          }).body;
        }
        return renderDocumentsPage(project, nav.documents, session, baseUrl, uniqueId, {
          fromForm: formName,
          thenForm: appendHtml ? null : thenForm,
          appendHtml,
          freshThenForm: true,
          freshBack: true,
        });
      }
      return renderSubmitAck(project, form, session, baseUrl, uniqueId, {
        note: `Post-process “${form.process}” finished without a show step in dev runtime.`,
      });
    }

    clearFormAnswers(session, formName, form);
    return renderSubmitAck(project, form, session, baseUrl, uniqueId, {
      note: isRegistrationForm(formName)
        ? "Dev runtime recorded field values."
        : "Your response has been recorded.",
    });
  };

  if (skipTarget === "__EndOfForm__") {
    return finishFormAndMaybeProcess();
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

  return finishFormAndMaybeProcess();
}

function applySubmissionToSession(session, formName, body, form) {
  if (!session.formFields[formName]) session.formFields[formName] = {};
  const sharedSlots = new Set(["a", "b", "c"]);
  const aliases = blankAliasesFromForm(form);
  // blank.name → alternate/display labels (inverse of aliases map)
  const labelsForBlank = new Map();
  for (const [label, blankName] of Object.entries(aliases)) {
    if (label === blankName) continue;
    if (!labelsForBlank.has(blankName)) labelsForBlank.set(blankName, []);
    labelsForBlank.get(blankName).push(label);
  }
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
        for (const label of labelsForBlank.get(blank) ?? []) {
          session.fields[label] = value;
          session.fields[`${formName}:${label}`] = value;
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
  const isReg = isRegistrationForm(form.name);
  // Fresh start so Signup Sheet (and other append-style forms) reopen blank.
  const back = `${baseUrl}/p/${uniqueId}/${encodeURIComponent(form.name)}?fresh=1`;
  const heading = isReg ? "Registration step complete" : "Thank you";
  const note =
    opts.note ??
    (isReg ? "Dev runtime recorded field values." : "Your response has been recorded.");
  const backLabel = isReg ? "← Back to registration" : `← Back to ${form.name}`;
  const body = `
  <h1>${esc(heading)}</h1>
  <p><strong>${esc(project.name)}</strong> — ${esc(form.name)}</p>
  <p>${esc(note)}</p>
  <p><a href="${back}">${esc(backLabel)}</a></p>`;
  return pageShell("Submitted", body, "Tawala dev runtime", theme);
}
