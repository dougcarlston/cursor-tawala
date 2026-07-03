import { getFieldValue, itemKey, expandDynamicChoices } from "./runtimeEngine.mjs";
import { buildRegistrationReviewTable } from "./registrationReview.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function fieldStored(ctx, formName, itemLabel, blankName) {
  const itemKey = `${itemLabel}:${blankName}`;
  if (ctx.formFields?.[formName]?.[itemKey] !== undefined) {
    return ctx.formFields[formName][itemKey];
  }
  const formItemKey = `${formName}:${itemLabel}:${blankName}`;
  if (ctx.fields[formItemKey] !== undefined) return ctx.fields[formItemKey];
  if (ctx.fields[itemKey] !== undefined) return ctx.fields[itemKey];
  return undefined;
}

function val(ctx, formName, item, blank) {
  const stored = fieldStored(ctx, formName, item.label, blank.name);
  if (stored !== undefined) return stored;
  const alt = blank.alternateLabel || blank.name;
  if (alt !== blank.name) {
    const altStored = fieldStored(ctx, formName, item.label, alt);
    if (altStored !== undefined) return altStored;
  }
  return "";
}

function textInput(formName, item, blank, ctx, { size, className = "reg-input" } = {}) {
  const fname = `${item.label}:${blank.name}`;
  const v = val(ctx, formName, item, blank);
  const layoutSized = /\breg-(name|full|email|phone|grid-address|friend)\b/.test(className);
  const sz = size ?? blank.length ?? 20;
  const sizeAttr = layoutSized ? "" : ` size="${sz}"`;
  return `<input type="text" class="${className}" name="${esc(fname)}" value="${esc(v)}"${sizeAttr} />`;
}

function hintRow(...hints) {
  return `<div class="reg-hint-row">${hints.map((h) => `<em class="fib-hint">${esc(h)}</em>`).join("")}</div>`;
}

function nameHintRow() {
  return `<div class="reg-hint-row reg-name-hints"><em class="fib-hint">First</em><em class="fib-hint">Last</em></div>`;
}

function labelRow(label, fieldsHtml, { bold = true } = {}) {
  const cls = bold ? "reg-label reg-label-bold" : "reg-label";
  return `<div class="reg-field-row"><span class="${cls}">${esc(label)}:</span><div class="reg-fields">${fieldsHtml}</div></div>`;
}

function renderQ1(item, ctx, formName) {
  const blanks = item.blanks ?? [];
  const [first, last, mo, day, yr] = blanks;
  return `<div class="fib fib-reg" id="item-${esc(itemKey(item))}">
    ${nameHintRow()}
    <div class="reg-field-row reg-name-row">
      <span class="reg-label reg-label-bold">Name of Registrant:</span>
      <div class="reg-fields reg-name-fields">
        ${textInput(formName, item, first, ctx, { className: "reg-input reg-name" })}
        ${textInput(formName, item, last, ctx, { className: "reg-input reg-name reg-name-last" })}
      </div>
    </div>
    <div class="reg-field-row reg-dob-row">
      <span class="reg-label reg-label-bold">Date of Birth: <em class="fib-hint fib-hint-inline">(mm/dd/yyyy)</em></span>
      <div class="reg-fields reg-dob-fields">
        ${textInput(formName, item, mo, ctx, { size: 3, className: "reg-input reg-date" })}
        <span class="reg-date-sep">/</span>
        ${textInput(formName, item, day, ctx, { size: 3, className: "reg-input reg-date" })}
        <span class="reg-date-sep">/</span>
        ${textInput(formName, item, yr, ctx, { size: 5, className: "reg-input reg-date-year" })}
      </div>
    </div>
  </div>`;
}

function renderQ3(item, ctx, formName) {
  const blank = item.blanks?.[0];
  return `<div class="fib fib-reg" id="item-${esc(itemKey(item))}">
    ${labelRow("Name of your School", textInput(formName, item, blank, ctx, { className: "reg-input reg-full" }))}
  </div>`;
}

