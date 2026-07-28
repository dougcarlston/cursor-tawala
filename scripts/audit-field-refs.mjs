#!/usr/bin/env node
/**
 * Field-reference audit for browser-Designer project JSON.
 *
 * Report-only. Does NOT invent FIB alternate labels.
 *
 * Flags:
 *  1. Duplicate blank / field labels across forms (collision if Form: dropped)
 *  2. Bare <<Name>> tokens that match a form blank but not a Variable
 *  3. Record-scoped refs missing Record: (MQL / function Where / SUM), or
 *     Record:FIBn:a without a form name
 *  4. Get / ForEach missing recordName; loop body refs that look like Form:Field
 *     where peers use RecordName:Form:Field
 *
 * Also reuses Jul 22–24 hazard checks from triage-library-json.mjs when available.
 *
 * Usage (repo root):
 *   node scripts/audit-field-refs.mjs
 *   node scripts/audit-field-refs.mjs "/path/to/Library Projects"
 *   node scripts/audit-field-refs.mjs "/path/to/one.json"
 *
 * Default: ~/Projects/Tawala Projects/Library Projects
 *
 * Spec: Tawala_Key_Documents/FIELD_REFERENCE_AUDIT.md
 */

import fs from "node:fs";
import path from "node:path";
import os from "node:os";
import { fileURLToPath, pathToFileURL } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "..");

const CANDIDATE_DIRS = [
  path.join(os.homedir(), "Projects/Tawala Projects/Library Projects"),
  path.join(os.homedir(), "Projects/Tawala Projects/XLibrary Projects"),
];

function resolveDefaultDir() {
  for (const d of CANDIDATE_DIRS) {
    if (fs.existsSync(d)) return d;
  }
  return CANDIDATE_DIRS[0];
}

function usage() {
  console.error(`Usage: node scripts/audit-field-refs.mjs [path]

  path   Folder of *.json or a single .json
         (default: ~/Projects/Tawala Projects/Library Projects)
`);
  process.exit(2);
}

function parseArgs(argv) {
  let target = null;
  for (const a of argv) {
    if (a === "-h" || a === "--help") usage();
    if (a.startsWith("-")) {
      console.error(`Unknown option: ${a}`);
      usage();
    }
    target = a;
  }
  return {
    target: target ? path.resolve(process.cwd(), target) : resolveDefaultDir(),
  };
}

function projectRoot(data) {
  return data?.project && typeof data.project === "object" ? data.project : data;
}

function listJsonFiles(target) {
  if (!fs.existsSync(target)) {
    console.error(`Path not found: ${target}`);
    process.exit(1);
  }
  const st = fs.statSync(target);
  if (st.isFile()) {
    if (!target.toLowerCase().endsWith(".json")) {
      console.error("File must be .json");
      process.exit(1);
    }
    return [target];
  }
  return fs
    .readdirSync(target)
    .filter(
      (n) =>
        n.toLowerCase().endsWith(".json") &&
        !n.includes(".amended.") &&
        !n.includes("-field-audit"),
    )
    .map((n) => path.join(target, n))
    .sort((a, b) => path.basename(a).localeCompare(path.basename(b)));
}

function stripAngle(name) {
  let s = String(name ?? "").trim();
  if (s.startsWith("<<") && s.endsWith(">>")) s = s.slice(2, -2).trim();
  return s;
}

