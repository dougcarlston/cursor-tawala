#!/usr/bin/env node
/**
 * Batch field-ref audit → Amended copy → Deploy smoke for Library Sort /
 * Being Reconverted trees. Skips 00* leaves, report JSON, and prior Amended.
 *
 * Usage:
 *   node scripts/batch-field-audit-trees.mjs
 *
 * Only safe mechanical fix: itemization-table column <<Form:Field>> → <<Record:Form:Field>>
 * when Form is a known form name. No alternate-label invention.
 * Deploys only when audit is clean after fixes.
 */

import fs from "node:fs";
import path from "node:path";
import os from "node:os";
import { spawnSync } from "node:child_process";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO = path.resolve(__dirname, "..");
const HOME = os.homedir();
const TP = path.join(HOME, "Projects/Tawala Projects");

const TREES = [
  {
    root: path.join(TP, "JSON Library Sort"),
    amended: path.join(TP, "JSON Library Sort/Amended"),
    skipName: (n) =>
      n === "DEPLOY-SMOKE-REPORT.json" || n.startsWith("_"),
  },
  {
    root: path.join(TP, "Being Reconverted"),
    amended: path.join(TP, "Being Reconverted/Amended"),
    skipName: (n) => n.startsWith("_") || n === "DEPLOY-SMOKE-REPORT.json",
  },
];

const AUDIT = path.join(REPO, "scripts/audit-field-refs.mjs");

function shouldSkipPath(filePath) {
  const parts = filePath.split(path.sep);
  for (const part of parts) {
    if (part === "00" || part.startsWith("00-") || part.startsWith("00_")) {
      return true;
    }
    if (part === "Amended") return true;
  }
  if (filePath.includes("-field-audit")) return true;
  return false;
}

function collectJson(tree) {
  const out = [];
  const walk = (dir) => {
    if (!fs.existsSync(dir)) return;
    for (const ent of fs.readdirSync(dir, { withFileTypes: true })) {
      const full = path.join(dir, ent.name);
      if (ent.isDirectory()) {
        if (ent.name === "Amended" || ent.name.startsWith("00")) continue;
        walk(full);
        continue;
      }
      if (!ent.name.toLowerCase().endsWith(".json")) continue;
      if (tree.skipName?.(ent.name)) continue;
      if (shouldSkipPath(full)) continue;
      out.push(full);
    }
  };
  walk(tree.root);
  return out.sort();
}

function auditFile(file) {
  const r = spawnSync(process.execPath, [AUDIT, file], {
    encoding: "utf8",
    cwd: REPO,
  });
  const text = (r.stdout || "") + (r.stderr || "");
  const hazards = [];
  for (const m of text.matchAll(/^\s+\[(?!info_)([^\]]+)\] (\d+)/gm)) {
    hazards.push(`${m[1]}=${m[2]}`);
  }
  return { text, clean: hazards.length === 0, hazards };
}

function projectRoot(data) {
  return data?.project && typeof data.project === "object" ? data.project : data;
}

function formNames(project) {
  return new Set((project.forms || []).map((f) => String(f.name || "")).filter(Boolean));
}

/** Safe MQL column qualify: <<Form:Field>> → <<Record:Form:Field>> inside itemization configs. */
function fixItemizationRecordPrefix(project) {
  const forms = formNames(project);
  const fixes = [];
  const re =
    /data-function-id="itemization-table"([^>]*)data-function-config="([^"]*)"/gi;

  const fixHtml = (html, pathStr) => {
    if (typeof html !== "string" || !html.includes("itemization-table")) {
      return html;
    }
    return html.replace(re, (full, mid, confRaw) => {
      let conf = confRaw
        .replace(/&quot;/g, '"')
        .replace(/&lt;/g, "<")
        .replace(/&gt;/g, ">")
        .replace(/&amp;/g, "&");
      let cfg;
      try {
        cfg = JSON.parse(conf);
      } catch {
        return full;
      }
      let changed = false;
      for (const col of cfg.column || cfg.columns || []) {
        const c = String(col.contents || col.field || "");
        const m = c.match(/^<<([^>]+)>>$/);
        if (!m) continue;
        const name = m[1];
        if (name.startsWith("Record:")) continue;
        const parts = name.split(":");
        if (parts.length === 2 && forms.has(parts[0])) {
          const next = `<<Record:${name}>>`;
          if (col.contents) col.contents = next;
          if (col.field && col.field === c) col.field = next;
          fixes.push(`${pathStr}: ${c} → ${next}`);
          changed = true;
        }
      }
      if (!changed) return full;
      const enc = JSON.stringify(cfg)
        .replace(/&/g, "&amp;")
        .replace(/"/g, "&quot;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;");
      return `data-function-id="itemization-table"${mid}data-function-config="${enc}"`;
    });
  };

  const walk = (o, pathStr) => {
    if (o == null) return;
    if (typeof o === "string") return;
    if (Array.isArray(o)) {
      o.forEach((v, i) => {
        if (typeof v === "string" && v.includes("itemization")) {
          o[i] = fixHtml(v, `${pathStr}[${i}]`);
        } else walk(v, `${pathStr}[${i}]`);
      });
      return;
    }
    if (typeof o === "object") {
      for (const [k, v] of Object.entries(o)) {
        if (typeof v === "string" && (k === "content" || k === "html" || k === "prompt" || k === "text")) {
          o[k] = fixHtml(v, `${pathStr}.${k}`);
        } else walk(v, `${pathStr}.${k}`);
      }
    }
  };

  walk(project, "");
  return fixes;
}