function renderQ4(item, ctx, formName) {
  const b = item.blanks ?? [];
  const byName = (n) => b.find((x) => x.name === n || x.alternateLabel === n) ?? { name: n, length: 18 };
  return `<div class="fib fib-reg fib-reg-q4" id="item-${esc(itemKey(item))}">
    ${nameHintRow()}
    <div class="reg-field-row reg-name-row">
      <span class="reg-label reg-label-bold">Parent Name:</span>
      <div class="reg-fields reg-name-fields">
        ${textInput(formName, item, byName("ParentFirstName"), ctx, { className: "reg-input reg-name" })}
        ${textInput(formName, item, byName("ParentLastName"), ctx, { className: "reg-input reg-name reg-name-last" })}
      </div>
    </div>
    <div class="reg-field-row reg-address-row">
      <span class="reg-label reg-label-bold">Address:</span>
      <div class="reg-fields reg-address-line">
        ${textInput(formName, item, byName("Address"), ctx, { className: "reg-input reg-grid-address" })}
        <span class="reg-inline-label reg-label-bold">City:</span>
        ${textInput(formName, item, byName("City"), ctx, { className: "reg-input reg-city" })}
        <span class="reg-inline-label reg-label-bold">Zip:</span>
        ${textInput(formName, item, byName("Zip"), ctx, { className: "reg-input reg-zip" })}
      </div>
    </div>
    <div class="reg-field-row reg-email-row">
      <span class="reg-label reg-label-bold">Parent Email:</span>
      <div class="reg-fields reg-email-pair">
        ${textInput(formName, item, byName("ParentEmail"), ctx, { className: "reg-input reg-email" })}
        <span class="reg-email-again-label"><em class="fib-hint fib-hint-inline">(again)</em></span>
        ${textInput(formName, item, byName("g"), ctx, { className: "reg-input reg-email reg-email-last" })}
      </div>
    </div>
    <div class="reg-field-row reg-email-row">
      <span class="reg-label reg-label-bold reg-label-stack">
        <span>Additional Parent Email:</span>
        <em class="fib-hint">(optional)</em>
      </span>
      <div class="reg-fields reg-email-pair">
        ${textInput(formName, item, byName("ParentEmail2"), ctx, { className: "reg-input reg-email" })}
        <span class="reg-email-again-label"><em class="fib-hint fib-hint-inline">(again)</em></span>
        ${textInput(formName, item, byName("ParentEmail2Confirm"), ctx, { className: "reg-input reg-email reg-email-last" })}
      </div>
    </div>
    <p class="reg-note-line"><em class="fib-hint">(All communications will be by email only)</em></p>
    <div class="reg-field-row reg-phone-row">
      <span class="reg-label reg-label-bold reg-label-stack">
        <span>Parent Phone Numbers:</span>
        <em class="fib-hint">(for emergencies)</em>
      </span>
      <div class="reg-fields reg-phone-line">
        <span class="reg-inline-label reg-label-bold">P1:</span>
        ${textInput(formName, item, byName("Phone1"), ctx, { className: "reg-input reg-phone" })}
        <span class="reg-inline-label reg-label-bold">P2:</span>
        ${textInput(formName, item, byName("Phone2"), ctx, { className: "reg-input reg-phone" })}
        <span class="reg-inline-label reg-label-bold">P3:</span>
        ${textInput(formName, item, byName("Phone3"), ctx, { className: "reg-input reg-phone" })}
      </div>
    </div>
  </div>`;
}

function renderQ9(item, ctx, formName) {
  const blank = item.blanks?.[0];
  return `<div class="fib fib-reg" id="item-${esc(itemKey(item))}">
    <div class="reg-field-row">
      <span class="reg-label reg-label-bold">Coach Preferences <em class="fib-hint fib-hint-inline">(if any)</em>:</span>
      <div class="reg-fields">${textInput(formName, item, blank, ctx, { className: "reg-input reg-full" })}</div>
    </div>
  </div>`;
}

