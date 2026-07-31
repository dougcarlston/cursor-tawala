#!/usr/bin/env node
/**
 * Tiny static file server for website-mock (Node built-ins only).
 * Prefer this over `python3 -m http.server` — more reliable under rapid reloads.
 *
 * Usage (via serve.sh): SERVE_ROOT=… SERVE_PORT=5500 SERVE_HOST=127.0.0.1 node serve-static.mjs
 */
import http from "node:http";
import fs from "node:fs";
import path from "node:path";
import { fileURLToPath } from "node:url";

const __dirname = path.dirname(fileURLToPath(import.meta.url));
const ROOT = path.resolve(process.env.SERVE_ROOT || __dirname);
const PORT = Number(process.env.SERVE_PORT || process.argv[2] || 5500);
const HOST = process.env.SERVE_HOST || "127.0.0.1";

const MIME = {
  ".html": "text/html; charset=utf-8",
  ".css": "text/css; charset=utf-8",
  ".js": "text/javascript; charset=utf-8",
  ".mjs": "text/javascript; charset=utf-8",
  ".json": "application/json; charset=utf-8",
  ".png": "image/png",
  ".jpg": "image/jpeg",
  ".jpeg": "image/jpeg",
  ".gif": "image/gif",
  ".svg": "image/svg+xml",
  ".ico": "image/x-icon",
  ".webp": "image/webp",
  ".woff": "font/woff",
  ".woff2": "font/woff2",
  ".map": "application/json",
  ".txt": "text/plain; charset=utf-8",
  ".md": "text/plain; charset=utf-8",
};

function safeJoin(root, urlPath) {
  const raw = (urlPath || "/").split("?")[0].split("#")[0];
  let decoded;
  try {
    decoded = decodeURIComponent(raw);
  } catch {
    return null;
  }
  const rel = decoded.replace(/^\/+/, "");
  const full = path.normalize(path.join(root, rel));
  const rootNorm = path.normalize(root);
  if (full !== rootNorm && !full.startsWith(rootNorm + path.sep)) {
    return null;
  }
  return full;
}

const server = http.createServer((req, res) => {
  if (req.method !== "GET" && req.method !== "HEAD") {
    res.writeHead(405, { Allow: "GET, HEAD" });
    res.end("Method Not Allowed");
    return;
  }

  let filePath = safeJoin(ROOT, req.url || "/");
  if (!filePath) {
    res.writeHead(403, { "Content-Type": "text/plain; charset=utf-8" });
    res.end("Forbidden");
    return;
  }

  fs.stat(filePath, (statErr, st) => {
    if (!statErr && st.isDirectory()) {
      filePath = path.join(filePath, "index.html");
    }
    fs.readFile(filePath, (readErr, data) => {
      if (readErr) {
        res.writeHead(404, { "Content-Type": "text/plain; charset=utf-8" });
        res.end("Not Found");
        return;
      }
      const ext = path.extname(filePath).toLowerCase();
      res.writeHead(200, {
        "Content-Type": MIME[ext] || "application/octet-stream",
        "Cache-Control": "no-cache",
      });
      if (req.method === "HEAD") {
        res.end();
      } else {
        res.end(data);
      }
    });
  });
});

server.on("error", (err) => {
  console.error(`[serve-static] ${err.message || err}`);
  process.exit(1);
});

server.listen(PORT, HOST, () => {
  console.log(`Serving ${ROOT} at http://${HOST}:${PORT}/`);
  console.log(`Library: http://${HOST}:${PORT}/library.html`);
  console.log(`My Tawala: http://${HOST}:${PORT}/mytawala.html`);
});
