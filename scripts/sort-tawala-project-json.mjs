#!/usr/bin/env node
/**
 * Consolidate JSON projects under ~/Projects/Tawala Projects into
 * "JSON Library Sort/" — Main Menu first, then unique families, X* ignored.
 *
 * Copies only; does not delete sources.
 *
 *   node scripts/sort-tawala-project-json.mjs
 */
import fs from "node:fs";
import path from "node:path";
import os from "node:os";
import crypto from "node:crypto";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO = path.resolve(__dirname, "..");
const ROOT = path.join(os.homedir(), "Projects/Tawala Projects");
const DEST = path.join(ROOT, "JSON Library Sort");
const DESIGNER_TEMPLATES = path.join(
  REPO,
  "designer-web/public/samples/templates",
);
const DESIGNER_SAMPLES = path.join(REPO, "designer-web/public/samples");

const MAIN_MENU = [
  ["empty", "Empty", "empty-project.json"],
  ["form with process", "Form with Process", "form-with-process.json"],
  [
    "form process document",
    "Form Process and Document",
    "form-process-document.json",
  ],
  ["sign-up sheet", "Sign-up Sheet", "signup-sheet.json"],
  ["get together", "Get Together", "get-together.json"],
  ["potluck", "Potluck", "potluck.json"],
  ["simple survey", "Simple Survey", "simple-survey.json"],
  [
    "multiple question survey",
    "Multiple Question Survey",
    "multiple-question-survey.json",
  ],
];
const MAIN_KEYS = new Set(MAIN_MENU.map(([k]) => k));
const RELATED = { "sign-up sheet w email": "Sign-up Sheet w Email" };

const SCRATCH_KEYS = new Set([
  "test",
  "test2",
  "test3",
  "testpageheaderjson",
  "temp",
  "latesttest",
  "save1",
  "save2",
  "mcq",
  "prelunch single mcq",
  "function where tests",
  "test for stored choices",
  "test of new document flow",
  "testofstyles",
  "displaylabeltest",
  "volunteertest",
  "paypal tester",
]);

function normKey(name) {
  let n = path.parse(name).name;
  n = n.replace(/^[Xx]/, "");
  n = n.toLowerCase().replace(/[_-]/g, " ");
  n = n.replace(/\s+copy(\s+\d+)?$/i, "");
  n = n.replace(/\s+template$/i, "");
  n = n.replace(/\.dgmod$/i, "");
  n = n.replace(/\.jdffix$/i, "");
  n = n.replace(/\s+/g, " ").trim();
  const compact = n.replace(/\s+/g, "");
  if (compact.includes("sportsdashboard") || n.includes("sports dashboard")) {
    if (compact.includes("stpatrick") || n.includes("st patrick")) {
      return "st patrick sports dashboards";
    }
    if (n.includes("v3") || compact.includes("version3") || n.includes("version 3")) {
      return "sports dashboards v3";
    }
    if (compact.includes("jdffix")) return "sports dashboards jdffix";
    return "sports dashboards";
  }
  const aliases = {
    "signup sheet": "sign-up sheet",
    "sign up sheet": "sign-up sheet",
    signupsheets: "signup sheets",
    dirtbowl: "dirtbowl",
    "simple survey": "simple survey",
    "get together": "get together",
    "form with process connecting a document": "form process document",
    "form process document": "form process document",
    "form with process": "form with process",
    "empty project": "empty",
    empty: "empty",
    "multiple question survey": "multiple question survey",
    "potluck kids too": "potluck kids too",
    "bb potluck": "bb potluck",
    "poll or survey": "poll or survey",
    "single question poll or survey": "single question poll or survey",
    "signup sheet w email": "sign-up sheet w email",
    "signup sheet template w email": "sign-up sheet w email",
    "sign up sheet w email": "sign-up sheet w email",
    "cff revised": "cff",
    "dirtbowl definition v2": "dirtbowl definition",
    "dirtbowl definition v3": "dirtbowl definition",
    realdirtsergei: "dirtbowl variants",
    realdirtwheader: "dirtbowl variants",
    "horses and penguins test": "horses and penguins",
    "cyo checkdeposit request": "cyo check deposit",
    "cyo checkdeposit request1": "cyo check deposit",
    "cyo check request": "cyo check request",
  };
  if (aliases[n]) return aliases[n];
  // Signup Sheet (+ Template) with Email — not "Emailer With Signup"
  if (
    (n.includes("signup sheet") ||
      n.includes("sign-up sheet") ||
      n.includes("sign up sheet")) &&
    n.includes("email")
  ) {
    return "sign-up sheet w email";
  }
  if (["sign up sheet", "signup sheet", "sign-up sheet"].includes(n)) {
    return "sign-up sheet";
  }
  return n;
}