function renderQ10(item, ctx, formName) {
  const blanks = item.blanks ?? [];
  const fields = blanks
    .map((blank) => textInput(formName, item, blank, ctx, { className: "reg-input reg-friend" }))
    .join("");
  return `<div class="fib fib-reg fib-reg-friends" id="item-${esc(itemKey(item))}"><div class="reg-friend-fields">${fields}</div></div>`;
}

function renderQ12(item, ctx, formName) {
  const rows = [
    ["Plan Name", "MedPlanName"],
    ["Plan Number", "MedPlanNumber"],
    ["Child's Physician", "DocName"],
    ["Physician Phone", "DocPhone"],
    ["Physician Address", "DocAddress"],
  ];
  const b = item.blanks ?? [];
  const html = rows
    .map(([label, name]) => {
      const blank = b.find((x) => x.name === name || x.alternateLabel === name) ?? { name, length: 39 };
      return labelRow(label, textInput(formName, item, blank, ctx, { className: "reg-input reg-full" }));
    })
    .join("");
  return `<div class="fib fib-reg fib-reg-medical" id="item-${esc(itemKey(item))}">${html}</div>`;
}

export function renderRegistrationFib(item, ctx, formName) {
  switch (item.label) {
    case "Q1":
      return renderQ1(item, ctx, formName);
    case "Q3":
      return renderQ3(item, ctx, formName);
    case "Q4":
      return renderQ4(item, ctx, formName);
    case "Q9":
      return renderQ9(item, ctx, formName);
    case "Q10":
      return renderQ10(item, ctx, formName);
    case "Q12":
      return renderQ12(item, ctx, formName);
    default:
      return null;
  }
}

export function renderRegistrationText(item, ctx, formName, project) {
  switch (item.label) {
    case "T1": {
      const msg = getFieldValue(ctx, "Message")?.trim();
      if (!msg) return "";
      return `<div class="text reg-message"><p><strong><em>${esc(msg)}</em></strong></p></div>`;
    }
    case "T10": {
      const msg = getFieldValue(ctx, "Message")?.trim();
      if (!msg) return "";
      return `<div class="text reg-message"><p><strong><em>${esc(msg)}</em></strong></p></div>`;
    }
    case "T2": {
      const league = getFieldValue(ctx, "League") || "Dirt Bowl";
      const season = getFieldValue(ctx, "SeasonYr") || "";
      return `<div class="text reg-banner">
        <p class="reg-banner-league">${esc(league)}</p>
        <p class="reg-banner-season">${esc(season)} Summer Basketball League: Games Only</p>
        <p class="reg-banner-sub">Boys and Girls</p>
        <p class="reg-banner-invite">YOU ARE INVITED to participate in the Annual "${esc(league)}" Basketball League and Tournament!</p>
        <p class="reg-banner-invite">Information below. If you are age 8-13, please join us!</p>
      </div>`;
    }
    case "T3": {
      const admin = getFieldValue(ctx, "AdminName") || "«AdminName»";
      return `<div class="text reg-director">
        <p>Athletic Director: ${esc(admin)}</p>
        <p class="reg-signup-line"><strong>Sign up as a team or individually. If you sign up individually, you will be placed on a team.</strong></p>
      </div>`;
    }
    case "T4": {
      const league = getFieldValue(ctx, "League") || "";
      const status = getFieldValue(ctx, "PaymentStatus") || " ";
      const fee = getFieldValue(ctx, "SignupFeeForIndividual") || "";
      return `<div class="text reg-info-table-wrap">
        <table class="reg-info-table">
          <tr><td class="reg-info-label"><b>Registration:</b></td><td><b class="reg-league-red">${esc(league)}</b></td><td>Due date: <b>April 28, 2008</b></td></tr>
          <tr><td></td><td>${esc(getFieldValue(ctx, "AdminAdrss"))}</td><td></td></tr>
          <tr><td></td><td>${esc(getFieldValue(ctx, "AdminsCity"))}, ${esc(getFieldValue(ctx, "AdminState"))} ${esc(getFieldValue(ctx, "AdminsZIP"))}</td><td>Cost per player: <b>$${esc(fee)}</b></td></tr>
          <tr><td></td><td><span class="reg-payment-status">${esc(status)}</span></td><td></td></tr>
        </table>
      </div>`;
    }
    case "T6":
      return `<div class="text reg-friends-heading"><p><strong>Name of friends you would like on your team:</strong></p></div>`;
    case "T7":
      return `<div class="text reg-medical-heading"><p><strong>Medical Information:</strong></p></div>`;
    case "T8":
      return `<div class="text reg-review-intro"><p><strong><em>Please review the information below carefully.</em></strong></p>${buildRegistrationReviewTable(project.forms?.find((f) => f.name === formName) ?? { items: [] }, ctx)}</div>`;
    case "T9":
      return "";
    default:
      return null;
  }
}

