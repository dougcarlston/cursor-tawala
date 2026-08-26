import express from "express";
import fs from "fs";
import path from "path";
import { fileURLToPath } from "url";
import { XMLParser } from "fast-xml-parser";
import * as store from "./store.mjs";
import * as runtime from "./runtime.mjs";
import { buildUploadRequest } from "./jsonToXml.mjs";
import {
  filterStartpointsForMarkedForms,
  parseDeployFailure,
  parseStartpointsForProject,
  startPointFormNames,
  uniqueIdFromStartpoints,
} from "./deployParse.mjs";
import { resolveProjectForDeploy } from "./deployIdentity.mjs";
import {
  decideNameOccupancy,
  findOccupantInDeploymentsXml,
  findOccupantInNodeIndex,
  occupancyHatchAllowed,
  queryDeploymentsRequestXml,
} from "./deployOccupancy.mjs";
import {
  commandUnknownMessage,
  isValidUniqueId as isRetireUniqueId,
  parseRetireDeploymentXml,
  retireDeploymentRequestXml,
  vacatedTomcatName,
  vacateNodeStoreName,
} from "./retireDeployment.mjs";
import { clearFormAnswers, getOrCreateSession, resetSession, saveSession } from "./sessionStore.mjs";
import {
  buildQueryEmailStatusXml,
  buildSendTestEmailXml,
  parseEmailStatusXml,
} from "./emailStatus.mjs";
import {
  isValidUniqueId,
  purgeProjectResponsesByUniqueId,
  uniqueIdFromRuntimeUrl,
} from "./purgeProjectResponses.mjs";
import {
  exportProjectResponsesByUniqueId,
  importProjectResponsesByUniqueId,
} from "./projectResponses.mjs";
import { getVersionSnapshot, saveVersionSnapshot } from "./versionSnapshots.mjs";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
/** Repo root (…/Tawala) — website-mock/projects lives beside designer-web. */
const REPO_ROOT = path.resolve(__dirname, "../..");
const PORT = Number(process.env.TAWALA_DEV_PORT || 3001);
let HOST = process.env.TAWALA_DEV_HOST || "http://localhost:5173";
let JAVA_URL = process.env.TAWALA_JAVA_URL || "";
const DEV_USERS = { dev: "dev", designer: "designer" };

async function resolveJavaBackend() {
  if (process.env.TAWALA_DEV_ONLY === "1") {
    JAVA_URL = "";
  } else {
    if (!JAVA_URL) JAVA_URL = "http://localhost:8080";
    try {
      const res = await fetch(`${JAVA_URL.replace(/\/$/, "")}/login`, {
        signal: AbortSignal.timeout(5000),
      });
      if (!res.ok) throw new Error(`HTTP ${res.status}`);
      console.log(`  Java backend OK at ${JAVA_URL} — deploy will use Tomcat`);
    } catch (e) {
      // Always fall back to the Node runtime when Tomcat is down. start-dev.sh sets
      // TAWALA_JAVA_URL by default, so treating that as "must use Java" left Deploy
      // and live URLs broken whenever :8080 was offline.
      console.log(
        `  Java not reachable at ${JAVA_URL} (${e.message}) — deploy uses Node runtime on :5173/:3001`,
      );
      JAVA_URL = "";
    }
  }

  // Form action URLs must hit the Designer proxy (:5173), not Tomcat, when we are
  // on the Node runtime. A stale TAWALA_DEV_HOST=…:8080 made Submit open a dead page.
  if (!JAVA_URL && /:8080(?:\/|$)/.test(HOST)) {
    HOST = "http://localhost:5173";
    console.log(`  Corrected form URL base to ${HOST} (Node runtime)`);
  }
}

const app = express();
app.use(express.text({ type: ["text/xml", "application/xml", "text/plain"], limit: "50mb" }));
app.use(express.json({ limit: "50mb" }));

const xmlParser = new XMLParser({
  ignoreAttributes: false,
  attributeNamePrefix: "@_",
});

function checkAuth(user, password) {
  if (process.env.TAWALA_DEV_AUTH === "any") return true;
  return DEV_USERS[user] === password;
}

