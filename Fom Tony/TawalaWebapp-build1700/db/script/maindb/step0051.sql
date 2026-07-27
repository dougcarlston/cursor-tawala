begin transaction;
	ALTER TABLE email ADD COLUMN orig_size integer, ADD COLUMN compressed_size integer;

	CREATE INDEX ix_email_create_dt ON email (create_dt);

	update schema_version set version = 51;
commit;