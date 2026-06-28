begin transaction;
    ALTER TABLE lib_project ADD COLUMN vetted bool;
    update lib_project set vetted = false;
    ALTER TABLE lib_project ALTER COLUMN vetted SET NOT NULL;

    ALTER TABLE lib_project ADD COLUMN featured bool;
    update lib_project set featured = false;
    ALTER TABLE lib_project ALTER COLUMN featured SET NOT NULL;
    
    update schema_version set version = 13;

    select acl_admin.grant_on_visible('tawala_user', 'ALL');
commit;