/** Collect form blank / field leaf names → list of form names that own them. */
function collectFormLeaves(project) {
  /** @type {Map<string, string[]>} */
  const byLabel = new Map();
  /** @type {Map<string, Set<string>>} label → forms (unique) */
  const add = (label, formName) => {
    const lab = String(label ?? "").trim();
    if (!lab) return;
    if (!byLabel.has(lab)) byLabel.set(lab, []);
    byLabel.get(lab).push(formName);
  };

  for (const form of project.forms ?? []) {
    const formName = String(form.name ?? "");
    for (const item of form.items ?? []) {
      if (!item || typeof item !== "object") continue;
      const t = item.type;
      if (t === "fib" || t === "fillInBlank") {
        const itemLabel = String(item.label ?? "");
        (item.blanks ?? []).forEach((b, i) => {
          const alt = b?.alternateLabel ?? b?.alt ?? b?.name ?? b?.label;
          if (alt) add(alt, formName);
          const letter =
            b?.name && /^[a-z]$/i.test(String(b.name))
              ? String(b.name)
              : String.fromCharCode(97 + i);
          if (itemLabel) add(`${itemLabel}:${letter}`, formName);
        });
      } else if (
        t === "mc" ||
        t === "multipleChoice" ||
        t === "hidden" ||
        t === "fileUploader"
      ) {
        const lab = item.fieldName ?? item.name ?? item.label;
        if (lab) add(lab, formName);
      }
    }
  }
  return byLabel;
}

function collectVariableNames(project) {
  const set = new Set();
  for (const v of project.variables ?? []) {
    if (typeof v === "string") set.add(v);
    else if (v && typeof v === "object") {
      const n = v.name ?? v.id ?? v.label;
      if (n) set.add(String(n));
    }
  }
  // Process Set targets often act as variables even if not listed
  walkCommands(project, (cmd) => {
    if (cmd.cmd === "set" && typeof cmd.field === "string") {
      const f = stripAngle(cmd.field);
      if (f && !f.includes(":")) set.add(f);
    }
  });
  return set;
}

function walkCommands(project, visit, pathStr = "") {
  const walk = (o, p) => {
    if (!o || typeof o !== "object") return;
    if (typeof o.cmd === "string") visit(o, p);
    for (const [k, v] of Object.entries(o)) {
      if (k === "parent") continue;
      if (Array.isArray(v)) {
        v.forEach((x, i) => walk(x, `${p}.${k}[${i}]`));
      } else if (v && typeof v === "object") {
        walk(v, `${p}.${k}`);
      }
    }
  };
  (project.processes ?? []).forEach((pr, i) => {
    const base = `processes[${i}:${pr.name ?? ""}]`;
    walk(pr, base);
  });
}

function walkAllStrings(o, pathStr, visit) {
  if (o == null) return;
  if (typeof o === "string") {
    visit(pathStr, o);
    return;
  }
  if (Array.isArray(o)) {
    o.forEach((v, i) => walkAllStrings(v, `${pathStr}[${i}]`, visit));
    return;
  }
  if (typeof o === "object") {
    for (const [k, v] of Object.entries(o)) {
      walkAllStrings(v, pathStr ? `${pathStr}.${k}` : k, visit);
    }
  }
}

/**
 * @returns {{ kind: string, path: string, n: number, sample: string }[]}
 */
