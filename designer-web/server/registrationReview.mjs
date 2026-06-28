import { expandDynamicChoices, getFieldValue, itemKey } from "./runtimeEngine.mjs";

function esc(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/</g, "&lt;")
    .replace(/>/g, "&gt;")
    .replace(/"/g, "&quot;");
}

function mcChoiceText(item, rawValue, ctx) {
  if (!rawValue) return "";
  const expanded = [];
  for (const c of item.choices ?? []) {
    if (c.type === "dynamic") expanded.push(...expandDynamicChoices(c, ctx));
    else expanded.push(c);
  }
  const hit = expanded.find((c) => String(c.value ?? c.label ?? c.name) === String(rawValue));
  return hit?.text ?? rawValue;
}

function checkboxText(item, ctx) {
  const val = getFieldValue(ctx, itemKey(item)) || getFieldValue(ctx, item.name) || "";
  const checked = String(val).split(",").includes("a");
  return checked ? "Yes" : "No";
}

function row(label, value) {
  const display = value != null && String(value).trim() !== "" ? String(value) : " ";
  return `<tr><th scope="row">${esc(label)}</th><td>${esc(display)}</td></tr>`;
}

function section(title, rows) {
  return `<div class="reg-review-section">
    <div class="reg-review-section-head">${esc(title)}</div>
    <table class="reg-review-section-body"><tbody>${rows.join("")}</tbody></table>
  </div>`;
}

/** Build page-3 review synopsis — grouped sections matching legacy DirtBowl review page. */
export function buildRegistrationReviewTable(form, ctx) {
  const g = (ref) => getFieldValue(ctx, ref) || getFieldValue(ctx, `Registration:${ref}`) || "";
  const dob = [g("RegAgeMo"), g("RegAgeDay"), g("RegAgeYr")].filter(Boolean).join("/");

  const sexItem = form.items?.find((i) => i.name === "SexMCQ");
  const shirtItem = form.items?.find((i) => i.name === "ShirtSize");
  const divItem = form.items?.find((i) => i.name === "DivRequest");
  const lastDivItem = form.items?.find((i) => i.name === "LastDiv");
  const coachItem = form.items?.find((i) => i.label === "Q7");
  const umpireItem = form.items?.find((i) => i.label === "Q8");

  const friends = [g("Q10:a"), g("Q10:b"), g("Q10:c")]
    .filter((v) => v && String(v).trim())
    .join(", ");

  const addressParts = [g("Address"), g("City"), g("Zip")].filter((v) => v && String(v).trim());
  const addressLine = addressParts.join(", ");

  const emails = [g("ParentEmail"), g("ParentEmail2")].filter((v) => v && String(v).trim()).join(", ");

  const playerRows = [
    row("Name", `${g("FirstName")} ${g("LastName")}`.trim()),
    row("Date of Birth", dob),
    row("Gender", g("Sx") || mcChoiceText(sexItem, g("SexMCQ"), ctx)),
    row("School", g("Q3:a") || g("a")),
    row("Division requested", mcChoiceText(divItem, g("DivRequest"), ctx)),
    row("Division last year", mcChoiceText(lastDivItem, g("LastDiv"), ctx)),
  ];

  const parentRows = [
    row("Name", `${g("ParentFirstName")} ${g("ParentLastName")}`.trim()),
    row("Address", addressLine),
    row("Email", emails),
    row("Phone 1", g("Phone1")),
    row("Phone 2", g("Phone2")),
    row("Phone 3", g("Phone3")),
  ];

  const prefRows = [
    row("Coach preference", g("PreferredCoach")),
    row("Friends on team", friends),
    row("T-shirt size", mcChoiceText(shirtItem, g("ShirtSize"), ctx) || g("Jersey")),
    row("Parent willing to coach", coachItem ? checkboxText(coachItem, ctx) : ""),
    row("Player willing to umpire", umpireItem ? checkboxText(umpireItem, ctx) : ""),
  ];

  const medicalRows = [
    row("Plan name", g("MedPlanName")),
    row("Plan number", g("MedPlanNumber")),
    row("Physician", g("DocName")),
    row("Physician phone", g("DocPhone")),
    row("Physician address", g("DocAddress")),
  ];

  return `<div class="reg-review-sections">
    ${section("Player", playerRows)}
    ${section("Parent / Guardian", parentRows)}
    ${section("Preferences", prefRows)}
    ${section("Medical", medicalRows)}
  </div>`;
}
