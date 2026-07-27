begin transaction; 
    ALTER TABLE users ADD COLUMN original_domain varchar(60);
    update schema_version set version = 24;
commit;