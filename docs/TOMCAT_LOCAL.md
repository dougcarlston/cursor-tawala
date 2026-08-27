# Local Tomcat (`:8080`) — start and refresh CSS

Plain-English ops for viewing Push/Deploy forms (e.g. DirtBowl Registration).

## What Tomcat is

Docker runs a **Tomcat** container (`tawala-tomcat`) that serves the Java Tawala webapp on **http://localhost:8080**. Designer Push sends projects there; you open the start URL in a browser to smoke-test.

## Start Tomcat (first time or after reboot)

1. Open **Terminal**.
2. Make sure **Docker Desktop** is running (whale icon in the menu bar).
3. Run:

```bash
cd ~/Projects/Tawala
docker compose up -d
```

Wait until it finishes. Check: open http://localhost:8080/client — you should get a page (not “connection refused”).

Stop later (optional): `docker compose stop`

## After editing CSS under `docker/tomcat/css/`

The container has its **own copy** of the CSS. Editing files on disk does **not** update `:8080` until you copy or rebuild.

### Fast path — copy into the running container

```bash
cd ~/Projects/Tawala

docker cp docker/tomcat/css/project/form-layout-core.css \
  tawala-tomcat:/usr/local/tomcat/webapps/ROOT/css/project/form-layout-core.css

docker cp docker/tomcat/css/project/dirtbowl2/project.css \
  tawala-tomcat:/usr/local/tomcat/webapps/ROOT/css/project/dirtbowl2/project.css
```

### Fast path — `default.js` (TinyMCE / Online Exam SetupVariables)

Canonical file: `TawalaWebapp-build1700/web/scripts/project/default.js`. Copy into the hot-patch tree, then into the container:

```bash
cd ~/Projects/Tawala
cp TawalaWebapp-build1700/web/scripts/project/default.js \
  docker/tomcat/scripts/project/default.js

docker cp docker/tomcat/scripts/project/default.js \
  tawala-tomcat:/usr/local/tomcat/webapps/ROOT/scripts/project/default.js
```

Then in the browser: **hard-refresh** — **Cmd+Shift+R** (Mac) so the old CSS/JS is not cached. On Online Exam SetupVariables, Pre/Post boxes should no longer show a first line of `&#x0D;`. Java (`TextInput` / `FormSubmission`) needs a WAR rebuild or class copy + Tomcat restart to stop writing new CLOB rows with that entity.

### Full rebuild (slower; use after many CSS/image changes)

```bash
cd ~/Projects/Tawala
docker compose build tawala && docker compose up -d tawala
```

Then hard-refresh again.

## Hard-refresh

On the form tab in Chrome/Safari/Firefox: **Cmd+Shift+R** (or empty cache + reload). A normal refresh often keeps old CSS.

## Product vs theme CSS

| File | Role |
|------|------|
| `docker/tomcat/css/project/form-layout-core.css` | **All themes** — FIB geometry, content width contract, shared field-column rules |
| `docker/tomcat/css/project/dirtbowl2/project.css` | **DirtBowl brand only** — colors, fonts, beige/white chrome; sets size **variables** that layout-core reads |

Preview (`:5173`) uses `designer-web/server/themes/*.css` — restart or hard-refresh Designer Preview separately; it does not use Tomcat’s copy.