const LABEL_OVERRIDES = {
  "bb potluck": "BB Potluck",
  bbbulkmail: "BBBulkMail",
  cff: "CFF",
  "cyo check deposit": "CYO Check Deposit",
  "cyo check request": "CYO Check Request",
  "cyo exceptions app": "CYO Exceptions App",
  dirtbowl: "Dirtbowl",
  "dirtbowl definition": "Dirtbowl Definition",
  "dirtbowl variants": "Dirtbowl Variants",
  "dirtbowl communicator": "Dirtbowl Communicator",
  alextimon: "AlexTimon",
  campaigndashboards: "CampaignDashboards",
  clientprofiler: "ClientProfiler",
  genericlistmanager: "GenericListManager",
  "mvsc communicator": "MVSC Communicator",
  "mvsc registration": "MVSC Registration",
  "sports dashboards": "SportsDashboards",
  "sports dashboards v3": "SportsDashboards V3",
  "st patrick sports dashboards": "St Patrick SportsDashboards",
  "shared to do": "Shared To-Do",
  "emailer with signup": "Emailer With Signup",
};

function folderLabel(key) {
  for (const [k, label] of MAIN_MENU) {
    if (k === key) return label;
  }
  if (RELATED[key]) return RELATED[key];
  if (LABEL_OVERRIDES[key]) return LABEL_OVERRIDES[key];
  return key
    .split(" ")
    .map((w) =>
      ["and", "or", "w", "a", "the", "of"].includes(w)
        ? w
        : w.charAt(0).toUpperCase() + w.slice(1),
    )
    .join(" ");
}

function safeName(s) {
  return s.replace(/[/\\]/g, "-").replace(/\s+/g, " ").trim().slice(0, 180);
}

function isJsonFile(name) {
  if (name.startsWith(".") || name.startsWith("_") || name === "README.txt") {
    return false;
  }
  if (name.toLowerCase().endsWith(".json")) return true;
  if (name === "TestPageHeaderjson") return true;
  return false;
}

function walkFiles(dir, out = []) {
  let entries;
  try {
    entries = fs.readdirSync(dir, { withFileTypes: true });
  } catch {
    return out;
  }
  for (const ent of entries) {
    const full = path.join(dir, ent.name);
    if (ent.isDirectory()) {
      if (path.resolve(full) === path.resolve(DEST)) continue;
      walkFiles(full, out);
    } else if (ent.isFile() && isJsonFile(ent.name)) {
      out.push(full);
    }
  }
  return out;
}

function collect() {
  const files = [];
  for (const full of walkFiles(ROOT)) {
    const rel = path.relative(ROOT, full);
    const data = fs.readFileSync(full);
    const hash = crypto.createHash("sha256").update(data).digest("hex");
    let proj = null;
    try {
      const j = JSON.parse(data.toString("utf8"));
      if (j && typeof j === "object") {
        proj = j.name || null;
        if (!proj && j.project && typeof j.project === "object") {
          proj = j.project.name || null;
        }
      }
    } catch {
      /* ignore */
    }
    const name = path.basename(full);
    const ignore = /^[Xx]/.test(name);
    files.push({
      path: full,
      rel,
      name,
      size: data.length,
      hash,
      hash16: hash.slice(0, 16),
      key: normKey(name),
      proj,
      ignore,
      fromDiscard: rel.replace(/\\/g, "/").includes("Triage group/Discard"),
      mtime: fs.statSync(full).mtimeMs,
    });
  }
  return files;
}

