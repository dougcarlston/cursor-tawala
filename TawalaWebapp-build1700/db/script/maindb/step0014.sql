begin transaction;  
    delete from lib_project_version where project_id in (select project_id from project where project_definition not like '<%');
    delete from project where project_definition not like '<%';

    ALTER TABLE project ADD COLUMN random_id char(40);
    update project set random_id = project_id;
    ALTER TABLE project ALTER COLUMN random_id SET NOT NULL;

    ALTER TABLE project ADD CONSTRAINT project_random_id_key UNIQUE(random_id);
	
    update schema_version set version = 14;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;