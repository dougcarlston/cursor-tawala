-- Roles for local development (Docker entrypoint runs as POSTGRES_USER / tawala_admin).
-- Legacy production used tawala_admin (owner) + tawala_user (app). Here: tawala_app ≈ tawala_user.

DO $$
BEGIN
    IF NOT EXISTS (SELECT FROM pg_roles WHERE rolname = 'tawala_app') THEN
        CREATE ROLE tawala_app WITH LOGIN PASSWORD 'tawala';
    END IF;
END $$;

GRANT CONNECT ON DATABASE tawala TO tawala_app;
