begin transaction; 
    ALTER TABLE user_project ADD COLUMN original_lib_project_version_id int8;
    update schema_version set version = 25;
commit;