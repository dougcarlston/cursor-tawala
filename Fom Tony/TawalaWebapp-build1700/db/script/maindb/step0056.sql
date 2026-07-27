begin transaction;
	ALTER TABLE lib_project_user_downloaded ALTER user_id_and_version TYPE character varying(60);

	update schema_version set version = 56;
commit;