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

| User | Password |
|------|----------|
| `dev` | `dev` |
| `designer` | `designer` |

Use **File → Deploy** or the toolbar **Deploy** button. On first deploy you’ll be prompted for Designer credentials (not DirtBowl participant login).

After deploy, the dialog lists **start point URLs** like:

`http://localhost:5173/p/{uniqueId}/Registration`

Open those links to exercise the dev runtime HTML forms.

## Load DirtBowl sample

```bash
cp ../dirtbowl_definition_v3.json public/samples/
```

Then **File → Open DirtBowl Sample…**, edit, and deploy.

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
| 5 — Full Java + PostgreSQL integration | Documented — needs WAR (see `../docs/STACK.md`) |

### Phase 3 notes

- **Forms:** `contentEditable` rich text for text items (replaces legacy IE toolbar)
- **Processes:** command list + JSON editor (`+set`, `+get`, `+if`, `+foreach`, …)
- **Documents:** rich editor for simple docs; full DirtBowl docs remain JSON-editable

Dev runtime **v2** adds pre-process, skip logic, field refs, and dynamic division lists (DirtBowl). Post-process chains, payments, and email still need Java — see `../docs/STACK.md`.

## Project format

JSON **format 2.0** per `../TAWALA_XML_TO_JSON_MAPPING.md`.
