# Tawala database initialization

Consolidated PostgreSQL schema from legacy **build-1700** migrations (`schema_version` **68**).

## Quick start (Docker)

```bash
cd ~/Projects/AI-Tawala
docker compose up -d
docker compose exec postgres psql -U tawala_admin -d tawala -c "SELECT * FROM schema_version;"
```

Expected: `version = 68`.

### If init failed (`lo_manage() does not exist`)

The schema uses PostgreSQL large-object triggers; init scripts now run `CREATE EXTENSION lo` first. If you already had a failed first boot, reset the volume and re-init:

```bash
cd ~/Projects/AI-Tawala
docker compose down -v
docker compose up -d
docker compose exec postgres psql -U tawala_admin -d tawala -c "SELECT * FROM schema_version;"
```

Use service name **`postgres`**, not `psql`:

```bash
docker compose exec postgres psql -U tawala_admin -d tawala -c "SELECT * FROM schema_version;"
```

Verify backup database:

```bash
docker compose exec postgres psql -U tawala_admin -d tawala_backup -c "\dt"
```

## Init script order

| File | Purpose |
|------|---------|
| `00_roles.sql` | Creates `tawala_app` login role (≈ legacy `tawala_user`) |
| `01_tawala_schema.sql` | Main platform tables, sequences, indexes |
| `02_jbpm_schema.sql` | jBPM tables for Sports Dashboards provisioning (optional) |
| `03_tawala_backup.sql` | Separate `tawala_backup` database |
| `04_seed_dev_user.sql` | `dev` / `designer` users (password `dev`) for Java `/client` API |
| `99_grants.sql` | Grants on `tawala` public schema |

To skip jBPM, remove or rename `02_jbpm_schema.sql` before first `docker compose up`.

## DirtBowl dev data (local)

After deploying from Designer, seed league/fee/address variables:

```bash
./scripts/dev-data.sh seed-admin
./scripts/dev-data.sh seed-divisions   # Q5/Q6 division pickers on Registration
```

Remove test registrations (keeps AdminSetup):

```bash
./scripts/dev-data.sh cleanup-registrations
```

See also `docs/COMPARING_RUNTIMES.md` for **5173 test bed** vs **8080 Java** Registration URLs.

Check form row counts:

```bash
./scripts/dev-data.sh status
```

**Note:** Form data lives on `user_project.project_id` (shared storage), not the deployed version’s `project_id`. Re-run `seed-admin` after each Designer deploy if fee/address are blank.

## Publish to GitHub

From your Mac Terminal (so Git can prompt for login):

```bash
./scripts/publish-to-github.sh
```

Do **not** paste your GitHub password into Cursor chat. Use a [Personal Access Token](https://github.com/settings/tokens) when Git prompts for a password, or set up SSH keys.

## Connection strings

| Role | User | Password | Database |
|------|------|----------|----------|
| Owner (legacy admin) | `tawala_admin` | `tawala` | `tawala` |
| App (legacy user) | `tawala_app` | `tawala` | `tawala` |
| Backups | `tawala_app` | `tawala` | `tawala_backup` |

JDBC example:

```
jdbc:postgresql://localhost:5432/tawala?user=tawala_app&password=tawala
```

## Manual install (existing PostgreSQL)

```bash
createdb -U postgres tawala
psql -U postgres -d tawala -f db/init/00_roles.sql
psql -U postgres -d tawala -f db/init/01_tawala_schema.sql
psql -U postgres -d tawala -f db/init/02_jbpm_schema.sql   # optional
psql -U postgres -d tawala -f db/init/99_grants.sql
psql -U postgres -f db/init/03_tawala_backup.sql
```

## Design notes

- **TIMESTAMPTZ** used instead of legacy `timestamp`.
- **OID** retained on `email.body`, `user_file.image_data`, and `user_project_backup.backup_data` for unmodified Java Hibernate code.
- **Sequence names** unchanged (`seq_user_id`, `seq_project_id`, etc.).
- Legacy `acl_admin` grant helpers replaced by `99_grants.sql`.

## Source

Original incremental migrations: `TawalaWebapp-build1700/db/script/maindb/step0001.sql` … `step0068.sql`.
