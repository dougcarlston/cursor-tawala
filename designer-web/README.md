# Tawala Designer (Web)

Browser-based replacement for the legacy **C# WinForms Tawala Designer**, with deploy-to-test against a local dev runtime or the Java backend.

## Stack

- React 19 + TypeScript + Vite
- Zustand (project state)
- Express dev API (`server/`) — legacy `/client` + JSON deploy + form runtime

## Run locally

```bash
cd designer-web
npm install
npm run dev
```

This starts **both**:
- Designer UI — http://localhost:5173
- Dev API + runtime — http://localhost:3001 (proxied via Vite)

### Deploy credentials (dev)

| User | Password | Notes |
|------|----------|--------|
| `dev` | `dev` | Works for Node runtime **and** Java Tomcat `/client` |
| `designer` | `dev` | Java DB seed (same password as `dev`). Node API also accepts `designer` / `designer`. |

Use **File → Deploy** or the toolbar **Deploy** button. On first deploy you’ll be prompted for Designer credentials (not DirtBowl participant login). For **Java Deploy** (`javaProxy: true`), prefer **`dev` / `dev`**.

After deploy, the dialog lists **start point URLs** like:

`http://localhost:5173/p/{uniqueId}/Registration`

Open those links to exercise the dev runtime HTML forms.

### Preview / Deploy “failed to fetch”

Vite (`:5173`) can stay up while the Express API (`:3001`) dies (common when a tool shell exits and SIGTERMs the API). Check and restart:

```bash
curl -s http://localhost:3001/api/health
cd designer-web && bash scripts/ensure-dev-api.sh
```

Prefer starting Designer from a **terminal you leave open**: `cd designer-web && npm run dev` (or `npm run dev:local` for Node-only Deploy).

### Java Deploy (Tomcat `:8080`)

1. Start containers from the repo root: `docker compose up -d`
2. Wait until `http://localhost:8080` responds.
3. Restart the Designer API **without** `TAWALA_DEV_ONLY` so it can discover Java:
   ```bash
   cd designer-web
   # stop old API/Vite if needed, then:
   unset TAWALA_DEV_ONLY
   export TAWALA_JAVA_URL=http://localhost:8080
   npm run dev
   ```
4. Confirm `/api/health` shows `"javaProxy": true`, then **File → Deploy** again — start URLs will target Tomcat.

Until Tomcat is up, Deploy stays on the Node runtime (`:5173`/`:3001`).

## Load DirtBowl (advanced)

DirtBowl is no longer on **File** menu — use **Open Project…** with `public/samples/dirtbowl_definition_v3.json`, or deploy via `node scripts/deploy-dirtbowl-java.mjs` from the repo root. It will move to the website **Library** as an advanced sample.

**SignupSheets sample:** `public/samples/signup-sheets.json` (converted from legacy XML via `scripts/convert-signupsheets-xml-to-json.mjs`; see `public/samples/legacy/SignupSheets_CONVERSION_GAPS.md`).

## Proxy to Java backend (build-1700)

When Tomcat is running the legacy webapp:

```bash
export TAWALA_JAVA_URL=http://localhost:8080
export TAWALA_DEV_HOST=http://localhost:8080   # URLs in deploy response
npm run dev:api
```

Deploy converts JSON → XML (`server/jsonToXml.mjs`) and POSTs to `/client` like the original Designer.

## Phase status

| Phase | Status |
|-------|--------|
| 1 — Shell, explorer, form items, preview | Done |
| 2 — Item property panels, delete | Partial (properties for main types) |
| 3 — Rich text, process editor, documents | Done (MVP) |
| 4 — Deploy + dev runtime v2 | Done |
| 5 — **New Project templates** (featured home apps) | **In progress** — File → New Project… |
| 6 — Full Java + PostgreSQL integration | Documented — see `../docs/STACK.md` |

### New Project templates (July 2026)

**File → New Project…** opens the legacy template catalog. JSON starters live in `public/samples/templates/`:

| Template | File |
|----------|------|
| Simple Survey | `simple-survey.json` |
| Sign-up Sheet | `signup-sheet.json` |
| Potluck (simplified starter) | `potluck.json` |
| Get Together | `get-together.json` |
| Empty / Form+Process / Form+Process+Document | `empty-project.json`, etc. |

Deploy to **8080** with `TAWALA_JAVA_URL=http://localhost:8080 npm run dev:api` (or `scripts/dev-java.sh`). Survey/report function tables export via `jsonToXml.mjs` (`choiceTallyTable`, `itemizationTable`, `questionCorrelationTable`).

**Not in browser Designer yet:** Sign-up Sheet w Email (blocked on mail backlog), full Potluck process/doc chain, general `.tawala` import (SignupSheets one-off converter only — see above).

### Phase 3 notes

- **Forms:** `contentEditable` rich text for text items (replaces legacy IE toolbar)
- **Processes:** command list + JSON editor (`+set`, `+get`, `+if`, `+foreach`, …)
- **Documents:** rich editor for simple docs; full DirtBowl docs remain JSON-editable

Dev runtime **v2** adds pre-process, skip logic, field refs, and dynamic division lists (DirtBowl). Post-process chains, payments, and email still need Java — see `../docs/STACK.md`.

## Project format

JSON **format 2.0** per `../TAWALA_XML_TO_JSON_MAPPING.md`.
