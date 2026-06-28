-- Grant app role access to all objects created in the tawala database.

GRANT USAGE ON SCHEMA public TO tawala_app;
GRANT ALL ON ALL TABLES IN SCHEMA public TO tawala_app;
GRANT ALL ON ALL SEQUENCES IN SCHEMA public TO tawala_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON TABLES TO tawala_app;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT ALL ON SEQUENCES TO tawala_app;
