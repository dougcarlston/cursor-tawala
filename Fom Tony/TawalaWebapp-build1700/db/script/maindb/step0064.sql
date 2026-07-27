begin transaction;
	ALTER TABLE user_file ALTER content_type TYPE character varying(100);

	update schema_version set version = 64;
commit;