export function renderRegistrationMc(item, ctx) {
  const inputType = item.onlyone !== false ? "radio" : "checkbox";
  const name = itemKey(item);
  const expanded = [];
  for (const c of item.choices ?? []) {
    if (c.type === "dynamic") expanded.push(...expandDynamicChoices(c, ctx));
    else expanded.push(c);
  }

  const choices = expanded
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

  if (item.label === "Q7" || item.label === "Q8") {
    return `<fieldset class="mc mc-checkbox-compact" id="item-${esc(itemKey(item))}"><div class="mc-choices">${choices}</div></fieldset>`;
  }

  if (item.label === "Q5") {
    const link = `<a href="#" class="reg-divisions-link" onclick="document.getElementById('divisions-help').showModal();return false;">click here</a>`;
    const prompt = `<p class="reg-division-prompt"><strong>Please indicate the division in which you'd like to play:</strong> <em class="fib-hint fib-hint-inline">(To learn more about divisions, ${link})</em></p>`;
    return `<div class="reg-division-block">${prompt}<fieldset class="mc mc-radio-aligned mc-divisions" id="item-${esc(itemKey(item))}"><div class="mc-choices">${choices}</div></fieldset>
      <dialog id="divisions-help" class="reg-help-dialog"><p>Divisions group players by age and skill. Your league administrator defines the available divisions each season.</p><button type="button" class="reg-dialog-close" onclick="document.getElementById('divisions-help').close()">Close</button></dialog></div>`;
  }

  if (item.label === "Q6") {
    const q = item.question ?? "If you played last year, please indicate the division in which you played:";
    return `<div class="reg-division-block"><p class="reg-division-prompt"><strong>${esc(q)}</strong></p><fieldset class="mc mc-radio-aligned mc-divisions" id="item-${esc(itemKey(item))}"><div class="mc-choices">${choices}</div></fieldset></div>`;
  }

  if (item.label === "Q2") {
    const q = item.question ?? "Sex of Registrant:";
    return `<div class="reg-field-row reg-sex-row" id="item-${esc(itemKey(item))}">
      <span class="reg-label reg-label-bold">${esc(q)}</span>
      <div class="reg-fields reg-sex-choices">${choices}</div>
    </div>`;
  }

  if (item.label === "Q11") {
    const q = item.question ?? "What is your jersey size?";
    return `<fieldset class="mc mc-jersey-size" id="item-${esc(itemKey(item))}"><legend class="reg-jersey-legend">${esc(q)}</legend><div class="mc-choices">${choices}</div></fieldset>`;
  }

  if (item.label === "Q13") {
    const q = item.question ?? "Select an option and click Submit.";
    return `<fieldset class="mc mc-review-confirm" id="item-${esc(itemKey(item))}"><legend>${esc(q)}</legend><div class="mc-choices">${choices}</div></fieldset>`;
  }

  const q = item.question ?? "";
  const legend = esc(q);
  const cls = "mc";
  return `<fieldset class="${cls}" id="item-${esc(itemKey(item))}"><legend>${legend}</legend><div class="mc-choices">${choices}</div></fieldset>`;
}

export function registrationPage2Footer() {
  return `<div class="text instructional reg-page-footer"><p>Press Submit to continue.</p></div>`;
}

export function isRegistrationForm(formName) {
  return formName === "Registration";
}
