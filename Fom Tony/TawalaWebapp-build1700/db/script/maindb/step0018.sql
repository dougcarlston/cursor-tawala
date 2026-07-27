begin transaction;  
    ALTER TABLE project DROP COLUMN random_id;
    ALTER TABLE project ADD COLUMN random_id char(20);
    UPDATE project SET random_id = project_id;
    ALTER TABLE project ALTER COLUMN random_id SET NOT NULL;
    ALTER TABLE project ADD CONSTRAINT project_random_id_key UNIQUE(random_id);

    update schema_version set version = 18;
commit;