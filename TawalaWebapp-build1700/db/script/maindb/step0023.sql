begin transaction; 
    ALTER TABLE domain ADD COLUMN display_name varchar(60);
    update domain set display_name = name;
    ALTER TABLE domain ALTER COLUMN display_name SET NOT NULL;

    update schema_version set version = 23;
commit;