function auditProject(project) {
  const findings = [];
  const leaves = collectFormLeaves(project);
  const variables = collectVariableNames(project);

  // 1) Duplicate blank labels across forms
  // Hazard only when a bare <<label>> also appears (Form: drop risk is real).
  // Shared labels that are always referenced as Form:Label stay info.
  const bareTokenNames = new Set();
  walkAllStrings(project, "", (_p, s) => {
    if (!s.includes("<<")) return;
    for (const m of s.matchAll(/<<([^<>]+)>>/g)) {
      const name = m[1].trim();
      if (name && !name.includes(":")) bareTokenNames.add(name);
    }
  });

  for (const [label, forms] of leaves) {
    const uniq = [...new Set(forms)];
    if (uniq.length <= 1) continue;
    const synthetic = /^(FIB|Q)\d+:[a-z]$/i.test(label) || /^Q\d+$/i.test(label);
    if (synthetic) {
      findings.push({
        kind: "info_dup_item_label_across_forms",
        path: `fields:${label}`,
        n: uniq.length,
        sample: `${label} on ${uniq.join(", ")}`,
      });
      continue;
    }
    const bareUsed = bareTokenNames.has(label);
    findings.push({
      kind: bareUsed
        ? "dup_blank_label_across_forms"
        : "info_dup_blank_label_qualified_refs",
      path: `fields:${label}`,
      n: uniq.length,
      sample: bareUsed
        ? `${label} on ${uniq.join(", ")} + bare <<${label}>> present`
        : `${label} on ${uniq.join(", ")} (refs appear Form-qualified; no bare token)`,
    });
  }

  // 2) Bare <<Name>> matching a form blank but not a declared/process variable
  const bareCounts = new Map();
  walkAllStrings(project, "", (p, s) => {
    if (!s.includes("<<")) return;
    for (const m of s.matchAll(/<<([^<>]+)>>/g)) {
      const name = m[1].trim();
      if (!name || name.includes(":")) continue;
      if (variables.has(name)) continue;
      if (!leaves.has(name)) continue;
      const forms = [...new Set(leaves.get(name))];
      const key = name;
      if (!bareCounts.has(key)) bareCounts.set(key, { n: 0, path: p, forms });
      bareCounts.get(key).n += 1;
      if (!bareCounts.get(key).path) bareCounts.get(key).path = p;
    }
  });
  for (const [name, info] of bareCounts) {
    findings.push({
      kind: "bare_token_matches_form_blank",
      path: info.path,
      n: info.n,
      sample: `<<${name}>> (forms: ${info.forms.join(", ")}) — likely needs Form: qualification`,
    });
  }

  // 3) Record-scoped contexts missing Record: / Record:FIBn without form
  const recordScopedKeys = new Set([
    "field",
    "where",
    "contents",
    "column",
    "columns",
  ]);

  const checkRecordishRef = (ref, pathStr, kindHint) => {
    const name = stripAngle(ref);
    if (!name) return;
    if (
      /^Record:(FIB|Q)\d+:[a-z]$/i.test(name) ||
      /^Record:(FIB|Q)\d+$/i.test(name)
    ) {
      findings.push({
        kind: "record_missing_form",
        path: pathStr,
        n: 1,
        sample: name,
      });
      return;
    }
    if (kindHint !== "record_scoped") return;
    // Already Record:Form:Field or Loop:Form:Field — OK
    if (name.startsWith("Record:")) return;
    if (/^[A-Za-z_][\w]*:[A-Za-z_][\w]*:[A-Za-z_]/.test(name)) return;
    // Two-part Form:Field in itemization/Where — candidate missing Record:
    const parts = name.split(":");
    if (parts.length === 2 && leaves.has(parts[1])) {
      findings.push({
        kind: "record_scoped_missing_Record_prefix",
        path: pathStr,
        n: 1,
        sample: name,
      });
    }
  };

  // Structured itemization / sum nodes
  const walkNodes = (nodes, pathStr) => {
    if (!Array.isArray(nodes)) return;
    nodes.forEach((n, i) => {
      if (!n || typeof n !== "object") return;
      const p = `${pathStr}[${i}]`;
      const t = n.type;
      if (t === "sum" || t === "itemizationTable" || t === "recordCount") {
        const field = n.field ?? n.contents ?? "";
        if (typeof field === "string" && field) {
          checkRecordishRef(field, `${p}.field`, "record_scoped");
        }
        for (const col of n.columns ?? n.column ?? []) {
          if (col && typeof col === "object") {
            const cfield = col.field ?? col.contents ?? col.value;
            if (typeof cfield === "string") {
              checkRecordishRef(cfield, `${p}.column`, "record_scoped");
            }
          }
        }
        const where = n.where ?? n.conditions;
        if (where && typeof where === "object") {
          const wf = where.field;
          if (typeof wf === "string") {
            checkRecordishRef(wf, `${p}.where.field`, "record_scoped");
          }
        }
      }
      if (Array.isArray(n.nodes)) walkNodes(n.nodes, `${p}.nodes`);
    });
  };

  (project.documents ?? []).forEach((doc, di) => {
    const content = doc?.content;
    if (Array.isArray(content)) {
      content.forEach((block, bi) => {
        if (block?.nodes) {
          walkNodes(block.nodes, `documents[${di}].content[${bi}].nodes`);
        }
      });
    }
  });

  // Function-token HTML configs (sum / itemization)
  walkAllStrings(project, "", (p, s) => {
    if (!s.includes("data-function")) return;
    for (const m of s.matchAll(
      /data-function-id="(sum|itemization-table)"[^>]*data-function-config="([^"]*)"/gi,
    )) {
      let cfg;
      try {
        const raw = m[2]
          .replace(/&quot;/g, '"')
          .replace(/&lt;/g, "<")
          .replace(/&gt;/g, ">")
          .replace(/&amp;/g, "&");
        cfg = JSON.parse(raw);
      } catch {
        continue;
      }
      const field = cfg.field ?? cfg.contents;
      if (typeof field === "string") {
        checkRecordishRef(field, p, "record_scoped");
      }
      for (const row of cfg.conditionsRows ?? cfg.conditions ?? []) {
        if (row?.field) checkRecordishRef(row.field, p, "record_scoped");
      }
      for (const col of cfg.column ?? cfg.columns ?? []) {
        const cf = col?.field ?? col?.contents;
        if (typeof cf === "string") checkRecordishRef(cf, p, "record_scoped");
      }
    }
    for (const m of s.matchAll(/Record:((?:FIB|Q)\d+:[a-z])\b/gi)) {
      findings.push({
        kind: "record_missing_form",
        path: p,
        n: 1,
        sample: `Record:${m[1]}`,
      });
    }
  });

  // Process where clauses on get/delete/foreach
  walkCommands(project, (cmd, p) => {
    const where = cmd.where;
    if (where && typeof where === "object" && typeof where.field === "string") {
      const f = stripAngle(where.field);
      // Get/delete where on stored forms often need Record: or RecordList:Form:Field
      if (
        (cmd.cmd === "get" || cmd.cmd === "delete") &&
        f.includes(":") &&
        !f.startsWith("Record:") &&
        !/^[A-Za-z_][\w]*:[A-Za-z_][\w]*:[A-Za-z_]/.test(f)
      ) {
        const parts = f.split(":");
        if (parts.length === 2) {
          // Common pattern: filter by current form multi-select / id field — info, not auto-fix.
          findings.push({
            kind: "info_get_where_form_field",
            path: `${p}.where.field`,
            n: 1,
            sample: f,
          });
        }
      }
    }
  });

  // 4) Get / ForEach recordName
  walkCommands(project, (cmd, p) => {
    if (cmd.cmd === "foreach") {
      const rn = String(cmd.recordName ?? "").trim();
      if (!rn) {
        findings.push({
          kind: "foreach_missing_recordName",
          path: p,
          n: 1,
          sample: `recordList=${cmd.recordList ?? ""}`,
        });
      } else {
        // Inside do[], look for <<Form:Field>> that might should be <<RecordName:Form:Field>>
        const body = cmd.do ?? cmd.commands ?? [];
        const bodyStr = JSON.stringify(body);
        const loopPrefixed = new RegExp(`<<${rn}:`, "g");
        const hasLoopPrefixed = loopPrefixed.test(bodyStr);
        if (hasLoopPrefixed) {
          for (const m of bodyStr.matchAll(/<<([^<>]+)>>/g)) {
            const name = m[1];
            const parts = name.split(":");
            // Two-part Form:Field inside a foreach that also uses RecordName:Form:Field.
            // Often intentional (current submission while iterating another list) — info only.
            if (
              parts.length === 2 &&
              !name.startsWith("Record:") &&
              parts[0] !== rn &&
              leaves.has(parts[1])
            ) {
              findings.push({
                kind: "info_foreach_body_current_form_field",
                path: p,
                n: 1,
                sample: `<<${name}>> inside foreach recordName=${rn} (peers use ${rn}:…; often current-form on purpose)`,
              });
            }
          }
        }
      }
    }
    if (cmd.cmd === "get") {
      findings.push({
        kind: "info_get_command",
        path: p,
        n: 1,
        sample: `recordList=${cmd.recordList ?? ""} forms=${(cmd.sourceForms || []).join(",")}`,
      });
    }
    if (cmd.cmd === "foreach" && cmd.recordName) {
      findings.push({
        kind: "info_foreach_command",
        path: p,
        n: 1,
        sample: `recordName=${cmd.recordName} list=${cmd.recordList ?? ""}`,
      });
    }
  });

  // Info: leave counts
  findings.push({
    kind: "info_form_leaf_count",
    path: "forms",
    n: leaves.size,
    sample: `${(project.forms ?? []).length} forms, ${leaves.size} distinct leaf labels`,
  });

  return findings;
}