function pickPrimary(group) {
  const scored = group.map((f) => {
    let score = 0;
    if (f.ignore) score -= 1000;
    if (f.fromDiscard) score -= 500;
    const rel = f.rel.replace(/\\/g, "/");
    if (rel.startsWith("Library Projects/")) score += 200;
    if (rel.includes("Triage group/Good/")) score += 80;
    if (rel.includes("Under Review/")) score += 40;
    if (rel.includes("From PC/JSON Conversions/")) score += 30;
    if (rel.includes("Misc old .Tawala files/JSON Conversions/")) score += 20;
    score += Math.min(Math.floor(f.size / 1000), 500);
    return { score, f };
  });
  scored.sort(
    (a, b) =>
      b.score - a.score || b.f.mtime - a.f.mtime || b.f.size - a.f.size,
  );
  return scored[0].f;
}

function provenanceFilename(f, used) {
  let origin = path.dirname(f.rel).replace(/[/\\]/g, "__").replace(/ /g, "_");
  if (!origin || origin === ".") origin = "ROOT";
  let base = f.name;
  if (!base.toLowerCase().endsWith(".json")) base += ".json";
  let candidate = safeName(`${f.hash16}__${origin}__${base}`);
  const stem = candidate;
  let n = 2;
  while (used.has(candidate.toLowerCase())) {
    candidate = `${stem}__${n}`;
    n += 1;
  }
  used.add(candidate.toLowerCase());
  return candidate;
}

const manifestRows = [];

function ensureDir(d) {
  fs.mkdirSync(d, { recursive: true });
}

function copyInto(folder, f, destName) {
  ensureDir(folder);
  fs.copyFileSync(f.path, path.join(folder, destName));
  manifestRows.push({
    group: path.relative(DEST, folder),
    dest_name: destName,
    bytes: String(f.size),
    sha256_16: f.hash16,
    project_name: f.proj || "",
    source: f.rel,
    ignore_flag: f.ignore ? "X" : "",
    role: "",
  });
}

function setRole(role) {
  manifestRows[manifestRows.length - 1].role = role;
}

function rmrf(dir) {
  if (!fs.existsSync(dir)) return;
  fs.rmSync(dir, { recursive: true, force: true });
}

function countJson(dir) {
  let n = 0;
  for (const full of walkFiles(dir)) n += 1;
  return n;
}

