#!/usr/bin/env node
const REG_URL =
  process.env.REG_URL ||
  "http://localhost:8080/p/1ucetrtc0an5ue0/nftivyp.Registration";

const jar = new Map();
const cookie = () => [...jar].map(([k, v]) => `${k}=${v}`).join("; ");
const pick = (h, n) => h.match(new RegExp(`name="${n}" value="([^"]*)"`))?.[1] ?? "";

async function fetchRaw(method, url, body) {
  const headers = { Cookie: cookie() };
  if (body) headers["Content-Type"] = "application/x-www-form-urlencoded";
  const res = await fetch(url, { method, headers, body, redirect: "manual" });
  for (const [k, v] of res.headers.entries()) {
    if (k.toLowerCase() === "set-cookie") {
      const [pair] = v.split(";");
      const i = pair.indexOf("=");
      jar.set(pair.slice(0, i), pair.slice(i + 1));
    }
  }
  return { status: res.status, location: res.headers.get("location"), html: await res.text() };
}

async function follow(res) {
  if (res.status < 300 || res.status >= 400 || !res.location) return res;
  return fetchRaw("GET", new URL(res.location, REG_URL).href);
}

async function post(html, fields) {
  const payload = new URLSearchParams({
    _form: pick(html, "_form"),
    _segment: pick(html, "_segment"),
    _uio01: pick(html, "_uio01"),
    ...fields,
  });
  return follow(await fetchRaw("POST", REG_URL, payload.toString()));
}

function label(html) {
  if (/error occured/i.test(html)) return "BackButtonNavigationPage";
  if (/Step 1 of your registration/i.test(html)) return "RegStep2";
  if (/review the information below carefully/i.test(html)) return "Review";
  if (pick(html, "_segment") === "1") return "Page2";
  if (pick(html, "_segment") === "0") return "Page1";
  return `Unknown(seg=${pick(html, "_segment")})`;
}

// Page 1 (segment 0): registrant + parent block Q4
const page1 = {
  FirstName: "Test",
  LastName: "Player",
  RegAgeMo: "06",
  RegAgeDay: "15",
  RegAgeYr: "2015",
  SexMCQ: "a",
  "Q3:a": "on",
  ParentFirstName: "Pat",
  ParentLastName: "Parent",
  Address: "123 Main St",
  City: "Springfield",
  Zip: "62701",
  ParentEmail: "a@b.com",
  "Q4:g": "a@b.com",
  ParentEmail2: "",
  ParentEmail2Confirm: "",
  Phone1: "217",
  Phone2: "555",
  Phone3: "1234",
};

// Page 2 (segment 1): divisions + medical
const page2 = {
  DivRequest: "2",
  LastDiv: "1",
  ShirtSize: "a",
  DocName: "Dr Smith",
  DocPhone: "2175559999",
  DocAddress: "123 Clinic",
  MedPlanName: "",
  MedPlanNumber: "",
  "Friend:a": "",
  "Friend:b": "",
  "Friend:c": "",
};

let res = await fetchRaw("GET", REG_URL);
console.log("Start  →", label(res.html));
res = await post(res.html, page1);
console.log("Page 1 →", label(res.html));
res = await post(res.html, page2);
console.log("Page 2 →", label(res.html));
const reviewHtml = res.html;
res = await post(reviewHtml, { InfoCorrect: "b" });
console.log("Accept →", label(res.html), res.location ? `(redirect ${res.location})` : "");

if (/Review/.test(label(reviewHtml))) {
  const stale = await post(reviewHtml, { InfoCorrect: "b" });
  console.log("Resubmit stale token →", label(stale.html));
}
