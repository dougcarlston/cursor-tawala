# Comparing Registration: dev test bed (:5173) vs Java (:8080)

The Designer ships **two runtimes** for exercising forms. They use the **same JSON project**, but Registration rendering differed during the July 2026 parity pass. **DirtBowl Registration page 1** Preview/Deploy parity is **substantially complete / closed for now**, pending owner visual verification of the final Q4 email-note alignment fix.

## Side-by-side

| | **Dev test bed** | **Java server** |
|---|------------------|-----------------|
| **Purpose** | Pixel-polished Registration UI prototype | Production Tawala runtime (Tomcat + Postgres) |
| **Start Designer** | `cd designer-web && npm run dev:local` | `cd designer-web && npm run dev` (or `npm run dev:java`) |
| **Deploy target** | Dev runtime (`TAWALA_DEV_ONLY=1`) | Tomcat `:8080` when reachable |
| **Registration URL** | `http://localhost:5173/p/{uniqueId}/Registration` | `http://localhost:8080/p/{uniqueId}/Registration` |
| **Layout code** | `designer-web/server/registrationLayout.mjs`, `registrationReview.mjs`, `themes/dirtbowl2.css` | Legacy Java/XSL rendering from deployed XML |
| **Post-RegStep2 / email** | Partial (dev engine) | Full (Java + Postgres) |

After **File → Deploy**, the deploy dialog lists start-point URLs for the active runtime.

## Open the formatted test bed (5173)

1. Start Postgres + Tomcat if you need DB-backed flows later, but for **layout-only comparison** only the dev server is required:

   ```bash
   cd ~/Projects/AI-Tawala/designer-web
   npm run dev:local
   ```

   `dev:local` sets `TAWALA_DEV_ONLY=1` so deploy stays on `:5173` (does not forward to Java).

2. In the browser: **File → Open DirtBowl Sample…** (or your saved project).

3. **Deploy** (credentials `dev` / `dev`).

4. Open the Registration link from the deploy dialog, e.g.:

   `http://localhost:5173/p/4d24f014ff3b51a3/Registration`

   The dev project id is stored under `designer-web/.deployed/dev/_index.json` (gitignored; recreated on deploy).

## Open the Java server version (8080)

1. Ensure Docker is up:

   ```bash
   cd ~/Projects/AI-Tawala
   docker compose up -d
   ```

2. Deploy to Tomcat (Designer UI or CLI):

   ```bash
   node scripts/deploy-dirtbowl-java.mjs
   ```

   Or from Designer: `npm run dev` (without `dev:local`), then **File → Deploy**.

3. Use the Registration URL from deploy output / dialog, e.g.:

   `http://localhost:8080/p/1ucetrtc0an5ue0/Registration`

   Java URLs include a short hash prefix before the form name (from the deploy dialog), e.g.:

   `http://localhost:8080/p/1ucetrtc0an5ue0/nftivyp.Registration`

   Plain `/Registration` without the hash returns **Project Not Found**.

4. Seed dev data after deploy:

   ```bash
   ./scripts/dev-data.sh seed-admin
   ./scripts/dev-data.sh seed-divisions   # optional, for Q5/Q6 pickers
   ```

## Compare in two browser tabs

| Tab A — test bed | Tab B — Java |
|------------------|--------------|
| `npm run dev:local` | `docker compose up -d` + `npm run dev` + Deploy |
| `:5173/p/…/Registration` | `:8080/p/…/Registration` |

Walk the same path on both: page 1 → segment 1 (jersey/friends) → review → RegStep2 → payment instructions.

## Current findings (July 2026)

Preview-vs-Deploy notes were kept in three buckets; **page 1 style/theme parity is substantially complete** pending owner sign-off on the final Q4 CSS tweak.

| Bucket | Current conclusion |
|--------|--------------------|
| **Style / theme parity** | **Substantially complete / closed for now** — page 1 Deploy closely matches Preview after MCQ, phone, submit, and Q4 parent-contact passes. Uncommitted fix: Q4 email-only note `margin-left` in `docker/tomcat/css/project/dirtbowl2/project.css` (align note with field column, not label column + gap). |
| **Data / seed mismatch** | Keep separate from rendering bugs. Admin / division seed differences can change what Registration shows and can look like layout regressions. |
| **General authoring architecture** | **Documented backlog** — DirtBowl stress-testing surfaced five structural browser-Designer gaps (MDI, form–process links, collapsible explorer, properties popups, multiple menu bars). See [`docs/DESIGNER_BACKLOG_ARCHITECTURE.md`](DESIGNER_BACKLOG_ARCHITECTURE.md). |

### DirtBowl as stress test (not authoring benchmark)

DirtBowl served its purpose: **surfacing failures not visible in small templates**. Simple Survey, Sign-up Sheet, and Get Together passed Preview + Deploy earlier without exposing these issues. DirtBowl remains useful for **runtime regression checks** on Registration; it is **not** the right browser-Designer authoring benchmark until architecture backlog items land.

## DirtBowl Registration parity pass — page 1 (closed pending Q4 verify)

- Fixed: **Sex of Registrant** MCQ alignment.
- Fixed: **Parent Phone Numbers** overlap / crowding.
- Fixed: Submit button styling.
- Fixed: Q4 parent-contact block row spacing (`--reg-q4-row-gap` flex layout in `project.css`).
- **Pending owner verify:** Q4 email-only note field-column alignment (`margin-left: var(--reg-tabbed-label-width)` on bare `div` under Parent block).

### Designer architecture findings (from DirtBowl stress test)

Captured in [`docs/DESIGNER_BACKLOG_ARCHITECTURE.md`](DESIGNER_BACKLOG_ARCHITECTURE.md) with cross-refs to `Tawala_Key_Documents/DESIGNER_*.md`:

1. Multi-window / MDI architecture
2. Forms ↔ Processes connection transparency
3. Collapsible Explorer menus (Fields; Processes under Forms)
4. Properties panel vs legacy popup dialogs
5. Multiple context-sensitive menu bars

## What is intentionally different today

- **5173** uses custom HTML/CSS for Registration (name grid, DOB, friends block, green review table, jersey radio column, division help dialog).
- **8080** uses XML generated by `jsonToXml.mjs` and the legacy renderer — functionally complete for the full flow, but not yet visually matched to the test bed.
- Closing the terminal running `npm run dev` stops `:5173` forms; Docker keeps `:8080` running independently.

## Quick reference scripts

```bash
./scripts/dev-data.sh seed-admin
./scripts/dev-data.sh seed-divisions
./scripts/dev-data.sh cleanup-registrations
./scripts/dev-data.sh status
node scripts/deploy-dirtbowl-java.mjs
```
