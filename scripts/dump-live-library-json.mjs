#!/usr/bin/env node
/**
 * Dump live Tomcat project_definition XML (by uniqueId) to Designer JSON.
 *
 * Usage (repo root):
 *   node scripts/dump-live-library-json.mjs --out "/path/to/dir" uid:File.json ...
 *
 * Requires Docker postgres (tawala-postgres). Conversion is the same path as
 * File → Open (.tawala XML): designer-web/src/lib/tawalaXmlToJson.mjs.
 */
import { execFileSync } from "node:child_process";
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath, pathToFileURL } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const REPO_ROOT = path.resolve(__dirname, "..");
const converterUrl = pathToFileURL(
  path.join(REPO_ROOT, "designer-web/src/lib/tawalaXmlToJson.mjs"),
).href;

const { convertTawalaXmlToProject } = await import(converterUrl);

function usage() {
  console.error(
    'Usage: node scripts/dump-live-library-json.mjs --out DIR uniqueId:File.json [...]',
  );
  process.exit(2);
}

function sqlLiteral(uid) {
  if (!/^[A-Za-z0-9]{1,20}$/.test(uid)) {
    throw new Error(`Invalid uniqueId: ${uid}`);
  }
  return uid;
}

function fetchLiveRow(uid) {
  const id = sqlLiteral(uid);
  const sql = `
SELECT json_build_object(
  'uniqueId', up.unique_random_id,
  'tomcatName', up.name,
  'lastUpdated', up.last_updated_dt,
  'version', up.version,
  'xml', p.project_definition
)::text
FROM user_project up
JOIN project p ON p.project_id = up.project_id
WHERE up.unique_random_id = '${id}';
`.trim();
  const raw = execFileSync(
    "docker",
    ["exec", "-i", "tawala-postgres", "psql", "-U", "tawala_admin", "-d", "tawala", "-Atc", sql],
    { encoding: "utf8", maxBuffer: 80 * 1024 * 1024 },
  ).trim();
  if (!raw) throw new Error(`No user_project for uniqueId ${uid}`);
  return JSON.parse(raw);
}

const args = process.argv.slice(2);
const outIdx = args.indexOf("--out");
if (outIdx < 0 || !args[outIdx + 1]) usage();
const outDir = path.resolve(args[outIdx + 1]);
const pairs = args.filter((_, i) => i !== outIdx && i !== outIdx + 1);
if (!pairs.length) usage();

fs.mkdirSync(outDir, { recursive: true });

const dumpedAt = new Date().toISOString();
const summary = [];

for (const pair of pairs) {
  const colon = pair.indexOf(":");
  if (colon < 1) {
    console.error(`Bad pair (want uniqueId:File.json): ${pair}`);
    process.exit(1);
  }
  const uid = pair.slice(0, colon);
  const fileName = pair.slice(colon + 1);
  const row = fetchLiveRow(uid);
  const { project, warnings } = convertTawalaXmlToProject(row.xml, {
    sourceLabel: `tomcat:${uid}`,
  });
  const displayName = path.basename(fileName, ".json");
  const stamped = {
    ...project,
    name: displayName,
    deployIdentityName: row.tomcatName,
    deployUniqueId: row.uniqueId,
    _dumpedFrom: "tomcat-postgres",
    _dumpedAt: dumpedAt,
    _tomcatLastUpdated: row.lastUpdated,
    _tomcatVersion: row.version,
  };
  const outPath = path.join(outDir, fileName);
  fs.writeFileSync(outPath, JSON.stringify(stamped, null, 2) + "\n");
  const warnN = (warnings || []).length;
  summary.push({
    file: fileName,
    uniqueId: uid,
    tomcatName: row.tomcatName,
    bytes: fs.statSync(outPath).size,
    forms: (stamped.forms || []).length,
    warnings: warnN,
  });
  console.log(
    `Wrote ${outPath}  uid=${uid}  forms=${(stamped.forms || []).length}  warn=${warnN}`,
  );
  for (const w of warnings || []) {
    console.error(`  [warn ${fileName}] ${w}`);
  }
}

console.log(JSON.stringify({ dumpedAt, outDir, count: summary.length, summary }, null, 2));
