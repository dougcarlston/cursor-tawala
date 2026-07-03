# Tawala stack — local development

Two paths: **dev runtime** (works today) and **Java backend** (full fidelity).

## Path A — Dev runtime (Designer + Node)

Already working on your machine:

```bash
cd ~/Projects/AI-Tawala/designer-web
npm run dev
```

| Service | URL |
|---------|-----|
| Designer UI | http://localhost:5173 |
| Dev API | http://localhost:3001 |
| Deployed forms | http://localhost:5173/p/{id}/{FormName} |

Credentials: `dev` / `dev`

### Dev runtime v2 (incremental)

The Node runtime at `designer-web/server/` now includes:

- **Pre-process** — runs `RetrieveAdminSetupVariables` etc. (`set`, `get`, `foreach`)
- **Skip instructions** — `if` / `set` / `skip` / `__EndOfForm__`
- **Field references** — `<<League>>`, `Registration:Field`, rich-text `field` nodes
- **Dynamic MCQ** — choices from `Divisions` / `AdminSetup` seed records (DirtBowl)
- **Session state** — field values persist across submit/skip

Still **not** in dev runtime: full post-process chains, payments, email, multi-form navigation, file uploads.

---

## Path B — PostgreSQL (ready)

```bash
cd ~/Projects/AI-Tawala
docker compose up -d
docker compose exec postgres psql -U tawala_admin -d tawala -c "SELECT * FROM schema_version;"
```

Expected: `version = 68`.

Seed users for Java (when Tomcat is running):

```bash
docker compose exec postgres psql -U tawala_admin -d tawala -c "SELECT user_name FROM users;"
```

Should include `dev` and `designer` (password `dev`) from `db/init/04_seed_dev_user.sql`.

Connection:

```
jdbc:postgresql://localhost:5432/tawala?user=tawala_app&password=tawala
```

---

## Path C — Java webapp (build-1700)

### Quick start (Docker — recommended)

Postgres and Tomcat run together; no manual Tomcat install needed.

```bash
cd ~/Projects/AI-Tawala
./scripts/docker-up.sh
```

That script builds `ROOT.war` if it is missing, then runs `docker compose up -d --build`. When ready:

| Service | URL |
|---------|-----|
| Java backend | http://localhost:8080 |
| Postgres | `localhost:5432` (db `tawala`, user `tawala_app` / `tawala`) |

Useful commands:

```bash
docker compose logs -f tawala    # Tomcat logs
docker compose ps
docker compose down              # stop (data kept in volumes)
```

Point the Designer at Java:

```bash
export TAWALA_JAVA_URL=http://localhost:8080
cd designer-web && npm run dev
```

The Docker image patches `ROOT.war` for container networking (`postgres` hostname, in-process Lucene search). It also injects **project theme CSS** and **`/images/silk/`** icons (FamFamFam tick/star for correlation tables) from `docker/tomcat/`. See `docker/tomcat/README.md`. Rebuild after changing those assets:

```bash
docker compose build tawala && docker compose up -d tawala
```

Local `ant` builds still use `localhost` via `src/dbconnection.properties`.

### Manual build (without Docker for Tomcat)

### What we have

| Asset | Status |
|-------|--------|
| Java sources | `TawalaWebapp-build1700/src/` (~655 files) |
| JSP / static web | `TawalaWebapp-build1700/web/` |
| Spring / servlet config | `TawalaWebapp-build1700/web/WEB-INF/` (from `NEWZIPS/web.zip`) |
| Dependency JARs | `TawalaWebapp-build1700/lib/` |
| Ant **compile + war** | `TawalaWebapp-build1700/build.xml` (from `NEWZIPS/Tawala.zip`) |
| Local JDBC overrides | `TawalaWebapp-build1700/src/dbconnection.properties` |
| DB schema | `db/init/` consolidated from migrations |
| Ant **deploy-only** scripts | `backup/tawala-dev/home/tawala/bin/` (expect a pre-built WAR) |

### ROOT.war is not in the archive — rebuild it

Your administrator is correct: **`ROOT.war` was produced on `build.tawala.com` for each build**, then copied to dev/live (`push_build_to_dev.mk` scp’d from `tomcat-build/webapps/ROOT.war`). It was never something the team kept long-term.

There is **no** `ROOT.war` anywhere in `NEWZIPS/` or `backup/`. The pieces to **rebuild** it are present but were split across zips:

| Zip | Contents |
|-----|----------|
| `282a64f2d_TawalaWebapp-source.zip` | `src/`, `web/` (JSP/static), `lib/` |
| `web.zip` | `WEB-INF/web.xml`, Spring XML, tiles, email templates |
| `Tawala.zip` | `build.xml`, Eclipse project files |
| `test-config.zip` | test/runtime property files |

### Build ROOT.war locally

**Prerequisites:** JDK 8 (Temurin 8 recommended), Apache Ant, Postgres via Docker.

```bash
cd ~/Projects/AI-Tawala
docker compose up -d

cd TawalaWebapp-build1700
# brew install --cask temurin@8 && brew install ant   # if needed
./scripts/build-root-war.sh
```

This runs `ant war-without-videos` (skips the long unit-test suite) and copies `tawala.war.dev` → `../ROOT.war`.

JDBC points at Docker Postgres via `src/dbconnection.properties` (`tawala_app` / `tawala` on `localhost:5432`).

### Run Tomcat

1. Start Postgres: `docker compose up -d`
2. Install Tomcat **7** (Spring 2.x / Hibernate 3 / Servlet 2.4 era)
3. Deploy `ROOT.war` as Tomcat’s root context (`webapps/ROOT.war`)
4. Point Designer dev API at Java:

```bash
export TAWALA_JAVA_URL=http://localhost:8080
export TAWALA_DEV_HOST=http://localhost:8080
cd designer-web && npm run dev:api
```

Designer **Deploy** will JSON→XML and POST to `http://localhost:8080/client`.

### API contract (unchanged since 2008)

```xml
<request type="uploadProject" protocol="1.0">
  <credentials user="dev" password="dev"/>
  <project name="..." ...>...</project>
</request>
```

POST to `/client` — see `ClientApiController.java`.

---

## Suggested order

1. **Now** — test Registration with dev runtime v2 (re-deploy DirtBowl after pulling changes)
2. **When you have WAR or build env** — wire Java + Postgres
3. **Your issue list** — prioritize gaps between dev runtime and Java behavior
