#!/usr/bin/env node
/**
 * Scan browser-Designer project JSON for known Deploy/import hazards.
 *
 * Captures the Jul 22 Library triage (ad-hoc Python) plus Jul 23–24 checks
 * (Heading Main/Sub glue risk, Page Header, rich Form nested font / MCQ).
 *
 * Usage (repo root):
 *   node scripts/triage-library-json.mjs
 *   node scripts/triage-library-json.mjs "/path/to/folder"
 *   node scripts/triage-library-json.mjs "/path/to/one.json"
 *   node scripts/triage-library-json.mjs --amend   # optional Amended/ for SUM Record: only
 *
 * Default folder (Jul 23+ layout):
 *   ~/Projects/Tawala Projects/Library Projects
 * Older Jul 22 path under Triage group/Best/ is also accepted if present.
 *
 * Does NOT rewrite bare Form:Field / process variables — those are usually intentional.
 * Report goes to stdout. Exit 0 always (informational); use findings for smoke triage.
 *
 * Spec notes: Tawala_Key_Documents/LIBRARY_PROJECTS_TRIAGE_JUL22.md
 */

import fs from "node:fs";
import path from "node:path";
import os from "node:os";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "..");

const CANDIDATE_DIRS = [
  path.join(os.homedir(), "Projects/Tawala Projects/Library Projects"),
  path.join(
    os.homedir(),
    "Projects/Tawala Projects/Triage group/Best/Library Projects",
  ),
];

function resolveDefaultDir() {
  for (const d of CANDIDATE_DIRS) {
    if (fs.existsSync(d)) return d;
  }
  return CANDIDATE_DIRS[0];
}

const FN_IN_TABLE =
  /data-function-id="(sum|itemization-table|record-count|choice-tally-table|response-totals-table|simple-list)"/i;

function usage() {
  console.error(`Usage: node scripts/triage-library-json.mjs [options] [path]

  path     Folder of *.json, or a single .json file
           (default: ~/Projects/Tawala Projects/Library Projects)
  --amend  Write Amended/<name>.amended.json when SUM fields can get Record: prefix
  -h       Help
`);
  process.exit(2);
}

function parseArgs(argv) {
  let amend = false;
  let target = null;
  for (const a of argv) {
    if (a === "-h" || a === "--help") usage();
    if (a === "--amend") {
      amend = true;
      continue;
    }
    if (a.startsWith("-")) {
      console.error(`Unknown option: ${a}`);
      usage();
    }
    target = a;
  }
  return {
    amend,
    target: target ? path.resolve(process.cwd(), target) : resolveDefaultDir(),
  };
}

function walkStrings(o, pathStr = "", acc = []) {
  if (o == null) return acc;
  if (typeof o === "string") {
    if (
      o.includes("<") ||
      /font/i.test(o) ||
      o.includes("function-token") ||
      o.includes("<<") ||
      o.includes("data-function") ||
      o.includes("heading-size")
    ) {
      acc.push([pathStr, o]);
    }
    return acc;
  }
  if (Array.isArray(o)) {
    o.forEach((v, i) => walkStrings(v, `${pathStr}[${i}]`, acc));
    return acc;
  }
  if (typeof o === "object") {
    for (const [k, v] of Object.entries(o)) {
      walkStrings(v, pathStr ? `${pathStr}.${k}` : k, acc);
    }
  }
  return acc;
}

function parseFnConfig(attrVal) {
  let conf = String(attrVal ?? "")
    .replace(/&quot;/g, '"')
    .replace(/&lt;/g, "<")
    .replace(/&gt;/g, ">")
    .replace(/&amp;/g, "&");
  try {
    return JSON.parse(conf);
  } catch {
    try {
      return JSON.parse(
        conf
          .replace(/&quot;/g, '"')
          .replace(/&amp;/g, "&"),
      );
    } catch {
      return null;
    }
  }
}

function stripAngleField(field) {
  let f = String(field ?? "").trim();
  if (f.startsWith("<<") && f.endsWith(">>")) f = f.slice(2, -2).trim();
  return f;
}