function dstName(srcPath, treeRoot) {
  const rel = path.relative(treeRoot, srcPath).replace(/[\\/]/g, "__");
  const base = rel.replace(/\.json$/i, "");
  return `${base}-field-audit.json`;
}

async function deploySmoke(project) {
  const res = await fetch("http://localhost:3001/api/deploy", {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({
      credentials: { user: "dev", password: "dev" },
      project,
    }),
  });
  let body;
  try {
    body = await res.json();
  } catch {
    body = { status: "failure", error: await res.text() };
  }
  const smokes = [];
  if (body.status === "success" && Array.isArray(body.startpoints)) {
    for (const sp of body.startpoints.slice(0, 8)) {
      try {
        const r = await fetch(sp.url);
        const html = await r.text();
        const bad =
          html.includes("No class registered") ||
          /Exception|Error id=/i.test(html.slice(0, 2000));
        smokes.push({
          form: sp.form,
          status: r.status,
          ok: r.status === 200 && !bad,
          url: sp.url,
          bytes: html.length,
        });
      } catch (e) {
        smokes.push({ form: sp.form, status: 0, ok: false, url: sp.url, error: String(e) });
      }
    }
  }
  return { http: res.status, body, smokes };
}

async function processFile(src, tree) {
  const result = {
    src,
    rel: path.relative(tree.root, src),
    dst: null,
    before: null,
    after: null,
    fixes: [],
    deployed: false,
    uniqueId: null,
    smokes: [],
    error: null,
  };

  console.log("\n==========", result.rel, "==========");
  try {
    result.before = auditFile(src);
    console.log(
      "audit before:",
      result.before.clean ? "CLEAN" : result.before.hazards.join(", "),
    );

    fs.mkdirSync(tree.amended, { recursive: true });
    const dst = path.join(tree.amended, dstName(src, tree.root));
    result.dst = dst;
    fs.copyFileSync(src, dst);

    const data = JSON.parse(fs.readFileSync(dst, "utf8"));
    const root = projectRoot(data);
    if (!root?.forms && !root?.name) {
      result.error = "not a project JSON (no forms/name)";
      console.log("SKIP:", result.error);
      fs.unlinkSync(dst);
      result.dst = null;
      return result;
    }

    const baseName = root.name || path.basename(src, ".json");
    root.name = `${baseName} (field-audit copy)`;
    result.fixes = fixItemizationRecordPrefix(root);
    if (result.fixes.length) {
      console.log("fixes:", result.fixes.length, result.fixes.slice(0, 8).join("; "));
    }
    if (data.project) data.project = root;
    fs.writeFileSync(dst, JSON.stringify(data, null, 2) + "\n");

    result.after = auditFile(dst);
    console.log(
      "audit after:",
      result.after.clean ? "CLEAN" : result.after.hazards.join(", "),
    );

    if (!result.after.clean) {
      console.log("NO DEPLOY — audit not clean");
      return result;
    }

    const dep = await deploySmoke(projectRoot(data));
    result.deployed = dep.body.status === "success";
    result.uniqueId = dep.body.uniqueId || null;
    result.smokes = dep.smokes;
    result.deployError = dep.body.error || null;
    console.log(
      "deploy:",
      dep.body.status,
      result.uniqueId || result.deployError || "",
    );
    for (const s of dep.smokes) {
      console.log("  smoke", s.form, s.status, s.ok ? "ok" : "BAD", s.url);
    }
  } catch (e) {
    result.error = String(e.stack || e);
    console.log("ERROR", result.error.slice(0, 400));
  }
  return result;
}

async function main() {
  const allResults = [];
  for (const tree of TREES) {
    console.log("\n##### TREE", tree.root);
    const files = collectJson(tree);
    console.log("files:", files.length);
    for (const f of files) {
      allResults.push(await processFile(f, tree));
    }
  }

  const summaryPath = path.join(TP, "FIELD-AUDIT-BATCH-JUL28.json");
  fs.writeFileSync(summaryPath, JSON.stringify(allResults, null, 2) + "\n");

  console.log("\n## BATCH SUMMARY");
  let ok = 0;
  let skip = 0;
  let fail = 0;
  for (const r of allResults) {
    const label = r.rel || r.src;
    if (r.error && !r.dst) {
      skip++;
      console.log("SKIP", label, r.error);
    } else if (r.deployed && r.smokes.every((s) => s.ok)) {
      ok++;
      console.log("OK  ", label, r.uniqueId, `fixes=${r.fixes.length}`);
    } else if (r.after && !r.after.clean) {
      fail++;
      console.log("HOLD", label, r.after.hazards.join(", "));
    } else {
      fail++;
      console.log(
        "FAIL",
        label,
        r.deployError || r.error || "deploy/smoke issue",
      );
    }
  }
  console.log({ ok, holdOrFail: fail, skip, total: allResults.length });
  console.log("Wrote", summaryPath);
}

main().catch((e) => {
  console.error(e);
  process.exit(1);
});
