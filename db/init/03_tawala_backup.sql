-- Separate backup database (legacy: tawala_backup).
-- Stores online project export blobs (user_project_backup).

\connect postgres

SELECT 'CREATE DATABASE tawala_backup OWNER tawala_admin'
WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'tawala_backup')\gexec

GRANT CONNECT ON DATABASE tawala_backup TO tawala_app;

\connect tawala_backup

CREATE EXTENSION IF NOT EXISTS lo;

CREATE SEQUENCE seq_user_project_backup_id;

CREATE TABLE user_project_backup (
    user_project_backup_id  BIGINT NOT NULL,
    user_id                 BIGINT NOT NULL,
    user_project_id         BIGINT NOT NULL,
    project_name            VARCHAR(100) NOT NULL,
    project_version_number  INTEGER NOT NULL,
    record_count            INTEGER NOT NULL,
    backup_size             INTEGER NOT NULL,
    create_dt               TIMESTAMPTZ NOT NULL,
    backup_data             OID,
    PRIMARY KEY (user_project_backup_id)
);

CREATE TABLE schema_version (
    version BIGINT NOT NULL
);

INSERT INTO schema_version (version) VALUES (3);

CREATE TRIGGER t_user_project_backup
    BEFORE DELETE ON user_project_backup
    FOR EACH ROW EXECUTE FUNCTION lo_manage(backup_data);

GRANT USAGE ON SCHEMA public TO tawala_app;
GRANT ALL ON ALL TABLES IN SCHEMA public TO tawala_app;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO tawala_app;