function groupFindings(findings) {
  /** @type {Record<string, typeof findings>} */
  const by = {};
  for (const f of findings) {
    (by[f.kind] ??= []).push(f);
  }
  return by;
}

function isInfo(kind) {
  return kind.startsWith("info_");
}

async function maybeRunTriageHazards(file, project) {
  // Optional: import triage scan helpers — keep field audit standalone if import fails
  try {
    const triageUrl = pathToFileURL(
      path.join(REPO_ROOT, "scripts/triage-library-json.mjs"),
    ).href;
    // triage is a CLI main(); don't import side effects. Skip embedded run.
    void triageUrl;
    void file;
    void project;
  } catch {
    /* ignore */
  }
}

function main() {
  const { target } = parseArgs(process.argv.slice(2));
  const files = listJsonFiles(target);
  if (!files.length) {
    console.error(`No .json files under ${target}`);
    process.exit(1);
  }

  console.log(`# Field-reference audit`);
  console.log(`Root: ${target}`);
  console.log(`Files: ${files.length}`);
  console.log(`Repo: ${REPO_ROOT}`);
  console.log(`Rule: no alternate-label invention (report only)`);
  console.log("");

  /** @type {Record<string, Record<string, number>>} */
  const summary = {};

  for (const file of files) {
    const name = path.basename(file);
    let data;
    try {
      data = JSON.parse(fs.readFileSync(file, "utf8"));
    } catch (e) {
      console.log(`######## ${name}`);
      console.log(`  JSON PARSE FAIL: ${e.message}`);
      console.log("");
      continue;
    }
    const project = projectRoot(data);
    const findings = auditProject(project);
    void maybeRunTriageHazards;
    const by = groupFindings(findings);
    const hazardKeys = Object.keys(by).filter((k) => !isInfo(k)).sort();
    const infoKeys = Object.keys(by).filter((k) => isInfo(k)).sort();

    summary[name] = {};
    for (const k of hazardKeys) summary[name][k] = by[k].length;

    console.log(`######## ${name}`);
    if (!hazardKeys.length) {
      console.log("  No field-reference hazard findings.");
    } else {
      for (const kind of hazardKeys) {
        const items = by[kind];
        console.log(`  [${kind}] ${items.length}`);
        for (const it of items.slice(0, 8)) {
          console.log(`    ${it.path} ×${it.n}: ${it.sample}`);
        }
        if (items.length > 8) console.log(`    … +${items.length - 8} more`);
      }
    }
    if (infoKeys.length) {
      console.log("  — info —");
      for (const kind of infoKeys) {
        const items = by[kind];
        const show = items.slice(0, kind.includes("get") || kind.includes("foreach") ? 4 : 6);
        console.log(`  [${kind}] ${items.length}`);
        for (const it of show) {
          console.log(`    ${it.path}: ${it.sample}`);
        }
        if (items.length > show.length) {
          console.log(`    … +${items.length - show.length} more`);
        }
      }
    }
    console.log("");
  }

  console.log("## Summary (hazard counts)");
  for (const [name, counts] of Object.entries(summary)) {
    const parts = Object.entries(counts)
      .map(([k, n]) => `${k}=${n}`)
      .join(", ");
    console.log(`- ${name}: ${parts || "clean"}`);
  }
  console.log("");
  console.log("Hazard kinds: dup_blank_label_across_forms, bare_token_matches_form_blank,");
  console.log("  record_missing_form, record_scoped_missing_Record_prefix,");
  console.log("  get_where_form_field_no_Record, foreach_missing_recordName,");
  console.log("  foreach_body_unprefixed_form_field");
}

main();