function main() {
  if (!fs.existsSync(ROOT)) {
    console.error(`Missing: ${ROOT}`);
    process.exit(1);
  }

  rmrf(DEST);
  const mainDir = path.join(DEST, "1-Main-Menu");
  const relatedDir = path.join(DEST, "1b-Main-Menu-related");
  const uniqueDir = path.join(DEST, "2-Unique-Projects");
  const ignoreDir = path.join(DEST, "3-Ignore-X-prefixed");
  const scratchDir = path.join(DEST, "4-Scratch-and-test-JSON");
  for (const d of [mainDir, relatedDir, uniqueDir, ignoreDir, scratchDir]) {
    ensureDir(d);
  }

  const files = collect();
  const byKey = new Map();
  for (const f of files) {
    if (!byKey.has(f.key)) byKey.set(f.key, []);
    byKey.get(f.key).push(f);
  }

  for (const [key, label, designerFile] of MAIN_MENU) {
    const folder = path.join(mainDir, label);
    const near = path.join(folder, "_near-duplicates");
    ensureDir(near);
    const used = new Set();

    const srcT = path.join(DESIGNER_TEMPLATES, designerFile);
    if (fs.existsSync(srcT)) {
      const data = fs.readFileSync(srcT);
      const h16 = crypto.createHash("sha256").update(data).digest("hex").slice(0, 16);
      const destName = `00-WebDesigner-MainMenu__${designerFile}`;
      fs.copyFileSync(srcT, path.join(folder, destName));
      manifestRows.push({
        group: path.relative(DEST, folder),
        dest_name: destName,
        bytes: String(data.length),
        sha256_16: h16,
        project_name: "(Web Designer New Project template)",
        source: `AI-Tawala/designer-web/public/samples/templates/${designerFile}`,
        ignore_flag: "",
        role: "web-designer-main-menu",
      });
      used.add(destName.toLowerCase());
    }

    const group = byKey.get(key) || [];
    const active = group.filter((f) => !f.ignore);
    const ignored = group.filter((f) => f.ignore);

    if (active.length) {
      const primary = pickPrimary(active);
      const primaryName = `${label}__suggested-primary.json`;
      copyInto(folder, primary, primaryName);
      setRole("suggested-primary");
      used.add(primaryName.toLowerCase());
      const seen = new Set([primary.hash]);
      for (const f of [...active].sort(
        (a, b) => b.size - a.size || a.rel.localeCompare(b.rel),
      )) {
        if (f === primary) continue;
        if (seen.has(f.hash)) {
          copyInto(
            near,
            f,
            `EXACT-DUP-of-primary__${provenanceFilename(f, used)}`,
          );
          setRole("exact-duplicate");
          continue;
        }
        seen.add(f.hash);
        copyInto(near, f, provenanceFilename(f, used));
        setRole("near-duplicate");
      }
    }

    for (const f of ignored) {
      const gignore = path.join(ignoreDir, label);
      ensureDir(gignore);
      let name = provenanceFilename(f, new Set());
      if (!/^[Xx]/.test(name)) name = `X__${name}`;
      copyInto(gignore, f, name);
      setRole("ignore-X");
    }
  }

  for (const [key, label] of Object.entries(RELATED)) {
    const folder = path.join(relatedDir, label);
    const near = path.join(folder, "_near-duplicates");
    ensureDir(near);
    const used = new Set();
    const group = (byKey.get(key) || []).filter((f) => !f.ignore);
    if (!group.length) continue;
    const primary = pickPrimary(group);
    copyInto(folder, primary, `${label}__suggested-primary.json`);
    setRole("suggested-primary");
    const seen = new Set([primary.hash]);
    for (const f of [...group].sort(
      (a, b) => b.size - a.size || a.rel.localeCompare(b.rel),
    )) {
      if (f === primary) continue;
      if (seen.has(f.hash)) {
        copyInto(
          near,
          f,
          `EXACT-DUP-of-primary__${provenanceFilename(f, used)}`,
        );
        setRole("exact-duplicate");
        continue;
      }
      seen.add(f.hash);
      copyInto(near, f, provenanceFilename(f, used));
      setRole("near-duplicate");
    }
  }

  // Signup Sheets open sample
  {
    const fname = "signup-sheets.json";
    const label = "Signup Sheets";
    const key = "signup sheets";
    const folder = path.join(relatedDir, label);
    const near = path.join(folder, "_near-duplicates");
    ensureDir(near);
    const used = new Set();
    const src = path.join(DESIGNER_SAMPLES, fname);
    if (fs.existsSync(src)) {
      const data = fs.readFileSync(src);
      const h16 = crypto
        .createHash("sha256")
        .update(data)
        .digest("hex")
        .slice(0, 16);
      const destName = `00-WebDesigner-OpenSample__${fname}`;
      fs.copyFileSync(src, path.join(folder, destName));
      manifestRows.push({
        group: path.relative(DEST, folder),
        dest_name: destName,
        bytes: String(data.length),
        sha256_16: h16,
        project_name: "(Web Designer Open Project sample)",
        source: `AI-Tawala/designer-web/public/samples/${fname}`,
        ignore_flag: "",
        role: "web-designer-open-sample",
      });
    }
    const group = (byKey.get(key) || []).filter((f) => !f.ignore);
    if (group.length) {
      const primary = pickPrimary(group);
      copyInto(folder, primary, `${label}__suggested-primary.json`);
      setRole("suggested-primary");
      const seen = new Set([primary.hash]);
      for (const f of [...group].sort(
        (a, b) => b.size - a.size || a.rel.localeCompare(b.rel),
      )) {
        if (f === primary) continue;
        if (seen.has(f.hash)) {
          copyInto(
            near,
            f,
            `EXACT-DUP-of-primary__${provenanceFilename(f, used)}`,
          );
          setRole("exact-duplicate");
          continue;
        }
        seen.add(f.hash);
        copyInto(near, f, provenanceFilename(f, used));
        setRole("near-duplicate");
      }
    }
  }

  const handled = new Set([...MAIN_KEYS, ...Object.keys(RELATED), "signup sheets"]);

  for (const key of [...byKey.keys()].sort()) {
    if (handled.has(key)) continue;
    const group = byKey.get(key);
    const active = group.filter((f) => !f.ignore);
    const ignored = group.filter((f) => f.ignore);
    const label = folderLabel(key);

    for (const f of ignored) {
      const gignore = path.join(ignoreDir, label);
      ensureDir(gignore);
      copyInto(gignore, f, provenanceFilename(f, new Set()));
      setRole("ignore-X");
    }
    if (!active.length) continue;

    const targetRoot = SCRATCH_KEYS.has(key) ? scratchDir : uniqueDir;

    if (active.length === 1 && !SCRATCH_KEYS.has(key)) {
      const f = active[0];
      let destName = `${label}.json`;
      let n = 2;
      while (fs.existsSync(path.join(targetRoot, destName))) {
        destName = `${label}__${n}.json`;
        n += 1;
      }
      copyInto(targetRoot, f, destName);
      setRole("unique-single");
      continue;
    }

    const folder = path.join(targetRoot, label);
    const near = path.join(folder, "_near-duplicates");
    ensureDir(near);
    const used = new Set();
    const primary = pickPrimary(active);
    copyInto(folder, primary, `${label}__suggested-primary.json`);
    setRole("suggested-primary");
    const seen = new Set([primary.hash]);
    for (const f of [...active].sort(
      (a, b) => b.size - a.size || a.rel.localeCompare(b.rel),
    )) {
      if (f === primary) continue;
      if (seen.has(f.hash)) {
        copyInto(
          near,
          f,
          `EXACT-DUP-of-primary__${provenanceFilename(f, used)}`,
        );
        setRole("exact-duplicate");
        continue;
      }
      seen.add(f.hash);
      copyInto(near, f, provenanceFilename(f, used));
      setRole("near-duplicate");
    }
  }

  const today = new Date().toISOString().slice(0, 10);
  const readme = `JSON Library Sort
Created: ${today}

Purpose
-------
Consolidate JSON project copies from across ~/Projects/Tawala Projects into one
place, grouped by near-duplicate name/content families, BEFORE hunting for
images and theme CSS.

IMPORTANT
---------
- Source folders were NOT deleted. This tree is COPIES only.
- Files whose names start with "X" are under 3-Ignore-X-prefixed/ (your ignore mark).
- Suggested primary = Library / Good / size / freshness heuristic — please confirm.
- Exact byte-duplicates of the primary are labeled EXACT-DUP-of-primary__…
- Near-duplicate filenames include sha16 + source path so you can see provenance.

Layout
------
1-Main-Menu/
  One folder per Web Designer File → New Project template.
  Each folder has:
    00-WebDesigner-MainMenu__*.json   = current New Project starter from designer-web
    *__suggested-primary.json         = best copy found under Tawala Projects
    _near-duplicates/                  = other name/size variants for YOU to triage

1b-Main-Menu-related/
  Sign-up Sheet w Email (not in New Project picker; mail backlog)
  Signup Sheets (Web Designer Open Project sample + Library copies)

2-Unique-Projects/
  Distinct Library / converted projects (singles as files; families as folders)

3-Ignore-X-prefixed/
  Copies of projects you marked with a leading X

4-Scratch-and-test-JSON/
  Designer scratch / test saves (Test.json, Save1, MCQ dumps, etc.)

Your next step
--------------
Walk each _near-duplicates/ folder. Keep/rename as you like, or prefix with X
any you want ignored going forward. When a family is settled, we can promote
suggested primaries into Library Projects/ for Deploy smoke.

Web Designer Main Menu (File → New Project) covered
-------------------------------------------------
Empty; Form with Process; Form, Process and Document; Sign-up Sheet;
Get Together; Potluck; Simple Survey; Multiple Question Survey.

Re-run
------
  cd ~/Projects/AI-Tawala && node scripts/sort-tawala-project-json.mjs
`;
  fs.writeFileSync(path.join(DEST, "README.txt"), readme);

  const header = [
    "group",
    "role",
    "dest_name",
    "bytes",
    "sha256_16",
    "project_name",
    "source",
    "ignore_flag",
  ];
  const csvLines = [
    header.join(","),
    ...manifestRows.map((row) =>
      header
        .map((h) => {
          const v = String(row[h] ?? "");
          return /["\n,]/.test(v) ? `"${v.replace(/"/g, '""')}"` : v;
        })
        .join(","),
    ),
  ];
  fs.writeFileSync(path.join(DEST, "MANIFEST.csv"), csvLines.join("\n") + "\n");

  const lines = [];
  lines.push(`Copied entries in MANIFEST: ${manifestRows.length}`);
  lines.push(`1-Main-Menu files: ${countJson(mainDir)}`);
  lines.push(`1b-related files: ${countJson(relatedDir)}`);
  lines.push(`2-Unique files: ${countJson(uniqueDir)}`);
  lines.push(`3-Ignore-X files: ${countJson(ignoreDir)}`);
  lines.push(`4-Scratch files: ${countJson(scratchDir)}`);
  lines.push("");
  lines.push("Main Menu folders:");
  for (const name of fs.readdirSync(mainDir).sort()) {
    const child = path.join(mainDir, name);
    if (!fs.statSync(child).isDirectory()) continue;
    const near = path.join(child, "_near-duplicates");
    const nTop = fs.readdirSync(child).filter((n) => {
      const p = path.join(child, n);
      return fs.statSync(p).isFile();
    }).length;
    const nNear = fs.existsSync(near)
      ? fs.readdirSync(near).filter((n) =>
          fs.statSync(path.join(near, n)).isFile(),
        ).length
      : 0;
    lines.push(`  ${name}: ${nTop} top-level, ${nNear} near-duplicates`);
  }
  lines.push("");
  lines.push("Unique folders / singles:");
  const uniqueKids = fs.readdirSync(uniqueDir).sort((a, b) => {
    const ad = fs.statSync(path.join(uniqueDir, a)).isDirectory();
    const bd = fs.statSync(path.join(uniqueDir, b)).isDirectory();
    if (ad !== bd) return ad ? -1 : 1;
    return a.localeCompare(b);
  });
  for (const name of uniqueKids) {
    const child = path.join(uniqueDir, name);
    if (fs.statSync(child).isDirectory()) {
      const near = path.join(child, "_near-duplicates");
      const nNear = fs.existsSync(near)
        ? fs.readdirSync(near).filter((n) =>
            fs.statSync(path.join(near, n)).isFile(),
          ).length
        : 0;
      lines.push(`  [family] ${name}: primary + ${nNear} near-dups`);
    } else {
      lines.push(`  [single] ${name}`);
    }
  }

  fs.writeFileSync(path.join(DEST, "SUMMARY.txt"), lines.join("\n") + "\n");
  console.log(lines.join("\n"));
  console.log(`\nDEST=${DEST}`);
}

main();