/** @returns {{ kind: string, path: string, n: number, sample: string }[]} */
function scanString(pathStr, s) {
  const findings = [];
  const nested = s.match(/<font\b[^>]*>\s*<font\b/gi);
  if (nested?.length) {
    findings.push({
      kind: "nested_font_html",
      path: pathStr,
      n: nested.length,
      sample: "nested <font><font>",
    });
  }

  if (s.includes("function-token") && /<t[dh]\b/i.test(s)) {
    const ids = [...s.matchAll(FN_IN_TABLE)].map((m) => m[1].toLowerCase());
    if (ids.length) {
      findings.push({
        kind: "fn_token_in_html_table",
        path: pathStr,
        n: ids.length,
        sample: [...new Set(ids)].join(","),
      });
    }
  }

  for (const m of s.matchAll(
    /data-function-id="sum"[^>]*data-function-config="([^"]*)"/gi,
  )) {
    const cfg = parseFnConfig(m[1]);
    if (!cfg) {
      findings.push({
        kind: "sum_config_unparseable",
        path: pathStr,
        n: 1,
        sample: m[1].slice(0, 60),
      });
      continue;
    }
    const field = stripAngleField(cfg.field);
    if (field && !field.startsWith("Record:") && field.includes(":")) {
      findings.push({
        kind: "sum_field_no_Record_prefix",
        path: pathStr,
        n: 1,
        sample: field,
      });
    }
  }

  if (/Your\s*>:|<>\s*:/.test(s)) {
    findings.push({
      kind: "orphan_field_punctuation",
      path: pathStr,
      n: 1,
      sample: "Your >: or <>:",
    });
  }

  if (/<font[^>]*color="rgb\(/i.test(s)) {
    findings.push({
      kind: "font_color_rgb",
      path: pathStr,
      n: 1,
      sample: 'font color="rgb(...)"',
    });
  }

  for (const m of s.matchAll(/(?:field="|name="|>)Record:([^"<]+)/g)) {
    const parts = m[1].split(":");
    if (parts.length === 2 && /^(FIB|Q)\d+$/i.test(parts[0])) {
      findings.push({
        kind: "record_missing_form",
        path: pathStr,
        n: 1,
        sample: `Record:${m[1]}`,
      });
    }
  }

  return findings;
}

/**
 * Jul 24+: mixed Main/Sub size spans with no <br> / block break between them
 * → Deploy used to glue (`hotelWe`). Exporter now splits on size change, but
 * missing breaks still worth smoke.
 */
function scanHeadingItem(formIdx, itemIdx, item) {
  const findings = [];
  const base = `forms[${formIdx}].items[${itemIdx}]`;
  if (!item || (item.type !== "heading" && item.type !== "subheading")) return findings;

  const html = String(item.content ?? "");
  const hasMain = /heading-size-main/i.test(html);
  const hasSub = /heading-size-sub/i.test(html);
  if (hasMain && hasSub) {
    // Adjacent size spans without an intervening <br> / </div> / </p>
    const adjacent = /heading-size-(?:main|sub)[^>]*>[\s\S]*?<\/span>\s*<span[^>]*heading-size-(?:main|sub)/i.test(
      html,
    );
    const hasBreak = /<br\s*\/?>|<\/(?:div|p)>/i.test(html);
    if (adjacent && !hasBreak) {
      findings.push({
        kind: "heading_mixed_sizes_no_break",
        path: `${base}.content`,
        n: 1,
        sample: "Main+Sub spans with no <br> between (glue risk / smoke Redeploy)",
      });
    }
    if (item.level === "main" || item.level === "sub") {
      findings.push({
        kind: "heading_sticky_level_with_size_spans",
        path: `${base}.level`,
        n: 1,
        sample: `level=${item.level} + mixed size spans (exporter should ignore level)`,
      });
    }
  }

  // Informative: heading present (for Library smoke lists)
  if (html.trim() || item.label) {
    findings.push({
      kind: "info_heading_present",
      path: base,
      n: 1,
      sample: `label=${item.label ?? ""} level=${item.level ?? ""}`,
    });
  }
  return findings;
}

function scanPageHeader(project) {
  const findings = [];
  const ph = project?.pageHeader;
  if (!ph || typeof ph !== "object") return findings;

  const text = String(ph.text ?? "").trim();
  const imageId = String(ph.imageId ?? ph.image?.id ?? "").trim();
  const w = Number(ph.width ?? ph.image?.width ?? 0);
  const h = Number(ph.height ?? ph.image?.height ?? 0);

  findings.push({
    kind: "info_page_header_present",
    path: "pageHeader",
    n: 1,
    sample: `text=${text ? "yes" : "empty"} image=${imageId || "none"} ${w}x${h}`,
  });

  if (!text && !imageId) {
    findings.push({
      kind: "page_header_empty",
      path: "pageHeader",
      n: 1,
      sample: "pageHeader object with no text and no image id",
    });
  }
  if (h > 160) {
    findings.push({
      kind: "page_header_tall_image",
      path: "pageHeader",
      n: 1,
      sample: `height=${h} (Deploy CSS caps ~160px — smoke visual crop)`,
    });
  }
  return findings;
}

function scanFormRichFields(project) {
  const findings = [];
  const forms = project?.forms ?? [];
  forms.forEach((form, fi) => {
    (form.items ?? []).forEach((item, ii) => {
      if (!item || typeof item !== "object") return;
      findings.push(...scanHeadingItem(fi, ii, item));

      const keys =
        item.type === "mc" || item.type === "multipleChoice"
          ? ["question", "prompt", "content", "html", "text"]
          : item.type === "fib" || item.type === "fillInBlank"
            ? ["prompt", "content", "html", "text"]
            : item.type === "text"
              ? ["content", "html", "text"]
              : [];

      for (const key of keys) {
        const val = item[key];
        if (typeof val !== "string" || !val) continue;
        const p = `forms[${fi}].items[${ii}].${key}`;
        if (/<font\b[^>]*>\s*<font\b/i.test(val)) {
          findings.push({
            kind: "form_rich_nested_font",
            path: p,
            n: (val.match(/<font\b[^>]*>\s*<font\b/gi) || []).length,
            sample: `${item.type} nested font (Deploy drop risk)`,
          });
        }
        if (
          (item.type === "mc" || item.type === "multipleChoice") &&
          /<(?:b|i|u|strong|em|span|font)\b/i.test(val)
        ) {
          findings.push({
            kind: "info_mcq_rich_question",
            path: p,
            n: 1,
            sample: "MCQ question has rich markup — smoke Deploy",
          });
        }
        if (
          (item.type === "fib" || item.type === "fillInBlank") &&
          /<(?:b|i|u|strong|em|span|font)\b/i.test(val)
        ) {
          findings.push({
            kind: "info_fib_rich_prompt",
            path: p,
            n: 1,
            sample: "FIB prompt has rich markup — smoke Deploy",
          });
        }
      }
    });
  });
  return findings;
}

function walkStructuredNodes(nodes, pathStr, findings) {
  if (!Array.isArray(nodes)) return;
  nodes.forEach((n, i) => {
    if (!n || typeof n !== "object") return;
    const p = `${pathStr}[${i}]`;
    const t = n.type;
    if (t === "font") {
      const kids = n.nodes ?? [];
      if (
        kids.length === 1 &&
        kids[0] &&
        typeof kids[0] === "object" &&
        kids[0].type === "font"
      ) {
        findings.push({
          kind: "nested_font_node",
          path: p,
          n: 1,
          sample: "structured font→font",
        });
      }
    }
    if (t === "sum") {
      const f = stripAngleField(n.field);
      if (f && !f.startsWith("Record:") && f.includes(":")) {
        findings.push({
          kind: "sum_field_no_Record_prefix",
          path: `${p}.field`,
          n: 1,
          sample: f,
        });
      }
    }
    if (t === "field") {
      const name = String(n.name ?? "");
      if (name.startsWith("Record:")) {
        const parts = name.split(":");
        if (parts.length === 3 && /^(FIB|Q)\d+$/i.test(parts[1])) {
          findings.push({
            kind: "info_record_fib_style",
            path: p,
            n: 1,
            sample: name,
          });
        }
      } else if (name.includes(":")) {
        findings.push({
          kind: "info_field_form_field_no_Record",
          path: p,
          n: 1,
          sample: name,
        });
      } else if (name) {
        findings.push({
          kind: "info_field_bare_name",
          path: p,
          n: 1,
          sample: name,
        });
      }
    }
    if (["itemizationTable", "sum", "recordCount", "choiceTallyTable", "simpleList"].includes(t)) {
      findings.push({
        kind: `info_structured_${t}`,
        path: p,
        n: 1,
        sample: String(n.form ?? n.field ?? ""),
      });
    }
    if (Array.isArray(n.nodes)) walkStructuredNodes(n.nodes, `${p}.nodes`, findings);
  });
}

function scanStructuredDocuments(project) {
  const findings = [];
  (project.documents ?? []).forEach((doc, di) => {
    const content = doc?.content;
    if (Array.isArray(content)) {
      content.forEach((block, bi) => {
        if (block && typeof block === "object" && Array.isArray(block.nodes)) {
          walkStructuredNodes(
            block.nodes,
            `documents[${di}].content[${bi}].nodes`,
            findings,
          );
        }
      });
    }
  });
  return findings;
}

function fixSumRecordPrefixInString(s) {
  const changes = [];
  const next = s.replace(
    /(data-function-id="sum"[^>]*data-function-config=")([^"]*)(")/gi,
    (full, before, confRaw, after) => {
      const cfg = parseFnConfig(confRaw);
      if (!cfg) return full;
      const field = stripAngleField(cfg.field);
      if (!field || field.startsWith("Record:") || !field.includes(":")) return full;
      cfg.field = `Record:${field}`;
      const newConf = JSON.stringify(cfg);
      const enc = newConf
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
      changes.push(`sum field → Record:${field}`);
      return `${before}${enc}${after}`;
    },
  );
  return { next, changes };
}

function mutateSumPrefixes(o, changes) {
  if (typeof o === "string") {
    const { next, changes: ch } = fixSumRecordPrefixInString(o);
    changes.push(...ch);
    return next;
  }
  if (Array.isArray(o)) {
    return o.map((v) => mutateSumPrefixes(v, changes));
  }
  if (o && typeof o === "object") {
    if (o.type === "sum" && typeof o.field === "string") {
      const f = stripAngleField(o.field);
      if (f && !f.startsWith("Record:") && f.includes(":")) {
        changes.push(`structured sum → Record:${f}`);
        return { ...o, field: `Record:${f}` };
      }
    }
    const out = {};
    for (const [k, v] of Object.entries(o)) {
      out[k] = mutateSumPrefixes(v, changes);
    }
    return out;
  }
  return o;
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
    .filter((n) => n.toLowerCase().endsWith(".json") && !n.includes(".amended."))
    .map((n) => path.join(target, n))
    .sort();
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

function main() {
  const { amend, target } = parseArgs(process.argv.slice(2));
  const files = listJsonFiles(target);
  if (!files.length) {
    console.error(`No .json files under ${target}`);
    process.exit(1);
  }

  console.log(`# Library / project JSON triage`);
  console.log(`Root: ${target}`);
  console.log(`Files: ${files.length}`);
  console.log(`Amend: ${amend ? "yes (SUM Record: only)" : "no (report only)"}`);
  console.log(`Repo: ${REPO_ROOT}`);
  console.log("");

  const amendDir = path.join(
    fs.statSync(target).isDirectory() ? target : path.dirname(target),
    "Amended",
  );
  const amended = [];

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
    const findings = [];

    for (const [p, s] of walkStrings(data)) {
      findings.push(...scanString(p, s));
    }
    findings.push(...scanFormRichFields(project));
    findings.push(...scanPageHeader(project));
    findings.push(...scanStructuredDocuments(project));

    const by = groupFindings(findings);
    const hazardKeys = Object.keys(by).filter((k) => !isInfo(k)).sort();
    const infoKeys = Object.keys(by).filter((k) => isInfo(k)).sort();

    console.log(`######## ${name}`);
    if (!hazardKeys.length) {
      console.log("  No hazard-class findings.");
    } else {
      for (const kind of hazardKeys) {
        const items = by[kind];
        console.log(`  [${kind}] ${items.length}`);
        for (const it of items.slice(0, 6)) {
          console.log(`    ${it.path} ×${it.n}: ${it.sample}`);
        }
      }
    }
    if (infoKeys.length) {
      console.log("  — info (usually OK; smoke if Deploy-sensitive) —");
      for (const kind of infoKeys) {
        const items = by[kind];
        // Cap noisy field-name dumps
        const show =
          kind === "info_field_bare_name" || kind === "info_field_form_field_no_Record"
            ? items.slice(0, 3)
            : items.slice(0, 8);
        console.log(`  [${kind}] ${items.length}`);
        for (const it of show) {
          console.log(`    ${it.path}: ${it.sample}`);
        }
        if (items.length > show.length) {
          console.log(`    … +${items.length - show.length} more`);
        }
      }
    }

    if (amend) {
      const changes = [];
      const fixed = mutateSumPrefixes(structuredClone(data), changes);
      if (changes.length) {
        fs.mkdirSync(amendDir, { recursive: true });
        const out = path.join(amendDir, `${path.basename(file, ".json")}.amended.json`);
        fs.writeFileSync(out, `${JSON.stringify(fixed, null, 2)}\n`, "utf8");
        amended.push({ out, n: changes.length });
        console.log(`  Amended → ${out} (${changes.length} SUM Record: fix(es))`);
      } else {
        console.log("  No auto-amendments.");
      }
    }
    console.log("");
  }

  console.log("## Hazard kinds (Jul 22 + Jul 24)");
  console.log("- nested_font_html / nested_font_node / form_rich_nested_font");
  console.log("- fn_token_in_html_table, sum_field_no_Record_prefix, sum_config_unparseable");
  console.log("- orphan_field_punctuation, font_color_rgb, record_missing_form");
  console.log("- heading_mixed_sizes_no_break, heading_sticky_level_with_size_spans");
  console.log("- page_header_empty, page_header_tall_image");
  console.log("");
  console.log("Do not auto-rewrite bare Form:Field / process vars (info_* only).");
  if (amended.length) {
    console.log("Amended files:", amended.map((a) => a.out).join(", "));
  }
}

main();
