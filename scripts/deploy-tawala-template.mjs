#!/usr/bin/env node
/**
 * Deploy a legacy Designer .tawala template to Tomcat via POST /client.
 *
 * Usage:
 *   node scripts/deploy-tawala-template.mjs --list
 *   node scripts/deploy-tawala-template.mjs "Simple Survey Template"
 *   node scripts/deploy-tawala-template.mjs path/to/project.tawala
 */
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";

const root = path.dirname(path.dirname(fileURLToPath(import.meta.url)));
const templatesDir = path.join(
  root,
  "TawalaDesigner/Code/TAWALA/Setup/Templates"
);
const javaUrl = (process.env.TAWALA_JAVA_URL || "http://localhost:8080").replace(/\/$/, "");
const user = process.env.TAWALA_DEV_USER || "dev";
const password = process.env.TAWALA_DEV_PASSWORD || "dev";

function escAttr(s) {
  return String(s ?? "")
    .replace(/&/g, "&amp;")
    .replace(/"/g, "&quot;")
    .replace(/</g, "&lt;");
}

function buildUploadRequest(credentials, projectXml) {
  const creds = `<credentials user="${escAttr(credentials.user)}" password="${escAttr(credentials.password)}"/>`;
  return `<?xml version="1.0" encoding="utf-8" ?>\n<request type="uploadProject" protocol="1.0">\n${creds}\n${projectXml.trim()}\n</request>\n`;
}

function readProjectXml(filePath) {
  let text = fs.readFileSync(filePath, "utf8");
  text = text.replace(/^\uFEFF?<\?xml[^?]*\?>\s*/i, "").trim();
  if (!text.startsWith("<project")) {
    throw new Error(`Expected <project> root in ${filePath}`);
  }
  return text;
}

function listTemplates() {
  const xmlPath = path.join(templatesDir, "templates.xml");
  if (!fs.existsSync(xmlPath)) {
    console.error("templates.xml not found");
    process.exit(1);
  }
  const xml = fs.readFileSync(xmlPath, "utf8");
  const re = /<project\s+label="([^"]+)"\s+file="([^"]+)"/g;
  console.log("New Project templates (deploy by label or file stem):\n");
  let m;
  while ((m = re.exec(xml)) !== null) {
    const file = path.join(templatesDir, `${m[2]}.tawala`);
    const ok = fs.existsSync(file) ? "" : " [MISSING FILE]";
    console.log(`  ${m[1]}${ok}`);
    console.log(`    → ${m[2]}.tawala`);
  }
}

function resolveTemplatePath(arg) {
  if (arg.endsWith(".tawala") && fs.existsSync(arg)) {
    return path.resolve(arg);
  }
  const direct = path.join(templatesDir, `${arg}.tawala`);
  if (fs.existsSync(direct)) return direct;

  const xmlPath = path.join(templatesDir, "templates.xml");
  const xml = fs.readFileSync(xmlPath, "utf8");
  const labelRe = new RegExp(
    `<project\\s+label="${arg.replace(/[.*+?^${}()|[\]\\]/g, "\\$&")}"\\s+file="([^"]+)"`
  );
  const labelMatch = xml.match(labelRe);
  if (labelMatch) {
    return path.join(templatesDir, `${labelMatch[1]}.tawala`);
  }
  throw new Error(`Template not found: ${arg}`);
}

function parseStartUrls(responseXml, projectNameFilter) {
  const urls = [];
  const deploymentRe = /<deployment\s+project="([^"]+)"[^>]*>([\s\S]*?)<\/deployment>/g;
  let dm;
  while ((dm = deploymentRe.exec(responseXml)) !== null) {
    if (projectNameFilter && dm[1] !== projectNameFilter) continue;
    const block = dm[2];
    const re = /<startpoint\s+form="([^"]+)"\s+url="([^"]+)"/g;
    let m;
    while ((m = re.exec(block)) !== null) {
      urls.push({ project: dm[1], form: m[1], url: m[2] });
    }
  }
  return urls;
}

async function main() {
  const arg = process.argv[2];
  if (!arg || arg === "--help" || arg === "-h") {
    console.log(`Usage:
  node scripts/deploy-tawala-template.mjs --list
  node scripts/deploy-tawala-template.mjs "Simple Survey Template"

Env: TAWALA_JAVA_URL (default http://localhost:8080), TAWALA_DEV_USER, TAWALA_DEV_PASSWORD`);
    process.exit(arg ? 0 : 1);
  }
  if (arg === "--list") {
    listTemplates();
    return;
  }

  const filePath = resolveTemplatePath(arg);
  const projectXml = readProjectXml(filePath);
  const nameMatch = projectXml.match(/<project\s+name="([^"]+)"/);
  const projectName = nameMatch?.[1];
  const body = buildUploadRequest({ user, password }, projectXml);

  console.log(`Deploying: ${path.basename(filePath)}`);
  console.log(`  → ${javaUrl}/client (${user})`);

  const res = await fetch(`${javaUrl}/client`, {
    method: "POST",
    headers: { "Content-Type": "text/xml; charset=utf-8" },
    body,
  });
  const text = await res.text();

  if (!res.ok) {
    console.error(`HTTP ${res.status}`);
    console.error(text.slice(0, 800));
    process.exit(1);
  }
  if (text.includes('status="failure"')) {
    console.error(text);
    process.exit(1);
  }

  const starts = parseStartUrls(text, projectName);
  console.log(`\nDeploy OK — project "${projectName}". Start points:\n`);
  for (const s of starts) {
    console.log(`  ${s.form}`);
    console.log(`    ${s.url}\n`);
  }
  if (!starts.length) {
    console.log("(no startpoint URLs in response — check XML)");
    console.log(text.slice(0, 500));
  }
}

main().catch((e) => {
  console.error(e.message || e);
  process.exit(1);
});
