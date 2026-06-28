begin transaction; 
	ALTER TABLE project ADD COLUMN properties text;
	update schema_version set version = 35;
commit;