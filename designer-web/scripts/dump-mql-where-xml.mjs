#!/usr/bin/env node
/**
 * Proof dump: Sign-up Sheet T2 with Form 1:lastName equals Carlston → Deploy XML.
 *
 *   node designer-web/scripts/dump-mql-where-xml.mjs
 *
 * Prints the <itemization-table>…</itemization-table> snippet. Exit 1 if Where missing.
 */
import { readFileSync } from "node:fs";
import { dirname, join } from "node:path";
import { fileURLToPath } from "node:url";
import { projectToXml } from "../server/jsonToXml.mjs";

const here = dirname(fileURLToPath(import.meta.url));
const templatePath = join(here, "../public/samples/templates/signup-sheet.json");
const project = JSON.parse(readFileSync(templatePath, "utf8"));
const t2 = project.forms[0].items.find((i) => i.label === "T2");
if (!t2 || typeof t2.content !== "string") {
  console.error("T2 missing or not HTML string");
  process.exit(1);
}

const modern =
  t2.content.includes("data-function-id") && t2.content.includes("itemization-table");
const legacyBrace = t2.content.includes("{ MULTIPLE QUESTION LIST }");
const structuredArray = Array.isArray(t2.content);
console.log("T2 token form: modern function-token =", modern, "; legacy brace =", legacyBrace, "; structured array =", structuredArray);
if (!modern || legacyBrace || structuredArray) {
  console.error("FAIL: New Project Sign-up Sheet T2 must be modern HTML function-token only");
  process.exit(1);
}

// Stock template (no Where edit) must still Deploy itemization-table with zero brace dependency.
const stockXml = projectToXml({ ...project, name: "SignupStockProof" });
const stockTable = stockXml.match(/<itemization-table[\s\S]*?<\/itemization-table>/);
if (!stockTable) {
  console.error("FAIL: stock template Deploy has no itemization-table");
  process.exit(1);
}
if (stockXml.includes("{ MULTIPLE QUESTION LIST }") || /field name="[^"]*MULTIPLE QUESTION/.test(stockXml)) {
  console.error("FAIL: stock Deploy still depends on legacy brace / junk field path");
  process.exit(1);
}
console.log("Stock Deploy: itemization-table present; no legacy brace / junk MQL field");

const withWhere = t2.content.replace(
  /&quot;conditionsRows&quot;:\[\{&quot;field&quot;:&quot;&quot;,&quot;op&quot;:&quot;equals&quot;,&quot;value&quot;:&quot;&quot;\}]/,
  "&quot;conditionsRows&quot;:[{&quot;field&quot;:&quot;Form 1:lastName&quot;,&quot;op&quot;:&quot;equals&quot;,&quot;value&quot;:&quot;Carlston&quot;}]",
);

const xml = projectToXml({
  ...project,
  name: "MqlWhereDump",
  forms: [
    {
      ...project.forms[0],
      items: project.forms[0].items.map((i) =>
        i.label === "T2" ? { ...i, content: withWhere } : i,
      ),
    },
  ],
});

const snip = xml.match(/<itemization-table[\s\S]*?<\/itemization-table>/);
if (!snip) {
  console.error("No itemization-table in Deploy XML");
  process.exit(1);
}
console.log("\n=== itemization-table snippet ===\n");
console.log(snip[0]);
console.log("");

const ok =
  /<equals field="Record:Form 1:lastName"><string value="Carlston"\/><\/equals>/.test(
    snip[0],
  );
if (!ok) {
  console.error("FAIL: Where equals Carlston missing from XML");
  process.exit(1);
}
console.log("OK: Where equals Record:Form 1:lastName / Carlston present");
