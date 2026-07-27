begin transaction; 
	ALTER TABLE users ALTER email TYPE character varying(100);
	update schema_version set version = 34;
commit;