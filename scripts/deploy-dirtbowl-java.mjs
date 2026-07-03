#!/usr/bin/env node
/** Deploy DirtBowl sample JSON to Tomcat via /client (dev/dev). */
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import { buildUploadRequest } from "../designer-web/server/jsonToXml.mjs";

const root = path.dirname(path.dirname(fileURLToPath(import.meta.url)));
const samplePath = path.join(root, "designer-web/public/samples/dirtbowl_definition_v3.json");
const javaUrl = (process.env.TAWALA_JAVA_URL || "http://localhost:8080").replace(/\/$/, "");
const user = process.env.TAWALA_DEV_USER || "dev";
const password = process.env.TAWALA_DEV_PASSWORD || "dev";

const raw = JSON.parse(fs.readFileSync(samplePath, "utf8"));
const project = raw.project ?? raw;
const xml = buildUploadRequest({ user, password }, project);

const res = await fetch(`${javaUrl}/client`, {
  method: "POST",
  headers: { "Content-Type": "text/xml; charset=utf-8" },
  body: xml,
});
const text = await res.text();
if (!res.ok) {
  console.error(`Deploy failed: HTTP ${res.status}`);
  console.error(text.slice(0, 500));
  process.exit(1);
}
if (text.includes('status="failure"')) {
  console.error(text);
  process.exit(1);
}
console.log(text);
const m = text.match(/url="([^"]+\/Registration)"/);
if (m) console.log(`\nRegistration URL:\n  ${m[1]}`);

if (process.env.TAWALA_SKIP_SEED !== "1") {
  const { execSync } = await import("child_process");
  const devData = path.join(root, "scripts/dev-data.sh");
  console.log("\nSeeding AdminSetup + Divisions for dev user...");
  try {
    execSync(`"${devData}" seed-admin`, { stdio: "inherit", cwd: root });
    execSync(`"${devData}" seed-divisions`, { stdio: "inherit", cwd: root });
  } catch (e) {
    console.warn("Seed step failed (is postgres running?). Run:");
    console.warn("  ./scripts/dev-data.sh seed-admin && ./scripts/dev-data.sh seed-divisions");
  }
}
