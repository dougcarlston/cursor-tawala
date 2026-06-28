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