/** CORS for website-mock (:5500) calling purge / deploy / snapshots from the browser. */
function allowMockCors(req, res) {
  const origin = req.headers.origin || "";
  if (
    /^https?:\/\/(localhost|127\.0\.0\.1)(:\d+)?$/i.test(origin) ||
    origin === "null"
  ) {
    res.setHeader("Access-Control-Allow-Origin", origin === "null" ? "*" : origin);
    res.setHeader("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
    res.setHeader("Access-Control-Allow-Headers", "Content-Type");
    res.setHeader("Vary", "Origin");
  }
}

function deploymentXml(userId, projects, baseUrl) {
  const deployments = projects
    .map((entry) => {
      const data = store.getProjectByUniqueId(entry.uniqueId);
      if (!data) return "";
      const formLines = (data.project.forms ?? [])
        .map((form) => {
          const url = `${baseUrl}/p/${entry.uniqueId}/${encodeURIComponent(form.name)}`;
          return `      <startpoint form="${form.name}" url="${url}"/>`;
        })
        .join("\n");
      return `    <deployment project="${entry.name}">\n${formLines}\n    </deployment>`;
    })
    .join("\n");
  return `<?xml version="1.0" encoding="UTF-8"?>\n\n<response status="success">\n  <deployments user="${userId}">\n${deployments}\n  </deployments>\n</response>\n`;
}

function authFailedXml() {
  return `<?xml version="1.0" encoding="UTF-8"?>\n\n<response status="failure">\n  <error id="auth.failed" message="unknown userid or password"/>\n</response>\n`;
}

async function forwardToJava(xmlBody) {
  const res = await fetch(`${JAVA_URL.replace(/\/$/, "")}/client`, {
    method: "POST",
    headers: { "Content-Type": "text/xml; charset=utf-8" },
    body: xmlBody,
  });
  return res.text();
}

/** Legacy Designer XML API — same contract as ClientApiController */
app.post("/client", async (req, res) => {
  try {
    const parsed = xmlParser.parse(req.body);
    const request = parsed.request ?? parsed;
    const type = request["@_type"];
    const credNode = request.credentials ?? {};
    const user = credNode["@_user"] ?? credNode.user;
    const password = credNode["@_password"] ?? credNode.password;

    if (!checkAuth(user, password)) {
      res.type("text/xml").send(authFailedXml());
      return;
    }

    if (type === "queryDeployments") {
      const projects = store.listProjects(user);
      res.type("text/xml").send(deploymentXml(user, projects, HOST));
      return;
    }

    if (type === "retireDeployment") {
      const parsedReq = request.deployment || {};
      const uniqueId = String(
        parsedReq["@_uniqueId"] || parsedReq.uniqueId || request["@_uniqueId"] || "",
      ).trim();
      if (JAVA_URL) {
        const xml =
          typeof req.body === "string" && /uniqueId=/.test(req.body)
            ? req.body
            : retireDeploymentRequestXml(user, password, uniqueId);
        const javaResponse = await forwardToJava(xml);
        res.type("text/xml").send(javaResponse);
        return;
      }
      const vacated = vacateNodeStoreName(uniqueId);
      if (!vacated) {
        res.type("text/xml").send(
          failureXml("retire.notFound", `No live project for uniqueId ${uniqueId}`),
        );
        return;
      }
      const esc = (s) =>
        String(s ?? "")
          .replace(/&/g, "&amp;")
          .replace(/"/g, "&quot;")
          .replace(/</g, "&lt;");
      res.type("text/xml").send(
        `<?xml version="1.0" encoding="UTF-8"?>\n\n<response status="success">\n  <retired uniqueId="${esc(vacated.uniqueId)}" previousName="${esc(vacated.previousName)}" name="${esc(vacated.name)}" alreadyVacated="${vacated.alreadyVacated ? "true" : "false"}"/>\n</response>\n`,
      );
      return;
    }

    if (type === "uploadProject") {
      let project;
      if (request.project) {
        project = jsonFromLegacyXmlProject(request.project);
      }
      if (!project) {
        res.type("text/xml").send(failureXml("project.invalid", "Could not parse project"));
        return;
      }

      if (JAVA_URL) {
        const xml = typeof req.body === "string" ? req.body : buildUploadRequest({ user, password }, project);
        const javaResponse = await forwardToJava(xml);
        res.type("text/xml").send(javaResponse);
        return;
      }

      const entry = store.saveProject(user, project);
      const projects = store.listProjects(user);
      res.type("text/xml").send(deploymentXml(user, projects, HOST));
      return;
    }

    if (type === "previewForm") {
      const formName = request["@_form"];
      const project = request.project ? jsonFromLegacyXmlProject(request.project) : null;
      if (!project) {
        res.type("text/xml").send(failureXml("project.invalid", "Could not parse project for preview"));
        return;
      }
      store.putPreview(user, project);
      const url = `${HOST}/preview/${encodeURIComponent(user)}/${encodeURIComponent(project.name)}/${encodeURIComponent(formName)}`;
      res.type("text/xml").send(
        `<?xml version="1.0" encoding="UTF-8"?>\n\n<response status="success">\n  <formPreview url="${url}"/>\n</response>\n`,
      );
      return;
    }

    res.type("text/xml").send(failureXml("command.unknown", `Unknown command '${type}'.`));
  } catch (e) {
    console.error(e);
    res.type("text/xml").send(failureXml("server.error", String(e.message ?? e)));
  }
});

function failureXml(id, message) {
  return `<?xml version="1.0" encoding="UTF-8"?>\n\n<response status="failure">\n  <error id="${id}" message="${message}"/>\n</response>\n`;
}

app.options("/api/deploy", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

async function lookupOccupantForDeploy(user, password, tomcatName) {
  if (JAVA_URL) {
    const xml = queryDeploymentsRequestXml(user, password);
    const text = await forwardToJava(xml);
    const failure = parseDeployFailure(text);
    if (failure || !/status="success"/i.test(text)) {
      throw new Error(failure || "queryDeployments failed");
    }
    return findOccupantInDeploymentsXml(text, tomcatName);
  }
  return findOccupantInNodeIndex(store.listProjects(user), tomcatName);
}

/** JSON deploy from web Designer (also My Tawala Deploy-this-version via CORS). */
app.post("/api/deploy", async (req, res) => {
  allowMockCors(req, res);
  const { credentials, project } = req.body ?? {};
  if (!credentials?.user || !credentials?.password) {
    res.status(400).json({ status: "failure", error: "credentials required" });
    return;
  }
  if (!checkAuth(credentials.user, credentials.password)) {
    res.status(401).json({ status: "failure", error: "auth.failed" });
    return;
  }
  if (!project?.name) {
    res.status(400).json({ status: "failure", error: "project required" });
    return;
  }

  try {
      // File→New: mint a Tomcat name that cannot match an existing UserProject
      // (Java put reuses by name and keeps submissions). Later Pushes use deployIdentityName.
      const resolved = resolveProjectForDeploy(project);
      const {
        projectForDeploy,
        deployIdentityName,
        displayName,
        freshFromTemplate,
        identityMinted,
      } = resolved;

      let occupant = null;
      let lookupFailed = false;
      try {
        occupant = await lookupOccupantForDeploy(
          credentials.user,
          credentials.password,
          deployIdentityName,
        );
      } catch (e) {
        lookupFailed = true;
        console.error("occupancy lookup failed:", e);
      }
      const occupancy = decideNameOccupancy({
        tomcatName: deployIdentityName,
        incomingProject: project,
        occupant,
        hatch: occupancyHatchAllowed(req.body),
        lookupFailed,
      });
      if (!occupancy.allow) {
        const status = occupancy.code === "occupancy-lookup-failed" ? 503 : 409;
        res.status(status).json({
          status: "failure",
          error: occupancy.message,
          code: occupancy.code,
          occupantName: occupant?.name || null,
          occupantUniqueId: occupant?.uniqueId || null,
        });
        return;
      }

      if (JAVA_URL) {
        const xml = buildUploadRequest(credentials, projectForDeploy);
        const javaResponse = await forwardToJava(xml);
        const allForProject = parseStartpointsForProject(javaResponse, projectForDeploy.name);
        const startpoints = filterStartpointsForMarkedForms(projectForDeploy, allForProject);
        const failure = parseDeployFailure(javaResponse);
        if (failure) {
          res.status(502).json({
            status: "failure",
            mode: "java",
            error: failure,
            raw: javaResponse.slice(0, 2000),
          });
          return;
        }
        if (!javaResponse.includes('status="success"') || startpoints.length === 0) {
          const snippet = javaResponse.replace(/\s+/g, " ").slice(0, 200);
          const marked = startPointFormNames(projectForDeploy);
          let error;
          if (marked.size === 0) {
            error = `No forms marked as Starting Point in "${displayName}". Open a form, check Starting Point in Properties, then deploy again.`;
          } else if (allForProject.length === 0) {
            error = `Java deploy returned no start points for project "${projectForDeploy.name}". Restart the dev API if you still see DirtBowl URLs.`;
          } else {
            error = `Java deploy did not return URLs for the Starting Point form(s): ${[...marked].join(", ")}.`;
          }
          if (!javaResponse.includes('status="success"')) {
            error = `Java backend did not return success (${snippet})`;
          }
          res.status(502).json({
            status: "failure",
            mode: "java",
            error,
            raw: javaResponse.slice(0, 2000),
          });
          return;
        }
        const uniqueId = uniqueIdFromStartpoints(startpoints);
        res.json({
          status: "success",
          mode: "java",
          project: displayName,
          deployIdentityName,
          uniqueId,
          startpoints,
          freshFromTemplate,
          identityMinted,
        });
        return;
      }

    const entry = store.saveProject(credentials.user, projectForDeploy, {
      forceNewId: freshFromTemplate || identityMinted,
    });
    if (freshFromTemplate) {
      resetSession(entry.uniqueId, projectForDeploy);
    }
    const startpoints = (projectForDeploy.forms ?? [])
      .filter((f) => f.startPoint === true)
      .map((form) => ({
        form: form.name,
        url: `${HOST}/p/${entry.uniqueId}/${encodeURIComponent(form.name)}`,
      }));
    res.json({
      status: "success",
      mode: "dev",
      project: displayName,
      deployIdentityName,
      uniqueId: entry.uniqueId,
      startpoints,
      freshFromTemplate,
      identityMinted,
    });
  } catch (e) {
    console.error(e);
    res.status(500).json({ status: "failure", error: String(e.message ?? e) });
  }
});

app.options("/api/retire-name", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

/**
 * Library admin Retire: free the Tomcat / Node Deploy *name* for occupancy.
 * Lookup is uniqueId-only (do not retarget Online Exam catalog ids). uniqueId is kept.
 */
app.post("/api/retire-name", async (req, res) => {
  allowMockCors(req, res);
  const body = req.body ?? {};
  const credentials = body.credentials;
  const uniqueId = String(body.uniqueId || "").trim();
  if (!credentials?.user || !credentials?.password) {
    res.status(400).json({ status: "failure", error: "credentials required" });
    return;
  }
  if (!checkAuth(credentials.user, credentials.password)) {
    res.status(401).json({ status: "failure", error: "auth.failed" });
    return;
  }
  if (!isRetireUniqueId(uniqueId)) {
    res.status(400).json({
      status: "failure",
      error: "uniqueId required (1–20 alphanumeric)",
    });
    return;
  }

  try {
    if (JAVA_URL) {
      const xml = retireDeploymentRequestXml(credentials.user, credentials.password, uniqueId);
      const text = await forwardToJava(xml);
      const parsed = parseRetireDeploymentXml(text);
      if (parsed.code === "command.unknown") {
        res.status(501).json({
          status: "failure",
          uniqueId,
          uniqueIdUnchanged: true,
          code: "command.unknown",
          error: commandUnknownMessage(),
        });
        return;
      }
      if (parsed.status !== "success") {
        const status = parsed.code === "retire.notFound" ? 404 : 502;
        res.status(status).json({
          status: "failure",
          uniqueId,
          uniqueIdUnchanged: true,
          code: parsed.code || "retire-failed",
          error: parsed.error || "retireDeployment failed",
        });
        return;
      }
      /* Same uniqueId may also sit in the Node store from a prior dev-only Push. */
      vacateNodeStoreName(uniqueId);
      res.json({
        status: "success",
        mode: "java",
        uniqueId: parsed.uniqueId || uniqueId,
        previousName: parsed.previousName || null,
        name: parsed.name || vacatedTomcatName(parsed.previousName, uniqueId),
        alreadyVacated: !!parsed.alreadyVacated,
        uniqueIdUnchanged: true,
      });
      return;
    }

    const vacated = vacateNodeStoreName(uniqueId);
    if (!vacated) {
      res.status(404).json({
        status: "failure",
        uniqueId,
        uniqueIdUnchanged: true,
        code: "retire.notFound",
        error: `No live Node Deploy slot for uniqueId ${uniqueId}`,
      });
      return;
    }
    res.json({
      status: "success",
      mode: "dev",
      uniqueId: vacated.uniqueId,
      previousName: vacated.previousName,
      name: vacated.name,
      alreadyVacated: vacated.alreadyVacated,
      uniqueIdUnchanged: true,
    });
  } catch (e) {
    console.error("retire-name failed:", e);
    res.status(503).json({
      status: "failure",
      uniqueId,
      uniqueIdUnchanged: true,
      code: "retire-unreachable",
      error:
        "Couldn't reach :8080 to free that live name. Retire aborted so occupancy is not lying. Confirm Tomcat is up, then retry.",
    });
  }
});

app.get("/p/:uniqueId/:formName", (req, res) => {
  const data = store.getProjectByUniqueId(req.params.uniqueId);
  if (!data) {
    res.status(404).send("Project not found");
    return;
  }
  if (req.query.reset !== undefined) {
    resetSession(req.params.uniqueId, data.project);
    res.redirect(302, `/p/${req.params.uniqueId}/${encodeURIComponent(req.params.formName)}`);
    return;
  }
  const session = getOrCreateSession(req.params.uniqueId, data.project);
  // Fresh signup / next respondent: clear prior answers but keep stored records.
  if (req.query.fresh !== undefined) {
    const form = data.project.forms?.find((f) => f.name === req.params.formName);
    clearFormAnswers(session, req.params.formName, form);
    saveSession(req.params.uniqueId, session);
    res.redirect(302, `/p/${req.params.uniqueId}/${encodeURIComponent(req.params.formName)}`);
    return;
  }
  const from = req.query.from ? String(req.query.from) : undefined;
  res
    .type("html")
    .send(
      runtime.renderFormPage(data.project, req.params.formName, HOST, req.params.uniqueId, session, {
        fromLabel: from,
      }),
    );
  saveSession(req.params.uniqueId, session);
});

app.post("/p/:uniqueId/:formName", express.urlencoded({ extended: true }), (req, res) => {
  const data = store.getProjectByUniqueId(req.params.uniqueId);
  if (!data) {
    res.status(404).send("Project not found");
    return;
  }
  const session = getOrCreateSession(req.params.uniqueId, data.project);
  const html = runtime.handleFormSubmit(
    data.project,
    req.params.formName,
    session,
    req.body,
    HOST,
    req.params.uniqueId,
  );
  saveSession(req.params.uniqueId, session);
  res.type("html").send(html);
});

app.post("/api/preview", (req, res) => {
  const { project, resetSession: doReset } = req.body ?? {};
  if (!project?.name) {
    res.status(400).json({ error: "project required" });
    return;
  }
  const { runtimeId } = store.putPreview("designer", project);
  if (doReset) {
    resetSession(runtimeId, project);
  }
  res.json({ ok: true, project: project.name, runtimeId });
});

app.get("/preview/:userId/:projectName/:formName", (req, res) => {
  const data = store.getPreview(req.params.userId, req.params.projectName);
  if (!data) {
    res.status(404).send("Preview not found — use Preview tab in Designer");
    return;
  }
  const runtimeId = store.previewRuntimeId(req.params.userId, data.project.name);
  // Ensure runtime project file exists (may have been put under old id).
  store.putPreview(req.params.userId, data.project);
  const session = getOrCreateSession(runtimeId, data.project);
  res
    .type("html")
    .send(
      runtime.renderFormPage(data.project, req.params.formName, HOST, runtimeId, session, {
        designerPreview: true,
      }),
    );
});

app.get("/api/health", (_req, res) => {
  res.json({
    ok: true,
    javaProxy: Boolean(JAVA_URL),
    javaUrl: JAVA_URL || null,
    host: HOST,
    runtime: JAVA_URL ? "java" : "dev",
  });
});

/**
 * Website-mock Test Drive gate: fetch a local :8080 form URL server-side (no CORS) and
 * detect the legacy fail page (“We are very sorry”) when World is not initialized or
 * the path is missing. HTTP 200 alone is not success.
 */
app.options("/api/probe-java-url", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

app.get("/api/probe-java-url", async (req, res) => {
  allowMockCors(req, res);
  const raw = String(req.query.url || "").trim();
  if (!raw) {
    res.status(400).json({ ok: false, error: "url query required" });
    return;
  }
  let parsed;
  try {
    parsed = new URL(raw);
  } catch {
    res.status(400).json({ ok: false, error: "invalid url" });
    return;
  }
  const host = (parsed.hostname || "").toLowerCase();
  if (host !== "localhost" && host !== "127.0.0.1") {
    res.status(400).json({ ok: false, error: "only localhost :8080 URLs are allowed" });
    return;
  }
  const port = String(parsed.port || (parsed.protocol === "https:" ? "443" : "80"));
  if (port !== "8080") {
    res.status(400).json({ ok: false, error: "only localhost :8080 URLs are allowed" });
    return;
  }
  try {
    const upstream = await fetch(parsed.toString(), {
      method: "GET",
      redirect: "follow",
      signal: AbortSignal.timeout(4000),
      headers: { Accept: "text/html,*/*" },
    });
    const text = await upstream.text();
    const titleMatch = text.match(/<title[^>]*>([^<]*)<\/title>/i);
    const title = titleMatch ? titleMatch[1].trim() : "";
    const failPage =
      /We are very sorry/i.test(text) ||
      /not currently available/i.test(text) ||
      /World is not initialized/i.test(text) ||
      /Project Not Found/i.test(title) ||
      /Project Not Found/i.test(text) ||
      (/<h1[^>]*>\s*Error\s*<\/h1>/i.test(text) && /Tawala Team/i.test(text));
    if (failPage) {
      res.json({
        ok: false,
        reason: "fail-page",
        status: upstream.status,
        title,
        detail:
          "Java returned the legacy fail page (often World not initialized — restart tawala-tomcat after Postgres is healthy).",
      });
      return;
    }
    if (!upstream.ok) {
      res.json({
        ok: false,
        reason: "http-error",
        status: upstream.status,
        title,
        detail: `Java returned HTTP ${upstream.status}`,
      });
      return;
    }
    res.json({ ok: true, status: upstream.status, title });
  } catch (e) {
    res.status(502).json({
      ok: false,
      reason: "unreachable",
      error: String(e.message || e),
      detail: "Could not reach the Java form URL from the Designer API.",
    });
  }
});

app.options("/api/purge-responses", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

/**
 * Purge form submissions for a deployed project by uniqueId (`/p/{id}/…`).
 * Local Docker Postgres (same as Project Manager purgeProjectResponses) + Node session clear.
 * Used by website-mock Test drive (purge-on-start) and My Tawala PURGE.
 */
app.post("/api/purge-responses", async (req, res) => {
  allowMockCors(req, res);
  const body = req.body ?? {};
  const credentials = body.credentials;
  let uniqueId = body.uniqueId;
  if (!uniqueId && body.url) {
    uniqueId = uniqueIdFromRuntimeUrl(body.url);
  }
  if (!credentials?.user || !credentials?.password) {
    res.status(400).json({ status: "failure", error: "credentials required" });
    return;
  }
  if (!checkAuth(credentials.user, credentials.password)) {
    res.status(401).json({ status: "failure", error: "auth.failed" });
    return;
  }
  if (!isValidUniqueId(uniqueId)) {
    res.status(400).json({
      status: "failure",
      error: "uniqueId required (1–20 alphanumeric, or pass url with /p/{id}/…)",
    });
    return;
  }
  try {
    const result = await purgeProjectResponsesByUniqueId(uniqueId);
    const code = result.status === "success" ? 200 : 502;
    res.status(code).json(result);
  } catch (e) {
    console.error(e);
    res.status(500).json({ status: "failure", uniqueId, error: String(e.message ?? e) });
  }
});

app.options("/api/export-responses", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

/**
 * Export form submissions for a deployed project by uniqueId — Excel-response-data EXPORT
 * and the data half of BACKUP. Postgres (Java) or dev session store; see projectResponses.mjs.
 */
app.post("/api/export-responses", async (req, res) => {
  allowMockCors(req, res);
  const body = req.body ?? {};
  const { credentials, uniqueId } = body;
  if (!credentials?.user || !credentials?.password) {
    res.status(400).json({ status: "failure", error: "credentials required" });
    return;
  }
  if (!checkAuth(credentials.user, credentials.password)) {
    res.status(401).json({ status: "failure", error: "auth.failed" });
    return;
  }
  if (!isValidUniqueId(uniqueId)) {
    res.status(400).json({ status: "failure", error: "uniqueId required (1–20 alphanumeric)" });
    return;
  }
  try {
    const result = await exportProjectResponsesByUniqueId(uniqueId);
    res.status(result.status === "success" ? 200 : 502).json(result);
  } catch (e) {
    console.error(e);
    res.status(500).json({ status: "failure", uniqueId, error: String(e.message ?? e) });
  }
});

app.options("/api/import-responses", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

/**
 * Import (replace) form submissions for a deployed project by uniqueId — data-only IMPORT
 * and the data half of RESTORE. Expects the same `forms` shape returned by export-responses
 * for the matching `source`. Field-mismatch validation happens client-side (website-mock);
 * this endpoint just writes what it's given.
 */
app.post("/api/import-responses", async (req, res) => {
  allowMockCors(req, res);
  const body = req.body ?? {};
  const { credentials, uniqueId, forms, source, mode } = body;
  if (!credentials?.user || !credentials?.password) {
    res.status(400).json({ status: "failure", error: "credentials required" });
    return;
  }
  if (!checkAuth(credentials.user, credentials.password)) {
    res.status(401).json({ status: "failure", error: "auth.failed" });
    return;
  }
  if (!isValidUniqueId(uniqueId)) {
    res.status(400).json({ status: "failure", error: "uniqueId required (1–20 alphanumeric)" });
    return;
  }
  try {
    const result = await importProjectResponsesByUniqueId(uniqueId, forms, { source, mode });
    res.status(result.status === "success" ? 200 : 502).json(result);
  } catch (e) {
    console.error(e);
    res.status(500).json({ status: "failure", uniqueId, error: String(e.message ?? e) });
  }
});

app.options("/api/version-snapshots", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

app.options("/api/version-snapshots/:snapshotId", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

/**
 * Store a Designer project JSON for My Tawala Deploy-this-version.
 * Called from Designer Show in My Tawala (receipt URL cannot carry the full definition).
 */
app.post("/api/version-snapshots", (req, res) => {
  allowMockCors(req, res);
  const body = req.body ?? {};
  try {
    const saved = saveVersionSnapshot({
      project: body.project,
      uniqueId: body.uniqueId || null,
      projectId: body.projectId || null,
      versionDescription: body.versionDescription || "",
      at: body.at || null,
    });
    res.json({ status: "success", ...saved });
  } catch (e) {
    res.status(400).json({ status: "failure", error: String(e.message ?? e) });
  }
});

app.get("/api/version-snapshots/:snapshotId", (req, res) => {
  allowMockCors(req, res);
  const record = getVersionSnapshot(req.params.snapshotId);
  if (!record) {
    res.status(404).json({ status: "failure", error: "snapshot not found" });
    return;
  }
  res.json({ status: "success", ...record });
});

/**
 * Read a seeded website-mock / samples catalog JSON for Edit project in Designer
 * (`?mockJson=` on :5173). Path-restricted to:
 *   website-mock/projects/{mytawala|library}/*.json
 *   designer-web/public/samples/…/*.json (any depth under samples)
 */
app.options("/api/open-mock-json", (req, res) => {
  allowMockCors(req, res);
  res.status(204).end();
});

app.get("/api/open-mock-json", (req, res) => {
  allowMockCors(req, res);
  const rel = String(req.query.path || "").trim().replace(/\\/g, "/");
  if (rel.includes("..") || rel.startsWith("/") || rel.includes("\0")) {
    res.status(400).json({ status: "failure", error: "invalid path" });
    return;
  }
  const projectsOk = /^projects\/(mytawala|library)\/[^/]+\.json$/i.test(rel);
  const samplesOk = /^designer-web\/public\/samples\/.+\.json$/i.test(rel);
  if (!projectsOk && !samplesOk) {
    res.status(400).json({
      status: "failure",
      error: "invalid path (projects/mytawala|library/*.json or designer-web/public/samples/…/*.json only)",
    });
    return;
  }
  const full = path.resolve(path.join(REPO_ROOT, rel.startsWith("designer-web/") ? rel : path.join("website-mock", rel)));
  const allowedRoot = projectsOk
    ? path.resolve(path.join(REPO_ROOT, "website-mock", "projects"))
    : path.resolve(path.join(REPO_ROOT, "designer-web", "public", "samples"));
  if (full !== allowedRoot && !full.startsWith(allowedRoot + path.sep)) {
    res.status(400).json({ status: "failure", error: "path escapes allowed root" });
    return;
  }
  if (!fs.existsSync(full)) {
    res.status(404).json({ status: "failure", error: "file not found" });
    return;
  }
  try {
    const project = JSON.parse(fs.readFileSync(full, "utf8"));
    if (!project || typeof project !== "object" || !project.name) {
      res.status(400).json({ status: "failure", error: "file is not a Designer project JSON" });
      return;
    }
    res.json({ status: "success", path: rel, project });
  } catch (e) {
    res.status(400).json({ status: "failure", error: String(e.message ?? e) });
  }
});

/** Server-owned outbound email status (no secrets returned). */
app.post("/api/email/status", async (req, res) => {
  const { credentials } = req.body ?? {};
  if (!credentials?.user || !credentials?.password) {
    res.status(400).json({ status: "failure", error: "credentials required", mode: "offline" });
    return;
  }
  if (!checkAuth(credentials.user, credentials.password)) {
    res.status(401).json({ status: "failure", error: "auth.failed", mode: "offline" });
    return;
  }
  if (!JAVA_URL) {
    res.json({
      available: false,
      mode: "dev",
      enabled: false,
      configured: false,
      host: "",
      port: 0,
      auth: false,
      starttls: false,
      fromAddress: "",
      fromName: "",
      workerEnabled: false,
      readyCount: 0,
      sendingCount: 0,
      sentCount: 0,
      errorCount: 0,
      lastError: "",
      lastErrorAt: 0,
      lastSuccessAt: 0,
      lastWorkerRunAt: 0,
      error: "Java/Tomcat is not connected — email delivery only runs on :8080",
    });
    return;
  }
  try {
    const javaResponse = await forwardToJava(buildQueryEmailStatusXml(credentials));
    const parsed = parseEmailStatusXml(javaResponse, "java");
    if (parsed.error && !parsed.available) {
      res.status(502).json(parsed);
      return;
    }
    res.json(parsed);
  } catch (e) {
    res.status(502).json({
      available: false,
      mode: "offline",
      enabled: false,
      configured: false,
      host: "",
      port: 0,
      auth: false,
      starttls: false,
      fromAddress: "",
      fromName: "",
      workerEnabled: false,
      readyCount: 0,
      sendingCount: 0,
      sentCount: 0,
      errorCount: 0,
      lastError: "",
      lastErrorAt: 0,
      lastSuccessAt: 0,
      lastWorkerRunAt: 0,
      error: String(e.message ?? e),
    });
  }
});

/** Rate-limited test send via Java Client API. */
app.post("/api/email/test", async (req, res) => {
  const { credentials, to } = req.body ?? {};
  if (!credentials?.user || !credentials?.password) {
    res.status(400).json({ status: "failure", error: "credentials required", mode: "offline" });
    return;
  }
  if (!checkAuth(credentials.user, credentials.password)) {
    res.status(401).json({ status: "failure", error: "auth.failed", mode: "offline" });
    return;
  }
  if (!to || typeof to !== "string" || !to.includes("@")) {
    res.status(400).json({ status: "failure", error: "valid to address required", mode: JAVA_URL ? "java" : "dev" });
    return;
  }
  if (!JAVA_URL) {
    res.status(503).json({
      status: "failure",
      mode: "dev",
      error: "Java/Tomcat is not connected — cannot send test email",
    });
    return;
  }
  try {
    const javaResponse = await forwardToJava(buildSendTestEmailXml(credentials, to.trim()));
    const parsed = parseEmailStatusXml(javaResponse, "java");
    if (javaResponse.includes('status="failure"') || parsed.error) {
      res.status(502).json({
        ...parsed,
        error: parsed.error ?? "Test email failed",
      });
      return;
    }
    res.json(parsed);
  } catch (e) {
    res.status(502).json({
      status: "failure",
      mode: "offline",
      error: String(e.message ?? e),
    });
  }
});

/**
 * Safari-friendly project download. Blob `<a download>` is unreliable in WebKit
 * (silent no-op / `.json`→`.html` nudge). Form POST + Content-Disposition forces a
 * real Downloads entry named `*.json` while staying inside the user-gesture turn.
 */
app.post(
  "/api/download-project",
  express.urlencoded({ extended: true, limit: "50mb" }),
  (req, res) => {
    const rawName = typeof req.body?.filename === "string" ? req.body.filename : "Untitled.json";
    const cleaned = rawName.replace(/[\\/:*?"<>|\r\n\0]+/g, "_").trim() || "Untitled.json";
    const filename = cleaned.toLowerCase().endsWith(".json") ? cleaned : `${cleaned}.json`;
    const json = typeof req.body?.json === "string" ? req.body.json : "{}";
    const ascii = filename.replace(/[^\x20-\x7E]/g, "_").replace(/"/g, "");
    res.setHeader("Content-Type", "application/json; charset=utf-8");
    res.setHeader(
      "Content-Disposition",
      `attachment; filename="${ascii}"; filename*=UTF-8''${encodeURIComponent(filename)}`,
    );
    res.setHeader("X-Content-Type-Options", "nosniff");
    res.status(200).send(json);
  },
);

await resolveJavaBackend();

app.listen(PORT, () => {
  console.log(`Tawala dev API listening on http://localhost:${PORT}`);
  const runtimeBase = JAVA_URL ? `${JAVA_URL.replace(/\/$/, "")}/p/{id}/{form}` : `${HOST}/p/{id}/{form}`;
  console.log(`  Runtime URLs base: ${runtimeBase}`);
  console.log(`  Dev credentials: dev/dev or designer/designer`);
  if (JAVA_URL) console.log(`  Forwarding deploy to: ${JAVA_URL}/client`);
  else console.log(`  Tip: start Docker (port 8080) and restart to deploy to Java`);
});

/** Minimal legacy XML project → JSON for /client uploads that include XML project body */
function jsonFromLegacyXmlProject(node) {
  if (!node) return null;
  const name = node["@_name"] ?? node.name ?? "Untitled";
  const themePath = node["@_themePath"] ?? node.themePath ?? "default";
  const formsNode = node.forms?.form ?? node.form ?? [];
  const forms = [].concat(formsNode).map((f) => ({
    name: f["@_name"] ?? f.name,
    startPoint: (f["@_startPoint"] ?? f.startPoint) === "true" || f.startPoint === true,
    process: f["@_process"] ?? f.process,
    preProcess: f["@_preProcess"] ?? f.preProcess,
    themePath: f["@_themePath"] ?? f.themePath,
    items: [],
  }));
  return { name, format: "2.0", themePath, forms, processes: [], documents: [] };
}
