import express from "express";
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
import { getOrCreateSession, resetSession, saveSession } from "./sessionStore.mjs";

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

/** JSON deploy from web Designer */
app.post("/api/deploy", async (req, res) => {
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
      if (JAVA_URL) {
        const xml = buildUploadRequest(credentials, project);
        const javaResponse = await forwardToJava(xml);
        const allForProject = parseStartpointsForProject(javaResponse, project.name);
        const startpoints = filterStartpointsForMarkedForms(project, allForProject);
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
          const marked = startPointFormNames(project);
          let error;
          if (marked.size === 0) {
            error = `No forms marked as Starting Point in "${project.name}". Open a form, check Starting Point in Properties, then deploy again.`;
          } else if (allForProject.length === 0) {
            error = `Java deploy returned no start points for project "${project.name}". Restart the dev API if you still see DirtBowl URLs.`;
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
        res.json({
          status: "success",
          mode: "java",
          project: project.name,
          uniqueId: uniqueIdFromStartpoints(startpoints),
          startpoints,
        });
        return;
      }

    const entry = store.saveProject(credentials.user, project);
    const startpoints = (project.forms ?? [])
      .filter((f) => f.startPoint === true)
      .map((form) => ({
        form: form.name,
        url: `${HOST}/p/${entry.uniqueId}/${encodeURIComponent(form.name)}`,
      }));
    res.json({
      status: "success",
      mode: "dev",
      project: project.name,
      uniqueId: entry.uniqueId,
      startpoints,
    });
  } catch (e) {
    console.error(e);
    res.status(500).json({ status: "failure", error: String(e.message ?? e) });
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
  const { project } = req.body ?? {};
  if (!project?.name) {
    res.status(400).json({ error: "project required" });
    return;
  }
  store.putPreview("designer", project);
  res.json({ ok: true, project: project.name });
});

app.get("/preview/:userId/:projectName/:formName", (req, res) => {
  const data = store.getPreview(req.params.userId, req.params.projectName);
  if (!data) {
    res.status(404).send("Preview not found — use Preview tab in Designer");
    return;
  }
  const session = getOrCreateSession(`preview-${req.params.userId}`, data.project);
  res
    .type("html")
    .send(
      runtime.renderFormPage(data.project, req.params.formName, HOST, `preview-${req.params.userId}`, session, {
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
