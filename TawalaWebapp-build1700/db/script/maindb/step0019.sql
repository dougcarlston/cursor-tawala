begin transaction;  
    ALTER TABLE lib_project ADD COLUMN snapshot_tile varchar(100);
	
    update schema_version set version = 19;
commit;